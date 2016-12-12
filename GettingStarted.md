# OSVR for Unity Developers

> Last Updated November 29, 2016

## Why use OSVR?

Open-source Virtual Reality (OSVR) is an open-source software platform for VR/AR applications. It provides abstraction layers for VR devices and peripherals so that a game developer can support many different VR devices with one SDK, rather than maintaining a separate SDK for each supported device. 
OSVR provides interfaces – pipes of data – as opposed to an API tied to a specific piece of hardware. If you want hand position, for example, OSVR will give you hand position regardless of whether the data comes from a Microsoft Kinect, Razer Hydra, or Leap Motion. 
Game developers can focus on what they want to do with the data, as opposed to how to obtain it.

For a more complete introduction to OSVR, please visit: http://osvr.github.io/whitepapers/introduction_to_osvr/

The rest of this document assumes you're ready to start developing VR applications in Unity with the OSVR-Unity plugin.

## Install OSVR SDK

**Download and install the latest OSVR SDK installer from: http://access.osvr.com/binary/osvr-sdk-installer**

This installer creates a directory: "C:\Program Files\OSVR\"

We can find **osvr_server.exe** in the directory: "C:\Program Files\OSVR\Runtime\bin"

## OSVR Server

When using OSVR, **OSVR server needs to be running in order for your application to receive data from connected devices.**
There are multiple ways to start the server (pick one):

- As of July 14, 2016 (OSVR-Unity Build 421), the server will launch automatically when your Unity application is launched on Windows. This option is configurable on the ClientKit prefab if you wish to disable server-autostart. This feature is not yet available on Android. 
- **osvr_central.exe**, which ships with the OSVR SDK, acts as a hub for multiple OSVR utilities/tools, controls and settings. You can launch and configure the server from this app.
- Launch the server and other OSVR utilities from the **OSVR Editor Window**, available from the Unity menu bar.
- Launch the server manually by double-clicking **osvr_server.exe**. By default, it uses **osvr_server_config.json** as a configuration file. You can use a different config file by dragging-and-dropping the .json config file onto **osvr_server.exe**.
- Launch the server from the command line, with the config file passed in as an argument, such as "osvr_server.exe osvr_server_config.example.json"

Since the configuration file is crucial to enabling functionality in OSVR, we will explore it in more detail later.

This document will assume you’re using a Hacker Development Kit HMD, HDK 1.x or HDK2. If you need help with HDK setup, please visit the HDK Unboxing and Starting Guide: https://github.com/OSVR/OSVR-General/wiki/HDK-Unboxing-and-Getting-Started

Connect the device and launch **osvr_server.exe**. If you see this message:

The "Added device" message shows that the server has found an HDK:

![osvr_server found HDK](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_foundhdk.png?raw=true)

## Tracker View

The Tracker View utility (OSVRTrackerView.exe) is helpful for quickly checking if tracking is working as expected. It shows the position and orientation of tracked objects. Here we see the orientation of the HDK without positional tracking. Z (blue) points towards the back of the head, Y (green) up, and X (red) to the right.

![Tracker View no positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_nopos.png?raw=true)

## Getting Started with OSVR-Unity
For an introductory video tutorial, visit: https://youtu.be/xSOq3bOBPxs

Let’s examine the OSVR-Unity plugin. You can view the source code at https://github.com/OSVR/OSVR-Unity. 

**Download the latest OSVR-Unity plugin from: http://access.osvr.com/binary/osvr-unity**

or get OSVR-Unity from the Unity Asset Store: http://u3d.as/g8N. This version may be slightly behind the link above.

* Create a new Unity project. OSVR works with Unity 4.6 or higher, but we recommend Unity 5.3 or higher.
* Import OSVRUnity.unitypackage into the project.
* Open VRFirstPerson.unity scene. This scene demonstrates a first-person controller VR setup.
* Optionally open the OSVR Editor window from the Unity menu bar (pictured below). You can use this to quickly change config files, launch utilities, and access links to documentation.

![OSVR-Unity Editor](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_editor.png?raw=true)

Now let’s examine some of the objects in the scene hierarchy:

### ClientKit
The ClientKit object communicates with OSVR Server and must be in every scene. It requires an app ID, which can be any string identifier.

![OSVR-Unity ClientKit](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_clientkit.png?raw=true)

### VRDisplayTracked
This prefab contains a DisplayController component, which constructs our tracked camera rig at runtime.

![OSVR-Unity VRDisplayTracked](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_vrdisplaytracked.png?raw=true)

Press Play. The stereo camera rig is created at runtime and updates its orientation as the HDK moves around. With my HDK connected as an extended display, I can click-and-drag the Game View to the HDK display and select Maximize on Play to view the game in VR.

### VRViewer
A child of VRDisplayTracked, VRViewer is the "head" in our VR setup. It moves around with the HMD. If you want to know which direction the user is looking, use this transform's forward direction.

