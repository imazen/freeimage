// ==========================================================
// TARGA Loader and Writer
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Jani Kajala (janik@remedy.fi)
// - Martin Weber (martweb@gmx.net)
// - Machiel ten Brinke (brinkem@uni-one.nl)
// - Peter Lemmens (peter.lemmens@planetinternet.be)
// - Hervé Drolon (drolon@infonie.fr)
// - Mihail Naydenov (mnaydenov@users.sourceforge.net)
//
// This file is part of FreeImage 3
//
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS, WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, WITHOUT LIMITATION, WARRANTIES
// THAT THE COVERED CODE IS FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE
// OR NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE COVERED
// CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE DEFECTIVE IN ANY RESPECT, YOU (NOT
// THE INITIAL DEVELOPER OR ANY OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY
// SERVICING, REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN ESSENTIAL
// PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS AUTHORIZED HEREUNDER EXCEPT UNDER
// THIS DISCLAIMER.
//
// Use at your own risk!
// ==========================================================

#include "FreeImage.h"
#include "Utilities.h"

// ----------------------------------------------------------
//   Constants + headers
// ----------------------------------------------------------

#ifdef _WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif

typedef struct tagTGAHEADER {
	BYTE id_length;				// ID length
	BYTE color_map_type;		// color map type
	BYTE image_type;			// image type

	WORD cm_first_entry;		// first entry index
	WORD cm_length;				// color map length
	BYTE cm_size;				// color map entry size, in bits

	WORD is_xorigin;			// X-origin of image
	WORD is_yorigin;			// Y-origin of image
	WORD is_width;				// image width
	WORD is_height;				// image height
	BYTE is_pixel_depth;		// pixel depth
	BYTE is_image_descriptor;	// image descriptor
} TGAHEADER;

typedef struct tagTGAFOOTER {
	DWORD extension_offset;	// extension area offset
	DWORD developer_offset;	// developer directory offset
	char signature[18];		// signature string
} TGAFOOTER;

#ifdef _WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif

static const char *FI_MSG_ERROR_CORRUPTED = "Image data corrupted";

// ----------------------------------------------------------
// Image type
//
#define TGA_NULL		0	// no image data included
#define TGA_CMAP		1	// uncompressed, color-mapped image
#define TGA_RGB			2	// uncompressed, true-color image
#define TGA_MONO		3	// uncompressed, black-and-white image
#define TGA_RLECMAP		9	// run-length encoded, color-mapped image
#define TGA_RLERGB		10	// run-length encoded, true-color image
#define TGA_RLEMONO		11	// run-length encoded, black-and-white image
#define TGA_CMPCMAP		32	// compressed (Huffman/Delta/RLE) color-mapped image (e.g., VDA/D) - Obsolete
#define TGA_CMPCMAP4	33	// compressed (Huffman/Delta/RLE) color-mapped four pass image (e.g., VDA/D) - Obsolete


// ==========================================================
// Internal functions
// ==========================================================

/** This class is used when loading RLE compressed images, it implements io cache of fixed size.
	In general RLE compressed images *should* be compressed line by line with line sizes stored in Scan Line Table section.
	In reality, however there are images not obeying the specification compressing image data continuously across lines,
	making it impossible to load the file cached at every line.
*/
class IOCache
{
public:
	IOCache(FreeImageIO *io, fi_handle handle, size_t size) :
		_ptr(NULL), _begin(NULL), _end(NULL), _size(size), _io(io), _handle(handle)	{
			_begin = (BYTE*)malloc(size);
			if (_begin) {
			_end = _begin + _size;
			_ptr = _end;	// will force refill on first access
		}
	}
	
	~IOCache() {
		if (_begin != NULL) {
			free(_begin);
		}	
	}
		
	BOOL isNull() { return _begin == NULL;}
	
	BYTE getByte() {
		if (_ptr >= _end) {
			// need refill
			_ptr = _begin;
			_io->read_proc(_ptr, sizeof(BYTE), (unsigned)_size, _handle);	//### EOF - no problem?
		}

		BYTE result = *_ptr;

		_ptr++;

		return result;
	}
	
