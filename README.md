# OSVR-Unity

## .NET Binding for OSVR - "Managed-OSVR"
Contained in `Managed-OSVR/ClientKit`. The `Managed-OSVR` directory contains a solution with both the ClientKit wrapper and ported examples based on the C++ examples from the core.

## OSVR-Unity Wrapper
In the `OSVR-Unity` directory. Note that if you're looking at the source, you'll need to build the Managed-OSVR/ClientKit project to get the plugins installed into this directory.

It contains its own README with some basic documentation.

## Bundled binary snapshot
Windows x86 native DLLs are bundled in the `Managed-OSVR/ClientKit` directory. The are presently from the snapshot identified as:

> `OSVR-Core-Snapshot-v0.1-387-g62748f2-build51-vs12-32bit`

If you have binary preview access, note that the OSVR-Unity snapshots there are re-built with every new core snapshot, rather than the version of the binaries mentioned here.

## Development Information

### Managed-OSVR
This project is entirely Unity-independent, and aside from a post-build action that copies the assembly and the dependent DLL files over into the OSVR-Unity tree, that subdirectory could basically stand alone.

### OSVR-Unity
This contains the source project used to generate `.unitypackage` files, since that seems more useful as "source" than a repo containing a `.unitypackage` file. Any one of the scenes should work to get you in there. There is an editor script to automate the bundling of a `.unitypackage` for the sake of CI.

### Other files

- `third-party/discount-2.1.6-win32` - This contains binaries of a liberally-licensed Markdown-compatible file formatter, used by CI to generate HTML documentation from the markdown README in OSVR-Unity.
- `prep-package.cmd` - Used by CI to create a directory that will eventually be packed as a snapshot, copy the source into it, and format the Unity readme markdown file as HTML. Not used in the course of normal development.
- `build-unity-packages.cmd` - Used by CI (but potentially others as well) to invoke the Unity editor script that packs the Unity plugin into a `.unitypackage` file. Requires that the Managed-OSVR build already be run to copy over the .NET assembly and native DLLs into the right place in the tree. It copies that `.unitypackage` file into the distribution directory made by `prep-package.cmd`
- `unity-generate.lua` - Generates some very repetitive code wrapping the raw C-style pinvoke callbacks in something more .NET-idiomatic. It can be run with any reasonably recent version of a Lua interpreter, and its output should be placed into `OSVR-Unity/Assets/OSVRUnity/src/InterfaceCallbacks.cs` in the designated area (see comments in that file)
