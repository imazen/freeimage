using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FreeImageAPI
{
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
}