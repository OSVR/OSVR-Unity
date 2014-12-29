/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        public class ClientKit : MonoBehaviour
        {
            public string AppID;

            private OSVR.ClientKit.ClientContext context;

            /// <summary>
            /// Use to find the single instance of this object/script in your game.
            /// </summary>
            /// <returns>The instance, or null in case of error</returns>
            public static ClientKit Get()
            {
                ClientKit candidate = GameObject.FindObjectOfType<ClientKit>();
                if (null == candidate)
                {
                    Debug.LogError("You need the ClientKit prefab!");
                }
                return candidate;
            }

            public OSVR.ClientKit.ClientContext GetContext()
            {
                if (context == null)
                {
                    if (0 == AppID.Length)
                    {
                        Debug.LogError("OSVR ClientKit instance needs AppID set to a reverse-order DNS name! Using dummy name...");
                        AppID = "org.opengoggles.osvr-unity.dummy";
                    }
                    Debug.Log("Starting OSVR with app ID: " + AppID);
                    context = new OSVR.ClientKit.ClientContext(AppID, 0);
                }
                return context;
            }

            static ClientKit()
            {
                DLLSearchPathFixer.fix();
            }

            void Start()
            {
                GetContext();
            }

            void FixedUpdate()
            {
                context.update();
            }

            void OnDestroy()
            {
                Debug.Log("Disconnecting from OSVR server.");
                context = null;
            }
        }
    }
}