// ==========================================================
// ZLib library interface
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

#include "../ZLib/zlib.h"
#include "../ZLib/zutil.h"
#include "FreeImage.h"
#include "Utilities.h"

/**
Compresses a source buffer into a target buffer, using the ZLib library. 
Upon entry, target_size is the total size of the destination buffer, 
which must be at least 0.1% larger than source_size plus 12 bytes. 

@param target Destination buffer
@param target_size Size of the destination buffer, in bytes
@param source Source buffer
@param source_size Size of the source buffer, in bytes
@return Returns the actual size of the compressed buffer, returns 0 if an error occured
@see FreeImage_ZLibUncompress
*/
DWORD DLL_CALLCONV 
FreeImage_ZLibCompress(BYTE *target, DWORD target_size, BYTE *source, DWORD source_size) {
	unsigned long dest_len = target_size;

	int zerr = compress(target, &dest_len, source, source_size);
	switch(zerr) {
		case Z_MEM_ERROR:	// not enough memory
		case Z_BUF_ERROR:	// not enough room in the output buffer
			FreeImage_OutputMessageProc(FIF_UNKNOWN, "Zlib error : %s", zError(zerr));
			return 0;
		case Z_OK:
			return dest_len;
	}

	return 0;
}

/**
Decompresses a source buffer into a target buffer, using the ZLib library. 
Upon entry, target_size is the total size of the destination buffer, 
which must be large enough to hold the entire uncompressed data. 
The size of the uncompressed data must have been saved previously by the compressor 
and transmitted to the decompressor by some mechanism outside the scope of this 
compression library.

@param target Destination buffer
@param target_size Size of the destination buffer, in bytes
@param source Source buffer
@param source_size Size of the source buffer, in bytes
@return Returns the actual size of the uncompressed buffer, returns 0 if an error occured
@see FreeImage_ZLibCompress
*/
DWORD DLL_CALLCONV 
FreeImage_ZLibUncompress(BYTE *target, DWORD target_size, BYTE *source, DWORD source_size) {
	unsigned long dest_len = target_size;

	int zerr = uncompress(target, &dest_len, source, source_size);
	switch(zerr) {
		case Z_MEM_ERROR:	// not enough memory
		case Z_BUF_ERROR:	// not enough room in the output buffer
		case Z_DATA_ERROR:	// input data was corrupted
			FreeImage_OutputMessageProc(FIF_UNKNOWN, "Zlib error : %s", zError(zerr));
			return 0;
		case Z_OK:
			return dest_len;
	}

	return 0;
}

/**
Compresses a source buffer into a target buffer, using the ZLib library. 
On success, the target buffer contains a GZIP compatible layout.
Upon entry, target_size is the total size of the destination buffer, 
which must be at least 0.1% larger than source_size plus 24 bytes. 

@param target Destination buffer
@param target_size Size of the destination buffer, in bytes
@param source Source buffer
@param source_size Size of the source buffer, in bytes
@return Returns the actual size of the compressed buffer, returns 0 if an error occured
@see FreeImage_ZLibCompress
*/
DWORD DLL_CALLCONV 
FreeImage_ZLibGZip(BYTE *target, DWORD target_size, BYTE *source, DWORD source_size) {
	unsigned long dest_len = target_size - 12;
    unsigned long crc = crc32(0L, NULL, 0);

    // set up header (stolen from zlib/gzipio.c)
    sprintf((char *)target, "%c%c%c%c%c%c%c%c", 0x1f, 0x8b,
         Z_DEFLATED, 0 /*flags*/, 0,0,0,0 /*time*/);
    int zerr = compress2(target + 8, &dest_len, source, source_size, Z_BEST_COMPRESSION);
	switch(zerr) {
		case Z_MEM_ERROR:	// not enough memory
		case Z_BUF_ERROR:	// not enough room in the output buffer
			FreeImage_OutputMessageProc(FIF_UNKNOWN, "Zlib error : %s", zError(zerr));
			return 0;
        case Z_OK: {
            // patch header, setup crc and length (stolen from mod_trace_output)
            BYTE *p = target + 8; *p++ = 2; *p = OS_CODE; // xflags, os_code
 	        crc = crc32(crc, source, source_size);
	        memcpy(target + 4 + dest_len, &crc, 4);
	        memcpy(target + 8 + dest_len, &source_size, 4);
            return dest_len + 12;
        }
	}
	return 0;
}

/**
Update a running crc from source and return the updated crc, using the ZLib library.
If source is NULL, this function returns the required initial value for the crc.

@param crc Running crc value
@param source Source buffer
@param source_size Size of the source buffer, in bytes
@return Returns the new crc value
*/
DWORD DLL_CALLCONV 
FreeImage_ZLibCRC32(DWORD crc, BYTE *source, DWORD source_size) {

    return crc32(crc, source, source_size);
}



