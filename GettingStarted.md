# OSVR for Unity Developers

> Last Updated July 28, 2016

## Why use OSVR?

Open-source Virtual Reality (OSVR) is an open-source software platform for VR/AR applications. It provides abstraction layers for VR devices and peripherals so that a game developer can support many different VR devices with one SDK, rather than maintaining a separate SDK for each supported device. 
OSVR provides interfaces – pipes of data – as opposed to an API tied to a specific piece of hardware. If you want hand position, for example, OSVR will give you hand position regardless of whether the data comes from a Microsoft Kinect, Razer Hydra, or Leap Motion. 
Game developers can focus on what they want to do with the data, as opposed to how to obtain it.

For a more complete introduction to OSVR, please visit: http://osvr.github.io/whitepapers/introduction_to_osvr/

The rest of this document assumes you're ready to start developing VR applications in Unity with the OSVR-Unity plugin. We also assume you are using a Hacker Development Kit

Before we jump into OSVR-Unity, you'll need to install the OSVR SDK:

## Install OSVR SDK

**Download and install the latest OSVR SDK installer from: http://osvr.github.io/using/**

This installer creates a directory: "C:\Program Files\OSVR\"

We can find **osvr_server.exe** in the directory: "C:\Program Files\OSVR\Runtime\bin"

## OSVR Server

When using OSVR, OSVR server needs to be running in order for your Unity application to receive data from connected devices.
There are multiple ways to start the server (pick one):

- As of July 14, 2016 (OSVR-Unity Build 421), the server will launch automatically when your Unity application is launched on Windows. This option is configurable on the ClientKit prefab if you wish to disable server-autostart. This feature is not yet available on Android. 
- **osvr_central.exe**, which ships with the OSVR SDK, acts as a hub for multiple OSVR utilities/tools, controls and settings. You can launch and configure the server from this app.
- Launch the server and other OSVR utilities from the **OSVR Editor Window**, available from the Unity menu bar.
- Launch the server manually by double-clicking **osvr_server.exe**. By default, it uses **osvr_server_config.json** as a configuration file. You can use a different config file by dragging-and-dropping the .json config file onto **osvr_server.exe**.
- Launch the server from the command line, with the config file passed in as an argument, such as "osvr_server.exe osvr_server_config.example.json"

Since the configuration file is crucial to enabling functionality in OSVR, we will explore it in more detail later.

This document will assume you’re using an HDK. If you need help with HDK setup, please visit the HDK Unboxing and Starting Guide: https://github.com/OSVR/OSVR-General/wiki/HDK-Unboxing-and-Getting-Started

Connect the device and launch **osvr_server.exe**. If you see this message:

The "Added device" message shows that the server has found an HDK:

![osvr_server found HDK](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_foundhdk.png?raw=true)

## Tracker View

The Tracker View utility (OSVRTrackerView.exe) is helpful for quickly checking if tracking is working as expected. It shows the position and orientation of tracked objects. Here we see the orientation of the HDK without positional tracking. Z (blue) points towards the back of the head, Y (green) up, and X (red) to the right.

![Tracker View no positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_nopos.png?raw=true)

## Getting Started with OSVR-Unity
Let’s examine the OSVR-Unity plugin. You can view the source code at https://github.com/OSVR/OSVR-Unity. 

**Download the latest OSVR-Unity plugin from: http://access.osvr.com/binary/osvr-unity**

or get OSVR-Unity from the Unity Asset Store: http://u3d.as/g8N. This version may be slightly behind the link above.

* Create a new Unity (4.6 or higher) project.
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


![OSVR-Unity SBS](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_sbs.png?raw=true)

### Recenter

![OSVR-Unity Recenter](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_recenter.png?raw=true)

The **SetRoomRotationUsingHead** script “recenters” the room when a key is pressed (“R” by default in this sample), so that your head is facing the forward direction. There is another key (“U” by default in this sample) for undoing that rotation.

### VRFirstPersonController

![OSVR-Unity FPS Controller](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_fpc.png?raw=true)

This prefab provides very similar controls to the Unity first-person controller scripts. Mouse-look can be controlled with the “M” key, by default.

That covers the basics! There are examples of tracked controllers, eyetrackers, and locomotion in other example scenes (see TrackerView.unity). If you want to enable features like positional tracking and Direct Mode rendering, that all happens in the server configuration file.

## OSVR Server Configuration - Adding Positional Tracking and RenderManager 
Want to enable positional tracking? Enable DirectMode rendering support? Use a device plugin? All of this requires some setup in your server configuration file. Many sample configuration files are provided in the sample-configs directory of the Core Snapshot. Some devices, such as the Hacker Development Kit (HDK) and Razer Hydra can be auto-detected and will work with a default (empty) config file.

**osvr_server.exe** can be run from the command line with an optional parameter for the server config file. If no parameter is specified, **osvr_server_config.json** will be used instead. Since the HDK is detected automatically, we don’t need to edit **osvr_server_config.json** in order to obtain tracking data from the HDK. If we want to enable positional tracking, however, we’ll need to edit the config file.

Copy the contents of **osvr_server_config.HDK13DirectMode.sample.json** into **osvr_server_config.json**.

![OSVR Snapshot sample-configs](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_snapshot_sampleconfigs.png?raw=true)

Let’s examine our new **osvr_server_config.json**:

![osvr_server_config](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos_rm.png?raw=true)

These two lines determine which HMD display to use, and which RenderManager config file to use. If you didn’t want positional tracking, these are the only two lines you need to enable RenderManager with the HDK 1.3.

