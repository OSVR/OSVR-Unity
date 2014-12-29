# OSVR-Unity README

> _This is an Alpha Release, and probably not entirely idiomatic for Unity. Check back for updates often._

## Basic Principles and Files
On a machine where you're running an OSVR-Unity application, you need to run an OSVR server. This is found in the OSVR-Core builds. [Contact us](mailto:support@opengoggles.org) if you need help.

The `OSVR-Unity.unitypackage` package should contain the x86 binary plugins, the compiled Managed-OSVR wrapper, the OSVRUnity scripts (in the `Assets` directory), and a directory of prefabs. Import this package into your project.

There is also a few sample/demo application included (that is also used for development of the code/prefabs).

### ClientKit object
You need exactly one instance of `OSVR.Unity.ClientKit` in your project: get one using the `ClientKit` prefab. You need to set the app ID: use a reversed DNS name as seen elsewhere (Java, Android, etc). This just uniquely identifies your application to the OSVR software. If you fail to do this, you'll see an error in the Unity console.

### Tracking
For trackers (Pose, Position, Orientation), there are prefabs of nodes that update their transform accordingly. You'll need to set the path you want to use. Please see the C/C++ documentation for client apps to find valid interface paths. (Note that the OSVR-Unity package handles normalization of the coordinate system to the Unity standard: ignore the one seen in the C++ documentation.)

### Other interaction
Any other interaction with the OSVR framework goes directly through the Managed-OSVR (.NET) wrapper without any Unity-specific adaptations. See that source for examples of button and analog callbacks, as well as display parameter access (ideally used to set up the display properly. In terms of API, the Managed-OSVR API is effectively a direct translation of the C++ wrappers of OSVR `ClientKit`, so please see the main OSVR-Core client documentation for more information.

There is a prototype of a more Unity-adapted button callback - it does not presently work, but your assistance in fixing it would be greatly appreciated!

### Execution
A standalone player built for Windows may end up needing the `-adapter N` argument, where `N` is a Direct3D display adapter, to put the rendered output on the HMD display.
