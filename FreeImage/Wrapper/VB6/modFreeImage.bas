Attribute VB_Name = "FreeImage"
Option Explicit

Public Const unix As Long = 1
Public Const linux As Long = 1
Public Const i386 As Long = 1
Public Const FREEIMAGE_MAJOR_VERSION As Long = 3
Public Const FREEIMAGE_MINOR_VERSION As Long = 5
Public Const FREEIMAGE_RELEASE_SERIAL As Long = 0
Public Const SEEK_SET As Long = 0
Public Const SEEK_CUR As Long = 1
Public Const SEEK_END As Long = 2
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
Public Const FIICC_DEFAULT As Long = &H0
Public Const FIICC_COLOR_IS_CMYK As Long = &H1
Public Const BMP_DEFAULT As Long = 0
Public Const BMP_SAVE_RLE As Long = 1
Public Const CUT_DEFAULT As Long = 0
Public Const DDS_DEFAULT As Long = 0
Public Const GIF_DEFAULT As Long = 0
Public Const ICO_DEFAULT As Long = 0
Public Const ICO_MAKEALPHA As Long = 1
Public Const IFF_DEFAULT As Long = 0
Public Const JPEG_DEFAULT As Long = 0
Public Const JPEG_FAST As Long = 1
Public Const JPEG_ACCURATE As Long = 2
Public Const JPEG_QUALITYSUPERB As Long = &H80
Public Const JPEG_QUALITYGOOD As Long = &H100
Public Const JPEG_QUALITYNORMAL As Long = &H200
Public Const JPEG_QUALITYAVERAGE As Long = &H400
Public Const JPEG_QUALITYBAD As Long = &H800
Public Const KOALA_DEFAULT As Long = 0
Public Const LBM_DEFAULT As Long = 0
Public Const MNG_DEFAULT As Long = 0
Public Const PCD_DEFAULT As Long = 0
Public Const PCD_BASE As Long = 1
Public Const PCD_BASEDIV4 As Long = 2
Public Const PCD_BASEDIV16 As Long = 3
Public Const PCX_DEFAULT As Long = 0
Public Const PNG_DEFAULT As Long = 0
Public Const PNG_IGNOREGAMMA As Long = 1
Public Const PNM_DEFAULT As Long = 0
Public Const PNM_SAVE_RAW As Long = 0
Public Const PNM_SAVE_ASCII As Long = 1
Public Const PSD_DEFAULT As Long = 0
Public Const RAS_DEFAULT As Long = 0
Public Const TARGA_DEFAULT As Long = 0
Public Const TARGA_LOAD_RGB888 As Long = 1
Public Const TIFF_DEFAULT As Long = 0
Public Const TIFF_CMYK As Long = &H1
Public Const TIFF_PACKBITS As Long = &H100
Public Const TIFF_DEFLATE As Long = &H200
Public Const TIFF_ADOBE_DEFLATE As Long = &H400
Public Const TIFF_NONE As Long = &H800
Public Const TIFF_CCITTFAX3 As Long = &H1000
Public Const TIFF_CCITTFAX4 As Long = &H2000
Public Const TIFF_LZW As Long = &H4000
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
End Enum
Public Enum FREE_IMAGE_TYPE
        FIT_UNKNOWN = 0
        FIT_BITMAP = 1
        FIT_UINT16 = 2
        FIT_INT16 = 3
        FIT_UINT32 = 4
        FIT_INT32 = 5
        FIT_FLOAT = 6
        FIT_DOUBLE = 7
        FIT_COMPLEX = 8
End Enum
Public Enum FREE_IMAGE_COLOR_TYPE
        FIC_MINISWHITE = 0
        FIC_MINISBLACK = 1
        FIC_RGB = 2
        FIC_PALETTE = 3
        FIC_RGBALPHA = 4
        FIC_CMYK = 5
End Enum
Public Enum FREE_IMAGE_QUANTIZE
        FIQ_WUQUANT = 0
        FIQ_NNQUANT = 1
End Enum
Public Enum FREE_IMAGE_DITHER
        FID_FS = 0
        FID_BAYER4x4 = 1
        FID_BAYER8x8 = 2
        FID_CLUSTER6x6 = 3
        FID_CLUSTER8x8 = 4
        FID_CLUSTER16x16 = 5
