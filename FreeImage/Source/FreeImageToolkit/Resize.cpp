// ==========================================================
// Upsampling / downsampling classes
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

/**
  Filter weights table.
  This class stores contribution information for an entire line (row or column).
*/
CWeightsTable::CWeightsTable(CGenericFilter *pFilter, DWORD uDstSize, DWORD uSrcSize) {
	DWORD u;
	double dWidth;
	double dFScale = 1.0;
	double dFilterWidth = pFilter->GetWidth();
    double dScale = double(uDstSize) / double(uSrcSize);

	if(dScale < 1.0) {
		// minification
		dWidth = dFilterWidth / dScale; 
		dFScale = dScale; 
	} else {
		// magnification
		dWidth= dFilterWidth; 
	}

	// allocate a new line contributions structure
	//
	// window size is the number of sampled pixels
	m_WindowSize = 2 * (int)ceil(dWidth) + 1; 
	m_LineLength = uDstSize; 
	 // allocate list of contributions 
	m_WeightTable = (Contribution*)malloc(m_LineLength * sizeof(Contribution));
	for(u = 0 ; u < m_LineLength ; u++) {
		// allocate contributions for every pixel
		m_WeightTable[u].Weights = (double*)malloc(m_WindowSize * sizeof(double));
	}

	// offset for discrete to continuous coordinate conversion
	double dOffset = (0.5 / dScale) - 0.5;


	for(u = 0; u < m_LineLength; u++) {
		// scan through line of contributions
		double dCenter = (double)u / dScale + dOffset;   // reverse mapping
		// find the significant edge points that affect the pixel
		int iLeft = MAX (0, (int)floor (dCenter - dWidth)); 
		int iRight = MIN ((int)ceil (dCenter + dWidth), int(uSrcSize) - 1); 

		// cut edge points to fit in filter window in case of spill-off
		if((iRight - iLeft + 1) > int(m_WindowSize)) {
			if(iLeft < (int(uSrcSize) - 1 / 2)) {
				iLeft++; 
			} else {
				iRight--; 
			}
		}

		m_WeightTable[u].Left = iLeft; 
		m_WeightTable[u].Right = iRight;

		int iSrc = 0;
		double dTotalWeight = 0.0;  // zero sum of weights
		for(iSrc = iLeft; iSrc <= iRight; iSrc++) {
			// calculate weights
			double weight = dFScale * pFilter->Filter(dFScale * (dCenter - (double)iSrc));
			m_WeightTable[u].Weights[iSrc-iLeft] = weight;
			dTotalWeight += weight;
		}
		if((dTotalWeight > 0) && (dTotalWeight != 1)) {
			// normalize weight of neighbouring points
			for(iSrc = iLeft; iSrc <= iRight; iSrc++) {
				// normalize point
				m_WeightTable[u].Weights[iSrc-iLeft] /= dTotalWeight; 
			}
		}
   } 
}

CWeightsTable::~CWeightsTable() {
	for(DWORD u = 0; u < m_LineLength; u++) {
		// free contributions for every pixel
		free(m_WeightTable[u].Weights);
	}
	// free list of pixels contributions
	free(m_WeightTable);
}

// ---------------------------------------------

/**
 CResizeEngine<br>
 This class performs filtered zoom. It scales an image to the desired dimensions with 
 any of the CGenericFilter derived filter class.<br>
 It works with 8-, 24- and 32-bit buffers.<br><br>

 <b>References</b> : <br>
 [1] Paul Heckbert, C code to zoom raster images up or down, with nice filtering. 
 UC Berkeley, August 1989. [online] http://www-2.cs.cmu.edu/afs/cs.cmu.edu/Web/People/ph/heckbert.html
 [2] Eran Yariv, Two Pass Scaling using Filters. The Code Project, December 1999. 
 [online] http://www.codeproject.com/bitmap/2_pass_scaling.asp

*/

