using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FreeImageAPI
{
	/// <summary>
	/// Class handling non-bitmap related functions.
	/// </summary>
	public static class FreeImageEngine
	{
		#region Callback

		// Callback delegate
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static OutputMessageFunction outputMessageFunction;
		// Handle to pin the functions address
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static GCHandle outputMessageHandle;

		static FreeImageEngine()
		{
			// Check if FreeImage.dll is present and cancel setting the callbackfuntion if not
			if (!IsAvailable)
			{
				return;
			}
			// Create a delegate (function pointer) to 'OnMessage'
			outputMessageFunction = new OutputMessageFunction(OnMessage);
			// Pin the object so the garbage collector does not move it around in memory
			outputMessageHandle = GCHandle.Alloc(outputMessageFunction, GCHandleType.Normal);
			// Set the callback
			FreeImage.SetOutputMessage(outputMessageFunction);
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
		public static event OutputMessageFunction Message;

		#endregion

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
}