// ==========================================================
// TIFF Loader and Writer
//
// Design and implementation by 
// - Floris van den Berg (flvdberg@wxs.nl)
// - Hervé Drolon (drolon@infonie.fr)
// - Markus Loibl (markus.loibl@epost.de)
// - Luca Piergentili (l.pierge@terra.es)
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

#ifdef unix
#undef unix
#endif
#ifdef __unix
#undef __unix
#endif

#include "../LibTIFF/tiffiop.h"

#include "FreeImage.h"
#include "Utilities.h"

// ==========================================================
// Plugin Interface
// ==========================================================

static FreeImageIO *g_io = NULL;

static int s_format_id;

// ----------------------------------------------------------
//   libtiff interface 
// ----------------------------------------------------------

static tsize_t 
_tiffReadProc(thandle_t file, tdata_t buf, tsize_t size) {
	return g_io->read_proc(buf, size, 1, (fi_handle)file) * size;
}

static tsize_t
_tiffWriteProc(thandle_t file, tdata_t buf, tsize_t size) {
	return g_io->write_proc(buf, size, 1, (fi_handle)file) * size;
}

static toff_t
_tiffSeekProc(thandle_t file, toff_t off, int whence) {
	g_io->seek_proc((fi_handle)file, off, whence);
	return g_io->tell_proc((fi_handle)file);
}

static int
_tiffCloseProc(thandle_t fd) {
	return 0;
}

#include <sys/stat.h>

static toff_t
_tiffSizeProc(thandle_t file) {
	struct stat sb;
	return (fstat((int) file, &sb) < 0 ? 0 : sb.st_size);
}

static int
_tiffMapProc(thandle_t fd, tdata_t* pbase, toff_t* psize) {
	return 0;
}

static void
_tiffUnmapProc(thandle_t fd, tdata_t base, toff_t size) {
}

// ----------------------------------------------------------
//   Open a TIFF file descriptor for read/writing.
// ----------------------------------------------------------

TIFF *
TIFFFdOpen(thandle_t handle, const char *name, const char *mode) {
	TIFF *tif;

	tif = TIFFClientOpen(name, mode, handle,
	    _tiffReadProc, _tiffWriteProc, _tiffSeekProc, _tiffCloseProc,
	    _tiffSizeProc, _tiffMapProc, _tiffUnmapProc);

	if (tif)
		tif->tif_fd = (int)handle;

	return tif;
}

// ----------------------------------------------------------
//   Open a TIFF file for read/writing.
// ----------------------------------------------------------

TIFF*
TIFFOpen(const char* name, const char* mode) {
	return 0;
}

tdata_t
_TIFFmalloc(tsize_t s) {
	return malloc(s);
}

void
_TIFFfree(tdata_t p) {
	free(p);
}

tdata_t
_TIFFrealloc(tdata_t p, tsize_t s) {
	return realloc(p, s);
}

void
_TIFFmemset(tdata_t p, int v, tsize_t c) {
	memset(p, v, (size_t) c);
}

void
_TIFFmemcpy(tdata_t d, const tdata_t s, tsize_t c) {
	memcpy(d, s, (size_t) c);
}

int
_TIFFmemcmp(const tdata_t p1, const tdata_t p2, tsize_t c) {
	return (memcmp(p1, p2, (size_t) c));
}

// ----------------------------------------------------------
//   in FreeImage warnings and errors are disabled
// ----------------------------------------------------------

static void
msdosWarningHandler(const char* module, const char* fmt, va_list ap) {
}

TIFFErrorHandler _TIFFwarningHandler = msdosWarningHandler;

static void
msdosErrorHandler(const char* module, const char* fmt, va_list ap) {
	/*
	if (module != NULL) {
		char msg[1024];
		vsprintf(msg, fmt, ap);
		FreeImage_OutputMessageProc(s_format_id, "%s%s", module, msg);
	}
	*/
}

TIFFErrorHandler _TIFFerrorHandler = msdosErrorHandler;

// ----------------------------------------------------------

#define CVT(x)      (((x) * 255L) / ((1L<<16)-1))
#define	SCALE(x)	(((x)*((1L<<16)-1))/255)

// ==========================================================
// Internal functions
// ==========================================================

static uint16
CheckColormap(int n, uint16* r, uint16* g, uint16* b) {
    while (n-- > 0) {
        if (*r++ >= 256 || *g++ >= 256 || *b++ >= 256) {
			return 16;
		}
	}

    return 8;
}

