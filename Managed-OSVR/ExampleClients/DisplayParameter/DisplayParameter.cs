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
