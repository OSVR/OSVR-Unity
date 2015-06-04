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
third-party\discount-2.1.6-win32\markdown.exe -F 0x4 -o CONTRIBUTING.html CONTRIBUTING.md
move CONTRIBUTING.html OSVR-Unity-Dist\


rem Copy the license and the notice
xcopy LICENSE OSVR-Unity-Dist /Y
xcopy NOTICE OSVR-Unity-Dist /Y

rem Copy the readme and changes to text files inside the Unity editor
copy OSVR-Unity\README.md OSVR-Unity-Dist\src\OSVR-Unity\Assets\OSVRUnity\README.txt /Y
copy OSVR-Unity\CHANGES.md OSVR-Unity-Dist\src\OSVR-Unity\Assets\OSVRUnity\CHANGES.txt /Y