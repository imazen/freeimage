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
	/// The structure wraps all operations needed to work with an array of FIRGBA16s.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FIRGBA16ARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FIRGBA16ARRAY : IComparable, IComparable<FIRGBA16ARRAY>, IEnumerable, IEquatable<FIRGBA16ARRAY>
	{
		readonly FIRGBA16* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FIRGBA16ARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FIRGBA16ARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (FIRGBA16*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FIRGBA16ARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_RGBA16.</exception>
		public FIRGBA16ARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_RGBA16)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (FIRGBA16*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FIRGBA16ARRAY value1, FIRGBA16ARRAY value2)
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
				sizeof(FIRGBA16) * value1.length);
		}

		public static bool operator !=(FIRGBA16ARRAY value1, FIRGBA16ARRAY value2)
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
		/// Gets or sets the FIRGBA16 structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>FIRGBA16 structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBA16 this[int index]
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
		/// Returns the color as an FIRGBA16 structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FIRGBA16 structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBA16 GetFIRGBA16(int index)
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
		public unsafe void SetFIRGBA16(int index, FIRGBA16 color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index] = color;
		}

		/// <summary>
		/// Returns the data representing the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the red part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe ushort GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((ushort*)(baseAddress + index))[0];
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetRed(int index, ushort red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((ushort*)(baseAddress + index))[0] = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe ushort GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((ushort*)(baseAddress + index))[1];
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetGreen(int index, ushort green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((ushort*)(baseAddress + index))[1] = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe ushort GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((ushort*)(baseAddress + index))[2];
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetBlue(int index, ushort blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((ushort*)(baseAddress + index))[2] = blue;
		}

		/// <summary>
		/// Returns the data representing the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the alpha part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe ushort GetAlpha(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((ushort*)(baseAddress + index))[3];
		}

		/// <summary>
		/// Sets the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="alpha">The new alpha part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetAlpha(int index, ushort alpha)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((ushort*)(baseAddress + index))[3] = alpha;
		}

		/// <summary>
		/// Returns the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The color at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public Color GetColor(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return GetFIRGBA16(index).color;
		}

		/// <summary>
		/// Sets the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public void SetColor(int index, Color color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			SetFIRGBA16(index, new FIRGBA16(color));
		}

		/// <summary>
		/// Returns an array of FIRGBA16.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBA16[] Data
		{
			get
			{
				FIRGBA16[] result = new FIRGBA16[length];
				fixed (FIRGBA16* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(FIRGBA16) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (FIRGBA16* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(FIRGBA16) * length);
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
			if (!(obj is FIRGBA16ARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBA16ARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBA16ARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FIRGBA16ARRAY array;
			private int index = -1;

			public Enumerator(FIRGBA16ARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetFIRGBA16(index);
					}
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				index++;
				if (index < (int)array.length)
				{
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
		public bool Equals(FIRGBA16ARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}