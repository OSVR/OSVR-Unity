/// OSVR-Unity
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2016 Sensics, Inc.
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
using UnityEditor;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using System.Text;

public class OsvrEditorUtils : EditorWindow
{
    private const string OSVR_RUNTIME_DIR = "C:\\Program Files\\OSVR\\Runtime\\bin"; //default Runtime install path
    private const string OSVR_SDK_DIR = "C:\\Program Files\\OSVR\\SDK\\bin"; //default SDK install path
    private const string OSVR_SERVER_FILENAME = "osvr_server.exe"; //default server filename
    private const string OSVR_SERVER_PROCESS = "osvr_server"; //default server filename
    private const string OSVR_SERVER_CONFIG = "osvr_server_config.json"; //default server config

    //trackerview
    private const string OSVR_TRACKERVIEW_PROCESS = "OSVRTrackerView";
    private const string OSVR_TRACKERVIEW_FILENAME = "OSVRTrackerView.exe";
    private const string OSVR_TRACKERVIEW_README = "https://github.com/OSVR/OSVR-Tracker-Viewer/blob/master/README.md";
    private const string OSVR_GETTINGSTARTED_README = "https://github.com/OSVR/OSVR-Unity/blob/master/GettingStarted.md";

    //print tree
    private const string OSVR_PRINTTREE_PROCESS = "osvr_print_tree";
    private const string OSVR_PRINTTREE_FILENAME = "osvr_print_tree.exe";
    private const string OSVR_PRINTTREE_README = "http://resource.osvr.com/docs/OSVR-Core/OSVRPrintTree.html";

    private const string OSVR_CONFIG_URL = "https://github.com/OSVR/OSVR-Config";

    //reset yaw
    private const string OSVR_RESETYAW_PROCESS = "osvr_reset_yaw";
    private const string OSVR_RESETYAW_FILENAME = "osvr_reset_yaw.exe";

    //PlayerPrefs keys for caching values
    private const string PP_OSVR_DIR_KEY = "osvr_server_dir"; //PlayerPrefs key
    private const string PP_OSVR_EXE_KEY = "osvr_server_exe"; //PlayerPrefs key
    private const string PP_OSVR_ARGS_KEY = "osvr_server_args"; //PlayerPrefs key
    private const string PP_TRACKERVIEW_ARGS_KEY = "trackerview_args"; //PlayerPrefs key

    private bool isServerRunning = false; //is an osvr_server.exe process running?

    public static string OsvrServerDirectory = OSVR_RUNTIME_DIR; //current server directory
    public static string OsvrServerFilename = OSVR_SERVER_FILENAME; //current filename of server
    public static string OsvrServerArguments = OSVR_SERVER_CONFIG; //current command-line args 

    public static string TrackerViewArguments = ""; //current command-line args 
    public static string TrackerViewFilename = OSVR_TRACKERVIEW_FILENAME; //current command-line args 

    public static string PrintTreeArguments = ""; //current command-line args 
    public static string PrintTreeFilename = OSVR_PRINTTREE_FILENAME; //current command-line args 

    [MenuItem("Window/OSVR Utilities")]
    public static void ShowWindow()
    {
        Load();
        OsvrEditorUtils osvrUtilsWindow = EditorWindow.GetWindow<OsvrEditorUtils>();

        // Loads an icon from an image stored at the specified path
        //Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");

        GUIContent titleContent = new GUIContent("OSVR");
        osvrUtilsWindow.titleContent = titleContent;

        string path = "";
        //set the OSVR server directory
        if (Directory.Exists(OSVR_RUNTIME_DIR))
        {
            OsvrServerDirectory = OSVR_RUNTIME_DIR;
        }
        else if (Directory.Exists(OSVR_SDK_DIR))
        {
            OsvrServerDirectory = OSVR_SDK_DIR;
        }
        SavePath(path);
    }

    //Load server properties from PlayerPrefs
    public static void Load()
    {
        OsvrServerDirectory = PlayerPrefs.GetString(PP_OSVR_DIR_KEY, OSVR_RUNTIME_DIR);
        OsvrServerFilename = PlayerPrefs.GetString(PP_OSVR_EXE_KEY, OSVR_SERVER_FILENAME);
        OsvrServerArguments = PlayerPrefs.GetString(PP_OSVR_ARGS_KEY, OSVR_SERVER_CONFIG);
        TrackerViewArguments = PlayerPrefs.GetString(PP_TRACKERVIEW_ARGS_KEY, "");
    }