	BYTE* getBytes(size_t count /*must be < _size!*/) {
		if (_ptr + count >= _end) {
			
			// need refill

			// 'count' bytes might span two cache bounds,
			// SEEK back to add the remains of the current cache again into the new one

			long read = long(_ptr - _begin);
			long remaining = long(_size - read);

			_io->seek_proc(_handle, -remaining, SEEK_CUR);

			_ptr = _begin;
			_io->read_proc(_ptr, sizeof(BYTE), (unsigned)_size, _handle);	//### EOF - no problem?
		}

		BYTE *result = _ptr;

		_ptr += count;

		return result;
	}
private:
	IOCache& operator=(const IOCache& src);

private:
	BYTE *_ptr;
	BYTE *_begin;
	BYTE *_end;
	const size_t _size;
	const FreeImageIO *_io;	
	const fi_handle _handle;	
};

#ifdef FREEIMAGE_BIGENDIAN
static void
SwapHeader(TGAHEADER *header) {
	SwapShort(&header->cm_first_entry);
	SwapShort(&header->cm_length);
	SwapShort(&header->is_xorigin);
	SwapShort(&header->is_yorigin);
	SwapShort(&header->is_width);
	SwapShort(&header->is_height);
}

static void
SwapFooter(TGAFOOTER *footer) {
	SwapLong(&footer->extension_offset);
	SwapLong(&footer->developer_offset);
}

#endif // FREEIMAGE_BIGENDIAN

// ==========================================================
// Plugin Interface
// ==========================================================

static int s_format_id;

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "TARGA";
}

static const char * DLL_CALLCONV
Description() {
	return "Truevision Targa";
}

static const char * DLL_CALLCONV
Extension() {
	return "tga,targa";
}

static const char * DLL_CALLCONV
RegExpr() {
	return NULL;
}

static const char * DLL_CALLCONV
MimeType() {
	return "image/freeimage-tga";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {
	// tga_signature = "TRUEVISION-XFILE."
	BYTE tga_signature[18] = { 84, 82, 85, 69, 86, 73, 83, 73, 79, 78, 45, 88, 70, 73, 76, 69, 46, 0 };
	// get the start offset
	long start_offset = io->tell_proc(handle);
	// get the end-of-file
	io->seek_proc(handle, 0, SEEK_END);
	long eof = io->tell_proc(handle);
	// read the signature

	if (io->seek_proc(handle, start_offset + eof - 18, SEEK_SET) == 0) {
		BYTE signature[18];
		io->read_proc(&signature, 1, 18, handle);
		return (memcmp(tga_signature, signature, 18) == 0);
	}

	return FALSE;
}

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return (
		(depth == 8) ||
		(depth == 16) ||
		(depth == 24) ||
		(depth == 32)
		);
}

static BOOL DLL_CALLCONV
SupportsExportType(FREE_IMAGE_TYPE type) {
	return (type == FIT_BITMAP) ? TRUE : FALSE;
}

static BOOL DLL_CALLCONV
SupportsNoPixels() {
	return TRUE;
}

// ----------------------------------------------------------
/**
Used for all 32 and 24 bit loading of uncompressed images
*/
static
void loadTrueColor(FIBITMAP* dib, int width, int height, int file_pixel_size, FreeImageIO* io, fi_handle handle, BOOL as24bit) {
	const int pixel_size = as24bit ? 3 : file_pixel_size;

	// input line cache
	BYTE* file_line = (BYTE*)malloc( width * file_pixel_size);

	if (!file_line)
		throw FI_MSG_ERROR_MEMORY;

	for (int y = 0; y < height; ++y) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);
		io->read_proc(file_line, file_pixel_size, width, handle);
		BYTE *bgra = file_line;

		for (int x = 0; x < width; ++x) {

			bits[FI_RGBA_BLUE]	= bgra[0];
			bits[FI_RGBA_GREEN] = bgra[1];
			bits[FI_RGBA_RED]	= bgra[2];

			if (!as24bit) {
				bits[FI_RGBA_ALPHA] = bgra[3];
			}

			bgra += file_pixel_size;

			bits += pixel_size;
		}
	}

	free(file_line);
}

/**
For the generic RLE loader we need to abstract away the pixel format.
We use a specific overload based on bits-per-pixel for each type of pixel
*/

