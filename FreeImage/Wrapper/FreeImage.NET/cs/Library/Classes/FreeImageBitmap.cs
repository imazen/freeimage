// ==========================================================
// FreeImage 3 .NET wrapper
// Original FreeImage 3 functions and .NET compatible derived functions
//
// Design and implementation by
// - Jean-Philippe Goerke (jpgoerke@users.sourceforge.net)
// - Carsten Klein (cklein05@users.sourceforge.net)
//
// Contributors:
// - David Boland (davidboland@vodafone.ie)
//
// Main reference : MSDN Knowlede Base
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

// ==========================================================
// CVS
// $Revision$
// $Date$
// $Id$
// ==========================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;

namespace FreeImageAPI
{
	/// <summary>
	/// Encapsulates a FreeImage-bitmap.
	/// </summary>
	[Serializable, Guid("64a4c935-b757-499c-ab8c-6110316a9e51")]
	public class FreeImageBitmap : ICloneable, IDisposable, IEnumerable, ISerializable
	{
		private bool disposed = false;
		private object tag = null;
		private object lockObject = new object();
		private SaveInformation saveInformation = new SaveInformation();
		protected FREE_IMAGE_FORMAT originalFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
		protected FIBITMAP dib = 0;
		protected FIMULTIBITMAP mdib = 0;

		#region Constructors and Destructor

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// </summary>
		protected FreeImageBitmap()
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// For internal use only.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		internal FreeImageBitmap(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new Exception();
			}
			this.dib = dib;
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(FreeImageBitmap original)
		{
			original.ThrowOnDisposed();
			dib = FreeImage.Clone(original.dib);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(FreeImageBitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(FreeImageBitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			original.ThrowOnDisposed();
			dib = FreeImage.Rescale(original.dib, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Image original)
			: this(original as Bitmap)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(Image original, Size newSize)
			: this(original as Bitmap, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(Image original, int width, int height)
			: this(original as Bitmap, width, height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Bitmap original)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			dib = FreeImage.CreateFromBitmap(original, true);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(Bitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(Bitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			FIBITMAP temp = FreeImage.CreateFromBitmap(original, true);
			if (temp.IsNull)
			{
				throw new Exception();
			}
			else
			{
				dib = FreeImage.Rescale(temp, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
				FreeImage.Unload(temp);
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, bool useIcm)
			: this(stream)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream in the specified format.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format)
			: this(stream, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_LOAD_FLAGS flags)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream in the specified format
		/// with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			saveInformation.loadFlags = flags;

			dib = FreeImage.LoadFromStream(stream, flags, ref format);

			if (dib.IsNull)
			{
				throw new Exception();
			}

			originalFormat = format;
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename)
			: this(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, bool useIcm)
			: this(filename)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_LOAD_FLAGS flags)
			: this(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// in the specified format.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format)
			: this(filename, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// in the specified format with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			saveInformation.loadFlags = flags;

			mdib = FreeImage.OpenMultiBitmapEx(filename, ref format, flags, false, true, false);

			if (mdib.IsNull)
			{
				throw new Exception();
			}

			originalFormat = format;

			if (FreeImage.GetPageCount(mdib) != 0)
			{
				if (FreeImage.GetPageCount(mdib) == 1)
				{
					if (!FreeImage.CloseMultiBitmapEx(ref mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT))
					{
						throw new Exception();
					}

					dib = FreeImage.LoadEx(filename, flags, ref format);
					if (dib.IsNull)
					{
						throw new Exception();
					}

					return;
				}
				else
				{
					dib = FreeImage.LockPage(mdib, 0);
					if (!dib.IsNull)
					{
						return;
					}
				}
			}

			FreeImage.CloseMultiBitmap(mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
			throw new Exception();
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// with the specified size.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(int width, int height)
		{
			dib = FreeImage.Allocate(
				width,
				height,
				24,
				FreeImage.FI_RGBA_RED_MASK,
				FreeImage.FI_RGBA_GREEN_MASK,
				FreeImage.FI_RGBA_BLUE_MASK);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from a specified resource.
		/// </summary>
		/// <param name="type">The class used to extract the resource.</param>
		/// <param name="resource">The name of the resource.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Type type, string resource)
			: this(type.Module.Assembly.GetManifestResourceStream(type, resource))
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size
		/// and with the resolution of the specified Graphics object.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="g">The Graphics object that specifies the resolution for the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="g"/> is null.</exception>
		public FreeImageBitmap(int width, int height, Graphics g)
			: this(width, height)
		{
			FreeImage.SetResolutionX(dib, (uint)g.DpiX);
			FreeImage.SetResolutionX(dib, (uint)g.DpiY);
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size and format.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="format">The PixelFormat enumeration for the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		public FreeImageBitmap(int width, int height, PixelFormat format)
		{
			uint bpp, redMask, greenMask, blueMask;
			if (!FreeImage.GetFormatParameters(format, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid.");
			}
			dib = FreeImage.Allocate(width, height, (int)bpp, redMask, greenMask, blueMask);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size,
		/// pixel format, and pixel data.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="stride">nteger that specifies the byte offset between the beginning
		/// of one scan line and the next. This is usually (but not necessarily)
		/// the number of bytes in the pixel format (for example, 2 for 16 bits per pixel)
		/// multiplied by the width of the bitmap. The value passed to this parameter must
		/// be a multiple of four..</param>
		/// <param name="format">The PixelFormat enumeration for the new FreeImageBitmap.</param>
		/// <param name="scan0">Pointer to an array of bytes that contains the pixel data.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		public FreeImageBitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		{
			uint bpp, redMask, greenMask, blueMask;
			bool topDown = (stride > 0);
			stride = (stride > 0) ? stride : (stride * -1);

			if (!FreeImage.GetFormatParameters(format, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid.");
			}

			dib = FreeImage.ConvertFromRawBits(
				scan0, width, height, stride, bpp, redMask, greenMask, blueMask, topDown);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="SerializationException">The operation failed.</exception>
		public FreeImageBitmap(SerializationInfo info, StreamingContext context)
		{
			try
			{
				byte[] data = (byte[])info.GetValue("Bitmap Data", typeof(byte[]));
				if (data != null && data.Length > 0)
				{
					MemoryStream memory = new MemoryStream(data);
					FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_TIFF;
					dib = FreeImage.LoadFromStream(memory, ref format);
				}
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
			catch
			{
				throw new SerializationException();
			}
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~FreeImageBitmap()
		{
			Dispose(false);
		}

		#endregion

		#region Operators

		/// <summary>
		/// The implicit conversion from FreeImageBitmap into Bitmap
		/// allows to create an instance on the fly and use it as if
		/// was a Bitmap. This way it can be directly used with a
		/// PixtureBox for example without having to call any
		/// conversion operations.
		/// </summary>
		public static implicit operator Bitmap(FreeImageBitmap value)
		{
			value.ThrowOnDisposed();
			return FreeImage.GetBitmap(value.dib, true);
		}

		/// <summary>
		/// The implicit conversion from Bitmap into FreeImageBitmap
		/// allows to create an instance on the fly to perform
		/// image processing operations and converting it back.
		/// </summary>
		public static implicit operator FreeImageBitmap(Bitmap value)
		{
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.CreateFromBitmap(value, true);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		public static bool operator ==(FreeImageBitmap fib1, FreeImageBitmap fib2)
		{
			bool result = false;
			if (object.ReferenceEquals(fib1, fib2))
			{
				if (!object.ReferenceEquals(fib1, null))
				{
					fib1.ThrowOnDisposed();
				}
				result = true;
			}
			else if (object.ReferenceEquals(fib1, null))
			{
				fib2.ThrowOnDisposed();
			}
			else if (object.ReferenceEquals(fib2, null))
			{
				fib1.ThrowOnDisposed();
			}
			else
			{
				fib1.ThrowOnDisposed();
				fib2.ThrowOnDisposed();
				result = FreeImage.Compare(fib1.dib, fib2.dib, FREE_IMAGE_COMPARE_FLAGS.COMPLETE);
			}
			return result;
		}

		public static bool operator !=(FreeImageBitmap fib1, FreeImageBitmap fib2)
		{
			return !(fib1 == fib2);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Type of the bitmap.
		/// </summary>
		public FREE_IMAGE_TYPE ImageType
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetImageType(dib);
			}
		}

		/// <summary>
		/// Number of palette entries.
		/// </summary>
		public int ColorsUsed
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetColorsUsed(dib);
			}
		}

		/// <summary>
		/// The number of unique colors actually used by the bitmap. This might be different from
		/// what ColorsUsed returns, which actually returns the palette size for palletised images.
		/// Works for FIT_BITMAP type bitmaps only.
		/// </summary>
		public int UniqueColors
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetUniqueColors(dib);
			}
		}

		/// <summary>
		/// The size of one pixel in the bitmap in bits.
		/// </summary>
		public int ColorDepth
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetBPP(dib);
			}
		}

		/// <summary>
		/// Width of the bitmap in pixel units.
		/// </summary>
		public int Width
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetWidth(dib);
			}
		}

		/// <summary>
		/// Height of the bitmap in pixel units.
		/// </summary>
		public int Height
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetHeight(dib);
			}
		}

		/// <summary>
		/// Returns the width of the bitmap in bytes, rounded to the next 32-bit boundary.
		/// </summary>
		public int Pitch
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetPitch(dib);
			}
		}

		/// <summary>
		/// Size of the bitmap in memory.
		/// </summary>
		public int DataSize
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetDIBSize(dib);
			}
		}

