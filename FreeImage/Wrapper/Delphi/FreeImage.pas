unit FreeImage;

interface

uses Windows;
// ==========================================================
// Delphi wrapper for FreeImage 3
//
// Design and implementation by
// - Simon Beavis
// - Peter Byström
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

{$MINENUMSIZE 4}      // Make sure enums are stored as an integer to be compatible with C/C++

const
  FIDLL = 'FreeImage.dll';

// Indexes for byte arrays, masks and shifts for treating pixels as words ---
// These coincide with the order of RGBQUAD and RGBTRIPLE -------------------
// Little Endian (x86 / MS Windows, Linux) : BGR(A) order
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

type
  FIBITMAP = record
    data : Pointer;
  end;
  PFIBITMAP = ^FIBITMAP;

  FIMULTIBITMAP = record
    data : Pointer;
  end;
  PFIMULTIBITMAP = ^FIMULTIBITMAP;

// ICC profile support ------------------------------------------------------
const
  FIICC_DEFAULT = $0;
  FIICC_COLOR_IS_CMYK	= $1;

type
  FIICCPROFILE = record
    flags : WORD;   // info flag
    size : DWORD;   // profile's size measured in bytes
    data : pointer; // points to a block of contiguous memory containing the profile
  end;
  PFIICCPROFILE = ^FIICCPROFILE;

// Important enums ----------------------------------------------------------
type
  // I/O image format identifiers.
  FREE_IMAGE_FORMAT = (
	FIF_UNKNOWN = -1,
	FIF_BMP		  = 0,
	FIF_ICO	  	= 1,
	FIF_JPEG   	= 2,
	FIF_JNG	  	= 3,
	FIF_KOALA 	= 4,
	FIF_LBM	  	= 5,
	FIF_IFF     = FIF_LBM,
	FIF_MNG	  	= 6,
	FIF_PBM	  	= 7,
	FIF_PBMRAW	= 8,
	FIF_PCD	  	= 9,
	FIF_PCX	  	= 10,
	FIF_PGM	  	= 11,
	FIF_PGMRAW	= 12,
	FIF_PNG	  	= 13,
	FIF_PPM	  	= 14,
	FIF_PPMRAW	= 15,
	FIF_RAS	  	= 16,
	FIF_TARGA 	= 17,
	FIF_TIFF  	= 18,
	FIF_WBMP  	= 19,
	FIF_PSD	  	= 20,
	FIF_CUT	  	= 21,
	FIF_XBM	  	= 22,
	FIF_XPM	  	= 23
  );

{
  Image type used in FreeImage.
}

  FREE_IMAGE_TYPE = (
    FIT_UNKNOWN = 0,	// unknown type
    FIT_BITMAP  = 1,	// standard image			: 1-, 4-, 8-, 16-, 24-, 32-bit
    FIT_UINT16	= 2,	// array of unsigned short	: unsigned 16-bit
    FIT_INT16	  = 3,	// array of short			: signed 16-bit
    FIT_UINT32	= 4,	// array of unsigned long	: unsigned 32-bit
    FIT_INT32  	= 5,	// array of long			: signed 32-bit
    FIT_FLOAT	  = 6,	// array of float			: 32-bit IEEE floating point
    FIT_DOUBLE	= 7,	// array of double			: 64-bit IEEE floating point
    FIT_COMPLEX	= 8		// array of FICOMPLEX		: 2 x 64-bit IEEE floating point
  );

  // Image color type used in FreeImage.
  FREE_IMAGE_COLOR_TYPE = (
	  FIC_MINISWHITE = 0,		// min value is white
    FIC_MINISBLACK = 1,		// min value is black
    FIC_RGB        = 2,		// RGB color model
    FIC_PALETTE    = 3,		// color map indexed
	  FIC_RGBALPHA   = 4,		// RGB color model with alpha channel
	  FIC_CMYK       = 5		// CMYK color model
  );

  // Color quantization algorithms. Constants used in FreeImage_ColorQuantize.
  FREE_IMAGE_QUANTIZE = (
    FIQ_WUQUANT = 0,		// Xiaolin Wu color quantization algorithm
    FIQ_NNQUANT = 1			// NeuQuant neural-net quantization algorithm by Anthony Dekker
  );

  // Dithering algorithms. Constants used FreeImage_Dither.
  FREE_IMAGE_DITHER = (
    FID_FS            = 0,	// Floyd & Steinberg error diffusion
    FID_BAYER4x4      = 1,	// Bayer ordered dispersed dot dithering (order 2 dithering matrix)
    FID_BAYER8x8      = 2,	// Bayer ordered dispersed dot dithering (order 3 dithering matrix)
    FID_CLUSTER6x6    = 3,	// Ordered clustered dot dithering (order 3 - 6x6 matrix)
    FID_CLUSTER8x8    = 4,	// Ordered clustered dot dithering (order 4 - 8x8 matrix)
    FID_CLUSTER16x16  = 5   // Ordered clustered dot dithering (order 8 - 16x16 matrix)
  );

  // Upsampling / downsampling filters. Constants used in FreeImage_Rescale.
  FREE_IMAGE_FILTER = (
    FILTER_BOX	      = 0,	// Box, pulse, Fourier window, 1st order (constant) b-spline
    FILTER_BICUBIC    = 1,	// Mitchell & Netravali's two-param cubic filter
    FILTER_BILINEAR   = 2,	// Bilinear filter
    FILTER_BSPLINE    = 3,	// 4th order (cubic) b-spline
    FILTER_CATMULLROM = 4,	// Catmull-Rom spline, Overhauser spline
    FILTER_LANCZOS3   = 5	// Lanczos3 filter
  );
  // Color channels. Constants used in color manipulation routines.
  FREE_IMAGE_COLOR_CHANNEL = (
    FICC_RGB	  = 0, // Use red, green and blue channels
    FICC_RED	  = 1, // Use red channel
    FICC_GREEN	= 2, // Use green channel
    FICC_BLUE	  = 3, // Use blue channel
    FICC_ALPHA	= 4, // Use alpha channel
    FICC_BLACK	= 5, // Use black channel
    FICC_REAL	  = 6, // Complex images: use real part
	  FICC_IMAG	  = 7, // Complex images: use imaginary part
	  FICC_MAG	  = 8, // Complex images: use magnitude
  	FICC_PHASE	= 9	 // Complex images: use phase
  );

