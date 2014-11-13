// =====================================================================
// FreeImage Plugin Interface
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Rui Lopes (ruiglopes@yahoo.com)
// - Detlev Vendt (detlev.vendt@brillit.de)
// - Petr Pytelka (pyta@lightcomp.com)
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
// =====================================================================

#ifdef _MSC_VER 
#pragma warning (disable : 4786) // identifier was truncated to 'number' characters
#endif

#ifdef _WIN32
#include <windows.h>
#include <io.h>
#else
#include <ctype.h>
#endif // _WIN32

#include "FreeImage.h"
#include "Utilities.h"
#include "FreeImageIO.h"
#include "Plugin.h"

// =====================================================================

using namespace std;

// =====================================================================
// Plugin search list
// =====================================================================

const char *
s_search_list[] = {
	"",
	"plugins\\",
};

static int s_search_list_size = sizeof(s_search_list) / sizeof(char *);
static PluginList *s_plugins = NULL;
static int s_plugin_reference_count = 0;


// =====================================================================
// Reimplementation of stricmp (it is not supported on some systems)
// =====================================================================

int
FreeImage_stricmp(const char *s1, const char *s2) {
	int c1, c2;

	do {
		c1 = tolower(*s1++);
		c2 = tolower(*s2++);
	} while (c1 && c1 == c2);

	return c1 - c2;
}

// =====================================================================
//  Implementation of PluginList
// =====================================================================

PluginList::PluginList() :
m_lastId(int(FIF_CUSTOM))
{
}

FREE_IMAGE_FORMAT
PluginList::AddNode(FI_InitProc init_proc, FREE_IMAGE_FORMAT fif, void *instance, const char *format, const char *description, const char *extension, const char *regexpr) {
	if (!init_proc) 
		return FIF_UNKNOWN;
		
	int id = -1;
	if(fif != FIF_UNKNOWN)
		id = (int) fif;
	else
		id = m_lastId++;
		
	PluginNode *node = new (std::nothrow) PluginNode;
	Plugin *plugin = new (std::nothrow) Plugin;
	
	if(!node || !plugin)
		return FIF_UNKNOWN;
	
	memset(plugin, 0, sizeof(Plugin));

	assert(id >= 0);
	
	// fill-in the plugin structure
	
	init_proc(plugin, id/*(int)m_plugin_map.size()*/);

	// get the format string (two possible ways)

	const char *the_format = NULL;

	if (format != NULL) 
		the_format = format;
	else if (plugin->format_proc != NULL)
		the_format = plugin->format_proc();

	// add the node if it wasn't there already

	if (the_format != NULL) {
		if (FindNodeFromFormat(the_format) == NULL) {
			node->m_id = id/*(int)m_plugin_map.size()*/;
			node->m_instance = instance;
			node->m_plugin = plugin;
			node->m_format = format;
			node->m_description = description;
			node->m_extension = extension;
			node->m_regexpr = regexpr;
			node->m_enabled = TRUE;

			m_plugin_map[/*(const int)m_plugin_map.size()*/id] = node;

			return (FREE_IMAGE_FORMAT)id;
		}
	}

	// something went wrong while allocating the plugin... cleanup

	delete plugin;
	delete node;
	

	
}

PluginNode *
PluginList::FindNodeFromFormat(const char *format) {
//	int count = 0;

	for (map<int, PluginNode *>::iterator i = m_plugin_map.begin(); i != m_plugin_map.end(); ++i) {
		const char *the_format = ((*i).second->m_format != NULL) ? (*i).second->m_format : (*i).second->m_plugin->format_proc();

		if (FreeImage_stricmp(the_format, format) == 0)
			return (*i).second;		

//		count++;
	}

	return NULL;
}

PluginNode *
PluginList::FindNodeFromMime(const char *mime) {
	int count = 0;

	for (map<int, PluginNode *>::iterator i = m_plugin_map.begin(); i != m_plugin_map.end(); ++i) {
		const char *the_mime = ((*i).second->m_plugin->mime_proc != NULL) ? (*i).second->m_plugin->mime_proc() : "";

		if ((the_mime != NULL) && (strcmp(the_mime, mime) == 0))
			return (*i).second;		

		count++;
	}

	return NULL;
}

