pushd Wrapper\FreeImage.NET\cs\

bash -c "curl -L -o utd.zip https://s3.amazonaws.com/public-unit-test-resources/FreeImage.NET/UnitTestData.zip"
bash -c "unzip utd.zip"
popd

pushd Wrapper\FreeImage.NET\cs\Library
msbuild Library.csproj /p:Configuration=Release /v:m
popd


echo "Now attempting to build FreeImageIO, a nonessential C++ managed library that is out of date.
copy Dist\FreeImage.lib Wrapper\FreeImage.NET\cpp\FreeImageIO 
copy Dist\FreeImage.h Wrapper\FreeImage.NET\cpp\FreeImageIO 
pushd Wrapper\FreeImage.NET\cpp\FreeImageIO 
msbuild FreeImageIO.sln /p:Configuration=Release /v:m
popd
      
      

pushd Wrapper\FreeImage.NET\cs\UnitTest
..\nuget restore -PackagesDirectory ..\packages
msbuild UnitTest.csproj /p:Configuration=Release /v:m
popd


set nunit_console=Wrapper\FreeImage.NET\cs\packages\NUnit.Runners.2.6.4\tools\nunit-console
powershell copy $(.\thumbs list_bin) "Wrapper\FreeImage.NET\cs\UnitTest\bin\Release\FreeImage.dll"
if [%tbs_arch%]==[x86] %nunit_console%-x86  /framework:net-4.5 Wrapper\FreeImage.NET\cs\UnitTest\bin\Release\UnitTest.exe
if [%tbs_arch%]==[x64] %nunit_console%  /framework:net-4.5 Wrapper\FreeImage.NET\cs\UnitTest\bin\Release\UnitTest.exe