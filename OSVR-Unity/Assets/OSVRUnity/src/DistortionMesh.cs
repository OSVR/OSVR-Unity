using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

/// Describes a vertex 3D position plus 2D texture coordinate.
public class DistortionMeshVertex
{
    public Vector3 Position;
    public Vector2 TexRed;
    public Vector2 TexGreen;
    public Vector2 TexBlue;
    public DistortionMeshVertex(Vector3 pos, Vector2 texRed, Vector2 texGreen, Vector2 texBlue)
    {
        Position = pos;
        TexBlue = texBlue;
        TexGreen = texGreen;
        TexRed = texRed;
    }
    public DistortionMeshVertex()
    {
        Position = Vector3.zero;
        TexRed = TexBlue = TexGreen = Vector2.zero;
    }


    // Flips a texture coordinate that is in the range 0..1 so that
    // it is inverted about 0.5 to be in the range 1..0.  Useful for
    // flipping OpenGL Y coordinates into Direct3D ones.
    public static float flipTexCoord(float c) { return 1.0f - c; }
}

/// Describes a vertex 3D position plus 2D texture coordinate.
public struct Triangle
{
    public int[] indices;

    public Triangle(int size = 3)
    {
        indices = new int[size];
    }
}

public class DistortionMesh : MonoBehaviour {

    private const float RENDER_OVERFILL_FACTOR = 1.0f; //@todo get from core

    /// Describes the type of mesh to be constructed for distortion
    /// correction.
    public enum DistortionMeshType { SQUARE, RADIAL }


