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

namespace ButtonCallback
{
    class ButtonCallback
    {
        static void myButtonCallback(IntPtr userdata, ref TimeValue timestamp, ref ButtonReport report)
        {
            Console.WriteLine("Got report: button is {0}", report.state == 1 ? "pressed" : "released");
        }
        static void Main(string[] args)
        {
            OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.managed.ButtonCallback");

            // This is just one of the paths: specifically, the Hydra's left
            // controller's button labelled "1". More are in the docs and/or listed on
            // startup
            Interface button1 = context.getInterface("/controller/left/1");

            OSVR.ClientKit.ButtonCallback mycb = new OSVR.ClientKit.ButtonCallback(myButtonCallback);
            button1.registerCallback(mycb, IntPtr.Zero);

            // Pretend that this is your application's main loop
            for (int i = 0; i < 1000000; ++i)
            {
                context.update();
            }

            Console.WriteLine("Library shut down; exiting.");
        }
    }
}
