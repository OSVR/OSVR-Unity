using System;
using OSVR.ClientKit;

namespace TrackerCallback
{

    public class TrackerCallbacks
    {
        // Pose callback
        public void myTrackerCallback(IntPtr userdata, ref TimeValue timestamp, ref PoseReport report)
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
        public void myOrientationCallback(IntPtr userdata, ref TimeValue timestamp, ref OrientationReport report)
        {
            Console.WriteLine("Got ORIENTATION report: Orientation = ({0}, {1}, {2}, {3})",
                report.rotation.w,
                report.rotation.x,
                report.rotation.y,
                report.rotation.z);
        }

        // Position callback
        public void myPositionCallback(IntPtr userdata, ref TimeValue timestamp, ref PositionReport report)
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
            OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.ExampleClient");

            // This is just one of the paths. You can also use:
            // /me/hands/right
            // /me/head
            Interface lefthand = context.getInterface("/me/hands/left");

            TrackerCallbacks callbacks = new TrackerCallbacks();

            // The coordinate system is right-handed, withX to the right, Y up, and Z near.
            lefthand.registerCallback(callbacks.myTrackerCallback, IntPtr.Zero);

            // If you just want orientation
            lefthand.registerCallback(callbacks.myOrientationCallback, IntPtr.Zero);

            // or position
            lefthand.registerCallback(callbacks.myPositionCallback, IntPtr.Zero);

            // Pretend that this is your application's main loop
            for (int i = 0; i < 1000000; ++i)
            {
                context.update();
            }

            Console.WriteLine("Library shut down; exiting.");
        }
    }
}
