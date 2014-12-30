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
        /// <summary>
        /// Base class for a script that requires an OSVR interface.
        /// </summary>
        [RequireComponent(typeof(InterfaceGameObject))]
        public class InterfaceBase : MonoBehaviour
        {
            /// <summary>
            /// Accessor for the sibling InterfaceGameObject component.
            /// </summary>
            public InterfaceGameObject interfaceGameObject
            {
                get
                {
                    return GetComponent<InterfaceGameObject>();
                }
            }

            /// <summary>
            /// Accessor for the InterfaceCallbacks object in the sibling InterfaceGameObject component.
            /// </summary>
            public InterfaceCallbacks osvrInterface
            {
                get
                {
					if (null == interfaceGameObject) {
						return null;
					}
                    return interfaceGameObject.osvrInterface;
                }
            }
        }
    }
}