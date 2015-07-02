# OSVR Unity Plugin Changes

This is an abbreviated changelog for the OSVR Unity Plugin.

Use git for a full changelog.
##Recent Changes (updated 30-June-2015)

##Update/rename of Managed-OSVR assembly v0.1-94-gf4d3c44
- The Unity plugin now uses the external Managed-OSVR project and assembly, providing improved reliability, development progress, and 64-bit support.
- Projects upgrading to the new version should remove all copies of `ClientKit.dll` from their `Assets` (including subdirectories) before importing the updated package, as the file has been renamed to `OSVR.ClientKit.dll` to match .NET conventions.

##Displays Folder removed v0.1.78-ge7ad2a0 (31-May-2015)
- Unity developers no longer need to assign a JSON display descriptor in the Unity editor. The OSVR server sends the display descriptor to Unity, so switching HMDs requires changing the server config file to use a different display, not recompiling your Unity project. 

##DLL Search Path v0.1-69-gb80966b (31-March-2015)
- The built executable will now find the plugins it needs in the _Data folder. Previously, the executable had to be in the same directory with Assets/Plugins. That is no longer necessary.

##External Json File v0.1-56-gf2d8bab (28-March-2015)
- The JSON file containing display configuration can now be read from a file at runtime. To do this, add a config file "hmd.json" to the _Data folder that is created in a build. The format of the JSON file has not changed at all. It is the same file you would drag-and-drop onto the DisplayInterface component on your VRDisplayTracked prefab. If "hmd.json" does not exist in the Data folder (and it won't by default unless you put it there), then the plugin will look for the JSON file assigned in the DisplayInterface component in your scene. If that also has not been assigned, it will fall back to reading OSVR's /display parameter (this will eventually be the default).

##Distortion v0.1-55-g046c709(20-March-2015)
- Unity 4 Free limitations. The new distortion shader will not work with Unity 4 Free version. It will work with Unity 4 Pro or Unity 5. If you are using Unity 4 Free, everything will still work but there won't be any distortion. This is due to the distortion shader using a render to texture pass.
- Fixed Timestep was changed from 0.2 to 0.01667. This is how often FixedUpdate() gets called. The change should result in a more comfortable VR experience, but make sure it doesn't break any physics in your game.
- Changed the default quality settings. Turned on 4x AA and disabled shadows.

##Unity 5 Update v0.1-55-g046c709 (updated 18-March-2015)
- Refactored VRHead.cs and VREye.cs to be Unity 5-friendly (mostly instances of "camera" that needed to become GetComponent<Camera>()). Upgrading from Unity 4 to Unity 5 should be seamless.
- VRDisplayTracked.prefab has been updated to use the json descriptor file for the HDK by default. If you are using another HMD, be sure to set the corresponding json file on the VRDisplayTracked prefab's DisplayInterface script. 
- Fixed issue with stereo overlap and roll for HMDs with multiple video inputs.
