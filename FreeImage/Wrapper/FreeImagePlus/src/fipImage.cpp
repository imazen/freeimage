// ==========================================================
// fipImage class implementation
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

#include "FreeImagePlus.h"

///////////////////////////////////////////////////////////////////   
// Protected functions

BOOL fipImage::replace(FIBITMAP *new_dib) {
	if(new_dib == NULL) 
		return FALSE;
	if(_dib)
		FreeImage_Unload(_dib);
	_dib = new_dib;
	_bHasChanged = TRUE;
	return TRUE;
}

///////////////////////////////////////////////////////////////////
// Creation & Destruction

fipImage::fipImage(FREE_IMAGE_TYPE image_type, WORD width, WORD height, WORD bpp) {
	_dib = NULL;
	_bHasChanged = FALSE;
	if(width && height && bpp)
		setSize(image_type, width, height, bpp);
}

fipImage::~fipImage() {
	if(_dib) {
		FreeImage_Unload(_dib);
	}
}

BOOL fipImage::setSize(FREE_IMAGE_TYPE image_type, WORD width, WORD height, WORD bpp, unsigned red_mask, unsigned green_mask, unsigned blue_mask) {
	if(_dib) {
		FreeImage_Unload(_dib);
	}
	if((_dib = FreeImage_AllocateT(image_type, width, height, bpp, red_mask, green_mask, blue_mask)) == NULL)
		return FALSE;

	if(image_type == FIT_BITMAP) {
		// Create palette if needed
		switch(bpp)	{
			case 1:
			case 4:
			case 8:
				RGBQUAD *pal = FreeImage_GetPalette(_dib);
				for(unsigned i = 0; i < FreeImage_GetColorsUsed(_dib); i++) {
					pal[i].rgbRed = i;
					pal[i].rgbGreen = i;
					pal[i].rgbBlue = i;
				}
				break;
		}
	}

	_bHasChanged = TRUE;

	return TRUE;
}

///////////////////////////////////////////////////////////////////
// Copying

fipImage::fipImage(const fipImage& Image) {
	_dib = NULL;
	FIBITMAP *clone = FreeImage_Clone((FIBITMAP*)Image._dib);
	replace(clone);
}

fipImage& fipImage::operator=(const fipImage& Image) {
	if(this != &Image) {
		FIBITMAP *clone = FreeImage_Clone((FIBITMAP*)Image._dib);
		replace(clone);
	}
	return *this;
}

fipImage& fipImage::operator=(FIBITMAP *dib) {
	replace(dib);
	return *this;
}

BOOL fipImage::copySubImage(fipImage& dst, int left, int top, int right, int bottom) {
	if(_dib) {
		dst = FreeImage_Copy(_dib, left, top, right, bottom);
		return dst.isValid();
	}
	return FALSE;
}

BOOL fipImage::pasteSubImage(fipImage& src, int left, int top, int alpha) {
	if(_dib) {
		BOOL bResult = FreeImage_Paste(_dib, src._dib, left, top, alpha);
		_bHasChanged = TRUE;
		return bResult;
	}
	return FALSE;
}


///////////////////////////////////////////////////////////////////
// Information functions

FREE_IMAGE_TYPE fipImage::getImageType() {
	return FreeImage_GetImageType(_dib);
}

WORD fipImage::getWidth() {
	return FreeImage_GetWidth(_dib); 
}

WORD fipImage::getHeight() {
	return FreeImage_GetHeight(_dib); 
}

WORD fipImage::getScanWidth() {
	return FreeImage_GetPitch(_dib);
}

BOOL fipImage::isValid() {
	return (_dib != NULL) ? TRUE:FALSE;
}

BITMAPINFO* fipImage::getInfo() {
	return FreeImage_GetInfo(_dib);
}

BITMAPINFOHEADER* fipImage::getInfoHeader() {
	return FreeImage_GetInfoHeader(_dib);
}

LONG fipImage::getImageSize() {
	return FreeImage_GetDIBSize(_dib);
}

WORD fipImage::getBitsPerPixel() {
	return FreeImage_GetBPP(_dib);
}

