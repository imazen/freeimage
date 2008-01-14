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
// To build the project without VS use the following commandline:
// "csc.exe /out:FreeImageNET.dll /target:library /doc:FreeImageNET.XML /debug- /o /nowarn:659,660,661,1591 /unsafe+ /filealign:512 FreeImage.cs"
// ==========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

/////////////////////////////////////////////////////
//                                                 //
//              FreeImage.h import                 //
//                                                 //
/////////////////////////////////////////////////////

namespace FreeImageAPI
{
	#region Structs

	// FIBITMAP, FIMULTIBITMAP, FIMEMORY, FIMETADATA and FITAG are wrapped pointers.
	// The strcutures contain a single IntPtr wrapping the unmanaged memory address.
	// The structures implement conversions between Int and IntPtr and the 'Equals'
	// method.
	// The 'IsNull' property allows a null-pointer check.

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

	/// <summary>
	/// Handle to a multi-paged bitmap
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMULTIBITMAP : IComparable, IComparable<FIMULTIBITMAP>, IEquatable<FIMULTIBITMAP>
	{
		private IntPtr data;
		public FIMULTIBITMAP(int ptr) { data = new IntPtr(ptr); }
		public FIMULTIBITMAP(IntPtr ptr) { data = ptr; }

		public static bool operator !=(FIMULTIBITMAP value1, FIMULTIBITMAP value2)
		{
			return value1.data != value2.data;
		}

		public static bool operator ==(FIMULTIBITMAP value1, FIMULTIBITMAP value2)
		{
			return value1.data == value2.data;
		}

		public static implicit operator FIMULTIBITMAP(int ptr)
		{
			return new FIMULTIBITMAP(ptr);
		}

		public static implicit operator int(FIMULTIBITMAP fi)
		{
			return fi.data.ToInt32();
		}

		public static implicit operator FIMULTIBITMAP(IntPtr ptr)
		{
			return new FIMULTIBITMAP(ptr);
		}

		public static implicit operator IntPtr(FIMULTIBITMAP fi)
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
			if (obj is FIMULTIBITMAP)
			{
				return Equals((FIMULTIBITMAP)obj);
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
			if (obj is FIMULTIBITMAP)
			{
				return CompareTo((FIMULTIBITMAP)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIMULTIBITMAP other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIMULTIBITMAP other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// Handle to an opened memory stream
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMEMORY : IComparable, IComparable<FIMEMORY>, IEquatable<FIMEMORY>
	{
		private IntPtr data;
		public FIMEMORY(int ptr) { data = new IntPtr(ptr); }
		public FIMEMORY(IntPtr ptr) { data = ptr; }

		public static bool operator !=(FIMEMORY value1, FIMEMORY value2)
		{
			return value1.data != value2.data;
		}

		public static bool operator ==(FIMEMORY value1, FIMEMORY value2)
		{
			return value1.data == value2.data;
		}

		public static implicit operator FIMEMORY(int ptr)
		{
			return new FIMEMORY(ptr);
		}

		public static implicit operator int(FIMEMORY fi)
		{
			return fi.data.ToInt32();
		}

		public static implicit operator FIMEMORY(IntPtr ptr)
		{
			return new FIMEMORY(ptr);
		}

		public static implicit operator IntPtr(FIMEMORY fi)
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
			if (obj is FIMEMORY)
			{
				return Equals((FIMEMORY)obj);
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
			if (obj is FIMEMORY)
			{
				return CompareTo((FIMEMORY)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIMEMORY other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIMEMORY other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// Handle to a metadata model
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIMETADATA : IComparable, IComparable<FIMETADATA>, IEquatable<FIMETADATA>
	{
		private IntPtr data;
		public FIMETADATA(int ptr) { data = new IntPtr(ptr); }
		public FIMETADATA(IntPtr ptr) { data = ptr; }

		public static bool operator !=(FIMETADATA value1, FIMETADATA value2)
		{
			return value1.data != value2.data;
		}

		public static bool operator ==(FIMETADATA value1, FIMETADATA value2)
		{
			return value1.data == value2.data;
		}

		public static implicit operator FIMETADATA(int ptr)
		{
			return new FIMETADATA(ptr);
		}

		public static implicit operator int(FIMETADATA fi)
		{
			return fi.data.ToInt32();
		}

		public static implicit operator FIMETADATA(IntPtr ptr)
		{
			return new FIMETADATA(ptr);
		}

		public static implicit operator IntPtr(FIMETADATA fi)
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
			if (obj is FIMETADATA)
			{
				return Equals((FIMETADATA)obj);
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
			if (obj is FIMETADATA)
			{
				return CompareTo((FIMETADATA)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIMETADATA other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIMETADATA other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// Handle to a FreeImage tag
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FITAG : IComparable, IComparable<FITAG>, IEquatable<FITAG>
	{
		private IntPtr data;
		public FITAG(int ptr) { data = new IntPtr(ptr); }
		public FITAG(IntPtr ptr) { data = ptr; }

		public static bool operator !=(FITAG value1, FITAG value2)
		{
			return value1.data != value2.data;
		}

		public static bool operator ==(FITAG value1, FITAG value2)
		{
			return value1.data == value2.data;
		}

		public static implicit operator FITAG(int ptr)
		{
			return new FITAG(ptr);
		}

		public static implicit operator int(FITAG fi)
		{
			return fi.data.ToInt32();
		}

		public static implicit operator FITAG(IntPtr ptr)
		{
			return new FITAG(ptr);
		}

		public static implicit operator IntPtr(FITAG fi)
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
			if (obj is FITAG)
			{
				return Equals((FITAG)obj);
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
			if (obj is FITAG)
			{
				return CompareTo((FITAG)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FITAG other)
		{
			return this.data.ToInt64().CompareTo(other.data.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FITAG other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// Structure for implementing access to custom handles.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct FreeImageIO
	{
		public ReadProc readProc;
		public WriteProc writeProc;
		public SeekProc seekProc;
		public TellProc tellProc;
	}

	// Bitmaps are made up of different structures.
	// Some bitmaps have palettes that define the used colors and the bitmaps real
	// data links to the palette index.
	// Others don't have a palette and each pixel is stored directly.
	//
	// No matter which type of bitmap is accessed FreeImage provides pointers to
	// beginning of a structure and its length.
	// In unmanaged code pointers would be used to access the data. In .NET
	// unsafe code is needed to that.
	//
	// RGBQUAD, RGBTRIPLE, FIRGB16, FIRGBA16, FIRGBF and FIRGBAF represent
	// the structures that bitmaps use to store their data.
	// Its up to the coder to choose the right one.
	//
	// Each structure can be converted from and to the .NETs structure 'Color'.

	// Bitmaps are made up of different structures.
	// Some bitmaps have palettes that define the used colors and the bitmaps real
	// data links to the palette index.
	// Others don't have a palette and each pixel is stored directly.
	//
	// No matter which type of bitmap is accessed FreeImage provides pointers to
	// beginning of a structure and its length.
	// In unmanaged code pointers would be used to access the data. In .NET
	// unsafe code is needed to that.
	//
	// RGBQUAD, RGBTRIPLE, FIRGB16, FIRGBA16, FIRGBF and FIRGBAF represent
	// the structures that bitmaps use to store their data.
	// Its up to the coder to choose the right one.
	//
	// Each structure can be converted from and to the .NETs structure 'Color'.

	/// <summary>
	/// The RGBQUAD structure describes a color consisting of relative intensities of red, green, and blue
	/// combined with an alpha factor. Each color is using 1 byte of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RGBQUAD : IComparable, IComparable<RGBQUAD>, IEquatable<RGBQUAD>
	{
		public byte rgbBlue;
		public byte rgbGreen;
		public byte rgbRed;
		public byte rgbReserved;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public RGBQUAD(Color color)
		{
			rgbBlue = color.B;
			rgbGreen = color.G;
			rgbRed = color.R;
			rgbReserved = color.A;
		}

		public static bool operator ==(RGBQUAD value1, RGBQUAD value2)
		{
			return
				value1.rgbBlue == value2.rgbBlue &&
				value1.rgbGreen == value2.rgbGreen &&
				value1.rgbRed == value2.rgbRed &&
				value1.rgbReserved == value2.rgbReserved;
		}

		public static bool operator !=(RGBQUAD value1, RGBQUAD value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator RGBQUAD(Color color)
		{
			return new RGBQUAD(color);
		}

		public static implicit operator Color(RGBQUAD rgbquad)
		{
			return rgbquad.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					rgbReserved,
					rgbRed,
					rgbGreen,
					rgbBlue);
			}
			set
			{
				rgbRed = value.R;
				rgbGreen = value.G;
				rgbBlue = value.B;
				rgbReserved = value.A;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is RGBQUAD)
			{
				return CompareTo((RGBQUAD)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBQUAD other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(RGBQUAD other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return ((rgbBlue << 16) | (rgbGreen << 8) | (rgbRed));
		}
	}

	/// <summary>
	/// The RGBTRIPLE structure describes a color consisting of relative intensities of red, green, and blue.
	/// Each color is using 1 byte of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RGBTRIPLE : IComparable, IComparable<RGBTRIPLE>, IEquatable<RGBTRIPLE>
	{
		public byte rgbtBlue;
		public byte rgbtGreen;
		public byte rgbtRed;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public RGBTRIPLE(Color color)
		{
			rgbtBlue = color.B;
			rgbtGreen = color.G;
			rgbtRed = color.R;
		}

		public static bool operator ==(RGBTRIPLE value1, RGBTRIPLE value2)
		{
			return
				value1.rgbtBlue == value2.rgbtBlue &&
				value1.rgbtGreen == value2.rgbtGreen &&
				value1.rgbtRed == value2.rgbtRed;
		}

		public static bool operator !=(RGBTRIPLE value1, RGBTRIPLE value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator RGBTRIPLE(Color color)
		{
			return new RGBTRIPLE(color);
		}

		public static implicit operator Color(RGBTRIPLE rgbtripple)
		{
			return rgbtripple.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					rgbtRed,
					rgbtGreen,
					rgbtBlue);
			}
			set
			{
				rgbtBlue = value.B;
				rgbtGreen = value.G;
				rgbtRed = value.R;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is RGBTRIPLE)
			{
				return CompareTo((RGBTRIPLE)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBTRIPLE other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(RGBTRIPLE other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return ((rgbtBlue << 16) | (rgbtGreen << 8) | (rgbtRed));
		}
	}

	/// <summary>
	/// The FIRGBA16 structure describes a color consisting of relative intensities of red, green, and blue
	/// combined with an alpha factor. Each color is using 2 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBA16 : IComparable, IComparable<FIRGBA16>, IEquatable<FIRGBA16>
	{
		public ushort red;
		public ushort green;
		public ushort blue;
		public ushort alpha;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public FIRGBA16(Color color)
		{
			red = (ushort)(color.R * 256);
			green = (ushort)(color.G * 256);
			blue = (ushort)(color.B * 256);
			alpha = (ushort)(color.A * 256);
		}

		public static bool operator ==(FIRGBA16 value1, FIRGBA16 value2)
		{
			return
				value1.alpha == value2.alpha &&
				value1.blue == value2.blue &&
				value1.green == value2.green &&
				value1.red == value2.red;
		}

		public static bool operator !=(FIRGBA16 value1, FIRGBA16 value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator FIRGBA16(Color color)
		{
			return new FIRGBA16(color);
		}

		public static implicit operator Color(FIRGBA16 firgba16)
		{
			return firgba16.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					alpha / 256,
					red / 256,
					green / 256,
					blue / 256);
			}
			set
			{
				red = (ushort)(value.R * 256);
				green = (ushort)(value.G * 256);
				blue = (ushort)(value.B * 256);
				alpha = (ushort)(value.A * 256);
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIRGBA16)
			{
				return CompareTo((FIRGBA16)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBA16 other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIRGBA16 other)
		{
			return this == other;
		}
	}

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

	/// <summary>
	/// The FIRGBAF structure describes a color consisting of relative intensities of red, green, and blue
	/// combined with an alpha factor. Each color is using 4 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBAF : IComparable, IComparable<FIRGBAF>, IEquatable<FIRGBAF>
	{
		public float red;
		public float green;
		public float blue;
		public float alpha;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public FIRGBAF(Color color)
		{
			red = (float)color.R / 255f;
			green = (float)color.G / 255f;
			blue = (float)color.B / 255f;
			alpha = (float)color.A / 255f;
		}

		public static bool operator ==(FIRGBAF value1, FIRGBAF value2)
		{
			return
				value1.alpha == value2.alpha &&
				value1.blue == value2.blue &&
				value1.green == value2.green &&
				value1.red == value2.red;
		}

		public static bool operator !=(FIRGBAF value1, FIRGBAF value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator FIRGBAF(Color color)
		{
			return new FIRGBAF(color);
		}

		public static implicit operator Color(FIRGBAF firgbaf)
		{
			return firgbaf.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					(int)(alpha * 255f),
					(int)(red * 255f),
					(int)(green * 255f),
					(int)(blue * 255f));
			}
			set
			{
				red = (float)value.R / 255f;
				green = (float)value.G / 255f;
				blue = (float)value.B / 255f;
				alpha = (float)value.A / 255f;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIRGBAF)
			{
				return CompareTo((FIRGBAF)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBAF other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIRGBAF other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// The FIRGBF structure describes a color consisting of relative intensities of red, green, and blue.
	/// Each color is using 4 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIRGBF : IComparable, IComparable<FIRGBF>, IEquatable<FIRGBF>
	{
		public float red;
		public float green;
		public float blue;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="color">Color to initialize with.</param>
		public FIRGBF(Color color)
		{
			red = (float)color.R / 255f;
			green = (float)color.G / 255f;
			blue = (float)color.B / 255f;
		}

		public static bool operator ==(FIRGBF value1, FIRGBF value2)
		{
			return
				value1.blue == value2.blue &&
				value1.green == value2.green &&
				value1.red == value2.red;
		}

		public static bool operator !=(FIRGBF value1, FIRGBF value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator FIRGBF(Color color)
		{
			return new FIRGBF(color);
		}

		public static implicit operator Color(FIRGBF firgbf)
		{
			return firgbf.color;
		}

		/// <summary>
		/// Gets or sets the color of the structure.
		/// </summary>
		public Color color
		{
			get
			{
				return Color.FromArgb(
					(int)(red * 255f),
					(int)(green * 255f),
					(int)(blue * 255f));
			}
			set
			{
				red = (float)value.R / 255f;
				green = (float)value.G / 255f;
				blue = (float)value.B / 255f;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIRGBF)
			{
				return CompareTo((FIRGBF)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBF other)
		{
			return this.color.ToArgb().CompareTo(other.color.ToArgb());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIRGBF other)
		{
			return this == other;
		}
	}

	/// <summary>
	/// The FICOMPLEX structure describes a color consisting of a real and an imaginary part.
	/// Each part is using 4 bytes of data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FICOMPLEX : IComparable, IComparable<FICOMPLEX>, IEquatable<FICOMPLEX>
	{
		public double r;
		public double i;

		public static bool operator ==(FICOMPLEX value1, FICOMPLEX value2)
		{
			return ((value1.r == value2.r) && (value1.i == value2.i));
		}

		public static bool operator !=(FICOMPLEX value1, FICOMPLEX value2)
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
			if (obj is FICOMPLEX)
			{
				return CompareTo((FICOMPLEX)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FICOMPLEX other)
		{
			return (this.r * this.i).CompareTo(other.r * other.i);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FICOMPLEX other)
		{
			return this == other;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (r * i).GetHashCode();
		}
	}

	/// <summary>
	/// The BITMAP structure defines the type, width, height, color format, and bit values of a bitmap.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAP
	{
		public int bmType;
		public int bmWidth;
		public int bmHeight;
		public int bmWidthBytes;
		public ushort bmPlanes;
		public ushort bmBitsPixel;
		public IntPtr bmBits;
	}

	/// <summary>
	/// This structure contains information about the dimensions and color format of a device-independent bitmap (DIB).
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFOHEADER : IEquatable<BITMAPINFOHEADER>
	{
		/// <summary>
		/// Specifies the size of the structure, in bytes.
		/// </summary>
		public uint biSize;
		/// <summary>
		/// Specifies the width of the bitmap, in pixels.
		/// </summary>
		public int biWidth;
		/// <summary>
		/// Specifies the height of the bitmap, in pixels.
		/// If biHeight is positive, the bitmap is a bottom-up DIB and its origin is the lower left corner.
		/// If biHeight is negative, the bitmap is a top-down DIB and its origin is the upper left corner.
		/// If biHeight is negative, indicating a top-down DIB, biCompression must be either BI_RGB or BI_BITFIELDS.
		/// Top-down DIBs cannot be compressed.
		/// </summary>
		public int biHeight;
		/// <summary>
		/// Specifies the number of planes for the target device. This value must be set to 1.
		/// </summary>
		public ushort biPlanes;
		/// <summary>
		/// Specifies the number of bits per pixel.
		/// </summary>
		public ushort biBitCount;
		/// <summary>
		/// Specifies the type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
		/// </summary>
		public uint biCompression;
		/// <summary>
		/// Specifies the size, in bytes, of the image.
		/// </summary>
		public uint biSizeImage;
		/// <summary>
		/// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
		/// </summary>
		public int biXPelsPerMeter;
		/// <summary>
		/// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap
		/// </summary>
		public int biYPelsPerMeter;
		/// <summary>
		/// Specifies the number of color indexes in the color table that are actually used by the bitmap.
		/// </summary>
		public uint biClrUsed;
		/// <summary>
		/// Specifies the number of color indexes required for displaying the bitmap.
		/// If this value is zero, all colors are required.
		/// </summary>
		public uint biClrImportant;

		public static bool operator ==(BITMAPINFOHEADER value1, BITMAPINFOHEADER value2)
		{
			return !((value1.biSize != value2.biSize) ||
					(value1.biWidth != value2.biWidth) ||
					(value1.biHeight != value2.biHeight) ||
					(value1.biPlanes != value2.biPlanes) ||
					(value1.biBitCount != value2.biBitCount) ||
					(value1.biCompression != value2.biCompression) ||
					(value1.biSizeImage != value2.biSizeImage) ||
					(value1.biXPelsPerMeter != value2.biXPelsPerMeter) ||
					(value1.biYPelsPerMeter != value2.biYPelsPerMeter) ||
					(value1.biClrUsed != value2.biClrUsed) ||
					(value1.biClrImportant != value2.biClrImportant));
		}

		public static bool operator !=(BITMAPINFOHEADER value1, BITMAPINFOHEADER value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is BITMAPINFOHEADER)
			{
				return Equals((BITMAPINFOHEADER)obj);
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(BITMAPINFOHEADER other)
		{
			return this == other;
		}
	}

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

	/// <summary>
	/// This Structure contains ICC-Profile data.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct FIICCPROFILE
	{
		private ICC_FLAGS flags;
		private uint size;
		private IntPtr data;

		/// <summary>
		/// Creates a new ICC-Profile for dib.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The ICC-Profile data.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public FIICCPROFILE(FIBITMAP dib, byte[] data)
			: this(dib, data, (int)data.Length)
		{
		}

		/// <summary>
		/// Creates a new ICC-Profile for dib.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The ICC-Profile data.</param>
		/// <param name="size">Number of bytes to use from data.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public unsafe FIICCPROFILE(FIBITMAP dib, byte[] data, int size)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FIICCPROFILE prof;
			size = Math.Min(size, (int)data.Length);
			prof = *(FIICCPROFILE*)FreeImage.CreateICCProfile(dib, data, size);
			this.flags = prof.flags;
			this.size = prof.size;
			this.data = prof.data;
		}

		/// <summary>
		/// Info flag of the profile.
		/// </summary>
		public ICC_FLAGS Flags
		{
			get { return flags; }
			set { flags = value; }
		}

		/// <summary>
		/// Profile's size measured in bytes.
		/// </summary>
		public uint Size
		{
			get { return size; }
		}

		/// <summary>
		/// Points to a block of contiguous memory containing the profile.
		/// </summary>
		public IntPtr DataPointer
		{
			get { return data; }
		}

		/// <summary>
		/// Copy of the ICC-Profiles data.
		/// </summary>
		public unsafe byte[] Data
		{
			get
			{
				byte[] result = new byte[size];
				byte* ptr = (byte*)data;
				for (int i = 0; i < size; i++)
					result[i] = ptr[i];
				return result;
			}
		}

		/// <summary>
		/// Indicates whether the profile is CMYK.
		/// </summary>
		public bool IsCMYK
		{
			get
			{
				return ((flags & ICC_FLAGS.FIICC_COLOR_IS_CMYK) > 0);
			}
		}
	}

	/// <summary>
	/// The structure contains functionpointers that make up a FreeImage plugin.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct Plugin
	{
		public FormatProc formatProc;
		public DescriptionProc descriptionProc;
		public ExtensionListProc extensionListProc;
		public RegExprProc regExprProc;
		public OpenProc openProc;
		public CloseProc closeProc;
		public PageCountProc pageCountProc;
		public PageCapabilityProc pageCapabilityProc;
		public LoadProc loadProc;
		public SaveProc saveProc;
		public ValidateProc validateProc;
		public MimeProc mimeProc;
		public SupportsExportBPPProc supportsExportBPPProc;
		public SupportsExportTypeProc supportsExportTypeProc;
		public SupportsICCProfilesProc supportsICCProfilesProc;
	}

	#endregion

	#region Enums

	/// <summary>
	/// I/O image format identifiers.
	/// </summary>
	public enum FREE_IMAGE_FORMAT
	{
		/// <summary>
		/// Unknown format (returned value only, never use it as input value)
		/// </summary>
		FIF_UNKNOWN = -1,
		/// <summary>
		/// Windows or OS/2 Bitmap File (*.BMP)
		/// </summary>
		FIF_BMP = 0,
		/// <summary>
		/// Windows Icon (*.ICO)
		/// </summary>
		FIF_ICO = 1,
		/// <summary>
		/// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
		/// </summary>
		FIF_JPEG = 2,
		/// <summary>
		/// JPEG Network Graphics (*.JNG)
		/// </summary>
		FIF_JNG = 3,
		/// <summary>
		/// Commodore 64 Koala format (*.KOA)
		/// </summary>
		FIF_KOALA = 4,
		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		FIF_LBM = 5,
		/// <summary>
		/// Amiga IFF (*.IFF, *.LBM)
		/// </summary>
		FIF_IFF = 5,
		/// <summary>
		/// Multiple Network Graphics (*.MNG)
		/// </summary>
		FIF_MNG = 6,
		/// <summary>
		/// Portable Bitmap (ASCII) (*.PBM)
		/// </summary>
		FIF_PBM = 7,
		/// <summary>
		/// Portable Bitmap (BINARY) (*.PBM)
		/// </summary>
		FIF_PBMRAW = 8,
		/// <summary>
		/// Kodak PhotoCD (*.PCD)
		/// </summary>
		FIF_PCD = 9,
		/// <summary>
		/// Zsoft Paintbrush PCX bitmap format (*.PCX)
		/// </summary>
		FIF_PCX = 10,
		/// <summary>
		/// Portable Graymap (ASCII) (*.PGM)
		/// </summary>
		FIF_PGM = 11,
		/// <summary>
		/// Portable Graymap (BINARY) (*.PGM)
		/// </summary>
		FIF_PGMRAW = 12,
		/// <summary>
		/// Portable Network Graphics (*.PNG)
		/// </summary>
		FIF_PNG = 13,
		/// <summary>
		/// Portable Pixelmap (ASCII) (*.PPM)
		/// </summary>
		FIF_PPM = 14,
		/// <summary>
		/// Portable Pixelmap (BINARY) (*.PPM)
		/// </summary>
		FIF_PPMRAW = 15,
		/// <summary>
		/// Sun Rasterfile (*.RAS)
		/// </summary>
		FIF_RAS = 16,
		/// <summary>
		/// truevision Targa files (*.TGA, *.TARGA)
		/// </summary>
		FIF_TARGA = 17,
		/// <summary>
		/// Tagged Image File Format (*.TIF, *.TIFF)
		/// </summary>
		FIF_TIFF = 18,
		/// <summary>
		/// Wireless Bitmap (*.WBMP)
		/// </summary>
		FIF_WBMP = 19,
		/// <summary>
		/// Adobe Photoshop (*.PSD)
		/// </summary>
		FIF_PSD = 20,
		/// <summary>
		/// Dr. Halo (*.CUT)
		/// </summary>
		FIF_CUT = 21,
		/// <summary>
		/// X11 Bitmap Format (*.XBM)
		/// </summary>
		FIF_XBM = 22,
		/// <summary>
		/// X11 Pixmap Format (*.XPM)
		/// </summary>
		FIF_XPM = 23,
		/// <summary>
		/// DirectDraw Surface (*.DDS)
		/// </summary>
		FIF_DDS = 24,
		/// <summary>
		/// Graphics Interchange Format (*.GIF)
		/// </summary>
		FIF_GIF = 25,
		/// <summary>
		/// High Dynamic Range (*.HDR)
		/// </summary>
		FIF_HDR = 26,
		/// <summary>
		/// Raw Fax format CCITT G3 (*.G3)
		/// </summary>
		FIF_FAXG3 = 27,
		/// <summary>
		/// Silicon Graphics SGI image format (*.SGI)
		/// </summary>
		FIF_SGI = 28,
		/// <summary>
		/// OpenEXR format (*.EXR)
		/// </summary>
		FIF_EXR = 29,
		/// <summary>
		/// JPEG-2000 format (*.J2K, *.J2C)
		/// </summary>
		FIF_J2K = 30,
		/// <summary>
		/// JPEG-2000 format (*.JP2)
		/// </summary>
		FIF_JP2 = 31
	}

	/// <summary>
	/// Image types used in FreeImage.
	/// </summary>
	public enum FREE_IMAGE_TYPE
	{
		/// <summary>
		/// unknown type
		/// </summary>
		FIT_UNKNOWN = 0,
		/// <summary>
		/// standard image : 1-, 4-, 8-, 16-, 24-, 32-bit
		/// </summary>
		FIT_BITMAP = 1,
		/// <summary>
		/// array of unsigned short : unsigned 16-bit
		/// </summary>
		FIT_UINT16 = 2,
		/// <summary>
		/// array of short : signed 16-bit
		/// </summary>
		FIT_INT16 = 3,
		/// <summary>
		/// array of unsigned long : unsigned 32-bit
		/// </summary>
		FIT_UINT32 = 4,
		/// <summary>
		/// array of long : signed 32-bit
		/// </summary>
		FIT_INT32 = 5,
		/// <summary>
		/// array of float : 32-bit IEEE floating point
		/// </summary>
		FIT_FLOAT = 6,
		/// <summary>
		/// array of double : 64-bit IEEE floating point
		/// </summary>
		FIT_DOUBLE = 7,
		/// <summary>
		/// array of FICOMPLEX : 2 x 64-bit IEEE floating point
		/// </summary>
		FIT_COMPLEX = 8,
		/// <summary>
		/// 48-bit RGB image : 3 x 16-bit
		/// </summary>
		FIT_RGB16 = 9,
		/// <summary>
		/// 64-bit RGBA image : 4 x 16-bit
		/// </summary>
		FIT_RGBA16 = 10,
		/// <summary>
		/// 96-bit RGB float image : 3 x 32-bit IEEE floating point
		/// </summary>
		FIT_RGBF = 11,
		/// <summary>
		/// 128-bit RGBA float image : 4 x 32-bit IEEE floating point
		/// </summary>
		FIT_RGBAF = 12
	}

	/// <summary>
	/// Image color types used in FreeImage.
	/// </summary>
	public enum FREE_IMAGE_COLOR_TYPE
	{
		/// <summary>
		/// min value is white
		/// </summary>
		FIC_MINISWHITE = 0,
		/// <summary>
		/// min value is black
		/// </summary>
		FIC_MINISBLACK = 1,
		/// <summary>
		/// RGB color model
		/// </summary>
		FIC_RGB = 2,
		/// <summary>
		/// color map indexed
		/// </summary>
		FIC_PALETTE = 3,
		/// <summary>
		/// RGB color model with alpha channel
		/// </summary>
		FIC_RGBALPHA = 4,
		/// <summary>
		/// CMYK color model
		/// </summary>
		FIC_CMYK = 5
	}

	/// <summary>
	/// Color quantization algorithms.
	/// Constants used in FreeImage_ColorQuantize.
	/// </summary>
	public enum FREE_IMAGE_QUANTIZE
	{
		/// <summary>
		/// Xiaolin Wu color quantization algorithm
		/// </summary>
		FIQ_WUQUANT = 0,
		/// <summary>
		/// NeuQuant neural-net quantization algorithm by Anthony Dekker
		/// </summary>
		FIQ_NNQUANT = 1
	}

	/// <summary>
	/// Dithering algorithms.
	/// Constants used in FreeImage_Dither.
	/// </summary>
	public enum FREE_IMAGE_DITHER
	{
		/// <summary>
		/// Floyd and Steinberg error diffusion
		/// </summary>
		FID_FS = 0,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 2 dithering matrix)
		/// </summary>
		FID_BAYER4x4 = 1,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 3 dithering matrix)
		/// </summary>
		FID_BAYER8x8 = 2,
		/// <summary>
		/// Ordered clustered dot dithering (order 3 - 6x6 matrix)
		/// </summary>
		FID_CLUSTER6x6 = 3,
		/// <summary>
		/// Ordered clustered dot dithering (order 4 - 8x8 matrix)
		/// </summary>
		FID_CLUSTER8x8 = 4,
		/// <summary>
		/// Ordered clustered dot dithering (order 8 - 16x16 matrix)
		/// </summary>
		FID_CLUSTER16x16 = 5,
		/// <summary>
		/// Bayer ordered dispersed dot dithering (order 4 dithering matrix)
		/// </summary>
		FID_BAYER16x16 = 6
	}

	/// <summary>
	/// Lossless JPEG transformations constants used in FreeImage_JPEGTransform.
	/// </summary>
	public enum FREE_IMAGE_JPEG_OPERATION
	{
		/// <summary>
		/// no transformation
		/// </summary>
		FIJPEG_OP_NONE = 0,
		/// <summary>
		/// horizontal flip
		/// </summary>
		FIJPEG_OP_FLIP_H = 1,
		/// <summary>
		/// vertical flip
		/// </summary>
		FIJPEG_OP_FLIP_V = 2,
		/// <summary>
		/// transpose across UL-to-LR axis
		/// </summary>
		FIJPEG_OP_TRANSPOSE = 3,
		/// <summary>
		/// transpose across UR-to-LL axis
		/// </summary>
		FIJPEG_OP_TRANSVERSE = 4,
		/// <summary>
		/// 90-degree clockwise rotation
		/// </summary>
		FIJPEG_OP_ROTATE_90 = 5,
		/// <summary>
		/// 180-degree rotation
		/// </summary>
		FIJPEG_OP_ROTATE_180 = 6,
		/// <summary>
		/// 270-degree clockwise (or 90 ccw)
		/// </summary>
		FIJPEG_OP_ROTATE_270 = 7
	}

	/// <summary>
	/// Tone mapping operators. Constants used in FreeImage_ToneMapping.
	/// </summary>
	public enum FREE_IMAGE_TMO
	{
		/// <summary>
		/// Adaptive logarithmic mapping (F. Drago, 2003)
		/// </summary>
		FITMO_DRAGO03 = 0,
		/// <summary>
		/// Dynamic range reduction inspired by photoreceptor physiology (E. Reinhard, 2005)
		/// </summary>
		FITMO_REINHARD05 = 1,
		/// <summary>
		/// Gradient domain high dynamic range compression (R. Fattal, 2002)
		/// </summary>
		FITMO_FATTAL02
	}

	/// <summary>
	/// Upsampling / downsampling filters. Constants used in FreeImage_Rescale.
	/// </summary>
	public enum FREE_IMAGE_FILTER
	{
		/// <summary>
		/// Box, pulse, Fourier window, 1st order (constant) b-spline
		/// </summary>
		FILTER_BOX = 0,
		/// <summary>
		/// Mitchell and Netravali's two-param cubic filter
		/// </summary>
		FILTER_BICUBIC = 1,
		/// <summary>
		/// Bilinear filter
		/// </summary>
		FILTER_BILINEAR = 2,
		/// <summary>
		/// 4th order (cubic) b-spline
		/// </summary>
		FILTER_BSPLINE = 3,
		/// <summary>
		/// Catmull-Rom spline, Overhauser spline
		/// </summary>
		FILTER_CATMULLROM = 4,
		/// <summary>
		/// Lanczos3 filter
		/// </summary>
		FILTER_LANCZOS3 = 5
	}

	/// <summary>
	/// Color channels. Constants used in color manipulation routines.
	/// </summary>
	public enum FREE_IMAGE_COLOR_CHANNEL
	{
		/// <summary>
		/// Use red, green and blue channels
		/// </summary>
		FICC_RGB = 0,
		/// <summary>
		/// Use red channel
		/// </summary>
		FICC_RED = 1,
		/// <summary>
		/// Use green channel
		/// </summary>
		FICC_GREEN = 2,
		/// <summary>
		/// Use blue channel
		/// </summary>
		FICC_BLUE = 3,
		/// <summary>
		/// Use alpha channel
		/// </summary>
		FICC_ALPHA = 4,
		/// <summary>
		/// Use black channel
		/// </summary>
		FICC_BLACK = 5,
		/// <summary>
		/// Complex images: use real part
		/// </summary>
		FICC_REAL = 6,
		/// <summary>
		/// Complex images: use imaginary part
		/// </summary>
		FICC_IMAG = 7,
		/// <summary>
		/// Complex images: use magnitude
		/// </summary>
		FICC_MAG = 8,
		/// <summary>
		/// Complex images: use phase
		/// </summary>
		FICC_PHASE = 9
	}

	/// <summary>
	/// Tag data type information (based on TIFF specifications)
	/// Note: RATIONALs are the ratio of two 32-bit integer values.
	/// </summary>
	public enum FREE_IMAGE_MDTYPE
	{
		/// <summary>
		/// placeholder
		/// </summary>
		FIDT_NOTYPE = 0,
		/// <summary>
		/// 8-bit unsigned integer
		/// </summary>
		FIDT_BYTE = 1,
		/// <summary>
		/// 8-bit bytes w/ last byte null
		/// </summary>
		FIDT_ASCII = 2,
		/// <summary>
		/// 16-bit unsigned integer
		/// </summary>
		FIDT_SHORT = 3,
		/// <summary>
		/// 32-bit unsigned integer
		/// </summary>
		FIDT_LONG = 4,
		/// <summary>
		/// 64-bit unsigned fraction
		/// </summary>
		FIDT_RATIONAL = 5,
		/// <summary>
		/// 8-bit signed integer
		/// </summary>
		FIDT_SBYTE = 6,
		/// <summary>
		/// 8-bit untyped data
		/// </summary>
		FIDT_UNDEFINED = 7,
		/// <summary>
		/// 16-bit signed integer
		/// </summary>
		FIDT_SSHORT = 8,
		/// <summary>
		/// 32-bit signed integer
		/// </summary>
		FIDT_SLONG = 9,
		/// <summary>
		/// 64-bit signed fraction
		/// </summary>
		FIDT_SRATIONAL = 10,
		/// <summary>
		/// 32-bit IEEE floating point
		/// </summary>
		FIDT_FLOAT = 11,
		/// <summary>
		/// 64-bit IEEE floating point
		/// </summary>
		FIDT_DOUBLE = 12,
		/// <summary>
		/// 32-bit unsigned integer (offset)
		/// </summary>
		FIDT_IFD = 13,
		/// <summary>
		/// 32-bit RGBQUAD
		/// </summary>
		FIDT_PALETTE = 14
	}

	/// <summary>
	/// Metadata models supported by FreeImage.
	/// </summary>
	public enum FREE_IMAGE_MDMODEL
	{
		/// <summary>
		/// No data
		/// </summary>
		FIMD_NODATA = -1,
		/// <summary>
		/// single comment or keywords
		/// </summary>
		FIMD_COMMENTS = 0,
		/// <summary>
		/// Exif-TIFF metadata
		/// </summary>
		FIMD_EXIF_MAIN = 1,
		/// <summary>
		/// Exif-specific metadata
		/// </summary>
		FIMD_EXIF_EXIF = 2,
		/// <summary>
		/// Exif GPS metadata
		/// </summary>
		FIMD_EXIF_GPS = 3,
		/// <summary>
		/// Exif maker note metadata
		/// </summary>
		FIMD_EXIF_MAKERNOTE = 4,
		/// <summary>
		/// Exif interoperability metadata
		/// </summary>
		FIMD_EXIF_INTEROP = 5,
		/// <summary>
		/// IPTC/NAA metadata
		/// </summary>
		FIMD_IPTC = 6,
		/// <summary>
		/// Abobe XMP metadata
		/// </summary>
		FIMD_XMP = 7,
		/// <summary>
		/// GeoTIFF metadata
		/// </summary>
		FIMD_GEOTIFF = 8,
		/// <summary>
		/// Animation metadata
		/// </summary>
		FIMD_ANIMATION = 9,
		/// <summary>
		/// Used to attach other metadata types to a dib
		/// </summary>
		FIMD_CUSTOM = 10
	}

	/// <summary>
	/// Flags used in load functions.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_LOAD_FLAGS
	{
		/// <summary>
		/// Default option for all types.
		/// </summary>
		DEFAULT = 0,
		/// <summary>
		/// Load the image as a 256 color image with ununsed palette entries, if it's 16 or 2 color.
		/// </summary>
		GIF_LOAD256 = 1,
		/// <summary>
		/// 'Play' the GIF to generate each frame (as 32bpp) instead of returning raw frame data when loading.
		/// </summary>
		GIF_PLAYBACK = 2,
		/// <summary>
		/// Convert to 32bpp and create an alpha channel from the AND-mask when loading.
		/// </summary>
		ICO_MAKEALPHA = 1,
		/// <summary>
		/// Load the file as fast as possible, sacrificing some quality.
		/// </summary>
		JPEG_FAST = 0x0001,
		/// <summary>
		/// Load the file with the best quality, sacrificing some speed.
		/// </summary>
		JPEG_ACCURATE = 0x0002,
		/// <summary>
		/// load separated CMYK "as is" (use | to combine with other load flags).
		/// </summary>
		JPEG_CMYK = 0x0004,
		/// <summary>
		/// Load the bitmap sized 768 x 512.
		/// </summary>
		PCD_BASE = 1,
		/// <summary>
		/// Load the bitmap sized 384 x 256.
		/// </summary>
		PCD_BASEDIV4 = 2,
		/// <summary>
		/// Load the bitmap sized 192 x 128.
		/// </summary>
		PCD_BASEDIV16 = 3,
		/// <summary>
		/// Avoid gamma correction.
		/// </summary>
		PNG_IGNOREGAMMA = 1,
		/// <summary>
		/// If set the loader converts RGB555 and ARGB8888 -> RGB888.
		/// </summary>
		TARGA_LOAD_RGB888 = 1,
		/// <summary>
		/// Reads tags for separated CMYK.
		/// </summary>
		TIFF_CMYK = 0x0001
	}

	/// <summary>
	/// Flags used in save functions.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_SAVE_FLAGS
	{
		/// <summary>
		/// Default option for all types.
		/// </summary>
		DEFAULT = 0,
		/// <summary>
		/// Save with run length encoding.
		/// </summary>
		BMP_SAVE_RLE = 1,
		/// <summary>
		/// Save data as float instead of as half (not recommended).
		/// </summary>
		EXR_FLOAT = 0x0001,
		/// <summary>
		/// Save with no compression.
		/// </summary>
		EXR_NONE = 0x0002,
		/// <summary>
		/// Save with zlib compression, in blocks of 16 scan lines.
		/// </summary>
		EXR_ZIP = 0x0004,
		/// <summary>
		/// Save with piz-based wavelet compression.
		/// </summary>
		EXR_PIZ = 0x0008,
		/// <summary>
		/// Save with lossy 24-bit float compression.
		/// </summary>
		EXR_PXR24 = 0x0010,
		/// <summary>
		/// Save with lossy 44% float compression - goes to 22% when combined with EXR_LC.
		/// </summary>
		EXR_B44 = 0x0020,
		/// <summary>
		/// Save images with one luminance and two chroma channels, rather than as RGB (lossy compression).
		/// </summary>
		EXR_LC = 0x0040,
		/// <summary>
		/// Save with superb quality (100:1).
		/// </summary>
		JPEG_QUALITYSUPERB = 0x80,
		/// <summary>
		/// Save with good quality (75:1).
		/// </summary>
		JPEG_QUALITYGOOD = 0x0100,
		/// <summary>
		/// Save with normal quality (50:1).
		/// </summary>
		JPEG_QUALITYNORMAL = 0x0200,
		/// <summary>
		/// Save with average quality (25:1).
		/// </summary>
		JPEG_QUALITYAVERAGE = 0x0400,
		/// <summary>
		/// Save with bad quality (10:1).
		/// </summary>
		JPEG_QUALITYBAD = 0x0800,
		/// <summary>
		/// Save as a progressive-JPEG (use | to combine with other save flags).
		/// </summary>
		JPEG_PROGRESSIVE = 0x2000,
		/// <summary>
		/// If set the writer saves in ASCII format (i.e. P1, P2 or P3).
		/// </summary>
		PNM_SAVE_ASCII = 1,
		/// <summary>
		/// Stores tags for separated CMYK (use | to combine with compression flags).
		/// </summary>
		TIFF_CMYK = 0x0001,
		/// <summary>
		/// Save using PACKBITS compression.
		/// </summary>
		TIFF_PACKBITS = 0x0100,
		/// <summary>
		/// Save using DEFLATE compression (a.k.a. ZLIB compression).
		/// </summary>
		TIFF_DEFLATE = 0x0200,
		/// <summary>
		/// Save using ADOBE DEFLATE compression.
		/// </summary>
		TIFF_ADOBE_DEFLATE = 0x0400,
		/// <summary>
		/// Save without any compression.
		/// </summary>
		TIFF_NONE = 0x0800,
		/// <summary>
		/// Save using CCITT Group 3 fax encoding.
		/// </summary>
		TIFF_CCITTFAX3 = 0x1000,
		/// <summary>
		/// Save using CCITT Group 4 fax encoding.
		/// </summary>
		TIFF_CCITTFAX4 = 0x2000,
		/// <summary>
		/// Save using LZW compression.
		/// </summary>
		TIFF_LZW = 0x4000,
		/// <summary>
		/// Save using JPEG compression.
		/// </summary>
		TIFF_JPEG = 0x8000
	}

	/// <summary>
	/// Flags for ICC profiles.
	/// </summary>
	[System.Flags]
	public enum ICC_FLAGS : ushort
	{
		/// <summary>
		/// Default value.
		/// </summary>
		FIICC_DEFAULT = 0x00,
		/// <summary>
		/// The color is CMYK.
		/// </summary>
		FIICC_COLOR_IS_CMYK = 0x01
	}

	#endregion

	#region Delegates

	// Delegates used by the 'FreeImageIO' structure

	/// <summary>
	/// Delegate for capturing error messages.
	/// </summary>
	/// <param name="fif">The format of the image.</param>
	/// <param name="message">The errormessage.</param>
	// DLL_API is missing in the definition of the callbackfuntion.
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void OutputMessageFunction(FREE_IMAGE_FORMAT fif, string message);

	/// <summary>
	/// Delegate wrapping the C++ function 'fread'.
	/// </summary>
	/// <param name="buffer">Pointer to read from.</param>
	/// <param name="size">Item size in bytes.</param>
	/// <param name="count">Maximum number of items to be read.</param>
	/// <param name="handle">Handle/stream to read from.</param>
	/// <returns>number of full items actually read,
	/// which may be less than count if an error occurs or
	/// if the end of the file is encountered before reaching count.</returns>
	public delegate uint ReadProc(IntPtr buffer, uint size, uint count, fi_handle handle);

	/// <summary>
	/// Delegate wrapping the C++ function 'fwrite'.
	/// </summary>
	/// <param name="buffer">Pointer to data to be written.</param>
	/// <param name="size">Item size in bytes.</param>
	/// <param name="count">Maximum number of items to be written.</param>
	/// <param name="handle">Handle/stream to write to.</param>
	/// <returns>Number of full items actually written,
	/// which may be less than count if an error occurs.
	/// Also, if an error occurs, the file-position indicator cannot be determined.</returns>
	public delegate uint WriteProc(IntPtr buffer, uint size, uint count, fi_handle handle);

	/// <summary>
	/// Delegate wrapping the C++ function 'fseek'.
	/// </summary>
	/// <param name="handle">Handle/stream to seek in.</param>
	/// <param name="offset">Number of bytes from origin.</param>
	/// <param name="origin">Initial position.</param>
	/// <returns>If successful 0 is returned; otherwise a nonzero value. </returns>
	public delegate int SeekProc(fi_handle handle, int offset, SeekOrigin origin);

	/// <summary>
	/// Delegate wrapping the C++ function 'ftell'.
	/// </summary>
	/// <param name="handle">Handle/stream to retrieve its currents position from.</param>
	/// <returns>The current position.</returns>
	public delegate int TellProc(fi_handle handle);

	// Delegates used by 'Plugin' structure

	/// <summary>
	/// Delegate to a function returning a string which describes
	/// the plugins format.
	/// </summary>
	public delegate string FormatProc();

	/// <summary>
	/// Delegate to a function returning a string which contains
	/// a more detailed description.
	/// </summary>
	public delegate string DescriptionProc();

	/// <summary>
	/// Delegate to a function returning a comma seperated list
	/// of file-extensions the plugin can read or write.
	/// </summary>
	public delegate string ExtensionListProc();

	/// <summary>
	/// Delegate to a function returning a regular expression that
	/// can be used to idientify whether a file can be handled by the plugin.
	/// </summary>
	public delegate string RegExprProc();

	/// <summary>
	/// Delegate to a function that opens a file.
	/// </summary>
	public delegate IntPtr OpenProc(ref FreeImageIO io, fi_handle handle, bool read);

	/// <summary>
	/// Delegate to a function that closes a previosly opened file.
	/// </summary>
	public delegate void CloseProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// Delegate to a function that returns the number of pages of a multi-paged
	/// bitmap if the plugin is capable of handling multi-paged bitmaps
	/// </summary>
	public delegate int PageCountProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// UNKNOWN
	/// </summary>
	public delegate int PageCapabilityProc(ref FreeImageIO io, fi_handle handle, IntPtr data);

	/// <summary>
	/// Delegate to a function that loads a bitmap into memory.
	/// </summary>
	public delegate FIBITMAP LoadProc(ref FreeImageIO io, fi_handle handle, int page, int flags, IntPtr data);

	/// <summary>
	///  Delegate to a function that saves a bitmap.
	/// </summary>
	public delegate bool SaveProc(ref FreeImageIO io, FIBITMAP dib, fi_handle handle, int page, int flags, IntPtr data);

	/// <summary>
	/// Delegate to a function that validates a bitmap.
	/// </summary>
	public delegate bool ValidateProc(ref FreeImageIO io, fi_handle handle);

	/// <summary>
	/// Delegate to a function returning a string which contains
	/// a Mime.
	/// </summary>
	public delegate string MimeProc();

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle a
	/// given color depth.
	/// </summary>
	public delegate bool SupportsExportBPPProc(int bpp);

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle a
	/// given image type.
	/// </summary>
	public delegate bool SupportsExportTypeProc(FREE_IMAGE_TYPE type);

	/// <summary>
	/// Delegate to a function that returns whether the plugin can handle
	/// ICC-Profiles.
	/// </summary>
	/// <returns></returns>
	public delegate bool SupportsICCProfilesProc();

	/// <summary>
	/// Callback used by FreeImage to register plugins.
	/// </summary>
	public delegate void InitProc(ref Plugin plugin, int format_id);

	#endregion

	public static partial class FreeImage
	{
		#region Constants

		/// <summary>
		/// Filename of the FreeImage library.
		/// </summary>
		private const string FreeImageLibrary = "FreeImage.dll";

		/// <summary>
		/// Major version of the wrapper.
		/// </summary>
		public const int FREEIMAGE_MAJOR_VERSION = 3;
		/// <summary>
		/// Minor version of the wrapper.
		/// </summary>
		public const int FREEIMAGE_MINOR_VERSION = 10;
		/// <summary>
		/// Release version of the wrapper.
		/// </summary>
		public const int FREEIMAGE_RELEASE_SERIAL = 0;

		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_RED = 2;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_GREEN = 1;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_BLUE = 0;
		/// <summary>
		/// Number of bytes to shift left within a 4 byte block.
		/// </summary>
		public const int FI_RGBA_ALPHA = 3;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_RED_MASK = 0x00FF0000;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_GREEN_MASK = 0x0000FF00;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_BLUE_MASK = 0x000000FF;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const uint FI_RGBA_ALPHA_MASK = 0xFF000000;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_RED_SHIFT = 16;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_GREEN_SHIFT = 8;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_BLUE_SHIFT = 0;
		/// <summary>
		/// Number of bits to shift left within a 32 bit block.
		/// </summary>
		public const int FI_RGBA_ALPHA_SHIFT = 24;
		/// <summary>
		/// Mask indicating the position of color components of a 32 bit color.
		/// </summary>
		public const uint FI_RGBA_RGB_MASK = (FI_RGBA_RED_MASK | FI_RGBA_GREEN_MASK | FI_RGBA_BLUE_MASK);

		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_RED_MASK = 0x7C00;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_GREEN_MASK = 0x03E0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_555_BLUE_MASK = 0x001F;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_RED_SHIFT = 10;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_GREEN_SHIFT = 5;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_555_BLUE_SHIFT = 0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_RED_MASK = 0xF800;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_GREEN_MASK = 0x07E0;
		/// <summary>
		/// Mask indicating the position of the given color.
		/// </summary>
		public const int FI16_565_BLUE_MASK = 0x001F;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_RED_SHIFT = 11;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_GREEN_SHIFT = 5;
		/// <summary>
		/// Number of bits to shift left within a 16 bit block.
		/// </summary>
		public const int FI16_565_BLUE_SHIFT = 0;

		#endregion

		#region General functions

		/// <summary>
		/// Initialises the library.
		/// </summary>
		/// <param name="load_local_plugins_only">
		/// When the load_local_plugins_only parameter is true, FreeImage won't make use of external plugins.
		/// </param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Initialise")]
		private static extern void Initialise(bool load_local_plugins_only);

		/// <summary>
		/// Deinitialises the library.
		/// </summary>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeInitialise")]
		private static extern void DeInitialise();

		/// <summary>
		/// Returns a string containing the current version of the library.
		/// </summary>
		/// <returns>The current version of the library.</returns>
		public static unsafe string GetVersion() { return PtrToStr(GetVersion_()); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetVersion")]
		private static unsafe extern byte* GetVersion_();

		/// <summary>
		/// Returns a string containing a standard copyright message.
		/// </summary>
		/// <returns>A standard copyright message.</returns>
		public static unsafe string GetCopyrightMessage() { return PtrToStr(GetCopyrightMessage_()); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetCopyrightMessage")]
		private static unsafe extern byte* GetCopyrightMessage_();

		/// <summary>
		/// Calls the set error message function in FreeImage.
		/// </summary>
		/// <param name="fif">Format of the bitmaps.</param>
		/// <param name="message">The error message.</param>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_OutputMessageProc")]
		public static extern void OutputMessageProc(FREE_IMAGE_FORMAT fif, string message);

		/// <summary>
		/// You use the function FreeImage_SetOutputMessage to capture the log string
		/// so that you can show it to the user of the program.
		/// The callback is implemented in the 'Message' event of this class.
		/// </summary>
		/// <remarks>The function is private because FreeImage can only have a single
		/// callback function. To use the callback use the 'Message' event of this class.</remarks>
		/// <param name="omf">Handler to the callback function.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetOutputMessage")]
		internal static extern void SetOutputMessage(OutputMessageFunction omf);

		#endregion

		#region Bitmap management functions

		/// <summary>
		/// Creates a new bitmap in memory.
		/// </summary>
		/// <param name="width">Width of the new bitmap.</param>
		/// <param name="height">Height of the new bitmap.</param>
		/// <param name="bpp">Bit depth of the new Bitmap.
		/// Supported pixel depth: 1-, 4-, 8-, 16-, 24-, 32-bit per pixel for standard bitmap</param>
		/// <param name="red_mask">Red part of the color layout.
		/// eg: 0xFF0000</param>
		/// <param name="green_mask">Green part of the color layout.
		/// eg: 0x00FF00</param>
		/// <param name="blue_mask">Blue part of the color layout.
		/// eg: 0x0000FF</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Allocate")]
		public static extern FIBITMAP Allocate(int width, int height, int bpp,
				uint red_mask, uint green_mask, uint blue_mask);

		/// <summary>
		/// Creates a new bitmap in memory.
		/// </summary>
		/// <param name="type">Type of the image.</param>
		/// <param name="width">Width of the new bitmap.</param>
		/// <param name="height">Height of the new bitmap.</param>
		/// <param name="bpp">Bit depth of the new Bitmap.
		/// Supported pixel depth: 1-, 4-, 8-, 16-, 24-, 32-bit per pixel for standard bitmap</param>
		/// <param name="red_mask">Red part of the color layout.
		/// eg: 0xFF0000</param>
		/// <param name="green_mask">Green part of the color layout.
		/// eg: 0x00FF00</param>
		/// <param name="blue_mask">Blue part of the color layout.
		/// eg: 0x0000FF</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AllocateT")]
		public static extern FIBITMAP AllocateT(FREE_IMAGE_TYPE type, int width, int height, int bpp,
				uint red_mask, uint green_mask, uint blue_mask);

		/// <summary>
		/// Makes an exact reproduction of an existing bitmap, including metadata and attached profile if any.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Clone")]
		public static extern FIBITMAP Clone(FIBITMAP dib);

		/// <summary>
		/// Deletes a previously loaded FIBITMAP from memory.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Unload")]
		public static extern void Unload(FIBITMAP dib);

		/// <summary>
		/// Decodes a bitmap, allocates memory for it and returns it as a FIBITMAP.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="filename">Name of the file to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_LoadU")]
		public static extern FIBITMAP Load(FREE_IMAGE_FORMAT fif, string filename, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Decodes a bitmap, allocates memory for it and returns it as a FIBITMAP.
		/// The filename supports UNICODE.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="filename">Name of the file to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_LoadU")]
		private static extern FIBITMAP LoadU(FREE_IMAGE_FORMAT fif, string filename, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Loads a bitmap from an arbitrary source.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="io">A FreeImageIO structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadFromHandle")]
		public static extern FIBITMAP LoadFromHandle(FREE_IMAGE_FORMAT fif, ref FreeImageIO io, fi_handle handle, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a file.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_SaveU")]
		public static extern bool Save(FREE_IMAGE_FORMAT fif, FIBITMAP dib, string filename, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a file.
		/// The filename supports UNICODE.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_SaveU")]
		private static extern bool SaveU(FREE_IMAGE_FORMAT fif, FIBITMAP dib, string filename, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Saves a bitmap to an arbitrary source.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="io">A FreeImageIO structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SaveToHandle")]
		public static extern bool SaveToHandle(FREE_IMAGE_FORMAT fif, FIBITMAP dib, ref FreeImageIO io, fi_handle handle,
				FREE_IMAGE_SAVE_FLAGS flags);

		#endregion

		#region Memory I/O streams

		/// <summary>
		/// Open a memory stream.
		/// </summary>
		/// <param name="data">Pointer to the data in memory.</param>
		/// <param name="size_in_bytes">Length of the data in byte.</param>
		/// <returns>Handle to a memory stream.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMemory")]
		public static extern FIMEMORY OpenMemory(IntPtr data, uint size_in_bytes);

		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMemory")]
		internal static extern FIMEMORY OpenMemoryEx(byte[] data, uint size_in_bytes);

		/// <summary>
		/// Close and free a memory stream.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloseMemory")]
		public static extern void CloseMemory(FIMEMORY stream);

		/// <summary>
		/// Decodes a bitmap from a stream, allocates memory for it and returns it as a FIBITMAP.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadFromMemory")]
		public static extern FIBITMAP LoadFromMemory(FREE_IMAGE_FORMAT fif, FIMEMORY stream, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Saves a previosly loaded FIBITMAP to a stream.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SaveToMemory")]
		public static extern bool SaveToMemory(FREE_IMAGE_FORMAT fif, FIBITMAP dib, FIMEMORY stream, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Gets the current position of a memory handle.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <returns>The current file position if successful, -1 otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TellMemory")]
		public static extern int TellMemory(FIMEMORY stream);

		/// <summary>
		/// Moves the memory handle to a specified location.
		/// </summary>
		/// <param name="stream">Handle to a memory stream.</param>
		/// <param name="offset">Number of bytes from origin.</param>
		/// <param name="origin">Initial position.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SeekMemory")]
		public static extern bool SeekMemory(FIMEMORY stream, int offset, System.IO.SeekOrigin origin);

		/// <summary>
		/// Provides a direct buffer access to a memory stream.
		/// </summary>
		/// <param name="stream">The target memory stream.</param>
		/// <param name="data">Pointer to the data in memory.</param>
		/// <param name="size_in_bytes">Size of the data in bytes.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AcquireMemory")]
		public static extern bool AcquireMemory(FIMEMORY stream, ref IntPtr data, ref uint size_in_bytes);

		/// <summary>
		/// Reads data from a memory stream.
		/// </summary>
		/// <param name="buffer">The buffer to store the data in.</param>
		/// <param name="size">Size in bytes of the items.</param>
		/// <param name="count">Number of items to read.</param>
		/// <param name="stream">The stream to read from.
		/// The memory pointer associated with stream is increased by the number of bytes actually read.</param>
		/// <returns>The number of full items actually read.
		/// May be less than count on error or stream-end.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ReadMemory")]
		public static extern uint ReadMemory(byte[] buffer, uint size, uint count, FIMEMORY stream);

		/// <summary>
		/// Writes data to a memory stream.
		/// </summary>
		/// <param name="buffer">The buffer to read the data from.</param>
		/// <param name="size">Size in bytes of the items.</param>
		/// <param name="count">Number of items to write.</param>
		/// <param name="stream">The stream to write to.
		/// The memory pointer associated with stream is increased by the number of bytes actually written.</param>
		/// <returns>The number of full items actually written.
		/// May be less than count on error or stream-end.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_WriteMemory")]
		public static extern uint WriteMemory(byte[] buffer, uint size, uint count, FIMEMORY stream);

		/// <summary>
		/// Open a multi-page bitmap from a memory stream.
		/// </summary>
		/// <param name="fif">Type of the bitmap.</param>
		/// <param name="stream">The stream to decode.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadMultiBitmapFromMemory")]
		public static extern FIMULTIBITMAP LoadMultiBitmapFromMemory(FREE_IMAGE_FORMAT fif, FIMEMORY stream, FREE_IMAGE_LOAD_FLAGS flags);

		#endregion

		#region Plugin functions

		/// <summary>
		/// Registers a new plugin to be used in FreeImage.
		/// </summary>
		/// <param name="proc_address">Pointer to the function that initialises the plugin.</param>
		/// <param name="format">A string describing the format of the plugin.</param>
		/// <param name="description">A string describing the plugin.</param>
		/// <param name="extension">A string witha comma sperated list of extensions. f.e: "pl,pl2,pl4"</param>
		/// <param name="regexpr">A regular expression used to identify the bitmap.</param>
		/// <returns>The format idientifier assigned by FreeImage.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_RegisterLocalPlugin")]
		public static extern FREE_IMAGE_FORMAT RegisterLocalPlugin(InitProc proc_address,
			string format, string description, string extension, string regexpr);

		/// <summary>
		/// Registers a new plugin to be used in FreeImage. The plugin is residing in a DLL.
		/// The Init function must be called �Init� and must use the stdcall calling convention.
		/// </summary>
		/// <param name="path">Complete path to the dll file hosting the plugin.</param>
		/// <param name="format">A string describing the format of the plugin.</param>
		/// <param name="description">A string describing the plugin.</param>
		/// <param name="extension">A string witha comma sperated list of extensions. f.e: "pl,pl2,pl4"</param>
		/// <param name="regexpr">A regular expression used to identify the bitmap.</param>
		/// <returns>The format idientifier assigned by FreeImage.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_RegisterExternalPlugin")]
		public static extern FREE_IMAGE_FORMAT RegisterExternalPlugin(string path,
			string format, string description, string extension, string regexpr);

		/// <summary>
		/// Retrieves the number of FREE_IMAGE_FORMAT identifiers being currently registered.
		/// </summary>
		/// <returns>The number of registered formats.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFCount")]
		public static extern int GetFIFCount();

		/// <summary>
		/// Enables or disables a plugin.
		/// </summary>
		/// <param name="fif">The plugin to enable or disable.</param>
		/// <param name="enable">True: enable the plugin. false: disable the plugin.</param>
		/// <returns>The previous state of the plugin.
		/// 1 - enabled. 0 - disables. -1 plugin does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPluginEnabled")]
		public static extern int SetPluginEnabled(FREE_IMAGE_FORMAT fif, bool enable);

		/// <summary>
		/// Retrieves the state of a plugin.
		/// </summary>
		/// <param name="fif">The plugin to check.</param>
		/// <returns>1 - enabled. 0 - disables. -1 plugin does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsPluginEnabled")]
		public static extern int IsPluginEnabled(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a FREE_IMAGE_FORMAT identifier from the format string that was used to register the FIF.
		/// </summary>
		/// <param name="format">The string that was used to register the plugin.</param>
		/// <returns>A FREE_IMAGE_FORMAT identifier from the format.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetFIFFromFormat")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromFormat(string format);

		/// <summary>
		/// Returns a FREE_IMAGE_FORMAT identifier from a MIME content type string
		/// (MIME stands for Multipurpose Internet Mail Extension).
		/// </summary>
		/// <param name="mime">A MIME content type.</param>
		/// <returns>A FREE_IMAGE_FORMAT identifier from the MIME.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetFIFFromMime")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromMime(string mime);

		/// <summary>
		/// Returns the string that was used to register a plugin from the system assigned FREE_IMAGE_FORMAT.
		/// </summary>
		/// <param name="fif">The assigned FREE_IMAGE_FORMAT.</param>
		/// <returns>The string that was used to register the plugin.</returns>
		public static unsafe string GetFormatFromFIF(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFormatFromFIF_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFormatFromFIF")]
		private static unsafe extern byte* GetFormatFromFIF_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a comma-delimited file extension list describing the bitmap formats the given plugin can read and/or write.
		/// </summary>
		/// <param name="fif">The desired FREE_IMAGE_FORMAT.</param>
		/// <returns>A comma-delimited file extension list.</returns>
		public static unsafe string GetFIFExtensionList(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFExtensionList_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFExtensionList")]
		private static unsafe extern byte* GetFIFExtensionList_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a descriptive string that describes the bitmap formats the given plugin can read and/or write.
		/// </summary>
		/// <param name="fif">The desired FREE_IMAGE_FORMAT.</param>
		/// <returns>A descriptive string that describes the bitmap formats.</returns>
		public static unsafe string GetFIFDescription(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFDescription_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFDescription")]
		private static unsafe extern byte* GetFIFDescription_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Returns a regular expression string that can be used by a regular expression engine to identify the bitmap.
		/// FreeImageQt makes use of this function.
		/// </summary>
		/// <param name="fif">The desired FREE_IMAGE_FORMAT.</param>
		/// <returns>A regular expression string.</returns>
		public static unsafe string GetFIFRegExpr(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFRegExpr_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFRegExpr")]
		private static unsafe extern byte* GetFIFRegExpr_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Given a FREE_IMAGE_FORMAT identifier, returns a MIME content type string (MIME stands for Multipurpose Internet Mail Extension).
		/// </summary>
		/// <param name="fif">The desired FREE_IMAGE_FORMAT.</param>
		/// <returns>A MIME content type string.</returns>
		public static unsafe string GetFIFMimeType(FREE_IMAGE_FORMAT fif) { return PtrToStr(GetFIFMimeType_(fif)); }
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFIFMimeType")]
		private static unsafe extern byte* GetFIFMimeType_(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// This function takes a filename or a file-extension and returns the plugin that can
		/// read/write files with that extension in the form of a FREE_IMAGE_FORMAT identifier.
		/// </summary>
		/// <param name="filename">The filename or -extension.</param>
		/// <returns>The FREE_IMAGE_FORMAT of the plugin.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFIFFromFilenameU")]
		public static extern FREE_IMAGE_FORMAT GetFIFFromFilename(string filename);

		/// <summary>
		/// This function takes a filename or a file-extension and returns the plugin that can
		/// read/write files with that extension in the form of a FREE_IMAGE_FORMAT identifier.
		/// Supports UNICODE filenames.
		/// </summary>
		/// <param name="filename">The filename or -extension.</param>
		/// <returns>The FREE_IMAGE_FORMAT of the plugin.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFIFFromFilenameU")]
		private static extern FREE_IMAGE_FORMAT GetFIFFromFilenameU(string filename);

		/// <summary>
		/// Checks if a plugin can load bitmaps.
		/// </summary>
		/// <param name="fif">The FREE_IMAGE_FORMAT of the plugin.</param>
		/// <returns>True if the plugin can load bitmaps, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsReading")]
		public static extern bool FIFSupportsReading(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Checks if a plugin can save bitmaps.
		/// </summary>
		/// <param name="fif">The FREE_IMAGE_FORMAT of the plugin.</param>
		/// <returns>True if the plugin can save bitmaps, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsWriting")]
		public static extern bool FIFSupportsWriting(FREE_IMAGE_FORMAT fif);

		/// <summary>
		/// Checks if a plugin can save bitmaps in the desired bit depth.
		/// </summary>
		/// <param name="fif">The FREE_IMAGE_FORMAT of the plugin.</param>
		/// <param name="bpp">The desired bit depth.</param>
		/// <returns>True if the plugin can save bitmaps in the desired bit depth, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsExportBPP")]
		public static extern bool FIFSupportsExportBPP(FREE_IMAGE_FORMAT fif, int bpp);

		/// <summary>
		/// Checks if a plugin can save a bitmap in the desired data type.
		/// </summary>
		/// <param name="fif">The FREE_IMAGE_FORMAT of the plugin.</param>
		/// <param name="type">The desired image type.</param>
		/// <returns>True if the plugin can save bitmaps as the desired type, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsExportType")]
		public static extern bool FIFSupportsExportType(FREE_IMAGE_FORMAT fif, FREE_IMAGE_TYPE type);

		/// <summary>
		/// Checks if a plugin can load or save an ICC profile.
		/// </summary>
		/// <param name="fif">The FREE_IMAGE_FORMAT of the plugin.</param>
		/// <returns>True if the plugin can load or save an ICC profile, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FIFSupportsICCProfiles")]
		public static extern bool FIFSupportsICCProfiles(FREE_IMAGE_FORMAT fif);

		#endregion

		#region Multipage functions

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="fif">Format of the image.</param>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMultiBitmap")]
		public static extern FIMULTIBITMAP OpenMultiBitmap(FREE_IMAGE_FORMAT fif, string filename, bool create_new,
				bool read_only, bool keep_cache_in_memory, FREE_IMAGE_LOAD_FLAGS flags);

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only, applies any changes made to it.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloseMultiBitmap")]
		public static extern bool CloseMultiBitmap(FIMULTIBITMAP bitmap, FREE_IMAGE_SAVE_FLAGS flags);

		/// <summary>
		/// Returns the number of pages currently available in the multi-paged bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Number of pages.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPageCount")]
		public static extern int GetPageCount(FIMULTIBITMAP bitmap);

		/// <summary>
		/// Appends a new page to the end of the bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AppendPage")]
		public static extern void AppendPage(FIMULTIBITMAP bitmap, FIBITMAP data);

		/// <summary>
		/// Inserts a new page before the given position in the bitmap.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Page has to be a number smaller than the current number of pages available in the bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_InsertPage")]
		public static extern void InsertPage(FIMULTIBITMAP bitmap, int page, FIBITMAP data);

		/// <summary>
		/// Deletes the page on the given position.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Number of the page to delete.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeletePage")]
		public static extern void DeletePage(FIMULTIBITMAP bitmap, int page);

		/// <summary>
		/// Locks a page in memory for editing.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="page">Number of the page to lock.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LockPage")]
		public static extern FIBITMAP LockPage(FIMULTIBITMAP bitmap, int page);

		/// <summary>
		/// Unlocks a previously locked page and gives it back to the multi-page engine.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="data">Handle to a FreeImage bitmap.</param>
		/// <param name="changed">If true, the page is applied to the multi-page bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_UnlockPage")]
		public static extern void UnlockPage(FIMULTIBITMAP bitmap, FIBITMAP data, bool changed);

		/// <summary>
		/// Moves the source page to the position of the target page.
		/// </summary>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="target">New position of the page.</param>
		/// <param name="source">Old position of the page.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MovePage")]
		public static extern bool MovePage(FIMULTIBITMAP bitmap, int target, int source);

		/// <summary>
		/// Returns an array of page-numbers that are currently locked in memory.
		/// When the pages parameter is null, the size of the array is returned in the count variable.
		/// </summary>
		/// <example>
		/// <code>
		/// int[] lockedPages = null;
		/// int count = 0;
		/// GetLockedPageNumbers(dib, lockedPages, ref count);
		/// lockedPages = new int[count];
		/// GetLockedPageNumbers(dib, lockedPages, ref count);
		/// </code>
		/// </example>
		/// <param name="bitmap">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="pages">The list of locked pages in the multi-pages bitmap.
		/// If set to 'null', count will contain the number of pages.</param>
		/// <param name="count">If 'pages' is set to 'null' count will contain the number of locked pages.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetLockedPageNumbers")]
		public static extern bool GetLockedPageNumbers(FIMULTIBITMAP bitmap, int[] pages, ref int count);

		#endregion

		#region Filetype functions

		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// </summary>
		/// <param name="filename">Name of the file to analyze.</param>
		/// <param name="size">Reserved parameter - use 0.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFileTypeU")]
		public static extern FREE_IMAGE_FORMAT GetFileType(string filename, int size);


		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// Supports UNICODE filenames.
		/// </summary>
		/// <param name="filename">Name of the file to analyze.</param>
		/// <param name="size">Reserved parameter - use 0.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Unicode, EntryPoint = "FreeImage_GetFileTypeU")]
		private static extern FREE_IMAGE_FORMAT GetFileTypeU(string filename, int size);

		/// <summary>
		/// Uses the FreeImageIO structure as described in the topic Bitmap management functions
		/// to identify a bitmap type.
		/// </summary>
		/// <param name="io">A FreeImageIO structure with functionpointers to handle the source.</param>
		/// <param name="handle">A handle to the source.</param>
		/// <param name="size">Size in bytes of the source.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFileTypeFromHandle")]
		public static extern FREE_IMAGE_FORMAT GetFileTypeFromHandle(ref FreeImageIO io, fi_handle handle, int size);

		/// <summary>
		/// Uses a memory handle to identify a bitmap type.
		/// </summary>
		/// <param name="stream">Pointer to the stream.</param>
		/// <param name="size">Size in bytes of the source.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFileTypeFromMemory")]
		public static extern FREE_IMAGE_FORMAT GetFileTypeFromMemory(FIMEMORY stream, int size);

		#endregion

		#region Helper functions

		/// <summary>
		/// Returns whether the platform is using Little Endian.
		/// </summary>
		/// <returns>Returns true if the platform is using Litte Endian, else false.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsLittleEndian")]
		public static extern bool IsLittleEndian();

		/// <summary>
		/// Converts a X11 color name into a corresponding RGB value.
		/// </summary>
		/// <param name="szColor">Name of the color to convert.</param>
		/// <param name="nRed">Red component.</param>
		/// <param name="nGreen">Green component.</param>
		/// <param name="nBlue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_LookupX11Color")]
		public static extern bool LookupX11Color(string szColor, out byte nRed, out byte nGreen, out byte nBlue);

		/// <summary>
		/// Converts a SVG color name into a corresponding RGB value.
		/// </summary>
		/// <param name="szColor">Name of the color to convert.</param>
		/// <param name="nRed">Red component.</param>
		/// <param name="nGreen">Green component.</param>
		/// <param name="nBlue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_LookupSVGColor")]
		public static extern bool LookupSVGColor(string szColor, out byte nRed, out byte nGreen, out byte nBlue);

		#endregion

		#region Pixel access functions

		/// <summary>
		/// Returns a pointer to the data-bits of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the data-bits.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBits")]
		public static extern IntPtr GetBits(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the start of the given scanline in the bitmap's data-bits.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Pointer to the scanline.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetScanLine")]
		public static extern IntPtr GetScanLine(FIBITMAP dib, int scanline);

		/// <summary>
		/// Get the pixel index of a palettized image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The pixel index.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPixelIndex")]
		public static extern bool GetPixelIndex(FIBITMAP dib, uint x, uint y, out byte value);

		/// <summary>
		/// Get the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The pixel color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPixelColor")]
		public static extern bool GetPixelColor(FIBITMAP dib, uint x, uint y, out RGBQUAD value);

		/// <summary>
		/// Set the pixel index of a palettized image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The new pixel index.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPixelIndex")]
		public static extern bool SetPixelIndex(FIBITMAP dib, uint x, uint y, ref byte value);

		/// <summary>
		/// Set the pixel color of a 16-, 24- or 32-bit image at position (x, y), including range check (slow access).
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="x">Pixel position in horizontal direction.</param>
		/// <param name="y">Pixel position in vertical direction.</param>
		/// <param name="value">The new pixel color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetPixelColor")]
		public static extern bool SetPixelColor(FIBITMAP dib, uint x, uint y, ref RGBQUAD value);

		#endregion

		#region Bitmap information functions

		/// <summary>
		/// Retrieves the type of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetImageType")]
		public static extern FREE_IMAGE_TYPE GetImageType(FIBITMAP dib);

		/// <summary>
		/// Returns the number of colors used in a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Palette-size for palletised bitmaps, and 0 for high-colour bitmaps.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetColorsUsed")]
		public static extern uint GetColorsUsed(FIBITMAP dib);

		/// <summary>
		/// Returns the size of one pixel in the bitmap in bits.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Size of one pixel in the bitmap in bits.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBPP")]
		public static extern uint GetBPP(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in pixel units.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetWidth")]
		public static extern uint GetWidth(FIBITMAP dib);

		/// <summary>
		/// Returns the height of the bitmap in pixel units.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Height of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetHeight")]
		public static extern uint GetHeight(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in bytes.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap in bytes.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetLine")]
		public static extern uint GetLine(FIBITMAP dib);

		/// <summary>
		/// Returns the width of the bitmap in bytes, rounded to the next 32-bit boundary,
		/// also known as pitch or stride or scan width.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>With of the bitmap in bytes.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPitch")]
		public static extern uint GetPitch(FIBITMAP dib);

		/// <summary>
		/// Returns the size of the DIB-element of a FIBITMAP in memory.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Size of the DIB-element</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDIBSize")]
		public static extern uint GetDIBSize(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the bitmap's palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the bitmap's palette.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetPalette")]
		public static extern IntPtr GetPalette(FIBITMAP dib);

		/// <summary>
		/// Returns the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The horizontal resolution, in pixels-per-meter.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDotsPerMeterX")]
		public static extern uint GetDotsPerMeterX(FIBITMAP dib);

		/// <summary>
		/// Returns the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The vertical resolution, in pixels-per-meter.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetDotsPerMeterY")]
		public static extern uint GetDotsPerMeterY(FIBITMAP dib);

		/// <summary>
		/// Set the horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new horizontal resolution.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetDotsPerMeterX")]
		public static extern void SetDotsPerMeterX(FIBITMAP dib, uint res);

		/// <summary>
		/// Set the vertical resolution, in pixels-per-meter, of the target device for the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new vertical resolution.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetDotsPerMeterY")]
		public static extern void SetDotsPerMeterY(FIBITMAP dib, uint res);

		/// <summary>
		/// Returns a pointer to the BITMAPINFOHEADER of the DIB-element in a FIBITMAP.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Poiter to the header of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetInfoHeader")]
		public static extern IntPtr GetInfoHeader(FIBITMAP dib);

		/// <summary>
		/// Alias for FreeImage_GetInfoHeader that returns a pointer to a BITMAPINFO
		/// rather than to a BITMAPINFOHEADER.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the BITMAPINFO structure for the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetInfo")]
		public static extern IntPtr GetInfo(FIBITMAP dib);

		/// <summary>
		/// Investigates the color type of the bitmap by reading the bitmap's pixel bits and analysing them.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The color type of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetColorType")]
		public static extern FREE_IMAGE_COLOR_TYPE GetColorType(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the red color component of a pixel in a FIBITMAP.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for RED.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetRedMask")]
		public static extern uint GetRedMask(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the green color component of a pixel in a FIBITMAP.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for green.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetGreenMask")]
		public static extern uint GetGreenMask(FIBITMAP dib);

		/// <summary>
		/// Returns a bit pattern describing the blue color component of a pixel in a FIBITMAP.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bit pattern for blue.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBlueMask")]
		public static extern uint GetBlueMask(FIBITMAP dib);

		/// <summary>
		/// Returns the number of transparent colors in a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The number of transparent colors in a palletised bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparencyCount")]
		public static extern uint GetTransparencyCount(FIBITMAP dib);

		/// <summary>
		/// Returns a pointer to the bitmap's transparency table.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the bitmap's transparency table.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparencyTable")]
		public static extern IntPtr GetTransparencyTable(FIBITMAP dib);

		/// <summary>
		/// Tells FreeImage if it should make use of the transparency table
		/// or the alpha channel that may accompany a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="enabled">True to enable the transparency, false to disable.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparent")]
		public static extern void SetTransparent(FIBITMAP dib, bool enabled);

		/// <summary>
		/// Set the bitmap's transparency table. Only affects palletised bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="table">Pointer to the bitmap's new transparency table.</param>
		/// <param name="count">The number of transparent colors in the new transparency table.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparencyTable")]
		internal static extern void SetTransparencyTable_(FIBITMAP dib, byte[] table, int count);

		/// <summary>
		/// Returns whether the transparency table is enabled.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true when the transparency table is enabled (1-, 4- or 8-bit images)
		/// or when the input dib contains alpha values (32-bit images). Returns false otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_IsTransparent")]
		public static extern bool IsTransparent(FIBITMAP dib);

		/// <summary>
		/// Returns whether the bitmap has a file background color.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true when the image has a file background color, false otherwise.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_HasBackgroundColor")]
		public static extern bool HasBackgroundColor(FIBITMAP dib);

		/// <summary>
		/// Returns the file background color of an image.
		/// For 8-bit images, the color index in the palette is returned in the
		/// rgbReserved member of the bkcolor parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The background color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetBackgroundColor")]
		public static extern bool GetBackgroundColor(FIBITMAP dib, out RGBQUAD bkcolor);

		/// <summary>
		/// Set the file background color of an image.
		/// When saving an image to PNG, this background color is transparently saved to the PNG file.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The new background color.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetBackgroundColor")]
		public static unsafe extern bool SetBackgroundColor(FIBITMAP dib, ref RGBQUAD bkcolor);

		/// <summary>
		/// Set the file background color of an image.
		/// When saving an image to PNG, this background color is transparently saved to the PNG file.
		/// When the bkcolor parameter is null, the background color is removed from the image.
		/// <para>
		/// This overloaded version of the function with an array parameter is provided to allow
		/// passing <c>null</c> in the <c>bkcolor</c> parameter. This is similar to the
		/// original C/C++ function. Passing <c>null</c> as <c>bkcolor</c> parameter will
		/// unset the dib's previously set background color.
		/// </para> 
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="bkcolor">The new background color.
		/// The first entry in the array is used.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <example>
		/// <code>
		/// // create a RGBQUAD color
		/// RGBQUAD color = new RGBQUAD(Color.Green);
		/// 
		/// // set the dib's background color (using the other version of the function)
		/// FreeImage.SetBackgroundColor(dib, ref color);
		/// 
		/// // remove it again (this only works due to the array parameter RGBQUAD[])
		/// FreeImage.SetBackgroundColor(dib, null);
		/// </code>
		/// </example>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetBackgroundColor")]
		public static unsafe extern bool SetBackgroundColor(FIBITMAP dib, RGBQUAD[] bkcolor);

		/// <summary>
		/// Sets the index of the palette entry to be used as transparent color
		/// for the image specified. Does nothing on high color images.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="index">The index of the palette entry to be set as transparent color.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTransparentIndex")]
		public static extern void SetTransparentIndex(FIBITMAP dib, int index);

		/// <summary>
		/// Returns the palette entry used as transparent color for the image specified.
		/// Works for palletised images only and returns -1 for high color
		/// images or if the image has no color set to be transparent.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>the index of the palette entry used as transparent color for
		/// the image specified or -1 if there is no transparent color found
		/// (e.g. the image is a high color image).</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTransparentIndex")]
		public static extern int GetTransparentIndex(FIBITMAP dib);

		#endregion

		#region ICC profile functions

		/// <summary>
		/// Retrieves the FIICCPROFILE data of the bitmap.
		/// This function can also be called safely, when the original format does not support profiles.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The FIICCPROFILE data of the bitmap.</returns>
		public static FIICCPROFILE GetICCProfileEx(FIBITMAP dib) { unsafe { return *(FIICCPROFILE*)FreeImage.GetICCProfile(dib); } }

		/// <summary>
		/// Retrieves a pointer to the FIICCPROFILE data of the bitmap.
		/// This function can also be called safely, when the original format does not support profiles.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pointer to the FIICCPROFILE data of the bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetICCProfile")]
		public static extern IntPtr GetICCProfile(FIBITMAP dib);

		/// <summary>
		/// Creates a new FIICCPROFILE block from ICC profile data previously read from a file
		/// or built by a color management system. The profile data is attached to the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">Pointer to the new FIICCPROFILE data.</param>
		/// <param name="size">Size of the FIICCPROFILE data.</param>
		/// <returns>Pointer to the created FIICCPROFILE structure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CreateICCProfile")]
		public static extern IntPtr CreateICCProfile(FIBITMAP dib, byte[] data, int size);

		/// <summary>
		/// This function destroys an FIICCPROFILE previously created by FreeImage_CreateICCProfile.
		/// After this call the bitmap will contain no profile information.
		/// This function should be called to ensure that a stored bitmap will not contain any profile information.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DestroyICCProfile")]
		public static extern void DestroyICCProfile(FIBITMAP dib);

		#endregion

		#region Internal Functions

		/*
		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine1To4")]
		public static extern void ConvertLine1To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine8To4")]
		public static extern void ConvertLine8To4(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To4_555")]
		public static extern void ConvertLine16To4_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To4_565")]
		public static extern void ConvertLine16To4_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine24To4")]
		public static extern void ConvertLine24To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine32To4")]
		public static extern void ConvertLine32To4(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine1To8")]
		public static extern void ConvertLine1To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine4To8")]
		public static extern void ConvertLine4To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To8_555")]
		public static extern void ConvertLine16To8_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine16To8_565")]
		public static extern void ConvertLine16To8_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImage_ConvertLine24To8")]
		public static extern void ConvertLine24To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To8")]
		public static extern void ConvertLine32To8(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To16_555")]
		public static extern void ConvertLine1To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To16_555")]
		public static extern void ConvertLine4To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To16_555")]
		public static extern void ConvertLine8To16_555(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16_565_To16_555")]
		public static extern void ConvertLine16_565_To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To16_555")]
		public static extern void ConvertLine24To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To16_555")]
		public static extern void ConvertLine32To16_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To16_565")]
		public static extern void ConvertLine1To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To16_565")]
		public static extern void ConvertLine4To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To16_565")]
		public static extern void ConvertLine8To16_565(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16_555_To16_565")]
		public static extern void ConvertLine16_555_To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To16_565")]
		public static extern void ConvertLine24To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To16_565")]
		public static extern void ConvertLine32To16_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To24")]
		public static extern void ConvertLine1To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To24")]
		public static extern void ConvertLine4To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To24")]
		public static extern void ConvertLine8To24(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To24_555")]
		public static extern void ConvertLine16To24_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To24_565")]
		public static extern void ConvertLine16To24_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine32To24")]
		public static extern void ConvertLine32To24(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine1To32")]
		public static extern void ConvertLine1To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine4To32")]
		public static extern void ConvertLine4To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine8To32")]
		public static extern void ConvertLine8To32(ref byte target, ref byte source, int width_in_pixels, ref RGBQUAD palette);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To32_555")]
		public static extern void ConvertLine16To32_555(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine16To32_565")]
		public static extern void ConvertLine16To32_565(ref byte target, ref byte source, int width_in_pixels);

		[DllImport(dllName, EntryPoint = "FreeImageonvertLine24To32")]
		public static extern void ConvertLine24To32(ref byte target, ref byte source, int width_in_pixels);

		*/

		#endregion

		#region Conversion functions

		/// <summary>
		/// Converts a bitmap to 4 bits.
		/// If the bitmap was a high-color bitmap (16, 24 or 32-bit) or if it was a
		/// monochrome or greyscale bitmap (1 or 8-bit), the end result will be a
		/// greyscale bitmap, otherwise (1-bit palletised bitmaps) it will be a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo4Bits")]
		public static extern FIBITMAP ConvertTo4Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 8 bits. If the bitmap was a high-color bitmap (16, 24 or 32-bit)
		/// or if it was a monochrome or greyscale bitmap (1 or 4-bit), the end result will be a
		/// greyscale bitmap, otherwise (1 or 4-bit palletised bitmaps) it will be a palletised bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo8Bits")]
		public static extern FIBITMAP ConvertTo8Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to a 8-bit greyscale image with a linear ramp.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToGreyscale")]
		public static extern FIBITMAP ConvertToGreyscale(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 16 bits, where each pixel has a color pattern of
		/// 5 bits red, 5 bits green and 5 bits blue. One bit in each pixel is unused.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo16Bits555")]
		public static extern FIBITMAP ConvertTo16Bits555(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 16 bits, where each pixel has a color pattern of
		/// 5 bits red, 6 bits green and 5 bits blue.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo16Bits565")]
		public static extern FIBITMAP ConvertTo16Bits565(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 24 bits. A clone of the input bitmap is returned for 24-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo24Bits")]
		public static extern FIBITMAP ConvertTo24Bits(FIBITMAP dib);

		/// <summary>
		/// Converts a bitmap to 32 bits. A clone of the input bitmap is returned for 32-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo32Bits")]
		public static extern FIBITMAP ConvertTo32Bits(FIBITMAP dib);

		/// <summary>
		/// Quantizes a high-color 24-bit bitmap to an 8-bit palette color bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="quantize">Specifies the color reduction algorithm to be used.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ColorQuantize")]
		public static extern FIBITMAP ColorQuantize(FIBITMAP dib, FREE_IMAGE_QUANTIZE quantize);

		/// <summary>
		/// ColorQuantizeEx is an extension to the FreeImage_ColorQuantize function that
		/// provides additional options used to quantize a 24-bit image to any
		/// number of colors (up to 256), as well as quantize a 24-bit image using a
		/// partial or full provided palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="quantize">Specifies the color reduction algorithm to be used.</param>
		/// <param name="PaletteSize">Size of the desired output palette.</param>
		/// <param name="ReserveSize">Size of the provided palette of ReservePalette.</param>
		/// <param name="ReservePalette">The provided palette.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ColorQuantizeEx")]
		public static extern FIBITMAP ColorQuantizeEx(FIBITMAP dib, FREE_IMAGE_QUANTIZE quantize, int PaletteSize, int ReserveSize, RGBQUAD[] ReservePalette);
		//public static extern FIBITMAP ColorQuantizeEx(FIBITMAP dib, FREE_IMAGE_QUANTIZE quantize, int PaletteSize, int ReserveSize, IntPtr ReservePalette);

		/// <summary>
		/// Converts a bitmap to 1-bit monochrome bitmap using a threshold T between [0..255].
		/// The function first converts the bitmap to a 8-bit greyscale bitmap.
		/// Then, any brightness level that is less than T is set to zero, otherwise to 1.
		/// For 1-bit input bitmaps, the function clones the input bitmap and builds a monochrome palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="t">The threshold.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Threshold")]
		public static extern FIBITMAP Threshold(FIBITMAP dib, byte t);

		/// <summary>
		/// Converts a bitmap to 1-bit monochrome bitmap using a dithering algorithm.
		/// For 1-bit input bitmaps, the function clones the input bitmap and builds a monochrome palette.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="algorithm">The dithering algorithm to use.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Dither")]
		public static extern FIBITMAP Dither(FIBITMAP dib, FREE_IMAGE_DITHER algorithm);

		/// <summary>
		/// Converts a raw bitmap somewhere in memory to a FIBITMAP.
		/// The parameters in this function are used to describe the raw bitmap.
		/// </summary>
		/// <param name="bits">Pointer to start of the raw bits.</param>
		/// <param name="width">Width of the bitmap.</param>
		/// <param name="height">Height of the bitmap.</param>
		/// <param name="pitch">Defines the total width of a scanline in the source bitmap,
		/// including padding bytes that may be applied.</param>
		/// <param name="bpp">The bit depth of the bitmap.</param>
		/// <param name="red_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="green_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="blue_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="topdown">Stores the bitmap top-left pixel first when it is true
		/// or bottom-left pixel first when it is false</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertFromRawBits")]
		public static extern FIBITMAP ConvertFromRawBits(IntPtr bits, int width, int height, int pitch,
				uint bpp, uint red_mask, uint green_mask, uint blue_mask, bool topdown);

		/// <summary>
		/// Converts a FIBITMAP to a raw piece of memory.
		/// </summary>
		/// <param name="bits">Pointer to the start of the raw bits.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="pitch">Defines the total width of a scanline in the source bitmap,
		/// including padding bytes that may be applied.</param>
		/// <param name="bpp">The bit depth of the bitmap.</param>
		/// <param name="red_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="green_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="blue_mask">The bit-layout of the color components in the bitmap.</param>
		/// <param name="topdown">Store the bitmap top-left pixel first when it is true
		/// or bottom-left pixel first when it is false.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRawBits")]
		public static extern void ConvertToRawBits(IntPtr bits, FIBITMAP dib, int pitch, uint bpp,
				uint red_mask, uint green_mask, uint blue_mask, bool topdown);

		/// <summary>
		/// Converts a 24- or 32-bit RGB(A) standard image or a 48-bit RGB image to a FIT_RGBF type image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRGBF")]
		public static extern FIBITMAP ConvertToRGBF(FIBITMAP dib);

		/// <summary>
		/// Converts a non standard image whose color type is FIC_MINISBLACK
		/// to a standard 8-bit greyscale image.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="scale_linear">When true the conversion is done by scaling linearly
		/// each pixel value from [min, max] to an integer value between [0..255],
		/// where min and max are the minimum and maximum pixel values in the image.
		/// When false the conversion is done by rounding each pixel value to an integer between [0..255].
		///
		/// Rounding is done using the following formula:
		///
		/// dst_pixel = (BYTE) MIN(255, MAX(0, q)) where int q = int(src_pixel + 0.5);</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToStandardType")]
		public static extern FIBITMAP ConvertToStandardType(FIBITMAP src, bool scale_linear);

		/// <summary>
		/// Converts an image of any type to type dst_type.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="dst_type">Destination type.</param>
		/// <param name="scale_linear">True to scale linear, else false.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToType")]
		public static extern FIBITMAP ConvertToType(FIBITMAP src, FREE_IMAGE_TYPE dst_type, bool scale_linear);

		#endregion

		#region Tone mapping operators

		/// <summary>
		/// Converts a High Dynamic Range image (48-bit RGB or 96-bit RGBF) to a 24-bit RGB image, suitable for display.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tmo">The tone mapping operator to be used.</param>
		/// <param name="first_param">Parmeter depending on the used algorithm</param>
		/// <param name="second_param">Parmeter depending on the used algorithm</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ToneMapping")]
		public static extern FIBITMAP ToneMapping(FIBITMAP dib, FREE_IMAGE_TMO tmo, double first_param, double second_param);

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global
		/// operator based on logarithmic compression of luminance values, imitating the human response to light.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="gamma">A gamma correction that is applied after the tone mapping.
		/// A value of 1 means no correction.</param>
		/// <param name="exposure">Scale factor allowing to adjust the brightness of the output image.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoDrago03")]
		public static extern FIBITMAP TmoDrago03(FIBITMAP src, double gamma, double exposure);

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global operator inspired
		/// by photoreceptor physiology of the human visual system.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="intensity">Controls the overall image intensity in the range [-8, 8].</param>
		/// <param name="contrast">Controls the overall image contrast in the range [0.3, 1.0[.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoReinhard05")]
		public static extern FIBITMAP TmoReinhard05(FIBITMAP src, double intensity, double contrast);

		/// <summary>
		/// Apply the Gradient Domain High Dynamic Range Compression to a RGBF image and convert to 24-bit RGB.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="color_saturation">Color saturation (s parameter in the paper) in [0.4..0.6]</param>
		/// <param name="attenuation">Atenuation factor (beta parameter in the paper) in [0.8..0.9]</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_TmoFattal02")]
		public static extern FIBITMAP TmoFattal02(FIBITMAP src, double color_saturation, double attenuation);

		#endregion

		#region Compression functions

		/// <summary>
		/// Compresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must be at least 0.1% larger than source_size plus 12 bytes.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the compressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibCompress")]
		public static extern uint ZLibCompress(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Decompresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must have been saved outlide of zlib.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the uncompressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibUncompress")]
		public static extern uint ZLibUncompress(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Compresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must be at least 0.1% larger than source_size plus 24 bytes.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the compressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibGZip")]
		public static extern uint ZLibGZip(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Decompresses a source buffer into a target buffer, using the ZLib library.
		/// </summary>
		/// <param name="target">Pointer to the target buffer.</param>
		/// <param name="target_size">Size of the target buffer.
		/// Must have been saved outlide of zlib.</param>
		/// <param name="source">Pointer to the source buffer.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns>The actual size of the uncompressed buffer, or 0 if an error occurred.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibGUnzip")]
		public static extern uint ZLibGUnzip(byte[] target, uint target_size, byte[] source, uint source_size);

		/// <summary>
		/// Generates a CRC32 checksum.
		/// </summary>
		/// <param name="crc">The CRC32 checksum to begin with.</param>
		/// <param name="source">Pointer to the source buffer.
		/// If the value is 0, the function returns the required initial value for the crc.</param>
		/// <param name="source_size">Size of the source buffer.</param>
		/// <returns></returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ZLibCRC32")]
		public static extern uint ZLibCRC32(uint crc, byte[] source, uint source_size);

		#endregion

		#region Tag creation and destruction

		/// <summary>
		/// Allocates a new FITAG object.
		/// This object must be destroyed with a call to FreeImage_DeleteTag when no longer in use.
		/// </summary>
		/// <returns>The new FITAG.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CreateTag")]
		public static extern FITAG CreateTag();

		/// <summary>
		/// Delete a previously allocated FITAG object.
		/// </summary>
		/// <param name="tag">The FITAG to destroy.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_DeleteTag")]
		public static extern void DeleteTag(FITAG tag);

		/// <summary>
		/// Creates and returns a copy of a FITAG object.
		/// </summary>
		/// <param name="tag">The FITAG to clone.</param>
		/// <returns>The new FITAG.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloneTag")]
		public static extern FITAG CloneTag(FITAG tag);

		#endregion

		#region Tag accessors

		/// <summary>
		/// Returns the tag field name (unique inside a metadata model).
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The field name.</returns>
		public static unsafe string GetTagKey(FITAG tag) { return PtrToStr(GetTagKey_(tag)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetTagKey")]
		private static unsafe extern byte* GetTagKey_(FITAG tag);

		/// <summary>
		/// Returns the tag description.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The description or NULL if unavailable.</returns>
		public static unsafe string GetTagDescription(FITAG tag) { return PtrToStr(GetTagDescription_(tag)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetTagDescription")]
		private static unsafe extern byte* GetTagDescription_(FITAG tag);

		/// <summary>
		/// Returns the tag ID.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The ID or 0 if unavailable.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagID")]
		public static extern ushort GetTagID(FITAG tag);

		/// <summary>
		/// Returns the tag data type.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The tag type.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagType")]
		public static extern FREE_IMAGE_MDTYPE GetTagType(FITAG tag);

		/// <summary>
		/// Returns the number of components in the tag (in tag type units).
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The number of components.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagCount")]
		public static extern uint GetTagCount(FITAG tag);

		/// <summary>
		/// Returns the length of the tag value in bytes.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>The length of the tag value.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagLength")]
		public static extern uint GetTagLength(FITAG tag);

		/// <summary>
		/// Returns the tag value.
		/// It is up to the programmer to interpret the returned pointer correctly,
		/// according to the results of GetTagType and GetTagCount.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <returns>Pointer to the value.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetTagValue")]
		public static extern IntPtr GetTagValue(FITAG tag);

		/// <summary>
		/// Sets the tag field name.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="key">The new name.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetTagKey")]
		public static extern bool SetTagKey(FITAG tag, string key);

		/// <summary>
		/// Sets the tag description.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="description">The new description.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetTagDescription")]
		public static extern bool SetTagDescription(FITAG tag, string description);

		/// <summary>
		/// Sets the tag ID.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="id">The new ID.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagID")]
		public static extern bool SetTagID(FITAG tag, ushort id);

		/// <summary>
		/// Sets the tag data type.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="type">The new type.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagType")]
		public static extern bool SetTagType(FITAG tag, FREE_IMAGE_MDTYPE type);

		/// <summary>
		/// Sets the number of data in the tag.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="count">New number of data.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagCount")]
		public static extern bool SetTagCount(FITAG tag, uint count);

		/// <summary>
		/// Sets the length of the tag value in bytes.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="length">The new length.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagLength")]
		public static extern bool SetTagLength(FITAG tag, uint length);

		/// <summary>
		/// Sets the tag value.
		/// </summary>
		/// <param name="tag">The tag field.</param>
		/// <param name="value">Pointer to the new value.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetTagValue")]
		public static extern bool SetTagValue(FITAG tag, byte[] value);
		//public static extern bool SetTagValue(FITAG tag, IntPtr value);

		#endregion

		#region Metadata iterator

		/// <summary>
		/// Provides information about the first instance of a tag that matches the metadata model.
		/// </summary>
		/// <param name="model">The model to match.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Unique search handle that can be used to call FindNextMetadata or FindCloseMetadata.
		/// Null if the metadata model does not exist.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindFirstMetadata")]
		public static extern FIMETADATA FindFirstMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, out FITAG tag);

		/// <summary>
		/// Find the next tag, if any, that matches the metadata model argument in a previous call
		/// to FindFirstMetadata, and then alters the tag object contents accordingly.
		/// </summary>
		/// <param name="mdhandle">Unique search handle provided by FindFirstMetadata.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindNextMetadata")]
		public static extern bool FindNextMetadata(FIMETADATA mdhandle, out FITAG tag);

		/// <summary>
		/// Closes the specified metadata search handle and releases associated resources.
		/// </summary>
		/// <param name="mdhandle">The handle to close.</param>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FindCloseMetadata")]
		private static extern void FindCloseMetadata_(FIMETADATA mdhandle);

		#endregion

		#region Metadata setter and getter

		/// <summary>
		/// Retrieve a metadata attached to a dib.
		/// </summary>
		/// <param name="model">The metadata model to look for.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The metadata field name.</param>
		/// <param name="tag">A FITAG structure returned by the function.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_GetMetadata")]
		public static extern bool GetMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, string key, out FITAG tag);

		/// <summary>
		/// Attach a new FreeImage tag to a dib.
		/// </summary>
		/// <param name="model">The metadata model used to store the tag.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The tag field name.</param>
		/// <param name="tag">The FreeImage tag to be attached.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_SetMetadata")]
		public static extern bool SetMetadata(FREE_IMAGE_MDMODEL model, FIBITMAP dib, string key, FITAG tag);

		#endregion

		#region Metadata helper functions

		/// <summary>
		/// Returns the number of tags contained in the model metadata model attached to the input dib.
		/// </summary>
		/// <param name="model">The metadata model.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Number of tags contained in the metadata model.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetMetadataCount")]
		public static extern uint GetMetadataCount(FREE_IMAGE_MDMODEL model, FIBITMAP dib);

		/// <summary>
		/// Converts a FreeImage tag structure to a string that represents the interpreted tag value.
		/// The function is not thread safe.
		/// </summary>
		/// <param name="model">The metadata model.</param>
		/// <param name="tag">The interpreted tag value.</param>
		/// <param name="Make">Reserved.</param>
		/// <returns>The representing string.</returns>
		public static unsafe string TagToString(FREE_IMAGE_MDMODEL model, FITAG tag, uint Make) { return PtrToStr(TagToString_(model, tag, Make)); }
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_TagToString")]
		private static unsafe extern byte* TagToString_(FREE_IMAGE_MDMODEL model, FITAG tag, uint Make);

		#endregion

		#region Rotation and flipping

		/// <summary>
		/// This function rotates a 1-, 8-bit greyscale or a 24-, 32-bit color image by means of 3 shears.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Handle to a FreeImage bitmap.
		/// 1-bit images rotation is limited to integer multiple of 90�.
		/// Null is returned for other values.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_RotateClassic")]
		public static extern FIBITMAP RotateClassic(FIBITMAP dib, double angle);

		/// <summary>
		/// This function performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="x_shift">Horizontal image translation.</param>
		/// <param name="y_shift">Vertical image translation.</param>
		/// <param name="x_origin">Rotation center x-coordinate.</param>
		/// <param name="y_origin">Rotation center y-coordinate.</param>
		/// <param name="use_mask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_RotateEx")]
		public static extern FIBITMAP RotateEx(FIBITMAP dib, double angle,
			double x_shift, double y_shift, double x_origin, double y_origin, bool use_mask);

		/// <summary>
		/// Flip the input dib horizontally along the vertical axis.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FlipHorizontal")]
		public static extern bool FlipHorizontal(FIBITMAP dib);

		/// <summary>
		/// Flip the input dib vertically along the horizontal axis.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_FlipVertical")]
		public static extern bool FlipVertical(FIBITMAP dib);

		/// <summary>
		/// Performs a lossless rotation or flipping on a JPEG file.
		/// </summary>
		/// <param name="src_file">Source file.</param>
		/// <param name="dst_file">Destination file; can be the source file; will be overwritten.</param>
		/// <param name="operation">The operation to apply.</param>
		/// <param name="perfect">To avoid lossy transformation, you can set the perfect parameter to true.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_JPEGTransform")]
		public static extern bool JPEGTransform(string src_file, string dst_file,
			FREE_IMAGE_JPEG_OPERATION operation, bool perfect);

		#endregion

		#region Upsampling / downsampling

		/// <summary>
		/// Performs resampling (or scaling, zooming) of a greyscale or RGB(A) image
		/// to the desired destination width and height.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="dst_width">Destination width.</param>
		/// <param name="dst_height">Destination height.</param>
		/// <param name="filter">The filter to apply.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Rescale")]
		public static extern FIBITMAP Rescale(FIBITMAP dib, int dst_width, int dst_height, FREE_IMAGE_FILTER filter);

		/// <summary>
		/// Creates a thumbnail from a greyscale or RGB(A) image, keeping aspect ratio.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="max_pixel_size">Thumbnail square size.</param>
		/// <param name="convert">When true HDR images are transperantly converted to standard images.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MakeThumbnail")]
		public static extern FIBITMAP MakeThumbnail(FIBITMAP dib, int max_pixel_size, bool convert);

		#endregion

		#region Color manipulation

		/// <summary>
		/// Perfoms an histogram transformation on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="LUT">The lookup table (LUT).
		/// It's size is assumed to be 256 in length.</param>
		/// <param name="channel">The color channel to be transformed.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustCurve")]
		public static extern bool AdjustCurve(FIBITMAP dib, byte[] LUT, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Performs gamma correction on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="gamma">The parameter represents the gamma value to use (gamma > 0).
		/// A value of 1.0 leaves the image alone, less than one darkens it, and greater than one lightens it.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustGamma")]
		public static extern bool AdjustGamma(FIBITMAP dib, double gamma);

		/// <summary>
		/// Adjusts the brightness of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will make the image darker and greater than 0 will make the image brighter.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustBrightness")]
		public static extern bool AdjustBrightness(FIBITMAP dib, double percentage);

		/// <summary>
		/// Adjusts the contrast of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will decrease the contrast and greater than 0 will increase the contrast of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustContrast")]
		public static extern bool AdjustContrast(FIBITMAP dib, double percentage);

		/// <summary>
		/// Inverts each pixel data.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Invert")]
		public static extern bool Invert(FIBITMAP dib);

		/// <summary>
		/// Computes the image histogram.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="histo">Array of integers with a size of 256.</param>
		/// <param name="channel">Channel to compute from.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetHistogram")]
		public static extern bool GetHistogram(FIBITMAP dib, int[] histo, FREE_IMAGE_COLOR_CHANNEL channel);

		#endregion

		#region Channel processing

		/// <summary>
		/// Retrieves the red, green, blue or alpha channel of a 24- or 32-bit image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetChannel")]
		public static extern FIBITMAP GetChannel(FIBITMAP dib, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Insert a 8-bit dib into a 24- or 32-bit image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="dib8">Handle to the bitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetChannel")]
		public static extern bool SetChannel(FIBITMAP dib, FIBITMAP dib8, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Retrieves the real part, imaginary part, magnitude or phase of a complex image.
		/// </summary>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetComplexChannel")]
		public static extern FIBITMAP GetComplexChannel(FIBITMAP src, FREE_IMAGE_COLOR_CHANNEL channel);

		/// <summary>
		/// Set the real or imaginary part of a complex image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="dst">Handle to a FreeImage bitmap.</param>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SetComplexChannel")]
		public static extern bool SetComplexChannel(FIBITMAP dst, FIBITMAP src, FREE_IMAGE_COLOR_CHANNEL channel);

		#endregion

		#region Copy / Paste / Composite routines

		/// <summary>
		/// Copy a sub part of the current dib image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Copy")]
		public static extern FIBITMAP Copy(FIBITMAP dib, int left, int top, int right, int bottom);

		/// <summary>
		/// Alpha blend or combine a sub part image with the current dib image.
		/// The bit depth of the dst bitmap must be greater than or equal to the bit depth of the src.
		/// </summary>
		/// <param name="dst">Handle to a FreeImage bitmap.</param>
		/// <param name="src">Handle to a FreeImage bitmap.</param>
		/// <param name="left">Specifies the left position of the sub image.</param>
		/// <param name="top">Specifies the top position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Paste")]
		public static extern bool Paste(FIBITMAP dst, FIBITMAP src, int left, int top, int alpha);

		/// <summary>
		/// This function composite a transparent foreground image against a single background color or
		/// against a background image.
		/// </summary>
		/// <param name="fg">Handle to a FreeImage bitmap.</param>
		/// <param name="useFileBkg">When true the background of fg is used if it contains one.</param>
		/// <param name="appBkColor">The application background is used if useFileBkg is false.</param>
		/// <param name="bg">Image used as background when useFileBkg is false or fg has no background
		/// and appBkColor is null.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Composite")]
		public static extern FIBITMAP Composite(FIBITMAP fg, bool useFileBkg, ref RGBQUAD appBkColor, FIBITMAP bg);

		/// <summary>
		/// This function composite a transparent foreground image against a single background color or
		/// against a background image.
		/// </summary>
		/// <param name="fg">Handle to a FreeImage bitmap.</param>
		/// <param name="useFileBkg">When true the background of fg is used if it contains one.</param>
		/// <param name="appBkColor">The application background is used if useFileBkg is false
		/// and 'appBkColor' is not null.</param>
		/// <param name="bg">Image used as background when useFileBkg is false or fg has no background
		/// and appBkColor is null.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Composite")]
		public static extern FIBITMAP Composite(FIBITMAP fg, bool useFileBkg, RGBQUAD[] appBkColor, FIBITMAP bg);

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="src_file">Source filename.</param>
		/// <param name="dst_file">Destination filename.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, CharSet = CharSet.Ansi, EntryPoint = "FreeImage_JPEGCrop")]
		public static extern bool JPEGCrop(string src_file, string dst_file, int left, int top, int right, int bottom);

		/// <summary>
		/// Applies the alpha value of each pixel to its color components.
		/// The aplha value stays unchanged.
		/// Only works with 32-bits color depth.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_PreMultiplyWithAlpha")]
		public static extern bool PreMultiplyWithAlpha(FIBITMAP dib);

		#endregion

		#region Miscellaneous algorithms

		/// <summary>
		/// Solves a Poisson equation, remap result pixels to [0..1] and returns the solution.
		/// </summary>
		/// <param name="Laplacian">Handle to a FreeImage bitmap.</param>
		/// <param name="ncycle">Number of cycles in the multigrid algorithm (usually 2 or 3)</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_MultigridPoissonSolver")]
		public static extern FIBITMAP MultigridPoissonSolver(FIBITMAP Laplacian, int ncycle);

		#endregion

		#region Colors

		/// <summary>
		/// Creates a lookup table to be used with FreeImage_AdjustCurve() which
		/// may adjusts brightness and contrast, correct gamma and invert the image with a
		/// single call to FreeImage_AdjustCurve().
		/// </summary>
		/// <param name="LUT">Output lookup table to be used with FreeImage_AdjustCurve().
		/// The size of 'LUT' is assumed to be 256.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para></param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>The number of adjustments applied to the resulting lookup table
		/// compared to a blind lookup table.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetAdjustColorsLookupTable")]
		public static extern int GetAdjustColorsLookupTable(byte[] LUT, double brightness, double contrast, double gamma, bool invert);

		/// <summary>
		/// Adjusts an image's brightness, contrast and gamma as well as it may
		/// optionally invert the image within a single operation.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para>
		/// This parameter must not be zero or smaller than zero.
		/// If so, it will be ignored and no gamma correction will be performed on the image.</param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AdjustColors")]
		public static extern bool AdjustColors(FIBITMAP dib, double brightness, double contrast, double gamma, bool invert);

		/// <summary>
		/// Applies color mapping for one or several colors on a 1-, 4- or 8-bit
		/// palletized or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="srccolors">Array of colors to be used as the mapping source.</param>
		/// <param name="dstcolors">Array of colors to be used as the mapping destination.</param>
		/// <param name="count">The number of colors to be mapped. This is the size of both
		/// srccolors and dstcolors.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <param name="swap">If true, source and destination colors are swapped, that is,
		/// each destination color is also mapped to the corresponding source color.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ApplyColorMapping")]
		public static extern uint ApplyColorMapping(FIBITMAP dib, RGBQUAD[] srccolors, RGBQUAD[] dstcolors, uint count, bool ignore_alpha, bool swap);

		/// <summary>
		/// Swaps two specified colors on a 1-, 4- or 8-bit palletized
		/// or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="color_a">One of the two colors to be swapped.</param>
		/// <param name="color_b">The other of the two colors to be swapped.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SwapColors")]
		public static extern uint SwapColors(FIBITMAP dib, ref RGBQUAD color_a, ref RGBQUAD color_b, bool ignore_alpha);

		/// <summary>
		/// Applies palette index mapping for one or several indices
		/// on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="srcindices">Array of palette indices to be used as the mapping source.</param>
		/// <param name="dstindices">Array of palette indices to be used as the mapping destination.</param>
		/// <param name="count">The number of palette indices to be mapped. This is the size of both
		/// srcindices and dstindices</param>
		/// <param name="swap">If true, source and destination palette indices are swapped, that is,
		/// each destination index is also mapped to the corresponding source index.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ApplyPaletteIndexMapping")]
		public static extern uint ApplyPaletteIndexMapping(FIBITMAP dib, byte[] srcindices, byte[] dstindices, uint count, bool swap);

		/// <summary>
		/// Swaps two specified palette indices on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="index_a">One of the two palette indices to be swapped.</param>
		/// <param name="index_b">The other of the two palette indices to be swapped.</param>
		/// <returns>The total number of pixels changed.</returns>
		[DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SwapPaletteIndices")]
		public static extern uint SwapPaletteIndices(FIBITMAP dib, ref byte index_a, ref byte index_b);

		#endregion
	}
}

/////////////////////////////////////////////////////
//                                                 //
//               Wrapper functions                 //
//                                                 //
/////////////////////////////////////////////////////

namespace FreeImageAPI
{
	#region Structs

	// RGBQUADARRAY, RGBTRIPLEARRAY, FIRGB16ARRAY, FIRGBFARRAY and FIRGBAFARRAY
	// are structures that wrap a block of memory. Each structure has its own
	// definitions (size, color position ect).
	//
	// In unmanaged code pointers would be used to access a bitmaps data.
	// In .NET unsafe code is needed to perform the same operations.
	// All mentioned structures have the same core methods and properties.
	// They return Colors or the original structure (RGBQUADARRAY returns
	// RGBQUADs ect) that can be altered and then saved back to the bitmap
	// (always keep in mind, that the structure returns a copy of the block
	// of memory; any change made to it must be saved back or will be lost).
	//
	// These structures provide a comfortable way of accessing a bitmaps data.
	// The structure is defined by the base address of the memory-block to wrap
	// and the !number of elements! the block contains, not the size of the block
	// in bytes.
	// Keep in mind that the structure accepts any length and can be used to
	// write in memory that does not belong to the bitmap, which will lead to
	// data loss.

	/// <summary>
	/// The structure wraps all operations needed to work with an array of RGBQUADs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(RGBQUADARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct RGBQUADARRAY : IComparable, IComparable<RGBQUADARRAY>, IEnumerable, IEquatable<RGBQUADARRAY>
	{
		readonly RGBQUAD* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an RGBQUADARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public RGBQUADARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (RGBQUAD*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an RGBQUADARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 32bpp.</exception>
		public RGBQUADARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 32)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (RGBQUAD*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		/// <summary>
		/// Creates an RGBQUADARRAY structure.
		/// In case the bitmap has a palette this will be wrapped.
		/// Otherwise the bitmap must be a 32-bit color bitmap and its first
		/// scanline will be wrapped.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 32bpp and has no palette.</exception>
		public RGBQUADARRAY(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			uint colorsUsed = FreeImage.GetColorsUsed(dib);
			if (colorsUsed != 0)
			{
				baseAddress = (RGBQUAD*)FreeImage.GetPalette(dib);
				length = colorsUsed;
			}
			else
			{
				if (FreeImage.GetBPP(dib) != 32)
				{
					throw new ArgumentException("dib");
				}
				baseAddress = (RGBQUAD*)FreeImage.GetScanLine(dib, 0);
				length = FreeImage.GetWidth(dib);
			}
		}

		public static bool operator ==(RGBQUADARRAY value1, RGBQUADARRAY value2)
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
				sizeof(RGBQUAD) * value1.length);
		}

		public static bool operator !=(RGBQUADARRAY value1, RGBQUADARRAY value2)
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
		/// Gets or sets the RGBQUAD structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>RGBQUAD structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBQUAD this[int index]
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
		/// Returns the color as an UInt32 value.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An UInt32 value representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe uint GetUIntColor(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((uint*)baseAddress)[index];
		}

		/// <summary>
		/// Sets the color at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">The index of the color to change.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetUIntColor(int index, uint color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((uint*)baseAddress)[index] = color;
		}

		/// <summary>
		/// Returns the color as an RGBQUAD structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An RGBQUAD structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBQUAD GetRGBQUAD(int index)
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
		public unsafe void SetRGBQUAD(int index, RGBQUAD color)
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
		public unsafe byte GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbRed;
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetRed(int index, byte red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbRed = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbGreen;
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetGreen(int index, byte green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbGreen = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbBlue;
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetBlue(int index, byte blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbBlue = blue;
		}

		/// <summary>
		/// Returns the data representing the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the alpha part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetAlpha(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbReserved;
		}

		/// <summary>
		/// Sets the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="alpha">The new alpha part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetAlpha(int index, byte alpha)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbReserved = alpha;
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
			return GetRGBQUAD(index).color;
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
			SetRGBQUAD(index, new RGBQUAD(color));
		}

		/// <summary>
		/// Returns an array of RGBQUAD.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBQUAD[] Data
		{
			get
			{
				RGBQUAD[] result = new RGBQUAD[length];
				fixed (RGBQUAD* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(RGBQUAD) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (RGBQUAD* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(RGBQUAD) * length);
				}
			}
		}

		/// <summary>
		/// Get an array of Color that the block of memory represents.
		/// This property is used for internal palette operations.
		/// </summary>
		internal unsafe Color[] ColorData
		{
			get
			{
				Color[] data = new Color[length];
				for (int i = 0; i < length; i++)
				{
					data[i] = Color.FromArgb((int)(((uint*)baseAddress)[i] | 0xFF000000));
				}
				return data;
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (!(obj is RGBQUADARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((RGBQUADARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBQUADARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly RGBQUADARRAY array;
			private int index = -1;

			public Enumerator(RGBQUADARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetRGBQUAD(index);
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
				index = -1;
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
		public bool Equals(RGBQUADARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure wraps all operations needed to work with an array of RGBTRIPLEs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(RGBTRIPLEARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct RGBTRIPLEARRAY : IComparable, IComparable<RGBTRIPLEARRAY>, IEnumerable, IEquatable<RGBTRIPLEARRAY>
	{
		readonly RGBTRIPLE* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an RGBTRIPLEARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public RGBTRIPLEARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (RGBTRIPLE*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an RGBTRIPLEARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 24bpp.</exception>
		public RGBTRIPLEARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 24)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (RGBTRIPLE*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(RGBTRIPLEARRAY value1, RGBTRIPLEARRAY value2)
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
				sizeof(RGBTRIPLE) * value1.length);
		}

		public static bool operator !=(RGBTRIPLEARRAY value1, RGBTRIPLEARRAY value2)
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
		/// Gets or sets the RGBTRIPLE structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>RGBTRIPLE structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBTRIPLE this[int index]
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
		/// Returns the color as an UInt32 value.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An UInt32 value representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe uint GetUIntColor(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((uint*)(baseAddress + index))[0] & 0x00FFFFFF;
		}

		/// <summary>
		/// Sets the color at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">The index of the color to change.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetUIntColor(int index, uint color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			byte* ptrDestination = (byte*)(baseAddress + index);
			byte* ptrSource = (byte*)&color;
			*ptrDestination++ = *ptrSource++;
			*ptrDestination++ = *ptrSource++;
			*ptrDestination++ = *ptrSource++;
		}

		/// <summary>
		/// Returns the color as an RGBTRIPLE structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An RGBTRIPLE structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBTRIPLE GetRGBTRIPLE(int index)
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
		public unsafe void SetRGBTRIPLE(int index, RGBTRIPLE color)
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
		public unsafe byte GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbtRed;
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetRed(int index, byte red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbtRed = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbtGreen;
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetGreen(int index, byte green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbtGreen = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index].rgbtBlue;
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetBlue(int index, byte blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index].rgbtBlue = blue;
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
			return GetRGBTRIPLE(index).color;
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
			SetRGBTRIPLE(index, new RGBTRIPLE(color));
		}

		/// <summary>
		/// Returns an array of RGBTRIPLE.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe RGBTRIPLE[] Data
		{
			get
			{
				RGBTRIPLE[] result = new RGBTRIPLE[length];
				fixed (RGBTRIPLE* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(RGBTRIPLE) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (RGBTRIPLE* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(RGBTRIPLE) * length);
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
			if (!(obj is RGBTRIPLEARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((RGBTRIPLEARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(RGBTRIPLEARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly RGBTRIPLEARRAY array;
			private int index = -1;

			public Enumerator(RGBTRIPLEARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetRGBTRIPLE(index);
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
		public bool Equals(RGBTRIPLEARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

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

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FIRGB16s.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FIRGB16ARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FIRGB16ARRAY : IComparable, IComparable<FIRGB16ARRAY>, IEnumerable, IEquatable<FIRGB16ARRAY>
	{
		readonly FIRGB16* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FIRGB16ARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FIRGB16ARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (FIRGB16*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FIRGB16ARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_RGB16.</exception>
		public FIRGB16ARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_RGB16)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (FIRGB16*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FIRGB16ARRAY value1, FIRGB16ARRAY value2)
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
				sizeof(FIRGB16) * value1.length);
		}

		public static bool operator !=(FIRGB16ARRAY value1, FIRGB16ARRAY value2)
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
		/// Gets or sets the FIRGB16 structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>FIRGB16 structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGB16 this[int index]
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
		/// Returns the color as an FIRGB16 structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FIRGB16 structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGB16 GetFIRGB16(int index)
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
		public unsafe void SetFIRGB16(int index, FIRGB16 color)
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
			return GetFIRGB16(index).color;
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
			SetFIRGB16(index, new FIRGB16(color));
		}

		/// <summary>
		/// Returns an array of FIRGB16.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGB16[] Data
		{
			get
			{
				FIRGB16[] result = new FIRGB16[length];
				fixed (FIRGB16* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(FIRGB16) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (FIRGB16* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(FIRGB16) * length);
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
			if (!(obj is FIRGB16ARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGB16ARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGB16ARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FIRGB16ARRAY array;
			private int index = -1;

			public Enumerator(FIRGB16ARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetFIRGB16(index);
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
		public bool Equals(FIRGB16ARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FIRGBAFs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FIRGBAFARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FIRGBAFARRAY : IComparable, IComparable<FIRGBAFARRAY>, IEnumerable, IEquatable<FIRGBAFARRAY>
	{
		readonly FIRGBAF* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FIRGBAFARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FIRGBAFARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (FIRGBAF*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FIRGBAFARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_RGBAF.</exception>
		public FIRGBAFARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_RGBAF)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (FIRGBAF*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FIRGBAFARRAY value1, FIRGBAFARRAY value2)
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
				sizeof(FIRGBAF) * value1.length);
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBAF this[int index]
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
		/// Returns the color as an FIRGBAF structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FIRGBAF structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBAF GetFIRGBAF(int index)
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
		public unsafe void SetFIRGBAF(int index, FIRGBAF color)
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
		public unsafe float GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[0];
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetRed(int index, float red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[0] = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe float GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[1];
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetGreen(int index, float green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[1] = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe float GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[2];
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetBlue(int index, float blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[2] = blue;
		}

		/// <summary>
		/// Returns the data representing the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the alpha part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe float GetAlpha(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[3];
		}

		/// <summary>
		/// Sets the alpha part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="alpha">The new alpha part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetAlpha(int index, float alpha)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[3] = alpha;
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
			return GetFIRGBAF(index).color;
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBAF[] Data
		{
			get
			{
				FIRGBAF[] result = new FIRGBAF[length];
				fixed (FIRGBAF* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(FIRGBAF) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (FIRGBAF* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(FIRGBAF) * length);
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
			if (!(obj is FIRGBAFARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBAFARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBAFARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
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
					{
						return array.GetFIRGBAF(index);
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
		public bool Equals(FIRGBAFARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FIRGBFs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FIRGBFARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FIRGBFARRAY : IComparable, IComparable<FIRGBFARRAY>, IEnumerable, IEquatable<FIRGBFARRAY>
	{
		readonly FIRGBF* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FIRGBFARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FIRGBFARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (FIRGBF*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FIRGBFARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_RGBF.</exception>
		public FIRGBFARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_RGBF)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (FIRGBF*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
		}

		public static bool operator ==(FIRGBFARRAY value1, FIRGBFARRAY value2)
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
				sizeof(FIRGBF) * value1.length);
		}

		public static bool operator !=(FIRGBFARRAY value1, FIRGBFARRAY value2)
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
		/// Gets or sets the FIRGBF structure representing the color at the given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>FIRGBF structure of the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBF this[int index]
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
		/// Returns the color as an FIRGBF structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FIRGBF structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBF GetFIRGBF(int index)
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
		public unsafe void SetFIRGBF(int index, FIRGBF color)
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
		public unsafe float GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[0];
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetRed(int index, float red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[0] = red;
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe float GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[1];
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetGreen(int index, float green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[1] = green;
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe float GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((float*)(baseAddress + index))[2];
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetBlue(int index, float blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			((float*)(baseAddress + index))[2] = blue;
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
			return GetFIRGBF(index).color;
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
			SetFIRGBF(index, new FIRGBF(color));
		}

		/// <summary>
		/// Returns an array of FIRGBF.
		/// Changes to the array will NOT be applied to the bitmap directly.
		/// After all changes have been done, the changes will be applied by
		/// calling the setter of 'Data' with the array.
		/// Keep in mind that using 'Data' is only useful if all values
		/// are being read or/and written.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FIRGBF[] Data
		{
			get
			{
				FIRGBF[] result = new FIRGBF[length];
				fixed (FIRGBF* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(FIRGBF) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (FIRGBF* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(FIRGBF) * length);
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
			if (!(obj is FIRGBFARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FIRGBFARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRGBFARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FIRGBFARRAY array;
			private int index = -1;

			public Enumerator(FIRGBFARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetFIRGBF(index);
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
		public bool Equals(FIRGBFARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// Storage of bitmasks and -shifts.
	/// </summary>
	internal struct BitSettings
	{
		public ushort RED_MASK;
		public ushort GREEN_MASK;
		public ushort BLUE_MASK;
		public ushort RED_SHIFT;
		public ushort GREEN_SHIFT;
		public ushort BLUE_SHIFT;
		public ushort RED_MAX;
		public ushort GREEN_MAX;
		public ushort BLUE_MAX;
	}

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
			{
				return false;
			}
			if (value1.bitSettings.GREEN_MASK != value2.bitSettings.GREEN_MASK)
			{
				return false;
			}
			if (value1.bitSettings.BLUE_MASK != value2.bitSettings.BLUE_MASK)
			{
				return false;
			}
			int MASK =
				(value1.bitSettings.RED_MASK |
				value1.bitSettings.GREEN_MASK |
				value1.bitSettings.BLUE_MASK);
			return ((value1.data & MASK) == (value2.data & MASK));
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

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FI16RGBs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI16RGBARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FI16RGBARRAY : IComparable, IComparable<FI16RGBARRAY>, IEnumerable, IEquatable<FI16RGBARRAY>
	{
		readonly ushort* baseAddress;
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
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FI16RGBARRAY(
			IntPtr baseAddress,
			uint length,
			ushort red_mask,
			ushort green_mask,
			ushort blue_mask)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (ushort*)baseAddress;
			this.length = length;
			bitSettings = GetBitSettings(red_mask, green_mask, blue_mask);
		}

		/// <summary>
		/// Creates an FIRGBFARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 16bpp.</exception>
		public FI16RGBARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 16)
			{
				throw new ArgumentException("dib");
			}
			baseAddress = (ushort*)FreeImage.GetScanLine(dib, scanline);
			length = FreeImage.GetWidth(dib);
			bitSettings = GetBitSettings(
				FreeImage.GetRedMask(dib),
				FreeImage.GetGreenMask(dib),
				FreeImage.GetBlueMask(dib));
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public FI16RGB this[int index]
		{
			get
			{
				if (index >= length || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return GetFI16RGB(index);
			}
			set
			{
				if (index >= length || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				SetFI16RGB(index, value);
			}
		}

		/// <summary>
		/// Returns the color as an FI16RGB structure.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>An FI16RGB structure representing the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe ushort GetUShort(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return baseAddress[index];
		}

		/// <summary>
		/// Sets the ushort value at position 'index' to the value of 'color'.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="color">The new value of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe void SetUShort(int index, ushort color)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			baseAddress[index] = color;
		}

		/// <summary>
		/// Extract bits from a value
		/// </summary>
		private unsafe byte GetColorComponent(int index, ushort mask, ushort shift, ushort max)
		{
			ushort value = baseAddress[index];
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
			ushort orgValue = baseAddress[index];
			orgValue &= invertMask;
			ushort newValue = (ushort)(((ushort)value * max) / 255);
			newValue <<= shift;
			newValue |= orgValue;
			baseAddress[index] = newValue;
		}

		/// <summary>
		/// Returns the data representing the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the red part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public byte GetRed(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return GetColorComponent(
				index,
				bitSettings.RED_MASK,
				bitSettings.RED_SHIFT,
				bitSettings.RED_MAX);
		}

		/// <summary>
		/// Sets the red part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="red">The new red part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public void SetRed(int index, byte red)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			SetColorComponent(
				index,
				red,
				bitSettings.RED_MASK,
				bitSettings.RED_SHIFT,
				bitSettings.RED_MAX);
		}

		/// <summary>
		/// Returns the data representing the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the green part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public byte GetGreen(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return GetColorComponent(
				index,
				bitSettings.GREEN_MASK,
				bitSettings.GREEN_SHIFT,
				bitSettings.GREEN_MAX);
		}

		/// <summary>
		/// Sets the green part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="green">The new green part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public void SetGreen(int index, byte green)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			SetColorComponent(
				index,
				green,
				bitSettings.GREEN_MASK,
				bitSettings.GREEN_SHIFT,
				bitSettings.GREEN_MAX);
		}

		/// <summary>
		/// Returns the data representing the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <returns>The value representing the blue part of the color.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public byte GetBlue(int index)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return GetColorComponent(
				index,
				bitSettings.BLUE_MASK,
				bitSettings.BLUE_SHIFT,
				bitSettings.BLUE_MAX);
		}

		/// <summary>
		/// Sets the blue part of the color at a given index.
		/// </summary>
		/// <param name="index">Index of the color.</param>
		/// <param name="blue">The new blue part of the color.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public void SetBlue(int index, byte blue)
		{
			if (index >= length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			SetColorComponent(
				index,
				blue,
				bitSettings.BLUE_MASK,
				bitSettings.BLUE_SHIFT,
				bitSettings.BLUE_MAX);
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
			int red, green, blue;
			red = GetColorComponent(
				index,
				bitSettings.RED_MASK,
				bitSettings.RED_SHIFT,
				bitSettings.RED_MAX);
			green = GetColorComponent(
				index,
				bitSettings.GREEN_MASK,
				bitSettings.GREEN_SHIFT, 
				bitSettings.GREEN_MAX);
			blue = GetColorComponent(
				index,
				bitSettings.BLUE_MASK,
				bitSettings.BLUE_SHIFT,
				bitSettings.BLUE_MAX);
			return Color.FromArgb(red, green, blue);
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe FI16RGB[] Data
		{
			get
			{
				FI16RGB[] result = new FI16RGB[length];
				for (int i = 0; i < length; i++)
				{
					result[i] = GetFI16RGB(i);
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
					SetFI16RGB(i, value[i]);
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
			if (!(obj is FI16RGBARRAY))
			{
				throw new ArgumentException();
			}
			return CompareTo((FI16RGBARRAY)obj);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI16RGBARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
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
					{
						return array.GetFI16RGB(index);
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
		public bool Equals(FI16RGBARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FI8BITs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI8BITARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FI8BITARRAY : IComparable, IComparable<FI8BITARRAY>, IEnumerable, IEquatable<FI8BITARRAY>
	{
		readonly byte* baseAddress;
		readonly uint length;

		/// <summary>
		/// Creates an FI8BITARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FI8BITARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (byte*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FI8BITARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 8bpp.</exception>
		public FI8BITARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 8)
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
		/// Gets or sets the palette-index representing at the given index.
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
			return baseAddress[index];
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
			baseAddress[index] = value;
		}

		/// <summary>
		/// Returns an array of byte.
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
				fixed (byte* dst = result)
				{
					FreeImage.MoveMemory(dst, baseAddress, sizeof(byte) * length);
				}
				return result;
			}
			set
			{
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				fixed (byte* src = value)
				{
					FreeImage.MoveMemory(baseAddress, src, sizeof(byte) * length);
				}
			}
		}

		public static bool operator ==(FI8BITARRAY value1, FI8BITARRAY value2)
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
				sizeof(byte) * value1.length);
		}

		public static bool operator !=(FI8BITARRAY value1, FI8BITARRAY value2)
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
			if (obj is FI8BITARRAY)
			{
				return CompareTo((FI8BITARRAY)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FI8BITARRAY other)
		{
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
		}

		private class Enumerator : IEnumerator
		{
			private readonly FI8BITARRAY array;
			private int index = -1;

			public Enumerator(FI8BITARRAY array)
			{
				this.array = array;
			}

			public object Current
			{
				get
				{
					if (index >= 0 && index <= array.length)
					{
						return array.GetIndex(index);
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
		public bool Equals(FI8BITARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

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
					if (index >= 0 && index <= array.length)
					{
						return array.GetIndex(index);
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
		public bool Equals(FI4BITARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure wraps all operations needed to work with an array of FI1BITs.
	/// Be aware that the data recieved from the structure are copies, and changes
	/// made to them have to be applied by calling a setter function of the structure.
	/// <para>Two arrays can be compared by their data using the equality or inequality
	/// operators.
	/// The equals(FI1BITARRAY other)-method can be used to check whether two
	/// arrays map the same block of memory.</para>
	/// </summary>
	public unsafe struct FI1BITARRAY : IComparable, IComparable<FI1BITARRAY>, IEnumerable, IEquatable<FI1BITARRAY>
	{
		readonly byte* baseAddress;
		readonly uint length;
		private const byte Zero = 0;
		private const byte One = 1;

		/// <summary>
		/// Creates an FI1BITARRAY structure.
		/// </summary>
		/// <param name="baseAddress">Startaddress of the memory to wrap.</param>
		/// <param name="length">Length of the array.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="baseAddress"/> is null.</exception>
		public FI1BITARRAY(IntPtr baseAddress, uint length)
		{
			if (baseAddress == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			this.baseAddress = (byte*)baseAddress;
			this.length = length;
		}

		/// <summary>
		/// Creates an FI1BITARRAY structure.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="scanline">Number of the scanline to wrap</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps type is not FIT_BITMAP
		/// or color depth is not 1bpp.</exception>
		public FI1BITARRAY(FIBITMAP dib, int scanline)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException();
			}
			if (FreeImage.GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("dib");
			}
			if (FreeImage.GetBPP(dib) != 1)
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
		/// Gets or sets the bit representing the index of the bitmaps palette at the given index.
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
		/// Returns the bit at a given index.
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
			byte mask = (byte)(1 << (7 - (index % 8)));
			return ((baseAddress[index / 8] & mask) > 0) ? FI1BITARRAY.One : FI1BITARRAY.Zero;
		}

		/// <summary>
		/// Sets the bit at a given index.
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
			int mask = 1 << (7 - (index % 8));
			if ((value & 0x01) > 0)
			{
				baseAddress[index / 8] |= (byte)mask;
			}
			else
			{
				baseAddress[index / 8] &= (byte)(~mask);
			}
		}

		/// <summary>
		/// Returns the bit at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <returns>Data at the index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		internal unsafe byte GetIndexUnsafe(int index)
		{
			byte mask = (byte)(1 << (7 - (index % 8)));
			return ((baseAddress[index / 8] & mask) > 0) ? FI1BITARRAY.One : FI1BITARRAY.Zero;
		}

		/// <summary>
		/// Sets the bit at a given index.
		/// </summary>
		/// <param name="index">Index of the data.</param>
		/// <param name="value">The new data.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		internal unsafe void SetIndexUnsafe(int index, byte value)
		{
			int mask = 1 << (7 - (index % 8));
			if ((value & 0x01) > 0)
			{
				baseAddress[index / 8] |= (byte)mask;
			}
			else
			{
				baseAddress[index / 8] &= (byte)(~mask);
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
		/// Thrown if <paramref name="index"/> is greater or same as Length.</exception>
		public unsafe byte[] Data
		{
			get
			{
				byte[] result = new byte[length];
				int mask = 0x80;
				int j = 0;

				for (int i = 0; i < length; i++)
				{
					result[i] = (byte)((baseAddress[j] & mask) > 0 ? 1 : 0);
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
				if (value.Length != length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int buffer = 0;
				int mask = 0x80;
				int j = 0;

				for (int i = 0; i < length; i++)
				{
					if ((value[i] & 0x01) > 0)
					{
						buffer |= mask;
					}
					mask >>= 1;
					if (mask == 0)
					{
						baseAddress[j] = (byte)buffer;
						buffer = 0;
						mask = 0x80;
						j++;
					}
				}
				if ((length % 8) != 0)
				{
					baseAddress[j] = (byte)buffer;
				}
			}
		}

		public static bool operator ==(FI1BITARRAY value1, FI1BITARRAY value2)
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
			return ((uint)baseAddress).CompareTo((uint)other.baseAddress);
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
					{
						return array.GetIndex(index);
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
		public bool Equals(FI1BITARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

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
					if (index >= 0 && index <= array.length)
					{
						return array.GetFICOMPLEX(index);
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
		public bool Equals(FICOMPLEXARRAY other)
		{
			return ((this.baseAddress == other.baseAddress) && (this.length == other.length));
		}
	}

	/// <summary>
	/// The structure represents a fraction by saving two integeres which are interpreted
	/// as numerator and denominator. The structure implements all common operations
	/// like +, -, ++, --, ==, != , >, >==, &lt;, &lt;== and ~ (which switches nominator and
	/// denomiator). No other bit-operations are implemented.
	/// The structure can be converted into all .NET standard types either implicit or
	/// explicit.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct FIRational : IConvertible, IComparable, IFormattable, IComparable<FIRational>, IEquatable<FIRational>
	{
		private int numerator;
		private int denominator;

		public const int MaxValue = Int32.MaxValue;
		public const int MinValue = Int32.MinValue;
		public const double Epsilon = 1d / (double)Int32.MaxValue;

		/// <summary>
		/// Creates a new FIRational structure.
		/// </summary>
		/// <param name="n">The numerator.</param>
		/// <param name="d">The denominator.</param>
		public FIRational(int n, int d)
		{
			numerator = n;
			denominator = d;
			Normalize();
		}

		/// <summary>
		/// Creates a new FIRational structure.
		/// </summary>
		/// <param name="tag">The tag to read the data from.</param>
		public unsafe FIRational(FITAG tag)
		{
			switch (FreeImage.GetTagType(tag))
			{
				case FREE_IMAGE_MDTYPE.FIDT_RATIONAL:
					uint* pvalue = (uint*)FreeImage.GetTagValue(tag);
					numerator = (int)pvalue[0];
					denominator = (int)pvalue[1];
					Normalize();
					return;
				case FREE_IMAGE_MDTYPE.FIDT_SRATIONAL:
					int* value = (int*)FreeImage.GetTagValue(tag);
					numerator = (int)value[0];
					denominator = (int)value[1];
					Normalize();
					return;
			}
			numerator = 0;
			denominator = 0;
			Normalize();
		}

		/// <summary>
		/// Creates a new FIRational structure by converting the value into
		/// a fraction. The fraction might slightly differ from value.
		/// </summary>
		/// <param name="value">The value to convert into a fraction.</param>
		/// <exception cref="OverflowException">
		/// Thrown if <paramref name="value"/> cannot be converted into a fraction
		/// represented by two integer values.</exception>
		public FIRational(decimal value)
		{
			try
			{
				int sign = value < 0 ? -1 : 1;
				value = Math.Abs(value);
				try
				{
					int[] contFract = CreateContinuedFraction(value);
					CreateFraction(contFract, out numerator, out denominator);
					Normalize();
				}
				catch
				{
					numerator = 0;
					denominator = 1;
				}
				if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
				{
					int maxDen = (Int32.MaxValue / (int)value) - 2;
					maxDen = maxDen < 10000 ? maxDen : 10000;
					ApproximateFraction(value, maxDen, out numerator, out denominator);
					Normalize();
					if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
						throw new OverflowException();
				}
				numerator *= sign;
				Normalize();
			}
			catch (Exception ex)
			{
				throw new OverflowException("Unable to calculate fraction.", ex);
			}
		}

		/// <summary>
		/// Creates a new FIRational structure by cloning.
		/// </summary>
		/// <param name="r">The structure to clone from.</param>
		public FIRational(FIRational r)
		{
			numerator = r.numerator;
			denominator = r.denominator;
			Normalize();
		}

		/// <summary>
		/// The numerator of the fraction.
		/// </summary>
		public int Numerator
		{
			get { return numerator; }
		}

		/// <summary>
		/// The denominator of the fraction.
		/// </summary>
		public int Denominator
		{
			get { return denominator; }
		}

		/// <summary>
		/// Returns the truncated value of the fraction.
		/// </summary>
		/// <returns></returns>
		public int Truncate()
		{
			return denominator > 0 ? (int)(numerator / denominator) : 0;
		}

		/// <summary>
		/// Returns whether the fraction is representing an integer value.
		/// </summary>
		public bool IsInteger
		{
			get
			{
				return (denominator == 1 ||
					(denominator != 0 && (numerator % denominator == 0)) ||
					(denominator == 0 && numerator == 0));
			}
		}

		/// <summary>
		/// Calculated the greatest common divisor of 'a' and 'b'.
		/// </summary>
		private static long Gcd(long a, long b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			long r;
			while (b > 0)
			{
				r = a % b;
				a = b;
				b = r;
			}
			return a;
		}

		/// <summary>
		/// Calculated the smallest common multiple of 'a' and 'b'.
		/// </summary>
		private static long Scm(int n, int m)
		{
			return Math.Abs((long)n * (long)m) / Gcd(n, m);
		}

		/// <summary>
		/// Normalizes the fraction.
		/// </summary>
		private void Normalize()
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
				return;
			}

			if (numerator != 1 && denominator != 1)
			{
				int common = (int)Gcd(numerator, denominator);
				if (common != 1 && common != 0)
				{
					numerator /= common;
					denominator /= common;
				}
			}

			if (denominator < 0)
			{
				numerator *= -1;
				denominator *= -1;
			}
		}

		/// <summary>
		/// Normalizes a fraction.
		/// </summary>
		private static void Normalize(ref long numerator, ref long denominator)
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
			}
			else if (numerator != 1 && denominator != 1)
			{
				long common = Gcd(numerator, denominator);
				if (common != 1)
				{
					numerator /= common;
					denominator /= common;
				}
			}
			if (denominator < 0)
			{
				numerator *= -1;
				denominator *= -1;
			}
		}

		/// <summary>
		/// Returns the digits after the point.
		/// </summary>
		private static int GetDigits(decimal value)
		{
			int result = 0;
			value -= decimal.Truncate(value);
			while (value != 0)
			{
				value *= 10;
				value -= decimal.Truncate(value);
				result++;
			}
			return result;
		}

		/// <summary>
		/// Creates a continued fraction of a decimal value.
		/// </summary>
		private static int[] CreateContinuedFraction(decimal value)
		{
			int precision = GetDigits(value);
			decimal epsilon = 0.0000001m;
			List<int> list = new List<int>();
			value = Math.Abs(value);

			byte b = 0;

			list.Add((int)value);
			value -= ((int)value);

			while (value != 0m)
			{
				if (++b == byte.MaxValue || value < epsilon) break;
				value = 1m / value;
				if (Math.Abs((Math.Round(value, precision - 1) - value)) < epsilon)
					value = Math.Round(value, precision - 1);
				list.Add((int)value);
				value -= ((int)value);
			}
			return list.ToArray();
		}

		/// <summary>
		/// Creates a fraction from a continued fraction.
		/// </summary>
		private static void CreateFraction(int[] continuedFraction, out int numerator, out int denominator)
		{
			numerator = 1;
			denominator = 0;
			int temp;

			for (int i = continuedFraction.Length - 1; i > -1; i--)
			{
				temp = numerator;
				numerator = continuedFraction[i] * numerator + denominator;
				denominator = temp;
			}
		}

		/// <summary>
		/// Tries 'brute force' to approximate 'value' with a fraction.
		/// </summary>
		private static void ApproximateFraction(decimal value, int maxDen, out int num, out int den)
		{
			num = 0;
			den = 0;
			decimal bestDifference = 1m;
			decimal currentDifference = -1m;
			int digits = GetDigits(value);

			if (digits <= 9)
			{
				int mul = 1;
				for (int i = 1; i <= digits; i++)
					mul *= 10;
				if (mul <= maxDen)
				{
					num = (int)(value * mul);
					den = mul;
					return;
				}
			}

			for (int i = 1; i <= maxDen; i++)
			{
				int numerator = (int)Math.Floor(value * (decimal)i + 0.5m);
				currentDifference = Math.Abs(value - (decimal)numerator / (decimal)i);
				if (currentDifference < bestDifference)
				{
					num = numerator;
					den = i;
					bestDifference = currentDifference;
				}
			}
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return ((IConvertible)this).ToDouble(null).ToString();
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is FIRational)
				return Equals((FIRational)obj);
			throw new ArgumentException("obj is no FIRational");
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Object.</returns>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		#region Operators

		public static FIRational operator +(FIRational r1)
		{
			return r1;
		}

		public static FIRational operator -(FIRational r1)
		{
			r1.numerator *= -1;
			return r1;
		}

		public static FIRational operator ~(FIRational r1)
		{
			int temp = r1.denominator;
			r1.denominator = r1.numerator;
			r1.numerator = temp;
			r1.Normalize();
			return r1;
		}

		public static FIRational operator ++(FIRational r1)
		{
			checked
			{
				r1.numerator += r1.denominator;
			}
			return r1;
		}

		public static FIRational operator --(FIRational r1)
		{
			checked
			{
				r1.numerator -= r1.denominator;
			}
			return r1;
		}

		public static FIRational operator +(FIRational r1, FIRational r2)
		{
			long numerator = 0;
			long denominator = Scm(r1.denominator, r2.denominator);
			numerator = (r1.numerator * (denominator / r1.denominator)) + (r2.numerator * (denominator / r2.denominator));
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIRational((int)numerator, (int)denominator);
			}
		}

		public static FIRational operator -(FIRational r1, FIRational r2)
		{
			return r1 + (-r2);
		}

		public static FIRational operator *(FIRational r1, FIRational r2)
		{
			long numerator = r1.numerator * r2.numerator;
			long denominator = r1.denominator * r2.denominator;
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIRational((int)numerator, (int)denominator);
			}
		}

		public static FIRational operator /(FIRational r1, FIRational r2)
		{
			int temp = r2.denominator;
			r2.denominator = r2.numerator;
			r2.numerator = temp;
			return r1 * r2;
		}

		public static FIRational operator %(FIRational r1, FIRational r2)
		{
			r2.Normalize();
			if (Math.Abs(r2.numerator) < r2.denominator)
				return new FIRational(0, 0);
			int div = (int)(r1 / r2);
			return r1 - (r2 * div);
		}

		public static bool operator ==(FIRational r1, FIRational r2)
		{
			r1.Normalize();
			r2.Normalize();
			return (r1.numerator == r2.numerator) && (r1.denominator == r2.denominator);
		}

		public static bool operator !=(FIRational r1, FIRational r2)
		{
			return !(r1 == r2);
		}

		public static bool operator >(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) > (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator <(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) < (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator >=(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) >= (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator <=(FIRational r1, FIRational r2)
		{
			long denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) <= (r2.numerator * (denominator / r2.denominator));
		}

		#endregion

		#region Conversions

		public static explicit operator bool(FIRational r)
		{
			return (r.numerator > 0);
		}

		public static explicit operator byte(FIRational r)
		{
			return (byte)(double)r;
		}

		public static explicit operator char(FIRational r)
		{
			return (char)(double)r;
		}

		public static implicit operator decimal(FIRational r)
		{
			return r.denominator == 0 ? 0m : (decimal)r.numerator / (decimal)r.denominator;
		}

		public static implicit operator double(FIRational r)
		{
			return r.denominator == 0 ? 0d : (double)r.numerator / (double)r.denominator;
		}

		public static explicit operator short(FIRational r)
		{
			return (short)(double)r;
		}

		public static explicit operator int(FIRational r)
		{
			return (int)(double)r;
		}

		public static explicit operator long(FIRational r)
		{
			return (byte)(double)r;
		}

		public static implicit operator float(FIRational r)
		{
			return r.denominator == 0 ? 0f : (float)r.numerator / (float)r.denominator;
		}

		public static explicit operator sbyte(FIRational r)
		{
			return (sbyte)(double)r;
		}

		public static explicit operator ushort(FIRational r)
		{
			return (ushort)(double)r;
		}

		public static explicit operator uint(FIRational r)
		{
			return (uint)(double)r;
		}

		public static explicit operator ulong(FIRational r)
		{
			return (ulong)(double)r;
		}

		//

		public static explicit operator FIRational(bool value)
		{
			return new FIRational(value ? 1 : 0, 1);
		}

		public static implicit operator FIRational(byte value)
		{
			return new FIRational(value, 1);
		}

		public static implicit operator FIRational(char value)
		{
			return new FIRational(value, 1);
		}

		public static explicit operator FIRational(decimal value)
		{
			return new FIRational(value);
		}

		public static explicit operator FIRational(double value)
		{
			return new FIRational((decimal)value);
		}

		public static implicit operator FIRational(short value)
		{
			return new FIRational(value, 1);
		}

		public static implicit operator FIRational(int value)
		{
			return new FIRational(value, 1);
		}

		public static explicit operator FIRational(long value)
		{
			return new FIRational((int)value, 1);
		}

		public static implicit operator FIRational(sbyte value)
		{
			return new FIRational(value, 1);
		}

		public static explicit operator FIRational(float value)
		{
			return new FIRational((decimal)value);
		}

		public static implicit operator FIRational(ushort value)
		{
			return new FIRational(value, 1);
		}

		public static explicit operator FIRational(uint value)
		{
			return new FIRational((int)value, 1);
		}

		public static explicit operator FIRational(ulong value)
		{
			return new FIRational((int)value, 1);
		}

		#endregion

		#region IConvertible Member

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Double;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(((IConvertible)this).ToDouble(provider));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return this;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString(((double)this).ToString(), provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(((IConvertible)this).ToDouble(provider), conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		#endregion

		#region IComparable Member

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIRational)
				return CompareTo((FIRational)obj);
			else if (obj is IConvertible)
				return CompareTo(new FIRational(((IConvertible)obj).ToDecimal(null)));
			throw new ArgumentException("obj is not convertable to double");
		}

		#endregion

		#region IFormattable Member

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The String specifying the format to use.</param>
		/// <param name="formatProvider">The IFormatProvider to use to format the value.</param>
		/// <returns>A String containing the value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null) format = "";
			return String.Format(formatProvider, format, ((IConvertible)this).ToDouble(formatProvider));
		}

		#endregion

		#region IEquatable<FIRational> Member

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIRational other)
		{
			return ((FIRational)other).numerator == numerator && ((FIRational)other).denominator == denominator;
		}

		#endregion

		#region IComparable<FIRational> Member

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIRational other)
		{
			FIRational difference = this - other;
			difference.Normalize();
			if (difference.numerator > 0) return 1;
			if (difference.numerator < 0) return -1;
			else return 0;
		}

		#endregion
	}

	/// <summary>
	/// The structure represents a fraction by saving two unsigned integeres which are interpreted
	/// as numerator and denominator. The structure implements all common operations
	/// like +, -, ++, --, ==, != , >, >==, &lt;, &lt;== and ~ (which switches nominator and
	/// denomiator). No other bit-operations are implemented.
	/// The structure can be converted into all .NET standard types either implicit or
	/// explicit.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct FIURational : IConvertible, IComparable, IFormattable, IComparable<FIURational>, IEquatable<FIURational>
	{
		private uint numerator;
		private uint denominator;

		public const uint MaxValue = UInt32.MaxValue;
		public const uint MinValue = UInt32.MinValue;
		public const double Epsilon = 1d / (double)UInt32.MaxValue;

		/// <summary>
		/// Creates a new FIRational structure.
		/// </summary>
		/// <param name="n">The numerator.</param>
		/// <param name="d">The denominator.</param>
		public FIURational(uint n, uint d)
		{
			numerator = n;
			denominator = d;
			Normalize();
		}

		/// <summary>
		/// Creates a new FIRational structure.
		/// </summary>
		/// <param name="tag">The tag to read the data from.</param>
		public unsafe FIURational(FITAG tag)
		{
			switch (FreeImage.GetTagType(tag))
			{
				case FREE_IMAGE_MDTYPE.FIDT_RATIONAL:
					uint* pvalue = (uint*)FreeImage.GetTagValue(tag);
					numerator = pvalue[0];
					denominator = pvalue[1];
					Normalize();
					return;
				case FREE_IMAGE_MDTYPE.FIDT_SRATIONAL:
					throw new ArgumentException("tag");
			}
			numerator = 0;
			denominator = 0;
			Normalize();
		}

		/// <summary>
		/// Creates a new FIRational structure by converting the value into
		/// a fraction. The fraction might slightly differ from value.
		/// </summary>
		/// <param name="value">The value to convert into a fraction.</param>
		/// <exception cref="OverflowException">
		/// Thrown if <paramref name="value"/> cannot be converted into a fraction
		/// represented by two integer values.</exception>
		public FIURational(decimal value)
		{
			try
			{
				if (value < 0) throw new ArgumentOutOfRangeException("value");
				try
				{
					int[] contFract = CreateContinuedFraction(value);
					CreateFraction(contFract, out numerator, out denominator);
					Normalize();
				}
				catch
				{
					numerator = 0;
					denominator = 1;
				}
				if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
				{
					int maxDen = (Int32.MaxValue / (int)value) - 2;
					maxDen = maxDen < 10000 ? maxDen : 10000;
					ApproximateFraction(value, maxDen, out numerator, out denominator);
					Normalize();
					if (Math.Abs(((decimal)numerator / (decimal)denominator) - value) > 0.0001m)
						throw new OverflowException();
				}
				Normalize();
			}
			catch (Exception ex)
			{
				throw new OverflowException("Unable to calculate fraction.", ex);
			}
		}

		/// <summary>
		/// Creates a new FIRational structure by cloning.
		/// </summary>
		/// <param name="r">The structure to clone from.</param>
		public FIURational(FIURational r)
		{
			numerator = r.numerator;
			denominator = r.denominator;
			Normalize();
		}

		/// <summary>
		/// The numerator of the fraction.
		/// </summary>
		public uint Numerator
		{
			get { return numerator; }
		}

		/// <summary>
		/// The denominator of the fraction.
		/// </summary>
		public uint Denominator
		{
			get { return denominator; }
		}

		/// <summary>
		/// Returns the truncated value of the fraction.
		/// </summary>
		/// <returns></returns>
		public int Truncate()
		{
			return denominator > 0 ? (int)(numerator / denominator) : 0;
		}

		/// <summary>
		/// Returns whether the fraction is representing an integer value.
		/// </summary>
		public bool IsInteger
		{
			get
			{
				return (denominator == 1 ||
					(denominator != 0 && (numerator % denominator == 0)) ||
					(denominator == 0 && numerator == 0));
			}
		}

		/// <summary>
		/// Calculated the greatest common divisor of 'a' and 'b'.
		/// </summary>
		private static ulong Gcd(ulong a, ulong b)
		{
			ulong r;
			while (b > 0)
			{
				r = a % b;
				a = b;
				b = r;
			}
			return a;
		}

		/// <summary>
		/// Calculated the smallest common multiple of 'a' and 'b'.
		/// </summary>
		private static ulong Scm(uint n, uint m)
		{
			return (ulong)n * (ulong)m / Gcd(n, m);
		}

		/// <summary>
		/// Normalizes the fraction.
		/// </summary>
		private void Normalize()
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
				return;
			}

			if (numerator != 1 && denominator != 1)
			{
				uint common = (uint)Gcd(numerator, denominator);
				if (common != 1 && common != 0)
				{
					numerator /= common;
					denominator /= common;
				}
			}
		}

		/// <summary>
		/// Normalizes a fraction.
		/// </summary>
		private static void Normalize(ref ulong numerator, ref ulong denominator)
		{
			if (denominator == 0)
			{
				numerator = 0;
				denominator = 1;
			}
			else if (numerator != 1 && denominator != 1)
			{
				ulong common = Gcd(numerator, denominator);
				if (common != 1)
				{
					numerator /= common;
					denominator /= common;
				}
			}
		}

		/// <summary>
		/// Returns the digits after the point.
		/// </summary>
		private static int GetDigits(decimal value)
		{
			int result = 0;
			value -= decimal.Truncate(value);
			while (value != 0)
			{
				value *= 10;
				value -= decimal.Truncate(value);
				result++;
			}
			return result;
		}

		/// <summary>
		/// Creates a continued fraction of a decimal value.
		/// </summary>
		private static int[] CreateContinuedFraction(decimal value)
		{
			int precision = GetDigits(value);
			decimal epsilon = 0.0000001m;
			List<int> list = new List<int>();
			value = Math.Abs(value);

			byte b = 0;

			list.Add((int)value);
			value -= ((int)value);

			while (value != 0m)
			{
				if (++b == byte.MaxValue || value < epsilon) break;
				value = 1m / value;
				if (Math.Abs((Math.Round(value, precision - 1) - value)) < epsilon)
					value = Math.Round(value, precision - 1);
				list.Add((int)value);
				value -= ((int)value);
			}
			return list.ToArray();
		}

		/// <summary>
		/// Creates a fraction from a continued fraction.
		/// </summary>
		private static void CreateFraction(int[] continuedFraction, out uint numerator, out uint denominator)
		{
			numerator = 1;
			denominator = 0;
			uint temp;

			for (int i = continuedFraction.Length - 1; i > -1; i--)
			{
				temp = numerator;
				numerator = (uint)(continuedFraction[i] * numerator + denominator);
				denominator = temp;
			}
		}

		/// <summary>
		/// Tries 'brute force' to approximate 'value' with a fraction.
		/// </summary>
		private static void ApproximateFraction(decimal value, int maxDen, out uint num, out uint den)
		{
			num = 0;
			den = 0;
			decimal bestDifference = 1m;
			decimal currentDifference = -1m;
			int digits = GetDigits(value);

			if (digits <= 9)
			{
				uint mul = 1;
				for (int i = 1; i <= digits; i++)
					mul *= 10;
				if (mul <= maxDen)
				{
					num = (uint)(value * mul);
					den = mul;
					return;
				}
			}

			for (uint u = 1; u <= maxDen; u++)
			{
				uint numerator = (uint)Math.Floor(value * (decimal)u + 0.5m);
				currentDifference = Math.Abs(value - (decimal)numerator / (decimal)u);
				if (currentDifference < bestDifference)
				{
					num = numerator;
					den = u;
					bestDifference = currentDifference;
				}
			}
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return ((IConvertible)this).ToDouble(null).ToString();
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is FIURational)
				return Equals((FIURational)obj);
			throw new ArgumentException("obj is no FIRational");
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Object.</returns>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		#region Operators

		public static FIURational operator +(FIURational r1)
		{
			return r1;
		}

		public static FIURational operator ~(FIURational r1)
		{
			uint temp = r1.denominator;
			r1.denominator = r1.numerator;
			r1.numerator = temp;
			r1.Normalize();
			return r1;
		}

		public static FIURational operator ++(FIURational r1)
		{
			checked
			{
				r1.numerator += r1.denominator;
			}
			return r1;
		}

		public static FIURational operator --(FIURational r1)
		{
			checked
			{
				r1.numerator -= r1.denominator;
			}
			return r1;
		}

		public static FIURational operator +(FIURational r1, FIURational r2)
		{
			ulong numerator = 0;
			ulong denominator = Scm(r1.denominator, r2.denominator);
			numerator = (r1.numerator * (denominator / r1.denominator)) + (r2.numerator * (denominator / r2.denominator));
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIURational((uint)numerator, (uint)denominator);
			}
		}

		public static FIURational operator -(FIURational r1, FIURational r2)
		{
			checked
			{
				if (r1.denominator != r2.denominator)
				{
					uint denom = r1.denominator;
					r1.numerator *= r2.denominator;
					r1.denominator *= r2.denominator;
					r2.numerator *= denom;
					r2.denominator *= denom;
				}
				r1.numerator -= r2.numerator;
				r1.Normalize();
				return r1;
			}
		}

		public static FIURational operator *(FIURational r1, FIURational r2)
		{
			ulong numerator = r1.numerator * r2.numerator;
			ulong denominator = r1.denominator * r2.denominator;
			Normalize(ref numerator, ref denominator);
			checked
			{
				return new FIURational((uint)numerator, (uint)denominator);
			}
		}

		public static FIURational operator /(FIURational r1, FIURational r2)
		{
			uint temp = r2.denominator;
			r2.denominator = r2.numerator;
			r2.numerator = temp;
			return r1 * r2;
		}

		public static FIURational operator %(FIURational r1, FIURational r2)
		{
			r2.Normalize();
			if (Math.Abs(r2.numerator) < r2.denominator)
				return new FIURational(0, 0);
			int div = (int)(r1 / r2);
			return r1 - (r2 * div);
		}

		public static bool operator ==(FIURational r1, FIURational r2)
		{
			r1.Normalize();
			r2.Normalize();
			return (r1.numerator == r2.numerator) && (r1.denominator == r2.denominator);
		}

		public static bool operator !=(FIURational r1, FIURational r2)
		{
			return !(r1 == r2);
		}

		public static bool operator >(FIURational r1, FIURational r2)
		{
			ulong denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) > (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator <(FIURational r1, FIURational r2)
		{
			ulong denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) < (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator >=(FIURational r1, FIURational r2)
		{
			ulong denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) >= (r2.numerator * (denominator / r2.denominator));
		}

		public static bool operator <=(FIURational r1, FIURational r2)
		{
			ulong denominator = Scm(r1.denominator, r2.denominator);
			return (r1.numerator * (denominator / r1.denominator)) <= (r2.numerator * (denominator / r2.denominator));
		}

		#endregion

		#region Conversions

		public static explicit operator bool(FIURational r)
		{
			return (r.numerator > 0);
		}

		public static explicit operator byte(FIURational r)
		{
			return (byte)(double)r;
		}

		public static explicit operator char(FIURational r)
		{
			return (char)(double)r;
		}

		public static implicit operator decimal(FIURational r)
		{
			return r.denominator == 0 ? 0m : (decimal)r.numerator / (decimal)r.denominator;
		}

		public static implicit operator double(FIURational r)
		{
			return r.denominator == 0 ? 0d : (double)r.numerator / (double)r.denominator;
		}

		public static explicit operator short(FIURational r)
		{
			return (short)(double)r;
		}

		public static explicit operator int(FIURational r)
		{
			return (int)(double)r;
		}

		public static explicit operator long(FIURational r)
		{
			return (byte)(double)r;
		}

		public static implicit operator float(FIURational r)
		{
			return r.denominator == 0 ? 0f : (float)r.numerator / (float)r.denominator;
		}

		public static explicit operator sbyte(FIURational r)
		{
			return (sbyte)(double)r;
		}

		public static explicit operator ushort(FIURational r)
		{
			return (ushort)(double)r;
		}

		public static explicit operator uint(FIURational r)
		{
			return (uint)(double)r;
		}

		public static explicit operator ulong(FIURational r)
		{
			return (ulong)(double)r;
		}

		//

		public static explicit operator FIURational(bool value)
		{
			return new FIURational(value ? 1u : 0u, 1u);
		}

		public static implicit operator FIURational(byte value)
		{
			return new FIURational(value, 1);
		}

		public static implicit operator FIURational(char value)
		{
			return new FIURational(value, 1);
		}

		public static explicit operator FIURational(decimal value)
		{
			return new FIURational(value);
		}

		public static explicit operator FIURational(double value)
		{
			return new FIURational((decimal)value);
		}

		public static implicit operator FIURational(short value)
		{
			return new FIURational((uint)value, 1u);
		}

		public static implicit operator FIURational(int value)
		{
			return new FIURational((uint)value, 1u);
		}

		public static explicit operator FIURational(long value)
		{
			return new FIURational((uint)value, 1u);
		}

		public static implicit operator FIURational(sbyte value)
		{
			return new FIURational((uint)value, 1u);
		}

		public static explicit operator FIURational(float value)
		{
			return new FIURational((decimal)value);
		}

		public static implicit operator FIURational(ushort value)
		{
			return new FIURational(value, 1);
		}

		public static explicit operator FIURational(uint value)
		{
			return new FIURational(value, 1u);
		}

		public static explicit operator FIURational(ulong value)
		{
			return new FIURational((uint)value, 1u);
		}

		#endregion

		#region IConvertible Member

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Double;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(((IConvertible)this).ToDouble(provider));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return this;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString(((double)this).ToString(), provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(((IConvertible)this).ToDouble(provider), conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		#endregion

		#region IComparable Member

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is FIURational)
				return CompareTo((FIURational)obj);
			else if (obj is IConvertible)
				return CompareTo(new FIURational(((IConvertible)obj).ToDecimal(null)));
			throw new ArgumentException("obj is not convertable to double");
		}

		#endregion

		#region IFormattable Member

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The String specifying the format to use.</param>
		/// <param name="formatProvider">The IFormatProvider to use to format the value.</param>
		/// <returns>A String containing the value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null) format = "";
			return String.Format(formatProvider, format, ((IConvertible)this).ToDouble(formatProvider));
		}

		#endregion

		#region IEquatable<FIRational> Member

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(FIURational other)
		{
			return ((FIURational)other).numerator == numerator && ((FIURational)other).denominator == denominator;
		}

		#endregion

		#region IComparable<FIRational> Member

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(FIURational other)
		{
			FIURational difference = this - other;
			difference.Normalize();
			if (difference.numerator > 0) return 1;
			if (difference.numerator < 0) return -1;
			else return 0;
		}

		#endregion
	}

	// The 'fi_handle' of FreeImage in C++ is a simple pointer, but in .NET
	// it's not that simple. This wrapper uses fi_handle in two different ways.
	//
	// We implement a new plugin and FreeImage gives us a handle (pointer) that
	// we can simply pass through to the given functions in a 'FreeImageIO'
	// structure.
	// But when we want to use LoadFromhandle or SaveToHandle we need
	// a fi_handle (that we recieve again in our own functions).
	// This handle is for example a stream (see LoadFromStream / SaveToStream)
	// that we want to work with. To know which stream a read/write is meant for
	// we could use a hash value that the wrapper itself handles or we can
	// go the unmanaged way of using a handle.
	// Therefor we use a 'GCHandle' to recieve a unique pointer that we can
	// convert back into a .NET object.
	// When the fi_handle instance is no longer needed the instance must be disposed
	// by the creater manually! It is recommended to use the "using" statement to
	// be sure the instance is always disposed:
	//
	// using (fi_handle handle = new fi_handle(object))
	// {
	//     callSomeFunctions(handle);
	// }
	//
	// What does that mean?
	// If we get a fi_handle from unmanaged code we get a pointer to unmanaged
	// memory that we do not have to care about, and just pass ist back to FreeImage.
	// If we have to create a handle our own we use the standard constructur
	// that fills the IntPtr with an pointer that represents the given object.
	// With calling 'GetObject' the IntPtr is used to retrieve the original
	// object we passed through the constructor.
	//
	// This way we can implement a fi_handle that works with managed an unmanaged
	// code.

	/// <summary>
	/// Wrapper for a custom handle.
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct fi_handle : IComparable, IComparable<fi_handle>, IEquatable<fi_handle>, IDisposable
	{
		/// <summary>
		/// The handle to wrap.
		/// </summary>
		public IntPtr handle;

		/// <summary>
		/// Creates a new fi_handle structure wrapping a managed object.
		/// </summary>
		/// <param name="obj">The object to wrap.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="obj"/> is null.</exception>
		public fi_handle(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			GCHandle gch = GCHandle.Alloc(obj, GCHandleType.Normal);
			handle = GCHandle.ToIntPtr(gch);
		}

		public static bool operator !=(fi_handle value1, fi_handle value2)
		{
			return value1.handle != value2.handle;
		}

		public static bool operator ==(fi_handle value1, fi_handle value2)
		{
			return value1.handle == value2.handle;
		}

		/// <summary>
		/// Gets whether the pointer is a null pointer.
		/// </summary>
		public bool IsNull { get { return handle == IntPtr.Zero; } }

		/// <summary>
		/// Returns the object assigned to the handle in case this instance
		/// was created by managed code.
		/// </summary>
		/// <returns>Object assigned to this handle or null on failure.</returns>
		internal object GetObject()
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			try
			{
				return GCHandle.FromIntPtr(handle).Target;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return String.Format("0x{0:X}", (uint)handle);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Object.</returns>
		public override int GetHashCode()
		{
			return handle.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is fi_handle)
			{
				return (((fi_handle)obj).handle == handle);
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
			if (obj is fi_handle)
			{
				return CompareTo((fi_handle)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(fi_handle other)
		{
			return handle.ToInt64().CompareTo(other.handle.ToInt64());
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(fi_handle other)
		{
			return this == other;
		}

		/// <summary>
		/// Frees the GCHandle.
		/// </summary>
		public void Dispose()
		{
			if (this.handle != IntPtr.Zero)
			{
				try
				{
					GCHandle.FromIntPtr(handle).Free();
				}
				catch
				{
				}
				finally
				{
					this.handle = IntPtr.Zero;
				}
			}
		}
	}

	#endregion

	#region Classes

	/// <summary>
	/// Encapsulates a FreeImage-bitmap.
	/// </summary>
	[Serializable, Guid("64a4c935-b757-499c-ab8c-6110316a9e51")]
	public class FreeImageBitmap : ICloneable, IDisposable, IEnumerable, ISerializable
	{
		private bool disposed = false;
		private object tag = null;
		private object lockObject = new object();
		private SaveInformation saveInformation = new SaveInformation();
		protected FREE_IMAGE_FORMAT originalFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
		protected FIBITMAP dib = 0;
		protected FIMULTIBITMAP mdib = 0;

		#region Constructors and Destructor

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// </summary>
		protected FreeImageBitmap()
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// For internal use only.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		internal FreeImageBitmap(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new Exception();
			}
			this.dib = dib;
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(FreeImageBitmap original)
		{
			original.ThrowOnDisposed();
			dib = FreeImage.Clone(original.dib);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(FreeImageBitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(FreeImageBitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			original.ThrowOnDisposed();
			dib = FreeImage.Rescale(original.dib, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Image original)
			: this(original as Bitmap)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(Image original, Size newSize)
			: this(original as Bitmap, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(Image original, int width, int height)
			: this(original as Bitmap, width, height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Bitmap original)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			dib = FreeImage.CreateFromBitmap(original, true);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="newSize.Width"/> or <paramref name="newSize.Height"/> are less than or zero.
		/// </exception>
		public FreeImageBitmap(Bitmap original, Size newSize)
			: this(original, newSize.Width, newSize.Height)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from a specified existing image with the specified size.
		/// </summary>
		/// <param name="original">The original to clone from.</param>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="width"/> or <paramref name="height"/> are less than or zero.</exception>
		public FreeImageBitmap(Bitmap original, int width, int height)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			FIBITMAP temp = FreeImage.CreateFromBitmap(original, true);
			if (temp.IsNull)
			{
				throw new Exception();
			}
			else
			{
				dib = FreeImage.Rescale(temp, width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
				FreeImage.Unload(temp);
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, bool useIcm)
			: this(stream)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream in the specified format.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format)
			: this(stream, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_LOAD_FLAGS flags)
			: this(stream, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// from the specified data stream in the specified format
		/// with the specified loading flags.
		/// </summary>
		/// <param name="stream">Stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public FreeImageBitmap(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			saveInformation.loadFlags = flags;

			dib = FreeImage.LoadFromStream(stream, flags, ref format);

			if (dib.IsNull)
			{
				throw new Exception();
			}

			originalFormat = format;
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename)
			: this(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="useIcm">Ignored.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, bool useIcm)
			: this(filename)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_LOAD_FLAGS flags)
			: this(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, flags)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// in the specified format.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format)
			: this(filename, format, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from the specified file
		/// in the specified format with the specified loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		public FreeImageBitmap(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_LOAD_FLAGS flags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			saveInformation.loadFlags = flags;

			mdib = FreeImage.OpenMultiBitmapEx(filename, ref format, flags, false, true, false);

			if (mdib.IsNull)
			{
				throw new Exception();
			}

			originalFormat = format;

			if (FreeImage.GetPageCount(mdib) != 0)
			{
				if (FreeImage.GetPageCount(mdib) == 1)
				{
					if (!FreeImage.CloseMultiBitmapEx(ref mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT))
					{
						throw new Exception();
					}

					dib = FreeImage.LoadEx(filename, flags, ref format);
					if (dib.IsNull)
					{
						throw new Exception();
					}

					return;
				}
				else
				{
					dib = FreeImage.LockPage(mdib, 0);
					if (!dib.IsNull)
					{
						return;
					}
				}
			}

			FreeImage.CloseMultiBitmap(mdib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
			throw new Exception();
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class
		/// with the specified size.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(int width, int height)
		{
			dib = FreeImage.Allocate(
				width,
				height,
				24,
				FreeImage.FI_RGBA_RED_MASK,
				FreeImage.FI_RGBA_GREEN_MASK,
				FreeImage.FI_RGBA_BLUE_MASK);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class from a specified resource.
		/// </summary>
		/// <param name="type">The class used to extract the resource.</param>
		/// <param name="resource">The name of the resource.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		public FreeImageBitmap(Type type, string resource)
			: this(type.Module.Assembly.GetManifestResourceStream(type, resource))
		{
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size
		/// and with the resolution of the specified Graphics object.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="g">The Graphics object that specifies the resolution for the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="g"/> is null.</exception>
		public FreeImageBitmap(int width, int height, Graphics g)
			: this(width, height)
		{
			FreeImage.SetResolutionX(dib, (uint)g.DpiX);
			FreeImage.SetResolutionX(dib, (uint)g.DpiY);
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size and format.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="format">The PixelFormat enumeration for the new FreeImageBitmap.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		public FreeImageBitmap(int width, int height, PixelFormat format)
		{
			uint bpp, redMask, greenMask, blueMask;
			if (!FreeImage.GetFormatParameters(format, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid.");
			}
			dib = FreeImage.Allocate(width, height, (int)bpp, redMask, greenMask, blueMask);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class with the specified size,
		/// pixel format, and pixel data.
		/// </summary>
		/// <param name="width">The width, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="height">The height, in pixels, of the new FreeImageBitmap.</param>
		/// <param name="stride">nteger that specifies the byte offset between the beginning
		/// of one scan line and the next. This is usually (but not necessarily)
		/// the number of bytes in the pixel format (for example, 2 for 16 bits per pixel)
		/// multiplied by the width of the bitmap. The value passed to this parameter must
		/// be a multiple of four..</param>
		/// <param name="format">The PixelFormat enumeration for the new FreeImageBitmap.</param>
		/// <param name="scan0">Pointer to an array of bytes that contains the pixel data.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
		public FreeImageBitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		{
			uint bpp, redMask, greenMask, blueMask;
			bool topDown = (stride > 0);
			stride = (stride > 0) ? stride : (stride * -1);

			if (!FreeImage.GetFormatParameters(format, out bpp, out redMask, out greenMask, out blueMask))
			{
				throw new ArgumentException("format is invalid.");
			}

			dib = FreeImage.ConvertFromRawBits(
				scan0, width, height, stride, bpp, redMask, greenMask, blueMask, topDown);
			if (dib.IsNull)
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Initializes a new instance of the FreeImageBitmap class.
		/// </summary>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="SerializationException">The operation failed.</exception>
		public FreeImageBitmap(SerializationInfo info, StreamingContext context)
		{
			try
			{
				byte[] data = (byte[])info.GetValue("Bitmap Data", typeof(byte[]));
				if (data != null && data.Length > 0)
				{
					MemoryStream memory = new MemoryStream(data);
					FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_TIFF;
					dib = FreeImage.LoadFromStream(memory, ref format);
				}
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
			catch
			{
				throw new SerializationException();
			}
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~FreeImageBitmap()
		{
			Dispose(false);
		}

		#endregion

		#region Operators

		/// <summary>
		/// The implicit conversion from FreeImageBitmap into Bitmap
		/// allows to create an instance on the fly and use it as if
		/// was a Bitmap. This way it can be directly used with a
		/// PixtureBox for example without having to call any
		/// conversion operations.
		/// </summary>
		public static implicit operator Bitmap(FreeImageBitmap value)
		{
			value.ThrowOnDisposed();
			return FreeImage.GetBitmap(value.dib, true);
		}

		/// <summary>
		/// The implicit conversion from Bitmap into FreeImageBitmap
		/// allows to create an instance on the fly to perform
		/// image processing operations and converting it back.
		/// </summary>
		public static implicit operator FreeImageBitmap(Bitmap value)
		{
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.CreateFromBitmap(value, true);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		public static bool operator ==(FreeImageBitmap fib1, FreeImageBitmap fib2)
		{
			bool result = false;
			if (object.ReferenceEquals(fib1, fib2))
			{
				if (!object.ReferenceEquals(fib1, null))
				{
					fib1.ThrowOnDisposed();
				}
				result = true;
			}
			else if (object.ReferenceEquals(fib1, null))
			{
				fib2.ThrowOnDisposed();
			}
			else if (object.ReferenceEquals(fib2, null))
			{
				fib1.ThrowOnDisposed();
			}
			else
			{
				fib1.ThrowOnDisposed();
				fib2.ThrowOnDisposed();
				result = FreeImage.Compare(fib1.dib, fib2.dib, FREE_IMAGE_COMPARE_FLAGS.COMPLETE);
			}
			return result;
		}

		public static bool operator !=(FreeImageBitmap fib1, FreeImageBitmap fib2)
		{
			return !(fib1 == fib2);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Type of the bitmap.
		/// </summary>
		public FREE_IMAGE_TYPE ImageType
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetImageType(dib);
			}
		}

		/// <summary>
		/// Number of palette entries.
		/// </summary>
		public int ColorsUsed
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetColorsUsed(dib);
			}
		}

		/// <summary>
		/// The number of unique colors actually used by the bitmap. This might be different from
		/// what ColorsUsed returns, which actually returns the palette size for palletised images.
		/// Works for FIT_BITMAP type bitmaps only.
		/// </summary>
		public int UniqueColors
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetUniqueColors(dib);
			}
		}

		/// <summary>
		/// The size of one pixel in the bitmap in bits.
		/// </summary>
		public int ColorDepth
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetBPP(dib);
			}
		}

		/// <summary>
		/// Width of the bitmap in pixel units.
		/// </summary>
		public int Width
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetWidth(dib);
			}
		}

		/// <summary>
		/// Height of the bitmap in pixel units.
		/// </summary>
		public int Height
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetHeight(dib);
			}
		}

		/// <summary>
		/// Returns the width of the bitmap in bytes, rounded to the next 32-bit boundary.
		/// </summary>
		public int Pitch
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetPitch(dib);
			}
		}

		/// <summary>
		/// Size of the bitmap in memory.
		/// </summary>
		public int DataSize
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetDIBSize(dib);
			}
		}

		/// <summary>
		/// Returns a structure that wraps the palette of a FreeImage bitmap.
		/// </summary>
		/// <exception cref="Exception">'HasPalette' is false.</exception>
		public RGBQUADARRAY Palette
		{
			get
			{
				ThrowOnDisposed();
				if (HasPalette)
				{
					return FreeImage.GetPaletteEx(dib);
				}
				throw new Exception();
			}
		}

		/// <summary>
		/// Gets the horizontal resolution, in pixels per inch, of this bitmap.
		/// </summary>
		public float HorizontalResolution
		{
			get
			{
				ThrowOnDisposed();
				return (float)FreeImage.GetResolutionX(dib);
			}
			private set
			{
				ThrowOnDisposed();
				FreeImage.SetResolutionX(dib, (uint)value);
			}
		}

		/// <summary>
		/// Gets the vertical resolution, in pixels per inch, of this bitmap.
		/// </summary>
		public float VerticalResolution
		{
			get
			{
				ThrowOnDisposed();
				return (float)FreeImage.GetResolutionY(dib);
			}
			private set
			{
				ThrowOnDisposed();
				FreeImage.SetResolutionY(dib, (uint)value);
			}
		}

		/// <summary>
		/// Returns the BITMAPINFOHEADER structure of this bitmap.
		/// </summary>
		public BITMAPINFOHEADER InfoHeader
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetInfoHeaderEx(dib);
			}
		}

		/// <summary>
		/// Returns the BITMAPINFO structure of a this bitmap.
		/// </summary>
		public BITMAPINFO Info
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetInfoEx(dib);
			}
		}

		/// <summary>
		/// Investigates the color type of the bitmap
		/// by reading the bitmaps pixel bits and analysing them.
		/// </summary>
		public FREE_IMAGE_COLOR_TYPE ColorType
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetColorType(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the red color component of a pixel in this bitmap.
		/// </summary>
		public uint RedMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetRedMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the green color component of a pixel in this bitmap.
		/// </summary>
		public uint GreenMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetGreenMask(dib);
			}
		}

		/// <summary>
		/// Bit pattern describing the blue color component of a pixel in this bitmap.
		/// </summary>
		public uint BlueMask
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetBlueMask(dib);
			}
		}

		/// <summary>
		/// Number of transparent colors in a palletised bitmap.
		/// </summary>
		public int TransparencyCount
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetTransparencyCount(dib);
			}
		}

		/// <summary>
		/// Get or sets the bitmap's transparency table.
		/// </summary>
		public byte[] TransparencyTable
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetTransparencyTableEx(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparencyTable(dib, value);
			}
		}

		/// <summary>
		/// Gets or sets whether this bitmap is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.IsTransparent(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparent(dib, value);
			}
		}

		/// <summary>
		/// Gets whether this bitmap has a file background color.
		/// </summary>
		public bool HasBackgroundColor
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.HasBackgroundColor(dib);
			}
		}

		/// <summary>
		/// Gets or sets the bitmap's background color.
		/// In case the value is null, the background color is removed.
		/// </summary>
		/// <exception cref="InvalidOperationException">Get: There is no background color available.</exception>
		/// <exception cref="Exception">Set: Setting background color failed.</exception>
		public Color? BackgroundColor
		{
			get
			{
				ThrowOnDisposed();
				if (!FreeImage.HasBackgroundColor(dib))
				{
					throw new InvalidOperationException("No background color available.");
				}
				RGBQUAD rgbq;
				FreeImage.GetBackgroundColor(dib, out rgbq);
				return rgbq.color;
			}
			set
			{
				ThrowOnDisposed();
				if (!FreeImage.SetBackgroundColor(dib, (value.HasValue ? new RGBQUAD[] { value.Value } : null)))
				{
					throw new Exception("Setting background color failed.");
				}
			}
		}

		/// <summary>
		/// Pointer to the data-bits of the bitmap.
		/// </summary>
		public IntPtr Bits
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetBits(dib);
			}
		}

		/// <summary>
		/// Width of the bitmap in bytes.
		/// </summary>
		public int Line
		{
			get
			{
				ThrowOnDisposed();
				return (int)FreeImage.GetLine(dib);
			}
		}

		/// <summary>
		/// Pointer to the scanline of the bitmap's top most pixel row.
		/// </summary>
		public IntPtr Scan0
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetScanLine(dib, (int)(FreeImage.GetHeight(dib) - 1));
			}
		}

		/// <summary>
		/// Width of the bitmap in bytes.
		/// In case the bitmap is top down 'Stride' will be positive, else negative.
		/// </summary>
		public int Stride
		{
			get
			{
				return -Line;
			}
		}

		/// <summary>
		/// Gets attribute flags for the pixel data of this bitmap.
		/// </summary>
		public unsafe int Flags
		{
			get
			{
				ThrowOnDisposed();
				int result = 0;
				byte alpha;
				int cd = ColorDepth;

				if ((cd == 32) || (FreeImage.GetTransparencyCount(dib) != 0))
				{
					result += (int)ImageFlags.HasAlpha;
				}

				if (cd == 32)
				{
					uint width = FreeImage.GetWidth(dib);
					uint height = FreeImage.GetHeight(dib);
					for (int y = 0; y < height; y++)
					{
						RGBQUAD* scanline = (RGBQUAD*)FreeImage.GetScanLine(dib, y);
						for (int x = 0; x < width; x++)
						{
							alpha = scanline[x].color.A;
							if (alpha != byte.MinValue && alpha != byte.MaxValue)
							{
								result += (int)ImageFlags.HasTranslucent;
								y = (int)height;
								break;
							}
						}
					}
				}
				else if (FreeImage.GetTransparencyCount(dib) != 0)
				{
					byte[] transTable = FreeImage.GetTransparencyTableEx(dib);
					for (int i = 0; i < transTable.Length; i++)
					{
						if (transTable[i] != byte.MinValue && transTable[i] != byte.MaxValue)
						{
							result += (int)ImageFlags.HasTranslucent;
							break;
						}
					}
				}

				if (FreeImage.GetICCProfileEx(dib).IsCMYK)
				{
					result += (int)ImageFlags.ColorSpaceCmyk;
				}
				else
				{
					result += (int)ImageFlags.ColorSpaceRgb;
				}

				if (FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISBLACK ||
					FreeImage.GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_MINISWHITE)
				{
					result += (int)ImageFlags.ColorSpaceGray;
				}

				if (originalFormat == FREE_IMAGE_FORMAT.FIF_BMP ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_FAXG3 ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_ICO ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_JPEG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PCX ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PNG ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_PSD ||
					originalFormat == FREE_IMAGE_FORMAT.FIF_TIFF)
				{
					result += (int)ImageFlags.HasRealDpi;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets the width and height of this bitmap.
		/// </summary>
		public SizeF PhysicalDimension
		{
			get
			{
				ThrowOnDisposed();
				return new SizeF((float)FreeImage.GetWidth(dib), (float)FreeImage.GetHeight(dib));
			}
		}

		/// <summary>
		/// Gets the pixel format for this bitmap.
		/// </summary>
		public PixelFormat PixelFormat
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetPixelFormat(dib);
			}
		}

		/// <summary>
		/// Gets IDs of the property items stored in this bitmap.
		/// </summary>
		public int[] PropertyIdList
		{
			get
			{
				ThrowOnDisposed();
				List<int> list = new List<int>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.ID);
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets all the property items (pieces of metadata) stored in this bitmap.
		/// </summary>
		public PropertyItem[] PropertyItems
		{
			get
			{
				ThrowOnDisposed();
				List<PropertyItem> list = new List<PropertyItem>();
				ImageMetadata metaData = new ImageMetadata(dib, true);

				foreach (MetadataModel metadataModel in metaData)
				{
					foreach (MetadataTag metadataTag in metadataModel)
					{
						list.Add(metadataTag.GetPropertyItem());
					}
				}

				return list.ToArray();
			}
		}

		/// <summary>
		/// Gets the format of this bitmap.
		/// </summary>
		public ImageFormat RawFormat
		{
			get
			{
				ThrowOnDisposed();
				Attribute guidAttribute =
					Attribute.GetCustomAttribute(
						typeof(FreeImageBitmap), typeof(System.Runtime.InteropServices.GuidAttribute)
					);
				return (guidAttribute == null) ?
					null :
					new ImageFormat(new Guid(((GuidAttribute)guidAttribute).Value));
			}
		}

		/// <summary>
		/// Gets the width and height, in pixels, of this bitmap.
		/// </summary>
		public Size Size
		{
			get
			{
				ThrowOnDisposed();
				return new Size(Width, Height);
			}
		}

		/// <summary>
		/// Gets or sets an object that provides additional data about the bitmap.
		/// </summary>
		public Object Tag
		{
			get
			{
				ThrowOnDisposed();
				return tag;
			}
			set
			{
				ThrowOnDisposed();
				tag = value;
			}
		}

		/// <summary>
		/// Gets whether the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return disposed;
			}
		}

		/// <summary>
		/// Gets a new instance of a metadata wrapping class.
		/// </summary>
		public ImageMetadata Metadata
		{
			get
			{
				ThrowOnDisposed();
				return new ImageMetadata(dib, true);
			}
		}

		/// <summary>
		/// Gets or sets the comment of the bitmap.
		/// Supported formats are JPEG, PNG and GIF.
		/// </summary>
		public string Comment
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetImageComment(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetImageComment(dib, value);
			}
		}

		/// <summary>
		/// Returns whether the bitmap has a palette.
		/// </summary>
		public bool HasPalette
		{
			get
			{
				ThrowOnDisposed();
				return (FreeImage.GetPalette(dib) != IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets or sets the bitmap's palette entry used as transparent color.
		/// Only works for 1-, 4- and 8-bpp.
		/// </summary>
		public int TransparentIndex
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetTransparentIndex(dib);
			}
			set
			{
				ThrowOnDisposed();
				FreeImage.SetTransparentIndex(dib, value);
			}
		}

		/// <summary>
		/// Gets the number of frames in the bitmap.
		/// </summary>
		public int FrameCount
		{
			get
			{
				ThrowOnDisposed();
				int result = 1;
				if (!mdib.IsNull)
				{
					result = FreeImage.GetPageCount(mdib);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the ICCProfile structure of the bitmap.
		/// </summary>
		public FIICCPROFILE ICCProfile
		{
			get
			{
				ThrowOnDisposed();
				return FreeImage.GetICCProfileEx(dib);
			}
		}

		/// <summary>
		/// Gets the format of the original image in case
		/// the bitmap was loaded from a file or stream.
		/// </summary>
		public FREE_IMAGE_FORMAT ImageFormat
		{
			get
			{
				ThrowOnDisposed();
				return originalFormat;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the bounds of the image in the specified unit.
		/// </summary>
		/// <param name="pageUnit">One of the GraphicsUnit values indicating
		/// the unit of measure for the bounding rectangle.</param>
		/// <returns>The RectangleF that represents the bounds of the image,
		/// in the specified unit.</returns>
		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			ThrowOnDisposed();
			pageUnit = GraphicsUnit.Pixel;
			return new RectangleF(
					0f,
					0f,
					(float)FreeImage.GetWidth(dib),
					(float)FreeImage.GetHeight(dib));
		}

		/// <summary>
		/// Gets the specified property item from this bitmap.
		/// </summary>
		/// <param name="propid">The ID of the property item to get.</param>
		/// <returns>The PropertyItem this method gets.</returns>
		public PropertyItem GetPropertyItem(int propid)
		{
			ThrowOnDisposed();
			ImageMetadata metadata = new ImageMetadata(dib, true);
			foreach (MetadataModel metadataModel in metadata)
			{
				foreach (MetadataTag tag in metadataModel)
				{
					if (tag.ID == propid)
					{
						return tag.GetPropertyItem();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a thumbnail for this bitmap.
		/// </summary>
		/// <param name="thumbWidth">The width, in pixels, of the requested thumbnail image.</param>
		/// <param name="thumbHeight">The height, in pixels, of the requested thumbnail image.</param>
		/// <param name="callback">Ignored.</param>
		/// <param name="callBackData">Ignored.</param>
		/// <returns>A bitmap that represents the thumbnail.</returns>
		public FreeImageBitmap GetThumbnailImage(int thumbWidth, int thumbHeight,
			Image.GetThumbnailImageAbort callback, IntPtr callBackData)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(
				dib, thumbWidth, thumbHeight, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Returns a thumbnail for this FreeImageBitmap, keeping aspect ratio.
		/// <paramref name="maxPixelSize"/> defines the maximum width or height
		/// of the thumbnail.
		/// </summary>
		/// <param name="maxPixelSize">Thumbnail square size.</param>
		/// <param name="convert">When true HDR images are transperantly
		/// converted to standard images.</param>
		/// <returns>The thumbnail in a new instance.</returns>
		public FreeImageBitmap GetThumbnailImage(int maxPixelSize, bool convert)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.MakeThumbnail(dib, maxPixelSize, convert);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Returns a structure, wrapping the bitmap's scanline.
		/// Due to FreeImage bitmaps are bottum up, scanline 0 is the
		/// bottom-most line of the image.
		/// <para>
		/// The structures are FI1BITARRAY (1bpp), FI4BITARRAY (4bpp),
		/// FI8BITARRAY (8bpp), FI16RGBARRAY (16bpp), RGBTRIPLEARRAY (24bpp)
		/// or RGBQUADARRAY (32bpp).
		/// </para>
		/// </summary>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Structure wrapping the scanline.</returns>
		/// <exception cref="ArgumentException">
		/// The bitmap's type or color depth are not supported.
		/// </exception>
		public object GetScanline(int scanline)
		{
			ThrowOnDisposed();
			object result = null;

			switch (FreeImage.GetBPP(dib))
			{
				case 1u: result = new FI1BITARRAY(dib, scanline); break;
				case 4u: result = new FI4BITARRAY(dib, scanline); break;
				case 8u: result = new FI8BITARRAY(dib, scanline); break;
				case 16u: result = new FI16RGBARRAY(dib, scanline); break;
				case 24u: result = new RGBTRIPLEARRAY(dib, scanline); break;
				case 32u: result = new RGBQUADARRAY(dib, scanline); break;
				default: throw new ArgumentException("Color depth or type is not supported.");
			}

			return result;
		}

		/// <summary>
		/// Returns a pointer to the specified scanline.
		/// Due to FreeImage bitmaps are bottum up,
		/// scanline 0 is the most bottom line of the image.
		/// </summary>
		/// <param name="scanline">Number of the scanline.</param>
		/// <returns>Pointer to the scanline.</returns>
		public IntPtr GetScanlinePointer(int scanline)
		{
			ThrowOnDisposed();
			return FreeImage.GetScanLine(dib, scanline);
		}

		/// <summary>
		/// Returns a list of structures, wrapping the bitmap's scanlines.
		/// Due to FreeImage bitmaps are bottum up, scanline 0 is the
		/// bottom-most line of the image.
		/// Each color depth has a different wrapping structure
		/// due to different memory layouts.
		/// <para>
		/// The structures are FI1BITARRAY (1bpp), FI4BITARRAY (4bpp),
		/// FI8BITARRAY (8bpp), FI16RGBARRAY (16bpp), RGBTRIPLEARRAY (24bpp)
		/// or RGBQUADARRAY (32bpp).
		/// </para>
		/// </summary>
		/// <returns>A list containing structures wrapping the bitmap's
		/// scanlines.</returns>
		/// <remarks>
		/// Instead of calling his method it is also possible to the
		/// <c>foreach</c> clause to iterate over each scanline.
		/// </remarks>
		/// <exception cref="NotSupportedException">
		/// The bitmap's type or color depth are not supported.
		/// </exception>
		public IList GetScanlines()
		{
			ThrowOnDisposed();

			IList list = null;
			int height = (int)FreeImage.GetHeight(dib);

			switch (FreeImage.GetBPP(dib))
			{
				case 1u:

					list = new List<FI1BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI1BITARRAY(dib, i));
					}
					break;

				case 4u:

					list = new List<FI4BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI4BITARRAY(dib, i));
					}
					break;

				case 8u:

					list = new List<FI8BITARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI8BITARRAY(dib, i));
					}
					break;

				case 16u:

					list = new List<FI16RGBARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new FI16RGBARRAY(dib, i));
					}
					break;

				case 24u:

					list = new List<RGBTRIPLEARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new RGBTRIPLEARRAY(dib, i));
					}
					break;

				case 32u:

					list = new List<RGBQUADARRAY>(height);
					for (int i = 0; i < height; i++)
					{
						list.Add(new RGBQUADARRAY(dib, i));
					}
					break;

				default:

					throw new NotSupportedException("Color depth or type is not supported.");
			}

			return list;
		}

		/// <summary>
		/// Removes the specified property item from this bitmap.
		/// </summary>
		/// <param name="propid">The ID of the property item to remove.</param>
		public void RemovePropertyItem(int propid)
		{
			ThrowOnDisposed();
			ImageMetadata mdata = new ImageMetadata(dib, true);
			foreach (MetadataModel model in mdata)
			{
				foreach (MetadataTag tag in model)
				{
					if (tag.ID == propid)
					{
						model.RemoveTag(tag.Key);
						return;
					}
				}
			}
		}

		/// <summary>
		/// This method rotates, flips, or rotates and flips the bitmap.
		/// </summary>
		/// <param name="rotateFlipType">A RotateFlipType member
		/// that specifies the type of rotation and flip to apply to the bitmap.</param>
		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			ThrowOnDisposed();

			FIBITMAP newDib = 0;
			uint bpp = FreeImage.GetBPP(dib);

			switch (rotateFlipType)
			{
				case RotateFlipType.RotateNoneFlipX:

					FreeImage.FlipHorizontal(dib);
					break;

				case RotateFlipType.RotateNoneFlipY:

					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.RotateNoneFlipXY:

					FreeImage.FlipHorizontal(dib);
					FreeImage.FlipVertical(dib);
					break;

				case RotateFlipType.Rotate90FlipNone:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					break;

				case RotateFlipType.Rotate90FlipX:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					break;

				case RotateFlipType.Rotate90FlipY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate90FlipXY:

					newDib = (bpp == 4u) ? FreeImage.Rotate4bit(dib, 90d) : FreeImage.RotateClassic(dib, 90d);
					FreeImage.FlipHorizontal(newDib);
					FreeImage.FlipVertical(newDib);
					break;

				case RotateFlipType.Rotate180FlipXY:
					newDib = FreeImage.Clone(dib);
					break;
			}
			ReplaceDib(newDib);
		}

		/// <summary>
		/// Saves this bitmap to the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename)
		{
			Save(filename, FREE_IMAGE_FORMAT.FIF_UNKNOWN, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this bitmap to the specified file in the specified format.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format)
		{
			Save(filename, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this bitmap to the specified file in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file to which
		/// to save this bitmap.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentException"><paramref name="filename"/> is null or empty.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(string filename, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			ThrowOnDisposed();
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("filename");
			}
			if (!FreeImage.SaveEx(dib, filename, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = filename;
			saveInformation.format = format;
			saveInformation.saveFlags = flags;
		}

		/// <summary>
		/// Saves this image to the specified stream in the specified format.
		/// </summary>
		/// <param name="stream">The stream where the image will be saved.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format)
		{
			Save(stream, format, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves this image to the specified stream in the specified format
		/// using the specified saving flags.
		/// </summary>
		/// <param name="stream">The stream where the image will be saved.</param>
		/// <param name="format">An FREE_IMAGE_FORMAT that specifies the format of the saved image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void Save(Stream stream, FREE_IMAGE_FORMAT format, FREE_IMAGE_SAVE_FLAGS flags)
		{
			ThrowOnDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!FreeImage.SaveToStream(dib, stream, format, flags))
			{
				throw new Exception();
			}

			saveInformation.filename = null;
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the Save method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd()
		{
			SaveAdd(this);
		}

		/// <summary>
		/// Adds a frame to the file specified in a previous call to the Save method.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="bitmap">A FreeImageBitmap that contains the frame to add.</param>
		/// <exception cref="InvalidOperationException">
		/// This instance has not been saved to a file using Save(...) before.</exception>
		public void SaveAdd(FreeImageBitmap bitmap)
		{
			if (saveInformation.filename == null)
			{
				throw new InvalidOperationException();
			}

			SaveAdd(
				saveInformation.filename,
				bitmap,
				saveInformation.format,
				saveInformation.loadFlags,
				saveInformation.saveFlags);
		}

		/// <summary>
		/// Adds a frame to the file specified.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(string filename)
		{
			SaveAdd(
				filename,
				this,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Adds a frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		public void SaveAdd(
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			SaveAdd(
				filename,
				this,
				format,
				loadFlags,
				saveFlags);
		}

		/// <summary>
		/// Selects the frame specified by the dimension and index.
		/// </summary>
		/// <param name="frameIndex">The index of the active frame.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="frameIndex"/> is out of range.
		/// </exception>
		/// <exception cref="Exception">The operation failed.</exception>
		public void SelectActiveFrame(int frameIndex)
		{
			ThrowOnDisposed();
			if (frameIndex < 0)
			{
				throw new ArgumentOutOfRangeException("frameIndex");
			}
			if (!mdib.IsNull)
			{
				if (frameIndex >= FrameCount)
				{
					throw new ArgumentOutOfRangeException("frameIndex");
				}
				ReplaceDib(FreeImage.LockPage(mdib, frameIndex));
				if (dib.IsNull)
				{
					throw new Exception();
				}
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap()
		{
			ThrowOnDisposed();
			return FreeImage.GetHbitmap(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap.
		/// </summary>
		/// <param name="background">A Color structure that specifies the background color.
		/// This parameter is ignored if the bitmap is totally opaque.</param>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmap(Color background)
		{
			ThrowOnDisposed();
			using (FreeImageBitmap temp = new FreeImageBitmap(this))
			{
				temp.BackgroundColor = background;
				return temp.GetHbitmap();
			}
		}

		/// <summary>
		/// Returns the handle to an icon.
		/// </summary>
		/// <returns>A Windows handle to an icon with the same image as the FreeImageBitmap.</returns>
		public IntPtr GetHicon()
		{
			ThrowOnDisposed();
			using (Bitmap bitmap = FreeImage.GetBitmap(dib, true))
			{
				return bitmap.GetHicon();
			}
		}

		/// <summary>
		/// Creates a GDI bitmap object from this FreeImageBitmap with the same
		/// color depth as the primary device.
		/// </summary>
		/// <returns>A handle to the GDI bitmap object that this method creates.</returns>
		public IntPtr GetHbitmapForDevice()
		{
			ThrowOnDisposed();
			return FreeImage.GetBitmapForDevice(dib, IntPtr.Zero, false);
		}

		/// <summary>
		/// Gets the color of the specified pixel in this FreeImageBitmap.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to retrieve.</param>
		/// <param name="y">The y-coordinate of the pixel to retrieve.</param>
		/// <returns>A Color structure that represents the color of the specified pixel.</returns>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe Color GetPixel(int x, int y)
		{
			ThrowOnDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq;
					if (!FreeImage.GetPixelColor(dib, (uint)x, (uint)y, out rgbq))
					{
						throw new Exception();
					}
					return rgbq.color;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					byte index;
					if (!FreeImage.GetPixelIndex(dib, (uint)x, (uint)y, out index))
					{
						throw new Exception();
					}
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					return palette[index].color;
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Makes the default transparent color transparent for this FreeImageBitmap.
		/// </summary>
		public void MakeTransparent()
		{
			ThrowOnDisposed();
			MakeTransparent(Color.Transparent);
		}

		/// <summary>
		/// Makes the specified color transparent for this FreeImageBitmap.
		/// </summary>
		/// <param name="transparentColor">The Color structure that represents
		/// the color to make transparent.</param>
		public void MakeTransparent(Color transparentColor)
		{
			ThrowOnDisposed();
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Sets the color of the specified pixel in this FreeImageBitmap.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel to set.</param>
		/// <param name="y">The y-coordinate of the pixel to set.</param>
		/// <param name="color">A Color structure that represents the color
		/// to assign to the specified pixel.</param>
		/// <exception cref="Exception">The operation failed.</exception>
		/// <exception cref="NotSupportedException">The type of this bitmap is not supported.</exception>
		public unsafe void SetPixel(int x, int y, Color color)
		{
			ThrowOnDisposed();
			if (FreeImage.GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				if (ColorDepth == 16 || ColorDepth == 24 || ColorDepth == 32)
				{
					RGBQUAD rgbq = color;
					if (!FreeImage.SetPixelColor(dib, (uint)x, (uint)y, ref rgbq))
					{
						throw new Exception();
					}
					return;
				}
				else if (ColorDepth == 1 || ColorDepth == 4 || ColorDepth == 8)
				{
					uint colorsUsed = FreeImage.GetColorsUsed(dib);
					RGBQUAD* palette = (RGBQUAD*)FreeImage.GetPalette(dib);
					for (int i = 0; i < colorsUsed; i++)
					{
						if (palette[i].color == color)
						{
							byte index = (byte)i;
							if (!FreeImage.SetPixelIndex(dib, (uint)x, (uint)y, ref index))
							{
								throw new Exception();
							}
							return;
						}
					}
					throw new ArgumentOutOfRangeException("color");
				}
			}
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the resolution for this FreeImageBitmap.
		/// </summary>
		/// <param name="xDpi">The horizontal resolution, in dots per inch, of the FreeImageBitmap.</param>
		/// <param name="yDpi">The vertical resolution, in dots per inch, of the FreeImageBitmap.</param>
		public void SetResolution(float xDpi, float yDpi)
		{
			ThrowOnDisposed();
			FreeImage.SetResolutionX(dib, (uint)xDpi);
			FreeImage.SetResolutionY(dib, (uint)yDpi);
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is not yet implemented.
		/// </summary>
		public void UnlockBits(BitmapData bitmapdata)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this FreeImageBitmap into a different color depth.
		/// The parameter 'bpp' specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the 'FICD_FORCE_GREYSCALE' flag will first perform a
		/// convesion to greyscale. This can be done with any target color depth.</para>
		/// <para>Adding the 'FICD_REORDER_PALETTE' flag will allow the algorithm
		/// to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.ConvertColorDepth(dib, bpp, false));
		}

		/// <summary>
		/// Converts this bitmap's FREE_IMAGE_TYPE to 'type' creating
		/// a new instance.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool ConvertType(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			ThrowOnDisposed();
			return (ImageType == type) ? false : ReplaceDib(FreeImage.ConvertToType(dib, type, scaleLinear));
		}

		/// <summary>
		/// Converts this bitmap's FREE_IMAGE_TYPE to 'type'.
		/// In case source and destination type are the same, the operation fails.
		/// An error message can be catched using the 'Message' event.
		/// </summary>
		/// <param name="type">Destination type.</param>
		/// <param name="scaleLinear">True to scale linear, else false.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetTypeConvertedInstance(FREE_IMAGE_TYPE type, bool scaleLinear)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			if (ImageType != type)
			{
				FIBITMAP newDib = FreeImage.ConvertToType(dib, type, scaleLinear);
				if (!newDib.IsNull)
				{
					result = new FreeImageBitmap();
					result.dib = newDib;
				}
			}
			return result;
		}

		/// <summary>
		/// Converts this FreeImageBitmap into a different color depth creating
		/// a new instance.
		/// The parameter 'bpp' specifies color depth, greyscale conversion
		/// and palette reorder.
		/// <para>Adding the 'FICD_FORCE_GREYSCALE' flag will first perform a
		/// convesion to greyscale. This can be done with any target color depth.</para>
		/// <para>Adding the 'FICD_REORDER_PALETTE' flag will allow the algorithm
		/// to reorder the palette. This operation will not be performed to
		/// non-greyscale images to prevent data lost by mistake.</para>
		/// </summary>
		/// <param name="bpp">A bitfield containing information about the conversion
		/// to perform.</param>
		/// <returns>The converted instance.</returns>
		public FreeImageBitmap GetColorConvertedInstance(FREE_IMAGE_COLOR_DEPTH bpp)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.ConvertColorDepth(dib, bpp, false);
			if (newDib == dib)
			{
				newDib = FreeImage.Clone(dib);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return Rescale(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter.
		/// </summary>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rescale(int width, int height, FREE_IMAGE_FILTER filter)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.Rescale(dib, width, height, filter));
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter creating a new instance.
		/// </summary>
		/// <param name="newSize">The Size structure that represent the
		/// size of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(Size newSize, FREE_IMAGE_FILTER filter)
		{
			return GetScaledInstance(newSize.Width, newSize.Height, filter);
		}

		/// <summary>
		/// Rescales this FreeImageBitmap to the specified size using the
		/// specified filter creating a new instance.
		/// </summary>
		/// <param name="width">Width of the new FreeImageBitmap.</param>
		/// <param name="height">Height of the new FreeImageBitmap.</param>
		/// <param name="filter">Filter to use for resizing.</param>
		/// <returns>The rescaled instance.</returns>
		public FreeImageBitmap GetScaledInstance(int width, int height, FREE_IMAGE_FILTER filter)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Rescale(dib, width, height, filter);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global
		/// operator based on logarithmic compression of luminance values, imitating the human response to light.
		/// </summary>
		/// <param name="gamma">A gamma correction that is applied after the tone mapping.
		/// A value of 1 means no correction.</param>
		/// <param name="exposure">Scale factor allowing to adjust the brightness of the output image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoDrago03(double gamma, double exposure)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoDrago03(dib, gamma, exposure));
		}

		/// <summary>
		/// Converts a High Dynamic Range image to a 24-bit RGB image using a global operator inspired
		/// by photoreceptor physiology of the human visual system.
		/// </summary>
		/// <param name="intensity">Controls the overall image intensity in the range [-8, 8].</param>
		/// <param name="contrast">Controls the overall image contrast in the range [0.3, 1.0[.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoReinhard05(double intensity, double contrast)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoReinhard05(dib, intensity, contrast));
		}

		/// <summary>
		/// Apply the Gradient Domain High Dynamic Range Compression to a RGBF image and convert to 24-bit RGB.
		/// </summary>
		/// <param name="color_saturation">Color saturation (s parameter in the paper) in [0.4..0.6]</param>
		/// <param name="attenuation">Atenuation factor (beta parameter in the paper) in [0.8..0.9]</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool TmoFattal02(double color_saturation, double attenuation)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.TmoFattal02(dib, color_saturation, attenuation));
		}

		/// <summary>
		/// This method rotates a 1-, 4-, 8-bit greyscale or a 24-, 32-bit color image by means of 3 shears.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle)
		{
			ThrowOnDisposed();
			bool result = false;
			if (ColorDepth == 4)
			{
				result = ReplaceDib(FreeImage.Rotate4bit(dib, angle));
			}
			else
			{
				result = ReplaceDib(FreeImage.RotateClassic(dib, angle));
			}
			return result;
		}

		/// <summary>
		/// Rotates this FreeImageBitmap by the specified angle creating a new instance.
		/// For 1- and 4-bit images, rotation is limited to angles whose value is an integer
		/// multiple of 90.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib;
			if (ColorDepth == 4)
			{
				newDib = FreeImage.Rotate4bit(dib, angle);
			}
			else
			{
				newDib = FreeImage.RotateClassic(dib, angle);
			}
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Rotate(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.RotateEx(dib, angle, xShift, yShift, xOrigin, yOrigin, useMask));
		}

		/// <summary>
		/// This method performs a rotation and / or translation of an 8-bit greyscale,
		/// 24- or 32-bit image, using a 3rd order (cubic) B-Spline returning a new instance.
		/// </summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="xShift">Horizontal image translation.</param>
		/// <param name="yShift">Vertical image translation.</param>
		/// <param name="xOrigin">Rotation center x-coordinate.</param>
		/// <param name="yOrigin">Rotation center y-coordinate.</param>
		/// <param name="useMask">When true the irrelevant part of the image is set to a black color,
		/// otherwise, a mirroring technique is used to fill irrelevant pixels.</param>
		/// <returns>The rotated instance.</returns>
		public FreeImageBitmap GetRotatedInstance(double angle, double xShift, double yShift,
			double xOrigin, double yOrigin, bool useMask)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.RotateEx(
				dib, angle, xShift, yShift, xOrigin, yOrigin, useMask);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Perfoms an histogram transformation on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="lookUpTable">The lookup table (LUT).
		/// It's size is assumed to be 256 in length.</param>
		/// <param name="channel">The color channel to be transformed.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustCurve(byte[] lookUpTable, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustCurve(dib, lookUpTable, channel);
		}

		/// <summary>
		/// Performs gamma correction on a 8-, 24- or 32-bit image.
		/// </summary>
		/// <param name="gamma">The parameter represents the gamma value to use (gamma > 0).
		/// A value of 1.0 leaves the image alone, less than one darkens it, and greater than one lightens it.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustGamma(double gamma)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustGamma(dib, gamma);
		}

		/// <summary>
		/// Adjusts the brightness of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will make the image darker and greater than 0 will make the image brighter.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustBrightness(double percentage)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustBrightness(dib, percentage);
		}

		/// <summary>
		/// Adjusts the contrast of a 8-, 24- or 32-bit image by a certain amount.
		/// </summary>
		/// <param name="percentage">A value 0 means no change,
		/// less than 0 will decrease the contrast and greater than 0 will increase the contrast of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustContrast(double percentage)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustContrast(dib, percentage);
		}

		/// <summary>
		/// Inverts each pixel data.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Invert()
		{
			ThrowOnDisposed();
			return FreeImage.Invert(dib);
		}

		/// <summary>
		/// Computes the image histogram.
		/// </summary>
		/// <param name="channel">Channel to compute from.</param>
		/// <param name="histogram">Array of integers containing the histogram.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool GetHistogram(FREE_IMAGE_COLOR_CHANNEL channel, out int[] histogram)
		{
			ThrowOnDisposed();
			histogram = new int[256];
			return FreeImage.GetHistogram(dib, histogram, channel);
		}

		/// <summary>
		/// Retrieves the red, green, blue or alpha channel of a 24- or 32-bit image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Insert a 8-bit dib into a 24- or 32-bit image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.SetChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Retrieves the real part, imaginary part, magnitude or phase of a complex image.
		/// </summary>
		/// <param name="channel">The color channel to extract.</param>
		/// <returns>The color channel in a new instance.</returns>
		public FreeImageBitmap GetComplexChannel(FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.GetComplexChannel(dib, channel);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Set the real or imaginary part of a complex image.
		/// Both images must have to same width and height.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to insert.</param>
		/// <param name="channel">The color channel to replace.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool SetComplexChannel(FreeImageBitmap bitmap, FREE_IMAGE_COLOR_CHANNEL channel)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.SetComplexChannel(dib, bitmap.dib, channel);
		}

		/// <summary>
		/// Copy a sub part of this FreeImageBitmap.
		/// </summary>
		/// <param name="rect">The subpart to copy.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(Rectangle rect)
		{
			ThrowOnDisposed();
			return Copy(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Copy a sub part of this FreeImageBitmap.
		/// </summary>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>The sub part in a new instance.</returns>
		public FreeImageBitmap Copy(int left, int top, int right, int bottom)
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Copy(dib, left, top, right, bottom);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with the current image.
		/// The bit depth of 'bitmap' must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to paste into this instance.</param>
		/// <param name="left">Specifies the left position of the sub image.</param>
		/// <param name="top">Specifies the top position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, int left, int top, int alpha)
		{
			ThrowOnDisposed();
			bitmap.ThrowOnDisposed();
			return FreeImage.Paste(dib, bitmap.dib, left, top, alpha);
		}

		/// <summary>
		/// Alpha blend or combine a sub part image with the current image.
		/// The bit depth of 'bitmap' must be greater than or equal to the bit depth this instance.
		/// </summary>
		/// <param name="bitmap">The FreeImageBitmap to paste into this instance.</param>
		/// <param name="point">Specifies the position of the sub image.</param>
		/// <param name="alpha">alpha blend factor.
		/// The source and destination images are alpha blended if alpha=0..255.
		/// If alpha > 255, then the source image is combined to the destination image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Paste(FreeImageBitmap bitmap, Point point, int alpha)
		{
			ThrowOnDisposed();
			return Paste(bitmap, point.X, point.Y, alpha);
		}

		/// <summary>
		/// This method composite a transparent foreground image against a single background color or
		/// against a background image.
		/// In case 'useBitmapBackground' is false and 'applicationBackground' and 'bitmapBackGround'
		/// are null, a checkerboard will be used as background.
		/// </summary>
		/// <param name="useBitmapBackground">When true the background of this instance is used
		/// if it contains one.</param>
		/// <param name="applicationBackground">Backgroundcolor used in case 'useBitmapBackground' is false
		/// and 'applicationBackground' is not null.</param>
		/// <param name="bitmapBackGround">Background used in case 'useBitmapBackground' is false and 
		/// 'applicationBackground' is null.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool Composite(bool useBitmapBackground, Color? applicationBackground, FreeImageBitmap bitmapBackGround)
		{
			ThrowOnDisposed();
			bitmapBackGround.ThrowOnDisposed();
			RGBQUAD? rgb = applicationBackground;
			return ReplaceDib(
				FreeImage.Composite(
					dib,
					useBitmapBackground,
					rgb.HasValue ? new RGBQUAD[] { rgb.Value } : null,
					bitmapBackGround.dib));
		}

		/// <summary>
		/// Applies the alpha value of each pixel to its color components.
		/// The aplha value stays unchanged.
		/// Only works with 32-bits color depth.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool PreMultiplyWithAlpha()
		{
			ThrowOnDisposed();
			return FreeImage.PreMultiplyWithAlpha(dib);
		}

		/// <summary>
		/// Solves a Poisson equation, remap result pixels to [0..1] and returns the solution.
		/// </summary>
		/// <param name="ncycle">Number of cycles in the multigrid algorithm (usually 2 or 3)</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool MultigridPoissonSolver(int ncycle)
		{
			ThrowOnDisposed();
			return ReplaceDib(FreeImage.MultigridPoissonSolver(dib, ncycle));
		}

		/// <summary>
		/// Adjusts an image's brightness, contrast and gamma as well as it may
		/// optionally invert the image within a single operation.
		/// </summary>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para>
		/// This parameter must not be zero or smaller than zero.
		/// If so, it will be ignored and no gamma correction will be performed on the image.</param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool AdjustColors(double brightness, double contrast, double gamma, bool invert)
		{
			ThrowOnDisposed();
			return FreeImage.AdjustColors(dib, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Applies color mapping for one or several colors on a 1-, 4- or 8-bit
		/// palletized or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="srccolors">Array of colors to be used as the mapping source.</param>
		/// <param name="dstcolors">Array of colors to be used as the mapping destination.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <param name="swap">If true, source and destination colors are swapped, that is,
		/// each destination color is also mapped to the corresponding source color.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyColorMapping(RGBQUAD[] srccolors, RGBQUAD[] dstcolors, bool ignore_alpha, bool swap)
		{
			ThrowOnDisposed();
			if (srccolors == null)
			{
				throw new ArgumentNullException("srccolors");
			}
			if (dstcolors == null)
			{
				throw new ArgumentNullException("dstcolors");
			}
			if (srccolors.Length != dstcolors.Length)
			{
				throw new ArgumentException("srccolors and dstcolors must have the same length.");
			}
			return FreeImage.ApplyColorMapping(dib, srccolors, dstcolors, (uint)srccolors.Length, ignore_alpha, swap);
		}

		/// <summary>
		/// Swaps two specified colors on a 1-, 4- or 8-bit palletized
		/// or a 16-, 24- or 32-bit high color image.
		/// </summary>
		/// <param name="color_a">One of the two colors to be swapped.</param>
		/// <param name="color_b">The other of the two colors to be swapped.</param>
		/// <param name="ignore_alpha">If true, 32-bit images and colors are treated as 24-bit.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapColors(RGBQUAD color_a, RGBQUAD color_b, bool ignore_alpha)
		{
			ThrowOnDisposed();
			return FreeImage.SwapColors(dib, ref color_a, ref color_b, ignore_alpha);
		}

		/// <summary>
		/// Applies palette index mapping for one or several indices
		/// on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="srcindices">Array of palette indices to be used as the mapping source.</param>
		/// <param name="dstindices">Array of palette indices to be used as the mapping destination.</param>
		/// <param name="count">The number of palette indices to be mapped. This is the size of both
		/// srcindices and dstindices</param>
		/// <param name="swap">If true, source and destination palette indices are swapped, that is,
		/// each destination index is also mapped to the corresponding source index.</param>
		/// <returns>The total number of pixels changed.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="srccolors"/> or <paramref name="dstcolors"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="srccolors"/> has a different length than <paramref name="dstcolors"/>.
		/// </exception>
		public uint ApplyPaletteIndexMapping(byte[] srcindices, byte[] dstindices, uint count, bool swap)
		{
			ThrowOnDisposed();
			if (srcindices == null)
			{
				throw new ArgumentNullException("srcindices");
			}
			if (dstindices == null)
			{
				throw new ArgumentNullException("dstindices");
			}
			if (srcindices.Length != dstindices.Length)
			{
				throw new ArgumentException("srcindices and dstindices must have the same length.");
			}
			return FreeImage.ApplyPaletteIndexMapping(dib, srcindices, dstindices, (uint)srcindices.Length, swap);
		}

		/// <summary>
		/// Swaps two specified palette indices on a 1-, 4- or 8-bit palletized image.
		/// </summary>
		/// <param name="index_a">One of the two palette indices to be swapped.</param>
		/// <param name="index_b">The other of the two palette indices to be swapped.</param>
		/// <returns>The total number of pixels changed.</returns>
		public uint SwapPaletteIndices(byte index_a, byte index_b)
		{
			ThrowOnDisposed();
			return FreeImage.SwapPaletteIndices(dib, ref index_a, ref index_b);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return CreateICCProfile(data, data.Length);
		}

		/// <summary>
		/// Creates a new ICC-Profile.
		/// </summary>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <param name="size">The number of bytes of 'data' to use.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		public FIICCPROFILE CreateICCProfile(byte[] data, int size)
		{
			ThrowOnDisposed();
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return FreeImage.CreateICCProfileEx(dib, data, size);
		}

		#endregion

		#region Static functions

		/// <summary>
		/// Returns a value that indicates whether the pixel format for this image contains alpha information.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat to test.</param>
		/// <returns>true if pixfmt contains alpha information; otherwise, false.</returns>
		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsAlphaPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 32 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat to test.</param>
		/// <returns>true if pixfmt is canonical; otherwise, false.</returns>
		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsCanonicalPixelFormat(pixfmt);
		}

		/// <summary>
		/// Returns a value that indicates whether the pixel format is 64 bits per pixel.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat enumeration to test.</param>
		/// <returns>true if pixfmt is extended; otherwise, false.</returns>
		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			return Bitmap.IsExtendedPixelFormat(pixfmt);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a Windows handle to an icon.
		/// </summary>
		/// <param name="hicon">A handle to an icon.</param>
		/// <returns>The FreeImageBitmap that this method creates.</returns>
		public static FreeImageBitmap FromHicon(IntPtr hicon)
		{
			using (Bitmap bitmap = Bitmap.FromHicon(hicon))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified Windows resource.
		/// </summary>
		/// <param name="hinstance">A handle to an instance of the executable
		/// file that contains the resource.</param>
		/// <param name="bitmapName">A string containing the name of the resource bitmap.</param>
		/// <returns>The FreeImageBitmap that this method creates.</returns>
		public static FreeImageBitmap FromResource(IntPtr hinstance, string bitmapName)
		{
			using (Bitmap bitmap = Bitmap.FromResource(hinstance, bitmapName))
			{
				return new FreeImageBitmap(bitmap);
			}
		}

		/// <summary>
		/// Creates an FreeImageImage from the specified file.
		/// </summary>
		/// <param name="filename">A string that contains the name of the file
		/// from which to create the FreeImageBitmap.</param>
		/// <returns>The FreeImageImage this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates an Image from the specified file
		/// using embedded color management information in that file.
		/// </summary>
		/// <param name="filename">A string that contains the
		/// name of the file from which to create the FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromFile(string filename, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(filename);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a handle to a GDI bitmap.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the FreeImageBitmap.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap)
		{
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.CreateFromHbitmap(hbitmap, IntPtr.Zero);
			if (!newDib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
			}
			return result;
		}

		/// <summary>
		/// Creates a FreeImageBitmap from a handle to a GDI bitmap and a handle to a GDI palette.
		/// </summary>
		/// <param name="hbitmap">The GDI bitmap handle from which to create the FreeImageBitmap.</param>
		/// <param name="hpalette">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
		{
			return FromHbitmap(hbitmap);
		}

		/// <summary>
		/// Frees a bitmap handle.
		/// </summary>
		/// <param name="hbitmap">Handle to a bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool FreeHbitmap(IntPtr hbitmap)
		{
			return FreeImage.FreeHbitmap(hbitmap);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream)
		{
			try
			{
				return new FreeImageBitmap(stream);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Creates a FreeImageBitmap from the specified data stream.
		/// </summary>
		/// <param name="stream">A Stream that contains the data for this FreeImageBitmap.</param>
		/// <param name="useEmbeddedColorManagement">Ignored.</param>
		/// <param name="validateImageData">Ignored.</param>
		/// <returns>The FreeImageBitmap this method creates.</returns>
		public static FreeImageBitmap FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
		{
			return new FreeImageBitmap(stream);
		}

		/// <summary>
		/// Returns the color depth, in number of bits per pixel,
		/// of the specified pixel format.
		/// </summary>
		/// <param name="pixfmt">The PixelFormat member that specifies
		/// the format for which to find the size.</param>
		/// <returns>The color depth of the specified pixel format.</returns>
		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			return Bitmap.GetPixelFormatSize(pixfmt);
		}

		/// <summary>
		/// Performs a lossless rotation or flipping on a JPEG file.
		/// </summary>
		/// <param name="source">Source file.</param>
		/// <param name="destination">Destination file; can be the source file; will be overwritten.</param>
		/// <param name="operation">The operation to apply.</param>
		/// <param name="perfect">To avoid lossy transformation, you can set the perfect parameter to true.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool JPEGTransform(string source, string destination, FREE_IMAGE_JPEG_OPERATION operation, bool perfect)
		{
			return FreeImage.JPEGTransform(source, destination, operation, perfect);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="rect">Specifies the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, Rectangle rect)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return JPEGCrop(source, destination, rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
		/// Performs a lossless crop on a JPEG file.
		/// </summary>
		/// <param name="source">Source filename.</param>
		/// <param name="destination">Destination filename.</param>
		/// <param name="left">Specifies the left position of the cropped rectangle.</param>
		/// <param name="top">Specifies the top position of the cropped rectangle.</param>
		/// <param name="right">Specifies the right position of the cropped rectangle.</param>
		/// <param name="bottom">Specifies the bottom position of the cropped rectangle.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> or <paramref name="destination"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// <paramref name="source"/> does not exist.
		/// </exception>
		public static bool JPEGCrop(string source, string destination, int left, int top, int right, int bottom)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!File.Exists(source))
			{
				throw new FileNotFoundException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			return FreeImage.JPEGCrop(source, destination, left, top, right, bottom);
		}

		/// <summary>
		/// Converts a X11 color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupX11Color(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupX11Color(color, out red, out green, out blue);
		}

		/// <summary>
		/// Converts a SVG color name into a corresponding RGB value.
		/// </summary>
		/// <param name="color">Name of the color to convert.</param>
		/// <param name="red">Red component.</param>
		/// <param name="green">Green component.</param>
		/// <param name="blue">Blue component.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="color"/> is null.</exception>
		public static bool LookupSVGColor(string color, out byte red, out byte green, out byte blue)
		{
			if (color == null)
			{
				throw new ArgumentNullException("color");
			}
			return FreeImage.LookupSVGColor(color, out red, out green, out blue);
		}

		/// <summary>
		/// Creates a lookup table to be used with AdjustCurve() which
		/// may adjusts brightness and contrast, correct gamma and invert the image with a
		/// single call to AdjustCurve().
		/// </summary>
		/// <param name="lookUpTable">Output lookup table to be used with AdjustCurve().
		/// The size of 'LUT' is assumed to be 256.</param>
		/// <param name="brightness">Percentage brightness value where -100 &lt;= brightness &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will make the image darker and greater
		/// than 0 will make the image brighter.</para></param>
		/// <param name="contrast">Percentage contrast value where -100 &lt;= contrast &lt;= 100.
		/// <para>A value of 0 means no change, less than 0 will decrease the contrast
		/// and greater than 0 will increase the contrast of the image.</para></param>
		/// <param name="gamma">Gamma value to be used for gamma correction.
		/// <para>A value of 1.0 leaves the image alone, less than one darkens it,
		/// and greater than one lightens it.</para></param>
		/// <param name="invert">If set to true, the image will be inverted.</param>
		/// <returns>The number of adjustments applied to the resulting lookup table
		/// compared to a blind lookup table.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="lookUpTable"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="lookUpTable.Length"/> is not 256.</exception>
		public static int GetAdjustColorsLookupTable(byte[] lookUpTable, double brightness, double contrast, double gamma, bool invert)
		{
			if (lookUpTable == null)
			{
				throw new ArgumentNullException("lookUpTable");
			}
			if (lookUpTable.Length != 256)
			{
				throw new ArgumentException("lookUpTable");
			}
			return FreeImage.GetAdjustColorsLookupTable(lookUpTable, brightness, contrast, gamma, invert);
		}

		/// <summary>
		/// Adds a specified frame to the file specified using the specified parameters.
		/// Use this method to save selected frames from a multiple-frame image to
		/// another multiple-frame image.
		/// </summary>
		/// <param name="filename">File to add this frame to.</param>
		/// <param name="bitmap">A FreeImageBitmap that contains the frame to add.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="loadFlags">Flags to enable or disable plugin-features.</param>
		/// <param name="saveFlags">Flags to enable or disable plugin-features.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="filename"/> or <paramref name="bitmap"/> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException"><paramref name="filename"/> does not exist.</exception>
		/// <exception cref="Exception">Saving the image failed.</exception>
		internal static void SaveAdd(
			string filename,
			FreeImageBitmap bitmap,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS loadFlags,
			FREE_IMAGE_SAVE_FLAGS saveFlags)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("filename");
			}
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			bitmap.ThrowOnDisposed();
			if (bitmap.dib.IsNull)
			{
				throw new Exception();
			}

			FIMULTIBITMAP mpBitmap =
				FreeImage.OpenMultiBitmapEx(filename, ref format, loadFlags, false, false, true);

			if (mpBitmap.IsNull)
			{
				throw new Exception();
			}

			FreeImage.AppendPage(mpBitmap, bitmap.dib);

			if (!FreeImage.CloseMultiBitmap(mpBitmap, saveFlags))
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Returns a new instance of the 'PropertyItem' class which
		/// has no public accessible constructor.
		/// </summary>
		/// <returns>A new instace of 'PropertyItem'.</returns>
		public static PropertyItem CreateNewPropertyItem()
		{
			return FreeImage.CreatePropertyItem();
		}

		#endregion

		#region Helper functions

		/// <summary>
		/// Throws an exception in case the instance has already been disposed.
		/// </summary>
		protected void ThrowOnDisposed()
		{
			lock (lockObject)
			{
				if (!this.disposed)
				{
					return;
				}
			}
			throw new ObjectDisposedException(ToString());
		}

		/// <summary>
		/// Tries to replace the wrapped FIBITMAP with a new one.
		/// In case the new dib is null or the same as the already
		/// wrapped one, nothing will be changed and the result will
		/// be false.
		/// Otherwise the wrapped FIBITMAP will be unloaded and replaced.
		/// </summary>
		/// <param name="newDib">The new dib.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		protected bool ReplaceDib(FIBITMAP newDib)
		{
			bool result = false;
			if (dib != newDib && (!newDib.IsNull))
			{
				UnloadDib();
				dib = newDib;
				result = true;
			}
			return result;
		}

		/// <summary>
		/// Unloads currently wrapped FIBITMAP or unlocks the locked page
		/// in case it came from a multipaged bitmap.
		/// </summary>
		protected void UnloadDib()
		{
			if (mdib.IsNull || FreeImage.GetLockedPageCount(mdib) == 0)
			{
				FreeImage.UnloadEx(ref dib);
			}
			else if (!dib.IsNull)
			{
				FreeImage.UnlockPage(mdib, dib, false);
				dib = 0;
			}
		}

		#endregion

		#region Interfaces

		/// <summary>
		/// Helper class to store informations for 'SaveAdd'.
		/// </summary>
		private class SaveInformation : ICloneable
		{
			public string filename = null;
			public FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			public FREE_IMAGE_LOAD_FLAGS loadFlags = FREE_IMAGE_LOAD_FLAGS.DEFAULT;
			public FREE_IMAGE_SAVE_FLAGS saveFlags = FREE_IMAGE_SAVE_FLAGS.DEFAULT;

			public object Clone()
			{
				return base.MemberwiseClone();
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			ThrowOnDisposed();
			FreeImageBitmap result = null;
			FIBITMAP newDib = FreeImage.Clone(dib);
			if (!dib.IsNull)
			{
				result = new FreeImageBitmap();
				result.dib = newDib;
				result.saveInformation = saveInformation.Clone() as SaveInformation;
				result.tag = tag;
				result.originalFormat = originalFormat;
			}
			return result;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">If true managed ressources are released.</param>
		protected virtual void Dispose(bool disposing)
		{
			// Only clean up once
			lock (lockObject)
			{
				if (disposed)
				{
					return;
				}
				disposed = true;
			}

			// Clean up managed resources
			if (disposing)
			{
				tag = null;
			}

			// Clean up unmanaged resources
			UnloadDib();
			FreeImage.CloseMultiBitmapEx(ref mdib);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		/// <exception cref="ArgumentException">The bitmaps's type is not supported.</exception>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetScanlines().GetEnumerator();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ThrowOnDisposed();
			using (MemoryStream memory = new MemoryStream(DataSize))
			{
				if (!FreeImage.SaveToStream(ref dib, memory, FREE_IMAGE_FORMAT.FIF_TIFF, FREE_IMAGE_SAVE_FLAGS.TIFF_LZW, false))
				{
					throw new SerializationException();
				}
				memory.Capacity = (int)memory.Length;
				info.AddValue("Bitmap Data", memory.GetBuffer());
			}
		}

		#endregion
	}

	/// <summary>
	/// Class handling non-bitmap related functions.
	/// </summary>
	public static class FreeImageEngine
	{
		/// <summary>
		/// Gets a value indicating if the FreeImage DLL is available or not.
		/// </summary>
		public static bool IsAvailable
		{
			get
			{
				return FreeImage.IsAvailable();
			}
		}

		/// <summary>
		/// Internal errors in FreeImage generate a logstring that can be
		/// captured by this event.
		/// </summary>
		public static event OutputMessageFunction Message
		{
			add
			{
				FreeImage.Message += value;
			}
			remove
			{
				FreeImage.Message -= value;
			}
		}

		/// <summary>
		/// Gets a string containing the current version of the library.
		/// </summary>
		public static string Version
		{
			get
			{
				return FreeImage.GetVersion();
			}
		}

		/// <summary>
		/// Gets a string containing a standard copyright message.
		/// </summary>
		public static string CopyrightMessage
		{
			get
			{
				return FreeImage.GetCopyrightMessage();
			}
		}

		/// <summary>
		/// Gets whether the platform is using Little Endian.
		/// </summary>
		public static bool IsLittleEndian
		{
			get
			{
				return FreeImage.IsLittleEndian();
			}
		}
	}

	// FreeImages itself is plugin based. Each supported format is integrated by a seperat plugin,
	// that handles loading, saving, descriptions, identifing ect.
	// And of course the user can create own plugins and use them in FreeImage.
	// To do that the above mentioned predefined methodes need to be implemented.
	//
	// The class below handles the creation of such a plugin. The class itself is abstract
	// as well as some core functions that need to be implemented.
	// The class can be used to enable or disable the plugin in FreeImage after regististration or
	// retrieve the formatid, assigned by FreeImage.
	// The class handles the callback functions, garbage collector and pointer operation to make
	// the implementation as user friendly as possible.
	//
	// How to:
	// There are two functions that need to be implemented: 'GetImplementedMethods' and 'FormatProc'.
	// 'GetImplementedMethods' is used by the constructor of the abstract class. FreeImage wants
	// a list of the implemented functions. Each function is represented by a function pointer
	// (a .NET delegate). In case a function is not implemented FreeImage recieves an empty
	// delegate (null). To tell the constructor which functions have been implemented the information
	// is represented by a disjunction of 'MethodFlags'.
	//
	// For example:
	//		return MethodFlags.LoadProc | MethodFlags.SaveProc;
	//
	// The above statement means that LoadProc and SaveProc have been implemented by the user.
	// Keep in mind, that each function has a standard implementation that has static return
	// values that may cause errors if listed in 'GetImplementedMethods' without a real implementation.
	//
	// 'FormatProc' is used by some checks of FreeImage and must be implemented.
	// 'LoadProc' for example can be implemented if the plugin supports reading, but it
	// doesn't have to, the plugin could only be used to save an already loaded bitmap in
	// a special format.

	/// <summary>
	/// Wrapper class for creating an own FreeImage-Plugin.
	/// </summary>
	public abstract class LocalPlugin
	{
		/// <summary>
		/// Struct containing function pointers
		/// </summary>
		private Plugin plugin;
		/// <summary>
		/// Delegate for register callback by FreeImage
		/// </summary>
		private InitProc initProc;
		/// <summary>
		/// GCHandles to prevent the garbage collector from chaning function addresses
		/// </summary>
		private GCHandle[] handles = new GCHandle[16];
		/// <summary>
		/// The format id assiged to the plugin
		/// </summary>
		protected FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
		/// <summary>
		/// When true the plugin was registered successfully else false.
		/// </summary>
		protected readonly bool registered = false;
		/// <summary>
		/// A copy of the functions used to register.
		/// </summary>
		protected readonly MethodFlags implementedMethods;

		/// <summary>
		/// MethodFlags defines values to fill a bitfield telling which
		/// functions have been implemented by a plugin.
		/// </summary>
		[Flags]
		protected enum MethodFlags
		{
			None = 0x0,
			DescriptionProc = 0x1,
			ExtensionListProc = 0x2,
			RegExprProc = 0x4,
			OpenProc = 0x8,
			CloseProc = 0x10,
			PageCountProc = 0x20,
			PageCapabilityProc = 0x40,
			LoadProc = 0x80,
			SaveProc = 0x100,
			ValidateProc = 0x200,
			MimeProc = 0x400,
			SupportsExportBPPProc = 0x800,
			SupportsExportTypeProc = 0x1000,
			SupportsICCProfilesProc = 0x2000
		}

		// Functions that must be implemented.

		/// <summary>
		/// Function that returns a bitfield containing the
		/// implemented methods.
		/// </summary>
		/// <returns>Bitfield of the implemented methods.</returns>
		protected abstract MethodFlags GetImplementedMethods();

		/// <summary>
		/// Implementation of 'FormatProc'
		/// </summary>
		/// <returns>A string containing the plugins format.</returns>
		protected abstract string FormatProc();

		// Functions that can be implemented.

		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string DescriptionProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string ExtensionListProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string RegExprProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual IntPtr OpenProc(ref FreeImageIO io, fi_handle handle, bool read) { return IntPtr.Zero; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual void CloseProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual int PageCountProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { return 0; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual int PageCapabilityProc(ref FreeImageIO io, fi_handle handle, IntPtr data) { return 0; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual FIBITMAP LoadProc(ref FreeImageIO io, fi_handle handle, int page, int flags, IntPtr data) { return 0; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SaveProc(ref FreeImageIO io, FIBITMAP dib, fi_handle handle, int page, int flags, IntPtr data) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool ValidateProc(ref FreeImageIO io, fi_handle handle) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual string MimeProc() { return ""; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsExportBPPProc(int bpp) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsExportTypeProc(FREE_IMAGE_TYPE type) { return false; }
		/// <summary>
		/// Function that can be implemented.
		/// </summary>
		protected virtual bool SupportsICCProfilesProc() { return false; }

		/// <summary>
		/// The constructor automatically registeres the plugin in FreeImage.
		/// To do this it prepares a FreeImage defined structure with function pointers
		/// to the implemented functions or null if not implemented.
		/// Before registing the functions they are pinned in memory so the garbage collector
		/// can't move them around in memory after we passed there addresses to FreeImage.
		/// </summary>
		public LocalPlugin()
		{
			int i = 0;
			implementedMethods = GetImplementedMethods();

			if ((implementedMethods & MethodFlags.DescriptionProc) > 0)
			{
				plugin.descriptionProc = new DescriptionProc(DescriptionProc);
				handles[i++] = GetHandle(plugin.descriptionProc);
			}
			if ((implementedMethods & MethodFlags.ExtensionListProc) > 0)
			{
				plugin.extensionListProc = new ExtensionListProc(ExtensionListProc);
				handles[i++] = GetHandle(plugin.extensionListProc);
			}
			if ((implementedMethods & MethodFlags.RegExprProc) > 0)
			{
				plugin.regExprProc = new RegExprProc(RegExprProc);
				handles[i++] = GetHandle(plugin.regExprProc);
			}
			if ((implementedMethods & MethodFlags.OpenProc) > 0)
			{
				plugin.openProc = new OpenProc(OpenProc);
				handles[i++] = GetHandle(plugin.openProc);
			}
			if ((implementedMethods & MethodFlags.CloseProc) > 0)
			{
				plugin.closeProc = new CloseProc(CloseProc);
				handles[i++] = GetHandle(plugin.closeProc);
			}
			if ((implementedMethods & MethodFlags.PageCountProc) > 0)
			{
				plugin.pageCountProc = new PageCountProc(PageCountProc);
				handles[i++] = GetHandle(plugin.pageCountProc);
			}
			if ((implementedMethods & MethodFlags.PageCapabilityProc) > 0)
			{
				plugin.pageCapabilityProc = new PageCapabilityProc(PageCapabilityProc);
				handles[i++] = GetHandle(plugin.pageCapabilityProc);
			}
			if ((implementedMethods & MethodFlags.LoadProc) > 0)
			{
				plugin.loadProc = new LoadProc(LoadProc);
				handles[i++] = GetHandle(plugin.loadProc);
			}
			if ((implementedMethods & MethodFlags.SaveProc) > 0)
			{
				plugin.saveProc = new SaveProc(SaveProc);
				handles[i++] = GetHandle(plugin.saveProc);
			}
			if ((implementedMethods & MethodFlags.ValidateProc) > 0)
			{
				plugin.validateProc = new ValidateProc(ValidateProc);
				handles[i++] = GetHandle(plugin.validateProc);
			}
			if ((implementedMethods & MethodFlags.MimeProc) > 0)
			{
				plugin.mimeProc = new MimeProc(MimeProc);
				handles[i++] = GetHandle(plugin.mimeProc);
			}
			if ((implementedMethods & MethodFlags.SupportsExportBPPProc) > 0)
			{
				plugin.supportsExportBPPProc = new SupportsExportBPPProc(SupportsExportBPPProc);
				handles[i++] = GetHandle(plugin.supportsExportBPPProc);
			}
			if ((implementedMethods & MethodFlags.SupportsExportTypeProc) > 0)
			{
				plugin.supportsExportTypeProc = new SupportsExportTypeProc(SupportsExportTypeProc);
				handles[i++] = GetHandle(plugin.supportsExportTypeProc);
			}
			if ((implementedMethods & MethodFlags.SupportsICCProfilesProc) > 0)
			{
				plugin.supportsICCProfilesProc = new SupportsICCProfilesProc(SupportsICCProfilesProc);
				handles[i++] = GetHandle(plugin.supportsICCProfilesProc);
			}

			// FormatProc is always implemented
			plugin.formatProc = new FormatProc(FormatProc);
			handles[i++] = GetHandle(plugin.formatProc);

			// InitProc is the register call back.
			initProc = new InitProc(RegisterProc);
			handles[i++] = GetHandle(initProc);

			// Register the plugin. The result will be saved and can be accessed later.
			registered = FreeImage.RegisterLocalPlugin(initProc, null, null, null, null) != FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			if (registered)
			{
				PluginRepository.RegisterLocalPlugin(this);
			}
		}

		~LocalPlugin()
		{
			for (int i = 0; i < handles.Length; i++)
			{
				if (handles[i].IsAllocated)
					handles[i].Free();
			}
		}

		private GCHandle GetHandle(Delegate d)
		{
			return GCHandle.Alloc(d, GCHandleType.Normal);
		}

		private void RegisterProc(ref Plugin plugin, int format_id)
		{
			// Copy the function pointers
			plugin = this.plugin;
			// Retrieve the format if assigned to this plugin by FreeImage.
			format = (FREE_IMAGE_FORMAT)format_id;
		}

		/// <summary>
		/// Gets or sets if the plugin is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				if (registered)
					return (FreeImage.IsPluginEnabled(format) > 0);
				else
					throw new ObjectDisposedException("plugin not registered");
			}
			set
			{
				if (registered)
					FreeImage.SetPluginEnabled(format, value);
				else
					throw new ObjectDisposedException("plugin not registered");
			}
		}

		/// <summary>
		/// Gets if the plugin was registered successfully.
		/// </summary>
		public bool Registered
		{
			get { return registered; }
		}

		/// <summary>
		/// Gets the FREE_IMAGE_FORMAT FreeImage assigned to this plugin.
		/// </summary>
		public FREE_IMAGE_FORMAT Format
		{
			get
			{
				return format;
			}
		}

		/// <summary>
		/// Reads from an unmanaged stream.
		/// </summary>
		protected unsafe int Read(FreeImageIO io, fi_handle handle, uint size, uint count, ref byte[] buffer)
		{
			fixed (byte* ptr = buffer)
			{
				return (int)io.readProc(new IntPtr(ptr), size, count, handle);
			}
		}

		/// <summary>
		/// Reads a single byte from an unmanaged stream.
		/// </summary>
		protected unsafe int ReadByte(FreeImageIO io, fi_handle handle)
		{
			byte buffer = 0;
			return (int)io.readProc(new IntPtr(&buffer), 1, 1, handle) > 0 ? buffer : -1;
		}

		/// <summary>
		/// Writes to an unmanaged stream.
		/// </summary>
		protected unsafe int Write(FreeImageIO io, fi_handle handle, uint size, uint count, ref byte[] buffer)
		{
			fixed (byte* ptr = buffer)
			{
				return (int)io.writeProc(new IntPtr(ptr), size, count, handle);
			}
		}

		/// <summary>
		/// Writes a single byte to an unmanaged stream.
		/// </summary>
		protected unsafe int WriteByte(FreeImageIO io, fi_handle handle, byte value)
		{
			return (int)io.writeProc(new IntPtr(&value), 1, 1, handle);
		}

		/// <summary>
		/// Seeks in an unmanaged stream.
		/// </summary>
		protected int Seek(FreeImageIO io, fi_handle handle, int offset, SeekOrigin origin)
		{
			return io.seekProc(handle, offset, origin);
		}

		/// <summary>
		/// Retrieves the position of an unmanaged stream.
		/// </summary>
		protected int Tell(FreeImageIO io, fi_handle handle)
		{
			return io.tellProc(handle);
		}
	}

	// FreeImage can read files from a disk or a network drive but also allows the user to
	// implement their own loading or saving functions to load them directly from an ftp or web
	// server for example.
	//
	// In .NET streams are a common way to handle data. The FreeImageStreamIO class handles
	// the loading and saving from and to streams. It implements the funtions FreeImage needs
	// to load data from an an arbitrary source.
	//
	// FreeImage requests a 'FreeImageIO' structure containing pointers (delegates) to these
	// functions. FreeImageStreamIO implements the function creates the structure and
	// prevents the garbage collector from moving these functions in memory.
	//
	// The class is for internal use only.

	/// <summary>
	/// Internal class wrapping stream io functions.
	/// </summary>
	internal static class FreeImageStreamIO
	{
		private static GCHandle readHandle;
		private static GCHandle writeHandle;
		private static GCHandle seekHandle;
		private static GCHandle tellHandle;

		/// <summary>
		/// FreeImageIO structure that can be used to read
		/// from streams via 'LoadFromHandle'.
		/// </summary>
		public static FreeImageIO io;

		/// <summary>
		/// Creates a new FreeImageStreamIO class which can be used to
		/// create a FreeImage compatible FreeImageIO structure.
		/// </summary>
		static FreeImageStreamIO()
		{
			io.readProc = new ReadProc(streamRead);
			io.writeProc = new WriteProc(streamWrite);
			io.seekProc = new SeekProc(streamSeek);
			io.tellProc = new TellProc(streamTell);
			readHandle = GCHandle.Alloc(io.readProc, GCHandleType.Normal);
			writeHandle = GCHandle.Alloc(io.writeProc, GCHandleType.Normal);
			seekHandle = GCHandle.Alloc(io.seekProc, GCHandleType.Normal);
			tellHandle = GCHandle.Alloc(io.tellProc, GCHandleType.Normal);
		}

		// Reads the requested data from the stream and writes it to the given address
		static unsafe uint streamRead(IntPtr buffer, uint size, uint count, fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if ((stream == null) || (!stream.CanRead))
				return 0;
			uint readCount = 0;
			byte* ptr = (byte*)buffer;
			byte[] bufferTemp = new byte[size];
			int read;
			while (readCount < count)
			{
				read = stream.Read(bufferTemp, 0, (int)size);
				if (read != (int)size)
				{
					stream.Seek(-read, SeekOrigin.Current);
					break;
				}
				for (int i = 0; i < read; i++, ptr++)
					*ptr = bufferTemp[i];
				readCount++;
			}
			return (uint)readCount;
		}

		// Reads the given data and writes it into the stream
		static unsafe uint streamWrite(IntPtr buffer, uint size, uint count, fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if ((stream == null) || (!stream.CanWrite))
				return 0;
			uint writeCount = 0;
			byte[] bufferTemp = new byte[size];
			byte* ptr = (byte*)buffer;
			while (writeCount < count)
			{
				for (int i = 0; i < size; i++, ptr++)
					bufferTemp[i] = *ptr;
				try
				{
					stream.Write(bufferTemp, 0, bufferTemp.Length);
				}
				catch
				{
					return writeCount;
				}
				writeCount++;
			}
			return writeCount;
		}

		// Moves the streams position
		static int streamSeek(fi_handle handle, int offset, SeekOrigin origin)
		{
			Stream stream = handle.GetObject() as Stream;
			if (stream == null)
				return 1;
			stream.Seek((long)offset, origin);
			return 0;
		}

		// Returns the streams current position
		static int streamTell(fi_handle handle)
		{
			Stream stream = handle.GetObject() as Stream;
			if (stream == null)
				return -1;
			return (int)stream.Position;
		}
	}

	// As mentioned above FreeImage can load bitmaps from arbitrary sources.
	// .NET works with different streams like File- or NetConnection-strams.
	// NetConnection streams, which are used to load files from web servers,
	// for example cannot seek.
	// But FreeImage frequently uses the seek operation when loading bitmaps.
	// StreamWrapper wrapps a stream and makes it seekable by caching all read
	// data into an internal MemoryStream to jump back- and forward.
	// StreamWapper is for internal use and only for loading from streams.

	internal class StreamWrapper : Stream
	{
		/// <summary>
		/// The stream to wrap
		/// </summary>
		private readonly Stream stream;
		/// <summary>
		/// The caching stream
		/// </summary>
		private MemoryStream memoryStream = new MemoryStream();
		/// <summary>
		/// Indicates if the wrapped stream reached its end
		/// </summary>
		private bool eos = false;
		/// <summary>
		/// Tells the wrapper to block readings or not
		/// </summary>
		private bool blocking = false;
		/// <summary>
		/// Indicates if the wrapped stream is disposed or not
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// Creates a new StreamWrapper
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		/// <param name="blocking">When true the wrapper always tries to read the requested
		/// amount of data from the wrapped stream.</param>
		public StreamWrapper(Stream stream, bool blocking)
		{
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			this.stream = stream;
			this.blocking = blocking;
		}

		~StreamWrapper()
		{
			Dispose(false);
		}

		// The wrapper only accepts readable streams
		public override bool CanRead
		{
			get { checkDisposed(); return true; }
		}

		// We implement that feature
		public override bool CanSeek
		{
			get { checkDisposed(); return true; }
		}

		// The wrapper is readonly
		public override bool CanWrite
		{
			get { checkDisposed(); return false; }
		}

		// Just forward it
		public override void Flush()
		{
			checkDisposed();
			stream.Flush();
		}

		// Calling this property will cause the wrapper to read the stream
		// to its end and cache it completely.
		public override long Length
		{
			get
			{
				checkDisposed();
				if (!eos)
				{
					Fill();
				}
				return memoryStream.Length;
			}
		}

		// Gets or sets the current position
		public override long Position
		{
			get
			{
				checkDisposed();
				return memoryStream.Position;
			}
			set
			{
				checkDisposed();
				Seek(value, SeekOrigin.Begin);
			}
		}

		// Implements the reading feature
		public override int Read(byte[] buffer, int offset, int count)
		{
			checkDisposed();
			// total bytes read from memory-stream
			int memoryBytes = 0;
			// total bytes read from the original stream
			int streamBytes = 0;
			memoryBytes = memoryStream.Read(buffer, offset, count);
			if ((count > memoryBytes) && (!eos))
			{
				// read the rest from the original stream (can be 0 bytes)
				do
				{
					int read = stream.Read(
						buffer,
						offset + memoryBytes + streamBytes,
						count - memoryBytes - streamBytes);
					streamBytes += read;
					if (read == 0)
					{
						eos = true;
						break;
					}
					if (!blocking)
					{
						break;
					}
				} while ((memoryBytes + streamBytes) < count);
				// copy the bytes from the original stream into the memory stream
				// if 0 bytes were read we write 0 so the memory-stream is not changed
				memoryStream.Write(buffer, offset + memoryBytes, streamBytes);
			}
			return memoryBytes + streamBytes;
		}

		// Implements the seeking feature
		public override long Seek(long offset, SeekOrigin origin)
		{
			checkDisposed();
			long newPosition = 0L;
			// get new position
			switch (origin)
			{
				case SeekOrigin.Begin:
					newPosition = offset;
					break;
				case SeekOrigin.Current:
					newPosition = memoryStream.Position + offset;
					break;
				case SeekOrigin.End:
					// to seek from the end have have to read to the end first
					if (!eos)
					{
						Fill();
					}
					newPosition = memoryStream.Length + offset;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			// in case the new position is beyond the memory-streams end
			// and the original streams end hasn't been reached
			// the original stream is read until either the stream ends or
			// enough bytes have been read
			if ((newPosition > memoryStream.Length) && (!eos))
			{
				memoryStream.Position = memoryStream.Length;
				int bytesToRead = (int)(newPosition - memoryStream.Length);
				byte[] buffer = new byte[1024];
				do
				{
					bytesToRead -= Read(buffer, 0, (bytesToRead >= buffer.Length) ? buffer.Length : bytesToRead);
				} while ((bytesToRead > 0) && (!eos));
			}
			memoryStream.Position = (newPosition <= memoryStream.Length) ? newPosition : memoryStream.Length;
			return 0;
		}

		// No write-support
		public override void SetLength(long value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		// No write-support
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Reset()
		{
			checkDisposed();
			Position = 0;
		}

		// Reads the wrapped stream until its end.
		private void Fill()
		{
			if (!eos)
			{
				memoryStream.Position = memoryStream.Length;
				int bytesRead = 0;
				byte[] buffer = new byte[1024];
				do
				{
					bytesRead = stream.Read(buffer, 0, buffer.Length);
					memoryStream.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);
				eos = true;
			}
		}

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private new void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					if (memoryStream != null) memoryStream.Dispose();
				}
			}
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		private void checkDisposed()
		{
			if (disposed) throw new ObjectDisposedException("StreamWrapper");
		}
	}

	/// <summary>
	/// Manages metadata objects and operations.
	/// </summary>
	public class MetadataTag : IComparable, IComparable<MetadataTag>, ICloneable, IEquatable<MetadataTag>, IDisposable
	{
		internal protected FITAG tag;
		internal protected FREE_IMAGE_MDMODEL model;
		protected bool disposed = false;
		protected bool selfCreated;

		protected MetadataTag()
		{
		}

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="model">The new model the tag should be of.</param>
		public MetadataTag(FREE_IMAGE_MDMODEL model)
		{
			this.model = model;
			tag = FreeImage.CreateTag();
			selfCreated = true;
		}

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="tag">The FITAG to wrap.</param>
		/// <param name="dib">The bitmap 'tag' was extracted from.</param>
		public MetadataTag(FITAG tag, FIBITMAP dib)
		{
			if (tag.IsNull)
			{
				throw new ArgumentNullException("tag");
			}
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			this.tag = tag;
			model = GetModel(dib, tag);
			selfCreated = false;
		}

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="tag">The FITAG to wrap.</param>
		/// <param name="model">The model of 'tag'.</param>
		public MetadataTag(FITAG tag, FREE_IMAGE_MDMODEL model)
		{
			if (tag.IsNull)
			{
				throw new ArgumentNullException("tag");
			}
			this.tag = tag;
			this.model = model;
			selfCreated = false;
		}

		~MetadataTag()
		{
			Dispose();
		}

		public static bool operator ==(MetadataTag value1, MetadataTag value2)
		{
			// Check whether both are null
			if (Object.ReferenceEquals(value1, null) && Object.ReferenceEquals(value2, null))
				return true;
			// Check whether only one is null
			if (Object.ReferenceEquals(value1, null) || Object.ReferenceEquals(value2, null))
				return false;
			// Check all properties
			if (value1.Count != value2.Count) return false;
			if (value1.Description != value2.Description) return false;
			if (value1.ID != value2.ID) return false;
			if (value1.Key != value2.Key) return false;
			if (value1.Length != value2.Length) return false;
			if (value1.Model != value2.Model) return false;
			if (value1.Type != value2.Type) return false;
			if (value1.Value.GetType() != value2.Value.GetType()) return false;
			// Value is 'Object' so IComparable is used to compare either
			// each value seperatly in case its an array or the single value
			// in case its no array
			if (value1.Value.GetType().IsArray)
			{
				Array array1 = (Array)value1.Value;
				Array array2 = (Array)value2.Value;
				if (array1.Length != array2.Length) return false;
				for (int i = 0; i < array1.Length; i++)
					if (((IComparable)array1.GetValue(i)).CompareTo(array2.GetValue(i)) != 0)
						return false;
			}
			else
			{
				if (((IComparable)value1.Value).CompareTo(value2.Value) != 0) return false;
			}
			// No difference found
			return true;
		}

		public static bool operator !=(MetadataTag value1, MetadataTag value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator FITAG(MetadataTag value)
		{
			return value.tag;
		}

		protected FREE_IMAGE_MDMODEL GetModel(FIBITMAP dib, FITAG tag)
		{
			FITAG value;
			foreach (FREE_IMAGE_MDMODEL model in FreeImage.FREE_IMAGE_MDMODELS)
			{
				FIMETADATA mData = FreeImage.FindFirstMetadata(model, dib, out value);
				if (mData.IsNull)
				{
					continue;
				}
				try
				{
					do
					{
						if (value == tag)
							return model;
					}
					while (FreeImage.FindNextMetadata(mData, out value));
				}
				finally
				{
					if (!mData.IsNull) FreeImage.FindCloseMetadata(mData);
				}
			}
			throw new ArgumentException("'tag' is no metadata object of 'dib'");
		}

		/// <summary>
		/// Gets the model of the metadata.
		/// </summary>
		public FREE_IMAGE_MDMODEL Model
		{
			get { CheckDisposed(); return model; }
		}

		/// <summary>
		/// Gets or sets the key of the metadata.
		/// </summary>
		public string Key
		{
			get { CheckDisposed(); return FreeImage.GetTagKey(tag); }
			set { CheckDisposed(); FreeImage.SetTagKey(tag, value); }
		}

		/// <summary>
		/// Gets or sets the description of the metadata.
		/// </summary>
		public string Description
		{
			get { CheckDisposed(); return FreeImage.GetTagDescription(tag); }
			set { CheckDisposed(); FreeImage.SetTagDescription(tag, value); }
		}

		/// <summary>
		/// Gets or sets the ID of the metadata.
		/// </summary>
		public ushort ID
		{
			get { CheckDisposed(); return FreeImage.GetTagID(tag); }
			set { CheckDisposed(); FreeImage.SetTagID(tag, value); }
		}

		/// <summary>
		/// Gets the type of the metadata.
		/// </summary>
		public FREE_IMAGE_MDTYPE Type
		{
			get { CheckDisposed(); return FreeImage.GetTagType(tag); }
			protected set { FreeImage.SetTagType(tag, value); }
		}

		/// <summary>
		/// Gets the number of elements the metadata object contains.
		/// </summary>
		public uint Count
		{
			get { CheckDisposed(); return Type == FREE_IMAGE_MDTYPE.FIDT_ASCII ? FreeImage.GetTagCount(tag) - 1 : FreeImage.GetTagCount(tag); }
			protected set { FreeImage.SetTagCount(tag, value); }
		}

		/// <summary>
		/// Gets the length of the value in bytes.
		/// </summary>
		public uint Length
		{
			get { CheckDisposed(); return Type == FREE_IMAGE_MDTYPE.FIDT_ASCII ? FreeImage.GetTagLength(tag) - 1 : FreeImage.GetTagLength(tag); }
			protected set { FreeImage.SetTagLength(tag, value); }
		}

		private unsafe byte[] GetData()
		{
			uint length = Length;
			byte[] value = new byte[length];
			byte* ptr = (byte*)FreeImage.GetTagValue(tag);
			for (int i = 0; i < length; i++)
				value[i] = ptr[i];
			return value;
		}

		/// <summary>
		/// Gets or sets the value of the metadata.
		/// <para> In case value is of byte or byte[] FREE_IMAGE_MDTYPE.FIDT_UNDEFINED is assumed.</para>
		/// <para> In case value is of uint or uint[] FREE_IMAGE_MDTYPE.FIDT_LONG is assumed.</para>
		/// </summary>
		public unsafe object Value
		{
			get
			{
				CheckDisposed();
				byte[] value;

				if (Type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
				{
					value = GetData();
					StringBuilder sb = new StringBuilder(value.Length);
					for (int i = 0; i < value.Length; i++)
						sb.Append((char)value[i]);
					return sb.ToString();
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_SRATIONAL)
				{
					FIRational[] rationResult = new FIRational[Count];
					int* ptr = (int*)FreeImage.GetTagValue(tag);
					for (int i = 0; i < rationResult.Length; i++)
						rationResult[i] = new FIRational(ptr[i * 2], ptr[(i * 2) + 1]);
					return rationResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_RATIONAL)
				{
					FIURational[] urationResult = new FIURational[Count];
					uint* ptr = (uint*)FreeImage.GetTagValue(tag);
					for (int i = 0; i < urationResult.Length; i++)
						urationResult[i] = new FIURational(ptr[i * 2], ptr[(i * 2) + 1]);
					return urationResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_DOUBLE)
				{
					value = GetData();
					double[] doubleResult = new double[Count];
					for (int i = 0; i < doubleResult.Length; i++)
						doubleResult[i] = BitConverter.ToDouble(value, i * sizeof(double));
					return doubleResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_UNDEFINED || Type == FREE_IMAGE_MDTYPE.FIDT_BYTE)
				{
					return GetData();
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_FLOAT)
				{
					value = GetData();
					float[] floatResult = new float[Count];
					for (int i = 0; i < floatResult.Length; i++)
						floatResult[i] = BitConverter.ToSingle(value, i * sizeof(float));
					return floatResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_IFD || Type == FREE_IMAGE_MDTYPE.FIDT_LONG)
				{
					value = GetData();
					uint[] uintegerResult = new uint[Count];
					for (int i = 0; i < uintegerResult.Length; i++)
						uintegerResult[i] = BitConverter.ToUInt32(value, i * sizeof(uint));
					return uintegerResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_SHORT)
				{
					value = GetData();
					ushort[] ushortResult = new ushort[Count];
					for (int i = 0; i < ushortResult.Length; i++)
						ushortResult[i] = BitConverter.ToUInt16(value, i * sizeof(short));
					return ushortResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_SLONG)
				{
					value = GetData();
					int[] intResult = new int[Count];
					for (int i = 0; i < intResult.Length; i++)
						intResult[i] = BitConverter.ToInt32(value, i * sizeof(int));
					return intResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_SSHORT)
				{
					value = GetData();
					short[] shortResult = new short[Count];
					for (int i = 0; i < shortResult.Length; i++)
						shortResult[i] = BitConverter.ToInt16(value, i * sizeof(short));
					return shortResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_SBYTE)
				{
					sbyte[] sbyteResult = new sbyte[Length];
					sbyte* ptr = (sbyte*)FreeImage.GetTagValue(tag);
					for (int i = 0; i < sbyteResult.Length; i++)
						sbyteResult[i] = ptr[i];
					return sbyteResult;
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_PALETTE)
				{
					RGBQUAD[] rgbqResult = new RGBQUAD[Count];
					RGBQUAD* ptr = (RGBQUAD*)FreeImage.GetTagValue(tag);
					for (int i = 0; i < rgbqResult.Length; i++)
						rgbqResult[i] = ptr[i];
					return rgbqResult;
				}
				else
				{
					return null;
				}
			}
			set
			{
				SetValue(value);
			}
		}

		/// <summary>
		/// Sets the value of the metadata.
		/// <para> In case value is of byte or byte[] FREE_IMAGE_MDTYPE.FIDT_UNDEFINED is assumed.</para>
		/// <para> In case value is of uint or uint[] FREE_IMAGE_MDTYPE.FIDT_LONG is assumed.</para>
		/// </summary>
		/// <param name="value">New data of the metadata.</param>
		/// <returns>True on success, false on failure.</returns>
		/// <exception cref="NotSupportedException">
		/// Thrown in case the data format is not supported.</exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'value' is null.</exception>
		public bool SetValue(object value)
		{
			Type type = value.GetType();

			if (type == typeof(string))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_ASCII);
			}
			else if (type == typeof(byte) || type == typeof(byte[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
			}
			else if (type == typeof(double) || type == typeof(double[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
			}
			else if (type == typeof(float) || type == typeof(float[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_FLOAT);
			}
			else if (type == typeof(uint) || type == typeof(uint[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_LONG);
			}
			else if (type == typeof(RGBQUAD) || type == typeof(RGBQUAD[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_PALETTE);
			}
			else if (type == typeof(FIURational) || type == typeof(FIURational[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
			}
			else if (type == typeof(sbyte) || type == typeof(sbyte[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_SBYTE);
			}
			else if (type == typeof(ushort) || type == typeof(ushort[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_SHORT);
			}
			else if (type == typeof(int) || type == typeof(int[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_SLONG);
			}
			else if (type == typeof(FIRational) || type == typeof(FIRational[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
			}
			else if (type == typeof(short) || type == typeof(short[]))
			{
				return SetValue(value, FREE_IMAGE_MDTYPE.FIDT_SSHORT);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Sets the value of the metadata.
		/// </summary>
		/// <param name="value">New data of the metadata.</param>
		/// <param name="type">Type of the data.</param>
		/// <returns>True on success, false on failure.</returns>
		/// <exception cref="NotSupportedException">
		/// Thrown in case the data type is not supported.</exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'value' is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown in case 'value' and 'type' to not fit.</exception>
		public bool SetValue(object value, FREE_IMAGE_MDTYPE type)
		{
			CheckDisposed();
			if ((!value.GetType().IsArray) && (!(value is string)))
			{
				Array array = Array.CreateInstance(value.GetType(), 1);
				array.SetValue(value, 0);
				return SetArrayValue(array, type);
			}
			return SetArrayValue(value, type);
		}

		protected unsafe bool SetArrayValue(object value, FREE_IMAGE_MDTYPE type)
		{
			if (value == null) throw new ArgumentNullException("value");
			byte[] data = null;

			if (type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
			{
				string tempValue = value as string;
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length + 1);
				Length = (uint)((tempValue.Length * sizeof(byte)) + 1);
				data = new byte[Length + 1];

				for (int i = 0; i < tempValue.Length; i++)
					data[i] = (byte)tempValue[i];
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_SRATIONAL)
			{
				FIRational[] tempValue = value as FIRational[];
				if (tempValue == null) throw new ArgumentException("value");

				int size = sizeof(FIRational);
				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * size);
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp1 = BitConverter.GetBytes(tempValue[i].Numerator);
					byte[] temp2 = BitConverter.GetBytes(tempValue[i].Denominator);
					temp1.CopyTo(data, i * size);
					temp2.CopyTo(data, i * size + (size / 2));
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_RATIONAL)
			{
				FIURational[] tempValue = value as FIURational[];
				if (tempValue == null) throw new ArgumentException("value");

				int size = sizeof(FIURational);
				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * size);
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp1 = BitConverter.GetBytes(tempValue[i].Numerator);
					byte[] temp2 = BitConverter.GetBytes(tempValue[i].Denominator);
					temp1.CopyTo(data, i * size);
					temp2.CopyTo(data, i * size + (size / 2));
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_UNDEFINED || type == FREE_IMAGE_MDTYPE.FIDT_BYTE)
			{
				byte[] tempValue = value as byte[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(byte));
				data = tempValue;
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_DOUBLE)
			{
				double[] tempValue = value as double[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(double));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(double)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_FLOAT)
			{
				float[] tempValue = value as float[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(float));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(float)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_IFD || type == FREE_IMAGE_MDTYPE.FIDT_LONG)
			{
				uint[] tempValue = value as uint[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(uint));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(uint)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_SBYTE)
			{
				sbyte[] tempValue = value as sbyte[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(sbyte));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
					data[i] = (byte)tempValue[i];
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_SHORT)
			{
				ushort[] tempValue = value as ushort[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(ushort));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(ushort)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_SLONG)
			{
				int[] tempValue = value as int[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(int));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(int)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_SSHORT)
			{
				short[] tempValue = value as short[];
				if (tempValue == null) throw new ArgumentException("value");

				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * sizeof(short));
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					byte[] temp = BitConverter.GetBytes(tempValue[i]);
					for (int j = 0; j < temp.Length; j++)
						data[(i * sizeof(short)) + j] = temp[j];
				}
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_PALETTE)
			{
				RGBQUAD[] tempValue = value as RGBQUAD[];
				if (tempValue == null) throw new ArgumentException("value");

				int size = sizeof(RGBQUAD);
				Type = type;
				Count = (uint)(tempValue.Length);
				Length = (uint)(tempValue.Length * size);
				data = new byte[Length];

				for (int i = 0; i < tempValue.Length; i++)
				{
					data[i * size + 0] = tempValue[i].rgbBlue;
					data[i * size + 1] = tempValue[i].rgbGreen;
					data[i * size + 2] = tempValue[i].rgbRed;
					data[i * size + 3] = tempValue[i].rgbReserved;
				}
			}
			else
			{
				throw new NotSupportedException();
			}

			return FreeImage.SetTagValue(tag, data);
		}

		/// <summary>
		/// Add this metadata to an image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public bool AddToImage(FIBITMAP dib)
		{
			CheckDisposed();
			if (dib.IsNull) throw new ArgumentNullException("dib");
			if (Key == null) throw new ArgumentNullException("Key");
			if (!selfCreated)
			{
				tag = FreeImage.CloneTag(tag);
				if (tag.IsNull) throw new Exception();
				selfCreated = true;
			}
			if (!FreeImage.SetMetadata(Model, dib, Key, tag))
				return false;
			FREE_IMAGE_MDMODEL _model = Model;
			string _key = Key;
			selfCreated = false;
			FreeImage.DeleteTag(tag);
			return FreeImage.GetMetadata(_model, dib, _key, out tag);
		}

		/// <summary>
		/// Gets a .NET PropertyItem for this metadata tag.
		/// </summary>
		/// <returns>The .NET PropertyItem.</returns>
		public unsafe System.Drawing.Imaging.PropertyItem GetPropertyItem()
		{
			System.Drawing.Imaging.PropertyItem item = FreeImage.CreatePropertyItem();
			item.Id = ID;
			item.Len = (int)Length;
			item.Type = (short)Type;
			byte[] data = new byte[item.Len];
			byte* ptr = (byte*)FreeImage.GetTagValue(tag);
			for (int i = 0; i < data.Length; i++)
				data[i] = ptr[i];
			item.Value = data;
			return item;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			CheckDisposed();
			string fiString = FreeImage.TagToString(model, tag, 0);

			if (String.IsNullOrEmpty(fiString))
			{
				return tag.ToString();
			}
			else
			{
				return fiString;
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			CheckDisposed();
			MetadataTag clone = new MetadataTag();
			clone.model = model;
			clone.tag = FreeImage.CloneTag(tag);
			clone.selfCreated = true;
			return clone;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(MetadataTag other)
		{
			CheckDisposed();
			return this.tag == other.tag && this.model == other.model;
		}

		/// <summary>
		/// Determines whether the specified Object is equal to the current Object.
		/// </summary>
		/// <param name="obj">The Object to compare with the current Object.</param>
		/// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
		public int CompareTo(object obj)
		{
			CheckDisposed();
			if (obj is MetadataTag)
			{
				return CompareTo((MetadataTag)obj);
			}
			throw new ArgumentException("obj");
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(MetadataTag other)
		{
			CheckDisposed();
			return this.tag.CompareTo(other.tag);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				if (selfCreated)
				{
					FreeImage.DeleteTag(tag);
				}
			}
		}

		/// <summary>
		/// Gets whether this instance has already been disposed.
		/// </summary>
		public bool Disposed
		{
			get { return disposed; }
		}

		protected void CheckDisposed()
		{
			if (disposed) throw new ObjectDisposedException("The object has already been disposed.");
		}
	}

	/// <summary>
	/// Base class for managing different metadata models.
	/// </summary>
	public abstract class MetadataModel : IEnumerable
	{
		protected readonly FIBITMAP dib;

		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'dib' is null.</exception>
		protected MetadataModel(FIBITMAP dib)
		{
			if (dib.IsNull) throw new ArgumentNullException("dib");
			this.dib = dib;
		}

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public abstract FREE_IMAGE_MDMODEL Model
		{
			get;
		}

		/// <summary>
		/// Adds new tag to the bitmap
		/// or updates its value in case it already exists.
		/// 'tag.Key' will be used as key.
		/// </summary>
		/// <param name="tag">The tag to add or update.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'tag' is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown in case the tags model differs from this instances model.</exception>
		public bool AddTag(MetadataTag tag)
		{
			if (tag == null) throw new ArgumentNullException("tag");
			if (tag.Model != Model) throw new ArgumentException("tag.Model");
			return tag.AddToImage(dib);
		}

		/// <summary>
		/// Adds a list of tags to the bitmap
		/// or updates their values in case they already exist.
		/// 'tag.Key' will be used as key.
		/// </summary>
		/// <param name="list">A list of tags to add or update.</param>
		/// <returns>Returns the number of successfully added tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'list' is null.</exception>
		public int AddTag(IEnumerable<MetadataTag> list)
		{
			if (list == null) throw new ArgumentNullException("list");
			int count = 0;
			foreach (MetadataTag tag in list)
			{
				if (tag.Model == Model && tag.AddToImage(dib))
					count++;
			}
			return count;
		}

		/// <summary>
		/// Removes the specified tag from the bitmap.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'key' is null.</exception>
		public bool RemoveTag(string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			return FreeImage.SetMetadata(Model, dib, key, 0);
		}

		/// <summary>
		/// Destroys the metadata model
		/// which will remove all tags of this model from the bitmap.
		/// </summary>
		/// <returns>Returns true on success, false on failure.</returns>
		public bool DestoryModel()
		{
			return FreeImage.SetMetadata(Model, dib, null, 0);
		}

		/// <summary>
		/// Returns the specified metadata tag.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>The metadata tag.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'key' is null.</exception>
		public MetadataTag GetTag(string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			MetadataTag tag;
			return FreeImage.GetMetadata(Model, dib, key, out tag) ? tag : null;
		}

		/// <summary>
		/// Returns whether the specified tag exists.
		/// </summary>
		/// <param name="key">The key of the tag.</param>
		/// <returns>True in case the tag exists, else false.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'key' is null.</exception>
		public bool TagExists(string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			MetadataTag tag;
			return FreeImage.GetMetadata(Model, dib, key, out tag);
		}

		/// <summary>
		/// Returns a list of all metadata tags this instance represents.
		/// </summary>
		public List<MetadataTag> List
		{
			get
			{
				List<MetadataTag> list = new List<MetadataTag>((int)FreeImage.GetMetadataCount(Model, dib));
				MetadataTag tag;
				FIMETADATA mdHandle = FreeImage.FindFirstMetadata(Model, dib, out tag);
				if (!mdHandle.IsNull)
				{
					do
					{
						list.Add(tag);
					}
					while (FreeImage.FindNextMetadata(mdHandle, out tag));
					FreeImage.FindCloseMetadata(mdHandle);
				}
				return list;
			}
		}

		protected MetadataTag GetTagFromIndex(int index)
		{
			if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index");
			MetadataTag tag;
			int count = 0;
			FIMETADATA mdHandle = FreeImage.FindFirstMetadata(Model, dib, out tag);
			if (!mdHandle.IsNull)
			{
				do
				{
					if (count++ == index)
						break;
				}
				while (FreeImage.FindNextMetadata(mdHandle, out tag));
				FreeImage.FindCloseMetadata(mdHandle);
			}
			return tag;
		}

		/// <summary>
		/// Returns the metadata tag at the given index.
		/// This operation is slow when accessing all tags.
		/// </summary>
		/// <param name="index">Index of the tag.</param>
		/// <returns>The metadata tag.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown in case index is greater or equal 'Count'
		/// or index is less than zero.</exception>
		public MetadataTag this[int index]
		{
			get
			{
				return GetTagFromIndex(index);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return List.GetEnumerator();
		}

		/// <summary>
		/// Returns the number of metadata tags this instance represents.
		/// </summary>
		public int Count
		{
			get { return (int)FreeImage.GetMetadataCount(Model, dib); }
		}

		/// <summary>
		/// Returns whether this model exists in the bitmaps metadata structure.
		/// </summary>
		public bool Exists
		{
			get
			{
				return Count > 0;
			}
		}

		/// <summary>
		/// Searches for a pattern in each metadata tag and returns the result as a list.
		/// </summary>
		/// <param name="searchPattern">The regular expression to use for the search.</param>
		/// <param name="flags">A bitfield that controls which fields should be searched in.</param>
		/// <returns>A list containing all found metadata tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'searchPattern' is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown in case 'searchPattern' is empty.</exception>
		public List<MetadataTag> RegexSearch(string searchPattern, MD_SEARCH_FLAGS flags)
		{
			if (searchPattern == null) throw new ArgumentNullException("searchString");
			if (searchPattern.Length == 0) throw new ArgumentException("searchString is empty");
			List<MetadataTag> result = new List<MetadataTag>(Count);
			Regex regex = new Regex(searchPattern);
			List<MetadataTag> list = List;
			foreach (MetadataTag tag in list)
			{
				if (((flags & MD_SEARCH_FLAGS.KEY) > 0) && regex.Match(tag.Key).Success)
				{
					result.Add(tag);
					continue;
				}
				if (((flags & MD_SEARCH_FLAGS.DESCRIPTION) > 0) && regex.Match(tag.Description).Success)
				{
					result.Add(tag);
					continue;
				}
				if (((flags & MD_SEARCH_FLAGS.TOSTRING) > 0) && regex.Match(tag.ToString()).Success)
				{
					result.Add(tag);
					continue;
				}
			}
			result.Capacity = result.Count;
			return result;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return Model.ToString();
		}
	}

	#region Metadata Models

	/// <summary>
	/// Class that manages the metadata model type FIMD_ANIMATION.
	/// </summary>
	public class MDM_ANIMATION : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_ANIMATION(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_ANIMATION; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_COMMENTS.
	/// </summary>
	public class MDM_COMMENTS : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_COMMENTS(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_COMMENTS; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_CUSTOM.
	/// </summary>
	public class MDM_CUSTOM : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_CUSTOM(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_CUSTOM; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_EXIF_EXIF.
	/// </summary>
	public class MDM_EXIF_EXIF : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_EXIF_EXIF(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_EXIF_GPS.
	/// </summary>
	public class MDM_EXIF_GPS : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_EXIF_GPS(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_GPS; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_EXIF_INTEROP.
	/// </summary>
	public class MDM_INTEROP : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_INTEROP(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_INTEROP; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_EXIF_MAIN.
	/// </summary>
	public class MDM_MAIN : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_MAIN(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_EXIF_MAKERNOTE.
	/// </summary>
	public class MDM_MAKERNOTE : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_MAKERNOTE(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_EXIF_MAKERNOTE; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_GEOTIFF.
	/// </summary>
	public class MDM_GEOTIFF : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_GEOTIFF(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_GEOTIFF; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_IPTC.
	/// </summary>
	public class MDM_IPTC : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_IPTC(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_IPTC; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_NODATA.
	/// </summary>
	public class MDM_NODATA : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_NODATA(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_NODATA; }
		}
	}

	/// <summary>
	/// Class that manages the metadata model type FIMD_XMP.
	/// </summary>
	public class MDM_XMP : MetadataModel
	{
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public MDM_XMP(FIBITMAP dib) : base(dib) { }

		/// <summary>
		/// Retrieves the datamodel that this instance represents.
		/// </summary>
		public override FREE_IMAGE_MDMODEL Model
		{
			get { return FREE_IMAGE_MDMODEL.FIMD_XMP; }
		}
	}

	#endregion

	/// <summary>
	/// Class handling metadata of a FreeImage bitmap.
	/// </summary>
	public class ImageMetadata : IEnumerable, IComparable, IComparable<ImageMetadata>
	{
		private readonly List<MetadataModel> data;
		private readonly FIBITMAP dib;
		private bool hideEmptyModels;

		/// <summary>
		/// Creates a new ImageMetadata instance, showing all known models.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public ImageMetadata(FIBITMAP dib) : this(dib, false) { }

		/// <summary>
		/// Creates a new ImageMetadata instance.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hideEmptyModels">When true, empty metadata models
		/// will be hidden until a tag to this model is added.</param>
		public ImageMetadata(FIBITMAP dib, bool hideEmptyModels)
		{
			if (dib.IsNull) throw new ArgumentNullException("dib");
			data = new List<MetadataModel>(FreeImage.FREE_IMAGE_MDMODELS.Length);
			this.dib = dib;
			this.hideEmptyModels = hideEmptyModels;

			foreach (Type exportedType in Assembly.GetAssembly(this.GetType()).GetExportedTypes())
			{
				if (exportedType.IsClass &&
					exportedType.IsPublic &&
					exportedType.BaseType != null &&
					exportedType.BaseType == typeof(MetadataModel))
				{
					ConstructorInfo constructorInfo = exportedType.GetConstructor(new Type[] { typeof(FIBITMAP) });
					if (constructorInfo != null)
					{
						MetadataModel model = (MetadataModel)constructorInfo.Invoke(new object[] { dib });
						if (model != null)
						{
							data.Add(model);
						}
					}
				}
			}
			data.Capacity = data.Count;
		}

		/// <summary>
		/// Gets or sets the MetadataModel of the specified type.
		/// <para>In case the getter returns null the model is not contained
		/// by the list.</para>
		/// <para>'null' can be used calling the setter to destroy the model.</para>
		/// </summary>
		/// <param name="model">Type of the model.</param>
		/// <returns>The MetadataModel object of the specified type.</returns>
		public MetadataModel this[FREE_IMAGE_MDMODEL model]
		{
			get
			{
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i].Model == model)
					{
						if (!data[i].Exists && hideEmptyModels)
							return null;
						return data[i];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the MetadataModel at the specified index.
		/// <para>In case the getter returns null the model is not contained
		/// by the list.</para>
		/// <para>'null' can be used calling the setter to destroy the model.</para>
		/// </summary>
		/// <param name="index">Index of the MetadataModel within this instance.</param>
		/// <returns>The MetadataModel object at the specified index.</returns>
		public MetadataModel this[int index]
		{
			get
			{
				if (index < 0 || index >= data.Count) throw new ArgumentOutOfRangeException("index");
				return (hideEmptyModels && !data[index].Exists) ? null : data[index];
			}
		}

		/// <summary>
		/// Returns a list of all visible metadata models.
		/// </summary>
		public List<MetadataModel> List
		{
			get
			{
				if (hideEmptyModels)
				{
					List<MetadataModel> result = new List<MetadataModel>();
					for (int i = 0; i < data.Count; i++)
						if (data[i].Exists)
							result.Add(data[i]);
					return result;
				}
				else
				{
					return data;
				}
			}
		}

		/// <summary>
		/// Adds new tag to the bitmap
		/// or updates its value in case it already exists.
		/// 'tag.Key' will be used as key.
		/// </summary>
		/// <param name="tag">The tag to add or update.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case 'tag' is null.</exception>
		public bool AddTag(MetadataTag tag)
		{
			for (int i = 0; i < data.Count; i++)
			{
				if (tag.Model == data[i].Model)
				{
					return data[i].AddTag(tag);
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the number of visible metadata models.
		/// </summary>
		public int Count
		{
			get
			{
				if (hideEmptyModels)
				{
					int count = 0;
					for (int i = 0; i < data.Count; i++)
						if (data[i].Exists)
							count++;
					return count;
				}
				else
				{
					return data.Count;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether empty metadata models are hidden.
		/// </summary>
		public bool HideEmptyModels
		{
			get
			{
				return hideEmptyModels;
			}
			set
			{
				hideEmptyModels = value;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			if (hideEmptyModels)
			{
				List<MetadataModel> tempList = new List<MetadataModel>(data.Count);
				for (int i = 0; i < data.Count; i++)
					if (data[i].Exists)
						tempList.Add(data[i]);
				return tempList.GetEnumerator();
			}
			else
			{
				return data.GetEnumerator();
			}
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(object obj)
		{
			if (obj is ImageMetadata)
			{
				return CompareTo((ImageMetadata)obj);
			}
			throw new ArgumentException();
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(ImageMetadata other)
		{
			return this.dib.CompareTo(other.dib);
		}
	}

	/// <summary>
	/// Class wrapping a FreeImage format.
	/// </summary>
	public sealed class FreeImagePlugin
	{
		private readonly FREE_IMAGE_FORMAT fif;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="fif">The FreeImage format to wrap.</param>
		internal FreeImagePlugin(FREE_IMAGE_FORMAT fif)
		{
			this.fif = fif;
		}

		/// <summary>
		/// Gets the format of this instance.
		/// </summary>
		public FREE_IMAGE_FORMAT FIFormat
		{
			get
			{
				return fif;
			}
		}

		/// <summary>
		/// Gets or sets whether this plugin is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return (FreeImage.IsPluginEnabled(fif) == 1);
			}
			set
			{
				FreeImage.SetPluginEnabled(fif, value);
			}
		}

		/// <summary>
		/// Gets a string describing the format.
		/// </summary>
		public string Format
		{
			get
			{
				return FreeImage.GetFormatFromFIF(fif);
			}
		}

		/// <summary>
		/// Gets a comma-delimited file extension list describing the bitmap formats
		/// this plugin can read and/or write.
		/// </summary>
		public string ExtentsionList
		{
			get
			{
				return FreeImage.GetFIFExtensionList(fif);
			}
		}

		/// <summary>
		/// Gets a descriptive string that describes the bitmap formats
		/// this plugin can read and/or write.
		/// </summary>
		public string Description
		{
			get
			{
				return FreeImage.GetFIFDescription(fif);
			}
		}

		/// <summary>
		/// Returns a regular expression string that can be used by
		/// a regular expression engine to identify the bitmap.
		/// FreeImageQt makes use of this function.
		/// </summary>
		public string RegExpr
		{
			get
			{
				return FreeImage.GetFIFRegExpr(fif);
			}
		}

		/// <summary>
		/// Gets whether this plugin can load bitmaps.
		/// </summary>
		public bool SupportsReading
		{
			get
			{
				return FreeImage.FIFSupportsReading(fif);
			}
		}

		/// <summary>
		/// Gets whether this plugin can save bitmaps.
		/// </summary>
		public bool SupportsWriting
		{
			get
			{
				return FreeImage.FIFSupportsWriting(fif);
			}
		}

		/// <summary>
		/// Checks whether this plugin can save a bitmap in the desired data type.
		/// </summary>
		/// <param name="type">The desired image type.</param>
		/// <returns>True if this plugin can save bitmaps as the desired type, else false.</returns>
		public bool SupportsExportType(FREE_IMAGE_TYPE type)
		{
			return FreeImage.FIFSupportsExportType(fif, type);
		}

		/// <summary>
		/// Checks whether this plugin can save bitmaps in the desired bit depth.
		/// </summary>
		/// <param name="bpp">The desired bit depth.</param>
		/// <returns>True if this plugin can save bitmaps in the desired bit depth, else false.</returns>
		public bool SupportsExportBPP(int bpp)
		{
			return FreeImage.FIFSupportsExportBPP(fif, bpp);
		}

		/// <summary>
		/// Gets whether this plugin can load or save an ICC profile.
		/// </summary>
		public bool SupportsICCProfiles
		{
			get
			{
				return FreeImage.FIFSupportsICCProfiles(fif);
			}
		}

		/// <summary>
		/// Checks whether an extension is valid for this format.
		/// </summary>
		/// <param name="extension">The desired extension.</param>
		/// <returns>True if the extension is valid for this format, false otherwise.</returns>
		public bool ValidExtension(string extension)
		{
			return FreeImage.IsExtensionValidForFIF(fif, extension);
		}

		/// <summary>
		/// Checks whether an extension is valid for this format.
		/// </summary>
		/// <param name="extension">The desired extension.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the extension is valid for this format, false otherwise.</returns>
		public bool ValidExtension(string extension, StringComparison comparisonType)
		{
			return FreeImage.IsExtensionValidForFIF(fif, extension, comparisonType);
		}

		/// <summary>
		/// Checks whether a filename is valid for this format.
		/// </summary>
		/// <param name="filename">The desired filename.</param>
		/// <returns>True if the filename is valid for this format, false otherwise.</returns>
		public bool ValidFilename(string filename)
		{
			return FreeImage.IsFilenameValidForFIF(fif, filename);
		}

		/// <summary>
		/// Checks whether a filename is valid for this format.
		/// </summary>
		/// <param name="filename">The desired filename.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the filename is valid for this format, false otherwise.</returns>
		public bool ValidFilename(string filename, StringComparison comparisonType)
		{
			return FreeImage.IsFilenameValidForFIF(fif, filename, comparisonType);
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return Description;
		}
	}

	/// <summary>
	/// Class wrapping all registered plugins in FreeImage.
	/// </summary>
	public static class PluginRepository
	{
		private static readonly List<FreeImagePlugin> plugins = null;
		private static readonly List<FreeImagePlugin> localPlugins = null;

		static PluginRepository()
		{
			plugins = new List<FreeImagePlugin>(FreeImage.GetFIFCount());
			localPlugins = new List<FreeImagePlugin>(0);
			for (int i = 0; i < plugins.Capacity; i++)
			{
				plugins.Add(new FreeImagePlugin((FREE_IMAGE_FORMAT)i));
			}
		}

		/// <summary>
		/// Adds local plugin to this class.
		/// </summary>
		/// <param name="localPlugin">The registered plugin.</param>
		internal static void RegisterLocalPlugin(LocalPlugin localPlugin)
		{
			FreeImagePlugin plugin = new FreeImagePlugin(localPlugin.Format);
			plugins.Add(plugin);
			localPlugins.Add(plugin);
		}

		/// <summary>
		/// Returns an instance of 'Plugin', wrapping the given format.
		/// </summary>
		/// <param name="fif">The format to wrap.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin Plugin(FREE_IMAGE_FORMAT fif)
		{
			return Plugin((int)fif);
		}

		/// <summary>
		/// Returns an instance of 'Plugin', wrapping the format at the given index.
		/// </summary>
		/// <param name="index">The index of the format to wrap.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin Plugin(int index)
		{
			return (index >= 0) ? plugins[index] : null;
		}

		/// <summary>
		/// Returns an instance of 'Plugin'.
		/// <typeparamref name="expression"/> is searched in:
		/// <c>Format</c>, <c>RegExpr</c>,
		/// <c>ValidExtension</c> and <c>ValidFilename</c>.
		/// </summary>
		/// <param name="expression">The expression to search for.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin Plugin(string expression)
		{
			FreeImagePlugin result = null;
			expression = expression.ToLower();

			foreach (FreeImagePlugin plugin in plugins)
			{
				if (plugin.Format.ToLower().Contains(expression) ||
					plugin.RegExpr.ToLower().Contains(expression) ||
					plugin.ValidExtension(expression, StringComparison.CurrentCultureIgnoreCase) ||
					plugin.ValidFilename(expression, StringComparison.CurrentCultureIgnoreCase))
				{
					result = plugin;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns an instance of 'Plugin' for the given format.
		/// </summary>
		/// <param name="format">The format of the Plugin.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin PluginFromFormat(string format)
		{
			return Plugin(FreeImage.GetFIFFromFormat(format));
		}

		/// <summary>
		/// Returns an instance of 'Plugin' for the given filename.
		/// </summary>
		/// <param name="filename">The valid filename for the plugin.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin PluginFromFilename(string filename)
		{
			return Plugin(FreeImage.GetFIFFromFilename(filename));
		}

		/// <summary>
		/// Returns an instance of 'Plugin' for the given mime.
		/// </summary>
		/// <param name="mime">The valid mime for the plugin.</param>
		/// <returns>An instance of 'Plugin'.</returns>
		public static FreeImagePlugin PluginFromMime(string mime)
		{
			return Plugin(FreeImage.GetFIFFromMime(mime));
		}

		/// <summary>
		/// Gets the number of registered plugins.
		/// </summary>
		public static int FIFCount
		{
			get
			{
				return FreeImage.GetFIFCount();
			}
		}

		/// <summary>
		/// Gets a readonly collection of all plugins.
		/// </summary>
		public static ReadOnlyCollection<FreeImagePlugin> PluginList
		{
			get
			{
				return plugins.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets a list of plugins that are only able to
		/// read but not to write.
		/// </summary>
		public static List<FreeImagePlugin> ReadOnlyPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsReading && !p.SupportsWriting) 
					{
						list.Add(p); 
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are only able to
		/// write but not to read.
		/// </summary>
		public static List<FreeImagePlugin> WriteOnlyPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!p.SupportsReading && p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are not able to
		/// read or write.
		/// </summary>
		public static List<FreeImagePlugin> StupidPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!p.SupportsReading && !p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are able to read.
		/// </summary>
		public static List<FreeImagePlugin> ReadablePlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsReading)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of plugins that are able to write.
		/// </summary>
		public static List<FreeImagePlugin> WriteablePlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (p.SupportsWriting)
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Gets a list of local plugins.
		/// </summary>
		public static ReadOnlyCollection<FreeImagePlugin> LocalPlugins
		{
			get
			{
				return localPlugins.AsReadOnly();  
			}
		}

		/// <summary>
		/// Gets a list of built-in plugins.
		/// </summary>
		public static List<FreeImagePlugin> BuiltInPlugins
		{
			get
			{
				List<FreeImagePlugin> list = new List<FreeImagePlugin>();
				foreach (FreeImagePlugin p in plugins)
				{
					if (!localPlugins.Contains(p))
					{
						list.Add(p);
					}
				}
				return list;
			}
		}

		public static FreeImagePlugin BMP { get { return plugins[0]; } }
		public static FreeImagePlugin ICO { get { return plugins[1]; } }
		public static FreeImagePlugin JPEG { get { return plugins[2]; } }
		public static FreeImagePlugin JNG { get { return plugins[3]; } }
		public static FreeImagePlugin KOALA { get { return plugins[4]; } }
		public static FreeImagePlugin LBM { get { return plugins[5]; } }
		public static FreeImagePlugin IFF { get { return plugins[5]; } }
		public static FreeImagePlugin MNG { get { return plugins[6]; } }
		public static FreeImagePlugin PBM { get { return plugins[7]; } }
		public static FreeImagePlugin PBMRAW { get { return plugins[8]; } }
		public static FreeImagePlugin PCD { get { return plugins[9]; } }
		public static FreeImagePlugin PCX { get { return plugins[10]; } }
		public static FreeImagePlugin PGM { get { return plugins[11]; } }
		public static FreeImagePlugin PGMRAW { get { return plugins[12]; } }
		public static FreeImagePlugin PNG { get { return plugins[13]; } }
		public static FreeImagePlugin PPM { get { return plugins[14]; } }
		public static FreeImagePlugin PPMRAW { get { return plugins[15]; } }
		public static FreeImagePlugin RAS { get { return plugins[16]; } }
		public static FreeImagePlugin TARGA { get { return plugins[17]; } }
		public static FreeImagePlugin TIFF { get { return plugins[18]; } }
		public static FreeImagePlugin WBMP { get { return plugins[19]; } }
		public static FreeImagePlugin PSD { get { return plugins[20]; } }
		public static FreeImagePlugin CUT { get { return plugins[21]; } }
		public static FreeImagePlugin XBM { get { return plugins[22]; } }
		public static FreeImagePlugin XPM { get { return plugins[23]; } }
		public static FreeImagePlugin DDS { get { return plugins[24]; } }
		public static FreeImagePlugin GIF { get { return plugins[25]; } }
		public static FreeImagePlugin HDR { get { return plugins[26]; } }
		public static FreeImagePlugin FAXG3 { get { return plugins[27]; } }
		public static FreeImagePlugin SGI { get { return plugins[28]; } }
		public static FreeImagePlugin EXR { get { return plugins[29]; } }
		public static FreeImagePlugin J2K { get { return plugins[30]; } }
		public static FreeImagePlugin JP2 { get { return plugins[31]; } }
	}

	#endregion

	#region Enums

	/// <summary>
	/// Enumeration used for color conversions.
	/// FREE_IMAGE_COLOR_DEPTH contains several colors to convert to.
	/// The default value 'FICD_AUTO'.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_COLOR_DEPTH
	{
		/// <summary>
		/// Unknown.
		/// </summary>
		FICD_UNKNOWN = 0,
		/// <summary>
		/// Auto selected by the used algorithm.
		/// </summary>
		FICD_AUTO = FICD_UNKNOWN,
		/// <summary>
		/// 1-bit.
		/// </summary>
		FICD_01_BPP = 1,
		/// <summary>
		/// 1-bit using dithering.
		/// </summary>
		FICD_01_BPP_DITHER = FICD_01_BPP,
		/// <summary>
		/// 1-bit using threshold.
		/// </summary>
		FICD_01_BPP_THRESHOLD = FICD_01_BPP | 2,
		/// <summary>
		/// 4-bit.
		/// </summary>
		FICD_04_BPP = 4,
		/// <summary>
		/// 8-bit.
		/// </summary>
		FICD_08_BPP = 8,
		/// <summary>
		/// 16-bit 555 (1 bit remains unused).
		/// </summary>
		FICD_16_BPP_555 = FICD_16_BPP | 2,
		/// <summary>
		/// 16-bit 565 (all bits are used).
		/// </summary>
		FICD_16_BPP = 16,
		/// <summary>
		/// 24-bit.
		/// </summary>
		FICD_24_BPP = 24,
		/// <summary>
		/// 32-bit.
		/// </summary>
		FICD_32_BPP = 32,
		/// <summary>
		/// Reorder palette (make it linear). Only affects 1-, 4- and 8-bit images.
		/// <para>The palette is only reordered in case the image is greyscale
		/// (all palette entries have the same red, green and blue value).</para>
		/// </summary>
		FICD_REORDER_PALETTE = 1024,
		/// <summary>
		/// Converts the image to greyscale.
		/// </summary>
		FICD_FORCE_GREYSCALE = 2048,
		/// <summary>
		/// Flag to mask out all non color depth flags.
		/// </summary>
		FICD_COLOR_MASK = FICD_01_BPP | FICD_04_BPP | FICD_08_BPP | FICD_16_BPP | FICD_24_BPP | FICD_32_BPP
	}

	/// <summary>
	/// List of combinable compare modes.
	/// </summary>
	[System.Flags]
	public enum FREE_IMAGE_COMPARE_FLAGS
	{
		/// <summary>
		/// Compare headers.
		/// </summary>
		HEADER = 0x1,
		/// <summary>
		/// Compare palettes.
		/// </summary>
		PALETTE = 0x2,
		/// <summary>
		/// Compare pixel data.
		/// </summary>
		DATA = 0x4,
		/// <summary>
		/// Compare meta data.
		/// </summary>
		METADATA = 0x8,
		/// <summary>
		/// Compare everything.
		/// </summary>
		COMPLETE = (HEADER | PALETTE | DATA | METADATA)
	}

	/// <summary>
	/// List different search modes.
	/// </summary>
	[System.Flags]
	public enum MD_SEARCH_FLAGS
	{
		/// <summary>
		/// The key of the metadata.
		/// </summary>
		KEY = 0x1,
		/// <summary>
		/// The description of the metadata
		/// </summary>
		DESCRIPTION = 0x2,
		/// <summary>
		/// The ToString value of the metadata
		/// </summary>
		TOSTRING = 0x4,
	}

	/// <summary>
	/// Flags for copying data from a bitmap to another.
	/// </summary>
	public enum FREE_IMAGE_METADATA_COPY
	{
		/// <summary>
		/// Exisiting metadata will remain unchanged.
		/// </summary>
		KEEP_EXISITNG = 0x0,
		/// <summary>
		/// Existing metadata will be cleared.
		/// </summary>
		CLEAR_EXISTING = 0x1,
		/// <summary>
		/// Existing metadata will be overwritten.
		/// </summary>
		REPLACE_EXISTING = 0x2
	}

	#endregion

	/// <summary>
	/// Static class importing functions from the FreeImage library
	/// and providing additional wrapper functions.
	/// </summary>
	public static partial class FreeImage
	{
		#region Constants

		/// <summary>
		/// Array containing all 'FREE_IMAGE_MDMODEL's.
		/// </summary>
		public static readonly FREE_IMAGE_MDMODEL[] FREE_IMAGE_MDMODELS =
			(FREE_IMAGE_MDMODEL[])Enum.GetValues(typeof(FREE_IMAGE_MDMODEL));

		/// <summary>
		/// Saved instance for faster access.
		/// </summary>
		private static readonly ConstructorInfo PropertyItemConstructor =
			typeof(PropertyItem).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

		private const int DIB_RGB_COLORS = 0;
		private const int DIB_PAL_COLORS = 1;
		private const int CBM_INIT = 0x4;

		#endregion

		#region Callback

		// Callback delegate
		private static OutputMessageFunction outputMessageFunction;
		// Handle to pin the functions address
		private static GCHandle outputMessageHandle;

		static FreeImage()
		{
			// Check if FreeImage.dll is present and cancel setting the callbackfuntion if not
			if (!IsAvailable())
			{
				return;
			}
			// Create a delegate (function pointer) to 'OnMessage'
			outputMessageFunction = new OutputMessageFunction(OnMessage);
			// Pin the object so the garbage collector does not move it around in memory
			outputMessageHandle = GCHandle.Alloc(outputMessageFunction, GCHandleType.Normal);
			// Set the callback
			SetOutputMessage(outputMessageFunction);
		}

		/// <summary>
		/// Internal callback
		/// </summary>
		private static void OnMessage(FREE_IMAGE_FORMAT fif, string message)
		{
			// Invoke the message
			if (Message != null)
			{
				Message.Invoke(fif, message);
			}
		}

		/// <summary>
		/// Internal errors in FreeImage generate a logstring that can be
		/// captured by this event.
		/// </summary>
		public static event OutputMessageFunction Message;

		#endregion

		#region General functions

		/// <summary>
		/// Returns the internal version of this FreeImage 3 .NET wrapper.
		/// </summary>
		/// <returns>The internal version of this FreeImage 3 .NET wrapper.</returns>
		public static string GetWrapperVersion()
		{
			return FREEIMAGE_MAJOR_VERSION + "." + FREEIMAGE_MINOR_VERSION + "." + FREEIMAGE_RELEASE_SERIAL;
		}

		/// <summary>
		/// Returns a value indicating if the FreeImage DLL is available or not.
		/// </summary>
		/// <returns>True, if the FreeImage DLL is available, false otherwise.</returns>
		public static bool IsAvailable()
		{
			try
			{
				// Call a static fast executing function
				GetVersion();
				// No exception thrown, the dll seems to be present
				return true;
			}
			catch (EntryPointNotFoundException)
			{
				return false;
			}
		}

		#endregion

		#region Bitmap management functions

		/// <summary>
		/// Converts a FreeImage bitmap to a .NET bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The converted .NET bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the image type of <paramref name="dib"/> is not FIT_BITMAP.</exception>
		public static Bitmap GetBitmap(FIBITMAP dib)
		{
			return GetBitmap(dib, true);
		}

		/// <summary>
		/// Converts a FreeImage bitmap to a .NET bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="copyMetadata">When true existing metadata will be copied.</param>
		/// <returns>The converted .NET bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the image type of <paramref name="dib"/> is not FIT_BITMAP.</exception>
		internal static Bitmap GetBitmap(FIBITMAP dib, bool copyMetadata)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (GetImageType(dib) != FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				throw new ArgumentException("Only bitmaps with type of FIT_BITMAP can be converted.");
			}
			PixelFormat format = GetPixelFormat(dib);
			int height = (int)GetHeight(dib);
			int width = (int)GetWidth(dib);
			int pitch = (int)GetPitch(dib);
			Bitmap result = new Bitmap(width, height, format);
			BitmapData data;
			// Locking the complete bitmap in writeonly mode
			data = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
			// Writing the bitmap data directly into the new created .NET bitmap.
			ConvertToRawBits(
				data.Scan0,
				dib,
				pitch,
				GetBPP(dib),
				GetRedMask(dib),
				GetGreenMask(dib),
				GetBlueMask(dib),
				true);
			// Unlock the bitmap
			result.UnlockBits(data);
			// Apply the bitmaps resolution
			result.SetResolution(GetResolutionX(dib), GetResolutionY(dib));
			// Check whether the bitmap has a palette
			if (GetPalette(dib) != IntPtr.Zero)
			{
				// Get the bitmaps palette to apply changes
				ColorPalette palette = result.Palette;
				// Get the orgininal palette
				Color[] colorPalette = new RGBQUADARRAY(dib).ColorData;
				// Copy each value
				if (palette.Entries.Length == colorPalette.Length)
				{
					for (int i = 0; i < colorPalette.Length; i++)
					{
						palette.Entries[i] = colorPalette[i];
					}
					// Set the bitmaps palette
					result.Palette = palette;
				}
			}
			// Copy metadata
			if (copyMetadata)
			{
				try
				{
					List<PropertyItem> list = new List<PropertyItem>();
					// Get a list of all types
					FITAG tag;
					FIMETADATA mData;
					foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
					{
						// Get a unique search handle
						mData = FindFirstMetadata(model, dib, out tag);
						// Check if metadata exists for this type
						if (mData.IsNull) continue;
						do
						{
							PropertyItem propItem = CreatePropertyItem();
							propItem.Len = (int)GetTagLength(tag);
							propItem.Id = (int)GetTagID(tag);
							propItem.Type = (short)GetTagType(tag);
							byte[] buffer = new byte[propItem.Len];

							unsafe
							{
								byte* src = (byte*)GetTagValue(tag);
								fixed (byte* dst = buffer)
								{
									MoveMemory(dst, src, (uint)propItem.Len);
								}
							}

							propItem.Value = buffer;
							list.Add(propItem);
						}
						while (FindNextMetadata(mData, out tag));
						FindCloseMetadata(mData);
					}
					foreach (PropertyItem propItem in list)
					{
						result.SetPropertyItem(propItem);
					}
				}
				catch
				{
				}
			}
			return result;
		}

		/// <summary>
		/// Converts an .NET bitmap into a FreeImage bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap to convert.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="bitmap"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps pixelformat is invalid.</exception>
		public static FIBITMAP CreateFromBitmap(Bitmap bitmap)
		{
			return CreateFromBitmap(bitmap, false);
		}

		/// <summary>
		/// Converts an .NET bitmap into a FreeImage bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap to convert.</param>
		/// <param name="copyMetadata">When true existing metadata will be copied.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>Copying metadata has been disabled until a proper way
		/// of reading and storing metadata in a .NET bitmap is found.</remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="bitmap"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps pixelformat is invalid.</exception>
		internal static FIBITMAP CreateFromBitmap(Bitmap bitmap, bool copyMetadata)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			uint bpp, red_mask, green_mask, blue_mask;
			if (!GetFormatParameters(bitmap.PixelFormat, out bpp, out red_mask, out green_mask, out blue_mask))
			{
				throw new ArgumentException("The bitmaps pixelformat is invalid.");
			}
			// Locking the complete bitmap in readonly mode
			BitmapData data = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, bitmap.PixelFormat);
			// Copying the bitmap data directly from the .NET bitmap
			FIBITMAP result =
				ConvertFromRawBits(
					data.Scan0,
					data.Width,
					data.Height,
					data.Stride,
					bpp,
					red_mask,
					green_mask,
					blue_mask,
					true);
			bitmap.UnlockBits(data);
			// Handle palette
			if (GetPalette(result) != IntPtr.Zero)
			{
				RGBQUADARRAY palette = new RGBQUADARRAY(result);
				if (palette.Length == bitmap.Palette.Entries.Length)
				{
					for (int i = 0; i < palette.Length; i++)
					{
						palette.SetColor(i, bitmap.Palette.Entries[i]);
					}
				}
			}
			// Handle meta data
			// Disabled
			//if (copyMetadata)
			//{
			//    foreach (PropertyItem propItem in bitmap.PropertyItems)
			//    {
			//        FITAG tag = CreateTag();
			//        SetTagLength(tag, (uint)propItem.Len);
			//        SetTagID(tag, (ushort)propItem.Id);
			//        SetTagType(tag, (FREE_IMAGE_MDTYPE)propItem.Type);
			//        SetTagValue(tag, propItem.Value);
			//        SetMetadata(FREE_IMAGE_MDMODEL.FIMD_EXIF_EXIF, result, "", tag);
			//    }
			//}
			return result;
		}

		/// <summary>
		/// Saves a .NET Bitmap to a file.
		/// </summary>
		/// <param name="bitmap">The .NET bitmap to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename)
		{
			return SaveBitmap(
				bitmap,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Saves a .NET Bitmap to a file.
		/// </summary>
		/// <param name="bitmap">The .NET bitmap to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveBitmap(
				bitmap,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags);
		}

		/// <summary>
		/// Saves a .NET Bitmap to a file.
		/// </summary>
		/// <param name="bitmap">The .NET bitmap to save.</param>
		/// <param name="filename">Name of the file to save to.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="bitmap"/> or <paramref name="filename"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the bitmaps pixelformat is invalid.</exception>
		public static bool SaveBitmap(
			Bitmap bitmap,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			FIBITMAP dib = CreateFromBitmap(bitmap);
			bool result = SaveEx(dib, filename, format, flags);
			Unload(dib);
			return result;
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The file will be loaded with default loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadEx(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, FREE_IMAGE_LOAD_FLAGS flags)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadEx(filename, flags, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the files real format is being analysed.
		/// If no plugin can read the file, format remains 'FIF_UNKNOWN' and 0 is returned.
		/// The file will be loaded with default loading flags.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, ref FREE_IMAGE_FORMAT format)
		{
			return LoadEx(filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the files real format is being analysed.
		/// If no plugin can read the file, format remains 'FIF_UNKNOWN' and 0 is returned.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadEx it will be returned in format.
		/// </param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists.</exception>
		public static FIBITMAP LoadEx(string filename, FREE_IMAGE_LOAD_FLAGS flags, ref FREE_IMAGE_FORMAT format)
		{
			// check if file exists
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename + " could not be found.");
			}
			FIBITMAP dib = 0;
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// query all plugins to see if one can read the file
				format = GetFileType(filename, 0);
			}
			// check if the plugin is capable of loading files
			if (FIFSupportsReading(format))
			{
				dib = Load(format, filename, flags);
			}
			return dib;
		}

		/// <summary>
		/// Loads a .NET Bitmap from a file.
		/// </summary>
		/// <param name="filename">Name of the file to be loaded.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the image type of the image is not FIT_BITMAP.</exception>
		public static Bitmap LoadBitmap(string filename, FREE_IMAGE_LOAD_FLAGS flags, ref FREE_IMAGE_FORMAT format)
		{
			FIBITMAP dib = LoadEx(filename, flags, ref format);
			Bitmap result = GetBitmap(dib, true);
			Unload(dib);
			return result;
		}

		/// <summary>
		/// Deletes a previously loaded FIBITMAP from memory and resets the handle to null.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		public static void UnloadEx(ref FIBITMAP dib)
		{
			if (!dib.IsNull)
			{
				Unload(dib);
				dib = 0;
			}
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// In case the loading format is 'FIF_UNKNOWN' the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// The format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_SAVE_FLAGS flags,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				FREE_IMAGE_FORMAT.FIF_UNKNOWN,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// In case the loading format is 'FIF_UNKNOWN' the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			bool unloadSource)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a file.
		/// In case the loading format is 'FIF_UNKNOWN' the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveEx(
				ref dib,
				filename,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded <c>FIBITMAP</c> to a file.
		/// In case the loading format is 'FIF_UNKNOWN' the format is taken off the filename.
		/// If no suitable format was found false will be returned.
		/// Save flags can be provided by the flags parameter.
		/// The bitmaps color depth can be set by 'colorDepth'.
		/// If set to 'FICD_AUTO' a suitable color depth will be taken if available.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="filename">The complete name of the file to save to.
		/// The extension will be corrected if it is no valid extension for the
		/// selected format or if no extension was specified.</param>
		/// <param name="format">Format of the image. If the format should be taken from the
		/// filename use 'FIF_UNKNOWN'.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to 'FIC_AUTO' if Save should take the best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentException">
		/// Thrown in case a direct color conversion failed.</exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown in case <paramref name="dib"/> or <paramref name="filename"/> is null.</exception>
		public static bool SaveEx(
			ref FIBITMAP dib,
			string filename,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			bool result = false;
			// Gets format from filename if the format is unknown
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				format = GetFIFFromFilename(filename);
			}
			if (format != FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// Checks writing support
				if (FIFSupportsWriting(format) && FIFSupportsExportType(format, GetImageType(dib)))
				{
					// Check valid filename and correct it if needed
					if (!IsFilenameValidForFIF(format, filename))
					{
						int index = filename.LastIndexOf('.');
						string extension = GetPrimaryExtensionFromFIF(format);

						if (index == -1)
						{
							// We have no '.' (dot) so just add the extension
							filename += "." + extension;
						}
						else
						{
							// Overwrite the old extension
							filename = filename.Substring(0, filename.LastIndexOf('.')) + extension;
						}
					}

					FIBITMAP dibToSave = PrepareBitmapColorDepth(dib, format, colorDepth);
					result = Save(format, dibToSave, filename, flags);

					// Always unload a temporary created bitmap.
					if (dibToSave != dib)
					{
						UnloadEx(ref dibToSave);
					}
					// On success unload the bitmap
					if (result && unloadSource)
					{
						UnloadEx(ref dib);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadFromStream(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			FREE_IMAGE_LOAD_FLAGS flags)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return LoadFromStream(stream, flags, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the bitmaps real format is being analysed.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadFromStream it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			ref FREE_IMAGE_FORMAT format)
		{
			return LoadFromStream(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref format);
		}

		/// <summary>
		/// Loads a FreeImage bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the bitmaps real format is being analysed.
		/// The stream must be set to the correct position before calling LoadFromStream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadFromStream it will be returned in format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> is not capable of reading.</exception>
		public static FIBITMAP LoadFromStream(
			Stream stream,
			FREE_IMAGE_LOAD_FLAGS flags,
			ref FREE_IMAGE_FORMAT format)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			// Wrap the source stream if it is unable to seek (which is required by FreeImage)
			stream = (stream.CanSeek) ? stream : new StreamWrapper(stream, true);
			// Save the streams position
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				long position = stream.Position;
				// Get the format of the bitmap
				format = GetFileTypeFromStream(stream);
				// Restore the streams position
				stream.Position = position;
			}
			if (!FIFSupportsReading(format))
			{
				return 0;
			}
			// Create a 'FreeImageIO' structure for calling 'LoadFromHandle'
			// using the internal structure 'FreeImageStreamIO'.
			FreeImageIO io = FreeImageStreamIO.io;
			using (fi_handle handle = new fi_handle(stream))
			{
				return LoadFromHandle(format, ref io, handle, flags);
			}
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			bool unloadSource)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				FREE_IMAGE_SAVE_FLAGS.DEFAULT,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			bool unloadSource)
		{
			return SaveToStream(
				ref dib, stream,
				format,
				flags,
				FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
				unloadSource);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to 'FIC_AUTO' if SaveToStream should take the best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth)
		{
			return SaveToStream(
				ref dib,
				stream,
				format,
				flags,
				colorDepth,
				false);
		}

		/// <summary>
		/// Saves a previously loaded FIBITMAP to a stream.
		/// The stream must be set to the correct position before calling SaveToStream.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="colorDepth">The new color depth of the bitmap.
		/// Set to 'FIC_AUTO' if SaveToStream should take the best suitable color depth.
		/// If a color depth is selected that the provided format cannot write an
		/// error-message will be thrown.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> cannot write.</exception>
		public static bool SaveToStream(
			ref FIBITMAP dib,
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_SAVE_FLAGS flags,
			FREE_IMAGE_COLOR_DEPTH colorDepth,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("stream is not capable of writing.");
			}
			if ((!FIFSupportsWriting(format)) || (!FIFSupportsExportType(format, FREE_IMAGE_TYPE.FIT_BITMAP)))
			{
				return false;
			}

			FIBITMAP dibToSave = PrepareBitmapColorDepth(dib, format, colorDepth);
			bool result = false;

			try
			{
				// Create a 'FreeImageIO' structure for calling 'SaveToHandle'
				FreeImageIO io = FreeImageStreamIO.io;

				using (fi_handle handle = new fi_handle(stream))
				{
					result = SaveToHandle(format, dibToSave, ref io, handle, flags);
				}
			}
			catch
			{
				// Always unload a temporary created bitmap.
				if (dibToSave != dib)
				{
					UnloadEx(ref dibToSave);
				}
				// On success unload the bitmap
				if (result && unloadSource)
				{
					UnloadEx(ref dib);
				}
			}

			return result;
		}

		#endregion

		#region Plugin functions

		/// <summary>
		/// Checks if an extension is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="extension">The desired extension.</param>
		/// <returns>True if the extension is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="extension"/> is null.</exception>
		public static bool IsExtensionValidForFIF(FREE_IMAGE_FORMAT fif, string extension)
		{
			return IsExtensionValidForFIF(fif, extension, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Checks if an extension is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="extension">The desired extension.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the extension is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="extension"/> is null.</exception>
		public static bool IsExtensionValidForFIF(FREE_IMAGE_FORMAT fif, string extension, StringComparison comparisonType)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			bool result = false;
			// Split up the string and compare each with the given extension
			string tempList = GetFIFExtensionList(fif);
			if (tempList != null)
			{
				string[] extensionList = tempList.Split(',');
				foreach (string ext in extensionList)
				{
					if (extension.Equals(ext, comparisonType))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Checks if a filename is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="filename">The desired filename.</param>
		/// <returns>True if the filename is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="filename"/> is null.</exception>
		public static bool IsFilenameValidForFIF(FREE_IMAGE_FORMAT fif, string filename)
		{
			return IsFilenameValidForFIF(fif, filename, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Checks if a filename is valid for a certain format.
		/// </summary>
		/// <param name="fif">The desired format.</param>
		/// <param name="filename">The desired filename.</param>
		/// <param name="comparisonType">The string comparison type.</param>
		/// <returns>True if the filename is valid for the given format, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="filename"/> is null.</exception>
		public static bool IsFilenameValidForFIF(FREE_IMAGE_FORMAT fif, string filename, StringComparison comparisonType)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			bool result = false;
			// Extract the filenames extension if it exists
			int position = filename.LastIndexOf('.');
			if (position >= 0)
			{
				result = IsExtensionValidForFIF(fif, filename.Substring(position + 1), comparisonType);
			}
			return result;
		}

		/// <summary>
		/// This function returns the primary (main or most commonly used?) extension of a certain
		/// image format (fif). This is done by returning the first of all possible extensions
		/// returned by GetFIFExtensionList().
		/// That assumes, that the plugin returns the extensions in ordered form.</summary>
		/// <param name="fif">The image format to obtain the primary extension for.</param>
		/// <returns>The primary extension of the specified image format.</returns>
		public static string GetPrimaryExtensionFromFIF(FREE_IMAGE_FORMAT fif)
		{
			string result = null;
			string extensions = GetFIFExtensionList(fif);
			if (extensions != null)
			{
				int position = extensions.IndexOf(',');
				if (position < 0)
				{
					result = extensions;
				}
				else
				{
					result = extensions.Substring(0, position);
				}
			}
			return result;
		}

		#endregion

		#region Multipage functions

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				false,
				false);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				false,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool read_only,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				false,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				create_new,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the files real format is being analysed.
		/// If no plugin can read the file, format remains 'FIF_UNKNOWN' and 0 is returned.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			ref FREE_IMAGE_FORMAT format,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			return OpenMultiBitmapEx(
				filename,
				ref format,
				FREE_IMAGE_LOAD_FLAGS.DEFAULT,
				create_new,
				read_only,
				keep_cache_in_memory);
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap.
		/// In case the loading format is 'FIF_UNKNOWN' the files real format is being analysed.
		/// If no plugin can read the file, format remains 'FIF_UNKNOWN' and 0 is returned.
		/// Load flags can be provided by the flags parameter.
		/// </summary>
		/// <param name="filename">The complete name of the file to load.</param>
		/// <param name="format">Format of the image. If the format is unknown use 'FIF_UNKNOWN'.
		/// In case a suitable format was found by LoadEx it will be returned in format.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="create_new">When true a new bitmap is created.</param>
		/// <param name="read_only">When true the bitmap will be loaded read only.</param>
		/// <param name="keep_cache_in_memory">When true performance is increased at the cost of memory.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="FileNotFoundException">
		/// Thrown in case <paramref name="filename"/> does not exists while opening.</exception>
		public static FIMULTIBITMAP OpenMultiBitmapEx(
			string filename,
			ref FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS flags,
			bool create_new,
			bool read_only,
			bool keep_cache_in_memory)
		{
			if (!File.Exists(filename) && !create_new)
			{
				throw new FileNotFoundException(filename + " could not be found.");
			}
			if (format == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			{
				// Check if a plugin can read the data
				format = GetFileType(filename, 0);
			}
			FIMULTIBITMAP dib = 0;
			if (FIFSupportsReading(format))
			{
				dib = OpenMultiBitmap(format, filename, create_new, read_only, keep_cache_in_memory, flags);
			}
			return dib;
		}

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only, applies any changes made to it.
		/// On success the handle will be reset to null.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool CloseMultiBitmapEx(ref FIMULTIBITMAP dib)
		{
			return CloseMultiBitmapEx(ref dib, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
		}

		/// <summary>
		/// Closes a previously opened multi-page bitmap and, when the bitmap was not opened read-only, applies any changes made to it.
		/// On success the handle will be reset to null.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool CloseMultiBitmapEx(ref FIMULTIBITMAP dib, FREE_IMAGE_SAVE_FLAGS flags)
		{
			bool result = false;
			if (!dib.IsNull)
			{
				if (CloseMultiBitmap(dib, flags))
				{
					dib = 0;
					result = true;
				}
			}
			return result;
		}

		/// <summary>
		/// Retrieves the number of pages that are locked in a multi-paged bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>Number of locked pages.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static int GetLockedPageCount(FIMULTIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			int result = 0;
			GetLockedPageNumbers(dib, null, ref result);
			return result;
		}

		/// <summary>
		/// Retrieves a list locked pages of a multi-paged bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage multi-paged bitmap.</param>
		/// <returns>List containing the indexes of the locked pages.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static int[] GetLockedPages(FIMULTIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			// Get the number of pages and create an array to save the information
			int count = 0;
			int[] result = null;
			// Get count
			if (GetLockedPageNumbers(dib, result, ref count))
			{
				result = new int[count];
				// Fill array
				if (!GetLockedPageNumbers(dib, result, ref count))
				{
					result = null;
				}
			}
			return result;
		}

		/// <summary>
		/// Loads a FreeImage multi-paged bitmap from a stream and returns the
		/// FreeImage memory stream used as temporary buffer.
		/// The bitmap can not be modified by calling 'AppendPage', 'InsertPage',
		/// 'MovePage' or 'DeletePage'.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="format">Format of the image.</param>
		/// <param name="flags">Flags to enable or disable plugin-features.</param>
		/// <param name="memory">The temporary memory buffer used to load the bitmap.</param>
		/// <returns>Handle to a FreeImage multi-paged bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="stream"/> can not read.</exception>
		public static FIMULTIBITMAP LoadMultiBitmapFromStream(
			Stream stream,
			FREE_IMAGE_FORMAT format,
			FREE_IMAGE_LOAD_FLAGS flags,
			out FIMEMORY memory)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream");
			}
			const int blockSize = 1024;
			int bytesRead;
			byte[] buffer = new byte[blockSize];

			stream = stream.CanSeek ? stream : new StreamWrapper(stream, true);
			memory = OpenMemory(IntPtr.Zero, 0);

			do
			{
				bytesRead = stream.Read(buffer, 0, blockSize);
				WriteMemory(buffer, (uint)blockSize, (uint)1, memory);
			}
			while (bytesRead == blockSize);

			return LoadMultiBitmapFromMemory(format, memory, flags);
		}

		#endregion

		#region Filetype functions

		/// <summary>
		/// Orders FreeImage to analyze the bitmap signature.
		/// In case the stream is not seekable, the stream will have been used
		/// and must be recreated for loading.
		/// </summary>
		/// <param name="stream">Name of the stream to analyze.</param>
		/// <returns>Type of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if stream can not read.</exception>
		public static FREE_IMAGE_FORMAT GetFileTypeFromStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream is not capable of reading.");
			}
			// Wrap the stream if it cannot seek
			stream = (stream.CanSeek) ? stream : new StreamWrapper(stream, true);
			// Create a 'FreeImageIO' structure for the stream
			FreeImageIO io = FreeImageStreamIO.io;
			using (fi_handle handle = new fi_handle(stream))
			{
				return GetFileTypeFromHandle(ref io, handle, 0);
			}
		}

		#endregion

		#region Pixel access functions

		/// <summary>
		/// Retrieves an hBitmap for a FIBITMAP.
		/// Call FreeHbitmap(IntPtr) to free the handle.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hdc">A reference device context.
		/// Use IntPtr.Zero if no reference is available.</param>
		/// <param name="unload">When true dib will be unloaded if the function succeeded.</param>
		/// <returns>The hBitmap for the FIBITMAP.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe IntPtr GetHbitmap(FIBITMAP dib, IntPtr hdc, bool unload)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			IntPtr hBitmap = IntPtr.Zero;
			bool release = false;
			IntPtr ppvBits = IntPtr.Zero;
			// Check if we have destination
			if (release = (hdc == IntPtr.Zero))
			{
				// We don't so request dc
				hdc = GetDC(IntPtr.Zero);
			}
			if (hdc != IntPtr.Zero)
			{
				// Get pointer to the infoheader of the bitmap
				IntPtr info = GetInfo(dib);
				// Create a bitmap in the dc
				hBitmap = CreateDIBSection(hdc, info, DIB_RGB_COLORS, out ppvBits, IntPtr.Zero, 0);
				if (hBitmap != IntPtr.Zero && ppvBits != IntPtr.Zero)
				{
					// Copy the data into the dc
					MoveMemory(
						(void*)ppvBits,
						(void*)GetBits(dib),
						(GetHeight(dib) * GetPitch(dib)));
					// Success: we unload the bitmap
					if (unload)
					{
						Unload(dib);
					}
				}
				// We have to release the dc
				if (release)
				{
					ReleaseDC(IntPtr.Zero, hdc);
				}
			}
			return hBitmap;
		}

		/// <summary>
		/// Returns an HBITMAP created by the CreateDIBitmap() function which in turn
		/// has always the same color depth as the reference DC, which may be provided
		/// through the 'reference' parameter. The desktop DC will be used,
		/// if 'IntPtr.Zero' DC is specified.
		/// Call FreeHbitmap(IntPtr) to free the handle.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="unload">When true the structure will be unloaded on success.
		/// If the function failed and returned false, the bitmap was not unloaded.</param>
		/// <returns>If the function succeeds, the return value is a handle to the
		/// compatible bitmap. If the function fails, the return value is NULL.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static IntPtr GetBitmapForDevice(FIBITMAP dib, IntPtr hdc, bool unload)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			IntPtr hbitmap = IntPtr.Zero;
			bool release = false;
			if (release = (hdc == IntPtr.Zero))
			{
				hdc = GetDC(IntPtr.Zero);
			}
			if (hdc != IntPtr.Zero)
			{
				hbitmap = CreateDIBitmap(
					hdc,
					GetInfoHeader(dib),
					CBM_INIT,
					GetBits(dib),
					GetInfo(dib),
					DIB_RGB_COLORS);
				if (unload)
				{
					Unload(dib);
				}
				if (release)
				{
					ReleaseDC(IntPtr.Zero, hdc);
				}
			}
			return hbitmap;
		}

		/// <summary>
		/// Creates a FreeImage DIB from a Device Context/Compatible Bitmap.
		/// </summary>
		/// <param name="hbitmap">Handle to the bitmap.</param>
		/// <param name="hdc">Handle to a device context.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="hbitmap"/> is null.</exception>
		public unsafe static FIBITMAP CreateFromHbitmap(IntPtr hbitmap, IntPtr hdc)
		{
			if (hbitmap == IntPtr.Zero)
			{
				throw new ArgumentNullException("hbitmap");
			}

			FIBITMAP dib = 0;
			BITMAP bm;
			uint colors;
			bool release;

			if (GetObject(hbitmap, sizeof(BITMAP), (IntPtr)(&bm)) != 0)
			{
				dib = Allocate(bm.bmWidth, bm.bmHeight, bm.bmBitsPixel, 0, 0, 0);
				if (!dib.IsNull)
				{
					colors = GetColorsUsed(dib);
					if (release = (hdc == IntPtr.Zero))
					{
						hdc = GetDC(IntPtr.Zero);
					}
					if (GetDIBits(
							hdc,
							hbitmap,
							0,
							(uint)bm.bmHeight,
							GetBits(dib),
							GetInfo(dib),
							DIB_RGB_COLORS) != 0)
					{
						if (colors != 0)
						{
							BITMAPINFOHEADER* bmih = (BITMAPINFOHEADER*)GetInfo(dib);
							bmih[0].biClrImportant = bmih[0].biClrUsed = colors;
						}
					}
					else
					{
						UnloadEx(ref dib);
					}
					if (release)
					{
						ReleaseDC(IntPtr.Zero, hdc);
					}
				}
			}

			return dib;
		}

		/// <summary>
		/// Frees a bitmap handle.
		/// </summary>
		/// <param name="hbitmap">Handle to a bitmap.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool FreeHbitmap(IntPtr hbitmap)
		{
			return DeleteObject(hbitmap);
		}

		#endregion

		#region Bitmap information functions

		/// <summary>
		/// Retrieves a DIB's resolution in X-direction measured in 'dots per inch' (DPI) and not in 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The resolution in 'dots per inch'.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static uint GetResolutionX(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return (uint)(0.5d + 0.0254d * GetDotsPerMeterX(dib));
		}

		/// <summary>
		/// Retrieves a DIB's resolution in Y-direction measured in 'dots per inch' (DPI) and not in 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The resolution in 'dots per inch'.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static uint GetResolutionY(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return (uint)(0.5d + 0.0254d * GetDotsPerMeterY(dib));
		}

		/// <summary>
		/// Sets a DIB's resolution in X-direction measured in 'dots per inch' (DPI) and not in 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new resolution in 'dots per inch'.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static void SetResolutionX(FIBITMAP dib, uint res)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			SetDotsPerMeterX(dib, (uint)((double)res / 0.0254d + 0.5d));
		}

		/// <summary>
		/// Sets a DIB's resolution in Y-direction measured in 'dots per inch' (DPI) and not in 'dots per meter'.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="res">The new resolution in 'dots per inch'.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static void SetResolutionY(FIBITMAP dib, uint res)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			SetDotsPerMeterY(dib, (uint)((double)res / 0.0254d + 0.5d));
		}

		/// <summary>
		/// Returns whether the image is a greyscale image or not.
		/// The function scans all colors in the bitmaps palette for entries where
		/// red, green and blue are not all the same (not a grey color).
		/// Supports 1-, 4- and 8-bit bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>True if the image is a greyscale image, else false.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe bool IsGreyscaleImage(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			bool result = true;
			uint bpp = GetBPP(dib);
			switch (bpp)
			{
				case 1:
				case 4:
				case 8:
					RGBQUAD* palette = (RGBQUAD*)GetPalette(dib);
					uint paletteLength = GetColorsUsed(dib);
					for (int i = 0; i < paletteLength; i++)
					{
						if (palette[i].rgbRed != palette[i].rgbGreen ||
							palette[i].rgbRed != palette[i].rgbBlue)
						{
							result = false;
							break;
						}
					}
					break;
				default:
					result = false;
					break;
			}
			return result;
		}

		/// <summary>
		/// Returns a structure that wraps the palette of a FreeImage bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>A structure wrapping the bitmaps palette.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static RGBQUADARRAY GetPaletteEx(FIBITMAP dib)
		{
			return new RGBQUADARRAY(dib);
		}

		/// <summary>
		/// Returns the BITMAPINFOHEADER structure of a FreeImage bitmap.
		/// The structure is a copy, so changes will have no effect on
		/// the bitmap itself.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>BITMAPINFOHEADER structure of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe BITMAPINFOHEADER GetInfoHeaderEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return *(BITMAPINFOHEADER*)GetInfoHeader(dib);
		}

		/// <summary>
		/// Returns the BITMAPINFO structure of a FreeImage bitmap.
		/// The structure is a copy, so changes will have no effect on
		/// the bitmap itself.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>BITMAPINFO structure of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static BITMAPINFO GetInfoEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			BITMAPINFO result = new BITMAPINFO();
			result.bmiHeader = GetInfoHeaderEx(dib);
			IntPtr ptr = GetPalette(dib);
			if (ptr == IntPtr.Zero)
			{
				result.bmiColors = new RGBQUAD[0];
			}
			else
			{
				RGBQUADARRAY array = new RGBQUADARRAY(ptr, result.bmiHeader.biClrUsed);
				result.bmiColors = array.Data;
			}
			return result;
		}

		/// <summary>
		/// Returns the pixelformat of the bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Pixelformat of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static PixelFormat GetPixelFormat(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			PixelFormat result = PixelFormat.Undefined;

			if (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				switch (GetBPP(dib))
				{
					case 1:
						result = PixelFormat.Format1bppIndexed;
						break;
					case 4:
						result = PixelFormat.Format4bppIndexed;
						break;
					case 8:
						result = PixelFormat.Format8bppIndexed;
						break;
					case 16:
						if ((GetBlueMask(dib) == FI16_565_BLUE_MASK) &&
							(GetGreenMask(dib) == FI16_565_GREEN_MASK) &&
							(GetRedMask(dib) == FI16_565_RED_MASK))
						{
							result = PixelFormat.Format16bppRgb565;
						}
						if ((GetBlueMask(dib) == FI16_555_BLUE_MASK) &&
							(GetGreenMask(dib) == FI16_555_GREEN_MASK) &&
							(GetRedMask(dib) == FI16_555_RED_MASK))
						{
							result = PixelFormat.Format16bppRgb555;
						}
						break;
					case 24:
						result = PixelFormat.Format24bppRgb;
						break;
					case 32:
						result = PixelFormat.Format32bppArgb;
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Retrieves all parameters needed to create a new FreeImage bitmap from
		/// the format of a .NET image.
		/// </summary>
		/// <param name="format">The format of the .NET image.</param>
		/// <param name="bpp">Returns the color depth for the new bitmap.</param>
		/// <param name="red_mask">Returns the red_mask for the new bitmap.</param>
		/// <param name="green_mask">Returns the green_mask for the new bitmap.</param>
		/// <param name="blue_mask">Returns the blue_mask for the new bitmap.</param>
		/// <returns>True in case <paramref name="format"/> represents FIT_BITMAP, else false.</returns>
		public static bool GetFormatParameters(
			PixelFormat format,
			out uint bpp,
			out uint red_mask,
			out uint green_mask,
			out uint blue_mask)
		{
			bool result = false;
			bpp = 0;
			red_mask = 0;
			green_mask = 0;
			blue_mask = 0;
			switch (format)
			{
				case PixelFormat.Format1bppIndexed:
					bpp = 1;
					result = true;
					break;
				case PixelFormat.Format4bppIndexed:
					bpp = 4;
					result = true;
					break;
				case PixelFormat.Format8bppIndexed:
					bpp = 8;
					result = true;
					break;
				case PixelFormat.Format16bppRgb565:
					bpp = 16;
					red_mask = FI16_565_RED_MASK;
					green_mask = FI16_565_GREEN_MASK;
					blue_mask = FI16_565_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format16bppRgb555:
					bpp = 16;
					red_mask = FI16_555_RED_MASK;
					green_mask = FI16_555_GREEN_MASK;
					blue_mask = FI16_555_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format24bppRgb:
					bpp = 24;
					red_mask = FI_RGBA_RED_MASK;
					green_mask = FI_RGBA_GREEN_MASK;
					blue_mask = FI_RGBA_BLUE_MASK;
					result = true;
					break;
				case PixelFormat.Format32bppArgb:
					bpp = 32;
					red_mask = FI_RGBA_RED_MASK;
					green_mask = FI_RGBA_GREEN_MASK;
					blue_mask = FI_RGBA_BLUE_MASK;
					result = true;
					break;
			}
			return result;
		}

		/// <summary>
		/// Compares two FreeImage bitmaps.
		/// </summary>
		/// <param name="dib1">The first bitmap to compare.</param>
		/// <param name="dib2">The second bitmap to compare.</param>
		/// <param name="flags">Determines which components of the bitmaps will be compared.</param>
		/// <returns>True in case both bitmaps match the compare conditions, false otherwise.</returns>
		public static bool Compare(FIBITMAP dib1, FIBITMAP dib2, FREE_IMAGE_COMPARE_FLAGS flags)
		{
			// Check whether one bitmap is null
			if (dib1.IsNull ^ dib2.IsNull)
			{
				return false;
			}
			// Check whether both pointers are the same
			if (dib1 == dib2)
			{
				return true;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.HEADER) > 0) && (!CompareHeader(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.PALETTE) > 0) && (!ComparePalette(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.DATA) > 0) && (!CompareData(dib1, dib2)))
			{
				return false;
			}
			if (((flags & FREE_IMAGE_COMPARE_FLAGS.METADATA) > 0) && (!CompareMetadata(dib1, dib2)))
			{
				return false;
			}
			return true;
		}

		private static unsafe bool CompareHeader(FIBITMAP dib1, FIBITMAP dib2)
		{
			IntPtr i1 = GetInfoHeader(dib1);
			IntPtr i2 = GetInfoHeader(dib2);
			return CompareMemory((void*)i1, (void*)i2, sizeof(BITMAPINFOHEADER));
		}

		private static unsafe bool ComparePalette(FIBITMAP dib1, FIBITMAP dib2)
		{
			IntPtr pal1 = GetPalette(dib1), pal2 = GetPalette(dib2);
			bool hasPalette1 = pal1 != IntPtr.Zero;
			bool hasPalette2 = pal2 != IntPtr.Zero;
			if (hasPalette1 ^ hasPalette2)
			{
				return false;
			}
			if (!hasPalette1)
			{
				return true;
			}
			uint colors = GetColorsUsed(dib1);
			if (colors != GetColorsUsed(dib2))
			{
				return false;
			}
			return CompareMemory((void*)pal1, (void*)pal2, sizeof(RGBQUAD) * colors);
		}

		private static unsafe bool CompareData(FIBITMAP dib1, FIBITMAP dib2)
		{
			uint width = GetWidth(dib1);
			if (width != GetWidth(dib2))
			{
				return false;
			}
			uint height = GetHeight(dib1);
			if (height != GetHeight(dib2))
			{
				return false;
			}
			uint bpp = GetBPP(dib1);
			if (bpp != GetBPP(dib2))
			{
				return false;
			}
			if (GetColorType(dib1) != GetColorType(dib2))
			{
				return false;
			}
			FREE_IMAGE_TYPE type = GetImageType(dib1);
			if (type != GetImageType(dib2))
			{
				return false;
			}

			if (type == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				switch (bpp)
				{
					case 32:
						for (int i = 0; i < height; i++)
						{
							RGBQUADARRAY array1 = new RGBQUADARRAY(dib1, i);
							RGBQUADARRAY array2 = new RGBQUADARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					case 24:
						for (int i = 0; i < height; i++)
						{
							RGBTRIPLEARRAY array1 = new RGBTRIPLEARRAY(dib1, i);
							RGBTRIPLEARRAY array2 = new RGBTRIPLEARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					case 16:
						for (int i = 0; i < height; i++)
						{
							FI16RGBARRAY array1 = new FI16RGBARRAY(dib1, i);
							FI16RGBARRAY array2 = new FI16RGBARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					case 8:
						for (int i = 0; i < height; i++)
						{
							FI8BITARRAY array1 = new FI8BITARRAY(dib1, i);
							FI8BITARRAY array2 = new FI8BITARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					case 4:
						for (int i = 0; i < height; i++)
						{
							FI4BITARRAY array1 = new FI4BITARRAY(dib1, i);
							FI4BITARRAY array2 = new FI4BITARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					case 1:
						for (int i = 0; i < height; i++)
						{
							FI1BITARRAY array1 = new FI1BITARRAY(dib1, i);
							FI1BITARRAY array2 = new FI1BITARRAY(dib2, i);
							if (array1 != array2)
							{
								return false;
							}
						}
						break;
					default:
						throw new NotSupportedException();
				}
			}
			else
			{
				IntPtr ptr1, ptr2;
				uint line = GetLine(dib1);
				for (int i = 0; i < height; i++)
				{
					ptr1 = GetScanLine(dib1, i);
					ptr2 = GetScanLine(dib2, i);
					if (!CompareMemory((void*)ptr1, (void*)ptr2, line))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool CompareMetadata(FIBITMAP dib1, FIBITMAP dib2)
		{
			MetadataTag tag1, tag2;

			foreach (FREE_IMAGE_MDMODEL metadataModel in FREE_IMAGE_MDMODELS)
			{
				if (GetMetadataCount(metadataModel, dib1) !=
					GetMetadataCount(metadataModel, dib2))
				{
					return false;
				}
				if (GetMetadataCount(metadataModel, dib1) == 0)
				{
					continue;
				}

				FIMETADATA mdHandle = FindFirstMetadata(metadataModel, dib1, out tag1);
				if (mdHandle.IsNull)
				{
					continue;
				}
				do
				{
					if ((!GetMetadata(metadataModel, dib2, tag1.Key, out tag2)) ||
						(tag1 != tag2))
					{
						FindCloseMetadata(mdHandle);
						return false;
					}
				}
				while (FindNextMetadata(mdHandle, out tag1));
				FindCloseMetadata(mdHandle);
			}

			return true;
		}

		/// <summary>
		/// Returns the bitmap's transparency table.
		/// The array is empty in case the bitmap has no transparency table.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>The bitmap's transparency table.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe byte[] GetTransparencyTableEx(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			uint count = GetTransparencyCount(dib);
			byte[] result = new byte[count];
			byte* ptr = (byte*)GetTransparencyTable(dib);
			fixed (byte* dst = result)
			{
				MoveMemory(dst, ptr, count);
			}
			return result;
		}

		/// <summary>
		/// Set the bitmap's transparency table. Only affects palletised bitmaps.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="table">The bitmap's new transparency table.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> or <paramref name="table"/> is null.</exception>
		public static void SetTransparencyTable(FIBITMAP dib, byte[] table)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			SetTransparencyTable_(dib, table, table.Length);
		}

		/// <summary>
		/// This function returns the number of unique colors actually used by the
		/// specified 1-, 4-, 8-, 16-, 24- or 32-bit image. This might be different from
		/// what function FreeImage_GetColorsUsed() returns, which actually returns the
		/// palette size for palletised images. Works for FIT_BITMAP type images only.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Returns the number of unique colors used by the image specified or
		/// zero, if the image type cannot be handled.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe int GetUniqueColors(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			int result = 0;

			if (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP)
			{
				BitArray bitArray;
				int uniquePalEnts;
				int hashcode;
				byte[] lut;
				int width = (int)GetWidth(dib);
				int height = (int)GetHeight(dib);

				switch (GetBPP(dib))
				{
					case 1:

						result = 1;
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							break;
						}

						if ((*(byte*)GetScanLine(dib, 0) & 0x80) == 0)
						{
							for (int y = 0; y < height; y++)
							{
								byte* scanline = (byte*)GetScanLine(dib, y);
								int mask = 0x80;
								for (int x = 0; x < width; x++)
								{
									if ((scanline[x / 8] & mask) > 0)
									{
										return 2;
									}
									mask = (mask == 0x1) ? 0x80 : (mask >> 1);
								}
							}
						}
						else
						{
							for (int y = 0; y < height; y++)
							{
								byte* scanline = (byte*)GetScanLine(dib, y);
								int mask = 0x80;
								for (int x = 0; x < width; x++)
								{
									if ((scanline[x / 8] & mask) == 0)
									{
										return 2;
									}
									mask = (mask == 0x1) ? 0x80 : (mask >> 1);
								}
							}
						}
						break;

					case 4:

						bitArray = new BitArray(0x10);
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							result = 1;
							break;
						}

						for (int y = 0; (y < height) && (result < uniquePalEnts); y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							bool top = true;
							for (int x = 0; (x < width) && (result < uniquePalEnts); x++)
							{
								if (top)
								{
									hashcode = lut[scanline[x / 2] >> 4];
								}
								else
								{
									hashcode = lut[scanline[x / 2] & 0xF];
								}
								top = !top;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 8:

						bitArray = new BitArray(0x100);
						lut = CreateShrunkenPaletteLUT(dib, out uniquePalEnts);
						if (uniquePalEnts == 1)
						{
							result = 1;
							break;
						}

						for (int y = 0; (y < height) && (result < uniquePalEnts); y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							for (int x = 0; (x < width) && (result < uniquePalEnts); x++)
							{
								hashcode = lut[scanline[x]];
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 16:

						bitArray = new BitArray(0x10000);

						for (int y = 0; y < height; y++)
						{
							short* scanline = (short*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline++)
							{
								hashcode = *scanline;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 24:

						bitArray = new BitArray(0x1000000);

						for (int y = 0; y < height; y++)
						{
							byte* scanline = (byte*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline += 3)
							{
								hashcode = *((int*)scanline) & 0x00FFFFFF;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;

					case 32:

						bitArray = new BitArray(0x1000000);

						for (int y = 0; y < height; y++)
						{
							int* scanline = (int*)GetScanLine(dib, y);
							for (int x = 0; x < width; x++, scanline++)
							{
								hashcode = *scanline & 0x00FFFFFF;
								if (!bitArray[hashcode])
								{
									bitArray[hashcode] = true;
									result++;
								}
							}
						}
						break;
				}
			}
			return result;
		}

		#endregion

		#region ICC profile functions

		/// <summary>
		/// Creates a new ICC-Profile for a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIICCPROFILE CreateICCProfileEx(FIBITMAP dib, byte[] data)
		{
			return new FIICCPROFILE(dib, data);
		}

		/// <summary>
		/// Creates a new ICC-Profile for a bitmap.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="data">The data of the new ICC-Profile.</param>
		/// <param name="size">The number of bytes of 'data' to use.</param>
		/// <returns>The new ICC-Profile of the bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIICCPROFILE CreateICCProfileEx(FIBITMAP dib, byte[] data, int size)
		{
			return new FIICCPROFILE(dib, data, size);
		}

		#endregion

		#region Conversion functions

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting to 'FICF_MONOCHROME_THRESHOLD'.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				threshold,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="ditherMethod">Dither algorithm when converting to 'FICF_MONOCHROME_DITHER'.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_DITHER ditherMethod)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				ditherMethod,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				false);
		}


		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_QUANTIZE quantizationMethod)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				quantizationMethod,
				false);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting to 'FICF_MONOCHROME_THRESHOLD'.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				threshold,
				FREE_IMAGE_DITHER.FID_FS,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="ditherMethod">Dither algorithm when converting to 'FICF_MONOCHROME_DITHER'.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_DITHER ditherMethod,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				ditherMethod,
				FREE_IMAGE_QUANTIZE.FIQ_WUQUANT,
				unloadSource);
		}


		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			FREE_IMAGE_QUANTIZE quantizationMethod,
			bool unloadSource)
		{
			return ConvertColorDepth(
				dib,
				conversion,
				128,
				FREE_IMAGE_DITHER.FID_FS,
				quantizationMethod,
				unloadSource);
		}

		/// <summary>
		/// Converts a bitmap from one color depth to another.
		/// If the conversion fails the original bitmap is returned.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="conversion">The desired output format.</param>
		/// <param name="threshold">Threshold value when converting to 'FICD_01_BPP_THRESHOLD'.</param>
		/// <param name="ditherMethod">Dither algorithm when converting to 'FICD_01_BPP_DITHER'.</param>
		/// <param name="quantizationMethod">The quantization algorithm for conversion to 8-bit color depth.</param>
		/// <param name="unloadSource">When true the structure will be unloaded on success.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		internal static FIBITMAP ConvertColorDepth(
			FIBITMAP dib,
			FREE_IMAGE_COLOR_DEPTH conversion,
			byte threshold,
			FREE_IMAGE_DITHER ditherMethod,
			FREE_IMAGE_QUANTIZE quantizationMethod,
			bool unloadSource)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FIBITMAP result = 0;
			FIBITMAP dibTemp = 0;
			uint bpp = GetBPP(dib);
			bool reorderPalette = ((conversion & FREE_IMAGE_COLOR_DEPTH.FICD_REORDER_PALETTE) > 0);
			bool forceGreyscale = ((conversion & FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE) > 0);

			switch (conversion & (FREE_IMAGE_COLOR_DEPTH)0xFF)
			{
				case FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD:

					if (bpp != 1)
					{
						result = Threshold(dib, threshold);
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
						(reorderPalette && isGreyscale))
						{
							result = Threshold(dib, threshold);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER:

					if (bpp != 1)
					{
						result = Dither(dib, ditherMethod);
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
						(reorderPalette && isGreyscale))
						{
							result = Dither(dib, ditherMethod);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP:

					if (bpp != 4)
					{
						// Special case when 1bpp and FIC_PALETTE
						if (forceGreyscale && (bpp == 1) && (GetColorType(dib) == FREE_IMAGE_COLOR_TYPE.FIC_PALETTE))
						{
							dibTemp = ConvertToGreyscale(dib);
							result = ConvertTo4Bits(dibTemp);
							Unload(dibTemp);
						}
						// All other cases are converted directly
						else
						{
							result = ConvertTo4Bits(dib);
						}
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) ||
							(reorderPalette && isGreyscale))
						{
							dibTemp = ConvertToGreyscale(dib);
							result = ConvertTo4Bits(dibTemp);
							Unload(dibTemp);
						}
					}

					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP:

					if (bpp != 8)
					{
						if (forceGreyscale)
						{
							result = ConvertToGreyscale(dib);
						}
						else
						{
							dibTemp = ConvertTo24Bits(dib);
							result = ColorQuantize(dibTemp, quantizationMethod);
							Unload(dibTemp);
						}
					}
					else
					{
						bool isGreyscale = IsGreyscaleImage(dib);
						if ((forceGreyscale && (!isGreyscale)) || (reorderPalette && isGreyscale))
						{
							result = ConvertToGreyscale(dib);
						}
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP_555:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo16Bits555(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 16 || GetRedMask(dib) != FI16_555_RED_MASK || GetGreenMask(dib) != FI16_555_GREEN_MASK || GetBlueMask(dib) != FI16_555_BLUE_MASK)
					{
						result = ConvertTo16Bits555(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo16Bits565(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 16 || GetRedMask(dib) != FI16_565_RED_MASK || GetGreenMask(dib) != FI16_565_GREEN_MASK || GetBlueMask(dib) != FI16_565_BLUE_MASK)
					{
						result = ConvertTo16Bits565(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo24Bits(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 24)
					{
						result = ConvertTo24Bits(dib);
					}
					break;

				case FREE_IMAGE_COLOR_DEPTH.FICD_32_BPP:

					if (forceGreyscale)
					{
						dibTemp = ConvertToGreyscale(dib);
						result = ConvertTo32Bits(dibTemp);
						Unload(dibTemp);
					}
					else if (bpp != 32)
					{
						result = ConvertTo32Bits(dib);
					}
					break;
			}

			if (result.IsNull)
			{
				return dib;
			}
			if (unloadSource)
			{
				Unload(dib);
			}

			return result;
		}

		#endregion

		#region Metadata

		/// <summary>
		/// Copies metadata from one bitmap to another.
		/// </summary>
		/// <param name="src">Source bitmap containing the metadata.</param>
		/// <param name="dst">Bitmap to copy the metadata to.</param>
		/// <param name="flags">Flags to switch different copy modes.</param>
		/// <returns>Returns -1 on failure else the number of copied tags.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="src"/> or <paramref name="dst"/> is null.</exception>
		public static int CopyMetadata(FIBITMAP src, FIBITMAP dst, FREE_IMAGE_METADATA_COPY flags)
		{
			if (src.IsNull)
			{
				throw new ArgumentNullException("src");
			}
			if (dst.IsNull)
			{
				throw new ArgumentNullException("dst");
			}

			FITAG tag = 0, tag2 = 0;
			int copied = 0;

			// Clear all existing metadata
			if ((flags & FREE_IMAGE_METADATA_COPY.CLEAR_EXISTING) > 0)
			{
				foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
				{
					if (!SetMetadata(model, dst, null, tag))
					{
						return -1;
					}
				}
			}

			bool keep = !((flags & FREE_IMAGE_METADATA_COPY.REPLACE_EXISTING) > 0);

			foreach (FREE_IMAGE_MDMODEL model in FREE_IMAGE_MDMODELS)
			{
				FIMETADATA mData = FindFirstMetadata(model, src, out tag);
				if (mData.IsNull) continue;
				do
				{
					string key = GetTagKey(tag);
					if (!(keep && GetMetadata(model, dst, key, out tag2)))
					{
						if (SetMetadata(model, dst, key, tag))
						{
							copied++;
						}
					}
				}
				while (FindNextMetadata(mData, out tag));
				FindCloseMetadata(mData);
			}

			return copied;
		}

		/// <summary>
		/// Returns the comment of a JPEG, PNG or GIF image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <returns>Comment of the image, or null in case no comment exists.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static string GetImageComment(FIBITMAP dib)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			string result = null;
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FITAG tag;
			if (GetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", out tag))
			{
				MetadataTag metadataTag = new MetadataTag(tag, FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
				result = metadataTag.Value as string;
			}
			return result;
		}

		/// <summary>
		/// Sets the comment of a JPEG, PNG or GIF image.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="comment">New comment of the image. Use null to remove the comment.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static bool SetImageComment(FIBITMAP dib, string comment)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			bool result;
			if (comment != null)
			{
				FITAG tag = CreateTag();
				MetadataTag metadataTag = new MetadataTag(tag, FREE_IMAGE_MDMODEL.FIMD_COMMENTS);
				metadataTag.Value = comment;
				result = SetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", tag);
				DeleteTag(tag);
			}
			else
			{
				result = SetMetadata(FREE_IMAGE_MDMODEL.FIMD_COMMENTS, dib, "Comment", 0);
			}
			return result;
		}

		/// <summary>
		/// Retrieve a metadata attached to a dib.
		/// </summary>
		/// <param name="model">The metadata model to look for.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The metadata field name.</param>
		/// <param name="tag">A MetadataTag structure returned by the function.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static bool GetMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			string key,
			out MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FITAG _tag;
			bool result;
			if (GetMetadata(model, dib, key, out _tag))
			{
				tag = new MetadataTag(_tag, model);
				result = true;
			}
			else
			{
				tag = null;
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Attach a new metadata tag to a dib.
		/// </summary>
		/// <param name="model">The metadata model used to store the tag.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="key">The tag field name.</param>
		/// <param name="tag">The metadata tag to be attached.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static bool SetMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			string key,
			MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			return SetMetadata(model, dib, key, tag.tag);
		}

		/// <summary>
		/// Provides information about the first instance of a tag that matches the metadata model.
		/// </summary>
		/// <param name="model">The model to match.</param>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Unique search handle that can be used to call FindNextMetadata or FindCloseMetadata.
		/// Null if the metadata model does not exist.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static FIMETADATA FindFirstMetadata(
			FREE_IMAGE_MDMODEL model,
			FIBITMAP dib,
			out MetadataTag tag)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			FITAG _tag;
			FIMETADATA result = FindFirstMetadata(model, dib, out _tag);
			if (result.IsNull)
			{
				tag = null;
				return result;
			}
			tag = new MetadataTag(_tag, model);
			if (metaDataSearchHandler.ContainsKey(result))
			{
				metaDataSearchHandler[result] = model;
			}
			else
			{
				metaDataSearchHandler.Add(result, model);
			}
			return result;
		}

		/// <summary>
		/// Find the next tag, if any, that matches the metadata model argument in a previous call
		/// to FindFirstMetadata, and then alters the tag object contents accordingly.
		/// </summary>
		/// <param name="mdhandle">Unique search handle provided by FindFirstMetadata.</param>
		/// <param name="tag">Tag that matches the metadata model.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		public static bool FindNextMetadata(FIMETADATA mdhandle, out MetadataTag tag)
		{
			FITAG _tag;
			bool result;
			if (FindNextMetadata(mdhandle, out _tag))
			{
				tag = new MetadataTag(_tag, metaDataSearchHandler[mdhandle]);
				result = true;
			}
			else
			{
				tag = null;
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Closes the specified metadata search handle and releases associated resources.
		/// </summary>
		/// <param name="mdhandle">The handle to close.</param>
		public static void FindCloseMetadata(FIMETADATA mdhandle)
		{
			if (metaDataSearchHandler.ContainsKey(mdhandle))
			{
				metaDataSearchHandler.Remove(mdhandle);
			}
			FindCloseMetadata_(mdhandle);
		}

		/// <summary>
		/// This dictionary links FIMETADATA handles and FREE_IMAGE_MDMODEL models.
		/// </summary>
		private static Dictionary<FIMETADATA, FREE_IMAGE_MDMODEL> metaDataSearchHandler
			= new Dictionary<FIMETADATA, FREE_IMAGE_MDMODEL>(1);

		#endregion

		#region Rotation and flipping

		/// <summary>
		/// This function rotates a 4-bit color image.
		/// Allowed values for <paramref name="angle"/> are 90, 180 and 270.
		/// In case <paramref name="angle"/> is 0 or 360 a clone is returned.
		/// Null is returned for other values or in case the rotation fails.
		/// </summary>
		/// <param name="dib">Handle to a FreeImage bitmap.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>Handle to a FreeImage bitmap.</returns>
		/// <remarks>
		/// This function is kind of temporary due to FreeImage's lack of
		/// rotating 4-bit images. It's particularly used by FreeImageBitmap's
		/// method RotateFlip. This function will be removed as soon as FreeImage
		/// supports rotating 4-bit images.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="dib"/> is null.</exception>
		public static unsafe FIBITMAP Rotate4bit(FIBITMAP dib, double angle)
		{
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}

			FIBITMAP result = 0;
			int ang = (int)angle;

			if ((GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP) &&
				(GetBPP(dib) == 4) &&
				((ang % 90) == 0))
			{
				int width, height, xOrg, yOrg;
				FI4BITARRAY[] src, dst;
				width = (int)GetWidth(dib);
				height = (int)GetHeight(dib);
				byte index = 0;
				switch (ang)
				{
					case 90:
						result = Allocate(height, width, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);
						for (int y = 0; y < width; y++)
						{
							yOrg = height - 1;
							for (int x = 0; x < height; x++, yOrg--)
							{
								index = src[yOrg].GetIndexUnsafe(y);
								dst[y].SetIndexUnsafe(x, index);
							}
						}
						break;
					case 180:
						result = Allocate(width, height, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);

						yOrg = height - 1;
						for (int y = 0; y < height; y++, yOrg--)
						{
							xOrg = width - 1;
							for (int x = 0; x < width; x++, xOrg--)
							{
								index = src[yOrg].GetIndexUnsafe(xOrg);
								dst[y].SetIndexUnsafe(x, index);
							}
						}
						break;
					case 270:
						result = Allocate(height, width, 4, 0, 0, 0);
						if (result.IsNull)
						{
							break;
						}
						CopyPalette(dib, result);
						src = Get04BitScanlines(dib);
						dst = Get04BitScanlines(result);
						xOrg = width - 1;
						for (int y = 0; y < width; y++, xOrg--)
						{
							for (int x = 0; x < height; x++)
							{
								index = src[x].GetIndexUnsafe(xOrg);
								dst[y].SetIndexUnsafe(x, index);
							}
						}
						break;
					case 0:
					case 360:
						result = Clone(dib);
						break;
				}
			}
			return result;
		}

		#endregion

		#region Wrapper functions

		/// <summary>
		/// Returns the next higher possible color depth.
		/// </summary>
		/// <param name="bpp">Color depth to increase.</param>
		/// <returns>The next higher color depth or 0 if there is no valid color depth.</returns>
		internal static int GetNextColorDepth(int bpp)
		{
			int result = 0;
			switch (bpp)
			{
				case 1:
					result = 4;
					break;
				case 4:
					result = 8;
					break;
				case 8:
					result = 16;
					break;
				case 16:
					result = 24;
					break;
				case 24:
					result = 32;
					break;
			}
			return result;
		}

		/// <summary>
		/// Returns the next lower possible color depth.
		/// </summary>
		/// <param name="bpp">Color depth to decrease.</param>
		/// <returns>The next lower color depth or 0 if there is no valid color depth.</returns>
		internal static int GetPrevousColorDepth(int bpp)
		{
			int result = 0;
			switch (bpp)
			{
				case 32:
					result = 24;
					break;
				case 24:
					result = 16;
					break;
				case 16:
					result = 8;
					break;
				case 8:
					result = 4;
					break;
				case 4:
					result = 1;
					break;
			}
			return result;
		}

		/// <summary>
		/// Reads a null-terminated c-string.
		/// </summary>
		/// <param name="ptr">Pointer to the first char of the string.</param>
		/// <returns>The converted string.</returns>
		internal static unsafe string PtrToStr(byte* ptr)
		{
			string result = null;
			if (ptr != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				while (*ptr != 0)
				{
					sb.Append((char)*(ptr++));
				}
				result = sb.ToString();
			}
			return result;
		}

		internal static unsafe byte[] CreateShrunkenPaletteLUT(FIBITMAP dib, out int uniqueColors)
		{
			byte[] result = null;
			uniqueColors = 0;

			if ((!dib.IsNull) && (GetImageType(dib) == FREE_IMAGE_TYPE.FIT_BITMAP) && (GetBPP(dib) <= 8))
			{
				int size = (int)GetColorsUsed(dib);
				List<RGBQUAD> newPalette = new List<RGBQUAD>(size);
				List<byte> lut = new List<byte>(size);
				RGBQUAD* palette = (RGBQUAD*)GetPalette(dib);
				RGBQUAD color;
				int index;

				for (int i = 0; i < size; i++)
				{
					color = palette[i];
					color.rgbReserved = 255; // ignore alpha

					index = newPalette.IndexOf(color);
					if (index < 0)
					{
						newPalette.Add(color);
						lut.Add((byte)(newPalette.Count - 1));
					}
					else
					{
						lut.Add((byte)index);
					}
				}
				result = lut.ToArray();
				uniqueColors = newPalette.Count;
			}
			return result;
		}

		internal static PropertyItem CreatePropertyItem()
		{
			PropertyItem result = null;
			try
			{
				result = (PropertyItem)PropertyItemConstructor.Invoke(null);
			}
			catch
			{
			}
			return result;
		}

		private static unsafe void CopyPalette(FIBITMAP src, FIBITMAP dst)
		{
			RGBQUAD* orgPal = (RGBQUAD*)GetPalette(src);
			RGBQUAD* newPal = (RGBQUAD*)GetPalette(dst);
			uint size = (uint)(sizeof(RGBQUAD) * GetColorsUsed(src));
			MoveMemory(newPal, orgPal, size);
		}

		private static unsafe FI4BITARRAY[] Get04BitScanlines(FIBITMAP dib)
		{
			int height = (int)GetHeight(dib);
			uint width = GetWidth(dib);
			FI4BITARRAY[] array = new FI4BITARRAY[height];
			for (int i = 0; i < height; i++)
			{
				array[i] = new FI4BITARRAY(GetScanLine(dib, i), width);
			}
			return array;
		}

		/// <summary>
		/// Changes a bitmaps color depth.
		/// Used by SaveEx and SaveToStream
		/// </summary>
		private static FIBITMAP PrepareBitmapColorDepth(FIBITMAP dibToSave, FREE_IMAGE_FORMAT format, FREE_IMAGE_COLOR_DEPTH colorDepth)
		{
			int bpp = (int)GetBPP(dibToSave);
			int targetBpp = (int)(colorDepth & FREE_IMAGE_COLOR_DEPTH.FICD_COLOR_MASK);

			if (colorDepth != FREE_IMAGE_COLOR_DEPTH.FICD_AUTO)
			{
				// A fix colordepth was chosen
				if (FIFSupportsExportBPP(format, targetBpp))
				{
					dibToSave = ConvertColorDepth(dibToSave, colorDepth, false);
				}
				else
				{
					throw new ArgumentException("FreeImage\n\nFreeImage Library plugin " +
						GetFormatFromFIF(format) + " is unable to write images with a color depth of " +
						targetBpp + " bpp.");
				}
			}
			else
			{
				// Auto selection was chosen
				if (!FIFSupportsExportBPP(format, bpp))
				{
					// The color depth is not supported
					int bppUpper = bpp;
					int bppLower = bpp;
					// Check from the bitmaps current color depth in both directions
					do
					{
						bppUpper = GetNextColorDepth(bppUpper);
						if (FIFSupportsExportBPP(format, bppUpper))
						{
							dibToSave = ConvertColorDepth(dibToSave, (FREE_IMAGE_COLOR_DEPTH)bppUpper, false);
							break;
						}
						bppLower = GetPrevousColorDepth(bppLower);
						if (FIFSupportsExportBPP(format, bppLower))
						{
							dibToSave = ConvertColorDepth(dibToSave, (FREE_IMAGE_COLOR_DEPTH)bppLower, false);
							break;
						}
					} while (!((bppLower == 0) && (bppUpper == 0)));
				}
			}
			return dibToSave;
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(void* buf1, void* buf2, uint length)
		{
			return (length == RtlCompareMemory(buf1, buf2, length));
		}

		/// <summary>
		/// Compares blocks of memory.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="length">Specifies the number of bytes to be compared.</param>
		/// <returns>If all bytes compare as equal, true is returned.</returns>
		public static unsafe bool CompareMemory(void* buf1, void* buf2, long length)
		{
			checked
			{
				uint len = (uint)length;
				return (len == RtlCompareMemory(buf1, buf2, len));
			}
		}

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		public static unsafe void MoveMemory(void* dst, void* src, long size)
		{
			checked
			{
				uint len = (uint)size;
				MoveMemory(dst, src, len);
			}
		}

		#endregion

		#region Dll-Imports

		/// <summary>
		/// Retrieves a handle to a display device context (DC) for the client area of a specified window
		/// or for the entire screen. You can use the returned handle in subsequent GDI functions to draw in the DC.
		/// </summary>
		/// <param name="hWnd">Handle to the window whose DC is to be retrieved.
		/// If this value is IntPtr.Zero, GetDC retrieves the DC for the entire screen. </param>
		/// <returns>If the function succeeds, the return value is a handle to the DC for the specified window's client area.
		/// If the function fails, the return value is NULL.</returns>
		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		/// <summary>
		/// Releases a device context (DC), freeing it for use by other applications.
		/// The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs.
		/// It has no effect on class or private DCs.
		/// </summary>
		/// <param name="hWnd">Handle to the window whose DC is to be released.</param>
		/// <param name="hDC">Handle to the DC to be released.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		/// <summary>
		/// Creates a DIB that applications can write to directly.
		/// The function gives you a pointer to the location of the bitmap bit values.
		/// You can supply a handle to a file-mapping object that the function will use to create the bitmap,
		/// or you can let the system allocate the memory for the bitmap.
		/// </summary>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="pbmi">Pointer to a BITMAPINFO structure that specifies various attributes of the DIB,
		/// including the bitmap dimensions and colors.</param>
		/// <param name="iUsage">Specifies the type of data contained in the bmiColors array member of the BITMAPINFO structure
		/// pointed to by pbmi (either logical palette indexes or literal RGB values).</param>
		/// <param name="ppvBits">Pointer to a variable that receives a pointer to the location of the DIB bit values.</param>
		/// <param name="hSection">Handle to a file-mapping object that the function will use to create the DIB.
		/// This parameter can be NULL.</param>
		/// <param name="dwOffset">Specifies the offset from the beginning of the file-mapping object referenced by hSection
		/// where storage for the bitmap bit values is to begin. This value is ignored if hSection is NULL.</param>
		/// <returns>If the function succeeds, the return value is a handle to the newly created DIB,
		/// and *ppvBits points to the bitmap bit values. If the function fails, the return value is NULL, and *ppvBits is NULL.</returns>
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBSection(
			IntPtr hdc,
			[In] IntPtr pbmi,
			uint iUsage,
			out IntPtr ppvBits,
			IntPtr hSection,
			uint dwOffset);

		/// <summary>
		/// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
		/// After the object is deleted, the specified handle is no longer valid.
		/// </summary>
		/// <param name="hObject">Handle to a logical pen, brush, font, bitmap, region, or palette.</param>
		/// <returns>Returns true on success, false on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		/// <summary>
		/// Creates a compatible bitmap (DDB) from a DIB and, optionally, sets the bitmap bits.
		/// </summary>
		/// <param name="hdc">Handle to a device context.</param>
		/// <param name="lpbmih">Pointer to a bitmap information header structure.</param>
		/// <param name="fdwInit">Specifies how the system initializes the bitmap bits - (use 4).</param>
		/// <param name="lpbInit">Pointer to an array of bytes containing the initial bitmap data.</param>
		/// <param name="lpbmi">Pointer to a BITMAPINFO structure that describes the dimensions
		/// and color format of the array pointed to by the lpbInit parameter.</param>
		/// <param name="fuUsage">Specifies whether the bmiColors member of the BITMAPINFO structure
		/// was initialized - (use 0).</param>
		/// <returns>Handle to a DIB or null on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBitmap(
			IntPtr hdc,
			IntPtr lpbmih,
			uint fdwInit,
			IntPtr lpbInit,
			IntPtr lpbmi,
			uint fuUsage);

		/// <summary>
		/// Retrieves information for the specified graphics object.
		/// </summary>
		/// <param name="hgdiobj">Handle to the graphics object of interest.</param>
		/// <param name="cbBuffer">Specifies the number of bytes of information to
		/// be written to the buffer.</param>
		/// <param name="lpvObject">Pointer to a buffer that receives the information
		/// about the specified graphics object.</param>
		/// <returns>0 on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

		/// <summary>
		/// Retrieves the bits of the specified compatible bitmap and copies them into a buffer
		/// as a DIB using the specified format.
		/// </summary>
		/// <param name="hdc">Handle to the device context.</param>
		/// <param name="hbmp">Handle to the bitmap. This must be a compatible bitmap (DDB).</param>
		/// <param name="uStartScan">Specifies the first scan line to retrieve.</param>
		/// <param name="cScanLines">Specifies the number of scan lines to retrieve.</param>
		/// <param name="lpvBits">Pointer to a buffer to receive the bitmap data.</param>
		/// <param name="lpbmi">Pointer to a BITMAPINFO structure that specifies the desired
		/// format for the DIB data.</param>
		/// <param name="uUsage">Specifies the format of the bmiColors member of the
		/// BITMAPINFO structure - (use 0).</param>
		/// <returns>0 on failure.</returns>
		[DllImport("gdi32.dll")]
		private static extern unsafe int GetDIBits(
			IntPtr hdc,
			IntPtr hbmp,
			uint uStartScan,
			uint cScanLines,
			IntPtr lpvBits,
			IntPtr lpbmi,
			uint uUsage);

		/// <summary>
		/// Moves a block of memory from one location to another.
		/// </summary>
		/// <param name="dst">Pointer to the starting address of the move destination.</param>
		/// <param name="src">Pointer to the starting address of the block of memory to be moved.</param>
		/// <param name="size">Size of the block of memory to move, in bytes.</param>
		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		public static unsafe extern void MoveMemory(void* dst, void* src, uint size);

		/// <summary>
		/// The RtlCompareMemory routine compares blocks of memory
		/// and returns the number of bytes that are equivalent.
		/// </summary>
		/// <param name="buf1">Pointer to a block of memory to compare.</param>
		/// <param name="buf2">Pointer to a block of memory to compare.</param>
		/// <param name="count">Specifies the number of bytes to be compared.</param>
		/// <returns>RtlCompareMemory returns the number of bytes that compare as equal.
		/// If all bytes compare as equal, the input Length is returned.</returns>
		[DllImport("ntdll.dll", EntryPoint = "RtlCompareMemory", SetLastError = false)]
		private static unsafe extern uint RtlCompareMemory(void* buf1, void* buf2, uint count);

		#endregion
	}
}
