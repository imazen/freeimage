// ==========================================================
// Metadata functions implementation
//
// Design and implementation by
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

#ifdef _MSC_VER 
#pragma warning (disable : 4786) // identifier was truncated to 'number' characters
#endif

#include "FreeImage.h"
#include "Utilities.h"
#include "FreeImageTag.h"

// ----------------------------------------------------------
//   IPTC JPEG / TIFF markers routines
// ----------------------------------------------------------

/**
	Read IPTC binary data
*/
BOOL 
read_iptc_profile(FIBITMAP *dib, const BYTE *dataptr, unsigned int datalen) {
	size_t length = datalen;
	BYTE *profile = (BYTE*)dataptr;

	std::string Keywords;
	std::string SupplementalCategory;

	WORD tag_id;

	// create a tag

	FITAG *tag = FreeImage_CreateTag();

	TagLib& tag_lib = TagLib::instance();

    // find start of the BIM portion of the binary data
    size_t offset = 0;
	while(offset < length - 1) {
		if((profile[offset] == 0x1C) && (profile[offset+1] == 0x02))
			break;
		offset++;
	}

    // for each tag
    while (offset < length) {

        // identifies start of a tag
        if (profile[offset] != 0x1c) {
            break;
        }
        // we need at least five bytes left to read a tag
        if ((offset + 5) >= length) {
            break;
        }

        offset++;

		int directoryType	= profile[offset++];
        int tagType			= profile[offset++];;
        int tagByteCount	= ((profile[offset] & 0xFF) << 8) | (profile[offset + 1] & 0xFF);
        offset += 2;

        if ((offset + tagByteCount) > length) {
            // data for tag extends beyond end of iptc segment
            break;
        }

		// process the tag

		tag_id = tagType | (directoryType << 8);

		FreeImage_SetTagID(tag, tag_id);
		FreeImage_SetTagLength(tag, tagByteCount);

		// allocate a buffer to store the tag value
		BYTE *iptc_value = (BYTE*)malloc((tagByteCount + 1) * sizeof(BYTE));
		memset(iptc_value, 0, (tagByteCount + 1) * sizeof(BYTE));

		// get the tag value

		switch (tag_id) {
			case TAG_RECORD_VERSION:
			{
				// short
				FreeImage_SetTagType(tag, FIDT_SSHORT);
				FreeImage_SetTagCount(tag, 1);
				short *pvalue = (short*)&iptc_value[0];
				*pvalue = (short)((profile[offset] << 8) | profile[offset + 1]);
				FreeImage_SetTagValue(tag, pvalue);
				break;
			}

			case TAG_RELEASE_DATE:
			case TAG_DATE_CREATED:
				// Date object
			case TAG_RELEASE_TIME:
			case TAG_TIME_CREATED:
				// time
			default:
			{
				// string
				FreeImage_SetTagType(tag, FIDT_ASCII);
				FreeImage_SetTagCount(tag, tagByteCount);
				for(int i = 0; i < tagByteCount; i++) {
					iptc_value[i] = profile[offset + i];
				}
				iptc_value[tagByteCount] = '\0';
				FreeImage_SetTagValue(tag, (char*)&iptc_value[0]);
				break;
			}
		}

		if(tag_id == TAG_SUPPLEMENTAL_CATEGORIES) {
			// concatenate the categories
			if(SupplementalCategory.length() == 0) {
				SupplementalCategory.append((char*)iptc_value);
			} else {
				SupplementalCategory.append(",");
				SupplementalCategory.append((char*)iptc_value);
			}
		}
		else if(tag_id == TAG_KEYWORDS) {
			// concatenate the keywords
			if(Keywords.length() == 0) {
				Keywords.append((char*)iptc_value);
			} else {
				Keywords.append(",");
				Keywords.append((char*)iptc_value);
			}
		}
		else {
			// get the tag key and description
			const char *key = tag_lib.getTagFieldName(TagLib::IPTC, tag_id);
			FreeImage_SetTagKey(tag, key);
			const char *description = tag_lib.getTagDescription(TagLib::IPTC, tag_id);
			FreeImage_SetTagDescription(tag, description);

			// store the tag
			if(key) {
				FreeImage_SetMetadata(FIMD_IPTC, dib, key, tag);
			}
		}

		free(iptc_value);

        // next tag
		offset += tagByteCount;

    }

	// store the 'keywords' tag
	if(Keywords.length()) {
		FreeImage_SetTagType(tag, FIDT_ASCII);
		FreeImage_SetTagID(tag, TAG_KEYWORDS);
		FreeImage_SetTagKey(tag, tag_lib.getTagFieldName(TagLib::IPTC, TAG_KEYWORDS));
		FreeImage_SetTagDescription(tag, tag_lib.getTagDescription(TagLib::IPTC, TAG_KEYWORDS));
		FreeImage_SetTagLength(tag, Keywords.length());
		FreeImage_SetTagCount(tag, Keywords.length());
		FreeImage_SetTagValue(tag, (char*)Keywords.c_str());
		FreeImage_SetMetadata(FIMD_IPTC, dib, FreeImage_GetTagKey(tag), tag);
	}

	// store the 'supplemental category' tag
	if(SupplementalCategory.length()) {
		FreeImage_SetTagType(tag, FIDT_ASCII);
		FreeImage_SetTagID(tag, TAG_SUPPLEMENTAL_CATEGORIES);
		FreeImage_SetTagKey(tag, tag_lib.getTagFieldName(TagLib::IPTC, TAG_SUPPLEMENTAL_CATEGORIES));
		FreeImage_SetTagDescription(tag, tag_lib.getTagDescription(TagLib::IPTC, TAG_SUPPLEMENTAL_CATEGORIES));
		FreeImage_SetTagLength(tag, SupplementalCategory.length());
		FreeImage_SetTagCount(tag, SupplementalCategory.length());
		FreeImage_SetTagValue(tag, (char*)SupplementalCategory.c_str());
		FreeImage_SetMetadata(FIMD_IPTC, dib, FreeImage_GetTagKey(tag), tag);
	}

	// delete the tag

	FreeImage_DeleteTag(tag);

	return TRUE;
}

