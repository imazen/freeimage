// ==========================================================
// FreeImagePlus 3
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

#ifndef FREEIMAGEPLUS_H
#define FREEIMAGEPLUS_H

#ifdef WIN32
#include <windows.h>
#endif // WIN32
#include "FreeImage.h"


// Compiler options ---------------------------------------------------------

#if defined(FREEIMAGE_LIB) || !defined(WIN32)
#define FIP_API
#define FIP_CALLCONV
#else
#define WIN32_LEAN_AND_MEAN
#define FIP_CALLCONV __stdcall
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the FIP_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// FIP_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#ifdef FIP_EXPORTS
#define FIP_API __declspec(dllexport)
#else
#define FIP_API __declspec(dllimport)
#endif // FIP_EXPORTS
#endif // FREEIMAGE_LIB || !WIN32

///////////////////////////////////////////////////////////////////////////////////////////

/**@name Useful constants */
//@{
/// White value (= 255)
const BYTE WHITE = 255;
/// Black value (= 0)
const BYTE BLACK = 0;
//@}

// ----------------------------------------------------------

/** Abstract base class for all image types used by the library.
	@version FreeImage 3
	@author Hervé Drolon
*/

class FIP_API fipGenericImage
{
public:
	/**@name Information functions */
	//@{
	/// Returns TRUE if the image is allocated, FALSE otherwise
	virtual BOOL isValid() = 0;
	//@}
};

// ----------------------------------------------------------

/** A class used to manage all photo related images and all image types used by the library.

	fipImage encapsulates the FIBITMAP format. It relies on the FreeImage library, especially for 
	loading / saving images and for bit depth conversion.
	@version FreeImage 3
	@author Hervé Drolon
*/

class FIP_API fipImage : public fipGenericImage
{
protected:
	/// DIB data
	FIBITMAP *_dib;
	/// TRUE whenever the display need to be refreshed
	BOOL _bHasChanged;

public:

	/**@name Creation & Destruction */
	//@{	
	/// Constructor
	fipImage(FREE_IMAGE_TYPE image_type = FIT_BITMAP, WORD width = 0, WORD height = 0, WORD bpp = 0);
	/// Destructor
	~fipImage();
	/// Image allocator
	BOOL setSize(FREE_IMAGE_TYPE image_type, WORD width, WORD height, WORD bpp, unsigned red_mask = 0, unsigned green_mask = 0, unsigned blue_mask = 0);
	//@}

	/**@name Copying */
	//@{	
	/// Copy constructor
	fipImage(const fipImage& src);
	/// Copy constructor
	fipImage(const FIBITMAP *dib);
	/// Copy constructor
	fipImage& operator=(const fipImage& src);
	/// Assignement operator
	fipImage& operator=(FIBITMAP *dib);


	/**
	@brief Copy a sub part of the current image and returns it as a fipImage object.

	The bit depth of the bitmap must be equal to 8, 16, 24 or 32.
	@param dst Output subimage
	@param left Specifies the left position of the cropped rectangle. 
	@param top Specifies the top position of the cropped rectangle. 
	@param right Specifies the right position of the cropped rectangle. 
	@param bottom Specifies the bottom position of the cropped rectangle. 
	@return Returns TRUE if successful, FALSE otherwise.
	*/
	BOOL copySubImage(fipImage& dst, int left, int top, int right, int bottom);

	/**
	@brief Alpha blend or combine a sub part image with the current image.

    The bit depth of dst bitmap must be greater than or equal to the bit depth of src. 
	Upper promotion of src is done internally. Supported bit depth equals to 8, 16, 24 or 32.
	@param src Source subimage
	@param left Specifies the left position of the sub image. 
	@param top Specifies the top position of the sub image. 
	@param alpha Alpha blend factor. The source and destination images are alpha blended if 
	alpha = 0..255. If alpha > 255, then the source image is combined to the destination image.
	@return Returns TRUE if successful, FALSE otherwise.
	*/
	BOOL pasteSubImage(fipImage& src, int left, int top, int alpha = 256);

