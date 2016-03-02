set DEST=OSVR-Unity\Assets\Plugins\x86
move /Y RenderManager\RenderManager32\install\bin\glew32.dll %DEST%
move /Y RenderManager\RenderManager32\install\bin\SDL2.dll %DEST%
move /Y RenderManager\RenderManager32\install\bin\osvrRenderManager.dll %DEST%
move /Y RenderManager\RenderManager32\install\rendermanager-ver.txt %DEST%
set DEST=OSVR-Unity\Assets\Plugins\x86_64
move /Y RenderManager\RenderManager64\install\bin\glew32.dll %DEST%
move /Y RenderManager\RenderManager64\install\bin\SDL2.dll %DEST%
move /Y RenderManager\RenderManager64\install\bin\osvrRenderManager.dll %DEST%
move /Y RenderManager\RenderManager64\install\rendermanager-ver.txt %DEST%
del /F /Q RenderManager