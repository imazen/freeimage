// ==========================================================
// Loader/Saver Plugin Cradle
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
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

#include <windows.h>
#include <stdlib.h>

#include "FreeImage.h"
#include "Utilities.h"

// ==========================================================

BOOL APIENTRY
DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH :
		case DLL_PROCESS_DETACH :
		case DLL_THREAD_ATTACH  :
		case DLL_THREAD_DETACH  :
			break;
    }

    return TRUE;
}

// ==========================================================
// Plugin Interface
// ==========================================================

static int s_format_id;

// ==========================================================
// Plugin Implementation
// ==========================================================

/**
    Returns the format string for the plugin. Each plugin,
	both internal in the DLL and external in a .fip file, must have
	a unique format string to be addressable.
*/

static const char * DLL_CALLCONV
Format() {
	return "CRADLE";
}

/**
    Returns a description string for the plugin. Though a
	description is not necessary per-se,
	it is advised to return an unique string in order to tell the
	user what type of bitmaps this plugin will read and/or write.
*/

static const char * DLL_CALLCONV
Description() {
	return "Here comes the description for your image loader/saver";
}

/**
    Returns a comma separated list of file extensions indicating 
	what files this plugin can open. no spaces or whatsoever are allowed. 
	The list, being used by FreeImage_GetFIFFromFilename, is usually
	used as a last resort in finding the type of the bitmap we
	are dealing with. Best is to check the first few bytes on
	the low-level bits level first and compare them with a known
	signature . If this fails, FreeImage_GetFIFFromFilename can be
	used.
*/

static const char * DLL_CALLCONV
Extension() {
	return "ext1,ext2";
}

// RegExpr is only needed for the Qt wrapper
// It allows the Qt mechanism for loading bitmaps to identify the bitmap

static const char * DLL_CALLCONV
RegExpr() {
	return NULL;
}

// FreeImage's internal way of seeing if a bitmap is of the desired type.
// When the type of a bitmap is to be retrieved, FreeImage runs Validate
// for each registered plugin until one returns true. If a plugin doesn't
// have a validate function, a return value of false is assumed.
//
// You can always force to use a particular plugin by directly specifying
// it on the command line, but this can result in a dead DLL if the plugin
// was not made for the bitmap.

static BOOL DLL_CALLCONV
Validate(FreeImageIO &io, fi_handle handle) {
	return FALSE;
}

// SupportsExportDepth is the first in a possible range of new plugin functions
// to ask specific information to that plugin. This function returns TRUE if it
// can save a bitmap in the required bitdepth. If it can't the bitmap has to be
// converted by the user or another plugin has to be chosen.

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return FALSE;
}

// SupportsICCProfiles informs FreeImage that a plugin supports ICC profiles. 
// This function returns TRUE if the plugin can load and save a profile.
// ICC profile information is accessed via freeimage->get_icc_profile_proc(dib)

static BOOL DLL_CALLCONV
SupportsICCProfiles() {
	return FALSE;
}


// ----------------------------------------------------------

// NEVER EVER use direct function calls to FreeImage when writing
// a plugin. Always use the function pointers provides in the FreeImage
// structure. Using this indirect approach ensures that plugins run
// from the FreeImage DLL, an external DLL or within the application
// code with a simple recompile. Also NEVER EVER allocate memory in
// a plugin and return it. Memory allocated in one DLL can't be freed
// in another DLL unless they share the same c runtime library (which
// usually is not the case). Always use freeimage.allocate_proc to
// allocate memory for  a bitmap.

static FIBITMAP * DLL_CALLCONV
Load(FreeImage *freeimage, FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	return NULL;
}

static BOOL DLL_CALLCONV
Save(FreeImage *freeimage, FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	return FALSE;
}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
Init(Plugin *plugin, int format_id) {
	s_format_id = format_id;

	plugin.format_proc = Format;
	plugin.description_proc = Description;
	plugin.extension_proc = Extension;
	plugin.regexpr_proc = RegExpr;
	plugin.load_proc = Load;
	plugin.save_proc = Save;
	plugin.validate_proc = Validate;
	plugin.supports_export_bpp_proc = SupportsExportDepth;
	plugin.supports_icc_profiles_proc = SupportsICCProfiles;
}
