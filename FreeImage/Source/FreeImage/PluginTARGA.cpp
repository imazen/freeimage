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

/**
This struct is used only when loading RLE compressed images.
It is needed because it is (really) hard to determine the size of a
compressed line in the file (and allocate line cache as usual, refilling it at line change).
We use an arbitrary size instead and access it through this struct, it takes care to refill when needed.
NOTE: access must be *fast*, so safety measures are minimal.
*/
typedef struct tagCacheIO {
	BYTE *ptr;
	BYTE *home;
	BYTE *end;
	size_t size;
	FreeImageIO *io;	// not necessary, but
	fi_handle handle;	// passing them as args is slower
} CacheIO;

/**
Returns TRUE on success and FAlSE if malloc fails
Note however that I do not use this returned value in the code.
Allocating line cache even for a 100 000 wide 32bit bitmap will take under
half a megabyte. Out of Mem is really not an issue!
*/
static BOOL
cacheIO_alloc(CacheIO *ch, FreeImageIO *io, fi_handle handle, size_t size) {
	ch->home = NULL;
	ch->home = (BYTE*)malloc(size);
	if(ch->home == NULL) {
		return FALSE;
	}
	ch->end = ch->home + size;
	ch->size = size;
	ch->io = io;
	ch->handle = handle;

	ch->ptr = ch->end;	//will force refill on first access

	return TRUE;
}

static void
cacheIO_free(CacheIO *ch) {
	if(ch->home != NULL) {
		free(ch->home);
	}
}

inline static BYTE
cacheIO_getByte(CacheIO *ch) {
	if(ch->ptr >= ch->end) {
		// need refill
		ch->ptr = ch->home;
		ch->io->read_proc(ch->ptr, sizeof(BYTE), (unsigned)ch->size, ch->handle);	//### EOF - no problem?
	}

	BYTE result = *ch->ptr;
	ch->ptr++;

	return result;
}

