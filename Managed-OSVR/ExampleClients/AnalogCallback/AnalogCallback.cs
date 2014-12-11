using System;
using OSVR.ClientKit;

namespace AnalogCallback
{
    class AnalogCallback
    {
        static void myAnalogCallback(IntPtr userdata, ref TimeValue timestamp, ref AnalogReport report)
        {
            Console.WriteLine("Got report: channel is {0}", report.state);
        }
        static void Main(string[] args)
        {
            OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("org.opengoggles.exampleclients.managed.AnalogCallback");

            // This is just one of the paths: specifically, the Hydra's left
            // controller's analog trigger. More are in the docs and/or listed on
            // startup
            Interface analogTrigger = context.getInterface("/controller/left/trigger");

            OSVR.ClientKit.AnalogCallback mycb = new OSVR.ClientKit.AnalogCallback(myAnalogCallback);
            analogTrigger.registerCallback(mycb, IntPtr.Zero);

            // Pretend that this is your application's main loop
            for (int i = 0; i < 1000000; ++i)
            {
                context.update();
            }

            Console.WriteLine("Library shut down; exiting.");
        }
    }
}
