echo on
cd /d "%~dp0"

xcopy "%1" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 
xcopy "%2osvr*.dll" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 
xcopy "%2msvc*.dll" ..\..\OSVR-Unity\Assets\Plugins\%3\ /Y 