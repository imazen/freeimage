Attribute VB_Name = "MFreeImage"
'// ==========================================================
'// Visual Basic Wrapper for FreeImage 3
'// Original FreeImage 3 functions and VB compatible derived functions
'// Design and implementation by
'// - Carsten Klein (cklein05@users.sourceforge.net)
'//
'// Main reference : Curland, Matthew., Advanced Visual Basic 6, Addison Wesley, ISBN 0201707128, (c) 2000
'//                  Steve McMahon, creator of the excellent site vbAccelerator at http://www.vbaccelerator.com/
'//                  MSDN Knowlede Base
'//
'// This file is part of FreeImage 3
'//
'// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS, WITHOUT WARRANTY
'// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, WITHOUT LIMITATION, WARRANTIES
'// THAT THE COVERED CODE IS FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE
'// OR NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE COVERED
'// CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE DEFECTIVE IN ANY RESPECT, YOU (NOT
'// THE INITIAL DEVELOPER OR ANY OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY
'// SERVICING, REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN ESSENTIAL
'// PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS AUTHORIZED HEREUNDER EXCEPT UNDER
'// THIS DISCLAIMER.
'//
'// Use at your own risk!
'// ==========================================================

'// ==========================================================
'// CVS
'// $Revision$
'// $Date$
'// $Id$
'// ==========================================================


Option Explicit

'--------------------------------------------------------------------------------
' General notes on implementation and design
'--------------------------------------------------------------------------------

' Most of the pointer type parameters used in the FreeImage API are actually
' declared as Long in VB. That is also true for return values. Out parameters are
' declared ByRef, so they can receive the provided address of the pointer. In
' parameters are declared ByVal since in VB the Long variable is not a pointer
' type but contains the address of the pointer.

' Some of the following external function declarations of the FreeImage 3 functions
' are declared Private. Additionally the token 'Int' is appended to the VB function
' name, what means 'Internal' to avoid naming confusion. All of these are declared
' as 'const char *' in C/C++ and so actually return a string pointer. Without using
' a type library for declaring these functions, in VB it is impossible to declare
' these functions to return a VB String type. So each of these functions is wrapped
' by a VB implemented function named correctly according to theFreeImage API,
' actually returning a 'real' VB String.

' Some of the functions are additionally provided in an extended, call it a more VB
' friendly version, named '...Ex'. For example look at the 'FreeImage_GetPaletteEx'
' function. Most of them are dealing with arrays and so actually return a VB style
' array of correct type.

' The wrapper also includes some derived functions that should make life easier for
' not only a VB programmer.

' Better VB interoperability is given by offering conversion between DIBs and
' VB Picture objects. See the FreeImage_CreateFromOlePicture and
' FreeImage_GetOlePicture functions.

' Both known VB functions LoadPicture() and SavePicture() are provided in extended
' versions calles LoadPictureEx() and SavePictureEx() offering the FreeImage 3´s
' image file types.

' The FreeImage 3 error handling is provided in VB after calling the VB specific
' function FreeImage_InitErrorHandler()


' All of the enumaration members are additionally 'declared' as constants in a
' conditional compiler directive '#If...#Then' block that is actually unreachable.
' For example see:
'
' Public Enum FREE_IMAGE_QUANTIZE
'    FIQ_WUQUANT = 0           ' Xiaolin Wu color quantization algorithm
'    FIQ_NNQUANT = 1           ' NeuQuant neural-net quantization algorithm by Anthony Dekker
' End Enum
' #If False Then
'    Const FIQ_WUQUANT = 0
'    Const FIQ_NNQUANT = 1
' #End If
'
' Since this module is supposed to be used directly in VB projects rather than in
' compiled form (mybe through an ActiveX-DLL), this is for tweaking some ugly VB
' behaviour regarding enumerations. Enum members are automatically adjusted in case
' by the VB IDE whenever you type these members in wrong case. Since these are also
' constants now, they are no longer adjusted to wrong case but always corrected
' according to the definition of the constant. As the expression '#If False Then'
' actually never comes true, these constants are not really defined either when running
' in the VB IDE nor in compiled form.


'--------------------------------------------------------------------------------
' ToDo and known issues (unordered and with no priority)
'--------------------------------------------------------------------------------

' ToDo: implement icon and image list related functions

' ToDo: implement scanline conversion functions

' ToDo: implement image alpha transparency helper functions

' Bug:  FreeImage_PaintDC paints more scanlines than needed

' ToDo: support better clipping for FreeImage_PaintDC

' ToDo: currently poor and experimental support of tag accessing functions

' ToDo: implement ICC Profile function wrappers (do we need that?)


'--------------------------------------------------------------------------------
' Change Log
'--------------------------------------------------------------------------------