// MetaData support ---------------------------------------------------------

{
  Tag data type information (based on TIFF specifications)

  Note: RATIONALs are the ratio of two 32-bit integer values.
}

type
  FREE_IMAGE_MDTYPE = (
    FIDT_NOTYPE		 = 0,	 // placeholder
    FIDT_BYTE		   = 1,	 // 8-bit unsigned integer
    FIDT_ASCII	   = 2,	 // 8-bit bytes w/ last byte null
    FIDT_SHORT	   = 3,	 // 16-bit unsigned integer
    FIDT_LONG		   = 4,	 // 32-bit unsigned integer
    FIDT_RATIONAL  = 5,	 // 64-bit unsigned fraction
    FIDT_SBYTE		 = 6,	 // 8-bit signed integer
    FIDT_UNDEFINED = 7,	 // 8-bit untyped data
    FIDT_SSHORT	   = 8,	 // 16-bit signed integer
    FIDT_SLONG	   = 9,	 // 32-bit signed integer
    FIDT_SRATIONAL = 10, // 64-bit signed fraction
    FIDT_FLOAT	   = 11, // 32-bit IEEE floating point
    FIDT_DOUBLE	   = 12, // 64-bit IEEE floating point
    FIDT_IFD	     = 13	 // 32-bit unsigned integer (offset)
  );

{
  Metadata models supported by FreeImage
}
  FREE_IMAGE_MDMODEL = (
    FIMD_NODATA		    	= -1,
    FIMD_COMMENTS		    = 0,	// single comment or keywords
    FIMD_EXIF_MAIN		  = 1,	// Exif-TIFF metadata
    FIMD_EXIF_EXIF	  	= 2,	// Exif-specific metadata
    FIMD_EXIF_GPS		    = 3,	// Exif GPS metadata
    FIMD_EXIF_MAKERNOTE = 4,	// Exif maker note metadata
    FIMD_EXIF_INTEROP	  = 5,	// Exif interoperability metadata
    FIMD_IPTC		      	= 6,	// IPTC/NAA metadata
    FIMD_XMP		      	= 7,	// Abobe XMP metadata
    FIMD_GEOTIFF	    	= 8,	// GeoTIFF metadata (to be implemented)
    FIMD_CUSTOM		    	= 9		// Used to attach other metadata types to a dib
  );

{
  Handle to a metadata model
}

  FIMETADATA = record
    data: Pointer;
  end;
  PFIMETADATA = ^FIMETADATA;

{
  Metadata attribute
}

  FITAG = record
    key: PChar;			    // tag field name
    description: PChar; // tag description
    id: WORD;			      // tag ID
    mdtype: WORD;			  // tag data type (see FREE_IMAGE_MDTYPE)
    count: DWORD;		    // number of components (in 'tag data types' units)
    length: DWORD;	    // value length in bytes
    value: Pointer;		  // tag value
  end;
  PFITAG = ^FITAG;


// File IO routines ---------------------------------------------------------
type
  fi_handle = pointer;
  PCardinal = ^Cardinal;
  PInt = ^integer;

  FI_ReadProc = function(buffer : pointer; size : Cardinal; count : Cardinal; handle : fi_handle) : PCardinal; stdcall;
  FI_WriteProc = function(buffer : pointer; size, count : Cardinal; handle : fi_handle) : PCardinal; stdcall;
  FI_SeekProc = function(handle : fi_handle; offset : longint; origin : integer) : pint; stdcall;
  FI_TellProc = function(handle : fi_handle) : PCardinal; stdcall;

  FreeImageIO = packed record
    read_proc : FI_ReadProc;     // pointer to the function used to read data
    write_proc: FI_WriteProc;    // pointer to the function used to write data
    seek_proc : FI_SeekProc;     // pointer to the function used to seek
    tell_proc : FI_TellProc;     // pointer to the function used to aquire the current position
  end;

  PFreeImageIO = ^FreeImageIO;

