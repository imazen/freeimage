// ==========================================================
// G3 Fax Loader
//
// Design and implementation by
// - Petr Pytelka <pyta@lightcomp.com>
// Based on G3 FAX decoder from GIMP and mgetty:
// - Parts Copyright (C) 1995 Gert Doering
// - Parts Copyright (C) 1995 Spencer Kimball and Peter Mattis
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

// ==========================================================
// Plugin Interface
// ==========================================================

static int s_format_id;

// ==========================================================
// Internal structures and functions
// ==========================================================

// Definitions from original g3.h 

/*
 nr_bits is set to ( bit_length MOD FBITS ) by build_g3_tree,
 nr_pels is the number of pixels to write for that code,
 bit_code is the code itself (msb2lsb), and bit_length its length
*/ 
struct g3code { int nr_bits, nr_pels, bit_code, bit_length; };

#define g3_leaf g3code

/*
 The number of bits looked up simultaneously determines the amount
 of memory used by the program - some values:
 10 bits : 87 Kbytes, 8 bits : 20 Kbytes
 5 bits :  6 Kbytes, 1 bit  :  4 Kbytes
 - naturally, using less bits is also slower...
*/
#define FBITS 8
#define BITM 0xff

#define BITN 1<<FBITS

#define MAX_ROWS 4300
#define MAX_COLS 1728		// !! FIXME - command line parameter 


struct g3_tree { 
	int nr_bits;
	struct g3_tree *nextb[ BITN ];
};

static int byte_tab[ 256 ];

g3code t_white[66] = {
{ 0,   0, 0x0ac,  8 },
{ 0,   1, 0x038,  6 },
{ 0,   2, 0x00e,  4 },
{ 0,   3, 0x001,  4 },
{ 0,   4, 0x00d,  4 },
{ 0,   5, 0x003,  4 },
{ 0,   6, 0x007,  4 },
{ 0,   7, 0x00f,  4 },
{ 0,   8, 0x019,  5 },
{ 0,   9, 0x005,  5 },
{ 0,  10, 0x01c,  5 },
{ 0,  11, 0x002,  5 },
{ 0,  12, 0x004,  6 },
{ 0,  13, 0x030,  6 },
{ 0,  14, 0x00b,  6 },
{ 0,  15, 0x02b,  6 },
{ 0,  16, 0x015,  6 },
{ 0,  17, 0x035,  6 },
{ 0,  18, 0x072,  7 },
{ 0,  19, 0x018,  7 },
{ 0,  20, 0x008,  7 },
{ 0,  21, 0x074,  7 },
{ 0,  22, 0x060,  7 },
{ 0,  23, 0x010,  7 },
{ 0,  24, 0x00a,  7 },
{ 0,  25, 0x06a,  7 },
{ 0,  26, 0x064,  7 },
{ 0,  27, 0x012,  7 },
{ 0,  28, 0x00c,  7 },
{ 0,  29, 0x040,  8 },
{ 0,  30, 0x0c0,  8 },
{ 0,  31, 0x058,  8 },
{ 0,  32, 0x0d8,  8 },
{ 0,  33, 0x048,  8 },
{ 0,  34, 0x0c8,  8 },
{ 0,  35, 0x028,  8 },
{ 0,  36, 0x0a8,  8 },
{ 0,  37, 0x068,  8 },
{ 0,  38, 0x0e8,  8 },
{ 0,  39, 0x014,  8 },
{ 0,  40, 0x094,  8 },
{ 0,  41, 0x054,  8 },
{ 0,  42, 0x0d4,  8 },
{ 0,  43, 0x034,  8 },
{ 0,  44, 0x0b4,  8 },
{ 0,  45, 0x020,  8 },
{ 0,  46, 0x0a0,  8 },
{ 0,  47, 0x050,  8 },
{ 0,  48, 0x0d0,  8 },
{ 0,  49, 0x04a,  8 },
{ 0,  50, 0x0ca,  8 },
{ 0,  51, 0x02a,  8 },
{ 0,  52, 0x0aa,  8 },
{ 0,  53, 0x024,  8 },
{ 0,  54, 0x0a4,  8 },
{ 0,  55, 0x01a,  8 },
{ 0,  56, 0x09a,  8 },
{ 0,  57, 0x05a,  8 },
{ 0,  58, 0x0da,  8 },
{ 0,  59, 0x052,  8 },
{ 0,  60, 0x0d2,  8 },
{ 0,  61, 0x04c,  8 },
{ 0,  62, 0x0cc,  8 },
{ 0,  63, 0x02c,  8 },
{ 0, -1, 0, 11 },		// 11 0-bits == EOL, special handling 
{ 0, -1, 0, 0 }};		// end of table 

