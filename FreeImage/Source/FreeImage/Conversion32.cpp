// ==========================================================
// Bitmap conversion routines
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Hervé Drolon (drolon@infonie.fr)
// - Jani Kajala (janik@remedy.fi)
// - Detlev Vendt (detlev.vendt@brillit.de)
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
//  internal conversions X to 32 bits
// ----------------------------------------------------------

void DLL_CALLCONV
FreeImage_ConvertLine1To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		int index = (source[cols>>3] & (0x80 >> (cols & 0x07))) != 0 ? 1 : 0;

		target[0] = palette[index].rgbBlue;
		target[1] = palette[index].rgbGreen;
		target[2] = palette[index].rgbRed;
		target[3] = 0xff;
		target += 4;
	}	
}

void DLL_CALLCONV
FreeImage_ConvertLine4To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	BOOL low_nibble = FALSE;
	int x = 0;

	for (int cols = 0 ; cols < width_in_pixels ; ++cols) {
		if (low_nibble) {
			target[0] = palette[LOWNIBBLE(source[x])].rgbBlue;
			target[1] = palette[LOWNIBBLE(source[x])].rgbGreen;
			target[2] = palette[LOWNIBBLE(source[x])].rgbRed;

			x++;
		} else {
			target[0] = palette[HINIBBLE(source[x]) >> 4].rgbBlue;
			target[1] = palette[HINIBBLE(source[x]) >> 4].rgbGreen;
			target[2] = palette[HINIBBLE(source[x]) >> 4].rgbRed;
		}

		low_nibble = !low_nibble;

		target[3] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine8To32(BYTE *target, BYTE *source, int width_in_pixels, RGBQUAD *palette) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[0] = palette[source[cols]].rgbBlue;
		target[1] = palette[source[cols]].rgbGreen;
		target[2] = palette[source[cols]].rgbRed;
		target[3] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine16To32_555(BYTE *target, BYTE *source, int width_in_pixels) {
	WORD *bits = (WORD *)source;

	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[2] = (((bits[cols] & 0x7C00) >> 10) * 0xFF) / 0x1F;
		target[1] = (((bits[cols] & 0x3E0) >> 5) * 0xFF) / 0x1F;
		target[0] = ((bits[cols] & 0x1F) * 0xFF) / 0x1F;
		target[3] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine16To32_565(BYTE *target, BYTE *source, int width_in_pixels) {
	WORD *bits = (WORD *)source;

	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[2] = ((bits[cols] & 0xF800) >> 11) << 3;
		target[1] = ((bits[cols] & 0x7E0) >> 5) << 2;
		target[0] = (bits[cols] & 0x1F) << 3;
		target[3] = 0xff;
		target += 4;
	}
}

void DLL_CALLCONV
FreeImage_ConvertLine24To32(BYTE *target, BYTE *source, int width_in_pixels) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		*(DWORD *)target = (*(DWORD *) source & 0x00FFFFFF) | 0xFF000000;

		target += 4;
		source += 3;
	}
}

// ----------------------------------------------------------

static void DLL_CALLCONV
MapTransparentTableToAlpha(RGBQUAD *target, BYTE *source, BYTE *table, int transparent_pixels, int width_in_pixels) {
	for (int cols = 0; cols < width_in_pixels; cols++) {
		target[cols].rgbReserved = (source[cols] < transparent_pixels) ? table[source[cols]] : 255;
	}
}

// ----------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_ConvertTo32Bits(FIBITMAP *dib) {
	if(!dib) return NULL;

	int bpp = FreeImage_GetBPP(dib);

	if(bpp != 32) {
		int width = FreeImage_GetWidth(dib);
		int height = FreeImage_GetHeight(dib);

		switch(bpp) {
			case 1 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, 0xFF, 0xFF00, 0xFF0000);

				if (new_dib != NULL)
					for (int rows = 0; rows < height; rows++)
						FreeImage_ConvertLine1To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));
				
				return new_dib;
			}

			case 4 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, 0xFF, 0xFF00, 0xFF0000);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						FreeImage_ConvertLine4To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));
				
						if (FreeImage_IsTransparent(dib)) {
							MapTransparentTableToAlpha((RGBQUAD *)FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), FreeImage_GetTransparencyTable(dib), FreeImage_GetTransparencyCount(dib), width);
						}
					}

					return new_dib;
				}
			}
				
			case 8 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, 0xFF, 0xFF00, 0xFF0000);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						FreeImage_ConvertLine8To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width, FreeImage_GetPalette(dib));

						if (FreeImage_IsTransparent(dib)) {
							MapTransparentTableToAlpha((RGBQUAD *)FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), FreeImage_GetTransparencyTable(dib), FreeImage_GetTransparencyCount(dib), width);
						}
					}

					return new_dib;
				}
			}

			case 16 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, 0xFF, 0xFF00, 0xFF0000);

				if (new_dib != NULL) {
					for (int rows = 0; rows < height; rows++) {
						if ((FreeImage_GetRedMask(dib) == 0x1F) && (FreeImage_GetGreenMask(dib) == 0x7E0) && (FreeImage_GetBlueMask(dib) == 0xF800)) {
							FreeImage_ConvertLine16To32_565(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
						} else {
							// includes case where all the masks are 0
							FreeImage_ConvertLine16To32_555(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
						}
					}
				}

				return new_dib;
			}

			case 24 :
			{
				FIBITMAP *new_dib = FreeImage_Allocate(width, height, 32, 0xFF, 0xFF00, 0xFF0000);

				if (new_dib != NULL)
					for (int rows = 0; rows < height; rows++)
						FreeImage_ConvertLine24To32(FreeImage_GetScanLine(new_dib, rows), FreeImage_GetScanLine(dib, rows), width);
				
				return new_dib;
			}
		}
	}

	return FreeImage_Clone(dib);
}