static uint16
CheckPhotometric(FIBITMAP *dib, uint16 bitspersample) {
	RGBQUAD *rgb;
	uint16 i;

	switch(bitspersample) {
		case 1:
		{
			rgb = FreeImage_GetPalette(dib);

			if ((rgb->rgbRed == 0) && (rgb->rgbGreen == 0) && (rgb->rgbBlue == 0)) {
				rgb++;

				if ((rgb->rgbRed == 255) && (rgb->rgbGreen == 255) && (rgb->rgbBlue == 255))
					return PHOTOMETRIC_MINISBLACK;				
			}

			if ((rgb->rgbRed == 255) && (rgb->rgbGreen == 255) && (rgb->rgbBlue == 255)) {
				rgb++;

				if ((rgb->rgbRed == 0) && (rgb->rgbGreen == 0) && (rgb->rgbBlue == 0))
					return PHOTOMETRIC_MINISWHITE;				
			}

			return PHOTOMETRIC_PALETTE;
		}

		case 4:	// Check if the DIB has a color or a greyscale palette
		case 8:
			rgb = FreeImage_GetPalette(dib);

			for (i = 0; i < FreeImage_GetColorsUsed(dib); i++) {
				if ((rgb->rgbRed != rgb->rgbGreen) || (rgb->rgbRed != rgb->rgbBlue))
					return PHOTOMETRIC_PALETTE;
				
				// The DIB has a color palette if the greyscale isn't a linear ramp

				if (rgb->rgbRed != i)
					return PHOTOMETRIC_PALETTE;				

				rgb++;
			}

			return PHOTOMETRIC_MINISBLACK;
			
		case 16:
			return PHOTOMETRIC_MINISBLACK;			

		case 24:
		case 32:
			return PHOTOMETRIC_RGB;			
	}

	return PHOTOMETRIC_MINISBLACK;
}

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "TIFF";
}

static const char * DLL_CALLCONV
Description() {
	return "Tagged Image File Format";
}

static const char * DLL_CALLCONV
Extension() {
	return "tif,tiff";
}

static const char * DLL_CALLCONV
RegExpr() {
	return "^[MI][MI][\\x01*][\\x01*]";
}

static const char * DLL_CALLCONV
MimeType() {
	return "image/tiff";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {	
	BYTE tiff_id1[] = { 0x49, 0x49, 0x2A, 0x00 };
	BYTE tiff_id2[] = { 0x4D, 0x4D, 0x00, 0x2A };
	BYTE signature[4] = { 0, 0, 0, 0 };

	io->read_proc(signature, 1, 4, handle);

	if(memcmp(tiff_id1, signature, 4) == 0)
		return TRUE;

	if(memcmp(tiff_id2, signature, 4) == 0)
		return TRUE;

	return FALSE;
}

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return (
			(depth == 1)  ||
			(depth == 4)  ||
			(depth == 8)  ||
			(depth == 24) ||
			(depth == 32)
		);
}

static BOOL DLL_CALLCONV 
SupportsExportType(FREE_IMAGE_TYPE type) {
	return (
		(type == FIT_BITMAP) ||
		(type == FIT_UINT16) ||
		(type == FIT_INT16)  ||
		(type == FIT_UINT32) ||
		(type == FIT_INT32)  ||
		(type == FIT_FLOAT)  ||
		(type == FIT_DOUBLE) ||
		(type == FIT_COMPLEX)
	);
}

static BOOL DLL_CALLCONV
SupportsICCProfiles() {
	return TRUE;
}

// ----------------------------------------------------------

static void * DLL_CALLCONV
Open(FreeImageIO *io, fi_handle handle, BOOL read) {
	g_io = io;

	if (read)
		return TIFFFdOpen((thandle_t)handle, "", "r");
	else
		return TIFFFdOpen((thandle_t)handle, "", "w");
}

static void DLL_CALLCONV
Close(FreeImageIO *io, fi_handle handle, void *data) {
	if(data)
		TIFFClose((TIFF *)data);
	g_io = NULL;
}

// ----------------------------------------------------------

static int DLL_CALLCONV
PageCount(FreeImageIO *io, fi_handle handle, void *data) {
	int nr_ifd = 0;

	TIFF *tif = (TIFF *)data;

	do {
		nr_ifd++;
	} while (TIFFReadDirectory(tif));
			
	return nr_ifd;
}

// ----------------------------------------------------------