{
  Handle to a memory I/O stream
}
  FIMEMORY = record
    data: Pointer;
  end;
  PFIMEMORY = ^FIMEMORY;

const
  SEEK_SET = 0;
  SEEK_CUR = 1;
  SEEK_END = 2;

// Plugin routines ----------------------------------------------------------
type
  PPluginStruct = ^PluginStruct;

  FI_InitProc = procedure(plugin : PPluginStruct; format_id : integer); stdcall;
  FI_FormatProc = function : PChar; stdcall;
  FI_DescriptionProc = function : PChar; stdcall;
  FI_ExtensionListProc = function : PChar; stdcall;
  FI_RegExprProc = function : PChar; stdcall;
  FI_OpenProc = function(io : PFreeImageIO; handle : fi_handle; read : boolean) : pointer; stdcall;
  FI_CloseProc = procedure(io : PFreeImageIO; handle : fi_handle; data : pointer); stdcall;
  FI_PageCountProc = function(io : PFreeImageIO; handle : fi_handle; data : pointer) : integer; stdcall;
  FI_PageCapabilityProc = function(io : PFreeImageIO; handle : fi_handle; data : pointer) : integer; stdcall;
  FI_LoadProc = function(io : PFreeImageIO; handle : fi_handle; page, flags : integer; data : pointer) : PFIBITMAP; stdcall;
  FI_SaveProc = function(io : PFreeImageIO; dib : PFIBITMAP; handle : fi_handle; page, flags : integer; data : pointer) : boolean; stdcall;
  FI_ValidateProc = function(io : PFreeImageIO; handle : fi_handle) : boolean; stdcall;
  FI_MimeProc = function : PChar; stdcall;
  FI_SupportsExportBPPProc = function(bpp : integer) : boolean; stdcall;
  FI_SupportsICCProfilesProc = function : boolean; stdcall;

  PluginStruct = record
    format_proc : FI_FormatProc;
    description_proc : FI_DescriptionProc;
    extension_proc : FI_ExtensionListProc;
    regexpr_proc : FI_RegExprProc;
    open_proc : FI_OpenProc;
    close_proc : FI_CloseProc;
    pagecount_proc : FI_PageCountProc;
    pagecapability_proc : FI_PageCapabilityProc;
    load_proc : FI_LoadProc;
    save_proc : FI_SaveProc;
    validate_proc : FI_ValidateProc;
    mime_proc : FI_MimeProc;
    supports_export_bpp_proc : FI_SupportsExportBPPProc;
    supports_icc_profiles_proc : FI_SupportsICCProfilesProc;
  end;

// Load/Save flag constants -----------------------------------------------------
const
  BMP_DEFAULT =        0;
  BMP_SAVE_RLE =       1;
  CUT_DEFAULT =        0;
  ICO_DEFAULT =        0;
  ICO_FIRST =          0;
  ICO_SECOND =         0;
  ICO_THIRD =          0;
  IFF_DEFAULT =        0;
  JPEG_DEFAULT =       0;
  JPEG_FAST =          1;
  JPEG_ACCURATE =      2;
  JPEG_QUALITYSUPERB = $80;
  JPEG_QUALITYGOOD =   $100;
  JPEG_QUALITYNORMAL = $200;
  JPEG_QUALITYAVERAGE =$400;
  JPEG_QUALITYBAD =    $800;
  KOALA_DEFAULT =      0;
  LBM_DEFAULT =        0;
  MNG_DEFAULT =        0;
  PCD_DEFAULT =        0;
  PCD_BASE =           1;
  PCD_BASEDIV4 =       2;
  PCD_BASEDIV16 =      3;
  PCX_DEFAULT =        0;
  PNG_DEFAULT =        0;
  PNG_IGNOREGAMMA =	   1;       // avoid gamma correction
  PNM_DEFAULT =        0;
  PNM_SAVE_RAW =       0;       // If set the writer saves in RAW format (i.e. P4, P5 or P6)
  PNM_SAVE_ASCII =     1;       // If set the writer saves in ASCII format (i.e. P1, P2 or P3)
  PSD_DEFAULT =        0;
  RAS_DEFAULT =        0;
  TARGA_DEFAULT =      0;
  TARGA_LOAD_RGB888 =  1;      // If set the loader converts RGB555 and ARGB8888 -> RGB888.
  TIFF_DEFAULT =       0;
  TIFF_CMYK	=          $0001;  // reads/stores tags for separated CMYK (use | to combine with compression flags)
  TIFF_PACKBITS =      $0100;  // save using PACKBITS compression
  TIFF_DEFLATE =       $0200;  // save using DEFLATE compression
  TIFF_ADOBE_DEFLATE = $0400;  // save using ADOBE DEFLATE compression
  TIFF_NONE =          $0800;  // save without any compression
  WBMP_DEFAULT =       0;
  XBM_DEFAULT =		   0;
  XPM_DEFAULT =        0;

  
