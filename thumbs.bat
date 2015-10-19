@echo off
rem thumbs wrapper for windows; see main file for details


rem find bash from git
rem assumes git is in [gitdir]\cmd
rem and msys in [gitdir]\bin

for %%i in (git.exe) do set gitexe=%%~$PATH:i
pushd "%gitexe%\..\..\bin"
set bashdir=%cd%
popd
pushd "%gitexe%\..\..\usr\bin"
set git_usrbin=%cd%
popd
set path=%bashdir%;%path%


if exist "%git_usrbin%\link.exe" echo Rename "%git_usrbin%\link.exe" before continuing or build will fail. && exit /B

rem copy all known env vars to bash

setlocal enableDelayedExpansion
set exports=

for %%i in (tbs_conf tbs_arch tbs_tools tbs_static_runtime tbs_fi_tests tbs_fi_webp tbs_fi_jpeg tbs_fi_tiff tbs_fi_png tbs_fi_raw tbs_fi_openjp tbs_fi_static) do (
  if not [!%%i!]==[] (
    set exports=!exports!export %%i=!%%i!;
  )
)

rem copy dep settings

for %%i in (zlib libjpeg_turbo libpng libtiff libwebp openjpeg libraw) do (
  for %%j in (repo incdir libdir built) do (
    if not [!tbsd_%%i_%%j!]==[] (
      set exports=!exports!export tbsd_%%i_%%j=!tbsd_%%i_%%j!;
    )
  )
)

bash -c "%exports%./thumbs.sh %*"
