// ==========================================================
// Background filling routines
//
// Design and implementation by
// - Carsten Klein (c.klein@datagis.com)
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

/** @brief Determines, whether a palletized image is visually greyscale or not.
 
 Unlike with FreeImage_GetColorType, which returns either FIC_MINISBLACK or
 FIC_MINISWHITE for a greyscale image with a linear ramp palette, the return  
 value of this function does not depend on the palette's order, but only on the
 palette's individial colors.
 @param dib The image to be tested.
 @return Returns TRUE if the palette of the image specified contains only
 greyscales, FALSE otherwise.
 */
static BOOL
IsVisualGreyscaleImage(FIBITMAP *dib) {

	switch (FreeImage_GetBPP(dib)) {
		case 1:
		case 4:
		case 8: {
			unsigned ncolors = FreeImage_GetColorsUsed(dib);
			RGBQUAD *rgb = FreeImage_GetPalette(dib);
			for (unsigned i = 0; i< ncolors; i++) {
				if ((rgb->rgbRed != rgb->rgbGreen) || (rgb->rgbRed != rgb->rgbBlue)) {
					return FALSE;
				}
			}
			return TRUE;
		}
		default: {
			return (FreeImage_GetColorType(dib) == FIC_MINISBLACK);
		}
	}
}

static int
GetPaletteIndex(FIBITMAP *dib, const RGBQUAD *color, int options, FREE_IMAGE_COLOR_TYPE *color_type) {
	
	int result = -1;
	
	if ((!dib) || (!color)) {
		return result;
	}
	
	int bpp = FreeImage_GetBPP(dib);

	// First check trivial case: return color->rgbReserved if only
	// DRAWING_ALPHA_IS_INDEX is set.
	if ((options & FI_COLOR_PALETTE_SEARCH_MASK) == FI_COLOR_ALPHA_IS_INDEX) {
		if (bpp == 1) {
			return color->rgbReserved & 0x01;
		} else if (bpp == 4) {
			return color->rgbReserved & 0x0F;
		}
		return color->rgbReserved;
	}
	
	if (bpp == 8) {
		FREE_IMAGE_COLOR_TYPE ct =
			(color_type == NULL || *color_type < 0) ?
				FreeImage_GetColorType(dib) : *color_type;
		if (ct == FIC_MINISBLACK) {
			return GREY(color->rgbRed, color->rgbGreen, color->rgbBlue);
		}
		if (ct == FIC_MINISWHITE) {
			return 255 - GREY(color->rgbRed, color->rgbGreen, color->rgbBlue);
		}
	} else if (bpp > 8) {
		// for palettized images only
		return result;
	}

	if (options & FI_COLOR_FIND_EQUAL_COLOR) {
		
		// Option DRAWING_ALPHA_IS_INDEX is implicit so, set
		// index to color->rgbReserved
		result = color->rgbReserved;
		if (bpp == 1) {
			result &= 0x01;
		} else if (bpp == 4) {
			result &= 0x0F;
		}		

		unsigned ucolor;
		if (!IsVisualGreyscaleImage(dib)) {
			ucolor = (*((unsigned *)color)) & 0xFFFFFF;
		} else {
			ucolor = GREY(color->rgbRed, color->rgbGreen, color->rgbBlue) * 0x010101;
			//ucolor = (ucolor | (ucolor << 8) | (ucolor << 16));
		}
		unsigned ncolors = FreeImage_GetColorsUsed(dib);
		unsigned *palette = (unsigned *)FreeImage_GetPalette(dib);
		for (unsigned i = 0; i < ncolors; i++) {
			if ((palette[i] & 0xFFFFFF) == ucolor) {
				result = i;
				break;
			}
		}
	} else {
		unsigned minimum = UINT_MAX;
		unsigned ncolors = FreeImage_GetColorsUsed(dib);
		BYTE *palette = (BYTE *)FreeImage_GetPalette(dib);
		BYTE red, green, blue;
		if (!IsVisualGreyscaleImage(dib)) {
			red = color->rgbRed;
			green = color->rgbGreen;
			blue = color->rgbBlue;
		} else {
			red = GREY(color->rgbRed, color->rgbGreen, color->rgbBlue);
			green = blue = red;
		}
		for (unsigned i = 0; i < ncolors; i++) {
			unsigned m = abs(palette[FI_RGBA_BLUE] - blue)
					+ abs(palette[FI_RGBA_GREEN] - green)
					+ abs(palette[FI_RGBA_RED] - red);
			if (m < minimum) {
				minimum = m;
				result = i;
				if (m == 0) {
					break;
				}
			}
			palette += sizeof(RGBQUAD);
		}		
	}
	return result;
}

