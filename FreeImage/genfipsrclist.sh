#!/bin/sh

DIRLIST=". Source Source/Metadata Source/FreeImageToolkit Source/LibJPEG Source/LibMNG Source/LibPNG Source/LibTIFF Source/ZLib Wrapper/FreeImagePlus"

echo "VER_MAJOR = 3" > fipMakefile.srcs
echo "VER_MINOR = 9.2" >> fipMakefile.srcs

echo -n "SRCS = " >> fipMakefile.srcs
for DIR in $DIRLIST; do
	VCPRJS=`echo $DIR/*.vcproj`
	if [ "$VCPRJS" != "$DIR/*.vcproj" ]; then
		egrep 'RelativePath=.*\.(c|cpp)' $DIR/*.vcproj | cut -d'"' -f2 | tr '\\' '/' | awk '{print "'$DIR'/"$0}' | tr '\r\n' '  ' | tr -s ' ' >> fipMakefile.srcs
	fi
done
echo >> fipMakefile.srcs

echo -n "INCLUDE =" >> fipMakefile.srcs
for DIR in $DIRLIST; do
	echo -n " -I$DIR" >> fipMakefile.srcs
done
echo >> fipMakefile.srcs