WORD fipImage::getLine() {
	return FreeImage_GetLine(_dib);
}

WORD fipImage::getHorizontalResolution() {
	return (FreeImage_GetDotsPerMeterX(_dib) / 100); 
}

WORD fipImage::getVerticalResolution() {
	return (FreeImage_GetDotsPerMeterY(_dib) / 100);
}

void fipImage::setHorizontalResolution(LONG value) {
	BITMAPINFOHEADER *bmih = FreeImage_GetInfoHeader(_dib);
	bmih->biXPelsPerMeter = value * 100;
}

void fipImage::setVerticalResolution(LONG value) {
	BITMAPINFOHEADER *bmih = FreeImage_GetInfoHeader(_dib);
	bmih->biYPelsPerMeter = value * 100;
}


///////////////////////////////////////////////////////////////////
// Palette operations

RGBQUAD* fipImage::getPalette() {
	return FreeImage_GetPalette(_dib);
}

WORD fipImage::getPaletteSize() {
	return FreeImage_GetColorsUsed(_dib) * sizeof(RGBQUAD);
}

WORD fipImage::getColorsUsed() {
	return FreeImage_GetColorsUsed(_dib);
}

FREE_IMAGE_COLOR_TYPE fipImage::getColorType() { 
	return FreeImage_GetColorType(_dib);
}

BOOL fipImage::isGrayscale() {
	return ((FreeImage_GetBPP(_dib) == 8) && (FreeImage_GetColorType(_dib) != FIC_PALETTE)); 
}

///////////////////////////////////////////////////////////////////
// Pixel access

BYTE* fipImage::accessPixels() {
	return FreeImage_GetBits(_dib); 
}

BYTE* fipImage::getScanLine(WORD scanline) {
	if(scanline < FreeImage_GetHeight(_dib)) {
		return FreeImage_GetScanLine(_dib, scanline);
	}
	return NULL;
}

BOOL fipImage::getPixelIndex(unsigned x, unsigned y, BYTE *value) {
	return FreeImage_GetPixelIndex(_dib, x, y, value);
}

BOOL fipImage::getPixelColor(unsigned x, unsigned y, RGBQUAD *value) {
	return FreeImage_GetPixelColor(_dib, x, y, value);
}

BOOL fipImage::setPixelIndex(unsigned x, unsigned y, BYTE *value) {
	_bHasChanged = TRUE;
	return FreeImage_SetPixelIndex(_dib, x, y, value);
}

BOOL fipImage::setPixelColor(unsigned x, unsigned y, RGBQUAD *value) {
	_bHasChanged = TRUE;
	return FreeImage_SetPixelColor(_dib, x, y, value);
}

///////////////////////////////////////////////////////////////////
// Loading & Saving

BOOL fipImage::load(const char* lpszPathName, int flag) {
	FREE_IMAGE_FORMAT fif = FIF_UNKNOWN;

	// check the file signature and get its format
	// (the second argument is currently not used by FreeImage)
	fif = FreeImage_GetFileType(lpszPathName, 0);
	if(fif == FIF_UNKNOWN) {
		// no signature ?
		// try to guess the file format from the file extension
		fif = FreeImage_GetFIFFromFilename(lpszPathName);
	}
	// check that the plugin has reading capabilities ...
	if((fif != FIF_UNKNOWN) && FreeImage_FIFSupportsReading(fif)) {
		// Free the previous dib
		if(_dib) {
			FreeImage_Unload(_dib);			
		}
		// Load the file
		_dib = FreeImage_Load(fif, lpszPathName, flag);
		_bHasChanged = TRUE;
		if(_dib == NULL)
			return FALSE;
		return TRUE;
	}
	return FALSE;
}

BOOL fipImage::loadFromHandle(FreeImageIO *io, fi_handle handle, int flag) {
	FREE_IMAGE_FORMAT fif = FIF_UNKNOWN;

	// check the file signature and get its format
	fif = FreeImage_GetFileTypeFromHandle(io, handle, 16);
	if((fif != FIF_UNKNOWN) && FreeImage_FIFSupportsReading(fif)) {
		// Free the previous dib
		if(_dib) {
			FreeImage_Unload(_dib);			
		}
		// Load the file
		_dib = FreeImage_LoadFromHandle(fif, io, handle, flag);
		_bHasChanged = TRUE;
		if(_dib == NULL)
			return FALSE;
		return TRUE;
	}
	return FALSE;
}

