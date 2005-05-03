// ==========================================================
// High Dynamic Range bitmap conversion routines
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
#include "ToneMapping.h"

// ----------------------------------------------------------
// Convert RGB to and from Yxy, same as in Reinhard et al. SIGGRAPH 2002
// References : 
// [1] Radiance Home Page [Online] http://radsite.lbl.gov/radiance/HOME.html
// [2] E. Reinhard, M. Stark, P. Shirley, and J. Ferwerda,  
//     Photographic Tone Reproduction for Digital Images, ACM Transactions on Graphics, 
//     21(3):267-276, 2002 (Proceedings of SIGGRAPH 2002). 
// [3] J. Tumblin and H.E. Rushmeier, 
//     Tone Reproduction for Realistic Images. IEEE Computer Graphics and Applications, 
//     13(6):42-48, 1993.
// ----------------------------------------------------------

/**
nominal CRT primaries 
*/
static const float CIE_x_r = 0.640F;
static const float CIE_y_r = 0.330F;
static const float CIE_x_g = 0.290F;
static const float CIE_y_g = 0.600F;
static const float CIE_x_b = 0.150F;
static const float CIE_y_b = 0.060F;
static const float CIE_x_w = 0.3333F;	// use true white
static const float CIE_y_w = 0.3333F;


static const float CIE_D = ( CIE_x_r*(CIE_y_g - CIE_y_b) + CIE_x_g*(CIE_y_b - CIE_y_r) + CIE_x_b*(CIE_y_r - CIE_y_g) );
static const float CIE_C_rD = ( (1/CIE_y_w) * ( CIE_x_w*(CIE_y_g - CIE_y_b) - CIE_y_w*(CIE_x_g - CIE_x_b) + CIE_x_g*CIE_y_b - CIE_x_b*CIE_y_g) );
static const float CIE_C_gD = ( (1/CIE_y_w) * ( CIE_x_w*(CIE_y_b - CIE_y_r) - CIE_y_w*(CIE_x_b - CIE_x_r) - CIE_x_r*CIE_y_b + CIE_x_b*CIE_y_r) );
static const float CIE_C_bD = ( (1/CIE_y_w) * ( CIE_x_w*(CIE_y_r - CIE_y_g) - CIE_y_w*(CIE_x_r - CIE_x_g) + CIE_x_r*CIE_y_g - CIE_x_g*CIE_y_r) );

/**
RGB to XYZ (no white balance)
*/
static const float  RGB2XYZ[3][3] = {
	{ CIE_x_r*CIE_C_rD / CIE_D, 
	  CIE_x_g*CIE_C_gD / CIE_D, 
	  CIE_x_b*CIE_C_bD / CIE_D 
	},
	{ CIE_y_r*CIE_C_rD / CIE_D, 
	  CIE_y_g*CIE_C_gD / CIE_D, 
	  CIE_y_b*CIE_C_bD / CIE_D 
	},
	{ (1 - CIE_x_r-CIE_y_r)*CIE_C_rD / CIE_D,
	  (1 - CIE_x_g-CIE_y_g)*CIE_C_gD / CIE_D,
	  (1 - CIE_x_b-CIE_y_b)*CIE_C_bD / CIE_D
	}
};

