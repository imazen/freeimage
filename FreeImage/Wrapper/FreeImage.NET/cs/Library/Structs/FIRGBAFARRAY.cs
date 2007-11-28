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
	/// The structure wraps all operations needed to work with an array of FIRGBAFs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FIRGBAFARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public struct FIRGBAFARRAY : IComparable, IComparable<FIRGBAFARRAY>, IEnumerable, IEquatable<FIRGBAFARRAY>
	{
		readonly uint baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FIRGBAFARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		public FIRGBAFARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero) throw new ArgumentNullException();
			this.baseAddress = (uint)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FIRGBAFARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		public FIRGBAFARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull) throw new ArgumentNullException();
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP) throw new ArgumentException("dib");
			if (FreeImage.GetBPP(dib) != 128) throw new ArgumentException("dib");
			baseAddress = (uint)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FIRGBAFARRAY value1, FIRGBAFARRAY value2)
		{
			FIRGBAF[] array1 = value1.Data;
			FIRGBAF[] array2 = value2.Data;
			if (array1.Length != array2.Length)
				return false;
			for (int i = 0; i < array1.Length; i++)
				if (array1[i] != array2[i])
					return false;
			return true;
		}

		public static bool operator !=(FIRGBAFARRAY value1, FIRGBAFARRAY value2)
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
		/// Gets or sets the FIRGBAF structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>FIRGBAF structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe FIRGBAF this[int index]
		{
			get
			{
				if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
				return ((FIRGBAF*)baseAddress)[index];
			}
			set
			{
				if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
				((FIRGBAF*)baseAddress)[index] = value;
			}
		}

		/// <summary>
		/// Returns the color as an FIRGBAF structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FIRGBAF structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe FIRGBAF GetFIRGBAF(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((FIRGBAF*)baseAddress)[index];
		}

		/// <summary>
		/// Sets the color at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetFIRGBAF(int index, FIRGBAF color)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((FIRGBAF*)baseAddress)[index] = color;
		}

		/// <summary>
		/// Returns the data representing the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the red part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe float GetRed(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((float*)((FIRGBAF*)baseAddress + index))[0];
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetRed(int index, float red)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((float*)((FIRGBAF*)baseAddress + index))[0] = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe float GetGreen(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((float*)((FIRGBAF*)baseAddress + index))[1];
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetGreen(int index, float green)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((float*)((FIRGBAF*)baseAddress + index))[1] = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe float GetBlue(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((float*)((FIRGBAF*)baseAddress + index))[2];
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetBlue(int index, float blue)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((float*)((FIRGBAF*)baseAddress + index))[2] = blue;
		}

		/// <summary>
		/// Returns the data representing the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the alpha part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe float GetAlpha(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((float*)((FIRGBAF*)baseAddress + index))[3];
		}

		/// <summary>
		/// Sets the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="alpha">The new alpha part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetAlpha(int index, float alpha)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((float*)((FIRGBAF*)baseAddress + index))[3] = alpha;
		}

		/// <summary>
		/// Returns the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The color at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public Color GetColor(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return GetFIRGBAF(index).color;
		}

		/// <summary>
		/// Sets the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public void SetColor(int index, Color color)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			SetFIRGBAF(index, new FIRGBAF(color));
		}

		/// <summary>
		/// Returns an array of FIRGBAF.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe FIRGBAF[] Data
		{
			get
			{
				FIRGBAF[] result = new FIRGBAF[length];
				for (int i = 0; i < length; i++)
					result[i] = ((FIRGBAF*)baseAddress)[i];
				return result;
			}
			set
			{
				if (value.Length != length) throw new ArgumentOutOfRangeException();
				for (int i = 0; i < length; i++)
					((FIRGBAF*)baseAddress)[i] = value[i];
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (!(obj is FIRGBAFARRAY))
				throw new ArgumentException();
			return CompareTo((FIRGBAFARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBAFARRAY other)
		{
			return this.baseAddress.CompareTo(other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FIRGBAFARRAY array;
			private int index = -1;

			public Enumerator(FIRGBAFARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
						return array.GetFIRGBAF(index);
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
		public bool Equals(FIRGBAFARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}