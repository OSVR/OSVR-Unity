cd /d %~dp0

rem Check to make sure we've actually copied over at least something that could be the OSVR and Managed-OSVR binaries.
if not exist OSVR-Unity\Assets\OSVRUnity\Plugins\x86\*.dll exit /B 1

"C:\Program Files\Unity\Hub\Editor\2019.1.0f2\Editor\Unity.exe" -quit -batchmode -projectPath "%~dp0OSVR-Unity" -logFile "%~dp0unity.log" -executeMethod OSVRUnityBuild.build
type "%~dp0unity.log"

rem Fail the build if we didn't get a unitypackage out.
if not exist OSVR-Unity\*.unitypackage exit /B 1

rem Copy the unitypackage to the dist directory if it exists.
if exist OSVR-Unity-Dist xcopy OSVR-Unity\*.unitypackage OSVR-Unity-Dist /Y
