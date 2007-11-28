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
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
	// Bitmaps are made up of different structures.
	// Some bitmaps have palettes that define the used colors and the bitmaps real
	// data links to the palette index.
	// Others don't have a palette and each pixel is stored directly.
	//
	// No matter which type of bitmap is accessed FreeImage provides pointers to
	// beginning of a structure and its length.
	// In unmanaged code pointers would be used to access the data. In .NET
	// unsafe code is needed to that.
	//
	// RGBQUAD, RGBTRIPLE, FIRGB16, FIRGBA16, FIRGBF and FIRGBAF represent
	// the structures that bitmaps use to store their data.
	// Its up to the coder to choose the right one.
	//
	// Each structure can be converted from and to the .NETs structure 'Color'.

	/// <summary>
	/// The RGBQUAD structure describes a color consisting of relative intensities of red, green, and blue
	/// combined with an alpha factor. Each color is using 1 byte of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RGBQUAD : IComparable, IComparable<RGBQUAD>, IEquatable<RGBQUAD>
	{
		public byte rgbBlue;
		public byte rgbGreen;
		public byte rgbRed;
		public byte rgbReserved;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public RGBQUAD(Color color)
		{
			rgbBlue = color.B;
			rgbGreen = color.G;
			rgbRed = color.R;
			rgbReserved = color.A;
		}

		public static bool operator ==(RGBQUAD value1, RGBQUAD value2)
		{
			return
				value1.rgbBlue == value2.rgbBlue &&
				value1.rgbGreen == value2.rgbGreen &&
				value1.rgbRed == value2.rgbRed &&
				value1.rgbReserved == value2.rgbReserved;
		}

		public static bool operator !=(RGBQUAD value1, RGBQUAD value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator RGBQUAD(Color color)
		{
			return new RGBQUAD(color);
		}

		public static implicit operator Color(RGBQUAD rgbquad)
		{
			return rgbquad.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					rgbReserved,
					rgbRed,
					rgbGreen,
					rgbBlue);
			}
			set
			{
				rgbRed = value.R;
				rgbGreen = value.G;
				rgbBlue = value.B;
				rgbReserved = value.A;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is RGBQUAD)
			{
				return CompareTo((RGBQUAD)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBQUAD other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(RGBQUAD other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return ((rgbBlue << 16) | (rgbGreen << 8) | (rgbRed));
		}
	}
}