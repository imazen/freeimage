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
	/// Handle to FIBITMAP structure
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIBITMAP : IComparable, IComparable<FIBITMAP>, IEquatable<FIBITMAP>
	{
		private IntPtr data;
		public FIBITMAP(int ptr) { data = new IntPtr(ptr); }
		public FIBITMAP(IntPtr ptr) { data = ptr; }

		public static bool operator !=(FIBITMAP value1, FIBITMAP value2)
		{
			return value1.data != value2.data;
		}

		public static bool operator ==(FIBITMAP value1, FIBITMAP value2)
		{
			return value1.data == value2.data;
		}

		public static implicit operator FIBITMAP(int ptr)
		{
			return new FIBITMAP(ptr);
		}

		public static implicit operator int(FIBITMAP fi)
		{
			return fi.data.ToInt32();
		}

		public static implicit operator FIBITMAP(IntPtr ptr)
		{
			return new FIBITMAP(ptr);
		}

		public static implicit operator IntPtr(FIBITMAP fi)
		{
			return fi.data;
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer.
		/// </summary>
		public bool IsNull { get { return data == IntPtr.Zero; } }

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return String.Format("0x{0:X}", (uint)data);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Object.</returns>
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is FIBITMAP)
			{
				return Equals((FIBITMAP)obj);
			}
			return false;
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIBITMAP)
			{
				return CompareTo((FIBITMAP)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIBITMAP other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIBITMAP other)
		{
			return this == other;
		}
	}
}