	//@}

	/** @name Loading & Saving
	 * Loading and saving is handled by the FreeImage library.
	 */
	//@{	
	/**
	@brief Loads an image from disk, given its file name and an optional flag.
	@param lpszPathName Path and file name of the image to load.
	@param flag The signification of this flag depends on the image to be read.
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage.h, FreeImage documentation
	*/
	BOOL load(const char* lpszPathName, int flag = 0);

	/**
	@brief Loads an image using the specified FreeImageIO struct and fi_handle, and an optional flag.
	@param io FreeImageIO structure
	@param handle FreeImage fi_handle
	@param flag The signification of this flag depends on the image to be read.
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage.h, FreeImage documentation
	*/
	BOOL loadFromHandle(FreeImageIO *io, fi_handle handle, int flag = 0);

	/**
	@brief Saves an image to disk, given its file name and an optional flag.
	@param lpszPathName Path and file name of the image to save.
	@param flag The signification of this flag depends on the image to be saved.
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage.h, FreeImage documentation
	*/
	BOOL save(const char* lpszPathName, int flag = 0);

	/**
	@brief Saves an image using the specified FreeImageIO struct and fi_handle, and an optional flag.
	@param fif Format identifier (FreeImage format)
	@param io FreeImageIO structure
	@param handle FreeImage fi_handle
	@param flag The signification of this flag depends on the image to be saved.
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage.h, FreeImage documentation
	*/
	BOOL saveToHandle(FREE_IMAGE_FORMAT fif, FreeImageIO *io, fi_handle handle, int flag = 0);
	//@}

	/**	@name Information functions
	 *  Accessors to the DIB BITMAPINFO structure.
	 */
	//@{	

	/**
	Returns the data type of the image
	@see FreeImage_GetImageType
	*/
	FREE_IMAGE_TYPE getImageType();

	/// Returns the image width in pixels
	WORD getWidth();
	
	/// Returns the image height in pixels
	WORD getHeight();
	
	/// Returns the width of the bitmap in bytes rounded to the nearest DWORD.
	WORD getScanWidth();

	/// Returns a pointer to the FIBITMAP data. Used for direct access from FREEIMAGE functions.
	FIBITMAP* getFIBITMAP();

	/// Returns TRUE if the image is allocated, FALSE otherwise
	BOOL  isValid();

	/// Returns a pointer to the bitmap's BITMAPINFO header. 
	BITMAPINFO* getInfo();

	/// Returns a pointer to the bitmap's BITMAPINFOHEADER. 
    BITMAPINFOHEADER* getInfoHeader();

	/**
	Returns the size of the bitmap in bytes. 
	The size of the bitmap is the BITMAPINFOHEADER + the size of the palette + the size of the bitmap data. 
	*/
	LONG getImageSize();
	
	/**
	Returns the bitdepth of the bitmap. <br>
	When the image type is FIT_BITMAP, valid bitdepth can be 1, 4, 8, 16, 24 or 32.
	@see getImageType
	*/
	WORD getBitsPerPixel();

	/// Returns the width of the bitmap in bytes. 
	WORD getLine();

	/// Returns the bitmap resolution along the X axis, in pixels / cm
	WORD getHorizontalResolution();
	
	/// Returns the bitmap resolution along the Y axis, in pixels / cm
	WORD getVerticalResolution();

	/// set the bitmap resolution along the X axis, in pixels / cm
	void setHorizontalResolution(LONG value);
	
	/// set the bitmap resolution along the Y axis, in pixels / cm
	void setVerticalResolution(LONG value);
	//@}

	/**@name Palette operations */
	//@{
	/// Returns a pointer to the bitmap's palette. If the bitmap doesn't have a palette, getPalette returns NULL. 
	RGBQUAD* getPalette();
	
	/// Returns the palette size in bytes.
	WORD getPaletteSize();

