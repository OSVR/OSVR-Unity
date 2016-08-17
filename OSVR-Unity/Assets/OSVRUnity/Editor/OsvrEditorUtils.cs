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

public class OsvrEditorUtils : EditorWindow
{
    private const string OSVR_SERVER_ROOT = "OSVR_SERVER_ROOT";
    private const string OSVR_SERVER_ROOT_DEFAULT = "C:\\Program Files (x86)\\OSVR\\Runtime\\bin\\x64"; //Runtime OSVR Configurator Path
    private const string OSVR_CONFIG_RUNTIME_DIR = "C:\\Program Files (x86)\\OSVR\\Runtime\\config"; //Runtime OSVR Configurator Path
    private const string OSVR_CONFIG_SDK_DIR = "C:\\Program Files (x86)\\OSVR\\SDK\\config"; //SDK OSVR Configurator Path
    private const string OSVR_SERVER_FILENAME = "osvr_server.exe"; //default server filename
    private const string OSVR_SERVER_PROCESS = "osvr_server"; //default server filename
    private const string OSVR_SERVER_CONFIG = "\"C:\\Program Files (x86)\\OSVR\\Runtime\\bin\\x64\\osvr_server_config.json\""; //default server config
    private const string OSVR_CONFIG_FILENAME = "OSVR-Config.exe"; //default server config
    private const string OSVR_UNITY_VER_X86 = "\\Plugins\\x86\\osvrUnity-ver.txt";
    private const string OSVR_UNITY_VER_X86_64 = "\\Plugins\\x86_64\\osvrUnity-ver.txt";

    //osvr-central
    private const string OSVR_CENTRAL_FILENAME = "osvr_central.exe";

    //trackerview 
    private const string OSVR_TRACKERVIEW_PROCESS = "OSVRTrackerView";
    private const string OSVR_TRACKERVIEW_FILENAME = "OSVRTrackerView.exe";
    //OSVR URLS
    private const string OSVR_GETTINGSTARTED_README = "https://github.com/OSVR/OSVR-Unity/blob/master/GettingStarted.md";
    private const string OSVR_UNITY_SOURCE = "https://github.com/OSVR/OSVR-Unity";
    private const string RENDERMANAGER_OPTIMIZATION = "https://github.com/sensics/OSVR-RenderManager/blob/master/doc/renderingOptimization.md";
    private const string OSVR_DOCS = "https://github.com/OSVR/OSVR-Docs";
    private const string OSVR_GITHUB_IO = "http://osvr.github.io/";
    private const string OSVR_SDK_INSTALLER = "http://access.osvr.com/binary/osvr-sdk-installer";
    private const string OSVR_CONFIG_INSTALLER = "http://access.osvr.com/binary/osvr_config";
    private const string OSVR_CONTROL = "https://github.com/OSVR/OSVR-Docs/blob/master/Utilities/OSVRControl.md";
    private const string OSVR_UNITY_DOWNLOADS = "http://access.osvr.com/binary/osvr-unity";
    private const string OSVR_UNITY_LATEST = "http://resource.osvr.com/public_download/OSVR-Unity/osvrUnity-ver.txt";

    //print tree
    private const string OSVR_PRINTTREE_PROCESS = "osvr_print_tree";
    private const string OSVR_PRINTTREE_FILENAME = "osvr_print_tree.exe";

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
    private bool usingDefaultServer = false; //is osvr_server.exe in the official SDK install directory?

    public string OsvrServerDirectory = ""; //current server directory
    public string OsvrServerFilename = OSVR_SERVER_FILENAME; //current filename of server
    public string OsvrServerArguments = OSVR_SERVER_CONFIG; //current command-line args 
    public string TrackerViewFilename = OSVR_TRACKERVIEW_FILENAME; //current command-line args 
    public string PrintTreeFilename = OSVR_PRINTTREE_FILENAME; //current command-line args 

    public string currentOsvrUnityVersion = "";
    private string latestOsvrUnityVersion = "";
    private bool updateAvailable = false;
    private bool checkedForUpdate = false;
    private bool failedToReachServer = false;
    private WWW wwwOsvrUnityVersion;
    private GUIStyle versionLabelStyle;

    //OSVR logo
    private Texture2D osvrLogo;

