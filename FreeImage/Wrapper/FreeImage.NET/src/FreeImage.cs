// ==========================================================
// FreeImage.NET 3
//
// Design and implementation by
// - David Boland (davidboland@vodafone.ie)
//
// Contributors:
// - Andrew S. Townley
//
// This file is part of FreeImage 3
//
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS,
// WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
// INCLUDING, WITHOUT LIMITATION, WARRANTIES THAT THE COVERED CODE IS
// FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE OR
// NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE
// OF THE COVERED CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE
// DEFECTIVE IN ANY RESPECT, YOU (NOT THE INITIAL DEVELOPER OR ANY
// OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY SERVICING,
// REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN
// ESSENTIAL PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS
// AUTHORIZED HEREUNDER EXCEPT UNDER THIS DISCLAIMER.
//
// Use at your own risk!
//
// ==========================================================

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
	using PVOID = IntPtr;
	using FIBITMAP = UInt32;
	using FIMULTIBITMAP = UInt32;
	
	[StructLayout(LayoutKind.Sequential)]
	public class RGBQUAD
	{ 
		public byte rgbBlue;
		public byte rgbGreen;
		public byte rgbRed;
		public byte rgbReserved;
	}
	
/*	[StructLayout(LayoutKind.Sequential)]
	public class FreeImageIO
	{
		public FI_ReadProc readProc;
		public FI_WriteProc writeProc;
		public FI_SeekProc seekProc;
		public FI_TellProc tellProc;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public class FI_Handle
	{
		public FileStream stream;
	}
	public delegate void FI_ReadProc(IntPtr buffer, uint size, uint count, IntPtr handle);
	public delegate void FI_WriteProc(IntPtr buffer, uint size, uint count, IntPtr handle);
	public delegate int FI_SeekProc(IntPtr handle, long offset, int origin);
	public delegate int FI_TellProc(IntPtr handle);
	*/
	
	[StructLayout(LayoutKind.Sequential)]
	public class BITMAPINFOHEADER
	{
		public uint size;
		public int width; 
		public int height; 
		public ushort biPlanes; 
		public ushort biBitCount;
		public uint biCompression; 
		public uint biSizeImage; 
		public int biXPelsPerMeter; 
		public int biYPelsPerMeter; 
		public uint biClrUsed; 
		public uint biClrImportant;
	}


	[StructLayout(LayoutKind.Sequential)]
	public class BITMAPINFO
	{
	  public BITMAPINFOHEADER bmiHeader; 
	  public RGBQUAD bmiColors;
	}

	public enum FIF
	{
		FIF_UNKNOWN = -1,
		FIF_BMP		= 0,
		FIF_ICO		= 1,
		FIF_JPEG	= 2,
		FIF_JNG		= 3,
		FIF_KOALA	= 4,
		FIF_LBM		= 5,
		FIF_IFF = FIF_LBM,
		FIF_MNG		= 6,
		FIF_PBM		= 7,
		FIF_PBMRAW	= 8,
		FIF_PCD		= 9,
		FIF_PCX		= 10,
		FIF_PGM		= 11,
		FIF_PGMRAW	= 12,
		FIF_PNG		= 13,
		FIF_PPM		= 14,
		FIF_PPMRAW	= 15,
		FIF_RAS		= 16,
		FIF_TARGA	= 17,
		FIF_TIFF	= 18,
		FIF_WBMP	= 19,
		FIF_PSD		= 20,
		FIF_CUT		= 21,
		FIF_XBM		= 22,
		FIF_XPM		= 23,
		FIF_DDS     = 24,
		FIF_GIF     = 25
	}
	
	public enum FI_QUANTIZE
	{
		FIQ_WUQUANT = 0,
    	FIQ_NNQUANT = 1		
	}
	
	public enum FI_DITHER
	{
	    FID_FS			= 0,
		FID_BAYER4x4	= 1,
		FID_BAYER8x8	= 2,
		FID_CLUSTER6x6	= 3,
		FID_CLUSTER8x8	= 4,
		FID_CLUSTER16x16= 5
	}
	
	public enum FI_FILTER
	{
		FILTER_BOX		  = 0,
		FILTER_BICUBIC	  = 1,
		FILTER_BILINEAR   = 2,
		FILTER_BSPLINE	  = 3,
		FILTER_CATMULLROM = 4,
		FILTER_LANCZOS3	  = 5
	}

	public enum FI_COLOR_CHANNEL
	{
		FICC_RGB	= 0,
		FICC_RED	= 1,
		FICC_GREEN	= 2,
		FICC_BLUE	= 3,
		FICC_ALPHA	= 4,
		FICC_BLACK	= 5
	}

	public enum FIT // FREE_IMAGE_TYPE
	{
		FIT_UNKNOWN	= 0,
		FIT_BITMAP	= 1,
		FIT_UINT16	= 2,
		FIT_INT16	= 3,
		FIT_UINT32	= 4,
		FIT_INT32	= 5,
		FIT_FLOAT	= 6,
		FIT_DOUBLE	= 7,
		FIT_COMPLEX	= 8
	}

	public delegate void FreeImage_OutputMessageFunction(FIF format, string msg);
	
	public class FreeImage
	{
		// Init/Error routines ----------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Initialise")]
		public static extern void Initialise(bool loadLocalPluginsOnly);
		
		// alias for Americans :)
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Initialise")]
		public static extern void Initialize(bool loadLocalPluginsOnly);
		
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_DeInitialise")]
		public static extern void DeInitialise();
		
		// alias for Americians :)
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_DeInitialise")]
		public static extern void DeInitialize();
		
		
		
		// Version routines -------------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetVersion")]
		public static extern string GetVersion();
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetCopyrightMessage")]
		public static extern string GetCopyrightMessage();
	

		
		// Message Output routines ------------------------------------
		// missing void FreeImage_OutputMessageProc(int fif, 
		// 				const char *fmt, ...);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_SetOutputMessage")]
		public static extern void SetOutputMessage(FreeImage_OutputMessageFunction omf);
		
		
		
		// Allocate/Clone/Unload routines -----------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Allocate")]
		public static extern FIBITMAP Allocate(int width, int height, 
				int bpp, uint red_mask, uint green_mask, uint blue_mask);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AllocateT")]
		public static extern FIBITMAP AllocateT(FIT ftype, int width, 
				int height, int bpp, uint red_mask, uint green_mask, 
				uint blue_mask);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Clone")]
		public static extern FIBITMAP Clone(FIBITMAP dib);

		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Unload")]
		public static extern void Unload(FIBITMAP dib);
		


		// Load/Save routines -----------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Load")]
		public static extern FIBITMAP Load(FIF format, string filename, int flags);
	
		// missing FIBITMAP FreeImage_LoadFromHandle(FIF fif,
		// 				FreeImageIO *io, fi_handle handle, int flags);

		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Save")]
		public static extern bool Save(FIF format, FIBITMAP dib, string filename, int flags);
		
		// missing BOOL FreeImage_SaveToHandle(FIF fif, FIBITMAP *dib,
		// 				FreeImageIO *io, fi_handle handle, int flags);
		

		// Plugin interface -------------------------------------------
		// missing FIF FreeImage_RegisterLocalPlugin(FI_InitProc proc_address, 
		// 				const char *format, const char *description, 
		// 				const char *extension, const char *regexpr);
		//
		// missing FIF FreeImage_RegisterExternalPlugin(const char *path,
		// 				const char *format, const char *description,
		// 				const char *extension, const char *regexpr);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFCount")]
		public static extern int GetFIFCount();
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_SetPluginEnabled")]
		public static extern int SetPluginEnabled(FIF format, bool enabled);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_IsPluginEnabled")]
		public static extern int IsPluginEnabled(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFFromFormat")]
		public static extern FIF GetFIFFromFormat(string format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFFromMime")]
		public static extern FIF GetFIFFromMime(string mime);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFormatFromFIF")]
		public static extern string GetFormatFromFIF(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFExtensionList")]
		public static extern string GetFIFExtensionList(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFDescription")]
		public static extern string GetFIFDescription(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFRegExpr")]
		public static extern string GetFIFRegExpr(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFIFFromFilename")]
		public static extern FIF GetFIFFromFilename(string filename);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FIFSupportsReading")]
		public static extern bool FIFSupportsReading(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FIFSupportsWriting")]
		public static extern bool FIFSupportsWriting(FIF format);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FIFSupportsExportBPP")]
		public static extern bool FIFSupportsExportBPP(FIF format, int bpp);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FIFSupportsExportType")]
		public static extern bool FIFSupportsExportType(FIF format, FIT ftype);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FIFSupportsICCProfiles")]
		public static extern bool FIFSupportsICCProfiles(FIF format, FIT ftype);



		// Multipage interface ----------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_OpenMultiBitmap")]
		public static extern FIMULTIBITMAP OpenMultiBitmap(
			FIF format, string filename, bool createNew, bool readOnly, bool keepCacheInMemory);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_CloseMultiBitmap")]
		public static extern long CloseMultiBitmap(FIMULTIBITMAP bitmap, int flags);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetPageCount")]
		public static extern int GetPageCount(FIMULTIBITMAP bitmap);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AppendPage")]
		public static extern void AppendPage(FIMULTIBITMAP bitmap, FIBITMAP data);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_InsertPage")]
		public static extern void InsertPage(FIMULTIBITMAP bitmap, int page, FIBITMAP data);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_DeletePage")]
		public static extern void DeletePage(FIMULTIBITMAP bitmap, int page);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_LockPage")]
		public static extern FIBITMAP LockPage(FIMULTIBITMAP bitmap, int page);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_UnlockPage")]
		public static extern void UnlockPage(FIMULTIBITMAP bitmap, int page, bool changed);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_MovePage")]
		public static extern bool MovePage(FIMULTIBITMAP bitmap, int target, int source);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetLockedPageNumbers")]
		public static extern bool GetLockedPageNumbers(FIMULTIBITMAP bitmap, IntPtr pages, IntPtr count);
		
		
		
		// File type request routines ---------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetFileType")]
		public static extern FIF GetFileType(string filename, int size);

		// missing FIF FreeImage_GetFileTypeFromHandle(FreeImageIO *io,
		// 			fi_handle handle, int size);

		// Image type request routines --------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetImageType")]
		public static extern FIT GetImageType(FIBITMAP dib);

		
		
		// Info functions ---------------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_IsLittleEndian")]
		public static extern bool IsLittleEndian();
		
		
		
		// Pixel access functions -------------------------------------
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetBits")]
		public static extern IntPtr GetBits(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetScanLine")]
		public static extern IntPtr GetScanLine(FIBITMAP dib, int scanline);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetPixelIndex")]
		public static extern bool GetPixelIndex(FIBITMAP dib, uint x, uint y, byte value);
		
		
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetColorsUsed")]
		public static extern uint GetColorsUsed(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetBPP")]
		public static extern uint GetBPP(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetWidth")]
		public static extern uint GetWidth(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetHeight")]
		public static extern uint GetHeight(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetLine")]
		public static extern uint GetLine(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetPitch")]
		public static extern uint GetPitch(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetDIBSize")]
		public static extern uint GetDIBSize(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetPalette")]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		public static extern RGBQUAD GetPalette(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetDotsPerMeter")]
		public static extern uint GetDotsPerMeterX(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetDotsPerMeterY")]
		public static extern uint GetDotsPerMeterY(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetInfoHeader")]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		public static extern BITMAPINFOHEADER GetInfoHeader(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetInfo")]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		public static extern BITMAPINFO GetInfo(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetColorType")]
		public static extern int GetColorType(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetRedMask")]
		public static extern uint GetRedMask(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetGreenMask")]
		public static extern uint GetGreenMask(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetBlueMask")]
		public static extern uint GetBlueMask(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetTransparencyCount")]
		public static extern uint GetTransparencyCount(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetTransparencyTable")]
		public static extern IntPtr GetTransparencyTable(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_SetTransparent")]
		public static extern void SetTransparent(FIBITMAP dib, bool enabled);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_IsTransparent")]
		public static extern bool IsTransparent(FIBITMAP dib);
		
		
		


		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertTo8Bits")]
		public static extern FIBITMAP ConvertTo8Bits(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertTo16Bits555")]
		public static extern FIBITMAP ConvertTo16Bits555(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertTo16Bits565")]
		public static extern FIBITMAP ConvertTo16Bits565(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertTo24Bits")]
		public static extern FIBITMAP ConvertTo24Bits(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertTo32Bits")]
		public static extern FIBITMAP ConvertTo32Bits(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="ColorQuantize")]
		public static extern FIBITMAP ColorQuantize(FIBITMAP dib, FI_QUANTIZE quantize);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Threshold")]
		public static extern FIBITMAP Threshold(FIBITMAP dib, uint t);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Dither")]
		public static extern FIBITMAP Dither(FIBITMAP dib, FI_DITHER algorithm);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertFromRawBits")]
		public static extern FIBITMAP ConvertFromRawBits(byte[] bits, int width, int height,
			int pitch, uint bpp, uint redMask, uint greenMask, uint blueMask, bool topDown);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_ConvertToRawBits")]
		public static extern void ConvertToRawBits(IntPtr bits, FIBITMAP dib, int pitch,
			uint bpp, uint redMask, uint greenMask, uint blueMask, bool topDown);





		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_RotateClassic")]
		public static extern FIBITMAP RotateClassic(FIBITMAP dib, Double angle);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_RotateEx")]
		public static extern FIBITMAP RotateEx(
			FIBITMAP dib, Double angle, Double xShift, Double yShift, Double xOrigin, Double yOrigin, bool useMask);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FlipHorizontal")]
		public static extern bool FlipHorizontal(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_FlipVertical")]
		public static extern bool FlipVertical(FIBITMAP dib);
		
		
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Rescale")]
		public static extern FIBITMAP Rescale(FIBITMAP dib, int dst_width, int dst_height, FI_FILTER filter);
		
		
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AdjustCurve")]
		public static extern bool AdjustCurve(FIBITMAP dib, byte[] lut, FI_COLOR_CHANNEL channel);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AdjustGamma")]
		public static extern bool AdjustGamma(FIBITMAP dib, Double gamma);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AdjustBrightness")]
		public static extern bool AdjustBrightness(FIBITMAP dib, Double percentage);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_AdjustContrast")]
		public static extern bool AdjustContrast(FIBITMAP dib, Double percentage);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Invert")]
		public static extern bool Invert(FIBITMAP dib);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetHistogram")]
		public static extern bool GetHistogram(FIBITMAP dib, int histo, FI_COLOR_CHANNEL channel);
		


		[DllImport("FreeImage.dll", EntryPoint="FreeImage_GetChannel")]
		public static extern FIBITMAP GetChannel(FIBITMAP dib, FI_COLOR_CHANNEL channel);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_SetChannel")]
		public static extern bool SetChannel(FIBITMAP dib, FIBITMAP dib8, FI_COLOR_CHANNEL channel);
		
		
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Copy")]
		public static extern FIBITMAP Copy(FIBITMAP dib, int left, int top, int right, int bottom);
		
		[DllImport("FreeImage.dll", EntryPoint="FreeImage_Paste")]
		public static extern bool Paste(FIBITMAP dst, FIBITMAP src, int left, int top, int alpha);
	}
}
