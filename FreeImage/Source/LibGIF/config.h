#if defined(WIN32) || defined(__WIN32__)
#if defined(DrawText)   /* avoid collision with windows DrawText defs */
#undef DrawText
#endif
#ifndef CDECL
#define CDECL       __cdecl
#endif
#define open        _open
#define close       _close
#define HAVE_IO_H
#define HAVE_FCNTL_H
#define HAVE_STDARG_H
#define _OPEN_BINARY
#endif