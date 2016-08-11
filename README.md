# OSVR-Unity

[![Join the chat at https://gitter.im/OSVR/OSVR-Unity](https://badges.gitter.im/OSVR/OSVR-Unity.svg)](https://gitter.im/OSVR/OSVR-Unity?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
> Maintained at <https://github.com/OSVR/OSVR-Unity>
>
> For details, see <http://osvr.github.io>
>
> For support, see <http://support.osvr.com>

## .NET Binding for OSVR - "Managed-OSVR"
The Unity integration is based on the [Managed-OSVR][] .NET binding for OSVR, which is maintained in a separate repository. That code is entirely Unity-independent so it can be used in other applications/frameworks.

[Managed-OSVR]: https://github.com/OSVR/Managed-OSVR

## OSVR Unity Integration
[Step-by-step getting started guide](https://github.com/OSVR/OSVR-Unity/blob/master/GettingStarted.md)

[Getting Started Video Tutorial](https://youtu.be/xSOq3bOBPxs)

The development "project" for Unity is in the `OSVR-Unity` directory. We are currently maintaining support for 4.6.1 as well as 5.x in the same tree, so be aware of that if you're using a newer version that you don't break the older version. (You can parallel-install different versions of Unity, you just need to specify a different install directory). This is the project used to generate the `.unitypackage` file (there is an editor script that does it for the CI's sake).

The `OSVR-Unity` directory contains its own [README](OSVR-Unity/README.md) with some basic documentation that is shipped with the built version, as well as a [CHANGES](OSVR-Unity/CHANGES.md) file (also shipped with builds) that should be updated when changes that affect consumers of this project (users of the plugin) are made.

Note that if you're looking at the source, you'll need to download and import the Managed-OSVR project artifacts, see below.

## Development Information
When the `build-for-unity.cmd` script in [Managed-OSVR][] is run (by the CI or a human), it generates a tree with `Managed-OSVR-Unity` as the root directory.

`install-managed-osvr.cmd` can put those files in the right spot if you place the `Managed-OSVR-Unity` in the root of this repository first. Otherwise, the contents of that (which will be a `.dll`, and some additional subdirectories and files) should be moved to the `OSVR-Unity/Assets/Plugins` directory.

### OSVR-Unity
This contains the source project used to generate `.unitypackage` files, since that seems more useful as "source" than a repo containing a `.unitypackage` file. Any one of the scenes should work to get you in there. There is an editor script to automate the bundling of a `.unitypackage` for the sake of CI.

### Other files

- `install-managed-osvr.cmd` - A script to copy the output of a Managed-OSVR build to the right spot in the current repo, given that you place the `Managed-OSVR-Unity` directory (produced by the `build-for-unity.cmd` script over in Managed-OSVR) in the root of this repository first.
- `prep-package.cmd` - Used by CI to create a directory that will eventually be packed as a snapshot, format the Unity readme and other markdown files as HTML, and copy files into the snapshot directory as well as the `OSVR-Unity` directory. Not used in the course of normal development.
- `build-unity-packages.cmd` - Used by CI (but potentially others as well) to invoke the Unity editor script that packs the Unity plugin into a `.unitypackage` file. Requires that the Managed-OSVR build already have been run to copy over the .NET assembly and native DLLs into the right place in the tree. It copies that `.unitypackage` file into the distribution directory made by `prep-package.cmd` if that directory exists.
- `unity-generate.lua` - Generates some very repetitive code wrapping the raw C-style pinvoke callbacks in something more .NET-idiomatic. It can be run with any reasonably recent version of a Lua interpreter, and its output should be placed into `OSVR-Unity/Assets/OSVRUnity/src/InterfaceCallbacks.cs` in the designated area (see comments in that file)
- `third-party/discount-2.1.6-win32` - This contains binaries of a liberally-licensed Markdown-compatible file formatter, used by CI (specifically `prep-package.cmd`) to generate HTML documentation from the markdown files in OSVR-Unity.

## Demonstration video

Video showing how to integrate OSVR into Unity project can be seen here: <https://youtu.be/TtLn6XpEisw>

## License

This project: Licensed under the Apache License, Version 2.0.
