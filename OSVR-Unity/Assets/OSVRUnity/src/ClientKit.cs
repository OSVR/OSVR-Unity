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
            [Tooltip("A string uniquely identifying your application, in reverse domain-name format.")]
            public string AppID;

            private OSVR.ClientKit.ClientContext contextObject;

            /// <summary>
            /// Use to access the single instance of this object/script in your game.
            /// </summary>
            /// <returns>The instance, or null in case of error</returns>
            public static ClientKit instance
            {
                get
                {
                    ClientKit candidate = GameObject.FindObjectOfType<ClientKit>();
                    if (null == candidate)
                    {
                        Debug.LogError("OSVR Error: You need the ClientKit prefab in your game!!");
                    }
                    return candidate;

                }
            }

            /// <summary>
            /// Access the underlying Managed-OSVR client context object.
            /// </summary>
            public OSVR.ClientKit.ClientContext context
            {
                get
                {
                    EnsureStarted();
                    return contextObject;
                }
            }

            private void EnsureStarted()
            {
                if (contextObject == null)
                {
                    if (0 == AppID.Length)
                    {
                        Debug.LogError("OSVR ClientKit instance needs AppID set to a reverse-order DNS name! Using dummy name...");
                        AppID = "org.opengoggles.osvr-unity.dummy";
                    }
                    Debug.Log("[OSVR] Starting with app ID: " + AppID);
                    contextObject = new OSVR.ClientKit.ClientContext(AppID, 0);
                }
            }

            /// <summary>
            /// Static constructor that enhances the DLL search path to ensure dependent native dlls are found.
            /// </summary>
            static ClientKit()
            {
                DLLSearchPathFixer.fix();
            }

            void Start()
            {
                Debug.Log("[OSVR] In Start()");
                EnsureStarted();
            }

            void OnEnable()
            {
                Debug.Log("[OSVR] In OnEnable()");
                EnsureStarted();
            }

            void FixedUpdate()
            {
                EnsureStarted();
                contextObject.update();
            }

            void Stop()
            {
                if (null != contextObject)
                {
                    Debug.Log("Shutting down OSVR.");
                    contextObject.Dispose();
                    contextObject = null;
                }
            }

            void OnDestroy()
            {
                Stop();
            }

            void OnApplicationQuit()
            {
                Stop();
            }
        }
    }
}