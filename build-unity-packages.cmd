cd /d %~dp0
"C:\Program Files (x86)\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "%~dp0OSVR-Unity" -executeMethod OSVRUnityBuild.build

xcopy OSVR-Unity\*.unitypackage OSVR-Unity-Dist /Y