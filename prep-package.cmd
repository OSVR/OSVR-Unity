cd /d %~dp0

mkdir OSVR-Unity-Dist
mkdir OSVR-Unity-Dist\src
mkdir OSVR-Unity-Dist\src\OSVR-Unity
mkdir OSVR-Unity-Dist\src\Managed-OSVR
xcopy Managed-OSVR OSVR-Unity-Dist\src\Managed-OSVR /Y /S
xcopy OSVR-Unity OSVR-Unity-Dist\src\OSVR-Unity /Y /S

rem the -F 0x4 is to turn off smartypants.
third-party\discount-2.1.6-win32\markdown.exe -F 0x4 -o README.html OSVR-Unity/README.md
move README.html OSVR-Unity-Dist\