### VREye
A child of VRViewer, represents the position and rotation of the user's eye(s). In a typical, two eye configuration, the second eye will be created at runtime. VREye does not have a camera component, delegating that responsiblity to its VRSurface child.

### VRSurface
A child of VREye, it has a camera component and is responsible for rendering each eye's view. Any image effects should be attached to this gameobject. Any attached scripts will be duplicated to any additional eye created at runtime. 

### Recenter

![OSVR-Unity Recenter](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_recenter.png?raw=true)

The **SetRoomRotationUsingHead** script “recenters” the room when a key is pressed (“R” by default in this sample), so that your head is facing the forward direction. There is another key (“U” by default in this sample) for undoing that rotation.

### VRFirstPersonController

![OSVR-Unity FPS Controller](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_fpc.png?raw=true)

This prefab provides very similar controls to the Unity first-person controller scripts. Mouse-look can be controlled with the “M” key, by default.

That covers the basics! There are examples of tracked controllers, eyetrackers, and locomotion in other example scenes (see TrackerView.unity). If you want to enable features like positional tracking and Direct Mode rendering, that all happens in the server configuration file.

## RenderManager and Unity-Rendering Plugin
RenderManager provides a number of additional functions in support of VR rendering. It adds features such as Direct Mode support, distortion correction, client-side predictive tracking, asynchronous time warp, overfill, and oversampling. For a more information about OSVR-RenderManager, including an overview of configuration options, visit: https://github.com/sensics/OSVR-RenderManager

For a more information about the Unity Rendering Plugin which enables OSVR-RenderManager in OSVR-Unity projects, visit: https://github.com/OSVR/OSVR-Unity-Rendering/blob/master/README.md

### Enabling RenderManager in Unity
RenderManager support is enabled in the server config file by adding a "renderManagerConfig" section:
![OSVR-Unity RenderManager Config](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_rendermanager_config.png?raw=true)

The following DLLs included in the unitypackage are also required for RenderManager support and are found in Assets/Plugins/x86_64 or Assets/Plugins/x86:
* glew32.dll
* SDL2.dll
* osvrUnityRenderingPlugin.dll -- built from https://github.com/OSVR/OSVR-Unity-Rendering
* osvrRenderManager.dll
* D3Dcompiler_47.dll -- Windows 7 only

It is possible to update RenderManager features in an existing Unity project by only replacing **osvrUnityRenderingPlugin.dll** and **osvrRenderManager.dll**. Most users will not need to do this, but it is good to know.

## Recommended OSVR Server Configs -- HDK2
If you're getting started with the HDK2, these are good configuration files to start with. To use these configurations, replace the contents of osvr_server_config.json with the contents of the files below, which can also be found in the sample-configs directory. Alternatively, drag-and-drop a server config file onto osvr_server.exe to start the server with that configuration.
### RenderManager - Direct mode, orientation only, client-side prediction
https://github.com/OSVR/OSVR-Core/blob/master/apps/sample-configs/osvr_server_config.renderManager.HDKv2.0.direct.json
### RenderManager - Extended mode, orientation only, client-side prediction
https://github.com/OSVR/OSVR-Core/blob/master/apps/sample-configs/osvr_server_config.renderManager.HDKv2.0.extended.json

For more information about client-side prediction, see: https://github.com/OSVR/OSVR-Docs/blob/master/Configuring/PredictiveTracking.md

### Optimizing RenderManager in Unity
There are a number of configuration options available when using RenderManager. We’ve found that settings may vary per application, so developers should experiment until an acceptable balance of visual quality and fast rendering is achieved. Generally, the configurations above are a good starting point. 

If judder is present (smearing and double-images), try increasing the "maxMsBeforeVsync" setting (0, 3, 5, etc.). This value should be no larger than the duration of one frame in milliseconds.

See the RenderManager optimization document for more information: https://github.com/sensics/OSVR-RenderManager/blob/master/doc/renderingOptimization.md

### Direct Mode Preview
You can see the game view mirrors the HDK display because the “Show Direct Mode Preview” option is checked on the VRDisplaytTracked prefab. It is a rather suboptimal implementation of mirror mode, and will soon be replaced by an equivalent RenderManager feature.

![OSVR-Unity Direct Mode Preview](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_directmode_preview.png?raw=true)

### Troubleshooting RenderManager in Unity
One issue users could run into is a "Failed to create RenderManager" error message in the Unity debug console. This could happen if you are trying to run in direct mode, but your machine does not support direct mode. It could also happen if the USB or HDMI cable is unplugged, or there could be an incompatibility with the version of RenderManager in your project and your currently installed graphics drivers. 

Users experiencing direct mode issues with HDK2 firmware 1.97 or below should update their firmware wto avoid most of the issues below.
Use OSVR Control to change firmware: http://sensics.com/software/OSVRControl-SW/publish.htm