		/// <summary>
		/// Returns a structure that wraps the palette of a FreeImage bitmap.
		/// </summary>
		/// <exception cref="Exception">'HasPalette' is false.</exception>
		public RGBQUADARRAY Palette
		{
			get
			{
				ThrowOnDisposed();
				if (HasPalette)
				{
					return FreeImage.GetPaletteEx(dib);
				}
				throw new Exception();
			}
		}

		/// <summary>
		/// Gets the horizontal resolution, in pixels per inch, of this bitmap.
		/// </summary>
		public float HorizontalResolution
		{
			get
			{
				ThrowOnDisposed();
				return (float)FreeImage.GetResolutionX(dib);
			}
			private set
			{
				ThrowOnDisposed();
				FreeImage.SetResolutionX(dib, (uint)value);
			}
		}

		/// <summary>
		/// Gets the vertical resolution, in pixels per inch, of this bitmap.
		/// </summary>
		public float VerticalResolution
		{
			get
			{
				ThrowOnDisposed();
				return (float)FreeImage.GetResolutionY(dib);
			}
			private set
			{
				ThrowOnDisposed();
				FreeImage.SetResolutionY(dib, (uint)value);
			}
		}

		/// <summary>
		/// Returns the BITMAPINFOHEADER structure of this bitmap.
		/// </summary>
		public BITMAPINFOHEADER InfoHeader
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetInfoHeaderEx(dib);
			}
		}

		/// <summary>
		/// Returns the BITMAPINFO structure of a this bitmap.
		/// </summary>
		public BITMAPINFO Info
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetInfoEx(dib);
			}
		}

		/// <summary>
		/// Investigates the color type of the bitmap
		/// by reading the bitmaps pixel bits and analysing them.
		/// </summary>
		public FREE_IMAGE_COLOR_TYPE ColorType
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetColorType(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the red color component of a pixel in this bitmap.
		/// </summary>
		public uint RedMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetRedMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the green color component of a pixel in this bitmap.
		/// </summary>
		public uint GreenMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetGreenMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the blue color component of a pixel in this bitmap.
		/// </summary>
		public uint BlueMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetBlueMask(dib);
			}
		}

		/// <summary>
		/// Number of transparent colors in a palletised bitmap.
		/// </summary>
		public int TransparencyCount
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetTransparencyCount(dib);
			}
		}

		/// <summary>
		/// Get or sets the bitmap's transparency table.
		/// </summary>
		public byte[] TransparencyTable
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetTransparencyTableEx(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparencyTable(dib, value);
			}
		}

		/// <summary>
		/// Gets or sets whether this bitmap is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.IsTransparent(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparent(dib, value);
			}
		}

		/// <summary>
		/// Gets whether this bitmap has a file background color.
		/// </summary>
		public bool HasBackgroundColor
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.HasBackgroundColor(dib);
			}
		}

		/// <summary>
		/// Gets or sets the bitmap's background color.
		/// In case the value is null, the background color is removed.
		/// </summary>
		/// <exception cref="InvalidOperationException">Get: There is no background color available.</exception>
		/// <exception cref="Exception">Set: Setting background color failed.</exception>
		public Color? BackgroundColor
		{
			get
			{
				ThrowOnDisposed();
				if (!FreeImage.HasBackgroundColor(dib))
				{
					throw new InvalidOperationException("No background color available.");
				}
				RGBQUAD rgbq;
				FreeImage.GetBackgroundColor(dib, out rgbq);
				return rgbq.color;
			}
			set
			{
				ThrowOnDisposed();
				if (!FreeImage.SetBackgroundColor(dib, (value.HasValue ? new RGBQUAD[] { value.Value } : null)))
				{
					throw new Exception("Setting background color failed.");
				}
			}
		}

		/// <summary>
		/// Pointer to the data-bits of the bitmap.
		/// </summary>
		public IntPtr Bits
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetBits(dib);
			}
		}

		/// <summary>
		/// Width of the bitmap in bytes.
		/// </summary>
		public int Line
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetLine(dib);
			}
		}

		/// <summary>
		/// Pointer to the scanline of the bitmap's top most pixel row.
		/// </summary>
		public IntPtr Scan0
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetScanLine(dib, (int)(FreeImage.GetHeight(dib) - 1));
			}
		}

		/// <summary>
		/// Width of the bitmap in bytes.
		/// In case the bitmap is top down 'Stride' will be positive, else negative.
		/// </summary>
		public int Stride
		{
			get
			{
				return -Line;
			}
		}

		/// <summary>
		/// Gets attribute flags for the pixel data of this bitmap.
		/// </summary>
		public unsafe int Flags
		{
			get
			{
				ThrowOnDisposed();
				int result = 0;
				byte alpha;
				int cd = ColorDepth;

				if ((cd == 32) || (FreeImage.GetTransparencyCount(dib) != 0))
				{
					result += (int)ImageFlags.HasAlpha;
				}

				if (cd == 32)
				{
					uint width = FreeImage.GetWidth(dib);
					uint height = FreeImage.GetHeight(dib);
					for (int y = 0; y < height; y++)
					{
						RGBQUAD* scanline = (RGBQUAD*)FreeImage.GetScanLine(dib, y);
						for (int x = 0; x < width; x++)
						{
							alpha = scanline[x].color.A;
							if (alpha != byte.MinValue && alpha != byte.MaxValue)
							{
								result += (int)ImageFlags.HasTranslucent;
								y = (int)height;
								break;
							}
						}
					}
				}
				else if (FreeImage.GetTransparencyCount(dib) != 0)
				{
					byte[] transTable = FreeImage.GetTransparencyTableEx(dib);
					for (int i = 0; i < transTable.Length; i++)
					{
						if (transTable[i] != byte.MinValue && transTable[i] != byte.MaxValue)
						{
							result += (int)ImageFlags.HasTranslucent;
							break;
						}
					}
				}

				if (FreeImage.GetICCProfileEx(dib).IsCMYK)
				{
					result += (int)ImageFlags.ColorSpaceCmyk;
				}
				else
				{
					result += (int)ImageFlags.ColorSpaceRgb;
				}

				if (FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISBLACK ||
					FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISWHITE)
				{
					result += (int)ImageFlags.ColorSpaceGray;
				}

				if (originalFormat == FREE_IMAGE_FORMAT.FIF_BMP ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_FAXG3 ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_ICO ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_JPEG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PCX ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PNG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PSD ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_TIFF)
				{
					result += (int)ImageFlags.HasRealDpi;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets the width and height of this bitmap.
		/// </summary>
		public SizeF PhysicalDimension
		{
			get
			{
				ThrowOnDisposed();
				return new SizeF((float)FreeImage.GetWidth(dib), (float)FreeImage.GetHeight(dib));
			}
		}

		/// <summary>
		/// Gets the pixel format for this bitmap.
		/// </summary>
		public PixelFormat PixelFormat
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetPixelFormat(dib);
			}
		}

		/// <summary>
		/// Gets IDs of the property items stored in this bitmap.
		/// </summary>
		public int[] PropertyIdList
		{
			get
			{
				ThrowOnDisposed();
				List<int> list = new List<int>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.ID);
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets all the property items (pieces of metadata) stored in this bitmap.
		/// </summary>
		public PropertyItem[] PropertyItems
		{
			get
			{
				ThrowOnDisposed();
				List<PropertyItem> list = new List<PropertyItem>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.GetPropertyItem());
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets the format of this bitmap.
		/// </summary>
		public ImageFormat RawFormat
		{
			get
			{
				ThrowOnDisposed();
				Attribute guidAttribute =
					Attribute.GetCustomAttribute(
						typeof(FreeImageBitmap), typeof(System.Runtime.InteropServices.GuidAttribute)
					);
				return (guidAttribute == null) ?
					null :
					new ImageFormat(new Guid(((GuidAttribute)guidAttribute).Value));
			}
		}

		/// <summary>
		/// Gets the width and height, in pixels, of this bitmap.
		/// </summary>
		public Size Size
		{
			get
			{
				ThrowOnDisposed();
				return new Size(Width, Height);
			}
		}

		/// <summary>
		/// Gets or sets an object that provides additional data about the bitmap.
		/// </summary>
		public Object Tag
		{
			get
			{
				ThrowOnDisposed();
				return tag;
			}
			set
			{
				ThrowOnDisposed();
				tag = value;
			}
		}

		/// <summary>
		/// Gets whether the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return disposed;
			}
		}

		/// <summary>
		/// Gets a new instance of a metadata wrapping class.
		/// </summary>
		public ImageMetadata Metadata
		{
			get
			{
				ThrowOnDisposed();
				return new ImageMetadata(dib, true);
			}
		}

		/// <summary>
		/// Gets or sets the comment of the bitmap.
		/// Supported formats are JPEG, PNG and GIF.
		/// </summary>
		public string Comment
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetImageComment(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetImageComment(dib, value);
			}
		}

		/// <summary>
		/// Returns whether the bitmap has a palette.
		/// </summary>
		public bool HasPalette
		{
			get
			{
				ThrowOnDisposed();
				return (FreeImage.GetPalette(dib) != IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets or sets the bitmap's palette entry used as transparent color.
		/// Only works for 1-, 4- and 8-bpp.
		/// </summary>
		public int TransparentIndex
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetTransparentIndex(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparentIndex(dib, value);
			}
		}

		/// <summary>
		/// Gets the number of frames in the bitmap.
		/// </summary>
		public int FrameCount
		{
			get
			{
				ThrowOnDisposed();
				int result = 1;
				if (!mdib.IsNull)
				{
					result = FreeImage.GetPageCount(mdib);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the ICCProfile structure of the bitmap.
		/// </summary>
		public FIICCPROFILE ICCProfile
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetICCProfileEx(dib);
			}
		}

		/// <summary>
		/// Gets the format of the original image in case
		/// the bitmap was loaded from a file or stream.
		/// </summary>
		public FREE_IMAGE_FORMAT ImageFormat
		{
			get
			{
				ThrowOnDisposed();
				return originalFormat;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the bounds of the image in the specified unit.
		/// </summary>
		/// <param name="pageUnit">One of the GraphicsUnit values indicating
		/// the unit of measure for the bounding rectangle.</param>
		/// <returns>The RectangleF that represents the bounds of the image,
		/// in the specified unit.</returns>
		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			ThrowOnDisposed();
			pageUnit = GraphicsUnit.Pixel;
			return new RectangleF(
					0f,
					0f,
					(float)FreeImage.GetWidth(dib),
					(float)FreeImage.GetHeight(dib));
		}

		/// <summary>
		/// Gets the specified property item from this bitmap.
		/// </summary>
		/// <param name="propid">The ID of the property item to get.</param>
		/// <returns>The PropertyItem this method gets.</returns>
		public PropertyItem GetPropertyItem(int propid)
		{
			ThrowOnDisposed();
			ImageMetadata metadata = new ImageMetadata(dib, true);
			foreach (MetadataModel metadataModel in metadata)
			{
				foreach (MetadataTag tag in metadataModel)
				{
					if (tag.ID == propid)
					{
						return tag.GetPropertyItem();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a thumbnail for this bitmap.
		/// </summary>
		/// <param name="thumbWidth">The width, in pixels, of the requested thumbnail image.</param>
		/// <param name="thumbHeight">The height, in pixels, of the requested thumbnail image.</param>
		/// <param name="callback">Ignored.</param>
		/// <param name="callBackData">Ignored.</param>
		/// <returns>A bitmap that represents the thumbnail.</returns>
		public FreeImageBitmap GetThumbnailImage(int thumbWidth, int thumbHeight,
			Image.GetThumbnailImageAbort callback, IntPtr callBackData)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(
				dib, thumbWidth, thumbHeight, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Returns a thumbnail for this FreeImageBitmap, keeping aspect ratio.
		/// <paramref name="maxPixelSize"/> defines the maximum width or height
		/// of the thumbnail.
		/// </summary>
		/// <param name="maxPixelSize">Thumbnail square size.</param>
		/// <param name="convert">When true HDR images are transperantly
		/// converted to standard images.</param>
		/// <returns>The thumbnail in a new instance.</returns>
		public FreeImageBitmap GetThumbnailImage(int maxPixelSize, bool convert)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.MakeThumbnail(dib, maxPixelSize, convert);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Returns a structure, wrapping the bitmap's scanline.
		/// Due to FreeImage bitmaps are bottum up, scanline 0 is the
		/// bottom-most line of the image.
		/// <para>
		/// The structures are FI1BITARRAY (1bpp), FI4BITARRAY (4bpp),
		/// FI8BITARRAY (8bpp), FI16RGBARRAY (16bpp), RGBTRIPLEARRAY (24bpp)
		/// or RGBQUADARRAY (32bpp).
		/// </para>
		/// </summary>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Structure wrapping the scanline.</returns>
		/// <exception cref="ArgumentException">
		/// The bitmap's type or color depth are not supported.
		/// </exception>
		public object GetScanline(int scanline)
		{
			ThrowOnDisposed();
			object result = null;

			switch (FreeImage.GetBPP(dib))
			{
				case 1u: result = new FI1BITARRAY(dib, scanline); break;
				case 4u: result = new FI4BITARRAY(dib, scanline); break;
				case 8u: result = new FI8BITARRAY(dib, scanline); break;
				case 16u: result = new FI16RGBARRAY(dib, scanline); break;
				case 24u: result = new RGBTRIPLEARRAY(dib, scanline); break;
				case 32u: result = new RGBQUADARRAY(dib, scanline); break;
				default: throw new ArgumentException("Color depth or type is not supported.");
			}

			return result;
		}

		/// <summary>
		/// Returns a pointer to the specified scanline.
		/// Due to FreeImage bitmaps are bottum up,
		/// scanline 0 is the most bottom line of the image.
		/// </summary>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Pointer to the scanline.</returns>
		public IntPtr GetScanlinePointer(int scanline)
		{
			ThrowOnDisposed();
			return FreeImage.GetScanLine(dib, scanline);
		}

		/// <summary>
		/// Returns a list of structures, wrapping the bitmap's scanlines.
		/// Due to FreeImage bitmaps are bottum up, scanline 0 is the
		/// bottom-most line of the image.
		/// Each color depth has a different wrapping structure
		/// due to different memory layouts.
		/// <para>
		/// The structures are FI1BITARRAY (1bpp), FI4BITARRAY (4bpp),
		/// FI8BITARRAY (8bpp), FI16RGBARRAY (16bpp), RGBTRIPLEARRAY (24bpp)
		/// or RGBQUADARRAY (32bpp).
		/// </para>
		/// </summary>
		/// <returns>A list containing structures wrapping the bitmap's
		/// scanlines.</returns>
		/// <remarks>
		/// Instead of calling his method it is also possible to the
		/// <c>foreach</c> clause to iterate over each scanline.
		/// </remarks>
		/// <exception cref="NotSupportedException">
		/// The bitmap's type or color depth are not supported.
		/// </exception>
		public IList GetScanlines()
		{
			ThrowOnDisposed();

			IList list = null;
			int height = (int)FreeImage.GetHeight(dib);

			switch (FreeImage.GetBPP(dib))
			{
				case 1u:

					list = new List<FI1BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI1BITARRAY(dib, i));
					}
					break;

				case 4u:

					list = new List<FI4BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI4BITARRAY(dib, i));
					}
					break;

				case 8u:

					list = new List<FI8BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI8BITARRAY(dib, i));
					}
					break;

				case 16u:

					list = new List<FI16RGBARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI16RGBARRAY(dib, i));
					}
					break;

				case 24u:

					list = new List<RGBTRIPLEARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new RGBTRIPLEARRAY(dib, i));
					}
					break;

				case 32u:

					list = new List<RGBQUADARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new RGBQUADARRAY(dib, i));
					}
					break;

				default:

					throw new NotSupportedException("Color depth or type is not supported.");
			}

			return list;
		}

		/// <summary>
		/// Removes the specified property item from this bitmap.
		/// </summary>
		/// <param name="propid">The ID of the property item to remove.</param>
		public void RemovePropertyItem(int propid)
		{
			ThrowOnDisposed();
			ImageMetadata mdata = new ImageMetadata(dib, true);
			foreach (MetadataModel model in mdata)
			{
				foreach (MetadataTag tag in model)
				{
					if (tag.ID == propid)
					{
						model.RemoveTag(tag.Key);
						return;
					}
				}
			}
		}

		/// <summary>
		/// This method rotates, flips, or rotates and flips the bitmap.
		/// </summary>
		/// <param name="rotateFlipType">A RotateFlipType member
		/// that specifies the type of rotation and flip to apply to the bitmap.</param>
		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			ThrowOnDisposed();

			FIBITMAP newDib = 0;
			uint bpp = FreeImage.GetBPP(dib);

			switch (rotateFlipType)
			{
				case RotateFlipType.RotateNoneFlipX:

					FreeImage.FlipHorizontal(dib);
					break;

				case RotateFlipType.RotateNoneFlipY:

					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.RotateNoneFlipXY:

					FreeImage.FlipHorizontal(dib);
					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.Rotate90FlipNone:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					break;

				case RotateFlipType.Rotate90FlipX:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					break;

				case RotateFlipType.Rotate90FlipY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate90FlipXY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate180FlipXY:
					newDib = FreeImage.Clone(dib);
					break;
			}
			ReplaceDib(newDib);
		}

		/// <summary>
		/// Saves this bitmap to the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename)
		{
			Save(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this bitmap to the specified file in the specified format.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format)
		{
			Save(filename, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this bitmap to the specified file in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			ThrowOnDisposed();
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("filename");
			}
			if (!FreeImage.SaveEx(dib, filename, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = filename;
			saveInformation.format = format;
			saveInformation.saveFlags = flags;
		}

		/// <summary>
		/// Saves this image to the specified stream in the specified format.
		/// </summary>
		/// <param name="stream">The stream where the image will be saved.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format)
		{
			Save(stream, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this image to the specified stream in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="stream">The stream where the image will be saved.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			ThrowOnDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!FreeImage.SaveToStream(dib, stream, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = null;
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the Save method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd()
		{
			SaveAdd(this);
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the Save method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="bitmap">A FreeImageBitmap that contains the frame to add.</param>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd(FreeImageBitmap bitmap)
		{
			if (saveInformation.filename == null)
			{
				throw new InvalidOperationException();
			}

			SaveAdd(
				saveInformation.filename,
				bitmap,
				saveInformation.format,
				saveInformation.loadFlags,
				saveInformation.saveFlags);
		}

		/// <summary>
		/// Adds a frame to the file specified.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(string filename)
		{
			SaveAdd(
				filename,
				this,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Adds a frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			SaveAdd(
				filename,
				this,
				format,
				loadFlags,
				saveFlags);
		}

		/// <summary>
		/// Selects the frame specified by the dimension and index.
		/// </summary>
		/// <param name="frameIndex">The index of the active frame.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="frameIndex"/> is out of range.
		/// </exception>
		/// <exception cref="Exception">The operation failed.</exception>
		public void SelectActiveFrame(int frameIndex)
		{
			ThrowOnDisposed();
			if (frameIndex < 0)
			{
				throw new ArgumentOutOfRangeException("frameIndex");
			}
			if (!mdib.IsNull)
			{
				if (frameIndex >= FrameCount)
				{
					throw new ArgumentOutOfRangeException("frameIndex");
				}
				ReplaceDib(FreeImage.LockPage(mdib, frameIndex));
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap()
		{
			ThrowOnDisposed();
			return FreeImage.GetHbitmap(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap.
		/// </summary>
		/// <param name="background">A Color structure that specifies the background color.
		/// This parameter is ignored if the bitmap is totally opaque.</param>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap(Color background)
		{
			ThrowOnDisposed();
			using (FreeImageBitmap temp = new FreeImageBitmap(this))
			{
				temp.BackgroundColor = background;
				return temp.GetHbitmap();
			}
		}

		/// <summary>
		/// Returns the handle to an icon.
		/// </summary>
		/// <returns>A Windows handle to an icon with the same image as the FreeImageBitmap.</returns>
		public IntPtr GetHicon()
		{
			ThrowOnDisposed();
			using (Bitmap bitmap = FreeImage.GetBitmap(dib, true))
			{
				return bitmap.GetHicon();
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap with the same
		/// color depth as the primary device.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmapForDevice()
		{
			ThrowOnDisposed();
			return FreeImage.GetBitmapForDevice(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Gets the color of the specified pixel in this FreeImageBitmap.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to retrieve.</param>
		/// <param name="y">The y-coordinate of the pixel to retrieve.</param>
		/// <returns>A Color structure that represents the color of the specified pixel.</returns>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe Color GetPixel(int x, int y)
		{
			ThrowOnDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq;
					if (!FreeImage.GetPixelColor(dib, (uint)x, (uint)y, out rgbq))
					{
						throw new Exception();
					}
					return rgbq.color;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					byte index;
					if (!FreeImage.GetPixelIndex(dib, (uint)x, (uint)y, out index))
					{
						throw new Exception();
					}
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					return palette[index].color;
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Makes the default transparent color transparent for this FreeImageBitmap.
		/// </summary>
		public void MakeTransparent()
		{
			ThrowOnDisposed();
			MakeTransparent(Color.Transparent);
		}

		/// <summary>
		/// Makes the specified color transparent for this FreeImageBitmap.
		/// </summary>
		/// <param name="transparentColor">The Color structure that represents
		/// the color to make transparent.</param>
		public void MakeTransparent(Color transparentColor)
		{
			ThrowOnDisposed();
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Sets the color of the specified pixel in this FreeImageBitmap.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to set.</param>
		/// <param name="y">The y-coordinate of the pixel to set.</param>
		/// <param name="color">A Color structure that represents the color
		/// to assign to the specified pixel.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe void SetPixel(int x, int y, Color color)
		{
			ThrowOnDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq = color;
					if (!FreeImage.SetPixelColor(dib, (uint)x, (uint)y, ref rgbq))
					{
						throw new Exception();
					}
					return;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					uint colorsUsed = FreeImage.GetColorsUsed(dib);
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					for (int i = 0; i < colorsUsed; i++)
					{
						if (palette[i].color == color)
						{
							byte index = (byte)i;
							if (!FreeImage.SetPixelIndex(dib, (uint)x, (uint)y, ref index))
							{
								throw new Exception();
							}
							return;
						}
					}
					throw new ArgumentOutOfRangeException("color");
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the resolution for this FreeImageBitmap.
		/// </summary>
		/// <param name="xDpi">The horizontal resolution, in dots per inch, of the FreeImageBitmap.</param>
		/// <param name="yDpi">The vertical resolution, in dots per inch, of the FreeImageBitmap.</param>
		public void SetResolution(float xDpi, float yDpi)
		{
			ThrowOnDisposed();
			FreeImage.SetResolutionX(dib, (uint)xDpi);
			FreeImage.SetResolutionY(dib, (uint)yDpi);
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public void UnlockBits(BitmapData bitmapdata)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this FreeImageBitmap into a different color depth.
		/// The parameter 'bpp' specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the 'FICD_FORCE_GREYSCALE' flag will first perform a
		/// convesion to greyscale. This can be done with any target color depth.</para>
		/// <para>Adding the 'FICD_REORDER_PALETTE' flag will allow the algorithm
		/// to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.ConvertColorDepth(dib, bpp, false));
		}

		/// <summary>
		/// Converts this bitmap's FREE_IMAGE_TYPE to 'type' creating
		/// a new instance.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertType(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			ThrowOnDisposed();
			return (ImageType == type) ? false : ReplaceDib(FreeImage.ConvertToType(dib, type, scaleLinear));
		}

		/// <summary>
		/// Converts this bitmap's FREE_IMAGE_TYPE to 'type'.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetTypeConvertedInstance(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			if (ImageType != type)
			{
				FIBITMAP newDib = FreeImage.ConvertToType(dib, type, scaleLinear);
				if (!newDib.IsNull)
				{
					result = new FreeImageBitmap();
					result.dib = newDib;
				}
			}
			return result;
		}

		/// <summary>
		/// Converts this FreeImageBitmap into a different color depth creating
		/// a new instance.
		/// The parameter 'bpp' specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the 'FICD_FORCE_GREYSCALE' flag will first perform a
		/// convesion to greyscale. This can be done with any target color depth.</para>
		/// <para>Adding the 'FICD_REORDER_PALETTE' flag will allow the algorithm
		/// to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetColorConvertedInstance(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.ConvertColorDepth(dib, bpp, false);
			if (newDib == dib)
			{
				newDib = FreeImage.Clone(dib);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return Rescale(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(int width, int height, FREE_IMAGE_FILTER filter)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.Rescale(dib, width, height, filter));
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter creating a new instance.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return GetScaledInstance(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter creating a new instance.
		/// </summary>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(int width, int height, FREE_IMAGE_FILTER filter)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(dib, width, height, filter);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global
		/// operator based on logarithmic compression of luminance values, imitating the human response to light.
		/// </summary>
		/// <param name="gamma">A gamma correction that is applied after the tone mapping.
		/// A value of 1 means no correction.</param>
		/// <param name="exposure">Scale factor allowing to adjust the brightness of the output image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoDrago03(double gamma, double exposure)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoDrago03(dib, gamma, exposure));
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global operator inspired
		/// by photoreceptor physiology of the human visual system.
		/// </summary>
		/// <param name="intensity">Controls the overall image intensity in the range [-8, 8].</param>
		/// <param name="contrast">Controls the overall image contrast in the range [0.3, 1.0[.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoReinhard05(double intensity, double contrast)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoReinhard05(dib, intensity, contrast));
		}

		/// <summary>
		/// Apply the Gradient Domain High Dynamic Range Compression to a RGBF image and convert to 24-bit RGB.
		/// </summary>
		/// <param name="color_saturation">Color saturation (s parameter in the paper) in [0.4..0.6]</param>
		/// <param name="attenuation">Atenuation factor (beta parameter in the paper) in [0.8..0.9]</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoFattal02(double color_saturation, double attenuation)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoFattal02(dib, color_saturation, attenuation));
		}

		/// <summary>
		/// This method rotates a 1-, 4-, 8-bit greyscale or a 24-, 32-bit color image by means of 3 shears.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle)
		{
			ThrowOnDisposed();
			bool result = false;
			if (ColorDepth == 4)
			{
				result = ReplaceDib(FreeImage.Rotate4bit(dib, angle));
			}
			else
			{
				result = ReplaceDib(FreeImage.RotateClassic(dib, angle));
			}
			return result;
		}

		/// <summary>
		/// Rotates this FreeImageBitmap by the specified angle creating a new instance.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib;
			if (ColorDepth == 4)
			{
				newDib = FreeImage.Rotate4bit(dib, angle);
			}
			else
			{
				newDib = FreeImage.RotateClassic(dib, angle);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.RotateEx(dib, angle, xShift, yShift, xOrigin, yOrigin, useMask));
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline returning a new instance.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.RotateEx(
				dib, angle, xShift, yShift, xOrigin, yOrigin, useMask);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Perfoms an histogram transformation on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="lookUpTable">The lookup table (LUT).
		/// It's size is assumed to be 256 in length.</param>
		/// <param name="channel">The color channel to be transformed.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustCurve(byte[] lookUpTable, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustCurve(dib, lookUpTable, channel);
		}

		/// <summary>
		/// Performs gamma correction on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="gamma">The parameter represents the gamma value to use (gamma > 0).
		/// A value of 1.0 leaves the image alone, less than one darkens it, and greater than one lightens it.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustGamma(double gamma)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustGamma(dib, gamma);
		}

		/// <summary>
		/// Adjusts the brightness of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will make the image darker and greater than 0 will make the image brighter.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustBrightness(double percentage)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustBrightness(dib, percentage);
		}

		/// <summary>
		/// Adjusts the contrast of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will decrease the contrast and greater than 0 will increase the contrast of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustContrast(double percentage)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustContrast(dib, percentage);
		}

		/// <summary>
		/// Inverts each pixel data.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Invert()
		{
			ThrowOnDisposed();
			return FreeImage.Invert(dib);
		}

		/// <summary>
		/// Computes the image histogram.
		/// </summary>
		/// <param name="channel">Channel to compute from.</param>
		/// <param name="histogram">Array of integers containing the histogram.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool GetHistogram(FREE_IMAGE_COLOR_CHANNEL channel, out int[] histogram)
		{
			ThrowOnDisposed();
			histogram = new int[256];
			return FreeImage.GetHistogram(dib, histogram, channel);
		}

		/// <summary>
		/// Retrieves the red, green, blue or alpha channel of a 24- or 32-bit image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Insert a 8-bit dib into a 24- or 32-bit image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.SetChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Retrieves the real part, imaginary part, magnitude or phase of a complex image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetComplexChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetComplexChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Set the real or imaginary part of a complex image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetComplexChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.SetComplexChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Copy a sub part of this FreeImageBitmap.
		/// </summary>
		/// <param name="rect">The subpart to copy.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(Rectangle rect)
		{
			ThrowOnDisposed();
			return Copy(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Copy a sub part of this FreeImageBitmap.
		/// </summary>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(int left, int top, int right, int bottom)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Copy(dib, left, top, right, bottom);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with the current image.
		/// The bit depth of 'bitmap' must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to paste into this instance.</param>
		/// <param name="left">Specifies the left position of the sub image.</param>
		/// <param name="top">Specifies the top position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, int left, int top, int alpha)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.Paste(dib, bitmap.dib, left, top, alpha);
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with the current image.
		/// The bit depth of 'bitmap' must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to paste into this instance.</param>
		/// <param name="point">Specifies the position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, Point point, int alpha)
		{
			ThrowOnDisposed();
			return Paste(bitmap, point.X, point.Y, alpha);
		}

		/// <summary>
		/// This method composite a transparent foreground image against a single background color or
		/// against a background image.
		/// In case 'useBitmapBackground' is false and 'applicationBackground' and 'bitmapBackGround'
		/// are null, a checkerboard will be used as background.
		/// </summary>
		/// <param name="useBitmapBackground">When true the background of this instance is used
		/// if it contains one.</param>
		/// <param name="applicationBackground">Backgroundcolor used in case 'useBitmapBackground' is false
		/// and 'applicationBackground' is not null.</param>
		/// <param name="bitmapBackGround">Background used in case 'useBitmapBackground' is false and 
		/// 'applicationBackground' is null.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Composite(bool useBitmapBackground, Color? applicationBackground, FreeImageBitmap bitmapBackGround)
		{
			ThrowOnDisposed();
			bitmapBackGround.ThrowOnDisposed();
			RGBQUAD? rgb = applicationBackground;
			return ReplaceDib(
				FreeImage.Composite(
					dib,
					useBitmapBackground,
					rgb.HasValue ? new RGBQUAD[] { rgb.Value } : null,
					bitmapBackGround.dib));
		}

		/// <summary>
		/// Applies the alpha value of each pixel to its color components.
		/// The aplha value stays unchanged.
		/// Only works with 32-bits color depth.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool PreMultiplyWithAlpha()
		{
			ThrowOnDisposed();
			return FreeImage.PreMultiplyWithAlpha(dib);
		}

		/// <summary>
		/// Solves a Poisson equation, remap result pixels to [0..1] and returns the solution.
		/// </summary>
		/// <param name="ncycle">Number of cycles in the multigrid algorithm (usually 2 or 3)</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool MultigridPoissonSolver(int ncycle)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.MultigridPoissonSolver(dib, ncycle));
		}

		/// <summary>
		/// Adjusts an image's brightness, contrast and gamma as well as it may
		/// optionally invert the image within a single operation.
		/// </summary>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para>
		/// This parameter must not be zero or smaller than zero.
		/// If so, it will be ignored and no gamma correction will be performed on the image.</param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustColors(double brightness, double contrast, double gamma, bool invert)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustColors(dib, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Applies color mapping for one or several colors on a 1-, 4- or 8-bit
		/// palletized or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="srccolors">Array of colors to be used as the mapping source.</param>
		/// <param name="dstcolors">Array of colors to be used as the mapping destination.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <param name="swap">If true, source and destination colors are swapped, that is,
		/// each destination color is also mapped to the corresponding source color.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyColorMapping(RGBQUAD[] srccolors, RGBQUAD[] dstcolors, bool ignore_alpha, bool swap)
		{
			ThrowOnDisposed();
			if (srccolors == null)
			{
				throw new ArgumentNullException("srccolors");
			}
			if (dstcolors == null)
			{
				throw new ArgumentNullException("dstcolors");
			}
			if (srccolors.Length != dstcolors.Length)
			{
				throw new ArgumentException("srccolors and dstcolors must have the same length.");
			}
			return FreeImage.ApplyColorMapping(dib, srccolors, dstcolors, (uint)srccolors.Length, ignore_alpha, swap);
		}

		/// <summary>
		/// Swaps two specified colors on a 1-, 4- or 8-bit palletized
		/// or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="color_a">One of the two colors to be swapped.</param>
		/// <param name="color_b">The other of the two colors to be swapped.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapColors(RGBQUAD color_a, RGBQUAD color_b, bool ignore_alpha)
		{
			ThrowOnDisposed();
			return FreeImage.SwapColors(dib, ref color_a, ref color_b, ignore_alpha);
		}

		/// <summary>
		/// Applies palette index mapping for one or several indices
		/// on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="srcindices">Array of palette indices to be used as the mapping source.</param>
		/// <param name="dstindices">Array of palette indices to be used as the mapping destination.</param>
		/// <param name="count">The number of palette indices to be mapped. This is the size of both
		/// srcindices and dstindices</param>
		/// <param name="swap">If true, source and destination palette indices are swapped, that is,
		/// each destination index is also mapped to the corresponding source index.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyPaletteIndexMapping(byte[] srcindices, byte[] dstindices, uint count, bool swap)
		{
			ThrowOnDisposed();
			if (srcindices == null)
			{
				throw new ArgumentNullException("srcindices");
			}
			if (dstindices == null)
			{
				throw new ArgumentNullException("dstindices");
			}
			if (srcindices.Length != dstindices.Length)
			{
				throw new ArgumentException("srcindices and dstindices must have the same length.");
			}
			return FreeImage.ApplyPaletteIndexMapping(dib, srcindices, dstindices, (uint)srcindices.Length, swap);
		}

		/// <summary>
		/// Swaps two specified palette indices on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="index_a">One of the two palette indices to be swapped.</param>
		/// <param name="index_b">The other of the two palette indices to be swapped.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapPaletteIndices(byte index_a, byte index_b)
		{
			ThrowOnDisposed();
			return FreeImage.SwapPaletteIndices(dib, ref index_a, ref index_b);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return CreateICCProfile(data, data.Length);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <param name="size">The number of bytes of 'data' to use.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data, int size)
		{
			ThrowOnDisposed();
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return FreeImage.CreateICCProfileEx(dib, data, size);
		}

		#endregion

		#region Static functions

		/// <summary>
		/// Returns a value that indicates whether the pixel format for this image contains alpha information.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat to test.</param>
		/// <returns>true if pixfmt contains alpha information; otherwise, false.</returns>
		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsAlphaPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 32 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat to test.</param>
		/// <returns>true if pixfmt is canonical; otherwise, false.</returns>
		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsCanonicalPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 64 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat enumeration to test.</param>
		/// <returns>true if pixfmt is extended; otherwise, false.</returns>
		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsExtendedPixelFormat(pixfmt);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a Windows handle to an icon.
		/// </summary>
		/// <param name="hicon">A handle to an icon.</param>
		/// <returns>The FreeImageBitmap that this method creates.</returns>
		public static FreeImageBitmap FromHicon(IntPtr hicon)
		{
			using (Bitmap bitmap = Bitmap.FromHicon(hicon))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified Windows resource.
		/// </summary>
		/// <param name="hinstance">A handle to an instance of the executable
		/// file that contains the resource.</param>
		/// <param name="bitmapName">A string containing the name of the resource bitmap.</param>
		/// <returns>The FreeImageBitmap that this method creates.</returns>
		public static FreeImageBitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			using (Bitmap bitmap = Bitmap.FromResource(hinstance, bitmapName))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates an FreeImageImage from the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file
		/// from which to create the FreeImageBitmap.</param>
		/// <returns>The FreeImageImage this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates an Image from the specified file
		/// using embedded color management information in that file.
		/// </summary>
		/// <param name="filename">A string that contains the
		/// name of the file from which to create the FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a handle to a GDI bitmap.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the FreeImageBitmap.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap)
		{
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.CreateFromHbitmap(hbitmap, IntPtr.Zero);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a handle to a GDI bitmap and a handle to a GDI palette.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the FreeImageBitmap.</param>
		/// <param name="hpalette">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
		{
			return FromHbitmap(hbitmap);
		}

		/// <summary>
		/// Frees a bitmap handle.
		/// </summary>
		/// <param name="hbitmap">Handle to a bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool FreeHbitmap(IntPtr hbitmap)
		{
			return FreeImage.FreeHbitmap(hbitmap);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream)
		{
			try
			{
				return new FreeImageBitmap(stream);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <param name="validateImageData">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Returns the color depth, in number of bits per pixel,
		/// of the specified pixel format.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat member that specifies
		/// the format for which to find the size.</param>
		/// <returns>The color depth of the specified pixel format.</returns>
		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			return Bitmap.GetPixelFormatSize(pixfmt);
		}

		/// <summary>
		/// Performs a lossless rotation or flipping on a JPEG file.
		/// </summary>
		/// <param name="source">Source file.</param>
		/// <param name="destination">Destination file; can be the source file; will be overwritten.</param>
		/// <param name="operation">The operation to apply.</param>
		/// <param name="perfect">To avoid lossy transformation, you can set the perfect parameter to true.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool JPEGTransform(string source, string destination, FREE_IMAGE_JPEG_OPERATION operation, bool perfect)
		{
			return FreeImage.JPEGTransform(source, destination, operation, perfect);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="rect">Specifies the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, Rectangle rect)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return JPEGCrop(source, destination, rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, int left, int top, int right, int bottom)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return FreeImage.JPEGCrop(source, destination, left, top, right, bottom);
		}

		/// <summary>
		/// Converts a X11 color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupX11Color(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupX11Color(color, out red, out green, out blue);
		}

		/// <summary>
		/// Converts a SVG color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupSVGColor(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupSVGColor(color, out red, out green, out blue);
		}

		/// <summary>
		/// Creates a lookup table to be used with AdjustCurve() which
		/// may adjusts brightness and contrast, correct gamma and invert the image with a
		/// single call to AdjustCurve().
		/// </summary>
		/// <param name="lookUpTable">Output lookup table to be used with AdjustCurve().
		/// The size of 'LUT' is assumed to be 256.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para></param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>The number of adjustments applied to the resulting lookup table
		/// compared to a blind lookup table.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="lookUpTable"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="lookUpTable.Length"/> is not 256.</exception>
		public static int GetAdjustColorsLookupTable(byte[] lookUpTable, double brightness, double contrast, double gamma, bool invert)
		{
			if (lookUpTable == null)
			{
				throw new ArgumentNullException("lookUpTable");
			}
			if (lookUpTable.Length != 256)
			{
				throw new ArgumentException("lookUpTable");
			}
			return FreeImage.GetAdjustColorsLookupTable(lookUpTable, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Adds a specified frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="bitmap">A FreeImageBitmap that contains the frame to add.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filename"/> or <paramref name="bitmap"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		internal static void SaveAdd(
			string filename,
			FreeImageBitmap bitmap,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			bitmap.ThrowOnDisposed();
			if (bitmap.dib.IsNull)
			{
				throw new Exception();
			}

			FIMULTIBITMAP mpBitmap =
				FreeImage.OpenMultiBitmapEx(filename, ref format, loadFlags, false, false, true);

			if (mpBitmap.IsNull)
			{
				throw new Exception();
			}

			FreeImage.AppendPage(mpBitmap, bitmap.dib);

			if (!FreeImage.CloseMultiBitmap(mpBitmap, saveFlags))
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Returns a new instance of the 'PropertyItem' class which
		/// has no public accessible constructor.
		/// </summary>
		/// <returns>A new instace of 'PropertyItem'.</returns>
		public static PropertyItem CreateNewPropertyItem()
		{
			return FreeImage.CreatePropertyItem();
		}

		#endregion

		#region Helper functions

		/// <summary>
		/// Throws an exception in case the instance has already been disposed.
		/// </summary>
		protected void ThrowOnDisposed()
		{
			lock (lockObject)
			{
				if (!this.disposed)
				{
					return;
				}
			}
			throw new ObjectDisposedException(ToString());
		}

		/// <summary>
		/// Tries to replace the wrapped FIBITMAP with a new one.
		/// In case the new dib is null or the same as the already
		/// wrapped one, nothing will be changed and the result will
		/// be false.
		/// Otherwise the wrapped FIBITMAP will be unloaded and replaced.
		/// </summary>
		/// <param name="newDib">The new dib.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		protected bool ReplaceDib(FIBITMAP newDib)
		{
			bool result = false;
			if (dib != newDib && (!newDib.IsNull))
			{
				UnloadDib();
				dib = newDib;
				result = true;
			}
			return result;
		}

		/// <summary>
		/// Unloads currently wrapped FIBITMAP or unlocks the locked page
		/// in case it came from a multipaged bitmap.
		/// </summary>
		protected void UnloadDib()
		{
			if (mdib.IsNull || FreeImage.GetLockedPageCount(mdib) == 0)
			{
				FreeImage.UnloadEx(ref dib);
			}
			else if (!dib.IsNull)
			{
				FreeImage.UnlockPage(mdib, dib, false);
				dib = 0;
			}
		}

		#endregion

		#region Interfaces

		/// <summary>
		/// Helper class to store informations for 'SaveAdd'.
		/// </summary>
		private class SaveInformation : ICloneable
		{
			public string filename = null;
			public FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			public FREE_IMAGE_LOAD_FLAGS loadFlags = FREE_IMAGE_LOAD_FLAGS.DEFAULT;
			public FREE_IMAGE_SAVE_FLAGS saveFlags = FREE_IMAGE_SAVE_FLAGS.DEFAULT;

			public object Clone()
			{
				return base.MemberwiseClone();
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Clone(dib);
			if (!dib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
				result.saveInformation = saveInformation.Clone() as SaveInformation;
				result.tag = tag;
				result.originalFormat = originalFormat;
			}
			return result;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">If true managed ressources are released.</param>
		protected virtual void Dispose(bool disposing)
		{
			// Only clean up once
			lock (lockObject)
			{
				if (disposed)
				{
					return;
				}
				disposed = true;
			}

			// Clean up managed resources
			if (disposing)
			{
				tag = null;
			}

			// Clean up unmanaged resources
			UnloadDib();
			FreeImage.CloseMultiBitmapEx(ref mdib);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		/// <exception cref="ArgumentException">The bitmaps's type is not supported.</exception>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetScanlines().GetEnumerator();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ThrowOnDisposed();
			using (MemoryStream memory = new MemoryStream(DataSize))
			{
				if (!FreeImage.SaveToStream(ref dib, memory, FREE_IMAGE_FORMAT.FIF_TIFF, FREE_IMAGE_SAVE_FLAGS.TIFF_LZW, false))
				{
					throw new SerializationException();
				}
				memory.Capacity = (int)memory.Length;
				info.AddValue("Bitmap Data", memory.GetBuffer());
			}
		}

		#endregion
	}
}