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
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
	/// <summary>
	/// This structure contains information about the dimensions and color format of a device-independent bitmap (DIB).
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFOHEADER : IEquatable<BITMAPINFOHEADER>
	{
		/// <summary>
		/// Specifies the size of the structure, in bytes.
		/// </summary>
		public uint biSize;
		/// <summary>
		/// Specifies the width of the bitmap, in pixels.
		/// </summary>
		public int biWidth;
		/// <summary>
		/// Specifies the height of the bitmap, in pixels.
		/// If biHeight is positive, the bitmap is a bottom-up DIB and its origin is the lower left corner.
		/// If biHeight is negative, the bitmap is a top-down DIB and its origin is the upper left corner.
		/// If biHeight is negative, indicating a top-down DIB, biCompression must be either BI_RGB or BI_BITFIELDS.
		/// Top-down DIBs cannot be compressed.
		/// </summary>
		public int biHeight;
		/// <summary>
		/// Specifies the number of planes for the target device. This value must be set to 1.
		/// </summary>
		public ushort biPlanes;
		/// <summary>
		/// Specifies the number of bits per pixel.
		/// </summary>
		public ushort biBitCount;
		/// <summary>
		/// Specifies the type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
		/// </summary>
		public uint biCompression;
		/// <summary>
		/// Specifies the size, in bytes, of the image.
		/// </summary>
		public uint biSizeImage;
		/// <summary>
		/// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
		/// </summary>
		public int biXPelsPerMeter;
		/// <summary>
		/// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap
		/// </summary>
		public int biYPelsPerMeter;
		/// <summary>
		/// Specifies the number of color indexes in the color table that are actually used by the bitmap.
		/// </summary>
		public uint biClrUsed;
		/// <summary>
		/// Specifies the number of color indexes required for displaying the bitmap.
		/// If this value is zero, all colors are required.
		/// </summary>
		public uint biClrImportant;

		public static bool operator ==(BITMAPINFOHEADER value1, BITMAPINFOHEADER value2)
		{
			return !((value1.biSize != value2.biSize) ||
					(value1.biWidth != value2.biWidth) ||
					(value1.biHeight != value2.biHeight) ||
					(value1.biPlanes != value2.biPlanes) ||
					(value1.biBitCount != value2.biBitCount) ||
					(value1.biCompression != value2.biCompression) ||
					(value1.biSizeImage != value2.biSizeImage) ||
					(value1.biXPelsPerMeter != value2.biXPelsPerMeter) ||
					(value1.biYPelsPerMeter != value2.biYPelsPerMeter) ||
					(value1.biClrUsed != value2.biClrUsed) ||
					(value1.biClrImportant != value2.biClrImportant));
		}

		public static bool operator !=(BITMAPINFOHEADER value1, BITMAPINFOHEADER value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is BITMAPINFOHEADER)
			{
				return Equals((BITMAPINFOHEADER)obj);
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(BITMAPINFOHEADER other)
		{
			return this == other;
		}
	}
}