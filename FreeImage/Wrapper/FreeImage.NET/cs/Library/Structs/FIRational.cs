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
		/// Throws if 'value' cannot be converted into a fraction
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
}