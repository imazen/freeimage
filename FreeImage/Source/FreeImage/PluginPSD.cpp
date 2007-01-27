// ==========================================================
// Photoshop Loader
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Thorsten Radde (support@IdealSoftware.com)
//
// Based on public domain code created and
// published by Thatcher Ulrich (ulrich@world.std.com)
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

// ==========================================================
// Bitmap pointer access macros
// ==========================================================

#define BP_START(DIB, WIDTH, HEIGHT) \
	int xxxscanline = 0; \
	int xxxpixel = 0; \
	RGBQUAD *xxxp = (RGBQUAD *)FreeImage_GetScanLine(DIB, HEIGHT - 1 - xxxscanline);

#define BP_NEXT(DIB, WIDTH, HEIGHT) \
	xxxp++; \
	if (++xxxpixel == WIDTH) { \
		xxxscanline++; \
		xxxpixel = 0; \
		xxxp = (RGBQUAD *)FreeImage_GetScanLine(DIB, HEIGHT - 1 - xxxscanline); \
	}

#define BP_SETVALUE(VALUE, OFFSET) \
	((BYTE *)xxxp)[OFFSET] = (BYTE)VALUE;

// ==========================================================
// Structures
// ==========================================================

#ifdef _WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif // _WIN32

typedef struct tagPSDChannelLayout {
	WORD channel_id;
	DWORD length;
} PSDChannelLayout;

typedef struct tagPSDChannelInfo {
	int	ofs, deflt;
} PSDChannelInfo;

typedef struct tagPSDMaskInfo {
	int top;
	int left;
	int bottom;
	int right;
	int default_color;
	int flags;
	int padding;
} PSDMaskInfo;

typedef struct tagPSDLayerInfo {
	std::string name;
	int left;
	int top;
	int right;
	int bottom;
	int channels;
	int blend_mode_sig;
	int blend_mode_key;
	int opacity;
	int clipping;
	int flags;
	int filler;
	BOOL has_mask;
	PSDMaskInfo mask_info;
	std::vector<unsigned int> ranges;
	BOOL top_layer;
	int offset_in_file;
	PSDChannelLayout channel_info[64];
} PSDLayerInfo;

typedef struct tagPSDInfo {
	int version;
	unsigned channel_count;
	unsigned height;
	unsigned width;
	unsigned depth;
	unsigned mode;
	int bitmap_offset_in_file;
	float hres;
	float vres;
	std::vector<PSDLayerInfo> layers;
} PSDInfo;

#ifdef _WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif // _WIN32

// ==========================================================
// Internal functions
// ==========================================================

static BYTE
Read8(FreeImageIO *io, fi_handle handle) {
	BYTE value = 0;
	if(io->read_proc(&value, 1, 1, handle) != 1)
		return 0;
	return value;
}

static WORD
Read16(FreeImageIO *io, fi_handle handle) {
	// reads a two-byte big-endian integer from the given file and returns its value.
	// assumes unsigned.

	WORD value = 0;
	if(io->read_proc(&value, 2, 1, handle) != 1) 
		return 0;
#ifndef FREEIMAGE_BIGENDIAN
	SwapShort(&value);
#endif
	return value;
}

static DWORD
Read32(FreeImageIO *io, fi_handle handle) {
	// reads a four-byte big-endian integer from the given file and returns its value.
	// assumes unsigned.

	DWORD value = 0;
	if(io->read_proc(&value, 4, 1, handle) != 1)
		return 0;
#ifndef FREEIMAGE_BIGENDIAN
	SwapLong(&value);
#endif
	return value;
}

// ----------------------------------------------------------

