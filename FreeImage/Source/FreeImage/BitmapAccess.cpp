// ==========================================================
// FreeImage implementation
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Hervé Drolon (drolon@infonie.fr)
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

#pragma warning (disable : 4786)

#include <stdlib.h>

#include "FreeImage.h"
#include "FreeImageIO.h"
#include "Utilities.h"

#include "../DeprecationManager/DeprecationMgr.h"

// ----------------------------------------------------------

FI_STRUCT (FREEIMAGEHEADER) {
    FREE_IMAGE_TYPE type;		// data type - bitmap, array of long, double, complex, etc

	unsigned red_mask;			// bit layout of the red components
	unsigned green_mask;		// bit layout of the green components
	unsigned blue_mask;			// bit layout of the blue components

	RGBQUAD bkgnd_color;		// background color used for RGB transparency

	BOOL transparent;			 // why another table? for easy transparency table retrieval!
	int  transparency_count;	 // transparency could be stored in the palette, which is better
	BYTE transparent_table[256]; // overall, but it requires quite some changes and it will render
								 // FreeImage_GetTransparencyTable obsolete in its current form;
	FIICCPROFILE iccProfile;	 // space to hold ICC profile
	//BYTE filler[1];              
};

// ----------------------------------------------------------
//  Internal
// ----------------------------------------------------------

int
FreeImage_GetFreeImageHeaderSize() {
	return sizeof(FREEIMAGEHEADER);
}

// ----------------------------------------------------------
//  DIB information functions
// ----------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_Allocate(int width, int height, int bpp, unsigned red_mask, unsigned green_mask, unsigned blue_mask) {
	return FreeImage_AllocateT(FIT_BITMAP, width, height, bpp, red_mask, green_mask, blue_mask);
}

FIBITMAP * DLL_CALLCONV
FreeImage_AllocateT(FREE_IMAGE_TYPE type, int width, int height, int bpp, unsigned red_mask, unsigned green_mask, unsigned blue_mask) {
	FIBITMAP *bitmap = (FIBITMAP *)malloc(sizeof(FIBITMAP));

	if (bitmap != NULL) {
		height = abs(height);

		// check pixel bit depth
		switch(type) {
			case FIT_BITMAP:
				switch(bpp) {
					case 1:
					case 4:
					case 8:
					case 16:
					case 24:
					case 32:
						break;
					default:
						bpp = 8;
						break;
				}
				break;
			case FIT_UINT16:
				bpp = 8 * sizeof(unsigned short);
				break;
			case FIT_INT16:
				bpp = 8 * sizeof(short);
				break;
			case FIT_UINT32:
				bpp = 8 * sizeof(unsigned long);
				break;
			case FIT_INT32:
				bpp = 8 * sizeof(long);
				break;
			case FIT_FLOAT:
				bpp = 8 * sizeof(float);
				break;
			case FIT_DOUBLE:
				bpp = 8 * sizeof(double);
				break;
			case FIT_COMPLEX:
				bpp = 8 * sizeof(FICOMPLEX);
				break;
			default:
				free(bitmap);
				return NULL;
		}

		// calculate the size of a FreeImage image
		unsigned dib_size = sizeof(FREEIMAGEHEADER);
		dib_size += sizeof(BITMAPINFOHEADER);
		dib_size += sizeof(RGBQUAD) * CalculateUsedPaletteEntries(bpp);
		dib_size += CalculatePitch(CalculateLine(width, bpp)) * height;

		bitmap->data = (BYTE *)malloc(dib_size * sizeof(BYTE));

		if (bitmap->data != NULL) {
			memset(bitmap->data, 0, dib_size);

			// write out the FREEIMAGEHEADER

			FREEIMAGEHEADER *fih    = (FREEIMAGEHEADER *)bitmap->data;
			fih->type				= type;

			fih->red_mask           = red_mask;
			fih->green_mask         = green_mask;
			fih->blue_mask          = blue_mask;

			memset(&fih->bkgnd_color, 0, sizeof(RGBQUAD));

			fih->transparent        = FALSE;
			fih->transparency_count = 0;
			memset(fih->transparent_table, 0xff, 256);

			// initialize FIICCPROFILE link

			FIICCPROFILE *iccProfile = FreeImage_GetICCProfile(bitmap);
			iccProfile->size		= 0;
			iccProfile->data		= 0;
			iccProfile->flags		= 0;

			// write out the BITMAPINFOHEADER

			BITMAPINFOHEADER *bih   = FreeImage_GetInfoHeader(bitmap);
			bih->biSize             = sizeof(BITMAPINFOHEADER);
			bih->biWidth            = width;
			bih->biHeight           = height;
			bih->biPlanes           = 1;
			bih->biCompression      = 0;
			bih->biBitCount         = bpp;
			bih->biClrUsed          = CalculateUsedPaletteEntries(bpp);
			bih->biClrImportant     = bih->biClrUsed;

			return bitmap;
		}

		free(bitmap);
	}

	return NULL;
}

