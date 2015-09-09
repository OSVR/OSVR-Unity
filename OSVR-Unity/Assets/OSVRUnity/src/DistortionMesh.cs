using UnityEngine;
using UnityEditor;
using System.Collections;

public class DistortionMesh : MonoBehaviour {

    public static Mesh CreateFullScreenMesh(float orthoSize, float aspect, int widthInQuads, int heightInQuads)
    {
        Mesh mesh = new Mesh();
        float worldUnitHeight = 2 * orthoSize;
        float worldUnitWidth = worldUnitHeight * aspect;       
        int width = widthInQuads + 1;
        int height = heightInQuads + 1;
        int numVertices = width * height;
        int numTriangles = widthInQuads * heightInQuads * 6;
        
        Vector3[] vertices = new Vector3[numVertices];
        //3 sets of UVs for RGB distortion
        Vector2[] uvRed = new Vector2[numVertices];
        Vector2[] uvGreen = new Vector2[numVertices];
        Vector2[] uvBlue = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];
        int i = 0;
        float uvX = 1.0f / widthInQuads;
        float uvY = 1.0f / heightInQuads;
        float scaleX = worldUnitWidth / widthInQuads;
        float scaleY = worldUnitHeight / heightInQuads;
        for (float y = 0.0f; y < height; y++)
        {
            for (float x = 0.0f; x < width; x++)
            {
                vertices[i] = new Vector3(x * scaleX - worldUnitWidth / 2f, y * scaleY - worldUnitHeight / 2f);
                uvRed[i] = new Vector2(x * uvX, y * uvY);
                uvGreen[i] = new Vector2(x * uvX, y * uvY);
                uvBlue[i] = new Vector2(x * uvX, y * uvY);
                i++;
            }
        }
        i = 0;
        for (int y = 0; y < heightInQuads; y++)
        {
            for (int x = 0; x < widthInQuads; x++)
            {
                triangles[i] = (y * width) + x;
                triangles[i + 1] = ((y + 1) * width) + x;
                triangles[i + 2] = (y * width) + x + 1;

                triangles[i + 3] = ((y + 1) * width) + x;
                triangles[i + 4] = ((y + 1) * width) + x + 1;
                triangles[i + 5] = (y * width) + x + 1;
                i += 6;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uvRed;
        mesh.uv2 = uvGreen;
        mesh.uv3 = uvBlue;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    public static Mesh LoadDistortionMesh()
    {
        return Resources.Load<Mesh>("DistortionMesh");
    }
}