/** @brief Blends an alpha-transparent foreground color over an opaque background
 color.
 
 This function blends the alpha-transparent foreground color fgcolor over the
 background color bgcolor. The background color is considered fully opaque,
 whatever it's alpha value contains, whereas the foreground color is considered
 to be a real RGBA color with an alpha value, which is used for the blend
 operation. The resulting color is returned through the blended parameter.
 @param bgcolor The background color for the blend operation.
 @param fgcolor The foreground color for the blend operation. This color's alpha
 value, stored in the rgbReserved member, is the alpha value used for the blend
 operation.
 @param blended This out parameter takes the blended color and so, returns it to
 the caller. This color's alpha value will be 0xFF (255) so, the blended color
 itself has no transparency. The this argument is not changed, if the function
 fails. 
 @return Returns TRUE on succes, FALSE otherwise. This function fails if any of
 the color arguments is a null pointer.
 */
static BOOL
GetAlphaBlendedColor(const RGBQUAD *bgcolor, const RGBQUAD *fgcolor, RGBQUAD *blended) {
	
	if ((!bgcolor) || (!fgcolor) || (!blended)) {
		return FALSE;
	}
	
	BYTE alpha = fgcolor->rgbReserved;
	BYTE not_alpha = ~alpha;
	
	blended->rgbRed   = (BYTE)( ((WORD)fgcolor->rgbRed   * alpha + not_alpha * (WORD)bgcolor->rgbRed)   >> 8 );
	blended->rgbGreen = (BYTE)( ((WORD)fgcolor->rgbGreen * alpha + not_alpha * (WORD)bgcolor->rgbGreen) >> 8) ;
	blended->rgbBlue  = (BYTE)( ((WORD)fgcolor->rgbRed   * alpha + not_alpha * (WORD)bgcolor->rgbBlue)  >> 8 );
	blended->rgbReserved = 0xFF;

	return TRUE;
}

