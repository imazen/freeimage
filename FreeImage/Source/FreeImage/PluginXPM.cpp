// ==========================================================
// XPM Loader
//
// Design and implementation by 
// - Karl-Heinz Bussian (khbussian@moss.de)
// - Hervé Drolon (drolon@infonie.fr)
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

// Notes on implementation :
// -------------------------
// Portion of this code was adapted from the GDAL open source library 
// built and maintained by Frank Wamerdam (http://gdal.velocet.ca/)
/*
 ******************************************************************************
 * Copyright (c) 2002, Frank Warmerdam
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 ******************************************************************************
*/
// - Plugin add support for 2char images (i.e. 24-bit) 
//   by Karl-Heinz Bussian (khbussian@aol.com)
//
// ==========================================================

#include "FreeImage.h"
#include "Utilities.h"

// ==========================================================
// Plugin Interface
// ==========================================================
static int s_format_id;

// ==========================================================
// StringList manipulation functions
// ==========================================================

#define CSLT_HONOURSTRINGS      0x0001
#define CSLT_ALLOWEMPTYTOKENS   0x0002
#define CSLT_PRESERVEQUOTES     0x0004
#define CSLT_PRESERVEESCAPES    0x0008

/**
Free all memory used by a StringList.
*/
static void 
CSLDestroy(char **papszStrList) {
    char **papszPtr;

    if(papszStrList) {
        papszPtr = papszStrList;
        while(*papszPtr != NULL) {
            free(*papszPtr);
            papszPtr++;
        }
        free(papszStrList);
    }
}

/**
Return the number of lines in a Stringlist.
*/
static int 
CSLCount(char **papszStrList) {
    int nItems=0;

    if (papszStrList) {
        while(*papszStrList != NULL) {
            nItems++;
            papszStrList++;
        }
    }
    return nItems;
}


/**
Append a string to a StringList and return a pointer to the modified StringList.
If the input StringList is NULL, then a new StringList is created.
*/
static char **
CSLAddString(char **papszStrList, const char *pszNewString) {
    int nItems=0;

    if (pszNewString == NULL)
        return papszStrList;    // Nothing to do!

    // allocate room for the new string
    if (papszStrList == NULL) {
        papszStrList = (char **)malloc(2 * sizeof(char *));
        if (papszStrList == NULL) 
			return NULL;
        memset(papszStrList, 0, sizeof(char *) * 2);
    }
    else {
        nItems = CSLCount(papszStrList);
        papszStrList = (char **)realloc(papszStrList, (nItems + 2) * sizeof(char *));
    }

    // copy the string in the list
    papszStrList[nItems] = (char *)malloc(strlen(pszNewString)+1);
    if (papszStrList[nItems]) 
		strcpy(papszStrList[nItems], pszNewString);
    papszStrList[nItems+1] = NULL;

    return papszStrList;
}


