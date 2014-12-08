// OSVRUnityPlugin.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <osvr/ClientKit/ContextC.h>
#include <osvr/ClientKit/InterfaceC.h>
#include <osvr/ClientKit/InterfaceCallbackC.h>
#include <iostream>

#if _MSC_VER // this is defined when compiling with Visual Studio
#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

// ------------------------------------------------------------------------
// Plugin itself

static OSVR_PoseCallback testCallback;

static void myTrackerCallback(void * useData, OSVR_TimeValue timeData,
                       const OSVR_PoseReport report) {


	testCallback(useData, timeData, report);

    std::cout << "Got report: Position = (" << report.pose.translation.data[0]
              << ", " << report.pose.translation.data[1] << ", "
              << report.pose.translation.data[2] << "), orientation = ("
              << osvrQuatGetW(&(report.pose.rotation)) << ", ("
              << osvrQuatGetX(&(report.pose.rotation)) << ", "
              << osvrQuatGetY(&(report.pose.rotation)) << ", "
              << osvrQuatGetZ(&(report.pose.rotation)) << ")" << std::endl;
}

static OSVR_ClientContext ctx;

// Link following functions C-style (required for plugins)
extern "C"
{

	// The functions we will call from Unity.
	//
	const EXPORT_API char* PrintHello(){
		return "Hello";
	}

	int EXPORT_API PrintANumber(){
		return 5;
	}

	int EXPORT_API AddTwoIntegers(int a, int b) {
		return a + b;
	}

	float EXPORT_API AddTwoFloats(float a, float b) {
		return a + b;
	}

	void EXPORT_API InitPlugin(char * applicationIdentity)
	{
		//ctx = osvrClientInit("org.opengoggles.exampleclients.TrackerCallback");
		ctx = osvrClientInit(applicationIdentity);

		//OSVR_ClientInterface lefthand = NULL;
		//osvrClientGetInterface(ctx, "/me/hands/left", &lefthand);
		//osvrRegisterPoseCallback(lefthand, &myTrackerCallback, NULL);

	}

	

	void EXPORT_API AddInterface(char * path, OSVR_PoseCallback callback)
	{
		//OSVR_ClientContext ctx = osvrClientInit("org.opengoggles.exampleclients.TrackerCallback");

		testCallback = callback;

		OSVR_ClientInterface lefthand = NULL;

		//osvrClientGetInterface(ctx, "/me/hands/left", &lefthand);

		osvrClientGetInterface(ctx, path, &lefthand);
		osvrRegisterPoseCallback(lefthand, callback, NULL);
				

		// Pretend that this is your application's mainloop.
		//for (int i = 0; i < 1000000; ++i) {
		//	osvrClientUpdate(ctx);
		//}

		//osvrClientShutdown(ctx);
		//std::cout << "Library shut down, exiting." << std::endl;
		std::cout << "Done" << std::endl;
	}

	void EXPORT_API ClientUpdate()
	{
		osvrClientUpdate(ctx);
	}

} // end of export C block