'* : fixed
'- : removed
'! : changed
'+ : added
'
'September 9, 2005 - 1.4
'! [Carsten Klein] changed wrapper function FreeImage_ConvertColorDepth(): now uses FreeImage_ConvertToGreyscale
'+ [Carsten Klein] added version numbers to change log
'+ [Carsten Klein] added comments to IOlePicture aware toolkit and conversion functions
'* [Carsten Klein] fixed and adjusted page numbers of the API documentation in header comments in FreeImage function declarations
'
'! not FreeImage version 3.8.0
'
'September 8, 2005 - 1.3.5
'! [Carsten Klein] changed version constant 'FREEIMAGE_MINOR_VERSION' to 8 to meet version 3.8.0
'+ [Carsten Klein] added function declarations for UNICODE dealing functions with additional token 'Int' appended:
'+                 added function FreeImage_LoadUInt()
'+                 added function FreeImage_SaveUInt()
'+                 added function FreeImage_GetFileTypeUInt()
'+                 added function FreeImage_GetFIFFromFilenameUInt()
'+ [Carsten Klein] added wrapper functions to ease the use of UNICODE dealing functions:
'+                 added function FreeImage_LoadU()
'+                 added function FreeImage_SaveU()
'+                 added function FreeImage_GetFileTypeU()
'+                 added function FreeImage_GetFIFFromFilenameU()
'+ [Carsten Klein] added function declaration for FreeImage_ConvertToGreyscale()
'
'July 18, 2005 - 1.3.4
'! [Carsten Klein] changed inline comments in these wrapper functions:
'!                 changed FreeImage_GetBitsEx(): mixed up width and height in SAFEAARAY creation
'!                 changed FreeImage_GetBitsExRGBTRIPLE(): mixed up width and height in SAFEAARAY creation
'!                 changed FreeImage_GetBitsExRGBQUAD(): mixed up width and height in SAFEAARAY creation
'
'June 30, 2005 - 1.3.3
'+ [Carsten Klein] added Kernel32 function FillMemory()
'
'June 24, 2005 - 1.3.2
'+ [Carsten Klein] added pixel access functions FreeImage_GetBitsExRGBTRIPLE() and FreeImage_GetBitsExRGBQUAD()
'+ [Carsten Klein] added IOlePicture based wrapper function FreeImage_ConvertColorDepthIOP()
'+ [Carsten Klein] added IOlePicture based wrapper functions for FreeImage_RescaleIOP():
'+                 added function FreeImage_FreeImage_RescaleByPixelIOP()
'+                 added function FreeImage_FreeImage_RescaleByPercentIOP()
'+                 added function FreeImage_FreeImage_RescaleByFactorIOP()
'+ [Carsten Klein] added IOlePicture based wrapper function FreeImage_RescaleIOP()
'* [Carsten Klein] fixed a bug in FreeImage_GetOlePicture(): now OlePictures returned through IPicture may be used directly by other functions accepting IPicture types without any assignment to IPictureDisp
'
'June 24, 2005 - 1.3.1
'! [Carsten Klein] changed inproper function declaration of function FreeImage_AdjustCurve(): now parameter 'LUT' is passed ByVal
'+ [Carsten Klein] added wrapper function FreeImage_AdjustCurveEx()
'+ [Carsten Klein] added IOlePicture based wrapper functions for FreeImage toolkit functions:
'+                 added function FreeImage_RotateClassicIOP()
'+                 added function FreeImage_RotateExIOP()
'+                 added function FreeImage_FlipHorizontalIOP()
'+                 added function FreeImage_FlipVerticalIOP()
'+                 added function FreeImage_AdjustCurveIOP()
'+                 added function FreeImage_AdjustGammaIOP()
'+                 added function FreeImage_AdjustBrightnessIOP()
'+                 added function FreeImage_AdjustContrastIOP()
'+                 added function FreeImage_InvertIOP()
'+                 added function FreeImage_GetChannelIOP()
'+                 added function FreeImage_CopyIOP()
'+                 added function FreeImage_PasteIOP()
'
'June 22, 2005 - 1.3
'+ [Carsten Klein] added inline comments and documentation for pixel access functions
'
'June 18, 2005 - 1.2.9
'+ [Carsten Klein] added function FreeImage_GetBitsEx()
'+ [Carsten Klein] added structure SAFEARRAY2D to create 2 dimensional custom arrays
'+ [Carsten Klein] added function declarations for converting scanlines to 4 bpp:
'+                 added declaration for FreeImage_ConvertLine1To4()
'+                 added declaration for FreeImage_ConvertLine8To4()
'+                 added declaration for FreeImage_ConvertLine16To4_555()
'+                 added declaration for FreeImage_ConvertLine16To4_565()
'+                 added declaration for FreeImage_ConvertLine24To4()
'+                 added declaration for FreeImage_ConvertLine32To4()
'
'June 16, 2005 - 1.2.8
'! [Carsten Klein] changed inproper function declaration for all functions FreeImage_ConvertLineXXXX(): now parameters 'target' and 'Source' are passed ByVal
'
'June 15, 2005 - 1.2.7
'+ [Carsten Klein] added function FreeImage_DestroyLockedArrayByPtr() to destroy a locked array by it's pointer (VB can't pass a array of structures through a Variant type)
'+ [Carsten Klein] added some wrapper functions for FreeImage_DestroyLockedArrayByPtr() for common FreeImage structures:
'+                 added function FreeImage_DestroyLockedArrayRGBTRIPLE()
'+                 added function FreeImage_DestroyLockedArrayRGBQUAD()
'+                 added function FreeImage_DestroyLockedArrayFICOMPLEX()
'+                 added function FreeImage_DestroyLockedArrayFIRGB16()
'+                 added function FreeImage_DestroyLockedArrayFIRGBA16()
'+                 added function FreeImage_DestroyLockedArrayFIRGBF()
'+                 added function FreeImage_DestroyLockedArrayFIRGBAF()
'+ [Carsten Klein] added functions to return scanlines as VB style arrays in all supported FreeImage formats:
'+                 added function FreeImage_GetScanLineBITMAP8()
'+                 added function FreeImage_GetScanLineBITMAP16()
'+                 added function FreeImage_GetScanLineBITMAP24()
'+                 added function FreeImage_GetScanLineBITMAP32()
'+                 added function FreeImage_GetScanLineINT16()
'+                 added function FreeImage_GetScanLineINT32()
'+                 added function FreeImage_GetScanLineFLOAT()
'+                 added function FreeImage_GetScanLineDOUBLE()
'+                 added function FreeImage_GetScanLineCOMPLEX()
'+                 added function FreeImage_GetScanLineRGB16()
'+                 added function FreeImage_GetScanLineRGBA16()
'+                 added function FreeImage_GetScanLineRGBF()
'+                 added function FreeImage_GetScanLineRGBAF()
'
'June 14, 2005 - 1.2.6
'! [Carsten Klein] updated documentation on array-dealing functions using arrays with custom array descriptors
'+ [Carsten Klein] added function FreeImage_DestroyLockedArray() to destroy a self created array 'FADF_AUTO Or FADF_FIXEDSIZE' array
'+ [Carsten Klein] added function FreeImage_GetPaletteExLong() to return palette data in an array of type Long
'+ [Carsten Klein] added parameters 'lPaletteSize', 'vntReservePalette' and 'lReserveSize' to FreeImage_ConvertColorDepth()
'
'June 13, 2005 - 1.2.5
'* [Carsten Klein] fixed a bug in helper function pGetMemoryBlockPtrFromVariant(): now 'size_in_bytes' will never exceed the size of an array provided
'
'June 12, 2005 - 1.2.4
'+ [Carsten Klein] added ZLib compression function wrappers dealing with VB style arrays:
'+                 added function FreeImage_ZLibCompressVB()
'+                 added function FreeImage_ZLibUncompressVB()
'+                 added function FreeImage_ZLibGZipVB()
'+                 added function FreeImage_ZLibGUnzipVB()
'
'June 10, 2005 - 1.2.3
'+ [Carsten Klein] added ZLib compression function wrappers dealing with VB style arrays:
'+                 added function FreeImage_ZLibCompressEx()
'+                 added function FreeImage_ZLibUncompressEx()
'+                 added function FreeImage_ZLibGZipEx()
'+                 added function FreeImage_ZLibCRC32Ex()
'+                 added function FreeImage_ZLibGUnzipEx()
'+ [Carsten Klein] added more VB friendly ZLib compression function wrappers:
'+                 added function FreeImage_ZLibCompressVB()
'+                 added function FreeImage_ZLibUncompressVB()
'+                 added function FreeImage_ZLibGZipVB()
'+                 added function FreeImage_ZLibGUnzipVB()
'! [Carsten Klein] fixed wrong function declaration of functions FreeImage_ZLibGUnzip(): alias was '_FreeImage_ZLibZlibGUnzip@16' (double ZLib)
'! [Carsten Klein] fixed function pGetArrayPtrFromVariantArray() that now can deal with uninitialized arrays
'!                 fixed function pGetMemoryBlockPtrFromVariant() that now can deal with uninitialized arrays
'! [Carsten Klein] fixed wrong function declaration of functions FreeImage_AdjustBrightness(): ...@8 -> ...@12
'!                 fixed wrong function declaration of functions FreeImage_AdjustContrast(): ...@8 -> ...@12
'!                 fixed wrong function declaration of functions FreeImage_AdjustGamma(): ...@8 -> ...@12
'!                 fixed wrong function declaration of functions FreeImage_RotateClassic(): ...@8 -> ...@12
'!                 fixed wrong function declaration of functions FreeImage_RotateEx(): ...@28 -> ...@48
'
'June 9, 2005 - 1.2.2
'! [Carsten Klein] fixed wrong function declaration of function FreeImage_OpenMultiBitmap(): added parameter 'flags' (...@20 -> ...@24)
'
'June 8, 2005 - 1.2.1
'! [Carsten Klein] refactored function FreeImage_LoadFromMemoryEx(): now using pGetMemoryBlockPtrFromVariant()
'+ [Carsten Klein] added private function pGetMemoryBlockPtrFromVariant() to get poiner and size of a memory block from a Variant parameter
'! [Carsten Klein] changed declaration of ZLib related functions: 'target' and 'Source' are now 'ByVal Long'
'
'June 7, 2005 - 1.2
'+ [Carsten Klein] added some more inline comments and documentation
'+ [Carsten Klein] added optional parameter 'bUnloadSource' to function FreeImage_SaveToMemoryEx()
'+                 added optional parameter 'bUnloadSource' to function FreeImage_SaveToMemoryEx2()
'+ [Carsten Klein] added optional parameter 'InPercent' to function SavePictureEx()
'!                 implemented the capability to resize the image on saving in function SavePictureEx()
'+ [Carsten Klein] added parameters 'InPercent' and 'Format' to function LoadPictureEx()
'* [Carsten Klein] fixed wrong function declaration of function FreeImage_JPEGTransform() (...@12 -> ...@16)
'
'June 6, 2005 - 1.1.2
'+ [Carsten Klein] added some more inline comments and documentation
'
'May 30, 2005 - 1.1.1
'* [Carsten Klein] fixed percent calculating bug in function FreeImage_RescaleEx()
'!                 changed behaviour of parameter 'bIsPercentValue' -> it now has no effect on integer values
'+                 added function FreeImage_RescaleByPixel() to avoid confusion with overloading
'+                 added function FreeImage_RescaleByPercent() to avoid confusion with overloading
'+                 added function FreeImage_RescaleByFactor() to avoid confusion with overloading
'! [Carsten Klein] changed name of parameter 'bUnloadDIB' to 'bUnloadSource' of function FreeImage_GetOlePicture()
'+ [Carsten Klein] added some more inline comments and documentation
'* [Carsten Klein] fixed a potential runtime error in function FreeImage_SetTransparencyTableEx(): 'Count' will no longer exceed 256
'
'May 24, 2005 - 1.1
'+ [Carsten Klein] added a new VB wrapper


'--------------------------------------------------------------------------------
' Win32 API function, struct and constant declarations
'--------------------------------------------------------------------------------

Private Const ERROR_SUCCESS As Long = 0

'KERNEL32
Private Declare Sub CopyMemory Lib "kernel32.dll" Alias "RtlMoveMemory" ( _
    ByRef Destination As Any, _
    ByRef Source As Any, _
    ByVal Length As Long)
    
Private Declare Sub FillMemory Lib "kernel32.dll" Alias "RtlFillMemory" ( _
    ByRef Destination As Any, _
    ByVal Length As Long, _
    ByVal Fill As Byte)

Private Declare Function lstrlen Lib "kernel32.dll" Alias "lstrlenA" ( _
    ByVal lpString As Long) As Long
    

'OLEAUT32
Private Declare Sub OleCreatePictureIndirect Lib "oleaut32.dll" ( _
    ByRef lpPictDesc As PictDesc, _
    ByRef riid As Guid, _
    ByVal fOwn As Long, _
    ByRef lplpvObj As IPicture)
    
Private Declare Function SafeArrayAllocDescriptor Lib "oleaut32.dll" ( _
    ByVal cDims As Long, _
    ByRef ppsaOut As Long) As Long
    
Private Declare Sub SafeArrayDestroyDescriptor Lib "oleaut32.dll" ( _
    ByVal psa As Long)
    
Private Declare Sub SafeArrayDestroyData Lib "oleaut32.dll" ( _
    ByVal psa As Long)

    

'SAFEARRAY
Private Const FADF_AUTO As Long = (&H1)
Private Const FADF_FIXEDSIZE As Long = (&H10)

Private Type SAVEARRAY1D
   cDims As Integer
   fFeatures As Integer
   cbElements As Long
   cLocks As Long
   pvData As Long
   cElements As Long
   lLbound As Long
End Type

Private Type SAVEARRAY2D
   cDims As Integer
   fFeatures As Integer
   cbElements As Long
   cLocks As Long
   pvData As Long
   cElements1 As Long
   lLbound1 As Long
   cElements2 As Long
   lLbound2 As Long
End Type


'MSVBVM60
Private Declare Function VarPtrArray Lib "msvbvm60.dll" Alias "VarPtr" ( _
    ByRef Ptr() As Any) As Long


'USER32
Private Declare Function ReleaseDC Lib "user32.dll" ( _
    ByVal hWnd As Long, _
    ByVal hDC As Long) As Long

Private Declare Function GetDC Lib "user32.dll" ( _
    ByVal hWnd As Long) As Long

Private Declare Function GetDesktopWindow Lib "user32.dll" () As Long

Private Declare Function GetClientRect Lib "user32.dll" ( _
    ByVal hWnd As Long, _
    ByRef lpRect As RECT) As Long

Private Type RECT
   Left As Long
   Top As Long
   Right As Long
   Bottom As Long
End Type

Private Type Guid
   Data1 As Long
   Data2 As Integer
   Data3 As Integer
   Data4(0 To 7) As Byte
End Type

Private Type PictDesc
   cbSizeofStruct As Long
   picType As Long
   hImage As Long
   xExt As Long
   yExt As Long
End Type

Private Type BITMAP
   bmType As Long
   bmWidth As Long
   bmHeight As Long
   bmWidthBytes As Long
   bmPlanes As Integer
   bmBitsPixel As Integer
   bmBits As Long
End Type
   
    
'GDI32
Private Declare Function GetStretchBltMode Lib "gdi32.dll" ( _
    ByVal hDC As Long) As Long

Private Declare Function SetStretchBltMode Lib "gdi32.dll" ( _
    ByVal hDC As Long, _
    ByVal nStretchMode As Long) As Long
    
Private Declare Function SetDIBitsToDevice Lib "gdi32.dll" ( _
    ByVal hDC As Long, _
    ByVal x As Long, _
    ByVal y As Long, _
    ByVal dx As Long, _
    ByVal dy As Long, _
    ByVal SrcX As Long, _
    ByVal SrcY As Long, _
    ByVal Scan As Long, _
    ByVal NumScans As Long, _
    ByVal Bits As Long, _
    ByVal BitsInfo As Long, _
    ByVal wUsage As Long) As Long
    
Private Declare Function StretchDIBits Lib "gdi32.dll" ( _
    ByVal hDC As Long, _
    ByVal x As Long, _
    ByVal y As Long, _
    ByVal dx As Long, _
    ByVal dy As Long, _
    ByVal SrcX As Long, _
    ByVal SrcY As Long, _
    ByVal wSrcWidth As Long, _
    ByVal wSrcHeight As Long, _
    ByVal lpBits As Long, _
    ByVal lpBitsInfo As Long, _
    ByVal wUsage As Long, _
    ByVal dwRop As Long) As Long
    
Private Declare Function CreateDIBitmap Lib "gdi32.dll" ( _
    ByVal hDC As Long, _
    ByVal lpInfoHeader As Long, _
    ByVal dwUsage As Long, _
    ByVal lpInitBits As Long, _
    ByVal lpInitInfo As Long, _
    ByVal wUsage As Long) As Long

Private Const CBM_INIT As Long = &H4
    
Private Declare Function GetObjectAPI Lib "gdi32.dll" Alias "GetObjectA" ( _
    ByVal hObject As Long, _
    ByVal nCount As Long, _
    ByRef lpObject As Any) As Long

Private Declare Function GetDIBits Lib "gdi32.dll" ( _
    ByVal aHDC As Long, _
    ByVal hBitmap As Long, _
    ByVal nStartScan As Long, _
    ByVal nNumScans As Long, _
    ByVal lpBits As Long, _
    ByVal lpBI As Long, _
    ByVal wUsage As Long) As Long


Private Const BLACKONWHITE As Long = 1
Private Const WHITEONBLACK As Long = 2
Private Const COLORONCOLOR As Long = 3

Public Enum STRETCH_MODE
   SM_BLACKONWHITE = BLACKONWHITE
   SM_WHITEONBLACK = WHITEONBLACK
   SM_COLORONCOLOR = COLORONCOLOR
End Enum
#If False Then
   Const SM_BLACKONWHITE = BLACKONWHITE
   Const SM_WHITEONBLACK = WHITEONBLACK
   Const SM_COLORONCOLOR = COLORONCOLOR
#End If



Private Const SRCAND As Long = &H8800C6
Private Const SRCCOPY As Long = &HCC0020
Private Const SRCERASE As Long = &H440328
Private Const SRCINVERT As Long = &H660046
Private Const SRCPAINT As Long = &HEE0086

Public Enum RASTER_OPERATOR
   ROP_SRCAND = SRCAND
   ROP_SRCCOPY = SRCCOPY
   ROP_SRCERASE = SRCERASE
   ROP_SRCINVERT = SRCINVERT
   ROP_SRCPAINT = SRCPAINT
End Enum
#If False Then
   Const ROP_SRCAND = SRCAND
   Const ROP_SRCCOPY = SRCCOPY
   Const ROP_SRCERASE = SRCERASE
   Const ROP_SRCINVERT = SRCINVERT
   Const ROP_SRCPAINT = SRCPAINT
#End If

Private Const DIB_PAL_COLORS As Long = 1
Private Const DIB_RGB_COLORS As Long = 0

Public Enum DRAW_MODE
   DM_DRAW_DEFAULT = &H0
   DM_MIRROR_NONE = DM_DRAW_DEFAULT
   DM_MIRROR_VERTICAL = &H1
   DM_MIRROR_HORIZONTAL = &H2
   DM_MIRROR_BOTH = DM_MIRROR_VERTICAL Or DM_MIRROR_HORIZONTAL
End Enum
#If False Then
   Const DM_DRAW_DEFAULT = &H0
   Const DM_MIRROR_NONE = DM_DRAW_DEFAULT
   Const DM_MIRROR_VERTICAL = &H1
   Const DM_MIRROR_HORIZONTAL = &H2
   Const DM_MIRROR_BOTH = DM_MIRROR_VERTICAL Or DM_MIRROR_HORIZONTAL
#End If

Public Enum HISTOGRAM_ORIENTATION
   HOR_TOP_DOWN = &H0
   HOR_BOTTOM_UP = &H1
End Enum
#If False Then
   Const HOR_TOP_DOWN = &H0
   Const HOR_BOTTOM_UP = &H1
#End If


'--------------------------------------------------------------------------------
' FreeImage 3 types, constants and enumerations
'--------------------------------------------------------------------------------

'FREEIMAGE

' Version information
Public Const FREEIMAGE_MAJOR_VERSION As Long = 3
Public Const FREEIMAGE_MINOR_VERSION As Long = 8
Public Const FREEIMAGE_RELEASE_SERIAL As Long = 0

' Memory stream pointer operation flags
Public Const SEEK_SET As Long = 0
Public Const SEEK_CUR As Long = 1
Public Const SEEK_END As Long = 2

' Indexes for byte arrays, masks and shifts for treating pixels as words
' These coincide with the order of RGBQUAD and RGBTRIPLE
' Little Endian (x86 / MS Windows, Linux) : BGR(A) order
Public Const FI_RGBA_RED As Long = 2
Public Const FI_RGBA_GREEN As Long = 1
Public Const FI_RGBA_BLUE As Long = 0
Public Const FI_RGBA_ALPHA As Long = 3
Public Const FI_RGBA_RED_MASK As Long = &HFF0000
Public Const FI_RGBA_GREEN_MASK As Long = &HFF00
Public Const FI_RGBA_BLUE_MASK As Long = &HFF
Public Const FI_RGBA_ALPHA_MASK As Long = &HFF000000
Public Const FI_RGBA_RED_SHIFT As Long = 16
Public Const FI_RGBA_GREEN_SHIFT As Long = 8
Public Const FI_RGBA_BLUE_SHIFT As Long = 0
Public Const FI_RGBA_ALPHA_SHIFT As Long = 24

' The 16 bit macros only include masks and shifts, since each color element is not byte aligned
Public Const FI16_555_RED_MASK As Long = &H7C00
Public Const FI16_555_GREEN_MASK As Long = &H3E0
Public Const FI16_555_BLUE_MASK As Long = &H1F
Public Const FI16_555_RED_SHIFT As Long = 10
Public Const FI16_555_GREEN_SHIFT As Long = 5
Public Const FI16_555_BLUE_SHIFT As Long = 0
Public Const FI16_565_RED_MASK As Long = &HF800
Public Const FI16_565_GREEN_MASK As Long = &H7E0
Public Const FI16_565_BLUE_MASK As Long = &H1F
Public Const FI16_565_RED_SHIFT As Long = 11
Public Const FI16_565_GREEN_SHIFT As Long = 5
Public Const FI16_565_BLUE_SHIFT As Long = 0

' ICC profile support
Public Const FIICC_DEFAULT As Long = &H0
Public Const FIICC_COLOR_IS_CMYK As Long = &H1

' Load / Save flag constants
Public Const BMP_DEFAULT As Long = 0
Public Const BMP_SAVE_RLE As Long = 1
Public Const CUT_DEFAULT As Long = 0
Public Const DDS_DEFAULT As Long = 0
Public Const GIF_DEFAULT As Long = 0
Public Const GIF_LOAD256 As Long = 1              ' Load the image as a 256 color image with ununsed palette entries, if it's 16 or 2 color
Public Const GIF_PLAYBACK As Long = 2             ''Play' the GIF to generate each frame (as 32bpp) instead of returning raw frame data when loading
Public Const HDR_DEFAULT As Long = 0
Public Const ICO_DEFAULT As Long = 0
Public Const ICO_MAKEALPHA As Long = 1            ' convert to 32bpp and create an alpha channel from the AND-mask when loading
Public Const IFF_DEFAULT As Long = 0
Public Const JPEG_DEFAULT As Long = 0
Public Const JPEG_FAST As Long = 1
Public Const JPEG_ACCURATE As Long = 2
Public Const JPEG_QUALITYSUPERB As Long = &H80
Public Const JPEG_QUALITYGOOD As Long = &H100
Public Const JPEG_QUALITYNORMAL As Long = &H200
Public Const JPEG_QUALITYAVERAGE As Long = &H400
Public Const JPEG_QUALITYBAD As Long = &H800
Public Const JPEG_CMYK As Long = &H1000           ' load separated CMYK "as is" (use 'OR' to combine with other flags)
Public Const KOALA_DEFAULT As Long = 0
Public Const LBM_DEFAULT As Long = 0
Public Const MNG_DEFAULT As Long = 0
Public Const PCD_DEFAULT As Long = 0
Public Const PCD_BASE As Long = 1                 ' load the bitmap sized 768 x 512
Public Const PCD_BASEDIV4 As Long = 2             ' load the bitmap sized 384 x 256
Public Const PCD_BASEDIV16 As Long = 3            ' load the bitmap sized 192 x 128
Public Const PCX_DEFAULT As Long = 0
Public Const PNG_DEFAULT As Long = 0
Public Const PNG_IGNOREGAMMA As Long = 1          ' avoid gamma correction
Public Const PNM_DEFAULT As Long = 0
Public Const PNM_SAVE_RAW As Long = 0             ' if set the writer saves in RAW format (i.e. P4, P5 or P6)
Public Const PNM_SAVE_ASCII As Long = 1           ' if set the writer saves in ASCII format (i.e. P1, P2 or P3)
Public Const PSD_DEFAULT As Long = 0
Public Const RAS_DEFAULT As Long = 0
Public Const TARGA_DEFAULT As Long = 0
Public Const TARGA_LOAD_RGB888 As Long = 1        ' if set the loader converts RGB555 and ARGB8888 -> RGB888
Public Const TIFF_DEFAULT As Long = 0
Public Const TIFF_CMYK As Long = &H1              ' reads/stores tags for separated CMYK (use 'OR' to combine with compression flags)
Public Const TIFF_PACKBITS As Long = &H100        ' save using PACKBITS compression
Public Const TIFF_DEFLATE As Long = &H200         ' save using DEFLATE compression (a.k.a. ZLIB compression)
Public Const TIFF_ADOBE_DEFLATE As Long = &H400   ' save using ADOBE DEFLATE compression
Public Const TIFF_NONE As Long = &H800            ' save without any compression
Public Const TIFF_CCITTFAX3 As Long = &H1000      ' save using CCITT Group 3 fax encoding
Public Const TIFF_CCITTFAX4 As Long = &H2000      ' save using CCITT Group 4 fax encoding
Public Const TIFF_LZW As Long = &H4000            ' save using LZW compression
Public Const TIFF_JPEG As Long = &H8000           ' save using JPEG compression
Public Const WBMP_DEFAULT As Long = 0
Public Const XBM_DEFAULT As Long = 0
Public Const XPM_DEFAULT As Long = 0

Public Enum FREE_IMAGE_FORMAT
   FIF_UNKNOWN = -1
   FIF_BMP = 0
   FIF_ICO = 1
   FIF_JPEG = 2
   FIF_JNG = 3
   FIF_KOALA = 4
   FIF_LBM = 5
   FIF_IFF = FIF_LBM
   FIF_MNG = 6
   FIF_PBM = 7
   FIF_PBMRAW = 8
   FIF_PCD = 9
   FIF_PCX = 10
   FIF_PGM = 11
   FIF_PGMRAW = 12
   FIF_PNG = 13
   FIF_PPM = 14
   FIF_PPMRAW = 15
   FIF_RAS = 16
   FIF_TARGA = 17
   FIF_TIFF = 18
   FIF_WBMP = 19
   FIF_PSD = 20
   FIF_CUT = 21
   FIF_XBM = 22
   FIF_XPM = 23
   FIF_DDS = 24
   FIF_GIF = 25
   FIF_HDR = 26
End Enum
#If False Then
   Const FIF_UNKNOWN = -1
   Const FIF_BMP = 0
   Const FIF_ICO = 1
   Const FIF_JPEG = 2
   Const FIF_JNG = 3
   Const FIF_KOALA = 4
   Const FIF_LBM = 5
   Const FIF_IFF = FIF_LBM
   Const FIF_MNG = 6
   Const FIF_PBM = 7
   Const FIF_PBMRAW = 8
   Const FIF_PCD = 9
   Const FIF_PCX = 10
   Const FIF_PGM = 11
   Const FIF_PGMRAW = 12
   Const FIF_PNG = 13
   Const FIF_PPM = 14
   Const FIF_PPMRAW = 15
   Const FIF_RAS = 16
   Const FIF_TARGA = 17
   Const FIF_TIFF = 18
   Const FIF_WBMP = 19
   Const FIF_PSD = 20
   Const FIF_CUT = 21
   Const FIF_XBM = 22
   Const FIF_XPM = 23
   Const FIF_DDS = 24
   Const FIF_GIF = 25
   Const FIF_HDR = 26
#End If

Public Enum FREE_IMAGE_SAVE_OPTIONS
   FISO_SAVE_DEFAULT = 0
   FISO_BMP_DEFAULT = BMP_DEFAULT
   FISO_BMP_SAVE_RLE = BMP_SAVE_RLE
   FISO_GIF_DEFAULT = GIF_DEFAULT
   FISO_HDR_DEFAULT = HDR_DEFAULT
   FISO_ICO_DEFAULT = ICO_DEFAULT
   FISO_JPEG_DEFAULT = JPEG_DEFAULT
   FISO_JPEG_QUALITYSUPERB = JPEG_QUALITYSUPERB
   FISO_JPEG_QUALITYGOOD = JPEG_QUALITYGOOD
   FISO_JPEG_QUALITYNORMAL = JPEG_QUALITYNORMAL
   FISO_JPEG_QUALITYAVERAGE = JPEG_QUALITYAVERAGE
   FISO_JPEG_QUALITYBAD = JPEG_QUALITYBAD
   FISO_PNG_DEFAULT = PNG_DEFAULT
   FISO_PNM_DEFAULT = PNM_DEFAULT
   FISO_PNM_SAVE_RAW = PNM_SAVE_RAW              ' if set the writer saves in RAW format (i.e. P4, P5 or P6)
   FISO_PNM_SAVE_ASCII = PNM_SAVE_ASCII          ' if set the writer saves in ASCII format (i.e. P1, P2 or P3)
   FISO_TARGA_DEFAULT = TARGA_DEFAULT
   FISO_TIFF_DEFAULT = TIFF_DEFAULT
   FISO_TIFF_CMYK = TIFF_CMYK                    ' reads/stores tags for separated CMYK (use 'OR' to combine with compression flags)
   FISO_TIFF_PACKBITS = TIFF_PACKBITS            ' save using PACKBITS compression
   FISO_TIFF_DEFLATE = TIFF_DEFLATE              ' save using DEFLATE compression (a.k.a. ZLIB compression)
   FISO_TIFF_ADOBE_DEFLATE = TIFF_ADOBE_DEFLATE  ' save using ADOBE DEFLATE compression
   FISO_TIFF_NONE = TIFF_NONE                    ' save without any compression
   FISO_TIFF_CCITTFAX3 = TIFF_CCITTFAX3          ' save using CCITT Group 3 fax encoding
   FISO_TIFF_CCITTFAX4 = TIFF_CCITTFAX4          ' save using CCITT Group 4 fax encoding
   FISO_TIFF_LZW = TIFF_LZW                      ' save using LZW compression
   FISO_TIFF_JPEG = TIFF_JPEG                    ' save using JPEG compression
   FISO_WBMP_DEFAULT = WBMP_DEFAULT
   FISO_XPM_DEFAULT = XPM_DEFAULT
End Enum
#If False Then
   Const FISO_SAVE_DEFAULT = 0
   Const FISO_BMP_DEFAULT = BMP_DEFAULT
   Const FISO_BMP_SAVE_RLE = BMP_SAVE_RLE
   Const FISO_GIF_DEFAULT = GIF_DEFAULT
   Const FISO_HDR_DEFAULT = HDR_DEFAULT
   Const FISO_ICO_DEFAULT = ICO_DEFAULT
   Const FISO_JPEG_DEFAULT = JPEG_DEFAULT
   Const FISO_JPEG_QUALITYSUPERB = JPEG_QUALITYSUPERB
   Const FISO_JPEG_QUALITYGOOD = JPEG_QUALITYGOOD
   Const FISO_JPEG_QUALITYNORMAL = JPEG_QUALITYNORMAL
   Const FISO_JPEG_QUALITYAVERAGE = JPEG_QUALITYAVERAGE
   Const FISO_JPEG_QUALITYBAD = JPEG_QUALITYBAD
   Const FISO_PNG_DEFAULT = PNG_DEFAULT
   Const FISO_PNM_DEFAULT = PNM_DEFAULT
   Const FISO_PNM_SAVE_RAW = PNM_SAVE_RAW
   Const FISO_PNM_SAVE_ASCII = PNM_SAVE_ASCII
   Const FISO_TARGA_DEFAULT = TARGA_DEFAULT
   Const FISO_TIFF_DEFAULT = TIFF_DEFAULT
   Const FISO_TIFF_CMYK = TIFF_CMYK
   Const FISO_TIFF_PACKBITS = TIFF_PACKBITS
   Const FISO_TIFF_DEFLATE = TIFF_DEFLATE
   Const FISO_TIFF_ADOBE_DEFLATE = TIFF_ADOBE_DEFLATE
   Const FISO_TIFF_NONE = TIFF_NONE
   Const FISO_TIFF_CCITTFAX3 = TIFF_CCITTFAX3
   Const FISO_TIFF_CCITTFAX4 = TIFF_CCITTFAX4
   Const FISO_TIFF_LZW = TIFF_LZW
   Const FISO_TIFF_JPEG = TIFF_JPEG
   Const FISO_WBMP_DEFAULT = WBMP_DEFAULT
   Const FISO_XPM_DEFAULT = XPM_DEFAULT
#End If

Public Enum FREE_IMAGE_TYPE
   FIT_UNKNOWN = 0           ' unknown type
   FIT_BITMAP = 1            ' standard image           : 1-, 4-, 8-, 16-, 24-, 32-bit
   FIT_UINT16 = 2            ' array of unsigned short  : unsigned 16-bit
   FIT_INT16 = 3             ' array of short           : signed 16-bit
   FIT_UINT32 = 4            ' array of unsigned long   : unsigned 32-bit
   FIT_INT32 = 5             ' array of long            : signed 32-bit
   FIT_FLOAT = 6             ' array of float           : 32-bit IEEE floating point
   FIT_DOUBLE = 7            ' array of double          : 64-bit IEEE floating point
   FIT_COMPLEX = 8           ' array of FICOMPLEX       : 2 x 64-bit IEEE floating point
   FIT_RGB16 = 9             ' 48-bit RGB image         : 3 x 16-bit
   FIT_RGBA16 = 10           ' 64-bit RGBA image        : 4 x 16-bit
   FIT_RGBF = 11             ' 96-bit RGB float image   : 3 x 32-bit IEEE floating point
   FIT_RGBAF = 12            ' 128-bit RGBA float image : 4 x 32-bit IEEE floating point
End Enum
#If False Then
   Const FIT_UNKNOWN = 0
   Const FIT_BITMAP = 1
   Const FIT_UINT16 = 2
   Const FIT_INT16 = 3
   Const FIT_UINT32 = 4
   Const FIT_INT32 = 5
   Const FIT_FLOAT = 6
   Const FIT_DOUBLE = 7
   Const FIT_COMPLEX = 8
   Const FIT_RGB16 = 9
   Const FIT_RGBA16 = 10
   Const FIT_RGBF = 11
   Const FIT_RGBAF = 12
#End If

Public Enum FREE_IMAGE_COLOR_TYPE
   FIC_MINISWHITE = 0        ' min value is white
   FIC_MINISBLACK = 1        ' min value is black
   FIC_RGB = 2               ' RGB color model
   FIC_PALETTE = 3           ' color map indexed
   FIC_RGBALPHA = 4          ' RGB color model with alpha channel
   FIC_CMYK = 5              ' CMYK color model
End Enum
#If False Then
   Const FIC_MINISWHITE = 0
   Const FIC_MINISBLACK = 1
   Const FIC_RGB = 2
   Const FIC_PALETTE = 3
   Const FIC_RGBALPHA = 4
   Const FIC_CMYK = 5
#End If

Public Enum FREE_IMAGE_QUANTIZE
   FIQ_WUQUANT = 0           ' Xiaolin Wu color quantization algorithm
   FIQ_NNQUANT = 1           ' NeuQuant neural-net quantization algorithm by Anthony Dekker
End Enum
#If False Then
   Const FIQ_WUQUANT = 0
   Const FIQ_NNQUANT = 1
#End If

Public Enum FREE_IMAGE_DITHER
   FID_FS = 0                ' Floyd & Steinberg error diffusion
   FID_BAYER4x4 = 1          ' Bayer ordered dispersed dot dithering (order 2 dithering matrix)
   FID_BAYER8x8 = 2          ' Bayer ordered dispersed dot dithering (order 3 dithering matrix)
   FID_CLUSTER6x6 = 3        ' Ordered clustered dot dithering (order 3 - 6x6 matrix)
   FID_CLUSTER8x8 = 4        ' Ordered clustered dot dithering (order 4 - 8x8 matrix)
   FID_CLUSTER16x16 = 5      ' Ordered clustered dot dithering (order 8 - 16x16 matrix)
End Enum
#If False Then
   Const FID_FS = 0
   Const FID_BAYER4x4 = 1
   Const FID_BAYER8x8 = 2
   Const FID_CLUSTER6x6 = 3
   Const FID_CLUSTER8x8 = 4
   Const FID_CLUSTER16x16 = 5
#End If

Public Enum FREE_IMAGE_JPEG_OPERATION
   FIJPEG_OP_NONE = 0        ' no transformation
   FIJPEG_OP_FLIP_H = 1      ' horizontal flip
   FIJPEG_OP_FLIP_V = 2      ' vertical flip
   FIJPEG_OP_TRANSPOSE = 3   ' transpose across UL-to-LR axis
   FIJPEG_OP_TRANSVERSE = 4  ' transpose across UR-to-LL axis
   FIJPEG_OP_ROTATE_90 = 5   ' 90-degree clockwise rotation
   FIJPEG_OP_ROTATE_180 = 6  ' 180-degree rotation
   FIJPEG_OP_ROTATE_270 = 7  ' 270-degree clockwise (or 90 ccw)
End Enum
#If False Then
   Const FIJPEG_OP_NONE = 0
   Const FIJPEG_OP_FLIP_H = 1
   Const FIJPEG_OP_FLIP_V = 2
   Const FIJPEG_OP_TRANSPOSE = 3
   Const FIJPEG_OP_TRANSVERSE = 4
   Const FIJPEG_OP_ROTATE_90 = 5
   Const FIJPEG_OP_ROTATE_180 = 6
   Const FIJPEG_OP_ROTATE_270 = 7
#End If

Public Enum FREE_IMAGE_TMO
   FITMO_DRAGO03 = 0         ' Adaptive logarithmic mapping (F. Drago, 2003)
   FITMO_REINHARD05 = 1      ' Dynamic range reduction inspired by photoreceptor physiology (E. Reinhard, 2005)
End Enum
#If False Then
   Const FITMO_DRAGO03 = 0
   Const FITMO_REINHARD05 = 1
#End If

Public Enum FREE_IMAGE_FILTER
   FILTER_BOX = 0            ' Box, pulse, Fourier window, 1st order (constant) b-spline
   FILTER_BICUBIC = 1        ' Mitchell & Netravali's two-param cubic filter
   FILTER_BILINEAR = 2       ' Bilinear filter
   FILTER_BSPLINE = 3        ' 4th order (cubic) b-spline
   FILTER_CATMULLROM = 4     ' Catmull-Rom spline, Overhauser spline
   FILTER_LANCZOS3 = 5       ' Lanczos3 filter
End Enum
#If False Then
   Const FILTER_BOX = 0
   Const FILTER_BICUBIC = 1
   Const FILTER_BILINEAR = 2
   Const FILTER_BSPLINE = 3
   Const FILTER_CATMULLROM = 4
   Const FILTER_LANCZOS3 = 5
#End If

Public Enum FREE_IMAGE_COLOR_CHANNEL
   FICC_RGB = 0              ' Use red, green and blue channels
   FICC_RED = 1              ' Use red channel
   FICC_GREEN = 2            ' Use green channel
   FICC_BLUE = 3             ' Use blue channel
   FICC_ALPHA = 4            ' Use alpha channel
   FICC_BLACK = 5            ' Use black channel
   FICC_REAL = 6             ' Complex images: use real part
   FICC_IMAG = 7             ' Complex images: use imaginary part
   FICC_MAG = 8              ' Complex images: use magnitude
   FICC_PHASE = 9            ' Complex images: use phase
End Enum
#If False Then
   Const FICC_RGB = 0
   Const FICC_RED = 1
   Const FICC_GREEN = 2
   Const FICC_BLUE = 3
   Const FICC_ALPHA = 4
   Const FICC_BLACK = 5
   Const FICC_REAL = 6
   Const FICC_IMAG = 7
   Const FICC_MAG = 8
   Const FICC_PHASE = 9
#End If

Public Enum FREE_IMAGE_MDTYPE
   FIDT_NOTYPE = 0           ' placeholder
   FIDT_BYTE = 1             ' 8-bit unsigned integer
   FIDT_ASCII = 2            ' 8-bit bytes w/ last byte null
   FIDT_SHORT = 3            ' 16-bit unsigned integer
   FIDT_LONG = 4             ' 32-bit unsigned integer
   FIDT_RATIONAL = 5         ' 64-bit unsigned fraction
   FIDT_SBYTE = 6            ' 8-bit signed integer
   FIDT_UNDEFINED = 7        ' 8-bit untyped data
   FIDT_SSHORT = 8           ' 16-bit signed integer
   FIDT_SLONG = 9            ' 32-bit signed integer
   FIDT_SRATIONAL = 10       ' 64-bit signed fraction
   FIDT_FLOAT = 11           ' 32-bit IEEE floating point
   FIDT_DOUBLE = 12          ' 64-bit IEEE floating point
   FIDT_IFD = 13             ' 32-bit unsigned integer (offset)
   FIDT_PALETTE = 14         ' 32-bit RGBQUAD
End Enum
#If False Then
   Const FIDT_NOTYPE = 0
   Const FIDT_BYTE = 1
   Const FIDT_ASCII = 2
   Const FIDT_SHORT = 3
   Const FIDT_LONG = 4
   Const FIDT_RATIONAL = 5
   Const FIDT_SBYTE = 6
   Const FIDT_UNDEFINED = 7
   Const FIDT_SSHORT = 8
   Const FIDT_SLONG = 9
   Const FIDT_SRATIONAL = 10
   Const FIDT_FLOAT = 11
   Const FIDT_DOUBLE = 12
   Const FIDT_IFD = 13
   Const FIDT_PALETTE = 14
#End If

Public Enum FREE_IMAGE_MDMODEL
   FIMD_NODATA = -1          '
   FIMD_COMMENTS = 0         ' single comment or keywords
   FIMD_EXIF_MAIN = 1        ' Exif-TIFF metadata
   FIMD_EXIF_EXIF = 2        ' Exif-specific metadata
   FIMD_EXIF_GPS = 3         ' Exif GPS metadata
   FIMD_EXIF_MAKERNOTE = 4   ' Exif maker note metadata
   FIMD_EXIF_INTEROP = 5     ' Exif interoperability metadata
   FIMD_IPTC = 6             ' IPTC/NAA metadata
   FIMD_XMP = 7              ' Abobe XMP metadata
   FIMD_GEOTIFF = 8          ' GeoTIFF metadata
   FIMD_ANIMATION = 9        ' Animation metadata
   FIMD_CUSTOM = 10          ' Used to attach other metadata types to a dib
End Enum
#If False Then
   FIMD_NODATA = -1
   FIMD_COMMENTS = 0
   FIMD_EXIF_MAIN = 1
   FIMD_EXIF_EXIF = 2
   FIMD_EXIF_GPS = 3
   FIMD_EXIF_MAKERNOTE = 4
   FIMD_EXIF_INTEROP = 5
   FIMD_IPTC = 6
   FIMD_XMP = 7
   FIMD_GEOTIFF = 8
   FIMD_ANIMATION = 9
   FIMD_CUSTOM = 10
#End If

' the next two enums are only used by derived functions of the
' FreeImage 3 VB wrapper
Public Enum FREE_IMAGE_CONVERSION_FLAGS
   FICF_MONOCHROME = &H1
   FICF_MONOCHROME_THRESHOLD = FICF_MONOCHROME
   FICF_MONOCHROME_DITHER = &H3
   FICF_GREYSCALE_4BPP = &H4
   FICF_PALETTISED_8BPP = &H8
   FICF_GREYSCALE_8BPP = FICF_PALETTISED_8BPP Or FICF_MONOCHROME
   FICF_GREYSCALE = FICF_GREYSCALE_8BPP
   FICF_RGB_15BPP = &HF
   FICF_RGB_16BPP = &H10
   FICF_RGB_24BPP = &H18
   FICF_RGB_32BPP = &H20
   FICF_RGB_ALPHA = FICF_RGB_32BPP
   FICF_PREPARE_RESCALE = &H100
   FICF_KEEP_UNORDERED_GREYSCALE_PALETTE = &H0
   FICF_REORDER_GREYSCALE_PALETTE = &H1000
End Enum
#If False Then
   Const FICF_MONOCHROME = &H1
   Const FICF_MONOCHROME_THRESHOLD = FICF_MONOCHROME
   Const FICF_MONOCHROME_DITHER = &H3
   Const FICF_GREYSCALE_4BPP = &H4
   Const FICF_PALETTISED_8BPP = &H8
   Const FICF_GREYSCALE_8BPP = FICF_PALETTISED_8BPP Or FICF_MONOCHROME
   Const FICF_GREYSCALE = FICF_GREYSCALE_8BPP
   Const FICF_RGB_15BPP = &HF
   Const FICF_RGB_16BPP = &H10
   Const FICF_RGB_24BPP = &H18
   Const FICF_RGB_32BPP = &H20
   Const FICF_RGB_ALPHA = FICF_RGB_32BPP
   Const FICF_PREPARE_RESCALE = &H100
   Const FICF_KEEP_UNORDERED_GREYSCALE_PALETTE = &H0
   Const FICF_REORDER_GREYSCALE_PALETTE = &H1000
#End If

Public Enum FREE_IMAGE_ADJUST_MODE
   AM_STRECH = &H1
   AM_DEFAULT = AM_STRECH
   AM_ADJUST_BOTH = AM_STRECH
   AM_ADJUST_WIDTH = &H2
   AM_ADJUST_HEIGHT = &H4
   AM_ADJUST_OPTIMAL_SIZE = &H8
End Enum
#If False Then
   Const AM_STRECH = &H1
   Const AM_DEFAULT = AM_STRECH
   Const AM_ADJUST_BOTH = AM_STRECH
   Const AM_ADJUST_WIDTH = &H2
   Const AM_ADJUST_HEIGHT = &H4
   Const AM_ADJUST_OPTIMAL_SIZE = &H8
#End If


Public Type RGBQUAD
   rgbBlue As Byte
   rgbGreen As Byte
   rgbRed As Byte
   rgbReserved As Byte
End Type

Public Type RGBTRIPLE
   rgbtBlue As Byte
   rgbtGreen As Byte
   rgbtRed As Byte
End Type

Public Type BITMAPINFOHEADER
   biSize As Long
   biWidth As Long
   biHeight As Long
   biPlanes As Integer
   biBitCount As Integer
   biCompression As Long
   biSizeImage As Long
   biXPelsPerMeter As Long
   biYPelsPerMeter As Long
   biClrUsed As Long
   biClrImportant As Long
End Type

Public Type BITMAPINFO
   bmiHeader As BITMAPINFOHEADER
   bmiColors(0) As RGBQUAD
End Type

Public Type FIICCPROFILE
   flags As Integer
   size As Long
   data As Long
End Type

Public Type FIRGB16
   red As Integer
   green As Integer
   blue As Integer
End Type

Public Type FIRGBA16
   red As Integer
   green As Integer
   blue As Integer
   alpha As Integer
End Type

Public Type FIRGBF
   red As Single
   green As Single
   blue As Single
End Type

Public Type FIRGBAF
   red As Single
   green As Single
   blue As Single
   alpha As Single
End Type

Public Type FICOMPLEX
   r As Double           ' real part
   i As Double           ' imaginary part
End Type

Public Type FITAG_int
   key As Long
   description As Long
   id As Integer
   type As Integer
   Count As Long
   Length As Long
   Value As Long
End Type

Public Type FITAG
   key As String
   description As String
   id As Integer
   type As FREE_IMAGE_MDTYPE
   Count As Long
   Length As Long
   Value As Variant
End Type


Public Type FreeImageIO
   read_proc As Long
   write_proc As Long
   seek_proc As Long
   tell_proc As Long
End Type

Public Type Plugin
   format_proc As Long
   description_proc As Long
   extension_proc As Long
   regexpr_proc As Long
   open_proc As Long
   close_proc As Long
   pagecount_proc As Long
   pagecapability_proc As Long
   load_proc As Long
   save_proc As Long
   validate_proc As Long
   mime_proc As Long
   supports_export_bpp_proc As Long
   supports_export_type_proc As Long
   supports_icc_profiles_proc As Long
End Type



'--------------------------------------------------------------------------------
' FreeImage 3 function declarations
'--------------------------------------------------------------------------------

' The FreeImage 3 functions are declared in the same order as they are described
' in the FreeImage 3 API documentation. The documentation's outline is included
' as comments.

'--------------------------------------------------------------------------------
' Bitmap functions
'--------------------------------------------------------------------------------

' General functions (p. 3 to 4)
Public Declare Sub FreeImage_Initialise Lib "FreeImage.dll" Alias "_FreeImage_Initialise@4" ( _
  Optional ByVal load_local_plugins_only As Long = 0)

Public Declare Sub FreeImage_DeInitialise Lib "FreeImage.dll" Alias "_FreeImage_DeInitialise@0" ()

Private Declare Function FreeImage_GetVersionInt Lib "FreeImage.dll" Alias "_FreeImage_GetVersion@0" () As Long

Private Declare Function FreeImage_GetCopyrightMessageInt Lib "FreeImage.dll" Alias "_FreeImage_GetCopyrightMessage@0" () As Long

Public Declare Sub FreeImage_SetOutputMessage Lib "FreeImage.dll" Alias "_FreeImage_SetOutputMessage@4" ( _
           ByVal omf As Long)


' Bitmap management functions (p. 5 to 11)
Public Declare Function FreeImage_Allocate Lib "FreeImage.dll" Alias "_FreeImage_Allocate@24" ( _
           ByVal Width As Long, _
           ByVal Height As Long, _
           ByVal bpp As Long, _
  Optional ByVal red_mask As Long = 0, _
  Optional ByVal green_mask As Long = 0, _
  Optional ByVal blue_mask As Long = 0) As Long

Public Declare Function FreeImage_AllocateT Lib "FreeImage.dll" Alias "_FreeImage_AllocateT@28" ( _
           ByVal type_ As FREE_IMAGE_TYPE, _
           ByVal Width As Long, _
           ByVal Height As Long, _
  Optional ByVal bpp As Long = 8, _
  Optional ByVal red_mask As Long = 0, _
  Optional ByVal green_mask As Long = 0, _
  Optional ByVal blue_mask As Long = 0) As Long

Public Declare Function FreeImage_Load Lib "FreeImage.dll" Alias "_FreeImage_Load@12" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal FileName As String, _
  Optional ByVal flags As Long = 0) As Long
  