// Init/Error routines ------------------------------------------------------

procedure FreeImage_Initialise(load_local_plugins_only : boolean = False); stdcall; external FIDLL name '_FreeImage_Initialise@4';
procedure FreeImage_DeInitialise; stdcall; external FIDLL name '_FreeImage_DeInitialise@0';

// Version routines ---------------------------------------------------------

function FreeImage_GetVersion : PChar; stdcall; external FIDLL name '_FreeImage_GetVersion@0';
function FreeImage_GetCopyrightMessage : PChar; stdcall; external FIDLL name '_FreeImage_GetCopyrightMessage@0';

// Message output functions -------------------------------------------------

procedure FreeImage_OutPutMessageProc(fif: Integer; fmt: PChar); stdcall;
  external FIDLL name 'FreeImage_OutputMessageProc';

type
  FreeImage_OutputMessageFunction = function(fif: FREE_IMAGE_FORMAT; msg: PChar): pointer; stdcall;

procedure FreeImage_SetOutputMessage(omf: FreeImage_OutputMessageFunction); stdcall;
  external FIDLL name '_FreeImage_SetOutputMessage@8';

// Allocate/Unload routines ------------------------------------------------

function FreeImage_Allocate(width, height, bpp: integer; red_mask: Cardinal = 0;
  green_mask: Cardinal = 0; blue_mask: Cardinal = 0): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_Allocate@24';

function FreeImage_Clone(dib: PFIBITMAP): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_Clone@4';

procedure FreeImage_Unload(dib: PFIBITMAP); stdcall;
  external FIDLL name '_FreeImage_Unload@4';

// Load / Save routines -----------------------------------------------------

function FreeImage_Load(fif: FREE_IMAGE_FORMAT; filename: PChar;
  flags: integer = 0): PFIBITMAP; stdcall; external FIDLL name '_FreeImage_Load@12';

function FreeImage_LoadFromHandle(fif: FREE_IMAGE_FORMAT; io: PFreeImageIO;
  handle: fi_handle; flags: integer = 0): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_LoadFromHandle@16';

function FreeImage_Save(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP; filename: PChar;
  flags: integer = 0): boolean; stdcall;
  external FIDLL name '_FreeImage_Save@16';

function FreeImage_SaveToHandle(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP;
  io : PFreeImageIO; handle : fi_handle; flags : integer = 0) : boolean; stdcall;
  external FIDLL name '_FreeImage_SaveToHandle@20';

// Memory I/O stream routines ----------------------------------------------

function FreeImage_OpenMemory(data: PByte = nil;
  size_in_bytes: DWORD = 0): PFIMEMORY; stdcall;
  external FIDLL name '_FreeImage_OpenMemory@8';

procedure FreeImage_CloseMemory(stream: PFIMEMORY); stdcall;
  external FIDLL name '_FreeImage_CloseMemory@4';

function FreeImage_LoadFromMemory(fif: FREE_IMAGE_FORMAT; stream: PFIMEMORY;
  flags: Integer = 0): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_LoadFromMemory@8';

function FreeImage_SaveToMemory(fif: FREE_IMAGE_FORMAT; dib: PFIBITMAP;
  stream: PFIMEMORY; flags: Integer = 0): Boolean; stdcall;
  external FIDLL name '_FreeImage_SaveToMemory@16';

function FreeImage_TellMemory(stream: PFIMEMORY): Longint; stdcall;
  external FIDLL name '_FreeImage_TellMemory@4';

function FreeImage_SeekMemory(stream: PFIMEMORY; offset: Longint;
  origin: Integer): Boolean; stdcall;
  external FIDLL name '_FreeImage_SeekMemory@12';

function FreeImage_AcquireMemory(stream: PFIMEMORY; var data: PByte;
  var size_in_bytes: DWORD): Boolean; stdcall;
  external FIDLL name '_FreeImage_AcquireMemory@12';

// Plugin Interface --------------------------------------------------------

function FreeImage_RegisterLocalPlugin(proc_address : FI_InitProc; format,
  description, extension, regexpr : PChar) : FREE_IMAGE_FORMAT; stdcall;
  external FIDLL name '_FreeImage_RegisterLocalPlugin@20';
  
function FreeImage_RegisterExternalPlugin(path, format, description, extension, regexpr : PChar): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL name '_FreeImage_RegisterExternalPlugin@20';
  
function FreeImage_GetFIFCount: integer; stdcall;
  external FIDLL name '_FreeImage_GetFIFCount@0';
  
procedure FreeImage_SetPluginEnabled(fif: FREE_IMAGE_FORMAT; enable: boolean); stdcall;
  external FIDLL Name '_FreeImage_SetPluginEnabled@8';
  
function FreeImage_IsPluginEnabled(fif: FREE_IMAGE_FORMAT): integer; stdcall;
  external FIDLL Name '_FreeImage_IsPluginEnabled@4';

function FreeImage_GetFIFFromFormat(format: PChar): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL Name '_FreeImage_GetFIFFromFormat@4';

function FreeImage_GetFIFFromMime(format: PChar): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL Name '_FreeImage_GetFIFFromMime@4';
  