PluginNode *
PluginList::FindNodeFromFIF(int node_id) {
	map<int, PluginNode *>::iterator i = m_plugin_map.find(node_id);

	if (i != m_plugin_map.end())
		return (*i).second;

	return NULL;
}

static
BOOL
validate(PluginNode *node, FreeImageIO *io, fi_handle handle)
{
	long tell = io->tell_proc(handle);

	BOOL isValid = (node != NULL) ? (node->m_enabled) ? (node->m_plugin->validate_proc != NULL) ? node->m_plugin->validate_proc(io, handle) : FALSE : FALSE : FALSE;

	io->seek_proc(handle, tell, SEEK_SET);
	
	return isValid;
}

int
PluginList::Size() const {
	return m_plugin_map.size();
}

int
PluginList::lastId() const {
	return m_lastId;
}

//BOOL
//PluginList::IsEmpty() const {
//	return m_plugin_map.empty();
//}

PluginList::const_iterator 
PluginList::begin() const { 
	return m_plugin_map.begin(); 
	} 

PluginList::const_iterator 
PluginList::end() const { 
	return m_plugin_map.end(); 
} 


PluginList::~PluginList() {
	for (PluginMap::iterator i = m_plugin_map.begin(); i != m_plugin_map.end(); ++i) {
#ifdef _WIN32
		if ((*i).second->m_instance != NULL)
			FreeLibrary((HINSTANCE)(*i).second->m_instance);
#endif
		delete (*i).second->m_plugin;
		delete ((*i).second);
	}
}

// =====================================================================
// Retrieve a pointer to the plugin list container
// =====================================================================

PluginList * DLL_CALLCONV
FreeImage_GetPluginList() {
	return s_plugins;
}

// =====================================================================
// Plugin System Initialization
// =====================================================================