Public Declare Function FreeImage_LoadUInt Lib "FreeImage.dll" Alias "_FreeImage_LoadU@12" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal FileName As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_LoadFromHandle Lib "FreeImage.dll" Alias "_FreeImage_LoadFromHandle@16" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal io As Long, _
           ByVal Handle As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_Save Lib "FreeImage.dll" Alias "_FreeImage_Save@16" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal dib As Long, _
           ByVal FileName As String, _
  Optional ByVal flags As Long = 0) As Long
  
Public Declare Function FreeImage_SaveUInt Lib "FreeImage.dll" Alias "_FreeImage_SaveU@16" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal dib As Long, _
           ByVal FileName As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_SaveToHandle Lib "FreeImage.dll" Alias "_FreeImage_SaveToHandle@20" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal dib As Long, _
           ByVal io As Long, _
           ByVal Handle As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_Clone Lib "FreeImage.dll" Alias "_FreeImage_Clone@4" ( _
           ByVal dib As Long) As Long

Public Declare Sub FreeImage_Unload Lib "FreeImage.dll" Alias "_FreeImage_Unload@4" ( _
           ByVal dib As Long)


' Bitmap information functions (p. 12 to 18)
Public Declare Function FreeImage_GetImageType Lib "FreeImage.dll" Alias "_FreeImage_GetImageType@4" ( _
           ByVal dib As Long) As FREE_IMAGE_TYPE

Public Declare Function FreeImage_GetColorsUsed Lib "FreeImage.dll" Alias "_FreeImage_GetColorsUsed@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetBPP Lib "FreeImage.dll" Alias "_FreeImage_GetBPP@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetWidth Lib "FreeImage.dll" Alias "_FreeImage_GetWidth@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetHeight Lib "FreeImage.dll" Alias "_FreeImage_GetHeight@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetLine Lib "FreeImage.dll" Alias "_FreeImage_GetLine@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetPitch Lib "FreeImage.dll" Alias "_FreeImage_GetPitch@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetDIBSize Lib "FreeImage.dll" Alias "_FreeImage_GetDIBSize@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetPalette Lib "FreeImage.dll" Alias "_FreeImage_GetPalette@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetDotsPerMeterX Lib "FreeImage.dll" Alias "_FreeImage_GetDotsPerMeterX@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetDotsPerMeterY Lib "FreeImage.dll" Alias "_FreeImage_GetDotsPerMeterY@4" ( _
           ByVal dib As Long) As Long

Public Declare Sub FreeImage_SetDotsPerMeterX Lib "FreeImage.dll" Alias "_FreeImage_SetDotsPerMeterX@8" ( _
           ByVal dib As Long, _
           ByVal res As Long)

Public Declare Sub FreeImage_SetDotsPerMeterY Lib "FreeImage.dll" Alias "_FreeImage_SetDotsPerMeterY@8" ( _
           ByVal dib As Long, _
           ByVal res As Long)

Public Declare Function FreeImage_GetInfoHeader Lib "FreeImage.dll" Alias "_FreeImage_GetInfoHeader@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetInfo Lib "FreeImage.dll" Alias "_FreeImage_GetInfo@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetColorType Lib "FreeImage.dll" Alias "_FreeImage_GetColorType@4" ( _
           ByVal dib As Long) As FREE_IMAGE_COLOR_TYPE

Public Declare Function FreeImage_GetRedMask Lib "FreeImage.dll" Alias "_FreeImage_GetRedMask@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetGreenMask Lib "FreeImage.dll" Alias "_FreeImage_GetGreenMask@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetBlueMask Lib "FreeImage.dll" Alias "_FreeImage_GetBlueMask@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetTransparencyCount Lib "FreeImage.dll" Alias "_FreeImage_GetTransparencyCount@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetTransparencyTable Lib "FreeImage.dll" Alias "_FreeImage_GetTransparencyTable@4" ( _
           ByVal dib As Long) As Long

Public Declare Sub FreeImage_SetTransparencyTable Lib "FreeImage.dll" Alias "_FreeImage_SetTransparencyTable@12" ( _
           ByVal dib As Long, _
           ByVal table As Long, _
           ByVal Count As Long)

Public Declare Sub FreeImage_SetTransparent Lib "FreeImage.dll" Alias "_FreeImage_SetTransparent@8" ( _
           ByVal dib As Long, _
           ByVal enabled As Long)

Public Declare Function FreeImage_IsTransparent Lib "FreeImage.dll" Alias "_FreeImage_IsTransparent@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_HasBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_HasBackgroundColor@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_GetBackgroundColor@8" ( _
           ByVal dib As Long, _
           ByVal bkcolor As Long) As Long

Public Declare Function FreeImage_SetBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_SetBackgroundColor@8" ( _
           ByVal dib As Long, _
           ByVal bkcolor As Long) As Long


' Filetype functions (p. 19 to 20)
Public Declare Function FreeImage_GetFileType Lib "FreeImage.dll" Alias "_FreeImage_GetFileType@8" ( _
           ByVal FileName As String, _
  Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT
  
Public Declare Function FreeImage_GetFileTypeUInt Lib "FreeImage.dll" Alias "_FreeImage_GetFileTypeU@8" ( _
           ByVal FileName As Long, _
  Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT

Public Declare Function FreeImage_GetFileTypeFromHandle Lib "FreeImage.dll" Alias "_FreeImage_GetFileTypeFromHandle@12" ( _
           ByVal io As Long, _
           ByVal Handle As Long, _
  Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT

Public Declare Function FreeImage_GetFileTypeFromMemory Lib "FreeImage.dll" Alias "_FreeImage_GetFileTypeFromMemory@8" ( _
           ByVal stream As Long, _
  Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT


' Pixel access functions (p. 21 to 26)
Public Declare Function FreeImage_GetBits Lib "FreeImage.dll" Alias "_FreeImage_GetBits@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_GetScanline Lib "FreeImage.dll" Alias "_FreeImage_GetScanLine@8" ( _
           ByVal dib As Long, _
           ByVal scanline As Long) As Long

Public Declare Function FreeImage_GetPixelIndex Lib "FreeImage.dll" Alias "_FreeImage_GetPixelIndex@16" ( _
           ByVal dib As Long, _
           ByVal x As Long, _
           ByVal y As Long, _
           ByRef Value As Byte) As Long

Public Declare Function FreeImage_GetPixelColor Lib "FreeImage.dll" Alias "_FreeImage_GetPixelColor@16" ( _
           ByVal dib As Long, _
           ByVal x As Long, _
           ByVal y As Long, _
           ByRef Value As RGBQUAD) As Long

Public Declare Function FreeImage_SetPixelIndex Lib "FreeImage.dll" Alias "_FreeImage_SetPixelIndex@16" ( _
           ByVal dib As Long, _
           ByVal x As Long, _
           ByVal y As Long, _
           ByRef Value As Byte) As Long

Public Declare Function FreeImage_SetPixelColor Lib "FreeImage.dll" Alias "_FreeImage_SetPixelColor@16" ( _
           ByVal dib As Long, _
           ByVal x As Long, _
           ByVal y As Long, _
           ByRef Value As RGBQUAD) As Long


' Conversion functions (p. 27 to 32)
Public Declare Function FreeImage_ConvertTo4Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo4Bits@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ConvertTo8Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo8Bits@4" ( _
           ByVal dib As Long) As Long
           
Public Declare Function FreeImage_ConvertToGreyscale Lib "FreeImage.dll" Alias "_FreeImage_ConvertToGreyscale@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ConvertTo16Bits555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo16Bits555@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ConvertTo16Bits565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo16Bits565@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ConvertTo24Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo24Bits@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ConvertTo32Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo32Bits@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_ColorQuantize Lib "FreeImage.dll" Alias "_FreeImage_ColorQuantize@8" ( _
           ByVal dib As Long, _
           ByVal quantize As FREE_IMAGE_QUANTIZE) As Long
           
Public Declare Function FreeImage_ColorQuantizeEx Lib "FreeImage.dll" Alias "_FreeImage_ColorQuantizeEx@20" ( _
           ByVal dib As Long, _
  Optional ByVal quantize As FREE_IMAGE_QUANTIZE = FIQ_WUQUANT, _
  Optional ByVal PaletteSize As Long = 256, _
  Optional ByVal ReserveSize As Long = 0, _
  Optional ByVal ReservePalette As Long = 0) As Long

Public Declare Function FreeImage_Threshold Lib "FreeImage.dll" Alias "_FreeImage_Threshold@8" ( _
           ByVal dib As Long, _
           ByVal T As Byte) As Long

Public Declare Function FreeImage_Dither Lib "FreeImage.dll" Alias "_FreeImage_Dither@8" ( _
           ByVal dib As Long, _
           ByVal algorithm As FREE_IMAGE_DITHER) As Long

Public Declare Function FreeImage_ConvertFromRawBits Lib "FreeImage.dll" Alias "_FreeImage_ConvertFromRawBits@36" ( _
           ByRef Bits As Long, _
           ByVal Width As Long, _
           ByVal Height As Long, _
           ByVal pitch As Long, _
           ByVal bpp As Long, _
           ByVal red_mask As Long, _
           ByVal green_mask As Long, _
           ByVal blue_mask As Long, _
  Optional ByVal topdown As Long = 0) As Long

Public Declare Sub FreeImage_ConvertToRawBits Lib "FreeImage.dll" Alias "_FreeImage_ConvertToRawBits@32" ( _
           ByRef Bits As Long, _
           ByVal dib As Long, _
           ByVal pitch As Long, _
           ByVal bpp As Long, _
           ByVal red_mask As Long, _
           ByVal green_mask As Long, _
           ByVal blue_mask As Long, _
  Optional ByVal topdown As Long = 0)

Public Declare Function FreeImage_ConvertToStandardType Lib "FreeImage.dll" Alias "_FreeImage_ConvertToStandardType@8" ( _
           ByVal src As Long, _
  Optional ByVal scale_linear As Long = 1) As Long

Public Declare Function FreeImage_ConvertToType Lib "FreeImage.dll" Alias "_FreeImage_ConvertToType@12" ( _
           ByVal src As Long, _
           ByVal dst_type As FREE_IMAGE_TYPE, _
  Optional ByVal scale_linear As Long = 1) As Long

Public Declare Function FreeImage_ConvertToRGBF Lib "FreeImage.dll" Alias "_FreeImage_ConvertToRGBF@4" ( _
           ByVal dib As Long) As Long


' Tone mapping operators (p. 33 to 34)
Public Declare Function FreeImage_ToneMapping Lib "FreeImage.dll" Alias "_FreeImage_ToneMapping@24" ( _
           ByVal dib As Long, _
           ByVal tmo As FREE_IMAGE_TMO, _
  Optional ByVal first_param As Double = 0, _
  Optional ByVal second_param As Double = 0) As Long
  
Public Declare Function FreeImage_TmoDrago03 Lib "FreeImage.dll" Alias "_FreeImage_TmoDrago03@20" ( _
           ByVal src As Long, _
  Optional ByVal gamma As Double = 2.2, _
  Optional ByVal exposure As Double = 0) As Long
  
Public Declare Function FreeImage_TmoReinhard05 Lib "FreeImage.dll" Alias "_FreeImage_TmoReinhard05@20" ( _
           ByVal src As Long, _
  Optional ByVal intensity As Double = 0, _
  Optional ByVal contrast As Double = 0) As Long


' ICC profile functions (p. 35 to 36)
Public Declare Function FreeImage_GetICCProfile Lib "FreeImage.dll" Alias "_FreeImage_GetICCProfile@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_CreateICCProfile Lib "FreeImage.dll" Alias "_FreeImage_CreateICCProfile@12" ( _
           ByVal dib As Long, _
           ByRef data As Long, _
           ByVal size As Long) As Long

Public Declare Sub FreeImage_DestroyICCProfile Lib "FreeImage.dll" Alias "_FreeImage_DestroyICCProfile@4" ( _
           ByVal dib As Long)


' Plugin functions (p. 37 to 43)
Public Declare Function FreeImage_GetFIFCount Lib "FreeImage.dll" Alias "_FreeImage_GetFIFCount@0" () As Long

Public Declare Function FreeImage_SetPluginEnabled Lib "FreeImage.dll" Alias "_FreeImage_SetPluginEnabled@8" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal enable As Long) As Long

Public Declare Function FreeImage_IsPluginEnabled Lib "FreeImage.dll" Alias "_FreeImage_IsPluginEnabled@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Public Declare Function FreeImage_GetFIFFromFormat Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromFormat@4" ( _
           ByVal Format As String) As FREE_IMAGE_FORMAT

Public Declare Function FreeImage_GetFIFFromMime Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromMime@4" ( _
           ByVal mime As String) As FREE_IMAGE_FORMAT

Private Declare Function FreeImage_GetFIFMimeTypeInt Lib "FreeImage.dll" Alias "_FreeImage_GetFIFMimeType@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Private Declare Function FreeImage_GetFormatFromFIFInt Lib "FreeImage.dll" Alias "_FreeImage_GetFormatFromFIF@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Private Declare Function FreeImage_GetFIFExtensionListInt Lib "FreeImage.dll" Alias "_FreeImage_GetFIFExtensionList@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Private Declare Function FreeImage_GetFIFDescriptionInt Lib "FreeImage.dll" Alias "_FreeImage_GetFIFDescription@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Private Declare Function FreeImage_GetFIFRegExprInt Lib "FreeImage.dll" Alias "_FreeImage_GetFIFRegExpr@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Public Declare Function FreeImage_GetFIFFromFilename Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromFilename@4" ( _
           ByVal FileName As String) As FREE_IMAGE_FORMAT
           
Public Declare Function FreeImage_GetFIFFromFilenameUInt Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromFilenameU@4" ( _
           ByVal FileName As Long) As FREE_IMAGE_FORMAT

Public Declare Function FreeImage_FIFSupportsReading Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsReading@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Public Declare Function FreeImage_FIFSupportsWriting Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsWriting@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Public Declare Function FreeImage_FIFSupportsExportType Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsExportType@8" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal type_ As FREE_IMAGE_TYPE) As Long

Public Declare Function FreeImage_FIFSupportsExportBPP Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsExportBPP@8" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal bpp As Long) As Long

Public Declare Function FreeImage_FIFSupportsICCProfiles Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsICCProfiles@4" ( _
           ByVal fif As FREE_IMAGE_FORMAT) As Long

Public Declare Function FreeImage_RegisterLocalPlugin Lib "FreeImage.dll" Alias "_FreeImage_RegisterLocalPlugin@20" ( _
           ByVal proc_address As Long, _
  Optional ByVal Format As String = 0, _
  Optional ByVal description As String = 0, _
  Optional ByVal extension As String = 0, _
  Optional ByVal regexpr As String = 0) As FREE_IMAGE_FORMAT

Public Declare Function FreeImage_RegisterExternalPlugin Lib "FreeImage.dll" Alias "_FreeImage_RegisterExternalPlugin@20" ( _
           ByVal path As String, _
  Optional ByVal Format As String = 0, _
  Optional ByVal description As String = 0, _
  Optional ByVal extension As String = 0, _
  Optional ByVal regexpr As String = 0) As FREE_IMAGE_FORMAT


' Multipage functions (p. 44 to 45)
Public Declare Function FreeImage_OpenMultiBitmap Lib "FreeImage.dll" Alias "_FreeImage_OpenMultiBitmap@24" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal FileName As String, _
           ByVal create_new As Long, _
           ByVal read_only As Long, _
  Optional ByVal keep_cache_in_memory As Long = 0, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_CloseMultiBitmap Lib "FreeImage.dll" Alias "_FreeImage_CloseMultiBitmap@8" ( _
           ByVal BITMAP As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_GetPageCount Lib "FreeImage.dll" Alias "_FreeImage_GetPageCount@4" ( _
           ByVal BITMAP As Long) As Long

Public Declare Sub FreeImage_AppendPage Lib "FreeImage.dll" Alias "_FreeImage_AppendPage@8" ( _
           ByVal BITMAP As Long, _
           ByVal data As Long)

Public Declare Sub FreeImage_InsertPage Lib "FreeImage.dll" Alias "_FreeImage_InsertPage@12" ( _
           ByVal BITMAP As Long, _
           ByVal page As Long, _
           ByVal data As Long)

Public Declare Sub FreeImage_DeletePage Lib "FreeImage.dll" Alias "_FreeImage_DeletePage@8" ( _
           ByVal BITMAP As Long, _
           ByVal page As Long)

Public Declare Function FreeImage_LockPage Lib "FreeImage.dll" Alias "_FreeImage_LockPage@8" ( _
           ByVal BITMAP As Long, _
           ByVal page As Long) As Long

Public Declare Sub FreeImage_UnlockPage Lib "FreeImage.dll" Alias "_FreeImage_UnlockPage@12" ( _
           ByVal BITMAP As Long, _
           ByVal page As Long, _
           ByVal changed As Long)

Public Declare Function FreeImage_MovePage Lib "FreeImage.dll" Alias "_FreeImage_MovePage@12" ( _
           ByVal BITMAP As Long, _
           ByVal target As Long, _
           ByVal Source As Long) As Long

Public Declare Function FreeImage_GetLockedPageNumbers Lib "FreeImage.dll" Alias "_FreeImage_GetLockedPageNumbers@12" ( _
           ByVal BITMAP As Long, _
           ByRef pages As Long, _
           ByRef Count As Long) As Long


' Memory I/O streams (p. 47 to 50)
Public Declare Function FreeImage_OpenMemory Lib "FreeImage.dll" Alias "_FreeImage_OpenMemory@8" ( _
  Optional ByRef data As Byte = 0, _
  Optional ByVal size_in_bytes As Long = 0) As Long
  
Public Declare Function FreeImage_OpenMemoryByPtr Lib "FreeImage.dll" Alias "_FreeImage_OpenMemory@8" ( _
  Optional ByVal data_ptr As Long, _
  Optional ByVal size_in_bytes As Long = 0) As Long

Public Declare Sub FreeImage_CloseMemory Lib "FreeImage.dll" Alias "_FreeImage_CloseMemory@4" ( _
           ByVal stream As Long)

Public Declare Function FreeImage_LoadFromMemory Lib "FreeImage.dll" Alias "_FreeImage_LoadFromMemory@12" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal stream As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_SaveToMemory Lib "FreeImage.dll" Alias "_FreeImage_SaveToMemory@16" ( _
           ByVal fif As FREE_IMAGE_FORMAT, _
           ByVal dib As Long, _
           ByVal stream As Long, _
  Optional ByVal flags As Long = 0) As Long

Public Declare Function FreeImage_AcquireMemory Lib "FreeImage.dll" Alias "_FreeImage_AcquireMemory@12" ( _
           ByVal stream As Long, _
           ByRef data As Long, _
           ByRef size_in_bytes As Long) As Long

Public Declare Function FreeImage_TellMemory Lib "FreeImage.dll" Alias "_FreeImage_TellMemory@4" ( _
           ByVal stream As Long) As Long

Public Declare Function FreeImage_SeekMemory Lib "FreeImage.dll" Alias "_FreeImage_SeekMemory@12" ( _
           ByVal stream As Long, _
           ByVal offset As Long, _
           ByVal origin As Long) As Long


' Compression functions (p. 51 to 53)
Public Declare Function FreeImage_ZLibCompress Lib "FreeImage.dll" Alias "_FreeImage_ZLibCompress@16" ( _
           ByVal target As Long, _
           ByVal target_size As Long, _
           ByVal Source As Long, _
           ByVal source_size As Long) As Long

Public Declare Function FreeImage_ZLibUncompress Lib "FreeImage.dll" Alias "_FreeImage_ZLibUncompress@16" ( _
           ByVal target As Long, _
           ByVal target_size As Long, _
           ByVal Source As Long, _
           ByVal source_size As Long) As Long

Public Declare Function FreeImage_ZLibGZip Lib "FreeImage.dll" Alias "_FreeImage_ZLibGZip@16" ( _
           ByVal target As Long, _
           ByVal target_size As Long, _
           ByVal Source As Long, _
           ByVal source_size As Long) As Long
           
Public Declare Function FreeImage_ZLibGUnzip Lib "FreeImage.dll" Alias "_FreeImage_ZLibGUnzip@16" ( _
           ByVal target As Long, _
           ByVal target_size As Long, _
           ByVal Source As Long, _
           ByVal source_size As Long) As Long

Public Declare Function FreeImage_ZLibCRC32 Lib "FreeImage.dll" Alias "_FreeImage_ZLibCRC32@12" ( _
           ByVal crc As Long, _
           ByVal Source As Long, _
           ByVal source_size As Long) As Long


' Helper functions (p. 54 to 54)
Public Declare Function FreeImage_IsLittleEndian Lib "FreeImage.dll" Alias "_FreeImage_IsLittleEndian@0" () As Long

Public Declare Function FreeImage_LookupX11Color Lib "FreeImage.dll" Alias "_FreeImage_LookupX11Color@16" ( _
           ByVal szColor As String, _
           ByRef nRed As Long, _
           ByRef nGreen As Long, _
           ByRef nBlue As Long) As Long

Public Declare Function FreeImage_LookupSVGColor Lib "FreeImage.dll" Alias "_FreeImage_LookupSVGColor@16" ( _
           ByVal szColor As String, _
           ByRef nRed As Long, _
           ByRef nGreen As Long, _
           ByRef nBlue As Long) As Long



'--------------------------------------------------------------------------------
' Metadata functions
'--------------------------------------------------------------------------------

' Tag creation and destruction (p. 58 to 58)

' Tag accessors (p. 59 to 60)

' Metadata iterator (p. 61 to 61)
Public Declare Function FreeImage_FindFirstMetadata Lib "FreeImage.dll" Alias "_FreeImage_FindFirstMetadata@12" ( _
           ByVal model As FREE_IMAGE_MDMODEL, _
           ByVal dib As Long, _
           ByRef tag As Long) As Long

Public Declare Function FreeImage_FindNextMetadata Lib "FreeImage.dll" Alias "_FreeImage_FindNextMetadata@8" ( _
           ByVal mdhandle As Long, _
           ByRef tag As Long) As Long

Public Declare Sub FreeImage_FindCloseMetadata Lib "FreeImage.dll" Alias "_FreeImage_FindCloseMetadata@4" ( _
           ByVal mdhandle As Long)


' Metadata accessors (p. 63 to 63)
Public Declare Function FreeImage_SetMetadata Lib "FreeImage.dll" Alias "_FreeImage_SetMetadata@16" ( _
           ByRef model As Long, _
           ByVal dib As Long, _
           ByVal key As String, _
           ByRef tag As FITAG) As Long

Public Declare Function FreeImage_GetMetadata Lib "FreeImage.dll" Alias "_FreeImage_GetMetadata@16" ( _
           ByRef model As Long, _
           ByVal dib As Long, _
           ByVal key As String, _
           ByRef tag As FITAG) As Long


' Metadata helper functions (p. 65 to 65)
Public Declare Function FreeImage_GetMetadataCount Lib "FreeImage.dll" Alias "_FreeImage_GetMetadataCount@8" ( _
           ByRef model As Long, _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_TagToString Lib "FreeImage.dll" Alias "_FreeImage_TagToString@12" ( _
           ByRef model As Long, _
           ByRef tag As FITAG, _
  Optional ByVal Make As String = vbNullString) As Byte


'--------------------------------------------------------------------------------
' Toolkit functions
'--------------------------------------------------------------------------------

' Rotating and flipping (p. 66 to 68)
Public Declare Function FreeImage_RotateClassic Lib "FreeImage.dll" Alias "_FreeImage_RotateClassic@12" ( _
           ByVal dib As Long, _
           ByVal angle As Double) As Long

Public Declare Function FreeImage_RotateEx Lib "FreeImage.dll" Alias "_FreeImage_RotateEx@48" ( _
           ByVal dib As Long, _
           ByVal angle As Double, _
           ByVal x_shift As Double, _
           ByVal y_shift As Double, _
           ByVal x_origin As Double, _
           ByVal y_origin As Double, _
           ByVal use_mask As Long) As Long

Public Declare Function FreeImage_FlipHorizontal Lib "FreeImage.dll" Alias "_FreeImage_FlipHorizontal@4" ( _
           ByVal dib As Long) As Long

Public Declare Function FreeImage_FlipVertical Lib "FreeImage.dll" Alias "_FreeImage_FlipVertical@4" ( _
           ByVal dib As Long) As Long
           
Public Declare Function FreeImage_JPEGTransform Lib "FreeImage.dll" Alias "_FreeImage_JPEGTransform@16" ( _
           ByVal src_file As String, _
           ByVal dst_file As String, _
           ByVal operation As FREE_IMAGE_JPEG_OPERATION, _
  Optional ByVal prefect As Long = 0) As Long


' Upsampling and downsampling (p. 69 to 69)
Public Declare Function FreeImage_Rescale Lib "FreeImage.dll" Alias "_FreeImage_Rescale@16" ( _
           ByVal dib As Long, _
           ByVal dst_width As Long, _
           ByVal dst_height As Long, _
           ByVal Filter As FREE_IMAGE_FILTER) As Long


' Color manipulation (p. 70 to 71)
Public Declare Function FreeImage_AdjustCurve Lib "FreeImage.dll" Alias "_FreeImage_AdjustCurve@12" ( _
           ByVal dib As Long, _
           ByVal LUT As Long, _
           ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long

Public Declare Function FreeImage_AdjustGamma Lib "FreeImage.dll" Alias "_FreeImage_AdjustGamma@12" ( _
           ByVal dib As Long, _
           ByVal gamma As Double) As Long

Public Declare Function FreeImage_AdjustBrightness Lib "FreeImage.dll" Alias "_FreeImage_AdjustBrightness@12" ( _
           ByVal dib As Long, _
           ByVal percentage As Double) As Long

Public Declare Function FreeImage_AdjustContrast Lib "FreeImage.dll" Alias "_FreeImage_AdjustContrast@12" ( _
           ByVal dib As Long, _
           ByVal percentage As Double) As Long

Public Declare Function FreeImage_Invert Lib "FreeImage.dll" Alias "_FreeImage_Invert@4" ( _
           ByVal dib As Long) As Boolean

Public Declare Function FreeImage_GetHistogram Lib "FreeImage.dll" Alias "_FreeImage_GetHistogram@12" ( _
           ByVal dib As Long, _
           ByRef histo As Long, _
  Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK) As Long


' Channel processing (p. 72 to 72)
Public Declare Function FreeImage_GetChannel Lib "FreeImage.dll" Alias "_FreeImage_GetChannel@8" ( _
           ByVal dib As Long, _
           ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long

Public Declare Function FreeImage_SetChannel Lib "FreeImage.dll" Alias "_FreeImage_SetChannel@12" ( _
           ByVal dib As Long, _
           ByVal dib8 As Long, _
           ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long

Public Declare Function FreeImage_GetComplexChannel Lib "FreeImage.dll" Alias "_FreeImage_GetComplexChannel@8" ( _
           ByVal src As Long, _
           ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long

Public Declare Function FreeImage_SetComplexChannel Lib "FreeImage.dll" Alias "_FreeImage_SetComplexChannel@12" ( _
           ByVal dst As Long, _
           ByVal src As Long, _
           ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long


' Copy / Paste / Composite routines (p. 73 to 73)
Public Declare Function FreeImage_Copy Lib "FreeImage.dll" Alias "_FreeImage_Copy@20" ( _
           ByVal dib As Long, _
           ByVal Left As Long, _
           ByVal Top As Long, _
           ByVal Right As Long, _
           ByVal Bottom As Long) As Long

Public Declare Function FreeImage_Paste Lib "FreeImage.dll" Alias "_FreeImage_Paste@20" ( _
           ByVal dst As Long, _
           ByVal src As Long, _
           ByVal Left As Long, _
           ByVal Top As Long, _
           ByVal alpha As Long) As Long

Public Declare Function FreeImage_Composite Lib "FreeImage.dll" Alias "_FreeImage_Composite@16" ( _
           ByVal fg As Long, _
  Optional ByVal useFileBkg As Long = 0, _
  Optional ByVal appBkColor As Long = 0, _
  Optional ByVal bg As Long = 0) As Long



'--------------------------------------------------------------------------------
' Line converting functions
'--------------------------------------------------------------------------------

' convert to 4 bpp
Public Declare Sub FreeImage_ConvertLine1To4 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To4@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)
           
Public Declare Sub FreeImage_ConvertLine8To4 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To8@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)
           
Public Declare Sub FreeImage_ConvertLine16To4_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To4_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)
                     
Public Declare Sub FreeImage_ConvertLine16To4_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To4_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)
           
Public Declare Sub FreeImage_ConvertLine24To4 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To24@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)
           
Public Declare Sub FreeImage_ConvertLine32To4 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To4@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)


' convert to 8 bpp
Public Declare Sub FreeImage_ConvertLine1To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To8@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine4To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To8@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine16To8_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To8_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine16To8_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To8_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine24To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To8@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine32To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To8@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)
           