    public static List<DistortionMeshVertex> ComputeDistortionMeshVertices(DistortionMeshType distortionMeshType, DistortionMeshParameters distortionParameters, float orthoSize, float aspect)
    {
        List<DistortionMeshVertex> vertices = new List<DistortionMeshVertex>();

        if (distortionParameters.m_distortionPolynomialRed.Count < 2)
        {
            Debug.Log("RenderManager::ComputeDistortionMesh: Need 2+ red polynomial coefficients, found "
               + distortionParameters.m_distortionPolynomialRed.Count);
            return vertices;
        }
        if (distortionParameters.m_distortionPolynomialGreen.Count < 2)
        {
            Debug.Log("RenderManager::ComputeDistortionMesh: Need 2+ green polynomial coefficients, found "
               + distortionParameters.m_distortionPolynomialGreen.Count);
            return vertices;
        }
        if (distortionParameters.m_distortionPolynomialBlue.Count < 2)
        {
            Debug.Log("RenderManager::ComputeDistortionMesh: Need 2+ blue polynomial coefficients, found "
               + distortionParameters.m_distortionPolynomialBlue.Count);
            return vertices;
        }

        // See what kind of mesh we're supposed to produce.  Make the appropriate
        // one.
        switch (distortionMeshType)
        {
            case DistortionMeshType.SQUARE:
                {
                  
                    // Figure out how many quads we should use in each dimension.  The
                    // minimum is 1.  We have an even number in each.  There are two
                    // triangles per quad.
                    int quadsPerSide = (int)Mathf.Sqrt(distortionParameters.m_desiredTriangles / 2);
                    if (quadsPerSide < 1) { quadsPerSide = 1; }

                   // float worldUnitHeight = 2 * orthoSize;
                    //float worldUnitWidth = worldUnitHeight * aspect;
                    //float scaleX = worldUnitWidth / (float)quadsPerSide;
                   // float scaleY = worldUnitHeight / (float)quadsPerSide;

                    // Figure out how large each quad will be.  Recall that we're covering
                    // a range of 2 (from -1 to 1) in each dimension, so the quads will all
                    // be square in texture space.
                    float quadSide = 2.0f / quadsPerSide;
                    float quadTexSide = 1.0f / quadsPerSide;

                    // Generate a pair of triangles for each quad, wound counter-clockwise,
                    // with appropriate spatial location and texture coordinates.
                    // Compute distorted texture coordinates and use those for each vertex.
                    for (int x = 0; x < quadsPerSide; x++)
                    {
                        float xLow = -1 + x * quadSide;
                        float xHigh = -1 + (x + 1) * quadSide;
                        float xTexLow = x * quadTexSide;
                        float xTexHigh = (x + 1) * quadTexSide;
                    
                        for (int y = 0; y < quadsPerSide; y++)
                        {
                            float yLow = -1 + y * quadSide;
                            float yHigh = -1 + (y + 1) * quadSide;
                            float yTexLow = y * quadTexSide;
                            float yTexHigh = (y + 1) * quadTexSide;

                            Vector2 posLL = new Vector2(xLow , yLow );
                            Vector2 posLH = new Vector2(xLow, yHigh );
                            Vector2 posHL = new Vector2(xHigh, yLow );
                            Vector2 posHH = new Vector2(xHigh, yHigh );

                            Vector2 texLL = new Vector2(xTexLow, yTexLow );
                            Vector2 texLH = new Vector2(xTexLow, yTexHigh );
                            Vector2 texHL = new Vector2(xTexHigh, yTexLow );
                            Vector2 texHH = new Vector2(xTexHigh, yTexHigh );

                            Debug.Log("Pos LL is " + posLL);
                            Debug.Log("Pos LH is " + posLH);
                            Debug.Log("Pos HL is " + posHL);
                            Debug.Log("Pos HH is " + posHH);
                            // First triangle
                            vertices.Add(new DistortionMeshVertex(posLL,
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 2)
                              ));
                            vertices.Add(new DistortionMeshVertex(posHL,
                              DistortionCorrectTextureCoordinate(texHL, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texHL, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texHL, distortionParameters, 2)
                              ));
                            vertices.Add(new DistortionMeshVertex(posHH,
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 2)
                              ));

                            // Second triangle
                            vertices.Add(new DistortionMeshVertex(posLL,
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texLL, distortionParameters, 2)
                              ));
                            vertices.Add(new DistortionMeshVertex(posHH,
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texHH, distortionParameters, 2)
                              ));
                            vertices.Add(new DistortionMeshVertex(posLH,
                              DistortionCorrectTextureCoordinate(texLH, distortionParameters, 0),
                              DistortionCorrectTextureCoordinate(texLH, distortionParameters, 1),
                              DistortionCorrectTextureCoordinate(texLH, distortionParameters, 2)
                              ));
                        }
                    }
                }
                break;
            case DistortionMeshType.RADIAL:
                {
                    Debug.Log("RenderManager::ComputeDistortionMesh: Radial mesh type not yet implemented.");
                }
                break;
            default:
                Debug.Log("RenderManager::ComputeDistortionMesh: Unsupported mesh type.");
                break;
        }

        return vertices;
    }

    //< 0 = red, 1 = green, 2 = blue
    private static Vector2 DistortionCorrectTextureCoordinate(Vector2 inCoords, DistortionMeshParameters distort, int color)
    {
        Vector2 ret = inCoords;

        // Check for invalid parameters
        if (distort.m_distortionPolynomialRed.Count < 2) { return ret; }
        if (distort.m_distortionPolynomialGreen.Count < 2) { return ret; }
        if (distort.m_distortionPolynomialBlue.Count < 2) { return ret; }
        if (distort.m_distortionD[0] <= 0) { return ret; }
        if (distort.m_distortionD[1] <= 0) { return ret; }
        if (color > 2 || color < 0) { return ret; }

        float x = inCoords[0];
        float y = inCoords[1];

        // Convert from coordinates in the overfilled texture to coordinates
        // that will cover the range (0,0) to (1,1) on the screen.  This is
        // done by scaling around (0.5,0.5) to push the edges of the screen
        // out to the (0,0) and (1,1) boundaries.
        float xN = (x - 0.5f) * RENDER_OVERFILL_FACTOR + 0.5f;
        float yN = (y - 0.5f) * RENDER_OVERFILL_FACTOR + 0.5f;

        // Convert from normalized range to (D[0], D[1]) range.  Here,
        // both coordinate systems share a common (0,0) boundary so we
        // can just scale around the origin.
        float xD = xN * distort.m_distortionD[0];
        float yD = yN * distort.m_distortionD[1];

        // Compute the distance from the COP in D space
        // (direction and squared magnitude)
        float xDDiff = xD - distort.m_distortionCOP[0];
        float yDDiff = yD - distort.m_distortionCOP[1];
        float rMag2 = xDDiff * xDDiff + yDDiff * yDDiff;
        float rMag = Mathf.Sqrt(rMag2);
        if (rMag2 == 0)
        { // We're at the center -- no distortion
            ret = inCoords;
            return ret;
        }
        float xDNorm = xDDiff / rMag;
        float yDNorm = yDDiff / rMag;

        // Compute the new location in D space based on the distortion parameters
        List<float> parameters = new List<float>();
        switch (color)
        {
            case 0: parameters = distort.m_distortionPolynomialRed; break;
            case 1: parameters = distort.m_distortionPolynomialGreen; break;
            case 2: parameters = distort.m_distortionPolynomialBlue; break;
        }

        float rFactor = 1;
        float rNew = parameters[0];
        for (int i = 1; i < parameters.Count; i++) {
            rFactor *= rMag;
            rNew += parameters[i] * rFactor;
        }
        float xDNew = distort.m_distortionCOP[0] + rNew * xDNorm;
        float yDNew = distort.m_distortionCOP[1] + rNew * yDNorm;

        // Convert from D space back to unit space
        float xNNew = xDNew / distort.m_distortionD[0];
        float yNNew = yDNew / distort.m_distortionD[1];

        // Convert from unit space back into overfill space.
        float xNew = (xNNew - 0.5f) / RENDER_OVERFILL_FACTOR + 0.5f;
        float yNew = (yNNew - 0.5f) / RENDER_OVERFILL_FACTOR + 0.5f;

        ret = new Vector2(xNew, yNew);

        return ret;
    }

    public static Mesh CreatePolynomialDistortionMesh(List<DistortionMeshVertex> computedVertices, int quadsPerSide)
    {

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[computedVertices.Count];
        //3 sets of UVs for RGB distortion
        Vector2[] uvRed = new Vector2[computedVertices.Count];
        Vector2[] uvGreen = new Vector2[computedVertices.Count];
        Vector2[] uvBlue = new Vector2[computedVertices.Count];

        int numTriangles = computedVertices.Count;
        int[] triangles = new int[numTriangles];

        for (int j = 0; j < computedVertices.Count; j++)
        {
            vertices[j] = computedVertices[j].Position;
            uvRed[j] = computedVertices[j].TexRed;
            uvGreen[j] = computedVertices[j].TexGreen;
            uvBlue[j] = computedVertices[j].TexBlue;
            triangles[j] = j;
        }

        mesh.vertices = vertices;
        mesh.uv = uvRed;
        mesh.uv2 = uvGreen;
        mesh.uv3 = uvBlue;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        FlipMesh(mesh);
        return mesh;
    }

    public static void FlipMesh(Mesh m)
    {
        if (m)
        {
            int[] tris  = m.triangles;
            for (int i = 0; i < tris.Length; i += 3)
            {
                int t  = tris[i];
                tris[i] = tris[i + 1];
                tris[i + 1] = t;
            }
            m.triangles = tris;
        }
    }

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

    public static Mesh CreateFullScreenMeshPoly(float orthoSize, float aspect, int widthInQuads, int heightInQuads)
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