BOOL fipImage::loadFromMemory(fipMemoryIO& memIO, int flag) {
	FREE_IMAGE_FORMAT fif = FIF_UNKNOWN;

	// check the file signature and get its format
	fif = memIO.getFileType();
	if((fif != FIF_UNKNOWN) && FreeImage_FIFSupportsReading(fif)) {
		// Free the previous dib
		if(_dib) {
			FreeImage_Unload(_dib);			
		}
		// Load the file
		_dib = memIO.read(fif, flag);
		_bHasChanged = TRUE;
		if(_dib == NULL)
			return FALSE;
		return TRUE;
	}
	return FALSE;
}

BOOL fipImage::save(const char* lpszPathName, int flag) {
	FREE_IMAGE_FORMAT fif = FIF_UNKNOWN;
	BOOL bSuccess = FALSE;

	// Try to guess the file format from the file extension
	fif = FreeImage_GetFIFFromFilename(lpszPathName);
	if(fif != FIF_UNKNOWN ) {
		// Check that the dib can be saved in this format
		BOOL bCanSave;

		FREE_IMAGE_TYPE image_type = FreeImage_GetImageType(_dib);
		if(image_type == FIT_BITMAP) {
			// standard bitmap type
			WORD bpp = FreeImage_GetBPP(_dib);
			bCanSave = (FreeImage_FIFSupportsWriting(fif) && FreeImage_FIFSupportsExportBPP(fif, bpp));
		} else {
			// special bitmap type
			bCanSave = FreeImage_FIFSupportsExportType(fif, image_type);
		}

		if(bCanSave) {
			bSuccess = FreeImage_Save(fif, _dib, lpszPathName, flag);
			return bSuccess;
		}
	}
	return bSuccess;
}

BOOL fipImage::saveToHandle(FREE_IMAGE_FORMAT fif, FreeImageIO *io, fi_handle handle, int flag) {
	BOOL bSuccess = FALSE;

	if(fif != FIF_UNKNOWN ) {
		// Check that the dib can be saved in this format
		BOOL bCanSave;

		FREE_IMAGE_TYPE image_type = FreeImage_GetImageType(_dib);
		if(image_type == FIT_BITMAP) {
			// standard bitmap type
			WORD bpp = FreeImage_GetBPP(_dib);
			bCanSave = (FreeImage_FIFSupportsWriting(fif) && FreeImage_FIFSupportsExportBPP(fif, bpp));
		} else {
			// special bitmap type
			bCanSave = FreeImage_FIFSupportsExportType(fif, image_type);
		}

		if(bCanSave) {
			bSuccess = FreeImage_SaveToHandle(fif, _dib, io, handle, flag);
			return bSuccess;
		}
	}
	return bSuccess;
}

BOOL fipImage::saveToMemory(FREE_IMAGE_FORMAT fif, fipMemoryIO& memIO, int flag) {
	BOOL bSuccess = FALSE;

	if(fif != FIF_UNKNOWN ) {
		// Check that the dib can be saved in this format
		BOOL bCanSave;

		FREE_IMAGE_TYPE image_type = FreeImage_GetImageType(_dib);
		if(image_type == FIT_BITMAP) {
			// standard bitmap type
			WORD bpp = FreeImage_GetBPP(_dib);
			bCanSave = (FreeImage_FIFSupportsWriting(fif) && FreeImage_FIFSupportsExportBPP(fif, bpp));
		} else {
			// special bitmap type
			bCanSave = FreeImage_FIFSupportsExportType(fif, image_type);
		}

		if(bCanSave) {
			bSuccess = memIO.write(fif, _dib, flag);
			return bSuccess;
		}
	}
	return bSuccess;
}

///////////////////////////////////////////////////////////////////   
// Conversion routines