' convert to 16 bpp
Public Declare Sub FreeImage_ConvertLine1To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To16_555@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine4To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To16_555@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine8To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To16_555@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine16_565_To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16_565_To16_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine24To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To16_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine32To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To16_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine1To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To16_565@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine4To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To16_565@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine8To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To16_565@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine16_555_To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16_555_To16_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine24To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To16_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine32To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To16_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)


' convert to 24 bpp
Public Declare Sub FreeImage_ConvertLine1To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To24@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine4To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To24@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine8To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To24@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine16To24_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To24_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine16To24_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To24_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine32To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To24@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)


' convert to 32 bpp
Public Declare Sub FreeImage_ConvertLine1To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To32@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine4To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To32@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine8To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To32@16" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long, _
           ByVal palette As Long)

Public Declare Sub FreeImage_ConvertLine16To32_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To32_555@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine16To32_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To32_565@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)

Public Declare Sub FreeImage_ConvertLine24To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To32@12" ( _
           ByVal target As Long, _
           ByVal Source As Long, _
           ByVal width_in_pixels As Long)



'--------------------------------------------------------------------------------
' Error handling functions
'--------------------------------------------------------------------------------

Public Sub FreeImage_InitErrorHandler()

   ' Call this function once for using the FreeImage 3 error handling callback.
   ' The 'FreeImage_ErrorHandler' function is called on each FreeImage 3 error.

   Call FreeImage_SetOutputMessage(AddressOf FreeImage_ErrorHandler)

End Sub

Private Sub FreeImage_ErrorHandler(ByVal fif As FREE_IMAGE_FORMAT, ByVal message As Long)

Dim strErrorMessage As String
Dim strImageFormat As String

   ' This function is called whenever the FreeImage 3 libraray throws an error.
   ' Currently this function gets the error message and the format name of the
   ' involved image type as VB string printing each to the VB Debug console. Feel
   ' free to modify this function to call an error handling routine of your on.

   strErrorMessage = pGetStringFromPointerA(message)
   strImageFormat = FreeImage_GetFormatFromFIF(fif)
   
   Debug.Print "[FreeImage] Error: " & strErrorMessage
   Debug.Print "            Image: " & strImageFormat

End Sub



'--------------------------------------------------------------------------------
' String returning functions wrappers
'--------------------------------------------------------------------------------

Public Function FreeImage_GetVersion() As String

   ' This function returns the version of the FreeImage 3 library
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   FreeImage_GetVersion = pGetStringFromPointerA(FreeImage_GetVersionInt)

End Function

Public Function FreeImage_GetCopyrightMessage() As String

   ' This function returns the copyright message of the FreeImage 3 library
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   FreeImage_GetCopyrightMessage = pGetStringFromPointerA(FreeImage_GetCopyrightMessageInt)

End Function

Public Function FreeImage_GetFormatFromFIF(ByVal fif As FREE_IMAGE_FORMAT) As String

   ' This function returns the result of the 'FreeImage_GetFormatFromFIF' function
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   ' The parameter 'fif' works according to the FreeImage 3 API documentation.
   
   FreeImage_GetFormatFromFIF = pGetStringFromPointerA(FreeImage_GetFormatFromFIFInt(fif))

End Function

Public Function FreeImage_GetFIFExtensionList(ByVal fif As FREE_IMAGE_FORMAT) As String

   ' This function returns the result of the 'FreeImage_GetFIFExtensionList' function
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   ' The parameter 'fif' works according to the FreeImage 3 API documentation.
   
   FreeImage_GetFIFExtensionList = pGetStringFromPointerA(FreeImage_GetFIFExtensionListInt(fif))

End Function

Public Function FreeImage_GetFIFDescription(ByVal fif As FREE_IMAGE_FORMAT) As String

   ' This function returns the result of the 'FreeImage_GetFIFDescription' function
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   ' The parameter 'fif' works according to the FreeImage 3 API documentation.
   
   FreeImage_GetFIFDescription = pGetStringFromPointerA(FreeImage_GetFIFDescriptionInt(fif))

End Function

Public Function FreeImage_GetFIFRegExpr(ByVal fif As FREE_IMAGE_FORMAT) As String

   ' This function returns the result of the 'FreeImage_GetFIFRegExpr' function
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   ' The parameter 'fif' works according to the FreeImage 3 API documentation.
   
   FreeImage_GetFIFRegExpr = pGetStringFromPointerA(FreeImage_GetFIFRegExprInt(fif))

End Function

Public Function FreeImage_GetFIFMimeType(ByVal fif As FREE_IMAGE_FORMAT) As String
   
   ' This function returns the result of the 'FreeImage_GetFIFMimeType' function
   ' as VB String. Read paragraph 2 of the "General notes on implementation
   ' and design" section to learn more about that technique.
   
   ' The parameter 'fif' works according to the FreeImage 3 API documentation.
   
   FreeImage_GetFIFMimeType = pGetStringFromPointerA(FreeImage_GetFIFMimeTypeInt(fif))
   
End Function



'--------------------------------------------------------------------------------
' UNICODE dealing functions wrappers
'--------------------------------------------------------------------------------

Public Function FreeImage_LoadU(ByVal fif As FREE_IMAGE_FORMAT, _
                                ByVal FileName As String, _
                       Optional ByVal flags As Long = 0) As Long
                       
   ' This function is just a thin wrapper to ease the call to an
   ' UNICODE function. Since VB's BSTR strings are actually UNICODE
   ' strings, we just need to pass the pointer to the string data
   ' returned by the (undocumented) function StrPtr().
   
   FreeImage_LoadU = FreeImage_LoadUInt(fif, StrPtr(FileName), flags)

End Function
  
Public Function FreeImage_SaveU(ByVal fif As FREE_IMAGE_FORMAT, _
                                ByVal dib As Long, _
                                ByVal FileName As String, _
                       Optional ByVal flags As Long = 0) As Long

   ' This function is just a thin wrapper to ease the call to an
   ' UNICODE function. Since VB's BSTR strings are actually UNICODE
   ' strings, we just need to pass the pointer to the string data
   ' returned by the (undocumented) function StrPtr().
   
   FreeImage_SaveU = FreeImage_SaveUInt(fif, dib, StrPtr(FileName), flags)

End Function

Public Function FreeImage_GetFileTypeU(ByVal FileName As String, _
                              Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT

   ' This function is just a thin wrapper to ease the call to an
   ' UNICODE function. Since VB's BSTR strings are actually UNICODE
   ' strings, we just need to pass the pointer to the string data
   ' returned by the (undocumented) function StrPtr().
   
   FreeImage_GetFileTypeU = FreeImage_GetFileTypeUInt(StrPtr(FileName), size)

End Function

Public Function FreeImage_GetFIFFromFilenameU(ByVal FileName As String) As FREE_IMAGE_FORMAT

   ' This function is just a thin wrapper to ease the call to an
   ' UNICODE function. Since VB's BSTR strings are actually UNICODE
   ' strings, we just need to pass the pointer to the string data
   ' returned by the (undocumented) function StrPtr().

   FreeImage_GetFIFFromFilenameU = FreeImage_GetFIFFromFilenameUInt(StrPtr(FileName))

End Function



'--------------------------------------------------------------------------------
' Extended functions derived from FreeImage 3 functions usually dealing
' with arrays
'--------------------------------------------------------------------------------

Public Function FreeImage_GetPaletteEx(ByVal dib As Long) As RGBQUAD()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a VB style array of type RGBQUAD, containing
   ' the palette data of the dib. This array provides read and write access
   ' to the actual palette data provided by FreeImage. This is done by
   ' creating a VB array with an own SAFEARRAY descriptor making the
   ' array point to the palette pointer returned by FreeImage_GetPalette.
   
   ' This makes you use code like you would in C/C++:
   
   ' // this code assumes there is a bitmap loaded and
   ' // present in a variable called ‘dib’
   ' if(FreeImage_GetBPP(dib) == 8) {
   '   // Build a greyscale palette
   '   RGBQUAD *pal = FreeImage_GetPalette(dib);
   '   for (int i = 0; i < 256; i++) {
   '     pal[i].rgbRed = i;
   '     pal[i].rgbGreen = i;
   '     pal[i].rgbBlue = i;
   '   }
   
   ' As in C/C++ the array is only valid while the dib is loaded and the
   ' palette data remains where the pointer returned by FreeImage_GetPalette
   ' has pointed to when this function was called. So, a good thing would
   ' be, not to keep the returned array in scope over the lifetime of the
   ' dib. Best practise is, to use this function within another routine and
   ' assign the return value (the array) to a local variable only. As soon
   ' as this local variable goes out of scope (when the calling function
   ' returns to their caller), the array and the descriptor is automatically
   ' cleaned up by VB.
   
   ' This function does not make a deep copy of the palette data, but only
   ' wraps a VB array around the FreeImge palette data. So, it can be called
   ' frequently "on demand" or somewhat "in place" without a significant
   ' performance loss.
   
   ' To learn more about this technique I recommend reading chapter 2 (Leveraging
   ' Arrays) of Matthew Curland's book "Advanced Visual Basic 6"
   
   ' The parameter 'dib' works according to the FreeImage 3 API documentation.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.
   
   
   If (dib) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 4                           ' size in bytes of RGBQUAD structure
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetColorsUsed(dib) ' the number of elements in the array is
                                                   ' the number of used colors in the dib
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetPalette(dib)       ' let the array point to the memory block, the
                                                   ' FreeImage palette pointer points to
      End With
      
      ' allocate memory for an array descriptor
      ' we cannot use the memory block used by tSA, since it is
      ' released when tSA goes out of scope, leaving us with an
      ' array with zeroed descriptor
      ' we use nearly the same method that VB uses, so VB is able
      ' to cleanup the array variable and it's descriptor; the
      ' array data is not touched when cleaning up, since both AUTO
      ' and FIXEDSIZE flags are set
      Call SafeArrayAllocDescriptor(1, lpSA)
      
      ' copy our own array descriptor over the descriptor allocated
      ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
      ' location
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      
      ' the implicit variable named like the function is an array
      ' variable in VB
      ' make it point to the allocated array descriptor
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetPaletteEx), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetPaletteExClone(ByVal dib As Long) As RGBQUAD()

Dim lColors As Long
Dim atRGB() As RGBQUAD
Dim i As Long

   ' This function returns a redundant clone of a dib's palette as a
   ' VB style array of type RGBQUAD.
   
   ' The parameter 'dib' works according to the FreeImage 3 API documentation.

   Select Case FreeImage_GetBPP(dib)
   
   Case 1, 4, 8
      lColors = FreeImage_GetColorsUsed(dib)
      ReDim atRGB(lColors - 1)
      Call CopyMemory(atRGB(0), ByVal FreeImage_GetPalette(dib), lColors * 4)
      
   End Select

End Function

Public Function FreeImage_GetPaletteExLong(ByVal dib As Long) As Long()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a VB style array of type Long, containing
   ' the palette data of the dib. This array provides read and write access
   ' to the actual palette data provided by FreeImage. This is done by
   ' creating a VB array with an own SAFEARRAY descriptor making the
   ' array point to the palette pointer returned by FreeImage_GetPalette.
   
   ' The function actually returns an array of type RGBQUAD with each
   ' element packed into a Long. This is possible, since the RGBQUAD
   ' structure is also four bytes in size. Palette data, stored in an
   ' array of type Long may be passed ByRef to a function through an
   ' optional paremeter. For an example have a look at function
   ' 'FreeImage_ConvertColorDepth'
   
   ' This makes you use code like you would in C/C++:
   
   ' // this code assumes there is a bitmap loaded and
   ' // present in a variable called ‘dib’
   ' if(FreeImage_GetBPP(dib) == 8) {
   '   // Build a greyscale palette
   '   RGBQUAD *pal = FreeImage_GetPalette(dib);
   '   for (int i = 0; i < 256; i++) {
   '     pal[i].rgbRed = i;
   '     pal[i].rgbGreen = i;
   '     pal[i].rgbBlue = i;
   '   }
   
   ' As in C/C++ the array is only valid while the dib is loaded and the
   ' palette data remains where the pointer returned by FreeImage_GetPalette
   ' has pointed to when this function was called. So, a good thing would
   ' be, not to keep the returned array in scope over the lifetime of the
   ' dib. Best practise is, to use this function within another routine and
   ' assign the return value (the array) to a local variable only. As soon
   ' as this local variable goes out of scope (when the calling function
   ' returns to their caller), the array and the descriptor is automatically
   ' cleaned up by VB.
   
   ' This function does not make a deep copy of the palette data, but only
   ' wraps a VB array around the FreeImge palette data. So, it can be called
   ' frequently "on demand" or somewhat "in place" without a significant
   ' performance loss.
   
   ' To learn more about this technique I recommend reading chapter 2 (Leveraging
   ' Arrays) of Matthew Curland's book "Advanced Visual Basic 6"
   
   ' The parameter 'dib' works according to the FreeImage 3 API documentation.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   
   If (dib) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 4                           ' size in bytes of RGBQUAD structure
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetColorsUsed(dib) ' the number of elements in the array is
                                                   ' the number of used colors in the dib
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetPalette(dib)       ' let the array point to the memory block, the
                                                   ' FreeImage palette pointer points to
      End With
      
      ' allocate memory for an array descriptor
      ' we cannot use the memory block used by tSA, since it is
      ' released when tSA goes out of scope, leaving us with an
      ' array with zeroed descriptor
      ' we use nearly the same method that VB uses, so VB is able
      ' to cleanup the array variable and it's descriptor; the
      ' array data is not touched when cleaning up, since both AUTO
      ' and FIXEDSIZE flags are set
      Call SafeArrayAllocDescriptor(1, lpSA)
      
      ' copy our own array descriptor over the descriptor allocated
      ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
      ' location
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      
      ' the implicit variable named like the function is an array
      ' variable in VB
      ' make it point to the allocated array descriptor
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetPaletteExLong), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetTransparencyTableEx(ByVal dib As Long) As Byte()

Dim abBuffer() As Byte
Dim lpTransparencyTable As Long

   ' This function returns a copy of a DIB's transparency table as VB style
   ' array of type Byte. So, the array provides read access only from the DIS's
   ' point of view.
   
   ' The parameter 'dib' works according to the FreeImage 3 API documentation.

   lpTransparencyTable = FreeImage_GetTransparencyTable(dib)
   If (lpTransparencyTable) Then
      ReDim abBuffer(255)
      Call CopyMemory(abBuffer(0), ByVal lpTransparencyTable, 256)
      Call swap(ByVal VarPtrArray(abBuffer), ByVal VarPtrArray(FreeImage_GetTransparencyTableEx))
   End If

End Function

Public Sub FreeImage_SetTransparencyTableEx(ByVal dib As Long, _
                                            ByRef table() As Byte, _
                                   Optional ByRef Count As Long = -1)

   ' This function sets a DIB's transparency table to the contents of the
   ' parameter table(). When the optional parameter Count is omitted, the
   ' number of entries used is taken from the number of elements stored in
   ' the array, but never greater than 256.
   
   ' The parameter 'dib' works according to the FreeImage 3 API documentation.
   
   If ((Count > UBound(table) + 1) Or _
       (Count < 0)) Then
      Count = UBound(table) + 1
   End If
   
   If (Count > 256) Then
      Count = 256
   End If

   Call FreeImage_SetTransparencyTable(dib, VarPtr(table(0)), Count)

End Sub

Public Function FreeImage_GetHistogramEx(ByVal dib As Long, _
                                Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK, _
                                Optional ByRef success As Boolean) As Long()
                                
Dim alResult() As Long

   ' This function returns a DIB's histogram data as VB style array of
   ' type Long. Since histogram data is never modified directly, it seems
   ' enough to return a clone of the data and no read/write accessible
   ' array wrapped around the actual pointer.
   
   ' All parameters work according to the FreeImage 3 API documentation.

   ReDim alResult(255)
   success = FreeImage_GetHistogram(dib, alResult(0), channel)
   If (success) Then
      Call swap(VarPtrArray(FreeImage_GetHistogramEx), VarPtrArray(alResult))
   End If

End Function

