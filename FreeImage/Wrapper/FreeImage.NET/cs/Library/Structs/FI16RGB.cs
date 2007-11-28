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
using System.Drawing;

namespace FreeImageAPI
{
	/// <summary>
	/// The FI16RGB structure describes a color consisting of relative intensities of red, green, and blue.
	/// The structure provides 16 bit which can be dynamically spread across the three colors.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public sealed class FI16RGB : IComparable, IComparable<FI16RGB>, IEquatable<FI16RGB>
	{
		public ushort data;
		private readonly BitSettings bitSettings;

		/// <summary>
		/// For internal use only.
		/// </summary>
		internal FI16RGB(ushort color, BitSettings bitSettings)
		{
			this.data = color;
			this.bitSettings = bitSettings;
		}

		public static bool operator ==(FI16RGB value1, FI16RGB value2)
		{
			if (value1.bitSettings.RED_MASK != value2.bitSettings.RED_MASK)
				return false;
			if (value1.bitSettings.GREEN_MASK != value2.bitSettings.GREEN_MASK)
				return false;
			if (value1.bitSettings.BLUE_MASK != value2.bitSettings.BLUE_MASK)
				return false;
			int MASK =
				(value1.bitSettings.RED_MASK |
				value1.bitSettings.GREEN_MASK |
				value1.bitSettings.BLUE_MASK);
			return (value1.data & MASK) == (value2.data & MASK);
		}

		public static bool operator !=(FI16RGB value1, FI16RGB value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				int red, green, blue;
				red = (255 * GetColorComponent(bitSettings.RED_MASK, bitSettings.RED_SHIFT)) / bitSettings.RED_MAX;
				green = (255 * GetColorComponent(bitSettings.GREEN_MASK, bitSettings.GREEN_SHIFT)) / bitSettings.GREEN_MAX;
				blue = (255 * GetColorComponent(bitSettings.BLUE_MASK, bitSettings.BLUE_SHIFT)) / bitSettings.BLUE_MAX;
				return Color.FromArgb(red, green, blue);
			}
			set
			{
				uint val = 0;
				val |= (((uint)(((float)value.R / 255f) * (float)bitSettings.RED_MAX)) << bitSettings.RED_SHIFT);
				val |= (((uint)(((float)value.G / 255f) * (float)bitSettings.GREEN_MAX)) << bitSettings.GREEN_SHIFT);
				val |= (((uint)(((float)value.B / 255f) * (float)bitSettings.BLUE_MAX)) << bitSettings.BLUE_SHIFT);
				data = (ushort)val;
			}
		}

		/// <summary>
		/// Extract bits from a value.
		/// </summary>
		private byte GetColorComponent(ushort mask, ushort shift)
		{
			ushort value = data;
			value &= mask;
			value >>= shift;
			return (byte)value;
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FI16RGB)
			{
				return CompareTo((FI16RGB)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI16RGB other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FI16RGB other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return data;
		}
	}
}