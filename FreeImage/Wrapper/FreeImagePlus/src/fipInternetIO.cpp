// ==========================================================
// fipInternetIO class implementation
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

#ifdef WIN32

#include "FreeImagePlus.h"
#include <assert.h>

#pragma comment (lib, "WinInet")

fipInternetIO::fipInternetIO() : _hInternet(0) {
	DWORD dwError;
	const char *sUserAgent = "FreeImage";
	DWORD dwFlags = INTERNET_FLAG_TRANSFER_BINARY | INTERNET_FLAG_RELOAD | INTERNET_FLAG_NO_CACHE_WRITE | INTERNET_FLAG_PRAGMA_NOCACHE | INTERNET_FLAG_KEEP_CONNECTION;

    // Initialize the Win32 Internet functions 
    _hInternet = InternetOpen(sUserAgent, 
        INTERNET_OPEN_TYPE_PRECONFIG, // Use registry settings. 
        NULL, // Proxy name. NULL indicates use default.
        NULL, // List of local servers. NULL indicates default. 
        0) ;

    dwError = GetLastError();
}

fipInternetIO::~fipInternetIO() { 
    // Close this Internet session
    InternetCloseHandle(_hInternet);
}

BOOL fipInternetIO::downloadFile(char *szURL) {
    HINTERNET hHttpFile;
    char szSizeBuffer[32];
    DWORD dwLengthSizeBuffer = sizeof(szSizeBuffer); 
    DWORD dwFileSize;
    DWORD dwBytesRead;
    BOOL bSuccessful;

	const int DEFAULT_FILE_SIZE = 0x10000;	// Max default size 64 Ko

    // Open the URL and get a handle for HTTP file
    hHttpFile = InternetOpenUrl(_hInternet, szURL, NULL, 0, 0, 0);

	if(hHttpFile) {    
		// Free previous buffer
		if(_start)
			free(_start);

		// Getting the size of HTTP Files
		BOOL bQuery = HttpQueryInfo(hHttpFile,HTTP_QUERY_CONTENT_LENGTH, szSizeBuffer, &dwLengthSizeBuffer, NULL) ;

		if(bQuery==TRUE) {    
			// Allocating the memory space for HTTP file contents
			dwFileSize = atol(szSizeBuffer);
		}
		else {
			dwFileSize = DEFAULT_FILE_SIZE;
		}
		_start = (BYTE*)malloc(dwFileSize * sizeof(BYTE));
		if(!_start)
			return FALSE;
		_cp = _start;
		_delete_me = TRUE;

		// Read the HTTP file 
		BOOL bRead;
		DWORD dwPos = 0;
		do {
			Sleep(0);
			bRead = ::InternetReadFile(hHttpFile, _start+dwPos, dwFileSize-dwPos, &dwBytesRead); 
			if(dwBytesRead == DEFAULT_FILE_SIZE) {
				// augment buffer size
				dwFileSize += DEFAULT_FILE_SIZE;
				BYTE *p = (BYTE*)malloc(dwFileSize * sizeof(BYTE));
				memcpy(p, _start, (dwFileSize - DEFAULT_FILE_SIZE) * sizeof(BYTE));
				free(_start);
				_start = p;
				_cp = _start;
			}
			dwPos += dwBytesRead;
		} while (bRead && (dwBytesRead > 0));

		if (bRead) 
			bSuccessful = TRUE;

		// Save the buffer size
		_size = dwPos;
		
		 // Close the connection.
		InternetCloseHandle(hHttpFile);
	}
	else {
        // Connection failed.
        bSuccessful = FALSE;
    } 
    
	return bSuccessful;
}


#endif // WIN32