void DLL_CALLCONV
FreeImage_Initialise(BOOL load_local_plugins_only) {
	if (s_plugin_reference_count++ == 0) {

		// internal plugin initialization
		if (!(s_plugins = new (std::nothrow) PluginList))
		{
			FreeImage_OutputMessageProc(0, FI_MSG_ERROR_MEMORY);
			return;
		}
		
		/* To remove internal plugins:
			1. Comment out the desired AddNode-s below
			2. Remove the relevant PluginXXX.cpp file and library from build
		*/
		s_plugins->AddNode(InitBMP, FIF_BMP);
		s_plugins->AddNode(InitICO, FIF_ICO);

#ifdef ENABLE_JPEG
		s_plugins->AddNode(InitJPEG, FIF_JPEG);
#endif

		s_plugins->AddNode(InitMNG, FIF_JNG, NULL, "JNG", "JPEG Network Graphics", "jng", "");
		s_plugins->AddNode(InitKOALA,FIF_KOALA);
		s_plugins->AddNode(InitIFF, FIF_LBM);
		s_plugins->AddNode(InitMNG, FIF_MNG);
		s_plugins->AddNode(InitPNM, FIF_PBM, NULL, "PBM", "Portable Bitmap (ASCII)", "pbm", "^P1");
		s_plugins->AddNode(InitPNM, FIF_PBMRAW, NULL, "PBMRAW", "Portable Bitmap (RAW)", "pbm", "^P4");
		s_plugins->AddNode(InitPCD, FIF_PCD);
		s_plugins->AddNode(InitPCX, FIF_PCX);
		s_plugins->AddNode(InitPNM, FIF_PGM, NULL, "PGM", "Portable Greymap (ASCII)", "pgm", "^P2");
		s_plugins->AddNode(InitPNM, FIF_PGMRAW, NULL, "PGMRAW", "Portable Greymap (RAW)", "pgm", "^P5");

#ifdef ENABLE_PNG
		s_plugins->AddNode(InitPNG, FIF_PNG);
#endif

		s_plugins->AddNode(InitPNM, FIF_PPM, NULL, "PPM", "Portable Pixelmap (ASCII)", "ppm", "^P3");
		s_plugins->AddNode(InitPNM, FIF_PPMRAW, NULL, "PPMRAW", "Portable Pixelmap (RAW)", "ppm", "^P6");
		s_plugins->AddNode(InitRAS, FIF_RAS);
		s_plugins->AddNode(InitTARGA, FIF_TARGA);
//		s_plugins->AddNode(InitTIFF, FIF_TIFF);
		s_plugins->AddNode(InitWBMP, FIF_WBMP);
		s_plugins->AddNode(InitPSD, FIF_PSD);
		s_plugins->AddNode(InitCUT, FIF_CUT);
		s_plugins->AddNode(InitXBM, FIF_XBM);
		s_plugins->AddNode(InitXPM, FIF_XPM);
		s_plugins->AddNode(InitDDS, FIF_DDS);
		s_plugins->AddNode(InitGIF, FIF_GIF );
		s_plugins->AddNode(InitHDR, FIF_HDR	);
//		s_plugins->AddNode(InitG3,  FIF_FAXG3);
		s_plugins->AddNode(InitSGI, FIF_SGI	);
//		s_plugins->AddNode(InitEXR, FIF_EXR	);
//		s_plugins->AddNode(InitJ2K, FIF_J2K	);
//		s_plugins->AddNode(InitJP2, FIF_JP2	);
		s_plugins->AddNode(InitPFM, FIF_PFM	);
		s_plugins->AddNode(InitPICT, FIF_PICT );
//		s_plugins->AddNode(InitRAW, FIF_RAW	);
		
		// external plugin initialization

#ifdef _WIN32
		if (!load_local_plugins_only) {
			int count = 0;
			char buffer[MAX_PATH + 200];
			char current_dir[2 * _MAX_PATH], module[2 * _MAX_PATH];
			BOOL bOk = FALSE;

			// store the current directory. then set the directory to the application location

			if (GetCurrentDirectory(2 * _MAX_PATH, current_dir) != 0) {
				if (GetModuleFileName(NULL, module, 2 * _MAX_PATH) != 0) {
					char *last_point = strrchr(module, '\\');

					if (last_point) {
						*last_point = '\0';

						bOk = SetCurrentDirectory(module);
					}
				}
			}

			// search for plugins

			while (count < s_search_list_size) {
				_finddata_t find_data;
				long find_handle;

				strcpy(buffer, s_search_list[count]);
				strcat(buffer, "*.fip");

				if ((find_handle = (long)_findfirst(buffer, &find_data)) != -1L) {
					do {
						strcpy(buffer, s_search_list[count]);
						strncat(buffer, find_data.name, MAX_PATH + 200);

						HINSTANCE instance = LoadLibrary(buffer);

						if (instance != NULL) {
							FARPROC proc_address = GetProcAddress(instance, "_Init@8");

							if (proc_address != NULL)
								s_plugins->AddNode((FI_InitProc)proc_address, FIF_UNKNOWN, (void *)instance);
							else
								FreeLibrary(instance);
						}
					} while (_findnext(find_handle, &find_data) != -1L);

					_findclose(find_handle);
				}

				count++;
			}

			// restore the current directory

			if (bOk)
				SetCurrentDirectory(current_dir);
		}
#endif // _WIN32
		
	}
}

void DLL_CALLCONV
FreeImage_DeInitialise() {
	--s_plugin_reference_count;

	if (s_plugin_reference_count == 0) {
		delete s_plugins;
	}
}

// =====================================================================
// Open and close a bitmap
// =====================================================================

void * DLL_CALLCONV
FreeImage_Open(PluginNode *node, FreeImageIO *io, fi_handle handle, BOOL open_for_reading) {
	if (node->m_plugin->open_proc != NULL)
       return node->m_plugin->open_proc(io, handle, open_for_reading);

	return NULL;
}

void DLL_CALLCONV
FreeImage_Close(PluginNode *node, FreeImageIO *io, fi_handle handle, void *data) {
	if (node->m_plugin->close_proc != NULL)
		node->m_plugin->close_proc(io, handle, data);
}

// =====================================================================
// Plugin System Load/Save Functions
// =====================================================================

FIBITMAP * DLL_CALLCONV
FreeImage_LoadFromHandle(FREE_IMAGE_FORMAT fif, FreeImageIO *io, fi_handle handle, int flags) {
	if ((fif >= 0) && (fif < FreeImage_GetLastFIF())) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);
		
		if (node != NULL) {
			if (node->m_enabled) {
				if(node->m_plugin->load_proc != NULL) {
					FIBITMAP *bitmap = NULL;
					
					void *data = FreeImage_Open(node, io, handle, TRUE);
					
					bitmap = node->m_plugin->load_proc(io, handle, -1, flags, data);
					
					FreeImage_Close(node, io, handle, data);
					
					return bitmap;
				}
			}
		}
	}

	return NULL;
}