// make-up codes white 
g3code m_white[28] = {
{ 0,  64, 0x01b,  5 },
{ 0, 128, 0x009,  5 },
{ 0, 192, 0x03a,  6 },
{ 0, 256, 0x076,  7 },
{ 0, 320, 0x06c,  8 },
{ 0, 384, 0x0ec,  8 },
{ 0, 448, 0x026,  8 },
{ 0, 512, 0x0a6,  8 },
{ 0, 576, 0x016,  8 },
{ 0, 640, 0x0e6,  8 },
{ 0, 704, 0x066,  9 },
{ 0, 768, 0x166,  9 },
{ 0, 832, 0x096,  9 },
{ 0, 896, 0x196,  9 },
{ 0, 960, 0x056,  9 },
{ 0,1024, 0x156,  9 },
{ 0,1088, 0x0d6,  9 },
{ 0,1152, 0x1d6,  9 },
{ 0,1216, 0x036,  9 },
{ 0,1280, 0x136,  9 },
{ 0,1344, 0x0b6,  9 },
{ 0,1408, 0x1b6,  9 },
{ 0,1472, 0x032,  9 },
{ 0,1536, 0x132,  9 },
{ 0,1600, 0x0b2,  9 },
{ 0,1664, 0x006,  6 },
{ 0,1728, 0x1b2,  9 },
{ 0,  -1, 0, 0} };

g3code t_black[66] = {
{ 0,   0, 0x3b0, 10 },
{ 0,   1, 0x002,  3 },
{ 0,   2, 0x003,  2 },
{ 0,   3, 0x001,  2 },
{ 0,   4, 0x006,  3 },
{ 0,   5, 0x00c,  4 },
{ 0,   6, 0x004,  4 },
{ 0,   7, 0x018,  5 },
{ 0,   8, 0x028,  6 },
{ 0,   9, 0x008,  6 },
{ 0,  10, 0x010,  7 },
{ 0,  11, 0x050,  7 },
{ 0,  12, 0x070,  7 },
{ 0,  13, 0x020,  8 },
{ 0,  14, 0x0e0,  8 },
{ 0,  15, 0x030,  9 },
{ 0,  16, 0x3a0, 10 },
{ 0,  17, 0x060, 10 },
{ 0,  18, 0x040, 10 },
{ 0,  19, 0x730, 11 },
{ 0,  20, 0x0b0, 11 },
{ 0,  21, 0x1b0, 11 },
{ 0,  22, 0x760, 11 },
{ 0,  23, 0x0a0, 11 },
{ 0,  24, 0x740, 11 },
{ 0,  25, 0x0c0, 11 },
{ 0,  26, 0x530, 12 },
{ 0,  27, 0xd30, 12 },
{ 0,  28, 0x330, 12 },
{ 0,  29, 0xb30, 12 },
{ 0,  30, 0x160, 12 },
{ 0,  31, 0x960, 12 },
{ 0,  32, 0x560, 12 },
{ 0,  33, 0xd60, 12 },
{ 0,  34, 0x4b0, 12 },
{ 0,  35, 0xcb0, 12 },
{ 0,  36, 0x2b0, 12 },
{ 0,  37, 0xab0, 12 },
{ 0,  38, 0x6b0, 12 },
{ 0,  39, 0xeb0, 12 },
{ 0,  40, 0x360, 12 },
{ 0,  41, 0xb60, 12 },
{ 0,  42, 0x5b0, 12 },
{ 0,  43, 0xdb0, 12 },
{ 0,  44, 0x2a0, 12 },
{ 0,  45, 0xaa0, 12 },
{ 0,  46, 0x6a0, 12 },
{ 0,  47, 0xea0, 12 },
{ 0,  48, 0x260, 12 },
{ 0,  49, 0xa60, 12 },
{ 0,  50, 0x4a0, 12 },
{ 0,  51, 0xca0, 12 },
{ 0,  52, 0x240, 12 },
{ 0,  53, 0xec0, 12 },
{ 0,  54, 0x1c0, 12 },
{ 0,  55, 0xe40, 12 },
{ 0,  56, 0x140, 12 },
{ 0,  57, 0x1a0, 12 },
{ 0,  58, 0x9a0, 12 },
{ 0,  59, 0xd40, 12 },
{ 0,  60, 0x340, 12 },
{ 0,  61, 0x5a0, 12 },
{ 0,  62, 0x660, 12 },
{ 0,  63, 0xe60, 12 },
{ 0,  -1, 0x000, 11 },
{ 0,  -1, 0, 0 } };

