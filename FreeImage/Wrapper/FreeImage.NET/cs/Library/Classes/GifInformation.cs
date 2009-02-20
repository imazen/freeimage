using System;
using System.Diagnostics;
using System.Drawing;

namespace FreeImageAPI.Metadata
{
	/// <summary>
	/// Provides additional information specific for GIF files. This class cannot be inherited.
	/// </summary>
	public sealed class GifInformation
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly FreeImageBitmap bitmap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MDM_ANIMATION metadata;

		/// <summary>
		/// Initializes a new instance of the <see cref="GifInformation"/> class
		/// with the specified <see cref="FreeImageBitmap"/>.
		/// </summary>
		/// <param name="bitmap">A reference to a <see cref="FreeImageBitmap"/> instance.</param>
		public GifInformation(FreeImageBitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			this.bitmap = bitmap;
			this.metadata = new MDM_ANIMATION(bitmap.Dib);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MDM_ANIMATION Metadata
		{
			get
			{
				if (bitmap.IsDisposed)
					throw new ObjectDisposedException("The underlaying bitmap has is disposed.");
				return metadata;
			}
		}

		/// <summary>
		/// Gets or sets the width of the entire canvas area, that each page is displayed in.
		/// </summary>
		public ushort? LogicalWidth
		{
			get
			{
				ushort? result = null;
				MetadataTag mdtag = Metadata.GetTag("LogicalWidth");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((ushort[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("LogicalWidth");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_SHORT;
						mdtag.Key = "LogicalWidth";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "LogicalWidth", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "LogicalWidth", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets the height of the entire canvas area, that each page is displayed in.
		/// </summary>
		public ushort? LogicalHeight
		{
			get
			{
				ushort? result = null;
				MetadataTag mdtag = Metadata.GetTag("LogicalHeight");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((ushort[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("LogicalHeight");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_SHORT;
						mdtag.Key = "LogicalHeight";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "LogicalHeight", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "LogicalHeight", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets the global palette of the GIF image.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="value"/> is null.
		/// </exception>
		public Palette GlobalPalette
		{
			get
			{
				MetadataTag mdtag = Metadata.GetTag("GlobalPalette");
				return mdtag == null ? null : new Palette(mdtag);
			}
			set
			{
				if (value == null)
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "GlobalPalette", FITAG.Zero);
				}
				else
				{
					RGBQUAD[] data = value.AsArray;
					MetadataTag mdtag = Metadata.GetTag("GlobalPalette");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_PALETTE;
						mdtag.Key = "GlobalPalette";
						mdtag.Value = data;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "GlobalPalette", mdtag.tag);
					}
					else
					{
						mdtag.Value = data;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of replays for the animation.
		/// Use 0 (zero) to specify an infinte number of replays.
		/// </summary>
		public uint? LoopCount
		{
			get
			{
				uint? result = null;
				MetadataTag mdtag = Metadata.GetTag("Loop");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((uint[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("Loop");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_LONG;
						mdtag.Key = "Loop";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "Loop", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "Loop", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets the horizontal offset within the logical canvas area, this frame is to be displayed at.
		/// </summary>
		public ushort? FrameLeft
		{
			get
			{
				ushort? result = null;
				MetadataTag mdtag = Metadata.GetTag("FrameLeft");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((ushort[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("FrameLeft");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_SHORT;
						mdtag.Key = "FrameLeft";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameLeft", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameLeft", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets the vertical offset within the logical canvas area, this frame is to be displayed at.
		/// </summary>
		public ushort? FrameTop
		{
			get
			{
				ushort? result = null;
				MetadataTag mdtag = Metadata.GetTag("FrameTop");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((ushort[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("FrameTop");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_SHORT;
						mdtag.Key = "FrameTop";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameTop", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameTop", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this frame uses the
		/// GIF image's global palette. If set to <b>false</b>, this
		/// frame uses its local palette.
		/// </summary>
		public bool? UseGlobalPalette
		{
			get
			{
				bool? result = null;
				MetadataTag mdtag = Metadata.GetTag("NoLocalPalette");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((byte[])mdtag.Value)[0] != 0;
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("NoLocalPalette");
					byte data = (value.Value) ? (byte)1 : (byte)0;
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_BYTE;
						mdtag.Key = "NoLocalPalette";
						mdtag.Value = data;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "NoLocalPalette", mdtag.tag);
					}
					else
					{
						mdtag.Value = data;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "NoLocalPalette", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the image is interlaced.
		/// </summary>
		public bool? Interlaced
		{
			get
			{
				bool? result = null;
				MetadataTag mdtag = Metadata.GetTag("Interlaced");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((byte[])mdtag.Value)[0] != 0;
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("Interlaced");
					byte data = (value.Value) ? (byte)1 : (byte)0;
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_BYTE;
						mdtag.Key = "Interlaced";
						mdtag.Value = data;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "Interlaced", mdtag.tag);
					}
					else
					{
						mdtag.Value = data;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "Interlaced", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets the amout of time in milliseconds this frame is to be displayed.
		/// </summary>
		public uint? FrameTime
		{
			get
			{
				uint? result = null;
				MetadataTag mdtag = Metadata.GetTag("FrameTime");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = ((uint[])mdtag.Value)[0];
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("FrameTime");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_LONG;
						mdtag.Key = "FrameTime";
						mdtag.Value = value.Value;
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameTime", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "FrameTime", FITAG.Zero);
				}
			}
		}

		/// <summary>
		/// Gets or sets this frame's disposal method. Generally, this method defines, how to
		/// remove or replace a frame when the next frame has to be drawn.<para/>
		/// </summary>
		public DisposalMethodType? DisposalMethod
		{
			get
			{
				DisposalMethodType? result = null;
				MetadataTag mdtag = Metadata.GetTag("DisposalMethod");
				if ((mdtag != null) && (mdtag.Count == 1))
				{
					result = (DisposalMethodType)(((byte[])mdtag.Value)[0]);
				}
				return result;
			}
			set
			{
				if (value.HasValue)
				{
					MetadataTag mdtag = Metadata.GetTag("DisposalMethod");
					if (mdtag == null)
					{
						mdtag = new MetadataTag(FREE_IMAGE_MDMODEL.FIMD_ANIMATION);
						mdtag.Type = FREE_IMAGE_MDTYPE.FIDT_BYTE;
						mdtag.Key = "DisposalMethod";
						mdtag.Value = (byte)(value.Value);
						FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "DisposalMethod", mdtag.tag);
					}
					else
					{
						mdtag.Value = value;
					}
				}
				else
				{
					FreeImage.SetMetadata(FREE_IMAGE_MDMODEL.FIMD_ANIMATION, bitmap.Dib, "DisposalMethod", FITAG.Zero);
				}
			}
		}
	}
}