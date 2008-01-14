using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using FreeImageAPI;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using FreeImageNETUnitTest;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace FreeImageNETUnitTest
{
	[TestFixture]
	public class ImportedFunctionsTest
	{
		ImageManager iManager = new ImageManager();
		FIBITMAP dib;
		string freeImageCallback = null;

		[TestFixtureSetUp]
		public void Init()
		{
			FreeImage.Message += new OutputMessageFunction(FreeImage_Message);
		}

		[TestFixtureTearDown]
		public void DeInit()
		{
			FreeImage.Message -= new OutputMessageFunction(FreeImage_Message);
		}

		[SetUp]
		public void InitEachTime()
		{
		}

		[TearDown]
		public void DeInitEachTime()
		{
		}

		void FreeImage_Message(FREE_IMAGE_FORMAT fif, string message)
		{
			freeImageCallback = message;
		}

		[Test]
		public void FreeImage_GetVersion()
		{
			string version = FreeImage.GetVersion();
			Assert.IsNotEmpty(version);
		}

		[Test]
		public void FreeImage_GetCopyrightMessage()
		{
			string copyright = FreeImage.GetCopyrightMessage();
			Assert.IsNotEmpty(copyright);
		}

		[Test]
		public void FreeImage_OutputMessageProc_SetOutputMessage()
		{
			Assert.IsNull(freeImageCallback);
			FreeImage.SetOutputMessage(new OutputMessageFunction(FreeImage_Message));
			FreeImage.OutputMessageProc(FREE_IMAGE_FORMAT.FIF_UNKNOWN, "unit test");
			FreeImage.SetOutputMessage(null);
			Assert.IsNotNull(freeImageCallback);
			freeImageCallback = null;
		}

		[Test]
		public void FreeImage_Allocate()
		{
			dib = FreeImage.Allocate(
				133,
				77,
				8,
				FreeImage.FI_RGBA_RED_MASK,
				FreeImage.FI_RGBA_GREEN_MASK,
				FreeImage.FI_RGBA_BLUE_MASK);

			Assert.AreNotEqual(0, dib);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AllocateT()
		{
			dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_RGBA16, 31, 555, 64, 0, 0, 0);

			Assert.AreNotEqual(0, dib);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Clone()
		{
			dib = FreeImage.Allocate(1, 1, 32, 0, 0, 0);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.Clone(dib);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Load()
		{
			Assert.AreEqual(0, dib);
			dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, iManager.baseDirectory + @"JPEG\Image.jpg", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, dib);
			FreeImage.UnloadEx(ref dib);
			Assert.AreEqual(0, dib);
		}

		[Test]
		public void FreeImage_Unload()
		{
			Assert.AreEqual(0, dib);
			dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, iManager.baseDirectory + @"JPEG\Image.jpg", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.IsNotNull(dib);
			FreeImage.Unload(dib);
			dib = 0;
		}

		[Test]
		public void FreeImage_LoadFromHandle()
		{
			byte[] data = File.ReadAllBytes(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_16_555));
			MemoryStream mStream = new MemoryStream(data);
			FreeImageIO io = FreeImageStreamIO.io;

			using (fi_handle handle = new fi_handle(mStream))
			{
				dib = FreeImage.LoadFromHandle(FREE_IMAGE_FORMAT.FIF_BMP, ref io, handle, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
				Assert.AreNotEqual(0, dib);

				FreeImage.UnloadEx(ref dib);
			}
		}

		[Test]
		public void FreeImage_Save()
		{
			string filename = @"test.bmp";
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.Save(FREE_IMAGE_FORMAT.FIF_BMP, dib, filename, FREE_IMAGE_SAVE_FLAGS.DEFAULT));
			Assert.IsTrue(File.Exists(filename));
			File.Delete(filename);
			Assert.IsFalse(File.Exists(filename));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SaveToHandle()
		{
			FreeImageIO io = new FreeImageIO();
			FreeImage.SaveToHandle(FREE_IMAGE_FORMAT.FIF_BMP, dib, ref io, new fi_handle(), FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		[Test]
		public void FreeImage_Memory()
		{
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.AreNotEqual(0, dib);
			FIMEMORY mem = FreeImage.OpenMemory(IntPtr.Zero, 0);
			Assert.AreNotEqual(0, mem);
			FreeImage.SaveToMemory(FREE_IMAGE_FORMAT.FIF_TIFF, dib, mem, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, FreeImage.TellMemory(mem));
			Assert.IsTrue(FreeImage.SeekMemory(mem, 0, System.IO.SeekOrigin.Begin));

			FIBITMAP temp = FreeImage.LoadFromMemory(FREE_IMAGE_FORMAT.FIF_TIFF, mem, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, temp);
			FreeImage.UnloadEx(ref temp);

			uint size = 0;
			byte[] ptr = new byte[1];
			IntPtr buffer = IntPtr.Zero;
			Assert.IsTrue(FreeImage.AcquireMemory(mem, ref buffer, ref size));
			Assert.AreNotEqual(IntPtr.Zero, ptr);
			Assert.AreNotEqual(0, size);

			Assert.AreEqual(1, FreeImage.WriteMemory(ptr, 1, 1, mem));
			FreeImage.SeekMemory(mem, 1, System.IO.SeekOrigin.Begin);
			Assert.AreEqual(1, FreeImage.TellMemory(mem));
			Assert.AreEqual(2, FreeImage.ReadMemory(ptr, 1, 2, mem));
			FreeImage.CloseMemory(mem);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_RegisterLocalPlugin()
		{
			InitProc proc = null;
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.RegisterLocalPlugin(proc, "", "", "", ""));
		}

		[Test]
		public void FreeImage_RegisterExternalPlugin()
		{
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.RegisterExternalPlugin("", "", "", "", ""));
		}

		[Test]
		public void FreeImage_GetFIFCount()
		{
			Assert.AreNotEqual(0, FreeImage.GetFIFCount());
		}

		[Test]
		public void FreeImage_SetPluginEnabled_IsPluginEnabled()
		{
			FreeImage.SetPluginEnabled(FREE_IMAGE_FORMAT.FIF_PNG, false);
			Assert.AreEqual(0, FreeImage.IsPluginEnabled(FREE_IMAGE_FORMAT.FIF_PNG));
			FreeImage.SetPluginEnabled(FREE_IMAGE_FORMAT.FIF_PNG, true);
			Assert.AreEqual(1, FreeImage.IsPluginEnabled(FREE_IMAGE_FORMAT.FIF_PNG));
		}

		[Test]
		public void FreeImage_GetFIFFromFormat()
		{
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromFormat(""));
			Assert.AreNotEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromFormat("TIFF"));
		}

		[Test]
		public void FreeImage_GetFIFFromMime()
		{
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromMime(""));
			Assert.AreNotEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromMime("image/jpeg"));
		}

		[Test]
		public void FreeImage_GetFormatFromFIF()
		{
			Assert.IsNotEmpty(FreeImage.GetFormatFromFIF(FREE_IMAGE_FORMAT.FIF_JNG));
		}

		[Test]
		public void FreeImage_GetFIFExtensionList()
		{
			Assert.IsNotEmpty(FreeImage.GetFIFExtensionList(FREE_IMAGE_FORMAT.FIF_PGM));
		}

		[Test]
		public void FreeImage_GetFIFDescription()
		{
			Assert.IsNotEmpty(FreeImage.GetFIFDescription(FREE_IMAGE_FORMAT.FIF_PBM));
		}

		[Test]
		public void FreeImage_GetFIFRegExpr()
		{
			Assert.IsNotEmpty(FreeImage.GetFIFRegExpr(FREE_IMAGE_FORMAT.FIF_JPEG));
		}

		[Test]
		public void FreeImage_GetFIFMimeType()
		{
			Assert.IsNotEmpty(FreeImage.GetFIFMimeType(FREE_IMAGE_FORMAT.FIF_ICO));
		}

		[Test]
		public void FreeImage_GetFIFFromFilename()
		{
			Assert.AreNotEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromFilename("test.bmp"));
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFIFFromFilename("test.000"));
		}

		[Test]
		public void FreeImage_FIFSupportsReading()
		{
			Assert.IsTrue(FreeImage.FIFSupportsReading(FREE_IMAGE_FORMAT.FIF_TIFF));
		}

		[Test]
		public void FreeImage_FIFSupportsWriting()
		{
			Assert.IsTrue(FreeImage.FIFSupportsWriting(FREE_IMAGE_FORMAT.FIF_GIF));
		}

		[Test]
		public void FreeImage_FIFSupportsExportBPP()
		{
			Assert.IsTrue(FreeImage.FIFSupportsExportBPP(FREE_IMAGE_FORMAT.FIF_BMP, 32));
			Assert.IsFalse(FreeImage.FIFSupportsExportBPP(FREE_IMAGE_FORMAT.FIF_GIF, 32));
		}

		[Test]
		public void FreeImage_FIFSupportsExportType()
		{
			Assert.IsTrue(FreeImage.FIFSupportsExportType(FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_TYPE.FIT_BITMAP));
			Assert.IsFalse(FreeImage.FIFSupportsExportType(FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_TYPE.FIT_COMPLEX));
		}

		[Test]
		public void FreeImage_FIFSupportsICCProfiles()
		{
			Assert.IsTrue(FreeImage.FIFSupportsICCProfiles(FREE_IMAGE_FORMAT.FIF_JPEG));
			Assert.IsFalse(FreeImage.FIFSupportsICCProfiles(FREE_IMAGE_FORMAT.FIF_BMP));
		}

		[Test]
		public void FreeImage_MultiBitmap()
		{
			FIBITMAP temp;
			FIMULTIBITMAP mdib = FreeImage.OpenMultiBitmap(
				FREE_IMAGE_FORMAT.FIF_TIFF,
				@"test.tif",
				true,
				false,
				true,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, mdib);
			Assert.AreEqual(0, FreeImage.GetPageCount(mdib));
			dib = FreeImage.Allocate(10, 10, 8, 0, 0, 0);
			FreeImage.AppendPage(mdib, dib);
			Assert.AreEqual(1, FreeImage.GetPageCount(mdib));
			FreeImage.AppendPage(mdib, dib);
			Assert.AreEqual(2, FreeImage.GetPageCount(mdib));
			FreeImage.AppendPage(mdib, dib);
			Assert.AreEqual(3, FreeImage.GetPageCount(mdib));
			FreeImage.CloseMultiBitmapEx(ref mdib);
			FreeImage.UnloadEx(ref dib);
			mdib = 0;
			mdib = FreeImage.OpenMultiBitmap(FREE_IMAGE_FORMAT.FIF_TIFF, @"test.tif", false, false, true, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, mdib);
			Assert.AreEqual(3, FreeImage.GetPageCount(mdib));
			dib = FreeImage.LockPage(mdib, 1);
			temp = FreeImage.LockPage(mdib, 2);

			int[] pages = null;
			int count = 0;
			FreeImage.GetLockedPageNumbers(mdib, pages, ref count);
			Assert.AreEqual(2, count);
			pages = new int[count];
			FreeImage.GetLockedPageNumbers(mdib, pages, ref count);
			Assert.AreEqual(2, pages.Length);
			FreeImage.UnlockPage(mdib, dib, false);
			FreeImage.UnlockPage(mdib, temp, true);
			dib = 0;
			Assert.IsTrue(FreeImage.MovePage(mdib, 0, 1));
			FreeImage.CloseMultiBitmapEx(ref mdib);
			Assert.IsTrue(System.IO.File.Exists("test.tif"));
			System.IO.File.Delete("test.tif");
		}

		[Test]
		public void FreeImage_GetFileType()
		{
			Assert.AreNotEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFileType(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_08_Greyscale_Unordered), 0));
		}

		[Test]
		public void FreeImage_GetFileTypeFromHandle()
		{
			FreeImageIO io = FreeImageStreamIO.io;
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFileTypeFromHandle(ref io, new fi_handle(), 0));
		}

		[Test]
		public void FreeImage_GetFileTypeFromMemory()
		{
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_UNKNOWN, FreeImage.GetFileTypeFromMemory(new FIMEMORY(0), 0));
		}

		[Test]
		public void FreeImage_IsLittleEndian()
		{
			Assert.IsTrue(FreeImage.IsLittleEndian());
		}

		[Test]
		public void FreeImage_LookupX11Color()
		{
			byte red, green, blue;
			FreeImage.LookupX11Color("lawngreen", out red, out green, out blue);
			Assert.AreEqual(124, red);
			Assert.AreEqual(252, green);
			Assert.AreEqual(0, blue);
		}

		[Test]
		public void FreeImage_LookupSVGColor()
		{
			byte red, green, blue;
			FreeImage.LookupX11Color("orchid", out red, out green, out blue);
			Assert.AreEqual(218, red);
			Assert.AreEqual(112, green);
			Assert.AreEqual(214, blue);
		}

		[Test]
		public void FreeImage_GetBits()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_01_Threshold);
			Assert.AreNotEqual(0, dib);
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetBits(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetScanLine()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetScanLine(dib, 0));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetPixelIndex_SetPixelIndex()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_04_Greyscale_Unordered);
			Assert.AreNotEqual(0, dib);
			byte index_old, index_new;
			Assert.IsTrue(FreeImage.GetPixelIndex(dib, 31, 10, out index_old));
			index_new = index_old == byte.MaxValue ? (byte)0 : (byte)(index_old + 1);
			Assert.IsTrue(FreeImage.SetPixelIndex(dib, 31, 10, ref index_new));
			Assert.IsTrue(FreeImage.GetPixelIndex(dib, 31, 10, out index_old));
			Assert.AreEqual(index_new, index_old);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetPixelColor_SetPixelColor()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			RGBQUAD value_old, value_new;
			Assert.IsTrue(FreeImage.GetPixelColor(dib, 77, 61, out value_old));
			value_new = (value_old == (RGBQUAD)Color.White) ? Color.Black : Color.White;
			Assert.IsTrue(FreeImage.SetPixelColor(dib, 77, 61, ref value_new));
			Assert.IsTrue(FreeImage.GetPixelColor(dib, 77, 61, out value_old));
			Assert.AreEqual(value_new, value_old);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Bitmap_information_functions()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_08_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_BITMAP, FreeImage.GetImageType(dib));
			Assert.AreNotEqual(0, FreeImage.GetColorsUsed(dib));
			Assert.AreEqual(8, FreeImage.GetBPP(dib));
			Assert.AreNotEqual(0, FreeImage.GetWidth(dib));
			Assert.AreNotEqual(0, FreeImage.GetHeight(dib));
			Assert.AreNotEqual(0, FreeImage.GetLine(dib));
			Assert.AreNotEqual(0, FreeImage.GetPitch(dib));
			Assert.AreNotEqual(0, FreeImage.GetDIBSize(dib));
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetPalette(dib));
			FreeImage.SetDotsPerMeterX(dib, 1234);
			FreeImage.SetDotsPerMeterY(dib, 4321);
			Assert.AreEqual(1234, FreeImage.GetDotsPerMeterX(dib));
			Assert.AreEqual(4321, FreeImage.GetDotsPerMeterY(dib));
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetInfoHeader(dib));
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetInfo(dib));
			Assert.AreEqual(FREE_IMAGE_COLOR_TYPE.FIC_MINISBLACK, FreeImage.GetColorType(dib));
			Assert.AreEqual(0, FreeImage.GetRedMask(dib));
			Assert.AreEqual(0, FreeImage.GetGreenMask(dib));
			Assert.AreEqual(0, FreeImage.GetBlueMask(dib));
			Assert.AreEqual(0, FreeImage.GetTransparencyCount(dib));
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.GetTransparencyTable(dib));
			FreeImage.SetTransparent(dib, false);
			FreeImage.SetTransparencyTable(dib, new byte[] { });
			Assert.IsTrue(FreeImage.IsTransparent(dib));
			Assert.IsFalse(FreeImage.HasBackgroundColor(dib));
			RGBQUAD rgb = Color.Teal;
			Assert.IsTrue(FreeImage.SetBackgroundColor(dib, ref rgb));
			Assert.IsTrue(FreeImage.GetBackgroundColor(dib, out rgb));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetICCProfile()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);
			new FIICCPROFILE(dib, new byte[] { 0xFF, 0xAA, 0x00, 0x33 });
			FIICCPROFILE p = FreeImage.GetICCProfileEx(dib);
			Assert.AreEqual(4, p.Size);
			Assert.AreEqual(0xAA, p.Data[1]);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_CreateICCProfile()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);
			byte[] data = new byte[256];
			Assert.AreNotEqual(IntPtr.Zero, FreeImage.CreateICCProfile(dib, data, 256));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_DestroyICCProfile()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);
			FreeImage.DestroyICCProfile(dib);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo4Bits()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo4Bits(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(4, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo8Bits()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo8Bits(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(8, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertToGreyscale()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertToGreyscale(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(8, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo16Bits555()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo16Bits555(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(16, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo16Bits565()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo16Bits565(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(16, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo24Bits()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo24Bits(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(24, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertTo32Bits()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertTo32Bits(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(32, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ColorQuantize()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ColorQuantize(dib, FREE_IMAGE_QUANTIZE.FIQ_WUQUANT);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(8, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ColorQuantizeEx()
		{
			FIBITMAP paletteDib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08);
			Assert.IsFalse(paletteDib.IsNull);
			RGBQUADARRAY palette = FreeImage.GetPaletteEx(paletteDib);
			RGBQUAD[] table = palette.Data;

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.ColorQuantizeEx(dib, FREE_IMAGE_QUANTIZE.FIQ_WUQUANT, (int)palette.Length, (int)palette.Length, table);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(8, FreeImage.GetBPP(temp));

			FreeImage.UnloadEx(ref paletteDib);
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Threshold()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.Threshold(dib, 128);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(1, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Dither()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.Dither(dib, FREE_IMAGE_DITHER.FID_FS);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(1, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_RawBits()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			IntPtr buffer = Marshal.AllocHGlobal((int)FreeImage.GetDIBSize(dib));
			FreeImage.ConvertToRawBits(
				buffer,
				dib,
				(int)FreeImage.GetPitch(dib),
				FreeImage.GetBPP(dib),
				FreeImage.GetRedMask(dib),
				FreeImage.GetGreenMask(dib),
				FreeImage.GetBlueMask(dib),
				true);
			FIBITMAP temp = FreeImage.ConvertFromRawBits(
				buffer,
				(int)FreeImage.GetWidth(dib),
				(int)FreeImage.GetHeight(dib),
				(int)FreeImage.GetPitch(dib),
				FreeImage.GetBPP(dib),
				FreeImage.GetRedMask(dib),
				FreeImage.GetGreenMask(dib),
				FreeImage.GetBlueMask(dib),
				true);

			Assert.AreNotEqual(0, temp);

			Marshal.FreeHGlobal(buffer);
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertToRGBF()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertToRGBF(dib);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_RGBF, FreeImage.GetImageType(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertToStandardType()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertToStandardType(dib, true);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(FREE_IMAGE_COLOR_TYPE.FIC_PALETTE, FreeImage.GetColorType(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertToType()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_Unordered);
			Assert.AreNotEqual(0, dib);
			FIBITMAP temp = FreeImage.ConvertToType(dib, FREE_IMAGE_TYPE.FIT_UINT32, true);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_UINT32, FreeImage.GetImageType(temp));
			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ToneMapping()
		{
			FIBITMAP temp;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP rgbf = FreeImage.ConvertToRGBF(dib);
			Assert.AreNotEqual(0, rgbf);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_RGBF, FreeImage.GetImageType(rgbf));
			Assert.AreEqual(96, FreeImage.GetBPP(rgbf));

			temp = FreeImage.ToneMapping(rgbf, FREE_IMAGE_TMO.FITMO_REINHARD05, 1f, 1.1f);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(24, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);

			FreeImage.UnloadEx(ref rgbf);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_TmoDrago03()
		{
			FIBITMAP temp;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP rgbf = FreeImage.ConvertToRGBF(dib);
			Assert.AreNotEqual(0, rgbf);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_RGBF, FreeImage.GetImageType(rgbf));
			Assert.AreEqual(96, FreeImage.GetBPP(rgbf));

			temp = FreeImage.TmoDrago03(rgbf, 1f, 1.2f);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(24, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);

			FreeImage.UnloadEx(ref rgbf);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_TmoReinhard05()
		{
			FIBITMAP temp;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP rgbf = FreeImage.ConvertToRGBF(dib);
			Assert.AreNotEqual(0, rgbf);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_RGBF, FreeImage.GetImageType(rgbf));
			Assert.AreEqual(96, FreeImage.GetBPP(rgbf));

			temp = FreeImage.TmoReinhard05(rgbf, 0f, 0.25f);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(24, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);

			FreeImage.UnloadEx(ref rgbf);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_TmoFattal02()
		{
			FIBITMAP temp;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP rgbf = FreeImage.ConvertToRGBF(dib);
			Assert.AreNotEqual(0, rgbf);
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_RGBF, FreeImage.GetImageType(rgbf));
			Assert.AreEqual(96, FreeImage.GetBPP(rgbf));

			temp = FreeImage.TmoFattal02(rgbf, 1f, 0.79f);
			Assert.AreNotEqual(0, temp);
			Assert.AreEqual(24, FreeImage.GetBPP(temp));
			FreeImage.UnloadEx(ref temp);

			FreeImage.UnloadEx(ref rgbf);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ZLibCompress_ZLibUncompress()
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			byte[] source = new byte[10240];
			byte[] compressed = new byte[(int)(10355f * 1.01 + 12f)];
			byte[] uncompressed = new byte[10240];
			rand.NextBytes(source);
			Assert.AreNotEqual(0, FreeImage.ZLibCompress(compressed, (uint)compressed.Length, source, (uint)source.Length));
			Assert.AreNotEqual(0, FreeImage.ZLibUncompress(uncompressed, (uint)source.Length, compressed, (uint)compressed.Length));
			for (int i = 0; i < source.Length; i++)
				if (source[i] != uncompressed[i])
					Assert.Fail();
		}

		[Test]
		public void FreeImage_ZLibGZip_ZLibGUnzip()
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			byte[] source = new byte[10240];
			byte[] compressed = new byte[(int)(10355f * 1.01 + 24f)];
			byte[] uncompressed = new byte[10240];
			rand.NextBytes(source);
			Assert.AreNotEqual(0, FreeImage.ZLibGZip(compressed, (uint)compressed.Length, source, (uint)source.Length));
			Assert.AreNotEqual(0, FreeImage.ZLibGUnzip(uncompressed, (uint)source.Length, compressed, (uint)compressed.Length));
			for (int i = 0; i < source.Length; i++)
				if (source[i] != uncompressed[i])
					Assert.Fail();
		}

		[Test]
		public void FreeImage_ZLibCRC32()
		{
			byte[] buffer = new byte[0];
			Assert.AreEqual(0xFEBCA008, FreeImage.ZLibCRC32(0xFEBCA008, buffer, 0));
		}

		[Test]
		public void FreeImage_CreateTag()
		{
			FITAG tag = FreeImage.CreateTag();
			Assert.AreNotEqual(0, tag);
			FITAG tag_clone = FreeImage.CloneTag(tag);
			Assert.AreNotEqual(0, tag_clone);
			FreeImage.DeleteTag(tag);
			FreeImage.DeleteTag(tag_clone);
		}

		[Test]
		public void FreeImage_Tag_accessors()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			Assert.IsTrue(FreeImage.FindNextMetadata(mData, out tag));
			Assert.AreNotEqual(0, tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagKey()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagKey(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagDescription()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagDescription(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagID()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagID(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagType()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagType(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagCount()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagCount(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagLength()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagLength(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTagValue()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.GetTagValue(tag);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagKey()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagKey(tag, "");

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagDescription()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagDescription(tag, "");

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagID()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagID(tag, 44);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagType()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagType(tag, FREE_IMAGE_MDTYPE.FIDT_ASCII);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagCount()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagCount(tag, 3);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagLength()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			FreeImage.SetTagLength(tag, 6);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTagValue()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			int length = (int)FreeImage.GetTagLength(tag);
			FREE_IMAGE_MDTYPE type = FreeImage.GetTagType(tag);
			int count = (int)FreeImage.GetTagCount(tag);

			byte[] buffer = new byte[length * count];

			FreeImage.SetTagValue(tag, buffer);

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetMetadataCount()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			Assert.AreNotEqual(0, FreeImage.GetMetadataCount(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_TagToString()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);

			FITAG tag;
			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, dib, out tag);
			Assert.AreNotEqual(0, mData);
			Assert.AreNotEqual(0, tag);

			Assert.IsNotEmpty(FreeImage.TagToString(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, tag, 0));

			FreeImage.FindCloseMetadata(mData);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_RotateClassic()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.RotateClassic(dib, 45d);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_RotateEx()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.RotateEx(dib, 261d, 0d, 33d, 51d, 9d, true);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_FlipHorizontal()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.FlipHorizontal(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_FlipVertical()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.FlipVertical(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_JPEGTransform()
		{
			string filename = iManager.GetBitmapPath(ImageType.JPEG, ImageColorType.Type_24);
			string filenameOut = filename + ".out.jpg";
			Assert.IsTrue(File.Exists(filename));

			Assert.IsTrue(FreeImage.JPEGTransform(filename, filenameOut, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_V, false));
			Assert.IsTrue(File.Exists(filenameOut));

			FIBITMAP temp = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, filenameOut, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);
			Assert.AreNotEqual(0, temp);

			File.Delete(filenameOut);
			Assert.IsFalse(File.Exists(filenameOut));

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Rescale()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_555);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.Rescale(dib, 100, 100, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_MakeThumbnail()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_555);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.MakeThumbnail(dib, 50, false);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AdjustCurve()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			byte[] lut = new byte[256];
			Assert.IsTrue(FreeImage.AdjustCurve(dib, lut, FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AdjustGamma()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.AdjustGamma(dib, 1.3d));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AdjustBrightness()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.AdjustBrightness(dib, 1.3d));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AdjustContrast()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.AdjustContrast(dib, 1.3d));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Invert()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_555);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.Invert(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetHistogram()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			int[] histo = new int[256];
			Assert.IsTrue(FreeImage.GetHistogram(dib, histo, FREE_IMAGE_COLOR_CHANNEL.FICC_RED));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetChannel()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.GetChannel(dib, FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetChannel()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP dib8 = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08);
			Assert.AreNotEqual(0, dib8);

			Assert.IsTrue(FreeImage.SetChannel(dib, dib8, FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE));

			FreeImage.UnloadEx(ref dib8);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetComplexChannel()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.ConvertToType(dib, FREE_IMAGE_TYPE.FIT_COMPLEX, true);
			Assert.AreNotEqual(0, temp);

			FIBITMAP temp2 = FreeImage.GetComplexChannel(temp, FREE_IMAGE_COLOR_CHANNEL.FICC_IMAG);
			Assert.AreNotEqual(0, temp2);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref temp2);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetComplexChannel()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_Unordered);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.ConvertToType(dib, FREE_IMAGE_TYPE.FIT_COMPLEX, true);
			Assert.AreNotEqual(0, temp);

			FIBITMAP temp2 = FreeImage.GetComplexChannel(temp, FREE_IMAGE_COLOR_CHANNEL.FICC_IMAG);
			Assert.AreNotEqual(0, temp2);

			Assert.IsTrue(FreeImage.SetComplexChannel(temp, temp2, FREE_IMAGE_COLOR_CHANNEL.FICC_IMAG));

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref temp2);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Copy()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.Copy(dib, 5, 9, 44, 2);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Paste()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);

			FIBITMAP temp = FreeImage.Allocate(3, 3, 8, 0, 0, 0);
			Assert.IsTrue(FreeImage.Paste(dib, temp, 31, 3, 256));

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Composite()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);
			RGBQUAD rgbq = new RGBQUAD();

			FIBITMAP temp = FreeImage.Composite(dib, false, ref rgbq, 0);
			Assert.AreNotEqual(0, temp);

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_JPEGCrop()
		{
			string filename = iManager.GetBitmapPath(ImageType.JPEG, ImageColorType.Type_01_Dither);
			Assert.IsTrue(File.Exists(filename));
			string filenameOut = filename + ".out.jpg";

			Assert.IsTrue(FreeImage.JPEGCrop(filename, filenameOut, 3, 2, 1, 5));
			Assert.IsTrue(File.Exists(filenameOut));

			FIBITMAP temp = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, filenameOut, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);
			Assert.AreNotEqual(0, temp);

			File.Delete(filenameOut);
			Assert.IsFalse(File.Exists(filenameOut));

			FreeImage.UnloadEx(ref temp);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_PreMultiplyWithAlpha()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.IsFalse(dib.IsNull);

			Assert.IsTrue(FreeImage.PreMultiplyWithAlpha(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_MultigridPoissonSolver()
		{
			dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_FLOAT, 10, 10, 32, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			FIBITMAP dib2 = FreeImage.MultigridPoissonSolver(dib, 2);

			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref dib2);
		}

		[Test]
		public void FreeImage_GetAdjustColorsLookupTable()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			byte[] lut = new byte[256];
			FreeImage.GetAdjustColorsLookupTable(lut, 55d, 0d, 2.1d, false);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_AdjustColors()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			Assert.IsTrue(FreeImage.AdjustColors(dib, -4d, 22d, 1.1d, false));

			FreeImage.UnloadEx(ref dib);
		}

		[Ignore]
		public void FreeImage_ApplyColorMapping()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			FreeImage_ApplyColorMapping2(dib);
		}

		private void FreeImage_ApplyColorMapping2(FIBITMAP dib)
		{
			Assert.IsFalse(dib.IsNull);

			RGBQUADARRAY rgbqa = new RGBQUADARRAY(dib, 0);

			RGBQUAD[] src = new RGBQUAD[1];
			RGBQUAD[] dst = new RGBQUAD[1];
			src[0] = rgbqa.GetRGBQUAD(0);
			dst[0].color = src[0].color == Color.White ? Color.Thistle : Color.White;

			uint count = FreeImage.ApplyColorMapping(dib, src, dst, 1, true, false); // Memory
			Assert.That(count > 0);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SwapColors()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_08);
			Assert.IsFalse(dib.IsNull);

			RGBQUAD src = new RGBQUAD(Color.FromArgb(93, 119, 170));
			RGBQUAD dst = new RGBQUAD(Color.FromArgb(90, 130, 148));

			uint count = FreeImage.SwapColors(dib, ref src, ref dst, true);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ApplyPaletteIndexMapping()
		{
			// alle farbtiefen

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_04);
			Assert.IsFalse(dib.IsNull);

			byte[] src = { 0, 3, 1 };
			byte[] dst = { 3, 1, 0 };

			uint count = FreeImage.ApplyPaletteIndexMapping(dib, src, dst, 3, false);
			Assert.That(count > 0);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SwapPaletteIndices()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_04);
			Assert.IsFalse(dib.IsNull);

			byte src = 0;
			byte dst = 3;

			uint count = FreeImage.SwapPaletteIndices(dib, ref src, ref dst);
			Assert.That(count > 0);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetTransparentIndex()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04);
			Assert.IsFalse(dib.IsNull);

			FreeImage.SetTransparentIndex(dib, 0);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetTransparentIndex()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04);
			Assert.IsFalse(dib.IsNull);

			int i = FreeImage.GetTransparentIndex(dib);

			FreeImage.UnloadEx(ref dib);
		}
	}

	[TestFixture]
	public class ImportedStructsTest
	{
		ImageManager iManager = new ImageManager();
		FIBITMAP dib = 0;
		string freeImageCallback = null;

		[TestFixtureSetUp]
		public void Init()
		{
			FreeImage.Message += new OutputMessageFunction(FreeImage_Message);
		}

		[TestFixtureTearDown]
		public void DeInit()
		{
			FreeImage.Message -= new OutputMessageFunction(FreeImage_Message);
		}

		[SetUp]
		public void InitEachTime()
		{
		}

		[TearDown]
		public void DeInitEachTime()
		{
		}

		void FreeImage_Message(FREE_IMAGE_FORMAT fif, string message)
		{
			freeImageCallback = message;
		}

		public bool EqualColors(Color color1, Color color2)
		{
			if (color1.A != color2.A) return false;
			if (color1.R != color2.R) return false;
			if (color1.G != color2.G) return false;
			if (color1.B != color2.B) return false;
			return true;
		}

		[Test]
		public void RGBQUAD()
		{
			RGBQUAD rgbq = new RGBQUAD();
			Assert.AreEqual(0, rgbq.rgbBlue);
			Assert.AreEqual(0, rgbq.rgbGreen);
			Assert.AreEqual(0, rgbq.rgbRed);
			Assert.AreEqual(0, rgbq.rgbReserved);

			rgbq = new RGBQUAD(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgbq.color));

			rgbq = new RGBQUAD(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173, rgbq.rgbBlue);
			Assert.AreEqual(95, rgbq.rgbGreen);
			Assert.AreEqual(83, rgbq.rgbRed);
			Assert.AreEqual(133, rgbq.rgbReserved);

			rgbq.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgbq.color));

			rgbq.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgbq.color));

			rgbq.color = Color.White;
			Assert.AreEqual(255, rgbq.rgbBlue);
			Assert.AreEqual(255, rgbq.rgbGreen);
			Assert.AreEqual(255, rgbq.rgbRed);
			Assert.AreEqual(255, rgbq.rgbReserved);

			rgbq.color = Color.Black;
			Assert.AreEqual(0, rgbq.rgbBlue);
			Assert.AreEqual(0, rgbq.rgbGreen);
			Assert.AreEqual(0, rgbq.rgbRed);
			Assert.AreEqual(255, rgbq.rgbReserved);

			rgbq = Color.DarkGoldenrod;
			Color color = rgbq;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Test]
		public void RGBTRIPLE()
		{
			RGBTRIPLE rgbt = new RGBTRIPLE();
			Assert.AreEqual(0, rgbt.rgbtBlue);
			Assert.AreEqual(0, rgbt.rgbtGreen);
			Assert.AreEqual(0, rgbt.rgbtRed);

			rgbt = new RGBTRIPLE(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgbt.color));

			rgbt = new RGBTRIPLE(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173, rgbt.rgbtBlue);
			Assert.AreEqual(95, rgbt.rgbtGreen);
			Assert.AreEqual(83, rgbt.rgbtRed);

			rgbt.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgbt.color));

			rgbt.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgbt.color));

			rgbt.color = Color.White;
			Assert.AreEqual(255, rgbt.rgbtBlue);
			Assert.AreEqual(255, rgbt.rgbtGreen);
			Assert.AreEqual(255, rgbt.rgbtRed);

			rgbt.color = Color.Black;
			Assert.AreEqual(0, rgbt.rgbtBlue);
			Assert.AreEqual(0, rgbt.rgbtGreen);
			Assert.AreEqual(0, rgbt.rgbtRed);

			rgbt = Color.DarkGoldenrod;
			Color color = rgbt;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Test]
		public void FIRGB16()
		{
			FIRGB16 rgb = new FIRGB16();
			Assert.AreEqual(0 * 256, rgb.blue);
			Assert.AreEqual(0 * 256, rgb.green);
			Assert.AreEqual(0 * 256, rgb.red);

			rgb = new FIRGB16(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgb.color));

			rgb = new FIRGB16(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173 * 256, rgb.blue);
			Assert.AreEqual(95 * 256, rgb.green);
			Assert.AreEqual(83 * 256, rgb.red);

			rgb.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgb.color));

			rgb.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgb.color));

			rgb.color = Color.White;
			Assert.AreEqual(255 * 256, rgb.blue);
			Assert.AreEqual(255 * 256, rgb.green);
			Assert.AreEqual(255 * 256, rgb.red);

			rgb.color = Color.Black;
			Assert.AreEqual(0 * 256, rgb.blue);
			Assert.AreEqual(0 * 256, rgb.green);
			Assert.AreEqual(0 * 256, rgb.red);

			rgb = Color.DarkGoldenrod;
			Color color = rgb;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Test]
		public void FIRGBA16()
		{
			FIRGBA16 rgb = new FIRGBA16();
			Assert.AreEqual(0 * 256, rgb.blue);
			Assert.AreEqual(0 * 256, rgb.green);
			Assert.AreEqual(0 * 256, rgb.red);
			Assert.AreEqual(0 * 256, rgb.alpha);

			rgb = new FIRGBA16(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgb.color));

			rgb = new FIRGBA16(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173 * 256, rgb.blue);
			Assert.AreEqual(95 * 256, rgb.green);
			Assert.AreEqual(83 * 256, rgb.red);
			Assert.AreEqual(133 * 256, rgb.alpha);

			rgb.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgb.color));

			rgb.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgb.color));

			rgb.color = Color.White;
			Assert.AreEqual(255 * 256, rgb.blue);
			Assert.AreEqual(255 * 256, rgb.green);
			Assert.AreEqual(255 * 256, rgb.red);
			Assert.AreEqual(255 * 256, rgb.alpha);

			rgb.color = Color.Black;
			Assert.AreEqual(0 * 256, rgb.blue);
			Assert.AreEqual(0 * 256, rgb.green);
			Assert.AreEqual(0 * 256, rgb.red);
			Assert.AreEqual(255 * 256, rgb.alpha);

			rgb = Color.DarkGoldenrod;
			Color color = rgb;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Test]
		public void FIRGBF()
		{
			FIRGBF rgb = new FIRGBF();
			Assert.AreEqual(0 / 255f, rgb.blue);
			Assert.AreEqual(0 / 255f, rgb.green);
			Assert.AreEqual(0 / 255f, rgb.red);

			rgb = new FIRGBF(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgb.color));

			rgb = new FIRGBF(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173 / 255f, rgb.blue);
			Assert.AreEqual(95 / 255f, rgb.green);
			Assert.AreEqual(83 / 255f, rgb.red);

			rgb.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgb.color));

			rgb.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgb.color));

			rgb.color = Color.White;
			Assert.AreEqual(255 / 255f, rgb.blue);
			Assert.AreEqual(255 / 255f, rgb.green);
			Assert.AreEqual(255 / 255f, rgb.red);

			rgb.color = Color.Black;
			Assert.AreEqual(0 / 255f, rgb.blue);
			Assert.AreEqual(0 / 255f, rgb.green);
			Assert.AreEqual(0 / 255f, rgb.red);

			rgb = Color.DarkGoldenrod;
			Color color = rgb;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Test]
		public void FIRGBAF()
		{
			FIRGBAF rgb = new FIRGBAF();
			Assert.AreEqual(0 / 255f, rgb.blue);
			Assert.AreEqual(0 / 255f, rgb.green);
			Assert.AreEqual(0 / 255f, rgb.red);
			Assert.AreEqual(0 / 255f, rgb.alpha);

			rgb = new FIRGBAF(Color.Chartreuse);
			Assert.That(EqualColors(Color.Chartreuse, rgb.color));

			rgb = new FIRGBAF(Color.FromArgb(133, 83, 95, 173));
			Assert.AreEqual(173 / 255f, rgb.blue);
			Assert.AreEqual(95 / 255f, rgb.green);
			Assert.AreEqual(83 / 255f, rgb.red);
			Assert.AreEqual(133 / 255f, rgb.alpha);

			rgb.color = Color.Crimson;
			Assert.That(EqualColors(Color.Crimson, rgb.color));

			rgb.color = Color.MidnightBlue;
			Assert.That(EqualColors(Color.MidnightBlue, rgb.color));

			rgb.color = Color.White;
			Assert.AreEqual(255 / 255f, rgb.blue);
			Assert.AreEqual(255 / 255f, rgb.green);
			Assert.AreEqual(255 / 255f, rgb.red);
			Assert.AreEqual(255 / 255f, rgb.alpha);

			rgb.color = Color.Black;
			Assert.AreEqual(0 / 255f, rgb.blue);
			Assert.AreEqual(0 / 255f, rgb.green);
			Assert.AreEqual(0 / 255f, rgb.red);
			Assert.AreEqual(255 / 255f, rgb.alpha);

			rgb = Color.DarkGoldenrod;
			Color color = rgb;
			Assert.That(EqualColors(Color.DarkGoldenrod, color));
		}

		[Ignore]
		public void FICOMPLEX()
		{
		}

		[Test]
		public void FIBITMAP()
		{
			FIBITMAP var = new FIBITMAP();
			Assert.IsTrue(var.IsNull);

			var = 41;
			Assert.IsFalse(var.IsNull);

			var = 0;
			Assert.IsTrue(var.IsNull);

			var = new IntPtr(654321);
			Assert.AreEqual(654321, var);

			var = IntPtr.Zero;
			Assert.AreEqual(0, var);

			var = 654321;
			FIBITMAP compVar = var;
			Assert.AreEqual(654321, compVar);
			Assert.AreEqual(var, compVar);

			var = 733;
			compVar = 733;

			Assert.AreEqual(var, compVar);
			Assert.AreEqual(0, var.CompareTo(compVar));
			Assert.AreEqual(0, var.CompareTo((object)compVar));

			compVar = 33;
			Assert.AreEqual(1, var.CompareTo(compVar));
			Assert.AreEqual(1, var.CompareTo((object)compVar));

			compVar = 9999;
			Assert.AreEqual(-1, var.CompareTo(compVar));
			Assert.AreEqual(-1, var.CompareTo((object)compVar));

			var = 1000;
			compVar = 1000;
			Assert.IsTrue(var == compVar);
			Assert.IsFalse(var != compVar);
			Assert.IsTrue(var.Equals(compVar));
			Assert.IsTrue(var.Equals((object)compVar));
			Assert.That(var.CompareTo(compVar) == 0);
			Assert.That(var.CompareTo((object)compVar) == 0);

			compVar = 2000;
			Assert.IsFalse(var == compVar);
			Assert.IsTrue(var != compVar);
			Assert.IsFalse(var.Equals(compVar));
			Assert.IsFalse(var.Equals((object)compVar));
			Assert.That(var.CompareTo(compVar) < 0);
			Assert.That(var.CompareTo((object)compVar) < 0);

			compVar = 500;
			Assert.That(var.CompareTo(compVar) > 0);
			Assert.That(var.CompareTo((object)compVar) > 0);
		}

		[Test]
		public void fi_handle()
		{
			fi_handle var = new fi_handle();
			Assert.IsTrue(var.IsNull);

			string test = "hello word!";
			using (var = new fi_handle(test))
			{
				Assert.IsFalse(var.IsNull);

				object obj = var.GetObject();
				Assert.That(obj is string);
				Assert.AreSame(obj, test);
			}
		}

		[Test]
		public void FIICCPROFILE()
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			FIICCPROFILE var = new FIICCPROFILE();
			Assert.AreEqual(0, var.Data.Length);
			Assert.AreEqual(IntPtr.Zero, var.DataPointer);
			Assert.AreEqual(0, var.Size);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			byte[] data = new byte[512];
			rand.NextBytes(data);

			var = FreeImage.GetICCProfileEx(dib);
			Assert.AreEqual(0, var.Size);

			var = new FIICCPROFILE(dib, data, 256);
			Assert.AreEqual(256, var.Data.Length);
			Assert.AreNotEqual(IntPtr.Zero, var.DataPointer);
			Assert.AreEqual(256, var.Size);
			byte[] dataComp = var.Data;
			for (int i = 0; i < data.Length && i < dataComp.Length; i++)
				if (data[i] != dataComp[i])
					Assert.Fail();

			FreeImage.DestroyICCProfile(dib);
			var = FreeImage.GetICCProfileEx(dib);
			Assert.AreEqual(0, var.Size);

			var = new FIICCPROFILE(dib, data);
			Assert.AreEqual(512, var.Data.Length);
			Assert.AreNotEqual(IntPtr.Zero, var.DataPointer);
			Assert.AreEqual(512, var.Size);
			dataComp = var.Data;
			for (int i = 0; i < data.Length && i < dataComp.Length; i++)
				if (data[i] != dataComp[i])
					Assert.Fail();

			var = FreeImage.GetICCProfileEx(dib);
			Assert.AreEqual(512, var.Data.Length);
			Assert.AreNotEqual(IntPtr.Zero, var.DataPointer);
			Assert.AreEqual(512, var.Size);

			FreeImage.DestroyICCProfile(dib);
			var = FreeImage.GetICCProfileEx(dib);
			Assert.AreEqual(0, var.Size);

			FreeImage.UnloadEx(ref dib);
		}
	}

	[TestFixture]
	public class WrapperStructsTest
	{
		ImageManager iManager = new ImageManager();
		FIBITMAP dib = 0;
		string freeImageCallback = null;

		[TestFixtureSetUp]
		public void Init()
		{
			FreeImage.Message += new OutputMessageFunction(FreeImage_Message);
		}

		[TestFixtureTearDown]
		public void DeInit()
		{
			FreeImage.Message -= new OutputMessageFunction(FreeImage_Message);
		}

		[SetUp]
		public void InitEachTime()
		{
		}

		[TearDown]
		public void DeInitEachTime()
		{
		}

		void FreeImage_Message(FREE_IMAGE_FORMAT fif, string message)
		{
			freeImageCallback = message;
		}

		public bool EqualColors(Color color1, Color color2)
		{
			if (color1.A != color2.A) return false;
			if (color1.R != color2.R) return false;
			if (color1.G != color2.G) return false;
			if (color1.B != color2.B) return false;
			return true;
		}

		[Test]
		public void RGBQUADARRAY()
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			RGBQUADARRAY rgbq;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			FIBITMAP clone = FreeImage.Clone(dib);
			Assert.AreNotEqual(0, clone);

			Bitmap bitmap = new Bitmap(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_32));
			Assert.IsNotNull(bitmap);
			Assert.AreEqual(bitmap.Height, FreeImage.GetHeight(dib));
			Assert.AreEqual(bitmap.Width, FreeImage.GetWidth(dib));

			//
			// Testing constructors
			//

			rgbq = new RGBQUADARRAY();
			Assert.AreEqual(0, rgbq.Length);

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);
				rgbq = new RGBQUADARRAY(dib, number);
				RGBQUAD[] scanline = rgbq.Data;

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, scanline[i].rgbRed);
					Assert.AreEqual(org.G, scanline[i].rgbGreen);
					Assert.AreEqual(org.B, scanline[i].rgbBlue);
					Assert.AreEqual(org.A, scanline[i].rgbReserved);
				}
			}

			{
				int number = 0;
				rgbq = new RGBQUADARRAY(dib);
				RGBQUAD[] scanline = rgbq.Data;

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, scanline[i].rgbRed);
					Assert.AreEqual(org.G, scanline[i].rgbGreen);
					Assert.AreEqual(org.B, scanline[i].rgbBlue);
					Assert.AreEqual(org.A, scanline[i].rgbReserved);
				}
			}

			//
			// Testing the 'Data' property
			//

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);

				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, number), (uint)bitmap.Width);
				RGBQUAD[] orgScanline = rgbq.Data;
				RGBQUAD[] newScanline = new RGBQUAD[orgScanline.Length];

				for (int i = 0; i < newScanline.Length; i++)
				{
					int value = i % 256;
					newScanline[i].color = Color.FromArgb(value, value, value, value);
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, orgScanline[i].rgbRed);
					Assert.AreEqual(org.G, orgScanline[i].rgbGreen);
					Assert.AreEqual(org.B, orgScanline[i].rgbBlue);
					Assert.AreEqual(org.A, orgScanline[i].rgbReserved);
				}

				rgbq.Data = newScanline;
				RGBQUAD[] comp = rgbq.Data;

				for (int i = 0; i < newScanline.Length; i++)
					Assert.That(EqualColors(newScanline[i], comp[i]));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}


			//
			// Testing the index
			//

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, number), (uint)bitmap.Width);

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, rgbq[i].rgbRed);
					Assert.AreEqual(org.G, rgbq[i].rgbGreen);
					Assert.AreEqual(org.B, rgbq[i].rgbBlue);
					Assert.AreEqual(org.A, rgbq[i].rgbReserved);
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					int value = i % 256;
					rgbq[i] = new RGBQUAD(Color.FromArgb(value, value, value, value));
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					int value = i % 256;
					Assert.That(EqualColors(Color.FromArgb(value, value, value, value), rgbq[i].color));
				}

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetRed' and 'SetRed'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetRed(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).R, color);
				rgbq.SetRed(position, newColor);
				Assert.AreEqual(newColor, rgbq.GetRed(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetGreen' and 'SetGreen'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetGreen(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).G, color);
				rgbq.SetGreen(position, newColor);
				Assert.AreEqual(newColor, rgbq.GetGreen(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetBlue' and 'SetBlue'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetBlue(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).B, color);
				rgbq.SetBlue(position, newColor);
				Assert.AreEqual(newColor, rgbq.GetBlue(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}


			//
			// Testing 'GetAlpha' and 'SetAlpha'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetAlpha(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).A, color);
				rgbq.SetAlpha(position, newColor);
				Assert.AreEqual(newColor, rgbq.GetAlpha(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetColor' and 'SetColor'
			//

			for (int run = 0; run < 5; run++)
			{
				Color color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetColor(position);
				newColor = (color == Color.BlueViolet) ? Color.LightSlateGray : Color.Orchid;

				Assert.That(EqualColors(bitmap.GetPixel(position, bitmap.Height - 1), color));
				rgbq.SetColor(position, newColor);
				Assert.That(EqualColors(newColor, rgbq.GetColor(position)));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetUIntColor' and 'SetUIntColor'
			//

			for (int run = 0; run < 5; run++)
			{
				uint color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbq = new RGBQUADARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbq.GetUIntColor(position);
				newColor = (color == 0) ? 0x4B6A910A : color / 2;

				Assert.AreEqual((uint)bitmap.GetPixel(position, bitmap.Height - 1).ToArgb(), color);
				rgbq.SetUIntColor(position, newColor);
				Assert.AreEqual(newColor, rgbq.GetUIntColor(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			bitmap.Dispose();
			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref clone);
		}

		[Test]
		public void RGBTRIPLEARRAY()
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			RGBTRIPLEARRAY rgbt;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);
			FIBITMAP clone = FreeImage.Clone(dib);
			Assert.AreNotEqual(0, clone);

			Bitmap bitmap = new Bitmap(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_24));
			Assert.IsNotNull(bitmap);
			Assert.AreEqual(bitmap.Height, FreeImage.GetHeight(dib));
			Assert.AreEqual(bitmap.Width, FreeImage.GetWidth(dib));

			//
			// Testing constructors
			//

			rgbt = new RGBTRIPLEARRAY();
			Assert.AreEqual(0, rgbt.Length);

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);
				rgbt = new RGBTRIPLEARRAY(dib, number);
				RGBTRIPLE[] scanline = rgbt.Data;

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, scanline[i].rgbtRed);
					Assert.AreEqual(org.G, scanline[i].rgbtGreen);
					Assert.AreEqual(org.B, scanline[i].rgbtBlue);
				}
			}

			//
			// Testing the 'Data' property
			//

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);

				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, number), (uint)bitmap.Width);
				RGBTRIPLE[] orgScanline = rgbt.Data;
				RGBTRIPLE[] newScanline = new RGBTRIPLE[orgScanline.Length];

				for (int i = 0; i < newScanline.Length; i++)
				{
					int value = i % 256;
					newScanline[i].color = Color.FromArgb(value, value, value, value);
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, orgScanline[i].rgbtRed);
					Assert.AreEqual(org.G, orgScanline[i].rgbtGreen);
					Assert.AreEqual(org.B, orgScanline[i].rgbtBlue);
				}

				rgbt.Data = newScanline;
				RGBTRIPLE[] comp = rgbt.Data;

				for (int i = 0; i < newScanline.Length; i++)
					Assert.That(EqualColors(newScanline[i], comp[i]));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing the index
			//

			for (int run = 0; run < 5; run++)
			{
				int number = rand.Next(0, bitmap.Height - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, number), (uint)bitmap.Width);

				for (int i = 0; i < bitmap.Width; i++)
				{
					Color org = bitmap.GetPixel(i, bitmap.Height - number - 1);
					Assert.AreEqual(org.R, rgbt[i].rgbtRed);
					Assert.AreEqual(org.G, rgbt[i].rgbtGreen);
					Assert.AreEqual(org.B, rgbt[i].rgbtBlue);
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					int value = i % 256;
					rgbt[i] = new RGBTRIPLE(Color.FromArgb(value, value, value));
				}

				for (int i = 0; i < bitmap.Width; i++)
				{
					int value = i % 256;
					Assert.That(EqualColors(Color.FromArgb(value, value, value), rgbt[i].color));
				}

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetRed' and 'SetRed'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbt.GetRed(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).R, color);
				rgbt.SetRed(position, newColor);
				Assert.AreEqual(newColor, rgbt.GetRed(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetGreen' and 'SetGreen'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbt.GetGreen(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).G, color);
				rgbt.SetGreen(position, newColor);
				Assert.AreEqual(newColor, rgbt.GetGreen(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetBlue' and 'SetBlue'
			//

			for (int run = 0; run < 5; run++)
			{
				byte color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbt.GetBlue(position);
				newColor = (color == 0) ? (byte)208 : (byte)(color / 2);

				Assert.AreEqual(bitmap.GetPixel(position, bitmap.Height - 1).B, color);
				rgbt.SetBlue(position, newColor);
				Assert.AreEqual(newColor, rgbt.GetBlue(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetColor' and 'SetColor'
			//

			for (int run = 0; run < 5; run++)
			{
				Color color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbt.GetColor(position);
				newColor = (color == Color.BlueViolet) ? Color.LightSlateGray : Color.Orchid;

				Assert.That(EqualColors(bitmap.GetPixel(position, bitmap.Height - 1), color));
				rgbt.SetColor(position, newColor);
				Assert.That(EqualColors(newColor, rgbt.GetColor(position)));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			//
			// Testing 'GetUIntColor' and 'SetUIntColor'
			//

			for (int run = 0; run < 5; run++)
			{
				uint color, newColor;
				int position = rand.Next(0, bitmap.Width - 1);
				rgbt = new RGBTRIPLEARRAY(FreeImage.GetScanLine(dib, 0), (uint)bitmap.Width);

				color = rgbt.GetUIntColor(position);
				newColor = (color == 0) ? 0x006A910A : color / 2;

				Assert.AreEqual((uint)bitmap.GetPixel(position, bitmap.Height - 1).ToArgb() & 0x00FFFFFF, color);
				rgbt.SetUIntColor(position, newColor);
				Assert.AreEqual(newColor & 0x00FFFFFF, rgbt.GetUIntColor(position));

				FreeImage.UnloadEx(ref dib);
				dib = FreeImage.Clone(clone);
				Assert.AreNotEqual(0, dib);
			}

			bitmap.Dispose();
			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref clone);
		}

		[Test]
		public void FIRational()
		{
			FIRational rational1 = new FIRational();
			FIRational rational2 = new FIRational();
			FIRational rational3 = new FIRational();

			//
			// Constructors
			//

			Assert.That(rational1.Numerator == 0);
			Assert.That(rational1.Denominator == 0);

			rational1 = new FIRational(412, 33);
			Assert.That(rational1.Numerator == 412);
			Assert.That(rational1.Denominator == 33);

			rational2 = new FIRational(rational1);
			Assert.That(rational2.Numerator == 412);
			Assert.That(rational2.Denominator == 33);

			rational3 = new FIRational(5.75m);
			Assert.That(rational3.Numerator == 23);
			Assert.That(rational3.Denominator == 4);

			//
			// == !=
			//

			rational1 = new FIRational(421, 51);
			rational2 = rational1;
			Assert.That(rational1 == rational2);
			Assert.That(!(rational1 != rational2));

			rational2 = new FIRational(1, 7);
			Assert.That(rational1 != rational2);
			Assert.That(!(rational1 == rational2));

			//
			// > >= < <=
			//

			rational1 = new FIRational(51, 4);
			rational2 = new FIRational(27, 9);
			Assert.That(rational1 != rational2);
			Assert.That(rational1 > rational2);
			Assert.That(rational1 >= rational2);

			rational1 = new FIRational(-412, 4);
			Assert.That(rational1 != rational2);
			Assert.That(rational1 < rational2);
			Assert.That(rational1 <= rational2);

			//
			// + / -
			//

			rational1 = new FIRational(41, 3);
			rational2 = new FIRational(612, 412);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(-7852, 63);
			rational2 = new FIRational(666111, -7654);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(-513, 88);
			rational2 = new FIRational(413, 5);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(-513, 88);
			rational2 = new FIRational(413, 5);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(7531, 23144);
			rational2 = new FIRational(-412, 78777);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(513, -42123);
			rational2 = new FIRational(-42, 77);
			rational3 = rational1 - rational2;
			Assert.That((rational3 + rational2) == rational1);

			rational1 = new FIRational(44, 11);
			rational1 = -rational1;
			Assert.That(rational1.Numerator == -4 && rational1.Denominator == 1);

			//
			// %
			//

			rational1 = new FIRational(23, 8);
			rational2 = new FIRational(77, 777);
			Assert.That((rational1 % rational2) == 0);

			rational2 = -rational2;
			Assert.That((rational1 % rational2) == 0);

			rational2 = new FIRational(7, 4);
			rational3 = new FIRational(9, 8);
			Assert.That((rational1 % rational2) == rational3);

			rational2 = -rational2;
			Assert.That((rational1 % rational2) == rational3);

			//
			// ~
			//

			rational1 = new FIRational(41, 77);
			rational1 = ~rational1;
			Assert.That(rational1.Numerator == 77 && rational1.Denominator == 41);

			//
			// -
			//

			rational1 = new FIRational(52, 4);
			rational1 = -rational1;
			Assert.That(rational1 < 0);

			//
			// ++ --
			//

			rational1 = new FIRational(5, 3);
			rational1++;
			rational2 = new FIRational(8, 3);
			Assert.That(rational1 == rational2);

			rational1 = new FIRational(41, -43);
			rational1++;
			Assert.That(rational1 > 0.0f);

			rational1--;
			Assert.That(rational1 == new FIRational(41, -43));

			rational1 = new FIRational(8134, 312);
			Assert.That(rational1 != 26);

			//
			// Direct assigns
			//

			rational1 = (FIRational)0.75m;
			Assert.That(rational1.Numerator == 3 && rational1.Denominator == 4);
			rational1 = (FIRational)0.33;
			Assert.That(rational1.Numerator == 33 && rational1.Denominator == 100);
			rational1 = (FIRational)62.975m;
			Assert.That(((decimal)rational1.Numerator / (decimal)rational1.Denominator) == 62.975m);
			rational1 = (FIRational)(-73.0975m);
			Assert.That(((decimal)rational1.Numerator / (decimal)rational1.Denominator) == -73.0975m);
			rational1 = (FIRational)(7m / 9m);
			Assert.That(rational1.Numerator == 7 && rational1.Denominator == 9);
			rational1 = (FIRational)(-15m / 9m);
			Assert.That(rational1.Numerator == -5 && rational1.Denominator == 3);
			rational1 = (FIRational)(0.7777m);
			Assert.That(rational1.Denominator != 9);

			//
			// Properties
			//

			rational1 = new FIRational(515, 5);
			Assert.That(rational1.IsInteger);

			rational1 = new FIRational(876, 77);
			Assert.That(rational1.Truncate() == (876 / 77));

			//
			// Special cases
			//

			rational1 = new FIRational(0, 10000);
			Assert.That(rational1 == 0m);

			rational1 = new FIRational(10000, 0);
			Assert.That(rational1 == 0f);

			rational1 = new FIRational(0, 0);
			Assert.That(rational1 == 0d);

			rational1 = new FIRational(-1, 0);
			Assert.That(rational1 == 0);

			rational1 = new FIRational(0, -1);
			Assert.That(rational1 == 0);
		}

		[Ignore]
		public void StreamWrapper()
		{
			string url = @"http://freeimage.sourceforge.net/images/logo.jpg";

			//
			// Non blocking
			//

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			Assert.IsNotNull(req);

			req.Timeout = 1000;
			HttpWebResponse resp;
			try
			{
				resp = (HttpWebResponse)req.GetResponse();
			}
			catch
			{
				return;
			}
			Assert.IsNotNull(resp);

			Stream stream = resp.GetResponseStream();
			Assert.IsNotNull(stream);

			StreamWrapper wrapper = new StreamWrapper(stream, false);
			Assert.IsNotNull(wrapper);
			Assert.IsTrue(wrapper.CanRead && wrapper.CanSeek && !wrapper.CanWrite);

			byte[] buffer = new byte[1024 * 100];
			int read;
			int count = 0;

			do
			{
				read = wrapper.Read(buffer, count, buffer.Length - count);
				count += read;
			} while (read != 0);

			Assert.AreEqual(7972, count);
			Assert.AreEqual(7972, wrapper.Length);

			wrapper.Position = 0;
			Assert.AreEqual(0, wrapper.Position);

			byte[] test = new byte[buffer.Length];
			int countTest = 0;

			do
			{
				read = wrapper.Read(test, countTest, test.Length - countTest);
				countTest += read;
			} while (read != 0);

			Assert.AreEqual(count, countTest);

			for (int i = 0; i < countTest; i++)
				if (buffer[i] != test[i])
					Assert.Fail();

			resp.Close();
			wrapper.Dispose();
			stream.Dispose();

			//
			// Blocking
			//

			req = (HttpWebRequest)HttpWebRequest.Create(url);
			Assert.IsNotNull(req);

			resp = (HttpWebResponse)req.GetResponse();
			Assert.IsNotNull(resp);

			stream = resp.GetResponseStream();
			Assert.IsNotNull(stream);

			wrapper = new StreamWrapper(stream, true);
			Assert.IsNotNull(wrapper);
			Assert.IsTrue(wrapper.CanRead && wrapper.CanSeek && !wrapper.CanWrite);

			buffer = new byte[1024 * 100];
			count = 0;

			count = wrapper.Read(buffer, 0, buffer.Length);
			Assert.AreEqual(7972, count);

			resp.Close();
			stream.Dispose();
			wrapper.Dispose();

			//
			// Position & Read byte
			//

			buffer = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0xFE, 0xFD };
			stream = new MemoryStream(buffer);
			wrapper = new StreamWrapper(stream, false);

			Assert.That(0x00 == wrapper.ReadByte());
			Assert.That(0x01 == wrapper.ReadByte());
			Assert.That(0x02 == wrapper.ReadByte());
			Assert.That(0xFF == wrapper.ReadByte());
			Assert.That(0xFE == wrapper.ReadByte());
			Assert.That(0xFD == wrapper.ReadByte());
			Assert.That(-1 == wrapper.ReadByte());

			Assert.That(wrapper.Length == buffer.Length);

			wrapper.Seek(0, SeekOrigin.Begin);
			Assert.That(0x00 == wrapper.ReadByte());
			wrapper.Seek(3, SeekOrigin.Begin);
			Assert.That(0xFF == wrapper.ReadByte());
			wrapper.Seek(0, SeekOrigin.End);
			Assert.That(-1 == wrapper.ReadByte());
			wrapper.Seek(-2, SeekOrigin.End);
			Assert.That(0xFE == wrapper.ReadByte());
			wrapper.Seek(0, SeekOrigin.Begin);
			Assert.That(0x00 == wrapper.ReadByte());
			wrapper.Seek(2, SeekOrigin.Current);
			Assert.That(0xFF == wrapper.ReadByte());
			wrapper.Seek(1, SeekOrigin.Current);
			Assert.That(0xFD == wrapper.ReadByte());
			Assert.That(wrapper.Position != 0);
			wrapper.Reset();
			Assert.That(wrapper.Position == 0);

			wrapper.Dispose();
			stream.Position = 0;
			wrapper = new StreamWrapper(stream, false);

			wrapper.Seek(10, SeekOrigin.Begin);
			Assert.That(wrapper.Position == buffer.Length);

			wrapper.Dispose();
			stream.Dispose();
		}

		[Test]
		public void FI16RGB()
		{
			FI16RGB rgb16;
			int r, g, b;
			BitSettings settings = new BitSettings();
			Random rand = new Random();
			settings.BLUE_MASK = (ushort)FreeImage.FI16_555_BLUE_MASK;
			settings.BLUE_MAX = 31;
			settings.BLUE_SHIFT = (ushort)FreeImage.FI16_555_BLUE_SHIFT;
			settings.GREEN_MASK = (ushort)FreeImage.FI16_555_GREEN_MASK;
			settings.GREEN_MAX = 31;
			settings.GREEN_SHIFT = FreeImage.FI16_555_GREEN_SHIFT;
			settings.RED_MASK = (ushort)FreeImage.FI16_555_RED_MASK;
			settings.RED_MAX = 31;
			settings.RED_SHIFT = FreeImage.FI16_555_RED_SHIFT;

			rgb16 = new FI16RGB(0xFFFF, settings);
			Assert.That(rgb16.color.B == 255);
			Assert.That(rgb16.color.G == 255);
			Assert.That(rgb16.color.R == 255);

			rgb16.color = Color.FromArgb(0, 0, 0);
			Assert.That(rgb16.color.B == 0);
			Assert.That(rgb16.color.G == 0);
			Assert.That(rgb16.color.R == 0);

			double delta = Math.Ceiling(255f / (float)settings.BLUE_MAX);

			for (int i = 0; i < 1000; i++)
			{
				r = rand.Next(0, 255);
				g = rand.Next(0, 255);
				b = rand.Next(0, 255);
				rgb16.color = Color.FromArgb(r, g, b);

				Assert.That(Math.Abs((float)r - (float)rgb16.color.R) <= delta);
				Assert.That(Math.Abs((float)g - (float)rgb16.color.G) <= delta);
				Assert.That(Math.Abs((float)b - (float)rgb16.color.B) <= delta);
			}
		}

		[Test]
		public void FI16RGBARRAY()
		{
			FI16RGBARRAY scanline;
			Bitmap bitmap;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_16_555);
			Assert.IsFalse(dib.IsNull);
			uint width = FreeImage.GetWidth(dib);
			double delta = Math.Ceiling(255f / 31f);
			Random rand = new Random();

			bitmap = new Bitmap(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_16_555));
			Assert.IsNotNull(bitmap);

			//
			// Constructors
			//

			IntPtr buffer = Marshal.AllocHGlobal(2);

			scanline = new FI16RGBARRAY(buffer, 1, 0x0007, 0x0038, 0x01C0);
			Assert.That(scanline.Length == 1);

			//
			// SetUShort, GetUShort
			//

			scanline.SetUShort(0, 0x0000);
			Assert.That(scanline.GetUShort(0) == 0x0000);

			scanline.SetUShort(0, 0xFFFF);
			Assert.That(scanline.GetUShort(0) == 0xFFFF);

			scanline.SetUShort(0, 0x01C7);
			Assert.That(scanline.GetUShort(0) == 0x01C7);
			Assert.That(scanline.GetRed(0) == 255);
			Assert.That(scanline.GetGreen(0) == 0);
			Assert.That(scanline.GetBlue(0) == 255);

			scanline.SetUShort(0, 0x0038);
			Assert.That(scanline.GetRed(0) == 0);
			Assert.That(scanline.GetGreen(0) == 255);
			Assert.That(scanline.GetBlue(0) == 0);

			Marshal.FreeHGlobal(buffer);

			scanline = new FI16RGBARRAY(dib, 0);
			Assert.That(scanline.Length == width && scanline.Length == bitmap.Width);

			FI16RGB[] rgb = scanline.Data;
			Assert.That(scanline.Length == rgb.Length);

			//
			// Data property
			//

			for (int u = 0; u < bitmap.Width; u++)
			{
				Color org = bitmap.GetPixel(u, bitmap.Height - 1);
				Assert.That(Math.Abs(org.R - rgb[u].color.R) <= 1);
				Assert.That(Math.Abs(org.G - rgb[u].color.G) <= 1);
				Assert.That(Math.Abs(org.B - rgb[u].color.B) <= 1);
			}

			//
			// GetFI16RGB
			//

			for (int u = 0; u < bitmap.Width; u++)
			{
				Color org = bitmap.GetPixel(u, bitmap.Height - 1);
				Assert.That(Math.Abs(org.R - scanline.GetFI16RGB(u).color.R) <= 1);
				Assert.That(Math.Abs(org.G - scanline.GetFI16RGB(u).color.G) <= 1);
				Assert.That(Math.Abs(org.B - scanline.GetFI16RGB(u).color.B) <= 1);
			}

			//
			// GetColor
			//

			for (int u = 0; u < bitmap.Width; u++)
			{
				Color org = bitmap.GetPixel(u, bitmap.Height - 1);
				Color str = scanline.GetColor(u);
				Assert.That(Math.Abs(org.R - str.R) <= 1);
				Assert.That(Math.Abs(org.G - str.G) <= 1);
				Assert.That(Math.Abs(org.B - str.B) <= 1);
			}

			//
			// GetRed, GetGreen, GetBlue
			//

			for (int u = 0; u < bitmap.Width; u++)
			{
				Color org = bitmap.GetPixel(u, bitmap.Height - 1);
				Assert.That(Math.Abs(org.R - scanline.GetRed(u)) <= 1);
				Assert.That(Math.Abs(org.G - scanline.GetGreen(u)) <= 1);
				Assert.That(Math.Abs(org.B - scanline.GetBlue(u)) <= 1);
			}

			//
			// Data Property setter
			//

			for (int u = 0; u < scanline.Length; u++)
			{
				rgb[u].color = Color.FromArgb(u % 255, u % 255, u % 255);
			}

			for (int u = 0; u < scanline.Length; u++)
			{
				Assert.That(Math.Abs(rgb[u].color.R - (u % 255)) <= delta);
				Assert.That(Math.Abs(rgb[u].color.G - (u % 255)) <= delta);
				Assert.That(Math.Abs(rgb[u].color.B - (u % 255)) <= delta);
			}

			//
			// SetFI16RGB
			//

			for (int u = 0; u < scanline.Length; u++)
			{
				scanline.SetFI16RGB(u, scanline.GetFI16RGB((int)scanline.Length - 1 - u));
			}

			for (int u = (int)scanline.Length - 1; u >= 0; u--)
			{
				Assert.That(Math.Abs(rgb[u].color.R - (u % 255)) <= delta);
				Assert.That(Math.Abs(rgb[u].color.G - (u % 255)) <= delta);
				Assert.That(Math.Abs(rgb[u].color.B - (u % 255)) <= delta);
			}

			//
			// SetColor
			//

			Color[] cArray = new Color[scanline.Length];
			for (int u = 0; u < (int)scanline.Length; u++)
			{
				cArray[u] = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
				scanline.SetColor(u, cArray[u]);
			}

			for (int u = 0; u < (int)scanline.Length; u++)
			{
				Assert.That(Math.Abs(cArray[u].R - scanline.GetColor(u).R) <= delta);
				Assert.That(Math.Abs(cArray[u].G - scanline.GetColor(u).G) <= delta);
				Assert.That(Math.Abs(cArray[u].B - scanline.GetColor(u).B) <= delta);
			}

			//
			// SetRed, SetGreen, SetBlue
			//

			for (int u = 0; u < (int)scanline.Length; u++)
			{
				scanline.SetRed(u, cArray[u].R);
				scanline.SetGreen(u, cArray[u].G);
				scanline.SetBlue(u, cArray[u].B);
			}

			for (int u = 0; u < (int)scanline.Length; u++)
			{
				Assert.That(Math.Abs(cArray[u].R - scanline.GetRed(u)) <= delta);
				Assert.That(Math.Abs(cArray[u].G - scanline.GetGreen(u)) <= delta);
				Assert.That(Math.Abs(cArray[u].B - scanline.GetBlue(u)) <= delta);
			}

			bitmap.Dispose();
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FI8BITARRAY()
		{
			FI8BITARRAY scanline;
			int length = 25;
			Random rand = new Random();

			//
			// Constructors
			//

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_08_Greyscale_Unordered);
			Assert.IsFalse(dib.IsNull);

			scanline = new FI8BITARRAY(dib, 0);
			Assert.That(FreeImage.GetWidth(dib) == scanline.Length);
			FreeImage.UnloadEx(ref dib);

			//
			// Data property
			//

			IntPtr buffer = Marshal.AllocHGlobal(length);
			scanline = new FI8BITARRAY(buffer, (uint)length);

			byte[] data = new byte[length];
			rand.NextBytes(data);

			scanline.Data = data;
			byte[] copy = scanline.Data;

			Assert.That(copy.Length == data.Length);
			for (int i = 0; i < data.Length; i++)
				Assert.That(copy[i] == data[i]);

			//
			// SetIndex, GetIndex
			//

			rand.NextBytes(data);

			for (int i = 0; i < length; i++)
				scanline.SetIndex(i, data[i]);

			for (int i = 0; i < length; i++)
				Assert.That(data[i] == scanline.GetIndex(i));

			Marshal.FreeHGlobal(buffer);
		}

		[Test]
		public void FI4BITARRAY()
		{
			FI4BITARRAY scanline;
			byte[] data, copy;
			int length = 62;
			Random rand = new Random();
			IntPtr buffer = Marshal.AllocHGlobal(length / 2);

			scanline = new FI4BITARRAY(buffer, (uint)length);
			Assert.That(scanline.Length == (length));

			//
			// Data property
			//

			data = new byte[length];
			for (int i = 0; i < data.Length; i++)
				data[i] = (byte)(rand.Next(0, 15) % 16);

			scanline.Data = data;

			copy = scanline.Data;
			Assert.That(data.Length == copy.Length);

			for (int i = 0; i < copy.Length; i++)
				Assert.That(data[i] == copy[i]);

			//
			// SetIndex, GetIndex
			//

			for (int i = 0; i < data.Length; i++)
				data[i] = (byte)rand.Next(0, 15);

			for (int i = 0; i < data.Length; i++)
				scanline.SetIndex(i, data[i]);

			for (int i = 0; i < data.Length; i++)
				Assert.That(data[i] == scanline.GetIndex(i));

			Marshal.FreeHGlobal(buffer);
		}

		[Test]
		public void FI1BITARRAY()
		{
			int lengthInBytes = 13;
			Random rand = new Random();
			IntPtr buffer = Marshal.AllocHGlobal(lengthInBytes);
			FI1BITARRAY array;

			for (int i = 0; i < lengthInBytes; i++)
				Marshal.WriteByte(buffer, i, 0x00);

			for (int i = 0; i < 1000; i++)
			{
				int size = rand.Next(1, lengthInBytes * 8);
				array = new FI1BITARRAY(buffer, (uint)size);
				byte[] data = array.Data;

				for (int index = 0; index < data.Length; index++)
					if (data[index] != 0)
						Assert.Fail();
			}

			for (int i = 0; i < lengthInBytes; i++)
				Marshal.WriteByte(buffer, i, 0xFF);

			for (int i = 0; i < 1000; i++)
			{
				int size = rand.Next(1, lengthInBytes * 8);
				array = new FI1BITARRAY(buffer, (uint)size);
				byte[] data = array.Data;

				for (int index = 0; index < data.Length; index++)
					Assert.That(data[index] == 1);
			}

			for (int i = 0; i < lengthInBytes; i++)
				Marshal.WriteByte(buffer, i, 0xAA);

			for (int i = 0; i < 1000; i++)
			{
				int size = rand.Next(1, lengthInBytes * 8);
				array = new FI1BITARRAY(buffer, (uint)size);
				byte[] data = array.Data;

				for (int index = 0; index < data.Length; index++)
					if ((((index % 2) == 0) && data[index] != 1) || (((index % 2) == 1) && data[index] != 0))
						Assert.Fail();
			}

			array = new FI1BITARRAY(buffer, (uint)(lengthInBytes * 8));

			for (int i = 0; i < array.Length; i++)
				array.SetIndex(i, (byte)(i % 2));

			for (int i = 0; i < 1000; i++)
			{
				int size = rand.Next(1, lengthInBytes * 8);
				array = new FI1BITARRAY(buffer, (uint)size);
				byte[] data = array.Data;

				for (int index = 0; index < data.Length; index++)
					if ((((index % 2) == 0) && data[index] != 0) || (((index % 2) == 1) && data[index] != 1))
						Assert.Fail();
			}

			Marshal.FreeHGlobal(buffer);
		}

		[Test]
		public unsafe void FICOMPLEXARRAY()
		{
			int length = sizeof(double) * 2 * 100;
			Random rand = new Random();
			IntPtr buffer = Marshal.AllocHGlobal(length);
			double[][] matrix = new double[100][];
			long high, low, val;

			for (int c = 0; c < matrix.Length; c++)
			{
				matrix[c] = new double[2];

				high = rand.Next(0, int.MaxValue);
				low = rand.Next(0, int.MaxValue);
				high <<= 32;
				val = low + high;
				matrix[c][0] = (double)val;

				high = rand.Next(0, int.MaxValue);
				low = rand.Next(0, int.MaxValue);
				high <<= 32;
				val = low + high;
				matrix[c][1] = (double)val;
			}

			double* ptr = (double*)buffer;

			for (int c = 0; c < matrix.Length; c++)
			{
				*ptr++ = matrix[c][0];
				*ptr++ = matrix[c][1];
			}

			ptr = (double*)buffer;
			FICOMPLEXARRAY array = new FICOMPLEXARRAY(buffer, 100u);

			for (int c = 0; c < matrix.Length; c++)
			{
				Assert.AreEqual(matrix[c][0], array.GetFICOMPLEX(c).r);
				Assert.AreEqual(matrix[c][1], array.GetFICOMPLEX(c).i);
			}

			FICOMPLEX[] data = array.Data;

			fixed (FICOMPLEX* fix = data)
			{
				Assert.IsTrue(FreeImage.CompareMemory((void*)buffer, fix, sizeof(FICOMPLEX) * 100));
			}

			data = new FICOMPLEX[data.Length];
			for (int c = 0; c < data.Length; c++)
			{
				high = rand.Next(0, int.MaxValue);
				low = rand.Next(0, int.MaxValue);
				high <<= 32;
				val = low + high;
				data[c].r = (double)val;

				high = rand.Next(0, int.MaxValue);
				low = rand.Next(0, int.MaxValue);
				high <<= 32;
				val = low + high;
				data[c].i = (double)val;
			}

			array.Data = data;
			fixed (FICOMPLEX* fix = data)
			{
				Assert.IsTrue(FreeImage.CompareMemory((void*)buffer, fix, sizeof(FICOMPLEX) * 100));
			}

			FICOMPLEX* fix2 = (FICOMPLEX*)buffer;

			for (int c = 0; c < 100; c++)
			{
				Assert.AreEqual(fix2[c].r, array[c].r);
				Assert.AreEqual(fix2[c].i, array.GetFICOMPLEX(c).i);
			}

			for (int c = 0; c < 100; c++)
			{
				FICOMPLEX comp;
				comp.r = matrix[c][0];
				comp.i = matrix[c][1];
				array.SetFICOMPLEX(c, comp);
				Assert.AreEqual(comp, fix2[c]);
			}

			Marshal.FreeHGlobal(buffer);
		}

		[Ignore]
		public void LocalPlugin()
		{
		}

		[Test]
		public void FreeImageStreamIO()
		{
			Random rand = new Random();
			byte[] bBuffer = new byte[256];
			IntPtr buffer = Marshal.AllocHGlobal(256);

			MemoryStream stream = new MemoryStream();
			Assert.IsNotNull(stream);
			using (fi_handle handle = new fi_handle(stream))
			{

				FreeImageIO io = FreeImageAPI.FreeImageStreamIO.io;
				Assert.IsNotNull(io.readProc);
				Assert.IsNotNull(io.writeProc);
				Assert.IsNotNull(io.seekProc);
				Assert.IsNotNull(io.tellProc);

				//
				// Procs
				//

				rand.NextBytes(bBuffer);

				stream.Write(bBuffer, 0, bBuffer.Length);
				Assert.That(io.tellProc(handle) == stream.Position);
				Assert.That(io.seekProc(handle, 0, SeekOrigin.Begin) == 0);
				Assert.That(io.tellProc(handle) == 0);
				Assert.That(io.tellProc(handle) == stream.Position);

				// Read one block
				Assert.That(io.readProc(buffer, (uint)bBuffer.Length, 1, handle) == 1);
				for (int i = 0; i < bBuffer.Length; i++)
					Assert.That(Marshal.ReadByte(buffer, i) == bBuffer[i]);

				Assert.That(io.tellProc(handle) == stream.Position);
				Assert.That(io.seekProc(handle, 0, SeekOrigin.Begin) == 0);
				Assert.That(io.tellProc(handle) == stream.Position);

				// Read 1 byte block
				Assert.That(io.readProc(buffer, 1, (uint)bBuffer.Length, handle) == bBuffer.Length);
				for (int i = 0; i < bBuffer.Length; i++)
					Assert.That(Marshal.ReadByte(buffer, i) == bBuffer[i]);

				Assert.That(io.tellProc(handle) == stream.Position);
				Assert.That(io.seekProc(handle, 0, SeekOrigin.Begin) == 0);
				Assert.That(io.tellProc(handle) == stream.Position);

				rand.NextBytes(bBuffer);
				for (int i = 0; i < bBuffer.Length; i++)
					Marshal.WriteByte(buffer, i, bBuffer[i]);

				// Write one block

				Assert.That(io.writeProc(buffer, (uint)bBuffer.Length, 1, handle) == 1);
				for (int i = 0; i < bBuffer.Length; i++)
					Assert.That(Marshal.ReadByte(buffer, i) == bBuffer[i]);
				Assert.That(io.tellProc(handle) == stream.Position);

				Assert.That(io.seekProc(handle, 0, SeekOrigin.Begin) == 0);
				Assert.That(io.tellProc(handle) == 0);

				// write 1 byte block

				Assert.That(io.writeProc(buffer, 1, (uint)bBuffer.Length, handle) == bBuffer.Length);
				for (int i = 0; i < bBuffer.Length; i++)
					Assert.That(Marshal.ReadByte(buffer, i) == bBuffer[i]);
				Assert.That(io.tellProc(handle) == stream.Position);

				// Seek and tell

				Assert.That(io.seekProc(handle, 0, SeekOrigin.Begin) == 0);
				Assert.That(io.tellProc(handle) == 0);

				Assert.That(io.seekProc(handle, 127, SeekOrigin.Current) == 0);
				Assert.That(io.tellProc(handle) == 127);

				Assert.That(io.seekProc(handle, 0, SeekOrigin.End) == 0);
				Assert.That(io.tellProc(handle) == 256);

				Marshal.FreeHGlobal(buffer);
				stream.Dispose();
			}
		}

		[Test]
		public void MetadataTag()
		{
			FITAG tag;
			MetadataTag metaTag;

			Random rand = new Random();
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);

			Assert.That(FreeImage.GetMetadataCount(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib) > 0);

			FIMETADATA mData = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib, out tag);
			Assert.IsFalse(tag.IsNull);
			Assert.IsFalse(mData.IsNull);

			//
			// Constructors
			//

			metaTag = new MetadataTag(tag, dib);
			Assert.That(metaTag.Model == FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN);

			metaTag = new MetadataTag(tag, FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN);
			Assert.That(metaTag.Model == FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN);

			//
			// Properties
			//

			metaTag.ID = ushort.MinValue;
			Assert.That(metaTag.ID == ushort.MinValue);

			metaTag.ID = ushort.MaxValue;
			Assert.That(metaTag.ID == ushort.MaxValue);

			metaTag.ID = ushort.MaxValue / 2;
			Assert.That(metaTag.ID == ushort.MaxValue / 2);

			metaTag.Description = "";
			Assert.That(metaTag.Description == "");

			metaTag.Description = "A";
			Assert.That(metaTag.Description == "A");

			metaTag.Description = "ABCDEFG";
			Assert.That(metaTag.Description == "ABCDEFG");

			metaTag.Key = "";
			Assert.That(metaTag.Key == "");

			metaTag.Key = "A";
			Assert.That(metaTag.Key == "A");

			metaTag.Key = "ABCDEFG";
			Assert.That(metaTag.Key == "ABCDEFG");

			//
			// SetValue
			//

			try
			{
				metaTag.SetValue(null, FREE_IMAGE_MDTYPE.FIDT_ASCII);
				Assert.Fail();
			}
			catch
			{
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_ASCII
			//

			string testString = "";

			Assert.IsTrue(metaTag.SetValue(testString, FREE_IMAGE_MDTYPE.FIDT_ASCII));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((string)metaTag.Value).Length == 0);

			testString = "X";

			Assert.IsTrue(metaTag.SetValue(testString, FREE_IMAGE_MDTYPE.FIDT_ASCII));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((string)metaTag.Value).Length == testString.Length);
			Assert.That(((string)metaTag.Value) == testString);

			testString = "TEST-STRING";

			Assert.IsTrue(metaTag.SetValue(testString, FREE_IMAGE_MDTYPE.FIDT_ASCII));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((string)metaTag.Value).Length == testString.Length);
			Assert.That(((string)metaTag.Value) == testString);

			//
			// FREE_IMAGE_MDTYPE.FIDT_BYTE
			//			

			byte testByte;
			byte[] testByteArray;

			Assert.IsTrue(metaTag.SetValue(byte.MinValue, FREE_IMAGE_MDTYPE.FIDT_BYTE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((byte[])metaTag.Value).Length == 1);
			Assert.That(((byte[])metaTag.Value)[0] == byte.MinValue);

			Assert.IsTrue(metaTag.SetValue(byte.MaxValue, FREE_IMAGE_MDTYPE.FIDT_BYTE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((byte[])metaTag.Value).Length == 1);
			Assert.That(((byte[])metaTag.Value)[0] == byte.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testByte = (byte)rand.Next(byte.MinValue, byte.MaxValue);
				testByteArray = new byte[rand.Next(0, 31)];

				for (int j = 0; j < testByteArray.Length; j++)
					testByteArray[j] = (byte)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testByte, FREE_IMAGE_MDTYPE.FIDT_BYTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((byte[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 1);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_BYTE);
				Assert.That(((byte[])metaTag.Value)[0] == testByte);

				Assert.IsTrue(metaTag.SetValue(testByteArray, FREE_IMAGE_MDTYPE.FIDT_BYTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((byte[])metaTag.Value).Length == testByteArray.Length);
				Assert.That(metaTag.Count == testByteArray.Length);
				Assert.That(metaTag.Length == testByteArray.Length * 1);

				byte[] value = (byte[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testByteArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_DOUBLE
			//

			double testDouble;
			double[] testDoubleArray;

			Assert.IsTrue(metaTag.SetValue(double.MinValue, FREE_IMAGE_MDTYPE.FIDT_DOUBLE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((double[])metaTag.Value).Length == 1);
			Assert.That(((double[])metaTag.Value)[0] == double.MinValue);

			Assert.IsTrue(metaTag.SetValue(double.MaxValue, FREE_IMAGE_MDTYPE.FIDT_DOUBLE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((double[])metaTag.Value).Length == 1);
			Assert.That(((double[])metaTag.Value)[0] == double.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testDouble = (double)rand.NextDouble();
				testDoubleArray = new double[rand.Next(0, 31)];

				for (int j = 0; j < testDoubleArray.Length; j++)
					testDoubleArray[j] = rand.NextDouble();

				Assert.IsTrue(metaTag.SetValue(testDouble, FREE_IMAGE_MDTYPE.FIDT_DOUBLE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((double[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 8);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
				Assert.That(((double[])metaTag.Value)[0] == testDouble);

				Assert.IsTrue(metaTag.SetValue(testDoubleArray, FREE_IMAGE_MDTYPE.FIDT_DOUBLE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((double[])metaTag.Value).Length == testDoubleArray.Length);
				Assert.That(metaTag.Count == testDoubleArray.Length);
				Assert.That(metaTag.Length == testDoubleArray.Length * 8);

				double[] value = (double[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testDoubleArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_FLOAT
			//

			float testfloat;
			float[] testFloatArray;

			Assert.IsTrue(metaTag.SetValue(float.MinValue, FREE_IMAGE_MDTYPE.FIDT_FLOAT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((float[])metaTag.Value).Length == 1);
			Assert.That(((float[])metaTag.Value)[0] == float.MinValue);

			Assert.IsTrue(metaTag.SetValue(float.MaxValue, FREE_IMAGE_MDTYPE.FIDT_FLOAT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((float[])metaTag.Value).Length == 1);
			Assert.That(((float[])metaTag.Value)[0] == float.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testfloat = (float)rand.NextDouble();
				testFloatArray = new float[rand.Next(0, 31)];

				for (int j = 0; j < testFloatArray.Length; j++)
					testFloatArray[j] = (float)rand.NextDouble();

				Assert.IsTrue(metaTag.SetValue(testfloat, FREE_IMAGE_MDTYPE.FIDT_FLOAT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((float[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 4);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_FLOAT);
				Assert.That(((float[])metaTag.Value)[0] == testfloat);

				Assert.IsTrue(metaTag.SetValue(testFloatArray, FREE_IMAGE_MDTYPE.FIDT_FLOAT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((float[])metaTag.Value).Length == testFloatArray.Length);
				Assert.That(metaTag.Count == testFloatArray.Length);
				Assert.That(metaTag.Length == testFloatArray.Length * 4);

				float[] value = (float[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testFloatArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_IFD
			//

			uint testUint;
			uint[] testUintArray;

			Assert.IsTrue(metaTag.SetValue(uint.MinValue, FREE_IMAGE_MDTYPE.FIDT_IFD));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((uint[])metaTag.Value).Length == 1);
			Assert.That(((uint[])metaTag.Value)[0] == uint.MinValue);

			Assert.IsTrue(metaTag.SetValue(uint.MaxValue, FREE_IMAGE_MDTYPE.FIDT_IFD));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((uint[])metaTag.Value).Length == 1);
			Assert.That(((uint[])metaTag.Value)[0] == uint.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testUint = (uint)rand.NextDouble();
				testUintArray = new uint[rand.Next(0, 31)];

				for (int j = 0; j < testUintArray.Length; j++)
					testUintArray[j] = (uint)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testUint, FREE_IMAGE_MDTYPE.FIDT_IFD));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((uint[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 4);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_IFD);
				Assert.That(((uint[])metaTag.Value)[0] == testUint);

				Assert.IsTrue(metaTag.SetValue(testUintArray, FREE_IMAGE_MDTYPE.FIDT_IFD));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((uint[])metaTag.Value).Length == testUintArray.Length);
				Assert.That(metaTag.Count == testUintArray.Length);
				Assert.That(metaTag.Length == testUintArray.Length * 4);

				uint[] value = (uint[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testUintArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_LONG
			//

			Assert.IsTrue(metaTag.SetValue(uint.MinValue, FREE_IMAGE_MDTYPE.FIDT_LONG));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((uint[])metaTag.Value).Length == 1);
			Assert.That(((uint[])metaTag.Value)[0] == uint.MinValue);

			Assert.IsTrue(metaTag.SetValue(uint.MaxValue, FREE_IMAGE_MDTYPE.FIDT_LONG));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((uint[])metaTag.Value).Length == 1);
			Assert.That(((uint[])metaTag.Value)[0] == uint.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testUint = (uint)rand.NextDouble();
				testUintArray = new uint[rand.Next(0, 31)];

				for (int j = 0; j < testUintArray.Length; j++)
					testUintArray[j] = (uint)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testUint, FREE_IMAGE_MDTYPE.FIDT_LONG));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((uint[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 4);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_LONG);
				Assert.That(((uint[])metaTag.Value)[0] == testUint);

				Assert.IsTrue(metaTag.SetValue(testUintArray, FREE_IMAGE_MDTYPE.FIDT_LONG));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((uint[])metaTag.Value).Length == testUintArray.Length);
				Assert.That(metaTag.Count == testUintArray.Length);
				Assert.That(metaTag.Length == testUintArray.Length * 4);

				uint[] value = (uint[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testUintArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_NOTYPE
			//

			try
			{
				metaTag.SetValue(new object(), FREE_IMAGE_MDTYPE.FIDT_NOTYPE);
				Assert.Fail();
			}
			catch (NotSupportedException)
			{
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_PALETTE
			//

			RGBQUAD testRGBQUAD;
			RGBQUAD[] testRGBQUADArray;

			for (int i = 0; i < 10; i++)
			{
				testRGBQUAD = new RGBQUAD(Color.FromArgb(rand.Next()));
				testRGBQUADArray = new RGBQUAD[rand.Next(0, 31)];

				for (int j = 0; j < testRGBQUADArray.Length; j++)
					testRGBQUADArray[j] = new RGBQUAD(Color.FromArgb(rand.Next()));

				Assert.IsTrue(metaTag.SetValue(testRGBQUAD, FREE_IMAGE_MDTYPE.FIDT_PALETTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((RGBQUAD[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 4);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_PALETTE);
				Assert.That(((RGBQUAD[])metaTag.Value)[0] == testRGBQUAD);

				Assert.IsTrue(metaTag.SetValue(testRGBQUADArray, FREE_IMAGE_MDTYPE.FIDT_PALETTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((RGBQUAD[])metaTag.Value).Length == testRGBQUADArray.Length);
				Assert.That(metaTag.Count == testRGBQUADArray.Length);
				Assert.That(metaTag.Length == testRGBQUADArray.Length * 4);

				RGBQUAD[] value = (RGBQUAD[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testRGBQUADArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_RATIONAL
			//

			FIURational testFIURational;
			FIURational[] testFIURationalArray;

			for (int i = 0; i < 10; i++)
			{
				testFIURational = new FIURational((uint)rand.Next(), (uint)rand.Next());
				testFIURationalArray = new FIURational[rand.Next(0, 31)];

				for (int j = 0; j < testFIURationalArray.Length; j++)
					testFIURationalArray[j] = new FIURational((uint)rand.Next(), (uint)rand.Next());

				Assert.IsTrue(metaTag.SetValue(testFIURational, FREE_IMAGE_MDTYPE.FIDT_RATIONAL));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((FIURational[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 8);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
				Assert.That(((FIURational[])metaTag.Value)[0] == testFIURational);

				Assert.IsTrue(metaTag.SetValue(testFIURationalArray, FREE_IMAGE_MDTYPE.FIDT_RATIONAL));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((FIURational[])metaTag.Value).Length == testFIURationalArray.Length);
				Assert.That(metaTag.Count == testFIURationalArray.Length);
				Assert.That(metaTag.Length == testFIURationalArray.Length * 8);

				FIURational[] value = (FIURational[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testFIURationalArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_SBYTE
			//

			sbyte testSByte;
			sbyte[] testSByteArray;

			Assert.IsTrue(metaTag.SetValue(sbyte.MinValue, FREE_IMAGE_MDTYPE.FIDT_SBYTE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((sbyte[])metaTag.Value).Length == 1);
			Assert.That(((sbyte[])metaTag.Value)[0] == sbyte.MinValue);

			Assert.IsTrue(metaTag.SetValue(sbyte.MaxValue, FREE_IMAGE_MDTYPE.FIDT_SBYTE));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((sbyte[])metaTag.Value).Length == 1);
			Assert.That(((sbyte[])metaTag.Value)[0] == sbyte.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testSByte = (sbyte)rand.Next(sbyte.MinValue, sbyte.MaxValue);
				testSByteArray = new sbyte[rand.Next(0, 31)];

				for (int j = 0; j < testSByteArray.Length; j++)
					testSByteArray[j] = (sbyte)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testSByte, FREE_IMAGE_MDTYPE.FIDT_SBYTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((sbyte[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 1);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_SBYTE);
				Assert.That(((sbyte[])metaTag.Value)[0] == testSByte);

				Assert.IsTrue(metaTag.SetValue(testSByteArray, FREE_IMAGE_MDTYPE.FIDT_SBYTE));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((sbyte[])metaTag.Value).Length == testSByteArray.Length);
				Assert.That(metaTag.Count == testSByteArray.Length);
				Assert.That(metaTag.Length == testSByteArray.Length * 1);

				sbyte[] value = (sbyte[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testSByteArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_SHORT
			//

			ushort testUShort;
			ushort[] testUShortArray;

			Assert.IsTrue(metaTag.SetValue(ushort.MinValue, FREE_IMAGE_MDTYPE.FIDT_SHORT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((ushort[])metaTag.Value).Length == 1);
			Assert.That(((ushort[])metaTag.Value)[0] == ushort.MinValue);

			Assert.IsTrue(metaTag.SetValue(ushort.MaxValue, FREE_IMAGE_MDTYPE.FIDT_SHORT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((ushort[])metaTag.Value).Length == 1);
			Assert.That(((ushort[])metaTag.Value)[0] == ushort.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testUShort = (ushort)rand.Next(ushort.MinValue, sbyte.MaxValue);
				testUShortArray = new ushort[rand.Next(0, 31)];

				for (int j = 0; j < testUShortArray.Length; j++)
					testUShortArray[j] = (ushort)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testUShort, FREE_IMAGE_MDTYPE.FIDT_SHORT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((ushort[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 2);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_SHORT);
				Assert.That(((ushort[])metaTag.Value)[0] == testUShort);

				Assert.IsTrue(metaTag.SetValue(testUShortArray, FREE_IMAGE_MDTYPE.FIDT_SHORT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((ushort[])metaTag.Value).Length == testUShortArray.Length);
				Assert.That(metaTag.Count == testUShortArray.Length);
				Assert.That(metaTag.Length == testUShortArray.Length * 2);

				ushort[] value = (ushort[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testUShortArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_SLONG
			//

			int testInt;
			int[] testIntArray;

			Assert.IsTrue(metaTag.SetValue(int.MinValue, FREE_IMAGE_MDTYPE.FIDT_SLONG));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((int[])metaTag.Value).Length == 1);
			Assert.That(((int[])metaTag.Value)[0] == int.MinValue);

			Assert.IsTrue(metaTag.SetValue(int.MaxValue, FREE_IMAGE_MDTYPE.FIDT_SLONG));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((int[])metaTag.Value).Length == 1);
			Assert.That(((int[])metaTag.Value)[0] == int.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testInt = (int)rand.NextDouble();
				testIntArray = new int[rand.Next(0, 31)];

				for (int j = 0; j < testIntArray.Length; j++)
					testIntArray[j] = rand.Next();

				Assert.IsTrue(metaTag.SetValue(testInt, FREE_IMAGE_MDTYPE.FIDT_SLONG));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((int[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 4);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_SLONG);
				Assert.That(((int[])metaTag.Value)[0] == testInt);

				Assert.IsTrue(metaTag.SetValue(testIntArray, FREE_IMAGE_MDTYPE.FIDT_SLONG));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((int[])metaTag.Value).Length == testIntArray.Length);
				Assert.That(metaTag.Count == testIntArray.Length);
				Assert.That(metaTag.Length == testIntArray.Length * 4);

				int[] value = (int[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testIntArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_SRATIONAL
			//

			FIRational testFIRational;
			FIRational[] testFIRationalArray;

			for (int i = 0; i < 10; i++)
			{
				testFIRational = new FIRational(rand.Next(), rand.Next());
				testFIRationalArray = new FIRational[rand.Next(0, 31)];

				for (int j = 0; j < testFIRationalArray.Length; j++)
					testFIRationalArray[j] = new FIRational(rand.Next(), rand.Next());

				Assert.IsTrue(metaTag.SetValue(testFIRational, FREE_IMAGE_MDTYPE.FIDT_SRATIONAL));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((FIRational[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 8);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
				Assert.That(((FIRational[])metaTag.Value)[0] == testFIRational);

				Assert.IsTrue(metaTag.SetValue(testFIRationalArray, FREE_IMAGE_MDTYPE.FIDT_SRATIONAL));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((FIRational[])metaTag.Value).Length == testFIRationalArray.Length);
				Assert.That(metaTag.Count == testFIRationalArray.Length);
				Assert.That(metaTag.Length == testFIRationalArray.Length * 8);

				FIRational[] value = (FIRational[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testFIRationalArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_SSHORT
			//

			short testShort;
			short[] testShortArray;

			Assert.IsTrue(metaTag.SetValue(short.MinValue, FREE_IMAGE_MDTYPE.FIDT_SSHORT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((short[])metaTag.Value).Length == 1);
			Assert.That(((short[])metaTag.Value)[0] == short.MinValue);

			Assert.IsTrue(metaTag.SetValue(short.MaxValue, FREE_IMAGE_MDTYPE.FIDT_SSHORT));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((short[])metaTag.Value).Length == 1);
			Assert.That(((short[])metaTag.Value)[0] == short.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testShort = (short)rand.Next(short.MinValue, short.MaxValue);
				testShortArray = new short[rand.Next(0, 31)];

				for (int j = 0; j < testShortArray.Length; j++)
					testShortArray[j] = (short)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testShort, FREE_IMAGE_MDTYPE.FIDT_SSHORT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((short[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 2);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_SSHORT);
				Assert.That(((short[])metaTag.Value)[0] == testShort);

				Assert.IsTrue(metaTag.SetValue(testShortArray, FREE_IMAGE_MDTYPE.FIDT_SSHORT));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((short[])metaTag.Value).Length == testShortArray.Length);
				Assert.That(metaTag.Count == testShortArray.Length);
				Assert.That(metaTag.Length == testShortArray.Length * 2);

				short[] value = (short[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testShortArray[j] == value[j]);
			}

			//
			// FREE_IMAGE_MDTYPE.FIDT_UNDEFINED
			//

			Assert.IsTrue(metaTag.SetValue(byte.MinValue, FREE_IMAGE_MDTYPE.FIDT_UNDEFINED));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((byte[])metaTag.Value).Length == 1);
			Assert.That(((byte[])metaTag.Value)[0] == byte.MinValue);

			Assert.IsTrue(metaTag.SetValue(byte.MaxValue, FREE_IMAGE_MDTYPE.FIDT_UNDEFINED));
			Assert.IsNotNull(metaTag.Value);
			Assert.That(((byte[])metaTag.Value).Length == 1);
			Assert.That(((byte[])metaTag.Value)[0] == byte.MaxValue);

			for (int i = 0; i < 10; i++)
			{
				testByte = (byte)rand.Next(byte.MinValue, byte.MaxValue);
				testByteArray = new byte[rand.Next(0, 31)];

				for (int j = 0; j < testByteArray.Length; j++)
					testByteArray[j] = (byte)rand.Next();

				Assert.IsTrue(metaTag.SetValue(testByte, FREE_IMAGE_MDTYPE.FIDT_UNDEFINED));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((byte[])metaTag.Value).Length == 1);
				Assert.That(metaTag.Count == 1);
				Assert.That(metaTag.Length == 1);
				Assert.That(metaTag.Type == FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
				Assert.That(((byte[])metaTag.Value)[0] == testByte);

				Assert.IsTrue(metaTag.SetValue(testByteArray, FREE_IMAGE_MDTYPE.FIDT_UNDEFINED));
				Assert.IsNotNull(metaTag.Value);
				Assert.That(((byte[])metaTag.Value).Length == testByteArray.Length);
				Assert.That(metaTag.Count == testByteArray.Length);
				Assert.That(metaTag.Length == testByteArray.Length * 1);

				byte[] value = (byte[])metaTag.Value;

				for (int j = 0; j < value.Length; j++)
					Assert.That(testByteArray[j] == value[j]);
			}

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void MetadataModel()
		{
			MetadataTag tag;
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			MetadataModel model = new MDM_GEOTIFF(dib);
			Assert.AreEqual(0, model.Count);
			Assert.IsFalse(model.Exists);
			Assert.IsEmpty(model.List);
			Assert.AreEqual(model.Model, FREE_IMAGE_MDMODEL.FIMD_GEOTIFF);
			Assert.IsTrue(model.DestoryModel());
			foreach (MetadataTag m in model) Assert.Fail();

			tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_GEOTIFF);
			tag.Key = "KEY";
			tag.Value = 54321f;
			Assert.IsTrue(model.AddTag(tag));

			Assert.AreEqual(1, model.Count);
			Assert.IsTrue(model.Exists);
			Assert.IsNotEmpty(model.List);
			Assert.AreEqual(model.Model, FREE_IMAGE_MDMODEL.FIMD_GEOTIFF);

			Assert.IsTrue(model.DestoryModel());
			Assert.AreEqual(0, model.Count);
			Assert.IsFalse(model.Exists);
			Assert.IsEmpty(model.List);
			Assert.AreEqual(model.Model, FREE_IMAGE_MDMODEL.FIMD_GEOTIFF);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void ImageMetadata()
		{
			ImageMetadata metadata;
			List<MetadataModel> modelList;
			MetadataTag tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
			tag.Key = "KEY";
			tag.ID = 11;
			tag.Value = new double[] { 0d, 41d, -523d, -0.41d };

			dib = FreeImage.Allocate(1, 1, 1, 1, 0, 0);
			Assert.IsFalse(dib.IsNull);

			metadata = new ImageMetadata(dib, true);
			Assert.AreEqual(0, metadata.Count);
			Assert.IsTrue(metadata.HideEmptyModels);
			Assert.IsEmpty(metadata.List);

			metadata = new ImageMetadata(dib, false);
			Assert.AreEqual(FreeImage.FREE_IMAGE_MDMODELS.Length, metadata.Count);
			Assert.IsFalse(metadata.HideEmptyModels);
			Assert.IsNotEmpty(metadata.List);

			metadata.HideEmptyModels = true;
			metadata.AddTag(tag);

			Assert.AreEqual(1, metadata.Count);
			Assert.IsNotEmpty(metadata.List);

			modelList = metadata.List;
			Assert.AreEqual(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, modelList[0].Model);

			System.Collections.IEnumerator enumerator = metadata.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsNotNull((MetadataModel)enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());

			FreeImage.UnloadEx(ref dib);
		}
	}

	[TestFixture]
	public class WrapperFunctionsTest
	{
		ImageManager iManager = new ImageManager();
		FIBITMAP dib = 0;
		string freeImageCallback = null;

		[TestFixtureSetUp]
		public void Init()
		{
			FreeImage.Message += new OutputMessageFunction(FreeImage_Message);
		}

		[TestFixtureTearDown]
		public void DeInit()
		{
			FreeImage.Message -= new OutputMessageFunction(FreeImage_Message);
		}

		[SetUp]
		public void InitEachTime()
		{
		}

		[TearDown]
		public void DeInitEachTime()
		{
		}

		void FreeImage_Message(FREE_IMAGE_FORMAT fif, string message)
		{
			freeImageCallback = message;
		}

		public bool EqualColors(Color color1, Color color2)
		{
			if (color1.A != color2.A) return false;
			if (color1.R != color2.R) return false;
			if (color1.G != color2.G) return false;
			if (color1.B != color2.B) return false;
			return true;
		}

		//
		// Tests
		//

		[Test]
		public void FreeImage_GetWrapperVersion()
		{
			Assert.That(FreeImage.GetWrapperVersion() ==
				String.Format("{0}.{1}.{2}",
				FreeImage.FREEIMAGE_MAJOR_VERSION,
				FreeImage.FREEIMAGE_MINOR_VERSION,
				FreeImage.FREEIMAGE_RELEASE_SERIAL));
		}

		[Test]
		public void FreeImage_IsAvailable()
		{
			Assert.IsTrue(FreeImage.IsAvailable());
		}

		[Test]
		public void FreeImage_GetBitmap()
		{
			Bitmap bitmap = null;

			try
			{
				bitmap = FreeImage.GetBitmap(0);
			}
			catch
			{
			}
			Assert.IsNull(bitmap);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);

			bitmap = FreeImage.GetBitmap(dib);
			Assert.IsNotNull(bitmap);
			Assert.AreEqual((int)FreeImage.GetHeight(dib), bitmap.Height);
			Assert.AreEqual((int)FreeImage.GetWidth(dib), bitmap.Width);

			bitmap.Dispose();
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_CreateFromBitmap()
		{
			Bitmap bitmap = (Bitmap)Bitmap.FromFile(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_24));
			Assert.IsNotNull(bitmap);

			dib = FreeImage.CreateFromBitmap(bitmap);
			Assert.AreNotEqual(0, dib);

			Assert.AreEqual((int)FreeImage.GetHeight(dib), bitmap.Height);
			Assert.AreEqual((int)FreeImage.GetWidth(dib), bitmap.Width);

			bitmap.Dispose();
			FreeImage.UnloadEx(ref dib);

			try
			{
				dib = FreeImage.CreateFromBitmap(null);
				Assert.Fail();
			}
			catch
			{
			}
		}

		[Test]
		public void FreeImage_SaveBitmap()
		{
			Bitmap bitmap = (Bitmap)Bitmap.FromFile(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_24));
			Assert.IsNotNull(bitmap);

			Assert.IsTrue(FreeImage.SaveBitmap(bitmap, @"test.png", FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.DEFAULT));
			bitmap.Dispose();

			Assert.IsTrue(File.Exists(@"test.png"));

			dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, @"test.png", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			Assert.AreNotEqual(0, dib);

			FreeImage.UnloadEx(ref dib);

			File.Delete(@"test.png");
			Assert.IsFalse(File.Exists(@"test.png"));
			bitmap.Dispose();
		}

		[Test]
		public void FreeImage_LoadEx()
		{
			dib = FreeImage.LoadEx(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_16_555));
			Assert.AreNotEqual(0, dib);
			FreeImage.UnloadEx(ref dib);

			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_TIFF;

			dib = FreeImage.LoadEx(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_16_565), ref format);
			Assert.AreEqual(0, dib);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_TIFF, format);
			FreeImage.UnloadEx(ref dib);

			format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			dib = FreeImage.LoadEx(iManager.GetBitmapPath(ImageType.JPEG, ImageColorType.Type_16_565),
				FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
			Assert.AreNotEqual(0, dib);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_JPEG, format);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_UnloadEx()
		{
			Assert.AreEqual(0, dib);
			FreeImage.UnloadEx(ref dib);
			Assert.AreEqual(0, dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_555);
			Assert.AreNotEqual(0, dib);

			FreeImage.UnloadEx(ref dib);
			Assert.AreEqual(0, dib);
		}

		[Test]
		public void FreeImage_SaveEx()
		{
			FREE_IMAGE_FORMAT format;
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08);
			Assert.AreNotEqual(0, dib);

			Assert.IsTrue(FreeImage.SaveEx(dib, @"test.png"));
			Assert.IsTrue(File.Exists(@"test.png"));
			format = FreeImage.GetFileType(@"test.png", 0);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_PNG, format);
			File.Delete(@"test.png");
			Assert.IsFalse(File.Exists(@"test.png"));

			Assert.IsTrue(FreeImage.SaveEx(ref dib, @"test.tiff", FREE_IMAGE_SAVE_FLAGS.DEFAULT, false));
			Assert.IsTrue(File.Exists(@"test.tiff"));
			format = FreeImage.GetFileType(@"test.tiff", 0);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_TIFF, format);
			File.Delete(@"test.tiff");
			Assert.IsFalse(File.Exists(@"test.tiff"));

			Assert.IsTrue(FreeImage.SaveEx(
							ref dib,
							@"test.gif",
							FREE_IMAGE_FORMAT.FIF_UNKNOWN,
							FREE_IMAGE_SAVE_FLAGS.DEFAULT,
							FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP,
							false));

			Assert.IsTrue(File.Exists(@"test.gif"));
			format = FreeImage.GetFileType(@"test.gif", 0);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_GIF, format);
			File.Delete(@"test.gif");
			Assert.IsFalse(File.Exists(@"test.gif"));

			Assert.IsFalse(FreeImage.SaveEx(dib, @""));
			Assert.IsFalse(FreeImage.SaveEx(dib, @"test.test"));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_LoadFromStream()
		{
			FREE_IMAGE_FORMAT format;
			FileStream fStream;

			fStream = new FileStream(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_16_565), FileMode.Open);
			Assert.IsNotNull(fStream);

			dib = FreeImage.LoadFromStream(fStream);
			Assert.AreNotEqual(0, dib);
			Assert.That(FreeImage.GetBPP(dib) == 16);
			Assert.That(FreeImage.GetGreenMask(dib) == FreeImage.FI16_565_GREEN_MASK);

			FreeImage.UnloadEx(ref dib);
			fStream.Close();

			fStream = new FileStream(iManager.GetBitmapPath(ImageType.Metadata, ImageColorType.Type_01_Dither), FileMode.Open);
			Assert.IsNotNull(fStream);

			format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			dib = FreeImage.LoadFromStream(fStream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
			Assert.AreNotEqual(0, dib);
			Assert.That(FreeImage.GetBPP(dib) == 24);
			Assert.That(format == FREE_IMAGE_FORMAT.FIF_JPEG);
			FreeImage.UnloadEx(ref dib);
			fStream.Close();

			fStream = new FileStream(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_32), FileMode.Open);
			format = FREE_IMAGE_FORMAT.FIF_TIFF;
			dib = FreeImage.LoadFromStream(fStream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
			Assert.AreNotEqual(0, dib);
			Assert.That(FreeImage.GetBPP(dib) == 32);
			Assert.That(format == FREE_IMAGE_FORMAT.FIF_TIFF);

			FreeImage.UnloadEx(ref dib);

			Assert.AreEqual(0, dib);
			dib = FreeImage.LoadFromStream(new MemoryStream(new byte[] { }));
			Assert.AreEqual(0, dib);

			format = FREE_IMAGE_FORMAT.FIF_BMP;
			fStream.Position = 0;
			dib = FreeImage.LoadFromStream(fStream, ref format);
			Assert.AreEqual(0, dib);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_BMP, format);

			fStream.Close();
		}

		[Test]
		public void FreeImage_SaveToStream()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_08_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);

			Stream stream = new FileStream(@"out_stream.bmp", FileMode.Create);
			Assert.IsNotNull(stream);

			Assert.IsTrue(FreeImage.SaveEx(ref dib, @"out_file.bmp", FREE_IMAGE_FORMAT.FIF_BMP, false));
			Assert.IsTrue(FreeImage.SaveToStream(dib, stream, FREE_IMAGE_FORMAT.FIF_BMP));
			stream.Flush();
			stream.Dispose();

			Assert.IsTrue(File.Exists(@"out_stream.bmp"));
			Assert.IsTrue(File.Exists(@"out_file.bmp"));
			byte[] buffer1 = File.ReadAllBytes(@"out_stream.bmp");
			byte[] buffer2 = File.ReadAllBytes(@"out_file.bmp");
			Assert.AreEqual(buffer1.Length, buffer2.Length);
			for (int i = 0; i < buffer1.Length; i++)
				if (buffer1[i] != buffer2[i])
					Assert.Fail();

			File.Delete(@"out_stream.bmp");
			File.Delete(@"out_file.bmp");
			Assert.IsFalse(File.Exists(@"out_stream.bmp"));
			Assert.IsFalse(File.Exists(@"out_file.bmp"));

			stream = new MemoryStream();
			Assert.IsFalse(FreeImage.SaveToStream(dib, stream, FREE_IMAGE_FORMAT.FIF_FAXG3));
			stream.Dispose();
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_IsExtensionValidForFIF()
		{
			Assert.IsTrue(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_BMP, "bmp", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_BMP, "BMP", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_BMP, "DUMMY", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_PCX, "pcx", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_TIFF, "tif", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_TIFF, "TIFF", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(FreeImage.IsExtensionValidForFIF(FREE_IMAGE_FORMAT.FIF_ICO, "ICO"));
		}

		[Test]
		public void FreeImage_IsFilenameValidForFIF()
		{
			Assert.IsTrue(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_JPEG, "file.jpg"));
			Assert.IsTrue(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_JPEG, "file.jpeg"));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_JPEG, "file.bmp"));
			Assert.IsTrue(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_GIF, "file.gif"));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_GIF, "file.GIF"));
			Assert.IsTrue(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_GIF, "file.GIF", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_GIF, "file.txt"));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_UNKNOWN, "file.jpg"));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_UNKNOWN, "file.bmp"));
			Assert.IsFalse(FreeImage.IsFilenameValidForFIF(FREE_IMAGE_FORMAT.FIF_UNKNOWN, "file.tif"));
		}

		[Test]
		public void FreeImage_GetPrimaryExtensionFromFIF()
		{
			Assert.AreEqual("gif", FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_GIF));
			Assert.AreEqual("tif", FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_TIFF));
			Assert.AreNotEqual("tiff", FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_TIFF));
			Assert.AreEqual("psd", FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_PSD));
			Assert.AreEqual("iff", FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_IFF));
			Assert.IsNull(FreeImage.GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT.FIF_UNKNOWN));
		}

		[Test]
		public void FreeImage_OpenMultiBitmapEx()
		{
			FIMULTIBITMAP dib = FreeImage.OpenMultiBitmapEx(iManager.GetBitmapPath(ImageType.Multipaged, ImageColorType.Type_01_Dither));
			Assert.IsFalse(dib.IsNull);
			Assert.AreEqual(4, FreeImage.GetPageCount(dib));
			FreeImage.CloseMultiBitmap(dib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);

			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			dib = FreeImage.OpenMultiBitmapEx(
				iManager.GetBitmapPath(ImageType.Multipaged, ImageColorType.Type_04), ref format, FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false, true, true);
			Assert.IsFalse(dib.IsNull);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_TIFF, format);
			FreeImage.CloseMultiBitmap(dib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		[Test]
		public void FreeImage_GetLockedPageCount()
		{
			FIMULTIBITMAP dib = FreeImage.OpenMultiBitmapEx(iManager.GetBitmapPath(ImageType.Multipaged, ImageColorType.Type_01_Dither));
			FIBITMAP page1, page2, page3;
			Assert.IsFalse(dib.IsNull);
			Assert.AreEqual(4, FreeImage.GetPageCount(dib));
			Assert.AreEqual(0, FreeImage.GetLockedPageCount(dib));

			page1 = FreeImage.LockPage(dib, 0);
			Assert.AreEqual(1, FreeImage.GetLockedPageCount(dib));

			page2 = FreeImage.LockPage(dib, 1);
			Assert.AreEqual(2, FreeImage.GetLockedPageCount(dib));

			page3 = FreeImage.LockPage(dib, 2);
			Assert.AreEqual(3, FreeImage.GetLockedPageCount(dib));

			FreeImage.UnlockPage(dib, page3, true);
			Assert.AreEqual(2, FreeImage.GetLockedPageCount(dib));

			FreeImage.UnlockPage(dib, page2, true);
			Assert.AreEqual(1, FreeImage.GetLockedPageCount(dib));

			FreeImage.UnlockPage(dib, page1, true);
			Assert.AreEqual(0, FreeImage.GetLockedPageCount(dib));

			FreeImage.CloseMultiBitmapEx(ref dib);
		}

		[Test]
		public void FreeImage_GetLockedPages()
		{
			FIMULTIBITMAP dib = FreeImage.OpenMultiBitmapEx(iManager.GetBitmapPath(ImageType.Multipaged, ImageColorType.Type_01_Dither));
			FIBITMAP page1, page2, page3;
			int[] lockedList;
			Assert.IsFalse(dib.IsNull);
			Assert.AreEqual(4, FreeImage.GetPageCount(dib));
			Assert.AreEqual(0, FreeImage.GetLockedPageCount(dib));

			page1 = FreeImage.LockPage(dib, 0);
			Assert.AreEqual(1, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.Contains(0, lockedList);

			page2 = FreeImage.LockPage(dib, 1);
			Assert.AreEqual(2, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.Contains(0, lockedList);
			Assert.Contains(1, lockedList);

			page3 = FreeImage.LockPage(dib, 3);
			Assert.AreEqual(3, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.Contains(0, lockedList);
			Assert.Contains(1, lockedList);
			Assert.Contains(3, lockedList);

			FreeImage.UnlockPage(dib, page2, true);
			Assert.AreEqual(2, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.Contains(0, lockedList);
			Assert.Contains(3, lockedList);

			FreeImage.UnlockPage(dib, page1, true);
			Assert.AreEqual(1, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.Contains(3, lockedList);

			FreeImage.UnlockPage(dib, page3, true);
			Assert.AreEqual(0, FreeImage.GetLockedPageCount(dib));
			lockedList = FreeImage.GetLockedPages(dib);
			Assert.AreEqual(0, lockedList.Length);

			FreeImage.CloseMultiBitmapEx(ref dib);
		}

		[Test]
		public void FreeImage_GetFileTypeFromStream()
		{
			FileStream fStream = new FileStream(iManager.GetBitmapPath(ImageType.JPEG, ImageColorType.Type_01_Dither), FileMode.Open);
			Assert.IsNotNull(fStream);

			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_JPEG, FreeImage.GetFileTypeFromStream(fStream));
			fStream.Dispose();

			fStream = new FileStream(iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_16_565), FileMode.Open);
			Assert.AreEqual(FREE_IMAGE_FORMAT.FIF_BMP, FreeImage.GetFileTypeFromStream(fStream));
			fStream.Close();
		}

		[Test]
		public void FreeImage_GetHbitmap()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			IntPtr hBitmap = FreeImage.GetHbitmap(dib, IntPtr.Zero, false);
			Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);
			Assert.IsNotNull(bitmap);
			Assert.AreEqual(FreeImage.GetWidth(dib), bitmap.Width);
			Assert.AreEqual(FreeImage.GetHeight(dib), bitmap.Height);

			bitmap.Dispose();
			FreeImage.FreeHbitmap(hBitmap);
			FreeImage.UnloadEx(ref dib);

			try
			{
				hBitmap = FreeImage.GetHbitmap(dib, IntPtr.Zero, false);
				Assert.Fail();
			}
			catch
			{
			}
		}

		[Test]
		public void FreeImage_GetResolutionX()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(72, FreeImage.GetResolutionX(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetResolutionY()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(72, FreeImage.GetResolutionY(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetResolutionX()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(72, FreeImage.GetResolutionX(dib));

			FreeImage.SetResolutionX(dib, 12u);
			Assert.AreEqual(12, FreeImage.GetResolutionX(dib));

			FreeImage.SetResolutionX(dib, 1u);
			Assert.AreEqual(1, FreeImage.GetResolutionX(dib));

			FreeImage.SetResolutionX(dib, 66u);
			Assert.AreEqual(66, FreeImage.GetResolutionX(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetResolutionY()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(72, FreeImage.GetResolutionY(dib));

			FreeImage.SetResolutionY(dib, 12u);
			Assert.AreEqual(12, FreeImage.GetResolutionY(dib));

			FreeImage.SetResolutionY(dib, 1u);
			Assert.AreEqual(1, FreeImage.GetResolutionY(dib));

			FreeImage.SetResolutionY(dib, 66u);
			Assert.AreEqual(66, FreeImage.GetResolutionY(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_IsGreyscaleImage()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.IsFalse(FreeImage.IsGreyscaleImage(dib));
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_Unordered);
			Assert.IsTrue(FreeImage.IsGreyscaleImage(dib));
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_MinIsBlack);
			Assert.IsTrue(FreeImage.IsGreyscaleImage(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetPaletteEx()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			RGBQUADARRAY palette = new RGBQUADARRAY();

			try
			{
				palette = FreeImage.GetPaletteEx(dib);
				Assert.Fail();
			}
			catch
			{
			}
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_08_Greyscale_MinIsBlack);
			try
			{
				palette = FreeImage.GetPaletteEx(dib);
			}
			catch
			{
				Assert.Fail();
			}
			Assert.AreEqual(256, palette.Length);
			for (int index = 0; index < 256; index++)
			{
				Assert.AreEqual(index, palette.GetRed(index));
				Assert.AreEqual(index, palette.GetGreen(index));
				Assert.AreEqual(index, palette.GetBlue(index));
			}

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetInfoHeaderEx()
		{
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04);
			Assert.IsFalse(dib.IsNull);

			BITMAPINFOHEADER iHeader = FreeImage.GetInfoHeaderEx(dib);
			Assert.AreEqual(4, iHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), iHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), iHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);

			iHeader = FreeImage.GetInfoHeaderEx(dib);
			Assert.AreEqual(1, iHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), iHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), iHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.IsFalse(dib.IsNull);

			iHeader = FreeImage.GetInfoHeaderEx(dib);
			Assert.AreEqual(24, iHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), iHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), iHeader.biHeight);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetInfoEx()
		{
			BITMAPINFO info;

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_01_Dither);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_04_Greyscale_MinIsBlack);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_08_Greyscale_Unordered);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_16_555);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_16_565);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_24);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.AreNotEqual(0, dib);
			info = FreeImage.GetInfoEx(dib);
			Assert.AreEqual(FreeImage.GetBPP(dib), info.bmiHeader.biBitCount);
			Assert.AreEqual(FreeImage.GetWidth(dib), info.bmiHeader.biWidth);
			Assert.AreEqual(FreeImage.GetHeight(dib), info.bmiHeader.biHeight);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetPixelFormat()
		{
			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_01_Threshold);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(PixelFormat.Format1bppIndexed, FreeImage.GetPixelFormat(dib));
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_04_Greyscale_Unordered);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(PixelFormat.Format4bppIndexed, FreeImage.GetPixelFormat(dib));
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_555);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(PixelFormat.Format16bppRgb555, FreeImage.GetPixelFormat(dib));
			FreeImage.UnloadEx(ref dib);

			dib = iManager.GetBitmap(ImageType.Odd, ImageColorType.Type_16_565);
			Assert.IsFalse(dib.IsNull);

			Assert.AreEqual(PixelFormat.Format16bppRgb565, FreeImage.GetPixelFormat(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetFormatParameters()
		{
			uint bpp, red, green, blue;

			Assert.IsTrue(FreeImage.GetFormatParameters(PixelFormat.Format16bppRgb555, out bpp, out red, out green, out blue));
			Assert.AreEqual(16, bpp);
			Assert.AreEqual(red, FreeImage.FI16_555_RED_MASK);
			Assert.AreEqual(green, FreeImage.FI16_555_GREEN_MASK);
			Assert.AreEqual(blue, FreeImage.FI16_555_BLUE_MASK);

			Assert.IsTrue(FreeImage.GetFormatParameters(PixelFormat.Format32bppArgb, out bpp, out red, out green, out blue));
			Assert.AreEqual(32, bpp);
			Assert.AreEqual(red, FreeImage.FI_RGBA_RED_MASK);
			Assert.AreEqual(green, FreeImage.FI_RGBA_GREEN_MASK);
			Assert.AreEqual(blue, FreeImage.FI_RGBA_BLUE_MASK);

			Assert.IsTrue(FreeImage.GetFormatParameters(PixelFormat.Format24bppRgb, out bpp, out red, out green, out blue));
			Assert.AreEqual(24, bpp);
			Assert.AreEqual(red, FreeImage.FI_RGBA_RED_MASK);
			Assert.AreEqual(green, FreeImage.FI_RGBA_GREEN_MASK);
			Assert.AreEqual(blue, FreeImage.FI_RGBA_BLUE_MASK);

			Assert.IsTrue(FreeImage.GetFormatParameters(PixelFormat.Format4bppIndexed, out bpp, out red, out green, out blue));
			Assert.AreEqual(4, bpp);
			Assert.AreEqual(red, 0);
			Assert.AreEqual(green, 0);
			Assert.AreEqual(blue, 0);
		}

		[Test]
		public void FreeImage_Compare()
		{
			FIBITMAP dib2;

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);
			dib2 = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_01_Threshold);
			Assert.IsFalse(dib2.IsNull);

			Assert.IsFalse(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));
			Assert.IsTrue(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.HEADER));

			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref dib2);

			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_MinIsBlack);
			dib2 = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_04_Greyscale_Unordered);

			Assert.IsFalse(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));

			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref dib2);
			dib = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			dib2 = iManager.GetBitmap(ImageType.Even, ImageColorType.Type_32);
			Assert.IsTrue(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));

			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref dib2);

			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);
			dib2 = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib2.IsNull);

			Assert.IsTrue(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));

			FreeImage.UnloadEx(ref dib);
			FreeImage.UnloadEx(ref dib2);
		}

		[Test]
		public void FreeImage_CreateICCProfileEx()
		{
			FIICCPROFILE prof;
			byte[] data = new byte[173];
			Random rand = new Random(DateTime.Now.Millisecond);
			dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP, 1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			prof = FreeImage.GetICCProfileEx(dib);
			Assert.That(prof.DataPointer == IntPtr.Zero);

			rand.NextBytes(data);
			prof = FreeImage.CreateICCProfileEx(dib, data);
			Assert.That(prof.Size == data.Length);
			for (int i = 0; i < data.Length; i++)
				if (prof.Data[i] != data[i])
					Assert.Fail();

			FreeImage.DestroyICCProfile(dib);
			prof = FreeImage.GetICCProfileEx(dib);
			Assert.That(prof.DataPointer == IntPtr.Zero);

			FreeImage.CreateICCProfileEx(dib, new byte[0], 0);
			prof = FreeImage.GetICCProfileEx(dib);
			Assert.That(prof.DataPointer == IntPtr.Zero);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_ConvertColorDepth()
		{
			int bpp = 1;
			FIBITMAP dib2;

			dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP, 80, 80, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			do
			{
				dib2 = FreeImage.ConvertColorDepth(dib, (FREE_IMAGE_COLOR_DEPTH)bpp);
				Assert.IsFalse(dib2.IsNull);
				Assert.AreEqual(bpp, FreeImage.GetBPP(dib2));
				if (dib != dib2)
					FreeImage.UnloadEx(ref dib2);
			} while (0 != (bpp = FreeImage.GetNextColorDepth(bpp)));

			FreeImage.UnloadEx(ref dib);

			dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP, 80, 80, 32,
				FreeImage.FI_RGBA_RED_MASK, FreeImage.FI_RGBA_GREEN_MASK, FreeImage.FI_RGBA_BLUE_MASK);
			Assert.IsFalse(dib.IsNull);
			bpp = 32;

			do
			{
				dib2 = FreeImage.ConvertColorDepth(dib, (FREE_IMAGE_COLOR_DEPTH)bpp);
				Assert.IsFalse(dib2.IsNull);
				Assert.AreEqual(bpp, FreeImage.GetBPP(dib2));
				if (dib != dib2)
					FreeImage.UnloadEx(ref dib2);
			} while (0 != (bpp = FreeImage.GetPrevousColorDepth(bpp)));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetNextColorDepth()
		{
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(5));
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(0));
			Assert.AreEqual(4, FreeImage.GetNextColorDepth(1));
			Assert.AreEqual(8, FreeImage.GetNextColorDepth(4));
			Assert.AreEqual(16, FreeImage.GetNextColorDepth(8));
			Assert.AreEqual(24, FreeImage.GetNextColorDepth(16));
			Assert.AreEqual(32, FreeImage.GetNextColorDepth(24));
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(32));
		}

		[Test]
		public void FreeImage_GetPrevousColorDepth()
		{
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(5));
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(0));
			Assert.AreEqual(4, FreeImage.GetNextColorDepth(1));
			Assert.AreEqual(8, FreeImage.GetNextColorDepth(4));
			Assert.AreEqual(16, FreeImage.GetNextColorDepth(8));
			Assert.AreEqual(24, FreeImage.GetNextColorDepth(16));
			Assert.AreEqual(32, FreeImage.GetNextColorDepth(24));
			Assert.AreEqual(0, FreeImage.GetNextColorDepth(32));
		}

		[Test]
		public unsafe void FreeImage_PtrToStr()
		{
			string testString;
			string resString;
			IntPtr buffer;
			int index;

			testString = "Test string";
			buffer = Marshal.AllocHGlobal(testString.Length + 1);

			for (index = 0; index < testString.Length; index++)
			{
				Marshal.WriteByte(buffer, index, (byte)testString[index]);
			}
			Marshal.WriteByte(buffer, index, (byte)0);

			resString = FreeImage.PtrToStr((byte*)buffer);
			Assert.That(resString == testString);

			Marshal.FreeHGlobal(buffer);

			testString = @"äöü?=§%/!)§(%&)(§";
			buffer = Marshal.AllocHGlobal(testString.Length + 1);

			for (index = 0; index < testString.Length; index++)
			{
				Marshal.WriteByte(buffer, index, (byte)testString[index]);
			}
			Marshal.WriteByte(buffer, index, (byte)0);

			resString = FreeImage.PtrToStr((byte*)buffer);
			Assert.That(resString == testString);

			Marshal.FreeHGlobal(buffer);
		}

		[Test]
		public void FreeImage_CopyMetadata()
		{
			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);
			int mdCount = 0;

			FIBITMAP dib2 = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib2.IsNull);

			FREE_IMAGE_MDMODEL[] modelList = (FREE_IMAGE_MDMODEL[])Enum.GetValues(typeof(FREE_IMAGE_MDMODEL));
			FITAG tag, tag2;
			FIMETADATA mdHandle;

			foreach (FREE_IMAGE_MDMODEL model in modelList)
			{
				mdHandle = FreeImage.FindFirstMetadata(model, dib2, out tag);
				Assert.IsTrue(mdHandle.IsNull);
				mdCount += (int)FreeImage.GetMetadataCount(model, dib);
			}

			Assert.AreEqual(mdCount, FreeImage.CopyMetadata(dib, dib2, FREE_IMAGE_METADATA_COPY.CLEAR_EXISTING));

			foreach (FREE_IMAGE_MDMODEL model in modelList)
			{
				mdHandle = FreeImage.FindFirstMetadata(model, dib, out tag);
				if (!mdHandle.IsNull)
				{
					do
					{
						Assert.IsTrue(FreeImage.GetMetadata(model, dib2, FreeImage.GetTagKey(tag), out tag2));
						Assert.That(FreeImage.GetTagCount(tag) == FreeImage.GetTagCount(tag2));
						Assert.That(FreeImage.GetTagDescription(tag) == FreeImage.GetTagDescription(tag2));
						Assert.That(FreeImage.GetTagID(tag) == FreeImage.GetTagID(tag2));
						Assert.That(FreeImage.GetTagKey(tag) == FreeImage.GetTagKey(tag2));
						Assert.That(FreeImage.GetTagLength(tag) == FreeImage.GetTagLength(tag2));
						Assert.That(FreeImage.GetTagType(tag) == FreeImage.GetTagType(tag2));
					}
					while (FreeImage.FindNextMetadata(mdHandle, out tag));
					FreeImage.FindCloseMetadata(mdHandle);
				}
			}

			Assert.AreEqual(0, FreeImage.CopyMetadata(dib, dib2, FREE_IMAGE_METADATA_COPY.KEEP_EXISITNG));

			foreach (FREE_IMAGE_MDMODEL model in modelList)
			{
				mdHandle = FreeImage.FindFirstMetadata(model, dib, out tag);
				if (!mdHandle.IsNull)
				{
					do
					{
						Assert.IsTrue(FreeImage.GetMetadata(model, dib2, FreeImage.GetTagKey(tag), out tag2));
						Assert.AreEqual(FreeImage.GetTagCount(tag), FreeImage.GetTagCount(tag2));
						Assert.AreEqual(FreeImage.GetTagDescription(tag), FreeImage.GetTagDescription(tag2));
						Assert.AreEqual(FreeImage.GetTagID(tag), FreeImage.GetTagID(tag2));
						Assert.AreEqual(FreeImage.GetTagKey(tag), FreeImage.GetTagKey(tag2));
						Assert.AreEqual(FreeImage.GetTagLength(tag), FreeImage.GetTagLength(tag2));
						Assert.AreEqual(FreeImage.GetTagType(tag), FreeImage.GetTagType(tag2));
					}
					while (FreeImage.FindNextMetadata(mdHandle, out tag));
					FreeImage.FindCloseMetadata(mdHandle);
				}
			}

			const string newTagDescription = "NEW_TAG_DESCRIPTION";

			Assert.IsTrue(FreeImage.GetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib, "Copyright", out tag));
			Assert.IsTrue(FreeImage.SetTagDescription(tag, newTagDescription));
			Assert.AreEqual(mdCount, FreeImage.CopyMetadata(dib, dib2, FREE_IMAGE_METADATA_COPY.REPLACE_EXISTING));
			Assert.IsTrue(FreeImage.GetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib2, "Copyright", out tag2));
			Assert.AreEqual(newTagDescription, FreeImage.GetTagDescription(tag2));

			FreeImage.UnloadEx(ref dib2);
			FreeImage.UnloadEx(ref dib);

			dib2 = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);

			Assert.AreEqual(0, FreeImage.CopyMetadata(dib, dib2, FREE_IMAGE_METADATA_COPY.CLEAR_EXISTING));

			FreeImage.UnloadEx(ref dib2);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetImageComment()
		{
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);
			const string comment = "C O M M E N T";

			Assert.IsNull(FreeImage.GetImageComment(dib));
			Assert.IsTrue(FreeImage.SetImageComment(dib, comment));
			Assert.AreEqual(comment, FreeImage.GetImageComment(dib));
			Assert.IsTrue(FreeImage.SetImageComment(dib, null));
			Assert.IsNull(FreeImage.GetImageComment(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetImageComment()
		{
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);
			const string comment = "C O M M E N T";

			Assert.IsNull(FreeImage.GetImageComment(dib));
			Assert.IsTrue(FreeImage.SetImageComment(dib, comment));
			Assert.AreEqual(comment, FreeImage.GetImageComment(dib));
			Assert.IsTrue(FreeImage.SetImageComment(dib, null));
			Assert.IsNull(FreeImage.GetImageComment(dib));
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetMetadata()
		{
			MetadataTag tag;

			dib = iManager.GetBitmap(ImageType.Metadata, ImageColorType.Type_01_Dither);
			Assert.IsFalse(dib.IsNull);

			Assert.IsFalse(FreeImage.GetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib, "~~~~~", out tag));
			Assert.IsNull(tag);

			Assert.IsTrue(FreeImage.GetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN, dib, "Artist", out tag));
			Assert.IsFalse(tag.tag.IsNull);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetMetadata()
		{
			MetadataTag tag;
			Random rand = new Random();

			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			ushort value = (ushort)rand.Next();

			tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_CUSTOM);
			tag.ID = value;

			Assert.IsTrue(FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_CUSTOM, dib, "~~~~~", tag));
			Assert.IsTrue(FreeImage.GetMetadata(FREE_IMAGE_MDMODEL.FIMD_CUSTOM, dib, "~~~~~", out tag));
			Assert.AreEqual(value, tag.ID);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_FindFirstMetadata()
		{
			MetadataTag tag;
			FIMETADATA mdHandle;
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			FREE_IMAGE_MDMODEL[] models = (FREE_IMAGE_MDMODEL[])Enum.GetValues(typeof(FREE_IMAGE_MDMODEL));
			foreach (FREE_IMAGE_MDMODEL model in models)
			{
				mdHandle = FreeImage.FindFirstMetadata(model, dib, out tag);
				Assert.IsTrue(mdHandle.IsNull);
			}

			tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
			tag.Key = "KEY";
			tag.Value = 12345;
			tag.AddToImage(dib);

			mdHandle = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, out tag);
			Assert.IsFalse(mdHandle.IsNull);

			FreeImage.FindCloseMetadata(mdHandle);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_FindNextMetadata()
		{
			MetadataTag tag;
			FIMETADATA mdHandle;
			dib = FreeImage.Allocate(1, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			FREE_IMAGE_MDMODEL[] models = (FREE_IMAGE_MDMODEL[])Enum.GetValues(typeof(FREE_IMAGE_MDMODEL));
			foreach (FREE_IMAGE_MDMODEL model in models)
			{
				mdHandle = FreeImage.FindFirstMetadata(model, dib, out tag);
				Assert.IsTrue(mdHandle.IsNull);
			}

			tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
			tag.Key = "KEY1";
			tag.Value = 12345;
			tag.AddToImage(dib);

			tag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
			tag.Key = "KEY2";
			tag.Value = 54321;
			tag.AddToImage(dib);

			mdHandle = FreeImage.FindFirstMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, out tag);
			Assert.IsFalse(mdHandle.IsNull);

			Assert.IsTrue(FreeImage.FindNextMetadata(mdHandle, out tag));
			Assert.IsFalse(FreeImage.FindNextMetadata(mdHandle, out tag));

			FreeImage.FindCloseMetadata(mdHandle);
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_SetGetTransparencyTableEx()
		{
			dib = FreeImage.Allocate(10, 10, 6, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			byte[] transTable = FreeImage.GetTransparencyTableEx(dib);
			Assert.That(transTable.Length == 0);

			Random rand = new Random();
			transTable = new byte[rand.Next(0, 255)];
			int length = transTable.Length;

			for (int i = 0; i < transTable.Length; i++)
				transTable[i] = (byte)i;

			FreeImage.SetTransparencyTable(dib, transTable);
			transTable = null;
			transTable = FreeImage.GetTransparencyTableEx(dib);
			Assert.That(transTable.Length == length);
			for (int i = 0; i < transTable.Length; i++)
				Assert.That(transTable[i] == i);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_GetUniqueColors()
		{
			RGBQUADARRAY palette;

			//
			// 1bpp
			//

			dib = FreeImage.Allocate(10, 1, 1, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			palette = new RGBQUADARRAY(dib);
			palette.SetRGBQUAD(0, new RGBQUAD(Color.FromArgb(43, 255, 255, 255)));
			palette.SetRGBQUAD(1, new RGBQUAD(Color.FromArgb(222, 0, 0, 0)));

			FI1BITARRAY sl1bit = new FI1BITARRAY(dib, 0);
			for (int x = 0; x < sl1bit.Length; x++)
			{
				sl1bit.SetIndex(x, 0);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			sl1bit.SetIndex(5, 1);
			Assert.AreEqual(2, FreeImage.GetUniqueColors(dib));

			palette.SetRGBQUAD(1, new RGBQUAD(Color.FromArgb(222, 255, 255, 255)));
			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			sl1bit.SetIndex(5, 0);
			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);

			//
			// 4bpp
			//

			dib = FreeImage.Allocate(10, 1, 4, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			palette = new RGBQUADARRAY(dib);
			palette.SetRGBQUAD(0, new RGBQUAD(Color.FromArgb(43, 255, 255, 255)));
			palette.SetRGBQUAD(1, new RGBQUAD(Color.FromArgb(222, 51, 2, 211)));
			palette.SetRGBQUAD(2, new RGBQUAD(Color.FromArgb(29, 25, 31, 52)));
			palette.SetRGBQUAD(3, new RGBQUAD(Color.FromArgb(173, 142, 61, 178)));
			palette.SetRGBQUAD(4, new RGBQUAD(Color.FromArgb(143, 41, 67, 199)));
			palette.SetRGBQUAD(5, new RGBQUAD(Color.FromArgb(2, 0, 2, 221)));

			FI4BITARRAY sl4bit = new FI4BITARRAY(dib, 0);

			for (int x = 0; x < sl4bit.Length; x++)
			{
				sl4bit.SetIndex(x, 0);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(1, 1);
			Assert.AreEqual(2, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(2, 1);
			Assert.AreEqual(2, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(3, 2);
			Assert.AreEqual(3, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(4, 3);
			Assert.AreEqual(4, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(5, 4);
			Assert.AreEqual(5, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(6, 5);
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(7, 6);
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(8, 7);
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			sl4bit.SetIndex(9, 7);
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);

			//
			// 8bpp
			//

			dib = FreeImage.Allocate(10, 1, 8, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			palette = new RGBQUADARRAY(dib);
			palette.SetRGBQUAD(0, new RGBQUAD(Color.FromArgb(43, 255, 255, 255)));
			palette.SetRGBQUAD(1, new RGBQUAD(Color.FromArgb(222, 51, 2, 211)));
			palette.SetRGBQUAD(2, new RGBQUAD(Color.FromArgb(29, 25, 31, 52)));
			palette.SetRGBQUAD(3, new RGBQUAD(Color.FromArgb(173, 142, 61, 178)));
			palette.SetRGBQUAD(4, new RGBQUAD(Color.FromArgb(143, 41, 67, 199)));
			palette.SetRGBQUAD(5, new RGBQUAD(Color.FromArgb(2, 0, 2, 221)));

			FI8BITARRAY sl8bit = new FI8BITARRAY(dib, 0);

			for (int x = 0; x < sl8bit.Length; x++)
			{
				sl8bit.SetIndex(x, 0);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(1, 1);
			Assert.AreEqual(2, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(2, 2);
			Assert.AreEqual(3, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(3, 3);
			Assert.AreEqual(4, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(4, 4);
			Assert.AreEqual(5, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(5, 6);
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(5, 7);
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			sl8bit.SetIndex(5, 8);
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);

			//
			// 16bpp
			//

			dib = FreeImage.Allocate(10, 1, 16, FreeImage.FI16_555_RED_MASK, FreeImage.FI16_565_GREEN_MASK, FreeImage.FI16_565_BLUE_MASK);
			Assert.IsFalse(dib.IsNull);

			FI16RGBARRAY sl16bit = new FI16RGBARRAY(dib, 0);

			for (int x = 0; x < sl16bit.Length; x++)
			{
				sl16bit.SetUShort(x, 0);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			for (int x = 0; x < sl16bit.Length; x++)
			{
				sl16bit.SetUShort(x, (ushort)x);
				Assert.AreEqual(1 + x, FreeImage.GetUniqueColors(dib));
			}

			sl16bit.SetUShort(3, (ushort)5);
			Assert.AreEqual(9, FreeImage.GetUniqueColors(dib));

			sl16bit.SetUShort(7, (ushort)5);
			Assert.AreEqual(8, FreeImage.GetUniqueColors(dib));

			sl16bit.SetUShort(1, (ushort)5);
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);

			//
			// 24bpp
			//

			dib = FreeImage.Allocate(10, 1, 24, FreeImage.FI_RGBA_RED_MASK, FreeImage.FI_RGBA_GREEN_MASK, FreeImage.FI_RGBA_BLUE_MASK);
			Assert.IsFalse(dib.IsNull);

			RGBTRIPLEARRAY sl24bit = new RGBTRIPLEARRAY(dib, 0);

			for (int x = 0; x < sl24bit.Length; x++)
			{
				sl24bit.SetUIntColor(x, 0x00000000);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			for (int x = 0; x < sl24bit.Length; x++)
			{
				sl24bit.SetUIntColor(x, (uint)x);
				Assert.AreEqual(1 + x, FreeImage.GetUniqueColors(dib));
			}

			sl24bit.SetUIntColor(3, (uint)2);
			Assert.AreEqual(9, FreeImage.GetUniqueColors(dib));

			sl24bit.SetUIntColor(1, (uint)5);
			Assert.AreEqual(8, FreeImage.GetUniqueColors(dib));

			sl24bit.SetUIntColor(7, (uint)9);
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			sl24bit.SetUIntColor(4, (uint)8);
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);

			//
			// 32bpp
			//

			dib = FreeImage.Allocate(10, 1, 32, FreeImage.FI_RGBA_RED_MASK, FreeImage.FI_RGBA_GREEN_MASK, FreeImage.FI_RGBA_BLUE_MASK);
			Assert.IsFalse(dib.IsNull);

			RGBQUADARRAY sl32bit = new RGBQUADARRAY(dib, 0);

			for (int x = 0; x < sl32bit.Length; x++)
			{
				sl32bit.SetUIntColor(x, 0x00000000);
			}

			Assert.AreEqual(1, FreeImage.GetUniqueColors(dib));

			for (int x = 0; x < sl32bit.Length; x++)
			{
				sl32bit.SetUIntColor(x, (uint)x);
				Assert.AreEqual(1 + x, FreeImage.GetUniqueColors(dib));
			}

			sl32bit.SetUIntColor(3, (uint)(2 | 0xDE000000));
			Assert.AreEqual(9, FreeImage.GetUniqueColors(dib));

			sl32bit.SetUIntColor(1, (uint)(5 | 0x31000000));
			Assert.AreEqual(8, FreeImage.GetUniqueColors(dib));

			sl32bit.SetUIntColor(7, (uint)(9 | 0x11000000));
			Assert.AreEqual(7, FreeImage.GetUniqueColors(dib));

			sl32bit.SetUIntColor(4, (uint)(8 | 0x9A000000));
			Assert.AreEqual(6, FreeImage.GetUniqueColors(dib));

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_CreateShrunkenPaletteLUT()
		{
			Random rand = new Random();
			dib = FreeImage.Allocate(1, 1, 8, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			RGBQUADARRAY palette = new RGBQUADARRAY(dib);
			byte[] lut;
			int colors;

			for (int x = 0; x < palette.Length; x++)
			{
				palette.SetUIntColor(x, 0xFF000000);
			}

			lut = FreeImage.CreateShrunkenPaletteLUT(dib, out colors);
			Assert.AreEqual(1, colors);

			for (int x = 0; x < palette.Length; x++)
			{
				Assert.AreEqual(0, lut[x]);
			}

			palette.SetUIntColor(1, 0x00000001);
			lut = FreeImage.CreateShrunkenPaletteLUT(dib, out colors);
			Assert.AreEqual(2, colors);

			Assert.AreEqual(0, lut[0]);
			Assert.AreEqual(1, lut[1]);

			for (int x = 2; x < palette.Length; x++)
			{
				Assert.AreEqual(0, lut[x]);
			}

			for (int x = 0; x < palette.Length; x++)
			{
				palette.SetUIntColor(x, (uint)x);
			}

			lut = FreeImage.CreateShrunkenPaletteLUT(dib, out colors);
			Assert.AreEqual(256, colors);

			for (int x = 0; x < palette.Length; x++)
			{
				Assert.AreEqual(x, lut[x]);
			}

			uint[] testColors = new uint[] { 0xFF4F387C, 0xFF749178, 0xFF84D51A, 0xFF746B71, 0x74718163, 0x91648106 };
			palette.SetUIntColor(0, testColors[0]);
			palette.SetUIntColor(1, testColors[1]);
			palette.SetUIntColor(2, testColors[2]);
			palette.SetUIntColor(3, testColors[3]);
			palette.SetUIntColor(4, testColors[4]);
			palette.SetUIntColor(5, testColors[5]);

			for (int x = testColors.Length; x < palette.Length; x++)
			{
				palette.SetUIntColor(x, testColors[rand.Next(0, testColors.Length - 1)]);
			}

			lut = FreeImage.CreateShrunkenPaletteLUT(dib, out colors);
			Assert.AreEqual(testColors.Length, colors);

			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public void FreeImage_Rotate4bit()
		{
			RGBQUADARRAY orgPal, rotPal;
			FIBITMAP rotated;
			byte index;
			dib = FreeImage.Allocate(2, 3, 4, 0, 0, 0);
			Assert.IsFalse(dib.IsNull);

			index = 1; if (!FreeImage.SetPixelIndex(dib, 0, 0, ref index)) throw new Exception();
			index = 2; if (!FreeImage.SetPixelIndex(dib, 1, 0, ref index)) throw new Exception();
			index = 3; if (!FreeImage.SetPixelIndex(dib, 0, 1, ref index)) throw new Exception();
			index = 4; if (!FreeImage.SetPixelIndex(dib, 1, 1, ref index)) throw new Exception();
			index = 5; if (!FreeImage.SetPixelIndex(dib, 0, 2, ref index)) throw new Exception();
			index = 6; if (!FreeImage.SetPixelIndex(dib, 1, 2, ref index)) throw new Exception();

			//
			// 90 deg
			//

			rotated = FreeImage.Rotate4bit(dib, 90d);
			Assert.IsFalse(rotated.IsNull);
			Assert.AreEqual(3, FreeImage.GetWidth(rotated));
			Assert.AreEqual(2, FreeImage.GetHeight(rotated));
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_BITMAP, FreeImage.GetImageType(rotated));
			Assert.AreEqual(4, FreeImage.GetBPP(rotated));
			orgPal = new RGBQUADARRAY(dib);
			rotPal = new RGBQUADARRAY(rotated);
			Assert.IsNotNull(orgPal);
			Assert.IsNotNull(rotPal);
			Assert.AreEqual(orgPal.Length, rotPal.Length);
			for (int i = 0; i < orgPal.Length; i++)
			{
				Assert.AreEqual(orgPal[i], rotPal[i]);
			}

			FreeImage.GetPixelIndex(rotated, 0, 0, out index);
			Assert.AreEqual(5, index);
			FreeImage.GetPixelIndex(rotated, 1, 0, out index);
			Assert.AreEqual(3, index);
			FreeImage.GetPixelIndex(rotated, 2, 0, out index);
			Assert.AreEqual(1, index);
			FreeImage.GetPixelIndex(rotated, 0, 1, out index);
			Assert.AreEqual(6, index);
			FreeImage.GetPixelIndex(rotated, 1, 1, out index);
			Assert.AreEqual(4, index);
			FreeImage.GetPixelIndex(rotated, 2, 1, out index);
			Assert.AreEqual(2, index);
			FreeImage.UnloadEx(ref rotated);

			//
			// 180 deg
			//

			rotated = FreeImage.Rotate4bit(dib, 180d);
			Assert.IsFalse(rotated.IsNull);
			Assert.AreEqual(FreeImage.GetWidth(dib), FreeImage.GetWidth(rotated));
			Assert.AreEqual(FreeImage.GetHeight(dib), FreeImage.GetHeight(rotated));
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_BITMAP, FreeImage.GetImageType(rotated));
			Assert.AreEqual(4, FreeImage.GetBPP(rotated));
			orgPal = new RGBQUADARRAY(dib);
			rotPal = new RGBQUADARRAY(rotated);
			Assert.IsNotNull(orgPal);
			Assert.IsNotNull(rotPal);
			Assert.AreEqual(orgPal.Length, rotPal.Length);
			for (int i = 0; i < orgPal.Length; i++)
			{
				Assert.AreEqual(orgPal[i], rotPal[i]);
			}

			FreeImage.GetPixelIndex(rotated, 0, 0, out index);
			Assert.AreEqual(6, index);
			FreeImage.GetPixelIndex(rotated, 1, 0, out index);
			Assert.AreEqual(5, index);
			FreeImage.GetPixelIndex(rotated, 0, 1, out index);
			Assert.AreEqual(4, index);
			FreeImage.GetPixelIndex(rotated, 1, 1, out index);
			Assert.AreEqual(3, index);
			FreeImage.GetPixelIndex(rotated, 0, 2, out index);
			Assert.AreEqual(2, index);
			FreeImage.GetPixelIndex(rotated, 1, 2, out index);
			Assert.AreEqual(1, index);
			FreeImage.UnloadEx(ref rotated);

			//
			// 270 deg
			//

			rotated = FreeImage.Rotate4bit(dib, 270d);
			Assert.IsFalse(rotated.IsNull);
			Assert.AreEqual(3, FreeImage.GetWidth(rotated));
			Assert.AreEqual(2, FreeImage.GetHeight(rotated));
			Assert.AreEqual(FREE_IMAGE_TYPE.FIT_BITMAP, FreeImage.GetImageType(rotated));
			Assert.AreEqual(4, FreeImage.GetBPP(rotated));
			orgPal = new RGBQUADARRAY(dib);
			rotPal = new RGBQUADARRAY(rotated);
			Assert.IsNotNull(orgPal);
			Assert.IsNotNull(rotPal);
			Assert.AreEqual(orgPal.Length, rotPal.Length);
			for (int i = 0; i < orgPal.Length; i++)
			{
				Assert.AreEqual(orgPal[i], rotPal[i]);
			}

			FreeImage.GetPixelIndex(rotated, 0, 0, out index);
			Assert.AreEqual(2, index);
			FreeImage.GetPixelIndex(rotated, 1, 0, out index);
			Assert.AreEqual(4, index);
			FreeImage.GetPixelIndex(rotated, 2, 0, out index);
			Assert.AreEqual(6, index);
			FreeImage.GetPixelIndex(rotated, 0, 1, out index);
			Assert.AreEqual(1, index);
			FreeImage.GetPixelIndex(rotated, 1, 1, out index);
			Assert.AreEqual(3, index);
			FreeImage.GetPixelIndex(rotated, 2, 1, out index);
			Assert.AreEqual(5, index);
			FreeImage.UnloadEx(ref rotated);

			FreeImage.UnloadEx(ref dib);
		}
	}

	[TestFixture]
	public class FreeImageBitmapTest
	{
		ImageManager iManager = new ImageManager();
		FIBITMAP dib = 0;
		string freeImageCallback = null;

		[TestFixtureSetUp]
		public void Init()
		{
			FreeImage.Message += new OutputMessageFunction(FreeImage_Message);
		}

		[TestFixtureTearDown]
		public void DeInit()
		{
			FreeImage.Message -= new OutputMessageFunction(FreeImage_Message);
		}

		[SetUp]
		public void InitEachTime()
		{
		}

		[TearDown]
		public void DeInitEachTime()
		{
		}

		void FreeImage_Message(FREE_IMAGE_FORMAT fif, string message)
		{
			freeImageCallback = message;
		}

		[Test]
		public void FreeImageBitmapConstructors()
		{
			Image bitmap;
			FreeImageBitmap fib, fib2;
			Stream stream;
			Graphics g;
			string filename = iManager.GetBitmapPath(ImageType.Odd, ImageColorType.Type_24);
			Assert.IsNotNull(filename);
			Assert.IsTrue(File.Exists(filename));

			bitmap = new Bitmap(filename);
			Assert.IsNotNull(bitmap);

			fib = new FreeImageBitmap(bitmap);
			Assert.AreEqual(bitmap.Width, fib.Width);
			Assert.AreEqual(bitmap.Height, fib.Height);
			fib.Dispose();
			fib.Dispose();

			fib = new FreeImageBitmap(bitmap, new Size(100, 100));
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();
			bitmap.Dispose();

			fib = new FreeImageBitmap(filename);
			fib.Dispose();

			fib = new FreeImageBitmap(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			fib.Dispose();

			fib = new FreeImageBitmap(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN);
			fib.Dispose();

			fib = new FreeImageBitmap(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			fib.Dispose();

			stream = new FileStream(filename, FileMode.Open);
			Assert.IsNotNull(stream);

			fib = new FreeImageBitmap(stream);
			fib.Dispose();
			stream.Seek(0, SeekOrigin.Begin);

			fib = new FreeImageBitmap(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN);
			fib.Dispose();
			stream.Seek(0, SeekOrigin.Begin);

			fib = new FreeImageBitmap(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			fib.Dispose();
			stream.Seek(0, SeekOrigin.Begin);

			fib = new FreeImageBitmap(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			fib.Dispose();
			stream.Dispose();

			fib = new FreeImageBitmap(100, 100);
			Assert.AreEqual(24, fib.ColorDepth);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			using (bitmap = new Bitmap(filename))
			{
				Assert.IsNotNull(bitmap);
				using (g = Graphics.FromImage(bitmap))
				{
					Assert.IsNotNull(g);
					fib = new FreeImageBitmap(100, 100, g);
				}
			}
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format1bppIndexed);
			Assert.AreEqual(PixelFormat.Format1bppIndexed, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format4bppIndexed);
			Assert.AreEqual(PixelFormat.Format4bppIndexed, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format8bppIndexed);
			Assert.AreEqual(PixelFormat.Format8bppIndexed, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format16bppRgb555);
			Assert.AreEqual(PixelFormat.Format16bppRgb555, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format16bppRgb565);
			Assert.AreEqual(PixelFormat.Format16bppRgb565, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format24bppRgb);
			Assert.AreEqual(PixelFormat.Format24bppRgb, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format32bppArgb);
			Assert.AreEqual(PixelFormat.Format32bppArgb, fib.PixelFormat);
			Assert.AreEqual(100, fib.Width);
			Assert.AreEqual(100, fib.Height);

			stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.Serialize(stream, fib);
			Assert.Greater(stream.Length, 0);
			stream.Position = 0;

			fib2 = formatter.Deserialize(stream) as FreeImageBitmap;
			stream.Dispose();
			fib.Dispose();
			fib2.Dispose();

			fib = new FreeImageBitmap(filename);
			fib2 = new FreeImageBitmap(fib);
			fib2.Dispose();

			fib2 = new FreeImageBitmap(fib, new Size(31, 22));
			Assert.AreEqual(31, fib2.Width);
			Assert.AreEqual(22, fib2.Height);
			fib2.Dispose();
			fib.Dispose();

			dib = FreeImage.Allocate(1000, 800, 24, 0xFF0000, 0xFF00, 0xFF);
			Assert.IsFalse(dib.IsNull);

			fib = new FreeImageBitmap(1000, 800, -(int)FreeImage.GetPitch(dib), FreeImage.GetPixelFormat(dib), FreeImage.GetScanLine(dib, 0));
			fib.Dispose();
			FreeImage.UnloadEx(ref dib);
		}

		[Test]
		public unsafe void Properties()
		{
			string filename = iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_32);
			Assert.IsNotNull(filename);
			Assert.IsTrue(File.Exists(filename));

			FreeImageBitmap fib = new FreeImageBitmap(filename);
			Assert.IsFalse(fib.HasPalette);

			try
			{
				RGBQUADARRAY palette = fib.Palette;
				Assert.Fail();
			}
			catch
			{
			}

			Assert.IsFalse(fib.HasBackgroundColor);
			fib.BackgroundColor = Color.LightSeaGreen;
			Assert.IsTrue(fib.HasBackgroundColor);
			Assert.That(
				Color.LightSeaGreen.B == fib.BackgroundColor.Value.B &&
				Color.LightSeaGreen.G == fib.BackgroundColor.Value.G &&
				Color.LightSeaGreen.R == fib.BackgroundColor.Value.R);
			fib.BackgroundColor = null;
			Assert.IsFalse(fib.HasBackgroundColor);
			fib.Dispose();

			fib = new FreeImageBitmap(100, 100, PixelFormat.Format1bppIndexed);
			ImageFlags flags = (ImageFlags)fib.Flags;
			Assert.That((flags & ImageFlags.ColorSpaceRgb) == ImageFlags.ColorSpaceRgb);
			Assert.That((flags & ImageFlags.HasAlpha) != ImageFlags.HasAlpha);
			Assert.That((flags & ImageFlags.HasRealDpi) != ImageFlags.HasRealDpi);
			Assert.That((flags & ImageFlags.HasTranslucent) != ImageFlags.HasTranslucent);
			fib.Dispose();

			dib = FreeImage.Allocate(100, 100, 32, 0xFF0000, 0xFF00, 0xFF);
			FIICCPROFILE* prof = (FIICCPROFILE*)FreeImage.CreateICCProfile(dib, new byte[] { 0, 1, 2, 3 }, 4);
			(*prof).Flags = ICC_FLAGS.FIICC_COLOR_IS_CMYK;
			fib = new FreeImageBitmap(dib);
			RGBQUADARRAY sc = (RGBQUADARRAY)fib.GetScanline(0);
			sc.SetAlpha(0, 127);
			flags = (ImageFlags)fib.Flags;
			Assert.That((flags & ImageFlags.ColorSpaceCmyk) == ImageFlags.ColorSpaceCmyk);
			Assert.That((flags & ImageFlags.HasAlpha) == ImageFlags.HasAlpha);
			Assert.That((flags & ImageFlags.HasRealDpi) != ImageFlags.HasRealDpi);
			Assert.That((flags & ImageFlags.HasTranslucent) == ImageFlags.HasTranslucent);
			fib.Dispose();
			fib = null;
			GC.Collect(2, GCCollectionMode.Forced);
			GC.WaitForPendingFinalizers();

			fib = new FreeImageBitmap(filename);
			flags = (ImageFlags)fib.Flags;
			Assert.That((flags & ImageFlags.HasRealDpi) == ImageFlags.HasRealDpi);
			fib.Dispose();

			fib = new FreeImageBitmap(iManager.GetBitmapPath(ImageType.Metadata, ImageColorType.Type_01_Dither));
			int[] propList = fib.PropertyIdList;
			Assert.IsNotNull(propList);
			Assert.Greater(propList.Length, 0);
			PropertyItem[] propItemList = fib.PropertyItems;
			Assert.IsNotNull(propItemList);
			Assert.Greater(propItemList.Length, 0);
			Assert.IsNotNull(fib.RawFormat);
			fib.Dispose();

			fib = new FreeImageBitmap(iManager.GetBitmapPath(ImageType.Multipaged, ImageColorType.Type_01_Dither));
			Assert.Greater(fib.FrameCount, 1);
			fib.Dispose();
		}

		[Test]
		public void GetBounds()
		{
			Random rand = new Random();
			int height = rand.Next(0, 100), width = rand.Next(0, 100);
			FreeImageBitmap fib = new FreeImageBitmap(width, height, PixelFormat.Format24bppRgb);

			Assert.AreEqual(fib.VerticalResolution, fib.HorizontalResolution);
			GraphicsUnit unit;
			RectangleF rect;

			unit = GraphicsUnit.Display;
			rect = fib.GetBounds(ref unit);

			Assert.AreEqual(GraphicsUnit.Pixel, unit);
			Assert.AreEqual(width, (int)rect.Width);
			Assert.AreEqual(height, (int)rect.Height);
			fib.Dispose();
		}

		[Test]
		public void GetPropertyItem()
		{
			FreeImageBitmap fib = new FreeImageBitmap(iManager.GetBitmapPath(ImageType.Metadata, ImageColorType.Type_01_Dither));
			int[] list = fib.PropertyIdList;
			Assert.IsNotNull(list);
			Assert.Greater(list.Length, 0);

			for (int i = 0; i < list.Length; i++)
			{
				PropertyItem item = fib.GetPropertyItem(list[i]);
				Assert.IsNotNull(item);
			}
		}

		[Test]
		public void RemovePropertyItem()
		{
			FreeImageBitmap fib = new FreeImageBitmap(iManager.GetBitmapPath(ImageType.Metadata, ImageColorType.Type_01_Dither));
			Random rand = new Random();
			int[] list = fib.PropertyIdList;
			int length = list.Length;
			Assert.Greater(list.Length, 0);

			int id = list[rand.Next(0, list.Length - 1)];
			Assert.IsNotNull(fib.GetPropertyItem(id));
			fib.RemovePropertyItem(id);
			list = fib.PropertyIdList;
			Assert.That((list.Length + 1) == length);
			fib.Dispose();
		}

		[Test]
		public unsafe void RotateFlip()
		{
			FreeImageBitmap fib = new FreeImageBitmap(2, 2, PixelFormat.Format32bppArgb);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.RotateNoneFlipX);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(1))[1]);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.RotateNoneFlipY);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(1))[1]);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.RotateNoneFlipXY);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(1))[1]);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.Rotate90FlipNone);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(1))[1]);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.Rotate90FlipX);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(1))[1]);

			ResetRotateBitmap(fib);
			fib.RotateFlip(RotateFlipType.Rotate90FlipY);
			Assert.AreEqual(0x00000004, ((int*)fib.GetScanlinePointer(0))[0]);
			Assert.AreEqual(0x00000002, ((int*)fib.GetScanlinePointer(0))[1]);
			Assert.AreEqual(0x00000003, ((int*)fib.GetScanlinePointer(1))[0]);
			Assert.AreEqual(0x00000001, ((int*)fib.GetScanlinePointer(1))[1]);

			fib.Dispose();
		}

		private unsafe void ResetRotateBitmap(FreeImageBitmap fib)
		{
			((int*)fib.GetScanlinePointer(0))[0] = 0x00000001;
			((int*)fib.GetScanlinePointer(0))[1] = 0x00000002;
			((int*)fib.GetScanlinePointer(1))[0] = 0x00000003;
			((int*)fib.GetScanlinePointer(1))[1] = 0x00000004;
		}

		[Test]
		public unsafe void GetSetPixel()
		{
			Random rand = new Random();
			FreeImageBitmap fib = new FreeImageBitmap(2, 1, PixelFormat.Format1bppIndexed);
			RGBQUADARRAY palette = fib.Palette;
			for (int i = 0; i < palette.Length; i++)
			{
				palette.SetUIntColor(i, (uint)rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, palette[i]);
			}
			for (int i = 0; i < palette.Length; i++)
			{
				Assert.AreEqual(fib.GetPixel(i, 0), palette[i].color);
			}
			fib.Dispose();

			fib = new FreeImageBitmap(16, 1, PixelFormat.Format4bppIndexed);
			palette = fib.Palette;
			for (int i = 0; i < palette.Length; i++)
			{
				palette.SetUIntColor(i, (uint)rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, palette[i]);
			}
			for (int i = 0; i < palette.Length; i++)
			{
				Assert.AreEqual(fib.GetPixel(i, 0), palette[i].color);
			}
			fib.Dispose();

			fib = new FreeImageBitmap(256, 1, PixelFormat.Format8bppIndexed);
			palette = fib.Palette;
			for (int i = 0; i < palette.Length; i++)
			{
				palette.SetUIntColor(i, (uint)rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, palette[i]);
			}
			for (int i = 0; i < palette.Length; i++)
			{
				Assert.AreEqual(fib.GetPixel(i, 0), palette[i].color);
			}
			fib.Dispose();

			fib = new FreeImageBitmap(1000, 1, PixelFormat.Format16bppRgb555);
			for (int i = 0; i < 1000; i++)
			{
				Color orgColor = Color.FromArgb(rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, orgColor);
				Color newColor = fib.GetPixel(i, 0);
				Assert.That(Math.Abs(orgColor.B - newColor.B) <= 8);
				Assert.That(Math.Abs(orgColor.G - newColor.G) <= 8);
				Assert.That(Math.Abs(orgColor.R - newColor.R) <= 8);
			}
			fib.Dispose();

			fib = new FreeImageBitmap(1000, 1, PixelFormat.Format24bppRgb);
			for (int i = 0; i < 1000; i++)
			{
				Color orgColor = Color.FromArgb(rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, orgColor);
				Color newColor = fib.GetPixel(i, 0);
				Assert.AreEqual(orgColor.B, newColor.B);
				Assert.AreEqual(orgColor.G, newColor.G);
				Assert.AreEqual(orgColor.R, newColor.R);
			}
			fib.Dispose();

			fib = new FreeImageBitmap(1000, 1, PixelFormat.Format32bppArgb);
			for (int i = 0; i < 1000; i++)
			{
				Color orgColor = Color.FromArgb(rand.Next(int.MinValue, int.MaxValue));
				fib.SetPixel(i, 0, orgColor);
				Color newColor = fib.GetPixel(i, 0);
				Assert.AreEqual(orgColor.B, newColor.B);
				Assert.AreEqual(orgColor.G, newColor.G);
				Assert.AreEqual(orgColor.R, newColor.R);
				Assert.AreEqual(orgColor.A, newColor.A);
			}
			fib.Dispose();
		}

		[Test]
		public void SaveAdd()
		{
			string filename = @"saveadd.tif";
			FreeImageBitmap fib = new FreeImageBitmap(100, 100, PixelFormat.Format24bppRgb);
			try
			{
				fib.SaveAdd();
				Assert.Fail();
			}
			catch { }
			Assert.IsFalse(File.Exists(filename));
			fib.Save(filename);
			fib.AdjustBrightness(0.3d);
			fib.SaveAdd();
			FreeImageBitmap other = new FreeImageBitmap(100, 100, PixelFormat.Format24bppRgb);
			foreach (RGBTRIPLEARRAY scanline in other)
			{
				for (int i = 0; i < scanline.Length; i++)
				{
					scanline.SetUIntColor(i, 0xFF339955);
				}
			}
			fib.SaveAdd(other);
			other.SaveAdd(filename);
			other.Dispose();
			fib.Dispose();

			fib = new FreeImageBitmap(filename);
			Assert.AreEqual(4, fib.FrameCount);
			fib.Dispose();
			File.Delete(filename);
			Assert.IsFalse(File.Exists(filename));
		}

		[Test]
		public void Clone()
		{
			FreeImageBitmap fib = new FreeImageBitmap(iManager.GetBitmapPath(ImageType.Even, ImageColorType.Type_24));
			object obj = new object();
			fib.Tag = obj;
			FreeImageBitmap clone = fib.Clone() as FreeImageBitmap;
			Assert.IsNotNull(clone);
			Assert.AreEqual(fib.Width, clone.Width);
			Assert.AreEqual(fib.Height, clone.Height);
			Assert.AreEqual(fib.ColorDepth, clone.ColorDepth);
			Assert.AreSame(fib.Tag, clone.Tag);
			Assert.AreEqual(fib.ImageFormat, clone.ImageFormat);
			clone.Dispose();
			fib.Dispose();
		}

		[Ignore]
		public void LockBits()
		{
		}

		[Ignore]
		public void UnlockBits()
		{
		}

		[Test]
		public void GetTypeConvertedInstance()
		{
			using (FreeImageBitmap fib = new FreeImageBitmap(10, 10, PixelFormat.Format8bppIndexed))
			{
				Assert.AreEqual(FREE_IMAGE_TYPE.FIT_BITMAP, fib.ImageType);
				using (FreeImageBitmap conv = fib.GetTypeConvertedInstance(FREE_IMAGE_TYPE.FIT_DOUBLE, true))
				{
					Assert.IsNotNull(conv);
					Assert.AreEqual(FREE_IMAGE_TYPE.FIT_DOUBLE, conv.ImageType);
				}
			}
		}

		[Test]
		public void GetColorConvertedInstance()
		{
			using (FreeImageBitmap fib = new FreeImageBitmap(10, 10, PixelFormat.Format32bppArgb))
			{
				Assert.AreEqual(32, fib.ColorDepth);
				using (FreeImageBitmap conv = fib.GetColorConvertedInstance(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP))
				{
					Assert.IsNotNull(conv);
					Assert.AreEqual(24, conv.ColorDepth);
				}
			}
		}

		[Test]
		public void GetScaledInstance()
		{
			using (FreeImageBitmap fib = new FreeImageBitmap(100, 80, PixelFormat.Format32bppArgb))
			{
				Assert.AreEqual(100, fib.Width);
				Assert.AreEqual(80, fib.Height);
				using (FreeImageBitmap conv = fib.GetScaledInstance(80, 60, FREE_IMAGE_FILTER.FILTER_BICUBIC))
				{
					Assert.IsNotNull(conv);
					Assert.AreEqual(80, conv.Width);
					Assert.AreEqual(60, conv.Height);
				}
			}
		}

		[Test]
		public unsafe void GetRotatedInstance()
		{
			using (FreeImageBitmap fib = new FreeImageBitmap(2, 2, PixelFormat.Format32bppArgb))
			{
				((int*)fib.GetScanlinePointer(0))[0] = 0x1;
				((int*)fib.GetScanlinePointer(0))[1] = 0x2;
				((int*)fib.GetScanlinePointer(1))[0] = 0x3;
				((int*)fib.GetScanlinePointer(1))[1] = 0x4;
				using (FreeImageBitmap conv = fib.GetRotatedInstance(90d))
				{
					Assert.IsNotNull(conv);
					Assert.AreEqual(((int*)conv.GetScanlinePointer(0))[0], 0x3);
					Assert.AreEqual(((int*)conv.GetScanlinePointer(0))[1], 0x1);
					Assert.AreEqual(((int*)conv.GetScanlinePointer(1))[0], 0x4);
					Assert.AreEqual(((int*)conv.GetScanlinePointer(1))[1], 0x2);
				}
			}
		}

		[Test]
		public void GetScanline()
		{
			FreeImageBitmap fib;

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format1bppIndexed);
			FI1BITARRAY scanline1 = (FI1BITARRAY)fib.GetScanline(0);
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format4bppIndexed);
			FI4BITARRAY scanline2 = (FI4BITARRAY)fib.GetScanline(0);
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format8bppIndexed);
			FI8BITARRAY scanline3 = (FI8BITARRAY)fib.GetScanline(0);
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format16bppRgb555);
			FI16RGBARRAY scanline4 = (FI16RGBARRAY)fib.GetScanline(0);
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format24bppRgb);
			RGBTRIPLEARRAY scanline5 = (RGBTRIPLEARRAY)fib.GetScanline(0);
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format32bppArgb);
			RGBQUADARRAY scanline6 = (RGBQUADARRAY)fib.GetScanline(0);
			fib.Dispose();
		}

		[Test]
		public void GetScanlines()
		{
			FreeImageBitmap fib;

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format1bppIndexed);
			IList<FI1BITARRAY> scanline1 = (IList<FI1BITARRAY>)fib.GetScanlines();
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format4bppIndexed);
			IList<FI4BITARRAY> scanline2 = (IList<FI4BITARRAY>)fib.GetScanlines();
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format8bppIndexed);
			IList<FI8BITARRAY> scanline3 = (IList<FI8BITARRAY>)fib.GetScanlines();
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format16bppRgb555);
			IList<FI16RGBARRAY> scanline4 = (IList<FI16RGBARRAY>)fib.GetScanlines();
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format24bppRgb);
			IList<RGBTRIPLEARRAY> scanline5 = (IList<RGBTRIPLEARRAY>)fib.GetScanlines();
			fib.Dispose();

			fib = new FreeImageBitmap(10, 10, PixelFormat.Format32bppArgb);
			IList<RGBQUADARRAY> scanline6 = (IList<RGBQUADARRAY>)fib.GetScanlines();
			fib.Dispose();
		}

		[Test]
		public void Operators()
		{
			FreeImageBitmap fib1 = null, fib2 = null;
			Assert.IsTrue(fib1 == fib2);
			Assert.IsFalse(fib1 != fib2);
			Assert.IsTrue(fib1 == null);
			Assert.IsFalse(fib1 != null);

			fib1 = new FreeImageBitmap(10, 10, PixelFormat.Format24bppRgb);
			Assert.IsFalse(fib1 == fib2);
			Assert.IsTrue(fib1 != fib2);

			fib2 = fib1;
			fib1 = null;
			Assert.IsFalse(fib1 == fib2);
			Assert.IsTrue(fib1 != fib2);

			fib1 = new FreeImageBitmap(10, 9, PixelFormat.Format24bppRgb);
			Assert.IsFalse(fib1 == fib2);
			Assert.IsTrue(fib1 != fib2);

			fib2.Dispose();
			fib2 = fib1;

			Assert.IsTrue(fib1 == fib2);
			Assert.IsFalse(fib1 != fib2);

			fib2 = fib1.Clone() as FreeImageBitmap;
			Assert.IsTrue(fib1 == fib2);
			Assert.IsFalse(fib1 != fib2);

			fib1.Dispose();
			fib2.Dispose();
		}
	}

	public class Program
	{
		static ImageManager iManager = new ImageManager();
		static ImportedFunctionsTest ift = new ImportedFunctionsTest();
		static ImportedStructsTest ist = new ImportedStructsTest();
		static WrapperStructsTest wst = new WrapperStructsTest();
		static WrapperFunctionsTest wft = new WrapperFunctionsTest();
		static FreeImageBitmapTest fib = new FreeImageBitmapTest();

		public static void Main()
		{
			List<TestClass> classList = new List<TestClass>(5);
			classList.Add(new TestClass(ift));
			classList.Add(new TestClass(ist));
			classList.Add(new TestClass(wst));
			classList.Add(new TestClass(wft));
			classList.Add(new TestClass(fib));

			for (int i = 0; i < 10000; )
			{
				for (int j = 0; j < classList.Count; j++)
					classList[j].ExecuteTests();
				Console.WriteLine("Loop {0}", ++i);
				//GC.Collect();
			}
		}
	}

	public class TestClass
	{
		private object classMember = null;

		private MethodInfo classSetUp = null;
		private MethodInfo classTearDown = null;

		private MethodInfo testSetUp = null;
		private MethodInfo testTearDown = null;

		private List<MethodInfo> methodList = null;

		private static object[] parameters = { };

		public TestClass(object classMember)
		{
			this.classMember = classMember;
			MethodInfo[] infos = classMember.GetType().GetMethods(System.Reflection.BindingFlags.Public | BindingFlags.Instance);
			methodList = new List<MethodInfo>(infos.Length);

			foreach (MethodInfo info in infos)
			{
				object[] attributes = info.GetCustomAttributes(false);
				foreach (Attribute attribute in attributes)
				{
					if (attribute.GetType() == typeof(TestAttribute))
					{
						methodList.Add(info);
						break;
					}
					else if (attribute.GetType() == typeof(TestFixtureSetUpAttribute))
					{
						classSetUp = info;
						break;
					}
					else if (attribute.GetType() == typeof(TestFixtureTearDownAttribute))
					{
						classTearDown = info;
						break;
					}
					else if (attribute.GetType() == typeof(SetUpAttribute))
					{
						testSetUp = info;
						break;
					}
					else if (attribute.GetType() == typeof(TearDownAttribute))
					{
						testTearDown = info;
						break;
					}
				}
			}
		}

		public void ExecuteTests()
		{
			if (classSetUp != null)
				classSetUp.Invoke(classMember, parameters);

			foreach (MethodInfo method in methodList)
			{
				if (testSetUp != null)
					testSetUp.Invoke(classMember, parameters);

				try
				{
					Console.WriteLine(method.ToString());
					method.Invoke(classMember, parameters);
				}
				catch (Exception ex)
				{
					while (ex.InnerException != null)
						ex = ex.InnerException;
					Console.WriteLine(ex.ToString());
					Environment.Exit(99);
				}

				if (testTearDown != null)
					testTearDown.Invoke(classMember, parameters);
			}

			if (classTearDown != null)
				classTearDown.Invoke(classMember, parameters);
		}
	}
}