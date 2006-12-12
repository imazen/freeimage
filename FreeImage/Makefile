# Entry point for FreeImage makefiles
# Default to 'make -f Makefile.gnu' for Linux and for unknown OS. 
#
OS = $(shell uname)
MAKEFILE = gnu

ifeq ($(OS), Darwin)
    MAKEFILE = osx
endif
ifeq ($(OS), Cygwin)
    MAKEFILE = cygwin
endif
ifeq ($(OS), Solaris)
    MAKEFILE = solaris
endif

default:
	make -f Makefile.$(MAKEFILE) 

all:
	make -f Makefile.$(MAKEFILE) all 

dist:
	make -f Makefile.$(MAKEFILE) dist 

install:
	make -f Makefile.$(MAKEFILE) install 

clean:
	make -f Makefile.$(MAKEFILE) clean 