'''Public Function FreeImage_DrawHistogram(ByVal dib As Long, _
'''                                        ByVal hDC As Long, _
'''                               Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK, _
'''                               optional byval orientation as HISTOGRAM_ORIENTATION=HOR_BOTTOM_UP, _
'''                               optional byval ) As Boolean
'''
'''End Function

Public Function FreeImage_SetChannelEx(ByVal dib As Long, _
                                       ByVal channel As FREE_IMAGE_COLOR_CHANNEL, _
                                       ByVal dib_channel As Long, _
                              Optional ByVal channel_src As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK, _
                              Optional ByVal Left As Long, _
                              Optional ByVal Top As Long) As Long
                              
Dim lBits As Long
Dim hDibSrc As Long
Dim lWidthDst As Long
Dim lHeightDst As Long
Dim lWidthSrc As Long
Dim lHeightSrc As Long
                              
   lBits = FreeImage_GetBPP(dib)
   If ((lBits = 24) Or (lBits = 32)) Then
      
      lBits = FreeImage_GetBPP(dib_channel)
      If (lBits >= 8) Then
         
         If (lBits > 8) Then
            hDibSrc = FreeImage_GetChannel(dib_channel, channel_src)
         Else
            hDibSrc = dib_channel
         End If
         
         If (hDibSrc) Then
         
            lWidthDst = FreeImage_GetWidth(dib)
            lHeightDst = FreeImage_GetHeight(dib)
            
            lWidthSrc = FreeImage_GetWidth(hDibSrc)
            lHeightSrc = FreeImage_GetHeight(hDibSrc)
            
            If ((lWidthSrc <> lWidthDst) Or _
                (lHeightSrc <> lHeightDst) Or _
                (Left <> 0) Or _
                (Top <> 0)) Then
                
                
                
            End If
         
            FreeImage_SetChannelEx = FreeImage_SetChannel(dib, hDibSrc, channel)
            Call FreeImage_Unload(hDibSrc)
         End If
      End If
   End If
                              
End Function

Public Function FreeImage_AdjustCurveEx(ByVal dib As Long, _
                                        ByRef LUT As Variant, _
                               Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK) As Long
                               
Dim lpLUTData As Long
Dim lSizeInBytes As Long

   ' This function extends the FreeImage function 'FreeImage_AdjustCurve'
   ' to a more VB suitable function. The parameter 'LUT' of type Variant my
   ' me either an array of type Byte or may contain the pointer to a memory
   ' block, what in VB is always the address of the memory block, since VB
   ' actually doesn's support native pointers.
   
   ' In case of providing the memory block as an array, make sure, that the
   ' array contains exactly 256 items. In case of providing an address of a
   ' memory block, the size of the memory block is assumed to be 256 bytes
   ' and it is up to the caller to ensure that it is large enough.

   If (IsArray(LUT)) Then
      lpLUTData = pGetMemoryBlockPtrFromVariant(LUT, lSizeInBytes)
   
   ElseIf (IsNumeric(LUT)) Then
      lSizeInBytes = 256
      lpLUTData = CLng(LUT)
   
   End If
   
   If ((lpLUTData <> 0) And (lSizeInBytes = 256)) Then
      FreeImage_AdjustCurveEx = FreeImage_AdjustCurve(dib, lpLUTData, channel)
   End If

End Function

Public Function FreeImage_LoadFromMemoryEx(ByRef data As Variant, _
                                  Optional ByRef size_in_bytes As Long = 0, _
                                  Optional ByRef fif As FREE_IMAGE_FORMAT) As Long

Dim hStream As Long
Dim lDataPtr As Long
Dim lPtr As Long

   ' This function extends the FreeImage function FreeImage_LoadFromMemory
   ' to a more VB suitable function. The parameter data of type Variant my
   ' me either an array of type Byte, Integer or Long or may contain the pointer
   ' to a memory block, what in VB is always the address of the memory block,
   ' since VB actually doesn's support native pointers.
   
   ' In case of providing the memory block as an array, the size_in_bytes may
   ' be omitted, zero or less than zero. Then, the size of the memory block
   ' is calculated correctly. When size_in_bytes is given, it is up to the caller
   ' to ensure, it is correct.
   
   ' In case of providing an address of a memory block, size_in_bytes must not
   ' be omitted.
   
   ' The parameter fif is an OUT parameter, that will contain the image type
   ' detected. Any values set by the caller will never be used within this
   ' function.

   ' get both pointer and size in bytes of the memory block provided
   ' through the Variant parameter 'data'.
   lDataPtr = pGetMemoryBlockPtrFromVariant(data, size_in_bytes)
   
   ' open the memory stream
   hStream = FreeImage_OpenMemoryByPtr(lDataPtr, size_in_bytes)
   If (hStream) Then
      ' on success, detect image type
      fif = FreeImage_GetFileTypeFromMemory(hStream)
      If (fif <> FIF_UNKNOWN) Then
         ' load the image from memory stream only, if known image type
         FreeImage_LoadFromMemoryEx = FreeImage_LoadFromMemory(fif, hStream)
      End If
      ' close the memory stream when open
      Call FreeImage_CloseMemory(hStream)
   End If

End Function

Public Function FreeImage_SaveToMemoryEx(ByVal fif As FREE_IMAGE_FORMAT, _
                                         ByVal dib As Long, _
                                         ByRef data() As Byte, _
                                Optional ByVal flags As FREE_IMAGE_SAVE_OPTIONS = FISO_SAVE_DEFAULT, _
                                Optional ByVal bUnloadSource As Boolean) As Boolean

Dim hStream As Long
Dim lpData As Long
Dim lSizeInBytes As Long

   ' This function saves a FreeImage DIB into memory by using the VB Byte
   ' array data(). It makes a deep copy of the image data and closes the
   ' memory stream opened before it returns to the caller.
   
   ' The Byte array 'data()' must not be a fixed sized array and will be
   ' redimensioned according to the size needed to hold all the data.
   
   ' The parameters 'fif', 'dib' and 'flags' work according to the FreeImage 3
   ' API documentation.
   
   ' The optional 'bUnloadSource' parameter is for unloading the original image
   ' after it has been saved into memory. There is no need to clean up the DIB
   ' at the caller's site.
   
   ' The function returns True on success and False otherwise.
   
   
   hStream = FreeImage_OpenMemory()
   If (hStream) Then
      FreeImage_SaveToMemoryEx = FreeImage_SaveToMemory(fif, dib, hStream, flags)
      If (FreeImage_SaveToMemoryEx) Then
         If (FreeImage_AcquireMemory(hStream, lpData, lSizeInBytes)) Then
            On Error Resume Next
            ReDim data(lSizeInBytes - 1)
            If (Err.Number = ERROR_SUCCESS) Then
               On Error GoTo 0
               Call CopyMemory(data(0), ByVal lpData, lSizeInBytes)
            Else
               On Error GoTo 0
               FreeImage_SaveToMemoryEx = False
            End If
         Else
            FreeImage_SaveToMemoryEx = False
         End If
      End If
      Call FreeImage_CloseMemory(hStream)
   Else
      FreeImage_SaveToMemoryEx = False
   End If
   
   If (bUnloadSource) Then
      Call FreeImage_Unload(dib)
   End If

End Function

Public Function FreeImage_SaveToMemoryEx2(ByVal fif As FREE_IMAGE_FORMAT, _
                                          ByVal dib As Long, _
                                          ByRef data() As Byte, _
                                          ByRef stream As Long, _
                                 Optional ByVal flags As FREE_IMAGE_SAVE_OPTIONS = FISO_SAVE_DEFAULT, _
                                 Optional ByVal bUnloadSource As Boolean) As Boolean

Dim lpData As Long
Dim tSA As SAVEARRAY1D

   ' This function saves a FreeImage DIB into memory by using the VB Byte
   ' array data(). It does not makes a deep copy of the image data, but uses
   ' the function 'FreeImage_AcquireMemoryEx' to wrap the array 'data()'
   ' around the memory block pointed to by the result of the
   ' 'FreeImage_AcquireMemory' function.
   
   ' The Byte array 'data()' must not be a fixed sized array and will be
   ' redimensioned according to the size needed to hold all the data.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.
   
   ' The parameter 'stream' is an IN/OUT parameter, tracking the memory
   ' stream, the VB array 'data()' is based on. This parameter may contain
   ' an already opened FreeImage memory stream when the function is called and
   ' contains a valid memory stream when the function returns in each case.
   ' After all, it is up to the caller to close that memory stream correctly.
   ' The array 'data()' will no longer be valid and accessable after the stream
   ' has been closed, so it should only be closed after the passed byte array
   ' variable either goes out of the caller's scope or is redimensioned.
   
   ' The parameters 'fif', 'dib' and 'flags' work according to the FreeImage 3
   ' API documentation.
   
   ' The optional 'bUnloadSource' parameter is for unloading the original image
   ' after it has been saved into memory. There is no need to clean up the DIB
   ' at the caller's site.
   
   ' The function returns True on success and False otherwise.

   
   If (stream = 0) Then
      stream = FreeImage_OpenMemory()
   End If
   If (stream) Then
      FreeImage_SaveToMemoryEx2 = FreeImage_SaveToMemory(fif, dib, stream, flags)
      If (FreeImage_SaveToMemoryEx2) Then
         FreeImage_SaveToMemoryEx2 = FreeImage_AcquireMemoryEx(stream, data)
      End If
      
      ' do not close the memory stream, since the returned array data()
      ' points to the stream's data
      ' the caller must close the stream after he is done
      ' with the array
   Else
      FreeImage_SaveToMemoryEx2 = False
   End If
   
   If (bUnloadSource) Then
      Call FreeImage_Unload(dib)
   End If

End Function

Public Function FreeImage_AcquireMemoryEx(ByVal stream As Long, _
                                          ByRef data() As Byte, _
                                 Optional ByRef size_in_bytes As Long) As Boolean
                                          
Dim lpData As Long
Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function wraps the byte array data() around acquired memory
   ' of the memory stream specified by then stream parameter. The adjusted
   ' array then points directly to the stream's data pointer and so
   ' provides full read and write access. All data contained in the array
   ' will be lost and freed properly.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.


   If (stream) Then
      If (FreeImage_AcquireMemory(stream, lpData, size_in_bytes)) Then
         With tSA
            .cbElements = 1                           ' one element is one byte
            .cDims = 1                                ' the array has only 1 dimension
            .cElements = size_in_bytes                ' the number of elements in the array is
                                                      ' the size in bytes of the memory block
            .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                      ' so the array can not be modified in size
                                                      ' or erased; according to Matthew Curland never
                                                      ' use FIXEDSIZE alone
            .pvData = lpData                          ' let the array point to the memory block
                                                      ' received by FreeImage_AcquireMemory
         End With
         
         lpSA = deref(VarPtrArray(data))
         Call SafeArrayDestroyData(ByVal lpSA)
         Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Else
         FreeImage_AcquireMemoryEx = False
      End If
   Else
      FreeImage_AcquireMemoryEx = False
   End If

End Function



'--------------------------------------------------------------------------------
' Tag accessing VB friendly helper functions
'--------------------------------------------------------------------------------

Public Function FreeImage_TagFromPointer(ByVal tag As Long) As FITAG

Dim tTagInt As FITAG_int

   ' This function is a helper routine for converting a FreeImage
   ' FITAG structure into an internal used VB friedly FITAG structure.
   
   ' This function is in experimental state and so subject to change!

   Call CopyMemory(tTagInt, ByVal deref(tag), Len(tTagInt))
   
   With FreeImage_TagFromPointer
      .Count = tTagInt.Count
      .description = pGetStringFromPointerA(tTagInt.description)
      .id = tTagInt.id
      .key = pGetStringFromPointerA(tTagInt.key)
      .Length = tTagInt.Length
      .type = tTagInt.type
      
      Select Case .type
      
      Case FIDT_ASCII
         .Value = pGetStringFromPointerA(tTagInt.Value)
      
      End Select
   
   End With

End Function



'--------------------------------------------------------------------------------
' Derived and hopefully useful functions
'--------------------------------------------------------------------------------

Public Function FreeImage_IsExtensionValidForFIF(ByVal fif As FREE_IMAGE_FORMAT, _
                                                 ByVal extension As String, _
                                        Optional ByVal compare As VbCompareMethod = vbBinaryCompare) As Boolean
   
   ' This function tests, whether a given filename extension is valid
   ' for a certain image format (fif).
   
   FreeImage_IsExtensionValidForFIF = (InStr(1, _
                                             FreeImage_GetFIFExtensionList(fif) & ",", _
                                             extension, _
                                             compare) > 0)

End Function

Public Function FreeImage_IsFilenameValidForFIF(ByVal fif As FREE_IMAGE_FORMAT, _
                                                ByVal FileName As String, _
                                       Optional ByVal compare As VbCompareMethod = vbBinaryCompare) As Boolean
                                                
Dim strExtension As String
Dim i As Long

   ' This function tests, whether a given complete filename is valid
   ' for a certain image format (fif).

   i = InStrRev(FileName, ".")
   If (i) Then
      strExtension = Mid$(FileName, i + 1)
      FreeImage_IsFilenameValidForFIF = (InStr(1, _
                                               FreeImage_GetFIFExtensionList(fif) & ",", _
                                               strExtension, _
                                               compare) > 0)
   End If
   
End Function

Public Function FreeImage_GetPrimaryExtensionFromFIF(ByVal fif As FREE_IMAGE_FORMAT) As String

Dim strExtensionList As String
Dim i As Long

   ' This function returns the primary (main or most commonly used?) extension
   ' of a certain image format (fif). This is done by returning the first of
   ' all possible extensions returned by 'FreeImage_GetFIFExtensionList'. That
   ' assumes, that the plugin returns the extensions in ordered form. If not,
   ' in most cases it is even enough, to receive any extension.
   
   ' This function is primarily used by the function 'SavePictureEx'.

   strExtensionList = FreeImage_GetFIFExtensionList(fif)
   i = InStr(strExtensionList, ",")
   If (i) Then
      FreeImage_GetPrimaryExtensionFromFIF = Left$(strExtensionList, i - 1)
   Else
      FreeImage_GetPrimaryExtensionFromFIF = strExtensionList
   End If

End Function

Public Function FreeImage_IsGreyscaleImage(ByVal dib As Long) As Boolean

Dim atRGB() As RGBQUAD
Dim i As Long

   ' This function returns a boolean value that is true, if the DIB is actually
   ' a greyscale image. Here, the only test condition is, that each palette
   ' entry must be a grey value, what means that each color component has the
   ' same value (red = green = blue).
   
   ' The FreeImage libraray doesn't offer a function to determine if a DIB is
   ' greyscale. The only thing you can do is to use the 'FreeImage_GetColorType'
   ' function, that returns either FIC_MINISWHITE or FIC_MINISBLACK for
   ' greyscale images. However, a DIB needs to have a ordered greyscale palette
   ' (linear ramp or inverse linear ramp) to be judged as FIC_MINISWHITE or
   ' FIC_MINISBLACK. DIB's with an unordered palette that are actually (visually)
   ' greyscale, are said to be (color-)palettized. That's also true for any 4 bpp
   ' image, since it will never have a palette that satifies the tests done
   ' in the 'FreeImage_GetColorType' function.
   
   ' So, there is a chance to omit some color depth conversions, when displaying
   ' an image in greyscale fashion. Maybe the problem will be solved in the
   ' FreeImage library one day.

   Select Case FreeImage_GetBPP(dib)
   
   Case 1, 4, 8
      atRGB = FreeImage_GetPaletteEx(dib)
      FreeImage_IsGreyscaleImage = True
      For i = 0 To UBound(atRGB)
         With atRGB(i)
            If ((.rgbRed <> .rgbGreen) Or (.rgbRed <> .rgbBlue)) Then
               FreeImage_IsGreyscaleImage = False
               Exit For
            End If
         End With
      Next i
   
   End Select

End Function

Public Function FreeImage_GetResolutionX(ByVal dib As Long) As Long

   ' This function gets a DIB's resolution in X-direction measured
   ' in 'dots per inch' (DPI) and not in 'dots per meter'.
   
   FreeImage_GetResolutionX = Int(0.5 + 0.0254 * FreeImage_GetDotsPerMeterX(dib))

End Function

Public Sub FreeImage_SetResolutionX(ByVal dib As Long, ByVal res As Long)

   ' This function sets a DIB's resolution in X-direction measured
   ' in 'dots per inch' (DPI) and not in 'dots per meter'.

   Call FreeImage_SetDotsPerMeterX(dib, Int(res / 0.0254 + 0.5))

End Sub

Public Function FreeImage_GetResolutionY(ByVal dib As Long) As Long

   ' This function gets a DIB's resolution in Y-direction measured
   ' in 'dots per inch' (DPI) and not in 'dots per meter'.

   FreeImage_GetResolutionY = Int(0.5 + 0.0254 * FreeImage_GetDotsPerMeterY(dib))

End Function

Public Sub FreeImage_SetResolutionY(ByVal dib As Long, ByVal res As Long)

   ' This function sets a DIB's resolution in Y-direction measured
   ' in 'dots per inch' (DPI) and not in 'dots per meter'.

   Call FreeImage_SetDotsPerMeterY(dib, Int(res / 0.0254 + 0.5))

End Sub

' Image color depth conversion wrapper

Public Function FreeImage_ConvertColorDepth(ByVal hDIB As Long, _
                                            ByVal eConversionFlag As FREE_IMAGE_CONVERSION_FLAGS, _
                                   Optional ByVal bUnloadSource As Boolean, _
                                   Optional ByVal bThreshold As Byte = 128, _
                                   Optional ByVal eDitherMethod As FREE_IMAGE_DITHER = FID_FS, _
                                   Optional ByVal eQuantizationMethod As FREE_IMAGE_QUANTIZE = FIQ_WUQUANT, _
                                   Optional ByVal lPaletteSize As Long = 256, _
                                   Optional ByRef vntReservePalette As Variant, _
                                   Optional ByVal lReserveSize As Long) As Long
                                            
Dim hDIBNew As Long
Dim hDIBTemp As Long
Dim lBPP As Long
Dim bConvertPalette As Boolean
Dim bKeepPalette As Boolean
Dim lpReservePalette As Long
Dim bAdjustReservePaletteSize As Boolean

   ' This function is an easy-to-use wrapper for color depth conversion, intended
   ' to work around some tweaks in the FreeImage library.
   
   ' The parameters 'bThreshold' and 'eDitherMode' control how thresholding or
   ' dithering are performed. The 'eQuantizationMethod' parameter determines, what
   ' quantization algorithm will be used when converting to 8 bit color images.
   
   ' The 'eConversionFlag' parameter, which can contain a single value or an OR'ed
   ' combination of some of the FREE_IMAGE_CONVERSION_FLAGS enumeration values,
   ' determines the desired output image format.
   
   ' The optional 'bUnloadSource' parameter is for unloading the original image, so
   ' you can "change" an image with this function rather than getting a new DIB
   ' pointer. There is no more need for a second DIB variable at the caller's site.
   
   ' The optional 'lPaletteSize' parameter lets you specify the desired size (the
   ' number of actually used palette entries) of the output palette when converting
   ' to an 8 bit color image. When 'lPaletteSize' differs from 256, this function
   ' uses 'FreeImage_ColorQuantizeEx' to get the converted image.
   
   ' Both parameters 'vntReservePalette' and 'lReserveSize' also work together with
   ' the 'FreeImage_ColorQuantizeEx' function. 'vntReservePalette' may either be a
   ' pointer to palette data (pointer to an array of type RGBQUAD) or an array of
   ' type Long, which must contain the palette data. You can receive palette data as
   ' an array of type Long with the function 'FreeImage_GetPaletteExLong'. According
   ' to the FreeImage API documentation, lReserveSize must contain the number of
   ' palette entries used from the reserve palette and may be omitted, if
   ' 'vntReservePalette' is an array. In that case, 'lReserveSize' will be assumed
   ' to be the number of array elements.
   

   bKeepPalette = ((eConversionFlag And FICF_REORDER_GREYSCALE_PALETTE) = 0)

   If (hDIB) Then
   
      Select Case (eConversionFlag And (Not FICF_REORDER_GREYSCALE_PALETTE))
      
      Case FICF_MONOCHROME_THRESHOLD
         ' FreeImage version 3.8.0 still has a minor bug when converting
         ' 8 bit palettized images or 8 bit greyscale images with an
         ' unordered or reverse ordered (from white to black) to 1 bit
         ' images with the FreeImage_Threshold method. Thresholding still
         ' needs a true linear ordered 8 bit greyscale palette. This first
         ' step of conversion is currently done by FreeImage properly only
         ' for images with bit depths different to 8 bits. The same is
         ' true for dithering.
         Select Case FreeImage_GetBPP(hDIB)

         Case 8
            ' FreeImage_Threshold only works for 8 bit images with a
            ' FIC_MINISBLACK palette, so if the color type differs, we
            ' need to convert the image to 24 bit before thresholding.
            If (FreeImage_GetColorType(hDIB) <> FIC_MINISBLACK) Then
               hDIBTemp = FreeImage_ConvertTo24Bits(hDIB)
               hDIBNew = FreeImage_Threshold(hDIBTemp, bThreshold)
               Call FreeImage_Unload(hDIBTemp)
            Else
               hDIBNew = FreeImage_Threshold(hDIB, bThreshold)
            End If

         Case Else
            ' any other bit depth is correctly converted by
            ' the FreeImage library
            hDIBNew = FreeImage_Threshold(hDIB, bThreshold)

         End Select
      
      Case FICF_MONOCHROME_DITHER
         ' FreeImage version 3.8.0 still has a minor bug when converting
         ' 8 bit palettized images or 8 bit greyscale images with an
         ' unordered or reverse ordered (from white to black) to 1 bit
         ' images with the FreeImage_Dither method. Dithering still
         ' needs a true linear ordered 8 bit greyscale palette. This first
         ' step of conversion is currently done by FreeImage properly only
         ' for images with bit depths different to 8 bits. The same is
         ' true for thresholding.
         Select Case FreeImage_GetBPP(hDIB)

         Case 8
            ' FreeImage_Dither only works for 8 bit images with a
            ' FIC_MINISBLACK palette, so if the color type differs, we
            ' need to convert the image to 24 bit before dithering.
            If (FreeImage_GetColorType(hDIB) <> FIC_MINISBLACK) Then
               hDIBTemp = FreeImage_ConvertTo24Bits(hDIB)
               hDIBNew = FreeImage_Dither(hDIBTemp, eDitherMethod)
               Call FreeImage_Unload(hDIBTemp)
            Else
               hDIBNew = FreeImage_Dither(hDIB, eDitherMethod)
            End If

         Case Else
            ' any other bit depth is correctly converted by
            ' the FreeImage library
            hDIBNew = FreeImage_Dither(hDIB, eDitherMethod)

         End Select
      
      Case FICF_GREYSCALE_4BPP
         If (FreeImage_GetBPP(hDIB) <> 4) Then
            hDIBNew = FreeImage_ConvertTo4Bits(hDIB)
         Else
            If (bKeepPalette) Then
               bConvertPalette = (Not FreeImage_IsGreyscaleImage(hDIB))
            Else
               bConvertPalette = (FreeImage_GetColorType(hDIB) = FIC_PALETTE)
            End If
            If (bConvertPalette) Then
               hDIBTemp = FreeImage_ConvertToGreyscale(hDIB)
               hDIBNew = FreeImage_ConvertTo4Bits(hDIBTemp)
               Call FreeImage_Unload(hDIBTemp)
            End If
         End If
            
      Case FICF_GREYSCALE_8BPP
         If (Not ((bKeepPalette) And _
                  (FreeImage_IsGreyscaleImage(hDIB)) And _
                  (FreeImage_GetBPP(hDIB) = 8))) Then
            hDIBNew = FreeImage_ConvertToGreyscale(hDIB)
         End If
         
      Case FICF_PALETTISED_8BPP
         ' check 'lPaletteSize' and adjust if needed
         If (lPaletteSize > 256) Then
            lPaletteSize = 256
         
         ElseIf (lPaletteSize < 2) Then
            lPaletteSize = 2
         
         End If
         
         bAdjustReservePaletteSize = (lReserveSize <= 0)
         
         ' check for reserve palette
         lpReservePalette = pGetMemoryBlockPtrFromVariant(vntReservePalette, _
                                                          lReserveSize)
                                                          
         If (bAdjustReservePaletteSize) Then
            lReserveSize = lReserveSize \ 4
         End If
         
         ' note, that the FreeImage library only quantizes 24 bit images
         lBPP = FreeImage_GetBPP(hDIB)
         ' do not convert any 8 bit images, as long as 'lPaletteSize' is 256
         If ((lBPP <> 8) Or _
             (lPaletteSize <> 256)) Then
            ' images with a color depth of 24 bits can directly be
            ' converted with the FreeImage_ColorQuantize function;
            ' other images need to be converted to 24 bits first
            If (lBPP = 24) Then
               If (lPaletteSize = 256) Then
                  hDIBNew = FreeImage_ColorQuantize(hDIB, eQuantizationMethod)
               Else
                  hDIBNew = FreeImage_ColorQuantizeEx(hDIB, eQuantizationMethod, _
                                                      lPaletteSize, lReserveSize, lpReservePalette)
               End If
            Else
               hDIBTemp = FreeImage_ConvertTo24Bits(hDIB)
               If (lPaletteSize = 256) Then
                  hDIBNew = FreeImage_ColorQuantize(hDIBTemp, eQuantizationMethod)
               Else
                  hDIBNew = FreeImage_ColorQuantizeEx(hDIBTemp, eQuantizationMethod, _
                                                      lPaletteSize, lReserveSize, lpReservePalette)
               End If
               Call FreeImage_Unload(hDIBTemp)
            End If
            
         End If
         
      Case FICF_RGB_15BPP
         If (FreeImage_GetBPP(hDIB) <> 15) Then
            hDIBNew = FreeImage_ConvertTo16Bits555(hDIB)
         End If
      
      Case FICF_RGB_16BPP
         If (FreeImage_GetBPP(hDIB) <> 16) Then
            hDIBNew = FreeImage_ConvertTo16Bits565(hDIB)
         End If
      
      Case FICF_RGB_24BPP
         If (FreeImage_GetBPP(hDIB) <> 24) Then
            hDIBNew = FreeImage_ConvertTo24Bits(hDIB)
         End If
      
      Case FICF_RGB_32BPP
         If (FreeImage_GetBPP(hDIB) <> 32) Then
            hDIBNew = FreeImage_ConvertTo32Bits(hDIB)
         End If
      
      Case FICF_PREPARE_RESCALE
         Select Case FreeImage_GetBPP(hDIB)
         
         Case 1
            hDIBNew = FreeImage_ConvertTo24Bits(hDIB)
         
         Case 4
            hDIBNew = FreeImage_ConvertTo8Bits(hDIB)
         
         End Select
      
      End Select
      
      If (hDIBNew) Then
         FreeImage_ConvertColorDepth = hDIBNew
         If (bUnloadSource) Then
            Call FreeImage_Unload(hDIB)
         End If
      Else
         FreeImage_ConvertColorDepth = hDIB
      End If
   
   End If

End Function

Public Function FreeImage_RescaleEx(ByVal hDIB As Long, _
                           Optional ByVal vntDstWidth As Variant, _
                           Optional ByVal vntDstHeight As Variant, _
                           Optional ByVal bIsPercentValue As Boolean, _
                           Optional ByVal bUnloadSource As Boolean, _
                           Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As Long
                     
Dim lNewWidth As Long
Dim lNewHeight As Long
Dim hDIBNew As Long

   ' This function is a easy-to-use wrapper for rescaling an image with the
   ' FreeImage library. It returns a pointer to a new rescaled DIB provided
   ' by FreeImage.
   
   ' The parameters 'vntDstWidth', 'vntDstHeight' and 'bIsPercentValue' control
   ' the size of the new image. Here, the function tries to fake something like
   ' overloading known from Java. It depends on the parameter's data type passed
   ' through the Variant, how the provided values for width and height are
   ' actually interpreted. The following rules apply:
   
   ' In general, non integer values are either interpreted as percent values or
   ' factors, the original image size will be multiplied with. The 'bIsPercentValue'
   ' parameter controls whether the values are percent values or factors. Integer
   ' values are always considered to be the direct new image size, not depending on
   ' the original image size. In that case, the 'bIsPercentValue' parameter has no
   ' effect. If one of the parameters is omitted, the image will not be resized in
   ' that direction (either in width or height) and keeps it's original size. It is
   ' possible to omit both, but that makes actually no sense.
   
   ' The following table shows some of possible data type and value combinations
   ' that might by used with that function: (assume an original image sized 100x100 px)
   
   ' Parameter         |  Values |  Values |  Values |  Values |     Values |
   ' ----------------------------------------------------------------------
   ' vntDstWidth       |    75.0 |    0.85 |     200 |     120 |      400.0 |
   ' vntDstHeight      |   120.0 |     1.3 |     230 |       - |      400.0 |
   ' bIsPercentValue   |    True |   False |    d.c. |    d.c. |      False | <- wrong option?
   ' ----------------------------------------------------------------------
   ' Result Size       |  75x120 |  85x130 | 200x230 | 120x100 |40000x40000 |
   ' Remarks           | percent |  factor |  direct |         |maybe not   |
   '                                                           |what you    |
   '                                                           |wanted, or? |
   
   ' The optional 'bUnloadSource' parameter is for unloading the original image, so
   ' you can "change" an image with this function rather than getting a new DIB
   ' pointer. There is no more need for a second DIB variable at the caller's site.
   
   ' Since this diversity my be confusing to VB developers, this function is also
   ' callable through three different functions called 'FreeImage_RescaleByPixel',
   ' 'FreeImage_RescaleByPercent' and 'FreeImage_RescaleByFactor'.

   If (Not IsMissing(vntDstWidth)) Then
      Select Case VarType(vntDstWidth)
      
      Case vbDouble, vbSingle, vbDecimal, vbCurrency
         lNewWidth = FreeImage_GetWidth(hDIB) * vntDstWidth
         If (bIsPercentValue) Then
            lNewWidth = lNewWidth / 100
         End If
      
      Case Else
         lNewWidth = vntDstWidth
      
      End Select
   End If
   
   If (Not IsMissing(vntDstHeight)) Then
      Select Case VarType(vntDstHeight)
      
      Case vbDouble, vbSingle, vbDecimal
         lNewHeight = FreeImage_GetHeight(hDIB) * vntDstHeight
         If (bIsPercentValue) Then
            lNewHeight = lNewHeight / 100
         End If
      
      Case Else
         lNewHeight = vntDstHeight
      
      End Select
   End If
   
   If ((lNewWidth > 0) And _
       (lNewHeight > 0)) Then
      hDIBNew = FreeImage_Rescale(hDIB, lNewWidth, lNewHeight, eFilter)
       
   ElseIf (lNewWidth > 0) Then
      lNewHeight = lNewWidth / (FreeImage_GetWidth(hDIB) / FreeImage_GetHeight(hDIB))
      hDIBNew = FreeImage_Rescale(hDIB, lNewWidth, lNewHeight, eFilter)
   
   ElseIf (lNewHeight > 0) Then
      lNewWidth = lNewHeight * (FreeImage_GetWidth(hDIB) / FreeImage_GetHeight(hDIB))
      hDIBNew = FreeImage_Rescale(hDIB, lNewWidth, lNewHeight, eFilter)
   
   End If
   
   If (hDIBNew) Then
      FreeImage_RescaleEx = hDIBNew
      If (bUnloadSource) Then
         Call FreeImage_Unload(hDIB)
      End If
   Else
      FreeImage_RescaleEx = hDIB
   End If
                     
End Function

Public Function FreeImage_RescaleByPixel(ByVal hDIB As Long, _
                                Optional ByVal lDstWidthPixel As Long, _
                                Optional ByVal lDstHeightPixel As Long, _
                                Optional ByVal bUnloadSource As Boolean, _
                                Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As Long
                                
   ' Thin wrapper for function 'FreeImage_RescaleEx' for removing method
   ' overload fake. This function rescales the image directly to the size
   ' specified by the 'lDstWidthPixel' and 'lDstHeightPixel' parameters.

   FreeImage_RescaleByPixel = FreeImage_RescaleEx(hDIB, _
                                                  lDstWidthPixel, _
                                                  lDstHeightPixel, _
                                                  False, _
                                                  bUnloadSource, _
                                                  eFilter)

End Function

Public Function FreeImage_RescaleByPercent(ByVal hDIB As Long, _
                                  Optional ByVal dblDstWidthPercent As Double, _
                                  Optional ByVal dblDstHeightPercent As Double, _
                                  Optional ByVal bUnloadSource As Boolean, _
                                  Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As Long

   ' Thin wrapper for function 'FreeImage_RescaleEx' for removing method
   ' overload fake. This function rescales the image by a percent value
   ' based on the image's original size.

   FreeImage_RescaleByPercent = FreeImage_RescaleEx(hDIB, _
                                                    dblDstWidthPercent, _
                                                    dblDstHeightPercent, _
                                                    True, _
                                                    bUnloadSource, _
                                                    eFilter)

End Function

Public Function FreeImage_RescaleByFactor(ByVal hDIB As Long, _
                                 Optional ByVal dblDstWidthFactor As Double, _
                                 Optional ByVal dblDstHeightFactor As Double, _
                                 Optional ByVal bUnloadSource As Boolean, _
                                 Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As Long

   ' Thin wrapper for function 'FreeImage_RescaleEx' for removing method
   ' overload fake. This function rescales the image by a factor
   ' based on the image's original size.

   FreeImage_RescaleByFactor = FreeImage_RescaleEx(hDIB, _
                                                   dblDstWidthFactor, _
                                                   dblDstHeightFactor, _
                                                   False, _
                                                   bUnloadSource, _
                                                   eFilter)

End Function

Public Function FreeImage_PaintDC(ByVal hDC As Long, _
                                  ByVal hDIB As Long, _
                         Optional ByVal lXDest As Long = 0, _
                         Optional ByVal lYDest As Long = 0, _
                         Optional ByVal lXSrc As Long = 0, _
                         Optional ByVal lYSrc As Long = 0, _
                         Optional ByVal lnSrcWidth As Long, _
                         Optional ByVal lnSrcHeight As Long, _
                         Optional ByVal eDrawMode As DRAW_MODE = DM_DRAW_DEFAULT, _
                         Optional ByVal eRasterOperator As RASTER_OPERATOR = ROP_SRCCOPY) As Long
 
Dim eLastStretchMode As STRETCH_MODE

   ' This function draws a FreeImage DIB directly onto a device context (DC). There
   ' are many (selfexplaining?) parameters that control the visual result.
   
   If ((hDC) And (hDIB)) Then
      
      If (lnSrcWidth = 0) Then
         lnSrcWidth = FreeImage_GetWidth(hDIB)
      End If
      
      If (lnSrcHeight = 0) Then
         lnSrcHeight = FreeImage_GetHeight(hDIB)
      End If
      
      If (eDrawMode And DM_MIRROR_VERTICAL) Then
         'lYDest = lnDestHeight
         'lnDestHeight = -lnDestHeight
      End If
     
      If (eDrawMode And DM_MIRROR_HORIZONTAL) Then
         'lXDest = lnDestWidth
         'lnDestWidth = -lnDestWidth
      End If
      lnSrcWidth = lnSrcWidth - 50
      lnSrcHeight = lnSrcHeight - 50
      lXSrc = 50
      lYSrc = 50
      
      'Debug.Print SetDIBitsToDevice(hDC, _
                             lXDest + 100, lYDest + 100, lnSrcWidth - lXSrc, lnSrcHeight - lYSrc, _
                             lXSrc, -lYSrc, lYSrc, lnSrcHeight, _
                             FreeImage_GetBits(hDIB), _
                             FreeImage_GetInfo(hDIB), _
                             DIB_RGB_COLORS)
   End If

End Function

Public Function FreeImage_PaintDCEx(ByVal hDC As Long, _
                                    ByVal hDIB As Long, _
                           Optional ByVal lXDest As Long = 0, _
                           Optional ByVal lYDest As Long = 0, _
                           Optional ByVal lnDestWidth As Long, _
                           Optional ByVal lnDestHeight As Long, _
                           Optional ByVal lXSrc As Long = 0, _
                           Optional ByVal lYSrc As Long = 0, _
                           Optional ByVal lnSrcWidth As Long, _
                           Optional ByVal lnSrcHeight As Long, _
                           Optional ByVal eDrawMode As DRAW_MODE = DM_DRAW_DEFAULT, _
                           Optional ByVal eRasterOperator As RASTER_OPERATOR = ROP_SRCCOPY, _
                           Optional ByVal eStretchMode As STRETCH_MODE = SM_COLORONCOLOR) As Long

Dim eLastStretchMode As STRETCH_MODE

   ' This function draws a FreeImage DIB directly onto a device context (DC). There
   ' are many (selfexplaining?) parameters that control the visual result.
   
   ' The main differences of this function compared to the 'FreeImage_PaintDC' are,
   ' that this function supports mirroring and stretching of the image to be
   ' painted and so, is somewhat slower than 'FreeImage_PaintDC'.
   
   If ((hDC) And (hDIB)) Then
      
      eLastStretchMode = GetStretchBltMode(hDC)
      Call SetStretchBltMode(hDC, eStretchMode)
      
      If (lnSrcWidth = 0) Then
         lnSrcWidth = FreeImage_GetWidth(hDIB)
      End If
      If (lnDestWidth = 0) Then
         lnDestWidth = lnSrcWidth
      End If
      
      If (lnSrcHeight = 0) Then
         lnSrcHeight = FreeImage_GetHeight(hDIB)
      End If
      If (lnDestHeight = 0) Then
         lnDestHeight = lnSrcHeight
      End If
      
      If (eDrawMode And DM_MIRROR_VERTICAL) Then
         lYDest = lnDestHeight
         lnDestHeight = -lnDestHeight
      End If
     
      If (eDrawMode And DM_MIRROR_HORIZONTAL) Then
         lXDest = lnDestWidth
         lnDestWidth = -lnDestWidth
      End If

      Call StretchDIBits(hDC, _
                         lXDest, lYDest, lnDestWidth, lnDestHeight, _
                         lXSrc, lYSrc, lnSrcWidth, lnSrcHeight, _
                         FreeImage_GetBits(hDIB), _
                         FreeImage_GetInfo(hDIB), _
                         DIB_RGB_COLORS, _
                         eRasterOperator)
      
      Call SetStretchBltMode(hDC, eLastStretchMode)
   End If

End Function



'--------------------------------------------------------------------------------
' Pixel access functions
'--------------------------------------------------------------------------------

Public Function FreeImage_GetBitsEx(ByVal dib As Long) As Byte()

Dim tSA As SAVEARRAY2D
Dim lpSA As Long

   ' This function returns a two dimensional Byte array containing a DIB's
   ' data-bits. This is done by wrapping a true VB array around the memory
   ' block the returned pointer of 'FreeImage_GetBits' is pointing to. So, the
   ' array returned provides full read and write acces to the image's data.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   If (dib) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 1                           ' size in bytes per array element
         .cDims = 2                                ' the array has 2 dimensions
         .cElements1 = FreeImage_GetHeight(dib)    ' the number of elements in y direction (height of dib)
         .cElements2 = FreeImage_GetPitch(dib)     ' the number of elements in x direction (byte width of dib)
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetBits(dib)          ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' allocate memory for an array descriptor
      ' we cannot use the memory block used by tSA, since it is
      ' released when tSA goes out of scope, leaving us with an
      ' array with zeroed descriptor
      ' we use nearly the same method that VB uses, so VB is able
      ' to cleanup the array variable and it's descriptor; the
      ' array data is not touched when cleaning up, since both AUTO
      ' and FIXEDSIZE flags are set
      Call SafeArrayAllocDescriptor(1, lpSA)
      
      ' copy our own array descriptor over the descriptor allocated
      ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
      ' location
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      
      ' the implicit variable named like the function is an array
      ' variable in VB
      ' make it point to the allocated array descriptor
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetBitsEx), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetBitsExRGBTRIPLE(ByVal dib As Long) As RGBTRIPLE()

Dim tSA As SAVEARRAY2D
Dim lpSA As Long

   ' This function returns a two dimensional RGBTRIPLE array containing a DIB's
   ' data-bits. This is done by wrapping a true VB array around the memory
   ' block the returned pointer of 'FreeImage_GetBits' is pointing to. So, the
   ' array returned provides full read and write acces to the image's data.
   
   ' This function only works with 24 bpp images and, since each FreeImage scanline
   ' is aligned to a 32-bit boundary, only if the image's width in pixels multiplied
   ' by three modulo four is zero. That means, that the image layout in memory must
   ' "naturally" be aligned to a 32-bit boundary, since arrays do not support padding.
   
   ' So, the function only returns an initialized array, if this equotion is true:
   ' (((ImageWidthPixels * 3) Mod 4) = 0)
   
   ' In other words, this is true for all images with no padding.
   
   ' For instance, only images with these widths will be suitable for this function:
   ' 100, 104, 108, 112, 116, 120, 124, ...

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   If (dib) Then
      
      If (FreeImage_GetBPP(dib) = 24) Then
         If (((FreeImage_GetWidth(dib) * 3) Mod 4) = 0) Then
         
            ' create a proper SAVEARRAY descriptor
            With tSA
               .cbElements = 3                           ' size in bytes per array element
               .cDims = 2                                ' the array has 2 dimensions
               .cElements1 = FreeImage_GetHeight(dib)    ' the number of elements in y direction (height of dib)
               .cElements2 = FreeImage_GetWidth(dib)     ' the number of elements in x direction (byte width of dib)
               .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                         ' so the array can not be modified in size
                                                         ' or erased; according to Matthew Curland never
                                                         ' use FIXEDSIZE alone
               .pvData = FreeImage_GetBits(dib)          ' let the array point to the memory block, the
                                                         ' FreeImage scanline data pointer points to
            End With
            
            ' allocate memory for an array descriptor
            ' we cannot use the memory block used by tSA, since it is
            ' released when tSA goes out of scope, leaving us with an
            ' array with zeroed descriptor
            ' we use nearly the same method that VB uses, so VB is able
            ' to cleanup the array variable and it's descriptor; the
            ' array data is not touched when cleaning up, since both AUTO
            ' and FIXEDSIZE flags are set
            Call SafeArrayAllocDescriptor(1, lpSA)
            
            ' copy our own array descriptor over the descriptor allocated
            ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
            ' location
            Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
            
            ' the implicit variable named like the function is an array
            ' variable in VB
            ' make it point to the allocated array descriptor
            Call CopyMemory(ByVal VarPtrArray(FreeImage_GetBitsExRGBTRIPLE), lpSA, 4)
         Else
         
            ' we could throw an error here
         End If
      Else
      
         ' we could throw an error here
      End If
   End If

End Function

Public Function FreeImage_GetBitsExRGBQUAD(ByVal dib As Long) As RGBQUAD()

Dim tSA As SAVEARRAY2D
Dim lpSA As Long

   ' This function returns a two dimensional RGBQUAD array containing a DIB's
   ' data-bits. This is done by wrapping a true VB array around the memory
   ' block the returned pointer of 'FreeImage_GetBits' is pointing to. So, the
   ' array returned provides full read and write acces to the image's data.
   
   ' This function only works with 32 bpp images. Since each scanline must
   ' "naturally" start at a 32-bit boundary if each pixel uses 32 bits, there
   ' are no padding problems like these known with 'FreeImage_GetBitsExRGBTRIPLE',
   ' so, this function is suitable for all 32 bpp images of any size.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   If (dib) Then
   
      If (FreeImage_GetBPP(dib) = 24) Then
      
         ' create a proper SAVEARRAY descriptor
         With tSA
            .cbElements = 4                           ' size in bytes per array element
            .cDims = 2                                ' the array has 2 dimensions
            .cElements1 = FreeImage_GetHeight(dib)    ' the number of elements in y direction (height of dib)
            .cElements2 = FreeImage_GetWidth(dib)     ' the number of elements in x direction (byte width of dib)
            .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                      ' so the array can not be modified in size
                                                      ' or erased; according to Matthew Curland never
                                                      ' use FIXEDSIZE alone
            .pvData = FreeImage_GetBits(dib)          ' let the array point to the memory block, the
                                                      ' FreeImage scanline data pointer points to
         End With
         
         ' allocate memory for an array descriptor
         ' we cannot use the memory block used by tSA, since it is
         ' released when tSA goes out of scope, leaving us with an
         ' array with zeroed descriptor
         ' we use nearly the same method that VB uses, so VB is able
         ' to cleanup the array variable and it's descriptor; the
         ' array data is not touched when cleaning up, since both AUTO
         ' and FIXEDSIZE flags are set
         Call SafeArrayAllocDescriptor(1, lpSA)
         
         ' copy our own array descriptor over the descriptor allocated
         ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
         ' location
         Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
         
         ' the implicit variable named like the function is an array
         ' variable in VB
         ' make it point to the allocated array descriptor
         Call CopyMemory(ByVal VarPtrArray(FreeImage_GetBitsExRGBQUAD), lpSA, 4)
      Else
      
         ' we could throw an error here
      End If
   End If

End Function

Public Function FreeImage_GetScanLineEx(ByVal dib As Long, _
                                        ByVal scanline As Long) As Byte()
                                        
Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a one dimensional Byte array containing a whole
   ' scanline's data-bits. This is done by wrapping a true VB array around
   ' the memory block the returned pointer of 'FreeImage_GetScanline' is
   ' pointing to. So, the array returned provides full read and write acces
   ' to the image's data.
   
   ' This is the most generic function of a complete function set dealing with
   ' scanline data, since this function returns an array of type Byte. It is
   ' up to the caller of the function to interpret these bytes correctly,
   ' according to the results of FreeImage_GetBPP and FreeImage_GetImageType.
   
   ' You may consider using any of the non-generic functions named
   ' 'FreeImage_GetScanLineXXX', that return an array of proper type, according
   ' to the images bit depth and type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.
   
   If (dib) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 1                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetLine(dib)       ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' allocate memory for an array descriptor
      ' we cannot use the memory block used by tSA, since it is
      ' released when tSA goes out of scope, leaving us with an
      ' array with zeroed descriptor
      ' we use nearly the same method that VB uses, so VB is able
      ' to cleanup the array variable and it's descriptor; the
      ' array data is not touched when cleaning up, since both AUTO
      ' and FIXEDSIZE flags are set
      Call SafeArrayAllocDescriptor(1, lpSA)
      
      ' copy our own array descriptor over the descriptor allocated
      ' by SafeArrayAllocDescriptor; lpSA is a pointer to that memory
      ' location
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      
      ' the implicit variable named like the function is an array
      ' variable in VB
      ' make it point to the allocated array descriptor
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineEx), lpSA, 4)
   End If
                                        
End Function

Public Function FreeImage_GetScanLineBITMAP8(ByVal dib As Long, _
                                             ByVal scanline As Long) As Byte()
                                             
   ' This function returns a one dimensional Byte array containing a whole
   ' scanline's data-bits of a 8 bit bitmap image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned provides
   ' full read and write acces to the image's data.
   
   ' This function is just a thin wrapper for 'FreeImage_GetScanLineEx' but
   ' includes checking of the image's bit depth and type, as all of the
   ' non-generic scanline functions do.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   If (FreeImage_GetImageType(dib) = FIT_BITMAP) Then
      Select Case FreeImage_GetBPP(dib)
      
      Case 1, 4, 8
         FreeImage_GetScanLineBITMAP8 = FreeImage_GetScanLineEx(dib, scanline)
         
      End Select
   End If

End Function

Public Function FreeImage_GetScanLineBITMAP16(ByVal dib As Long, _
                                              ByVal scanline As Long) As Integer()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a one dimensional Integer array containing a whole
   ' scanline's data-bits of a 16 bit bitmap image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   If (FreeImage_GetImageType(dib) = FIT_BITMAP) Then
      If (FreeImage_GetBPP(dib) = 16) Then
      
         ' create a proper SAVEARRAY descriptor
         With tSA
            .cbElements = 2                           ' size in bytes per array element
            .cDims = 1                                ' the array has only 1 dimension
            .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
            .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                      ' so the array can not be modified in size
                                                      ' or erased; according to Matthew Curland never
                                                      ' use FIXEDSIZE alone
            .pvData = FreeImage_GetScanline(dib, _
                                            scanline) ' let the array point to the memory block, the
                                                      ' FreeImage scanline data pointer points to
         End With
         
         ' For a complete source code documentation have a
         ' look at the funciton 'FreeImage_GetScanLineEx'
         Call SafeArrayAllocDescriptor(1, lpSA)
         Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
         Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineBITMAP16), lpSA, 4)
      End If
   End If

End Function

Public Function FreeImage_GetScanLineBITMAP24(ByVal dib As Long, _
                                              ByVal scanline As Long) As RGBTRIPLE()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a one dimensional RGBTRIPLE array containing a whole
   ' scanline's data-bits of a 24 bit bitmap image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayRGBTRIPLE' function.

   If (FreeImage_GetImageType(dib) = FIT_BITMAP) Then
      If (FreeImage_GetBPP(dib) = 24) Then
      
         ' create a proper SAVEARRAY descriptor
         With tSA
            .cbElements = 3                           ' size in bytes per array element
            .cDims = 1                                ' the array has only 1 dimension
            .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
            .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                      ' so the array can not be modified in size
                                                      ' or erased; according to Matthew Curland never
                                                      ' use FIXEDSIZE alone
            .pvData = FreeImage_GetScanline(dib, _
                                            scanline) ' let the array point to the memory block, the
                                                      ' FreeImage scanline data pointer points to
         End With
         
         ' For a complete source code documentation have a
         ' look at the funciton 'FreeImage_GetScanLineEx'
         Call SafeArrayAllocDescriptor(1, lpSA)
         Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
         Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineBITMAP24), lpSA, 4)
      End If
   End If

End Function

Public Function FreeImage_GetScanLineBITMAP32(ByVal dib As Long, _
                                              ByVal scanline As Long) As RGBQUAD()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long

   ' This function returns a one dimensional RGBQUAD array containing a whole
   ' scanline's data-bits of a 32 bit bitmap image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayRGBQUAD' function.

   If (FreeImage_GetImageType(dib) = FIT_BITMAP) Then
      If (FreeImage_GetBPP(dib) = 32) Then
      
         ' create a proper SAVEARRAY descriptor
         With tSA
            .cbElements = 4                           ' size in bytes per array element
            .cDims = 1                                ' the array has only 1 dimension
            .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
            .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                      ' so the array can not be modified in size
                                                      ' or erased; according to Matthew Curland never
                                                      ' use FIXEDSIZE alone
            .pvData = FreeImage_GetScanline(dib, _
                                            scanline) ' let the array point to the memory block, the
                                                      ' FreeImage scanline data pointer points to
         End With
         
         ' For a complete source code documentation have a
         ' look at the funciton 'FreeImage_GetScanLineEx'
         Call SafeArrayAllocDescriptor(1, lpSA)
         Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
         Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineBITMAP32), lpSA, 4)
      End If
   End If

End Function

Public Function FreeImage_GetScanLineINT16(ByVal dib As Long, _
                                           ByVal scanline As Long) As Integer()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional Integer array containing a whole
   ' scanline's data-bits of a FIT_INT16 or FIT_UINT16 image. This is done
   ' by wrapping a true VB array around the memory block the returned pointer
   ' of 'FreeImage_GetScanline' is pointing to. So, the array returned
   ' provides full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.
   
   ' Since VB does not distinguish between signed and unsigned data types, both
   ' image types FIT_INT16 and FIT_UINT16 are handled with this function. If 'dib'
   ' specifies an image of type FIT_UINT16, it is up to the caller to treat the
   ' array's Integers as unsigned, although VB knows signed Integers only.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   eImageType = FreeImage_GetImageType(dib)
   If ((eImageType = FIT_INT16) Or _
       (eImageType = FIT_UINT16)) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 2                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineINT16), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineINT32(ByVal dib As Long, _
                                           ByVal scanline As Long) As Long()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional Long array containing a whole
   ' scanline's data-bits of a FIT_INT32 or FIT_UINT32 image. This is done
   ' by wrapping a true VB array around the memory block the returned pointer
   ' of 'FreeImage_GetScanline' is pointing to. So, the array returned
   ' provides full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.
   
   ' Since VB does not distinguish between signed and unsigned data types, both
   ' image types FIT_INT32 and FIT_UINT32 are handled with this function. If 'dib'
   ' specifies an image of type FIT_UINT32, it is up to the caller to treat the
   ' array's Longs as unsigned, although VB knows signed Longs only.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   eImageType = FreeImage_GetImageType(dib)
   If ((eImageType = FIT_INT32) Or _
       (eImageType = FIT_UINT32)) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 4                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineINT32), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineFLOAT(ByVal dib As Long, _
                                           ByVal scanline As Long) As Single()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional Single array containing a whole
   ' scanline's data-bits of a FIT_FLOAT image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_FLOAT) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 4                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineFLOAT), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineDOUBLE(ByVal dib As Long, _
                                            ByVal scanline As Long) As Double()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional Double array containing a whole
   ' scanline's data-bits of a FIT_DOUBLE image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.
   
   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArray' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_DOUBLE) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 8                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineDOUBLE), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineCOMPLEX(ByVal dib As Long, _
                                             ByVal scanline As Long) As FICOMPLEX()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional FICOMPLEX array containing a whole
   ' scanline's data-bits of a FIT_COMPLEX image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayFICOMPLEX' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_COMPLEX) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 16                          ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineCOMPLEX), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineRGB16(ByVal dib As Long, _
                                           ByVal scanline As Long) As FIRGB16()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional FIRGB16 array containing a whole
   ' scanline's data-bits of a FIT_RGB16 image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayFIRGB16' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_RGB16) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 6                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineRGB16), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineRGBA16(ByVal dib As Long, _
                                            ByVal scanline As Long) As FIRGBA16()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional FIRGBA16 array containing a whole
   ' scanline's data-bits of a FIT_RGBA16 image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayFIRGBA16' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_RGBA16) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 8                           ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineRGBA16), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineRGBF(ByVal dib As Long, _
                                          ByVal scanline As Long) As FIRGBF()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional FIRGBF array containing a whole
   ' scanline's data-bits of a FIT_RGBF image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayFIRGBF' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_RGBF) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 12                          ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineRGBF), lpSA, 4)
   End If

End Function

Public Function FreeImage_GetScanLineRGBAF(ByVal dib As Long, _
                                           ByVal scanline As Long) As FIRGBAF()

Dim tSA As SAVEARRAY1D
Dim lpSA As Long
Dim eImageType As FREE_IMAGE_TYPE

   ' This function returns a one dimensional FIRGBAF array containing a whole
   ' scanline's data-bits of a FIT_RGBAF image. This is done by wrapping
   ' a true VB array around the memory block the returned pointer of
   ' 'FreeImage_GetScanline' is pointing to. So, the array returned  provides
   ' full read and write acces to the image's data.
   
   ' The function includes checking of the image's bit depth and type and
   ' returns a non-initialized array if 'dib' is an image of inproper type.

   ' To reuse the caller's array variable, this function's result was assigned to,
   ' before it goes out of scope, the caller's array variable must be destroyed with
   ' the 'FreeImage_DestroyLockedArrayFIRGBAF' function.

   eImageType = FreeImage_GetImageType(dib)
   If (eImageType = FIT_RGBAF) Then
      
      ' create a proper SAVEARRAY descriptor
      With tSA
         .cbElements = 12                          ' size in bytes per array element
         .cDims = 1                                ' the array has only 1 dimension
         .cElements = FreeImage_GetWidth(dib)      ' the number of elements in the array
         .fFeatures = FADF_AUTO Or FADF_FIXEDSIZE  ' need AUTO and FIXEDSIZE for safety issues,
                                                   ' so the array can not be modified in size
                                                   ' or erased; according to Matthew Curland never
                                                   ' use FIXEDSIZE alone
         .pvData = FreeImage_GetScanline(dib, _
                                         scanline) ' let the array point to the memory block, the
                                                   ' FreeImage scanline data pointer points to
      End With
      
      ' For a complete source code documentation have a
      ' look at the funciton 'FreeImage_GetScanLineEx'
      Call SafeArrayAllocDescriptor(1, lpSA)
      Call CopyMemory(ByVal lpSA, tSA, Len(tSA))
      Call CopyMemory(ByVal VarPtrArray(FreeImage_GetScanLineRGBAF), lpSA, 4)
   End If

End Function


'--------------------------------------------------------------------------------
' OlePicture conversion functions
'--------------------------------------------------------------------------------

Public Function FreeImage_GetOlePicture(ByVal hDIB As Long, _
                               Optional ByVal hDC As Long, _
                               Optional ByVal bUnloadSource As Boolean) As IPicture

Dim bReleaseDC As Boolean
Dim hBMP As Long
Dim tPicDesc As PictDesc
Dim tGuid As Guid
Dim cPictureDisp As IPictureDisp

   ' This function creates a VB Picture object (OlePicture) from a FreeImage DIB.
   ' The original image must not remain valid nor loaded after the VB Picture
   ' object has been created.
   
   ' The optional parameter 'hDC' determines the device context (DC) used for
   ' transforming the device independent bitmap (DIB) to a device dependent
   ' bitmap (DDB). This device context's color depth is responsible for this
   ' transformation. This parameter may be null or omitted. In that case, the
   ' windows desktop's device context will be used, what will be the desired
   ' way in almost any cases.
   
   ' The optional 'bUnloadSource' parameter is for unloading the original image
   ' after the OlePicture has been created, so you can easiely "switch" from a
   ' FreeImage DIB to a VB Picture object. There is no need to clean up the DIB
   ' at the caller's site.
   
   If (hDIB) Then
   
      If (hDC = 0) Then
         hDC = GetDC(0)
         bReleaseDC = True
      End If
      
      If (hDC) Then
         
         hBMP = CreateDIBitmap(hDC, _
                               FreeImage_GetInfoHeader(hDIB), _
                               CBM_INIT, _
                               FreeImage_GetBits(hDIB), _
                               FreeImage_GetInfo(hDIB), _
                               DIB_RGB_COLORS)
         If (hBMP) Then
   
            ' fill tPictDesc structure with necessary parts
            With tPicDesc
               .cbSizeofStruct = Len(tPicDesc)
               .picType = vbPicTypeBitmap
               .hImage = hBMP
            End With
   
            ' fill in IDispatch Interface ID
            With tGuid
               .Data1 = &H20400
               .Data4(0) = &HC0
               .Data4(7) = &H46
            End With
   
            ' create a picture object
            Call OleCreatePictureIndirect(tPicDesc, tGuid, True, cPictureDisp)
            Set FreeImage_GetOlePicture = cPictureDisp
         End If
      End If

      If (bReleaseDC) Then
         Call ReleaseDC(0, hDC)
      End If
      
      If (bUnloadSource) Then
         Call FreeImage_Unload(hDIB)
      End If
   End If

End Function

Public Function FreeImage_CreateFromOlePicture(ByRef IOlePicture As IPicture) As Long

Dim hBMP As Long
Dim tBM As BITMAP
Dim hDIB As Long
Dim hDC As Long
Dim lResult As Long

   ' Creates a FreeImage DIB from a VB Picture object (OlePicture). This functions
   ' returns a pointer to the DIB as, for instance, the FreeImage function
   ' 'FreeImage_Load' does. So, this could be a real replacement for 'FreeImage_Load'
   ' when working with VB Picture objects.

   If (Not IOlePicture Is Nothing) Then
      hBMP = IOlePicture.Handle
      If (hBMP) Then
         lResult = GetObjectAPI(hBMP, Len(tBM), tBM)
         If (lResult) Then
            hDIB = FreeImage_Allocate(tBM.bmWidth, _
                                      tBM.bmHeight, _
                                      tBM.bmBitsPixel)
            If (hDIB) Then
               hDC = GetDC(0)
               lResult = GetDIBits(hDC, hBMP, 0, _
                                   FreeImage_GetHeight(hDIB), _
                                   FreeImage_GetBits(hDIB), _
                                   FreeImage_GetInfo(hDIB), _
                                   DIB_RGB_COLORS)
               If (lResult) Then
                  FreeImage_CreateFromOlePicture = hDIB
               Else
                  Call FreeImage_Unload(hDIB)
               End If
               Call ReleaseDC(0, hDC)
            End If
         End If
      End If
   End If

End Function

Public Function FreeImage_AdjustPictureBox(ByRef pbox As Object, _
                                  Optional ByVal adjust_mode As FREE_IMAGE_ADJUST_MODE, _
                                  Optional ByVal Filter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture

Dim tR As RECT
Dim hDIB As Long
Dim hDIBTemp As Long
Dim lNewWidth As Long
Dim lNewHeight As Long

Const vbObjectOrWithBlockVariableNotSet As Long = 91

   ' This function adjusts an already loaded picture in a VB PictureBox
   ' control in size. This is done by converting the picture to a dib
   ' by FreeImage_CreateFromOlePicture. After resizing the dib it is
   ' converted back to a Ole Picture object and re-assigned to the
   ' PictureBox control.
   
   ' The pbox paramater is actually of type Object so any object or control
   ' providing Picture, hWnd, Width and Height properties can be used instead
   ' of a PictureBox control
   
   ' This may be useful when using compile time provided images in VB like
   ' logos or backgrounds that need to be resized during runtime. Using
   ' FreeImage's sophisticated rescaling methods is a much better aproach
   ' than using VB's stretchable Image control.
   
   ' One reason for resizing a usually fixed size logo or background image
   ' may be the following scenario:
   
   ' When running on a Windos machine using smaller or bigger fonts (what can
   ' be configured in the control panel by using different dpi fonts), the
   ' operation system automatically adjusts the sizes of Forms, Labels,
   ' TextBoxes, Frames and even PictureBoxes. So, the hole VB application is
   ' perfectly adapted to these font metrics with the exception of compile time
   ' provided images. Although the PictureBox control is resized, the containing
   ' image remains untouched. This problem could be solved with this function.
   
   ' This function is also wrapped by the function 'AdjustPicture', giving you
   ' a more VB common function name.
   

   If (Not pbox Is Nothing) Then
      Call GetClientRect(pbox.hWnd, tR)
      If ((tR.Right <> pbox.Picture.Width) Or _
          (tR.Bottom <> pbox.Picture.Height)) Then
         hDIB = FreeImage_CreateFromOlePicture(pbox.Picture)
         If (hDIB) Then
            hDIB = FreeImage_ConvertColorDepth(hDIB, FICF_PREPARE_RESCALE, True)
            
            If (adjust_mode = AM_ADJUST_OPTIMAL_SIZE) Then
               If (pbox.Picture.Width >= pbox.Picture.Height) Then
                  adjust_mode = AM_ADJUST_WIDTH
               Else
                  adjust_mode = AM_ADJUST_HEIGHT
               End If
            End If
            
            Select Case adjust_mode
            
            Case AM_STRECH
               lNewWidth = tR.Right
               lNewHeight = tR.Bottom
               
            Case AM_ADJUST_WIDTH
               lNewWidth = tR.Right
               lNewHeight = lNewWidth / (pbox.Picture.Width / pbox.Picture.Height)
               
            Case AM_ADJUST_HEIGHT
               lNewHeight = tR.Bottom
               lNewWidth = lNewHeight * (pbox.Picture.Width / pbox.Picture.Height)
            
            End Select
            
            hDIBTemp = hDIB
            hDIB = FreeImage_Rescale(hDIB, lNewWidth, lNewHeight, Filter)
            Call FreeImage_Unload(hDIBTemp)
            Set pbox.Picture = FreeImage_GetOlePicture(hDIB, , True)
            Set FreeImage_AdjustPictureBox = pbox.Picture
         End If
      End If
   Else
      Call Err.Raise(vbObjectOrWithBlockVariableNotSet)
   End If

End Function

Public Function AdjustPicture(ByRef Control As Object, _
                     Optional ByRef Mode As FREE_IMAGE_ADJUST_MODE = AM_DEFAULT, _
                     Optional ByRef Filter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture
                     
   ' This function is a more VB friendly signed wrapper for
   ' the FreeImage_AdjustPictureBox function.
   
   Set AdjustPicture = FreeImage_AdjustPictureBox(Control, Mode, Filter)

End Function

Public Function LoadPictureEx(Optional ByRef FileName As Variant, _
                              Optional ByRef Width As Variant, _
                              Optional ByRef Height As Variant, _
                              Optional ByRef InPercent As Boolean = False, _
                              Optional ByRef Filter As FREE_IMAGE_FILTER, _
                              Optional ByRef Format As FREE_IMAGE_FORMAT = FIF_UNKNOWN) As IPicture

Dim hDIB As Long
Dim hDIBTmp As Long

Const vbInvalidPictureError As Long = 481

   ' This function is an extended version of the VB method 'LoadPicture'. As
   ' the VB version it takes a filename parameter to load the image and throws
   ' the same errors in most cases.
   
   ' The function provides all image formats, the FreeImage library can read. The
   ' image format is determined from the image file to load, the optional parameter
   ' 'Format' is an OUT parameter that will contain the image format that has
   ' been loaded.
   
   ' The parameters 'Width', 'Height', 'InPercent' and 'Filter' make it possible
   ' to "load" the image in a resized version. 'Width', 'Height' specify the desired
   ' width and height, 'Filter' determines, what image filter should be used
   ' on the resizing process.
   
   ' The parameters 'Width', 'Height', 'InPercent' and 'Filter' map directly to the
   ' according parameters of the 'FreeImage_RescaleEx' function. So, read the
   ' documentation of the 'FreeImage_RescaleEx' for a complete understanding of the
   ' usage of these parameters.


   If (Not IsMissing(FileName)) Then
      Format = FreeImage_GetFIFFromFilename(FileName)
      If (Format <> FIF_UNKNOWN) Then
         If (FreeImage_FIFSupportsReading(Format)) Then
            hDIB = FreeImage_Load(Format, FileName)
            If (hDIB) Then
               
               If ((Not IsMissing(Width)) Or _
                   (Not IsMissing(Height))) Then
                  
                  hDIB = FreeImage_ConvertColorDepth(hDIB, FICF_PREPARE_RESCALE, True)
                  hDIB = FreeImage_RescaleEx(hDIB, Width, Height, InPercent, True, Filter)
               End If
            
               Set LoadPictureEx = FreeImage_GetOlePicture(hDIB, , True)
               If (LoadPictureEx Is Nothing) Then
                  Call Err.Raise(vbInvalidPictureError)
               End If
            Else
               Call Err.Raise(vbInvalidPictureError)
            End If
         Else
            Call Err.Raise(vbInvalidPictureError)
         End If
      Else
         Call Err.Raise(vbInvalidPictureError)
      End If
   End If

End Function

Public Function SavePictureEx(ByRef Picture As IPicture, _
                              ByRef FileName As String, _
                     Optional ByRef Format As FREE_IMAGE_FORMAT = FIF_UNKNOWN, _
                     Optional ByRef Options As FREE_IMAGE_SAVE_OPTIONS = FISO_SAVE_DEFAULT, _
                     Optional ByRef Width As Variant, _
                     Optional ByRef Height As Variant, _
                     Optional ByRef InPercent As Boolean = False, _
                     Optional ByRef Filter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As Long
                     
Dim hDIB As Long
Dim strExtension As String

Const vbObjectOrWithBlockVariableNotSet As Long = 91
Const vbInvalidPictureError As Long = 481

   ' This function is an extended version of the VB method 'SavePicture'. As
   ' the VB version it takes a Picture object and a filename parameter to
   ' save the image and throws the same errors in most cases.
   
   ' The function provides all image formats, and save options, the FreeImage
   ' library can write. The optional parameter 'Format' may contain the desired
   ' image format. When omitted, the function tries to get the image format from
   ' the filename extension.
   
   ' The function checks, whether the given filename has a valid extension or
   ' not. If not, the "primary" extension for the used image format will be
   ' appended to the filename. The parameter 'FileName' remains untouched in
   ' this case.
   
   ' To learn more about the "primary" extension, read the documentation for
   ' the 'FreeImage_GetPrimaryExtensionFromFIF' function.
   
   ' The parameters 'Width', 'Height', 'InPercent' and 'Filter' make it possible
   ' to save the image in a resized version. 'Width', 'Height' specify the desired
   ' width and height, 'Filter' determines, what image filter should be used
   ' on the resizing process.
   
   ' The parameters 'Width', 'Height', 'InPercent' and 'Filter' map directly to the
   ' according parameters of the 'FreeImage_RescaleEx' function. So, read the
   ' documentation of the 'FreeImage_RescaleEx' for a complete understanding of the
   ' usage of these parameters.
                         
   If (Not Picture Is Nothing) Then
      hDIB = FreeImage_CreateFromOlePicture(Picture)
      If (hDIB) Then
      
         If ((Not IsMissing(Width)) Or _
             (Not IsMissing(Height))) Then
             
            hDIB = FreeImage_ConvertColorDepth(hDIB, FICF_PREPARE_RESCALE, True)
            hDIB = FreeImage_RescaleEx(hDIB, Width, Height, InPercent, True, Filter)
         End If
         
         If (Format = FIF_UNKNOWN) Then
            Format = FreeImage_GetFIFFromFilename(FileName)
         End If
         If (Format <> FIF_UNKNOWN) Then
            If (Not FreeImage_IsFilenameValidForFIF(Format, FileName)) Then
               strExtension = "." & FreeImage_GetPrimaryExtensionFromFIF(Format)
            End If
            SavePictureEx = FreeImage_Save(Format, hDIB, FileName & strExtension, Options)
         Else
            ' unknown image format error
         End If
      Else
         Call Err.Raise(vbInvalidPictureError)
      End If
   Else
      Call Err.Raise(vbObjectOrWithBlockVariableNotSet)
   End If

End Function


'--------------------------------------------------------------------------------
' OlePicture aware toolkit, rescale and conversion functions
'--------------------------------------------------------------------------------

Public Function FreeImage_RescaleIOP(ByRef IOlePicture As IPicture, _
                            Optional ByVal vntDstWidth As Variant, _
                            Optional ByVal vntDstHeight As Variant, _
                            Optional ByVal bIsPercentValue As Boolean, _
                            Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture
                            
Dim hDIB As Long

   ' IOlePicture based wrapper for wrapper function FreeImage_RescaleEx()

   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      hDIB = FreeImage_ConvertColorDepth(hDIB, FICF_PREPARE_RESCALE, True)
      hDIB = FreeImage_RescaleEx(hDIB, vntDstWidth, vntDstHeight, bIsPercentValue, True, eFilter)
      Set FreeImage_RescaleIOP = FreeImage_GetOlePicture(hDIB, , True)
   End If

End Function

Public Function FreeImage_RescaleByPixelIOP(ByRef IOlePicture As IPicture, _
                                   Optional ByVal lDstWidthPixel As Long, _
                                   Optional ByVal lDstHeightPixel As Long, _
                                   Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture
                                
   ' Thin wrapper for function 'FreeImage_RescaleExIOP' for removing method
   ' overload fake. This function rescales the image directly to the size
   ' specified by the 'lDstWidthPixel' and 'lDstHeightPixel' parameters.

   Set FreeImage_RescaleByPixelIOP = FreeImage_RescaleIOP(IOlePicture, _
                                                          lDstWidthPixel, _
                                                          lDstHeightPixel, _
                                                          False, _
                                                          eFilter)

End Function

Public Function FreeImage_RescaleByPercentIOP(ByRef IOlePicture As IPicture, _
                                     Optional ByVal dblDstWidthPercent As Double, _
                                     Optional ByVal dblDstHeightPercent As Double, _
                                     Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture

   ' Thin wrapper for function 'FreeImage_RescaleExIOP' for removing method
   ' overload fake. This function rescales the image by a percent value
   ' based on the image's original size.

   Set FreeImage_RescaleByPercentIOP = FreeImage_RescaleIOP(IOlePicture, _
                                                            dblDstWidthPercent, _
                                                            dblDstHeightPercent, _
                                                            True, _
                                                            eFilter)

End Function

Public Function FreeImage_RescaleByFactorIOP(ByRef IOlePicture As IPicture, _
                                    Optional ByVal dblDstWidthFactor As Double, _
                                    Optional ByVal dblDstHeightFactor As Double, _
                                    Optional ByVal eFilter As FREE_IMAGE_FILTER = FILTER_BICUBIC) As IPicture

   ' Thin wrapper for function 'FreeImage_RescaleExIOP' for removing method
   ' overload fake. This function rescales the image by a factor
   ' based on the image's original size.

   Set FreeImage_RescaleByFactorIOP = FreeImage_RescaleIOP(IOlePicture, _
                                                           dblDstWidthFactor, _
                                                           dblDstHeightFactor, _
                                                           False, _
                                                           eFilter)

End Function

Public Function FreeImage_ConvertColorDepthIOP(ByRef IOlePicture As IPicture, _
                                               ByVal eConversionFlag As FREE_IMAGE_CONVERSION_FLAGS, _
                                      Optional ByVal bThreshold As Byte = 128, _
                                      Optional ByVal eDitherMethod As FREE_IMAGE_DITHER = FID_FS, _
                                      Optional ByVal eQuantizationMethod As FREE_IMAGE_QUANTIZE = FIQ_WUQUANT, _
                                      Optional ByVal lPaletteSize As Long = 256, _
                                      Optional ByRef vntReservePalette As Variant, _
                                      Optional ByVal lReserveSize As Long) As IPicture
                                   
Dim hDIB As Long

   ' IOlePicture based wrapper for wrapper function FreeImage_ConvertColorDepth()

   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      hDIB = FreeImage_ConvertColorDepth(hDIB, eConversionFlag, True, _
                                         bThreshold, eDitherMethod, eQuantizationMethod, _
                                         lPaletteSize, vntReservePalette, lReserveSize)
      Set FreeImage_ConvertColorDepthIOP = FreeImage_GetOlePicture(hDIB, , True)
   End If

End Function

Public Function FreeImage_RotateClassicIOP(ByRef IOlePicture As IPicture, _
                                           ByVal angle As Double) As IPicture

Dim hDIB As Long
Dim hDIBNew As Long

   ' IOlePicture based wrapper for FreeImage function FreeImage_RotateClassic()

   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 1, 8, 24, 32
         hDIBNew = FreeImage_RotateClassic(hDIB, angle)
         Set FreeImage_RotateClassicIOP = FreeImage_GetOlePicture(hDIBNew, , True)
         
      End Select
      Call FreeImage_Unload(hDIB)
   End If

End Function

Public Function FreeImage_RotateExIOP(ByRef IOlePicture As IPicture, _
                                      ByVal angle As Double, _
                                      ByVal x_shift As Double, _
                                      ByVal y_shift As Double, _
                                      ByVal x_origin As Double, _
                                      ByVal y_origin As Double, _
                                      ByVal use_mask As Long) As IPicture

Dim hDIB As Long
Dim hDIBNew As Long

   ' IOlePicture based wrapper for FreeImage function FreeImage_RotateEx()

   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 8, 24, 32
         hDIBNew = FreeImage_RotateEx(hDIB, angle, x_shift, y_shift, _
                                                   x_origin, y_origin, use_mask)
         Set FreeImage_RotateExIOP = FreeImage_GetOlePicture(hDIBNew, , True)
         
      End Select
      Call FreeImage_Unload(hDIB)
   End If

End Function

Public Function FreeImage_FlipHorizontalIOP(ByRef IOlePicture As IPicture) As IPicture

Dim hDIB As Long

   ' IOlePicture based wrapper for FreeImage function FreeImage_FlipHorizontal()

   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Call FreeImage_FlipHorizontal(hDIB)
      Set FreeImage_FlipHorizontalIOP = FreeImage_GetOlePicture(hDIB, , True)
   End If

End Function

Public Function FreeImage_FlipVerticalIOP(ByRef IOlePicture As IPicture) As IPicture

Dim hDIB As Long

   ' IOlePicture based wrapper for FreeImage function FreeImage_FlipVertical()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Call FreeImage_FlipVertical(hDIB)
      Set FreeImage_FlipVerticalIOP = FreeImage_GetOlePicture(hDIB, , True)
   End If

End Function

Public Function FreeImage_AdjustCurveIOP(ByRef IOlePicture As IPicture, _
                                         ByRef LUT As Variant, _
                                Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK) As IPicture

Dim hDIB As Long

   ' IOlePicture based wrapper for FreeImage function FreeImage_AdjustCurve()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 8, 24, 32
         Call FreeImage_AdjustCurveEx(hDIB, LUT, channel)
         Set FreeImage_AdjustCurveIOP = FreeImage_GetOlePicture(hDIB, , True)
         
      End Select
   End If
   
End Function

Public Function FreeImage_AdjustGammaIOP(ByRef IOlePicture As IPicture, _
                                         ByVal gamma As Double) As IPicture

Dim hDIB As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_AdjustGamma()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 8, 24, 32
         Call FreeImage_AdjustGamma(hDIB, gamma)
         Set FreeImage_AdjustGammaIOP = FreeImage_GetOlePicture(hDIB, , True)
         
      End Select
   End If
   
End Function

Public Function FreeImage_AdjustBrightnessIOP(ByRef IOlePicture As IPicture, _
                                              ByVal percentage As Double) As IPicture

Dim hDIB As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_AdjustBrightness()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 8, 24, 32
         Call FreeImage_AdjustBrightness(hDIB, percentage)
         Set FreeImage_AdjustBrightnessIOP = FreeImage_GetOlePicture(hDIB, , True)
         
      End Select
   End If
   
End Function

Public Function FreeImage_AdjustContrastIOP(ByRef IOlePicture As IPicture, _
                                            ByVal percentage As Double) As IPicture

Dim hDIB As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_AdjustContrast()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 8, 24, 32
         Call FreeImage_AdjustContrast(hDIB, percentage)
         Set FreeImage_AdjustContrastIOP = FreeImage_GetOlePicture(hDIB, , True)
         
      End Select
   End If
   
End Function

Public Function FreeImage_InvertIOP(ByRef IOlePicture As IPicture) As IPicture

Dim hDIB As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_Invert()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Call FreeImage_Invert(hDIB)
      Set FreeImage_InvertIOP = FreeImage_GetOlePicture(hDIB, , True)
   End If
   
End Function

Public Function FreeImage_GetChannelIOP(ByRef IOlePicture As IPicture, _
                                        ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As IPicture

Dim hDIB As Long
Dim hDIBNew As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_GetChannel()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      Select Case FreeImage_GetBPP(hDIB)
      
      Case 24, 32
         hDIBNew = FreeImage_GetChannel(hDIB, channel)
         Set FreeImage_GetChannelIOP = FreeImage_GetOlePicture(hDIBNew, , True)
         
      End Select
      Call FreeImage_Unload(hDIB)
   End If

End Function

' Placeholder for FreeImage_SetChannelIOP() function

'''Public Function FreeImage_SetChannelIOP(ByRef IOlePicture As IPicture, _
'''                                        ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As IPicture
'''
'''Dim hDIB As Long
'''Dim hDIBNew As Long
'''
'''   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
'''   If (hDIB) Then
'''      Select Case FreeImage_GetBPP(hDIB)
'''
'''      Case 24, 32
'''         hDIBNew = FreeImage_GetChannel(hDIB, channel)
'''         Set FreeImage_GetChannelIOP = FreeImage_GetOlePicture(hDIBNew, , True)
'''
'''      End Select
'''      Call FreeImage_Unload(hDIB)
'''   End If
'''
'''End Function

Public Function FreeImage_CopyIOP(ByRef IOlePicture As IPicture, _
                                  ByVal Left As Long, _
                                  ByVal Top As Long, _
                                  ByVal Right As Long, _
                                  ByVal Bottom As Long) As IPicture

Dim hDIB As Long
Dim hDIBNew As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_Copy()
   
   hDIB = FreeImage_CreateFromOlePicture(IOlePicture)
   If (hDIB) Then
      hDIBNew = FreeImage_Copy(hDIB, Left, Top, Right, Bottom)
      Call FreeImage_Unload(hDIB)
      Set FreeImage_CopyIOP = FreeImage_GetOlePicture(hDIBNew, , True)
   End If

End Function

Public Function FreeImage_PasteIOP(ByRef IOlePictureDst As IPicture, _
                                   ByRef IOlePictureSrc As IPicture, _
                                   ByVal Left As Long, _
                                   ByVal Top As Long, _
                                   ByVal alpha As Long, _
                          Optional ByVal keep_original_dst_image As Boolean = False) As IPicture

Dim hDIBDst As Long
Dim hDibSrc As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_Paste()
   
   hDIBDst = FreeImage_CreateFromOlePicture(IOlePictureDst)
   If (hDIBDst) Then
      hDibSrc = FreeImage_CreateFromOlePicture(IOlePictureSrc)
      If (hDibSrc) Then
         If FreeImage_Paste(hDIBDst, hDibSrc, Left, Top, alpha) Then
            Set FreeImage_PasteIOP = FreeImage_GetOlePicture(hDIBDst, , True)
            If (Not keep_original_dst_image) Then
               Set IOlePictureDst = FreeImage_PasteIOP
            End If
         End If
         Call FreeImage_Unload(hDibSrc)
      End If
   End If

End Function

Public Function FreeImage_CompositeIOP(ByRef IOlePictureFG As IPicture, _
                              Optional ByVal useFileBkg As Boolean = False, _
                              Optional ByVal appBkColor As OLE_COLOR = 0, _
                              Optional ByRef IOlePictureBG As IPicture) As IPicture

Dim hDIBfg As Long
Dim hDIBbg As Long
Dim hDIBResult As Long
   
   ' IOlePicture based wrapper for FreeImage function FreeImage_Composite()
   
   hDIBfg = FreeImage_CreateFromOlePicture(IOlePictureFG)
   If (hDIBfg) Then
    
      hDIBbg = FreeImage_CreateFromOlePicture(IOlePictureBG)
      If (appBkColor) Then
        appBkColor = VarPtr(appBkColor)
      End If
      
      hDIBResult = FreeImage_Composite(hDIBfg, useFileBkg, appBkColor, hDIBbg)
      If (hDIBResult) Then
         Set FreeImage_CompositeIOP = FreeImage_GetOlePicture(hDIBResult, , True)
      End If
      
      Call FreeImage_Unload(hDIBfg)
      If (hDIBbg) Then
         Call FreeImage_Unload(hDIBbg)
      End If
   End If
   
End Function


'--------------------------------------------------------------------------------
' Compression functions wrappers
'--------------------------------------------------------------------------------

Public Function FreeImage_ZLibCompressEx(ByRef target As Variant, _
                                Optional ByRef target_size As Long, _
                                Optional ByRef Source As Variant, _
                                Optional ByVal source_size As Long, _
                                Optional ByVal offset As Long = 0) As Long
                                
Dim lSourceDataPtr As Long
Dim lTargetDataPtr As Long
Dim bTargetCreated As Boolean

   ' This function is a more VB friendly wrapper for compressing data with
   ' the 'FreeImage_ZLibCompress' function.
   
   ' The parameter 'target' may either be a VB style array of Byte, Integer
   ' or Long or a pointer to a memory block. If 'target' is a pointer to a
   ' memory block (when it contains an address), 'target_size' must be
   ' specified and greater than zero. If 'target' is an initialized array,
   ' the hole array will be used to store compressed data when 'target_size'
   ' is missing or below or equal to zero. If 'target_size' is specified, only
   ' the first target_size bytes of the array will be used.
   ' In each case, all rules according to the FreeImage API documentation
   ' apply, what means that the target buffer must be at least 0.1% greater
   ' than the source buffer plus 12 bytes.
   ' If 'target' is an uninitialized array, the contents of 'target_size'
   ' will be ignored and the size of the array 'target' will be handled
   ' internally. When the function returns, 'target' will be initialized
   ' as an array of Byte and sized correctly to hold all the compressed
   ' data.
   
   ' Nearly all, that is true for the parameters 'target' and 'target_size',
   ' is also true for 'Source' and 'source_size', expect that 'Source' should
   ' never be an uninitialized array. In that case, the function returns
   ' immediately.
   
   ' The optional parameter 'offset' may contain a number of bytes to remain
   ' untouched at the beginning of 'target', when an uninitialized array is
   ' provided through 'target'. When 'target' is either a pointer or an
   ' initialized array, 'offset' will be ignored. This parameter is currently
   ' used by 'FreeImage_ZLibCompressVB' to store the length of the uncompressed
   ' data at the first four bytes of 'target'.

   
   ' get the pointer and the size in bytes of the source
   ' memory block
   lSourceDataPtr = pGetMemoryBlockPtrFromVariant(Source, source_size)
   If (lSourceDataPtr) Then
      ' when we got a valid pointer, get the pointer and the size in bytes
      ' of the target memory block
      lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
      If (lTargetDataPtr = 0) Then
         ' if 'target' is a null pointer, we will initialized it as an array
         ' of bytes; here we will take 'offset' into account
         ReDim target(source_size + Int(source_size * 0.1) + _
                      12 + offset) As Byte
         ' get pointer and size in bytes (will never be a null pointer)
         lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
         ' adjust according to 'offset'
         lTargetDataPtr = lTargetDataPtr + offset
         target_size = target_size - offset
         bTargetCreated = True
      End If
      
      ' compress source data
      FreeImage_ZLibCompressEx = FreeImage_ZLibCompress(lTargetDataPtr, _
                                                        target_size, _
                                                        lSourceDataPtr, _
                                                        source_size)
      
      ' the function returns the number of bytes needed to store the
      ' compressed data or zero on failure
      If (FreeImage_ZLibCompressEx) Then
         If (bTargetCreated) Then
            ' when we created the array, we need to adjust it's size
            ' according to the length of the compressed data
            ReDim Preserve target(FreeImage_ZLibCompressEx - 1 + offset)
         End If
      End If
   End If
                                
End Function

Public Function FreeImage_ZLibUncompressEx(ByRef target As Variant, _
                                  Optional ByRef target_size As Long, _
                                  Optional ByRef Source As Variant, _
                                  Optional ByVal source_size As Long) As Long
                                
Dim lSourceDataPtr As Long
Dim lTargetDataPtr As Long

   ' This function is a more VB friendly wrapper for compressing data with
   ' the 'FreeImage_ZLibUncompress' function.
   
   ' The parameter 'target' may either be a VB style array of Byte, Integer
   ' or Long or a pointer to a memory block. If 'target' is a pointer to a
   ' memory block (when it contains an address), 'target_size' must be
   ' specified and greater than zero. If 'target' is an initialized array,
   ' the hole array will be used to store uncompressed data when 'target_size'
   ' is missing or below or equal to zero. If 'target_size' is specified, only
   ' the first target_size bytes of the array will be used.
   ' In each case, all rules according to the FreeImage API documentation
   ' apply, what means that the target buffer must be at least as large, to
   ' hold all the uncompressed data.
   ' Unlike the function 'FreeImage_ZLibCompressEx', 'target' can not be
   ' an uninitialized array, since the size of the uncompressed data can
   ' not be determined by the ZLib functions, but must be specified by a
   ' mechanism outside the FreeImage compression functions' scope.
   
   ' Nearly all, that is true for the parameters 'target' and 'target_size',
   ' is also true for 'Source' and 'source_size'.
   
   
   ' get the pointer and the size in bytes of the source
   ' memory block
   lSourceDataPtr = pGetMemoryBlockPtrFromVariant(Source, source_size)
   If (lSourceDataPtr) Then
      ' when we got a valid pointer, get the pointer and the size in bytes
      ' of the target memory block
      lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
      If (lTargetDataPtr) Then
         ' if we do not have a null pointer, uncompress the data
         FreeImage_ZLibUncompressEx = FreeImage_ZLibUncompress(lTargetDataPtr, _
                                                               target_size, _
                                                               lSourceDataPtr, _
                                                               source_size)
      End If
   End If
                                
End Function

Public Function FreeImage_ZLibGZipEx(ByRef target As Variant, _
                            Optional ByRef target_size As Long, _
                            Optional ByRef Source As Variant, _
                            Optional ByVal source_size As Long, _
                            Optional ByVal offset As Long = 0) As Long
                                
Dim lSourceDataPtr As Long
Dim lTargetDataPtr As Long
Dim bTargetCreated As Boolean

   ' This function is a more VB friendly wrapper for compressing data with
   ' the 'FreeImage_ZLibGZip' function.
   
   ' The parameter 'target' may either be a VB style array of Byte, Integer
   ' or Long or a pointer to a memory block. If 'target' is a pointer to a
   ' memory block (when it contains an address), 'target_size' must be
   ' specified and greater than zero. If 'target' is an initialized array,
   ' the hole array will be used to store compressed data when 'target_size'
   ' is missing or below or equal to zero. If 'target_size' is specified, only
   ' the first target_size bytes of the array will be used.
   ' In each case, all rules according to the FreeImage API documentation
   ' apply, what means that the target buffer must be at least 0.1% greater
   ' than the source buffer plus 24 bytes.
   ' If 'target' is an uninitialized array, the contents of 'target_size'
   ' will be ignored and the size of the array 'target' will be handled
   ' internally. When the function returns, 'target' will be initialized
   ' as an array of Byte and sized correctly to hold all the compressed
   ' data.
   
   ' Nearly all, that is true for the parameters 'target' and 'target_size',
   ' is also true for 'Source' and 'source_size', expect that 'Source' should
   ' never be an uninitialized array. In that case, the function returns
   ' immediately.
   
   ' The optional parameter 'offset' may contain a number of bytes to remain
   ' untouched at the beginning of 'target', when an uninitialized array is
   ' provided through 'target'. When 'target' is either a pointer or an
   ' initialized array, 'offset' will be ignored. This parameter is currently
   ' used by 'FreeImage_ZLibGZipVB' to store the length of the uncompressed
   ' data at the first four bytes of 'target'.

   
   ' get the pointer and the size in bytes of the source
   ' memory block
   lSourceDataPtr = pGetMemoryBlockPtrFromVariant(Source, source_size)
   If (lSourceDataPtr) Then
      ' when we got a valid pointer, get the pointer and the size in bytes
      ' of the target memory block
      lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
      If (lTargetDataPtr = 0) Then
         ' if 'target' is a null pointer, we will initialized it as an array
         ' of bytes; here we will take 'offset' into account
         ReDim target(source_size + Int(source_size * 0.1) + _
                      24 + offset) As Byte
         ' get pointer and size in bytes (will never be a null pointer)
         lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
         ' adjust according to 'offset'
         lTargetDataPtr = lTargetDataPtr + offset
         target_size = target_size - offset
         bTargetCreated = True
      End If
      
      ' compress source data
      FreeImage_ZLibGZipEx = FreeImage_ZLibGZip(lTargetDataPtr, _
                                                target_size, _
                                                lSourceDataPtr, _
                                                source_size)
      
      ' the function returns the number of bytes needed to store the
      ' compressed data or zero on failure
      If (FreeImage_ZLibGZipEx) Then
         If (bTargetCreated) Then
            ' when we created the array, we need to adjust it's size
            ' according to the length of the compressed data
            ReDim Preserve target(FreeImage_ZLibGZipEx - 1 + offset)
         End If
      End If
   End If
                                
End Function

Public Function FreeImage_ZLibCRC32Ex(ByVal crc As Long, _
                             Optional ByRef Source As Variant, _
                             Optional ByVal source_size As Long) As Long
                                
Dim lSourceDataPtr As Long

   ' This function is a more VB friendly wrapper for compressing data with
   ' the 'FreeImage_ZLibCRC32' function.
   
   ' The parameter 'Source' may either be a VB style array of Byte, Integer
   ' or Long or a pointer to a memory block. If 'Source' is a pointer to a
   ' memory block (when it contains an address), 'source_size' must be
   ' specified and greater than zero. If 'Source' is an initialized array,
   ' the hole array will be used to calculate the new CRC when 'source_size'
   ' is missing or below or equal to zero. If 'source_size' is specified, only
   ' the first source_size bytes of the array will be used.

   
   ' get the pointer and the size in bytes of the source
   ' memory block
   lSourceDataPtr = pGetMemoryBlockPtrFromVariant(Source, source_size)
   If (lSourceDataPtr) Then
      ' if we do not have a null pointer, calculate the CRC including 'crc'
      FreeImage_ZLibCRC32Ex = FreeImage_ZLibCRC32(crc, _
                                                  lSourceDataPtr, _
                                                  source_size)
   End If
                                
End Function

Public Function FreeImage_ZLibGUnzipEx(ByRef target As Variant, _
                              Optional ByRef target_size As Long, _
                              Optional ByRef Source As Variant, _
                              Optional ByVal source_size As Long) As Long
                                
Dim lSourceDataPtr As Long
Dim lTargetDataPtr As Long

   ' This function is a more VB friendly wrapper for compressing data with
   ' the 'FreeImage_ZLibGUnzip' function.
   
   ' The parameter 'target' may either be a VB style array of Byte, Integer
   ' or Long or a pointer to a memory block. If 'target' is a pointer to a
   ' memory block (when it contains an address), 'target_size' must be
   ' specified and greater than zero. If 'target' is an initialized array,
   ' the hole array will be used to store uncompressed data when 'target_size'
   ' is missing or below or equal to zero. If 'target_size' is specified, only
   ' the first target_size bytes of the array will be used.
   ' In each case, all rules according to the FreeImage API documentation
   ' apply, what means that the target buffer must be at least as large, to
   ' hold all the uncompressed data.
   ' Unlike the function 'FreeImage_ZLibGZipEx', 'target' can not be
   ' an uninitialized array, since the size of the uncompressed data can
   ' not be determined by the ZLib functions, but must be specified by a
   ' mechanism outside the FreeImage compression functions' scope.
   
   ' Nearly all, that is true for the parameters 'target' and 'target_size',
   ' is also true for 'Source' and 'source_size'.
   
   
   ' get the pointer and the size in bytes of the source
   ' memory block
   lSourceDataPtr = pGetMemoryBlockPtrFromVariant(Source, source_size)
   If (lSourceDataPtr) Then
      ' when we got a valid pointer, get the pointer and the size in bytes
      ' of the target memory block
      lTargetDataPtr = pGetMemoryBlockPtrFromVariant(target, target_size)
      If (lTargetDataPtr) Then
         ' if we do not have a null pointer, uncompress the data
         FreeImage_ZLibGUnzipEx = FreeImage_ZLibGUnzip(lTargetDataPtr, _
                                                       target_size, _
                                                       lSourceDataPtr, _
                                                       source_size)
      End If
   End If
                                
End Function

Public Function FreeImage_ZLibCompressVB(ByRef data() As Byte, _
                                Optional ByVal IncludeSize As Boolean = True) As Byte()
                                
Dim lOffset As Long
Dim lArrayDataPtr As Long

   ' This function is another, even more VB friendly wrapper for the FreeImage
   ' 'FreeImage_ZLibCompress' function, that uses the 'FreeImage_ZLibCompressEx'
   ' function. This function is very easy to use, since it deals only with VB
   ' style Byte arrays.
   
   ' The parameter 'data()' is a Byte array, providing the uncompressed source
   ' data that will be compressed.
   
   ' The optional parameter 'IncludeSize' determines whether the size of the
   ' uncompressed data should be stored in the first four bytes of the returned
   ' byte buffer containing the compressed data or not. When 'IncludeSize' is
   ' True, the size of the uncompressed source data will be stored. This works
   ' in conjunction with the corresponding 'FreeImage_ZLibUncompressVB' function.
   
   ' The function returns a VB style Byte array containing the compressed data.
   

   ' start population the memory block with compressed data
   ' at offset 4 bytes, when the unclompressed size should
   ' be included
   If (IncludeSize) Then
      lOffset = 4
   End If
   
   Call FreeImage_ZLibCompressEx(FreeImage_ZLibCompressVB, , data, , lOffset)
                                 
   If (IncludeSize) Then
      ' get the pointer actual pointing to the array data of
      ' the Byte array 'FreeImage_ZLibCompressVB'
      lArrayDataPtr = deref(deref(VarPtrArray(FreeImage_ZLibCompressVB)) + 12)

      ' copy uncompressed size into the first 4 bytes
      Call CopyMemory(ByVal lArrayDataPtr, UBound(data) + 1, 4)
   End If

End Function

Public Function FreeImage_ZLibUncompressVB(ByRef data() As Byte, _
                                  Optional ByVal SizeIncluded As Boolean = True, _
                                  Optional ByVal SizeNeeded As Long) As Byte()

Dim abBuffer() As Byte

   ' This function is another, even more VB friendly wrapper for the FreeImage
   ' 'FreeImage_ZLibUncompress' function, that uses the 'FreeImage_ZLibUncompressEx'
   ' function. This function is very easy to use, since it deals only with VB
   ' style Byte arrays.
   
   ' The parameter 'data()' is a Byte array, providing the compressed source
   ' data that will be uncompressed either withthe size of the uncompressed
   ' data included or not.
   
   ' When the optional parameter 'SizeIncluded' is True, the function assumes,
   ' that the first four bytes contain the size of the uncompressed data as a
   ' Long value. In that case, 'SizeNeeded' will be ignored.
   
   ' When the size of the uncompressed data is not included in the buffer 'data()'
   ' containing the compressed data, the optional parameter 'SizeNeeded' must
   ' specify the size in bytes needed to hold all the uncompressed data.
   
   ' The function returns a VB style Byte array containing the uncompressed data.


   If (SizeIncluded) Then
      ' get uncompressed size from the first 4 bytes and allocate
      ' buffer accordingly
      Call CopyMemory(SizeNeeded, data(0), 4)
      ReDim abBuffer(SizeNeeded - 1)
      Call FreeImage_ZLibUncompressEx(abBuffer, , VarPtr(data(4)), UBound(data) - 3)
      Call swap(VarPtrArray(FreeImage_ZLibUncompressVB), VarPtrArray(abBuffer))
   
   ElseIf (SizeNeeded) Then
      ' no size included in compressed data, so just forward the
      ' call to 'FreeImage_ZLibUncompressEx' and trust on SizeNeeded
      ReDim abBuffer(SizeNeeded - 1)
      Call FreeImage_ZLibUncompressEx(abBuffer, , data)
      Call swap(VarPtrArray(FreeImage_ZLibUncompressVB), VarPtrArray(abBuffer))
   
   End If

End Function

Public Function FreeImage_ZLibGZipVB(ByRef data() As Byte, _
                            Optional ByVal IncludeSize As Boolean = True) As Byte()
                                
Dim lOffset As Long
Dim lArrayDataPtr As Long

   ' This function is another, even more VB friendly wrapper for the FreeImage
   ' 'FreeImage_ZLibGZip' function, that uses the 'FreeImage_ZLibGZipEx'
   ' function. This function is very easy to use, since it deals only with VB
   ' style Byte arrays.
   
   ' The parameter 'data()' is a Byte array, providing the uncompressed source
   ' data that will be compressed.
   
   ' The optional parameter 'IncludeSize' determines whether the size of the
   ' uncompressed data should be stored in the first four bytes of the returned
   ' byte buffer containing the compressed data or not. When 'IncludeSize' is
   ' True, the size of the uncompressed source data will be stored. This works
   ' in conjunction with the corresponding 'FreeImage_ZLibGUnzipVB' function.
   
   ' The function returns a VB style Byte array containing the compressed data.


   ' start population the memory block with compressed data
   ' at offset 4 bytes, when the unclompressed size should
   ' be included
   If (IncludeSize) Then
      lOffset = 4
   End If
   
   Call FreeImage_ZLibGZipEx(FreeImage_ZLibGZipVB, , data, , lOffset)
                                 
   If (IncludeSize) Then
      ' get the pointer actual pointing to the array data of
      ' the Byte array 'FreeImage_ZLibCompressVB'
      lArrayDataPtr = deref(deref(VarPtrArray(FreeImage_ZLibGZipVB)) + 12)

      ' copy uncompressed size into the first 4 bytes
      Call CopyMemory(ByVal lArrayDataPtr, UBound(data) + 1, 4)
   End If

End Function

Public Function FreeImage_ZLibGUnzipVB(ByRef data() As Byte, _
                              Optional ByVal SizeIncluded As Boolean = True, _
                              Optional ByVal SizeNeeded As Long) As Byte()

Dim abBuffer() As Byte

   ' This function is another, even more VB friendly wrapper for the FreeImage
   ' 'FreeImage_ZLibGUnzip' function, that uses the 'FreeImage_ZLibGUnzipEx'
   ' function. This function is very easy to use, since it deals only with VB
   ' style Byte arrays.
   
   ' The parameter 'data()' is a Byte array, providing the compressed source
   ' data that will be uncompressed either withthe size of the uncompressed
   ' data included or not.
   
   ' When the optional parameter 'SizeIncluded' is True, the function assumes,
   ' that the first four bytes contain the size of the uncompressed data as a
   ' Long value. In that case, 'SizeNeeded' will be ignored.
   
   ' When the size of the uncompressed data is not included in the buffer 'data()'
   ' containing the compressed data, the optional parameter 'SizeNeeded' must
   ' specify the size in bytes needed to hold all the uncompressed data.
   
   ' The function returns a VB style Byte array containing the uncompressed data.


   If (SizeIncluded) Then
      ' get uncompressed size from the first 4 bytes and allocate
      ' buffer accordingly
      Call CopyMemory(SizeNeeded, data(0), 4)
      ReDim abBuffer(SizeNeeded - 1)
      Call FreeImage_ZLibGUnzipEx(abBuffer, , VarPtr(data(4)), UBound(data) - 3)
      Call swap(VarPtrArray(FreeImage_ZLibGUnzipVB), VarPtrArray(abBuffer))
   
   ElseIf (SizeNeeded) Then
      ' no size included in compressed data, so just forward the
      ' call to 'FreeImage_ZLibUncompressEx' and trust on SizeNeeded
      ReDim abBuffer(SizeNeeded - 1)
      Call FreeImage_ZLibGUnzipEx(abBuffer, , data)
      Call swap(VarPtrArray(FreeImage_ZLibGUnzipVB), VarPtrArray(abBuffer))
   
   End If

End Function


'--------------------------------------------------------------------------------
' Public functions to destroy custom safearrays
'--------------------------------------------------------------------------------

Public Function FreeImage_DestroyLockedArray(ByRef data As Variant) As Long

Dim lpArrayPtr As Long

   ' This function destroys an array, that was self created with a custom
   ' array descriptor of type ('fFeatures' member) 'FADF_AUTO Or FADF_FIXEDSIZE'.
   ' Such arrays are returned by mostly all of the array-dealing wrapper
   ' functions. Since these should not destroy the actual array data, when
   ' going out of scope, they are craeted as 'FADF_FIXEDSIZE'.'
   
   ' So, VB sees them as fixed or temporarily locked, when you try to manipulate
   ' the array's dimensions. There will occur some strange effects, you should
   ' know about:
   
   ' 1. When trying to 'ReDim' the array, this run-time error will occur:
   '    Error #10, 'This array is fixed or temporarily locked'
   
   ' 2. When trying to assign another array to the array variable, this
   '    run-time error will occur:
   '    Error #13, 'Type mismatch'
   
   ' 3. The 'Erase' statement has no effect on the array
   
   ' Although VB clears up these arrays correctly, when the array variable
   ' goes out of scope, you have to destroy the array manually, when you want
   ' to reuse the array variable in current scope.
   
   ' For an example assume, that you want do walk all scanlines in an image:
   
   ' For i = 0 To FreeImage_GetHeight(dib)
   '
   '    ' assign scanline-array to array variable
   '    abByte = FreeImage_GetScanLineEx(dib, i)
   '
   '    ' do some work on it...
   '
   '    ' destroy the array (only the array, not the actual data)
   '    Call FreeImage_DestroyLockedArray(dbByte)
   ' Next i
   
   ' The function returns zero on success and any other value on failure
   
   ' !! Attention !!
   ' This function uses a Variant parameter for passing the array to be
   ' destroyed. Since VB does not allow to pass an array of non public
   ' structures through a Variant parameter, this function can not be used
   ' with arrays of cutom type.
   
   ' You will get this compiler error: "Only public user defined types defined
   ' in public object modules can be used as parameters or return types for
   ' public procedures of class modules or as fields of public user defined types"
   
   ' So, there is a function in the wrapper called 'FreeImage_DestroyLockedArrayByPtr'
   ' that takes a pointer to the array variable which can be used to work around
   ' that VB limitation and furthermore can be used for any of these self-created
   ' arrays. To get the array variable's pointer, a declared version of the
   ' VB 'VarPtr' function can be used which works for all types of arrays expect
   ' String arrays. Declare this function like this in your code:
   
   ' Private Declare Function VarPtrArray Lib "msvbvm60.dll" Alias "VarPtr" ( _
         ByRef Ptr() As Any) As Long
         
   ' Then an array could be destroyed by calling the 'FreeImage_DestroyLockedArrayByPtr'
   ' function like this:
   
   ' lResult = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(MyLockedArray))
   
   ' Additionally there are some handy wrapper functions available, one for each
   ' commonly used structure in FreeImage like RGBTRIPLE, RGBQUAD, FICOMPLEX etc.
   
   
   ' Currently, these functions do return 'FADF_AUTO Or FADF_FIXEDSIZE' arrays
   ' that must be destroyed using this or any of it's derived functions:
   
   ' FreeImage_GetPaletteEx()           with FreeImage_DestroyLockedArrayRGBQUAD()
   ' FreeImage_GetPaletteLong()         with FreeImage_DestroyLockedArray()
   ' FreeImage_SaveToMemoryEx2()        with FreeImage_DestroyLockedArray()
   ' FreeImage_AcquireMemoryEx()        with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineEx()          with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineBITMAP8()     with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineBITMAP16()    with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineBITMAP24()    with FreeImage_DestroyLockedArrayRGBTRIPLE()
   ' FreeImage_GetScanLineBITMAP32()    with FreeImage_DestroyLockedArrayRGBQUAD()
   ' FreeImage_GetScanLineINT16()       with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineINT32()       with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineFLOAT()       with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineDOUBLE()      with FreeImage_DestroyLockedArray()
   ' FreeImage_GetScanLineCOMPLEX()     with FreeImage_DestroyLockedArrayFICOMPLEX()
   ' FreeImage_GetScanLineRGB16()       with FreeImage_DestroyLockedArrayFIRGB16()
   ' FreeImage_GetScanLineRGBA16()      with FreeImage_DestroyLockedArrayFIRGBA16()
   ' FreeImage_GetScanLineRGBF()        with FreeImage_DestroyLockedArrayFIRGBF()
   ' FreeImage_GetScanLineRGBAF()       with FreeImage_DestroyLockedArrayFIRGBAF()

   
   ' ensure, this is an array
   If (VarType(data) And vbArray) Then
   
      ' data is a VB array, what means a SAFEARRAY in C/C++, that is
      ' passed through a ByRef Variant variable, that is a pointer to
      ' a VARIANTARG structure
      
      ' the VARIANTARG structure looks like this:
      
      ' typedef struct tagVARIANT VARIANTARG;
      ' struct tagVARIANT
      '     {
      '     Union
      '         {
      '         struct __tagVARIANT
      '             {
      '             VARTYPE vt;
      '             WORD wReserved1;
      '             WORD wReserved2;
      '             WORD wReserved3;
      '             Union
      '                 {
      '                 [...]
      '             SAFEARRAY *parray;    // used when not VT_BYREF
      '                 [...]
      '             SAFEARRAY **pparray;  // used when VT_BYREF
      '                 [...]
      
      ' the data element (SAFEARRAY) has an offset of 8, since VARTYPE
      ' and WORD both have a length of 2 bytes; the pointer to the
      ' VARIANTARG structure is the VarPtr of the Variant variable in VB
      
      ' getting the contents of the data element (in C/C++: *(data + 8))
      lpArrayPtr = deref(VarPtr(data) + 8)
      
      ' call the 'FreeImage_DestroyLockedArrayByPtr' function to destroy
      ' the array properly
      Call FreeImage_DestroyLockedArrayByPtr(lpArrayPtr)
   Else
      
      FreeImage_DestroyLockedArray = -1
   End If

