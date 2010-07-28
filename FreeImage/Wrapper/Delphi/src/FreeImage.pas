unit FreeImage;

// ==========================================================
// Delphi wrapper for FreeImage 3
//
// Design and implementation by
// - Simon Beavis
// - Peter Byström
// - Anatoliy Pulyaevskiy (xvel84@rambler.ru)
//
// Contributors:
// - Lorenzo Monti (LM)  lomo74@gmail.com
//
// Revision history
// When        Who   What
// ----------- ----- -----------------------------------------------------------
// 2010-07-14  LM    Fixed some C->Delphi translation errors,
//                   updated to 3.13.1, made RAD2010 compliant (unicode)
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

interface

{$I 'Version.inc'}

uses Windows;

{$MINENUMSIZE 4} // Make sure enums are stored as an integer to be compatible with C/C++

const
  // Version information
  FREEIMAGE_MAJOR_VERSION  = 3;
  FREEIMAGE_MINOR_VERSION  = 13;
  FREEIMAGE_RELEASE_SERIAL = 1;
  // This really only affects 24 and 32 bit formats, the rest are always RGB order.
  FREEIMAGE_COLORORDER_BGR = 0;
  FREEIMAGE_COLORORDER_RGB = 1;
  FREEIMAGE_COLORORDER = FREEIMAGE_COLORORDER_BGR;

// --------------------------------------------------------------------------
// Bitmap types -------------------------------------------------------------
// --------------------------------------------------------------------------

type
  FIBITMAP = record
    data: Pointer;
  end;
  PFIBITMAP = ^FIBITMAP;

  FIMULTIBITMAP = record
    data: Pointer;
  end;
  PFIMULTIBITMAP = ^FIMULTIBITMAP;

// --------------------------------------------------------------------------
// Types used in the library (specific to FreeImage) ------------------------
// --------------------------------------------------------------------------

type
  {* 48-bit RGB }
  tagFIRGB16 = packed record
	  red: WORD;
	  green: WORD;
	  blue: WORD;
  end;
  FIRGB16 = tagFIRGB16;

  {* 64-bit RGBA }
  tagFIRGBA16 = packed record
  	red: WORD;
	  green: WORD;
  	blue: WORD;
	  alpha: WORD;
  end;
  FIRGBA16 = tagFIRGBA16;

  {* 96-bit RGB Float }
  tagFIRGBF = packed record
    red: Single;
    green: Single;
    blue: Single;
  end;
  FIRGBF = tagFIRGBF;

  {* 128-bit RGBA Float }
  tagFIRGBAF = packed record
    red: Single;
    green: Single;
    blue: Single;
    alpha: Single;
  end;
  FIRGBAF = tagFIRGBAF;

  {* Data structure for COMPLEX type (complex number) }
  tagFICOMPLEX = packed record
    /// real part
	  r: Double;
  	/// imaginary part
    i: Double;
  end;
  FICOMPLEX = tagFICOMPLEX;

// --------------------------------------------------------------------------
// Indexes for byte arrays, masks and shifts for treating pixels as words ---
// These coincide with the order of RGBQUAD and RGBTRIPLE -------------------
// Little Endian (x86 / MS Windows, Linux) : BGR(A) order -------------------
// --------------------------------------------------------------------------

const
  FI_RGBA_RED         = 2;
  FI_RGBA_GREEN       = 1;
  FI_RGBA_BLUE        = 0;
  FI_RGBA_ALPHA       = 3;
  FI_RGBA_RED_MASK    = $00FF0000;
  FI_RGBA_GREEN_MASK  = $0000FF00;
  FI_RGBA_BLUE_MASK   = $000000FF;
  FI_RGBA_ALPHA_MASK  = $FF000000;
  FI_RGBA_RED_SHIFT   = 16;
  FI_RGBA_GREEN_SHIFT = 8;
  FI_RGBA_BLUE_SHIFT  = 0;
  FI_RGBA_ALPHA_SHIFT = 24;

  FI_RGBA_RGB_MASK = FI_RGBA_RED_MASK or FI_RGBA_GREEN_MASK or FI_RGBA_BLUE_MASK;

// --------------------------------------------------------------------------
// The 16bit macros only include masks and shifts, --------------------------
// since each color element is not byte aligned -----------------------------
// --------------------------------------------------------------------------

const
  FI16_555_RED_MASK		 = $7C00;
  FI16_555_GREEN_MASK	 = $03E0;
  FI16_555_BLUE_MASK	 = $001F;
  FI16_555_RED_SHIFT	 = 10;
  FI16_555_GREEN_SHIFT = 5;
  FI16_555_BLUE_SHIFT	 = 0;
  FI16_565_RED_MASK		 = $F800;
  FI16_565_GREEN_MASK	 = $07E0;
  FI16_565_BLUE_MASK	 = $001F;
  FI16_565_RED_SHIFT	 = 11;
  FI16_565_GREEN_SHIFT = 5;
  FI16_565_BLUE_SHIFT	 = 0;

// --------------------------------------------------------------------------
// ICC profile support ------------------------------------------------------
// --------------------------------------------------------------------------

const
  FIICC_DEFAULT = $0;
  FIICC_COLOR_IS_CMYK	= $1;

type
  FIICCPROFILE = record
    flags: WORD;   // info flag
    size: DWORD;   // profile's size measured in bytes
    data: Pointer; // points to a block of contiguous memory containing the profile
  end;
  PFIICCPROFILE = ^FIICCPROFILE;

// --------------------------------------------------------------------------
// Important enums ----------------------------------------------------------
// --------------------------------------------------------------------------

type
  FREE_IMAGE_FORMAT         = type Integer;
  FREE_IMAGE_TYPE           = type Integer;
  FREE_IMAGE_COLOR_TYPE     = type Integer;
  FREE_IMAGE_QUANTIZE       = type Integer;
  FREE_IMAGE_DITHER         = type Integer;
  FREE_IMAGE_FILTER         = type Integer;
  FREE_IMAGE_COLOR_CHANNEL  = type Integer;
  FREE_IMAGE_MDTYPE         = type Integer;
  FREE_IMAGE_MDMODEL        = type Integer;
  FREE_IMAGE_JPEG_OPERATION = type Integer;
  FREE_IMAGE_TMO            = type Integer;

