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
}