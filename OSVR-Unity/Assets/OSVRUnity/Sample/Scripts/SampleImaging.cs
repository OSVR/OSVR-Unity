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

    private byte[] imageBytes;
    private Color32[] imageData;
    private int[] colors;
    private bool videoChanged;
    private bool firstReport = false;

    void Start()
    {
        firstReport = false;
        imagingInterface = GetComponent<OSVR.Unity.ImagingInterface>();
        imagingInterface.Interface.StateChanged += HandleChanged;

    }

    public void Update()
    {
        if (videoChanged)
        {
            videoTexture.SetPixels32(imageData);           
            videoTexture.Apply();
            videoChanged = false;
        }     
    }

    private void HandleChanged(object sender, TimeValue timestamp, int sensor, ImagingState imageReport)
    {
        if(!firstReport)
        {
            firstReport = true;
            initVideoTexture(imageReport.metadata);
            return;
        }
        if (videoChanged)
        {
            return;
        }

        for (int i = 0; i < imageWidth * imageHeight; i++)
        {
            /*colors[i] = ((255 & 0xFF) << 24) | // alpha component
                   Marshal.ReadByte(imageReport.data, i * 3 + 0) & 0xFF << 16 | // blue component
                   Marshal.ReadByte(imageReport.data, i * 3 + 1) & 0xFF << 8 | // green component
                   Marshal.ReadByte(imageReport.data, i * 3 + 2) & 0xFF << 0; // red component
            */
            byte b = Marshal.ReadByte(imageReport.data, i * 3 + 2);
            byte g = Marshal.ReadByte(imageReport.data, i * 3 + 1);
            byte r = Marshal.ReadByte(imageReport.data, i * 3 + 0);
            imageData[i] = new Color32(r, g, b, 255);
        }
       /* for (int x = 0; x < imageWidth; x++)
        {
            for (int y = 0; y < imageHeight; y++)
            {
                //int byteI = x + y * imageWidth;
                //int colorI = (imageWidth - x - 1) + y * imageWidth;
                byte b = Marshal.ReadByte(imageReport.data, colorI);
                byte g = Marshal.ReadByte(imageReport.data, colorI + 1);
                byte r = Marshal.ReadByte(imageReport.data, colorI + 2);

                imageData[colorI] = new Color32(r, g, b, 255);
            }
        }*/


        
        videoChanged = true;
    }

    private void initVideoTexture(ImagingMetadata metadata)
    {
        imageHeight = (int)metadata.height;
        imageWidth = (int)metadata.width;
        imageBytes = new byte[imageWidth * imageHeight];
        imageData = new Color32[imageWidth * imageHeight];
        videoTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.BGRA32, false);
        colors = new int[imageWidth * imageHeight];
        Material mat = GetComponent<MeshRenderer>().material;
        mat.mainTexture = videoTexture;
        //this.transform.localScale = new Vector3(1, (float)(imageHeight / imageWidth), 1) * .5f;
    }

    public static Texture2D OpenCVImageToUnityTexture(byte[, ,] data, int width, int height, GameObject check)
    {

        Color32[] imgData = new Color32[width * height];

        int index = 0;
        byte alpha = 255;

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                imgData[index] = new Color32((data[x, y, 2]),
                                               (data[x, y, 1]),
                                               (data[x, y, 0]),
                                               alpha);
                check.SetActive(true);
                index++;
            }
        }

        Texture2D toReturn = new Texture2D(width, height, TextureFormat.RGBA32, false);
        toReturn.SetPixels32(imgData);
        toReturn.Apply();
        toReturn.wrapMode = TextureWrapMode.Clamp;

        return toReturn;
    }
}
