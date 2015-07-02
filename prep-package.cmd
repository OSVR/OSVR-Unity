cd /d %~dp0

mkdir OSVR-Unity-Dist

rem the -F 0x4 is to turn off smartypants.
third-party\discount-2.1.6-win32\markdown.exe -F 0x4 -o README.html OSVR-Unity/README.md
move README.html OSVR-Unity-Dist\
third-party\discount-2.1.6-win32\markdown.exe -F 0x4 -o CONTRIBUTING.html CONTRIBUTING.md
move CONTRIBUTING.html OSVR-Unity-Dist\


rem Copy the license and the notice
xcopy LICENSE OSVR-Unity-Dist /Y
xcopy NOTICE OSVR-Unity-Dist /Y