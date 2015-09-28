#!/bin/bash

# THe Ultimate Make Bash Script
# Used to wrap build scripts for easy dep
# handling and multiplatform support


# Basic usage on *nix:
# export tbs_arch=x86
# ./thumbs.sh make


# On Win (msvc 2013):
# C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall x86_amd64
# SET tbs_tools=msvc12
# thumbs make

# On Win (mingw32):
# SET path=C:\mingw32\bin;%path%
# SET tbs_tools=mingw
# SET tbs_arch=x86
# thumbs make


# Global settings are stored in env vars
# Should be inherited

[ $tbs_conf ]           || export tbs_conf=Release
[ $tbs_arch ]           || export tbs_arch=x64
[ $tbs_tools ]          || export tbs_tools=gnu
[ $tbs_static_runtime ] || export tbs_static_runtime=0

[ $tbs_fi_static ]      || export tbs_fi_static=0
[ $tbs_fi_tests ]       || export tbs_fi_tests=0
[ $tbs_fi_png ]         || export tbs_fi_png=1
[ $tbs_fi_jpeg ]        || export tbs_fi_jpeg=1
[ $tbs_fi_tiff ]        || export tbs_fi_tiff=1
[ $tbs_fi_webp ]        || export tbs_fi_webp=0
[ $tbs_fi_raw ]         || export tbs_fi_raw=0
[ $tbs_fi_openjp ]      || export tbs_fi_openjp=0


# tbsd_* contains dep related settings
# tbsd_[name]_* contains settings specific to the dep
# name should match the repo name

# deps contains a map of what should be built/used
# keep the keys in sync ... no assoc arrays on msys :/
# targ contains a target for each dep (default=empty str)
# post is executed after each thumbs dep build
# ^ used for copying/renaming any libs you need - uses eval

zname=zlib.lib
jname=jpeg.lib
pname=png.lib
tname=tiff.lib

if [ $tbs_tools = gnu -o $tbs_tools = mingw ]
then
  zname=libz.a
  jname=libjpeg.a
  pname=libpng.a
  tname=libtiff.a
fi

deps=()
targ=()
post=()

[[ $tbsd_zlib_repo ]]          || export tbsd_zlib_repo="git clone https://github.com/imazen/zlib_shallow ; cd zlib_shallow && git reset --hard 638262adec2b52e907e5703e4322a7f866896a13"
[[ $tbsd_libpng_repo ]]        || export tbsd_libpng_repo="git clone https://github.com/imazen/libpng ; cd libpng && git reset --hard 8a86aae5aecc7d128011d25f6ace5b10334c724e"
[[ $tbsd_libjpeg_turbo_repo ]] || export tbsd_libjpeg_turbo_repo="git clone https://github.com/imazen/libjpegturbo libjpeg_turbo ; cd libjpeg_turbo && git reset --hard 7f7ac7c2562afbecfd080448137543da9fc657f1"
[[ $tbsd_libtiff_repo ]]       || export tbsd_libtiff_repo="git clone https://github.com/imazen/libtiff ; cd libtiff && git reset --hard f87b43002d3e6fd7e68365ba947451ffff041ab2"
[[ $tbsd_libwebp_repo ]]       || export tbsd_libwebp_repo="git clone https://github.com/imazen/libwebp ; cd libwebp && git reset --hard 5dc1e4b2d07f21f5248d382369fc0d2e2a06860e"
[[ $tbsd_libraw_repo ]]        || export tbsd_libraw_repo="git clone https://github.com/imazen/libraw ; cd libraw && git reset --hard fb7444c57f8e01d6ed15f47dde3b07e264b7e83d"
[[ $tbsd_openjpeg_repo ]]      || export tbsd_openjpeg_repo="git clone https://github.com/imazen/openjpeg-offical ; cd openjpeg-offical && git reset --hard 5bff410c011548a168bba78ad0a5717e80b0ba51"

if [[ "$OSTYPE" == "darwin"* ]]; then cp="rsync"
else cp="cp"
fi

deps+=(zlib); targ+=(zlibstatic)
post+=("$cp -u \$(./thumbs.sh list_slib) ../../deps/$zname")

if [ $tbs_fi_png -gt 0 ]; then
  deps+=(libpng); targ+=(png16_static)
  post+=("$cp -u \$(./scripts/thumbs.sh list_slib) ../../deps/$pname")
fi

