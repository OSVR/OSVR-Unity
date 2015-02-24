/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using UnityEngine;
using System.Collections;

//a class for storing information about a device based on it's json descriptor file
public class DeviceDescriptor {

    //field of view
    private float monocular_horizontal = 60f;
    private float monocular_vertical = 60f;
    private float overlap_percent = 100f;
    private float pitch_tilt = 0;
    //resolutions
    private int width = 1920;
    private int height = 1080;
    private int video_inputs = 1;
    private string display_mode = "horz_side_by_side";
    //distortion
    private float k1_red = 0;
    private float k1_green = 0;
    private float k1_blue = 0;
    //rendering
    private float left_roll = 0;
    private float right_roll = 0;
    //eyes
    private float center_proj_x = 0.5f;
    private float center_proj_y = 0.5f;
    private int rotate_180 = 0;

    //constructors
    public DeviceDescriptor() { }
    public DeviceDescriptor(float monocular_horizontal, float monocular_vertical, float overlap_percent, float pitch_tilt,
        int width, int height, int video_inputs, string display_mode, float k1_red, float k1_green, float k1_blue, float left_roll,
        float right_roll, float center_proj_x, float center_proj_y, int rotate_180)
    {
        this.monocular_horizontal = monocular_horizontal;
        this.monocular_vertical = monocular_vertical;
        this.overlap_percent = overlap_percent;
        this.pitch_tilt = pitch_tilt;
        this.width = width;
        this.height = height;
        this.video_inputs = video_inputs;
        this.display_mode = display_mode;
        this.k1_red = k1_red;
        this.k1_green = k1_green;
        this.k1_blue = k1_blue;
        this.left_roll = left_roll;
        this.right_roll = right_roll;
        this.center_proj_x = center_proj_x;
        this.center_proj_y = center_proj_y;
        this.rotate_180 = rotate_180;      
    }

    //getters
    public float getMonocularHorizontal() { return monocular_horizontal; }
    public float getMonocularVertical() { return monocular_vertical; }
    public float getOverlapPercent() { return overlap_percent; }
    public float getPitchTilt() { return pitch_tilt; }
    public int getWidth() { return width; }
    public int getHeight() { return height; }
    public int getVideoInputs() { return video_inputs; }
    public string getDisplayMode() { return display_mode; }
    public float getK1Red() { return k1_red; }
    public float getK1Green() { return k1_green; }
    public float getK1Blue() { return k1_blue; }
    public float getLeftRoll() { return left_roll; }
    public float getRightRoll() { return right_roll; }
    public float getCenterProjX() { return center_proj_x; }
    public float getCenterProjY() { return center_proj_y; }
    public int getRotate180() { return rotate_180; }

    //setters
    public void setMonocularHorizontal(float monocular_horizontal) { this.monocular_horizontal = monocular_horizontal; }
    public void setMonocularVertical(float monocular_vertical) { this.monocular_vertical = monocular_vertical; }
    public void setOverlapPercent(float overlap_percent) { this.overlap_percent = overlap_percent; }
    public void setPitchTilt(float pitch_tilt) { this.pitch_tilt = pitch_tilt; }
    public void setWidth(int width) { this.width = width; }
    public void setHeight(int height) { this.height = height; }
    public void setVideoInputs(int video_inputs) { this.video_inputs = video_inputs; }
    public void setDisplayMode(string display_mode) { this.display_mode = display_mode; }
    public void setK1Red(float k1_red) { this.k1_red = k1_red; }
    public void setK1Green(float k1_green) { this.k1_green = k1_green; }
    public void setK1Blue(float k1_blue) { this.k1_blue = k1_blue; }
    public void setLeftRoll(float left_roll) { this.left_roll = left_roll; }
    public void setRightRoll(float right_roll) { this.right_roll = right_roll; }
    public void setCenterProjX(float center_proj_x) { this.center_proj_x = center_proj_x; }
    public void setCenterProjY(float center_proj_y) { this.center_proj_y = center_proj_y; }
    public void setRotate180(int rotate_180) { this.rotate_180 = rotate_180; }

}
