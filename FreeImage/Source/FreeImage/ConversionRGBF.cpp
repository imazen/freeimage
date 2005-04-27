// ==========================================================
// Bitmap conversion routines
//
// Design and implementation by
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
//   smart convert X to RGBF
// ----------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_ConvertToRGBF(FIBITMAP *dib) {
	unsigned x, y;
	FIBITMAP *src = NULL;
	FIBITMAP *dst = NULL;

	FREE_IMAGE_TYPE src_type = FreeImage_GetImageType(dib);

	FREE_IMAGE_COLOR_TYPE color_type;

	// check for allowed conversions 
	switch(src_type) {
		case FIT_BITMAP:
			// allow conversion from 24- and 32-bit
			color_type = FreeImage_GetColorType(dib);
			if((color_type != FIC_RGB) && (color_type != FIC_RGBALPHA)) {
				src = FreeImage_ConvertTo24Bits(dib);
				if(!src) return NULL;
			} else {
				src = dib;
			}
			break;
		case FIT_RGB16:
			// allow conversion from 48-bit
			src = dib;
			break;
		case FIT_RGBF:
			// RGBF type : clone the src
			return FreeImage_Clone(dib);
			break;
		default:
			return NULL;
	}

	// allocate dst image

	unsigned width	= FreeImage_GetWidth(src);
	unsigned height = FreeImage_GetHeight(src);

	dst = FreeImage_AllocateT(FIT_RGBF, width, height);
	if(!dst) return NULL;

	// convert from src type to RGBF

	unsigned src_pitch = FreeImage_GetPitch(src);
	unsigned dst_pitch = FreeImage_GetPitch(dst);

	switch(src_type) {
		case FIT_BITMAP:
		{
			// calculate the number of bytes per pixel (3 for 24-bit or 4 for 32-bit)
			unsigned bytespp = FreeImage_GetLine(src) / FreeImage_GetWidth(src);

			BYTE *src_bits = (BYTE*)FreeImage_GetBits(src);
			BYTE *dst_bits = (BYTE*)FreeImage_GetBits(dst);

			for(y = 0; y < height; y++) {
				BYTE   *src_pixel = (BYTE*)src_bits;
				FIRGBF *dst_pixel = (FIRGBF*)dst_bits;
				for(x = 0; x < width; x++) {
					dst_pixel->red   = (float)(src_pixel[FI_RGBA_RED]);
					dst_pixel->green = (float)(src_pixel[FI_RGBA_GREEN]);
					dst_pixel->blue  = (float)(src_pixel[FI_RGBA_BLUE]);

					src_pixel += bytespp;
					dst_pixel += sizeof(FIRGBF);
				}
				src_bits += src_pitch;
				dst_bits += dst_pitch;
			}
		}
		break;

		case FIT_RGB16:
		{
			BYTE *src_bits = (BYTE*)FreeImage_GetBits(src);
			BYTE *dst_bits = (BYTE*)FreeImage_GetBits(dst);

			for(y = 0; y < height; y++) {
				FIRGB16 *src_pixel = (FIRGB16*) src_bits;
				FIRGBF  *dst_pixel = (FIRGBF*)  dst_bits;

				for(x = 0; x < width; x++) {
					dst_pixel[x].red   = (float)(src_pixel[x].red);
					dst_pixel[x].green = (float)(src_pixel[x].green);
					dst_pixel[x].blue  = (float)(src_pixel[x].blue);
				}
				src_bits += src_pitch;
				dst_bits += dst_pitch;
			}
		}
		break;
	}

	if(src != dib) {
		FreeImage_Unload(src);
	}

	return dst;
}

