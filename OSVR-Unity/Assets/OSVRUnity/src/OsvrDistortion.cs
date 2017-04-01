using UnityEngine;

namespace OSVR.Unity
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public sealed class OsvrDistortion : MonoBehaviour
    {
        private bool isSupported = true;
		private Material distortionMaterial = null;
		private Shader distortionShader = null;

        public float K1Red = 0.0f;
        public float K1Green = 0.0f;
        public float K1Blue = 0.0f;
        public Vector4 Center = new Vector4(0.5f, 0.5f, 0.0f, 0.0f);

        private void Start()
        {
            CheckResources();
        }
		
        private void OnEnable()
        {
            isSupported = true;
        }
		
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            distortionMaterial.SetFloat("_K1_Red", K1Red);
            distortionMaterial.SetFloat("_K1_Green", K1Green);
            distortionMaterial.SetFloat("_K1_Blue", K1Blue);
            distortionMaterial.SetVector("_Center", Center);
            Graphics.Blit(source, destination, distortionMaterial);
        }
		
		private Material CheckShaderAndCreateMaterial(Shader shader, Material materialToCreate)
        {
            shader = distortionShader = Shader.Find("Osvr/OsvrDistortion");

            if (!shader)
            {
                Debug.Log("Missing shader in " + ToString());
                enabled = false;
                return null;
            }

            if (shader.isSupported && materialToCreate && materialToCreate.shader == shader)
                return materialToCreate;

            if (!shader.isSupported)
            {
                NotSupported();
                Debug.Log("The shader " + shader.ToString() + " on effect " + ToString() + " is not supported on this platform!");
                return null;
            }
            else
            {
                materialToCreate = new Material(shader);
                materialToCreate.hideFlags = HideFlags.DontSave;
				
                if (materialToCreate)
                    return materialToCreate;
                else 
					return null;
            }
        }

        private bool CheckResources()
        {
            CheckSupport();

            distortionMaterial = CheckShaderAndCreateMaterial(distortionShader, distortionMaterial);

            if (!isSupported)
                Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");

            return isSupported;
        }

        private bool CheckSupport()
        {
            isSupported = true;

            if (!SystemInfo.supportsImageEffects)
            {
                NotSupported();
                return false;
            }

            return true;
        }
		
        private void NotSupported()
        {
            enabled = false;
            isSupported = false;
            return;
        }
    }
}