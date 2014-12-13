using UnityEngine;

namespace OSVR
{
	namespace Unity
	{
		public class OSVRClientKit : MonoBehaviour
		{
			public string AppID;

			private OSVR.ClientKit.ClientContext context;

			public OSVR.ClientKit.ClientContext GetContext()
			{
				if (context == null) {
					Debug.Log ("Starting OSVR with app ID: " + AppID);
					context = new OSVR.ClientKit.ClientContext (AppID, 0);
				}
				return context;
			}

			static OSVRClientKit ()
			{
				DLLSearchPathFixer.fix ();
			}

			void Start()
			{
				GetContext();
			}

			void FixedUpdate ()
			{
				context.update ();
			}
		}
	}
}