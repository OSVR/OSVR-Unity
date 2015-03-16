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
/// <summary>
/// Based on Unity's (Pro Only) Image Effect: Fisheye.js
/// Author: Greg Aring
/// Email: greg@sensics.com
/// </summary>
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OsvrDistortion : OsvrPostEffectsBase {
    public float k1Red = 0.5f;
    public float k1Green = 0.5f;
    public float k1Blue = 0.5f;
    public Vector2 fullCenter = new Vector2(0.5f, 0.5f);
    public Vector2 leftCenter = new Vector2(0.5f, 0.5f);
    public Vector2 rightCenter = new Vector2(0.5f, 0.5f);
    public Shader distortionShader = null;

    private Material distortionMaterial = null;
	
	protected override bool CheckResources () {

        CheckSupport (false);
        distortionMaterial = CheckShaderAndCreateMaterial(distortionShader,distortionMaterial);
		
        if(!isSupported)
            ReportAutoDisable ();
        return isSupported;			
    }

	
    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        if(CheckResources()==false) {
            Graphics.Blit (source, destination);
            return;
        }

        distortionMaterial.SetFloat("_K1_Red", k1Red);
        distortionMaterial.SetFloat ("_K1_Green", k1Green);
        distortionMaterial.SetFloat ("_K1_Blue", k1Blue);
        distortionMaterial.SetVector("_Full_Center", fullCenter);
        distortionMaterial.SetVector("_Left_Center", leftCenter);
        distortionMaterial.SetVector("_Right_Center", rightCenter);
        Graphics.Blit (source, destination, distortionMaterial); 	
    }
}
