// ==========================================================
// Upsampling / downsampling routine
//
// Design and implementation by
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

#include "Resize.h"

FIBITMAP * DLL_CALLCONV 
FreeImage_Rescale(FIBITMAP *src, int dst_width, int dst_height, FREE_IMAGE_FILTER filter) {
	FIBITMAP *dst = NULL, *imt = NULL, *tmp = NULL;
    int bpp;

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

    bpp = FreeImage_GetBPP(src);
    CResizeEngine Engine(pFilter, (8 == bpp ? 24 : bpp));

	try {
        if(8 == bpp)   // palettized pictures are containing indices, no colours...
            imt = FreeImage_ConvertTo24Bits(src);

        dst = Engine.scale((imt ? imt : src), dst_width, dst_height);
		if(!dst) throw(1);

		if(8 == bpp) {
            tmp = FreeImage_ColorQuantize(dst, FIQ_WUQUANT);
            if(tmp) {
                FreeImage_Unload(dst);
                dst = tmp; 
            }
		}
		delete pFilter;
		return dst;

	} catch(int) {
        if(tmp) FreeImage_Unload(tmp);
        if(imt) FreeImage_Unload(imt);
		if(dst) FreeImage_Unload(dst);
        delete pFilter;
	}
	return NULL;
}