	/// Retrieves the number of colours used in the bitmap. If the bitmap is non-palletised, 0 is returned. 
	WORD getColorsUsed();

	/** @brief Investigates the colour type of the bitmap.
		@see FREE_IMAGE_COLOR_TYPE
	*/
	FREE_IMAGE_COLOR_TYPE getColorType();

	/// Returns TRUE if the bitmap is a 8-bit bitmap with a greyscale palette, FALSE otherwise
	BOOL isGrayscale();
	//@}

	/**@name Pixel access */
	//@{	

	/** @brief Returns a pointer to the bitmap bits.

	It is up to you to interpret these bytes correctly, 
	according to the results of FreeImage_GetBPP and 
	GetRedMask, FreeImage_GetGreenMask and FreeImage_GetBlueMask.
	@see FreeImage_GetBits
	*/
	BYTE* accessPixels();

	/** @brief Returns a pointer to the start of the given scanline in the bitmap’s data-bits.
		This pointer can be cast according to the result returned by getImageType.
		@see FreeImage_GetScanLine
	*/
	BYTE* getScanLine(WORD scanline);

	/** @brief Get the pixel index of a 1-, 4- or 8-bit palettized image at position (x, y), including range check (slow access). 
	@param x Pixel position in horizontal direction
	@param y Pixel position in vertical direction
	@param value Pixel index (returned value)
	@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL getPixelIndex(unsigned x, unsigned y, BYTE *value);

	/** @brief Get the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access). 
	@param x Pixel position in horizontal direction
	@param y Pixel position in vertical direction
	@param value Pixel color (returned value)
	@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL getPixelColor(unsigned x, unsigned y, RGBQUAD *value);

	/** @brief Set the pixel index of a 1-, 4- or 8-bit palettized image at position (x, y), including range check (slow access). 
	@param x Pixel position in horizontal direction
	@param y Pixel position in vertical direction
	@param value Pixel index
	@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL setPixelIndex(unsigned x, unsigned y, BYTE *value);

	/** @brief Set the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access). 
	@param x Pixel position in horizontal direction
	@param y Pixel position in vertical direction
	@param value Pixel color
	@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL setPixelColor(unsigned x, unsigned y, RGBQUAD *value);

	//@}

	/**	@name Conversion routines
	 *  Bitmaps are always loaded in their default bit depth. If you want the bitmap to be stored in another bit depth, the class provides several conversion functions.
	 */
	//@{	
	/** @brief Converts an image to a type supported by FreeImage.
		@param image_type New image type
		@param scale_linear TRUE if image pixels must be scaled linearly when converting to a standard bitmap
		@return Returns TRUE if successfull, FALSE otherwise. 
		@see FreeImage_ConvertToType, FreeImage_ConvertToStandardType
	*/
	BOOL convertToType(FREE_IMAGE_TYPE image_type, BOOL scale_linear = TRUE);

	/** @brief Converts the bitmap to 1 bit using a threshold T.
		@param T Threshold value in [0..255]
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL threshold(BYTE T);
	
	/** @brief Converts the bitmap to 8 bits. If the bitmap is 24 or 32-bit RGB, the colour values are converted to greyscale.
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertTo8Bits();

	/** @brief Converts the bitmap to 16 bits. The resulting bitmap has a layout of 5 bits red, 5 bits green, 5 bits blue and 1 unused bit. 
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertTo16Bits555();
	
	/** @brief Converts the bitmap to 16 bits. The resulting bitmap has a layout of 5 bits red, 6 bits green and 5 bits blue. 
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertTo16Bits565();
	
	/** @brief Converts the bitmap to 24 bits. 
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertTo24Bits();
	
	/** @brief Converts the bitmap to 32 bits. 
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertTo32Bits();

	/** @brief Converts the bitmap to 8 bits. 
		
		For palletized bitmaps, the color map is converted to a greyscale ramp.
		@see convertTo8Bits
		@return Returns TRUE if successfull, FALSE otherwise. 
	*/
	BOOL convertToGrayscale();
	
