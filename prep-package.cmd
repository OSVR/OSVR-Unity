cd /d %~dp0

mkdir OSVR-Unity-Dist
mkdir OSVR-Unity-Dist\src
mkdir OSVR-Unity-Dist\src\OSVR-Unity
mkdir OSVR-Unity-Dist\src\Managed-OSVR
xcopy Managed-OSVR OSVR-Unity-Dist\src\Managed-OSVR /Y /S
xcopy OSVR-Unity OSVR-Unity-Dist\src\OSVR-Unity /Y /S