/**
XYZ to RGB (no white balance)
*/
static const float  XYZ2RGB[3][3] = {
	{(CIE_y_g - CIE_y_b - CIE_x_b*CIE_y_g + CIE_y_b*CIE_x_g) / CIE_C_rD,
	 (CIE_x_b - CIE_x_g - CIE_x_b*CIE_y_g + CIE_x_g*CIE_y_b) / CIE_C_rD,
	 (CIE_x_g*CIE_y_b - CIE_x_b*CIE_y_g) / CIE_C_rD
	},
	{(CIE_y_b - CIE_y_r - CIE_y_b*CIE_x_r + CIE_y_r*CIE_x_b) / CIE_C_gD,
	 (CIE_x_r - CIE_x_b - CIE_x_r*CIE_y_b + CIE_x_b*CIE_y_r) / CIE_C_gD,
	 (CIE_x_b*CIE_y_r - CIE_x_r*CIE_y_b) / CIE_C_gD
	},
	{(CIE_y_r - CIE_y_g - CIE_y_r*CIE_x_g + CIE_y_g*CIE_x_r) / CIE_C_bD,
	 (CIE_x_g - CIE_x_r - CIE_x_g*CIE_y_r + CIE_x_r*CIE_y_g) / CIE_C_bD,
	 (CIE_x_r*CIE_y_g - CIE_x_g*CIE_y_r) / CIE_C_bD
	}
};

/**
This gives approximately the following matrices : 

static const float RGB2XYZ[3][3] = { 
	{ 0.514083F, 0.323889F, 0.162028F },
	{ 0.265074F, 0.670115F, 0.0648112F },
	{ 0.0240976F, 0.122854F, 0.853348F }
};
static const float XYZ2RGB[3][3] = { 
	{ 2.56562F, -1.16699F, -0.398511F },
	{ -1.02209F, 1.97826F, 0.0438210F }, 
	{ 0.0746980F, -0.251851F, 1.17680F } 
};
*/

// ----------------------------------------------------------

static const float EPSILON = 1e-06F;
static const float INF = 1e+10F;

/**
Convert in-place floating point RGB data to Yxy.<br>
On output, pixel->red == Y, pixel->green == x, pixel->blue == y
@param dib Input RGBF / Output Yxy image
@return Returns TRUE if successful, returns FALSE otherwise
*/
BOOL 
ConvertInPlaceRGBFToYxy(FIBITMAP *dib) {
	float result[3];

	if(FreeImage_GetImageType(dib) != FIT_RGBF)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(dib);
	unsigned height = FreeImage_GetHeight(dib);
	unsigned pitch  = FreeImage_GetPitch(dib);
	
	BYTE *bits = (BYTE*)FreeImage_GetBits(dib);
	for(unsigned y = 0; y < height; y++) {
		FIRGBF *pixel = (FIRGBF*)bits;
		for(unsigned x = 0; x < width; x++) {
			result[0] = result[1] = result[2] = 0;
			for (int i = 0; i < 3; i++) {
				result[i] += RGB2XYZ[i][0] * pixel[x].red;
				result[i] += RGB2XYZ[i][1] * pixel[x].green;
				result[i] += RGB2XYZ[i][2] * pixel[x].blue;
			}
			float W = result[0] + result[1] + result[2];
			float Y = result[1];
			if(W > 0) { 
				pixel[x].red   = Y;			    // Y 
				pixel[x].green = result[0] / W;	// x 
				pixel[x].blue  = result[1] / W;	// y 	
			} else {
				pixel[x].red = pixel[x].green = pixel[x].blue = 0;
			}
		}
		// next line
		bits += pitch;
	}

	return TRUE;
}