const
  // I/O image format identifiers.
  FIF_UNKNOWN = FREE_IMAGE_FORMAT(-1);
  FIF_BMP     = FREE_IMAGE_FORMAT(0);
  FIF_ICO     = FREE_IMAGE_FORMAT(1);
  FIF_JPEG    = FREE_IMAGE_FORMAT(2);
  FIF_JNG     = FREE_IMAGE_FORMAT(3);
  FIF_KOALA   = FREE_IMAGE_FORMAT(4);
  FIF_LBM     = FREE_IMAGE_FORMAT(5);
  FIF_IFF     = FIF_LBM;
  FIF_MNG     = FREE_IMAGE_FORMAT(6);
  FIF_PBM     = FREE_IMAGE_FORMAT(7);
  FIF_PBMRAW  = FREE_IMAGE_FORMAT(8);
  FIF_PCD     = FREE_IMAGE_FORMAT(9);
  FIF_PCX     = FREE_IMAGE_FORMAT(10);
  FIF_PGM     = FREE_IMAGE_FORMAT(11);
  FIF_PGMRAW  = FREE_IMAGE_FORMAT(12);
  FIF_PNG     = FREE_IMAGE_FORMAT(13);
  FIF_PPM     = FREE_IMAGE_FORMAT(14);
  FIF_PPMRAW  = FREE_IMAGE_FORMAT(15);
  FIF_RAS     = FREE_IMAGE_FORMAT(16);
  FIF_TARGA   = FREE_IMAGE_FORMAT(17);
  FIF_TIFF    = FREE_IMAGE_FORMAT(18);
  FIF_WBMP    = FREE_IMAGE_FORMAT(19);
  FIF_PSD     = FREE_IMAGE_FORMAT(20);
  FIF_CUT     = FREE_IMAGE_FORMAT(21);
  FIF_XBM     = FREE_IMAGE_FORMAT(22);
  FIF_XPM     = FREE_IMAGE_FORMAT(23);
  FIF_DDS     = FREE_IMAGE_FORMAT(24);
  FIF_GIF     = FREE_IMAGE_FORMAT(25);
  FIF_HDR     = FREE_IMAGE_FORMAT(26);
  FIF_FAXG3   = FREE_IMAGE_FORMAT(27);
  FIF_SGI     = FREE_IMAGE_FORMAT(28);
  FIF_EXR     = FREE_IMAGE_FORMAT(29);
  FIF_J2K     = FREE_IMAGE_FORMAT(30);
  FIF_JP2     = FREE_IMAGE_FORMAT(31);
  FIF_PFM     = FREE_IMAGE_FORMAT(32);
  FIF_PICT    = FREE_IMAGE_FORMAT(33);
  FIF_RAW     = FREE_IMAGE_FORMAT(34);

  // Image type used in FreeImage.
  FIT_UNKNOWN = FREE_IMAGE_TYPE(0);  // unknown type
  FIT_BITMAP  = FREE_IMAGE_TYPE(1);	 // standard image: 1-, 4-, 8-, 16-, 24-, 32-bit
  FIT_UINT16  = FREE_IMAGE_TYPE(2);	 // array of unsigned short: unsigned 16-bit
  FIT_INT16   = FREE_IMAGE_TYPE(3);  // array of short: signed 16-bit
  FIT_UINT32  = FREE_IMAGE_TYPE(4);	 // array of unsigned long: unsigned 32-bit
  FIT_INT32   = FREE_IMAGE_TYPE(5);	 // array of long: signed 32-bit
  FIT_FLOAT   = FREE_IMAGE_TYPE(6);	 // array of float: 32-bit IEEE floating point
  FIT_DOUBLE  = FREE_IMAGE_TYPE(7);	 // array of double: 64-bit IEEE floating point
  FIT_COMPLEX = FREE_IMAGE_TYPE(8);	 // array of FICOMPLEX: 2 x 64-bit IEEE floating point
  FIT_RGB16	  = FREE_IMAGE_TYPE(9);	 // 48-bit RGB image: 3 x 16-bit
	FIT_RGBA16	= FREE_IMAGE_TYPE(10); // 64-bit RGBA image: 4 x 16-bit
	FIT_RGBF	  = FREE_IMAGE_TYPE(11); // 96-bit RGB float image: 3 x 32-bit IEEE floating point
	FIT_RGBAF	  = FREE_IMAGE_TYPE(12); // 128-bit RGBA float image: 4 x 32-bit IEEE floating point

  // Image color type used in FreeImage.
  FIC_MINISWHITE = FREE_IMAGE_COLOR_TYPE(0); // min value is white
  FIC_MINISBLACK = FREE_IMAGE_COLOR_TYPE(1); // min value is black
  FIC_RGB        = FREE_IMAGE_COLOR_TYPE(2); // RGB color model
  FIC_PALETTE    = FREE_IMAGE_COLOR_TYPE(3); // color map indexed
  FIC_RGBALPHA   = FREE_IMAGE_COLOR_TYPE(4); // RGB color model with alpha channel
  FIC_CMYK       = FREE_IMAGE_COLOR_TYPE(5); // CMYK color model

  // Color quantization algorithms. Constants used in FreeImage_ColorQuantize.
  FIQ_WUQUANT = FREE_IMAGE_QUANTIZE(0);	// Xiaolin Wu color quantization algorithm
  FIQ_NNQUANT = FREE_IMAGE_QUANTIZE(1);	// NeuQuant neural-net quantization algorithm by Anthony Dekker

  // Dithering algorithms. Constants used FreeImage_Dither.
  FID_FS            = FREE_IMAGE_DITHER(0);	// Floyd & Steinberg error diffusion
  FID_BAYER4x4      = FREE_IMAGE_DITHER(1);	// Bayer ordered dispersed dot dithering (order 2 dithering matrix)
  FID_BAYER8x8      = FREE_IMAGE_DITHER(2);	// Bayer ordered dispersed dot dithering (order 3 dithering matrix)
  FID_CLUSTER6x6    = FREE_IMAGE_DITHER(3);	// Ordered clustered dot dithering (order 3 - 6x6 matrix)
  FID_CLUSTER8x8    = FREE_IMAGE_DITHER(4);	// Ordered clustered dot dithering (order 4 - 8x8 matrix)
  FID_CLUSTER16x16  = FREE_IMAGE_DITHER(5); // Ordered clustered dot dithering (order 8 - 16x16 matrix)
  FID_BAYER16x16    = FREE_IMAGE_DITHER(6); // Bayer ordered dispersed dot dithering (order 4 dithering matrix)

  // Lossless JPEG transformations Constants used in FreeImage_JPEGTransform
	FIJPEG_OP_NONE			  = FREE_IMAGE_JPEG_OPERATION(0);	// no transformation
	FIJPEG_OP_FLIP_H		  = FREE_IMAGE_JPEG_OPERATION(1);	// horizontal flip
	FIJPEG_OP_FLIP_V		  = FREE_IMAGE_JPEG_OPERATION(2);	// vertical flip
	FIJPEG_OP_TRANSPOSE		= FREE_IMAGE_JPEG_OPERATION(3);	// transpose across UL-to-LR axis
	FIJPEG_OP_TRANSVERSE	= FREE_IMAGE_JPEG_OPERATION(4);	// transpose across UR-to-LL axis
	FIJPEG_OP_ROTATE_90		= FREE_IMAGE_JPEG_OPERATION(5);	// 90-degree clockwise rotation
	FIJPEG_OP_ROTATE_180	= FREE_IMAGE_JPEG_OPERATION(6);	// 180-degree rotation
	FIJPEG_OP_ROTATE_270	= FREE_IMAGE_JPEG_OPERATION(7); // 270-degree clockwise (or 90 ccw)

  // Tone mapping operators. Constants used in FreeImage_ToneMapping.
  FITMO_DRAGO03	   = FREE_IMAGE_TMO(0);	// Adaptive logarithmic mapping (F. Drago, 2003)
	FITMO_REINHARD05 = FREE_IMAGE_TMO(1);	// Dynamic range reduction inspired by photoreceptor physiology (E. Reinhard, 2005)
  FITMO_FATTAL02   = FREE_IMAGE_TMO(2); // Gradient domain high dynamic range compression (R. Fattal, 2002)

  // Upsampling / downsampling filters. Constants used in FreeImage_Rescale.
  FILTER_BOX	      = FREE_IMAGE_FILTER(0);	// Box, pulse, Fourier window, 1st order (constant) b-spline
  FILTER_BICUBIC    = FREE_IMAGE_FILTER(1);	// Mitchell & Netravali's two-param cubic filter
  FILTER_BILINEAR   = FREE_IMAGE_FILTER(2);	// Bilinear filter
  FILTER_BSPLINE    = FREE_IMAGE_FILTER(3);	// 4th order (cubic) b-spline
  FILTER_CATMULLROM = FREE_IMAGE_FILTER(4);	// Catmull-Rom spline, Overhauser spline
  FILTER_LANCZOS3   = FREE_IMAGE_FILTER(5);	// Lanczos3 filter

  // Color channels. Constants used in color manipulation routines.
  FICC_RGB   = FREE_IMAGE_COLOR_CHANNEL(0); // Use red, green and blue channels
  FICC_RED   = FREE_IMAGE_COLOR_CHANNEL(1); // Use red channel
  FICC_GREEN = FREE_IMAGE_COLOR_CHANNEL(2); // Use green channel
  FICC_BLUE  = FREE_IMAGE_COLOR_CHANNEL(3); // Use blue channel
  FICC_ALPHA = FREE_IMAGE_COLOR_CHANNEL(4); // Use alpha channel
  FICC_BLACK = FREE_IMAGE_COLOR_CHANNEL(5); // Use black channel
  FICC_REAL  = FREE_IMAGE_COLOR_CHANNEL(6); // Complex images: use real part
  FICC_IMAG  = FREE_IMAGE_COLOR_CHANNEL(7); // Complex images: use imaginary part
  FICC_MAG   = FREE_IMAGE_COLOR_CHANNEL(8); // Complex images: use magnitude
  FICC_PHASE = FREE_IMAGE_COLOR_CHANNEL(9);	// Complex images: use phase

  // Tag data type information (based on TIFF specifications)
  FIDT_NOTYPE	   = FREE_IMAGE_MDTYPE(0);	// placeholder
  FIDT_BYTE	     = FREE_IMAGE_MDTYPE(1);	// 8-bit unsigned integer
  FIDT_ASCII	   = FREE_IMAGE_MDTYPE(2);	// 8-bit bytes w/ last byte null
  FIDT_SHORT	   = FREE_IMAGE_MDTYPE(3);	// 16-bit unsigned integer
  FIDT_LONG	     = FREE_IMAGE_MDTYPE(4);	// 32-bit unsigned integer
  FIDT_RATIONAL  = FREE_IMAGE_MDTYPE(5);	// 64-bit unsigned fraction
  FIDT_SBYTE	   = FREE_IMAGE_MDTYPE(6);	// 8-bit signed integer
  FIDT_UNDEFINED = FREE_IMAGE_MDTYPE(7);	// 8-bit untyped data
  FIDT_SSHORT	   = FREE_IMAGE_MDTYPE(8);	// 16-bit signed integer
  FIDT_SLONG	   = FREE_IMAGE_MDTYPE(9);	// 32-bit signed integer
  FIDT_SRATIONAL = FREE_IMAGE_MDTYPE(10); // 64-bit signed fraction
  FIDT_FLOAT	   = FREE_IMAGE_MDTYPE(11); // 32-bit IEEE floating point
  FIDT_DOUBLE	   = FREE_IMAGE_MDTYPE(12); // 64-bit IEEE floating point
  FIDT_IFD	     = FREE_IMAGE_MDTYPE(13);	// 32-bit unsigned integer (offset)
  FIDT_PALETTE	 = FREE_IMAGE_MDTYPE(14);	// 32-bit RGBQUAD

  // Metadata models supported by FreeImage
  FIMD_NODATA	        = FREE_IMAGE_MDMODEL(-1);
  FIMD_COMMENTS	      = FREE_IMAGE_MDMODEL(0);  // single comment or keywords
  FIMD_EXIF_MAIN      = FREE_IMAGE_MDMODEL(1);  // Exif-TIFF metadata
  FIMD_EXIF_EXIF      = FREE_IMAGE_MDMODEL(2);  // Exif-specific metadata
  FIMD_EXIF_GPS	      = FREE_IMAGE_MDMODEL(3);  // Exif GPS metadata
  FIMD_EXIF_MAKERNOTE = FREE_IMAGE_MDMODEL(4);  // Exif maker note metadata
  FIMD_EXIF_INTEROP   = FREE_IMAGE_MDMODEL(5);  // Exif interoperability metadata
  FIMD_IPTC	          = FREE_IMAGE_MDMODEL(6);  // IPTC/NAA metadata
  FIMD_XMP	          = FREE_IMAGE_MDMODEL(7);  // Abobe XMP metadata
  FIMD_GEOTIFF	      = FREE_IMAGE_MDMODEL(8);  // GeoTIFF metadata (to be implemented)
  FIMD_ANIMATION		  = FREE_IMAGE_MDMODEL(9);  // Animation metadata
  FIMD_CUSTOM	        = FREE_IMAGE_MDMODEL(10); // Used to attach other metadata types to a dib

//{$endif}

type
  // Handle to a metadata model
  FIMETADATA = record
    data: Pointer;
  end;
  PFIMETADATA = ^FIMETADATA;

  // Handle to a metadata tag
  FITAG = record
    data: Pointer;
  end;
  PFITAG = ^FITAG;

// --------------------------------------------------------------------------
// File IO routines ---------------------------------------------------------
// --------------------------------------------------------------------------

type
  fi_handle = Pointer;

  FI_ReadProc = function(buffer: Pointer; size, count: Cardinal;
    handle: fi_handle): Cardinal; stdcall;
  FI_WriteProc = function(buffer: Pointer; size, count: Cardinal;
    handle: fi_handle): Cardinal; stdcall;
  FI_SeekProc = function(handle: fi_handle; offset: LongInt;
    origin: Integer): Integer; stdcall;
  FI_TellProc = function(handle: fi_handle): LongInt; stdcall;

  FreeImageIO = packed record
    read_proc : FI_ReadProc;     // pointer to the function used to read data
    write_proc: FI_WriteProc;    // pointer to the function used to write data
    seek_proc : FI_SeekProc;     // pointer to the function used to seek
    tell_proc : FI_TellProc;     // pointer to the function used to aquire the current position
  end;
  PFreeImageIO = ^FreeImageIO;

  // Handle to a memory I/O stream
  FIMEMORY = record
    data: Pointer;
  end;
  PFIMEMORY = ^FIMEMORY;

const
  // constants used in FreeImage_Seek for Origin parameter
  SEEK_SET = 0;
  SEEK_CUR = 1;
  SEEK_END = 2;

// --------------------------------------------------------------------------
// Plugin routines ----------------------------------------------------------
// --------------------------------------------------------------------------

