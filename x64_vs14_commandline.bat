SET vcvar_arg=x86_amd64
set bin_subdir=x64
SET PATH=%PATH%;C:\Program Files (x86)\cmake\bin
SET PATH=%PATH%;%LOCALAPPDATA%\nasm
SET vc_setup=""C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall" x86_amd64"
SET tbs_tools=msvc14
SET tbs_arch=x64

rem find bash from git
rem assumes git is in [gitdir]\cmd
rem and msys in [gitdir]\bin

for %%i in (git.exe) do set gitexe=%%~$PATH:i
pushd "%gitexe%\..\..\bin"
set bashdir=%cd%
popd
set path=%bashdir%;%path%

cmd /k %VC_SETUP%