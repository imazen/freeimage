// ==========================================================
// fipMemoryIO class implementation
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

#include "FreeImagePlus.h"
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <stdio.h>

fipMemoryIO::fipMemoryIO(BYTE *data, DWORD size_in_bytes) 
: _start(data), _cp(data), _size(size_in_bytes), _delete_me(FALSE) {
	read_proc  = _ReadProc;
	write_proc = _WriteProc;
	tell_proc  = _TellProc;
	seek_proc  = _SeekProc;
}

BOOL fipMemoryIO::loadFile(const char *lpszPathName) {
	struct stat buf;
	int result;

	// release previous buffer
	if(_delete_me) {
		free(_start);
		_start = NULL;
		_delete_me = FALSE;
	}

	// get data associated with lpszPathName
	result = stat(lpszPathName, &buf);
	if(result == 0) {
		// allocate a memory buffer and load temporary data
		BYTE *mem_buffer = (BYTE*)malloc(buf.st_size * sizeof(BYTE));
		if(mem_buffer) {
			FILE *stream = fopen(lpszPathName, "rb");
			if(stream) {
				fread(mem_buffer, sizeof(BYTE), buf.st_size, stream);
				fclose(stream);

				// Initialize IO
				_start = mem_buffer;
				_cp = mem_buffer;
				_size = buf.st_size;

				// remember to delete the buffer
				_delete_me = TRUE;

				return TRUE;
			}
		}
	}

	return FALSE;
}

fipMemoryIO::~fipMemoryIO() { 
	if(_delete_me)
		free(_start);
}

BOOL fipMemoryIO::isValidIO() {
	if((_start != NULL) && (_size > 0)) {
		return TRUE;
	}

	return FALSE;
}

FREE_IMAGE_FORMAT fipMemoryIO::getFileType() {
	if((_start != NULL) && (_size > 0)) {
		return FreeImage_GetFileTypeFromHandle(this, (fi_handle)this);
	}

	return FIF_UNKNOWN;
}

unsigned FIP_CALLCONV fipMemoryIO::_ReadProc(void *buffer, unsigned size, unsigned count, fi_handle handle) {
    fipMemoryIO *memIO = (fipMemoryIO*)handle;
    
    BYTE *tmp = (BYTE *)buffer;

    for (unsigned c = 0; c < count; c++) {
        memcpy(tmp, memIO->_cp, size);

        memIO->_cp = memIO->_cp + size;

        tmp += size;
    }

    return count;
}

unsigned FIP_CALLCONV fipMemoryIO::_WriteProc(void *buffer, unsigned size, unsigned count, fi_handle handle) {
    assert( false );
    return size;
}

int FIP_CALLCONV fipMemoryIO::_SeekProc(fi_handle handle, long offset, int origin) {
    assert(origin != SEEK_END);

    fipMemoryIO *memIO = (fipMemoryIO*)handle;

    if (origin == SEEK_SET) 
        memIO->_cp = memIO->_start + offset;
    else
        memIO->_cp = memIO->_cp + offset;

    return 0;
}

long FIP_CALLCONV fipMemoryIO::_TellProc(fi_handle handle) {
    fipMemoryIO *memIO = (fipMemoryIO*)handle;

    return memIO->_cp - memIO->_start;
}
