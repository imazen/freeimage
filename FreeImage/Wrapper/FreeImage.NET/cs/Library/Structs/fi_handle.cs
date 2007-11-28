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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
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
		public fi_handle(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
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
}