End Enum
Public Enum FREE_IMAGE_FILTER
        FILTER_BOX = 0
        FILTER_BICUBIC = 1
        FILTER_BILINEAR = 2
        FILTER_BSPLINE = 3
        FILTER_CATMULLROM = 4
        FILTER_LANCZOS3 = 5
End Enum
Public Enum FREE_IMAGE_COLOR_CHANNEL
        FICC_RGB = 0
        FICC_RED = 1
        FICC_GREEN = 2
        FICC_BLUE = 3
        FICC_ALPHA = 4
        FICC_BLACK = 5
        FICC_REAL = 6
        FICC_IMAG = 7
        FICC_MAG = 8
        FICC_PHASE = 9
End Enum
Public Enum FREE_IMAGE_MDTYPE
        FIDT_NOTYPE = 0
        FIDT_BYTE = 1
        FIDT_ASCII = 2
        FIDT_SHORT = 3
        FIDT_LONG = 4
        FIDT_RATIONAL = 5
        FIDT_SBYTE = 6
        FIDT_UNDEFINED = 7
        FIDT_SSHORT = 8
        FIDT_SLONG = 9
        FIDT_SRATIONAL = 10
        FIDT_FLOAT = 11
        FIDT_DOUBLE = 12
        FIDT_IFD = 13
End Enum
Public Enum FREE_IMAGE_MDMODEL
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
        FIMD_CUSTOM = 9
End Enum

Public Type FIBITMAP
        data As Long
End Type
Public Type FIMULTIBITMAP
        data As Long
End Type
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
Public Type FICOMPLEX
        r As Double
        i As Double
End Type
Public Type FIMETADATA
        data As Long
End Type
Public Type FITAG
        key As Long
        description As Long
        id As Integer
        type As Integer
        count As Long
        length As Long
        Value As Long
End Type
Public Type FreeImageIO
        read_proc As Long
        write_proc As Long
        seek_proc As Long
        tell_proc As Long
End Type
Public Type FIMEMORY
        data As Long
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

