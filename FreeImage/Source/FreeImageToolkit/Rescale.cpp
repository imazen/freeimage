// ==========================================================
// Upsampling / downsampling routine
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

#include "Resize.h"

FIBITMAP * DLL_CALLCONV 
FreeImage_Rescale(FIBITMAP *src, int dst_width, int dst_height, FREE_IMAGE_FILTER filter) {
	int x, y, bpp;
	int channel, nb_channels;
	BYTE *src_bits, *dst_bits;
	FIBITMAP *src8 = NULL, *dst8 = NULL, *dst = NULL;

	if (!src || (dst_width <= 0) || (dst_height <= 0)) {
		return NULL;
	}

	// select the filter
	CGenericFilter *pFilter = NULL;
	switch(filter) {
		case FILTER_BOX:
			pFilter = new CBoxFilter();
			break;
		case FILTER_BICUBIC:
			pFilter = new CBicubicFilter();
			break;
		case FILTER_BILINEAR:
			pFilter = new CBilinearFilter();
			break;
		case FILTER_BSPLINE:
			pFilter = new CBSplineFilter();
			break;
		case FILTER_CATMULLROM:
			pFilter = new CCatmullRomFilter();
			break;
		case FILTER_LANCZOS3:
			pFilter = new CLanczos3Filter();
			break;
	}

	CResizeEngine Engine(pFilter);

	try {

		bpp = FreeImage_GetBPP(src);

		if(bpp == 8) {
			dst8 = Engine.scale(src, dst_width, dst_height);
			if(!dst8) throw(1);
			
			// buid a greyscale palette			
			RGBQUAD *dst_pal = FreeImage_GetPalette(dst8);
			for(int i = 0; i < 256; i++) {
				dst_pal[i].rgbRed = dst_pal[i].rgbGreen = dst_pal[i].rgbBlue = (BYTE)i;
			}

			delete pFilter;
			
			return dst8;
		}
		if((bpp == 24) || (bpp == 32)) {
			// allocate a temporary 8-bit dib (no need to build a palette)
			int width  = FreeImage_GetWidth(src);
			int height = FreeImage_GetHeight(src);

			src8 = FreeImage_Allocate(width, height, 8);
			if(!src8) throw(1);

			// process each channel separately
			// -------------------------------
			nb_channels = (bpp / 8);

			for(channel = 0; channel < nb_channels; channel++) {
				// extract channel from source dib
				for(y = 0; y < height; y++) {
					src_bits = FreeImage_GetScanLine(src, y);
					dst_bits = FreeImage_GetScanLine(src8, y);
					for(x = 0; x < width; x++) {
						dst_bits[x] = src_bits[channel];
						src_bits += nb_channels;
					}
				}

				// process channel
				dst8 = Engine.scale(src8, dst_width, dst_height);
				if(!dst8) throw(1);

				if(!dst) {
					// allocate dst image
					dst = FreeImage_Allocate(dst_width, dst_height, bpp, 0xFF, 0xFF00, 0xFF0000);
					if(!dst) throw(1);
				}
				// insert channel to destination dib
				for(y = 0; y < dst_height; y++) {
					src_bits = FreeImage_GetScanLine(dst8, y);
					dst_bits = FreeImage_GetScanLine(dst, y);
					for(x = 0; x < dst_width; x++) {
						dst_bits[channel] = src_bits[x];
						dst_bits += nb_channels;
					}
				}

				FreeImage_Unload(dst8);
			}

			FreeImage_Unload(src8);

			delete pFilter;

			return dst;
		}
	} catch(int) {
		delete pFilter;

		if(src8) FreeImage_Unload(src8);
		if(dst8) FreeImage_Unload(dst8);
		if(dst)  FreeImage_Unload(dst);
	}

	return NULL;
}

