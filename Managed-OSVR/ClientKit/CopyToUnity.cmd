rem For use as the following post-build command with command line arguments:
rem $(ProjectDir)CopyToUnity.cmd "$(TargetPath)" "$(TargetDir)" $(PlatformName)

echo on
cd /d "%~dp0"

rem Copy the managed wrapper DLL
xcopy "%1" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 

rem Copy the native DLLs that the build process puts alongside the build product
xcopy "%2osvr*.dll" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 

rem Copy the MSVC runtime redist libraries from alongside this script
xcopy "msvc*.dll" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 