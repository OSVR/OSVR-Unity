cd /d %~dp0

mkdir OSVR-Unity-Dist
mkdir OSVR-Unity-Dist\OSVR-Unity
xcopy Managed-OSVR OSVR-Unity-Dist\ /Y /S