BOOL fipImage::convertToType(FREE_IMAGE_TYPE image_type, BOOL scale_linear) {
	if(_dib) {
		FIBITMAP *dib = FreeImage_ConvertToType(_dib, image_type, scale_linear);
		return replace(dib);
	}
	return FALSE;
}

BOOL fipImage::threshold(BYTE T) {
	if(_dib) {
		FIBITMAP *dib1 = FreeImage_Threshold(_dib, T);
		return replace(dib1);
	}
	return FALSE;
}

BOOL fipImage::convertTo8Bits() {
	if(_dib) {
		FIBITMAP *dib8 = FreeImage_ConvertTo8Bits(_dib);
		return replace(dib8);
	}
	return FALSE;
}

BOOL fipImage::convertTo16Bits555() {
	if(_dib) {
		FIBITMAP *dib16_555 = FreeImage_ConvertTo16Bits555(_dib);
		return replace(dib16_555);
	}
	return FALSE;
}

BOOL fipImage::convertTo16Bits565() {
	if(_dib) {
		FIBITMAP *dib16_565 = FreeImage_ConvertTo16Bits565(_dib);
		return replace(dib16_565);
	}
	return FALSE;
}

BOOL fipImage::convertTo24Bits() {
	if(_dib) {
		FIBITMAP *dibRGB = FreeImage_ConvertTo24Bits(_dib);
		return replace(dibRGB);
	}
	return FALSE;
}

BOOL fipImage::convertTo32Bits() {
	if(_dib) {
		FIBITMAP *dib32 = FreeImage_ConvertTo32Bits(_dib);
		return replace(dib32);
	}
	return FALSE;
}

BOOL fipImage::convertToGrayscale() {
	if(_dib) {
		if(FreeImage_GetColorType(_dib) == FIC_PALETTE) {
			// Convert the palette to 24-bit, then to 8-bit
			BOOL bResult;
			bResult = convertTo24Bits();
			bResult &= convertTo8Bits();
			return bResult;
		} else if(FreeImage_GetBPP(_dib) != 8) {
			// Convert the bitmap to 8-bit greyscale
			return convertTo8Bits();
		}
	}
	return FALSE;
}

BOOL fipImage::colorQuantize(FREE_IMAGE_QUANTIZE algorithm) {
	if(_dib) {
		FIBITMAP *dib8 = FreeImage_ColorQuantize(_dib, algorithm);
		return replace(dib8);
	}
	return FALSE;
}

