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

#ifdef WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif

typedef struct tagBGRAQUAD { 
  BYTE bgraBlue; 
  BYTE bgraGreen; 
  BYTE bgraRed;
  BYTE bgraAlpha;
} BGRAQUAD; 

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

#ifdef WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif

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

static BYTE *
Internal_GetScanLine(FIBITMAP *dib, int scanline, int flipvert) {
	//assert ((scanline >= 0) && (scanline < (int)FreeImage_GetHeight(dib)));

	if (flipvert)
		return FreeImage_GetScanLine(dib, scanline);
	else
		return FreeImage_GetScanLine(dib, FreeImage_GetHeight(dib) - scanline - 1);
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
	TGAHEADER header;
	char extra[256]; // 256 temporary bytes (only 255 will be used though)

	// try to read the header in a whole
	if(io->read_proc(&header, sizeof(tagTGAHEADER), 1, handle) != 1)
		return FALSE;

	// The Color Map Type should be a 0 or a 1...
	// NOTE: are other values possible?
	//   NO, according to http://cvs.sf.net/viewcvs.py/eifogl/eifogl/utility/tga/tga_file.e?rev=1.3
	//       and it seems that the current Targa implementation for FreeImage
	//       assumes that anything other than 0 means that there is a Color Map
	if(header.color_map_type != 0 && header.color_map_type != 1)
		return FALSE;

	// If the Color Map Type is 1 then we validate the map entry information...
	// NOTA: let's stay compatible with the current FreeImage Targa implementation
	//       and check against Type 0 instead against Type 1
	if(header.color_map_type != 0) {
		// It doesn't make any sense if the first entry is larger than the Color Map Table
		if(header.cm_first_entry >= header.cm_length)
			return FALSE;

		// NOTE : maybe we should check header.cm_size for known values?
		//        for now, don't allow 0 or anything bigger than 32
		if(header.cm_size <= 0 || header.cm_size > 32)
			return FALSE;
	}
	else {
		// I have disabled this because I'm not really sure
		// but can one assume that if there is no Color Map
		// than all 3 related values are zeroed?
		// NOTE: atleast in my test files that seems to be the case...
		/*
		if(header.cm_first_entry != 0 || header.cm_length != 0 || header.cm_size != 0)
			return FALSE;
		*/
	}

	// the width/height shouldn't be 0, right?
	if(header.is_width == 0 || header.is_height == 0)
		return FALSE;

	// the extra data (following after the header) should be there
	if(io->read_proc(extra, 1, header.id_length, handle) != header.id_length)
		return FALSE;

	// let's now verify all the types that are supported by FreeImage
	// NOTE : this is our final verification
	switch(header.is_pixel_depth) {
		case 8: 
			switch(header.image_type) {
			  case TGA_CMAP:
			  case TGA_MONO:
			  case TGA_RLECMAP:
			  case TGA_RLEMONO: return TRUE;
			}
			break;
		case 15:
		case 16:
		case 24:
		case 32: 
			switch(header.image_type) {
				case TGA_RGB:
				case TGA_RLERGB: return TRUE;
			}
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

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	if (handle) {
		try {
			// remember the start offset

			long start_offset = io->tell_proc(handle);

			// read and process the bitmap's header

			FIBITMAP *dib = NULL;
			TGAHEADER header;

			io->read_proc(&header, sizeof(tagTGAHEADER), 1, handle);

			int line = CalculateLine(header.is_width, header.is_pixel_depth);
			int pitch = CalculatePitch(line);
			int alphabits = header.is_image_descriptor & 0x0f;
			int fliphoriz = (header.is_image_descriptor & 0x10) ? 0 : 1;
			int flipvert = (header.is_image_descriptor & 0x20) ? 1 : 0;

			// skip comment
			io->seek_proc(handle, header.id_length, SEEK_CUR);

			switch (header.is_pixel_depth) {
				case 8 :
				{
					dib = FreeImage_Allocate(header.is_width, header.is_height, 8);

					if (dib == NULL)
						throw "DIB allocation failed";
					
					// read the palette

					RGBQUAD *palette = FreeImage_GetPalette(dib);

					if (header.color_map_type == 0) {
						// no color-map data is included with this image ...
						// build a greyscale palette
						for (unsigned i = 0; i < 256; i++) {
							palette[i].rgbRed	= i;
							palette[i].rgbGreen = i;
							palette[i].rgbBlue	= i;
						}
					}
					else {
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
								for (count = header.cm_first_entry; count < header.cm_length; count++) {						
									palette[count].rgbRed = (((*rgb555 & 0x7C00) >> 10) * 0xFF) / 0x1F;
									palette[count].rgbGreen = (((*rgb555 & 0x3E0) >> 5) * 0xFF) / 0x1F;
									palette[count].rgbBlue = ((*rgb555 & 0x1F) * 0xFF) / 0x1F;
									rgb555++;
								}
							}
							break;

							case 24:
							{
								RGBTRIPLE *rgb = (RGBTRIPLE*)&cmap[0];
								for (count = header.cm_first_entry; count < header.cm_length; count++) {						
									palette[count].rgbBlue  = rgb->rgbtBlue;									
									palette[count].rgbGreen = rgb->rgbtGreen;
									palette[count].rgbRed   = rgb->rgbtRed;
									rgb++;
								}
							}
							break;

							case 32:
							{
								BYTE trns[256];

								// clear the transparency table
								memset(trns, 0xFF, 256);

								BGRAQUAD *quad = (BGRAQUAD*)&cmap[0];
								for (count = header.cm_first_entry; count < header.cm_length; count++) {						
									palette[count].rgbBlue  = quad->bgraBlue;									
									palette[count].rgbGreen = quad->bgraGreen;
									palette[count].rgbRed   = quad->bgraRed;
									// alpha
									trns[count] = quad->bgraAlpha;
									quad++;
								}

								// set the tranparency table
								FreeImage_SetTransparencyTable(dib, trns, 256);
							}
							break;
						}

						free(cmap);
					}
					
					// read in the bitmap bits

					switch (header.image_type) {
						case TGA_CMAP:
						case TGA_MONO:
						{
							BYTE *bits = NULL;

							if (fliphoriz) {
								for (unsigned count = 0; count < header.is_height; count++) {
									bits = Internal_GetScanLine(dib, header.is_height - count - 1, flipvert);
									io->read_proc(bits, sizeof(BYTE), line, handle);
								}
							}
							else {
								for (unsigned count = 0; count < header.is_height; count++) {
									bits = Internal_GetScanLine(dib, count, flipvert);
									io->read_proc(bits, sizeof(BYTE), line, handle);
								}
							}
							
							break;
						}

						case TGA_RLECMAP:
						case TGA_RLEMONO:
						{
							int x = 0;
							int y = 0;
							BYTE rle = 0;							
							BYTE *bits;
							
							if (fliphoriz)
								bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
							else
								bits = Internal_GetScanLine(dib, y, flipvert);

							while(1) {
								io->read_proc(&rle,1, 1, handle);
								
								if (rle>127) {
									rle -= 127;

									BYTE triple;

									io->read_proc(&triple, 1, 1, handle);

									for (int ix = 0; ix < rle; ix++) {
										bits[x++] = triple;

										if (x >= line) {
											x = 0;

											y++;

											if (y >= header.is_height)
												goto done89;
											
											if (fliphoriz)
												bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
											else
												bits = Internal_GetScanLine(dib, y, flipvert);
										}
									}
								} else {
									rle++;

									for (int ix = 0; ix < rle; ix++) {
										BYTE triple;		

										io->read_proc(&triple, 1, 1, handle);

										bits[x++] = triple;
										
										if (x >= line) {
											x = 0;

											y++;

											if (y >= header.is_height)
												goto done89;											
											
											if (fliphoriz)
												bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
											else
												bits = Internal_GetScanLine(dib, y, flipvert);
										}
									}
								}
							}
					done89 :
							break;
						}
						
						default :
							FreeImage_Unload(dib);
							return NULL;
					}

					break;
				}

				case 15 :
				case 16 :
				{
					int pixel_bits;

					// allocate the dib

					if (TARGA_LOAD_RGB888 & flags) {
						pixel_bits = 24;

						dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, 0xFF, 0xFF00, 0xFF0000);
					} else {			
						pixel_bits = 16;

						dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, 0x1F, 0x3E0, 0x7C00);
					}

					if (dib == NULL)
						throw "DIB allocation failed";

					int line = CalculateLine(header.is_width, pixel_bits);

					const unsigned pixel_size = unsigned(pixel_bits) / 8;

					// note header.cm_size is a misleading name, it should be seen as header.cm_bits 
					// ignore current position in file and set filepointer explicitly from the beginning of the file

					int garblen = 0;

					if (header.color_map_type != 0)
						garblen = (int)((header.cm_size + 7) / 8) * header.cm_length; /* should byte align */
					else
						garblen = 0;

					io->seek_proc(handle, start_offset, SEEK_SET);
					io->seek_proc(handle, sizeof(tagTGAHEADER) + header.id_length + garblen, SEEK_SET);

					// read in the bitmap bits

					WORD pixel;
							
					switch (header.image_type) {
						case TGA_RGB:
						{
							for (int y = 0; y < header.is_height; y++) {
								BYTE *bits;
								
								if (fliphoriz)
									bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
								else
									bits = Internal_GetScanLine(dib, y, flipvert);

								for (int x = 0; x < line; ) {
									io->read_proc(&pixel, sizeof(WORD), 1, handle);
								
									if (TARGA_LOAD_RGB888 & flags) {
										bits[x + 0] = ((pixel & 0x1F) * 0xFF) / 0x1F;
										bits[x + 1] = (((pixel & 0x3E0) >> 5) * 0xFF) / 0x1F;
										bits[x + 2] = (((pixel & 0x7C00) >> 10) * 0xFF) / 0x1F;
									} else {
										*reinterpret_cast<WORD*>(bits + x) = 0x7FFF & pixel;
									}

									x += pixel_size;
								}
							}

							break;
						}

						case TGA_RLERGB:
						{
							int x = 0;
							int y = 0;
							BYTE rle;
							WORD pixel;

							while(1) {
								BYTE *bits;
								
								if (fliphoriz)
									bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
								else
									bits = Internal_GetScanLine(dib, y, flipvert);
		
								io->read_proc(&rle,1, 1, handle);
								
								// compressed block
								
								if (rle > 127) {
									rle -= 127;

									io->read_proc(&pixel, sizeof(WORD), 1, handle);
								
									for (int ix = 0; ix < rle; ix++) {
										if (TARGA_LOAD_RGB888 & flags) {
											bits[x + 0] = ((pixel & 0x1F) * 0xFF) / 0x1F;
											bits[x + 1] = (((pixel & 0x3E0) >> 5) * 0xFF) / 0x1F;
											bits[x + 2] = (((pixel & 0x7C00) >> 10) * 0xFF) / 0x1F;
										} else {
											*reinterpret_cast<WORD *>(bits + x) = 0x7FFF & pixel;
										}

										x += pixel_size;
										
										if (x >= line) {
											x = 0;
											y++;

											if (y >= header.is_height)
												goto done2;																
										}
									}
								} else {
									rle++;

									for (int ix = 0; ix < rle; ix++) {
										io->read_proc(&pixel, sizeof(WORD), 1, handle);

										if (TARGA_LOAD_RGB888 & flags) {
											bits[x + 0] = ((pixel & 0x1F) * 0xFF) / 0x1F;
											bits[x + 1] = (((pixel & 0x3E0) >> 5) * 0xFF) / 0x1F;
											bits[x + 2] = (((pixel & 0x7C00) >> 10) * 0xFF) / 0x1F;
										} else {
											*reinterpret_cast<WORD*>(bits + x) = 0x7FFF & pixel;
										}

										x += pixel_size;

										if (x >= line) {
											x = 0;
											y++;

											if (y >= header.is_height)
												goto done2;																
										}
									}
								}
							}

					done2 :
							break;
						}

						default :
							FreeImage_Unload(dib);
							return NULL;
					}

					break;
				}

				case 24 :
				{
					dib = FreeImage_Allocate(header.is_width, header.is_height, 24, 0xFF, 0xFF00, 0xFF0000);

					if (dib == 0)
						throw "DIB allocation failed";					

					// read in the bitmap bits

					switch (header.image_type) {
						case TGA_RGB:
						{
							if (fliphoriz)
								for (unsigned count = header.is_height; count > 0; count--) {
									BYTE *bits = bits = Internal_GetScanLine(dib, count-1, flipvert);

									io->read_proc(bits, line, 1, handle);

									bits += pitch;
								}
							else
								for (unsigned count = 0; count < header.is_height; count++) {
									BYTE *bits = bits = Internal_GetScanLine(dib, count, flipvert);

									io->read_proc(bits, line, 1, handle);

									bits += pitch;
								}

							break;
						}

						case TGA_RLERGB:
						{
							int x = 0;
							int y = 0;
							BYTE rle;
							BYTE *bits;
							
							if (fliphoriz)
								bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
							else
								bits = Internal_GetScanLine(dib, y, flipvert);
							
							if (alphabits) {
								while(1) {
									io->read_proc(&rle,1, 1, handle);
									
									if (rle>127) {
										rle -= 127;

										BGRAQUAD quad;

										io->read_proc(&quad, sizeof(BGRAQUAD), 1, handle);

										for (int ix = 0; ix < rle; ix++) {
											bits[x++] = quad.bgraBlue;
											bits[x++] = quad.bgraGreen;
											bits[x++] = quad.bgraRed;
											bits[x++] = quad.bgraAlpha;

											if (x >= line) {
												x = 0;
												y++;

												if (y >= header.is_height)
													goto done243;

												if (fliphoriz)
													bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
												else
													bits = Internal_GetScanLine(dib, y, flipvert);
											}
										}
									} else {
										rle++;

										for (int ix = 0; ix < rle; ix++) {
											BGRAQUAD quad;

											io->read_proc(&quad, sizeof(BGRAQUAD), 1, handle);

											bits[x++] = quad.bgraBlue;
											bits[x++] = quad.bgraGreen;
											bits[x++] = quad.bgraRed;
											bits[x++] = quad.bgraAlpha;
											
											if (x >= line) {
												x = 0;
												y++;

												if (y >= header.is_height)
													goto done243;											

												if (fliphoriz)
													bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
												else
													bits = Internal_GetScanLine(dib, y, flipvert);
											}
										}
									}
								}
							} else {
								while (1) {
									io->read_proc(&rle,1, 1, handle);
									
									if (rle>127) {
										rle -= 127;

										RGBTRIPLE triple;

										io->read_proc(&triple, sizeof(RGBTRIPLE), 1, handle);

										for (int ix = 0; ix < rle; ix++) {
											bits[x++] = triple.rgbtBlue;
											bits[x++] = triple.rgbtGreen;
											bits[x++] = triple.rgbtRed;

											if (x >= line) {
												x = 0;
												y++;

												if (y >= header.is_height)
													goto done243;											
												
												if (fliphoriz)
													bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
												else
													bits = Internal_GetScanLine(dib, y, flipvert);
											}
										}
									} else {
										rle++;

										for (int ix = 0; ix < rle; ix++) {
											RGBTRIPLE triple;		

											io->read_proc(&triple, sizeof(RGBTRIPLE), 1, handle);

											bits[x++] = triple.rgbtBlue;
											bits[x++] = triple.rgbtGreen;
											bits[x++] = triple.rgbtRed;
											
											if (x >= line) {
												x = 0;
												y++;

												if (y >= header.is_height)
													goto done243;											

												if (fliphoriz)
													bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
												else
													bits = Internal_GetScanLine(dib, y, flipvert);
											}
										}
									}
								}
							}
					done243 :
							break;
						}

						default :
							FreeImage_Unload(dib);
							return NULL;
					}

					break;
				}
				
				case 32 :
				{
					int pixel_bits;

					if (TARGA_LOAD_RGB888 & flags) {
						pixel_bits = 24;

						line = CalculateLine(header.is_width, pixel_bits);
						pitch = CalculatePitch(line);
					} else {
						pixel_bits = 32;
					}

					const unsigned pixel_size = unsigned (pixel_bits) / 8;

					// Allocate the DIB

					dib = FreeImage_Allocate(header.is_width, header.is_height, pixel_bits, 0xFF, 0xFF00, 0xFF0000);
					
					if (dib == 0)
						throw "DIB allocation failed";					

					// read in the bitmap bits

					switch (header.image_type) {
						case TGA_RGB:
						{
							// uncompressed

							if (alphabits) {
								if (fliphoriz)
									for (unsigned count = header.is_height; count > 0; count--) {
										BYTE *bits = bits = Internal_GetScanLine(dib, count-1, flipvert);

										for (unsigned cols = 0; cols < header.is_width; cols++) {
											RGBQUAD rgb;

											io->read_proc(&rgb, sizeof(RGBQUAD), 1, handle);

											bits[0] = rgb.rgbBlue;
											bits[1] = rgb.rgbGreen;
											bits[2] = rgb.rgbRed;

											if ((TARGA_LOAD_RGB888 & flags) != TARGA_LOAD_RGB888)
												bits[3] = rgb.rgbReserved;											

											bits += pixel_size;
										}
									}
								else
									for (unsigned count = 0; count < header.is_height; count++) {
										BYTE *bits = Internal_GetScanLine(dib, count, flipvert);

										for (unsigned cols = 0; cols < header.is_width; cols++) {
											RGBQUAD rgb;

											io->read_proc(&rgb, sizeof(RGBQUAD), 1, handle);

											bits[0] = rgb.rgbBlue;
											bits[1] = rgb.rgbGreen;
											bits[2] = rgb.rgbRed;

											if ((TARGA_LOAD_RGB888 & flags) != TARGA_LOAD_RGB888)
												bits[3] = rgb.rgbReserved;											

											bits += pixel_size;
										}
									}

							} else {

								for (unsigned count = 0; count < header.is_height; count++) {
									BYTE *bits;

									if (fliphoriz)
										bits = Internal_GetScanLine(dib, header.is_height - count - 1, flipvert);
									else
										bits = Internal_GetScanLine(dib, count, flipvert);

									for (unsigned cols = 0; cols < header.is_width; cols++) {
										RGBQUAD rgb;

										io->read_proc(&rgb, sizeof(RGBQUAD), 1, handle);

										bits[0] = rgb.rgbBlue;
										bits[1] = rgb.rgbGreen;
										bits[2] = rgb.rgbRed;

										if ((TARGA_LOAD_RGB888 & flags) != TARGA_LOAD_RGB888)
											bits[3] = rgb.rgbReserved;											

										bits += pixel_size;
									}
								}
							}

							break;
						}
						case TGA_RLERGB:
						{
							int x = 0;
							int y = 0;
							BYTE rle;
							BYTE *bits;
							
							if (fliphoriz)
								bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
							else
								bits = Internal_GetScanLine(dib, y, flipvert);
							
							while(1) {
								io->read_proc(&rle,1, 1, handle);
									
								if (rle>127) {
									rle -= 127;

									BGRAQUAD quad;

									io->read_proc(&quad, sizeof(BGRAQUAD), 1, handle);

									for (int ix = 0; ix < rle; ix++) {
										bits[x++] = quad.bgraBlue;
										bits[x++] = quad.bgraGreen;
										bits[x++] = quad.bgraRed;
										bits[x++] = quad.bgraAlpha;

										if (x >= line) {
											x = 0;
											y++;

											if (y >= header.is_height)
												goto done3210;

											if (fliphoriz)
												bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
											else
												bits = Internal_GetScanLine(dib, y, flipvert);
										}
									}
								} else {
									rle++;

									for (int ix = 0; ix < rle; ix++) {
										BGRAQUAD quad;

										io->read_proc(&quad, sizeof(BGRAQUAD), 1, handle);

										bits[x++] = quad.bgraBlue;
										bits[x++] = quad.bgraGreen;
										bits[x++] = quad.bgraRed;
										bits[x++] = quad.bgraAlpha;
											
										if (x >= line) {
											x = 0;
											y++;

											if (y >= header.is_height)
												goto done3210;											

											if (fliphoriz)
												bits = Internal_GetScanLine(dib, header.is_height - y - 1, flipvert);
											else
												bits = Internal_GetScanLine(dib, y, flipvert);
										}
									}
								}
							}

					done3210 :
							break;
						}

						default :
							FreeImage_Unload(dib);
							return NULL;
					}

					break;
				}
			}

			return (FIBITMAP *)dib;

		} catch(const char *message) {
			FreeImage_OutputMessageProc(s_format_id, message);

			return NULL;
		}
	}

	return NULL;
}

