/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

using UnityEngine;

namespace OSVR
{
    namespace Unity
    {
        public class ClientKit : MonoBehaviour
        {
            [Tooltip("A string uniquely identifying your application, in reverse domain-name format.")]
            public string AppID;

            private OSVR.ClientKit.ClientContext _contextObject;
#if UNITY_STANDALONE_WIN
            private OSVR.ClientKit.ServerAutoStarter _serverAutoStarter;

            public bool autoStartServer = true;
#endif

            /// Uses the Unity "Persistent Singleton" pattern, see http://unitypatterns.com/singletons/
            private static ClientKit _instance;
            private bool _osvrServerError = false;
			private bool _dllFixed = false;

            /// <summary>
            /// Use to access the single instance of this object/script in your game.
            /// </summary>
            /// <returns>The instance, or null in case of error</returns>
            public static ClientKit instance
            {
                get
                {
                    if(_instance == null)
                    {
                        _instance = GameObject.FindObjectOfType<ClientKit>();
                        if (_instance == null)
                        {
                            Debug.LogError("[OSVR-Unity] Error: You need the ClientKit prefab in your game!!");
                        }
                        else
                        {
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                    return _instance;
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
                    return _contextObject;
                }
            }

            private void EnsureStarted()
            {
				if (!_dllFixed)
                {
                    DLLSearchPathFixer.fix();
                    _dllFixed = true;
                }
				
                if (_contextObject == null)
                {
                    if (0 == AppID.Length)
                    {
                        Debug.LogError("[OSVR-Unity] ClientKit instance needs AppID set to a reverse-order DNS name! Using dummy name...");
                        AppID = "com.osvr.osvr-unity.dummy";
                    }
                    Debug.Log("[OSVR-Unity] Starting with app ID: " + AppID);
                    _contextObject = new OSVR.ClientKit.ClientContext(AppID, 0);                  
                }

#if UNITY_STANDALONE_WIN
                if(_serverAutoStarter == null && autoStartServer)
                {
                    _serverAutoStarter = new OSVR.ClientKit.ServerAutoStarter();
                }
#endif

                //check if the server is running
                if (!_contextObject.CheckStatus())
                {
                    if(!_osvrServerError)
                    {
                        _osvrServerError = true;
                        Debug.LogError("[OSVR-Unity] OSVR Server not detected. Start OSVR Server and restart the application.");
                    }                                    
                }
                else if(_osvrServerError)
                {
                    Debug.Log("[OSVR-Unity] OSVR Server connection established. You can ignore previous errors about the server not being detected.");
                    _osvrServerError = false;
                }
            }

            void Awake()
            {
                //if an instance of this singleton does not exist, set the instance to this object and make it persist
                if(_instance == null)
                {
                    _instance = this;
					DontDestroyOnLoad(this);
                }
                else
                {
                    //if an instance of this singleton already exists, destroy this one
                    if(_instance != this)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
			
            void Start()
            {
                Debug.Log("[OSVR-Unity] In Start()");
                EnsureStarted();
            }

            void OnEnable()
            {
                Debug.Log("[OSVR-Unity] In OnEnable()");
                EnsureStarted();
            }
            
            void Update()
            {
                EnsureStarted();
                _contextObject.update();
            }

            void LateUpdate()
            {
                _contextObject.update();
            }
			
            void Stop()
        {
                // Only stop the main instance, since it is the only one that
                // ever actually starts-up.
                if (this == instance)
                {
                    if (null != _contextObject)
                    {
                        Debug.Log("[OSVR-Unity] Shutting down OSVR.");
                        _contextObject.Dispose();
                        _contextObject = null;
#if UNITY_STANDALONE_WIN
                        if(_serverAutoStarter != null)
                        {
                            _serverAutoStarter.Dispose();
                            _serverAutoStarter = null;
                        }
#endif
                    }
                }
            }

            void OnDisable()
            {
                Stop();
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
