// ==========================================================
// Photoshop Loader
//
// Design and implementation by
// - Hervé Drolon (drolon@infonie.fr)
// - Mihail Naydenov (mnaydenov@users.sourceforge.net)
//
// Based on LGPL code created and published by http://sourceforge.net/projects/elynx/
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
#include "PSDParser.h"

// --------------------------------------------------------------------------

using std::numeric_limits;

// --------------------------------------------------------------------------

// PSD signature (= '8BPS')
#define PSD_SIGNATURE	0x38425053
// Image resource block signature (= '8BIM')
#define PSD_RESOURCE	0x3842494D 

// PSD color modes
#define PSDP_BITMAP			0
#define PSDP_GRAYSCALE		1
#define PSDP_INDEXED		2
#define PSDP_RGB			3
#define PSDP_CMYK			4
#define PSDP_MULTICHANNEL	7
#define PSDP_DUOTONE		8
#define PSDP_LAB			9

// PSD compression schemes
#define PSDP_COMPRESSION_NONE	0	// Raw data
#define PSDP_COMPRESSION_RLE	1	// RLE compression (same as TIFF packed bits)

#define SAFE_DELETE_ARRAY(_p_) { if (NULL != (_p_)) { delete [] (_p_); (_p_) = NULL; } }

// --------------------------------------------------------------------------

static inline int 
psdGetValue(const BYTE * iprBuffer, const int iBytes) {
	int v = iprBuffer[0];
	for (int i=1; i<iBytes; ++i) {
		v = (v << 8) | iprBuffer[i];
	}
	return v;
}

// --------------------------------------------------------------------------

psdHeaderInfo::psdHeaderInfo() : _Channels(-1), _Height(-1), _Width(-1), _BitsPerChannel(-1), _ColourMode(-1) {
}

psdHeaderInfo::~psdHeaderInfo() {
}
	