Here are some general troubleshooting steps for fixing "Failed to create RenderManager" and other common issues, many of which have been fixed via firmware updates after v1.97:
- Update to the latest RenderManager DLLs which are included with the Unity packages here: http://access.osvr.com/binary/osvr-unity
- Use a known-working configuration file with orientation only. See the "Recommended OSVR Server Configs" section above.
- Update your graphics card drivers.
- HDK must be in direct mode already for a game to work in direct mode, it won't switch to direct mode automatically based on the configuration. The same goes for extended mode. Use EnableOSVRDirectMode.exe and DisableOSVRDirectMode.exe for switching between direct and extended mode. You know if you're in direct mode if the HDK disappears from Windows display settings.
- Run the RenderManager examples included with the SDK, such as RenderManagerD3DExample3D.exe. If these examples don't work, neither will RenderManager in Unity.
- If the RenderManager examples "work" but nothing displays (you keep seeing "Rendering at 90fps..." messages but no image in RenderManagerD3DPresentExample3d.exe), try switching to extended mode, power cycle (unplug the power and HMD from beltbox, wait a few seconds, replug), then switch back to direct mode.
- If USB disconnects between direct mode sessions, power cycle. If it happens frequently, make sure you have updated the HDK2 firmware to a version newer than 1.97.


Follow this guide for more RenderManager troubleshooting: https://github.com/OSVR/OSVR-Docs/blob/master/Troubleshooting/RenderManager.md#troubleshooting-rendermanager-in-unity.

## OSVR Server Configuration - Adding Positional Tracking
Positional tracking, like RenderManager, requires some setup in your server configuration file. Many sample configuration files are provided in the sample-configs directory of the OSVR SDK installation that include positional tracking.

For this example, we'll use a configuration for HDK 1.3 (which also works for 1.4).

Copy the contents of **osvr_server_config.HDK13DirectMode.sample.json** into **osvr_server_config.json**.

![OSVR Snapshot sample-configs](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_snapshot_sampleconfigs.png?raw=true)

Let’s examine our new **osvr_server_config.json**:

![osvr_server_config](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos_rm.png?raw=true)

These two lines determine which HMD display to use, and which RenderManager config file to use. If you didn’t want positional tracking, these are the only two lines you need to enable RenderManager with the HDK 1.3.

![osvr_server_config snippet](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos_rm_snippet.png?raw=true)

Note that the "display" and "renderManagerConfig" entries are referencing external files. This is not required. It only makes the file shorter and easier to read. You may prefer to include the contents of the referenced files in the server config file. See the Recommended HDK2 configurations above for examples of this.

The rest of the file deals with video-based tracking and sensor fusion. There are options for showing a camera debug view (impacts game performance if this is running in the background), enabling or disabling the LEDs located on the back of the head-strap, changing head circumference, and selecting a calibration file.

![osvr_server_config positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos.png?raw=true)

## Positional Tracking - Calibration
You’ll want to calibrate the camera before using positional tracking, and every time the camera moves. For a full guide on calibration, visit: https://github.com/OSVR/OSVR-Core/wiki/Video-Based-Tracking-Calibration

### Tracker View - Positional Tracking
Let’s run the server and check Tracker View again to see if we’re getting positional tracking.

![Tracker View Positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_pos.png?raw=true)

Yes! The gizmo is no longer fixed at the origin, and moves around with the HMD. 

## Quality Settings
Quality settings will differ per machine/graphics card capabilities. Make sure V Sync Count is set to Don’t Sync. Otherwise, we generally recommend using as much antialiasing as can be afforded without negatively affecting performance. Disable shadows unless your game mechanics demand it. Use Lightmapping instead of real-time lights. Use occlusion culling.

![OSVR-Unity Quality Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_quality.png?raw=true)

## Player Settings
The “Virtual Reality Supported” checkbox does not need to be checked, since OSVR is not a native Unity VR platform. If you are supporting multiple VR SDKs and need to check the "Virtual Reality Supported", make sure to add "None" for OSVR support. You can launch your OSVR game with the command-line option:
```
MyGame.exe -vrmode none 
```

![OSVR-Unity Player Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_player.png?raw=true)

## Building for Android
Building an OSVR Android app requires libraries from https://github.com/OSVR/OSVR-Android-SDK. As of OSVR-Unity-v0.6.4-37 build number 317, these are included by default in the OSVRUnity plugin.

Make sure your project is set to build for Android:

![OSVR-Unity Android Build](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_androidbuild.png?raw=true)

Before launching the app on an Android device, you'll need to run the OSVR-AndroidServerLauncher app first. Please visit this page for more information (.apk available in the Releases section): https://github.com/OSVR/OSVR-AndroidServerLauncher

An example Android app can be downloaded from the Releases section of OSVR-Unity-Palace-Demo: https://github.com/OSVR/OSVR-Unity-Palace-Demo/releases/tag/v0.6.4.23-android.