/**
A string tokenizer.
*/
static char **
CSLTokenizeString2(const char *pszString, const char *pszDelimiters, int nCSLTFlags) {
    char  **papszRetList = NULL;
    char   *pszToken;
    int     nTokenMax, nTokenLen;
    int     bHonourStrings = (nCSLTFlags & CSLT_HONOURSTRINGS);
    int     bAllowEmptyTokens = (nCSLTFlags & CSLT_ALLOWEMPTYTOKENS);

    pszToken = (char *)malloc(10);
    nTokenMax = 10;

    while( pszString != NULL && *pszString != '\0' ) {
        int  bInString = FALSE;

        nTokenLen = 0;

        // try to find the next delimeter, marking end of token
        for( ; *pszString != '\0'; pszString++ ) {

            // end if this is a delimeter skip it and break
            if( !bInString && strchr(pszDelimiters, *pszString) != NULL ) {
                pszString++;
                break;
            }

            // if this is a quote, and we are honouring constant strings, 
			// then process the constant strings, with out delim but don't copy over the quotes
            if( bHonourStrings && *pszString == '"' ) {
                if( nCSLTFlags & CSLT_PRESERVEQUOTES ) {
                    pszToken[nTokenLen] = *pszString;
                    nTokenLen++;
                }

                if( bInString ) {
                    bInString = FALSE;
                    continue;
                }
                else {
                    bInString = TRUE;
                    continue;
                }
            }

            // within string constants we allow for escaped quotes, 
			// but in processing them we will unescape the quotes
            if( bInString && pszString[0] == '\\' && pszString[1] == '"' ) {
                if( nCSLTFlags & CSLT_PRESERVEESCAPES ) {
                    pszToken[nTokenLen] = *pszString;
                    nTokenLen++;
                }

                pszString++;
            }

            // within string constants a \\ sequence reduces to a \ sequence
            else if( bInString && pszString[0] == '\\' && pszString[1] == '\\' ) {
                if( nCSLTFlags & CSLT_PRESERVEESCAPES ) {
                    pszToken[nTokenLen] = *pszString;
                    nTokenLen++;
                }
                pszString++;
            }

            if( nTokenLen >= nTokenMax-2 ) {
                nTokenMax = nTokenMax * 2 + 10;
                pszToken = (char *) realloc(pszToken, nTokenMax);
                if (pszToken == NULL) {
                    CSLDestroy(papszRetList);
                    return NULL;
                }
            }

            pszToken[nTokenLen] = *pszString;
            nTokenLen++;
        }

        pszToken[nTokenLen] = '\0';

        if (pszToken[0] != '\0' || bAllowEmptyTokens) {
            papszRetList = CSLAddString( papszRetList, pszToken );
        }

        // if the last token is an empty token, then we have to catch
		// it now, otherwise we won't reenter the loop and it will be lost         
        if ( *pszString == '\0' && bAllowEmptyTokens && strchr(pszDelimiters, *(pszString-1)) ) {
            papszRetList = CSLAddString( papszRetList, "" );
        }
    }

    if (papszRetList == NULL) {
        papszRetList = (char **)malloc(sizeof(char *));
        papszRetList[0] = NULL;
    }

    free(pszToken);
    return papszRetList;
}

/**
Tokenizes a string and returns a StringList with one string for each token.
*/
static char **
CSLTokenizeString(const char *pszString) {
    return CSLTokenizeString2(pszString, " \t", CSLT_HONOURSTRINGS);
}

// ==========================================================
// XPM parser
// ==========================================================