	/** @brief Quantizes a full colour 24-bit bitmap to a palletised 8-bit bitmap.
	
	    The quantize parameter specifies which colour reduction algorithm should be used.
		@param algorithm Color quantization algorithm to use.
		@return Returns TRUE if successfull, FALSE otherwise. 
		@see FREE_IMAGE_QUANTIZE
	*/
	BOOL colorQuantize(FREE_IMAGE_QUANTIZE algorithm);

	/** @brief Converts a 8-bit image to a monochrome image using a dithering algorithm.

		@param algorithm Dithering algorithm to use.
		@return Returns TRUE if successfull, FALSE otherwise. 
		@see FREE_IMAGE_DITHER
	*/
	BOOL dither(FREE_IMAGE_DITHER algorithm);
	//@}

	/**	@name Transparency support: background colour and alpha channel */
	//@{

	/**
	Returns TRUE if the image is transparent, returns FALSE otherwise
	@see FreeImage_IsTransparent
	*/
	BOOL isTransparent();

	/**
	@brief 8-bit transparency : get the number of transparent colors.

	@return Returns the number of transparent colors in a palletised bitmap.
	@see FreeImage_GetTransparencyCount
	*/
	unsigned getTransparencyCount();

	/**
	@brief 8-bit transparency : get the bitmap’s transparency table.

	@return Returns a pointer to the bitmap’s transparency table.
	@see FreeImage_GetTransparencyTable
	*/
	BYTE* getTransparencyTable();

	/** 
	@brief 8-bit transparency : set the bitmap’s transparency table.

	@see FreeImage_SetTransparencyTable
	*/
	void setTransparencyTable(BYTE *table, int count);

	/**
	Returns TRUE when the image has a file background color, FALSE otherwise.
	@see FreeImage_HasBackgroundColor
	*/
	BOOL hasFileBkColor();

	/**
	@brief Retrieves the file background color of an image. 
	
	For 8-bit images, the color index 
	in the palette is returned in the rgbReserved member of the bkcolor parameter.
	@return Returns TRUE if successful, FALSE otherwise. 
	@see FreeImage_GetBackgroundColor
	*/
	BOOL getFileBkColor(RGBQUAD *bkcolor);

	/**
	@brief Set the file background color of an image. 
	
	When saving an image to PNG, this background color is transparently saved to the PNG file. 
	When the bkcolor parameter is NULL, the background color is removed from the image.
	@return Returns TRUE if successful, FALSE otherwise. 
	@see FreeImage_SetBackgroundColor
	*/
	BOOL setFileBkColor(RGBQUAD *bkcolor);
	//@}

	/**@name Channel processing support */
	//@{	
	/** @brief Retrieves the red, green, blue or alpha channel of a 24- or 32-bit BGR[A] image. 
	@param image Output image to be extracted
	@param channel Color channel to extract
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage_GetChannel, FREE_IMAGE_COLOR_CHANNEL
	*/
	BOOL getChannel(fipImage& image, FREE_IMAGE_COLOR_CHANNEL channel);

	/**
	@brief Insert a 8-bit dib into a 24- or 32-bit image. 
	@param image Input 8-bit image to insert
	@param channel Color channel to replace
	@return Returns TRUE if successful, FALSE otherwise.
	@see FreeImage_SetChannel, FREE_IMAGE_COLOR_CHANNEL
	*/
	BOOL setChannel(fipImage& image, FREE_IMAGE_COLOR_CHANNEL channel);

	/** @brief Split a 24-bit RGB image into 3 greyscale images corresponding to the red, green and blue channels.
	@param RedChannel Output red channel.
	@param GreenChannel Output green channel.
	@param BlueChannel Output blue channel.
	@return Returns FALSE if the dib isn't a valid image, if it's not a 24-bit image or if 
	one of the output channel can't be allocated. Returns TRUE otherwise.
	*/
	BOOL splitChannels(fipImage& RedChannel, fipImage& GreenChannel, fipImage& BlueChannel);

