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
	/// <summary>
	/// The RGBTRIPLE structure describes a color consisting of relative intensities of red, green, and blue.
	/// Each color is using 1 byte of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RGBTRIPLE : IComparable, IComparable<RGBTRIPLE>, IEquatable<RGBTRIPLE>
	{
		public byte rgbtBlue;
		public byte rgbtGreen;
		public byte rgbtRed;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public RGBTRIPLE(Color color)
		{
			rgbtBlue = color.B;
			rgbtGreen = color.G;
			rgbtRed = color.R;
		}

		public static bool operator ==(RGBTRIPLE value1, RGBTRIPLE value2)
		{
			return
				value1.rgbtBlue == value2.rgbtBlue &&
				value1.rgbtGreen == value2.rgbtGreen &&
				value1.rgbtRed == value2.rgbtRed;
		}

		public static bool operator !=(RGBTRIPLE value1, RGBTRIPLE value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator RGBTRIPLE(Color color)
		{
			return new RGBTRIPLE(color);
		}

		public static implicit operator Color(RGBTRIPLE rgbtripple)
		{
			return rgbtripple.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					rgbtRed,
					rgbtGreen,
					rgbtBlue);
			}
			set
			{
				rgbtBlue = value.B;
				rgbtGreen = value.G;
				rgbtRed = value.R;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is RGBTRIPLE)
			{
				return CompareTo((RGBTRIPLE)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBTRIPLE other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(RGBTRIPLE other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return ((rgbtBlue << 16) | (rgbtGreen << 8) | (rgbtRed));
		}
	}
}