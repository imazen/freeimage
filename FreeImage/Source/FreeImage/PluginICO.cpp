// ==========================================================
// ICO Loader and Writer
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Hervé Drolon (drolon@infonie.fr)
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

#include "FreeImage.h"
#include "Utilities.h"

// ----------------------------------------------------------
//   Constants + headers
// ----------------------------------------------------------

#ifdef WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif

// These next two structs represent how the icon information is stored
// in an ICO file.

typedef struct tagICONHEADER {
	WORD			idReserved;   // reserved
	WORD			idType;       // resource type (1 for icons)
	WORD			idCount;      // how many images?
} ICONHEADER;

typedef struct tagICONDIRECTORYENTRY {
	BYTE	bWidth;               // width of the image
	BYTE	bHeight;              // height of the image (times 2)
	BYTE	bColorCount;          // number of colors in image (0 if >=8bpp)
	BYTE	bReserved;            // reserved
	WORD	wPlanes;              // color Planes
	WORD	wBitCount;            // bits per pixel
	DWORD	dwBytesInRes;         // how many bytes in this resource?
	DWORD	dwImageOffset;        // where in the file is this image
} ICONDIRENTRY;

#ifdef WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif

// ==========================================================
// Static helpers
// ==========================================================

/**  How wide, in bytes, would this many bits be, DWORD aligned ?
*/
static int 
WidthBytes(int bits) {
	return ((((bits) + 31)>>5)<<2);
}

/** Calculates the size of a single icon image
@return Returns the size for that image
*/
static DWORD 
CalculateImageSize(FIBITMAP* icon_dib) {
	DWORD dwNumBytes = 0;

	BITMAPINFOHEADER *bmih = FreeImage_GetInfoHeader(icon_dib);
	int colors		= FreeImage_GetColorsUsed(icon_dib);
	int width		= bmih->biWidth;
	int height		= bmih->biHeight;
	int pitch		= FreeImage_GetPitch(icon_dib);

	dwNumBytes = sizeof( BITMAPINFOHEADER );	// header
	dwNumBytes += colors * sizeof(RGBQUAD);		// palette
	dwNumBytes += height * pitch;				// XOR mask
	dwNumBytes += height * WidthBytes(width);	// AND mask

	return dwNumBytes;
}

/** Calculates the file offset for an icon image
@return Returns the file offset for that image
*/
static DWORD 
CalculateImageOffset(std::vector<FIBITMAP*>& vPages, int nIndex ) {
	DWORD	dwSize;

    // calculate the ICO header size
    dwSize = sizeof(ICONHEADER); 
    // add the ICONDIRENTRY's
    dwSize += vPages.size() * sizeof(ICONDIRENTRY);
    // add the sizes of the previous images
    for(int k = 0; k < nIndex; k++) {
		FIBITMAP *icon_dib = (FIBITMAP*)vPages[k];
		dwSize += CalculateImageSize(icon_dib);
	}

    return dwSize;
}


// ==========================================================
// Plugin Interface
// ==========================================================

static int s_format_id;

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "ICO";
}

static const char * DLL_CALLCONV
Description() {
	return "Windows Icon";
}

static const char * DLL_CALLCONV
Extension() {
	return "ico";
}

static const char * DLL_CALLCONV
RegExpr() {
	return NULL;
}

static const char * DLL_CALLCONV
MimeType() {
	return "image/x-icon";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {
	ICONHEADER icon_header;

	io->read_proc(&icon_header, 1, sizeof(ICONHEADER), handle);

	return ((icon_header.idReserved == 0) && (icon_header.idType == 1) && (icon_header.idCount > 0));
}

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return (
			(depth == 1) ||
			(depth == 4) ||
			(depth == 8) ||
			(depth == 16) ||
			(depth == 24) ||
			(depth == 32)
		);
}

static BOOL DLL_CALLCONV 
SupportsExportType(FREE_IMAGE_TYPE type) {
	return (type == FIT_BITMAP) ? TRUE : FALSE;
}

// ----------------------------------------------------------

