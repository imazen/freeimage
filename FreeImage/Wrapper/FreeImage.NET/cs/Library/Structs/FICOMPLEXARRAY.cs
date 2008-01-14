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
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
	/// <summary>
	/// The structure wraps all operations needed to work with an array of FICOMPLEXs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FICOMPLEXARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FICOMPLEXARRAY : IComparable, IComparable<FICOMPLEXARRAY>, IEnumerable, IEquatable<FICOMPLEXARRAY>
	{
		readonly FICOMPLEX* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FICOMPLEXARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FICOMPLEXARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (FICOMPLEX*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FICOMPLEXARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_RGBAF.</exception>
		public FICOMPLEXARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if ((scanline < 0) || scanline >= FreeImage.GetHeight(dib))
			{
				throw new ArgumentOutOfRangeException("scanline");
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_COMPLEX)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (FICOMPLEX*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FICOMPLEXARRAY value1, FICOMPLEXARRAY value2)
		{
			if (value1.length != value2.length)
			{
				return false;
			}
			if (value1.baseAddress == value2.baseAddress)
			{
				return true;
			}
			return FreeImage.CompareMemory(
				value1.baseAddress,
				value2.baseAddress,
				sizeof(FICOMPLEX) * value1.length);
		}

		public static bool operator !=(FICOMPLEXARRAY value1, FICOMPLEXARRAY value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Gets the number of elements being wrapped.
		/// </summary>
		public uint Length
		{
			get { return length; }
		}

		/// <summary>
		/// Gets or sets the FICOMPLEX structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>FICOMPLEX structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FICOMPLEX this[int index]
		{
			get
			{
				if (index >= length || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return baseAddress[index];
			}
			set
			{
				if (index >= length || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				baseAddress[index] = value;
			}
		}

		/// <summary>
		/// Returns the color as an FICOMPLEX structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FICOMPLEX structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FICOMPLEX GetFICOMPLEX(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index];
		}

		/// <summary>
		/// Sets the color at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetFICOMPLEX(int index, FICOMPLEX color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index] = color;
		}

		/// <summary>
		/// Returns an array of FICOMPLEX.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FICOMPLEX[] Data
		{
			get
			{
				FICOMPLEX[] result = new FICOMPLEX[length];
				fixed (FICOMPLEX* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(FICOMPLEX) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (FICOMPLEX* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(FICOMPLEX) * length);
				}
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (!(obj is FICOMPLEXARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FICOMPLEXARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FICOMPLEXARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FICOMPLEXARRAY array;
			private int index = -1;

			public Enumerator(FICOMPLEXARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if ((index >= 0) && (index < array.length))
					{
						return array.GetFICOMPLEX(index);
					}
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				if ((index + 1) < (int)array.length)
				{
					index++;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FICOMPLEXARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}