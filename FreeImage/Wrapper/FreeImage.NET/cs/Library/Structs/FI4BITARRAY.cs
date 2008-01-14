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
	/// The structure wraps all operations needed to work with an array of FI4BITs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI4BITARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FI4BITARRAY : IComparable, IComparable<FI4BITARRAY>, IEnumerable, IEquatable<FI4BITARRAY>
	{
		readonly byte* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FI4BITARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FI4BITARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (byte*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FI4BITARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 4bpp.</exception>
		public FI4BITARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if ((scanline < 0) || scanline >= FreeImage.GetHeight(dib))
			{
				throw new ArgumentOutOfRangeException("scanline");
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 4)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (byte*)FreeImage.GetScanLine(dib, scanline);
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
		/// Gets or sets the palette-index at the given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
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
		/// Returns the palette-index at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetIndex(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if ((index % 2) == 0)
			{
				return (byte)(baseAddress[index / 2] >> 4);
			}
			else
			{
				return (byte)(baseAddress[index / 2] & 0x0F);
			}
		}

		/// <summary>
		/// Sets the palette-index at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <param name="value">The new data.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetIndex(int index, byte value)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if ((index % 2) == 0)
			{
				baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0x0F) | (value << 4));
			}
			else
			{
				baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0xF0) | (value & 0x0F));
			}
		}

		/// <summary>
		/// Returns the palette-index at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		internal unsafe byte GetIndexUnsafe(int index)
		{
			if ((index % 2) == 0)
			{
				return (byte)(baseAddress[index / 2] >> 4);
			}
			else
			{
				return (byte)(baseAddress[index / 2] & 0x0F);
			}
		}

		/// <summary>
		/// Sets the palette-index at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <param name="value">The new data.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		internal unsafe void SetIndexUnsafe(int index, byte value)
		{
			if ((index % 2) == 0)
			{
				baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0x0F) | (value << 4));
			}
			else
			{
				baseAddress[index / 2] = (byte)((baseAddress[index / 2] & 0xF0) | (value & 0x0F));
			}
		}

		/// <summary>
		/// Returns an array of byte.
		/// In each byte the lower 4 bits are representing the value (0x0F).
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte[] Data
		{
			get
			{
				byte[] result = new byte[length];
				for (int i = 0; i < length; i++)
				{
					result[i] = GetIndex(i);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				for (int i = 0; i < length; i++)
				{
					SetIndex(i, value[i]);
				}
			}
		}

		public static bool operator ==(FI4BITARRAY value1, FI4BITARRAY value2)
		{
			byte[] array1 = value1.Data;
			byte[] array2 = value2.Data;
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator !=(FI4BITARRAY value1, FI4BITARRAY value2)
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
			if (obj is FI4BITARRAY)
			{
				return CompareTo((FI4BITARRAY)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI4BITARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FI4BITARRAY array;
			private int index = -1;

			public Enumerator(FI4BITARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if ((index >= 0) && (index < array.length))
					{
						return array.GetIndex(index);
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
		public bool Equals(FI4BITARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}