function FreeImage_GetFormatFromFIF(fif: FREE_IMAGE_FORMAT): PChar; stdcall;
  external FIDLL Name '_FreeImage_GetFormatFromFIF@4';
  
function FreeImage_GetFIFExtensionList(fif: FREE_IMAGE_FORMAT): PChar; stdcall;
  external FIDLL Name '_FreeImage_GetFIFExtensionList@4';
  
function FreeImage_GetFIFDescription(fif: FREE_IMAGE_FORMAT): PChar; stdcall;
  external FIDLL Name '_FreeImage_GetFIFDescription@4';
  
function FreeImage_GetFIFRegExpr(fif: FREE_IMAGE_FORMAT): PChar; stdcall;
  external FIDLL Name '_FreeImage_GetFIFRegExpr@4';
  
function FreeImage_GetFIFFromFilename(fname: PChar): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL Name '_FreeImage_GetFIFFromFilename@4';

function FreeImage_FIFSupportsReading(fif: FREE_IMAGE_FORMAT): boolean; stdcall;
  external FIDLL Name '_FreeImage_FIFSupportsReading@4';
  
function FreeImage_FIFSupportsWriting(fif: FREE_IMAGE_FORMAT): boolean; stdcall;
  external FIDLL Name '_FreeImage_FIFSupportsWriting@4';
  
function FreeImage_FIFSupportsExportBPP(fif: FREE_IMAGE_FORMAT;
  bpp: integer): boolean; stdcall;
  external FIDLL Name '_FreeImage_FIFSupportsExportBPP@8';

function FreeImage_FIFSupportsICCProfiles(fif: FREE_IMAGE_FORMAT): boolean; stdcall;
  external FIDLL Name '_FreeImage_FIFSupportsICCProfiles@4';

function FreeImage_FIFSupportsExportType(fif: FREE_IMAGE_FORMAT;
  image_type: FREE_IMAGE_TYPE): Boolean; stdcall;
  external FIDLL name '_FreeImage_FIFSupportsExportType@12';

// Multipaging interface ----------------------------------------------------

function FreeImage_OpenMultiBitmap(fif: FREE_IMAGE_FORMAT; filename: PChar;
  create_new, read_only, keep_cache_in_memory: boolean): PFIMULTIBITMAP; stdcall;
  external FIDLL Name '_FreeImage_OpenMultiBitmap@20';

function FreeImage_CloseMultiBitmap(bitmap: PFIMULTIBITMAP;
  flags: integer = 0): boolean; stdcall;
  external FIDLL Name '_FreeImage_CloseMultiBitmap@8';

function FreeImage_GetPageCount(bitmap: PFIMULTIBITMAP): integer; stdcall;
  external FIDLL Name '_FreeImage_GetPageCount@4';

procedure FreeImage_AppendPage(bitmap: PFIMULTIBITMAP; data: PFIBITMAP); stdcall;
  external FIDLL Name '_FreeImage_AppendPage@8';

procedure FreeImage_InsertPage(bitmap: PFIMULTIBITMAP; page: integer; data: PFIBITMAP); stdcall;
  external FIDLL Name '_FreeImage_InsertPage@12';

procedure FreeImage_DeletePage(bitmap: PFIMULTIBITMAP; page: integer); stdcall;
  external FIDLL Name '_FreeImage_DeletePage@8';

function FreeImage_LockPage(bitmap: PFIMULTIBITMAP; page: integer): PFIBITMAP; stdcall;
  external FIDLL Name '_FreeImage_LockPage@8';

procedure FreeImage_UnlockPage(bitmap: PFIMULTIBITMAP; page: PFIBITMAP; changed: boolean); stdcall;
  external FIDLL Name '_FreeImage_UnlockPage@12';

function FreeImage_MovePage(bitmap: PFIMULTIBITMAP; target, source: integer): Boolean; stdcall;
  external FIDLL Name '_FreeImage_MovePage@12';

function FreeImage_GetLockedPageNumbers(bitmap: PFIMULTIBITMAP;
  var pages: integer; var count : integer): Boolean; stdcall;
  external FIDLL Name '_FreeImage_GetLockedPageNumbers@12';

// Filetype request routines -----------------------------------------------

function FreeImage_GetFileType(filename: PChar; size: integer): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL name '_FreeImage_GetFileType@8';

function FreeImage_GetFileTypeFromHandle(io: PFreeImageIO; handle: fi_handle;
  size: integer): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL name '_FreeImage_GetFileTypeFromHandle@12';

function FreeImage_GetFileTypeFromMemory(stream: PFIMEMORY;
  size: Integer = 0): FREE_IMAGE_FORMAT; stdcall;
  external FIDLL name '_FreeImage_GetFileTypeFromMemory@8'; 

// ImageType request routine ------------------------------------------------

function FreeImage_GetImageType(dib: PFIBITMAP): FREE_IMAGE_TYPE; stdcall;
  external FIDLL name '_FreeImage_GetImageType@8';

// FreeImage info routines -------------------------------------------------

