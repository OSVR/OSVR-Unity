# OSVR-Unity README

> _This is an Alpha Release, and probably not entirely idiomatic for Unity. Check back for updates often._

## Basic Principles and Files
The `OSVR-Unity.unitypackage` package should contain the x86 binary plugins, the compiled Managed-OSVR wrapper, the OSVRUnity scripts (in the `Assets` directory), and a prefab called just OSVR. Import this package into your project, and drag in the prefab for sample versions of the currently-handled scripted game objects.

There is a second Unity package file that should contain roughly a complete demo application, though minimal.

### ClientKit object
You need exactly one instance of `OSVR.Unity.ClientKit` in your project: one is included in the OSVR prefab. You need to set the app ID: use a reversed DNS name as seen elsewhere (Java, Android, etc). This just uniquely identifies your application to the OSVR software. If you fail to do this, you'll see an error in the Unity console.

### Tracking
For trackers (Pose, Position, Orientation), there are samples in the prefab of nodes that update their transform accordingly. They need a reference to the `ClientKit` - which should be set automatically if you start from the prefab. You'll also need to set the path you want to use. Please see the C/C++ documentation for client apps to find valid interface paths. (Note that the OSVR-Unity package handles normalization of the coordinate system to the Unity standard: ignore the one seen in the C++ documentation.)

### Other interaction
Any other interaction with the OSVR framework goes directly through the Managed-OSVR (.NET) wrapper without any Unity-specific adaptations. See that source for examples of button and analog callbacks, as well as display parameter access (ideally used to set up the display properly. In terms of API, the Managed-OSVR API is effectively a direct translation of the C++ wrappers of OSVR `ClientKit`, so please see the main OSVR-Core client documentation for more information.

There is a prototype of a more Unity-adapted button callback in the prefab - it does not presently work, but your assistance in fixing it would be greatly appreciated!