void DLL_CALLCONV
FreeImage_Unload(FIBITMAP *dib) {
	if (NULL != dib) {	// delete bitmap and possible icc profile ...
		if (NULL != dib->data) {
			if (FreeImage_GetICCProfile(dib)->data)
				free(FreeImage_GetICCProfile(dib)->data);
			free(dib->data);
			dib->data = NULL;
		}
		free(dib);		// ... and the wrapper
	}
}

// ----------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_Clone(FIBITMAP *dib) {
	if(!dib) return NULL;
	
	FIBITMAP *new_dib = FreeImage_AllocateT(FreeImage_GetImageType(dib), 
		FreeImage_GetWidth(dib), FreeImage_GetHeight(dib), FreeImage_GetBPP(dib), 
			FreeImage_GetRedMask(dib), FreeImage_GetGreenMask(dib), FreeImage_GetBlueMask(dib));

	if (new_dib) {
		memcpy(new_dib->data, dib->data, 
			sizeof(FREEIMAGEHEADER) + FreeImage_GetDIBSize(dib));
		if (FreeImage_GetICCProfile(dib)->data &&
			FreeImage_GetICCProfile(dib)->size) {
			if (FreeImage_GetICCProfile(new_dib)->data = malloc(FreeImage_GetICCProfile(dib)->size))
				memcpy(FreeImage_GetICCProfile(new_dib)->data, 
					   FreeImage_GetICCProfile(dib)->data, 
					   FreeImage_GetICCProfile(dib)->size);
		}
		return new_dib;
	}

	return NULL;
}

// ----------------------------------------------------------

FREE_IMAGE_COLOR_TYPE DLL_CALLCONV
FreeImage_GetColorType(FIBITMAP *dib) {
	RGBQUAD *rgb;

	// special bitmap type
	if(FreeImage_GetImageType(dib) != FIT_BITMAP) {
		return FIC_MINISBLACK;
	}

	// standard image type
	switch (FreeImage_GetBPP(dib)) {
		case 1:
		{
			rgb = FreeImage_GetPalette(dib);

			if ((rgb->rgbRed == 0) && (rgb->rgbGreen == 0) && (rgb->rgbBlue == 0)) {
				rgb++;

				if ((rgb->rgbRed == 255) && (rgb->rgbGreen == 255) && (rgb->rgbBlue == 255))
					return FIC_MINISBLACK;				
			}

			if ((rgb->rgbRed == 255) && (rgb->rgbGreen == 255) && (rgb->rgbBlue == 255)) {
				rgb++;

				if ((rgb->rgbRed == 0) && (rgb->rgbGreen == 0) && (rgb->rgbBlue == 0))
					return FIC_MINISWHITE;				
			}

			return FIC_PALETTE;
		}

		case 4:
		case 8:	// Check if the DIB has a color or a greyscale palette
		{
			int ncolors = FreeImage_GetColorsUsed(dib);
		    int minisblack = 1;
			rgb = FreeImage_GetPalette(dib);

			for (int i = 0; i < ncolors; i++) {
				if ((rgb->rgbRed != rgb->rgbGreen) || (rgb->rgbRed != rgb->rgbBlue))
					return FIC_PALETTE;

				// The DIB has a color palette if the greyscale isn't a linear ramp
				// Take care of reversed grey images
				if (rgb->rgbRed != i) {
					if ((ncolors-i-1) != rgb->rgbRed)
						return FIC_PALETTE;
				    else
						minisblack = 0;
				}

				rgb++;
			}

			return minisblack ? FIC_MINISBLACK : FIC_MINISWHITE;
		}

		case 16:
		case 24:
			return FIC_RGB;

		case 32:
		{
			if (FreeImage_GetICCProfile(dib)->flags & FIICC_COLOR_IS_CMYK)
				return FIC_CMYK;

			for (unsigned y = 0; y < FreeImage_GetHeight(dib); y++) {
				rgb = (RGBQUAD *)FreeImage_GetScanLine(dib, y);

				for (unsigned x = 0; x < FreeImage_GetWidth(dib); x++)
					if (rgb[x].rgbReserved != 0xFF)
						return FIC_RGBALPHA;			
			}

			return FIC_RGB;
		}
				
		default :
			return FIC_MINISBLACK;
	}
}