	/** @brief Builds a 24-bit RGB image given its red, green and blue channel.
	@param red Input red channel.
	@param green Input green channel.
	@param blue Input blue channel.
	@return Returns FALSE if the dib can't be allocated, if the input channels are not 8-bit images. Returns TRUE otherwise.
	*/
	BOOL combineChannels(fipImage& red, fipImage& green, fipImage& blue);
	//@}

	/**@name Rotation and flipping */
	//@{	
	/** @brief Image translation and rotation using B-Splines.

	@param angle Image rotation angle, in degree
	@param x_shift Image horizontal shift
	@param y_shift Image vertical shift
	@param x_origin Origin of the x-axis
	@param y_origin Origin of the y-axis
	@param use_mask Whether or not to mask the image. Image mirroring is applied when use_mask is set to FALSE
	@return Returns the translated & rotated dib if successful, returns NULL otherwise
	*/
	BOOL rotateEx(double angle, double x_shift, double y_shift, double x_origin, double y_origin, BOOL use_mask);

	/** @brief Image rotation by means of three shears.

	@param angle Image rotation angle, in degree
	@return Returns the translated & rotated dib if successful, returns NULL otherwise
	*/
	BOOL rotate(double angle);

	/// Flip the image horizontally along the vertical axis
	BOOL flipHorizontal();

	/// Flip the image vertically along the horizontal axis
	BOOL flipVertical();
	//@}

	/**@name Color manipulation routines */
	//@{	
	/** @brief Inverts each pixel data.
	@return Returns TRUE if successful, FALSE otherwise.
	*/
	BOOL invert();
	
	/** @brief Perfoms an histogram transformation on a 8, 24 or 32-bit image 
	according to the values of a lookup table (LUT).

	The transformation is done as follows.<br>
	Image 8-bit : if the image has a color palette, the LUT is applied to this palette, 
	otherwise, it is applied to the grey values.<br>
	Image 24-bit & 32-bit : if channel == IPL_CC_RGB, the same LUT is applied to each color
	plane (R,G, and B). Otherwise, the LUT is applied to the specified channel only.
	@param LUT Lookup table. <b>The size of 'LUT' is assumed to be 256.</b>
	@param channel The color channel to be processed (only used with 24 & 32-bit DIB).
	@return Returns TRUE if the operation was successful, FALSE otherwise
	@see FREE_IMAGE_COLOR_CHANNEL
	*/
	BOOL adjustCurve(BYTE *LUT, FREE_IMAGE_COLOR_CHANNEL channel);

	/** @brief Performs gamma correction on a 8, 24 or 32-bit image.
	@param gamma Gamma value to use. A value of 1.0 leaves the image alone, 
	less than one darkens it, and greater than one lightens it.
	@return Returns TRUE if the operation was successful, FALSE otherwise
	@see adjustCurve
	*/
	BOOL adjustGamma(double gamma);

	/** @brief Adjusts the brightness of a 8, 24 or 32-bit image by a certain amount.
	@param percentage Where -100 <= percentage <= 100<br>
	A value 0 means no change, less than 0 will make the image darker 
	and greater than 0 will make the image brighter.
	@return Returns TRUE if the operation was succesful, FALSE otherwise
	@see adjustCurve
	*/
	BOOL adjustBrightness(double percentage);

	/** @brief Adjusts the contrast of a 8, 24 or 32-bit image by a certain amount.
	@param percentage Where -100 <= percentage <= 100<br>
	A value 0 means no change, less than 0 will decrease the contrast 
	and greater than 0 will increase the contrast of the image.
	@return Returns TRUE if the operation was succesfull, FALSE otherwise
	@see adjustCurve
	*/
	BOOL adjustContrast(double percentage);