static void
ResolutionInfo(float &hres, float &vres, FreeImageIO *io, fi_handle handle, int &byte_count, int &data_size) {
	int junk;

	int	hres_fixed = Read32(io, handle);
	junk = Read16(io, handle);	// display units of hres.
	junk = Read16(io, handle);	// display units of width.

	int	vres_fixed = Read32(io, handle);
	junk = Read16(io, handle);	// display units of vres.
	junk = Read16(io, handle);	// display units of height.

	byte_count -= data_size;
	data_size -= 16;

	// skip any extra bytes at the end of this block...

	if (data_size > 0)
		io->seek_proc(handle, data_size, SEEK_CUR);			

	// need to convert resolution figures from fixed point, pixels/inch
	// to floating point, pixels/meter.

	hres = hres_fixed * ((float)39.4 / (float)65536.0);
	vres = vres_fixed * ((float)39.4 / (float)65536.0);			
}

// ----------------------------------------------------------

static void
ReadModeData(FreeImageIO *io, fi_handle handle, PSDInfo &psd_info) {
	int	mode_data_count = Read32(io, handle);	

	if (mode_data_count)
		io->seek_proc(handle, mode_data_count, SEEK_CUR);	
}

static void
ReadResourceData(FreeImageIO *io, fi_handle handle, PSDInfo &psd_info) {
	int	resource_data_count = Read32(io, handle);
	int byte_count = resource_data_count;

	while (byte_count) {
		// read the image resource block header.

		if (Read32(io, handle) != 0x3842494D /* "8BIM" */)
			throw "image resource block has unknown signature";

		int	resource_id = Read16(io, handle);

		// skip the name.

		int	name_length = Read8(io, handle) | 1;	// name_length must be odd, so that total including size byte is even.
		char *name = new char[name_length];
		io->read_proc(name, 1, name_length, handle);

		// get the size of the data block.

		int	data_size = Read32(io, handle);

		if ((data_size & 1) == 1)
			data_size++; // block size must be even.		

		// account for header size.

		byte_count -= 11 + name_length;

		switch(resource_id) {
			case 0x03ED :
				ResolutionInfo(psd_info.hres, psd_info.vres, io, handle, byte_count, data_size);
				break;

			default :
				io->seek_proc(handle, data_size, SEEK_CUR);
				byte_count -= data_size;
				break;
		};

		delete [] name;
	}
}

static void
ReadLayerInfo(FreeImageIO *io, fi_handle handle, PSDLayerInfo &layer_info) {
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	// LAYER RECORDS
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	layer_info.top_layer = FALSE;
	layer_info.top = Read32(io, handle);
	layer_info.left = Read32(io, handle);
	layer_info.bottom = Read32(io, handle);
	layer_info.right = Read32(io, handle);
	layer_info.channels = Read16(io, handle);

	for (int j = 0; j < layer_info.channels; ++j) {
		layer_info.channel_info[j].channel_id = Read16(io, handle);
		layer_info.channel_info[j].length = Read32(io, handle);
	}

	layer_info.blend_mode_sig = Read32(io, handle);
	layer_info.blend_mode_key = Read32(io, handle);
	layer_info.opacity = Read8(io, handle);
	layer_info.clipping = Read8(io, handle);
	layer_info.flags = Read8(io, handle);
	layer_info.filler = Read8(io, handle);
	unsigned extra_data_size = Read32(io, handle);

	// layer mask data

	int mask_size = Read32(io, handle);

	if (mask_size > 0) {
		layer_info.has_mask = TRUE;
		layer_info.mask_info.top = Read32(io, handle);
		layer_info.mask_info.left = Read32(io, handle);
		layer_info.mask_info.bottom = Read32(io, handle);
		layer_info.mask_info.right = Read32(io, handle);
		layer_info.mask_info.default_color = Read8(io, handle);
		layer_info.mask_info.flags = Read8(io, handle);
		layer_info.mask_info.padding = Read16(io, handle);										
	} else {
		layer_info.has_mask = FALSE;
	}

	extra_data_size -= 4 + mask_size;

	// layer blending ranges data

	int ranges_length = Read32(io, handle);
	layer_info.ranges.resize(ranges_length);

	for (int k = 0; k < (ranges_length / (int)sizeof(int)); ++k)
		layer_info.ranges[k] = Read32(io, handle);

	extra_data_size -= 4 + ranges_length;

	// layer name

	int	layer_length = Read8(io, handle);
	int layer_length_pad = layer_length + 3 & ~3;	// length must be multiple of 4 bytes
	char *layer_name = new char[layer_length_pad + 1];
	io->read_proc(layer_name, 1, layer_length_pad, handle);
	layer_name[layer_length] = '\0';
	layer_info.name = layer_name;
	delete [] layer_name;

	extra_data_size -= 1 + layer_length_pad;

	// skip remaining bytes in section (data for photoshop 4.0 and higher)

	io->seek_proc(handle, extra_data_size, SEEK_CUR);
}