static BOOL
FillBackgroundBitmap(FIBITMAP *dib, const RGBQUAD *color, int options) {

	if ((!dib) || (FreeImage_GetImageType(dib) != FIT_BITMAP)) {
		return FALSE;;
	}
	
	if (!color) {
		return FALSE;
	}
	
	const RGBQUAD *color_intl = color;
	unsigned bpp = FreeImage_GetBPP(dib);
	unsigned width = FreeImage_GetWidth(dib);
	unsigned height = FreeImage_GetHeight(dib);
	
	FREE_IMAGE_COLOR_TYPE color_type = FreeImage_GetColorType(dib);
	
	// get a pointer to the first scanline (bottom line)
	BYTE *src_bits = FreeImage_GetScanLine(dib, 0);
	BYTE *dst_bits = src_bits;	
	
	BOOL supports_alpha = ((bpp >= 24) || ((bpp == 8) && (color_type != FIC_PALETTE)));
	
	// Check for RGBA case if bitmap supports alpha 
	// blending (8-bit greyscale, 24- or 32-bit images)
	if (supports_alpha && (options & FI_COLOR_IS_RGBA_COLOR)) {
		
		if (color->rgbReserved == 0) {
			// the fill color is fully transparent; we are done
			return TRUE;
		}
		
		// Only if the fill color is NOT fully opaque, draw it with
		// the (much) slower FreeImage_DrawLine function and return.
		if (color->rgbReserved < 255) {
							
			// If we will draw on an unicolor background, it's
			// faster to draw opaque with a alpha blended color.
			// So, first get the backcolor from the first pixel
			// in the image (bottom-left pixel).
			RGBQUAD bgcolor;
			if (bpp == 8) {
				bgcolor = FreeImage_GetPalette(dib)[*src_bits];
			} else {	
				bgcolor.rgbBlue = src_bits[FI_RGBA_BLUE];
				bgcolor.rgbGreen = src_bits[FI_RGBA_GREEN];
				bgcolor.rgbRed = src_bits[FI_RGBA_RED];
				bgcolor.rgbReserved = 0xFF;
			}
			RGBQUAD blend;
			GetAlphaBlendedColor(&bgcolor, color_intl, &blend);
			color_intl = &blend;
		}
	}
	
	int index = (bpp <= 8) ? GetPaletteIndex(dib, color_intl, options, &color_type) : 0;
	if (index == -1) {
		// No palette index found for palletized 
		// images. Should never happen...
		return FALSE;
	}
	
	// first, build the first scanline (line 0)
	switch (bpp) {
		case 1: {
			unsigned bytes = (width / 8);
			memset(dst_bits, ((index == 1) ? 0xFF : 0x00), bytes);
			//int n = width % 8;
			int n = width & 7;
			if (n) {
				if (index == 1) {
					// set n leftmost bits
					dst_bits[bytes] |= (0xFF << (8 - n));
				} else {
					// clear n leftmost bits
					dst_bits[bytes] &= (0xFF >> n);
				}
			}
			break;
		}
		case 4: {
			unsigned bytes = (width / 2);
			memset(dst_bits, (index | (index << 4)), bytes);
			//if (bytes % 2) {
			if (bytes & 1) {
				dst_bits[bytes] &= 0x0F;
				dst_bits[bytes] |= (index << 4);
			}
			break;
		}
		case 8: {
			memset(dst_bits, index, FreeImage_GetLine(dib));
			break;
		}
		case 16: {
			WORD wcolor = RGBQUAD_TO_WORD(dib, color_intl);
			for (unsigned x = 0; x < width; x++) {
				((WORD *)dst_bits)[x] = wcolor;
			}
			break;
		}
		case 24: {
			RGBTRIPLE rgbt = *((RGBTRIPLE *)color_intl);
			for (unsigned x = 0; x < width; x++) {
				((RGBTRIPLE *)dst_bits)[x] = rgbt;
			}
			break;
		}
		case 32: {
			RGBQUAD rgbq;
			rgbq.rgbBlue = ((RGBTRIPLE *)color_intl)->rgbtBlue;
			rgbq.rgbGreen = ((RGBTRIPLE *)color_intl)->rgbtGreen;
			rgbq.rgbRed = ((RGBTRIPLE *)color_intl)->rgbtRed;
			rgbq.rgbReserved = 0xFF;
			for (unsigned x = 0; x < width; x++) {
				((RGBQUAD *)dst_bits)[x] = rgbq;
			}
			break;
		}
		default:
			return FALSE;
	}

	// Then, copy the first scanline into all following scanlines.
	// 'src_bits' is a pointer to the first scanline and is already
	// set up correctly.
	if (src_bits) {
		unsigned pitch = FreeImage_GetPitch(dib);
		unsigned bytes = FreeImage_GetLine(dib);
		dst_bits = src_bits + pitch;
		for (unsigned y = 1; y < height; y++) {
			memcpy(dst_bits, src_bits, bytes);
			dst_bits += pitch;
		}
	}
	return TRUE;
}

// --------------------------------------------------------------------------