g3code m_black[28] = {
{ 0,  64, 0x3c0, 10 },
{ 0, 128, 0x130, 12 },
{ 0, 192, 0x930, 12 },
{ 0, 256, 0xda0, 12 },
{ 0, 320, 0xcc0, 12 },
{ 0, 384, 0x2c0, 12 },
{ 0, 448, 0xac0, 12 },
{ 0, 512, 0x6c0, 13 },
{ 0, 576,0x16c0, 13 },
{ 0, 640, 0xa40, 13 },
{ 0, 704,0x1a40, 13 },
{ 0, 768, 0x640, 13 },
{ 0, 832,0x1640, 13 },
{ 0, 896, 0x9c0, 13 },
{ 0, 960,0x19c0, 13 },
{ 0,1024, 0x5c0, 13 },
{ 0,1088,0x15c0, 13 },
{ 0,1152, 0xdc0, 13 },
{ 0,1216,0x1dc0, 13 },
{ 0,1280, 0x940, 13 },
{ 0,1344,0x1940, 13 },
{ 0,1408, 0x540, 13 },
{ 0,1472,0x1540, 13 },
{ 0,1536, 0xb40, 13 },
{ 0,1600,0x1b40, 13 },
{ 0,1664, 0x4c0, 13 },
{ 0,1728,0x14c0, 13 },
{ 0,  -1, 0, 0 } };


static bool 
tree_add_node(g3_tree *p, g3code *g3c, int bit_code, int bit_length) {
	int i;
    
	if ( bit_length <= FBITS )	{		// leaf (multiple bits) 
		g3c->nr_bits = bit_length;		// leaf tag 
		if ( bit_length == FBITS )	{	// full width 
			p->nextb[ bit_code ] = (g3_tree *) g3c;
		}
		else {				// fill bits 
			for ( i=0; i< ( 1 << (FBITS-bit_length)); i++ )	{
				p->nextb[ bit_code + ( i << bit_length ) ] = (g3_tree *) g3c;
			}
		}
	}
	else {				// node 
		g3_tree *p2;

		p2 = p->nextb[ bit_code & BITM ];
		if ( p2 == 0 ) {		// no sub-node exists 
			p2 = p->nextb[ bit_code & BITM ] = (g3_tree * ) calloc( 1, sizeof(g3_tree));
			if ( p2 == NULL ) { 
				FreeImage_OutputMessageProc(s_format_id, "Allocation failed" ); 
				return false;
			}
			p2->nr_bits = 0;		// node tag             
		}
		if ( p2->nr_bits != 0 ) {
			FreeImage_OutputMessageProc(s_format_id, "Internal table setup error");
			return false;
		}
		if(tree_add_node( p2, g3c, bit_code >> FBITS, bit_length - FBITS ) == false)
			return false;
	}
	return true;
}

static bool 
build_tree(g3_tree ** p, g3code *c) {
    if ( *p == NULL ) {
		(*p) = (g3_tree *) calloc( 1, sizeof(g3_tree) );
		if ( *p == NULL ) { 
			FreeImage_OutputMessageProc(s_format_id, "Allocation failed" ); 
			return false;
		}		
		(*p)->nr_bits=0;
    }

    while ( c->bit_length != 0 ) {
		if(tree_add_node( *p, c, c->bit_code, c->bit_length ) == false) {
			free(*p);
			*p=NULL;
			return false;
		}
		c++;
    }
	return true;
}

static void 
free_tree(g3_tree * p) {
	free((void *)p);
}