FIBITMAP* CResizeEngine::scale(FIBITMAP *src, unsigned dst_width, unsigned dst_height) { 
	DWORD src_width  = FreeImage_GetWidth(src); 
	DWORD src_height = FreeImage_GetHeight(src);

	// allocate the dst image
	FIBITMAP *dst = FreeImage_Allocate(dst_width, dst_height, m_bpp);
	if(!dst) return NULL;

	// decide which filtering order (xy or yx) is faster for this mapping by
	// counting convolution multiplies

	if(dst_width*src_height <= dst_height*src_width) {
		// xy filtering
		// -------------

		// allocate a temporary image
		FIBITMAP *tmp = FreeImage_Allocate(dst_width, src_height, m_bpp);
		if(!tmp) {
			FreeImage_Unload(dst);
			return NULL;
		}

		// scale source image horizontally into temporary image
		horizontalFilter(src, src_width, src_height, tmp, dst_width, src_height);

		// scale temporary image vertically into result image    
		verticalFilter(tmp, dst_width, src_height, dst, dst_width, dst_height);

		// free temporary image
		FreeImage_Unload(tmp);

	} else {
		// yx filtering
		// -------------

		// allocate a temporary image
		FIBITMAP *tmp = FreeImage_Allocate(src_width, dst_height, m_bpp);
		if(!tmp) {
			FreeImage_Unload(dst);
			return NULL;
		}

		// scale source image vertically into temporary image
		verticalFilter(src, src_width, src_height, tmp, src_width, dst_height);

		// scale temporary image horizontally into result image    
		horizontalFilter(tmp, src_width, dst_height, dst, dst_width, dst_height);

		// free temporary image
		FreeImage_Unload(tmp);
	}

	return dst;
} 


/// Performs horizontal image filtering
void CResizeEngine::horizontalFilter(FIBITMAP *src, unsigned src_width, unsigned src_height, FIBITMAP *dst, unsigned dst_width, unsigned dst_height) {
    int samplesperpixel = (m_bpp / 8);
	if(dst_width == src_width) {
		// no scaling required, just copy
		BYTE *src_bits = FreeImage_GetBits(src);
		BYTE *dst_bits = FreeImage_GetBits(dst);
		memcpy(dst_bits, src_bits, dst_height * FreeImage_GetPitch(dst));
	}
	else {
		// allocate and calculate the contributions
		CWeightsTable weightsTable(m_pFilter, dst_width, src_width); 

		// step through rows            
		for(unsigned y = 0; y < dst_height; y++) {
			// scale each row 
			BYTE *src_bits = FreeImage_GetScanLine(src, y);
			BYTE *dst_bits = FreeImage_GetScanLine(dst, y);

			for(unsigned x = 0; x < dst_width; x++) {
				// loop through row
                double value[4] = {0, 0, 0, 0};                 // 4 = 32bpp max
				int iLeft = weightsTable.getLeftBoundary(x);    // retrieve left boundary
				int iRight = weightsTable.getRightBoundary(x);  // retrieve right boundary

				for(int i = iLeft; i <= iRight; i++) {
					// scan between boundaries
					// accumulate weighted effect of each neighboring pixel
					double weight = weightsTable.getWeight(x, i-iLeft);
                    for (int j = 0; j < m_bpp / 8; j++)
					    value[j] += (weight * (double)src_bits[i*samplesperpixel + j]); 
				} 

				// place result in destination pixel
                for (int j = 0; j < m_bpp / 8; j++)
				    dst_bits[x*samplesperpixel + j] = 
                        (BYTE)MIN(MAX((int)0, (int)(value[j] + 0.5)), (int)255);
			} 
		}
	}
} 

/// Performs vertical image filtering
void CResizeEngine::verticalFilter(FIBITMAP *src, unsigned src_width, unsigned src_height, FIBITMAP *dst, unsigned dst_width, unsigned dst_height) { 
    int samplesperpixel = (m_bpp / 8);
	if(src_height == dst_height) {
		// no scaling required, just copy
		BYTE *src_bits = FreeImage_GetBits(src);
		BYTE *dst_bits = FreeImage_GetBits(dst);
		memcpy(dst_bits, src_bits, dst_height * FreeImage_GetPitch(dst));
	}
	else {
		// allocate and calculate the contributions
		CWeightsTable weightsTable(m_pFilter, dst_height, src_height); 

		// step through columns
		for(unsigned x = 0; x < dst_width; x++) {
			// scale each column
			for(unsigned y = 0; y < dst_height; y++) {
				// loop through column
                double value[4] = {0, 0, 0, 0};                 // 4 = 32bpp max
				int iLeft = weightsTable.getLeftBoundary(y);    // retrieve left boundary
				int iRight = weightsTable.getRightBoundary(y);  // retrieve right boundary

				BYTE *dst_bits = FreeImage_GetScanLine(dst, y);

				for(int i = iLeft; i <= iRight; i++) {
					// scan between boundaries
					// accumulate weighted effect of each neighboring pixel
					BYTE *src_bits = FreeImage_GetScanLine(src, i);
					double weight = weightsTable.getWeight(y, i-iLeft);
                    for (int j = 0; j < m_bpp / 8; j++)
					    value[j] += (weight * (double)src_bits[x*samplesperpixel + j]);
				}

				// clamp and place result in destination pixel
                for (int j = 0; j < m_bpp / 8; j++)
				    dst_bits[x*samplesperpixel + j] = 
                        (BYTE)MIN(MAX((int)0, (int)(value[j] + 0.5)), (int)255);
			}
		}
	}
} 

