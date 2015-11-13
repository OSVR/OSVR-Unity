using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistortionMeshParameters
{

    public List<float> m_distortionPolynomialRed;  //< Constant, linear, quadratic, ... for Red
    public List<float> m_distortionPolynomialGreen;  //< Constant, linear, quadratic, ... for Green
    public List<float> m_distortionPolynomialBlue;  //< Constant, linear, quadratic, ... for Blue
    public Vector2 m_distortionCOP; //< (X,Y) location of center of projection in texture coords
    public Vector2 m_distortionD; //< How many K1's wide and high is (0-1) in texture coords
    public int m_desiredTriangles; //< How many triangles would we like in the mesh?


    public DistortionMeshParameters()
    {
        m_distortionCOP = new Vector2( 0.5f /* X */, 0.5f /* Y */ );
        m_distortionD = new Vector2(1 /* DX */, 1 /* DY */ );
        m_distortionPolynomialRed = new List<float>() { 0, 1 };
        m_distortionPolynomialGreen = new List<float>() { 0, 1 };
        m_distortionPolynomialBlue = new List<float>() { 0, 1 };
        m_desiredTriangles = 2;
    }

}
