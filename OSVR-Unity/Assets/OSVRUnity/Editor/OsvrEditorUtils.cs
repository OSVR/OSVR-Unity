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
#if UNITY_5
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
    private const string OSVR_CONFIG_FILENAME = "OSVR-Config.exe"; //default server config

    //trackerview
    private const string OSVR_TRACKERVIEW_PROCESS = "OSVRTrackerView";
    private const string OSVR_TRACKERVIEW_FILENAME = "OSVRTrackerView.exe";
    private const string OSVR_CONFIG_PROCESS = "OSVRConfig (32 bit)"; //default server filename
    private const string OSVR_TRACKERVIEW_README = "https://github.com/OSVR/OSVR-Tracker-Viewer/blob/master/README.md";
    private const string OSVR_GETTINGSTARTED_README = "https://github.com/OSVR/OSVR-Unity/blob/master/GettingStarted.md";
    private const string OSVR_UNITY_SOURCE = "https://github.com/OSVR/OSVR-Unity";
    private const string OSVR_UNITY_RENDERING_SOURCE = "https://github.com/OSVR/OSVR-Unity-Rendering";
    private const string RENDERMANAGER_SOURCE = "https://github.com/sensics/OSVR-RenderManager";
    private const string OSVR_DOCS = "https://github.com/OSVR/OSVR-Docs";
    private const string OSVR_DEVICES = "http://osvr.github.io/compatibility/";
    private const string OSVR_GITHUB_IO = "http://osvr.github.io/";
    private const string OSVR_SDK_INSTALLER = "http://access.osvr.com/binary/osvr-sdk-installer";
    private const string OSVR_CONFIG_INSTALLER = "http://access.osvr.com/binary/osvr_config";
    private const string OSVR_CONTROL = "https://github.com/OSVR/OSVR-Docs/blob/master/Utilities/OSVRControl.md";


    //print tree
    private const string OSVR_PRINTTREE_PROCESS = "osvr_print_tree";
    private const string OSVR_PRINTTREE_FILENAME = "osvr_print_tree.exe";
    private const string OSVR_PRINTTREE_README = "http://resource.osvr.com/docs/OSVR-Core/OSVRPrintTree.html";

    //reset yaw
    private const string OSVR_RESETYAW_PROCESS = "osvr_reset_yaw";
    private const string OSVR_RESETYAW_FILENAME = "osvr_reset_yaw.exe";

    //EditorPrefs keys for caching values
    private const string PP_OSVR_DIR_KEY = "osvr_server_dir"; //EditorPrefs key
    private const string PP_OSVR_EXE_KEY = "osvr_server_exe"; //EditorPrefs key
    private const string PP_OSVR_ARGS_KEY = "osvr_server_args"; //EditorPrefs key
    private const string PP_TRACKERVIEW_ARGS_KEY = "trackerview_args"; //EditorPrefs key
    private const string PP_OSVR_CONFIG_KEY = "osvr_config_dir"; //EditorPrefs key

    private bool isServerRunning = false; //is an osvr_server.exe process running?

    public string OsvrServerDirectory = OSVR_RUNTIME_DIR; //current server directory
    public string OsvrServerFilename = OSVR_SERVER_FILENAME; //current filename of server
    public string OsvrServerArguments = OSVR_SERVER_CONFIG; //current command-line args 
    public string OsvrConfigDirectory = ""; //current OSVR-Config directory

    public string TrackerViewArguments = ""; //current command-line args 
    public string TrackerViewFilename = OSVR_TRACKERVIEW_FILENAME; //current command-line args 

    public string PrintTreeArguments = ""; //current command-line args 
    public string PrintTreeFilename = OSVR_PRINTTREE_FILENAME; //current command-line args 

    //OSVR logo
    private Texture2D osvrLogo;

    [MenuItem("OSVR/OSVR Utilities")]
    public static void ShowWindow()
    { 
        OsvrEditorUtils osvrUtilsWindow = EditorWindow.GetWindow<OsvrEditorUtils>();
        osvrUtilsWindow.Load();
        GUIContent titleContent = new GUIContent("OSVR");
        osvrUtilsWindow.titleContent = titleContent;

        //set the OSVR server directory
        if (osvrUtilsWindow.OsvrServerDirectory == "" && Directory.Exists(OSVR_RUNTIME_DIR))
        {
            osvrUtilsWindow.OsvrServerDirectory = OSVR_RUNTIME_DIR;
            osvrUtilsWindow.SavePath(OSVR_RUNTIME_DIR);
        }
        else if (osvrUtilsWindow.OsvrServerDirectory == "" && Directory.Exists(OSVR_SDK_DIR))
        {
            osvrUtilsWindow.OsvrServerDirectory = OSVR_SDK_DIR;
            osvrUtilsWindow.SavePath(OSVR_SDK_DIR);
        }       
    }

    void OnEnable()
    {
        var dir = MonoScript.FromScriptableObject(this);
        string logoPath = AssetDatabase.GetAssetPath(dir);
        logoPath = Path.GetDirectoryName(logoPath).Replace("Editor", "Textures/");
        osvrLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath + "osvr-logo.png");
    }

    void OnGUI()
    {
        if(osvrLogo != null)
        {
            GUILayout.Label(osvrLogo);
        }        

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
            OsvrServerArguments = "\"" + EditorUtility.OpenFilePanel("Select Configuration File", OsvrServerDirectory, "json").Replace("/", "\\") + "\"";
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
        if (GUILayout.Button("Save Server Path/Config"))
        {
            Save();
            
        }
        #endregion
        #region OSVR-CONFIG
        GUILayout.Label("OSVR-Config", EditorStyles.boldLabel);
        OsvrConfigDirectory = EditorGUILayout.TextField("OSVR-Config Directory", OsvrConfigDirectory);
        if (GUILayout.Button("Launch OSVR-Config Utility"))
        {
            Save();
            LaunchOSVRConfig();
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
        #region DOCUMENTATION
        GUILayout.Label("Documentation & Support", EditorStyles.boldLabel);
        if (GUILayout.Button("OSVR-Unity Getting Started Guide"))
        {
            Application.OpenURL(OSVR_GETTINGSTARTED_README);
        }
        if (GUILayout.Button("OSVR-Docs repo"))
        {
            Application.OpenURL(OSVR_DOCS);
        }
        if (GUILayout.Button("OSVR-Unity Source Code"))
        {
            Application.OpenURL(OSVR_UNITY_SOURCE);
        }
        if (GUILayout.Button("OSVR-Unity-Rendering-Plugin Source Code"))
        {
            Application.OpenURL(OSVR_UNITY_RENDERING_SOURCE);
        }
        if (GUILayout.Button("RenderManager Source Code"))
        {
            Application.OpenURL(RENDERMANAGER_SOURCE);
        }
        if (GUILayout.Button("OSVR Device Compatibility"))
        {
            Application.OpenURL(OSVR_DEVICES);
        }
        if (GUILayout.Button("Additional docs and support links"))
        {
            Application.OpenURL(OSVR_GITHUB_IO);
        }
        #endregion
        #region INSTALLERS
        GUILayout.Label("Installers", EditorStyles.boldLabel);
        if (GUILayout.Button("OSVR SDK"))
        {
            Application.OpenURL(OSVR_SDK_INSTALLER);
        }
        if (GUILayout.Button("OSVR Config"))
        {
            Application.OpenURL(OSVR_CONFIG_INSTALLER);
        }
        if (GUILayout.Button("OSVR Control"))
        {
            Application.OpenURL(OSVR_CONTROL);
        }
        #endregion
    }

    //Load server properties from EditorPrefs
    private void Load()
    {
        OsvrServerDirectory = EditorPrefs.GetString(PP_OSVR_DIR_KEY, OSVR_RUNTIME_DIR);
        OsvrServerFilename = EditorPrefs.GetString(PP_OSVR_EXE_KEY, OSVR_SERVER_FILENAME);
        OsvrServerArguments = EditorPrefs.GetString(PP_OSVR_ARGS_KEY, OSVR_SERVER_CONFIG);
        TrackerViewArguments = EditorPrefs.GetString(PP_TRACKERVIEW_ARGS_KEY, "");
        OsvrConfigDirectory = EditorPrefs.GetString(PP_OSVR_CONFIG_KEY, "");
    }

    //Save server properties in EditorPrefs
    private void Save()
    {
        EditorPrefs.SetString(PP_OSVR_DIR_KEY, OsvrServerDirectory);
        EditorPrefs.SetString(PP_OSVR_EXE_KEY, OsvrServerFilename);
        EditorPrefs.SetString(PP_OSVR_ARGS_KEY, OsvrServerArguments);
        EditorPrefs.SetString(PP_OSVR_CONFIG_KEY, OsvrConfigDirectory);
    }

    //Save OSVR Server path to EditorPrefs
    private void SavePath(string p)
    {
        EditorPrefs.SetString(PP_OSVR_DIR_KEY, p);
    }

    //Save TrackerView command-line args
    private void SaveTrackerViewArguments()
    {
        EditorPrefs.SetString(PP_TRACKERVIEW_ARGS_KEY, TrackerViewArguments);
    }

    //Launch osvr_reset_yaw.exe
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

    //Launch osvr_print_tree.exe
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

    //Launch osvr_server.exe with option to kill an existing server process
    private void LaunchServer(bool killRunningServer)
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

    //launch OSVR-Config utility
    private void LaunchOSVRConfig()
    {
        if(File.Exists(OsvrConfigDirectory + "\\" + OSVR_CONFIG_FILENAME))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrConfigDirectory,
                FileName = OSVR_CONFIG_FILENAME,
                Arguments = "",
                ErrorDialog = true
            });
        }
        
    }

    //launch OSVRTrackerView.exe
    private void LaunchTrackerView()
    {
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = TrackerViewFilename,
            Arguments = TrackerViewArguments,
            ErrorDialog = true
        });
    }

    //kill a process by process name
    private void KillProcess(string processName)
    {
        Process[] processNames = Process.GetProcessesByName(processName);
        for(int i = processNames.Length - 1; i > -1; i--)
        {
            Process p = processNames[i];
            p.Kill();
        }
    }

    //check if a process is running
    private bool CheckProcessRunning(string processName)
    {
        Process[] processNames = Process.GetProcessesByName(processName);
        return processNames.Length != 0;
    }
}
#endif