template <int nBITS>
inline static
void assignPixel(BYTE* bits, BYTE* val, BOOL as24bit = FALSE) {
	// static assert should go here
	assert(FALSE);
}

template <>
inline
void assignPixel<8>(BYTE* bits, BYTE* val, BOOL as24bit) {
	*bits = *val;
}

template <>
inline
void assignPixel<16>(BYTE* bits, BYTE* val, BOOL as24bit) {
	WORD value(*reinterpret_cast<WORD*>(val));

#ifdef FREEIMAGE_BIGENDIAN
	SwapShort(value);
#endif

	if (as24bit) {
		bits[FI_RGBA_BLUE]  = (BYTE)((((value & FI16_555_BLUE_MASK) >> FI16_555_BLUE_SHIFT) * 0xFF) / 0x1F);
		bits[FI_RGBA_GREEN] = (BYTE)((((value & FI16_555_GREEN_MASK) >> FI16_555_GREEN_SHIFT) * 0xFF) / 0x1F);
		bits[FI_RGBA_RED]   = (BYTE)((((value & FI16_555_RED_MASK) >> FI16_555_RED_SHIFT) * 0xFF) / 0x1F);

	} else {
		*reinterpret_cast<WORD *>(bits) = 0x7FFF & value;
	}
}

template <>
inline
void assignPixel<24>(BYTE* bits, BYTE* val, BOOL as24bit) {
	bits[FI_RGBA_BLUE]	= val[0];
	bits[FI_RGBA_GREEN] = val[1];
	bits[FI_RGBA_RED]	= val[2];
}

template <>
inline
void assignPixel<32>(BYTE* bits, BYTE* val, BOOL as24bit) {
	if (as24bit) {
		assignPixel<24>(bits, val, TRUE);

	} else {
#if FREEIMAGE_COLORORDER == FREEIMAGE_COLORORDER_BGR
		*(reinterpret_cast<DWORD*>(bits)) = *(reinterpret_cast<DWORD*> (val));
#else // NOTE This is faster then doing reinterpret_cast to int + INPLACESWAP !
		bits[FI_RGBA_BLUE]	= val[0];
		bits[FI_RGBA_GREEN] = val[1];
		bits[FI_RGBA_RED]	= val[2];
		bits[FI_RGBA_ALPHA]	= val[3];
#endif
	}
}

/**
Generic RLE loader
*/
template<int bPP>
static
void loadRLE(FIBITMAP* dib, int width, int height, FreeImageIO* io, fi_handle handle, long eof, BOOL as24bit) {
	const int file_pixel_size = bPP/8;
	const int pixel_size = as24bit ? 3 : file_pixel_size;

	const BYTE bpp = as24bit ? 24 : bPP;
	const int line_size = CalculateLine(width, bpp);

	// Note, many of the params can be computed inside the function.
	// However, because this is a template function, it will lead to redundant code duplication.

	BYTE rle;
	BYTE *line_bits;

	// this is used to guard against writing beyond the end of the image (on corrupted rle block)
	const BYTE* dib_end = FreeImage_GetScanLine(dib, height);//< one-past-end row

	// Compute the rough size of a line...
	long pixels_offset = io->tell_proc(handle);
	long sz = ((eof - pixels_offset) / height);

	// ...and allocate cache of this size (yields good results)
	IOCache cache(io, handle, sz);
	if(cache.isNull())
		return;
		
	int x = 0, y = 0;

	line_bits = FreeImage_GetScanLine(dib, y);

	while (y < height) {

		rle = cache.getByte();

		BOOL has_rle = rle & 0x80;
		rle &= ~0x80; // remove type-bit

		BYTE packet_count = rle + 1;

		// packet_count might be corrupt, test if we are not about to write beyond the last image bit

		if ((line_bits+x) + packet_count*pixel_size > dib_end) {
			FreeImage_OutputMessageProc(s_format_id, FI_MSG_ERROR_CORRUPTED);
			return;
		}

		if (has_rle) {

			// read a pixel value from file...
			BYTE *val = cache.getBytes(file_pixel_size);

			//...and fill packet_count pixels with it

			for (int ix = 0; ix < packet_count; ix++) {
				assignPixel<bPP>((line_bits+x), val, as24bit);
				x += pixel_size;

				if (x >= line_size) {
					x = 0;
					y++;
					line_bits = FreeImage_GetScanLine(dib, y);
				}
			}

		} else {
			// no rle commpresion

			// copy packet_count pixels from file to dib
			for (int ix = 0; ix < packet_count; ix++) {
				BYTE *val = cache.getBytes(file_pixel_size);
				assignPixel<bPP>((line_bits+x), val, as24bit);
				x += pixel_size;

				if (x >= line_size) {
					x = 0;
					y++;
					line_bits = FreeImage_GetScanLine(dib, y);
				}
			} //< packet_count
		} //< has_rle

	} //< while height

}

