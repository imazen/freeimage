// ==========================================================
// Metadata functions implementation
// Extended TIFF Directory GEO Tag Support
//
// Design and implementation by
// - Hervé Drolon (drolon@infonie.fr)
//
// Based on the LibTIFF xtiffio sample and on LibGeoTIFF
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

#ifdef _MSC_VER 
#pragma warning (disable : 4786) // identifier was truncated to 'number' characters
#endif

#include "../LibTIFF/tiffiop.h"

#include "FreeImage.h"
#include "Utilities.h"
#include "FreeImageTag.h"

// ----------------------------------------------------------
//   Extended TIFF Directory GEO Tag Support
// ----------------------------------------------------------

/**
  Tiff info structure.
  Entry format:
  { TAGNUMBER, ReadCount, WriteCount, DataType, FIELDNUM, OkToChange, PassDirCountOnSet, AsciiName }

  For ReadCount, WriteCount, -1 = unknown.
*/
static const TIFFFieldInfo xtiffFieldInfo[] = {
	{ TIFFTAG_GEOPIXELSCALE, -1, -1, TIFF_DOUBLE, FIELD_CUSTOM, TRUE, TRUE, "GeoPixelScale" },
	{ TIFFTAG_INTERGRAPH_MATRIX, -1, -1, TIFF_DOUBLE, FIELD_CUSTOM, TRUE, TRUE, "Intergraph TransformationMatrix" },
	{ TIFFTAG_GEOTRANSMATRIX, -1, -1, TIFF_DOUBLE, FIELD_CUSTOM, TRUE, TRUE, "GeoTransformationMatrix" },
	{ TIFFTAG_GEOTIEPOINTS,	-1, -1, TIFF_DOUBLE, FIELD_CUSTOM, TRUE, TRUE, "GeoTiePoints" },
	{ TIFFTAG_GEOKEYDIRECTORY,-1,-1, TIFF_SHORT, FIELD_CUSTOM, TRUE, TRUE, "GeoKeyDirectory" },
	{ TIFFTAG_GEODOUBLEPARAMS, -1, -1, TIFF_DOUBLE,	FIELD_CUSTOM, TRUE,	TRUE, "GeoDoubleParams" },
	{ TIFFTAG_GEOASCIIPARAMS, -1, -1, TIFF_ASCII, FIELD_CUSTOM, TRUE, FALSE, "GeoASCIIParams" },
	{ TIFFTAG_JPL_CARTO_IFD, 1, 1, TIFF_LONG, FIELD_CUSTOM, TRUE, TRUE,	"JPL Carto IFD offset" }  /** Don't use this! **/
};

static void 
_XTIFFLocalDefaultDirectory(TIFF *tif) {
	size_t tag_size = sizeof(xtiffFieldInfo) / sizeof(xtiffFieldInfo[0]);
    // Install the extended Tag field info
    TIFFMergeFieldInfo(tif, xtiffFieldInfo, tag_size);
}

static TIFFExtendProc _ParentExtender;

/**
This is the callback procedure, and is
called by the DefaultDirectory method
every time a new TIFF directory is opened.
*/
static void
_XTIFFDefaultDirectory(TIFF *tif) {
    // set up our own defaults
    _XTIFFLocalDefaultDirectory(tif);

	/*
	Since an XTIFF client module may have overridden
	the default directory method, we call it now to
	allow it to set up the rest of its own methods.
	*/
    if (_ParentExtender) 
        (*_ParentExtender)(tif);
}

/**
XTIFF Initializer -- sets up the callback procedure for the TIFF module
*/
void 
XTIFFInitialize(void) {
    static int first_time = 1;
	
    if (! first_time) return; /* Been there. Done that. */
    first_time = 0;
	
    // Grab the inherited method and install
    _ParentExtender = TIFFSetTagExtender(_XTIFFDefaultDirectory);
}

// ----------------------------------------------------------
//   GeoTIFF tag reading / writing
// ----------------------------------------------------------

