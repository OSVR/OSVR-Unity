/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using UnityEngine;
using System.Collections;

namespace OSVR
{
    namespace Unity
    {
        
        /// <summary>
        /// A script component to add to a GameObject in order to access an interface, managing lifetime and centralizing the path specification.
        /// </summary>
        public class InterfaceGameObject : MonoBehaviour
        {
            /// <summary>
            /// The interface path you want to connect to.
            /// </summary>
            [Tooltip("The interface path you want to access. If left blank, the path of the nearest ancestor with a path will be used.")]
            public string path;


            public InterfaceCallbacks osvrInterface
            {
                get
                {
                    Start();
                    return iface;
                }
            }

            protected InterfaceGameObject interfaceGameObject
            {
                get
                {
                    return GetComponent<InterfaceGameObject>();
                }
            }

            #region Private implementation
            private InterfaceCallbacks iface;

            private class PathHolder : MonoBehaviour
            {
                [HideInInspector]
                public string path;

                PathHolder()
                {
                    hideFlags = HideFlags.HideAndDontSave;
                }
            }
            #endregion

            #region Methods for Derived Classes
            /// <summary>
            /// Call from your Awake method to advertise the presence or absence of a path specification on this game object.
            /// </summary>
            protected void AdvertisePath()
            {
                if (null != iface)
                {
                    /// Already started.
                    return;
                }
                
                PathHolder holder = GetComponent<PathHolder>();
                if (path.Length > 0) {
                    /// If we have a path, be sure we advertise it.
                    if (null == holder) {
                        holder = gameObject.AddComponent<PathHolder>();
                    }
                    holder.path = path;
                } else {
                    /// Don't advertise a path that is empty
                    if (null != holder) {
                        Object.Destroy(holder);
                    }
                }
            }

            /// <summary>
            /// Call from your Start method
            /// </summary>
            protected void Start()
            {
                if (null != iface)
                {
                    return;
                }
                AdvertisePath();
                GameObject go = this.gameObject;
                PathHolder holder = null;
                string usedPath = path;
                while (null != go && 0 == usedPath.Length)
                {
                    holder = go.GetComponent<PathHolder>();
                    if (null != holder)
                    {
                        usedPath = holder.path;
                        //print("[OSVR] " + name + ": Found path " + usedPath + " in ancestor " + go.name);
                    }
                    go = GetParent.Get(go);
                }

                if (0 == usedPath.Length)
                {
                    Debug.LogError("[OSVR] Missing path for " + name + " - no path found in this object's InterfaceGameObject or any ancestor!");
                    return;
                }

                iface = ScriptableObject.CreateInstance<InterfaceCallbacks>();
                iface.path = usedPath;
                iface.Start();
            }

            protected void Stop()
            {
                if (null != iface)
                {
                    Object.Destroy(iface);
                    iface = null;
                }
                PathHolder holder = GetComponent<PathHolder>();
                if (null != holder)
                {
                    Object.Destroy(holder);
                }
            }
            #endregion

            #region Event Methods
            void Awake()
            {
                AdvertisePath();
            }

            void OnDestroy()
            {
                Stop();
            }

            void OnApplicationQuit()
            {
                Stop();
            }

            #endregion

        }
    }
}