// ------------
static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	FIBITMAP *dib = NULL;

	if (!handle) {
		return NULL;
	}

	try {

		const BOOL header_only =  (flags & FIF_LOAD_NOPIXELS) == FIF_LOAD_NOPIXELS;
		
		// remember the start offset
		long start_offset = io->tell_proc(handle);

		// remember end-of-file (used for RLE cache)
		io->seek_proc(handle, 0, SEEK_END);
		long eof = io->tell_proc(handle);
		io->seek_proc(handle, start_offset, SEEK_SET);

		// read and process the bitmap's header

		TGAHEADER header;

		io->read_proc(&header, sizeof(tagTGAHEADER), 1, handle);

#ifdef FREEIMAGE_BIGENDIAN
		SwapHeader(&header);
#endif

		const int line = CalculateLine(header.is_width, header.is_pixel_depth);
		const int pixel_bits = header.is_pixel_depth;
		const int pixel_size = pixel_bits/8;

		// It seems this is ALWAYS: 8 for 32bit, 1 for 16bit, 0 for 24bit
		// int alphabits = header.is_image_descriptor & 0x0f;

		int fliphoriz = (header.is_image_descriptor & 0x10) ? 1 : 0;
		int flipvert = (header.is_image_descriptor & 0x20) ? 1 : 0;

		// skip comment
		io->seek_proc(handle, header.id_length, SEEK_CUR);

		switch (header.is_pixel_depth) {
			case 8 : {
				dib = FreeImage_AllocateHeader(header_only, header.is_width, header.is_height, 8);

				if (dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}
					
				// read the palette (even if header only)

				RGBQUAD *palette = FreeImage_GetPalette(dib);

				if (header.color_map_type == 0) {
					// no color-map data is included with this image ...
					// build a greyscale palette
					for (unsigned i = 0; i < 256; i++) {
						palette[i].rgbRed	= (BYTE)i;
						palette[i].rgbGreen = (BYTE)i;
						palette[i].rgbBlue	= (BYTE)i;
					}

				} else {
					unsigned count, csize;

					// calculate the color map size
					csize = header.cm_length * header.cm_size / 8;
					BYTE *cmap = (BYTE*)malloc(csize * sizeof(BYTE));

					io->read_proc(cmap, sizeof(BYTE), csize, handle);

					// build the palette

					switch (header.cm_size) {
						case 16: {
							WORD *rgb555 = (WORD*)&cmap[0];

							for (count = header.cm_first_entry; count < header.cm_length; count++) {
								palette[count].rgbRed   = (BYTE)((((*rgb555 & FI16_555_RED_MASK) >> FI16_555_RED_SHIFT) * 0xFF) / 0x1F);
								palette[count].rgbGreen = (BYTE)((((*rgb555 & FI16_555_GREEN_MASK) >> FI16_555_GREEN_SHIFT) * 0xFF) / 0x1F);
								palette[count].rgbBlue  = (BYTE)((((*rgb555 & FI16_555_BLUE_MASK) >> FI16_555_BLUE_SHIFT) * 0xFF) / 0x1F);
								rgb555++;
							}
						}
						break;

						case 24: {
							FILE_BGR *bgr = (FILE_BGR*)&cmap[0];

							for (count = header.cm_first_entry; count < header.cm_length; count++) {
								palette[count].rgbBlue  = bgr->b;
								palette[count].rgbGreen = bgr->g;
								palette[count].rgbRed   = bgr->r;
								bgr++;
							}
						}
						break;

						case 32: {
							BYTE trns[256];

							// clear the transparency table
							memset(trns, 0xFF, 256);

							FILE_BGRA *bgra = (FILE_BGRA*)&cmap[0];

							for (count = header.cm_first_entry; count < header.cm_length; count++) {
								palette[count].rgbBlue  = bgra->b;
								palette[count].rgbGreen = bgra->g;
								palette[count].rgbRed   = bgra->r;
								// alpha
								trns[count] = bgra->a;
								bgra++;
							}

							// set the tranparency table
							FreeImage_SetTransparencyTable(dib, trns, 256);
						}
						break;

					} // switch(header.cm_size)

					free(cmap);
				}

				if(header_only) {
					return dib;
				}
					
				// read in the bitmap bits

				switch (header.image_type) {
					
					case TGA_CMAP:
					case TGA_MONO: {
						BYTE *bits = NULL;
						
						for (unsigned count = 0; count < header.is_height; count++) {
							bits = FreeImage_GetScanLine(dib, count);
							io->read_proc(bits, sizeof(BYTE), line, handle);
						}
					}
					break;
					
					case TGA_RLECMAP:
					case TGA_RLEMONO: { //(8 bit)
						loadRLE<8>(dib, header.is_width, header.is_height, io, handle, eof, FALSE);
					}
					break;
					
					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 15 :
			case 16 : {
				int pixel_bits = 16;

				// allocate the dib

				if ((flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888) {
					pixel_bits = 24;
					dib = FreeImage_AllocateHeader(header_only, header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);

				} else {
					dib = FreeImage_AllocateHeader(header_only, header.is_width, header.is_height, pixel_bits, FI16_555_RED_MASK, FI16_555_GREEN_MASK, FI16_555_BLUE_MASK);
				}

				if (dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}
				else if(header_only) {
					return dib;
				}
					
				int line = CalculateLine(header.is_width, pixel_bits);

				const unsigned pixel_size = unsigned(pixel_bits) / 8;
				const unsigned src_pixel_size = sizeof(WORD);

				// note header.cm_size is a misleading name, it should be seen as header.cm_bits
				// ignore current position in file and set filepointer explicitly from the beginning of the file

				int garblen = 0;

				if (header.color_map_type != 0) {
					garblen = (int)((header.cm_size + 7) / 8) * header.cm_length; /* should byte align */
				} else {
					garblen = 0;
				}

				io->seek_proc(handle, start_offset, SEEK_SET);
				io->seek_proc(handle, sizeof(tagTGAHEADER) + header.id_length + garblen, SEEK_SET);

				// read in the bitmap bits

				switch (header.image_type) {

					case TGA_RGB: { //(16 bit)
						// input line cache
						BYTE *in_line = (BYTE*)malloc(header.is_width * sizeof(WORD));

						if (!in_line)
							throw FI_MSG_ERROR_MEMORY;

						const int h = header.is_height;

						for (int y = 0; y < h; y++) {
							
							BYTE *bits = FreeImage_GetScanLine(dib, y);
							io->read_proc(in_line, src_pixel_size, header.is_width, handle);
							
							BYTE *val = in_line;
							for (int x = 0; x < line; x += pixel_size) {

								assignPixel<16>(bits+x, val, (flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888);

								val += src_pixel_size;
							}
						}

						free(in_line);
					}
					break;

					case TGA_RLERGB: { //(16 bit)
						loadRLE<16>(dib, header.is_width, header.is_height, io, handle, eof, (flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 24 : {

				dib = FreeImage_AllocateHeader(header_only, header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);

				if (dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}
				else if(header_only) {
					return dib;
				}
					
				// read in the bitmap bits

				switch (header.image_type) {

					case TGA_RGB: { //(24 bit)
						//uncompressed
						loadTrueColor(dib, header.is_width, header.is_height, pixel_size,io, handle, TRUE);
					}
					break;

					case TGA_RLERGB: { //(24 bit)
						loadRLE<24>(dib, header.is_width, header.is_height, io, handle, eof, TRUE);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 32 : {
				int pixel_bits = 32;

				if ((flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888) {
					pixel_bits = 24;
				}

				// Allocate the DIB
				dib = FreeImage_AllocateHeader(header_only, header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);

				if (dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}
				else if(header_only) {
					return dib;
				}
					
				// read in the bitmap bits

				switch (header.image_type) {

					case TGA_RGB: { //(32 bit)
						// uncompressed
						loadTrueColor(dib, header.is_width, header.is_height, 4 /*file_pixel_size*/, io, handle, (flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888);
					}

					break;

					case TGA_RLERGB: { //(32 bit)
						loadRLE<32>(dib, header.is_width, header.is_height, io, handle, eof, (flags & TARGA_LOAD_RGB888) ==  TARGA_LOAD_RGB888);
					}

					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

		} // switch(header.is_pixel_depth)

		if (flipvert) {
			FreeImage_FlipVertical(dib);
		}

		if (fliphoriz) {
			FreeImage_FlipHorizontal(dib);
		}

		return dib;

	} catch (const char *message) {
		if (dib) {
			FreeImage_Unload(dib);
		}

		FreeImage_OutputMessageProc(s_format_id, message);

		return NULL;
	}
}


static inline
void flushPacket(BYTE*& dest, unsigned pixel_size, BYTE* packet_begin, BYTE*& packet, BYTE& packet_count, BOOL& has_rle) {
	if (packet_count) {
		const BYTE type_bit = has_rle ? 0x80 : 0x0;
		const BYTE write_count = has_rle ? 1 : packet_count;

		//build packet header: zero based count + type bit
		assert(packet_count >= 1);
		BYTE rle = packet_count - 1;
		rle |= type_bit;

		//write packet header
		*dest = rle;
		++dest;

		//write packet data
		memcpy(dest, packet_begin, write_count * pixel_size);
		dest += write_count * pixel_size;

		//reset state
		packet_count = 0;
		packet = packet_begin;
		has_rle = FALSE;

	}
}

static inline
void assignPixel(BYTE* dest, BYTE* src, unsigned pixel_size) {
	// fast generic assign (faster than for loop) and no code bloat (template)

	switch (pixel_size) {
		case 1:
			*dest = *src;
			break;

		case 2: {
			WORD val(*(WORD*)src);
	#ifdef FREEIMAGE_BIGENDIAN
			SwapShort(&val);
	#endif
			*(WORD*)dest = val;
		}
		break;

		case 3: {
			// Write 24bit as 16+8 - it does make a speed difference
			WORD val(*(WORD*)src);
	#ifdef FREEIMAGE_BIGENDIAN //Not sure if this is needed :(
			SwapShort(&val);
	#endif
			*(WORD*)dest = val;
			dest[2] = src[2];
		}
		break;

		case 4:
			*(DWORD*)dest = *(DWORD*)src;
			break;

		default:
			assert(FALSE);
	}
}

static inline
BOOL isEqualPixel(BYTE* lhs, BYTE* rhs, unsigned pixel_size) {
	switch (pixel_size) {
		case 1:
			return (*lhs == *rhs);

		case 2:
			return ( *(WORD*)lhs == *(WORD*)rhs );

		case 3:
			return ( *(WORD*)lhs == *(WORD*)rhs ) && (lhs[2] == rhs[2]);

		case 4:
			return ( *(unsigned*)lhs == *(unsigned*)rhs );

		default:
			assert(FALSE);
			return FALSE;
	}
}

static
void saveRLE(FIBITMAP* dib, unsigned width, unsigned height, unsigned pixel_size, FreeImageIO* io, fi_handle handle) {
	const unsigned line_size = FreeImage_GetLine(dib);

	const BYTE max_packet_size = 128;
	BYTE packet_count = 0;
	BOOL has_rle = FALSE;

	// packet (compressed or not) to be written to line

	BYTE* const packet_begin = (BYTE*)malloc(max_packet_size * pixel_size);
	BYTE* packet = packet_begin;

	// line to be written to disk
	// Note: we need some extra bytes for anti-commpressed lines. The worst case is:
	// 8 bit images were every 3th pixel is different.
	// Rle packet becomes two pixels, but nothing is compressed: two byte pixels are transformed into byte header and byte pixel value
	// After every rle packet there is a non-rle packet of one pixel: an extra byte for the header will be added for it
	// In the end we gain no bytes from compression, but also must insert a byte at every 3th pixel

	// add extra space for anti-commpressed lines
	size_t extra_space = (size_t)ceil(width / 3.0);
	BYTE* const line_begin = (BYTE*)malloc(width * pixel_size + extra_space);
	BYTE* line = line_begin;

	BYTE *current = (BYTE*)malloc(pixel_size);
	BYTE *next    = (BYTE*)malloc(pixel_size);

	for(unsigned y = 0; y < height; y++) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);

		// rewind line pointer
		line = line_begin;

		for(unsigned x = 0; x < line_size; x += pixel_size) {

			assignPixel(current, (bits + x), pixel_size);

			// read next pixel from dib

			if((x + pixel_size) < line_size) {
				assignPixel(next, (bits + x + pixel_size), pixel_size);

			} else {
				// last pixel in line

				// include current pixel and flush
				if(!has_rle) {
					// write to packet
					assignPixel(packet, current, pixel_size);
					packet += pixel_size;

				}

				assert(packet_count < max_packet_size);

				++packet_count;

				flushPacket(line, pixel_size, packet_begin, packet, packet_count, has_rle);

				// start anew on next line
				break;
			}

			if(isEqualPixel(current, next, pixel_size)) {

				// has rle

				if(!has_rle) {
					// flush non rle packet

					flushPacket(line, pixel_size, packet_begin, packet, packet_count, has_rle);

					// start a rle packet

					has_rle = TRUE;

					// write to packet
					assignPixel(packet, current, pixel_size);
					packet += pixel_size;
				}

				// otherwise do nothing. We will just increase the count at the end

			} else {

				// no rle

				if(has_rle) {
					// flush rle packet

					// include current pixel first
					assert(packet_count < max_packet_size);
					++packet_count;

					flushPacket(line, pixel_size, packet_begin, packet, packet_count, has_rle);

					// start anew on the next pixel
					continue;

				} else {

					// write to packet
					assignPixel(packet, current, pixel_size);
					packet += pixel_size;
				}

			}

			// increase counter on every pixel

			++packet_count;

			if(packet_count == max_packet_size) {
				flushPacket(line, pixel_size, packet_begin, packet, packet_count, has_rle);
			}

		}//for width

		// write line to disk
		io->write_proc(line_begin, 1, (unsigned)(line - line_begin), handle);

	}//for height

	free(line_begin);
	free(packet_begin);
	free(current);
	free(next);
}

static BOOL DLL_CALLCONV
Save(FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	if ((dib == NULL) || (handle == NULL))
		return FALSE;

	RGBQUAD *palette = FreeImage_GetPalette(dib);
	const unsigned bpp = FreeImage_GetBPP(dib);
	const unsigned pixel_size = bpp/8;

	// write the file header

	TGAHEADER header;

	header.id_length = 0;
	header.cm_first_entry = 0;
	header.is_xorigin = 0;
	header.is_yorigin = 0;
	header.is_width = (WORD)FreeImage_GetWidth(dib);
	header.is_height = (WORD)FreeImage_GetHeight(dib);
	header.is_pixel_depth = (BYTE)bpp;
	header.is_image_descriptor = 0;

	if (palette) {
		header.color_map_type = 1;
		header.image_type = ((flags & TARGA_SAVE_RLE) == TARGA_SAVE_RLE) ? TGA_RLECMAP : TGA_CMAP;
		header.cm_length = (WORD)(1 << bpp);

		if (FreeImage_IsTransparent(dib)) {
			header.cm_size = 32;

		} else {
			header.cm_size = 24;
		}

	} else {
		header.color_map_type = 0;
		header.image_type = ((flags & TARGA_SAVE_RLE) == TARGA_SAVE_RLE) ? TGA_RLERGB : TGA_RGB;
		header.cm_length = 0;
		header.cm_size = 0;
	}

	// write the header

#ifdef FREEIMAGE_BIGENDIAN
	SwapHeader(&header);
#endif

	io->write_proc(&header, sizeof(header), 1, handle);

#ifdef FREEIMAGE_BIGENDIAN
	SwapHeader(&header);
#endif

	// write the palette

	if (palette) {
		if (FreeImage_IsTransparent(dib)) {
			FILE_BGRA *bgra_pal = (FILE_BGRA*)malloc(header.cm_length * sizeof(FILE_BGRA));

			// get the transparency table
			BYTE *trns = FreeImage_GetTransparencyTable(dib);

			for (unsigned i = 0; i < header.cm_length; i++) {
				bgra_pal[i].b = palette[i].rgbBlue;
				bgra_pal[i].g = palette[i].rgbGreen;
				bgra_pal[i].r = palette[i].rgbRed;
				bgra_pal[i].a = trns[i];
			}

			io->write_proc(bgra_pal, sizeof(FILE_BGRA), header.cm_length, handle);

			free(bgra_pal);

		} else {
			FILE_BGR *bgr_pal = (FILE_BGR*)malloc(header.cm_length * sizeof(FILE_BGR));

			for (unsigned i = 0; i < header.cm_length; i++) {
				bgr_pal[i].b = palette[i].rgbBlue;
				bgr_pal[i].g = palette[i].rgbGreen;
				bgr_pal[i].r = palette[i].rgbRed;
			}

			io->write_proc(bgr_pal, sizeof(FILE_BGR), header.cm_length, handle);

			free(bgr_pal);
		}
	}

	// write the data bits

	const unsigned width = header.is_width;
	const unsigned height = header.is_height;

	if ((flags & TARGA_SAVE_RLE) == TARGA_SAVE_RLE) {

		// Note, image is compressed line by line, this is, packets don't span multiple lines (TGA2.0 recommendation)
		
		saveRLE(dib, width, height, pixel_size, io, handle);

	} else {
		// no rle compression

		BYTE *line, *const line_begin = (BYTE*)malloc(width * pixel_size);
		BYTE *line_source = line_begin;

		for (unsigned y = 0; y < height; y++) {
			BYTE *bits = FreeImage_GetScanLine(dib, y);

			//rewind the line pointer
			line = line_begin;

			switch (bpp) {
				case 8: {
					//don't copy line, read straight from dib
					line_source = bits;
				}
				break;

				case 16: {
					for (unsigned x = 0; x < width; x++) {
						WORD pixel = *(((WORD *)bits) + x);
#ifdef FREEIMAGE_BIGENDIAN
						SwapShort(&pixel);
#endif
						*(WORD*)line = pixel;

						line += pixel_size;
					}
				}
				break;

				case 24: {
					for (unsigned x = 0; x < width; x++) {
						RGBTRIPLE* trip = ((RGBTRIPLE *)bits) + x;
						line[0] = trip->rgbtBlue;
						line[1] = trip->rgbtGreen;
						line[2] = trip->rgbtRed;

						line += pixel_size;
					}
				}
				break;

				case 32: {
					for (unsigned x = 0; x < width; x++) {

						*(unsigned*)line = *((unsigned*)bits + x);

						line += pixel_size;
					}

				}
				break;
			}//switch(bpp)

			// write line to disk

			io->write_proc(line_source, pixel_size, width, handle);

		}//for height

		free(line_begin);
	}


	// write the footer

	TGAFOOTER footer;
	footer.developer_offset = 0;
	footer.extension_offset = 0;
	strcpy(footer.signature, "TRUEVISION-XFILE.");

#ifdef FREEIMAGE_BIGENDIAN
	SwapFooter(&footer);
#endif

	io->write_proc(&footer, sizeof(footer), 1, handle);

	return TRUE;
}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitTARGA(Plugin *plugin, int format_id) {
	s_format_id = format_id;

	plugin->format_proc = Format;
	plugin->description_proc = Description;
	plugin->extension_proc = Extension;
	plugin->regexpr_proc = RegExpr;
	plugin->open_proc = NULL;
	plugin->close_proc = NULL;
	plugin->pagecount_proc = NULL;
	plugin->pagecapability_proc = NULL;
	plugin->load_proc = Load;
	plugin->save_proc = Save;
	plugin->validate_proc = Validate;
	plugin->mime_proc = MimeType;
	plugin->supports_export_bpp_proc = SupportsExportDepth;
	plugin->supports_export_type_proc = SupportsExportType;
	plugin->supports_icc_profiles_proc = NULL;
	plugin->supports_no_pixels_proc = SupportsNoPixels;
}

 	  	 