void 
tiff_read_geotiff_profile(TIFF *tif, FIBITMAP *dib) {

	size_t tag_size = sizeof(xtiffFieldInfo) / sizeof(xtiffFieldInfo[0]);

	TagLib& tag_lib = TagLib::instance();

	for(unsigned i = 0; i < tag_size; i++) {

		const TIFFFieldInfo *fieldInfo = &xtiffFieldInfo[i];

		if(fieldInfo->field_type == TIFF_ASCII) {
			char *params = NULL;

			if(TIFFGetField(tif, fieldInfo->field_tag, &params)) {
				// create a tag
				FITAG *tag = FreeImage_CreateTag();
				if(!tag) return;

				WORD tag_id = (WORD)fieldInfo->field_tag;

				FreeImage_SetTagType(tag, (FREE_IMAGE_MDTYPE)fieldInfo->field_type);
				FreeImage_SetTagID(tag, tag_id);
				FreeImage_SetTagKey(tag, tag_lib.getTagFieldName(TagLib::GEOTIFF, tag_id));
				FreeImage_SetTagDescription(tag, tag_lib.getTagDescription(TagLib::GEOTIFF, tag_id));
				FreeImage_SetTagLength(tag, strlen(params) + 1);
				FreeImage_SetTagCount(tag, FreeImage_GetTagLength(tag));
				FreeImage_SetTagValue(tag, params);
				FreeImage_SetMetadata(FIMD_GEOTIFF, dib, FreeImage_GetTagKey(tag), tag);
				
				// delete the tag
				FreeImage_DeleteTag(tag);
			}
		} else {
			short tag_count = 0;
			void* data = NULL;

			if(TIFFGetField(tif, fieldInfo->field_tag, &tag_count, &data)) {
				// create a tag
				FITAG *tag = FreeImage_CreateTag();
				if(!tag) return;

				WORD tag_id = (WORD)fieldInfo->field_tag;
				FREE_IMAGE_MDTYPE tag_type = (FREE_IMAGE_MDTYPE)fieldInfo->field_type;

				FreeImage_SetTagType(tag, tag_type);
				FreeImage_SetTagID(tag, tag_id);
				FreeImage_SetTagKey(tag, tag_lib.getTagFieldName(TagLib::GEOTIFF, tag_id));
				FreeImage_SetTagDescription(tag, tag_lib.getTagDescription(TagLib::GEOTIFF, tag_id));
				FreeImage_SetTagLength(tag, FreeImage_TagDataWidth(tag_type) * tag_count);
				FreeImage_SetTagCount(tag, tag_count);
				FreeImage_SetTagValue(tag, data);
				FreeImage_SetMetadata(FIMD_GEOTIFF, dib, FreeImage_GetTagKey(tag), tag);

				// delete the tag
				FreeImage_DeleteTag(tag);
			}
		}
	} // for(tag_size)	
}

void 
tiff_write_geotiff_profile(TIFF *tif, FIBITMAP *dib) {
	if(FreeImage_GetMetadataCount(FIMD_GEOTIFF, dib) == 0) {
		return;
	}

	size_t tag_size = sizeof(xtiffFieldInfo) / sizeof(xtiffFieldInfo[0]);

	TagLib& tag_lib = TagLib::instance();

	for(unsigned i = 0; i < tag_size; i++) {
		const TIFFFieldInfo *fieldInfo = &xtiffFieldInfo[i];

		FITAG *tag = NULL;
		const char *key = tag_lib.getTagFieldName(TagLib::GEOTIFF, (WORD)fieldInfo->field_tag);

		if(FreeImage_GetMetadata(FIMD_GEOTIFF, dib, key, &tag)) {
			if(FreeImage_GetTagType(tag) == FIDT_ASCII) {
				TIFFSetField(tif, fieldInfo->field_tag, FreeImage_GetTagValue(tag));
			} else {
				TIFFSetField(tif, fieldInfo->field_tag, FreeImage_GetTagCount(tag), FreeImage_GetTagValue(tag));
			}				
		}
	}
}