FIBITMAP * DLL_CALLCONV
FreeImage_Load(FREE_IMAGE_FORMAT fif, const char *filename, int flags) {
	FreeImageIO io;
	SetDefaultIO(&io);
	
	FILE *handle = fopen(filename, "rb");

	if (handle) {
		FIBITMAP *bitmap = FreeImage_LoadFromHandle(fif, &io, (fi_handle)handle, flags);

		fclose(handle);

		return bitmap;
	} else {
		FreeImage_OutputMessageProc((int)fif, "FreeImage_Load: failed to open file %s", filename);
	}

	return NULL;
}

FIBITMAP * DLL_CALLCONV
FreeImage_LoadU(FREE_IMAGE_FORMAT fif, const wchar_t *filename, int flags) {
	FreeImageIO io;
	SetDefaultIO(&io);
#ifdef _WIN32	
	FILE *handle = _wfopen(filename, L"rb");

	if (handle) {
		FIBITMAP *bitmap = FreeImage_LoadFromHandle(fif, &io, (fi_handle)handle, flags);

		fclose(handle);

		return bitmap;
	} else {
		FreeImage_OutputMessageProc((int)fif, "FreeImage_LoadU: failed to open input file");
	}
#endif
	return NULL;
}

BOOL DLL_CALLCONV
FreeImage_SaveToHandle(FREE_IMAGE_FORMAT fif, FIBITMAP *dib, FreeImageIO *io, fi_handle handle, int flags) {
	// cannot save "header only" formats
	if(FreeImage_HasPixels(dib) == FALSE) {
		FreeImage_OutputMessageProc((int)fif, "FreeImage_SaveToHandle: cannot save \"header only\" FIBITMAPs");
		return FALSE;
	}

	if ((fif >= 0) && (fif < FreeImage_GetLastFIF())) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);
		
		if (node) {
			if (node->m_enabled) {
				if(node->m_plugin->save_proc != NULL) {
					BOOL result = FALSE;

					void *data = FreeImage_Open(node, io, handle, FALSE);
					
					result = node->m_plugin->save_proc(io, dib, handle, -1, flags, data);
					
					FreeImage_Close(node, io, handle, data);
					
					return result;
				}
			}
		}
	}

	return FALSE;
}


BOOL DLL_CALLCONV
FreeImage_Save(FREE_IMAGE_FORMAT fif, FIBITMAP *dib, const char *filename, int flags) {
	FreeImageIO io;
	SetDefaultIO(&io);
	
	FILE *handle = fopen(filename, "w+b");
	
	if (handle) {
		BOOL success = FreeImage_SaveToHandle(fif, dib, &io, (fi_handle)handle, flags);

		fclose(handle);

		return success;
	} else {
		FreeImage_OutputMessageProc((int)fif, "FreeImage_Save: failed to open file %s", filename);
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_SaveU(FREE_IMAGE_FORMAT fif, FIBITMAP *dib, const wchar_t *filename, int flags) {
	FreeImageIO io;
	SetDefaultIO(&io);
#ifdef _WIN32	
	FILE *handle = _wfopen(filename, L"w+b");
	
	if (handle) {
		BOOL success = FreeImage_SaveToHandle(fif, dib, &io, (fi_handle)handle, flags);

		fclose(handle);

		return success;
	} else {
		FreeImage_OutputMessageProc((int)fif, "FreeImage_SaveU: failed to open output file");
	}
#endif
	return FALSE;
}

// =====================================================================
// Plugin construction + enable/disable functions
// =====================================================================

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_RegisterLocalPlugin(FI_InitProc proc_address, const char *format, const char *description, const char *extension, const char *regexpr) {
	return s_plugins->AddNode(proc_address, FIF_UNKNOWN, NULL, format, description, extension, regexpr);
}

#ifdef _WIN32
FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_RegisterExternalPlugin(const char *path, const char *format, const char *description, const char *extension, const char *regexpr) {
	if (path != NULL) {
		HINSTANCE instance = LoadLibrary(path);

		if (instance != NULL) {
			FARPROC proc_address = GetProcAddress(instance, "_Init@8");

			FREE_IMAGE_FORMAT result = s_plugins->AddNode((FI_InitProc)proc_address, FIF_UNKNOWN, (void *)instance, format, description, extension, regexpr);

			if (result == FIF_UNKNOWN)
				FreeLibrary(instance);

			return result;
		}
	}

	return FIF_UNKNOWN;
}
#endif // _WIN32

int DLL_CALLCONV
FreeImage_SetPluginEnabled(FREE_IMAGE_FORMAT fif, BOOL enable) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		if (node != NULL) {
			BOOL previous_state = node->m_enabled;

			node->m_enabled = enable;

			return previous_state;
		}
	}

	return -1;
}