End Function

Public Function FreeImage_DestroyLockedArrayByPtr(ByVal lpArrayPtr As Long) As Long

Dim lpSA As Long

   ' This function destroys a self-created array with a custom array
   ' descriptor by a pointer to the array variable.

   ' dereference the pointer once (in C/C++: *lpArrayPtr)
   lpSA = deref(lpArrayPtr)
   ' now 'lpSA' is a pointer to the actual SAFEARRAY structure
   ' and could be a null pointer when the array is not initialized
   ' then, we have nothing to do here but return (-1) to indicate
   ' an "error"
   If (lpSA) Then
      
      ' destroy the array descriptor
      Call SafeArrayDestroyDescriptor(lpSA)
      
      ' make 'lpSA' a null pointer, that is an uninitialized array;
      ' keep in mind, that we here use 'lpArrayPtr' as a ByVal argument,
      ' since 'lpArrayPtr' is a pointer to lpSA (the address of lpSA);
      ' we need to zero these four bytes, 'lpArrayPtr' points to
      Call CopyMemory(ByVal lpArrayPtr, 0&, 4)
   Else
      
      ' the array is already uninitialized, so return an "error" value
      FreeImage_DestroyLockedArrayByPtr = -1
   End If

End Function

Public Function FreeImage_DestroyLockedArrayRGBTRIPLE(ByRef data() As RGBTRIPLE) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'RGBTRIPLE'.
   
   FreeImage_DestroyLockedArrayRGBTRIPLE = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayRGBQUAD(ByRef data() As RGBQUAD) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'RGBQUAD'.

   FreeImage_DestroyLockedArrayRGBQUAD = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayFICOMPLEX(ByRef data() As FICOMPLEX) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'FICOMPLEX'.

   FreeImage_DestroyLockedArrayFICOMPLEX = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayFIRGB16(ByRef data() As FIRGB16) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'FIRGB16'.

   FreeImage_DestroyLockedArrayFIRGB16 = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayFIRGBA16(ByRef data() As FIRGBA16) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'FIRGBA16'.

   FreeImage_DestroyLockedArrayFIRGBA16 = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayFIRGBF(ByRef data() As FIRGBF) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'FIRGBF'.

   FreeImage_DestroyLockedArrayFIRGBF = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function