type
  PPlugin = ^Plugin;

  FI_FormatProc = function: PAnsiChar; stdcall;
  FI_DescriptionProc = function: PAnsiChar; stdcall;
  FI_ExtensionListProc = function: PAnsiChar; stdcall;
  FI_RegExprProc = function: PAnsiChar; stdcall;
  FI_OpenProc = function(io: PFreeImageIO; handle: fi_handle;
    read: Boolean): Pointer; stdcall;
  FI_CloseProc = procedure(io: PFreeImageIO; handle: fi_handle;
    data: Pointer); stdcall;
  FI_PageCountProc = function(io: PFreeImageIO; handle: fi_handle;
    data: Pointer): Integer; stdcall;
  FI_PageCapabilityProc = function(io: PFreeImageIO; handle: fi_handle;
    data: Pointer): Integer; stdcall;
  FI_LoadProc = function(io: PFreeImageIO; handle: fi_handle; page, flags: Integer;
    data: Pointer): PFIBITMAP; stdcall;
  FI_SaveProc = function(io: PFreeImageIO; dib: PFIBITMAP; handle: fi_handle;
    page, flags: Integer; data: Pointer): Boolean; stdcall;
  FI_ValidateProc = function(io: PFreeImageIO; handle: fi_handle): Boolean; stdcall;
  FI_MimeProc = function: PAnsiChar; stdcall;
  FI_SupportsExportBPPProc = function(bpp: integer): Boolean; stdcall;
  FI_SupportsExportTypeProc = function(atype: FREE_IMAGE_TYPE): Boolean; stdcall;
  FI_SupportsICCProfilesProc = function: Boolean; stdcall;

  Plugin = record
    format_proc: FI_FormatProc;
    description_proc: FI_DescriptionProc;
    extension_proc: FI_ExtensionListProc;
    regexpr_proc: FI_RegExprProc;
    open_proc: FI_OpenProc;
    close_proc: FI_CloseProc;
    pagecount_proc: FI_PageCountProc;
    pagecapability_proc: FI_PageCapabilityProc;
    load_proc: FI_LoadProc;
    save_proc: FI_SaveProc;
    validate_proc: FI_ValidateProc;
    mime_proc: FI_MimeProc;
    supports_export_bpp_proc: FI_SupportsExportBPPProc;
    supports_export_type_proc: FI_SupportsExportTypeProc;
    supports_icc_profiles_proc: FI_SupportsICCProfilesProc;
  end;

  FI_InitProc = procedure(aplugin: PPlugin; format_id: Integer); stdcall;

// --------------------------------------------------------------------------
// Load/Save flag constants -------------------------------------------------
// --------------------------------------------------------------------------

const
  BMP_DEFAULT         = 0;
  BMP_SAVE_RLE        = 1;
  CUT_DEFAULT         = 0;
  DDS_DEFAULT         = 0;
  EXR_DEFAULT			    = 0;		  // save data as half with piz-based wavelet compression
  EXR_FLOAT			      = $0001;  // save data as float instead of as half (not recommended)
  EXR_NONE			      = $0002;  // save with no compression
  EXR_ZIP				      = $0004;  // save with zlib compression, in blocks of 16 scan lines
  EXR_PIZ				      = $0008;  // save with piz-based wavelet compression
  EXR_PXR24			      = $0010;  // save with lossy 24-bit float compression
  EXR_B44				      = $0020;  // save with lossy 44% float compression - goes to 22% when combined with EXR_LC
  EXR_LC				      = $0040;  // save images with one luminance and two chroma channels, rather than as RGB (lossy compression)
  FAXG3_DEFAULT       = 0;
  GIF_DEFAULT         = 0;
  GIF_LOAD256         = 1;     // Load the image as a 256 color image with ununsed palette entries, if it's 16 or 2 color
  GIF_PLAYBACK        = 2;     // 'Play' the GIF to generate each frame (as 32bpp) instead of returning raw frame data when loading
  HDR_DEFAULT         = 0;
  ICO_DEFAULT         = 0;
  ICO_MAKEALPHA       = 1;     // convert to 32bpp and create an alpha channel from the AND-mask when loading
  IFF_DEFAULT         = 0;
  J2K_DEFAULT         = 0;     // save with a 16:1 rate
  JP2_DEFAULT         = 0;     // save with a 16:1 rate
  JPEG_DEFAULT        = 0;
  JPEG_FAST           = 1;
  JPEG_ACCURATE       = 2;
  JPEG_CMYK           = $0004; // load separated CMYK "as is" (use | to combine with other flags)
  JPEG_EXIFROTATE     = $0008; // load and rotate according to Exif 'Orientation' tag if available
  JPEG_QUALITYSUPERB  = $0080; // save with superb quality (100:1)
  JPEG_QUALITYGOOD    = $0100; // save with good quality (75:1)
  JPEG_QUALITYNORMAL  = $0200; // save with normal quality (50:1)
  JPEG_QUALITYAVERAGE = $0400; // save with average quality (25:1)
  JPEG_QUALITYBAD     = $0800; // save with bad quality (10:1)
  JPEG_PROGRESSIVE    = $2000; // save as a progressive-JPEG (use | to combine with other save flags)
  JPEG_SUBSAMPLING_411 = $1000;  // save with high 4x1 chroma subsampling (4:1:1)
  JPEG_SUBSAMPLING_420 = $4000;  // save with medium 2x2 medium chroma subsampling (4:2:0) - default value
  JPEG_SUBSAMPLING_422 = $8000;  // save with low 2x1 chroma subsampling (4:2:2)
  JPEG_SUBSAMPLING_444 = $10000; // save with no chroma subsampling (4:4:4)
  KOALA_DEFAULT       = 0;
  LBM_DEFAULT         = 0;
  MNG_DEFAULT         = 0;
  PCD_DEFAULT         = 0;
  PCD_BASE            = 1;     // load the bitmap sized 768 x 512
  PCD_BASEDIV4        = 2;     // load the bitmap sized 384 x 256
  PCD_BASEDIV16       = 3;     // load the bitmap sized 192 x 128
  PCX_DEFAULT         = 0;
  PFM_DEFAULT         = 0;
  PICT_DEFAULT        = 0;
  PNG_DEFAULT         = 0;
  PNG_IGNOREGAMMA     = 1;     // avoid gamma correction
  PNG_Z_BEST_SPEED          = $0001; // save using ZLib level 1 compression flag (default value is 6)
  PNG_Z_DEFAULT_COMPRESSION = $0006; // save using ZLib level 6 compression flag (default recommended value)
  PNG_Z_BEST_COMPRESSION    = $0009; // save using ZLib level 9 compression flag (default value is 6)
  PNG_Z_NO_COMPRESSION      = $0100; // save without ZLib compression
  PNG_INTERLACED            = $0200; // save using Adam7 interlacing (use | to combine with other save flags)
  PNM_DEFAULT         = 0;
  PNM_SAVE_RAW        = 0;     // If set the writer saves in RAW format (i.e. P4, P5 or P6)
  PNM_SAVE_ASCII      = 1;     // If set the writer saves in ASCII format (i.e. P1, P2 or P3)
  PSD_DEFAULT         = 0;
  RAS_DEFAULT         = 0;
  RAW_DEFAULT         = 0; // load the file as linear RGB 48-bit
  RAW_PREVIEW         = 1; // try to load the embedded JPEG preview with included Exif Data or default to RGB 24-bit
  RAW_DISPLAY         = 2; // load the file as RGB 24-bit
  SGI_DEFAULT         = 0;
  TARGA_DEFAULT       = 0;
  TARGA_LOAD_RGB888   = 1;     // If set the loader converts RGB555 and ARGB8888 -> RGB888.
  TIFF_DEFAULT        = 0;
  TIFF_CMYK	          = $0001;  // reads/stores tags for separated CMYK (use | to combine with compression flags)
  TIFF_PACKBITS       = $0100;  // save using PACKBITS compression
  TIFF_DEFLATE        = $0200;  // save using DEFLATE compression
  TIFF_ADOBE_DEFLATE  = $0400;  // save using ADOBE DEFLATE compression
  TIFF_NONE           = $0800;  // save without any compression
  TIFF_CCITTFAX3		  = $1000;  // save using CCITT Group 3 fax encoding
  TIFF_CCITTFAX4		  = $2000;  // save using CCITT Group 4 fax encoding
  TIFF_LZW			      = $4000; 	// save using LZW compression
  TIFF_JPEG			      = $8000;	// save using JPEG compression
  WBMP_DEFAULT        = 0;
  XBM_DEFAULT         = 0;
  XPM_DEFAULT         = 0;

// --------------------------------------------------------------------------
// Background filling options -----------------------------------------------
// Constants used in FreeImage_FillBackground and FreeImage_EnlargeCanvas
// --------------------------------------------------------------------------

const
  FI_COLOR_IS_RGB_COLOR         = $00; // RGBQUAD color is a RGB color (contains no valid alpha channel)
  FI_COLOR_IS_RGBA_COLOR        = $01; // RGBQUAD color is a RGBA color (contains a valid alpha channel)
  FI_COLOR_FIND_EQUAL_COLOR     = $02; // For palettized images: lookup equal RGB color from palette
  FI_COLOR_ALPHA_IS_INDEX       = $04; // The color's rgbReserved member (alpha) contains the palette index to be used
  FI_COLOR_PALETTE_SEARCH_MASK  = FI_COLOR_FIND_EQUAL_COLOR or FI_COLOR_ALPHA_IS_INDEX; // No color lookup is performed

// --------------------------------------------------------------------------
// Init/Error routines ------------------------------------------------------
// --------------------------------------------------------------------------

procedure FreeImage_Initialise(load_local_plugins_only: Boolean = False); stdcall;
procedure FreeImage_DeInitialise; stdcall;

// --------------------------------------------------------------------------
// Version routines ---------------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetVersion: PAnsiChar; stdcall;
function FreeImage_GetCopyrightMessage: PAnsiChar; stdcall;

// --------------------------------------------------------------------------
// Message output functions -------------------------------------------------
// --------------------------------------------------------------------------

type
  FreeImage_OutputMessageFunction = procedure(fif: FREE_IMAGE_FORMAT;
    msg: PAnsiChar); cdecl;
  FreeImage_OutputMessageFunctionStdCall = procedure(fif: FREE_IMAGE_FORMAT;
    msg: PAnsiChar); stdcall;

procedure FreeImage_SetOutputMessageStdCall(omf: FreeImage_OutputMessageFunctionStdCall); stdcall;
procedure FreeImage_SetOutputMessage(omf: FreeImage_OutputMessageFunction); stdcall;
{$IFDEF DELPHI6}
//this is declared stdcall in the C header but it is actually cdecl.
//with varargs functions, clearing the stack is caller's responsibility
//(since the callee doesn't know how many parameters were passed).
//cdecl is the right convention here, not stdcall
procedure FreeImage_OutputMessageProc(fif: Integer; fmt: PAnsiChar); cdecl; varargs;
{$ELSE}
//older Delphi versions (<6) do not support varargs.
//we provide a wrapper that uses open arrays instead
procedure FreeImage_OutputMessageProc(fif: Integer; fmt: PAnsiChar; args: array of const);
{$ENDIF}

