/// OSVR-Unity Connection
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
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using OSVR.ClientKit;
using System;
using Quaternion = UnityEngine.Quaternion;

public class SampleImaging : OSVR.Unity.RequiresImagingInterface
{
    private int imageWidth = 640;
    private int imageHeight = 480;

    private OSVR.Unity.ImagingInterface imagingInterface;

    private Texture2D videoTexture;

    private Color32[] imageData;
    private bool firstReport = false;

    void Start()
    {
        firstReport = false;
        imagingInterface = GetComponent<OSVR.Unity.ImagingInterface>();
        imagingInterface.Interface.StateChanged += HandleChanged;
    }

    void OnDestroy()
    {
        imagingInterface.Interface.StateChanged -= HandleChanged;
    }

    private void HandleChanged(object sender, TimeValue timestamp, int sensor, ImagingState imageReport)
    {
        if(!firstReport)
        {
            firstReport = true;
            initVideoTexture(imageReport.metadata);
        }

        for (int i = 0; i < imageWidth * imageHeight; i++)
        {                    
            byte r = Marshal.ReadByte(imageReport.data, i * 3 + 0);
            byte g = Marshal.ReadByte(imageReport.data, i * 3 + 1);
            byte b = Marshal.ReadByte(imageReport.data, i * 3 + 2);
            imageData[i] = new Color32(r, g, b, 255);
        }

        videoTexture.SetPixels32(imageData);
        videoTexture.Apply();    
    }

    private void initVideoTexture(ImagingMetadata metadata)
    {
        imageHeight = (int)metadata.height;
        imageWidth = (int)metadata.width;
        imageData = new Color32[imageWidth * imageHeight];
        videoTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.BGRA32, false);
        Material mat = GetComponent<MeshRenderer>().material;
        mat.mainTexture = videoTexture;
        //this.transform.localScale = new Vector3(1, (float)(imageHeight / imageWidth), 1) * .5f;
    }
}