static void * DLL_CALLCONV
Open(FreeImageIO *io, fi_handle handle, BOOL read) {
	ICONHEADER *lpIH = NULL;

	// Allocate memory for the header structure
	if((lpIH = (ICONHEADER*)malloc( sizeof(ICONHEADER) )) == NULL) 
		return NULL;

	if (read) {
		// Read in the header
		io->read_proc(lpIH, 1, sizeof(ICONHEADER), handle);

		if(!(lpIH->idReserved == 0) || !(lpIH->idType == 1)) {
			// Not an ICO file
			free(lpIH);
			return NULL;
		}
	}
	else {
		// Fill the header
		lpIH->idReserved = 0;
		lpIH->idType = 1;
		lpIH->idCount = 0;
	}
	return lpIH;
}

static void DLL_CALLCONV
Close(FreeImageIO *io, fi_handle handle, void *data) {
	// free the header structure
	ICONHEADER *lpIH = (ICONHEADER*)data;
	free(lpIH);
}

// ----------------------------------------------------------

static int DLL_CALLCONV
PageCount(FreeImageIO *io, fi_handle handle, void *data) {
	ICONHEADER *lpIH = (ICONHEADER*)data;

	if(lpIH) {
		return lpIH->idCount;
	}
	return -1;
}

// ----------------------------------------------------------

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
	if (page == -1)
		page = 0;

	if (handle != NULL) {
		FIBITMAP *dib;

		// get the icon header
		ICONHEADER *icon_header = (ICONHEADER*)data;

		if (icon_header) {
			// load the icon descriptions
			ICONDIRENTRY *icon_list = (ICONDIRENTRY*)malloc(icon_header->idCount * sizeof(ICONDIRENTRY));
			io->seek_proc(handle, sizeof(ICONHEADER), SEEK_SET);
			io->read_proc(icon_list, icon_header->idCount * sizeof(ICONDIRENTRY), 1, handle);

			// load the requested icon
			if (page < icon_header->idCount) {
				// seek to the start of the bitmap data for the icon
				io->seek_proc(handle, 0, SEEK_SET);
				io->seek_proc(handle, icon_list[page].dwImageOffset, SEEK_CUR);

				// load the BITMAPINFOHEADER
				BITMAPINFOHEADER bmih;
				io->read_proc(&bmih, sizeof(BITMAPINFOHEADER), 1, handle);

				// allocate the bitmap
				int width  = bmih.biWidth;
				int height = bmih.biHeight / 2; // height == xor + and mask
				int bit_count = bmih.biBitCount;
				int line   = CalculateLine(width, bit_count);
				int pitch  = CalculatePitch(line);

				// allocate memory for one icon

				dib = FreeImage_Allocate(width, height, bit_count);

				if (dib == NULL) {
					free(icon_list);

					return NULL;
				}

				// read the palette data
				io->read_proc(FreeImage_GetPalette(dib), CalculateUsedPaletteEntries(bit_count) * sizeof(RGBQUAD), 1, handle);

				// read the icon
				io->read_proc(FreeImage_GetBits(dib), height * pitch, 1, handle);

				free(icon_list);

				// bitmap has been loaded successfully!

				return (FIBITMAP *)dib;
			}
			else {
				FreeImage_OutputMessageProc(s_format_id, "Page doesn't exist");
			}
		} else {
			FreeImage_OutputMessageProc(s_format_id, "File is not an ICO file");
		}
	}

	return NULL;
}

