/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */

using System;
using UnityEngine;
using SimpleJSON;

namespace OSVR
{
    namespace Unity
    {
        /// <summary>
        /// Display interface: provides information about the device display that is needed to set correct values in Unity scene.
        /// 
        /// Currently parses a JSON descriptor file provided by developer in the Unity scene.
        /// If a file is not provided, it reads from /display, which will eventually be the default way that this class works.
        /// </summary>
        public class DisplayInterface : InterfaceGameObject
        {
            private string deviceDescriptorJson;
            public TextAsset jsonDescriptorFile;
            new void Awake()
            {
                if (jsonDescriptorFile != null)
                    deviceDescriptorJson = jsonDescriptorFile.text; //read JSON file directly from Unity if provided
                else
                    deviceDescriptorJson = ClientKit.instance.context.getStringParameter("/display"); //otherwise read from /display               
            }

            /// <summary>
            /// This function will parse the device parameters from a device descriptor json file.
            /// 
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDisplayParameters(TextAsset jsonDescriptor)
            {
                deviceDescriptorJson = jsonDescriptor.text;
                return GetDeviceDescription();
            }

            /// <summary>
            /// This function will parse the device parameters from a device descriptor json file.
            /// 
            /// Returns a DeviceDescriptor object containing stored json values.
            /// </summary>
            public DeviceDescriptor GetDeviceDescription()
            {
                
                JSONNode parsedJsonDisplayParams = JSON.Parse(deviceDescriptorJson);

                //field of view
                float monocular_horizontal = parsedJsonDisplayParams["hmd"]["field_of_view"]["monocular_horizontal"].AsFloat;
                float monocular_vertical = parsedJsonDisplayParams["hmd"]["field_of_view"]["monocular_vertical"].AsFloat;
                float overlap_percent = parsedJsonDisplayParams["hmd"]["field_of_view"]["overlap_percent"].AsFloat;
                float pitch_tilt = parsedJsonDisplayParams["hmd"]["field_of_view"]["pitch_tilt"].AsFloat;
              
                //resolutions
                int width = parsedJsonDisplayParams["hmd"]["resolutions"][0]["width"].AsInt;
                int height = parsedJsonDisplayParams["hmd"]["resolutions"][0]["height"].AsInt;
                int video_inputs = parsedJsonDisplayParams["hmd"]["resolutions"][0]["video_inputs"].AsInt;
                string display_mode = parsedJsonDisplayParams["hmd"]["resolutions"][0]["display_mode"].Value;

                //distortion
                float k1_red = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;
                float k1_green = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;
                float k1_blue = parsedJsonDisplayParams["hmd"]["distortion"]["k1_red"].AsFloat;

                //rendering
                float left_roll = parsedJsonDisplayParams["hmd"]["rendering"]["left_roll"].AsFloat;
                float right_roll = parsedJsonDisplayParams["hmd"]["rendering"]["right_roll"].AsFloat;

                //eyes
                float center_proj_x = parsedJsonDisplayParams["hmd"]["eyes"][0]["center_proj_x"].AsFloat;
                float center_proj_y = parsedJsonDisplayParams["hmd"]["eyes"][0]["center_proj_y"].AsFloat;
                int rotate_180 = parsedJsonDisplayParams["hmd"]["eyes"][0]["rotate_180"].AsInt;

                //print
                string parsedJson = "FIELD OF VIEW:\n";
                parsedJson += "monocular_horizontal = " + monocular_horizontal + "\n";
                parsedJson += "monocular_vertical = " + monocular_vertical + "\n";
                parsedJson += "overlap_percent = " + overlap_percent + "\n";
                parsedJson += "pitch_tilt = " + pitch_tilt + "\n";
                parsedJson += "\nRESOLUTION\n";
                parsedJson += "width = " + width + "\n";
                parsedJson += "height = " + width + "\n";
                parsedJson += "video_inputs = " + video_inputs + "\n";
                parsedJson += "display_mode = " + display_mode + "\n";
                parsedJson += "\nDISTORTION\n";
                parsedJson += "k1_red = " + k1_red + "\n";
                parsedJson += "k1_green = " + k1_green + "\n";
                parsedJson += "k1_blue = " + k1_blue + "\n";
                parsedJson += "\nRENDERING\n";
                parsedJson += "left_roll = " + left_roll + "\n";
                parsedJson += "right_roll = " + right_roll + "\n";
                parsedJson += "\nEYES\n";
                parsedJson += "center_proj_x = " + center_proj_x + "\n";
                parsedJson += "center_proj_y = " + center_proj_y + "\n";
                parsedJson += "rotate_180 = " + rotate_180 + "\n";
                Debug.Log("Parsed " + jsonDescriptorFile.name + ".json:\n" + parsedJson);

                //create a device descriptor object and store the parsed json
                DeviceDescriptor deviceDescriptor = new DeviceDescriptor();
                deviceDescriptor.setMonocularHorizontal(monocular_horizontal);
                deviceDescriptor.setMonocularVertical(monocular_vertical);
                deviceDescriptor.setOverlapPercent(overlap_percent);
                deviceDescriptor.setPitchTilt(pitch_tilt);
                deviceDescriptor.setWidth(width);
                deviceDescriptor.setHeight(height);
                deviceDescriptor.setVideoInputs(video_inputs);
                deviceDescriptor.setDisplayMode(display_mode);
                deviceDescriptor.setK1Red(k1_red);
                deviceDescriptor.setK1Green(k1_green);
                deviceDescriptor.setK1Blue(k1_blue);
                deviceDescriptor.setLeftRoll(left_roll);
                deviceDescriptor.setRightRoll(right_roll);
                deviceDescriptor.setCenterProjX(center_proj_x);
                deviceDescriptor.setCenterProjY(center_proj_y);
                deviceDescriptor.setRotate180(rotate_180);
                return deviceDescriptor;
            }      
        }
    }
}