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
using System.IO;
using System.Text;

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
                string[] dllPaths = {
                    "Assets" + Path.DirectorySeparatorChar + "Plugins",
                    "Assets"+ Path.DirectorySeparatorChar + "Plugins"+ Path.DirectorySeparatorChar + "x86", // todo don't hardcode this
                    "StreamingAssets"+ Path.DirectorySeparatorChar + "Plugins",
                    "StreamingAssets"+ Path.DirectorySeparatorChar + "Plugins"+ Path.DirectorySeparatorChar + "x86" // todo don't hardcode this
                };
                // Amend DLL search path - see http://forum.unity3d.com/threads/dllnotfoundexception-when-depend-on-another-dll.31083/#post-1042180
                // for original inspiration for this code.
                string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
                string[] currentPaths = currentPath.Split(Path.PathSeparator);
                StringBuilder paths = new StringBuilder();
                foreach (string dllPath in dllPaths)
                {
                    String fullDllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + dllPath;
                    if (Array.IndexOf(currentPaths, fullDllPath) < 0)
                    {
                        paths.AppendFormat("{1}{0}", Path.PathSeparator, fullDllPath);
                    }
                }
                // Keeps our new paths in front.
                paths.Append(currentPath);
                Environment.SetEnvironmentVariable("PATH", paths.ToString(), EnvironmentVariableTarget.Process);
            }

        }
    }
}
