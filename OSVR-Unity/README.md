# OSVR-Unity README

## Documentation
For links to details, documentation, and support, visit the repository on github: https://github.com/OSVR/OSVR-Unity#readme

## Known Issues
This list only includes Unity-specific issues that have a substantial impact on the development experience. For a full list of issues, see the [GitHub issue tracker](https://github.com/sensics/OSVR-Unity/issues)

- Unity 4 Free version will not use distortion as this is a Pro-only feature. You will get a warning that you can ignore and continue to use OSVR without distortion. OSVR plugins will still work with Unity 4 Free. 

## Basic Principles and Files
On a machine where you're running an OSVR-Unity application, you need to run an OSVR server, part of the OSVR-Core builds. For convenience, a 32-bit OSVR Server install is bundled in the OSVR-Unity snapshot archives. [Contact us](mailto:support@osvr.org) if you need help.

The `OSVR-Unity.unitypackage` package should contain the x86 and x86_64 binary plugins, the compiled Managed-OSVR wrapper, the OSVRUnity scripts (in the `Assets` directory), and a directory of prefabs. Import this package into your project.

There are also a few sample/demo scenes included.

### ClientKit object
You need exactly one instance of `OSVR.Unity.ClientKit` in your project: get one using the `ClientKit` prefab. You need to set the app ID: use a reversed DNS name as seen elsewhere (Java, Android, etc). This just uniquely identifies your application to the OSVR software. If you fail to do this, you'll see an error in the Unity console.

### Tracking
For trackers (Pose, Position, Orientation), there are prefabs of nodes that update their transform accordingly. You'll need to set the path you want to use. Please see the C/C++ documentation for client apps to find valid interface paths. (Note that the OSVR-Unity package handles normalization of the coordinate system to the Unity standard: ignore the one seen in the C++ documentation.)

### Manually handling callbacks
This involves two pieces:

- Adding an `OSVR.Unity.InterfaceGameObject` script component, in which you can specify the path. There is a prefab for this.
- Adding your own script component (which should inherit from `OSVR.Unity.InterfaceBase` instead of `MonoBehaviour` for simplest usage) that uses the `InterfaceGameObject` to register a callback.

Examples for buttons and analog triggers are included in the `minigame` scene.

Paths for these callbacks that provide useful information can be found in the main OSVR-Core documentation on the "Writing a client application" page.

### Rendering
The optimal path is to use OSVR-Unity with a direct-mode enabled config file. **OSVRDemo.unity** and **OSVRDemo2.unity** are simple scenes which demonstrate how to use the OSVR prefabs without harnessing Unity's built-in VR SDK. 
**OSVR-UnityVR-Demo.unity** is a scene with a prefab which uses Unity's native VR support and benefits from optimizations like single-pass rendering.

If a RenderManager configuration is not provided, the plugin will fall back to a more primitive rendering path.

### Other interaction
Any other interaction with the OSVR framework should go directly through the Managed-OSVR (.NET) wrapper without any Unity-specific adaptations. See that source for examples of button and analog callbacks, as well as display parameter access (ideally used to set up the display properly). In terms of API, the Managed-OSVR API is effectively a direct translation of the C++ wrappers of OSVR `ClientKit`, so please see the main OSVR-Core client documentation for more information.

### Execution
A standalone player built for Windows may end up needing the `-adapter N` argument, where `N` is a Direct3D display adapter, to put the rendered output on the HMD display. This is usually not necessary.

## Building for Android
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

![OSVR-Unity Android Player Settings](https://github.com/OSVR/OSVR-Unity/blob/master/images/unity_2017_android_playersettings?raw=true)