int DLL_CALLCONV
FreeImage_IsPluginEnabled(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? node->m_enabled : FALSE;
	}
	
	return -1;
}

// =====================================================================
// Plugin Access Functions
// =====================================================================

int DLL_CALLCONV
FreeImage_GetFIFCount() {
	return (s_plugins != NULL) ? s_plugins->Size() : 0;
}

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_GetLastFIF() {
	return (s_plugins != NULL) ? (FREE_IMAGE_FORMAT)s_plugins->lastId() : FIF_UNKNOWN;
}

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_GetFIFFromFormat(const char *format) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFormat(format);

		return (node != NULL) ? (node->m_enabled) ? (FREE_IMAGE_FORMAT)node->m_id : FIF_UNKNOWN : FIF_UNKNOWN;
	}

	return FIF_UNKNOWN;
}

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_GetFIFFromMime(const char *mime) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromMime(mime);

		return (node != NULL) ? (node->m_enabled) ? (FREE_IMAGE_FORMAT)node->m_id : FIF_UNKNOWN : FIF_UNKNOWN;
	}

	return FIF_UNKNOWN;
}

static inline
const char*
getFormat(const PluginNode* node)
{
	return (node != NULL) ? (node->m_format != NULL) ? node->m_format : node->m_plugin->format_proc() : NULL;
}

const char * DLL_CALLCONV
FreeImage_GetFormatFromFIF(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return getFormat(node);
	}

	return NULL;
}

const char * DLL_CALLCONV 
FreeImage_GetFIFMimeType(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? (node->m_plugin != NULL) ? ( node->m_plugin->mime_proc != NULL )? node->m_plugin->mime_proc() : NULL : NULL : NULL;
	}

	return NULL;
}

static inline
const char*
getExtensions(const PluginNode* node)
{
	return (node != NULL) ? (node->m_extension != NULL) ? node->m_extension : (node->m_plugin->extension_proc != NULL) ? node->m_plugin->extension_proc() : NULL : NULL;
}

const char * DLL_CALLCONV
FreeImage_GetFIFExtensionList(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return getExtensions(node);
	}

	return NULL;
}

const char * DLL_CALLCONV
FreeImage_GetFIFDescription(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? (node->m_description != NULL) ? node->m_description : (node->m_plugin->description_proc != NULL) ? node->m_plugin->description_proc() : NULL : NULL;
	}

	return NULL;
}