Public Declare Sub FreeImage_Initialise Lib "FreeImage.dll" Alias "_FreeImage_Initialise@4" (Optional ByVal load_local_plugins_only As Long = 0)
Public Declare Sub FreeImage_DeInitialise Lib "FreeImage.dll" Alias "_FreeImage_DeInitialise@0" ()
Public Declare Function FreeImage_GetVersion Lib "FreeImage.dll" Alias "_FreeImage_GetVersion@0" () As String
Public Declare Function FreeImage_GetCopyrightMessage Lib "FreeImage.dll" Alias "_FreeImage_GetCopyrightMessage@0" () As String
Public Declare Sub FreeImage_OutputMessageProc Lib "FreeImage.dll" Alias "_FreeImage_OutputMessageProc@8" (ByVal fif As Long, ByVal fmt As String, ParamArray VarArgs() As Variant)
Public Declare Sub FreeImage_SetOutputMessage Lib "FreeImage.dll" Alias "_FreeImage_SetOutputMessage@4" (ByVal omf As Long)
Public Declare Function FreeImage_Allocate Lib "FreeImage.dll" Alias "_FreeImage_Allocate@24" (ByVal width As Long, ByVal height As Long, ByVal bpp As Long, Optional ByVal red_mask As Long = 0, Optional ByVal green_mask As Long = 0, Optional ByVal blue_mask As Long = 0) As Long
Public Declare Function FreeImage_AllocateT Lib "FreeImage.dll" Alias "_FreeImage_AllocateT@28" (ByVal type_ As FREE_IMAGE_TYPE, ByVal width As Long, ByVal height As Long, Optional ByVal bpp As Long = 8, Optional ByVal red_mask As Long = 0, Optional ByVal green_mask As Long = 0, Optional ByVal blue_mask As Long = 0) As Long
Public Declare Function FreeImage_Clone Lib "FreeImage.dll" Alias "_FreeImage_Clone@4" (ByVal dib As Long) As Long
Public Declare Sub FreeImage_Unload Lib "FreeImage.dll" Alias "_FreeImage_Unload@4" (ByVal dib As Long)
Public Declare Function FreeImage_Load Lib "FreeImage.dll" Alias "_FreeImage_Load@12" (ByVal fif As FREE_IMAGE_FORMAT, ByVal filename As String, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_LoadFromHandle Lib "FreeImage.dll" Alias "_FreeImage_LoadFromHandle@16" (ByVal fif As FREE_IMAGE_FORMAT, ByVal io As Long, ByVal handle As Long, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_Save Lib "FreeImage.dll" Alias "_FreeImage_Save@16" (ByVal fif As FREE_IMAGE_FORMAT, ByVal dib As Long, ByVal filename As String, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_SaveToHandle Lib "FreeImage.dll" Alias "_FreeImage_SaveToHandle@20" (ByVal fif As FREE_IMAGE_FORMAT, ByVal dib As Long, ByVal io As Long, ByVal handle As Long, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_OpenMemory Lib "FreeImage.dll" Alias "_FreeImage_OpenMemory@8" (Optional ByRef data As Long = 0, Optional ByVal size_in_bytes As Long = 0) As FIMEMORY
Public Declare Sub FreeImage_CloseMemory Lib "FreeImage.dll" Alias "_FreeImage_CloseMemory@0" (stream As FIMEMORY)
Public Declare Function FreeImage_LoadFromMemory Lib "FreeImage.dll" Alias "_FreeImage_LoadFromMemory@8" (ByVal fif As FREE_IMAGE_FORMAT, stream As FIMEMORY, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_SaveToMemory Lib "FreeImage.dll" Alias "_FreeImage_SaveToMemory@12" (ByVal fif As FREE_IMAGE_FORMAT, ByVal dib As Long, stream As FIMEMORY, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_TellMemory Lib "FreeImage.dll" Alias "_FreeImage_TellMemory@0" (stream As FIMEMORY) As Long
Public Declare Function FreeImage_SeekMemory Lib "FreeImage.dll" Alias "_FreeImage_SeekMemory@8" (stream As FIMEMORY, ByVal offset As Long, ByVal origin As Long) As Long
Public Declare Function FreeImage_AcquireMemory Lib "FreeImage.dll" Alias "_FreeImage_AcquireMemory@4" (stream As FIMEMORY, data As Byte, ByRef size_in_bytes As Long) As Long
Public Declare Function FreeImage_RegisterLocalPlugin Lib "FreeImage.dll" Alias "_FreeImage_RegisterLocalPlugin@20" (ByVal proc_address As Long, Optional ByVal format As String = 0, Optional ByVal description As String = 0, Optional ByVal extension As String = 0, Optional ByVal regexpr As String = 0) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_RegisterExternalPlugin Lib "FreeImage.dll" Alias "_FreeImage_RegisterExternalPlugin@20" (ByVal path As String, Optional ByVal format As String = 0, Optional ByVal description As String = 0, Optional ByVal extension As String = 0, Optional ByVal regexpr As String = 0) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetFIFCount Lib "FreeImage.dll" Alias "_FreeImage_GetFIFCount@0" () As Long
Public Declare Function FreeImage_SetPluginEnabled Lib "FreeImage.dll" Alias "_FreeImage_SetPluginEnabled@8" (ByVal fif As FREE_IMAGE_FORMAT, ByVal enable As Long) As Long
Public Declare Function FreeImage_IsPluginEnabled Lib "FreeImage.dll" Alias "_FreeImage_IsPluginEnabled@4" (ByVal fif As FREE_IMAGE_FORMAT) As Long
Public Declare Function FreeImage_GetFIFFromFormat Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromFormat@4" (ByVal format As String) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetFIFFromMime Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromMime@4" (ByVal mime As String) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetFormatFromFIF Lib "FreeImage.dll" Alias "_FreeImage_GetFormatFromFIF@4" (ByVal fif As FREE_IMAGE_FORMAT) As String
Public Declare Function FreeImage_GetFIFExtensionList Lib "FreeImage.dll" Alias "_FreeImage_GetFIFExtensionList@4" (ByVal fif As FREE_IMAGE_FORMAT) As String
Public Declare Function FreeImage_GetFIFDescription Lib "FreeImage.dll" Alias "_FreeImage_GetFIFDescription@4" (ByVal fif As FREE_IMAGE_FORMAT) As String
Public Declare Function FreeImage_GetFIFRegExpr Lib "FreeImage.dll" Alias "_FreeImage_GetFIFRegExpr@4" (ByVal fif As FREE_IMAGE_FORMAT) As String
Public Declare Function FreeImage_GetFIFMimeType Lib "FreeImage.dll" Alias "_FreeImage_GetFIFMimeType@4" (ByVal fif As FREE_IMAGE_FORMAT) As String
Public Declare Function FreeImage_GetFIFFromFilename Lib "FreeImage.dll" Alias "_FreeImage_GetFIFFromFilename@4" (ByVal filename As String) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_FIFSupportsReading Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsReading@4" (ByVal fif As FREE_IMAGE_FORMAT) As Long
Public Declare Function FreeImage_FIFSupportsWriting Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsWriting@4" (ByVal fif As FREE_IMAGE_FORMAT) As Long
Public Declare Function FreeImage_FIFSupportsExportBPP Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsExportBPP@8" (ByVal fif As FREE_IMAGE_FORMAT, ByVal bpp As Long) As Long
Public Declare Function FreeImage_FIFSupportsExportType Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsExportType@8" (ByVal fif As FREE_IMAGE_FORMAT, ByVal type_ As FREE_IMAGE_TYPE) As Long
Public Declare Function FreeImage_FIFSupportsICCProfiles Lib "FreeImage.dll" Alias "_FreeImage_FIFSupportsICCProfiles@4" (ByVal fif As FREE_IMAGE_FORMAT) As Long
Public Declare Function FreeImage_OpenMultiBitmap Lib "FreeImage.dll" Alias "_FreeImage_OpenMultiBitmap@20" (ByVal fif As FREE_IMAGE_FORMAT, ByVal filename As String, ByVal create_new As Long, ByVal read_only As Long, Optional ByVal keep_cache_in_memory As Long = 0) As Long
Public Declare Function FreeImage_CloseMultiBitmap Lib "FreeImage.dll" Alias "_FreeImage_CloseMultiBitmap@8" (ByVal bitmap As Long, Optional ByVal flags As Long = 0) As Long
Public Declare Function FreeImage_GetPageCount Lib "FreeImage.dll" Alias "_FreeImage_GetPageCount@4" (ByVal bitmap As Long) As Long
Public Declare Sub FreeImage_AppendPage Lib "FreeImage.dll" Alias "_FreeImage_AppendPage@8" (ByVal bitmap As Long, ByVal data As Long)
Public Declare Sub FreeImage_InsertPage Lib "FreeImage.dll" Alias "_FreeImage_InsertPage@12" (ByVal bitmap As Long, ByVal page As Long, ByVal data As Long)
Public Declare Sub FreeImage_DeletePage Lib "FreeImage.dll" Alias "_FreeImage_DeletePage@8" (ByVal bitmap As Long, ByVal page As Long)
Public Declare Function FreeImage_LockPage Lib "FreeImage.dll" Alias "_FreeImage_LockPage@8" (ByVal bitmap As Long, ByVal page As Long) As Long
Public Declare Sub FreeImage_UnlockPage Lib "FreeImage.dll" Alias "_FreeImage_UnlockPage@12" (ByVal bitmap As Long, ByVal page As Long, ByVal changed As Long)
Public Declare Function FreeImage_MovePage Lib "FreeImage.dll" Alias "_FreeImage_MovePage@12" (ByVal bitmap As Long, ByVal target As Long, ByVal source As Long) As Long
Public Declare Function FreeImage_GetLockedPageNumbers Lib "FreeImage.dll" Alias "_FreeImage_GetLockedPageNumbers@12" (ByVal bitmap As Long, ByRef pages As Long, ByRef count As Long) As Long
Public Declare Function FreeImage_GetFileType Lib "FreeImage.dll" Alias "_FreeImage_GetFileType@8" (ByVal filename As String, Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetFileTypeFromHandle Lib "FreeImage.dll" Alias "_FreeImage_GetFileTypeFromHandle@12" (ByVal io As Long, ByVal handle As Long, Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetFileTypeFromMemory Lib "FreeImage.dll" Alias "_FreeImage_GetFileTypeFromMemory@4" (stream As FIMEMORY, Optional ByVal size As Long = 0) As FREE_IMAGE_FORMAT
Public Declare Function FreeImage_GetImageType Lib "FreeImage.dll" Alias "_FreeImage_GetImageType@4" (ByVal dib As Long) As FREE_IMAGE_TYPE
Public Declare Function FreeImage_IsLittleEndian Lib "FreeImage.dll" Alias "_FreeImage_IsLittleEndian@0" () As Long
Public Declare Function FreeImage_LookupX11Color Lib "FreeImage.dll" Alias "_FreeImage_LookupX11Color@16" (ByVal szColor As String, ByRef nRed As Long, ByRef nGreen As Long, ByRef nBlue As Long) As Long
Public Declare Function FreeImage_LookupSVGColor Lib "FreeImage.dll" Alias "_FreeImage_LookupSVGColor@16" (ByVal szColor As String, ByRef nRed As Long, ByRef nGreen As Long, ByRef nBlue As Long) As Long
Public Declare Function FreeImage_GetBits Lib "FreeImage.dll" Alias "_FreeImage_GetBits@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetScanLine Lib "FreeImage.dll" Alias "_FreeImage_GetScanLine@8" (ByVal dib As Long, ByVal scanline As Long) As Long
Public Declare Function FreeImage_GetPixelIndex Lib "FreeImage.dll" Alias "_FreeImage_GetPixelIndex@16" (ByVal dib As Long, ByVal x As Long, ByVal y As Long, ByRef Value As Byte) As Long
Public Declare Function FreeImage_GetPixelColor Lib "FreeImage.dll" Alias "_FreeImage_GetPixelColor@16" (ByVal dib As Long, ByVal x As Long, ByVal y As Long, ByRef Value As RGBQUAD) As Long
Public Declare Function FreeImage_SetPixelIndex Lib "FreeImage.dll" Alias "_FreeImage_SetPixelIndex@16" (ByVal dib As Long, ByVal x As Long, ByVal y As Long, ByRef Value As Byte) As Long
Public Declare Function FreeImage_SetPixelColor Lib "FreeImage.dll" Alias "_FreeImage_SetPixelColor@16" (ByVal dib As Long, ByVal x As Long, ByVal y As Long, ByRef Value As RGBQUAD) As Long
Public Declare Function FreeImage_GetColorsUsed Lib "FreeImage.dll" Alias "_FreeImage_GetColorsUsed@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetBPP Lib "FreeImage.dll" Alias "_FreeImage_GetBPP@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetWidth Lib "FreeImage.dll" Alias "_FreeImage_GetWidth@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetHeight Lib "FreeImage.dll" Alias "_FreeImage_GetHeight@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetLine Lib "FreeImage.dll" Alias "_FreeImage_GetLine@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetPitch Lib "FreeImage.dll" Alias "_FreeImage_GetPitch@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetDIBSize Lib "FreeImage.dll" Alias "_FreeImage_GetDIBSize@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetPalette Lib "FreeImage.dll" Alias "_FreeImage_GetPalette@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetDotsPerMeterX Lib "FreeImage.dll" Alias "_FreeImage_GetDotsPerMeterX@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetDotsPerMeterY Lib "FreeImage.dll" Alias "_FreeImage_GetDotsPerMeterY@4" (ByVal dib As Long) As Long
Public Declare Sub FreeImage_SetDotsPerMeterX Lib "FreeImage.dll" Alias "_FreeImage_SetDotsPerMeterX@8" (ByVal dib As Long, ByVal res As Long)
Public Declare Sub FreeImage_SetDotsPerMeterY Lib "FreeImage.dll" Alias "_FreeImage_SetDotsPerMeterY@8" (ByVal dib As Long, ByVal res As Long)
Public Declare Function FreeImage_GetInfoHeader Lib "FreeImage.dll" Alias "_FreeImage_GetInfoHeader@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetInfo Lib "FreeImage.dll" Alias "_FreeImage_GetInfo@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetColorType Lib "FreeImage.dll" Alias "_FreeImage_GetColorType@4" (ByVal dib As Long) As FREE_IMAGE_COLOR_TYPE
Public Declare Function FreeImage_GetRedMask Lib "FreeImage.dll" Alias "_FreeImage_GetRedMask@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetGreenMask Lib "FreeImage.dll" Alias "_FreeImage_GetGreenMask@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetBlueMask Lib "FreeImage.dll" Alias "_FreeImage_GetBlueMask@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetTransparencyCount Lib "FreeImage.dll" Alias "_FreeImage_GetTransparencyCount@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetTransparencyTable Lib "FreeImage.dll" Alias "_FreeImage_GetTransparencyTable@4" (ByVal dib As Long) As Long
Public Declare Sub FreeImage_SetTransparent Lib "FreeImage.dll" Alias "_FreeImage_SetTransparent@8" (ByVal dib As Long, ByVal enabled As Long)
Public Declare Sub FreeImage_SetTransparencyTable Lib "FreeImage.dll" Alias "_FreeImage_SetTransparencyTable@12" (ByVal dib As Long, ByRef table As Long, ByVal count As Long)
Public Declare Function FreeImage_IsTransparent Lib "FreeImage.dll" Alias "_FreeImage_IsTransparent@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_HasBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_HasBackgroundColor@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_GetBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_GetBackgroundColor@8" (ByVal dib As Long, ByVal bkcolor As Long) As Long
Public Declare Function FreeImage_SetBackgroundColor Lib "FreeImage.dll" Alias "_FreeImage_SetBackgroundColor@8" (ByVal dib As Long, ByVal bkcolor As Long) As Long
Public Declare Function FreeImage_GetICCProfile Lib "FreeImage.dll" Alias "_FreeImage_GetICCProfile@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_CreateICCProfile Lib "FreeImage.dll" Alias "_FreeImage_CreateICCProfile@12" (ByVal dib As Long, ByRef data As Long, ByVal size As Long) As Long
Public Declare Sub FreeImage_DestroyICCProfile Lib "FreeImage.dll" Alias "_FreeImage_DestroyICCProfile@4" (ByVal dib As Long)
Public Declare Sub FreeImage_ConvertLine1To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To8@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine4To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To8@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine16To8_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To8_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine16To8_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To8_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine24To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To8@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine32To8 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To8@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine1To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To16_555@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine4To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To16_555@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine8To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To16_555@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine16_565_To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16_565_To16_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine24To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To16_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine32To16_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To16_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine1To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To16_565@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine4To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To16_565@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine8To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To16_565@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine16_555_To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16_555_To16_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine24To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To16_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine32To16_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To16_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine1To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To24@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine4To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To24@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine8To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To24@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine16To24_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To24_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine16To24_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To24_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine32To24 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine32To24@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine1To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine1To32@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine4To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine4To32@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine8To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine8To32@16" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long, ByVal palette As Long)
Public Declare Sub FreeImage_ConvertLine16To32_555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To32_555@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine16To32_565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine16To32_565@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Sub FreeImage_ConvertLine24To32 Lib "FreeImage.dll" Alias "_FreeImage_ConvertLine24To32@12" (ByRef target As Long, ByRef source As Long, ByVal width_in_pixels As Long)
Public Declare Function FreeImage_ConvertTo4Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo4Bits@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ConvertTo8Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo8Bits@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ConvertTo16Bits555 Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo16Bits555@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ConvertTo16Bits565 Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo16Bits565@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ConvertTo24Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo24Bits@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ConvertTo32Bits Lib "FreeImage.dll" Alias "_FreeImage_ConvertTo32Bits@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_ColorQuantize Lib "FreeImage.dll" Alias "_FreeImage_ColorQuantize@8" (ByVal dib As Long, ByVal quantize As FREE_IMAGE_QUANTIZE) As Long
Public Declare Function FreeImage_Threshold Lib "FreeImage.dll" Alias "_FreeImage_Threshold@8" (ByVal dib As Long, ByVal T As Byte) As Long
Public Declare Function FreeImage_Dither Lib "FreeImage.dll" Alias "_FreeImage_Dither@8" (ByVal dib As Long, ByVal algorithm As FREE_IMAGE_DITHER) As Long
Public Declare Function FreeImage_ConvertFromRawBits Lib "FreeImage.dll" Alias "_FreeImage_ConvertFromRawBits@36" (ByRef bits As Long, ByVal width As Long, ByVal height As Long, ByVal pitch As Long, ByVal bpp As Long, ByVal red_mask As Long, ByVal green_mask As Long, ByVal blue_mask As Long, Optional ByVal topdown As Long = 0) As Long
Public Declare Sub FreeImage_ConvertToRawBits Lib "FreeImage.dll" Alias "_FreeImage_ConvertToRawBits@32" (ByRef bits As Long, ByVal dib As Long, ByVal pitch As Long, ByVal bpp As Long, ByVal red_mask As Long, ByVal green_mask As Long, ByVal blue_mask As Long, Optional ByVal topdown As Long = 0)
Public Declare Function FreeImage_ConvertToStandardType Lib "FreeImage.dll" Alias "_FreeImage_ConvertToStandardType@8" (ByVal src As Long, Optional ByVal scale_linear As Long = 1) As Long
Public Declare Function FreeImage_ConvertToType Lib "FreeImage.dll" Alias "_FreeImage_ConvertToType@12" (ByVal src As Long, ByVal dst_type As FREE_IMAGE_TYPE, Optional ByVal scale_linear As Long = 1) As Long
Public Declare Function FreeImage_ZLibCompress Lib "FreeImage.dll" Alias "_FreeImage_ZLibCompress@16" (ByRef target As Long, ByVal target_size As Long, ByRef source As Long, ByVal source_size As Long) As Long
Public Declare Function FreeImage_ZLibUncompress Lib "FreeImage.dll" Alias "_FreeImage_ZLibUncompress@16" (ByRef target As Long, ByVal target_size As Long, ByRef source As Long, ByVal source_size As Long) As Long
Public Declare Function FreeImage_RotateClassic Lib "FreeImage.dll" Alias "_FreeImage_RotateClassic@12" (ByVal dib As Long, ByVal angle As Double) As Long
Public Declare Function FreeImage_RotateEx Lib "FreeImage.dll" Alias "_FreeImage_RotateEx@48" (ByVal dib As Long, ByVal angle As Double, ByVal x_shift As Double, ByVal y_shift As Double, ByVal x_origin As Double, ByVal y_origin As Double, ByVal use_mask As Long) As Long
Public Declare Function FreeImage_FlipHorizontal Lib "FreeImage.dll" Alias "_FreeImage_FlipHorizontal@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_FlipVertical Lib "FreeImage.dll" Alias "_FreeImage_FlipVertical@4" (ByVal dib As Long) As Long
Public Declare Function FreeImage_Rescale Lib "FreeImage.dll" Alias "_FreeImage_Rescale@16" (ByVal dib As Long, ByVal dst_width As Long, ByVal dst_height As Long, ByVal filter As FREE_IMAGE_FILTER) As Long
Public Declare Function FreeImage_AdjustCurve Lib "FreeImage.dll" Alias "_FreeImage_AdjustCurve@12" (ByVal dib As Long, ByRef LUT As Long, ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long
Public Declare Function FreeImage_AdjustGamma Lib "FreeImage.dll" Alias "_FreeImage_AdjustGamma@12" (ByVal dib As Long, ByVal gamma As Double) As Long
Public Declare Function FreeImage_AdjustBrightness Lib "FreeImage.dll" Alias "_FreeImage_AdjustBrightness@12" (ByVal dib As Long, ByVal percentage As Double) As Long
Public Declare Function FreeImage_AdjustContrast Lib "FreeImage.dll" Alias "_FreeImage_AdjustContrast@12" (ByVal dib As Long, ByVal percentage As Double) As Long
Public Declare Function FreeImage_Invert Lib "FreeImage.dll" Alias "_FreeImage_Invert@4" (ByVal dib As Long) As Boolean
Public Declare Function FreeImage_GetHistogram Lib "FreeImage.dll" Alias "_FreeImage_GetHistogram@12" (ByVal dib As Long, ByRef histo As Long, Optional ByVal channel As FREE_IMAGE_COLOR_CHANNEL = FICC_BLACK) As Long
Public Declare Function FreeImage_GetChannel Lib "FreeImage.dll" Alias "_FreeImage_GetChannel@8" (ByVal dib As Long, ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long
Public Declare Function FreeImage_SetChannel Lib "FreeImage.dll" Alias "_FreeImage_SetChannel@12" (ByVal dib As Long, ByVal dib8 As Long, ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long
Public Declare Function FreeImage_GetComplexChannel Lib "FreeImage.dll" Alias "_FreeImage_GetComplexChannel@8" (ByVal src As Long, ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long
Public Declare Function FreeImage_SetComplexChannel Lib "FreeImage.dll" Alias "_FreeImage_SetComplexChannel@12" (ByVal dst As Long, ByVal src As Long, ByVal channel As FREE_IMAGE_COLOR_CHANNEL) As Long
Public Declare Function FreeImage_Copy Lib "FreeImage.dll" Alias "_FreeImage_Copy@20" (ByVal dib As Long, ByVal left As Long, ByVal top As Long, ByVal right As Long, ByVal bottom As Long) As Long
Public Declare Function FreeImage_Paste Lib "FreeImage.dll" Alias "_FreeImage_Paste@20" (ByVal dst As Long, ByVal src As Long, ByVal left As Long, ByVal top As Long, ByVal alpha As Long) As Long
Public Declare Function FreeImage_Composite Lib "FreeImage.dll" Alias "_FreeImage_Composite@16" (ByVal fg As Long, Optional ByVal useFileBkg As Long = 0, Optional ByVal appBkColor As Long = 0, Optional ByVal bg As Long = 0) As Long
