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
}