    //Save server properties in PlayerPrefs
    public static void Save()
    {
        PlayerPrefs.SetString(PP_OSVR_DIR_KEY, OsvrServerDirectory);
        PlayerPrefs.SetString(PP_OSVR_EXE_KEY, OsvrServerFilename);
        PlayerPrefs.SetString(PP_OSVR_ARGS_KEY, OsvrServerArguments);
        PlayerPrefs.Save();
    }

    public static void SavePath(string p)
    {
        PlayerPrefs.SetString(PP_OSVR_DIR_KEY, p);
        PlayerPrefs.Save();
    }

    public static void SaveTrackerViewArguments()
    {
        PlayerPrefs.SetString(PP_TRACKERVIEW_ARGS_KEY, TrackerViewArguments);
        PlayerPrefs.Save();
    }

    void OnGUI()
    {
        #region OSVR-Unity
        GUILayout.Label("OSVR-Unity", EditorStyles.boldLabel);
        if (GUILayout.Button("Open OSVR-Unity Getting Started Guide"))
        {
            Application.OpenURL(OSVR_GETTINGSTARTED_README);
        }
        //@todo add version information

        #endregion
        #region OSVR_SERVER
        GUILayout.Label("OSVR Server Settings", EditorStyles.boldLabel);
        OsvrServerDirectory = EditorGUILayout.TextField("OSVR Directory", OsvrServerDirectory);
        if (CheckProcessRunning(OSVR_SERVER_PROCESS))
        {
            isServerRunning = true;
            EditorGUILayout.LabelField("osvr_server.exe is running.");
        }
        else
        {
            isServerRunning = false;
            EditorGUILayout.LabelField("osvr_server.exe is not running.");
        }

        OsvrServerArguments = EditorGUILayout.TextField("Configuration file", OsvrServerArguments);
        if (GUILayout.Button("Select Config File"))
        {
            OsvrServerArguments = Path.GetFileName(EditorUtility.OpenFilePanel("Select Configuration File", OsvrServerDirectory, "json"));
        }

        if(isServerRunning)
        {
            if (GUILayout.Button("Save & Launch New OSVR Server"))
            {
                Save();
                LaunchServer(true);
            }
            if (GUILayout.Button("Shutdown OSVR Server"))
            {
                KillProcess(OSVR_SERVER_PROCESS);
            }
        }
        else
        {
            if (GUILayout.Button("Save & Launch OSVR Server"))
            {
                Save();
                LaunchServer(false);
            }
        }        
        if (GUILayout.Button("Save Server Settings"))
        {
            Save();
            
        }
        if (GUILayout.Button("Get OSVR-Config Utility"))
        {
            Application.OpenURL(OSVR_CONFIG_URL);
        }
            #endregion
        #region OSVR_TRACKERVIEW
            //Tracker View
            GUILayout.Label("Tracker Viewer", EditorStyles.boldLabel);
        if (isServerRunning)
        {
            TrackerViewArguments = EditorGUILayout.TextField("TrackerView arguments", TrackerViewArguments);
            if(GUILayout.Button("View OSVRTrackerView Readme"))
            {
                Application.OpenURL(OSVR_TRACKERVIEW_README);
            }
            bool isTrackerViewRunning = CheckProcessRunning(OSVR_TRACKERVIEW_PROCESS);
            if (!isTrackerViewRunning)
            {
                if (GUILayout.Button("Launch OSVRTrackerView"))
                {
                    SaveTrackerViewArguments();
                    LaunchTrackerView();
                }
            }
            else
            {
                if (GUILayout.Button("Shutdown OSVRTrackerView"))
                {
                    KillProcess(OSVR_TRACKERVIEW_PROCESS);
                }
            }
        }
        else
        {

            // Disable the jumping height control if canJump is false:
            EditorGUI.BeginDisabledGroup(isServerRunning == false);
            GUILayout.Button("Start osvr_server.exe to enable OSVRTrackerView");
            EditorGUI.EndDisabledGroup();
        }
        #endregion
        #region OSVR_PRINT_TREE
        //Print Tree
        GUILayout.Label("Print Tree", EditorStyles.boldLabel);
        if (isServerRunning)
        {
            PrintTreeArguments = EditorGUILayout.TextField("osvr_print_tree arguments", PrintTreeArguments);
            if (GUILayout.Button("View osvr_print_tree Readme"))
            {
                Application.OpenURL(OSVR_PRINTTREE_README);
            }
            bool isPrintTreeRunning = CheckProcessRunning(OSVR_PRINTTREE_PROCESS); ;
            if (!isPrintTreeRunning)
            {
                if (GUILayout.Button("Launch osvr_print_tree"))
                {
                    LaunchPrintTree();
                }
            }
            else
            {
                if (GUILayout.Button("Shut down osvr_print_tree.exe"))
                {
                    KillProcess(OSVR_PRINTTREE_PROCESS);
                }
            }
        }
        else
        {

            // Disable the jumping height control if canJump is false:
            EditorGUI.BeginDisabledGroup(isServerRunning == false);
            GUILayout.Button("Start osvr_server.exe to enable Print Tree");
            EditorGUI.EndDisabledGroup();
        }
        #endregion
        #region RECENTER
        //Recenter
        GUILayout.Label("Recenter", EditorStyles.boldLabel);
        if (isServerRunning)
        {
            bool isResetYawRunning = CheckProcessRunning(OSVR_RESETYAW_PROCESS);
            if (!isResetYawRunning)
            {
                if (GUILayout.Button("Launch osvr_reset_yaw.exe"))
                {
                    LaunchResetYaw();
                }
            }
            else
            {
                if (GUILayout.Button("Shut down osvr_reset_yaw.exe"))
                {
                    KillProcess(OSVR_RESETYAW_PROCESS);
                }
            }
        }
        else
        {

            // Disable the jumping height control if canJump is false:
            EditorGUI.BeginDisabledGroup(isServerRunning == false);
            GUILayout.Button("Start osvr_server.exe to recenter HMD");
            EditorGUI.EndDisabledGroup();
        }
        #endregion
        #region DIRECT MODE
        //Tracker View
        GUILayout.Label("Direct Mode", EditorStyles.boldLabel);
        if (GUILayout.Button("Enable Direct Mode NVIDIA"))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = "EnableOSVRDirectMode.exe",
                Arguments = "",
                ErrorDialog = true
            });
        }
        if (GUILayout.Button("Disable Direct Mode NVIDIA"))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = "DisableOSVRDirectMode.exe",
                Arguments = "",
                ErrorDialog = true
            });
        }
        if (GUILayout.Button("Enable Direct Mode AMD"))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = "EnableOSVRDirectModeAMD.exe",
                Arguments = "",
                ErrorDialog = true
            });
        }
        if (GUILayout.Button("Disable Direct Mode AMD"))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = "DisableOSVRDirectModeAMD.exe",
                Arguments = "",
                ErrorDialog = true
            });
        }

        if (GUILayout.Button("Direct Mode Debugging"))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = "DirectModeDebugging.exe",
                Arguments = "",
                ErrorDialog = true
            });
        }
        #endregion
    }

    private void LaunchResetYaw()
    {
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = OSVR_RESETYAW_FILENAME,
            Arguments = "",
            ErrorDialog = true
        });
    }

    private void LaunchPrintTree()
    {
        ProcessStartInfo psi = new ProcessStartInfo()
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = "cmd.exe",
            Arguments = "/C osvr_print_tree.exe " + PrintTreeArguments + " & pause",
            ErrorDialog = true
        };

        Process p = new Process()
        {
            StartInfo = psi
        };
        p.Start();
    }

    public static void LaunchServer(bool killRunningServer)
    {
        if(killRunningServer)
        {
            KillProcess(OSVR_SERVER_PROCESS);
        }
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = OsvrServerFilename,
            Arguments = OsvrServerArguments,
            ErrorDialog = true
        });
    }

    public static void LaunchTrackerView()
    {
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = TrackerViewFilename,
            Arguments = TrackerViewArguments,
            ErrorDialog = true
        });
    }

    public static void KillProcess(string processName)
    {
        Process[] processNames = Process.GetProcessesByName(processName);
        for(int i = processNames.Length - 1; i > -1; i--)
        {
            Process p = processNames[i];
            p.Kill();
        }
    }

    private bool CheckProcessRunning(string processName)
    {
        Process[] processNames = Process.GetProcessesByName(processName);
        return processNames.Length != 0;
    }
}
