#ifndef GIF_CONFIG_H
#define GIF_CONFIG_H

#if defined(WIN32) || defined(__WIN32__)
#define open        _open
#define close       _close
#include <io.h>
#endif

#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "gif_lib.h"
#include "gif_lib_private.h"

#endif // GIF_CONFIG_H