static void 
init_byte_tab (int reverse, int byte_tab[] ) {
	int i;
    if ( reverse ) {
		for ( i=0; i<256; i++ ) byte_tab[i] = i;
	}
	else {
		for ( i=0; i<256; i++ ) {
			byte_tab[i] = ( ((i & 0x01) << 7) | ((i & 0x02) << 5) |
			     ((i & 0x04) << 3) | ((i & 0x08) << 1) |
			     ((i & 0x10) >> 1) | ((i & 0x20) >> 3) |
			     ((i & 0x40) >> 5) | ((i & 0x80) >> 7) );
		}
	}
}

// ----------------------------------------------------------

static g3_tree * black, * white;

// ----------------------------------------------------------

static FIBITMAP* 
emitbitmap (int hcol, int row, char *bitmap, int bperrow) {
	FIBITMAP *ret = FreeImage_Allocate(hcol, row, 1);
	if(ret == NULL) { return NULL; }
	int i = 0;
	// get target buffer
	BYTE *pTarget = FreeImage_GetBits(ret);
	int iByteWidth = FreeImage_GetPitch(ret);
	pTarget += iByteWidth * (row-1);
	// copy rows
	for(i = 0; i < row; i++) {
		// copy data
		memcpy(pTarget, bitmap, iByteWidth);
		// move to next line
		pTarget -= iByteWidth;
		bitmap += bperrow;
	}
	// set palette
	RGBQUAD *pal = FreeImage_GetPalette(ret);
	for (i = 0; i < 2; i++) {
		pal[i].rgbRed = (1-i)*255;
		pal[i].rgbGreen = (1-i)*255;
		pal[i].rgbBlue = (1-i)*255;
	}

	return ret;
}

// ==========================================================
// Plugin Implementation
// ==========================================================

static const char * DLL_CALLCONV
Format() {
	return "G3";
}

static const char * DLL_CALLCONV 
Description() {
	return "Raw fax format CCITT G.3";
}

static const char * DLL_CALLCONV 
Extension() {
	return "g3";
}

static const char * DLL_CALLCONV 
RegExpr() {
	return NULL; // there is no reasonable regexp for raw G3
}

static const char * DLL_CALLCONV 
MimeType() {
	return "image/fax-g3";
}

