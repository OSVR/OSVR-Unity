using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;



public class PluginImport : MonoBehaviour 
{

	struct OSVR_TimeValue {
		/** @brief Seconds portion of the time value. */
		Int64 seconds;
		/** @brief Microseconds portion of the time value. */
		Int32 microseconds;
	};
	
	
	[StructLayout(LayoutKind.Sequential)]
	struct OSVR_Quaternion {
		double data; //4
	};

	[StructLayout(LayoutKind.Sequential)]
	struct OSVR_Vec3 {
		double data; //3
	};

	[StructLayout(LayoutKind.Sequential)]
	struct OSVR_Pose3 {
		OSVR_Vec3 translation;
		OSVR_Quaternion rotation;
	};
	
	[StructLayout(LayoutKind.Sequential)]
	struct OSVR_PoseReport {
		Int32 sensor;
		OSVR_Pose3 pos;
	};
	
	
	
	//Lets make our calls from the Plugin
	[DllImport ("OSVRUnityPlugin")]
	private static extern int PrintANumber();
	
	[DllImport ("OSVRUnityPlugin")]
	private static extern IntPtr PrintHello();
	
	[DllImport ("OSVRUnityPlugin")]
	private static extern int AddTwoIntegers(int i1,int i2);

	[DllImport ("OSVRUnityPlugin")]
	private static extern float AddTwoFloats(float f1,float f2);

	[DllImport ("OSVRUnityPlugin")]
	private static extern void ClientUpdate();	

	[DllImport ("OSVRUnityPlugin")]
	private static extern void InitPlugin(string applicationIdentity);	

	[DllImport ("OSVRUnityPlugin")]
	private static extern void AddInterface(string path, OSVR_PoseCallback callback);	


	delegate void OSVR_PoseCallback(IntPtr userData, OSVR_TimeValue timeStamp, OSVR_PoseReport report);
	
	IEnumerator Start () {
		Debug.Log(PrintANumber());
		Debug.Log(Marshal.PtrToStringAuto (PrintHello()));
		Debug.Log(AddTwoIntegers(2,2));
		Debug.Log(AddTwoFloats(2.5F,4F));
		InitPlugin("org.opengoggles.exampleclients.TrackerCallback");
		AddInterface ("/me/hands/left", new OSVR_PoseCallback(this.Test));
		yield return new WaitForSeconds (1);
		ClientUpdate();
	}

	void Update()
	{
		//ClientUpdate();
	}

	void Test(IntPtr userData, OSVR_TimeValue timeStamp, OSVR_PoseReport report)
	{
		Debug.Log ("Got Callback");
	}

}
