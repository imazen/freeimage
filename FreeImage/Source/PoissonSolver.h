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

#ifndef POISSON_SOLVER_H
#define POISSON_SOLVER_H

/**
Simple wrapper used to manipulate a FIT_FLOAT dib as a matrix
*/
class Array2D {
private:
	/// pointer to the dib
	FIBITMAP *m_dib;
	/// pointer to the bitmap bits
	float *m_bits;
	/// TRUE if the dib need to be unloaded in the destructor
	BOOL m_bDeleteMe;
	/// dib width
	int m_width;
	/// dib height
	int m_height;
	/// dib pitch
	int m_pitch;

public:
	Array2D(FIBITMAP *dib) {
		assert((dib != NULL) && (FreeImage_GetImageType(dib) == FIT_FLOAT));
		m_dib = dib;
		m_width = FreeImage_GetWidth(dib);
		m_height = FreeImage_GetHeight(dib);
		m_pitch = FreeImage_GetPitch(dib) / sizeof(float);
		m_bits = (float*)FreeImage_GetBits(dib);
		m_bDeleteMe = FALSE;
	}

	Array2D(int cols, int rows) {
		m_dib = FreeImage_AllocateT(FIT_FLOAT, cols, rows);
		if(m_dib) {
			m_width = FreeImage_GetWidth(m_dib);
			m_height = FreeImage_GetHeight(m_dib);
			m_pitch = FreeImage_GetPitch(m_dib) / sizeof(float);
			m_bits = (float*)FreeImage_GetBits(m_dib);
			m_bDeleteMe = TRUE;
		} else {
			m_width = 0;
			m_height = 0;
			m_pitch = 0;
			m_bits = NULL;
			m_bDeleteMe = FALSE;
		}
	}

	~Array2D() {
		if(m_bDeleteMe) FreeImage_Unload(m_dib);
	}

	inline BOOL isValid() {
		return (m_dib != NULL) && (m_width > 0) && (m_height > 0);
	}

	inline int getCols() const {
		return m_width;
	}

	inline int getRows() const {
		return m_height;
	}

	inline float& operator()(int col, int row) {
		return m_bits[row*m_pitch + col];
	}

	inline const float& operator()(int col, int row) const {
		return m_bits[row*m_pitch + col];
	}

	inline void setConst(const float value) {
		float *scanline = m_bits;
		for(int y = 0; y < m_height; y++) {	
			for(int x = 0; x < m_width; x++) {
				scanline[x] = value;
			}
			scanline += m_pitch;
		}
	}
};


#ifdef __cplusplus
extern "C" {
#endif

/**
Poisson solver.<br>
Starting with a constant valued image, iteratively adjust it until its Laplacian matches 'Laplacian', using
the Successive Over-Relaxation method - converges O(N).<br>
The correction for Laplacian error is exaggerated by an amount given by a Chebyshev polynomial. 
Rather than use a fixed value < 2,(e.g. 1.99) where error can grow for the first N iterations before converging, 
using the Chebyshev acceleration guarantees monotonically shrinking error.<br>

Recall that the Laplacian of F is just the sum of the 2nd derivs in x,y directions: Laplacian = d^2F/dx^2 + d^2F/dy^2.
Given an NxM image of scalar-valued Laplacian of intensity L, make an output double-buffer of size (N+2*JT_BORD)x(M+2*JT_BORD), 
big enough to include a JT_BORD-sized border of extra pixels on the image's top, bottom, left and right sides. 
Initialize to a constant-valued image.  For each non-border pixel, find the Laplacian; if it does not match the desired value 
found in L, adjust the buffer pixel's intensity to improve its Laplacian. 
'bDirichlet' let you define the Laplacian for pixels along the top,bottom, and sides of images.<br>

<ul>
<li>if bDirichlet is TRUE (DEFAULT)<br>
Use Dirichlet boundary conditions: Presume the image is surrounded by an unbounded field of zero-valued pixels. 
<li>if bDirichlet is FALSE (Not recommended)<br>
Uses Von Neumann boundary conditions: assumes gradients across across the image boundaries are zero, 
but unchanged in tangent direction.<br>
</ul>
<b>NOTES</b>: Dirichlet conditions give you a unique solution; Von Neumann conditions have an unknown constant offset. 
Poisson solutions can have persistent large error near boundaries if the laplacian source image was computed with unknown 
boundary conditions, or was a section of a larger Laplacian image. You can improve results by enclosing the Laplacian in a 
border of zero-valued pixels, and use either VonNeumann or Dirichlet..

@param L Laplacian of a width x height image
@param maxError Maximum desired squared error. The solver will iterate until the error is less that maxError or 
until maxCount iterations has been reached. 
@param maxCount Maximum number of iteration. If -1, use maxCount = MAX(1000, width * height);	
@param bDirichlet If TRUE, use Dirichlet boundary condition (default): presume all pixels outside of the image are zero-valued.  
If FALSE, use Von Neumann boundary condition: presume gradient across the outer edge of the image is zero 
(as if image extended to infinity by copying the outermost image pixels endlessly). 
@return Returns the solved equation if successful, returns NULL otherwise
*/
FIBITMAP* FreeImage_PoissonSolver(FIBITMAP *L, double maxError = 0.001, int maxCount = -1, BOOL bDirichlet = TRUE);

#ifdef __cplusplus
}
#endif

#endif // POISSON_SOLVER_H