// --------------------------------------------------------------------------
// Allocate/Unload routines -------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_Allocate(width, height, bpp: Integer; red_mask: Cardinal = 0;
  green_mask: Cardinal = 0; blue_mask: Cardinal = 0): PFIBITMAP; stdcall;
function FreeImage_AllocateT(atype: FREE_IMAGE_TYPE; width, height: Integer;
  bpp: Integer = 8; red_mask: Cardinal = 0; green_mask: Cardinal = 0;
  blue_mask: Cardinal = 0): PFIBITMAP; stdcall;
function FreeImage_Clone(dib: PFIBITMAP): PFIBITMAP; stdcall;
procedure FreeImage_Unload(dib: PFIBITMAP); stdcall;

// --------------------------------------------------------------------------
// Load / Save routines -----------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_Load(fif: FREE_IMAGE_FORMAT; filename: PAnsiChar;
  flags: Integer = 0): PFIBITMAP; stdcall;
function FreeImage_LoadU(fif: FREE_IMAGE_FORMAT; filename: PWideChar;
  flags: Integer = 0): PFIBITMAP; stdcall;
function FreeImage_LoadFromHandle(fif: FREE_IMAGE_FORMAT; io: PFreeImageIO;
  handle: fi_handle; flags: Integer = 0): PFIBITMAP; stdcall;
function FreeImage_Save(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP; filename: PAnsiChar;
  flags: Integer = 0): Boolean; stdcall;
function FreeImage_SaveU(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP; filename: PWideChar;
  flags: Integer = 0): Boolean; stdcall;
function FreeImage_SaveToHandle(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP;
  io: PFreeImageIO; handle: fi_handle; flags: Integer = 0): Boolean; stdcall;

// --------------------------------------------------------------------------
// Memory I/O stream routines -----------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_OpenMemory(data: PByte = nil; size_in_bytes: DWORD = 0): PFIMEMORY; stdcall;
procedure FreeImage_CloseMemory(stream: PFIMEMORY); stdcall;
function FreeImage_LoadFromMemory(fif: FREE_IMAGE_FORMAT; stream: PFIMEMORY;
  flags: Integer = 0): PFIBITMAP; stdcall;
function FreeImage_SaveToMemory(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP;
  stream: PFIMEMORY; flags: Integer = 0): Boolean; stdcall;
function FreeImage_TellMemory(stream: PFIMEMORY): LongInt; stdcall;
function FreeImage_SeekMemory(stream: PFIMEMORY; offset: LongInt;
  origin: Integer): Boolean; stdcall;
function FreeImage_AcquireMemory(stream: PFIMEMORY; var data: PByte;
  var size_in_bytes: DWORD): Boolean; stdcall;
function FreeImage_ReadMemory(buffer: Pointer; size, count: Cardinal;
  stream: PFIMEMORY): Cardinal; stdcall;
function FreeImage_WriteMemory(buffer: Pointer; size, count: Cardinal;
  stream: PFIMEMORY): Cardinal; stdcall;
function FreeImage_LoadMultiBitmapFromMemory(fif: FREE_IMAGE_FORMAT; stream: PFIMEMORY;
  flags: Integer = 0): PFIMULTIBITMAP; stdcall;

// --------------------------------------------------------------------------
// Plugin Interface ---------------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_RegisterLocalPlugin(proc_address: FI_InitProc; format: PAnsiChar = nil;
  description: PAnsiChar = nil; extension: PAnsiChar = nil;
  regexpr: PAnsiChar = nil): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_RegisterExternalPlugin(path: PAnsiChar; format: PAnsiChar = nil;
  description: PAnsiChar = nil; extension: PAnsiChar = nil;
  regexpr: PAnsiChar = nil): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFIFCount: Integer; stdcall;
procedure FreeImage_SetPluginEnabled(fif: FREE_IMAGE_FORMAT; enable: Boolean); stdcall;
function FreeImage_IsPluginEnabled(fif: FREE_IMAGE_FORMAT): Integer; stdcall;
function FreeImage_GetFIFFromFormat(format: PAnsiChar): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFIFFromMime(mime: PAnsiChar): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFormatFromFIF(fif: FREE_IMAGE_FORMAT): PAnsiChar; stdcall;
function FreeImage_GetFIFExtensionList(fif: FREE_IMAGE_FORMAT): PAnsiChar; stdcall;
function FreeImage_GetFIFDescription(fif: FREE_IMAGE_FORMAT): PAnsiChar; stdcall;
function FreeImage_GetFIFRegExpr(fif: FREE_IMAGE_FORMAT): PAnsiChar; stdcall;
function FreeImage_GetFIFMimeType(fif: FREE_IMAGE_FORMAT): PAnsiChar; stdcall;
function FreeImage_GetFIFFromFilename(filename: PAnsiChar): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFIFFromFilenameU(filename: PWideChar): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_FIFSupportsReading(fif: FREE_IMAGE_FORMAT): Boolean; stdcall;
function FreeImage_FIFSupportsWriting(fif: FREE_IMAGE_FORMAT): Boolean; stdcall;
function FreeImage_FIFSupportsExportBPP(fif: FREE_IMAGE_FORMAT;
  bpp: Integer): Boolean; stdcall;
function FreeImage_FIFSupportsExportType(fif: FREE_IMAGE_FORMAT;
  atype: FREE_IMAGE_TYPE): Boolean; stdcall;
function FreeImage_FIFSupportsICCProfiles(fif: FREE_IMAGE_FORMAT): Boolean; stdcall;

// --------------------------------------------------------------------------
// Multipaging interface ----------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_OpenMultiBitmap(fif: FREE_IMAGE_FORMAT; filename: PAnsiChar;
  create_new, read_only: Boolean; keep_cache_in_memory: Boolean = False;
  flags: Integer = 0): PFIMULTIBITMAP; stdcall;
function FreeImage_OpenMultiBitmapFromHandle(fif: FREE_IMAGE_FORMAT; io: PFreeImageIO;
  handle: fi_handle; flags: Integer = 0): PFIMULTIBITMAP; stdcall;
function FreeImage_CloseMultiBitmap(bitmap: PFIMULTIBITMAP;
  flags: Integer = 0): Boolean; stdcall;
function FreeImage_GetPageCount(bitmap: PFIMULTIBITMAP): Integer; stdcall;
procedure FreeImage_AppendPage(bitmap: PFIMULTIBITMAP; data: PFIBITMAP); stdcall;
procedure FreeImage_InsertPage(bitmap: PFIMULTIBITMAP; page: Integer;
  data: PFIBITMAP); stdcall;
procedure FreeImage_DeletePage(bitmap: PFIMULTIBITMAP; page: Integer); stdcall;
function FreeImage_LockPage(bitmap: PFIMULTIBITMAP; page: Integer): PFIBITMAP; stdcall;
procedure FreeImage_UnlockPage(bitmap: PFIMULTIBITMAP; data: PFIBITMAP;
  changed: Boolean); stdcall;
function FreeImage_MovePage(bitmap: PFIMULTIBITMAP; target, source: Integer): Boolean; stdcall;
function FreeImage_GetLockedPageNumbers(bitmap: PFIMULTIBITMAP; var pages: Integer;
  var count: Integer): Boolean; stdcall;

// --------------------------------------------------------------------------
// Filetype request routines ------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetFileType(filename: PAnsiChar;
  size: Integer = 0): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFileTypeU(filename: PWideChar;
  size: Integer = 0): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFileTypeFromHandle(io: PFreeImageIO; handle: FI_Handle;
  size: Integer = 0): FREE_IMAGE_FORMAT; stdcall;
function FreeImage_GetFileTypeFromMemory(stream: PFIMEMORY;
  size: Integer = 0): FREE_IMAGE_FORMAT; stdcall;

// --------------------------------------------------------------------------
// ImageType request routine ------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetImageType(dib: PFIBITMAP): FREE_IMAGE_TYPE; stdcall;

// --------------------------------------------------------------------------
// FreeImage helper routines ------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_IsLittleEndian: Boolean; stdcall;
function FreeImage_LookupX11Color(szColor: PAnsiChar; var nRed, nGreen, nBlue: Byte): Boolean; stdcall;
function FreeImage_LookupSVGColor(szColor: PAnsiChar; var nRed, nGreen, nBlue: Byte): Boolean; stdcall;

// --------------------------------------------------------------------------
// Pixels access routines ---------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetBits(dib: PFIBITMAP): PByte; stdcall;
function FreeImage_GetScanLine(dib: PFIBITMAP; scanline: Integer): PByte; stdcall;

function FreeImage_GetPixelIndex(dib: PFIBITMAP; x, y: Cardinal; var value: Byte): Boolean; stdcall;
function FreeImage_GetPixelColor(dib: PFIBITMAP; x, y: Cardinal; var value: RGBQUAD): Boolean; stdcall;
function FreeImage_SetPixelIndex(dib: PFIBITMAP; x, y: Cardinal; var value: Byte): Boolean; stdcall;
function FreeImage_SetPixelColor(dib: PFIBITMAP; x, y: Cardinal; var value: RGBQUAD): Boolean; stdcall;

// --------------------------------------------------------------------------
// DIB info routines --------------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetColorsUsed(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetBPP(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetWidth(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetHeight(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetLine(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetPitch(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetDIBSize(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetPalette(dib: PFIBITMAP): PRGBQuad; stdcall;