/**
Main funtion to parse the XPM-Image into a 8-bit or 24-bit image.
If an error occurs, the reason is reported by the FreeImage_OutputMessageProc 
function and NULL is returned.

@param pszInput Whole XPM loaded into memory
@param rgbmode Converts pixels to RGB if rgbmode = true, else converts pixel to BGR
@param pnXSize returns image width
@param pnYSize returns image height
@param colorCount returns the number of colors used, if < 0 -> 24 bit
@param red colorCount palette colors 0..255, used only if colorCount > 0
@param green
@param blue
@return Returns the XPM image if successful, returns NULL otherwise.
*/
static BYTE * 
ParseXPM(const char *pszInput, bool rgbmode, int *pnXSize, int *pnYSize, int *colorCount, BYTE *red, BYTE *green, BYTE *blue) {
   /*
    * Parse input into an array of strings from within the first
    * initializer (list of comma separated strings in braces).
    */
    int            iColor, iLine, iPixel, i, iPos;
    int            anCharLookup[256];
    int            nColorCount, nCharsPerPixel, nsize;
    char         **papszXPMList = NULL;
    const char    *pszNext = pszInput;
    BYTE *pabyImage;
    BYTE *colCharLookup = NULL;

    // skip till after open brace
    while (*pszNext != '\0' && *pszNext != '{')
        pszNext++;

    if (*pszNext == '\0')
        return NULL;

    pszNext++;

    // read lines till close brace

    while (*pszNext != '\0' && *pszNext != '}') {
        // skip whole comment
        if (strncmp(pszNext, "/*" , 2) == 0) {
            pszNext += 2;
            while (*pszNext != '\0' && strncmp(pszNext, "*/", 2) != 0)
                pszNext++;
        }

        // reading string constants
        else if (*pszNext == '"') {
            char   *pszLine;

            pszNext++;
            i = 0;

            while (pszNext[i] != '\0' && pszNext[i] != '"')
                i++;

            pszLine = (char *) malloc(i + 1);
            if (pszLine == NULL) {
                CSLDestroy( papszXPMList );
                return NULL;
            }

            strncpy (pszLine, pszNext, i);
            pszLine[i] = '\0';

            papszXPMList = CSLAddString( papszXPMList, pszLine );
            free(pszLine);

            pszNext = pszNext + i + 1;
        }
        // just ignore everything else (whitespace, commas, newlines, etc)
        else
            pszNext++;
    }

    if (CSLCount(papszXPMList) < 3 || *pszNext != '}') {
        CSLDestroy( papszXPMList );
        return NULL;
    }


    /*
     * Get the image information.
     */
    if (sscanf(papszXPMList[0], "%d %d %d %d", pnXSize, pnYSize, &nColorCount, &nCharsPerPixel) != 4) {
		FreeImage_OutputMessageProc(s_format_id, "Image definition (%s) not well formed.", papszXPMList[0]);
        CSLDestroy(papszXPMList);
        return NULL;
    }

    if (nCharsPerPixel != 1 && nCharsPerPixel != 2) {
		FreeImage_OutputMessageProc(s_format_id, "Only one/two character per pixel XPM images supported.");
        CSLDestroy(papszXPMList);
        return NULL;
    }


    if (nCharsPerPixel == 2) {
        /* Lookup-Array:
         * [ncharsPerPixel,red, green, blue] for every color
         */
        colCharLookup = (BYTE *)malloc((nCharsPerPixel+3)*nColorCount);
        if (colCharLookup == NULL) {
			FreeImage_OutputMessageProc(s_format_id, "Can't allocate %d color lookup buffer.", nColorCount);
            CSLDestroy(papszXPMList);
            return NULL;
        }
        *colorCount = -nColorCount;   /* indicate rgb image */
    }
    else {
        /* simple case:
         * character indexes into this Array for color value
         */
        for (i = 0; i < 256; i++)
            anCharLookup[i] = -1;

        *colorCount = nColorCount;
    }


    /*
     * Parse out colors.
     */
    for (iColor=0; iColor < nColorCount; iColor++) {
        char **papszTokens = CSLTokenizeString(papszXPMList[iColor+1]+nCharsPerPixel);
        int    nRed, nGreen, nBlue, iPos;

        nsize = CSLCount(papszTokens);

        /* use only the color definition part "c #xxxxxx" */
        for (i=0; i < nsize-1; i++) {
            if (strcmp(papszTokens[i], "c") == 0) {
                nsize = ((i+1) < nsize) ? i+1 : nsize;
                break;
            }
        }

        if ((nsize < 1) || strcmp(papszTokens[nsize-1], "c") != 0) {
			FreeImage_OutputMessageProc(s_format_id, "Unknown color definition (%s) in XPM header.", papszXPMList[iColor+1]);
            CSLDestroy( papszXPMList );
            if (colCharLookup)
                free(colCharLookup);
            return NULL;
        }
        if (strcmp(papszTokens[nsize], "None") == 0) {
            nRed   = 255;   /* TODO: should specify transp/background color */
            nGreen = 255;
            nBlue  = 255;
        }
        else if (strcmp(papszTokens[nsize], "#000") == 0) {   /* special case */
            nRed   = 0;
            nGreen = 0;
            nBlue  = 0;
        }
        else if( sscanf(papszTokens[nsize], "#%02x%02x%02x", &nRed, &nGreen, &nBlue ) != 3) {
			FreeImage_OutputMessageProc(s_format_id, "Ill formed color definition (%s) in XPM header.", papszXPMList[iColor+1]);
            CSLDestroy( papszXPMList );
            if (colCharLookup)
                free(colCharLookup);
            return NULL;
        }

        if (nCharsPerPixel == 2) {
            iPos = iColor * (nCharsPerPixel+3);
            strncpy((char *)&colCharLookup[iPos], papszXPMList[iColor+1], nCharsPerPixel);
            iPos += nCharsPerPixel;
            colCharLookup[iPos++] = nRed & 0xff;
            colCharLookup[iPos++] = nGreen & 0xff;
            colCharLookup[iPos++] = nBlue & 0xff;
        }
        else {
            anCharLookup[papszXPMList[iColor+1][0]] = iColor;

            red[iColor]   = nRed & 0xff;
            green[iColor] = nGreen & 0xff;
            blue[iColor]  = nBlue & 0xff;
        }
        CSLDestroy(papszTokens);
    }

    if (nCharsPerPixel == 2) {
        pabyImage = (BYTE *) malloc(*pnXSize * *pnYSize * 3);
        if (pabyImage) memset(pabyImage, 0xff, *pnXSize * *pnYSize * 3);
    }
    else {
        pabyImage = (BYTE *) malloc(*pnXSize * *pnYSize);
        if (pabyImage) memset(pabyImage, 0, *pnXSize * *pnYSize);
    }
    if (pabyImage == NULL) {
		FreeImage_OutputMessageProc(s_format_id, "Insufficient memory for %dx%d XPM image buffer.", *pnXSize, *pnYSize);
        if (colCharLookup) free(colCharLookup);
        CSLDestroy(papszXPMList);
        return NULL;
    }


    /* Prepare for scanline parse of Image nCharsPerPixel>1 */
    nsize = nCharsPerPixel * *pnXSize;
    if (rgbmode) {
        // index into colCharLookup
        anCharLookup[0] = 2;
        anCharLookup[1] = 3;
        anCharLookup[2] = 4;
    }
    else  {
        // bgr 
        anCharLookup[0] = 4;
        anCharLookup[1] = 3;
        anCharLookup[2] = 2;
    }

    /*
     * Parse image.
     */
    for (iLine=0; iLine < *pnYSize; iLine++) {
        const char *pszInLine = papszXPMList[iLine + nColorCount + 1];
        int iOff, iLast=0;

        if (pszInLine == NULL) {
            free(pabyImage);
            CSLDestroy( papszXPMList );
			FreeImage_OutputMessageProc(s_format_id, "Insufficient imagery lines in XPM image.");
            if (colCharLookup) free(colCharLookup);
            return NULL;
        }


        if (nCharsPerPixel == 1) {
            for (iPixel=0; pszInLine[iPixel]!='\0' && iPixel<*pnXSize; iPixel++) {
                int nPixelValue = anCharLookup[pszInLine[iPixel]];
                if (nPixelValue != -1)
                    pabyImage[iLine * *pnXSize + iPixel] = nPixelValue;
            }
        }
        else {
            iPixel = 3*iLine * *pnXSize;   // Pixel offset current scanline
            for (iPos=0; pszInLine[iPos]!='\0' && iPos<nsize; iPos+=nCharsPerPixel) {
				if (pszInLine[iPos] == colCharLookup[iLast] && pszInLine[iPos+1] == colCharLookup[iLast+1]) {
                    pabyImage[iPixel]   = colCharLookup[iLast+anCharLookup[0]];
                    pabyImage[iPixel+1] = colCharLookup[iLast+anCharLookup[1]];
                    pabyImage[iPixel+2] = colCharLookup[iLast+anCharLookup[2]];
					iPixel += 3;
					continue;
				}

                for (iOff=i=0; i < nColorCount; i++, iOff += 3+nCharsPerPixel) {
                    if (pszInLine[iPos] == colCharLookup[iOff] && pszInLine[iPos+1] == colCharLookup[iOff+1]) {
                        pabyImage[iPixel]   = colCharLookup[iOff+anCharLookup[0]];
                        pabyImage[iPixel+1] = colCharLookup[iOff+anCharLookup[1]];
                        pabyImage[iPixel+2] = colCharLookup[iOff+anCharLookup[2]];
						iLast = iOff;
                        break;
                    }
                }
                iPixel += 3;
            }
        }
    }

    CSLDestroy(papszXPMList);
    if (colCharLookup) 
		free(colCharLookup);

    return pabyImage;
}

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "XPM";
}