/**
Convert in-place Yxy image to floating point RGB data.<br>
On input, pixel->red == Y, pixel->green == x, pixel->blue == y
@param dib Input Yxy / Output RGBF image
@return Returns TRUE if successful, returns FALSE otherwise
*/
BOOL 
ConvertInPlaceYxyToRGBF(FIBITMAP *dib) {
	float result[3];
	float X, Y, Z;

	if(FreeImage_GetImageType(dib) != FIT_RGBF)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(dib);
	unsigned height = FreeImage_GetHeight(dib);
	unsigned pitch  = FreeImage_GetPitch(dib);

	BYTE *bits = (BYTE*)FreeImage_GetBits(dib);
	for(unsigned y = 0; y < height; y++) {
		FIRGBF *pixel = (FIRGBF*)bits;
		for(unsigned x = 0; x < width; x++) {
			Y = pixel[x].red;	        // Y 
			result[1] = pixel[x].green;	// x 
			result[2] = pixel[x].blue;	// y 
			if ((Y > EPSILON) && (result[1] > EPSILON) && (result[2] > EPSILON)) {
				X = (result[1] * Y) / result[2];
				Z = (X / result[1]) - X - Y;
			} else {
				X = Z = EPSILON;
			}
			pixel[x].red   = X;
			pixel[x].green = Y;
			pixel[x].blue  = Z;
			result[0] = result[1] = result[2] = 0;
			for (int i = 0; i < 3; i++) {
				result[i] += XYZ2RGB[i][0] * pixel[x].red;
				result[i] += XYZ2RGB[i][1] * pixel[x].green;
				result[i] += XYZ2RGB[i][2] * pixel[x].blue;
			}
			pixel[x].red   = result[0];	// R
			pixel[x].green = result[1];	// G
			pixel[x].blue  = result[2];	// B
		}
		// next line
		bits += pitch;
	}

	return TRUE;
}

/**
Get the maximum, minimum and average luminance.<br>
On input, pixel->red == Y, pixel->green == x, pixel->blue == y
@param Yxy Source Yxy image to analyze
@param maxLum Maximum luminance
@param minLum Minimum luminance
@param worldLum Average luminance (world adaptation luminance)
@return Returns TRUE if successful, returns FALSE otherwise
*/
BOOL 
LuminanceFromYxy(FIBITMAP *Yxy, float *maxLum, float *minLum, float *worldLum) {
	if(FreeImage_GetImageType(Yxy) != FIT_RGBF)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(Yxy);
	unsigned height = FreeImage_GetHeight(Yxy);
	unsigned pitch  = FreeImage_GetPitch(Yxy);

	float max_lum = 0, min_lum = 0;
	double sum = 0;

	BYTE *bits = (BYTE*)FreeImage_GetBits(Yxy);
	for(unsigned y = 0; y < height; y++) {
		const FIRGBF *pixel = (FIRGBF*)bits;
		for(unsigned x = 0; x < width; x++) {
			const float Y = pixel[x].red;
			max_lum = (max_lum < Y) ? Y : max_lum;	// max Luminance in the scene
			min_lum = (min_lum < Y) ? min_lum : Y;	// max Luminance in the scene
			sum += log(2.3e-5 + Y);					// contrast constant in Tumblin paper
		}
		// next line
		bits += pitch;
	}
	// maximum luminance
	*maxLum = max_lum;
	// minimum luminance
	*minLum = min_lum;
	// average log luminance
	double avgLogLum = (sum / (width * height));
	// world adaptation luminance
	*worldLum = (float)exp(avgLogLum);

	return TRUE;
}