inline static BYTE*
cacheIO_getBytes(CacheIO *ch, size_t count /*must be < ch.size!*/) {
	if(ch->ptr + count >= ch->end) {
		// need refill

		// 'count' bytes might span two cache bounds,
		// SEEK back to add the remains of the current cache again into the new cache

		long read = long(ch->ptr - ch->home);
		long remaining = long(ch->size - read);

		ch->io->seek_proc(ch->handle, -remaining, SEEK_CUR);

		ch->ptr = ch->home;
		ch->io->read_proc(ch->ptr, sizeof(BYTE), (unsigned)ch->size, ch->handle);	//### EOF - no problem?
	}

	BYTE *result = ch->ptr;
	ch->ptr += count;

	return result;
}

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
	if(io->seek_proc(handle, start_offset + eof - 18, SEEK_SET) == 0) {
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

// ----------------------------------------------------------
/**
Used for all 32 and 24 bit loading of uncompressed images
*/
static
void loadTrueColor(FIBITMAP* dib, int width, int height, int file_pixel_size, FreeImageIO* io, fi_handle handle, bool as24bit) {
	const int pixel_size = as24bit ? 3 : file_pixel_size;
	
	// input line cache
	BYTE* file_line = (BYTE*)malloc( width * file_pixel_size);
	if(!file_line) throw FI_MSG_ERROR_MEMORY;

	for(int y = 0; y < height; ++y) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);
		io->read_proc(file_line, file_pixel_size, width, handle);
		BYTE *bgra = file_line;
		for(int x = 0; x < width; ++x) {
			bits[FI_RGBA_BLUE]	= bgra[0];
			bits[FI_RGBA_GREEN] = bgra[1];
			bits[FI_RGBA_RED]	= bgra[2];
			if(!as24bit) {
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
void assignPixel(BYTE* bits, BYTE* val, bool as24bit) {
	// static assert should go here
	assert(false);
}

template <>
inline
void assignPixel<8>(BYTE* bits, BYTE* val, bool as24bit) {
	*bits = *val;
}
template <>
inline
void assignPixel<16>(BYTE* bits, BYTE* val, bool as24bit) {
	WORD value(*reinterpret_cast<WORD*>(val));
	
#ifdef FREEIMAGE_BIGENDIAN
	SwapShort(value);
#endif
	
	if(as24bit) {
		bits[FI_RGBA_BLUE]  = (BYTE)((((value & FI16_555_BLUE_MASK) >> FI16_555_BLUE_SHIFT) * 0xFF) / 0x1F);
		bits[FI_RGBA_GREEN] = (BYTE)((((value & FI16_555_GREEN_MASK) >> FI16_555_GREEN_SHIFT) * 0xFF) / 0x1F);
		bits[FI_RGBA_RED]   = (BYTE)((((value & FI16_555_RED_MASK) >> FI16_555_RED_SHIFT) * 0xFF) / 0x1F);
	} else {
		*reinterpret_cast<WORD *>(bits) = 0x7FFF & value;
	}
}

template <>
inline
void assignPixel<24>(BYTE* bits, BYTE* val, bool as24bit) {
	bits[FI_RGBA_BLUE]	= val[0];
	bits[FI_RGBA_GREEN] = val[1];
	bits[FI_RGBA_RED]	= val[2];
}

template <>
inline
void assignPixel<32>(BYTE* bits, BYTE* val, bool as24bit) {
	if(as24bit) {
		assignPixel<24>(bits, val, true);
	} else {
#if FREEIMAGE_COLORORDER == FREEIMAGE_COLORORDER_BGR
		*(reinterpret_cast<unsigned*>(bits)) = *(reinterpret_cast<unsigned*> (val));
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
void loadRLE(FIBITMAP* dib, int width, int height, FreeImageIO* io, fi_handle handle, long eof, bool as24bit) {
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
	CacheIO cache;
	cacheIO_alloc(&cache, io, handle, sz);

	int x = 0, y = 0;

	line_bits = FreeImage_GetScanLine(dib, y);

	while(y < height) {

		rle = cacheIO_getByte(&cache);

		BOOL has_rle = rle & 0x80;
		rle &= ~0x80; // remove type-bit

		BYTE packet_count = rle + 1;

		//packet_count might be corrupt, test if we are not about to write beyond the last image bit
		if((line_bits+x) + packet_count*pixel_size > dib_end) {
			cacheIO_free(&cache);
			FreeImage_OutputMessageProc(s_format_id, FI_MSG_ERROR_CORRUPTED);			
			return;
		}

		if(has_rle) {

			// read a pixel value from file...
			BYTE *val = cacheIO_getBytes(&cache, file_pixel_size);

			//...and fill packet_count pixels with it
			for(int ix = 0; ix < packet_count; ix++) {
				assignPixel<bPP>((line_bits+x), val, as24bit);
				x += pixel_size;
				if(x >= line_size) {
					x = 0;
					y++;			
					line_bits = FreeImage_GetScanLine(dib, y);
				}
			}
		} else {
			// no rle commpresion

			// copy packet_count pixels from file to dib
			for(int ix = 0; ix < packet_count; ix++) {
				BYTE *val = cacheIO_getBytes(&cache, file_pixel_size);
				assignPixel<bPP>((line_bits+x), val, as24bit);
				x += pixel_size;
				if(x >= line_size) {
					x = 0;
					y++;
					line_bits = FreeImage_GetScanLine(dib, y);
				}
			} //< packet_count
		} //< has_rle

	} //< while height

	cacheIO_free(&cache);
}

// ------------
static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	FIBITMAP *dib = NULL;

	if(!handle) {
		return NULL;
	}

	try {
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
		const int pitch = CalculatePitch(line);
		const int pixel_bits = header.is_pixel_depth;
		const int pixel_size = pixel_bits/8;
		
		int alphabits = header.is_image_descriptor & 0x0f;// It seems this is ALWAYS: 8 for 32bit, 1 for 16bit, 0 for 24bit
		int fliphoriz = (header.is_image_descriptor & 0x10) ? 1 : 0;
		int flipvert = (header.is_image_descriptor & 0x20) ? 1 : 0;

		// skip comment
		io->seek_proc(handle, header.id_length, SEEK_CUR);

		switch(header.is_pixel_depth) {
			case 8 : 
			{
				dib = FreeImage_Allocate(header.is_width, header.is_height, 8);
				if(dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}
				
				// read the palette

				RGBQUAD *palette = FreeImage_GetPalette(dib);
				
				if(header.color_map_type == 0) {
					// no color-map data is included with this image ...
					// build a greyscale palette
					for(unsigned i = 0; i < 256; i++) {
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
					switch(header.cm_size) {
						case 16: 
						{
							WORD *rgb555 = (WORD*)&cmap[0];
							for(count = header.cm_first_entry; count < header.cm_length; count++) {
								palette[count].rgbRed   = (BYTE)((((*rgb555 & FI16_555_RED_MASK) >> FI16_555_RED_SHIFT) * 0xFF) / 0x1F);
								palette[count].rgbGreen = (BYTE)((((*rgb555 & FI16_555_GREEN_MASK) >> FI16_555_GREEN_SHIFT) * 0xFF) / 0x1F);
								palette[count].rgbBlue  = (BYTE)((((*rgb555 & FI16_555_BLUE_MASK) >> FI16_555_BLUE_SHIFT) * 0xFF) / 0x1F);
								rgb555++;
							}
						}
						break;
						
						case 24: 
						{
							FILE_BGR *bgr = (FILE_BGR*)&cmap[0];
							for(count = header.cm_first_entry; count < header.cm_length; count++) {
								palette[count].rgbBlue  = bgr->b;
								palette[count].rgbGreen = bgr->g;
								palette[count].rgbRed   = bgr->r;
								bgr++;
							}
						}
						break;
						
						case 32: 
						{
							BYTE trns[256];
							
							// clear the transparency table
							memset(trns, 0xFF, 256);

							FILE_BGRA *bgra = (FILE_BGRA*)&cmap[0];
							for(count = header.cm_first_entry; count < header.cm_length; count++) {
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
				
				// read in the bitmap bits
				
				switch(header.image_type) {
					case TGA_CMAP:
					case TGA_MONO: 
					{
						BYTE *bits = NULL;
						for(unsigned count = 0; count < header.is_height; count++) {
							bits = FreeImage_GetScanLine(dib, count);
							io->read_proc(bits, sizeof(BYTE), line, handle);
						}
					}
					break;
					
					case TGA_RLECMAP:
					case TGA_RLEMONO: //(8 bit)
					{ 
						loadRLE<8>(dib, header.is_width, header.is_height, io, handle, eof, false);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 15 :
			case 16 : 
			{
				int pixel_bits = 16;

				// allocate the dib

				if(TARGA_LOAD_RGB888 & flags) {
					pixel_bits = 24;
					dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
				} else {
					dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, FI16_555_RED_MASK, FI16_555_GREEN_MASK, FI16_555_BLUE_MASK);
				}

				if(dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}

				int line = CalculateLine(header.is_width, pixel_bits);

				const unsigned pixel_size = unsigned(pixel_bits) / 8;
				const unsigned src_pixel_size = sizeof(WORD);

				// note header.cm_size is a misleading name, it should be seen as header.cm_bits
				// ignore current position in file and set filepointer explicitly from the beginning of the file

				int garblen = 0;

				if(header.color_map_type != 0) {
					garblen = (int)((header.cm_size + 7) / 8) * header.cm_length; /* should byte align */
				} else {
					garblen = 0;
				}

				io->seek_proc(handle, start_offset, SEEK_SET);
				io->seek_proc(handle, sizeof(tagTGAHEADER) + header.id_length + garblen, SEEK_SET);

				// read in the bitmap bits

				switch(header.image_type) {
					case TGA_RGB: //(16 bit)
					{ 
						// input line cache
						BYTE *in_line = (BYTE*)malloc(header.is_width * sizeof(WORD));
						if(!in_line) throw FI_MSG_ERROR_MEMORY;

						const int h = header.is_height;
						for(int y = 0; y < h; y++) {
							BYTE *bits;

							bits = FreeImage_GetScanLine(dib, y);

							io->read_proc(in_line, src_pixel_size, header.is_width, handle);

							BYTE *val = in_line;
							for(int x = 0; x < line; x += pixel_size) {

								assignPixel<16>(bits+x, val, TARGA_LOAD_RGB888 & flags);
								
								val += src_pixel_size;
							}
						}

						free(in_line);
					}
					break;

					case TGA_RLERGB: //(16 bit)
					{ 
						loadRLE<16>(dib, header.is_width, header.is_height, io, handle, eof, TARGA_LOAD_RGB888 & flags);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 24 : 
			{

				dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
				if(dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}

				// read in the bitmap bits

				switch(header.image_type) {
					case TGA_RGB: //(24 bit)
					{ 
						//uncompressed
						loadTrueColor(dib, header.is_width, header.is_height, pixel_size,io, handle, true);
					}
					break;

					case TGA_RLERGB: //(24 bit)
					{
						loadRLE<24>(dib, header.is_width, header.is_height, io, handle, eof, true);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;

			case 32 : 
			{
				int pixel_bits = 32;

				if(TARGA_LOAD_RGB888 & flags) {
					pixel_bits = 24;
				}

				// Allocate the DIB
				dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);

				if(dib == NULL) {
					throw FI_MSG_ERROR_DIB_MEMORY;
				}

				// read in the bitmap bits

				switch(header.image_type) {
					case TGA_RGB: //(32 bit)
					{
						// uncompressed
						loadTrueColor(dib, header.is_width, header.is_height, 4 /*file_pixel_size*/, io, handle, TARGA_LOAD_RGB888 & flags);
					}
					break;
					
					case TGA_RLERGB: //(32 bit)
					{
						loadRLE<32>(dib, header.is_width, header.is_height, io, handle, eof, TARGA_LOAD_RGB888 & flags);
					}
					break;

					default :
						FreeImage_Unload(dib);
						return NULL;
				}
			}
			break;
		} // switch(header.is_pixel_depth)
		
		if(flipvert) {
			FreeImage_FlipVertical(dib);
		}
		if(fliphoriz) {
			FreeImage_FlipHorizontal(dib);
		}

		return dib;

	} catch (const char *message) {
		if(dib) {
			FreeImage_Unload(dib);
		}
		FreeImage_OutputMessageProc(s_format_id, message);
		return NULL;
	}
}

static BOOL DLL_CALLCONV
Save(FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	if((dib != NULL) && (handle != NULL)) {
		RGBQUAD *palette = FreeImage_GetPalette(dib);
		int bpp = FreeImage_GetBPP(dib);

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

		if(palette) {
			header.color_map_type = 1;
			header.image_type = TGA_CMAP;
			header.cm_length = (WORD)(1 << bpp);
			if(FreeImage_IsTransparent(dib)) {
				header.cm_size = 32;
			} else {
				header.cm_size = 24;
			}
		} else {
			header.color_map_type = 0;
			header.image_type = TGA_RGB;
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

		if(palette) {
			if(FreeImage_IsTransparent(dib)) {
				FILE_BGRA *bgra_pal = (FILE_BGRA*)malloc(header.cm_length * sizeof(FILE_BGRA));

				// get the transparency table
				BYTE *trns = FreeImage_GetTransparencyTable(dib);

				for(unsigned i = 0; i < header.cm_length; i++) {
					bgra_pal[i].b = palette[i].rgbBlue;
					bgra_pal[i].g = palette[i].rgbGreen;
					bgra_pal[i].r = palette[i].rgbRed;
					bgra_pal[i].a = trns[i];
				}

				io->write_proc(bgra_pal, sizeof(FILE_BGRA), header.cm_length, handle);

				free(bgra_pal);

			} else {
				FILE_BGR *bgr_pal = (FILE_BGR*)malloc(header.cm_length * sizeof(FILE_BGR));

				for(unsigned i = 0; i < header.cm_length; i++) {
					bgr_pal[i].b = palette[i].rgbBlue;
					bgr_pal[i].g = palette[i].rgbGreen;
					bgr_pal[i].r = palette[i].rgbRed;
				}

				io->write_proc(bgr_pal, sizeof(FILE_BGR), header.cm_length, handle);

				free(bgr_pal);
			}
		}

		// write the data bits

		for(unsigned y = 0; y < header.is_height; y++) {
			BYTE *bits = FreeImage_GetScanLine(dib, y);

			switch(bpp) {
				case 8: 
				{
					io->write_proc(bits, header.is_width, 1, handle);
				}
				break;
				
				case 16: 
				{
					WORD pixel;
					for(unsigned x = 0; x < header.is_width; x++) {
						pixel = *(((WORD *)bits) + x);
#ifdef FREEIMAGE_BIGENDIAN
						SwapShort(&pixel);
#endif
						io->write_proc(&pixel, sizeof(WORD), 1, handle);
					}
				}
				break;
				
				case 24: 
				{
					FILE_BGR bgr;
					RGBTRIPLE *trip;
					for(unsigned x = 0; x < header.is_width; x++) {
						trip = ((RGBTRIPLE *)bits) + x;
						bgr.b = trip->rgbtBlue;
						bgr.g = trip->rgbtGreen;
						bgr.r = trip->rgbtRed;
						io->write_proc(&bgr, sizeof(FILE_BGR), 1, handle);
					}
				}
				break;
				
				case 32: 
				{
					FILE_BGRA bgra;
					RGBQUAD *quad;
					for(unsigned x = 0; x < header.is_width; x++) {
						quad = ((RGBQUAD *)bits) + x;
						bgra.b = quad->rgbBlue;
						bgra.g = quad->rgbGreen;
						bgra.r = quad->rgbRed;
						bgra.a = quad->rgbReserved;
						io->write_proc(&bgra, sizeof(FILE_BGRA), 1, handle);
					}
				}
				break;
			}
		}

		// write the TARGA signature

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

	return FALSE;
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
}