Public Function FreeImage_DestroyLockedArrayFIRGBAF(ByRef data() As FIRGBAF) As Long

   ' This function is a thin wrapper for 'FreeImage_DestroyLockedArrayByPtr'
   ' for destroying arrays of type 'FIRGBAF'.

   FreeImage_DestroyLockedArrayFIRGBAF = FreeImage_DestroyLockedArrayByPtr(VarPtrArray(data))

End Function


'--------------------------------------------------------------------------------
' Private pointer manipulation helper functions
'--------------------------------------------------------------------------------

Private Function pGetStringFromPointerA(ByRef lPtr As Long) As String

Dim abBuffer() As Byte
Dim lLength As Long

   ' This function creates and returns a VB BSTR variable from
   ' a C/C++ style string pointer by making a redundant deep
   ' copy of the string's characters.

   If (lPtr) Then
      ' get the length of the ANSI string pointed to by lPtr
      lLength = lstrlen(lPtr)
      If (lLength) Then
         ' copy charcters to a byte array
         ReDim abBuffer(lLength - 1)
         Call CopyMemory(abBuffer(0), ByVal lPtr, lLength)
         ' convert from byte array to unicode BSTR
         pGetStringFromPointerA = StrConv(abBuffer, vbUnicode)
      End If
   End If