static FIBITMAP *
LoadPSDLayer(FreeImageIO *io, fi_handle handle, PSDLayerInfo &layer_info) {
	// skip the mode data.  (it's the palette for indexed color; other info for other modes.)
	
	int width = layer_info.right - layer_info.left;
	int height = layer_info.bottom - layer_info.top;
	long area = width * height;

	// find out if the data is compressed
	//   0: no compression
	//   1: RLE compressed

	unsigned compression = Read16(io, handle);

	if ((compression > 1) || (compression < 0))
		return NULL;	

	// some formatting information about the channels

	PSDChannelLayout *channel_info = layer_info.channel_info;
	PSDChannelInfo Channel[4];

	for (int i = 0; i < 4; ++i) {
		if (i < layer_info.channels) {
			switch(channel_info[i].channel_id) {
				case 0x0000 :
					Channel[i].ofs = FI_RGBA_RED;
					Channel[i].deflt = 0;
					break;

				case 0x0001 :
					Channel[i].ofs = FI_RGBA_GREEN;
					Channel[i].deflt = 0;
					break;

				case 0x0002 :
					Channel[i].ofs = FI_RGBA_BLUE;
					Channel[i].deflt = 0;
					break;

				default :
					Channel[i].ofs = FI_RGBA_ALPHA;
					Channel[i].deflt = 0;
					break;
			}
		} else {
			Channel[i].ofs = FI_RGBA_ALPHA;
			Channel[i].deflt = 0;
		}
	}

	// Create the destination bitmap
	
	FIBITMAP *dib = FreeImage_Allocate(width, height, 32, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);

	// finally, read the image data.

	if (compression) {
		// the RLE-compressed data is preceeded by a 2-byte data count for each row in the data
		// which we're going to just skip. 

		if (layer_info.top_layer)
			io->seek_proc(handle, height * layer_info.channels * 2, SEEK_CUR);
		else
			io->seek_proc(handle, height * 2, SEEK_CUR);	

		// read the RLE data by channel

		for (int channel = 0; channel < 4; channel++) {
			const PSDChannelInfo &c = Channel[channel];
			BP_START(dib, width, height)

			int tell = io->tell_proc(handle);

			if (channel >= layer_info.channels) {
				// fill this channel with default data.

				for (int i = 0; i < area; i++) {
					BP_SETVALUE(c.deflt, c.ofs);
					BP_NEXT(dib, width, height)
				}
			} else {
				// read the RLE data.

				int	count = 0;

				while (count < area) {
					unsigned len = (unsigned)Read8(io, handle);

					if (len == 128) {
						// nop
					} else if (len < 128) {
						// copy next len + 1 bytes literally.

						len = 1 + len;

						if ((long)(count + len) > area)
							len = area - count; 

						count += len;

						while (len) {
							unsigned val = Read8(io, handle);
							BP_SETVALUE(val, c.ofs);
							BP_NEXT(dib, width, height)

							len--;
						}
					} else if (len > 128) {
						// next -len + 1 bytes in the dest are replicated from next source byte.
						// (interpret len as a negative 8-bit int.)

						len ^= 0x0FF;
						len += 2; 

						if ((long)(count + len) > area)
							len = area - count; 

						unsigned val = Read8(io, handle);
						count += len;

						while (len) {
							BP_SETVALUE(val, c.ofs);
							BP_NEXT(dib, width, height)

							len--;
						}
					}
				}
			}

			// skip any remaining padding data

			if (!layer_info.top_layer)
				io->seek_proc(handle, channel_info[channel].length - (io->tell_proc(handle) - tell), SEEK_CUR);
		}		
	} else {
		// we're at the raw image data.  it's each channel in order (Red, Green, Blue, Alpha, ...)
		// where each channel consists of an 8-bit value for each pixel in the image.

		for (int channel = 0; channel < 4; channel++) {
			const PSDChannelInfo &c = Channel[channel];

			BP_START(dib, width, height)

			if (channel >= layer_info.channels) {
				// fill this channel with default data.

				for (int i = 0; i < area; i++) {
					BP_SETVALUE(c.deflt, c.ofs);
					BP_NEXT(dib, width, height)
				}

			} else {
				// read the data

				for (int i = 0; i < area; i++) {
					unsigned val = Read8(io, handle);
					BP_SETVALUE(val, c.ofs);
					BP_NEXT(dib, width, height)
				}
			}

			// skip the 2 bytes that terminate the channel data

			if (!layer_info.top_layer)
				Read16(io, handle);
		}
	}
	
	return dib;
}