bool psdHeaderInfo::Read(FreeImageIO *io, fi_handle handle) {
	psdHeader header;

	const int n = (int)io->read_proc(&header, sizeof(header), 1, handle);
	if(!n) {
		return false;
	}

	// check the signature
	int nSignature = psdGetValue(header.Signature, sizeof(header.Signature));
	if (PSD_SIGNATURE == nSignature) {
		// check the version
		int nVersion = psdGetValue( header.Version, sizeof(header.Version) );
		if (1 == nVersion) {
			// header.Reserved must be zero
			BYTE psd_reserved[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			if(memcmp(header.Reserved, psd_reserved, 6) != 0) {
				FreeImage_OutputMessageProc(FIF_PSD, "Warning: file header reserved member is not equal to zero");
			}
			// read the header
			_Channels = (short)psdGetValue( header.Channels, sizeof(header.Channels) );
			_Height = psdGetValue( header.Rows, sizeof(header.Rows) );
			_Width = psdGetValue( header.Columns, sizeof(header.Columns) );
			_BitsPerChannel = (short)psdGetValue( header.Depth, sizeof(header.Depth) );
			_ColourMode = (short)psdGetValue( header.Mode, sizeof(header.Mode) );

			return true;
		}
	}

	return false;
}

// --------------------------------------------------------------------------

psdColourModeData::psdColourModeData() : _Length(-1), _plColourData(NULL) {
}

psdColourModeData::~psdColourModeData() { 
	SAFE_DELETE_ARRAY(_plColourData); 
}
	
bool psdColourModeData::Read(FreeImageIO *io, fi_handle handle) {
	if (0 < _Length) { 
		SAFE_DELETE_ARRAY(_plColourData);
	}
	
	BYTE Length[4];
	io->read_proc(&Length, sizeof(Length), 1, handle);
	
	_Length = psdGetValue( Length, sizeof(_Length) );
	if (0 < _Length) {
		_plColourData = new BYTE[_Length];
		io->read_proc(_plColourData, _Length, 1, handle);
	}

	return true;
}
	
bool psdColourModeData::FillPalette(FIBITMAP *dib) {
	RGBQUAD *pal = FreeImage_GetPalette(dib);
	if(pal) {
		for (int i = 0; i < 256; i++) {
			pal[i].rgbRed	= _plColourData[i + 0*256];
			pal[i].rgbGreen = _plColourData[i + 1*256];
			pal[i].rgbBlue	= _plColourData[i + 2*256];
		}
		return true;
	}
	return false;
}

// --------------------------------------------------------------------------

psdImageResource::psdImageResource() : _plName (0) { 
	Reset(); 
}

psdImageResource::~psdImageResource() { 
	SAFE_DELETE_ARRAY(_plName);
}

void psdImageResource::Reset() {
	_Length = -1;
	memset( _OSType, '\0', sizeof(_OSType) );
	_ID = -1;
	SAFE_DELETE_ARRAY(_plName);
	_Size = -1;
}

// --------------------------------------------------------------------------

psdResolutionInfo::psdResolutionInfo() : _widthUnit(-1), _heightUnit(-1), _hRes(-1), _vRes(-1), _hResUnit(-1), _vResUnit(-1) {
}

psdResolutionInfo::~psdResolutionInfo() {
}
	
int psdResolutionInfo::Read(FreeImageIO *io, fi_handle handle) {
	BYTE IntValue[4], ShortValue[2];
	int nBytes=0, n;
	
	// Horizontal resolution in pixels per inch.
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_hRes = (short)psdGetValue(ShortValue, sizeof(_hRes) );
	// 1=display horizontal resolution in pixels per inch; 2=display horizontal resolution in pixels per cm.
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_hResUnit = psdGetValue(IntValue, sizeof(_hResUnit) );
	// Display width as 1=inches; 2=cm; 3=points; 4=picas; 5=columns.
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_widthUnit = (short)psdGetValue(ShortValue, sizeof(_widthUnit) );
	// Vertical resolution in pixels per inch.
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_vRes = (short)psdGetValue(ShortValue, sizeof(_vRes) );
	// 1=display vertical resolution in pixels per inch; 2=display vertical resolution in pixels per cm.
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_vResUnit = psdGetValue(IntValue, sizeof(_vResUnit) );
	// Display height as 1=inches; 2=cm; 3=points; 4=picas; 5=columns.
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_heightUnit = (short)psdGetValue(ShortValue, sizeof(_heightUnit) );
	
	return nBytes;
}

void psdResolutionInfo::GetResolutionInfo(unsigned &res_x, unsigned &res_y) {
	if(_hResUnit == 1) {
		// convert pixels / inch to pixel / m
		res_x = (unsigned) (_hRes / 0.0254000 + 0.5);
	} else if(_hResUnit == 2) {
		// convert pixels / cm to pixel / m
		res_x = (unsigned) (_hRes * 100.0 + 0.5);
	}
	if(_vResUnit == 1) {
		// convert pixels / inch to pixel / m
		res_y = (unsigned) (_vRes / 0.0254000 + 0.5);
	} else if(_vResUnit == 2) {
		// convert pixels / cm to pixel / m
		res_y = (unsigned) (_vRes * 100.0 + 0.5);
	}
}

// --------------------------------------------------------------------------

psdResolutionInfo_v2::psdResolutionInfo_v2() { 
	_Channels = _Rows = _Columns = _Depth = _Mode = -1; 
}

psdResolutionInfo_v2::~psdResolutionInfo_v2() {
}

int psdResolutionInfo_v2::Read(FreeImageIO *io, fi_handle handle) {
	BYTE ShortValue[2];
	int nBytes=0, n;
	
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Channels = (short)psdGetValue(ShortValue, sizeof(_Channels) );

	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Rows = (short)psdGetValue(ShortValue, sizeof(_Rows) );
	
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Columns = (short)psdGetValue(ShortValue, sizeof(_Columns) );

	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Depth = (short)psdGetValue(ShortValue, sizeof(_Depth) );

	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Mode = (short)psdGetValue(ShortValue, sizeof(_Mode) );
	
	return nBytes;
}

// --------------------------------------------------------------------------

psdDisplayInfo::psdDisplayInfo() {
	_Opacity = _ColourSpace = -1;
	for (unsigned n = 0; n < 4; ++n) {
		_Colour[n] = 0;
	}
	_Kind = 0;
	_padding = '0';
}

psdDisplayInfo::~psdDisplayInfo() {
}
	
int psdDisplayInfo::Read(FreeImageIO *io, fi_handle handle) {
	BYTE ShortValue[2];
	int nBytes=0, n;
	
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_ColourSpace = (short)psdGetValue(ShortValue, sizeof(_ColourSpace) );
	
	for (unsigned i = 0; i < 4; ++i) {
		n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
		nBytes += n * sizeof(ShortValue);
		_Colour[i] = (short)psdGetValue(ShortValue, sizeof(_Colour[i]) );
	}
	
	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Opacity = (short)psdGetValue(ShortValue, sizeof(_Opacity) );
	assert( 0 <= _Opacity );
	assert( 100 >= _Opacity );
	
	BYTE c[1];
	n = (int)io->read_proc(&c, sizeof(c), 1, handle);
	nBytes += n * sizeof(c);
	_Kind = (BYTE)psdGetValue(c, sizeof(c));
	
	n = (int)io->read_proc(&c, sizeof(c), 1, handle);
	nBytes += n * sizeof(c);
	_padding = (BYTE)psdGetValue(c, sizeof(c));
	assert( 0 == _padding );
	
	return nBytes;
}

// --------------------------------------------------------------------------

psdThumbnail::psdThumbnail() : 
_Format(-1), _Width(-1), _Height(-1), _WidthBytes(-1), _Size(-1), _CompressedSize(-1), _BitPerPixel(-1), _Planes(-1), _plData(NULL) {
}

psdThumbnail::~psdThumbnail() { 
	SAFE_DELETE_ARRAY(_plData); 
}

int psdThumbnail::Read(FreeImageIO *io, fi_handle handle, int iTotalData, bool isBGR) {
	BYTE c[1], ShortValue[2], IntValue[4];
	int nBytes=0, n;
	
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_Format = psdGetValue(IntValue, sizeof(_Format) );
	
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_Width = psdGetValue(IntValue, sizeof(_Width) );
	
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_Height = psdGetValue(IntValue, sizeof(_Height) );
	
	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_WidthBytes = psdGetValue(IntValue, sizeof(_WidthBytes) );

	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_Size = psdGetValue(IntValue, sizeof(_Size) );

	n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
	nBytes += n * sizeof(IntValue);
	_CompressedSize = psdGetValue(IntValue, sizeof(_CompressedSize) );

	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_BitPerPixel = (short)psdGetValue(ShortValue, sizeof(_BitPerPixel) );

	n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
	nBytes += n * sizeof(ShortValue);
	_Planes = (short)psdGetValue(ShortValue, sizeof(_Planes) );

	_plData = new BYTE[iTotalData];
	  
	if (isBGR) {
		// In BGR format
		for (int i=0; i<iTotalData; i+=3 ) {
			n = (int)io->read_proc(&c, sizeof(BYTE), 1, handle);
			nBytes += n * sizeof(BYTE);
			_plData[i+2] = (BYTE)psdGetValue(c, sizeof(BYTE) );

			n = (int)io->read_proc(&c, sizeof(BYTE), 1, handle);
			nBytes += n * sizeof(BYTE);
			_plData[i+1] = (BYTE)psdGetValue(c, sizeof(BYTE) );

			n = (int)io->read_proc(&c, sizeof(BYTE), 1, handle);
			nBytes += n * sizeof(BYTE);
			_plData[i+0] = (BYTE)psdGetValue(c, sizeof(BYTE) );
		}
	} else {
		// In RGB format										
		for (int i=0; i<iTotalData; ++i) {
			n = (int)io->read_proc(&c, sizeof(BYTE), 1, handle);
			nBytes += n * sizeof(BYTE);
			_plData[i] = (BYTE)psdGetValue(c, sizeof(BYTE) );
		}
	}

	return nBytes;
}

//---------------------------------------------------------------------------

psdICCProfile::psdICCProfile() : _ProfileSize(0), _ProfileData(NULL) {
}

psdICCProfile::~psdICCProfile() {
	clear();
}

void psdICCProfile::clear() { SAFE_DELETE_ARRAY(_ProfileData); _ProfileSize = 0;}

int psdICCProfile::Read(FreeImageIO *io, fi_handle handle, int size) {
	int nBytes = 0, n;
	
	clear();
	
	_ProfileData = new (std::nothrow) BYTE[size];
	if(NULL != _ProfileData) {
		n = (int)io->read_proc(_ProfileData, 1, size, handle);
		_ProfileSize = size;
		nBytes += n * sizeof(BYTE);
	}

	return nBytes;
}

//---------------------------------------------------------------------------

#define PSD_CLAMP(v, min, max) ((v < min) ? min : (v > max) ? max : v)

/**
CIELab -> XYZ conversion from http://www.easyrgb.com/
*/
static void CIELabToXYZ(float L, float a, float b, float *X, float *Y, float *Z) {
	float pow_3;
	
	// CIELab -> XYZ conversion 
	// ------------------------
	float var_Y = (L + 16.F ) / 116.F;
	float var_X = a / 500.F + var_Y;
	float var_Z = var_Y - b / 200.F;

	pow_3 = powf(var_Y, 3);
	if(pow_3 > 0.008856F) {
		var_Y = pow_3;
	} else {
		var_Y = ( var_Y - 16.F / 116.F ) / 7.787F;
	}
	pow_3 = powf(var_X, 3);
	if(pow_3 > 0.008856F) {
		var_X = pow_3;
	} else {
		var_X = ( var_X - 16.F / 116.F ) / 7.787F;
	}
	pow_3 = powf(var_Z, 3);
	if(pow_3 > 0.008856F) {
		var_Z = pow_3;
	} else {
		var_Z = ( var_Z - 16.F / 116.F ) / 7.787F;
	}

	static const float ref_X =  95.047F;
	static const float ref_Y = 100.000F;
	static const float ref_Z = 108.883F;

	*X = ref_X * var_X;	// ref_X = 95.047 (Observer= 2°, Illuminant= D65)
	*Y = ref_Y * var_Y;	// ref_Y = 100.000
	*Z = ref_Z * var_Z;	// ref_Z = 108.883
}

/**
XYZ -> RGB conversion from http://www.easyrgb.com/
*/
static void XYZToRGB(float X, float Y, float Z, float *R, float *G, float *B) {
	float var_X = X / 100;        //X from 0 to  95.047 (Observer = 2°, Illuminant = D65)
	float var_Y = Y / 100;        //Y from 0 to 100.000
	float var_Z = Z / 100;        //Z from 0 to 108.883

	float var_R = var_X *  3.2406F + var_Y * -1.5372F + var_Z * -0.4986F;
	float var_G = var_X * -0.9689F + var_Y *  1.8758F + var_Z *  0.0415F;
	float var_B = var_X *  0.0557F + var_Y * -0.2040F + var_Z *  1.0570F;

	float exponent = 1.F / 2.4F;

	if(var_R > 0.0031308F) {
		var_R = 1.055F * powf(var_R, exponent) - 0.055F;
	} else {
		var_R = 12.92F * var_R;
	}
	if(var_G > 0.0031308F) {
		var_G = 1.055F * powf(var_G, exponent) - 0.055F;
	} else {
		var_G = 12.92F * var_G;
	}
	if(var_B > 0.0031308F) {
		var_B = 1.055F * powf(var_B, exponent) - 0.055F;
	} else {
		var_B = 12.92F * var_B;
	}

	*R = var_R;
	*G = var_G;
	*B = var_B;
}

/**
Red / Blue channel swapping
@param bits Image bits
@param height Image height
@param pitch Image pitch
@param Bpp Number of bytes per pixel (3 or 4)
*/
static 
void _swapRandB(BYTE* bits, unsigned height, unsigned pitch, unsigned Bpp) {
	if(Bpp > 4 || Bpp < 3)
		return;
	
	BYTE* line_start = bits;
	for(unsigned y = 0; y < height; ++y, line_start += pitch) {
		for(BYTE *line = line_start; line < line_start + pitch; line += Bpp) {
			INPLACESWAP(line[0], line[2]);
		}
	}
}

/**
Red / Blue channel swapping
@see _swapRandB
*/
static 
void swapRandB(FIBITMAP* dib) {
	_swapRandB(FreeImage_GetBits(dib), FreeImage_GetHeight(dib), FreeImage_GetPitch(dib), FreeImage_GetBPP(dib)/8);
}

inline
void assignTri(WORD r, WORD g, WORD b, WORD* out) {
	out[0]=	r;
	out[1]=	g;
	out[2]= b;
}

inline
void assignTri(BYTE r, BYTE g, BYTE b, BYTE* out) {
	out[FI_RGBA_RED]	= r;
	out[FI_RGBA_GREEN]	= g;
	out[FI_RGBA_BLUE]	= b;
}

template<class T>
static 
void CIELabToRGB(float L, float a, float b, T *rgb) {
	float X, Y, Z;
	float R, G, B;
	const float max_val = numeric_limits<T>::max();

	CIELabToXYZ(L, a, b, &X, &Y, &Z);
	XYZToRGB(X, Y, Z, &R, &G, &B);
	
	assignTri( (T)PSD_CLAMP(R * max_val, 0, max_val), (T)PSD_CLAMP(G * max_val, 0, max_val), (T)PSD_CLAMP(B * max_val, 0, max_val), rgb);
}

/**
CMYK -> CMY -> RGB conversion from http://www.easyrgb.com/

CMYK to CMY [0-1]: C,M,Y * (1 - K) + K
CMY to RGB [0-1]: (1 - C,M,Y)

=> R,G,B = (1 - C,M,Y) * (1 - K)
mapped to [0-MAX_VAL]: 
(MAX_VAL - C,M,Y) * (MAX_VAL - K) / MAX_VAL
*/
template<class T >
static inline
void CMYKToRGB(T C, T M, T Y, T K, T* out) {
	const unsigned max_val = numeric_limits<T>::max();
	
	unsigned r = (max_val - C) * (max_val - K) / max_val;
	unsigned g = (max_val - M) * (max_val - K) / max_val;
	unsigned b = (max_val - Y) * (max_val - K) / max_val;
	
	assignTri((T)PSD_CLAMP(r, 0, max_val), (T)PSD_CLAMP(g, 0, max_val), (T)PSD_CLAMP(b, 0, max_val), out);
}

template<class T>
static
void _convertCMYKtoRGBA(int width, int height, BYTE* line_start, unsigned pitch, unsigned ch) {
	const bool hasBlack = ch > 3;
	const T MAX_VAL = numeric_limits<T>::max();
		
	T K = 0;
	for(int y = 0; y < height; y++) {
		T *line = (T*)line_start;

		for(int x = 0; x < width; x++) {
			if(hasBlack) {
				K = line[FI_RGBA_ALPHA];
				line[FI_RGBA_ALPHA] = MAX_VAL; // TODO write the first extra channel as alpha!
			}
			
			
			CMYKToRGB<T>( line[0], line[1], line[2], K, line);
			
			line += ch;
		}
		line_start += pitch;
	}
}

/**
Inplace convert CMYK to RGBA (8- and 16-bit).
Alpha is filled with the first extra channel if any or white otherwise.
(Can be useful as public/utility function) (Also need in Tiff loading)
*/
static
void convertCMYKtoRGBA(FIBITMAP* dib) {
	const FREE_IMAGE_TYPE type = FreeImage_GetImageType(dib);
	const unsigned Bpp = FreeImage_GetBPP(dib)/8;
	
	BYTE chSize = 1;
	if (type == FIT_RGBA16 || type == FIT_RGB16) {
		chSize = sizeof(WORD);
	} else if (!(type == FIT_BITMAP && (Bpp > 2))) {
		return;
	}
				
	const int width = FreeImage_GetWidth(dib);
	const int height = FreeImage_GetHeight(dib);
	BYTE *line_start = FreeImage_GetScanLine(dib, 0);
	const unsigned pitch = FreeImage_GetPitch(dib);
	
	unsigned ch = FreeImage_GetLine(dib) / width / chSize;
		
	if(chSize == sizeof(WORD)) {
		_convertCMYKtoRGBA<WORD>(width, height, line_start, pitch, ch);
	}
	else {
		_convertCMYKtoRGBA<BYTE>(width, height, line_start, pitch, ch);
	}
}

template<class T>
static
void _convertLABtoRGB(int width, int height, BYTE* line_start, unsigned pitch, unsigned ch) {
	const unsigned max_val = numeric_limits<T>::max();
	const float sL = 100.F / max_val;
	const float sa = 256.F / max_val;
	const float sb = 256.F / max_val;
	
	for(int y = 0; y < height; y++) {
		T *line = (T*)line_start;

		for(int x = 0; x < width; x++) {
			CIELabToRGB(line[0]* sL, line[1]* sa - 128.F, line[2]* sb - 128.F, line);
			
			line += ch;
		}
		line_start += pitch;
	}
}

/**
Inplace convert CIELab to RGBA (8- and 16-bit).
(Can be useful as public/utility function)
*/
static
void convertLABtoRGB(FIBITMAP* dib) {
	const FREE_IMAGE_TYPE type = FreeImage_GetImageType(dib);
	const unsigned Bpp = FreeImage_GetBPP(dib)/8;
	
	BYTE chSize = 1;
	if (type == FIT_RGBA16 || type == FIT_RGB16) {
		chSize = sizeof(WORD);
	} else if (!(type == FIT_BITMAP && (Bpp > 2))) {
		return;
	}
				
	const int width = FreeImage_GetWidth(dib);
	const int height = FreeImage_GetHeight(dib);
	BYTE *line_start = FreeImage_GetScanLine(dib, 0);
	const unsigned pitch = FreeImage_GetPitch(dib);
	
	unsigned ch = FreeImage_GetLine(dib) / width / chSize;
			
	if(chSize == 1) {
		_convertLABtoRGB<BYTE>(width, height, line_start, pitch, ch);
	}
	else {
		_convertLABtoRGB<WORD>(width, height, line_start, pitch, ch);
	}
	
}

/**
RGBA to RGB (8- and 16-bit, trivial to extend to float)
(Can be useful as public/utility function)
*/
static
FIBITMAP* stripAlpha(FIBITMAP* dib) {
	const int width = FreeImage_GetWidth(dib);
	const int height = FreeImage_GetHeight(dib);
	BYTE *line_start = FreeImage_GetScanLine(dib, 0);
	const unsigned pitch = FreeImage_GetPitch(dib);
	const unsigned Bpp = FreeImage_GetBPP(dib)/8;
	
	FIBITMAP* dst_dib = NULL;
	if (FreeImage_GetImageType(dib) == FIT_RGBA16) {
		dst_dib = FreeImage_AllocateT(FIT_RGB16, width, height);
	} else {
		dst_dib = FreeImage_Allocate(width, height, 24);
	}
	if(!dst_dib)
		return NULL;
		
	BYTE *dst_line_start = FreeImage_GetScanLine(dst_dib, 0);
	const unsigned dstPitch = FreeImage_GetPitch(dst_dib);
	const unsigned dstBpp = FreeImage_GetBPP(dst_dib)/8;

	for(int y = 0; y < height; y++) {
		BYTE *line = line_start;
		BYTE *dst_line = dst_line_start;

		for(int x = 0; x < width; x++) {
			
			for(BYTE b=0; b < dstBpp; ++b) {
				dst_line[b] = line[b];
			}
				
			line += Bpp;
			dst_line += dstBpp;
		}
		line_start += pitch;
		dst_line_start += dstPitch;
	}
	
	// copy metadata from src to dst
	FreeImage_CloneMetadata(dst_dib, dib);
	
	return dst_dib;
}

/**
Invert only color components, skipping Alpha/Black
(Can be useful as public/utility function)
*/
static
BOOL invertColor(FIBITMAP* dib) {
	FREE_IMAGE_TYPE type = FreeImage_GetImageType(dib);
	const unsigned Bpp = FreeImage_GetBPP(dib)/8;
	
	if((type == FIT_BITMAP && Bpp == 4) || type == FIT_RGBA16) {
		const unsigned width = FreeImage_GetWidth(dib);
		const unsigned height = FreeImage_GetHeight(dib);
		BYTE *line_start = FreeImage_GetScanLine(dib, 0);
		const unsigned pitch = FreeImage_GetPitch(dib);
		const unsigned triBpp = Bpp - (Bpp == 4 ? 1 : 2);
				
		for(unsigned y = 0; y < height; y++) {
			BYTE *line = line_start;

			for(unsigned x = 0; x < width; x++) {
				for(unsigned b=0; b < triBpp; ++b) {
					line[b] = ~line[b];
				}
					
				line += Bpp;
			}
			line_start += pitch;
		}
		
		return TRUE;
	}
	else {
		return FreeImage_Invert(dib);
	}
}

//---------------------------------------------------------------------------

psdParser::psdParser() {
	_bThumbnailFilled = false;
	_bDisplayInfoFilled = false;
	_bResolutionInfoFilled = false;
	_bResolutionInfoFilled_v2 = false;
	_bCopyright = false;
	_GlobalAngle = 30;
	_ColourCount = -1;
	_TransparentIndex = -1;
	_fi_flags = 0;
	_fi_format_id = FIF_UNKNOWN;
}

psdParser::~psdParser() {
}

bool psdParser::ReadLayerAndMaskInfoSection(FreeImageIO *io, fi_handle handle)	{
	bool bSuccess = false;
	
	BYTE DataLength[4];
	int nBytes = 0;
	int n = (int)io->read_proc(&DataLength, sizeof(DataLength), 1, handle);
	int nTotalBytes = psdGetValue( DataLength, sizeof(DataLength) );
	
	BYTE data[1];
	while( n && ( nBytes < nTotalBytes ) ) {
		data[0] = '\0';
		n = (int)io->read_proc(&data, sizeof(data), 1, handle);
		nBytes += n * sizeof(data);
	}
	
	assert( nBytes == nTotalBytes );
	if ( nBytes == nTotalBytes ) {
		bSuccess = true;
	}
	
	return bSuccess;
}

bool psdParser::ReadImageResource(FreeImageIO *io, fi_handle handle) {
	psdImageResource oResource;
	bool bSuccess = false;
	
	BYTE Length[4];
	int n = (int)io->read_proc(&Length, sizeof(Length), 1, handle);
	
	oResource._Length = psdGetValue( Length, sizeof(oResource._Length) );
	
	int nBytes = 0;
	int nTotalBytes = oResource._Length;
	
	while(nBytes < nTotalBytes) {
		n = 0;
		oResource.Reset();
		
		n = (int)io->read_proc(&oResource._OSType, sizeof(oResource._OSType), 1, handle);
		nBytes += n * sizeof(oResource._OSType);
		assert( 0 == (nBytes % 2) );
		
		int nOSType = psdGetValue((BYTE*)&oResource._OSType, sizeof(oResource._OSType));

		if ( PSD_RESOURCE == nOSType ) {
			BYTE ID[2];
			n = (int)io->read_proc(&ID, sizeof(ID), 1, handle);
			nBytes += n * sizeof(ID);
			
			oResource._ID = (short)psdGetValue( ID, sizeof(ID) );
			
			BYTE SizeOfName;
			n = (int)io->read_proc(&SizeOfName, sizeof(SizeOfName), 1, handle);
			nBytes += n * sizeof(SizeOfName);
			
			int nSizeOfName = psdGetValue( &SizeOfName, sizeof(SizeOfName) );
			if ( 0 < nSizeOfName ) {
				oResource._plName = new BYTE[nSizeOfName];
				n = (int)io->read_proc(oResource._plName, nSizeOfName, 1, handle);
				nBytes += n * nSizeOfName;
			}
			
			if ( 0 == (nSizeOfName % 2) ) {
				n = (int)io->read_proc(&SizeOfName, sizeof(SizeOfName), 1, handle);
				nBytes += n * sizeof(SizeOfName);
			}
			
			BYTE Size[4];
			n = (int)io->read_proc(&Size, sizeof(Size), 1, handle);
			nBytes += n * sizeof(Size);
			
			oResource._Size = psdGetValue( Size, sizeof(oResource._Size) );
			
			if ( 0 != (oResource._Size % 2) ) {
				// resource data must be even
				oResource._Size++;
			}
			if ( 0 < oResource._Size ) {
				BYTE IntValue[4];
				BYTE ShortValue[2];
				
				switch( oResource._ID ) {
					case 1000:
						// Obsolete - Photoshop 2.0
						_bResolutionInfoFilled_v2 = true;
						nBytes += _resolutionInfo_v2.Read(io, handle);
						break;
					
					// ResolutionInfo structure
					case 1005:
						_bResolutionInfoFilled = true;
						nBytes += _resolutionInfo.Read(io, handle);
						break;
						
					// DisplayInfo structure
					case 1007:
						_bDisplayInfoFilled = true;
						nBytes += _displayInfo.Read(io, handle);
						break;
						
					// (Photoshop 4.0) Copyright flag
					// Boolean indicating whether image is copyrighted. Can be set via Property suite or by user in File Info...
					case 1034:
						n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
						nBytes += n * sizeof(ShortValue);
						_bCopyright = (1 == psdGetValue(ShortValue, sizeof(ShortValue)));
						break;
						
					// (Photoshop 4.0) Thumbnail resource for Photoshop 4.0 only
					case 1033:
					// (Photoshop 5.0) Thumbnail resource (supersedes resource 1033)
					case 1036:
					{
						_bThumbnailFilled = true;
						bool bBGR = (1033==oResource._ID);
						int nTotalData = oResource._Size - 28; // header
						nBytes += _thumbnail.Read(io, handle, nTotalData, bBGR);
						break;
					}
					
					// (Photoshop 5.0) Global Angle
					// 4 bytes that contain an integer between 0 and 359, which is the global
					// lighting angle for effects layer. If not present, assumed to be 30.
					case 1037:
						n = (int)io->read_proc(&IntValue, sizeof(IntValue), 1, handle);
						nBytes += n * sizeof(IntValue);
						_GlobalAngle = psdGetValue(IntValue, sizeof(_GlobalAngle) );
						break;

					// ICC profile
					case 1039:
						nBytes += _iccProfile.Read(io, handle, oResource._Size);
						break;

					// (Photoshop 6.0) Indexed Color Table Count
					// 2 bytes for the number of colors in table that are actually defined
					case 1046:
						n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
						nBytes += n * sizeof(ShortValue);
						_ColourCount = (short)psdGetValue(ShortValue, sizeof(ShortValue) );
						break;
						
					// (Photoshop 6.0) Transparency Index.
					// 2 bytes for the index of transparent color, if any.
					case 1047:
						n = (int)io->read_proc(&ShortValue, sizeof(ShortValue), 1, handle);
						nBytes += n * sizeof(ShortValue);
						_TransparentIndex = (short)psdGetValue(ShortValue, sizeof(ShortValue) );
						break;
						
					default:
					{
						// skip resource
						BYTE c[1];
						for(int i=0; i<oResource._Size; ++i) {
							n = (int)io->read_proc(&c, sizeof(c), 1, handle);
							nBytes += n * sizeof(c);
						}
					}
					break;
				}
			}
		}
  }
  
  assert(nBytes == nTotalBytes);
  if (nBytes == nTotalBytes) {
	  bSuccess = true;
  }
  
  return bSuccess;
  
} 

FIBITMAP* psdParser::ReadImageData(FreeImageIO *io, fi_handle handle) {
	if(handle == NULL) 
		return NULL;
	
	bool header_only = (_fi_flags & FIF_LOAD_NOPIXELS) == FIF_LOAD_NOPIXELS;
	
	assert(sizeof(WORD) == 2);
	WORD nCompression = 0;
	io->read_proc(&nCompression, sizeof(nCompression), 1, handle);
	
#ifndef FREEIMAGE_BIGENDIAN
	SwapShort(&nCompression);
#endif
	
	if((nCompression != PSDP_COMPRESSION_NONE && nCompression != PSDP_COMPRESSION_RLE))	{
		FreeImage_OutputMessageProc(_fi_format_id, "Unsupported compression %d", nCompression);
		return NULL;
	}
	
	const unsigned nWidth = _headerInfo._Width;
	const unsigned nHeight = _headerInfo._Height;
	const unsigned nChannels = _headerInfo._Channels;
	const unsigned depth = _headerInfo._BitsPerChannel;
	const unsigned bytes = (depth == 1) ? 1 : depth / 8;
		
	// channel(plane) line (BYTE aligned)
	const unsigned lineSize = (_headerInfo._BitsPerChannel == 1) ? (nWidth + 7) / 8 : nWidth * bytes;
	
	if(nCompression == PSDP_COMPRESSION_RLE && depth > 16) {
		FreeImage_OutputMessageProc(_fi_format_id, "Unsupported RLE with depth %d", depth);
		return NULL;
	}
	
	// build output buffer
	
	FIBITMAP* bitmap = NULL;
	unsigned dstCh = 0;
	
	short mode = _headerInfo._ColourMode;
	
	if(mode == PSDP_MULTICHANNEL && nChannels < 3) {
		// CM 
		mode = PSDP_GRAYSCALE; // C as gray, M as extra channel
	}
		
	bool needPalette = false;
	switch (mode) {
		case PSDP_BITMAP:
		case PSDP_DUOTONE:	
		case PSDP_INDEXED:
		case PSDP_GRAYSCALE:
			dstCh = 1;
			switch(depth) {
				case 16:
				bitmap = FreeImage_AllocateHeaderT(header_only, FIT_UINT16, nWidth, nHeight, depth*dstCh);
				break;
				case 32:
				bitmap = FreeImage_AllocateHeaderT(header_only, FIT_FLOAT, nWidth, nHeight, depth*dstCh);
				break;
				default: // 1-, 8-
				needPalette = true;
				bitmap = FreeImage_AllocateHeader(header_only, nWidth, nHeight, depth*dstCh);
				break;
			}
			break;
		case PSDP_RGB:	
		case PSDP_LAB:		
		case PSDP_CMYK	:
		case PSDP_MULTICHANNEL	:
			// force PSDP_MULTICHANNEL CMY as CMYK
			dstCh = (mode == PSDP_MULTICHANNEL && !header_only) ? 4 : MIN<unsigned>(nChannels, 4);
			assert(dstCh >= 3);
			switch(depth) {
				case 16:
				bitmap = FreeImage_AllocateHeaderT(header_only, dstCh < 4 ? FIT_RGB16 : FIT_RGBA16, nWidth, nHeight, depth*dstCh);
				break;
				case 32:
				bitmap = FreeImage_AllocateHeaderT(header_only, dstCh < 4 ? FIT_RGBF : FIT_RGBAF, nWidth, nHeight, depth*dstCh);
				break;
				default:
				bitmap = FreeImage_AllocateHeader(header_only, nWidth, nHeight, depth*dstCh);
				break;
			}
			break;
		default:
			assert(false);
			break;
	}
	if(!bitmap) {
		throw FI_MSG_ERROR_DIB_MEMORY;
	}
		
	// @todo Add some metadata model
		
	if(header_only) {
		return bitmap;
	}
	
	// Load pixels data

	const unsigned dstChannels = dstCh;
	
	const unsigned dstBpp =  (depth == 1) ? 1 : FreeImage_GetBPP(bitmap)/8;
	const unsigned dstLineSize = FreeImage_GetPitch(bitmap);	
	BYTE* const dst_first_line = FreeImage_GetScanLine(bitmap, nHeight - 1);//<*** flipped
	
	BYTE* line_start = new BYTE[lineSize]; //< fileline cache

	switch ( nCompression ) {
		case PSDP_COMPRESSION_NONE: // raw data	
		{			
			for(unsigned c = 0; c < nChannels; c++) {
				if(c >= dstChannels) {
					// @todo write extra channels
					break; 
				}
					
				const unsigned channelOffset = c * bytes;
				
				BYTE* dst_line_start = dst_first_line;
				for(unsigned h = 0; h < nHeight; ++h, dst_line_start -= dstLineSize) {//<*** flipped

					io->read_proc(line_start, lineSize, 1, handle);
					
					for (BYTE *line = line_start, *dst_line = dst_line_start; line < line_start + lineSize; 
						line += bytes, dst_line += dstBpp) {
#ifdef FREEIMAGE_BIGENDIAN
							memcpy(dst_line + channelOffset, line, bytes);
#else
						// reverse copy bytes
						for (unsigned b = 0; b < bytes; ++b) {
							dst_line[channelOffset + b] = line[(bytes-1) - b];
						}
#endif // FREEIMAGE_BIGENDIAN
					}
				} //< h
			}//< ch
			
			SAFE_DELETE_ARRAY(line_start);
					
		}
		break;
		
		case PSDP_COMPRESSION_RLE: // RLE compression	
		{			
									
			// The RLE-compressed data is preceeded by a 2-byte line size for each row in the data,
			// store an array of these

			// later use this array as WORD rleLineSizeList[nChannels][nHeight];
			WORD *rleLineSizeList = new (std::nothrow) WORD[nChannels*nHeight];

			if(!rleLineSizeList) {
				FreeImage_Unload(bitmap);
				SAFE_DELETE_ARRAY(line_start);
				throw std::bad_alloc();
			}	
			
			io->read_proc(rleLineSizeList, 2, nChannels * nHeight, handle);
			
			WORD largestRLELine = 0;
			for(unsigned ch = 0; ch < nChannels; ++ch) {
				for(unsigned h = 0; h < nHeight; ++h) {
					const unsigned index = ch * nHeight + h;

#ifndef FREEIMAGE_BIGENDIAN 
					SwapShort(&rleLineSizeList[index]);
#endif
					if(largestRLELine < rleLineSizeList[index]) {
						largestRLELine = rleLineSizeList[index];
					}
				}
			}

			BYTE* rle_line_start = new (std::nothrow) BYTE[largestRLELine];
			if(!rle_line_start) {
				FreeImage_Unload(bitmap);
				SAFE_DELETE_ARRAY(line_start);
				SAFE_DELETE_ARRAY(rleLineSizeList);
				throw std::bad_alloc();
			}
			
			// Read the RLE data (assume 8-bit)
			
			const BYTE* const line_end = line_start + lineSize;

			for (unsigned ch = 0; ch < nChannels; ch++) {
				const unsigned channelOffset = ch * bytes;
				
				BYTE* dst_line_start = dst_first_line;
				for(unsigned h = 0; h < nHeight; ++h, dst_line_start -= dstLineSize) {//<*** flipped
					const unsigned index = ch * nHeight + h;
					
					// - read and uncompress line -
					
					const WORD rleLineSize = rleLineSizeList[index];
					
					io->read_proc(rle_line_start, rleLineSize, 1, handle);
					
					for (BYTE* rle_line = rle_line_start, *line = line_start; 
						rle_line < rle_line_start + rleLineSize, line < line_end;) {

						int len = *rle_line++;
						
						// NOTE len is signed byte in PackBits RLE
						
						if ( len < 128 ) { //<- MSB is not set
							// uncompressed packet
							
							// (len + 1) bytes of data are copied
							
							++len;
							
							// assert we don't write beyound eol
							memcpy(line, rle_line, line + len > line_end ? line_end - line : len);
							line += len;
							rle_line += len;
						}
						else if ( len > 128 ) { //< MSB is set
						
							// RLE compressed packet
							
							// One byte of data is repeated (–len + 1) times
							
							len ^= 0xFF; // same as (-len + 1) & 0xFF 
							len += 2;    //

							// assert we don't write beyound eol
							memset(line, *rle_line++, line + len > line_end ? line_end - line : len);							
							line += len;

						}
						else if ( 128 == len ) {
							// Do nothing
						}
					}//< rle_line
					
					// - write line to destination -
					
					if(ch >= dstChannels) {
						// @todo write to extra channels
						break; 
					}
						
					// byte by byte copy a single channel to pixel
					for (BYTE *line = line_start, *dst_line = dst_line_start; line < line_start + lineSize; 
						line += bytes, dst_line += dstBpp) {

#ifdef FREEIMAGE_BIGENDIAN
							memcpy(dst_line + channelOffset, line, bytes);
#else
							// reverse copy bytes
							for (unsigned b = 0; b < bytes; ++b) {
								dst_line[channelOffset + b] = line[(bytes-1) - b];							
							}
#endif // FREEIMAGE_BIGENDIAN
					}	
				}//< h
			}//< ch
			
			SAFE_DELETE_ARRAY(line_start);
			SAFE_DELETE_ARRAY(rleLineSizeList);
			SAFE_DELETE_ARRAY(rle_line_start);
		}
		break;
		
		case 2: // ZIP without prediction, no specification
			break;
			
		case 3: // ZIP with prediction, no specification
			break;
			
		default: // Unknown format
			break;
		
	}
	
	// --- Further process the bitmap ---
	
	if((mode == PSDP_CMYK || mode == PSDP_MULTICHANNEL)) {	
		// CMYK values are "inverted", invert them back		

		if(mode == PSDP_MULTICHANNEL) {
			invertColor(bitmap);
		} else {
			FreeImage_Invert(bitmap);
		}

		if((_fi_flags & PSD_CMYK) == PSD_CMYK) {
			// keep as CMYK

			if(mode == PSDP_MULTICHANNEL) {
				//### we force CMY to be CMYK, but CMY has no ICC. 
				// Create empty profile and add the flag.
				FreeImage_CreateICCProfile(bitmap, NULL, 0);
				FreeImage_GetICCProfile(bitmap)->flags |= FIICC_COLOR_IS_CMYK;
			}
		}
		else { 
			// convert to RGB
			
			convertCMYKtoRGBA(bitmap);
			
			// The ICC Profile is no longer valid
			_iccProfile.clear();
			
			// remove the pending A if not present in source 
			if(nChannels == 4 || nChannels == 3 ) {
				FIBITMAP* t = stripAlpha(bitmap);
				if(t) {
					FreeImage_Unload(bitmap);
					bitmap = t;
				} // else: silently fail
			}
		}
	}
	else if ( mode == PSDP_LAB && !((_fi_flags & PSD_LAB) == PSD_LAB)) {
		convertLABtoRGB(bitmap);
	}
	else {
		if (needPalette) {
			assert(FreeImage_GetPalette(bitmap));
			
			if(mode == PSDP_BITMAP) {
				CREATE_GREYSCALE_PALETTE_REVERSE(FreeImage_GetPalette(bitmap), 2);
			}
			else if(mode == PSDP_INDEXED) {
				if(!_colourModeData._plColourData || _colourModeData._Length != 768 || _ColourCount < 0) {
					FreeImage_OutputMessageProc(_fi_format_id, "Indexed image has no palette. Using the default grayscale one.");
				} else {
					_colourModeData.FillPalette(bitmap);
				}
			}
			// GRAYSCALE, DUOTONE - use default grayscale palette
		}
		
#if FREEIMAGE_COLORORDER == FREEIMAGE_COLORORDER_BGR
		if(FreeImage_GetImageType(bitmap) == FIT_BITMAP) {
			swapRandB(bitmap);
		}
#endif
	}
	
	return bitmap;
} 

FIBITMAP* psdParser::Load(FreeImageIO *io, fi_handle handle, int s_format_id, int flags) {
	FIBITMAP *Bitmap = NULL;
	
	_fi_flags = flags;
	_fi_format_id = s_format_id;
	
	try {
		if (NULL == handle) {
			throw("Cannot open file");
		}
		
		if (!_headerInfo.Read(io, handle)) {
			throw("Error in header");
		}

		if (!_colourModeData.Read(io, handle)) {
			throw("Error in ColourMode Data");
		}
		
		if (!ReadImageResource(io, handle)) {
			throw("Error in Image Resource");
		}
		
		if (!ReadLayerAndMaskInfoSection(io, handle)) {
			throw("Error in Mask Info");
		}
		
		Bitmap = ReadImageData(io, handle);
		if (NULL == Bitmap) {
			throw("Error in Image Data");
		}

		// set resolution info
		if(NULL != Bitmap) {
			unsigned res_x = 2835;	// 72 dpi
			unsigned res_y = 2835;	// 72 dpi
			if (_bResolutionInfoFilled) {
				_resolutionInfo.GetResolutionInfo(res_x, res_y);
			}
			FreeImage_SetDotsPerMeterX(Bitmap, res_x);
			FreeImage_SetDotsPerMeterY(Bitmap, res_y);	
		}

		// set ICC profile
		if(NULL != _iccProfile._ProfileData) {
			FreeImage_CreateICCProfile(Bitmap, _iccProfile._ProfileData, _iccProfile._ProfileSize);
			if ((flags & PSD_CMYK) == PSD_CMYK) {
				FreeImage_GetICCProfile(Bitmap)->flags |= FIICC_COLOR_IS_CMYK;
			}
		}
		
	} catch(const char *text) {
		FreeImage_OutputMessageProc(s_format_id, text);
	}
	catch(const std::exception& e) {
		FreeImage_OutputMessageProc(s_format_id, "%s", e.what());
	}

	return Bitmap;
} 