function FreeImage_GetDotsPerMeterX(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetDotsPerMeterY(dib: PFIBITMAP): Cardinal; stdcall;
procedure FreeImage_SetDotsPerMeterX(dib: PFIBITMAP; res: Cardinal); stdcall;
procedure FreeImage_SetDotsPerMeterY(dib: PFIBITMAP; res: Cardinal); stdcall;

function FreeImage_GetInfoHeader(dib: PFIBITMAP): PBITMAPINFOHEADER; stdcall;
function FreeImage_GetInfo(dib: PFIBITMAP): PBITMAPINFO; stdcall;
function FreeImage_GetColorType(dib: PFIBITMAP): FREE_IMAGE_COLOR_TYPE; stdcall;

function FreeImage_GetRedMask(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetGreenMask(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetBlueMask(dib: PFIBITMAP): Cardinal; stdcall;

function FreeImage_GetTransparencyCount(dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_GetTransparencyTable(dib: PFIBITMAP): PByte; stdcall;
procedure FreeImage_SetTransparent(dib: PFIBITMAP; enabled: Boolean); stdcall;
procedure FreeImage_SetTransparencyTable(dib: PFIBITMAP; table: PByte;
  count: Integer); stdcall;
function FreeImage_IsTransparent(dib: PFIBITMAP): Boolean; stdcall;
procedure FreeImage_SetTransparentIndex(dib: PFIBITMAP; index: Integer); stdcall;
function FreeImage_GetTransparentIndex(dib: PFIBITMAP): Integer; stdcall;

function FreeImage_HasBackgroundColor(dib: PFIBITMAP): Boolean; stdcall;
function FreeImage_GetBackgroundColor(dib: PFIBITMAP; var bkcolor: RGBQUAD): Boolean; stdcall;
function FreeImage_SetBackgroundColor(dib: PFIBITMAP; bkcolor: PRGBQUAD): Boolean; stdcall;

// --------------------------------------------------------------------------
// ICC profile routines -----------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_GetICCProfile(dib: PFIBITMAP): PFIICCPROFILE; stdcall;
function FreeImage_CreateICCProfile(dib: PFIBITMAP; data: Pointer;
  size: LongInt): PFIICCPROFILE; stdcall;
procedure FreeImage_DestroyICCProfile(dib: PFIBITMAP); stdcall;

// --------------------------------------------------------------------------
// Line conversion routines -------------------------------------------------
// --------------------------------------------------------------------------

procedure FreeImage_ConvertLine1To4(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine8To4(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad);  stdcall;
procedure FreeImage_ConvertLine16To4_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine16To4_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine24To4(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine32To4(target, source: PByte; width_in_pixels: Integer); stdcall;

procedure FreeImage_ConvertLine1To8(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine4To8(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine16To8_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine16To8_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine24To8(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine32To8(target, source: PByte; width_in_pixels: Integer); stdcall;

procedure FreeImage_ConvertLine1To16_555(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine4To16_555(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine8To16_555(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine16_565_To16_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine24To16_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine32To16_555(target, source: PByte; width_in_pixels: Integer); stdcall;

procedure FreeImage_ConvertLine1To16_565(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine4To16_565(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine8To16_565(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine16_555_To16_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine24To16_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine32To16_565(target, source: PByte; width_in_pixels: Integer); stdcall;

procedure FreeImage_ConvertLine1To24(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine4To24(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine8To24(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine16To24_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine16To24_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine32To24(target, source: PByte; width_in_pixels: Integer); stdcall;

procedure FreeImage_ConvertLine1To32(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine4To32(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine8To32(target, source: PByte; width_in_pixels: Integer;
  palette: PRGBQuad); stdcall;
procedure FreeImage_ConvertLine16To32_555(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine16To32_565(target, source: PByte; width_in_pixels: Integer); stdcall;
procedure FreeImage_ConvertLine24To32(target, source: PByte; width_in_pixels: Integer); stdcall;

// --------------------------------------------------------------------------
// Smart conversion routines ------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_ConvertTo4Bits(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertTo8Bits(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertToGreyscale(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertTo16Bits555(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertTo16Bits565(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertTo24Bits(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ConvertTo32Bits(dib: PFIBITMAP): PFIBITMAP; stdcall;
function FreeImage_ColorQuantize(dib: PFIBITMAP; quantize: FREE_IMAGE_QUANTIZE): PFIBITMAP; stdcall;
function FreeImage_ColorQuantizeEx(dib: PFIBITMAP; quantize: FREE_IMAGE_QUANTIZE = FIQ_WUQUANT;
  PaletteSize: Integer = 256; ReserveSize: Integer = 0;
  ReservePalette: PRGBQuad = nil): PFIBITMAP; stdcall;
function FreeImage_Threshold(dib: PFIBITMAP; T: Byte): PFIBITMAP; stdcall;
function FreeImage_Dither(dib: PFIBITMAP; algorithm: FREE_IMAGE_DITHER): PFIBITMAP; stdcall;

function FreeImage_ConvertFromRawBits(bits: PByte; width, height, pitch: Integer;
  bpp, red_mask, green_mask, blue_mask: Cardinal; topdown: Boolean = False): PFIBITMAP; stdcall;
procedure FreeImage_ConvertToRawBits(bits: PByte; dib: PFIBITMAP; pitch: Integer;
  bpp, red_mask, green_mask, blue_mask: Cardinal; topdown: Boolean = False); stdcall;

function FreeImage_ConvertToRGBF(dib: PFIBITMAP): PFIBITMAP; stdcall;

function FreeImage_ConvertToStandardType(src: PFIBITMAP;
  scale_linear: Boolean = True): PFIBITMAP; stdcall;
function FreeImage_ConvertToType(src: PFIBITMAP; dst_type: FREE_IMAGE_TYPE;
  scale_linear: Boolean = True): PFIBITMAP; stdcall;

// tone mapping operators
function FreeImage_ToneMapping(dib: PFIBITMAP; tmo: FREE_IMAGE_TMO;
  first_param: Double = 0; second_param: Double = 0): PFIBITMAP; stdcall;
function FreeImage_TmoDrago03(src: PFIBITMAP; gamma: Double = 2.2;
  exposure: Double = 0): PFIBITMAP; stdcall;
function FreeImage_TmoReinhard05(src: PFIBITMAP; intensity: Double = 0;
  contrast: Double = 0): PFIBITMAP; stdcall;
function FreeImage_TmoReinhard05Ex(src: PFIBITMAP; intensity: Double = 0;
  contrast: Double = 0; adaptation: Double = 1; color_correction: Double = 0): PFIBITMAP; stdcall;

function FreeImage_TmoFattal02(src: PFIBITMAP; color_saturation: Double = 0.5;
  attenuation: Double = 0.85): PFIBITMAP; stdcall;

// --------------------------------------------------------------------------
// ZLib interface -----------------------------------------------------------
// --------------------------------------------------------------------------

function FreeImage_ZLibCompress(target: PByte; target_size: DWORD; source: PByte; source_size: DWORD): DWORD; stdcall;
function FreeImage_ZLibUncompress(target: PByte; target_size: DWORD; source: PByte; source_size: DWORD): DWORD; stdcall;
function FreeImage_ZLibGZip(target: PByte; target_size: DWORD; source: PByte; source_size: DWORD): DWORD; stdcall;
function FreeImage_ZLibGUnzip(target: PByte; target_size: DWORD; source: PByte; source_size: DWORD): DWORD; stdcall;
function FreeImage_ZLibCRC32(crc: DWORD; source: PByte; source_size: DWORD): DWORD; stdcall;

// --------------------------------------------------------------------------
// Metadata routines --------------------------------------------------------
// --------------------------------------------------------------------------

// tag creation / destruction
function FreeImage_CreateTag: PFITAG; stdcall;
procedure FreeImage_DeleteTag(tag: PFITAG); stdcall;
function FreeImage_CloneTag(tag: PFITAG): PFITAG; stdcall;

// tag getters and setters
function FreeImage_GetTagKey(tag: PFITAG): PAnsiChar; stdcall;
function FreeImage_GetTagDescription(tag: PFITAG): PAnsiChar; stdcall;
function FreeImage_GetTagID(tag: PFITAG): Word; stdcall;
function FreeImage_GetTagType(tag: PFITAG): FREE_IMAGE_MDTYPE; stdcall;
function FreeImage_GetTagCount(tag: PFITAG): DWORD; stdcall;
function FreeImage_GetTagLength(tag: PFITAG): DWORD; stdcall;
function FreeImage_GetTagValue(tag: PFITAG): Pointer; stdcall;

function FreeImage_SetTagKey(tag: PFITAG; key: PAnsiChar): Boolean; stdcall;
function FreeImage_SetTagDescription(tag: PFITAG; description: PAnsiChar): Boolean; stdcall;
function FreeImage_SetTagID(tag: PFITAG; id: Word): Boolean; stdcall;
function FreeImage_SetTagType(tag: PFITAG; atype: FREE_IMAGE_MDTYPE): Boolean; stdcall;
function FreeImage_SetTagCount(tag: PFITAG; count: DWORD): Boolean; stdcall;
function FreeImage_SetTagLength(tag: PFITAG; length: DWORD): Boolean; stdcall;
function FreeImage_SetTagValue(tag: PFITAG; value: Pointer): Boolean; stdcall;

// iterator
function FreeImage_FindFirstMetadata(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  var tag: PFITAG): PFIMETADATA; stdcall;
function FreeImage_FindNextMetadata(mdhandle: PFIMETADATA; var tag: PFITAG): Boolean; stdcall;
procedure FreeImage_FindCloseMetadata(mdhandle: PFIMETADATA); stdcall;

// metadata setter and getter
function FreeImage_SetMetadata(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  key: PAnsiChar; tag: PFITAG): Boolean; stdcall;
function FreeImage_GetMetaData(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  key: PAnsiChar; var tag: PFITAG): Boolean; stdcall;

// helpers
function FreeImage_GetMetadataCount(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP): Cardinal; stdcall;
function FreeImage_CloneMetadata(dst, src: PFIBITMAP): Boolean; stdcall;

// tag to C string conversion
function FreeImage_TagToString(model: FREE_IMAGE_MDMODEL; tag: PFITAG;
  Make: PAnsiChar = nil): PAnsiChar; stdcall;

// --------------------------------------------------------------------------
// Image manipulation toolkit -----------------------------------------------
// --------------------------------------------------------------------------

// rotation and flipping
function FreeImage_RotateClassic(dib: PFIBITMAP; angle: Double): PFIBITMAP; stdcall;
function FreeImage_Rotate(dib: PFIBITMAP; angle: Double; bkcolor: Pointer = nil): PFIBITMAP; stdcall;
function FreeImage_RotateEx(dib: PFIBITMAP; angle, x_shift, y_shift, x_origin, y_origin: Double;
  use_mask: Boolean): PFIBITMAP; stdcall;
function FreeImage_FlipHorizontal(dib: PFIBITMAP): Boolean; stdcall;
function FreeImage_FlipVertical(dib: PFIBITMAP): Boolean; stdcall;
function FreeImage_JPEGTransform(src_file, dst_file: PAnsiChar; operation: FREE_IMAGE_JPEG_OPERATION;
  perfect: Boolean = False): Boolean; stdcall;
function FreeImage_JPEGTransformU(src_file, dst_file: PWideChar; operation: FREE_IMAGE_JPEG_OPERATION;
  perfect: Boolean = False): Boolean; stdcall;

// upsampling / downsampling
function FreeImage_Rescale(dib: PFIBITMAP; dst_width, dst_height: Integer; filter: FREE_IMAGE_FILTER): PFIBITMAP; stdcall;
function FreeImage_MakeThumbnail(dib: PFIBITMAP; max_pixel_size: Integer; convert: Boolean = True): PFIBITMAP; stdcall;

// color manipulation routines (point operations)
function FreeImage_AdjustCurve(dib: PFIBITMAP; LUT: PByte;
  channel: FREE_IMAGE_COLOR_CHANNEL): Boolean; stdcall;
function FreeImage_AdjustGamma(dib: PFIBITMAP; gamma: Double): Boolean; stdcall;
function FreeImage_AdjustBrightness(dib: PFIBITMAP; percentage: Double): Boolean; stdcall;
function FreeImage_AdjustContrast(dib: PFIBITMAP; percentage: Double): Boolean; stdcall;
function FreeImage_Invert(dib: PFIBITMAP): Boolean; stdcall;
function FreeImage_GetHistogram(dib: PFIBITMAP; histo: PDWORD;
  channel: FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK): Boolean; stdcall;
function FreeImage_GetAdjustColorsLookupTable(LUT: PByte; brightness, contrast, gamma: Double;
  invert: Boolean): Integer; stdcall;
function FreeImage_AdjustColors(dib: PFIBITMAP; brightness, contrast, gamma: Double;
  invert: Boolean = False): Boolean; stdcall;
function FreeImage_ApplyColorMapping(dib: PFIBITMAP; srccolors, dstcolors: PRGBQuad;
  count: Cardinal; ignore_alpha, swap: Boolean): Cardinal; stdcall;
function FreeImage_SwapColors(dib: PFIBITMAP; color_a, color_b: PRGBQuad;
  ignore_alpha: Boolean): Cardinal; stdcall;
function FreeImage_ApplyPaletteIndexMapping(dib: PFIBITMAP; srcindices,	dstindices: PByte;
  count: Cardinal; swap: Boolean): Cardinal; stdcall;
function FreeImage_SwapPaletteIndices(dib: PFIBITMAP; index_a, index_b: PByte): Cardinal; stdcall;

// channel processing routines
function FreeImage_GetChannel(dib: PFIBITMAP; channel: FREE_IMAGE_COLOR_CHANNEL): PFIBITMAP; stdcall;
function FreeImage_SetChannel(dib, dib8: PFIBITMAP; channel: FREE_IMAGE_COLOR_CHANNEL): Boolean; stdcall;
function FreeImage_GetComplexChannel(src: PFIBITMAP; channel: FREE_IMAGE_COLOR_CHANNEL): PFIBITMAP; stdcall;
function FreeImage_SetComplexChannel(dst, src: PFIBITMAP; channel: FREE_IMAGE_COLOR_CHANNEL): Boolean; stdcall;

// copy / paste / composite routines

function FreeImage_Copy(dib: PFIBITMAP; left, top, right, bottom: Integer): PFIBITMAP; stdcall;
function FreeImage_Paste(dst, src: PFIBITMAP; left, top, alpha: Integer): Boolean; stdcall;
function FreeImage_Composite(fg: PFIBITMAP; useFileBkg: Boolean = False;
  appBkColor: PRGBQuad = nil; bg: PFIBITMAP = nil): PFIBITMAP; stdcall;
function FreeImage_JPEGCrop(src_file, dst_file: PAnsiChar;
  left, top, right, bottom: Integer): Boolean; stdcall;
function FreeImage_JPEGCropU(src_file, dst_file: PWideChar;
  left, top, right, bottom: Integer): Boolean; stdcall;
function FreeImage_PreMultiplyWithAlpha(dib: PFIBITMAP): Boolean; stdcall;

// background filling routines
function FreeImage_FillBackground(dib: PFIBITMAP; color: Pointer;
  options: Integer = 0): Boolean; stdcall;
function FreeImage_EnlargeCanvas(src: PFIBITMAP; left, top, right, bottom: Integer;
  color: Pointer; options: Integer = 0): PFIBITMAP; stdcall;
function FreeImage_AllocateEx(width, height, bpp: Integer; color: PRGBQuad;
  options: Integer = 0; palette: PRGBQuad = nil; red_mask: Cardinal = 0;
  green_mask: Cardinal = 0; blue_mask: Cardinal = 0): PFIBITMAP; stdcall;
function FreeImage_AllocateExT(atype: FREE_IMAGE_TYPE; width, height, bpp: Integer;
  color: Pointer; options: Integer = 0; palette: PRGBQuad = nil; red_mask: Cardinal = 0;
  green_mask: Cardinal = 0; blue_mask: Cardinal = 0): PFIBITMAP; stdcall;

// miscellaneous algorithms
function FreeImage_MultigridPoissonSolver(Laplacian: PFIBITMAP;
  ncycle: Integer = 3): PFIBITMAP; stdcall;


implementation

uses SysUtils;

const
  FIDLL = 'FreeImage.dll';

procedure FreeImage_Initialise; external FIDLL name '_FreeImage_Initialise@4';
procedure FreeImage_DeInitialise; external FIDLL name '_FreeImage_DeInitialise@0';
function FreeImage_GetVersion; external FIDLL name '_FreeImage_GetVersion@0';
function FreeImage_GetCopyrightMessage; external FIDLL name '_FreeImage_GetCopyrightMessage@0';
{$IFDEF DELPHI6}
procedure FreeImage_OutPutMessageProc; external FIDLL name 'FreeImage_OutputMessageProc';
{$ELSE}
//we provide a wrapper since we haven't varargs in older versions of Delphi
procedure __FreeImage_OutPutMessageProc; external FIDLL name 'FreeImage_OutputMessageProc';
procedure FreeImage_OutPutMessageProc(fif: Integer; fmt: PAnsiChar; args: array of const);
  function ArrayToBuffer(Args: array of const;
    var Argv: Pointer; Buffer: Pointer; Size: Cardinal): Integer;
  var
    i: Integer;
    temp: AnsiString;
    parg: Pointer;
    psrc, pbuf: PAnsiChar;
    len: Cardinal;
  begin
    Result := High(Args) + 1;
    if Result = 0 then
      Exit;
    //array of pointers to push on stack
    GetMem(Argv, Result * SizeOf(Pointer));
    //pointer to current string in buffer
    pbuf := Buffer;
    //pointer to current arg
    parg := Argv;
    //for each const...
    for i := 0 to Result - 1 do begin
      psrc := nil;
      case Args[i].VType of
        vtInteger: begin
          //integer
          len := 0;
          Integer(parg^) := Args[i].VInteger;
        end;
        vtString: begin
          //short string
          psrc := PAnsiChar(Cardinal(Args[i].VString) + SizeOf(Byte));
          len := PByte(Args[i].VString)^;
          PAnsiChar(parg^) := pbuf;
        end;
        vtPChar: begin
          //NULL terminated MBCS string
          len := 0;
          PAnsiChar(parg^) := Args[i].VPChar;
        end;
        vtPWideChar: begin
          //NULL terminated Unicode string
          temp := AnsiString(Args[i].VPWideChar);
          psrc := PAnsiChar(temp);
          len := Length(temp);
          PAnsiChar(parg^) := pbuf;
        end;
        vtAnsiString: begin
          //ANSI string
          psrc := PAnsiChar(Args[i].VAnsiString);
          len := StrLen(psrc);
          PAnsiChar(parg^) := pbuf;
        end;
        vtWideString: begin
          //Wide string (OLE)
          temp := AnsiString(PWideChar(Args[i].VWideString));
          psrc := PAnsiChar(temp);
          len := Length(temp);
          PAnsiChar(parg^) := pbuf;
        end;
        else raise Exception.Create('Unsupported argument type');
      end;
      if len <> 0 then begin
        //enough space to hold string?
        if Size < (len + 1) then
          raise Exception.Create('Buffer overflow');
        //copy string
        Move(psrc^, pbuf^, len);
        //NULL terminator
        PAnsiChar(Cardinal(pbuf) + len)^ := #0;
        //shift pointer...
        Inc(pbuf, len + 1);
        //...and decrease space left
        Dec(Size, len + 1);
      end;
      Cardinal(parg) := Cardinal(parg) + SizeOf(Pointer);
    end;
  end;

  procedure DoVarargsCall(fif: Integer; fmt: PAnsiChar; Argv: Pointer; Argc: Integer);
  {
	fif     -> EAX
	fmt     -> EDX
	Argv    -> ECX
	Argc    -> [EBP+$08]
  }
  asm
      PUSH    EAX                      //remember fif
      PUSH    ECX                      //make room for ESP backup

      MOV     DWORD PTR [EBP-$08], ESP //backup stack pointer

      MOV     EAX, DWORD PTR [EBP+$08] //store Argc

      TEST    EAX, EAX                 //Argc <= 0?
      JLE     @Call

    @Loop:
      PUSH    DWORD PTR [ECX+EAX*$04-$04] //push Argv in right to left order
      DEC     EAX
      JNZ     @Loop

    @Call:
      PUSH    EDX                      //push fmt
      PUSH    DWORD PTR [EBP-$04]      //push fif
      CALL    __FreeImage_OutPutMessageProc

      MOV     ESP, DWORD PTR [EBP-$08] //restore stack pointer

      POP     ECX                      //clean stack
      POP     EAX
  end;
var
  Argc: Integer;
  Argv: Pointer;
  //buffer to hold strings - FreeImage allocates 512 bytes, we needn't more...
  Buffer: array[1..512] of Byte;
begin
  Argv := nil;
  //build array of pointers from array of const
  Argc := ArrayToBuffer(args, Argv, @Buffer, SizeOf(Buffer));
  try
    //mimic cdecl call with varargs
    DoVarargsCall(fif, fmt, Argv, Argc);
  finally
    //cleanup
    FreeMem(Argv);
  end;
end;
{$ENDIF}

procedure FreeImage_SetOutputMessageStdCall; external FIDLL name '_FreeImage_SetOutputMessageStdCall@4';
procedure FreeImage_SetOutputMessage; external FIDLL name '_FreeImage_SetOutputMessage@4';
function FreeImage_Allocate; external FIDLL name '_FreeImage_Allocate@24';
function FreeImage_AllocateT; external FIDLL name '_FreeImage_AllocateT@28';
function FreeImage_Clone; external FIDLL name '_FreeImage_Clone@4';
procedure FreeImage_Unload; external FIDLL name '_FreeImage_Unload@4';
function FreeImage_Load; external FIDLL name '_FreeImage_Load@12';
function FreeImage_LoadU; external FIDLL name '_FreeImage_LoadU@12';
function FreeImage_LoadFromHandle; external FIDLL name '_FreeImage_LoadFromHandle@16';
function FreeImage_Save; external FIDLL name '_FreeImage_Save@16';
function FreeImage_SaveU; external FIDLL name '_FreeImage_SaveU@16';
function FreeImage_SaveToHandle; external FIDLL name '_FreeImage_SaveToHandle@20';
function FreeImage_OpenMemory; external FIDLL name '_FreeImage_OpenMemory@8';
procedure FreeImage_CloseMemory; external FIDLL name '_FreeImage_CloseMemory@4';
function FreeImage_LoadFromMemory; external FIDLL name '_FreeImage_LoadFromMemory@12';
function FreeImage_SaveToMemory; external FIDLL name '_FreeImage_SaveToMemory@16';
function FreeImage_TellMemory; external FIDLL name '_FreeImage_TellMemory@4';
function FreeImage_SeekMemory; external FIDLL name '_FreeImage_SeekMemory@12';
function FreeImage_AcquireMemory; external FIDLL name '_FreeImage_AcquireMemory@12';
function FreeImage_ReadMemory; external FIDLL name '_FreeImage_ReadMemory@16';
function FreeImage_WriteMemory; external FIDLL name '_FreeImage_WriteMemory@16';
function FreeImage_LoadMultiBitmapFromMemory; external FIDLL name '_FreeImage_LoadMultiBitmapFromMemory@12';
function FreeImage_RegisterLocalPlugin; external FIDLL name '_FreeImage_RegisterLocalPlugin@20';
function FreeImage_RegisterExternalPlugin; external FIDLL name '_FreeImage_RegisterExternalPlugin@20';
function FreeImage_GetFIFCount; external FIDLL name '_FreeImage_GetFIFCount@0';
procedure FreeImage_SetPluginEnabled; external FIDLL Name '_FreeImage_SetPluginEnabled@8';
function FreeImage_IsPluginEnabled; external FIDLL Name '_FreeImage_IsPluginEnabled@4';
function FreeImage_GetFIFFromFormat; external FIDLL Name '_FreeImage_GetFIFFromFormat@4';
function FreeImage_GetFIFFromMime; external FIDLL Name '_FreeImage_GetFIFFromMime@4';
function FreeImage_GetFormatFromFIF; external FIDLL Name '_FreeImage_GetFormatFromFIF@4';
function FreeImage_GetFIFExtensionList; external FIDLL Name '_FreeImage_GetFIFExtensionList@4';
function FreeImage_GetFIFDescription; external FIDLL Name '_FreeImage_GetFIFDescription@4';
function FreeImage_GetFIFRegExpr; external FIDLL Name '_FreeImage_GetFIFRegExpr@4';
function FreeImage_GetFIFMimeType; external FIDLL Name '_FreeImage_GetFIFMimeType@4';
function FreeImage_GetFIFFromFilename; external FIDLL Name '_FreeImage_GetFIFFromFilename@4';
function FreeImage_GetFIFFromFilenameU; external FIDLL Name '_FreeImage_GetFIFFromFilenameU@4';
function FreeImage_FIFSupportsReading; external FIDLL Name '_FreeImage_FIFSupportsReading@4';
function FreeImage_FIFSupportsWriting; external FIDLL Name '_FreeImage_FIFSupportsWriting@4';
function FreeImage_FIFSupportsExportBPP; external FIDLL Name '_FreeImage_FIFSupportsExportBPP@8';
function FreeImage_FIFSupportsICCProfiles; external FIDLL Name '_FreeImage_FIFSupportsICCProfiles@4';
function FreeImage_FIFSupportsExportType; external FIDLL name '_FreeImage_FIFSupportsExportType@8';
function FreeImage_OpenMultiBitmap; external FIDLL Name '_FreeImage_OpenMultiBitmap@24';
function FreeImage_OpenMultiBitmapFromHandle; external FIDLL Name '_FreeImage_OpenMultiBitmapFromHandle@16';
function FreeImage_CloseMultiBitmap; external FIDLL Name '_FreeImage_CloseMultiBitmap@8';
function FreeImage_GetPageCount; external FIDLL Name '_FreeImage_GetPageCount@4';
procedure FreeImage_AppendPage; external FIDLL Name '_FreeImage_AppendPage@8';
procedure FreeImage_InsertPage; external FIDLL Name '_FreeImage_InsertPage@12';
procedure FreeImage_DeletePage; external FIDLL Name '_FreeImage_DeletePage@8';
function FreeImage_LockPage; external FIDLL Name '_FreeImage_LockPage@8';
procedure FreeImage_UnlockPage; external FIDLL Name '_FreeImage_UnlockPage@12';
function FreeImage_MovePage; external FIDLL Name '_FreeImage_MovePage@12';
function FreeImage_GetLockedPageNumbers; external FIDLL Name '_FreeImage_GetLockedPageNumbers@12';
function FreeImage_GetFileType; external FIDLL name '_FreeImage_GetFileType@8';
function FreeImage_GetFileTypeU; external FIDLL name '_FreeImage_GetFileTypeU@8';
function FreeImage_GetFileTypeFromHandle; external FIDLL name '_FreeImage_GetFileTypeFromHandle@12';
function FreeImage_GetFileTypeFromMemory; external FIDLL name '_FreeImage_GetFileTypeFromMemory@8';
function FreeImage_GetImageType; external FIDLL name '_FreeImage_GetImageType@4';
function FreeImage_IsLittleEndian; external FIDLL name '_FreeImage_IsLittleEndian@0';
function FreeImage_LookupX11Color; external FIDLL name '_FreeImage_LookupX11Color@16';
function FreeImage_LookupSVGColor; external FIDLL name '_FreeImage_LookupSVGColor@16';
function FreeImage_GetBits; external FIDLL name '_FreeImage_GetBits@4';
function FreeImage_GetScanLine; external FIDLL name '_FreeImage_GetScanLine@8';
function FreeImage_GetPixelIndex; external FIDLL name '_FreeImage_GetPixelIndex@16';
function FreeImage_GetPixelColor; external FIDLL name '_FreeImage_GetPixelColor@16';
function FreeImage_SetPixelIndex; external FIDLL name '_FreeImage_SetPixelIndex@16';
function FreeImage_SetPixelColor; external FIDLL name '_FreeImage_SetPixelColor@16';
function FreeImage_GetColorsUsed; external FIDLL name '_FreeImage_GetColorsUsed@4';
function FreeImage_GetBPP; external FIDLL name '_FreeImage_GetBPP@4';
function FreeImage_GetWidth; external FIDLL name '_FreeImage_GetWidth@4';
function FreeImage_GetHeight; external FIDLL name '_FreeImage_GetHeight@4';
function FreeImage_GetLine; external FIDLL name '_FreeImage_GetLine@4';
function FreeImage_GetPitch; external FIDLL name '_FreeImage_GetPitch@4';
function FreeImage_GetDIBSize; external FIDLL name '_FreeImage_GetDIBSize@4';
function FreeImage_GetPalette; external FIDLL name '_FreeImage_GetPalette@4';
function FreeImage_GetDotsPerMeterX; external FIDLL name '_FreeImage_GetDotsPerMeterX@4';
function FreeImage_GetDotsPerMeterY; external FIDLL name '_FreeImage_GetDotsPerMeterY@4';
procedure FreeImage_SetDotsPerMeterX; external FIDLL name '_FreeImage_SetDotsPerMeterX@8';
procedure FreeImage_SetDotsPerMeterY; external FIDLL name '_FreeImage_SetDotsPerMeterY@8';
function FreeImage_GetInfoHeader; external FIDLL name '_FreeImage_GetInfoHeader@4';
function FreeImage_GetInfo; external FIDLL name '_FreeImage_GetInfo@4';
function FreeImage_GetColorType; external FIDLL name '_FreeImage_GetColorType@4';
function FreeImage_GetRedMask; external FIDLL name '_FreeImage_GetRedMask@4';
function FreeImage_GetGreenMask; external FIDLL name '_FreeImage_GetGreenMask@4';
function FreeImage_GetBlueMask; external FIDLL name '_FreeImage_GetBlueMask@4';
function FreeImage_GetTransparencyCount; external FIDLL name '_FreeImage_GetTransparencyCount@4';
function FreeImage_GetTransparencyTable; external FIDLL name '_FreeImage_GetTransparencyTable@4';
procedure FreeImage_SetTransparent; external FIDLL name '_FreeImage_SetTransparent@8';
procedure FreeImage_SetTransparencyTable; external FIDLL name '_FreeImage_SetTransparencyTable@12';
function FreeImage_IsTransparent; external FIDLL name '_FreeImage_IsTransparent@4';
procedure FreeImage_SetTransparentIndex; external FIDLL name '_FreeImage_SetTransparentIndex@8';
function FreeImage_GetTransparentIndex; external FIDLL name '_FreeImage_GetTransparentIndex@4';
function FreeImage_HasBackgroundColor; external FIDLL name '_FreeImage_HasBackgroundColor@4';
function FreeImage_GetBackgroundColor; external FIDLL name '_FreeImage_GetBackgroundColor@8';
function FreeImage_SetBackgroundColor; external FIDLL name '_FreeImage_SetBackgroundColor@8';
function FreeImage_GetICCProfile; external FIDLL name '_FreeImage_GetICCProfile@4';
function FreeImage_CreateICCProfile; external FIDLL name 'FreeImage_CreateICCProfile@12';
procedure FreeImage_DestroyICCProfile; external FIDLL name 'FreeImage_DestroyICCProfile@4';
procedure FreeImage_ConvertLine1To4; external FIDLL name '_FreeImage_ConvertLine1To4@12';
procedure FreeImage_ConvertLine8To4; external FIDLL name '_FreeImage_ConvertLine8To4@16';
procedure FreeImage_ConvertLine16To4_555; external FIDLL name '_FreeImage_ConvertLine16To4_555@12';
procedure FreeImage_ConvertLine16To4_565; external FIDLL name '_FreeImage_ConvertLine16To4_565@12';
procedure FreeImage_ConvertLine24To4; external FIDLL name '_FreeImage_ConvertLine24To4@12';
procedure FreeImage_ConvertLine32To4; external FIDLL name '_FreeImage_ConvertLine32To4@12';
procedure FreeImage_ConvertLine1To8; external FIDLL name '_FreeImage_ConvertLine1To8@12';
procedure FreeImage_ConvertLine4To8; external FIDLL name '_FreeImage_ConvertLine4To8@12';
procedure FreeImage_ConvertLine16To8_555; external FIDLL name '_FreeImage_ConvertLine16To8_555@12';
procedure FreeImage_ConvertLine16To8_565; external FIDLL name '_FreeImage_ConvertLine16To8_565@12';
procedure FreeImage_ConvertLine24To8; external FIDLL name '_FreeImage_ConvertLine24To8@12';
procedure FreeImage_ConvertLine32To8; external FIDLL name '_FreeImage_ConvertLine32To8@12';
procedure FreeImage_ConvertLine1To16_555; external FIDLL name '_FreeImage_ConvertLine1To16_555@16';
procedure FreeImage_ConvertLine4To16_555; external FIDLL name '_FreeImage_ConvertLine4To16_555@16';
procedure FreeImage_ConvertLine8To16_555; external FIDLL name '_FreeImage_ConvertLine8To16_555@16';
procedure FreeImage_ConvertLine16_565_To16_555; external FIDLL name '_FreeImage_ConvertLine16_565_To16_555@12';
procedure FreeImage_ConvertLine24To16_555; external FIDLL name '_FreeImage_ConvertLine24To16_555@12';
procedure FreeImage_ConvertLine32To16_555; external FIDLL name '_FreeImage_ConvertLine32To16_555@12';
procedure FreeImage_ConvertLine1To16_565; external FIDLL name '_FreeImage_ConvertLine1To16_565@16';
procedure FreeImage_ConvertLine4To16_565; external FIDLL name '_FreeImage_ConvertLine4To16_565@16';
procedure FreeImage_ConvertLine8To16_565; external FIDLL name '_FreeImage_ConvertLine8To16_565@16';
procedure FreeImage_ConvertLine16_555_To16_565; external FIDLL name '_FreeImage_ConvertLine16_555_To16_565@12';
procedure FreeImage_ConvertLine24To16_565; external FIDLL name '_FreeImage_ConvertLine24To16_565@12';
procedure FreeImage_ConvertLine32To16_565; external FIDLL name '_FreeImage_ConvertLine32To16_565@12';
procedure FreeImage_ConvertLine1To24; external FIDLL name '_FreeImage_ConvertLine1To24@16';
procedure FreeImage_ConvertLine4To24; external FIDLL name '_FreeImage_ConvertLine4To24@16';
procedure FreeImage_ConvertLine8To24; external FIDLL name '_FreeImage_ConvertLine8To24@16';
procedure FreeImage_ConvertLine16To24_555; external FIDLL name '_FreeImage_ConvertLine16To24_555@12';
procedure FreeImage_ConvertLine16To24_565; external FIDLL name '_FreeImage_ConvertLine16To24_565@12';
procedure FreeImage_ConvertLine32To24; external FIDLL name '_FreeImage_ConvertLine32To24@12';
procedure FreeImage_ConvertLine1To32; external FIDLL name '_FreeImage_ConvertLine1To32@16';
procedure FreeImage_ConvertLine4To32; external FIDLL name '_FreeImage_ConvertLine4To32@16';
procedure FreeImage_ConvertLine8To32; external FIDLL name '_FreeImage_ConvertLine8To32@16';
procedure FreeImage_ConvertLine16To32_555; external FIDLL name '_FreeImage_ConvertLine16To32_555@12';
procedure FreeImage_ConvertLine16To32_565; external FIDLL name '_FreeImage_ConvertLine16To32_565@12';
procedure FreeImage_ConvertLine24To32; external FIDLL name '_FreeImage_ConvertLine24To32@12';
function FreeImage_ConvertTo4Bits; external FIDLL name '_FreeImage_ConvertTo4Bits@4';
function FreeImage_ConvertTo8Bits; external FIDLL name '_FreeImage_ConvertTo8Bits@4';
function FreeImage_ConvertToGreyscale; external FIDLL name '_FreeImage_ConvertToGreyscale@4';
function FreeImage_ConvertTo16Bits555; external FIDLL name '_FreeImage_ConvertTo16Bits555@4';
function FreeImage_ConvertTo16Bits565; external FIDLL name '_FreeImage_ConvertTo16Bits565@4';
function FreeImage_ConvertTo24Bits; external FIDLL name '_FreeImage_ConvertTo24Bits@4';
function FreeImage_ConvertTo32Bits; external FIDLL name '_FreeImage_ConvertTo32Bits@4';
function FreeImage_ColorQuantize; external FIDLL name '_FreeImage_ColorQuantize@8';
function FreeImage_ColorQuantizeEx; external FIDLL name '_FreeImage_ColorQuantizeEx@20';
function FreeImage_Threshold; external FIDLL name '_FreeImage_Threshold@8';
function FreeImage_Dither; external FIDLL name '_FreeImage_Dither@8';
function FreeImage_ConvertFromRawBits; external FIDLL name '_FreeImage_ConvertFromRawBits@36';
procedure FreeImage_ConvertToRawBits; external FIDLL name '_FreeImage_ConvertToRawBits@32';
function FreeImage_ConvertToRGBF; external FIDLL name '_FreeImage_ConvertToRGBF@4';
function FreeImage_ConvertToStandardType; external FIDLL name '_FreeImage_ConvertToStandardType@8';
function FreeImage_ConvertToType; external FIDLL name '_FreeImage_ConvertToType@12';
function FreeImage_ToneMapping; external FIDLL name '_FreeImage_ToneMapping@24';
function FreeImage_TmoDrago03; external FIDLL name '_FreeImage_TmoDrago03@20';
function FreeImage_TmoReinhard05; external FIDLL name '_FreeImage_TmoReinhard05@20';
function FreeImage_TmoReinhard05Ex; external FIDLL name '_FreeImage_TmoReinhard05Ex@36';
function FreeImage_TmoFattal02; external FIDLL name '_FreeImage_TmoFattal02@20';
function FreeImage_ZLibCompress; external FIDLL name '_FreeImage_ZLibCompress@16';
function FreeImage_ZLibUncompress; external FIDLL name '_FreeImage_ZLibUncompress@16';
function FreeImage_ZLibGZip; external FIDLL name '_FreeImage_ZLibGZip@16';
function FreeImage_ZLibGUnzip; external FIDLL name '_FreeImage_ZLibGUnzip@16';
function FreeImage_ZLibCRC32; external FIDLL name '_FreeImage_ZLibCRC32@12';
function FreeImage_CreateTag: PFITAG; stdcall; external FIDLL name '_FreeImage_CreateTag@0';
procedure FreeImage_DeleteTag; external FIDLL name '_FreeImage_DeleteTag@4';
function FreeImage_CloneTag; external FIDLL name '_FreeImage_CloneTag@4';
function FreeImage_GetTagKey; external FIDLL name '_FreeImage_GetTagKey@4';
function FreeImage_GetTagDescription; external FIDLL name '_FreeImage_GetTagDescription@4';
function FreeImage_GetTagID; external FIDLL name '_FreeImage_GetTagID@4';
function FreeImage_GetTagType; external FIDLL name '_FreeImage_GetTagType@4';
function FreeImage_GetTagCount; external FIDLL name '_FreeImage_GetTagCount@4';
function FreeImage_GetTagLength; external FIDLL name '_FreeImage_GetTagLength@4';
function FreeImage_GetTagValue; external FIDLL name '_FreeImage_GetTagValue@4';
function FreeImage_SetTagKey; external FIDLL name '_FreeImage_SetTagKey@8';
function FreeImage_SetTagDescription; external FIDLL name '_FreeImage_SetTagDescription@8';
function FreeImage_SetTagID; external FIDLL name '_FreeImage_SetTagID@8';
function FreeImage_SetTagType; external FIDLL name '_FreeImage_SetTagType@8';
function FreeImage_SetTagCount; external FIDLL name '_FreeImage_SetTagCount@8';
function FreeImage_SetTagLength; external FIDLL name '_FreeImage_SetTagLength@8';
function FreeImage_SetTagValue; external FIDLL name '_FreeImage_SetTagValue@8';
function FreeImage_FindFirstMetadata; external FIDLL name '_FreeImage_FindFirstMetadata@12';
function FreeImage_FindNextMetadata; external FIDLL name '_FreeImage_FindNextMetadata@8';
procedure FreeImage_FindCloseMetadata; external FIDLL name '_FreeImage_FindCloseMetadata@4';
function FreeImage_SetMetadata; external FIDLL name '_FreeImage_SetMetadata@16';
function FreeImage_GetMetaData; external FIDLL name '_FreeImage_GetMetadata@16';
function FreeImage_GetMetadataCount; external FIDLL name '_FreeImage_GetMetadataCount@8';
function FreeImage_CloneMetadata; external FIDLL name '_FreeImage_CloneMetadata@8';
function FreeImage_TagToString; external FIDLL name '_FreeImage_TagToString@12';
function FreeImage_RotateClassic; external FIDLL name '_FreeImage_RotateClassic@12';
function FreeImage_Rotate; external FIDLL name '_FreeImage_Rotate@16';
function FreeImage_RotateEx; external FIDLL name '_FreeImage_RotateEx@48';
function FreeImage_FlipHorizontal; external FIDLL name '_FreeImage_FlipHorizontal@4';
function FreeImage_FlipVertical; external FIDLL name '_FreeImage_FlipVertical@4';
function FreeImage_JPEGTransform; external FIDLL name '_FreeImage_JPEGTransform@16';
function FreeImage_JPEGTransformU; external FIDLL name '_FreeImage_JPEGTransformU@16';
function FreeImage_Rescale; external FIDLL name '_FreeImage_Rescale@16';
function FreeImage_MakeThumbnail; external FIDLL name '_FreeImage_MakeThumbnail@12';
function FreeImage_AdjustCurve; external FIDLL name '_FreeImage_AdjustCurve@12';
function FreeImage_AdjustGamma; external FIDLL name '_FreeImage_AdjustGamma@12';
function FreeImage_AdjustBrightness; external FIDLL name '_FreeImage_AdjustBrightness@12';
function FreeImage_AdjustContrast; external FIDLL name '_FreeImage_AdjustContrast@12';
function FreeImage_Invert; external FIDLL name '_FreeImage_Invert@4';
function FreeImage_GetHistogram; external FIDLL name '_FreeImage_GetHistogram@12';
function FreeImage_GetAdjustColorsLookupTable; external FIDLL name '_FreeImage_GetAdjustColorsLookupTable@32';
function FreeImage_AdjustColors; external FIDLL name '_FreeImage_AdjustColors@32';
function FreeImage_ApplyColorMapping; external FIDLL name '_FreeImage_ApplyColorMapping@24';
function FreeImage_SwapColors; external FIDLL name '_FreeImage_SwapColors@16';
function FreeImage_ApplyPaletteIndexMapping; external FIDLL name '_FreeImage_ApplyPaletteIndexMapping@20';
function FreeImage_SwapPaletteIndices; external FIDLL name '_FreeImage_SwapPaletteIndices@12';
function FreeImage_GetChannel; external FIDLL name '_FreeImage_GetChannel@8';
function FreeImage_SetChannel; external FIDLL name '_FreeImage_SetChannel@12';
function FreeImage_GetComplexChannel; external FIDLL name '_FreeImage_GetComplexChannel@8';
function FreeImage_SetComplexChannel; external FIDLL name '_FreeImage_SetComplexChannel@12';
function FreeImage_Copy; external FIDLL name '_FreeImage_Copy@20';
function FreeImage_Paste; external FIDLL name '_FreeImage_Paste@20';
function FreeImage_Composite; external FIDLL name '_FreeImage_Composite@16';
function FreeImage_JPEGCrop; external FIDLL name '_FreeImage_JPEGCrop@24';
function FreeImage_JPEGCropU; external FIDLL name '_FreeImage_JPEGCropU@24';
function FreeImage_PreMultiplyWithAlpha; external FIDLL name '_FreeImage_PreMultiplyWithAlpha@4';
function FreeImage_FillBackground; external FIDLL name '_FreeImage_FillBackground@12';
function FreeImage_EnlargeCanvas; external FIDLL name '_FreeImage_EnlargeCanvas@28';
function FreeImage_AllocateEx; external FIDLL name '_FreeImage_AllocateEx@36';
function FreeImage_AllocateExT; external FIDLL name '_FreeImage_AllocateExT@40';
function FreeImage_MultigridPoissonSolver; external FIDLL name '_FreeImage_MultigridPoissonSolver@8';

end.
