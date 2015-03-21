# OSVR Unity Plugin Changes

This is an abbreviated changelog for the OSVR Unity Plugin.

Use git for a full changelog.
##Recent Changes (updated 18-March-2015)
##Distortion (20-March-2015)
- Unity 4 Free limitations. The new distortion shader will not work with Unity 4 Free version. It will work with Unity 4 Pro or Unity 5. If you are using Unity 4 Free, everything will still work but there won't be any distortion. This is due to the distortion shader using a render to texture pass.
- Fixed Timestep was changed from 0.2 to 0.01667. This is how often FixedUpdate() gets called. The change should result in a more comfortable VR experience, but make sure it doesn't break any physics in your game.
- Changed the default quality settings. Turned on 4x AA and disabled shadows.

##Unity 5 Update (updated 18-March-2015)
- Refactored VRHead.cs and VREye.cs to be Unity 5-friendly (mostly instances of "camera" that needed to become GetComponent<Camera>()). Upgrading from Unity 4 to Unity 5 should be seamless.
- VRDisplayTracked.prefab has been updated to use the json descriptor file for the HDK by default. If you are using another HMD, be sure to set the corresponding json file on the VRDisplayTracked prefab's DisplayInterface script. 
- Fixed issue with stereo overlap and roll for HMDs with multiple video inputs.
