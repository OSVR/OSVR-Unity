/// Managed-OSVR binding
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
using OSVR.ClientKit;

namespace DisplayParameter
{
    class DisplayParameter
    {
        static void Main(string[] args)
        {
            OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.managed.DisplayParameter");
            string displayDescription = context.getStringParameter("/display");

            Console.WriteLine("Got value of /display:");
            Console.WriteLine(displayDescription);

            Console.WriteLine("Library shut down; exiting.");
        }
    }
}
