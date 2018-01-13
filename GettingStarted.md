# OSVR for Unity Developers

> Last Updated January 11, 2018
## Contents
- [Introduction to OSVR](#intro) - An overview of OSVR including how to start and configure OSVR server.
        - [Installing OSVR SDK](#installsdk)
        - [OSVR Server](#osvrserver)
        - [Tracker View](#trackerview)
- [Getting Started with OSVR-Unity](#osvrunity)
        - [Unity VR Rendering Path](#unityvr)
        - [Legacy OSVR-Unity Rendering Path](#osvrlegacy)
- [RenderManager and Unity-Rendering-Plugin](#osvr-rendermanager)
        - [Server configuration](#osvr-directmode)
        - [Optimizing RenderManager](#osvr-opt-render)
        - [Troubleshooting RenderManager](#osvr-trouble-render)
- [Positional Tracking](#osvr-pos)
- [Quality Settings](#osvr-quality)
- [Player Settings](#osvr-player)
- [Building for Android](#osvr-android)
## <a name="intro"></a>Introduction to OSVR
Open-source Virtual Reality (OSVR) is an open-source software platform for VR/AR applications. It provides abstraction layers for VR devices and peripherals so that a game developer can support many different VR devices with one SDK, rather than maintaining a separate SDK for each supported device. OSVR provides generic interfaces which can be thought of like pipes of data between applications and hardware. Data is accessible to applications developers via a ["semantic path"](https://osvr.github.io/presentations/20150419-osvr-software-framework-path-tree/) naming system similar to URLs. An OSVR server plugin specifies which paths it is supplying data for, while OSVR client applications looks for data at the semantic paths it cares about. For example, OSVR server plugins for Leap Motion and Razer Hydra send hand positional tracking data to the semantic paths **/me/hands/left** and **/me/hands/right**. The OSVR application looks for pose data at those named paths, oblivious to what hardware is connected.
For a more complete introduction to OSVR, please visit: http://osvr.github.io/whitepapers/introduction_to_osvr/

### <a name="installsdk"></a>Installing OSVR SDK

**Download and install the latest OSVR SDK installer from: http://access.osvr.com/binary/osvr-sdk-installer**

This installer creates a directory: "C:\Program Files\OSVR\"

We can find **osvr_server.exe** in the directory: "C:\Program Files\OSVR\Runtime\bin"

### <a name="osvrserver"></a>OSVR Server

When using OSVR, **OSVR server needs to be running in order for your application to receive data from connected devices.**
There are multiple ways to start the server (pick one):

- Launch the server manually by double-clicking **osvr_server.exe**. By default, it uses **osvr_server_config.json** as the configuration file if it is not given. 
You can use a different config file by dragging-and-dropping the .json config file onto **osvr_server.exe**.
![osvr_server_drag_drop](/images/osvr_server_drag_drop.png?raw=true)

- **Server Autostart**. The server can launch automatically when your Unity application is launched on Windows and Android. This option is configurable on the ClientKit prefab if you wish to disable server-autostart.
![OSVR-Unity ClientKit](images/osvr_unity_clientkit.png?raw=true)
- **osvr_central.exe**, which ships with the OSVR SDK, acts as a hub for multiple OSVR utilities/tools, controls and settings. You can launch and configure the server from this app.
- Launch the server and other OSVR utilities from the **OSVR Editor Window**, available from the Unity menu bar.
- Launch the server from the command line, with the config file passed in as an argument, such as "osvr_server.exe osvr_server_config.example.json"

Since the configuration file is crucial to enabling functionality in OSVR, we will explore it in more detail later.

This document will assume you’re using a Hacker Development Kit HMD, HDK 1.x or HDK2. If you need help with HDK setup, please visit the HDK Unboxing and Starting Guide: https://github.com/OSVR/OSVR-General/wiki/HDK-Unboxing-and-Getting-Started

Connect the device and launch **osvr_server.exe**.

The "Added device" message shows that the server has found an HDK:

![osvr_server found HDK](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_server_foundhdk.png?raw=true)

### <a name="trackerview"></a>Tracker View

The Tracker View utility (OSVRTrackerView.exe) is helpful for quickly checking if tracking is working as expected. It shows the position and orientation of tracked objects. Here we see the orientation of the HDK without positional tracking. Z (blue) points towards the back of the head, Y (green) up, and X (red) to the right.

![Tracker View no positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_nopos.png?raw=true)

## <a name="osvrunity"></a>Getting Started with OSVR-Unity
The rest of this document assumes you're ready to start developing VR applications in Unity with the OSVR-Unity plugin.

### <a name="unityvr"></a>Unity VR Rendering Path
As of September 18, 2017, the plugin supports Unity's native VR mode. This means that instead of using the **DisplayController** and **VRDisplayTracked** prefabs (OSVR-Unity Legacy VR Mode) for rendering, instead we can enable Unity's VR Support and Single-Pass rendering path, rendering both eyes with the same camera. This is a major optimization over the two-camera setup OSVR-Unity has used previously. It is recommended to use this new rendering path if possible.

The **OSVR-UnityVR-Demo.unity** scene demonstrates the new **OsvrStereoCamera** prefab which uses Unity’s split-screen stereo VR rendering. To use this rendering path, make sure “Virtual Reality Supported” is checked in Player Settings, and Split-Screen Stereo (non head-mounted) is selected as the SDK.

![OSVR-Unity Player Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_player.png?raw=true)

Note that when using this rendering path, you won't using any of the DisplayController, VRViewer, VREye, or VRSurface prefabs.
### Tutorials and Resources
Let’s examine the OSVR-Unity plugin. You can view the source code at https://github.com/OSVR/OSVR-Unity. 

**Download the latest OSVR-Unity plugin from: http://access.osvr.com/binary/osvr-unity**

or get OSVR-Unity from the Unity Asset Store: http://u3d.as/g8N. This version may be slightly behind the link above.

* Create a new Unity project. OSVR works with Unity 4.6 or higher, but we recommend Unity 5.4 or higher.
* Import OSVRUnity.unitypackage into the project.
* Open OSVR-UnityVR-Demo.unity scene. This scene demonstrates a first-person controller VR setup.
* Optionally open the OSVR Editor window from the Unity menu bar (pictured below). You can use this to quickly change config files, launch utilities, and access links to documentation.

![OSVR-Unity Editor](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_editor.png?raw=true)

Now let’s examine some of the objects in the scene hierarchy:

### ClientKit
The ClientKit object communicates with OSVR Server and must be in every scene. It requires an app ID, which can be any string identifier.

![OSVR-Unity ClientKit](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_clientkit.png?raw=true)

### OsvrStereoCameraParent
This prefab is a VR camera with the OsvrUnityNativeVR.cs component attached, OsvrMirrorDisplay component is also attached for mirroring the game view in a window. With the server running, press play to go to VR mode with head tracking.

![OSVR-Unity-NativeVR](images/osvr_unity_nativevr.png?raw=true)

## <a name="unityvr"></a>Legacy OSVR-Unity Rendering Path
Scenes which use the VRDisplayTracked and DisplayController prefabs are using the legacy OSVR-Unity plugin. This was the default rendering method before Unity native VR mode. It is recommended to use the newer rendering path, but if circumstances dictate the more primitive approach, the legacy prefabs are described below.
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

## <a name="osvr-rendermanager"></a>RenderManager and Unity-Rendering-Plugin
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

## <a name="osvr-directmode"></a>Recommended OSVR Server Configs -- HDK2
If you're getting started with the HDK2, these are good configuration files to start with. To use these configurations, replace the contents of osvr_server_config.json with the contents of the files below, which can also be found in the sample-configs directory. Alternatively, drag-and-drop a server config file onto osvr_server.exe to start the server with that configuration.
### RenderManager - Direct mode, orientation only, client-side prediction
https://github.com/OSVR/OSVR-Core/blob/master/apps/sample-configs/osvr_server_config.renderManager.HDKv2.0.direct.json
### RenderManager - Extended mode, orientation only, client-side prediction
https://github.com/OSVR/OSVR-Core/blob/master/apps/sample-configs/osvr_server_config.renderManager.HDKv2.0.extended.json

For more information about client-side prediction, see: https://github.com/OSVR/OSVR-Docs/blob/master/Configuring/PredictiveTracking.md

### <a name="osvr-opt-render"></a>Optimizing RenderManager in Unity
There are a number of configuration options available when using RenderManager. We’ve found that settings may vary per application, so developers should experiment until an acceptable balance of visual quality and fast rendering is achieved. Generally, the configurations above are a good starting point. 

If judder is present (smearing and double-images), try increasing the "maxMsBeforeVsync" setting (0, 3, 5, etc.). This value should be no larger than the duration of one frame in milliseconds.

See the RenderManager optimization document for more information: https://github.com/sensics/OSVR-RenderManager/blob/master/doc/renderingOptimization.md

### <a name="osvr-trouble-render"></a>Troubleshooting RenderManager in Unity
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

## <a name="osvr-pos"></a>Positional Tracking - Calibration
You’ll want to calibrate the camera before using positional tracking, and every time the camera moves. For a full guide on calibration, visit: https://github.com/OSVR/OSVR-Core/wiki/Video-Based-Tracking-Calibration

### Tracker View - Positional Tracking
Let’s run the server and check Tracker View again to see if we’re getting positional tracking.

![Tracker View Positional](https://github.com/OSVR/OSVR-Unity/blob/master/images/tracker_view_pos.png?raw=true)

Yes! The gizmo is no longer fixed at the origin, and moves around with the HMD. 

## <a name="osvr-quality"></a>Quality Settings
Quality settings will differ per machine/graphics card capabilities. Make sure V Sync Count is set to Don’t Sync. Otherwise, we generally recommend using as much antialiasing as can be afforded without negatively affecting performance. Disable shadows unless your game mechanics demand it. Use Lightmapping instead of real-time lights. Use occlusion culling.

![OSVR-Unity Quality Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/osvr_unity_quality.png?raw=true)

## <a name="osvr-player"></a>Player Settings
As of September 18, 2017, there is experimental support for Unity's native VR rendering. If you're using the **OsvrStereoCamera** prefab, the “Virtual Reality Supported” option in Player Settings should be enabled. If you're using **DisplayController** and **VRViewer** prefabs instead, either disable the “Virtual Reality Supported” option in Player Settings, or add "None". You can launch your OSVR game with the command-line option:
```
MyGame.exe -vrmode none 
```

## <a name="osvr-android"></a>Building for Android
The libraries required for building for Android are included in the OSVR-Unity source. These will eventually be migrated out of the OSVR-Unity repo when the CI build is updated and will copy them for us when the unitypackage is created.

### Server Config File
Copy a file named **osvr_server_config.json** to _/sdcard/osvr/_.
The contents of osvr_server_config.json should match your display. Here is a server config for a Samsung Galaxy S6:
<details>
  <summary>Click to expand osvr_server_config.json</summary>
  
```json
{
  "display": {
    "meta": {
      "schemaVersion": 1
    },
    "hmd": {
      "device": {
        "vendor": "Samsung",
        "model": "Galaxy S6",
        "num_displays": 1,
        "Version": "1.1",
        "Note": "Samsung Galaxy S6"
      },
      "field_of_view": {
        "monocular_horizontal": 90,
        "monocular_vertical": 101.25,
        "overlap_percent": 100,
        "pitch_tilt": 0
      },
      "resolutions": [
        {
          "width": 2560,
          "height": 1440,
          "video_inputs": 1,
          "display_mode": "horz_side_by_side",
          "swap_eyes": 0
        }
      ],
      "distortion": {
        "distance_scale_x": 1,
        "distance_scale_y": 1,
        "polynomial_coeffs_red": [0, 1, -1.74, 5.15, -1.27, -2.23 ],
        "polynomial_coeffs_green": [0, 1, -1.74, 5.15, -1.27, -2.23 ],
        "polynomial_coeffs_blue": [0, 1, -1.74, 5.15, -1.27, -2.23 ]
      },
      "rendering": {
        "right_roll": 0,
        "left_roll": 0
      },
      "eyes": [
        {
          "center_proj_x": 0.5,
          "center_proj_y": 0.5,
          "rotate_180": 0
        },
        {
          "center_proj_x": 0.5,
          "center_proj_y": 0.5,
          "rotate_180": 0
        }
      ]
    }
  },
  "renderManagerConfig": {
    "meta": {
      "schemaVersion": 1
    },
    "renderManagerConfig": {
      "directModeEnabled": false,
      "directDisplayIndex": 0,
      "directHighPriorityEnabled": false,
      "numBuffers": 2,
      "verticalSyncEnabled": false,
      "verticalSyncBlockRenderingEnabled": false,
      "renderOverfillFactor": 1.0,
      "window": {
        "title": "OSVR",
        "fullScreenEnabled": false,
        "xPosition": 0,
        "yPosition": 0
      },
      "display": {
        "rotation": 0,
        "bitsPerColor": 8
      },
      "timeWarp": {
        "enabled": true,
        "asynchronous": false,
        "maxMsBeforeVSync": 5
      }
    }
  },
  "plugins": [
    "com_osvr_android_sensorTracker"
  ]
}
```
</details>

### Server Autostart
The current default path relies on the server autostart feature. Check the "server autotostart" flag on the ClientKit prefab for Android builds. See the OSVR-UnityVR-Android-Demo.unity scene. If the server fails to start, you'll see a black screen, have no tracking, or the application could crash.

### Disable Mirror Mode
If you target is a phone display and not an HMD, you'll want to disable the OsvrMirrorDisplay component in your scene.

### Player Settings
Note that you'll need Write Access to the External SD Card, and Internet Access set to Required. The following player settings have been tested to work with Unity 2017.1.0f3 and Unity 5.6.1f1:

![OSVR-Unity Android Player Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/unity_2017_android_playersettings.png?raw=true)

