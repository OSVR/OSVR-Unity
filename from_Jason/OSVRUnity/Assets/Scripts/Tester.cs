using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Tester : MonoBehaviour 
{

	[DllImport ("OSVRUnityPlugin")]
	private static extern void InitTest();

	[DllImport ("OSVRUnityPlugin")]
	private static extern int AddTwoIntegers(int a, int b);

	// Use this for initialization
	IEnumerator Start () 
	{
		yield return new WaitForSeconds(2);
		Debug.Log (AddTwoIntegers (2, 3));
		InitTest();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
