#include "../FreeImagePlus.h"
#include <sys/types.h>
#include <sys/stat.h>
#include <stdio.h>

#ifdef WIN32
#pragma comment (lib, "WinInet")
#endif 

/**
 Test loading from a memory buffer
*/
BOOL testMemoryHandle(char *lpszPathName) {
	struct _stat buf;
	int result;
	BOOL bOK = FALSE;

	// Get data associated with lpszPathName
	result = _stat(lpszPathName, &buf);
	if(result == 0) {
		// allocate a memory buffer and load temporary data
		BYTE *mem_buffer = (BYTE*)malloc(buf.st_size * sizeof(BYTE));
		if(mem_buffer) {
			FILE *stream = fopen(lpszPathName, "rb");
			fread(mem_buffer, sizeof(BYTE), buf.st_size, stream);
			fclose(stream);

			// create a fipMemory object and attach it to mem_buffer
			fipMemoryIO memIO(mem_buffer, buf.st_size);

			// load image from the memory buffer
			fipImage Image;
			bOK = Image.loadFromHandle(&memIO, (fi_handle)&memIO);

			// free temporary memory buffer
			free(mem_buffer);

			// save to a png file
			bOK &= Image.save("testMemoryHandle.png");

			return bOK;
		}
	}

	return bOK;
}

BOOL testMemoryFileHandle(char *lpszPathName) {
	BOOL bOK = FALSE;

	// create a fipMemory object
	fipMemoryIO memIO;

	// load image from lpszPathName
	memIO.loadFile(lpszPathName);

	// load image from the memory buffer
	fipImage Image;
	bOK = Image.loadFromHandle(&memIO, (fi_handle)&memIO);

	// save to a png file
	bOK &= Image.save("testMemoryFileHandle.png");

	return bOK;
}


BOOL testInternetHandle(char *lpszURL, char *lpszLocalName) {
	BOOL bOK = FALSE;

	// create a fipInternetIO object
	fipInternetIO inetIO;

	// download image file to the memory buffer
	bOK = inetIO.downloadFile(lpszURL);
	
	if(bOK) {
		// load image from this IO buffer 
		fipImage image;
		bOK &= image.loadFromHandle(&inetIO, (fi_handle)&inetIO, 0);

		// save image
		if(inetIO.getFileType() == FIF_JPEG) {
			// preserve JPEG quality
			bOK &= image.save(lpszLocalName, JPEG_QUALITYSUPERB);
		} else {
			bOK &= image.save(lpszLocalName, 0);
		}
	}
	return bOK;
}

main()
{
	BOOL bResult = FALSE;
	char *lpszPathName = "test.jpeg";
	//char *lpszURL = "http://freeimage.sourceforge.net/images/freeimage.jpg";
	//char *lpszURL = "http://sqez.home.att.net/kodak/kodim02.png";
	char *lpszURL = "http://sqe2.home.att.net/kodak/kodim22.png";

	bResult = testInternetHandle(lpszURL, "kodim22.png");

	//bResult = testMemoryHandle(lpszPathName);

	//bResult = testMemoryFileHandle(lpszPathName);

	return 0;
}