const char * DLL_CALLCONV
FreeImage_GetFIFRegExpr(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? (node->m_regexpr != NULL) ? node->m_regexpr : (node->m_plugin->regexpr_proc != NULL) ? node->m_plugin->regexpr_proc() : NULL : NULL;
	}

	return NULL;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsReading(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? node->m_plugin->load_proc != NULL : FALSE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsWriting(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? node->m_plugin->save_proc != NULL : FALSE ;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsExportBPP(FREE_IMAGE_FORMAT fif, int depth) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? 
			(node->m_plugin->supports_export_bpp_proc != NULL) ? 
				node->m_plugin->supports_export_bpp_proc(depth) : FALSE : FALSE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsExportType(FREE_IMAGE_FORMAT fif, FREE_IMAGE_TYPE type) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? 
			(node->m_plugin->supports_export_type_proc != NULL) ? 
				node->m_plugin->supports_export_type_proc(type) : FALSE : FALSE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsICCProfiles(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? 
			(node->m_plugin->supports_icc_profiles_proc != NULL) ? 
				node->m_plugin->supports_icc_profiles_proc() : FALSE : FALSE;
	}

	return FALSE;
}

BOOL DLL_CALLCONV
FreeImage_FIFSupportsNoPixels(FREE_IMAGE_FORMAT fif) {
	if (s_plugins != NULL) {
		PluginNode *node = s_plugins->FindNodeFromFIF(fif);

		return (node != NULL) ? 
			(node->m_plugin->supports_no_pixels_proc != NULL) ? 
				node->m_plugin->supports_no_pixels_proc() : FALSE : FALSE;
	}

	return FALSE;
}

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_GetFIFFromFilename(const char *filename) {
	if (filename != NULL) {
		const char *extension;

		// get the proper extension if we received a filename

		char *place = strrchr((char *)filename, '.');	
		extension = (place != NULL) ? ++place : filename;

		// look for the extension in the plugin table

		for (PluginList::const_iterator i = s_plugins->begin(); i != s_plugins->end(); ++i) {

			const	FREE_IMAGE_FORMAT fif = (FREE_IMAGE_FORMAT)i->first;
			const PluginNode*const node = i->second;
			
			if (node->m_enabled) {
				
				// compare the format id with the extension

				if (FreeImage_stricmp(getFormat(node), extension) == 0) {
					return fif;
				} else {
					// make a copy of the extension list and split it
					const char* extList = getExtensions(node);
					const size_t len = strlen(extList);
					
					char *copy = (char *)malloc(len + 1);
					if(!copy)
						return FIF_UNKNOWN;
						
					memset(copy, 0, len + 1);
					memcpy(copy, extList, len);

					// get the first token

					char *token = strtok(copy, ",");

					while (token != NULL) {
						if (FreeImage_stricmp(token, extension) == 0) {
							free(copy);

								return fif;
						}

						token = strtok(NULL, ",");
					}

					// free the copy of the extension list

					free(copy);
				}	
			}
		}
	}

	return FIF_UNKNOWN;
}

FREE_IMAGE_FORMAT DLL_CALLCONV 
FreeImage_GetFIFFromFilenameU(const wchar_t *filename) {
#ifdef _WIN32	
	if (filename == NULL) return FIF_UNKNOWN;
    	
	// get the proper extension if we received a filename
	wchar_t *place = wcsrchr((wchar_t *)filename, '.');	
	if (place == NULL) return FIF_UNKNOWN;
	// convert to single character - no national chars in extensions
	char *extension = (char *)malloc(wcslen(place)+1);
	unsigned int i=0;
	for(; i < wcslen(place); i++) // convert 16-bit to 8-bit
		extension[i] = (char)(place[i] & 0x00FF);
	// set terminating 0
	extension[i]=0;
	FREE_IMAGE_FORMAT fRet = FreeImage_GetFIFFromFilename(extension);
	free(extension);

	return fRet;
#else
	return FIF_UNKNOWN;
#endif // _WIN32
}

BOOL DLL_CALLCONV
FreeImage_Validate(FREE_IMAGE_FORMAT fif, FreeImageIO *io, fi_handle handle) {
	if (s_plugins != NULL) {

		PluginNode *node = s_plugins->FindNodeFromFIF(fif);
		// node cannot be NULL
		
		return validate(node, io, handle);
	}

	return FALSE;
}

FREE_IMAGE_FORMAT DLL_CALLCONV
FreeImage_GetFileTypeFromHandle(FreeImageIO *io, fi_handle handle, int size) {
	if (!handle) 
		return FIF_UNKNOWN;
		
	for (PluginList::const_iterator i = s_plugins->begin(); i != s_plugins->end(); ++i)
	{
		int fif = i->first;
		if(validate(i->second, io, handle))
		{
			if(fif == int(FIF_TIFF)) 
			{
				// many camera raw files use a TIFF signature ...
				// ... try to revalidate against FIF_RAW (even if it breaks the code genericity)
				if (FreeImage_Validate(FIF_RAW, io, handle)) {
					return FIF_RAW;
				}
			}
			return FREE_IMAGE_FORMAT(fif);
		}
	}
	
	return FIF_UNKNOWN;
		
//	int fif_count = FreeImage_GetFIFCount();
//
//	for (int i = 0; i < fif_count; ++i) {
//		FREE_IMAGE_FORMAT fif = (FREE_IMAGE_FORMAT)i;
//		if (FreeImage_Validate(fif, io, handle)) {
//			if(fif == FIF_TIFF) {
//				// many camera raw files use a TIFF signature ...
//				// ... try to revalidate against FIF_RAW (even if it breaks the code genericity)
//				if (FreeImage_Validate(FIF_RAW, io, handle)) {
//					return FIF_RAW;
//				}
//			}
//			return fif;
//		}
//	}
	
}
