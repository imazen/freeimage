#!/bin/sh

DIRLIST=". Source Source/Metadata Source/FreeImageToolkit Source/LibJPEG Source/LibMNG Source/LibPNG Source/LibTIFF Source/ZLib"

echo "VER_MAJOR = 3" > Makefile.srcs
echo "VER_MINOR = 7.0" >> Makefile.srcs

echo -n "SRCS = " >> Makefile.srcs
for DIR in $DIRLIST; do
	DSPS=`echo $DIR/*.dsp`
	if [ "$DSPS" != "$DIR/*.dsp" ]; then
		egrep '^SOURCE=.*\.(c|cpp)' $DIR/*.dsp | cut -d= -f2 | tr '\\' '/' | awk '{print "'$DIR'/"$0}' | tr '\r\n' '  ' | tr -s ' ' >> Makefile.srcs
	fi
done
echo >> Makefile.srcs

echo -n "INCLS = " >> Makefile.srcs
find . -name "*.h" -print | xargs echo >> Makefile.srcs
echo >> Makefile.srcs

echo -n "INCLUDE =" >> Makefile.srcs
for DIR in $DIRLIST; do
	echo -n " -I$DIR" >> Makefile.srcs
done
echo >> Makefile.srcs