function FreeImage_GetRedMask(dib: PFIBITMAP): cardinal; stdcall;
  external FIDLL name '_FreeImage_GetRedMask@4';

function FreeImage_GetGreenMask(dib: PFIBITMAP): cardinal; stdcall;
  external FIDLL name '_FreeImage_GetGreenMask@4';

function FreeImage_GetBlueMask(dib: PFIBITMAP): cardinal; stdcall;
  external FIDLL name '_FreeImage_GetBlueMask@4';

function FreeImage_GetTransparencyCount(dib: PFIBITMAP): cardinal; stdcall;
  external FIDLL name '_FreeImage_GetTransparencyCount@4';
  
function FreeImage_GetTransparencyTable(dib: PFIBITMAP): PByte; stdcall;
  external FIDLL name '_FreeImage_GetTransparencyTable@4';

procedure FreeImage_SetTransparent(dib: PFIBITMAP; enabled: boolean); stdcall;
  external FIDLL name '_FreeImage_SetTransparent@8';

procedure FreeImage_SetTransparencyTable(dib: PFIBITMAP; table: PByte; count: integer); stdcall;
  external FIDLL name '_FreeImage_SetTransparencyTable@12';

function FreeImage_IsTransparent(dib: PFIBITMAP): boolean; stdcall;
  external FIDLL name '_FreeImage_IsTransparent@4';

function FreeImage_HasBackgroundColor(dib: PFIBITMAP): Boolean; stdcall;
  external FIDLL name '_FreeImage_HasBackgroundColor@4';

function FreeImage_GetBackgroundColor(dib: PFIBITMAP; var bkcolor: PRGBQUAD): Boolean; stdcall;
  external FIDLL name '_FreeImage_GetBackgroundColor@12';

function FreeImage_SetBackgroundColor(dib: PFIBITMAP; bkcolor: PRGBQUAD): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetBackgroundColor@12';


// DIB info routines -------------------------------------------------------

