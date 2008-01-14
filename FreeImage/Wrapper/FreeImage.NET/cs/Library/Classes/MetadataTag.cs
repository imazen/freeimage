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
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace FreeImageAPI
{
	/// <summary>
	/// Manages metadata objects and operations.
	/// </summary>
	public class MetadataTag : IComparable, IComparable<MetadataTag>, ICloneable, IEquatable<MetadataTag>, IDisposable
	{
		internal protected FITAG tag;
		internal protected FREE_IMAGE_MDMODEL model;
		protected bool disposed = false;
		protected bool selfCreated;
		protected static readonly Dictionary<FREE_IMAGE_MDTYPE, Type> idList;
		protected static readonly Dictionary<Type, FREE_IMAGE_MDTYPE> typeList;

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

		static MetadataTag()
		{
			idList = new Dictionary<FREE_IMAGE_MDTYPE, Type>();
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_BYTE, typeof(byte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SHORT, typeof(ushort));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_LONG, typeof(uint));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_RATIONAL, typeof(FIURational));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SBYTE, typeof(sbyte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_UNDEFINED, typeof(byte));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SSHORT, typeof(short));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SLONG, typeof(int));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_SRATIONAL, typeof(FIRational));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_FLOAT, typeof(float));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_DOUBLE, typeof(double));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_IFD, typeof(uint));
			idList.Add(FREE_IMAGE_MDTYPE.FIDT_PALETTE, typeof(RGBQUAD));

			typeList = new Dictionary<Type, FREE_IMAGE_MDTYPE>();
			typeList.Add(typeof(ushort), FREE_IMAGE_MDTYPE.FIDT_SHORT);
			typeList.Add(typeof(ushort[]), FREE_IMAGE_MDTYPE.FIDT_SHORT);
			typeList.Add(typeof(string), FREE_IMAGE_MDTYPE.FIDT_ASCII);
			typeList.Add(typeof(uint), FREE_IMAGE_MDTYPE.FIDT_LONG);
			typeList.Add(typeof(uint[]), FREE_IMAGE_MDTYPE.FIDT_LONG);
			typeList.Add(typeof(FIURational), FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
			typeList.Add(typeof(FIURational[]), FREE_IMAGE_MDTYPE.FIDT_RATIONAL);
			typeList.Add(typeof(sbyte), FREE_IMAGE_MDTYPE.FIDT_SBYTE);
			typeList.Add(typeof(sbyte[]), FREE_IMAGE_MDTYPE.FIDT_SBYTE);
			typeList.Add(typeof(byte), FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
			typeList.Add(typeof(byte[]), FREE_IMAGE_MDTYPE.FIDT_UNDEFINED);
			typeList.Add(typeof(short), FREE_IMAGE_MDTYPE.FIDT_SSHORT);
			typeList.Add(typeof(short[]), FREE_IMAGE_MDTYPE.FIDT_SSHORT);
			typeList.Add(typeof(int), FREE_IMAGE_MDTYPE.FIDT_SLONG);
			typeList.Add(typeof(int[]), FREE_IMAGE_MDTYPE.FIDT_SLONG);
			typeList.Add(typeof(FIRational), FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
			typeList.Add(typeof(FIRational[]), FREE_IMAGE_MDTYPE.FIDT_SRATIONAL);
			typeList.Add(typeof(float), FREE_IMAGE_MDTYPE.FIDT_FLOAT);
			typeList.Add(typeof(float[]), FREE_IMAGE_MDTYPE.FIDT_FLOAT);
			typeList.Add(typeof(double), FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
			typeList.Add(typeof(double[]), FREE_IMAGE_MDTYPE.FIDT_DOUBLE);
			typeList.Add(typeof(RGBQUAD), FREE_IMAGE_MDTYPE.FIDT_PALETTE);
			typeList.Add(typeof(RGBQUAD[]), FREE_IMAGE_MDTYPE.FIDT_PALETTE);
		}

		~MetadataTag()
		{
			Dispose();
		}

		public static bool operator ==(MetadataTag value1, MetadataTag value2)
		{
			// Check whether both are null
			if (Object.ReferenceEquals(value1, null) && Object.ReferenceEquals(value2, null))
			{
				return true;
			}
			// Check whether only one is null
			if (Object.ReferenceEquals(value1, null) || Object.ReferenceEquals(value2, null))
			{
				return false;
			}
			// Check all properties
			if ((value1.Key != value2.Key) ||
				(value1.ID != value2.ID) ||
				(value1.Description != value2.Description) ||
				(value1.Count != value2.Count) ||
				(value1.Length != value2.Length) ||
				(value1.Model != value2.Model) ||
				(value1.Type != value2.Type))
			{
				return false;
			}
			if (value1.Length == 0)
			{
				return true;
			}
			IntPtr ptr1 = FreeImage.GetTagValue(value1.tag);
			IntPtr ptr2 = FreeImage.GetTagValue(value2.tag);
			return FreeImage.CompareMemory(ptr1, ptr2, value1.Length);
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
						{
							return model;
						}
					}
					while (FreeImage.FindNextMetadata(mData, out value));
				}
				finally
				{
					if (!mData.IsNull)
					{
						FreeImage.FindCloseMetadata(mData);
					}
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
			{
				value[i] = ptr[i];
			}
			return value;
		}

		/// <summary>
		/// Gets or sets the value of the metadata.
		/// <para> In case value is of byte or byte[], FREE_IMAGE_MDTYPE.FIDT_UNDEFINED is assumed.</para>
		/// <para> In case value is of uint or uint[], FREE_IMAGE_MDTYPE.FIDT_LONG is assumed.</para>
		/// </summary>
		public unsafe object Value
		{
			get
			{
				CheckDisposed();
				int cnt = (int)Count;

				if (Type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
				{
					byte* value = (byte*)FreeImage.GetTagValue(tag);
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < cnt; i++)
					{
						sb.Append(Convert.ToChar(value[i]));
					}
					return sb.ToString();
				}
				else if (Type == FREE_IMAGE_MDTYPE.FIDT_NOTYPE)
				{
					return null;
				}

				Array array = Array.CreateInstance(idList[Type], Count);
				void* src = (void*)FreeImage.GetTagValue(tag);
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				void* dst = (void*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
				FreeImage.MoveMemory(dst, src, Length);
				handle.Free();
				return array;
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
			if (!typeList.ContainsKey(type))
			{
				throw new NotSupportedException();
			}
			return SetValue(value, typeList[type]);
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
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			byte[] data = null;

			if (type == FREE_IMAGE_MDTYPE.FIDT_ASCII)
			{
				string tempValue = value as string;
				if (tempValue == null)
				{
					throw new ArgumentException("value");
				}
				Type = type;
				Count = (uint)(tempValue.Length + 1);
				Length = (uint)((tempValue.Length * sizeof(byte)) + 1);
				data = new byte[Length + 1];

				for (int i = 0; i < tempValue.Length; i++)
				{
					data[i] = (byte)tempValue[i];
				}
				data[data.Length - 1] = 0;
			}
			else if (type == FREE_IMAGE_MDTYPE.FIDT_NOTYPE)
			{
				throw new NotSupportedException();
			}
			else
			{
				Array array = value as Array;
				if (array == null)
				{
					throw new ArgumentException("value");
				}
				Type = type;
				Count = (uint)array.Length;
				Length = (uint)(array.Length * Marshal.SizeOf(idList[type]));
				data = new byte[Length];
				GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				void* src = (void*)Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
				fixed (byte* dst = data)
				{
					FreeImage.MoveMemory(dst, src, Length);
				}
				handle.Free();
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
			if (dib.IsNull)
			{
				throw new ArgumentNullException("dib");
			}
			if (Key == null)
			{
				throw new ArgumentNullException("Key");
			}
			if (!selfCreated)
			{
				tag = FreeImage.CloneTag(tag);
				if (tag.IsNull)
				{
					throw new Exception();
				}
				selfCreated = true;
			}
			if (!FreeImage.SetMetadata(Model, dib, Key, tag))
			{
				return false;
			}
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
			{
				data[i] = ptr[i];
			}
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
			return (this.tag == other.tag) && (this.model == other.model);
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
			return tag.CompareTo(other.tag);
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
			if (disposed)
			{
				throw new ObjectDisposedException("The object has already been disposed.");
			}
		}
	}
}