// ==========================================================
// Plugin Interface
// ==========================================================

static int s_format_id;

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "PSD";
}

static const char * DLL_CALLCONV
Description() {
	return "Adobe Photoshop";
}

static const char * DLL_CALLCONV
Extension() {
	return "psd";
}

static const char * DLL_CALLCONV
MimeType() {
	return "image/freeimage-psd";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {
	return (Read32(io, handle) == 0x38425053);
}

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return FALSE;
}

static BOOL DLL_CALLCONV 
SupportsExportType(FREE_IMAGE_TYPE type) {
	return FALSE;
}

// ----------------------------------------------------------

static void *DLL_CALLCONV 
Open(FreeImageIO *io, fi_handle handle, BOOL read) {
	if (Validate(io, handle)) { // '8BPS'
		PSDInfo *psd_info = new PSDInfo;
		psd_info->version = Read16(io, handle);

		if (psd_info->version == 1) { // Version
			// 6 reserved bytes.

			Read32(io, handle);
			Read16(io, handle);

			// Read the number of channels (R, G, B, A, etc).

			psd_info->channel_count = Read16(io, handle);

			if (psd_info->channel_count >= 0 && psd_info->channel_count <= 16) {
				psd_info->height = Read32(io, handle);
				psd_info->width = Read32(io, handle);
				psd_info->depth = Read16(io, handle);
				psd_info->mode = Read16(io, handle);

				if (psd_info->depth == 8) {
					// Resolution Information

					psd_info->hres = 2835; // 72 dpi
					psd_info->vres = 2835; // 72 dpi

					// Interpret image resource info blocks

					ReadModeData(io, handle, *psd_info);
					ReadResourceData(io, handle, *psd_info);

					// store the start of the merged bitmap data

					int	misc_info_size = Read32(io, handle);
					psd_info->bitmap_offset_in_file = io->tell_proc(handle) + misc_info_size;
						
					if (misc_info_size > 0) {
						DWORD layer_info_size = Read32(io, handle);
						WORD layer_count = Read16(io, handle);

						if (layer_count > 0) {
							psd_info->layers.resize(layer_count);

							// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
							// LAYER INFO
							// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

							for (int i = 0; i < layer_count; ++i)
								ReadLayerInfo(io, handle, psd_info->layers[i]);

							// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
							// LAYER BITMAP DATA
							// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

							for (int j = 0; j < layer_count; ++j) {
								psd_info->layers[j].offset_in_file = io->tell_proc(handle);

								unsigned compression = Read16(io, handle);
								int layer_width = psd_info->layers[j].right - psd_info->layers[j].left;
								int image_size = 0;

								for (int k = 0; k < psd_info->layers[j].channels; ++k)
									image_size += psd_info->layers[j].channel_info[k].length;

								if (compression)
									image_size += 2 * layer_width;

								io->seek_proc(handle, image_size - ((layer_width * 2) + 2), SEEK_CUR);
							}

							return (void *)psd_info;
						}
					}
				}
			}
		}

		delete psd_info;
	}

	return NULL;
}

