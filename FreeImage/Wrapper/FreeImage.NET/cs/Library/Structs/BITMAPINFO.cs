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
	/// This structure defines the dimensions and color information of a Windows-based device-independent bitmap (DIB).
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFO : IEquatable<BITMAPINFO>
	{
		/// <summary>
		/// Specifies a BITMAPINFOHEADER structure that contains information about the dimensions of color format.
		/// </summary>
		public BITMAPINFOHEADER bmiHeader;
		/// <summary>
		/// An array of RGBQUAD. The elements of the array that make up the color table.
		/// </summary>
		public RGBQUAD[] bmiColors;

		public static bool operator ==(BITMAPINFO value1, BITMAPINFO value2)
		{
			if (value1.bmiHeader != value2.bmiHeader)
				return false;
			if (value1.bmiColors.Length != value2.bmiColors.Length)
				return false;
			for (int i = 0; i < value1.bmiColors.Length; i++)
				if (value1.bmiColors[i] != value2.bmiColors[i])
					return false;
			return true;
		}

		public static bool operator !=(BITMAPINFO value1, BITMAPINFO value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(BITMAPINFO other)
		{
			return this == other;
		}
	}
}