    [MenuItem("OSVR/OSVR Utilities")]
    public static void ShowWindow()
    {
        OsvrEditorUtils osvrUtilsWindow = EditorWindow.GetWindow<OsvrEditorUtils>();
        osvrUtilsWindow.Load();
        osvrUtilsWindow.GetLatestOsvrUnity();
        GUIContent titleContent = new GUIContent("OSVR");
        osvrUtilsWindow.titleContent = titleContent;
        osvrUtilsWindow.versionLabelStyle = new GUIStyle();
        osvrUtilsWindow.versionLabelStyle.fontStyle = FontStyle.Bold;
        osvrUtilsWindow.versionLabelStyle.padding = new RectOffset(5, 0, 0, 0);

        //check if the server directory path has been set
        string serverPath = osvrUtilsWindow.GetPath(PP_OSVR_ARGS_KEY);
        if(Directory.Exists(serverPath) && File.Exists(serverPath + "\\" + OSVR_SERVER_FILENAME))
        {
            //a server exists at the saved path
            //check if it is the official SDK path
            if(serverPath.CompareTo(osvrUtilsWindow.GetDefaultPath()) == 0)
            {
                osvrUtilsWindow.usingDefaultServer = true;

            }
            else
            {
                osvrUtilsWindow.usingDefaultServer = false;
                Debug.LogWarning("[OSVR-Unity] Warning, saved server path in OSVR Editor Window is not the OSVR SDK default. This may or may not be intended.");
            }
        }
        else
        {
            Debug.Log("[OSVR-Unity] OSVR server not found, reverting to default path.");
            osvrUtilsWindow.OsvrServerDirectory = osvrUtilsWindow.GetDefaultPath();
            osvrUtilsWindow.SaveServerDirectory(osvrUtilsWindow.OsvrServerDirectory);
        }
    }