// ----------------------------------------------------------

FREE_IMAGE_TYPE DLL_CALLCONV 
FreeImage_GetImageType(FIBITMAP *dib) {
	return (dib != NULL) ? ((FREEIMAGEHEADER *)dib->data)->type : FIT_UNKNOWN;
}

// ----------------------------------------------------------

unsigned DLL_CALLCONV
FreeImage_GetRedMask(FIBITMAP *dib) {
	return dib ? ((FREEIMAGEHEADER *)dib->data)->red_mask : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetGreenMask(FIBITMAP *dib) {
	return dib ? ((FREEIMAGEHEADER *)dib->data)->green_mask : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetBlueMask(FIBITMAP *dib) {
	return dib ? ((FREEIMAGEHEADER *)dib->data)->blue_mask : 0;
}

// ----------------------------------------------------------

BOOL DLL_CALLCONV
FreeImage_HasBackgroundColor(FIBITMAP *dib) {
	if(dib) {
		RGBQUAD *bkgnd_color = &((FREEIMAGEHEADER *)dib->data)->bkgnd_color;
		return (bkgnd_color->rgbReserved != 0) ? TRUE : FALSE;
	}
	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_GetBackgroundColor(FIBITMAP *dib, RGBQUAD *bkcolor) {
	if(dib && bkcolor) {
		if(FreeImage_HasBackgroundColor(dib)) {
			// get the background color
			RGBQUAD *bkgnd_color = &((FREEIMAGEHEADER *)dib->data)->bkgnd_color;
			memcpy(bkcolor, bkgnd_color, sizeof(RGBQUAD));
			// get the background index
			if(FreeImage_GetBPP(dib) == 8) {
				RGBQUAD *pal = FreeImage_GetPalette(dib);
				for(int i = 0; i < FreeImage_GetColorsUsed(dib); i++) {
					if(bkgnd_color->rgbRed == pal[i].rgbRed) {
						if(bkgnd_color->rgbGreen == pal[i].rgbGreen) {
							if(bkgnd_color->rgbBlue == pal[i].rgbBlue) {
								bkcolor->rgbReserved = i;
								return TRUE;
							}
						}
					}
				}
			}

			bkcolor->rgbReserved = 0;

			return TRUE;
		}
	}

	return FALSE;
}

BOOL DLL_CALLCONV 
FreeImage_SetBackgroundColor(FIBITMAP *dib, RGBQUAD *bkcolor) {
	if(dib) {
		RGBQUAD *bkgnd_color = &((FREEIMAGEHEADER *)dib->data)->bkgnd_color;
		if(bkcolor) {
			// set the background color
			memcpy(bkgnd_color, bkcolor, sizeof(RGBQUAD));
			// enable the file background color
			bkgnd_color->rgbReserved = 1;
		} else {
			// clear and disable the file background color
			memcpy(bkgnd_color, 0, sizeof(RGBQUAD));
		}
		return TRUE;
	}

	return FALSE;
}

// ----------------------------------------------------------

BOOL DLL_CALLCONV
FreeImage_IsTransparent(FIBITMAP *dib) {
	if(dib) {
		if(FreeImage_GetBPP(dib) == 32) {
			if(FreeImage_GetColorType(dib) == FIC_RGBALPHA) {
				return TRUE;
			}
		} else {
			return ((FREEIMAGEHEADER *)dib->data)->transparent ? TRUE : FALSE;
		}
	}
	return FALSE;
}

BYTE * DLL_CALLCONV
FreeImage_GetTransparencyTable(FIBITMAP *dib) {
	return dib ? ((FREEIMAGEHEADER *)dib->data)->transparent_table : NULL;
}

void DLL_CALLCONV
FreeImage_SetTransparent(FIBITMAP *dib, BOOL enabled) {
	if (dib) {
		if ((FreeImage_GetBPP(dib) == 8) || (FreeImage_GetBPP(dib) == 32)) {
			((FREEIMAGEHEADER *)dib->data)->transparent = enabled;
		} else {
			((FREEIMAGEHEADER *)dib->data)->transparent = FALSE;
		}
	}
}

unsigned DLL_CALLCONV
FreeImage_GetTransparencyCount(FIBITMAP *dib) {
	return dib ? ((FREEIMAGEHEADER *)dib->data)->transparency_count : 0;
}

void DLL_CALLCONV
FreeImage_SetTransparencyTable(FIBITMAP *dib, BYTE *table, int count) {
	if (dib) {
		if (FreeImage_GetBPP(dib) == 8) {
			((FREEIMAGEHEADER *)dib->data)->transparent = TRUE;
			((FREEIMAGEHEADER *)dib->data)->transparency_count = count;

			if (table) {
				memcpy(((FREEIMAGEHEADER *)dib->data)->transparent_table, table, count);
			} else {
				memset(((FREEIMAGEHEADER *)dib->data)->transparent_table, 0xff, count);
			}
		} 
	}
}

// ----------------------------------------------------------

FIICCPROFILE * DLL_CALLCONV
FreeImage_GetICCProfile(FIBITMAP *dib) {
	FIICCPROFILE *profile = (dib) ? (FIICCPROFILE *)&((FREEIMAGEHEADER *)dib->data)->iccProfile : NULL;
	return profile;
}

FIICCPROFILE * DLL_CALLCONV
FreeImage_CreateICCProfile(FIBITMAP *dib, void *data, long size) {
	FIICCPROFILE *profile = FreeImage_GetICCProfile(dib);
	if (profile->data) {
		free (profile->data);
	}
	memset(profile, 0, sizeof(FIICCPROFILE));
	if (size && (profile->data = malloc(size))) {
		memcpy(profile->data, data, profile->size = size);
	}
	return profile;
}

void DLL_CALLCONV
FreeImage_DestroyICCProfile(FIBITMAP *dib) {
	FIICCPROFILE *profile = FreeImage_GetICCProfile(dib);
	if (profile->data) {
		free (profile->data);
	}
	memset(profile, 0, sizeof(FIICCPROFILE));
}

// ----------------------------------------------------------

BYTE * DLL_CALLCONV
FreeImage_GetBits(FIBITMAP *dib) {
	return dib ? ((BYTE *)FreeImage_GetInfoHeader(dib) + sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD) * FreeImage_GetColorsUsed(dib)) : NULL;
}

BYTE * DLL_CALLCONV
FreeImage_GetScanLine(FIBITMAP *dib, int scanline) {
	return (dib) ? CalculateScanLine(FreeImage_GetBits(dib), FreeImage_GetPitch(dib), scanline) : NULL;
}

BOOL DLL_CALLCONV
FreeImage_GetPixelIndex(FIBITMAP *dib, unsigned x, unsigned y, BYTE *value) {
	BYTE shift;

	if(!dib || (FreeImage_GetImageType(dib) != FIT_BITMAP))
		return FALSE;

	if((x < FreeImage_GetWidth(dib)) && (y < FreeImage_GetHeight(dib))) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);

		switch(FreeImage_GetBPP(dib)) {
			case 1:
				*value = (bits[x >> 3] & (0x80 >> (x & 0x07))) != 0;
				break;
			case 4:
				shift = (BYTE)((1 - x % 2) << 2);
				*value = (bits[x >> 1] & (0x0F << shift)) >> shift;
				break;
			case 8:
				*value = bits[x];
				break;
			default:
				return FALSE;
		}

		return TRUE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_GetPixelColor(FIBITMAP *dib, unsigned x, unsigned y, RGBQUAD *value) {
	if(!dib || (FreeImage_GetImageType(dib) != FIT_BITMAP))
		return FALSE;

	if((x < FreeImage_GetWidth(dib)) && (y < FreeImage_GetHeight(dib))) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);

		switch(FreeImage_GetBPP(dib)) {
			case 24:
				bits += 3*x;
				value->rgbBlue		= bits[0];	// B
				value->rgbGreen		= bits[1];	// G
				value->rgbRed		= bits[2];	// R
				value->rgbReserved	= 0;
				break;
			case 32:
				bits += 4*x;
				value->rgbBlue		= bits[0];	// B
				value->rgbGreen		= bits[1];	// G
				value->rgbRed		= bits[2];	// R
				value->rgbReserved	= bits[3];	// A
				break;
			default:
				return FALSE;
		}

		return TRUE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_SetPixelIndex(FIBITMAP *dib, unsigned x, unsigned y, BYTE *value) {
	BYTE shift;

	if(!dib || (FreeImage_GetImageType(dib) != FIT_BITMAP))
		return FALSE;

	if((x < FreeImage_GetWidth(dib)) && (y < FreeImage_GetHeight(dib))) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);

		switch(FreeImage_GetBPP(dib)) {
			case 1:
				*value ? bits[x >> 3] |= (0x80 >> (x & 0x7)) : bits[x >> 3] &= (0xFF7F >> (x & 0x7));
				break;
			case 4:
				shift = (BYTE)((1 - x % 2) << 2);
				bits[x >> 1] |= ((*value & 0x0F) << shift);
				break;
			case 8:
				bits[x] = *value;
				break;
			default:
				return FALSE;
		}

		return TRUE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_SetPixelColor(FIBITMAP *dib, unsigned x, unsigned y, RGBQUAD *value) {
	if(!dib || (FreeImage_GetImageType(dib) != FIT_BITMAP))
		return FALSE;

	if((x < FreeImage_GetWidth(dib)) && (y < FreeImage_GetHeight(dib))) {
		BYTE *bits = FreeImage_GetScanLine(dib, y);

		switch(FreeImage_GetBPP(dib)) {
			case 24:
				bits += 3*x;
				bits[0] = value->rgbBlue;	// B
				bits[1] = value->rgbGreen;	// G
				bits[2] = value->rgbRed;	// R
				break;
			case 32:
				bits += 4*x;
				bits[0] = value->rgbBlue;		// B
				bits[1] = value->rgbGreen;		// G
				bits[2] = value->rgbRed;		// R
				bits[3] = value->rgbReserved;	// A
				break;
			default:
				return FALSE;
		}

		return TRUE;
	}

	return FALSE;
}

// ----------------------------------------------------------

unsigned DLL_CALLCONV
FreeImage_GetWidth(FIBITMAP *dib) {
	return dib ? FreeImage_GetInfoHeader(dib)->biWidth : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetHeight(FIBITMAP *dib) {
	return (dib) ? FreeImage_GetInfoHeader(dib)->biHeight : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetBPP(FIBITMAP *dib) {
	return dib ? FreeImage_GetInfoHeader(dib)->biBitCount : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetLine(FIBITMAP *dib) {
	return dib ? ((FreeImage_GetWidth(dib) * FreeImage_GetBPP(dib)) + 7) / 8 : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetPitch(FIBITMAP *dib) {
	return dib ? FreeImage_GetLine(dib) + 3 & ~3 : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetColorsUsed(FIBITMAP *dib) {
	return dib ? FreeImage_GetInfoHeader(dib)->biClrUsed : 0;
}

unsigned DLL_CALLCONV
FreeImage_GetDIBSize(FIBITMAP *dib) {
	return (dib) ? sizeof(BITMAPINFOHEADER) + (FreeImage_GetColorsUsed(dib) * sizeof(RGBQUAD)) + (FreeImage_GetPitch(dib) * FreeImage_GetHeight(dib)) : 0;
}

RGBQUAD * DLL_CALLCONV
FreeImage_GetPalette(FIBITMAP *dib) {
	return (dib && FreeImage_GetBPP(dib) < 16) ? (RGBQUAD *)(((BYTE *)FreeImage_GetInfoHeader(dib)) + sizeof(BITMAPINFOHEADER)) : NULL;
}

unsigned DLL_CALLCONV
FreeImage_GetDotsPerMeterX(FIBITMAP *dib) {
	return FreeImage_GetInfoHeader(dib)->biXPelsPerMeter;
}

unsigned DLL_CALLCONV
FreeImage_GetDotsPerMeterY(FIBITMAP *dib) {
	return (dib) ? FreeImage_GetInfoHeader(dib)->biYPelsPerMeter : 0;
}

BITMAPINFOHEADER * DLL_CALLCONV
FreeImage_GetInfoHeader(FIBITMAP *dib) {
	return (dib) ? (BITMAPINFOHEADER *)((BYTE *)dib->data + sizeof(FREEIMAGEHEADER)) : NULL;
}

BITMAPINFO * DLL_CALLCONV
FreeImage_GetInfo(FIBITMAP *dib) {
	return (BITMAPINFO *)FreeImage_GetInfoHeader(dib);
}