static FIBITMAP* CreateImageType(FREE_IMAGE_TYPE fit, int width, int height, uint16 bitspersample, uint16 samplesperpixel) {
	FIBITMAP *dib = NULL;

	uint16 bpp = bitspersample * samplesperpixel;

	if(fit == FIT_BITMAP) {
		// standard bitmap type 

		if(bitspersample == 16) {
			if(samplesperpixel == 1) {
				// 16-bit greyscale -> convert to 8-bit
				dib = FreeImage_Allocate(width, height, 8);
			}
			else if(samplesperpixel = 3) {
				// 48-bit RGB -> convert to 24-bit RGB
				dib = FreeImage_Allocate(width, height, 24, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
			}
		}
		else if(bpp == 16) {
			// 16-bit RGB -> expect it to be 565
			dib = FreeImage_Allocate(width, height, bpp, FI16_565_RED_MASK, FI16_565_GREEN_MASK, FI16_565_BLUE_MASK);
		}
		else if(bpp == 24) {
			// BGR
			dib = FreeImage_Allocate(width, height, bpp, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
		}
		else if(bpp == 32) {
			// BGRA
			dib = FreeImage_Allocate(width, height, bpp, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
		}
		else {
			// anything else <= 8-bit
			dib = FreeImage_Allocate(width, height, bpp);
		}
	} else {
		// other bitmap types
		
		dib = FreeImage_AllocateT(fit, width, height, bpp);
	}

	return dib;
}

/** 
Read the TIFFTAG_SAMPLEFORMAT tag and convert to FREE_IMAGE_TYPE
@param tiff LibTIFF TIFF Handle
@param bitspersample # bit per sample
@param samplesperpixel # samples per pixel
@return Returns the image type as a FREE_IMAGE_TYPE value
*/
static FREE_IMAGE_TYPE GetImageType(TIFF *tiff, uint16 bitspersample, uint16 samplesperpixel) {
	uint16 sampleformat = 0;
	FREE_IMAGE_TYPE fit = FIT_BITMAP;

	// try the sampleformat tag
    if(TIFFGetField(tiff, TIFFTAG_SAMPLEFORMAT, &sampleformat)) {

        switch (sampleformat) {
			case SAMPLEFORMAT_UINT:
				switch (bitspersample) {
					case 1:
					case 4:
					case 8:
					case 24:
						fit = FIT_BITMAP;
						break;
					case 16:
						if(samplesperpixel == 3) {
							fit = FIT_BITMAP;
						} else {
							fit = FIT_UINT16;
						}
						break;

					case 32:
						if(samplesperpixel == 4) {
							fit = FIT_BITMAP;
						} else {
							fit = FIT_UINT32;
						}
						break;
				}
				break;

			case SAMPLEFORMAT_INT:
				switch (bitspersample) {
					case 16:
						if(samplesperpixel == 3) {
							fit = FIT_BITMAP;
						} else {
							fit = FIT_INT16;
						}
						break;
					case 32:
						fit = FIT_INT32;
						break;
				}
				break;

			case SAMPLEFORMAT_IEEEFP:
				switch (bitspersample) {
					case 32:
						fit = FIT_FLOAT;
						break;
					case 64:
						fit = FIT_DOUBLE;
						break;
				}
				break;
			case SAMPLEFORMAT_COMPLEXIEEEFP:
				switch (bitspersample) {
					case 64:
						break;
					case 128:
						fit = FIT_COMPLEX;
						break;
				}
				break;

			}
    }
	// no sampleformat tag : assume SAMPLEFORMAT_UINT
	else {
		if(samplesperpixel == 1) {
			switch (bitspersample) {
				case 16:
					fit = FIT_UINT16;
					break;
					
				case 32:
					fit = FIT_UINT32;
					break;
			}
		}
	}

    return fit;
}

/** 
Convert FREE_IMAGE_TYPE and write TIFFTAG_SAMPLEFORMAT
@param tiff LibTIFF TIFF Handle
@param fit Image type as a FREE_IMAGE_TYPE value
*/
static void SetImageType(TIFF *tiff, FREE_IMAGE_TYPE fit) {
	switch(fit) {
		case FIT_BITMAP:	// standard image: 1-, 4-, 8-, 16-, 24-, 32-bit
		case FIT_UINT16:	// array of unsigned short	: unsigned 16-bit
		case FIT_UINT32:	// array of unsigned long	: unsigned 32-bit
			TIFFSetField(tiff, TIFFTAG_SAMPLEFORMAT, SAMPLEFORMAT_UINT);
			break;

		case FIT_INT16:		// array of short	: signed 16-bit
		case FIT_INT32:		// array of long	: signed 32-bit
			TIFFSetField(tiff, TIFFTAG_SAMPLEFORMAT, SAMPLEFORMAT_INT);
			break;

		case FIT_FLOAT:		// array of float	: 32-bit
		case FIT_DOUBLE:	// array of double	: 64-bit
			TIFFSetField(tiff, TIFFTAG_SAMPLEFORMAT, SAMPLEFORMAT_IEEEFP);
			break;

		case FIT_COMPLEX:	// array of COMPLEX : 2 x 64-bit
			TIFFSetField(tiff, TIFFTAG_SAMPLEFORMAT, SAMPLEFORMAT_COMPLEXIEEEFP);
			break;
	}
}

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	if ((handle != NULL) && (data != NULL)) {
		TIFF   *tif = NULL;
		uint32 height = 0; 
		uint32 width = 0; 
		uint16 bitspersample = 1;
		uint16 samplesperpixel = 1;
		uint32 rowsperstrip;  
		uint16 photometric = PHOTOMETRIC_MINISWHITE;
		uint16 compression = COMPRESSION_NONE;
		uint32 x = 0;
		uint32 y = 0;

		int32 nrow;
		BYTE *buf = NULL;          
		FIBITMAP *dib = NULL;
		BYTE *bits = NULL;		// pointer to dib data
		RGBQUAD *pal = NULL;	// pointer to dib palette
		uint32 iccSize = 0;		// ICC profile length
		void *iccBuf = NULL;	// ICC profile data
    
		try {			
			tif = (TIFF *)data;

			if (page != -1)
				if (!tif || !TIFFSetDirectory(tif, page))
					throw "Error encountered while opening TIFF file";			

			TIFFGetField(tif, TIFFTAG_COMPRESSION, &compression);
			TIFFGetField(tif, TIFFTAG_IMAGEWIDTH, &width);
			TIFFGetField(tif, TIFFTAG_IMAGELENGTH, &height);
			TIFFGetField(tif, TIFFTAG_SAMPLESPERPIXEL, &samplesperpixel);
			TIFFGetField(tif, TIFFTAG_BITSPERSAMPLE, &bitspersample);
			TIFFGetField(tif, TIFFTAG_ROWSPERSTRIP, &rowsperstrip);   
			TIFFGetField(tif, TIFFTAG_PHOTOMETRIC, &photometric);
			TIFFGetField(tif, TIFFTAG_ICCPROFILE, &iccSize, &iccBuf);

			// get image data type

			FREE_IMAGE_TYPE image_type = GetImageType(tif, bitspersample, samplesperpixel);

			if (compression == COMPRESSION_LZW)
				throw "LZW compression is no longer supported due to Unisys patent enforcement";

			if (compression == COMPRESSION_OJPEG)
				throw "6.0 JPEG encoding is not supported";

			if((photometric == PHOTOMETRIC_SEPARATED) && (bitspersample == 16))
				throw "Unable to handle 16-bit CMYK TIFF";

			// Convert to 24 or 32 bits RGB if the image is full color

			BOOL isRGB = (bitspersample >= 8) &&
				(photometric == PHOTOMETRIC_RGB) ||
				(photometric == PHOTOMETRIC_YCBCR) ||
				(photometric == PHOTOMETRIC_SEPARATED) ||
				(photometric == PHOTOMETRIC_LOGLUV);

			// if image is PHOTOMETRIC_SEPARATED _and_ comes with an ICC profile, 
			// then the image should preserve its original (CMYK) colour model and 
			// should be read as CMYK (to keep the match of pixel and profile and 
			// to avoid multiple conversions. Conversion can be done by changing 
			// the profile from it's original CMYK to an RGB profile with an 
			// apropriate color management system. Works with non-tiled TIFFs.

			if (photometric == PHOTOMETRIC_SEPARATED &&	
				((flags & TIFF_CMYK) == TIFF_CMYK) && !TIFFIsTiled(tif))
				isRGB = false;

			if (isRGB) {

				// Read the whole image into one big RGBA buffer and then 
				// convert it to a DIB. This is using the traditional
				// TIFFReadRGBAImage() API that we trust.

				uint32 *raster = (uint32*)_TIFFmalloc(width * height * sizeof(uint32));

				if (raster == NULL)
					throw "No space for raster buffer";

				// read the image in one chunk into an RGBA array

				if (!TIFFReadRGBAImage(tif, width, height, raster, 0)) {
					_TIFFfree(raster);
					return NULL;
				}

				// create a new DIB

				dib = CreateImageType(image_type, width, height, bitspersample, samplesperpixel);
				if (dib == NULL) {
					// free the raster pointer and output an error if allocation failed

					_TIFFfree(raster);

					throw "DIB allocation failed";
				}
				
				// fill in the metrics (english or universal)

				float fResX = 300.0;
				float fResY = 300.0;
				uint16 resUnit = RESUNIT_INCH;

				TIFFGetField(tif, TIFFTAG_RESOLUTIONUNIT, &resUnit);
				TIFFGetField(tif, TIFFTAG_XRESOLUTION, &fResX);
				TIFFGetField(tif, TIFFTAG_YRESOLUTION, &fResY);

				BITMAPINFOHEADER *pInfoHeader = FreeImage_GetInfoHeader(dib);

				if (resUnit == RESUNIT_INCH) {
					pInfoHeader->biXPelsPerMeter = (int) (fResX/0.0254000 + 0.5);
					pInfoHeader->biYPelsPerMeter = (int) (fResY/0.0254000 + 0.5);
				} else if(resUnit == RESUNIT_CENTIMETER) {
					pInfoHeader->biXPelsPerMeter = (int) (fResX*100.0 + 0.5);
					pInfoHeader->biYPelsPerMeter = (int) (fResY*100.0 + 0.5);
				}

				// read the raster lines and save them in the DIB
				// with RGB mode, we have to change the order of the 3 samples RGB
				// We use macros for extracting components from the packed ABGR 
				// form returned by TIFFReadRGBAImage.

				BOOL has_alpha = FALSE;
				uint32 *row = &raster[0];

				for (y = 0; y < height; y++) {
					bits = FreeImage_GetScanLine(dib, y);

					if (samplesperpixel == 4) {
						for (x = 0; x < width; x++) {
							bits[FI_RGBA_BLUE]	= (BYTE)TIFFGetB(row[x]);
							bits[FI_RGBA_GREEN] = (BYTE)TIFFGetG(row[x]);
							bits[FI_RGBA_RED]	= (BYTE)TIFFGetR(row[x]);
							bits[FI_RGBA_ALPHA] = (BYTE)TIFFGetA(row[x]);

							if (bits[FI_RGBA_ALPHA] != 0)
								has_alpha = TRUE;								

							bits += 4;
						}
					} else {
						for (x = 0; x < width; x++) {
							bits[FI_RGBA_BLUE]	= (BYTE)TIFFGetB(row[x]);
							bits[FI_RGBA_GREEN] = (BYTE)TIFFGetG(row[x]);
							bits[FI_RGBA_RED]	= (BYTE)TIFFGetR(row[x]);

							bits += 3;
						}
					}

					row += width;
				}

				FreeImage_SetTransparent(dib, has_alpha);

				_TIFFfree(raster);

			} else {
				// calculate the line + pitch

				int line = CalculateLine(width, bitspersample * samplesperpixel);
				int pitch = CalculatePitch(line);

				// create a new DIB
				// ----------------

				dib = CreateImageType(image_type, width, height, bitspersample, samplesperpixel);
				if (dib == NULL) {
					throw "No space for DIB image";
				}

				// fill metrics

				float fResX = 300.0;
				float fResY = 300.0;
				uint16 resUnit = RESUNIT_INCH;

				TIFFGetField(tif, TIFFTAG_RESOLUTIONUNIT, &resUnit);
				TIFFGetField(tif, TIFFTAG_XRESOLUTION, &fResX);
				TIFFGetField(tif, TIFFTAG_YRESOLUTION, &fResY);

				if (resUnit == RESUNIT_INCH) {
					BITMAPINFOHEADER *pInfoHeader = FreeImage_GetInfoHeader(dib);
					pInfoHeader->biXPelsPerMeter = (int) (fResX/0.0254000 + 0.5);
					pInfoHeader->biYPelsPerMeter = (int) (fResY/0.0254000 + 0.5);
				} else if (resUnit == RESUNIT_CENTIMETER) {
					BITMAPINFOHEADER *pInfoHeader = FreeImage_GetInfoHeader(dib);
					pInfoHeader->biXPelsPerMeter = (int) (fResX * 100.0 + 0.5);
					pInfoHeader->biYPelsPerMeter = (int) (fResY * 100.0 + 0.5);
				}

				// set up the colormap based on photometric	

				pal = FreeImage_GetPalette(dib);

				switch(photometric) {
					case PHOTOMETRIC_MINISBLACK:	// bitmap and greyscale image types
					case PHOTOMETRIC_MINISWHITE:
						// Monochrome image

						if (bitspersample == 1) {
							if (photometric == PHOTOMETRIC_MINISWHITE) {
								pal[0].rgbRed = pal[0].rgbGreen = pal[0].rgbBlue = 255;
								pal[1].rgbRed = pal[1].rgbGreen = pal[1].rgbBlue = 0;
							} else {
								pal[0].rgbRed = pal[0].rgbGreen = pal[0].rgbBlue = 0;
								pal[1].rgbRed = pal[1].rgbGreen = pal[1].rgbBlue = 255;
							}

						} else if ((bitspersample == 4) ||(bitspersample == 8)) {
							// need to build the scale for greyscale images
							int ncolors = FreeImage_GetColorsUsed(dib);

							if (photometric == PHOTOMETRIC_MINISBLACK) {
								for (int i = 0; i < ncolors; i++) {
									pal[i].rgbRed	=
									pal[i].rgbGreen =
									pal[i].rgbBlue	= (BYTE)(i*(255/(ncolors-1)));
								}
							} else {
								for (int i = 0; i < ncolors; i++) {
									pal[i].rgbRed	=
									pal[i].rgbGreen =
									pal[i].rgbBlue	= (BYTE)(255-i*(255/(ncolors-1)));
								}
							}
						}

						break;

					case PHOTOMETRIC_PALETTE:	// color map indexed
						uint16 *red;
						uint16 *green;
						uint16 *blue;
						
						TIFFGetField(tif, TIFFTAG_COLORMAP, &red, &green, &blue); 

						// load the palette in the DIB

						if (CheckColormap(1<<bitspersample, red, green, blue) == 16) {
							for (int i = (1 << bitspersample) - 1; i >= 0; i--) {
								pal[i].rgbRed =(BYTE) CVT(red[i]);
								pal[i].rgbGreen = (BYTE) CVT(green[i]);
								pal[i].rgbBlue = (BYTE) CVT(blue[i]);           
							}
						} else {
							for (int i = (1 << bitspersample) - 1; i >= 0; i--) {
								pal[i].rgbRed = (BYTE) red[i];
								pal[i].rgbGreen = (BYTE) green[i];
								pal[i].rgbBlue = (BYTE) blue[i];        
							}
						}

						break;						
				}

				// In the tiff file the lines are save from up to down 
				// In a DIB the lines must be saved from down to up

				bits = FreeImage_GetBits(dib) + height * FreeImage_GetPitch(dib);

				// read the tiff lines and save them in the DIB

				buf = (BYTE*)malloc(TIFFStripSize(tif) * sizeof(BYTE));

				for (y = 0; y < height; y += rowsperstrip) {
					nrow = (y + rowsperstrip > height ? height - y : rowsperstrip);

					if (TIFFReadEncodedStrip(tif, TIFFComputeStrip(tif, y, 0), buf, nrow * line) == -1) {
						free(buf);
						throw "Parsing error";
					} else {
						// color/greyscale picture (1-, 4-, 8-bit) or special type (int, long, double, ...)
						// ... just copy

						for (int l = 0; l < nrow; l++) {
							bits -= pitch;
							memcpy(bits, buf + l * line, line);
						}
					}
				}
				if (photometric == PHOTOMETRIC_SEPARATED && ((flags & TIFF_CMYK) == TIFF_CMYK)) {
					FreeImage_GetICCProfile(dib)->flags |= FIICC_COLOR_IS_CMYK;
				}
				
				free(buf);
			}

			// copy ICC profile data (must be done after FreeImage_Allocate)

			FreeImage_CreateICCProfile(dib, iccBuf, iccSize);

			return (FIBITMAP *)dib;

		} catch (const char *message) {
			FreeImage_OutputMessageProc(s_format_id, message);

			if(dib)
				FreeImage_Unload(dib);

			return NULL;
		}
	}

	return NULL;	   
}

static BOOL DLL_CALLCONV
Save(FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	if (dib != NULL) {
		TIFF *out = (TIFF *)data;

		int32 height;
		int32 width;
		uint32 rowsperstrip = (uint32) -1;
		uint16 bitspersample;
		uint16 samplesperpixel;
		uint16 photometric;
		uint16 compression;
		uint16 pitch;
#ifndef FREEIMAGE_BIGENDIAN
		int32 x;
#endif
		int32 y;

		FREE_IMAGE_TYPE image_type = FreeImage_GetImageType(dib);

		width = FreeImage_GetWidth(dib);
		height = FreeImage_GetHeight(dib);
		bitspersample = FreeImage_GetBPP(dib);

		FIICCPROFILE *iccProfile = FreeImage_GetICCProfile(dib);

		if(image_type == FIT_BITMAP) {
			// standard image: 1-, 4-, 8-, 16-, 24-, 32-bit

			samplesperpixel = ((bitspersample == 24) ? 3 : ((bitspersample == 32) ? 4 : 1));
			photometric	= CheckPhotometric(dib, bitspersample);

			if(bitspersample == 32) {
				// 32-bit images : check for CMYK or alpha transparency

				if((((iccProfile->flags & FIICC_COLOR_IS_CMYK) == FIICC_COLOR_IS_CMYK) || ((flags & TIFF_CMYK) == TIFF_CMYK))) {
					// CMYK support
					photometric = PHOTOMETRIC_SEPARATED;
					TIFFSetField(out, TIFFTAG_INKSET, INKSET_CMYK);
					TIFFSetField(out, TIFFTAG_NUMBEROFINKS, 4);
				}
				else if(photometric == PHOTOMETRIC_RGB) {
					// transparency mask support
					uint16 sampleinfo[1]; 
					// unassociated alpha data is transparency information
					sampleinfo[0] = EXTRASAMPLE_UNASSALPHA;
					TIFFSetField(out, TIFFTAG_EXTRASAMPLES, 1, sampleinfo);
				}
			}

		} else {
			// special image type (int, long, double, ...)

			samplesperpixel = 1;
			photometric	= PHOTOMETRIC_MINISBLACK;
		}

		// set image data type

		SetImageType(out, image_type);
		
		// write possible ICC profile

		if (iccProfile->size && iccProfile->data) {
			TIFFSetField(out, TIFFTAG_ICCPROFILE, iccProfile->size, iccProfile->data);
		}

		// handle standard width/height/bpp stuff

		TIFFSetField(out, TIFFTAG_IMAGEWIDTH, width);
		TIFFSetField(out, TIFFTAG_IMAGELENGTH, height);
		TIFFSetField(out, TIFFTAG_SAMPLESPERPIXEL, samplesperpixel);
		TIFFSetField(out, TIFFTAG_BITSPERSAMPLE, bitspersample / samplesperpixel);
		TIFFSetField(out, TIFFTAG_PHOTOMETRIC, photometric);
		TIFFSetField(out, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);	// single image plane 
		TIFFSetField(out, TIFFTAG_ORIENTATION, ORIENTATION_TOPLEFT);
		TIFFSetField(out, TIFFTAG_ROWSPERSTRIP, TIFFDefaultStripSize(out, rowsperstrip));

		// handle metrics

		BITMAPINFOHEADER *pInfoHeader = FreeImage_GetInfoHeader(dib);
		TIFFSetField(out, TIFFTAG_RESOLUTIONUNIT, RESUNIT_INCH);

		double res = (unsigned long) (0.5 + 0.0254 * pInfoHeader->biXPelsPerMeter); // rounded ! (99,9998 -> 100)
		TIFFSetField(out, TIFFTAG_XRESOLUTION, res);
		
		res = (unsigned long) (0.5 + 0.0254 * pInfoHeader->biYPelsPerMeter);
		TIFFSetField(out, TIFFTAG_YRESOLUTION, res);

		// multi-paging

		if (page >= 0) {
			char page_number[20];
			sprintf(page_number, "Page %d", page);

			TIFFSetField(out, TIFFTAG_SUBFILETYPE, FILETYPE_PAGE);
			TIFFSetField(out, TIFFTAG_PAGENUMBER, page);
			TIFFSetField(out, TIFFTAG_PAGENAME, page_number);
		} else {
			TIFFSetField(out, TIFFTAG_SUBFILETYPE, 0);
		}

		// palettes (image colormaps are automatically scaled to 16-bits)

		if (photometric == PHOTOMETRIC_PALETTE) {
			uint16 *r, *g, *b;
			uint16 nColors = FreeImage_GetColorsUsed(dib);
			RGBQUAD *pal = FreeImage_GetPalette(dib);

			r = (uint16 *) _TIFFmalloc(sizeof(uint16) * 3 * nColors);
			g = r + nColors;
			b = g + nColors;

			for (int i = nColors - 1; i >= 0; i--) {
				r[i] = SCALE((uint16)pal[i].rgbRed);
				g[i] = SCALE((uint16)pal[i].rgbGreen);
				b[i] = SCALE((uint16)pal[i].rgbBlue);
			}

			TIFFSetField(out, TIFFTAG_COLORMAP, r, g, b);

			_TIFFfree(r);
		}

		// compression

		switch(bitspersample) {
			case 1 :
				compression = COMPRESSION_CCITTFAX4;
				break;

			case 4 :
			case 8 :
			case 16 :
			case 24 :
			case 32 :
			case 64 :
			case 128:
				compression = COMPRESSION_PACKBITS;

				if ((flags & TIFF_PACKBITS) == TIFF_PACKBITS)
					compression = COMPRESSION_PACKBITS;
				else if ((flags & TIFF_DEFLATE) == TIFF_DEFLATE)
					compression = COMPRESSION_DEFLATE;
				else if ((flags & TIFF_ADOBE_DEFLATE) == TIFF_ADOBE_DEFLATE)
					compression = COMPRESSION_ADOBE_DEFLATE;
				else if ((flags & TIFF_NONE) == TIFF_NONE)
					compression = COMPRESSION_NONE;

				break;

			default :
				compression = COMPRESSION_NONE;
				break;
		}

		TIFFSetField(out, TIFFTAG_COMPRESSION, compression);

		// read the DIB lines from bottom to top
		// and save them in the TIF
		// -------------------------------------
		
		pitch = FreeImage_GetPitch(dib);

		if(image_type == FIT_BITMAP) {
			// standard bitmap type
		
			switch(bitspersample) {
				case 1 :
				case 4 :
				case 8 :
				{
					for (y = height - 1; y >= 0; y--) {
						BYTE *bits = FreeImage_GetScanLine(dib, y);

						TIFFWriteScanline(out, bits, height - y - 1, 0);
					}

					break;
				}				

				case 24:
				case 32:
				{
					BYTE *buffer = (BYTE *)malloc(pitch * sizeof(BYTE));

					for (y = 0; y < height; y++) {
						// get a copy of the scanline

						memcpy(buffer, FreeImage_GetScanLine(dib, height - y - 1), pitch);

#ifndef FREEIMAGE_BIGENDIAN
						if (photometric != PHOTOMETRIC_SEPARATED) {
							// TIFFs store color data RGB(A) instead of BGR(A)
		
							BYTE *pBuf = buffer;
		
							for (x = 0; x < width; x++) {
								INPLACESWAP(pBuf[0], pBuf[2]);
								pBuf += samplesperpixel;
							}
						}
#endif
						// write the scanline to disc

						TIFFWriteScanline(out, buffer, y, 0);
					}

					free(buffer);

					break;
				}
			}

		} else {
			// special bitmap type (int, long, double, etc.)

			switch(bitspersample) {
				case 16:
				case 32:
				case 64:
				case 128:
				{
					for (y = height - 1; y >= 0; y--) {
						BYTE *bits = FreeImage_GetScanLine(dib, y);

						TIFFWriteScanline(out, bits, height - y - 1, 0);
					}

					break;
				}				
			}
		}

		// write out the directory tag if we wrote a page other than -1

		if (page >= 0)
			TIFFWriteDirectory(out);		

		return TRUE;
	}

	return FALSE;
}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitTIFF(Plugin *plugin, int format_id) {
	s_format_id = format_id;

	plugin->format_proc = Format;
	plugin->description_proc = Description;
	plugin->extension_proc = Extension;
	plugin->regexpr_proc = RegExpr;
	plugin->open_proc = Open;
	plugin->close_proc = Close;
	plugin->pagecount_proc = PageCount;
	plugin->pagecapability_proc = NULL;
	plugin->load_proc = Load;
	plugin->save_proc = Save;
	plugin->validate_proc = Validate;
	plugin->mime_proc = MimeType;
	plugin->supports_export_bpp_proc = SupportsExportDepth;
	plugin->supports_export_type_proc = SupportsExportType;
	plugin->supports_icc_profiles_proc = SupportsICCProfiles;
}
