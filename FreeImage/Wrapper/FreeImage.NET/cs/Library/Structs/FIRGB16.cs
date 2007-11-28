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
	/// The FIRGB16 structure describes a color consisting of relative intensities of red, green, and blue.
	/// Each color is using 2 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGB16 : IComparable, IComparable<FIRGB16>, IEquatable<FIRGB16>
	{
		public ushort red;
		public ushort green;
		public ushort blue;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public FIRGB16(Color color)
		{
			red = (ushort)(color.R * 256);
			green = (ushort)(color.G * 256);
			blue = (ushort)(color.B * 256);
		}

		public static bool operator ==(FIRGB16 value1, FIRGB16 value2)
		{
			return
				value1.blue == value2.blue &&
				value1.green == value2.green &&
				value1.red == value2.red;
		}

		public static bool operator !=(FIRGB16 value1, FIRGB16 value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator FIRGB16(Color color)
		{
			return new FIRGB16(color);
		}

		public static implicit operator Color(FIRGB16 firgb16)
		{
			return firgb16.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					red / 256,
					green / 256,
					blue / 256);
			}
			set
			{
				red = (ushort)(value.R * 256);
				green = (ushort)(value.G * 256);
				blue = (ushort)(value.B * 256);
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIRGB16)
			{
				return CompareTo((FIRGB16)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGB16 other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIRGB16 other)
		{
			return this == other;
		}
	}
}