BOOL DLL_CALLCONV
FreeImage_FillBackground(FIBITMAP *dib, const void *color, int options) {

	if (!dib) {
		return FALSE;
	}
	
	if (!color) {
		return FALSE;
	}

	// handle FIT_BITMAP images with FreeImage_FillBackground()
	if (FreeImage_GetImageType(dib) == FIT_BITMAP) {
		return FillBackgroundBitmap(dib, (RGBQUAD *)color, options);
	}
	
	// first, construct the first scanline (bottom line)
	unsigned bytespp = (FreeImage_GetBPP(dib) / 8);
	BYTE *src_bits = FreeImage_GetScanLine(dib, 0);
	BYTE *dst_bits = src_bits;
	for (unsigned x = 0; x < FreeImage_GetWidth(dib); x++) {
		memcpy(dst_bits, color, bytespp);
		dst_bits += bytespp;
	}

	// then, copy the first scanline into all following scanlines
	unsigned height = FreeImage_GetHeight(dib);
	unsigned pitch = FreeImage_GetPitch(dib);
	unsigned bytes = FreeImage_GetLine(dib);
	dst_bits = src_bits + pitch;
	for (unsigned y = 1; y < height; y++) {
		memcpy(dst_bits, src_bits, bytes);
		dst_bits += pitch;
	}
	return TRUE;
}

// --------------------------------------------------------------------------

// AllocateEx and AllocateExT are needed by FreeImage_EnlargeCanvas. As you
// can see, this is not just an Allocate followed by a FillBackground call for
// palletized images. These functions take an optional palette (or NULL) and ensure,
// that the fill color is present in the palette. 

