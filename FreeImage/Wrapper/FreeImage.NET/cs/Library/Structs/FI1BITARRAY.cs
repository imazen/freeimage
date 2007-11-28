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

namespace FreeImageAPI
{
	/// <summary>
	/// The structure wraps all operations needed to work with an array of FI1BITs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI1BITARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public struct FI1BITARRAY : IComparable, IComparable<FI1BITARRAY>, IEnumerable, IEquatable<FI1BITARRAY>
	{
		readonly uint baseAddress;
		readonly uint length;
		private const byte Zero = 0;
		private const byte One = 1;

		/// <summary>
		/// Creates an FI1BITARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		public FI1BITARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero) throw new ArgumentNullException();
			this.baseAddress = (uint)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FI1BITARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		public FI1BITARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull) throw new ArgumentNullException();
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP) throw new ArgumentException("dib");
			if (FreeImage.GetBPP(dib) != 1) throw new ArgumentException("dib");
			baseAddress = (uint)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		/// <summary>
		/// Gets the number of elements being wrapped.
		/// </summary>
		public uint Length
		{
			get { return length; }
		}

		/// <summary>
		/// Gets or sets the bit representing the index of the bitmaps palette at the given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public byte this[int index]
		{
			get
			{
				return GetIndex(index);
			}
			set
			{
				SetIndex(index, value);
			}
		}

		/// <summary>
		/// Returns the bit at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe byte GetIndex(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			byte mask = (byte)(1 << (7 - (index % 8)));
			return ((((byte*)baseAddress)[index / 8] & mask) > 0) ? FI1BITARRAY.One : FI1BITARRAY.Zero;
		}

		/// <summary>
		/// Sets the bit at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <param name="value">The new data.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetIndex(int index, byte value)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			int mask = 1 << (7 - (index % 8));
			if ((value & 0x01) > 0)
			{
				((byte*)baseAddress)[index / 8] |= (byte)mask;
			}
			else
			{
				((byte*)baseAddress)[index / 8] &= (byte)(~mask);
			}
		}

		/// <summary>
		/// Returns an array of byte.
		/// In each byte the lowest bit is representing the value (0x01).
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe byte[] Data
		{
			get
			{
				byte[] result = new byte[length];
				int mask = 0x80;
				int j = 0;

				for (int i = 0; i < length; i++)
				{
					result[i] = (byte)((((byte*)baseAddress)[j] & mask) > 0 ? 1 : 0);
					mask >>= 1;
					if (mask == 0)
					{
						mask = 0x80;
						j++;
					}
				}

				return result;
			}
			set
			{
				if (value.Length != length) throw new ArgumentOutOfRangeException();
				int buffer = 0;
				int mask = 0x80;
				int j = 0;

				for (int i = 0; i < length; i++)
				{
					if ((value[i] & 0x01) > 0)
						buffer |= mask;
					mask >>= 1;
					if (mask == 0)
					{
						((byte*)baseAddress)[j] = (byte)buffer;
						buffer = 0;
						mask = 0x80;
						j++;
					}
				}
				if ((length % 8) != 0)
				{
					((byte*)baseAddress)[j] = (byte)buffer;
				}
			}
		}

		public static bool operator ==(FI1BITARRAY value1, FI1BITARRAY value2)
		{
			byte[] array1 = value1.Data;
			byte[] array2 = value2.Data;
			if (array1.Length != array2.Length)
				return false;
			for (int i = 0; i < array1.Length; i++)
				if (array1[i] != array2[i])
					return false;
			return true;
		}

		public static bool operator !=(FI1BITARRAY value1, FI1BITARRAY value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FI1BITARRAY)
			{
				return CompareTo((FI1BITARRAY)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI1BITARRAY other)
		{
			return this.baseAddress.CompareTo(other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FI1BITARRAY array;
			private int index = -1;

			public Enumerator(FI1BITARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
						return array.GetIndex(index);
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				index++;
				if (index < (int)array.length)
					return true;
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
		public bool Equals(FI1BITARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}