BOOL fipImage::dither(FREE_IMAGE_DITHER algorithm) {
	if(_dib) {
		FIBITMAP *dib = FreeImage_Dither(_dib, algorithm);
		return replace(dib);
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////   
// Transparency support: background colour and alpha channel

BOOL fipImage::isTransparent() {
	return FreeImage_IsTransparent(_dib);
}

unsigned fipImage::getTransparencyCount() {
	return FreeImage_GetTransparencyCount(_dib);
}

BYTE* fipImage::getTransparencyTable() {
	return FreeImage_GetTransparencyTable(_dib);
}

void fipImage::setTransparencyTable(BYTE *table, int count) {
	FreeImage_SetTransparencyTable(_dib, table, count);
	_bHasChanged = TRUE;
}

BOOL fipImage::hasFileBkColor() {
	return FreeImage_HasBackgroundColor(_dib);
}

BOOL fipImage::getFileBkColor(RGBQUAD *bkcolor) {
	return FreeImage_GetBackgroundColor(_dib, bkcolor);
}

BOOL fipImage::setFileBkColor(RGBQUAD *bkcolor) {
	_bHasChanged = TRUE;
	return FreeImage_SetBackgroundColor(_dib, bkcolor);
}

///////////////////////////////////////////////////////////////////   
// Channel processing support

BOOL fipImage::getChannel(fipImage& image, FREE_IMAGE_COLOR_CHANNEL channel) {
	if(_dib) {
		image = FreeImage_GetChannel(_dib, channel);
		return image.isValid();
	}
	return FALSE;
}

BOOL fipImage::setChannel(fipImage& image, FREE_IMAGE_COLOR_CHANNEL channel) {
	if(_dib) {
		_bHasChanged = TRUE;
		return FreeImage_SetChannel(_dib, image._dib, channel);
	}
	return FALSE;
}

BOOL fipImage::splitChannels(fipImage& RedChannel, fipImage& GreenChannel, fipImage& BlueChannel) {
	if(_dib) {
		RedChannel = FreeImage_GetChannel(_dib, FICC_RED);
		GreenChannel = FreeImage_GetChannel(_dib, FICC_GREEN);
		BlueChannel = FreeImage_GetChannel(_dib, FICC_BLUE);

		return (RedChannel.isValid() && GreenChannel.isValid() && BlueChannel.isValid());
	}
	return FALSE;
}

BOOL fipImage::combineChannels(fipImage& red, fipImage& green, fipImage& blue) {
	if(!_dib) {
		int width = red.getWidth();
		int height = red.getHeight();
		_dib = FreeImage_Allocate(width, height, 24, FI_RGBA_RED_MASK, FI_RGBA_GREEN_MASK, FI_RGBA_BLUE_MASK);
	}

	if(_dib) {
		BOOL bResult = TRUE;
		bResult &= FreeImage_SetChannel(_dib, red._dib, FICC_RED);
		bResult &= FreeImage_SetChannel(_dib, green._dib, FICC_GREEN);
		bResult &= FreeImage_SetChannel(_dib, blue._dib, FICC_BLUE);

		_bHasChanged = TRUE;

		return bResult;
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////   
// Rotation and flipping

BOOL fipImage::rotateEx(double angle, double x_shift, double y_shift, double x_origin, double y_origin, BOOL use_mask) {
	if(_dib) {
		if(FreeImage_GetBPP(_dib) >= 8) {
			FIBITMAP *rotated = FreeImage_RotateEx(_dib, angle, x_shift, y_shift, x_origin, y_origin, use_mask);
			return replace(rotated);
		}
	}
	return FALSE;
}

BOOL fipImage::rotate(double angle) {
	if(_dib) {
		if(FreeImage_GetBPP(_dib) >= 8) {
			FIBITMAP *rotated = FreeImage_RotateClassic(_dib, angle);
			return replace(rotated);
		}
	}
	return FALSE;
}

BOOL fipImage::flipVertical() {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_FlipVertical(_dib);
	}
	return FALSE;
}

BOOL fipImage::flipHorizontal() {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_FlipHorizontal(_dib);
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////   
// Color manipulation routines

BOOL fipImage::invert() {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_Invert(_dib);
	}
	return FALSE;
}

BOOL fipImage::adjustCurve(BYTE *LUT, FREE_IMAGE_COLOR_CHANNEL channel) {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_AdjustCurve(_dib, LUT, channel);
	}
	return FALSE;
}

BOOL fipImage::adjustGamma(double gamma) {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_AdjustGamma(_dib, gamma);
	}
	return FALSE;
}

BOOL fipImage::adjustBrightness(double percentage) {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_AdjustBrightness(_dib, percentage);
	}
	return FALSE;
}

BOOL fipImage::adjustContrast(double percentage) {
	if(_dib) {
		_bHasChanged = TRUE;

		return FreeImage_AdjustContrast(_dib, percentage);
	}
	return FALSE;
}

BOOL fipImage::getHistogram(DWORD *histo, FREE_IMAGE_COLOR_CHANNEL channel) {
	if(_dib) {
		return FreeImage_GetHistogram(_dib, histo, channel);
	}
	return FALSE;
}

///////////////////////////////////////////////////////////////////
// Upsampling / downsampling routine

BOOL fipImage::rescale(WORD new_width, WORD new_height, FREE_IMAGE_FILTER filter) {
	if(_dib) {
		int bpp = FreeImage_GetBPP(_dib);
		if(bpp < 8) {
			// Convert to 8-bit
			if(!convertTo8Bits())
				return FALSE;
		} else if(bpp == 16) {
			// Convert to 24-bit
			if(!convertTo24Bits())
				return FALSE;
		}
		// Perform upsampling / downsampling
		FIBITMAP *dst = FreeImage_Rescale(_dib, new_width, new_height, filter);
		return replace(dst);
	}
	return FALSE;
}