static BOOL DLL_CALLCONV
Save(FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	if ((dib != NULL) && (handle != NULL)) {
		RGBQUAD *palette = FreeImage_GetPalette(dib);
		int bpp = FreeImage_GetBPP(dib);

		// write the file header

		TGAHEADER header;
		header.id_length = 0;
		header.cm_first_entry = 0;
		header.is_xorigin = 0;
		header.is_yorigin = 0;
		header.is_width = FreeImage_GetWidth(dib);
		header.is_height = FreeImage_GetHeight(dib);
		header.is_pixel_depth = bpp;
		header.is_image_descriptor = 0;

		if (palette) {
			header.color_map_type = 1;
			header.image_type = TGA_CMAP;
			header.cm_length = 1 << bpp;
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

		io->write_proc(&header, sizeof(header), 1, handle);

		// write the palette	

		if (palette) {
			if(FreeImage_IsTransparent(dib)) {
				BGRAQUAD *rgba_pal = (BGRAQUAD*)malloc(header.cm_length * sizeof(BGRAQUAD));

				// get the transparency table
				BYTE *trns = FreeImage_GetTransparencyTable(dib);

				for(int i = 0; i < header.cm_length; i++) {
					rgba_pal[i].bgraBlue  = palette[i].rgbBlue;
					rgba_pal[i].bgraGreen = palette[i].rgbGreen;
					rgba_pal[i].bgraRed   = palette[i].rgbRed;
					rgba_pal[i].bgraAlpha = trns[i];
				}

				io->write_proc(rgba_pal, sizeof(BGRAQUAD), header.cm_length, handle);

				free(rgba_pal);

			} else {
				RGBTRIPLE *rgb_pal = (RGBTRIPLE*)malloc(header.cm_length * sizeof(RGBTRIPLE));

				for(int i = 0; i < header.cm_length; i++) {
					rgb_pal[i].rgbtBlue  = palette[i].rgbBlue;
					rgb_pal[i].rgbtGreen = palette[i].rgbGreen;
					rgb_pal[i].rgbtRed   = palette[i].rgbRed;
				}

				io->write_proc(rgb_pal, sizeof(RGBTRIPLE), header.cm_length, handle);

				free(rgb_pal);
			}
		}

		// write the data bits

		int line = FreeImage_GetLine(dib);

		for (int i = 0; i < header.is_height; ++i) {
			BYTE *bits = FreeImage_GetScanLine(dib, i);
			io->write_proc(bits, line, 1, handle);
		}

		// write the TARGA signature

		TGAFOOTER footer;
		footer.developer_offset = 0;
		footer.extension_offset = 0;
		strcpy(footer.signature, "TRUEVISION-XFILE.");

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