	/** @brief Computes image histogram
	
	For 24-bit and 32-bit images, histogram can be computed from red, green, blue and 
	black channels. For 8-bit images, histogram is computed from the black channel. Other 
	bit depth is not supported.
	@param histo pointer to an histogram array. <b>Size of this array is assumed to be 256</b>.
	@param channel Color channel to use
	@return Returns TRUE if the operation was succesfull, FALSE otherwise
	*/
	BOOL getHistogram(DWORD *histo, FREE_IMAGE_COLOR_CHANNEL channel = FICC_BLACK);
	//@}

	/**@name Upsampling / downsampling */
	//@{	

	/** @brief Rescale the image to a new width / height.

	@param new_width New image width
	@param new_height New image height
	@param filter The filter parameter specifies which resampling filter should be used.
	@return Returns TRUE if the operation was successful, FALSE otherwise
	@see FREE_IMAGE_FILTER
	*/
	BOOL rescale(WORD new_width, WORD new_height, FREE_IMAGE_FILTER filter);
	//@}


  protected:
	/**@name Internal use */
	//@{
	  BOOL replace(FIBITMAP *new_dib);
	//@}

};

// ----------------------------------------------------------

/** A class designed for MS Windows (TM) platforms.

    fipWinImage provides methods used to :
	<ul>
	<li>Display a DIB on the screen
	<li>Copy / Paste a DIB to/from Windows devices (HANDLE, HBITMAP, Clipboard)
	<li>Capture a window (HWND) and convert it to an image
	</ul>
	@version FreeImage 3
	@author Hervé Drolon
*/
#ifdef WIN32

class FIP_API fipWinImage : public fipImage
{
public:
	/**@name Creation & Destruction */
	//@{	
	/// Constructor
	fipWinImage(FREE_IMAGE_TYPE image_type = FIT_BITMAP, WORD width = 0, WORD height = 0, WORD bpp = 0);

	/// Destructor
	~fipWinImage();
	//@}

	/**@name Copying */
	//@{	
	/// Copy constructor
	fipWinImage(const fipImage& Img);

	/// Copy constructor
	fipWinImage(const FIBITMAP *dib);

	/** Clone function used for clipboard copy.<br>
	Convert the FIBITMAP image to a DIB, 
	and transfer the DIB in a global bitmap handle.
	*/
	HANDLE copyToHandle();

	/** Copy constructor used for clipboard paste.<br>
	Converts a global object to a FIBITMAP. The clipboard format must be CF_DIB.
	@return Returns TRUE if successful, returns FALSE otherwise
	*/
	BOOL copyFromHandle(HANDLE hMem);

	/** Copy constructor.<br>
	Converts a HBITMAP object to a FIBITMAP.
	@return Returns TRUE if successful, returns FALSE otherwise
	*/
	BOOL copyFromBitmap(HBITMAP hbmp);
	//@}

	/**@name Clipboard operations */
	//@{	
	/**
	Clipboard copy.
	@param hWndNewOwner Handle to the window to be associated with the open clipboard. 
	In MFC, you can use AfxGetApp()->m_pMainWnd->GetSafeHwnd().
	@return Returns TRUE if successful, returns FALSE otherwise
	*/
	BOOL copyToClipboard(HWND hWndNewOwner);

	/**
	Retrieves data from the clipboard. The clipboard format must be CF_DIB.
	@return Returns TRUE if successful, returns FALSE otherwise
	*/
	BOOL pasteFromClipboard();
	//@}

	/**@name Screen capture */
	//@{	
	/** Capture a window and convert it to an image
	@param hWndApplicationWindow Handle to the application main window
	@param hWndSelectedWindow Handle to the window to be captured
	@return Returns TRUE if successful, returns FALSE otherwise
	*/
	BOOL captureWindow(HWND hWndApplicationWindow, HWND hWndSelectedWindow);
	//@}


	/**@name Painting operations */
	//@{	

	/** @brief Draw (stretch) the image on a HDC, using StretchDIBits.

    When the image is transparent or has a file background, this function composite 
	the foreground image against a checkerboard background image.
	@param hDC Handle to the device context
	@param rcDest Destination rectangle
	@see FreeImage_Composite
	*/
	void draw(HDC hDC, RECT& rcDest) {
		drawEx(hDC, rcDest, FALSE, NULL, NULL);
	}

