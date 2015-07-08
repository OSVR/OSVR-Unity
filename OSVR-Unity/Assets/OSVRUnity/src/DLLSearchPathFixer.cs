/// OSVR-Unity Connection
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        public class DLLSearchPathFixer
        {
            /// <summary>
            /// Call in a static constructor of an object depending on native code.
            ///
            /// It is required if that native DLL being accessed depends on other native DLLs alongside it.
            /// </summary>
            public static void fix()
            {
                // Amend DLL search path - see http://forum.unity3d.com/threads/dllnotfoundexception-when-depend-on-another-dll.31083/#post-1042180
                // for original inspiration for this code.

                var fixer = new DLLSearchPathFixer();
                fixer.ConditionallyAddRelativeDir("Plugins");
                fixer.ConditionallyAddRelativeDir(new List<String>() { "Plugins", IntPtr.Size == 4 ? "x86" : "x86_64" });
                fixer.ApplyChanges();
            }

            /// <summary>
            /// Constructor for private use as a helper within the static fix() method.
            /// </summary>
            private DLLSearchPathFixer()
            {
                var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
                //Debug.Log(String.Format("Old PATH: {0}", currentPath));
                OrigDirs = new List<string>(currentPath.Split(Path.PathSeparator));
                UnityDataDir = Application.dataPath;
                UnityDataDirBackslashed = Application.dataPath.Replace("/", "\\");
            }

            /// <summary>
            /// Update the process environment PATH variable to contain the full list (entries new and old) of directories.
            /// </summary>
            private void ApplyChanges()
            {
                // Combine new and old dirs
                var allDirs = new List<String>(NewDirs);
                allDirs.AddRange(OrigDirs);

                var newPathString = String.Join(Path.PathSeparator.ToString(), allDirs.ToArray());
                //Debug.Log(String.Format("New PATH: {0}", newPathString));
                Environment.SetEnvironmentVariable("PATH", newPathString, EnvironmentVariableTarget.Process);
            }

            /// <summary>
            /// If a directory specified relative to the Unity data dir is not yet in the PATH, add it.
            /// </summary>
            /// <param name="dirComponents">Components of a directory name relative to the Unity data dir.</param>
            private void ConditionallyAddRelativeDir(List<string> dirComponents)
            {
                ConditionallyAddRelativeDir(PathTools.Combine(dirComponents));
            }

            /// <summary>
            /// If a directory specified relative to the Unity data dir is not yet in the PATH, add it.
            /// </summary>
            /// <param name="dirComponents">A directory name relative to the Unity data dir.</param>
            private void ConditionallyAddRelativeDir(string relativePortion)
            {
                if (IsRelativeDirIncludedInPath(relativePortion))
                {
                    // early out.
                    return;
                }
                NewDirs.Add(PathTools.Combine(UnityDataDir, relativePortion));
            }

            /// <summary>
            /// Checks to see if a directory specified relative to the Unity data dir is included in the path so far.
            /// It checks using both forward-slashed and backslashed versions of the Unity data dir.
            /// </summary>
            /// <param name="relativePortion">Directory relative to the Unity data dir</param>
            /// <returns>true if the given directory is included in the path so far</returns>
            private bool IsRelativeDirIncludedInPath(string relativePortion)
            {
                return IsIncludedInPath(PathTools.Combine(UnityDataDir, relativePortion)) || IsIncludedInPath(PathTools.Combine(UnityDataDirBackslashed, relativePortion));
            }

            /// <summary>
            /// Checks to see if a directory is included in the path so far (both new and old directories).
            /// </summary>
            /// <param name="dir">Directory name</param>
            /// <returns>true if the given directory name is found in either the new or old directory lists.</returns>
            private bool IsIncludedInPath(string dir)
            {
                return NewDirs.Contains(dir) || OrigDirs.Contains(dir);
            }

            private string UnityDataDir;
            private string UnityDataDirBackslashed;
            private List<string> NewDirs = new List<string>();
            private List<string> OrigDirs;

            /// <summary>
            /// Utilities for combining path components with a wider variety of input data types than System.IO.Path.Combine
            /// </summary>
            private class PathTools
            {
                internal static string Combine(string a, string b)
                {
                    return Path.Combine(a, b);
                }

                internal static string Combine(string[] components)
                {
                    return String.Join(Path.DirectorySeparatorChar.ToString(), components);
                }

                internal static string Combine(List<String> components)
                {
                    return PathTools.Combine(components.ToArray());
                }
            }
        }
    }
}