FIBITMAP * DLL_CALLCONV
FreeImage_AllocateExT(FREE_IMAGE_TYPE type, int width, int height, int bpp, const void *color, int options, const RGBQUAD *palette, unsigned red_mask, unsigned green_mask, unsigned blue_mask) {

	if (!color) {
		return NULL;
	}

	FIBITMAP *bitmap = FreeImage_AllocateT(type, width, height, bpp, red_mask, green_mask, blue_mask);

	if (bitmap != NULL) {
		
		// Only fill the new bitmap if the specified color
		// differs from "black", that is not all bytes of the
		// color are equal to zero.
		switch (bpp) {
			case 1: {
				// although 1-bit implies FIT_BITMAP, better get an unsigned 
				// color and palette
				unsigned *urgb = (unsigned *)color;
				unsigned *upal = (unsigned *)FreeImage_GetPalette(bitmap);
				RGBQUAD rgbq;

				if (palette != NULL) {
					// clone the specified palette
					memcpy(FreeImage_GetPalette(bitmap), palette, 2 * sizeof(RGBQUAD));
				} else {
					// check, whether the specified color is either black or white
					if ((*urgb & 0xFFFFFF) == 0x000000) {
						// in any case build a FIC_MINISBLACK palette
						upal[0] = 0x00000000;
						upal[1] = 0x00FFFFFF;
						*((unsigned *)(&rgbq)) = 0x00000000;
						color = &rgbq;
					} else if ((*urgb & 0xFFFFFF) == 0xFFFFFF) {
						// in any case build a FIC_MINISBLACK palette
						upal[0] = 0x00000000;
						upal[1] = 0x00FFFFFF;
						*((unsigned *)(&rgbq)) = 0x01000000;
						color = &rgbq;
					} else {
						// Otherwise inject the specified color into the so far
						// black-only palette. We use color->rgbReserved as the
						// desired palette index.
						BYTE index = ((RGBQUAD *)color)->rgbReserved & 0x01;
						upal[index] = *urgb & 0x00FFFFFF;  
					}
					options |= FI_COLOR_ALPHA_IS_INDEX;
				}
				// and defer to FreeImage_FillBackground
				FreeImage_FillBackground(bitmap, color, options);
				break;
			}
			case 4: {
				// 4-bit implies FIT_BITMAP so, get a RGBQUAD color
				RGBQUAD *rgb = (RGBQUAD *)color;
				RGBQUAD *pal = FreeImage_GetPalette(bitmap);
				RGBQUAD rgbq;
				
				if (palette != NULL) {
					// clone the specified palette
					memcpy(pal, palette, 16 * sizeof(RGBQUAD));
				} else {
					// check, whether the specified color is a grey one
					if ((rgb->rgbRed == rgb->rgbGreen) && (rgb->rgbRed == rgb->rgbBlue)) {
						// if so, build a greyscale palette
						for (int i = 0; i < 16; i++) {
							((int *)pal)[i] = 0x00111111 * i;
						}
						*((unsigned *)(&rgbq)) = rgb->rgbRed << 24;
						color = &rgbq;
					} else {
						// Otherwise inject the specified color into the so far
						// black-only palette. We use color->rgbReserved as the
						// desired palette index.
						BYTE index = (rgb->rgbReserved & 0x0F);
						((unsigned *)pal)[index] = *((unsigned *)rgb) & 0x00FFFFFF;
					}
					options |= FI_COLOR_ALPHA_IS_INDEX;
				}
				// and defer to FreeImage_FillBackground
				FreeImage_FillBackground(bitmap, rgb, options);
				break;
			}
			case 8: {
				// 8-bit implies FIT_BITMAP so, get a RGBQUAD color
				RGBQUAD *rgb = (RGBQUAD *)color;
				RGBQUAD *pal = FreeImage_GetPalette(bitmap);
				RGBQUAD rgbq;

				if (palette != NULL) {
					// clone the specified palette
					memcpy(pal, palette, 256 * sizeof(RGBQUAD));
				} else {
					// check, whether the specified color is a grey one
					if ((rgb->rgbRed == rgb->rgbGreen) && (rgb->rgbRed == rgb->rgbBlue)) {
						// if so, build a greyscale palette
						for (int i = 0; i < 256; i++) {
							((int *)pal)[i] = 0x00010101 * i;
						}
						*((unsigned *)(&rgbq)) = rgb->rgbRed << 24;
						color = &rgbq;
					} else {
						// Otherwise inject the specified color into the so far
						// black-only palette. We use color->rgbReserved as the
						// desired palette index.
						BYTE index = rgb->rgbReserved;
						((unsigned *)pal)[index] = *((unsigned *)rgb) & 0x00FFFFFF;  
					}
					options |= FI_COLOR_ALPHA_IS_INDEX;
				}
				// and defer to FreeImage_FillBackground
				FreeImage_FillBackground(bitmap, rgb, options);
				break;
			}
			case 16: {
				WORD wcolor = (type == FIT_BITMAP) ?
					RGBQUAD_TO_WORD(bitmap, ((RGBQUAD *)color)) : *((WORD *)color);
				if (wcolor != 0) {
					FreeImage_FillBackground(bitmap, color, options);
				}
				break;
			}
			default: {
				int bytespp = bpp / 8;
				for (int i = 0; i < bytespp; i++) {
					if (((BYTE *)color)[i] != 0) {
						FreeImage_FillBackground(bitmap, color, options);
						break;
					}
				}
				break;
			}
		}
	}
	return bitmap;
}

FIBITMAP * DLL_CALLCONV
FreeImage_AllocateEx(int width, int height, int bpp, const RGBQUAD *color, int options, const RGBQUAD *palette, unsigned red_mask, unsigned green_mask, unsigned blue_mask) {
	return FreeImage_AllocateExT(FIT_BITMAP, width, height, bpp, ((void *)color), options, palette, red_mask, green_mask, blue_mask);
}

// --------------------------------------------------------------------------