static const char * DLL_CALLCONV
Description() {
	return "X11 Pixmap Format";
}

static const char * DLL_CALLCONV
Extension() {
	return "xpm";
}

static const char * DLL_CALLCONV
RegExpr() {
	return "^[ \\t]*/\\* XPM \\*/[ \\t]$";
}

static const char * DLL_CALLCONV
MimeType() {
	return "image/xpm";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {
	char buffer[256];

	// checks the first 256 characters for the magic string
	int count = io->read_proc(buffer, 1, 256, handle);
	if(count <= 9) return FALSE;
	for(int i = 0; i < (count - 9); i++) {
		if(strncmp(&buffer[i], "/* XPM */", 9) == 0)
			return TRUE;
	}
	return FALSE;
}

static BOOL DLL_CALLCONV
SupportsExportDepth(int depth) {
	return FALSE;
}

static BOOL DLL_CALLCONV 
SupportsExportType(FREE_IMAGE_TYPE type) {
	return FALSE;
}

// ----------------------------------------------------------

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *data) {
    BYTE *image = NULL;
    FIBITMAP *dib = NULL;

    if (!handle) return NULL;

    try {
        int x, y, width, height, ncolors;
        BYTE red[256], green[256], blue[256];
        long nOffset, nEnd, nSize;
		char *szInput = NULL;

        // Calculate the size of input buffer
		nOffset = io->tell_proc(handle);
        io->seek_proc(handle, nOffset, SEEK_END);
        nEnd = io->tell_proc(handle);
        io->seek_proc(handle, nOffset, SEEK_SET);

        // allocate input buffer
		nSize = nEnd - nOffset;
        szInput = (char*)malloc(nSize+2);
        if (!szInput)
            throw "can't allocate XPM read buffer";

        // read input stream into the whole buffer
		nEnd = io->read_proc(szInput, 1, nSize, handle);
        if(nEnd < nSize) {
            free(szInput);
            throw "can't read XPM data";
        }
		szInput[nEnd] = 0;


        // Use XPM parser to parse input stream. Ask for BGR parsing.
		image = ParseXPM(szInput, false, &width, &height, &ncolors, red, green, blue);
		free(szInput);
		if(image == NULL)
            throw "can't read XPM format";

        // allocate and fill the dib with image data

        if (ncolors < 0) {
			dib = FreeImage_Allocate(width, height, 24, 0xFF, 0xFF00, 0xFF0000);
		} else {
			dib = FreeImage_Allocate(width, height, 8);
		}
		if(dib == NULL)
			throw "DIB allocation failed";

		if (ncolors > 0) {
            // write the palette data
            RGBQUAD *pal = FreeImage_GetPalette(dib);
            for (int i = 0; i < ncolors; i++) {
                pal[i].rgbRed = red[i];
                pal[i].rgbGreen = green[i];
                pal[i].rgbBlue = blue[i];
            }

			// write the bitmap data
            for (y = 0; y < height; y++) {
				BYTE *src_bits = &image[y * width];
				BYTE *dst_bits = FreeImage_GetScanLine(dib, height - 1 - y);
                memcpy(dst_bits, src_bits, width);
            }
        }
        else {
			// write the bitmap data
            for (y = 0; y < height; y++) {
				BYTE *src_bits = &image[y * 3 * width];
				BYTE *dst_bits = FreeImage_GetScanLine(dib, height - 1 - y);
				for(x = 0; x < width; x++) {
					dst_bits[0] = src_bits[0];
					dst_bits[1] = src_bits[1];
					dst_bits[2] = src_bits[2];
					src_bits += 3; dst_bits += 3;
				}
			}
		}

		free(image);

		return dib;
	}
    catch(const char *text) {
       FreeImage_OutputMessageProc(s_format_id, text);

       if (image != NULL)
           free(image);

       return NULL;
    }
    return NULL;
}


// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitXPM(Plugin *plugin, int format_id)
{
    s_format_id = format_id;

	plugin->format_proc = Format;
	plugin->description_proc = Description;
	plugin->extension_proc = Extension;
	plugin->regexpr_proc = RegExpr;
	plugin->open_proc = NULL;
	plugin->close_proc = NULL;
	plugin->pagecount_proc = NULL;
	plugin->pagecapability_proc = NULL;
	plugin->load_proc = Load;
	plugin->save_proc = NULL;
	plugin->validate_proc = Validate;
	plugin->mime_proc = MimeType;
	plugin->supports_export_bpp_proc = SupportsExportDepth;
	plugin->supports_export_type_proc = SupportsExportType;
	plugin->supports_icc_profiles_proc = NULL;
}

