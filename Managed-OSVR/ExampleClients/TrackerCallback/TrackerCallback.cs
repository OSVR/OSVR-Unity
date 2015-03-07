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

﻿using System;
using OSVR.ClientKit;

namespace TrackerCallback
{

    public class TrackerCallbacks
    {
        // Pose callback
        public static void myTrackerCallback(IntPtr userdata, ref TimeValue timestamp, ref PoseReport report)
        {
            Console.WriteLine("Got POSE report: Position = ({0}, {1}, {2}), orientation ({3}, {4}, {5}, {6})",
                report.pose.translation.x,
                report.pose.translation.y,
                report.pose.translation.z,
                report.pose.rotation.w,
                report.pose.rotation.x,
                report.pose.rotation.y,
                report.pose.rotation.z);
        }

        // Orientation callback
        public static void myOrientationCallback(IntPtr userdata, ref TimeValue timestamp, ref OrientationReport report)
        {
            Console.WriteLine("Got ORIENTATION report: Orientation = ({0}, {1}, {2}, {3})",
                report.rotation.w,
                report.rotation.x,
                report.rotation.y,
                report.rotation.z);
        }

        // Position callback
        public static void myPositionCallback(IntPtr userdata, ref TimeValue timestamp, ref PositionReport report)
        {
            Console.WriteLine("Got POSITION report: Position = ({0}, {1}, {2})",
                report.xyz.x,
                report.xyz.y,
                report.xyz.z);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.managed.TrackerCallback");

            // This is just one of the paths. You can also use:
            // /me/hands/right
            // /me/head
            Interface lefthand = context.getInterface("/me/hands/left");

            TrackerCallbacks callbacks = new TrackerCallbacks();
            // The coordinate system is right-handed, withX to the right, Y up, and Z near.
            OSVR.ClientKit.PoseCallback poseCb = new PoseCallback(TrackerCallbacks.myTrackerCallback);
            lefthand.registerCallback(poseCb, IntPtr.Zero);

            // If you just want orientation
            OSVR.ClientKit.OrientationCallback oriCb = new OrientationCallback(TrackerCallbacks.myOrientationCallback);
            lefthand.registerCallback(oriCb, IntPtr.Zero);

            // or position
            OSVR.ClientKit.PositionCallback posCb = new PositionCallback(TrackerCallbacks.myPositionCallback);
            lefthand.registerCallback(posCb, IntPtr.Zero);

            // Pretend that this is your application's main loop
            for (int i = 0; i < 1000000; ++i)
            {
                context.update();
            }

            Console.WriteLine("Library shut down; exiting.");
        }
    }
}
