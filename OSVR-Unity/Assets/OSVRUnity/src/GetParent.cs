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
        public class GetParent
        {
            public static GameObject Get(GameObject go)
            {
                if (null == go || null == go.transform || null == go.transform.parent)
                {
                    return null;
                }
                Transform parent = go.transform.parent;
                return parent.gameObject;
            }
        }
    }
}
