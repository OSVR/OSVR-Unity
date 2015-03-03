# OSVR-Unity README

> _This is an Alpha Release, and probably not entirely idiomatic for Unity. Check back for updates often._

## Known Issues
This list only includes Unity-specific issues that have a substantial impact on the development experience. For a full list of issues, see the [GitHub issue tracker](https://github.com/sensics/OSVR-Unity/issues)

- Stereo will not work unless you load a display `.json` file manually into the display prefab. A number of these are bundled with the plugin: drag and drop the appropriate one into the slot in the Inspector.
- Display configuration is not always falling back to data from OSVR. ([GitHub issue](https://github.com/OSVR/OSVR-Unity/issues/2))

## Basic Principles and Files
On a machine where you're running an OSVR-Unity application, you need to run an OSVR server, part of the OSVR-Core builds. For convenience, a 32-bit OSVR Server install is bundled in the OSVR-Unity snapshot archives. [Contact us](mailto:support@osvr.org) if you need help.

The `OSVR-Unity.unitypackage` package should contain the x86 binary plugins, the compiled Managed-OSVR wrapper, the OSVRUnity scripts (in the `Assets` directory), and a directory of prefabs. Import this package into your project.

There is also a few sample/demo applications included, ranging from fairly sparse environments also used for development of the code/prefabs, to a high-detail demo.

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

### Other interaction
Any other interaction with the OSVR framework should go directly through the Managed-OSVR (.NET) wrapper without any Unity-specific adaptations. See that source for examples of button and analog callbacks, as well as display parameter access (ideally used to set up the display properly). In terms of API, the Managed-OSVR API is effectively a direct translation of the C++ wrappers of OSVR `ClientKit`, so please see the main OSVR-Core client documentation for more information.

### Execution
A standalone player built for Windows may end up needing the `-adapter N` argument, where `N` is a Direct3D display adapter, to put the rendered output on the HMD display.