	/** @brief Draw (stretch) the image on a HDC, using StretchDIBits.

    When the image is transparent or has a file background, this function can composite 
	the foreground image against a checkerboard background image, against a single background color or 
	against a user background image.
	@param hDC Handle to the device context
	@param rcDest Destination rectangle
	@param useFileBkg When set to TRUE, the function uses the file color background if there is one
	@param appBkColor When a color is given, the function uses it as the background color
	@param bg When a FIBITMAP is given, the function uses it as the background image
	@see FreeImage_Composite
	*/
	void drawEx(HDC hDC, RECT& rcDest, BOOL useFileBkg = FALSE, RGBQUAD *appBkColor = NULL, FIBITMAP *bg = NULL);

	//@}

protected:
	/// DIB used for display (this allow to display non-standard bitmaps)
	FIBITMAP *_display_dib;
	/// remember to delete _display_dib
	BOOL _bDeleteMe;
};

#endif // WIN32

// ----------------------------------------------------------

/** Memory handle
	
	fipMemoryIO is a class that allows you to load / save images from a memory handle.
	@version FreeImage 3
	@author Hervé Drolon
*/
class FIP_API fipMemoryIO : public FreeImageIO
{
protected:
	/// Remember to delete the buffer
	BOOL _delete_me;
	/// Start buffer address
    BYTE * _start;
	/// Current position
    BYTE *_cp;
	/// Buffer size
	DWORD _size;

public :
	/** Constructor.
	Wrap a memory buffer containing image data.<br>
	The memory buffer has to be freed by the user 
	when no longer in use.
	@param data Pointer to the memory buffer
	@param size_in_bytes Buffer size in bytes
	*/
    fipMemoryIO(BYTE *data = NULL, DWORD size_in_bytes = 0);

	/** Constructor.
	Load a binary file containing image data.
	into a memory buffer.<br>
	The memory buffer is automatically destroyed 
	when no longer in use.
	@param lpszPathName Path and file name of the image to load.
	@return Returns TRUE if successful, FALSE otherwise
	*/
    BOOL loadFile(const char *lpszPathName);
	
	/** Destructor.
	Free any allocated memory
	*/
	~fipMemoryIO();

	/** Returns TRUE if the internal memory buffer is a valid buffer, returns FALSE otherwise
	*/
	BOOL isValidIO();

	/** Returns the buffer image format
	@see FreeImage_GetFileTypeFromHandle
	*/
	FREE_IMAGE_FORMAT getFileType();

	/**@name Memory IO routines */
	//@{	
    static unsigned FIP_CALLCONV _ReadProc(void *buffer, unsigned size, unsigned count, fi_handle handle);
    static unsigned FIP_CALLCONV _WriteProc(void *buffer, unsigned size, unsigned count, fi_handle handle);
    static int FIP_CALLCONV _SeekProc(fi_handle handle, long offset, int origin);
	static long FIP_CALLCONV _TellProc(fi_handle handle);
	//@}

};

#ifdef WIN32

#include <WinInet.h>

/** Internet handle
	
	fipInternetIO is a class that allows you to load / save images from a Internet handle.
	@version FreeImage 3
	@author Hervé Drolon
*/
class FIP_API fipInternetIO : public fipMemoryIO
{
protected:
	/// Internet session
	HINTERNET _hInternet;
	/// handle to HTTP file
	HINTERNET _hHttpFile;

public :
	/** Constructor.
	Opens a new Internet session
	*/
    fipInternetIO();

	/** Destructor.
	Closes the Internet session
	*/
	~fipInternetIO();

	/** Download a file from a URL to internal memory buffer
	*/
	BOOL downloadFile(char *szURL);

};

#endif // WIN32

#endif	// FREEIMAGEPLUS_H
