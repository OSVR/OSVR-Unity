# OSVR Unity Plugin Changes

This is an abbreviated changelog for the OSVR Unity Plugin.

Use git for a full changelog.

##Recent Changes (updated 18-March-2015)
- Refactored VRHead.cs and VREye.cs to be Unity 5-friendly (mostly instances of "camera" that needed to become GetComponent<Camera>()). Upgrading from Unity 4 to Unity 5 should be seamless.
- VRDisplayTracked.prefab has been updated to use the json descriptor file for the HDK by default. If you are using another HMD, be sure to set the corresponding json file on the VRDisplayTracked prefab's DisplayInterface script. 
- Fixed issue with stereo overlap and roll for HMDs with multiple video inputs.