static void DLL_CALLCONV
Close(FreeImageIO *io, fi_handle handle, void *data) {
	PSDInfo *psd_info = (PSDInfo *)data;
	delete psd_info;
}

static int DLL_CALLCONV
PageCount(FreeImageIO *io, fi_handle handle, void *data) {
	PSDInfo *psd_info = (PSDInfo *)data;
	return (psd_info) ? psd_info->layers.size() : 0;
}

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	try {
		PSDInfo *psd_info = (PSDInfo *)data;

		if (psd_info) {
			FIBITMAP *pBitmap = NULL;

			// Valid options are:
			//   0: Bitmap (not implemented)
			//   1: Grayscale (not implemented)
			//   2: Indexed color (not implemented)
			//   3: RGB color
			//   4: CMYK color (not implemented)
			//   7: Multichannel (not implemented)
			//   8: Duotone (not implemented)
			//   9: Lab color (not implemented)

			switch (psd_info->mode) {
				case 3 :
					if (page == -1) {
						io->seek_proc(handle, psd_info->bitmap_offset_in_file, SEEK_SET);

						PSDLayerInfo info;
						info.top_layer = TRUE;
						info.left = 0;
						info.top = 0;
						info.right = psd_info->width;
						info.bottom = psd_info->height;
						info.channels = psd_info->channel_count;
						info.channel_info[0].channel_id = 0;
						info.channel_info[0].length = 0;
						info.channel_info[1].channel_id = 1;
						info.channel_info[1].length = 0;
						info.channel_info[2].channel_id = 2;
						info.channel_info[2].length = 0;
						info.channel_info[3].channel_id = 0xffff;
						info.channel_info[3].length = 0;

						pBitmap = LoadPSDLayer(io, handle, info);
					} else if (page < (int)psd_info->layers.size()) {
						io->seek_proc(handle, psd_info->layers[page].offset_in_file, SEEK_SET);
						pBitmap = LoadPSDLayer(io, handle, psd_info->layers[page]);
					}

					if (pBitmap) {
						FreeImage_SetDotsPerMeterX(pBitmap, (LONG)psd_info->hres);
						FreeImage_SetDotsPerMeterY(pBitmap, (LONG)psd_info->vres);
					}

					return pBitmap;
			
				default :
					throw "color mode not supported";
			}
		}
	} catch(const char *message) {
		FreeImage_OutputMessageProc(s_format_id, message);
	}

	return NULL;
}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitPSD(Plugin *plugin, int format_id) {
	s_format_id = format_id;

	plugin->format_proc = Format;
	plugin->description_proc = Description;
	plugin->extension_proc = Extension;
	plugin->regexpr_proc = NULL;
	plugin->open_proc = Open;
	plugin->close_proc = Close;
	plugin->pagecount_proc = PageCount;
	plugin->pagecapability_proc = NULL;
	plugin->load_proc = Load;
	plugin->save_proc = NULL;
	plugin->validate_proc = Validate;
	plugin->mime_proc = MimeType;
	plugin->supports_export_bpp_proc = SupportsExportDepth;
	plugin->supports_export_type_proc = SupportsExportType;
	plugin->supports_icc_profiles_proc = NULL;	// not implemented yet;
}
