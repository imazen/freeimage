using System;
using System.Runtime.InteropServices;

namespace FreeImageAPI
{
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
}
