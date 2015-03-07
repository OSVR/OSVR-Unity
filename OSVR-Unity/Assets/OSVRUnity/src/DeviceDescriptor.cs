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

using UnityEngine;
using System.Collections;

//a class for storing information about a device based on it's json descriptor file
public class DeviceDescriptor {

    //field of view
    private float _monocularHorizontal = 60f;
    public float MonocularHorizontal
    {
        get { return _monocularHorizontal; }
        set { _monocularHorizontal = value; }
    }
    private float _monocularVertical = 60f;
    public float MonocularVertical
    {
        get { return _monocularVertical; }
        set { _monocularVertical = value; }
    }
    private float _overlapPercent = 100f;
    public float OverlapPercent
    {
        get { return _overlapPercent; }
        set { _overlapPercent = value; }
    }
    private float _pitchTilt = 0;
    public float PitchTilt
    {
        get { return _pitchTilt; }
        set { _pitchTilt = value; }
    }
    //resolutions
    private int _width = 1920;
    public int Width
    {
        get { return _width; }
        set { _width = value;}
    }
    private int _height = 1080;
    public int Height
    {
        get { return _height; }
        set { _height = value; }
    }
    private int _videoInputs = 1;
    public int VideoInputs
    {
        get { return _videoInputs; }
        set { _videoInputs = value; }
    }
    private string _displayMode = "horz_side_by_side";
    public string DisplayMode
    {
        get { return _displayMode; }
        set { _displayMode = value; }
    }
    //distortion
    private float _k1Red = 0;
    public float K1Red
    {
        get { return _k1Red; }
        set { _k1Red = value; }
    }
    private float _k1Green = 0;
    public float K1Green
    {
        get { return _k1Green; }
        set { _k1Green = value; }
    }
    private float _k1Blue = 0;
    public float K1Blue
    {
        get { return _k1Blue; }
        set { _k1Blue = value; }
    }
    //rendering
    private float _leftRoll = 0;
    public float LeftRoll
    {
        get { return _leftRoll; }
        set { _leftRoll = value; }
    }
    private float _rightRoll = 0;
    public float RightRoll
    {
        get { return _rightRoll; }
        set { _rightRoll = value; }
    }
    //eyes
    private float _centerProjX = 0.5f;
    public float CenterProjX
    {
        get { return _centerProjX; }
        set { _centerProjX = value; }
    }
    private float _centerProjY = 0.5f;
    public float CenterProjY
    {
        get { return _centerProjY; }
        set { _centerProjY = value; }
    }
    private int _rotate180 = 0;
    public int Rotate180
    {
        get { return _rotate180; }
        set { _rotate180 = value; }
    }

    //constructors
    public DeviceDescriptor() { }
    public DeviceDescriptor(float monocularHorizontal, float monocularVertical, float overlapPercent, float pitchTilt,
        int width, int height, int videoInputs, string displayMode, float k1Red, float k1Green, float k1Blue, float leftRoll,
        float rightRoll, float centerProjX, float centerProjY, int rotate180)
    {
        this._monocularHorizontal = monocularHorizontal;
        this._monocularVertical = monocularVertical;
        this._overlapPercent = overlapPercent;
        this._pitchTilt = pitchTilt;
        this._width = width;
        this._height = height;
        this._videoInputs = videoInputs;
        this._displayMode = displayMode;
        this._k1Red = k1Red;
        this._k1Green = k1Green;
        this._k1Blue = k1Blue;
        this._leftRoll = leftRoll;
        this._rightRoll = rightRoll;
        this._centerProjX = centerProjX;
        this._centerProjY = centerProjY;
        this._rotate180 = rotate180;
    }
}
