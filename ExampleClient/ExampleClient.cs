using System;
using OSVR.ClientKit;

namespace ExampleClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.ExampleClient");

			// This is just one of the paths. You can also use:
			// /me/hands/right
			// /me/head
			Interface lefthand = context.getInterface("/me/hands/left");

			// Tracker callback
			void myTrackerCallback(void*, const OSVR_TimeValue*, const OSVR_PoseReport* report)
			{
				Console.WriteLine("Got POSE report: Position = ({0}, {1}, {2}), orientation ({3}, {4}, {5}, {6})", report.pose.translation.DataMisalignedException[0], report.pose.translation.DataMisalignedException[1], report.pose.translation.DataMisalignedException[2], );
			}

			// The coordinate system is right-handed, withX to the right, Y up, and Z near.
			lefthand.registerCallback (myTrackerCallback, null);
		}
	}
}