![osvr_server_config snippet](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos_rm_snippet.png?raw=true)

The rest of the file deals with video-based tracking and sensor fusion. There are options for showing a camera debug view (impacts game performance if this is running in the background), enabling or disabling the LEDs located on the back of the head-strap, changing head circumference, and selecting a calibration file.

![osvr_server_config positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_config_pos.png?raw=true)

## Positional Tracking - Calibration
You’ll want to calibrate the camera before using positional tracking, and every time the camera moves. For a full guide on calibration, visit: https://github.com/OSVR/OSVR-Core/wiki/Video-Based-Tracking-Calibration

### Tracker View - Positional Tracking
Let’s run the server and check Tracker View again to see if we’re getting positional tracking.

![Tracker View Positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_pos.png?raw=true)

Yes! The gizmo is no longer fixed at the origin, and moves around with the HMD. 

## RenderManager and Unity-Rendering Plugin
RenderManager provides a number of additional functions in support of VR rendering. It adds features suc has Direct Mode support, distortion correction, client-side predictive tracking, asynchronous time warp, overfill, and oversampling. For a more information about OSVR-RenderManager, including an overview of configuration options, visit: https://github.com/sensics/OSVR-RenderManager

For a more information about the Unity Rendering Plugin which enables OSVR-RenderManager in OSVR-Unity projects, visit: https://github.com/OSVR/OSVR-Unity-Rendering/blob/master/README.md

### Enabling RenderManager in Unity
The following DLLs included in the unitypackage are required for RenderManager support are found in Assets/Plugins/x86_64 or Assets/Plugins/x86:
* glew32.dll
* SDL2.dll
* osvrUnityRenderingPlugin.dll -- built from https://github.com/OSVR/OSVR-Unity-Rendering
* osvrRenderManager.dll
* D3Dcompiler_47.dll -- Windows 7 only, available in the Unity install directory (C:/Program Files/Unity/Editor/Data/Tools64/)

It is possible to update RenderManager features in an existing Unity project by only replacing **osvrUnityRenderingPlugin.dll** and **osvrRenderManager.dll**. Most users will not need to do this, but it is good to know.

With the RenderManager DLLs in our project and a renderManagerConfig specified in the server config (also the default), when we run the server and press play, we are now in DirectMode with positional tracking! 

### Direct Mode Preview
You can see the game view mirrors the HDK display because the “Show Direct Mode Preview” option is checked on the VRDisplaytTracked prefab. It is a rather suboptimal implementation of mirror mode, and will soon be replaced by an equivalent RenderManager feature.

![OSVR-Unity Direct Mode Preview](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_directmode_preview.png?raw=true)

### Optimizing RenderManager in Unity
There are a number of configuration options available when using RenderManager. We’ve found that settings may vary per application, so developers should experiment until an acceptable balance of visual quality and fast rendering is achieved. Generally, the configuration below is a good starting point. See the RenderManager document for more information: https://github.com/OSVR/OSVR-Unity-Rendering/blob/master/README.md

![OSVR-Unity RenderManager Config](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_rendermanager_config.png?raw=true)

### Troubleshooting RenderManager in Unity
One issue users could run into is a "Failed to create RenderManager" error message in the Unity debug console. This could happen if you are trying to run in direct mode, but your machine does not support direct mode. It could also happen if the USB or HDMI cable is unplugged, or there could be an incompatibility with the version of RenderManager in your project and your currently installed graphics drivers.

Follow this guide to troubleshoot RenderManager: https://github.com/OSVR/OSVR-Docs/blob/master/Troubleshooting/RenderManager.md#troubleshooting-rendermanager-in-unity.

## Quality Settings
Quality settings will differ per machine/graphics card capabilities. Make sure V Sync Count is set to Don’t Sync. Otherwise, we generally recommend that using as much antialiasing as can be afforded without negatively affecting performance. Disable shadows unless your game mechanics demand it. Use Lightmapping instead of real-time lights. Use occlusion culling.

![OSVR-Unity Quality Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_quality.png?raw=true)

## Player Settings
Generally, use Static and Dynamic batching to reduce draw calls. Note that the “Virtual Reality Supported” checkbox does not need to be checked, since Unity currently only supports Oculus and GearVR with this feature. Leaving it checked should not impact OSVR performance, but you will see a “[VRDevice] Initialization of device oculus failed” message that can be ignored.

![OSVR-Unity Player Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_player.png?raw=true)

## Building for Android
Building an OSVR Android app requires libraries from https://github.com/OSVR/OSVR-Android-SDK. As of OSVR-Unity-v0.6.4-37 build number 317, these are included by default in the OSVRUnity plugin.

Make sure your project is set to build for Android:

![OSVR-Unity Android Build](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_androidbuild.png?raw=true)

Before launching the app on an Android device, you'll need to run the OSVR-AndroidServerLauncher app first. Please visit this page for more information: https://github.com/OSVR/OSVR-AndroidServerLauncher

An example Android app can be downloaded from the Releases section of OSVR-Unity-Palace-Demo: https://github.com/OSVR/OSVR-Unity-Palace-Demo/releases.

You will still need to follow the instructions linked above to push the OSVR-AndroidServerLauncher app to the phone with adb. Since the Palace scene is not optimized for mobile, the example app consists of the simpler OSVRDemo2.unity scene. There is a separate branch in that repo for the Android build in order to better organize the releases, but there aren't any code changes from the master branch. Some Project and Quality settings differ.