if [ $tbs_fi_jpeg -gt 0 ]; then
  deps+=(libjpeg_turbo); targ+=(jpeg-static)
  post+=("for lib in \$(./thumbs.sh list_slib); do [ -f \$lib ] && $cp -u \$lib ../../deps/$jname; done")
fi

if [ $tbs_fi_tiff -gt 0 ]; then
  ttarg="libtiff/tiff_static"
  [ $tbs_tools = gnu -o $tbs_tools = mingw ] && ttarg=tiff_static
  deps+=(libtiff); targ+=($ttarg)
  post+=("$cp -u \$(./thumbs.sh list_slib) ../../deps/$tname")
fi

if [ $tbs_fi_webp -gt 0 ]; then
  deps+=(libwebp); targ+=("")
  post+=("for lib in \$(./thumbs.sh list_slib); do [ -f \$lib ] && $cp -u \$lib ../../deps/; done")
fi

if [ $tbs_fi_raw -gt 0 ]; then
  deps+=(libraw); targ+=("")
  post+=("for lib in \$(./thumbs.sh list_slib); do [ -f \$lib ] && $cp -u \$lib ../../deps/; done")
fi

if [ $tbs_fi_openjp -gt 0 ]; then
  export tbsd_openjpeg_static=1
  deps+=(openjpeg); targ+=("")
  post+=("for lib in \$(./thumbs.sh list_slib); do [ -f \$lib ] && $cp -u \$lib ../../deps/; done")
fi



# -----------
# dep processor

process_deps()
{
  mkdir build_deps
  mkdir deps
  cd build_deps

  for key in "${!deps[@]}"
  do
    dep=${deps[$key]}
    i_dep_repo="tbsd_${dep}_repo"
    i_dep_incdir="tbsd_${dep}_incdir"
    i_dep_libdir="tbsd_${dep}_libdir"
    i_dep_built="tbsd_${dep}_built"
    
    [ ${!i_dep_built} ] || export "${i_dep_built}=0"
    
    if [ ${!i_dep_built} -eq 0 ]
    then
      eval ${!i_dep_repo} || exit 1
      
      thumbs="./thumbs.sh"
      [ ! -f $thumbs ] && thumbs=$(find . -name thumbs.sh -maxdepth 2)
      
      $thumbs make ${targ[$key]} || exit 1
      
      # copy any includes and do poststep
      $cp -u -r $($thumbs list_inc) ../../deps
      eval ${post[$key]}
      
      # look in both local and parent dep dirs
      export "${i_dep_incdir}=../../deps;deps"
      export "${i_dep_libdir}=../../deps;deps"
      export "${i_dep_built}=1"
      
      cd ..
    fi
  done
  
  cd ..
}

# -----------
# constructs dep dirs for cmake

postproc_deps()
{
  cm_inc=
  cm_lib=
  
  for dep in "${deps[@]}"
  do
    i_dep_incdir="tbsd_${dep}_incdir"
    i_dep_libdir="tbsd_${dep}_libdir"
    
    cm_inc="${!i_dep_incdir};$cm_inc"
    cm_lib="${!i_dep_libdir};$cm_lib"
  done
  
  cm_args+=(-DCMAKE_LIBRARY_PATH=$cm_lib)
  cm_args+=(-DCMAKE_INCLUDE_PATH=$cm_inc)
}

# -----------

if [ $# -lt 1 ]
then
  echo ""
  echo " Usage : ./thumbs [command]"
  echo ""
  echo " Commands:"
  echo "   make      - builds everything"
  echo "   check     - runs tests"
  echo "   clean     - removes build files"
  echo "   list      - echo paths to any interesting files"
  echo "               space separated; relative"
  echo "   list_bin  - echo binary paths"
  echo "   list_inc  - echo lib include files"
  echo "   list_slib - echo static lib path"
  echo "   list_dlib - echo dynamic lib path"
  echo ""
  exit
fi

# -----------

upper()
{
  echo $1 | tr [:lower:] [:upper:]
}

# Local settings

l_inc="./Source/FreeImage.h"
l_slib=
l_dlib=
l_bin=
list=

make=
c_flags=
ld_flags=
cm_tools=
cm_args=(-DCMAKE_BUILD_TYPE=$tbs_conf)

cm_args+=(-DENABLE_TESTS=$tbs_fi_tests)
cm_args+=(-DENABLE_PNG=$tbs_fi_png)
cm_args+=(-DENABLE_JPEG=$tbs_fi_jpeg)
cm_args+=(-DENABLE_TIFF=$tbs_fi_tiff)
cm_args+=(-DENABLE_WEBP=$tbs_fi_webp)
cm_args+=(-DENABLE_RAW=$tbs_fi_raw)
cm_args+=(-DENABLE_OPENJP=$tbs_fi_openjp)

[ $tbs_fi_static -gt 0 ] && cm_args+=(-DFREEIMAGE_BUILD_STATIC=1)

target=
[ $2 ] && target=$2

# -----------

case "$tbs_tools" in
msvc12)
  cm_tools="Visual Studio 12"
  [ "$target" = "" ] && mstrg="FreeImage.sln" || mstrg="$target.vcxproj"
  make="msbuild.exe $mstrg //p:Configuration=$tbs_conf //v:m"
  
  l_slib=""
  l_dlib="./build/lib/$tbs_conf/FreeImage.lib"
  l_bin="./build/Bin/$tbs_conf/FreeImage.dll"
  list="$l_bin $l_slib $l_dlib $l_inc" ;;
