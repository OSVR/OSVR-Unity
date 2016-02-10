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
/// Author: Greg Aring, Ryan Pavlik
/// Email: greg@sensics.com
/// </summary>
using UnityEngine;

namespace OSVR.Unity
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class K1RadialDistortion : MonoBehaviour
	{
		public float k1Red = 0.0f;
		public float k1Green = 0.0f;
		public float k1Blue = 0.0f;
		public Vector2 center = new Vector2(0.5f, 0.5f);
		public Material DistortionMaterial;

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			DistortionMaterial.SetFloat("_K1_Red", k1Red);
			DistortionMaterial.SetFloat("_K1_Green", k1Green);
			DistortionMaterial.SetFloat("_K1_Blue", k1Blue);
			DistortionMaterial.SetVector("_Center", center);
			Graphics.Blit(source, destination, DistortionMaterial);
		}
	}

	public class K1RadialDistortionFactory
	{
		private const string ShaderName = "Osvr/OsvrDistortion";

		public bool Supported
		{
			get;
			private set;
		}

		private Shader DistortionShader;

		public K1RadialDistortionFactory()
		{
			Supported = DoSetup();
			if (!Supported)
			{
                Debug.Log("[OSVR-Unity] Execution will proceed, but without shader-based distortion.");
			}
		}

		/// <summary>
		/// Creates a K1RadialDistortion effect and adds it as a component to the given eye, if possible.
		/// </summary>
		/// <param name="Surface">VRSurface to apply the effect to.</param>
		/// <returns>K1RadialDistortion object for parameter setting, or null if not supported</returns>
		public K1RadialDistortion GetOrCreateDistortion(OSVR.Unity.VRSurface surface)
		{
			K1RadialDistortion ret = surface.DistortionEffect;
			if (!Supported)
			{
				if (ret)
				{
					// shouldn't be able to get here but...
					ret.enabled = false;
					ret = null;
				}
				return ret;
			}
			if (ret == null)
			{
				ret = surface.gameObject.AddComponent<K1RadialDistortion>();
				surface.DistortionEffect = ret;
				ret.hideFlags = HideFlags.HideAndDontSave;
				ret.DistortionMaterial = new Material(DistortionShader);
				if (!ret.DistortionMaterial)
				{
					/// weird error case, shouldn't get here.
                    Debug.LogWarning("[OSVR-Unity] Couldn't create material in OSVR distortion shader factory - shouldn't be able to happen!");
					ret.enabled = false;
					return null;
				}
				ret.DistortionMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			else
			{
				ret.enabled = (ret.DistortionMaterial != null);
			}
			return ret;
		}

		private bool DoSetup()
		{
			if (!IsMinimallyCompatible)
			{
                Debug.Log("[OSVR-Unity] distortion shader not compatible with this version of Unity: requires image effects and render textures (4.6 Pro or 5.x)");
				return false;
			}
			DistortionShader = Shader.Find(ShaderName);
			if (!DistortionShader)
			{
                Debug.Log("[OSVR-Unity] Could not find OSVR distortion shader '" + ShaderName + "' - must be in a Resource folder to be part of a build!");
				return false;
			}
			if (!DistortionShader.isSupported)
			{
                Debug.Log("[OSVR-Unity] distortion shader found and loaded but not supported on this platform.");
				DistortionShader = null;
				return false;
			}
			return true;
		}

		private static bool IsMinimallyCompatible
		{
			get
			{
				return SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures;
			}
		}
	}
}
