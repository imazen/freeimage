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
	/// The structure wraps all operations needed to work with an array of FI16RGBs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI16RGBARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public struct FI16RGBARRAY : IComparable, IComparable<FI16RGBARRAY>, IEnumerable, IEquatable<FI16RGBARRAY>
	{
		readonly uint baseAddress;
		readonly uint length;
		readonly BitSettings bitSettings;

		/// <summary>
		/// Creates an FIRGBFARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <param name="red_mask">Bitmask for the color red.</param>
		/// <param name="green_mask">Bitmask for the color green.</param>
		/// <param name="blue_mask">Bitmask for the color blue.</param>
		public FI16RGBARRAY(IntPtr baseAddress, uint length, ushort red_mask, ushort green_mask, ushort blue_mask)
		{
			if (baseAddress == IntPtr.Zero) throw new ArgumentNullException();
			this.baseAddress = (uint)baseAddress;
			this.length = length;
			bitSettings = GetBitSettings(red_mask, green_mask, blue_mask);
		}

		/// <summary>
		/// Creates an FIRGBFARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		public FI16RGBARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull) throw new ArgumentNullException();
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP) throw new ArgumentException("dib");
			if (FreeImage.GetBPP(dib) != 16) throw new ArgumentException("dib");
			baseAddress = (uint)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
			bitSettings = GetBitSettings(FreeImage.GetRedMask(dib), FreeImage.GetGreenMask(dib), FreeImage.GetBlueMask(dib));
		}

		/// <summary>
		/// Create a BitSettings structure from color masks
		/// </summary>
		private static BitSettings GetBitSettings(uint red_mask, uint green_mask, uint blue_mask)
		{
			return GetBitSettings((ushort)red_mask, (ushort)green_mask, (ushort)blue_mask);
		}

		/// <summary>
		/// Create a BitSettings structure from color masks
		/// </summary>
		private static BitSettings GetBitSettings(ushort red_mask, ushort green_mask, ushort blue_mask)
		{
			BitSettings bitSettings = new BitSettings();

			ushort temp;
			bitSettings.RED_MASK = red_mask;
			bitSettings.GREEN_MASK = green_mask;
			bitSettings.BLUE_MASK = blue_mask;

			bitSettings.RED_SHIFT = 0;
			temp = bitSettings.RED_MASK;
			while ((temp & 0x1) != 1)
			{
				temp >>= 1;
				bitSettings.RED_SHIFT++;
			}
			bitSettings.RED_MAX = temp;

			bitSettings.GREEN_SHIFT = 0;
			temp = bitSettings.GREEN_MASK;
			while ((temp & 0x1) != 1)
			{
				temp >>= 1;
				bitSettings.GREEN_SHIFT++;
			}
			bitSettings.GREEN_MAX = temp;

			bitSettings.BLUE_SHIFT = 0;
			temp = bitSettings.BLUE_MASK;
			while ((temp & 0x1) != 1)
			{
				temp >>= 1;
				bitSettings.BLUE_SHIFT++;
			}
			bitSettings.BLUE_MAX = temp;

			return bitSettings;
		}

		public static bool operator ==(FI16RGBARRAY value1, FI16RGBARRAY value2)
		{
			FI16RGB[] array1 = value1.Data;
			FI16RGB[] array2 = value2.Data;
			if (array1.Length != array2.Length)
				return false;
			for (int i = 0; i < array1.Length; i++)
				if (array1[i] != array2[i])
					return false;
			return true;
		}

		public static bool operator !=(FI16RGBARRAY value1, FI16RGBARRAY value2)
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
		/// Gets or sets the ushort value representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>Ushort value of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public FI16RGB this[int index]
		{
			get
			{
				if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
				return GetFI16RGB(index);
			}
			set
			{
				if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
				SetFI16RGB(index, value);
			}
		}

		/// <summary>
		/// Returns the color as an FI16RGB structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FI16RGB structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public FI16RGB GetFI16RGB(int index)
		{
			return new FI16RGB(GetUShort(index), bitSettings);
		}

		/// <summary>
		/// Sets the color at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public void SetFI16RGB(int index, FI16RGB color)
		{
			SetUShort(index, color.data);
		}

		/// <summary>
		/// Returns the ushort value of the index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An ushort value representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe ushort GetUShort(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return ((ushort*)baseAddress)[index];
		}

		/// <summary>
		/// Sets the ushort value at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe void SetUShort(int index, ushort color)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			((ushort*)baseAddress)[index] = color;
		}

		/// <summary>
		/// Extract bits from a value
		/// </summary>
		private unsafe byte GetColorComponent(int index, ushort mask, ushort shift, ushort max)
		{
			ushort value = ((ushort*)baseAddress + index)[0];
			value &= mask;
			value >>= shift;
			value = (byte)((value * 255) / max);
			return (byte)value;
		}

		/// <summary>
		/// Insert bits into a value
		/// </summary>
		private unsafe void SetColorComponent(int index, byte value, ushort mask, ushort shift, ushort max)
		{
			ushort invertMask = (ushort)(~mask);
			ushort orgValue = ((ushort*)baseAddress + index)[0];
			orgValue &= invertMask;
			ushort newValue = (ushort)(((ushort)value * max) / 255);
			newValue <<= shift;
			newValue |= orgValue;
			((ushort*)baseAddress + index)[0] = newValue;
		}

		/// <summary>
		/// Returns the data representing the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the red part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public byte GetRed(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return GetColorComponent(index, bitSettings.RED_MASK, bitSettings.RED_SHIFT, bitSettings.RED_MAX);
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public void SetRed(int index, byte red)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			SetColorComponent(index, red, bitSettings.RED_MASK, bitSettings.RED_SHIFT, bitSettings.RED_MAX);
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public byte GetGreen(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return GetColorComponent(index, bitSettings.GREEN_MASK, bitSettings.GREEN_SHIFT, bitSettings.GREEN_MAX);
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public void SetGreen(int index, byte green)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			SetColorComponent(index, green, bitSettings.GREEN_MASK, bitSettings.GREEN_SHIFT, bitSettings.GREEN_MAX);
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public byte GetBlue(int index)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			return GetColorComponent(index, bitSettings.BLUE_MASK, bitSettings.BLUE_SHIFT, bitSettings.BLUE_MAX);
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public void SetBlue(int index, byte blue)
		{
			if (index >= length || index < 0) throw new ArgumentOutOfRangeException();
			SetColorComponent(index, blue, bitSettings.BLUE_MASK, bitSettings.BLUE_SHIFT, bitSettings.BLUE_MAX);
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
			int red, green, blue;
			red = GetColorComponent(index, bitSettings.RED_MASK, bitSettings.RED_SHIFT, bitSettings.RED_MAX);
			green = GetColorComponent(index, bitSettings.GREEN_MASK, bitSettings.GREEN_SHIFT, bitSettings.GREEN_MAX);
			blue = GetColorComponent(index, bitSettings.BLUE_MASK, bitSettings.BLUE_SHIFT, bitSettings.BLUE_MAX);
			return Color.FromArgb(red, green, blue);
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
			uint value = 0;
			value |= (((uint)(((float)color.R / 255f) * (float)bitSettings.RED_MAX)) << bitSettings.RED_SHIFT);
			value |= (((uint)(((float)color.G / 255f) * (float)bitSettings.GREEN_MAX)) << bitSettings.GREEN_SHIFT);
			value |= (((uint)(((float)color.B / 255f) * (float)bitSettings.BLUE_MAX)) << bitSettings.BLUE_SHIFT);
			SetUShort(index, (ushort)value);
		}

		/// <summary>
		/// Returns an array of FI16RGB.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if index is greater or same as Length</exception>
		public unsafe FI16RGB[] Data
		{
			get
			{
				FI16RGB[] result = new FI16RGB[length];
				for (int i = 0; i < length; i++)
					result[i] = GetFI16RGB(i);
				return result;
			}
			set
			{
				if (value.Length != length) throw new ArgumentOutOfRangeException();
				for (int i = 0; i < length; i++)
					SetFI16RGB(i, value[i]);
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (!(obj is FI16RGBARRAY))
				throw new ArgumentException();
			return CompareTo((FI16RGBARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI16RGBARRAY other)
		{
			return this.baseAddress.CompareTo(other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FI16RGBARRAY array;
			private int index = -1;

			public Enumerator(FI16RGBARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
						return array.GetFI16RGB(index);
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
		public bool Equals(FI16RGBARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}
}