static BOOL DLL_CALLCONV
Save(FreeImageIO *io, FIBITMAP *dib, fi_handle handle, int page, int flags, void *data) {
	int k;

	if(dib) {
		// check format limits
		int w = FreeImage_GetWidth(dib);
		int h = FreeImage_GetHeight(dib);
		if((w < 16) || (w > 128) || (h < 16) || (h > 128)) {
			FreeImage_OutputMessageProc(s_format_id, "Unsupported icon size");
			return FALSE;
		}
	} else {
		return FALSE;
	}

	if (page == -1)
		page = 0;

	// get the icon header
	ICONHEADER *icon_header = (ICONHEADER*)data;

	if(icon_header) {

		std::vector<FIBITMAP*> vPages;
		FIBITMAP  *icon_dib = NULL;

		// load all icons
		for(k = 0; k < icon_header->idCount; k++) {
			icon_dib = Load(io, handle, k, flags, data);
			vPages.push_back(icon_dib);
		}

		// add the page
		icon_dib = FreeImage_Clone(dib);
		vPages.push_back(icon_dib);
		icon_header->idCount++;

		// write the header
		io->seek_proc(handle, 0, SEEK_SET);
		io->write_proc(icon_header, sizeof(ICONHEADER), 1, handle);

		// write all icons
		// ...

		// save the icon descriptions
		ICONDIRENTRY *icon_list = (ICONDIRENTRY *)malloc(icon_header->idCount * sizeof(ICONDIRENTRY));
		memset(icon_list, 0, icon_header->idCount * sizeof(ICONDIRENTRY));
		BITMAPINFOHEADER *bmih;
		for(k = 0; k < icon_header->idCount; k++) {
			icon_dib = (FIBITMAP*)vPages[k];

			// convert internal format to ICONDIRENTRY
			bmih = FreeImage_GetInfoHeader(icon_dib);
			icon_list[k].bWidth			= bmih->biWidth;
			icon_list[k].bHeight		= bmih->biHeight;
			icon_list[k].bReserved		= 0;
			icon_list[k].wPlanes		= bmih->biPlanes;
			icon_list[k].wBitCount		= bmih->biBitCount;
			if( (icon_list[k].wPlanes * icon_list[k].wBitCount) >= 8 ) {
				icon_list[k].bColorCount = 0;
			} else {
				icon_list[k].bColorCount = 1 << (icon_list[k].wPlanes * icon_list[k].wBitCount);
			}
			icon_list[k].dwBytesInRes	= CalculateImageSize(icon_dib);
			icon_list[k].dwImageOffset = CalculateImageOffset(vPages, k);
		}
		io->write_proc(icon_list, sizeof(ICONDIRENTRY) * icon_header->idCount, 1, handle);
		free(icon_list);

		// write the image bits for each image
		for(k = 0; k < icon_header->idCount; k++) {
			icon_dib = (FIBITMAP*)vPages[k];

			// write the BITMAPINFOHEADER
			bmih = FreeImage_GetInfoHeader(icon_dib);
			bmih->biHeight *= 2;	// height == xor + and mask
			io->write_proc(bmih, sizeof(BITMAPINFOHEADER), 1, handle);
			bmih->biHeight /= 2;

			// write the palette data
			RGBQUAD *pal = FreeImage_GetPalette(icon_dib);
			int colors = FreeImage_GetColorsUsed(icon_dib);
			io->write_proc(pal, colors * sizeof(RGBQUAD), 1, handle);

			// write the bits
			int width		= bmih->biWidth;
			int height		= bmih->biHeight;
			int bit_count	= bmih->biBitCount;
			int line		= CalculateLine(width, bit_count);
			int pitch		= CalculatePitch(line);
			int size_xor	= height * pitch;
			int size_and	= height * WidthBytes(width);

			// XOR mask
			BYTE *xor_mask = FreeImage_GetBits(icon_dib);
			io->write_proc(xor_mask, size_xor, 1, handle);
			// empty AND mask
			BYTE *and_mask = (BYTE*)malloc(size_and);
			memset(and_mask, 0, size_and);
			io->write_proc(and_mask, size_and, 1, handle);
			free(and_mask);

		}

		// free the vector class
		for(k = 0; k < icon_header->idCount; k++) {
			icon_dib = (FIBITMAP*)vPages[k];
			FreeImage_Unload(icon_dib);
		}

		return TRUE;
	}

	return FALSE;
}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitICO(Plugin *plugin, int format_id) {
	s_format_id = format_id;

	plugin->format_proc = Format;
	plugin->description_proc = Description;
	plugin->extension_proc = Extension;
	plugin->regexpr_proc = RegExpr;
	plugin->open_proc = Open;
	plugin->close_proc = Close;
	plugin->pagecount_proc = PageCount;
	plugin->pagecapability_proc = NULL;
	plugin->load_proc = Load;
	plugin->save_proc = Save;
	plugin->validate_proc = Validate;
	plugin->mime_proc = MimeType;
	plugin->supports_export_bpp_proc = SupportsExportDepth;
	plugin->supports_export_type_proc = SupportsExportType;
	plugin->supports_icc_profiles_proc = NULL;
}
