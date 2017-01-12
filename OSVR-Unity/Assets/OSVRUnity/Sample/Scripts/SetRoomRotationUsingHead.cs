/// OSVR-Unity
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2016 Sensics, Inc.
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
using System.Collections;

namespace OSVR
{
    namespace Unity
    {

        public class SetRoomRotationUsingHead : MonoBehaviour
        {
            public KeyCode setRoomRotationKey = KeyCode.R;
            public KeyCode clearRoomRotationKey = KeyCode.U;
            private ClientKit _clientKit;
            private DisplayController _displayController;

            void Awake()
            {
                _clientKit = ClientKit.instance;
                _displayController = FindObjectOfType<DisplayController>();
            }

            void Update()
            {
                if (Input.GetKeyDown(setRoomRotationKey))
                {                   
                    
                    if (_displayController != null && _displayController.UseRenderManager)
                    {
                        _displayController.RenderManager.SetRoomRotationUsingHead();
                    }
                    else
                    {
                        if(_clientKit.context.CheckStatus())
                        {
                            _clientKit.context.SetRoomRotationUsingHead();
                        }
                    }
                }
                if (Input.GetKeyDown(clearRoomRotationKey))
                {
                    if (_displayController != null && _displayController.UseRenderManager)
                    {
                        _displayController.RenderManager.ClearRoomToWorldTransform();
                    }
                    else
                    {
                        _clientKit.context.ClearRoomToWorldTransform();
                    }

                }
            }
        }
    }
}