End Function

Private Function deref(ByVal lPtr As Long) As Long

   ' This function dereferences a pointer and returns the
   ' contents as it's return value.
   
   ' in C/C++ this would be:
   ' return *(lPtr);
   
   Call CopyMemory(deref, ByVal lPtr, 4)

End Function

Private Sub swap(ByVal lpSrc As Long, _
                 ByVal lpDst As Long)

Dim lpTmp As Long

   ' This function swaps two DWORD memory blocks pointed to
   ' by lpSrc and lpDst, whereby lpSrc and lpDst are actually
   ' no pointer types but contain the pointer's address.
   
   ' in C/C++ this would be:
   ' void swap(int lpSrc, int lpDst) {
   '   int tmp = *(int*)lpSrc;
   '   *(int*)lpSrc = *(int*)lpDst;
   '   *(int*)lpDst = tmp;
   ' }
  
   Call CopyMemory(lpTmp, ByVal lpSrc, 4)
   Call CopyMemory(ByVal lpSrc, ByVal lpDst, 4)
   Call CopyMemory(ByVal lpDst, lpTmp, 4)

End Sub

Private Function pGetMemoryBlockPtrFromVariant(ByRef data As Variant, _
                                      Optional ByRef size_in_bytes As Long) As Long
                                            
   ' This function returns the pointer to the memory block provided through
   ' the Variant parameter 'data', which could be either a Byte, Integer or
   ' Long array or the address of the memory block itself. In the last case,
   ' the parameter 'size_in_bytes' must not be omitted or zero, since it's
   ' correct value (the size of the memory block) can not be determined by
   ' the address only. So, the function fails, if 'size_in_bytes' is omitted
   ' or zero and 'data' is not an array but contains a Long value (the address
   ' of a memory block) by returning Null.
   
   ' If 'data' contains either a Byte, Integer or Long array, the pointer to
   ' the actual array data is returned. The parameter 'size_in_bytes' will
   ' be adjusted correctly, if it was less or equal zero upon entry.
   
   ' The function returns Null (zero) if there was no supported memory block
   ' provided.
   
   ' do we have an array?
   If (VarType(data) And vbArray) Then
      Select Case (VarType(data) And (Not vbArray))
      
      Case vbByte
         pGetMemoryBlockPtrFromVariant = pGetArrayPtrFromVariantArray(data)
         If (pGetMemoryBlockPtrFromVariant) Then
            If (size_in_bytes <= 0) Then
               size_in_bytes = (UBound(data) + 1)
            
            ElseIf (size_in_bytes > (UBound(data) + 1)) Then
               size_in_bytes = (UBound(data) + 1)
            
            End If
         End If
      
      Case vbInteger
         pGetMemoryBlockPtrFromVariant = pGetArrayPtrFromVariantArray(data)
         If (pGetMemoryBlockPtrFromVariant) Then
            If (size_in_bytes <= 0) Then
               size_in_bytes = (UBound(data) + 1) * 2
            
            ElseIf (size_in_bytes > ((UBound(data) + 1) * 2)) Then
               size_in_bytes = (UBound(data) + 1) * 2
            
            End If
         End If
      
      Case vbLong
         pGetMemoryBlockPtrFromVariant = pGetArrayPtrFromVariantArray(data)
         If (pGetMemoryBlockPtrFromVariant) Then
            If (size_in_bytes <= 0) Then
               size_in_bytes = (UBound(data) + 1) * 4
            
            ElseIf (size_in_bytes > ((UBound(data) + 1) * 4)) Then
               size_in_bytes = (UBound(data) + 1) * 4
            
            End If
         End If
      
      End Select
   Else
      If ((VarType(data) = vbLong) And _
          (size_in_bytes >= 0)) Then
         pGetMemoryBlockPtrFromVariant = data
      End If
   End If
                                            
End Function

Private Function pGetArrayPtrFromVariantArray(ByRef data As Variant) As Long

Dim eVarType As VbVarType
Dim lDataPtr As Long

   ' This function returns a pointer to the first array element of
   ' a VB array (SAFEARRAY) that is passed through a Variant type
   ' parameter. (Don't try this at home...)
   
   ' cache VarType in variable
   eVarType = VarType(data)
   
   ' ensure, this is an array
   If (eVarType And vbArray) Then
      
      ' data is a VB array, what means a SAFEARRAY in C/C++, that is
      ' passed through a ByRef Variant variable, that is a pointer to
      ' a VARIANTARG structure
      
      ' the VARIANTARG structure looks like this:
      
      ' typedef struct tagVARIANT VARIANTARG;
      ' struct tagVARIANT
      '     {
      '     Union
      '         {
      '         struct __tagVARIANT
      '             {
      '             VARTYPE vt;
      '             WORD wReserved1;
      '             WORD wReserved2;
      '             WORD wReserved3;
      '             Union
      '                 {
      '                 [...]
      '             SAFEARRAY *parray;    // used when not VT_BYREF
      '                 [...]
      '             SAFEARRAY **pparray;  // used when VT_BYREF
      '                 [...]
      
      ' the data element (SAFEARRAY) has an offset of 8, since VARTYPE
      ' and WORD both have a length of 2 bytes; the pointer to the
      ' VARIANTARG structure is the VarPtr of the Variant variable in VB
      
      ' getting the contents of the data element (in C/C++: *(data + 8))
      lDataPtr = deref(VarPtr(data) + 8)
      
      ' dereference the pointer again (in C/C++: *(lDataPtr))
      lDataPtr = deref(lDataPtr)
      
      ' test, whether 'lDataPtr' now is a Null pointer
      ' in that case, the array is not yet initialized and so we can't dereference
      ' it another time since we have no permisson to acces address 0
      
      ' the contents of 'lDataPtr' may be Null now in case of an uninitialized
      ' array; then we can't access any of the SAFEARRAY members since the array
      ' variable doesn't event point to a SAFEARRAY structure, so we will return
      ' the null pointer
      
      If (lDataPtr) Then
         ' the contents of lDataPtr now is a pointer to the SAFEARRAY structure
            
         ' the SAFEARRAY structure looks like this:
         
         ' typedef struct FARSTRUCT tagSAFEARRAY {
         '    unsigned short cDims;       // Count of dimensions in this array.
         '    unsigned short fFeatures;   // Flags used by the SafeArray
         '                                // routines documented below.
         ' #if defined(WIN32)
         '    unsigned long cbElements;   // Size of an element of the array.
         '                                // Does not include size of
         '                                // pointed-to data.
         '    unsigned long cLocks;       // Number of times the array has been
         '                                // locked without corresponding unlock.
         ' #Else
         '    unsigned short cbElements;
         '    unsigned short cLocks;
         '    unsigned long handle;       // Used on Macintosh only.
         ' #End If
         '    void HUGEP* pvData;               // Pointer to the data.
         '    SAFEARRAYBOUND rgsabound[1];      // One bound for each dimension.
         ' } SAFEARRAY;
         
         ' since we live in WIN32, the pvData element has an offset
         ' of 12 bytes from the base address of the structure,
         ' so dereference the pvData pointer, what indeed is a pointer
         ' to the actual array (in C/C++: *(lDataPtr + 12))
         lDataPtr = deref(lDataPtr + 12)
      End If
      
      ' return this value
      pGetArrayPtrFromVariantArray = lDataPtr
      
      ' a more shorter form of this function would be:
      ' (doesn't work for uninitialized arrays, but will likely crash!)
      'pGetArrayPtrFromVariantArray = deref(deref(deref(VarPtr(data) + 8)) + 12)
   End If

End Function