FIBITMAP * DLL_CALLCONV
FreeImage_EnlargeCanvas(FIBITMAP *src, int left, int top, int right, int bottom, const void *color, int options) {

	if ((!src) || (!color)) {
		return NULL;
	}

	if ((left == 0) && (right == 0) && (top == 0) && (bottom == 0)) {
		return FreeImage_Clone(src);
	}

	int width = FreeImage_GetWidth(src);
	int height = FreeImage_GetHeight(src);

	if (((left < 0) && (-left >= width)) || ((right < 0) && (-right >= width)) ||
		((top < 0) && (-top >= height)) || ((bottom < 0) && (-bottom >= height))) {
		return NULL;
	}

	unsigned newWidth = width + left + right;
	unsigned newHeight = height + top + bottom;

	FREE_IMAGE_TYPE type = FreeImage_GetImageType(src);
	unsigned bpp = FreeImage_GetBPP(src);

	FIBITMAP *dst = FreeImage_AllocateExT(
		type, newWidth, newHeight, bpp, color, options,
		FreeImage_GetPalette(src),
		FreeImage_GetRedMask(src),
		FreeImage_GetGreenMask(src),
		FreeImage_GetBlueMask(src));

	if (!dst) {
		return NULL;
	}

	int bytespp = bpp / 8;
	BYTE *srcPtr = FreeImage_GetScanLine(src, height - 1 - ((top >= 0) ? 0 : -top));
	BYTE *dstPtr = FreeImage_GetScanLine(dst, newHeight - 1 - ((top <= 0) ? 0 : top));

	unsigned srcPitch = FreeImage_GetPitch(src);
	unsigned dstPitch = FreeImage_GetPitch(dst);

	int lineWidth;
	int lines = height + MIN(0, top) + MIN(0, bottom);

	if ((type == FIT_BITMAP) && (bpp <= 4)) {
		FIBITMAP *copy = FreeImage_Copy(src,
			((left >= 0) ? 0 : -left),
			((top >= 0) ? 0 : -top),
			width - 1 - ((right >= 0) ? 0 : -top),
			height - 1 - ((bottom >= 0) ? 0 : -bottom));
		
		if (!copy) {
			FreeImage_Unload(dst);
			return NULL;
		}

		if (!FreeImage_Paste(dst, copy,
				((left <= 0) ? 0 : left),
				((top <= 0) ? 0 : top), 256)) {
			FreeImage_Unload(copy);
			FreeImage_Unload(dst);
			return NULL;
		}

		FreeImage_Unload(copy);

	} else {

		lineWidth = bytespp * (width + MIN(0, left) + MIN(0, right));

		if (left <= 0) {
			srcPtr += (-left * bytespp);
		} else {
			dstPtr += (left * bytespp);
		}

		for (int i = 0; i < lines; i++) {
			memcpy(dstPtr, srcPtr, lineWidth);
			srcPtr -= srcPitch;
			dstPtr -= dstPitch;
		}
	}

	// copy metadata from src to dst
	FreeImage_CloneMetadata(dst, src);
	
	// copy transparency table 
	FreeImage_SetTransparencyTable(dst, FreeImage_GetTransparencyTable(src), FreeImage_GetTransparencyCount(src));
	
	// copy background color 
	RGBQUAD bkcolor; 
	if( FreeImage_GetBackgroundColor(src, &bkcolor) ) {
		FreeImage_SetBackgroundColor(dst, &bkcolor); 
	}
	
	// clone resolution 
	FreeImage_SetDotsPerMeterX(dst, FreeImage_GetDotsPerMeterX(src)); 
	FreeImage_SetDotsPerMeterY(dst, FreeImage_GetDotsPerMeterY(src)); 
	
	// clone ICC profile 
	FIICCPROFILE *src_profile = FreeImage_GetICCProfile(src); 
	FIICCPROFILE *dst_profile = FreeImage_CreateICCProfile(dst, src_profile->data, src_profile->size); 
	dst_profile->flags = src_profile->flags; 

	return dst;
}

