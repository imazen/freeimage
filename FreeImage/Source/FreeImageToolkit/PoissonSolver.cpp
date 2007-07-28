// ==========================================================
// Poisson solver 
//
// Design and implementation by
// - Hervé Drolon (drolon@infonie.fr)
// based on a code published by Jack Tumblin, 2004
// ref.: http://www.umiacs.umd.edu/~aagrawal/software.html
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
#include "PoissonSolver.h"

#ifndef M_PI
#define M_PI 3.14159265358979323846 
#endif // M_PI

/**
Poisson solver based on a Successive Over-Relaxation method with Chebyshev acceleration
*/
static BOOL Poisson2D_SOR(Array2D& Lap, int maxCount, int bound, double maxErr, Array2D& U) {
	int i, j, k, imax, jmax;
	int isOdd, chk;					// Checkerboard generator.
	double err2sum;					// squared error sum in output's Laplacian
	float chebW;					// Chebyshev acceleration weight.

	const int JT_BORD = 1;			// Width of zero-valued border around image.

	if(!Lap.isValid()) return FALSE;

	// maximum acceptable squared error
	const double maxSquaredError = maxErr * maxErr;

	// Make a single output image buffer that can hold Lap plus a border 
	// of 'JT_BORD' pixels wide on all 4 sides:
	Array2D buffer(2*JT_BORD + Lap.getCols(), 2*JT_BORD + Lap.getRows());
	if(!buffer.isValid()) return FALSE;

	// Find largest address within the border
	imax = JT_BORD + Lap.getCols();
	jmax = JT_BORD + Lap.getRows();

	if(bound == 1) {
		// Initialize the buffers: 
		// for (default) Dirichlet boundary conditions, set image & surrounds to zero.
		buffer.setConst((float)0.0);	
	}
	else {
		// for (not recommended) VonNeumann conditions, 
		// grow darker & lighter from the mid-level gray value of 0.5
		buffer.setConst((float)0.5);	
	}								

	// Radius of Convergence for the Jacobi method 
	// which is known analytically for the Poisson solver (pg 869, eqn 19.5.24, Num. Recipes)

	// To compute the Chebyshev acceleration weight chebW we need to know the
	// radius of convergence for Jacobi iterations for our solver. Rectangular
	// boundaries (like ours) with either Dirichlet or VonNeumann conditions
	// used in solution to a Poisson equation have a radius of convergence given
	// by an analytical expression (eqn. 19.5.24, pg. 869, Numerical Recipes)
	const double rjac = (cos(M_PI/imax) + cos(M_PI/jmax)) / 2.0;
	const double rjac2 = rjac * rjac;

	// (Note rjac is asymptotically 1.0 as imax and jmax grows.)
	chebW = (float)1.0;			// for 1st pass, no acceleration.

	for(k = 0; k < maxCount; k++) {	
		// Iteratively modify the output buffer so the Laplacian at each pixel matches Lap values:

		err2sum = 0.0;	// clear the max-error-finder for this pass.

		for(isOdd = 0; isOdd < 2; isOdd++) {
			// for each checkerboard (red-black) half-sweep of the image:		

			chk = isOdd;	// (starting pixel for current scanline)

			for(j = JT_BORD; j < jmax; j++) {				// update every scanline,
				for(i = JT_BORD + chk; i < imax; i += 2) {	// but only half the pixels

					// current pixel value at (col, row)
					const float pixelValue = buffer(i, j);

					// desired Laplacian
					const float lapWanted = Lap(i-JT_BORD, j-JT_BORD); 
					// sum NSEW neighbors
					// => current local average = (buffer(i+1, j) + buffer(i-1, j) + buffer(i, j+1) + buffer(i, j-1))
					const float lapAvg = buffer.localAverage(i, j);

					// current Laplacian
					const float lapNow = lapAvg - (float)4.0 * pixelValue;	
					// how far wrong it is
					const float lapErr = lapWanted - lapNow;

					// in-place correction of 1/4 of the Laplacian error
					// but exaggerate the correction by chebW factor,
					// which grows as large as 2.0.
					buffer(i, j) = pixelValue - (float)0.25 * chebW * lapErr;

					// (compute the error).
					err2sum += (double)lapErr*lapErr;	// sum-of-square err

				} // end of one scanline,

				chk = (chk + 1) % 2;	// toggle 'chk' on each scanline

				if((k == 0) && (isOdd == 0)) {	// Initial pass on checkerboard's 2nd half ?
					// calculate the Chebyshev polynomial:
					chebW = (float)( 1.0 / (1.0 - 0.5*rjac2));
				} else {	// (chebW should be ~2.0)					
					// No? asymptotically go to optimal value:
					chebW = (float)(1.0 / (1.0 - 0.25*rjac2*chebW));
				}			// (chebW should be ~2.0)

			}	// end of one half-sweep,

			// ---------------------------------------------------------------
			// Set Boundary conditions 
			// ---------------------------------------------------------------

			// (bound % 2 == 1) means Dirichlet Boundary conditions:
			// We already have them! Border pixels initialized to zero & unchanged.

			if(bound % 2 == 0) {		// VonNeumann Boundary conditions

				// update the border pixels so that their values match the adjacent pixel:
				for(j = 0; j < JT_BORD; j++) {				
					// for top & bottom border scanlines,
					for(i = JT_BORD; i < imax; i += 2) {	
						// and within the horiz. borders, 
						// copy nearest scanline within border
						buffer(i, j) = buffer(i, JT_BORD);		// top 
						buffer(i, jmax+j) = buffer(i, jmax-1);	// bottom
					}
				}
				for(j = JT_BORD; j < jmax; j++) {
					// for left & right side borders,				
					for(i = 0; i < JT_BORD; i++) {
						// copy nearest column within border
						buffer(i, j) = buffer(JT_BORD, j);		// left side
						buffer(i+imax, j) = buffer(imax-1, j);	// right side
					}
				}
				for(j = 0; j < JT_BORD; j++) {
					// Now fill in all the corner values
					for(i = 0; i < JT_BORD; i++) {
						buffer(i, j) = buffer(JT_BORD, JT_BORD);			// lower left,
						buffer(i+imax, j) = buffer(imax-1, JT_BORD);		// lower right,
						buffer(i, j+jmax) = buffer(JT_BORD, jmax-1);		// upper left,
						buffer(i+imax, j+jmax) = buffer(imax-1, jmax-1);	// upper right.
					}
				}
			}

			//----- END boundary-value setting -----------
			
		} // go on to the next iteration.

		if(err2sum < maxSquaredError) {	// Are we below the error maximum ?
			break;						// Yes. Stop.  Answer is in 'oldB' buffer.
		}								// No; keep going.
	}

	// ---------------------------------------------------------------
	// Copy result to output image, without border
	// ---------------------------------------------------------------

	for(j = JT_BORD; j < jmax; j++) {
		for(i = JT_BORD; i < imax; i++) {
			U(i-JT_BORD, j-JT_BORD) = buffer(i, j);
		}
	}

	return TRUE;
}

// ----------------------------------------------------------
//  FreeImage wrapper
// ----------------------------------------------------------

FIBITMAP* FreeImage_PoissonSolver(FIBITMAP *Laplacian, double maxError, int maxCount, BOOL bDirichlet) {
	int width = FreeImage_GetWidth(Laplacian);
	int height = FreeImage_GetHeight(Laplacian);
	// allocate the output image I
	FIBITMAP *I = FreeImage_AllocateT(FIT_FLOAT, width, height);
	if(!I) return NULL;

	// wrap input and output images and solve the PDE
	Array2D DivG(Laplacian);
	Array2D U(I);	
	
	int bound = bDirichlet ? 1 : 0;
	if(maxCount <= 0) maxCount = MAX(1000, width * height);	

	BOOL bResult = Poisson2D_SOR(DivG, maxCount, bound, maxError, U);
	if(!bResult) {
		FreeImage_Unload(I);
		return NULL;
	}
	// return the integrated image
	return I;
}