/**
Clamp RGBF image highest values to display white, 
then convert to 24-bit RGB
*/
FIBITMAP* 
ClampConvertRGBFTo24(FIBITMAP *src) {
	if(FreeImage_GetImageType(src) != FIT_RGBF)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(src);
	unsigned height = FreeImage_GetHeight(src);

	FIBITMAP *dst = FreeImage_Allocate(width, height, 24, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
	if(!dst) return NULL;

	unsigned src_pitch  = FreeImage_GetPitch(src);
	unsigned dst_pitch  = FreeImage_GetPitch(dst);

	BYTE *src_bits = (BYTE*)FreeImage_GetBits(src);
	BYTE *dst_bits = (BYTE*)FreeImage_GetBits(dst);

	for(unsigned y = 0; y < height; y++) {
		const FIRGBF *src_pixel = (FIRGBF*)src_bits;
		BYTE *dst_pixel = (BYTE*)dst_bits;
		for(unsigned x = 0; x < width; x++) {
			const float red   = (src_pixel[x].red > 1)   ? 1 : src_pixel[x].red;
			const float green = (src_pixel[x].green > 1) ? 1 : src_pixel[x].green;
			const float blue  = (src_pixel[x].blue > 1)  ? 1 : src_pixel[x].blue;
			
			dst_pixel[FI_RGBA_RED]   = (BYTE)(255 * red   + 0.5);
			dst_pixel[FI_RGBA_GREEN] = (BYTE)(255 * green + 0.5);
			dst_pixel[FI_RGBA_BLUE]  = (BYTE)(255 * blue  + 0.5);
			dst_pixel += 3;
		}
		src_bits += src_pitch;
		dst_bits += dst_pitch;
	}

	return dst;
}

/**
Extract the luminance channel L from a RGBF image. 
Luminance is calculated from the sRGB model (RGB2XYZ matrix) 
using a D65 white point : 
L = ( 0.2126 * r ) + ( 0.7152 * g ) + ( 0.0722 * b )
Reference : 
A Standard Default Color Space for the Internet - sRGB. 
[online] http://www.w3.org/Graphics/Color/sRGB
*/
FIBITMAP*  
ConvertRGBFToY(FIBITMAP *src) {
	if(FreeImage_GetImageType(src) != FIT_RGBF)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(src);
	unsigned height = FreeImage_GetHeight(src);

	FIBITMAP *dst = FreeImage_AllocateT(FIT_FLOAT, width, height);
	if(!dst) return NULL;

	unsigned src_pitch  = FreeImage_GetPitch(src);
	unsigned dst_pitch  = FreeImage_GetPitch(dst);

	
	BYTE *src_bits = (BYTE*)FreeImage_GetBits(src);
	BYTE *dst_bits = (BYTE*)FreeImage_GetBits(dst);

	for(unsigned y = 0; y < height; y++) {
		const FIRGBF *src_pixel = (FIRGBF*)src_bits;
		float  *dst_pixel = (float*)dst_bits;
		for(unsigned x = 0; x < width; x++) {
			float L = 0.2126F * src_pixel[x].red + 0.7152F * src_pixel[x].green + 0.0722F * src_pixel[x].blue;
			dst_pixel[x] = (L > 0) ? L : 0;
		}
		// next line
		src_bits += src_pitch;
		dst_bits += dst_pitch;
	}

	return dst;
}

/**
Get the maximum, minimum and average luminance
@param dib Source Y image to analyze
@param maxLum Maximum luminance
@param minLum Minimum luminance
@param worldLum Average luminance (world adaptation luminance)
@return Returns TRUE if successful, returns FALSE otherwise
@see ConvertRGBFToY
*/
BOOL 
LuminanceFromY(FIBITMAP *dib, float *maxLum, float *minLum, float *worldLum) {
	if(FreeImage_GetImageType(dib) != FIT_FLOAT)
		return FALSE;

	unsigned width  = FreeImage_GetWidth(dib);
	unsigned height = FreeImage_GetHeight(dib);
	unsigned pitch  = FreeImage_GetPitch(dib);

	float max_lum = -1e20F, min_lum = 1e20F;
	double sum = 0;

	BYTE *bits = (BYTE*)FreeImage_GetBits(dib);
	for(unsigned y = 0; y < height; y++) {
		const float *pixel = (float*)bits;
		for(unsigned x = 0; x < width; x++) {
			const float Y = pixel[x];
			max_lum = (max_lum < Y) ? Y : max_lum;				// max Luminance in the scene
			min_lum = ((Y > 0) && (min_lum < Y)) ? min_lum : Y;	// min Luminance in the scene
			sum += log(2.3e-5 + Y);								// contrast constant in Tumblin paper
		}
		// next line
		bits += pitch;
	}
	// maximum luminance
	*maxLum = max_lum;
	// minimum luminance
	*minLum = min_lum;
	// average log luminance
	double avgLogLum = (sum / (width * height));
	// world adaptation luminance
	*worldLum = (float)exp(avgLogLum);

	return TRUE;
}