    void OnEnable()
    {
        var dir = MonoScript.FromScriptableObject(this);
        string logoPath = AssetDatabase.GetAssetPath(dir);
        logoPath = Path.GetDirectoryName(logoPath).Replace("Editor", "Textures/");
        osvrLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath + "osvr-logo.png");
    }

    void OnFocus()
    {
        CheckOSVRUnityVersion();
        CheckServerRunning();
    }


    void OnLostFocus()
    {
        CheckServerRunning();
    }

   
    void OnGUI()
    {
        if(osvrLogo != null)
        {
            GUILayout.Label(osvrLogo);
        }
        GUILayout.Label(currentOsvrUnityVersion, EditorStyles.boldLabel);
        if(!checkedForUpdate)
        {
            GUILayout.Label("Checking latest version... ", EditorStyles.boldLabel);
        }
        else
        {
            if (updateAvailable)
            {
                versionLabelStyle.normal.textColor = Color.yellow;
                GUILayout.Label("OSVR-Unity update available:\n" + latestOsvrUnityVersion, versionLabelStyle);
                if (GUILayout.Button("Download latest OSVR-Unity"))
                {
                    Application.OpenURL(OSVR_UNITY_DOWNLOADS);
                }
            }
            else if(!failedToReachServer)
            {
                versionLabelStyle.normal.textColor = Color.black;               
                GUILayout.Label("OSVR-Unity is up-to-date.", versionLabelStyle);
            }
            else
            {
                //@todo cache last known latest version
                versionLabelStyle.normal.textColor = Color.black;
                GUILayout.Label("Failed to reach server. Latest OSVR-Unity version unknown.", versionLabelStyle);
            }
        }
        
        #region OSVR_SERVER
        GUILayout.Label("OSVR Server Settings", EditorStyles.boldLabel);
        OsvrServerDirectory = EditorGUILayout.TextField("OSVR Directory", OsvrServerDirectory);
        if (CheckServerRunning())
        {
            EditorGUILayout.LabelField("osvr_server.exe is running.");
        }
        else
        {
            EditorGUILayout.LabelField("osvr_server.exe is not running.");
        }
        if(!usingDefaultServer)
        {
            EditorGUILayout.LabelField("Warning, OSVR server directory does not match OSVR_SERVER_ROOT environment variable.");
        }

        string serverArgs = EditorGUILayout.TextField("Configuration file", OsvrServerArguments);
        if (GUILayout.Button("Select Config File"))
        {
            serverArgs = "\"" + EditorUtility.OpenFilePanel("Select Configuration File", OsvrServerDirectory, "json").Replace("/", "\\") + "\"";
            OsvrServerArguments = serverArgs;
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
        #region OSVR_CENTRAL
        //Tracker View
        GUILayout.Label("OSVR Central", EditorStyles.boldLabel);
        if (GUILayout.Button("Launch OSVR-Central"))
        {
            LaunchOSVRCentral();
        }      
        #endregion
        #region OSVR-CONFIG
        GUILayout.Label("OSVR-Config", EditorStyles.boldLabel);
        if (GUILayout.Button("Launch OSVR-Config Utility"))
        {
            LaunchOSVRConfig();
        }

        #endregion
        #region OSVR_TRACKERVIEW
        //Tracker View
        GUILayout.Label("Tracker Viewer", EditorStyles.boldLabel);
        bool isTrackerViewRunning = CheckProcessRunning(OSVR_TRACKERVIEW_PROCESS);
        if (!isTrackerViewRunning)
        {
            if (GUILayout.Button("Launch OSVRTrackerView"))
            {
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
        #endregion
        #region OSVR_PRINT_TREE
        //Print Tree
        GUILayout.Label("Print Tree", EditorStyles.boldLabel);
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
            if (GUILayout.Button("Shut down osvr_print_tree"))
            {
                KillProcess(OSVR_PRINTTREE_PROCESS);
            }
        }
        #endregion
        #region RECENTER
        //Recenter
        GUILayout.Label("Recenter", EditorStyles.boldLabel);
        if (GUILayout.Button("Launch osvr_reset_yaw"))
        {
            LaunchResetYaw();
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
        if(GUILayout.Button("OSVR-Unity Downloads"))
        {
            Application.OpenURL(OSVR_UNITY_DOWNLOADS);
        }
        if (GUILayout.Button("OSVR-Unity Source Code"))
        {
            Application.OpenURL(OSVR_UNITY_SOURCE);
        }
        if (GUILayout.Button("OSVR-Docs repo"))
        {
            Application.OpenURL(OSVR_DOCS);
        }
        if(GUILayout.Button("RenderManager Rendering Optimizations"))
        {
            Application.OpenURL(RENDERMANAGER_OPTIMIZATION);
        }        
        if (GUILayout.Button("Additional docs and support links"))
        {
            Application.OpenURL(OSVR_GITHUB_IO);
        }
        #endregion
        #region INSTALLERS
        GUILayout.Label("Installers", EditorStyles.boldLabel);
        if (GUILayout.Button(new GUIContent("OSVR SDK", "OSVR SDK Installer with included utilities.")))
        {
            Application.OpenURL(OSVR_SDK_INSTALLER);
        }
        if (GUILayout.Button(new GUIContent("OSVR Control", "Application for toggling side-by-side mode and updating firmware.")))
        {
            Application.OpenURL(OSVR_CONTROL);
        }
        #endregion
    }

    //Load server properties from EditorPrefs
    private void Load()
    {
        OsvrServerDirectory = EditorPrefs.GetString(PP_OSVR_DIR_KEY, GetDefaultPath());
        OsvrServerFilename = EditorPrefs.GetString(PP_OSVR_EXE_KEY, OSVR_SERVER_FILENAME);
        OsvrServerArguments = EditorPrefs.GetString(PP_OSVR_ARGS_KEY, OSVR_SERVER_CONFIG);
    }

    //Save server properties in EditorPrefs
    private void Save()
    {
        SaveServerDirectory(OsvrServerDirectory);
        EditorPrefs.SetString(PP_OSVR_EXE_KEY, OsvrServerFilename);
        EditorPrefs.SetString(PP_OSVR_ARGS_KEY, OsvrServerArguments);
    }

    //Save OSVR Server path to EditorPrefs
    private void SaveServerDirectory(string p)
    {
        if (p.CompareTo(GetDefaultPath()) == 0)
        {
            usingDefaultServer = true;
        }
        else
        {
            usingDefaultServer = false;
        }
        EditorPrefs.SetString(PP_OSVR_DIR_KEY, p);
    }
    private string GetDefaultPath()
    {
        //get the OSVR server directory
        string defaultPath = Environment.GetEnvironmentVariable(OSVR_SERVER_ROOT);
        if (defaultPath != null)
        {
            if (Directory.Exists(defaultPath) && File.Exists(defaultPath + "\\" + OSVR_SERVER_FILENAME))
            {
                return defaultPath;
            }
            else
            {
                Debug.Log("[OSVR-Unity] OSVR Server not found in default SDK path.");
            }
        }
        else
        {
            Debug.Log("[OSVR-Unity] OSVR_SERVER_ROOT environment variable not found.");
        }
        return OSVR_SERVER_ROOT_DEFAULT;
    }
    private string GetPath(string p)
    {
        return EditorPrefs.GetString(PP_OSVR_DIR_KEY, GetDefaultPath());
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
            Arguments = "/C osvr_print_tree.exe & pause",
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

    //launch OSVR-Central utility
    private void LaunchOSVRCentral()
    {
        if (File.Exists(OsvrServerDirectory + "\\" + OSVR_CENTRAL_FILENAME))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OsvrServerDirectory,
                FileName = OSVR_CENTRAL_FILENAME,
                Arguments = "",
                ErrorDialog = true
            });
        }
    }

    //launch OSVR-Config utility
    private void LaunchOSVRConfig()
    {
        if(File.Exists(OSVR_CONFIG_RUNTIME_DIR + "\\" + OSVR_CONFIG_FILENAME))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OSVR_CONFIG_RUNTIME_DIR,
                FileName = OSVR_CONFIG_FILENAME,
                Arguments = "",
                ErrorDialog = true
            });
        }
        else if (File.Exists(OSVR_CONFIG_SDK_DIR + "\\" + OSVR_CONFIG_FILENAME))
        {
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = OSVR_CONFIG_SDK_DIR,
                FileName = OSVR_CONFIG_FILENAME,
                Arguments = "",
                ErrorDialog = true
            });
        }
        else
        {
            Debug.LogError("[OSVR-Unity] OSVR-Config utility not found in " + OSVR_CONFIG_RUNTIME_DIR + " or " + OSVR_CONFIG_SDK_DIR);
        }
    }

    //launch OSVRTrackerView.exe
    private void LaunchTrackerView()
    {
        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = OsvrServerDirectory,
            FileName = TrackerViewFilename,
            Arguments = "",
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

    //helper function to set a flag indicating server status
    private bool CheckServerRunning()
    {
        return isServerRunning = CheckProcessRunning(OSVR_SERVER_PROCESS);
    }

    //check osvrUnity-ver.txt for OSVR-Unity version
    private void CheckOSVRUnityVersion()
    {
        string osvrUnityVerPath = Application.dataPath + OSVR_UNITY_VER_X86;
        //find version information in osvrUnity-ver.txt
        if (File.Exists(osvrUnityVerPath))
        {
            currentOsvrUnityVersion = readTextFile(osvrUnityVerPath);
        }
        else
        {
            //osvrUnity-ver.txt exists in x86 and x86_64 dirs
            osvrUnityVerPath = Application.dataPath + OSVR_UNITY_VER_X86_64;
            if (File.Exists(osvrUnityVerPath))
            {
                currentOsvrUnityVersion = readTextFile(osvrUnityVerPath);
            }
            else
            {
                currentOsvrUnityVersion = "OSVR-Unity version unknown. Cannot find osvrUnity-ver.txt";
            }
        }     
    }

    //get the latest OSVR-Unity version from the web
    private void GetLatestOsvrUnity()
    {
        wwwOsvrUnityVersion = new WWW(OSVR_UNITY_LATEST);
        EditorApplication.update += Update;
    }

    private void Update()
    {
        if (wwwOsvrUnityVersion != null)
        {
            if (!wwwOsvrUnityVersion.isDone)
                return;
            
            if (CheckUrlSuccess(wwwOsvrUnityVersion))
            {
                latestOsvrUnityVersion = wwwOsvrUnityVersion.text;
                failedToReachServer = false;
                
            }
            else
            {
                failedToReachServer = true;
            }

            wwwOsvrUnityVersion = null;

            updateAvailable = failedToReachServer ? false : IsUpdateAvailable();
            checkedForUpdate = true;
        }
        EditorApplication.update -= Update;
    }

    //check if there is an error returned or the text doesn't start with OSVR-Unity
    private bool CheckUrlSuccess(WWW url)
    {        
        if (!string.IsNullOrEmpty(url.error) || !url.text.StartsWith("OSVR-Unity"))
        {
            return false;
        }
        return true;
    }

    //compare latest available OSVR-Unity version to currently installed version
    private bool IsUpdateAvailable()
    {
        if(latestOsvrUnityVersion.CompareTo(currentOsvrUnityVersion) == 0)
        {
            return false;
        }
        return true;
    }

    //Helper function that reads one line from a text file.
    private string readTextFile(string filePath)
    {
        StreamReader sr = new StreamReader(filePath);
        string firstLine = sr.ReadLine();
        sr.Close();
        return firstLine;
    }
}
#endif