gnu)
  cm_tools="Unix Makefiles"
  c_flags+=" -fPIC"
  make="make $target"
  l_slib=""
  l_dlib="./build/bin/libFreeImage.so"
  l_bin="$l_dlib"
  list="$l_slib $l_dlib $l_inc" ;;
mingw)
  cm_tools="MinGW Makefiles"
  make="mingw32-make $target"
  ld_flags+=" -Wl,--add-stdcall-alias"
  
  # allow sh in path; some old cmake/mingw bug?
  cm_args+=(-DCMAKE_SH=)
  
  l_slib=""
  l_dlib="./build/lib/libFreeImage.dll.a"
  l_bin="./build/bin/libFreeImage.dll"
  list="$l_bin $l_slib $l_dlib $l_inc" ;;

*) echo "Tool config not found for $tbs_tools"
   exit 1 ;;
esac

# -----------

case "$tbs_arch" in
x64)
  [ $tbs_tools = msvc12 ] && cm_tools="$cm_tools Win64" && c_flags+=" -D_CRT_SECURE_NO_WARNINGS"
  [ $tbs_tools = gnu -o $tbs_tools = mingw ] && c_flags+=" -m64" ;;
x86)
  [ $tbs_tools = gnu -o $tbs_tools = mingw ] && c_flags+=" -m32" ;;

*) echo "Arch config not found for $tbs_arch"
   exit 1 ;;
esac

# -----------

if [ $tbs_static_runtime -gt 0 ]
then
  [ $tbs_tools = msvc12 ] && cm_args+=(-DFREEIMAGE_DYNAMIC_C_RUNTIME:BOOL=OFF)
  [ $tbs_tools = gnu ] && ld_flags+=" -static-libgcc -static-libstdc++"
  [ $tbs_tools = mingw ] && ld_flags+=" -static"
else
  [ $tbs_tools = msvc12 ] && cm_args+=(-DFREEIMAGE_DYNAMIC_C_RUNTIME:BOOL=ON)
fi

# -----------

case "$1" in
make)
  process_deps
  postproc_deps
  
  mkdir build
  cd build
  
  fsx=
  [ $tbs_tools = gnu ] || fsx=_$(upper $tbs_conf)
  
  cm_args+=(-DCMAKE_C_FLAGS$fsx="$c_flags")
  cm_args+=(-DCMAKE_CXX_FLAGS$fsx="$c_flags")
  cm_args+=(-DCMAKE_SHARED_LINKER_FLAGS$fsx="$ld_flags")
  cm_args+=(-DCMAKE_EXE_LINKER_FLAGS$fsx="$ld_flags")
  
  cmake -G "$cm_tools" "${cm_args[@]}" .. || exit 1
  $make || exit 1
  
  cd ..
  ;;
  
check)
  cd build
  ctest -C $tbs_conf . || exit 1
  cd ..
  ;;
  
clean)
  rm -rf build_deps
  rm -rf deps
  rm -rf build
  
  rm -f Dist/*.lib
  rm -f Dist/*.a
  rm -f Dist/*.dll
  rm -f Dist/*.so
  rm -f Dist/*.h
  ;;

list) echo $list;;
list_bin) echo $l_bin;;
list_inc) echo $l_inc;;
list_slib) echo $l_slib;;
list_dlib) echo $l_dlib;;

*) echo "Unknown command $1"
   exit 1
   ;;
esac
