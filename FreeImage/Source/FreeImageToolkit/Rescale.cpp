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
	FIBITMAP *dst = NULL;

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

	// perform upsampling or downsampling

	if((FreeImage_GetBPP(src) == 8) && (FreeImage_GetColorType(src) == FIC_PALETTE)) {
		// special case for color map indexed images ...
		FIBITMAP *src24 = NULL;
		FIBITMAP *dst24 = NULL;
		try {
			// transparent conversion to 24-bit (any transparency table will be destroyed)
			src24 = FreeImage_ConvertTo24Bits(src);
			if(!src24) throw(1);
			// perform upsampling or downsampling
			dst24 = Engine.scale(src24, dst_width, dst_height);
			if(!dst24) throw(1);
			// color quantize to 8-bit
			dst = FreeImage_ColorQuantize(dst24, FIQ_WUQUANT);
			// free and return
			FreeImage_Unload(src24);
			FreeImage_Unload(dst24);
		} catch(int) {
			if(src24) FreeImage_Unload(src24);
			if(dst24) FreeImage_Unload(dst24);
		}
	}
	else {
		// normal case (8-bit greyscale, 24- or 32-bit images)
		dst = Engine.scale(src, dst_width, dst_height);
	}


	delete pFilter;

	return dst;
}

