using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class FovVisualizer : MonoBehaviour
{
    public GameObject fov; //object with a mesh filter and material
    private OSVR.Unity.DisplayController displayController;
    private Mesh mesh;
    private Camera cam;
    private MeshFilter meshFilter;
    private Vector3[] vertices;
    private int[] triangles;
    private bool init = false;

    void Start()
    {
        
        if(fov == null)
        {
            fov = FindObjectOfType<FovVisualizer>().gameObject;
        }
        if(fov == null)
        {
            //@todo create a gameobject and load a material from Resources
            Debug.LogError("No FOV Visualizer found in scene.");
            return;
        }
        cam = (Camera)this.transform.GetComponent<Camera>();
        displayController = FindObjectOfType<OSVR.Unity.DisplayController>();
        if(displayController == null)
        {
            Debug.LogError("No DisplayController found in scene. Camera may be positioned incorrectly.");
        }
        fov.transform.parent = cam.transform;
        fov.transform.localPosition = Vector3.zero;
        fov.transform.localRotation = cam.transform.localRotation;
        mesh = new Mesh();
        meshFilter = fov.GetComponent<MeshFilter>();
    }

   //@todo check whether or not there is a camera
    void Update()
    {
        if (fov == null) return;
        if (!init && displayController != null && displayController.DisplayConfig != null)
        {
            if(displayController.DisplayConfig.CheckDisplayStartup())
            {
                GameObject go = new GameObject();
                go.name = "CamOrigin";
                go.transform.position = displayController.Viewers[0].transform.position;
                this.transform.parent = go.transform;         
                init = true;                
            }
        }
        Vector3[] v = new Vector3[8];

        // find the 8 points that define the near and far clipping planes
        // near clipping plane
        v[0] = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, -cam.nearClipPlane)); //near top left
        v[1] = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -cam.nearClipPlane)); //near top right
        v[2] = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, -cam.nearClipPlane)); //near bottom right
        v[3] = cam.ScreenToWorldPoint(new Vector3(0, 0, -cam.nearClipPlane)); //near bottom left
        // far clipping plane
        v[4] = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, -cam.farClipPlane)); //far top left
        v[5] = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -cam.farClipPlane)); //far top right
        v[6] = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, -cam.farClipPlane)); //far bottom right
        v[7] = cam.ScreenToWorldPoint(new Vector3(0, 0, -cam.farClipPlane)); //far bottom left

        vertices = new Vector3[8];
        for (int i = 0; i < v.Length; i++)
        {
            vertices[i] = fov.transform.InverseTransformPoint(v[i]);
        }

        //connect the vertices with triangles
        triangles = new int[72];
        //near plane
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;

        triangles[3] = 3;
        triangles[4] = 1;
        triangles[5] = 2;

        //far plane
        triangles[6] = 4;
        triangles[7] = 5;
        triangles[8] = 7;

        triangles[9] = 7;
        triangles[10] = 5;
        triangles[11] = 6;

        //left plane
        triangles[12] = 7;
        triangles[13] = 4;
        triangles[14] = 0;

        triangles[15] = 7;
        triangles[16] = 0;
        triangles[17] = 3;

        //right plane
        triangles[18] = 2;
        triangles[19] = 1;
        triangles[20] = 5;

        triangles[21] = 2;
        triangles[22] = 5;
        triangles[23] = 6;

        //top plane
        triangles[24] = 1;
        triangles[25] = 0;
        triangles[26] = 4;

        triangles[27] = 1;
        triangles[28] = 4;
        triangles[29] = 5;

        //bottom plane
        triangles[30] = 2;
        triangles[31] = 3;
        triangles[32] = 7;

        triangles[33] = 2;
        triangles[34] = 7;
        triangles[35] = 6;

        //we want to see the FOV from inside or out, so duplicate each side with inverted normals
        //inv near plane
        triangles[36] = 3;
        triangles[37] = 1;
        triangles[38] = 0;

        triangles[39] = 2;
        triangles[40] = 1;
        triangles[41] = 3;

        //inv far plane
        triangles[42] = 7;
        triangles[43] = 5;
        triangles[44] = 4;

        triangles[45] = 6;
        triangles[46] = 5;
        triangles[47] = 7;

        //inv left plane
        triangles[48] = 0;
        triangles[49] = 4;
        triangles[50] = 7;

        triangles[51] = 3;
        triangles[52] = 0;
        triangles[53] = 7;

        //inv right plane
        triangles[54] = 5;
        triangles[55] = 1;
        triangles[56] = 2;

        triangles[57] = 6;
        triangles[58] = 5;
        triangles[59] = 2;

        //inv top plane
        triangles[60] = 4;
        triangles[61] = 0;
        triangles[62] = 1;

        triangles[63] = 5;
        triangles[64] = 4;
        triangles[65] = 1;

        //inv bottom plane
        triangles[66] = 7;
        triangles[67] = 3;
        triangles[68] = 2;

        triangles[69] = 6;
        triangles[70] = 7;
        triangles[71] = 2;

        //update the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }
}
