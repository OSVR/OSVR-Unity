using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DLLSearchPathFixer
{
	public static void fix ()
	{
		string[] dllPaths = {
			"Assets" + Path.DirectorySeparatorChar + "Plugins",
			"Assets"+ Path.DirectorySeparatorChar + "Plugins"+ Path.DirectorySeparatorChar + "x86", // todo don't hardcode this
			"StreamingAssets"+ Path.DirectorySeparatorChar + "Plugins",
			"StreamingAssets"+ Path.DirectorySeparatorChar + "Plugins"+ Path.DirectorySeparatorChar + "x86" // todo don't hardcode this
		};
		// Amend DLL search path - see http://forum.unity3d.com/threads/dllnotfoundexception-when-depend-on-another-dll.31083/#post-1042180
		// for original inspiration for this code.
		string currentPath = Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Process);
		string[] currentPaths = currentPath.Split (Path.PathSeparator);
		StringBuilder paths = new StringBuilder();
		foreach (string dllPath in dllPaths) {
			String fullDllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + dllPath;
			if (Array.IndexOf(currentPaths, fullDllPath) < 0) {
				paths.AppendFormat ("{1}{0}", Path.PathSeparator, fullDllPath);
			}
		}
		paths.Append(currentPath);
		Environment.SetEnvironmentVariable ("PATH", paths.ToString (), EnvironmentVariableTarget.Process);
	}
	
}