function FreeImage_GetColorsUsed(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetColorsUsed@4';

function FreeImage_GetBits(dib: PFIBITMAP): PByte; stdcall;
  external FIDLL name '_FreeImage_GetBits@4';

function FreeImage_GetScanLine(dib: PFIBITMAP; scanline: integer): PByte; stdcall;
  external FIDLL name '_FreeImage_GetScanLine@8';

function FreeImage_GetBPP(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetBPP@4';

function FreeImage_GetWidth(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetWidth@4';

function FreeImage_GetHeight(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetHeight@4';

function FreeImage_GetLine(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetLine@4';

function FreeImage_GetPitch(dib : PFIBITMAP) : Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetPitch@4';
  
function FreeImage_GetDIBSize(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetDIBSize@4';

function FreeImage_GetPalette(dib: PFIBITMAP): PRGBQUAD; stdcall;
  external FIDLL name '_FreeImage_GetPalette@4';

function FreeImage_GetDotsPerMeterX(dib: PFIBITMAP): Cardinal; stdcall;
external FIDLL name '_FreeImage_GetDotsPerMeterX@4';

function FreeImage_GetDotsPerMeterY(dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetDotsPerMeterY@4';

function FreeImage_GetInfoHeader(dib: PFIBITMAP): PBITMAPINFOHEADER; stdcall;
  external FIDLL name '_FreeImage_GetInfoHeader@4';

function FreeImage_GetInfo(var dib: FIBITMAP): PBITMAPINFO; stdcall;
  external FIDLL name '_FreeImage_GetInfo@4';

function FreeImage_GetColorType(dib: PFIBITMAP): FREE_IMAGE_COLOR_TYPE; stdcall;
  external FIDLL name '_FreeImage_GetColorType@4';

// Pixels access routines ---------------------------------------------------

function FreeImage_GetPixelIndex(dib: PFIBITMAP; X, Y: Longint;
  var Value: PByte): Boolean; stdcall;
  external FIDLL name '_FreeImage_GetPixelIndex@16';

function FreeImage_GetPixelColor(dib: PFIBITMAP; X, Y: Longint;
  var Value: PRGBQuad): Boolean; stdcall;
  external FIDLL name '_FreeImage_GetPixelColor@16';

function FreeImage_SetPixelIndex(dib: PFIBITMAP; X, Y: Longint;
  Value: PByte): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetPixelIndex@16';

function FreeImage_SetPixelColor(dib: PFIBITMAP; X, Y: Longint;
  Value: PRGBQuad): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetPixelColor@16';

// ICC profile routines -----------------------------------------------------

function FreeImage_GetICCProfile(var dib: FIBITMAP): PFIICCPROFILE; stdcall;
  external FIDLL name '_FreeImage_GetICCProfile@4';

function FreeImage_CreateICCProfile(var dib: FIBITMAP; data: pointer;
  size: longint): PFIICCPROFILE; stdcall;
  external FIDLL name 'FreeImage_CreateICCProfile@12';

procedure FreeImage_DestroyICCProfile(var dib : FIBITMAP); stdcall;
  external FIDLL name 'FreeImage_DestroyICCProfile@4';

// Line conversion routines -----------------------------------------------------

procedure FreeImage_ConvertLine1To8(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine1To8@12';
procedure FreeImage_ConvertLine4To8(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine4To8@12';
procedure FreeImage_ConvertLine16To8_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To8_555@12';
procedure FreeImage_ConvertLine16To8_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To8_565@12';
procedure FreeImage_ConvertLine24To8(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine24To8@12';
procedure FreeImage_ConvertLine32To8(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine32To8@12';

procedure FreeImage_ConvertLine1To16_555(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine1To16_555@16';
procedure FreeImage_ConvertLine4To16_555(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine4To16_555@16';
procedure FreeImage_ConvertLine8To16_555(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine8To16_555@16';
procedure FreeImage_ConvertLine16_565_To16_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16_565_To16_555@12';
procedure FreeImage_ConvertLine24To16_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine24To16_555@12';
procedure FreeImage_ConvertLine32To16_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine32To16_555@12';

procedure FreeImage_ConvertLine1To16_565(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine1To16_565@16';
procedure FreeImage_ConvertLine4To16_565(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine4To16_565@16';
procedure FreeImage_ConvertLine8To16_565(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine8To16_565@16';
procedure FreeImage_ConvertLine16_555_To16_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16_555_To16_565@12';
procedure FreeImage_ConvertLine24To16_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine24To16_565@12';
procedure FreeImage_ConvertLine32To16_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine32To16_565@12';

procedure FreeImage_ConvertLine1To24(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine1To24@16';
procedure FreeImage_ConvertLine4To24(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine4To24@16';
procedure FreeImage_ConvertLine8To24(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine8To24@16';
procedure FreeImage_ConvertLine16To24_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To24_555@12';
procedure FreeImage_ConvertLine16To24_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To24_565@12';
procedure FreeImage_ConvertLine32To24(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine32To24@12';

procedure FreeImage_ConvertLine1To32(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine1To32@16';
procedure FreeImage_ConvertLine4To32(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine4To32@16';
procedure FreeImage_ConvertLine8To32(target, source : PBYTE; width_in_pixels : integer; palette : PRGBQUAD);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine8To32@16';
procedure FreeImage_ConvertLine16To32_555(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To32_555@12';
procedure FreeImage_ConvertLine16To32_565(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine16To32_565@12';
procedure FreeImage_ConvertLine24To32(target, source : PBYTE; width_in_pixels : integer);
                                stdcall; external FIDLL name '_FreeImage_ConvertLine24To32@12';

// Smart conversion routines ------------------------------------------------

function FreeImage_ConvertTo8Bits(dib : PFIBITMAP) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertTo8Bits@4';
function FreeImage_ConvertTo16Bits555(dib : PFIBITMAP) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertTo16Bits555@4';
function FreeImage_ConvertTo16Bits565(dib : PFIBITMAP) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertTo16Bits565@4';
function FreeImage_ConvertTo24Bits(dib : PFIBITMAP) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertTo24Bits@4';
function FreeImage_ConvertTo32Bits(dib : PFIBITMAP) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertTo32Bits@4';

function FreeImage_ColorQuantize(dib : PFIBITMAP; quantize : FREE_IMAGE_QUANTIZE) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ColorQuantize@8';

function FreeImage_Threshold(dib : PFIBITMAP; T : Byte) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_Threshold@8';
function FreeImage_Dither(dib : PFIBITMAP; algorithm : FREE_IMAGE_DITHER) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_Dither@8';


function FreeImage_ConvertFromRawBits(bits : PBYTE; width, height, pitch : integer;
                                bpp, red_mask, green_mask, blue_mask : longword;
                                topdown : boolean) : FIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_ConvertFromRawBits@36';

procedure FreeImage_ConvertToRawBits(bits : PBYTE; dib : PFIBITMAP; pitch : integer;
                                bpp, red_mask, green_mask, blue_mask : longword;
                                topdown  : boolean);
                                stdcall; external FIDLL name '_FreeImage_ConvertToRawBits@32';

function FreeImage_ConvertToStandardType(src: PFIBITMAP;
  scale_linear: Boolean = True): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_ConvertToStandardType@8';

function FreeImage_ConvertToType(src: PFIBITMAP; dst_type: FREE_IMAGE_TYPE;
  scale_linear: Boolean = True): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_ConvertToType@16';



// ZLib interface -----------------------------------------------------------

function FreeImage_ZLibCompress(target : PBYTE; target_size : DWORD; source : PBYTE;
                                source_size : DWORD) : DWORD; stdcall;external FIDLL name '_FreeImage_ZLibCompress@16';


function FreeImage_ZLibUncompress(target : PBYTE; target_size : DWORD; source : PBYTE;
                                source_size : DWORD) : DWORD; stdcall;external FIDLL name '_FreeImage_ZLibUncompress@16';

// --------------------------------------------------------------------------
// Metadata routines --------------------------------------------------------
// --------------------------------------------------------------------------

// iterator
function FreeImage_FindFirstMetadata(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  var tag: PFITAG): PFIMETADATA; stdcall;
  external FIDLL name '_FreeImage_FindFirstMetadata@12';

function FreeImage_FindNextMetadata(mdhandle: PFIMETADATA;
  var tag: PFITAG): Boolean; stdcall;
  external FIDLL name '_FreeImage_FindNextMetadata@8';

procedure FreeImage_FindCloseMetadata(mdhandle: PFIMETADATA); stdcall;
  external FIDLL name '_FreeImage_FindCloseMetadata@4';


// setter and getter

function FreeImage_SetMetadata(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  const key: PChar; tag: PFITAG): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetMetadata@16';

function FreeImage_GetMetaData(model: FREE_IMAGE_MDMODEL; dib: PFIBITMAP;
  const key: PChar; var tag: PFITAG): Boolean; stdcall;
  external FIDLL name '_FreeImage_GetMetadata@16';

// helpers
function FreeImage_GetMetadataCount(model: FREE_IMAGE_MDMODEL;
  dib: PFIBITMAP): Cardinal; stdcall;
  external FIDLL name '_FreeImage_GetMetadataCount@8';

// tag to C string conversion
function FreeImage_TagToString(model: FREE_IMAGE_MDMODEL; tag: PFITAG;
  Make: PChar = nil): PChar; stdcall;
  external FIDLL name '_FreeImage_TagToString@12';


// --------------------------------------------------------------------------
// Image manipulation toolkit -----------------------------------------------
// --------------------------------------------------------------------------

// rotation and flipping
function FreeImage_RotateClassic(dib : PFIBITMAP; angle : Double) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_RotateClassic@12';
function FreeImage_RotateEx(dib : PFIBITMAP; angle, x_shift, y_shift, x_origin, y_origin : Double;
                                use_mask : Boolean) : PFIBITMAP; stdcall; external FIDLL name '_FreeImage_RotateEx@48';
function FreeImage_FlipHorizontal(dib : PFIBITMAP) : Boolean; stdcall; external FIDLL name '_FreeImage_FlipHorizontal@4';
function FreeImage_FlipVertical(dib : PFIBITMAP) : Boolean; stdcall; external FIDLL name '_FreeImage_FlipVertical@4';

// upsampling / downsampling
function FreeImage_Rescale(dib : PFIBITMAP; dst_width, dst_height : Integer; filter : FREE_IMAGE_FILTER) : PFIBITMAP;
                                stdcall; external FIDLL name '_FreeImage_Rescale@16';
// color manipulation routines (point operations)
function FreeImage_AdjustCurve(dib : PFIBITMAP; LUT : PBYTE; channel : FREE_IMAGE_COLOR_CHANNEL) : Boolean;
                                stdcall; external FIDLL name '_FreeImage_AdjustCurve@12';
function FreeImage_AdjustGamma(dib : PFIBITMAP; gamma : Double) : Boolean;
                                stdcall; external FIDLL name '_FreeImage_AdjustGamma@12';
function FreeImage_AdjustBrightness(dib : PFIBITMAP; percentage : Double) : Boolean;
                                stdcall; external FIDLL name '_FreeImage_AdjustBrightness@12';
function FreeImage_AdjustContrast(dib : PFIBITMAP; percentage : Double) : Boolean;
                                stdcall; external FIDLL name '_FreeImage_AdjustContrast@12';
function FreeImage_Invert(dib : PFIBITMAP) : Boolean; stdcall; external FIDLL name '_FreeImage_Invert@4';
function FreeImage_GetHistogram(dib : PFIBITMAP; histo : PDWORD; channel : FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK) : Boolean;
                                stdcall; external FIDLL name '_FreeImage_GetHistogram@12';
// channel processing routines
function FreeImage_GetChannel(dib: PFIBITMAP;
  channel: FREE_IMAGE_COLOR_CHANNEL): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_GetChannel@8';
  
function FreeImage_SetChannel(dib, dib8: PFIBITMAP;
  channel: FREE_IMAGE_COLOR_CHANNEL): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetChannel@12';

function FreeImage_GetComplexChannel(src: PFIBITMAP;
  channel: FREE_IMAGE_COLOR_CHANNEL): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_GetComplexChannel@8';

function FreeImage_SetComplexChannel(src: PFIBITMAP;
  channel: FREE_IMAGE_COLOR_CHANNEL): Boolean; stdcall;
  external FIDLL name '_FreeImage_SetComplexChannel@12';

// copy / paste / composite routines

function FreeImage_Copy(dib: PFIBITMAP;
  left, top, right, bottom: Integer): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_Copy@20';
  
function FreeImage_Paste(dst, src: PFIBITMAP;
  left, top, alpha: Integer): Boolean; stdcall;
  external FIDLL name '_FreeImage_Paste@20';

function FreeImage_Composite(fg: PFIBITMAP; useFileBkg: Boolean = False;
  appBkColor: PRGBQUAD = nil; bg: PFIBITMAP = nil): PFIBITMAP; stdcall;
  external FIDLL name '_FreeImage_Composite@16';
  
{$MINENUMSIZE 1}
implementation

end.