static BOOL DLL_CALLCONV
Validate(FreeImageIO *io, fi_handle handle) {
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

static FIBITMAP * DLL_CALLCONV
Load(FreeImageIO *io, fi_handle handle, int page, int flags, void *_data) {
	if(handle==NULL) return NULL;

	int color = 0;			// start with white 
	char rbuf[2048] = {0};	// read buffer 
	int rp = 0;				// read pointer 
	int rs = 0;				// read buffer size 
	int nr_pels;

	rs = io->read_proc(rbuf, 1, sizeof(rbuf), handle);
	if(rs < 0) return NULL;
	// skip GhostScript header 
    rp = ( rs >= 64 && strcmp( rbuf+1, "PC Research, Inc" ) == 0 ) ? 64 : 0;

	// initialize bitmap 
	int row,col,hcol;
    row = col = hcol = 0;
	int max_rows = MAX_ROWS;
	int cons_eol=0; // consecutive EOLs 
	int hibit=0;
	int data=0;

	char *bp;			// bitmap pointer 
	char *bitmap = (char *)calloc(max_rows * MAX_COLS / 8, 1 );
	if(bitmap == NULL) return NULL;
	bp = &bitmap[ row * MAX_COLS/8 ];
	
	while ( rs > 0 && cons_eol < 4 ) {	// i.e., while (!EOF) 

		while ( hibit < 20 ) {
			data |= ( byte_tab[ (int) (unsigned char) rbuf[ rp++] ] << hibit );
			hibit += 8;

			if ( rp >= rs )	{
				rs = io->read_proc( rbuf, 1, sizeof( rbuf), handle);
				if ( rs < 0 ) { 
					FreeImage_OutputMessageProc(s_format_id, "Cannot read input file - 1.");
					break; 
				}
				rp = 0;
				if ( rs == 0 ) { goto do_write; }
			}
		}
		g3_tree * p;
		if ( color == 0 ) {		// white 
			p = white->nextb[ data & BITM ];
		} else {			// black 
			p = black->nextb[ data & BITM ];
		}
		while ( p != NULL && ! ( p->nr_bits ) )	{
			data >>= FBITS;
			hibit -= FBITS;
			p = p->nextb[ data & BITM ];
		}

		if ( p == NULL ) {	// invalid code 
			char msg[256]={0};
			sprintf (msg, "Invalid code, row=%d, col=%d, file offset=%lx, skip to eol\n",
				row, col, (unsigned long) io->seek_proc(handle, 0, 1 ) - rs + rp );
			FreeImage_OutputMessageProc(s_format_id, msg);
			while ( ( data & 0x03f ) != 0 )	{
				data >>= 1; hibit--;
				if ( hibit < 20 ) {
					data |= ( byte_tab[ (int) (unsigned char) rbuf[ rp++] ] << hibit );
					hibit += 8;

					if ( rp >= rs )	{ // buffer underrun 
						rs = io->read_proc( rbuf, 1, sizeof( rbuf), handle);
						if ( rs < 0 ) { 
							FreeImage_OutputMessageProc(s_format_id, "Cannot read input file - 2.");
							break; 
						}
						rp = 0;
						if ( rs == 0 ) goto do_write;
					}
				}
			}
			nr_pels = -1;		// handle as if eol 
		}
		else {				// p != NULL <-> valid code 
			data >>= p->nr_bits;
			hibit -= p->nr_bits;
			nr_pels = ( (struct g3_leaf *) p ) ->nr_pels;
		}
        
		// handle EOL (including fill bits) 
		if ( nr_pels == -1 ) {
			// skip filler 0bits -> seek for "1"-bit 
			while ( ( data & 0x01 ) != 1 ) {
				if ( ( data & 0xf ) == 0 )	{ // nibble optimization 
					hibit-= 4; data >>= 4;
				}
				else {
					hibit--; data >>= 1;
				}
				// fill higher bits 
				if ( hibit < 20 ) {
					data |= ( byte_tab[ (int) (unsigned char) rbuf[ rp++] ] << hibit );
					hibit += 8;
					
					if ( rp >= rs )	{ // buffer underrun 
						rs = io->read_proc( rbuf, 1, sizeof( rbuf), handle);
						if ( rs < 0 ) { 
							FreeImage_OutputMessageProc(s_format_id, "Cannot read input file - 3.");
							break; 
						}
						rp = 0;
						if ( rs == 0 ) goto do_write;
					}
				}
			}				// end skip 0bits 
			hibit--; data >>=1;

			color=0;
            
			if ( col == 0 ) {
				cons_eol++;		// consecutive EOLs 
			} else {
				if ( col > hcol && col <= MAX_COLS ) {
					hcol = col;
				}
				row++;
				
				// bitmap memory full? make it larger! 
				if ( row >= max_rows ) {
					char *p = (char *)realloc( bitmap, ( max_rows += 500 ) * MAX_COLS/8 );
					if ( p == NULL ) {
						FreeImage_OutputMessageProc(s_format_id, "realloc() failed, page truncated" );
						rs = 0;
					}
					else {
						bitmap = p;
						memset( &bitmap[ row * MAX_COLS/8 ], 0, ( max_rows - row ) * MAX_COLS/8 );
					}
				}
                col = 0; bp = &bitmap[ row * MAX_COLS/8 ];
				cons_eol = 0;
			}
		}
		else {		// not eol 
			if ( col+nr_pels > MAX_COLS ) nr_pels = MAX_COLS - col;
			if ( color == 0 )  {                // white 
				col += nr_pels;
			} else {                               // black 
				register int bit = ( 0x80 >> ( col & 07 ) );
				register char *w = & bp[ col>>3 ];
				for (int i=nr_pels; i > 0; i-- ) {
					*w |= bit;
					bit >>=1; if ( bit == 0 ) { bit = 0x80; w++; }
					col++;
				}
			}
			if ( nr_pels < 64 ) color = !color;		// terminating code 
		}
	}		// end main loop 

do_write:      	// write pbm (or whatever) file 
    
    FIBITMAP *image_id = emitbitmap (hcol, row, bitmap, MAX_COLS/8);

    free ((void*)bitmap);

    return image_id;

}

// ==========================================================
//   Init
// ==========================================================

void DLL_CALLCONV
InitG3(Plugin *plugin, int format_id) {

	// build tree 
	build_tree( &white, t_white );
	build_tree( &white, m_white );
	build_tree( &black, t_black );
	build_tree( &black, m_black );
	init_byte_tab( 0, byte_tab );

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
