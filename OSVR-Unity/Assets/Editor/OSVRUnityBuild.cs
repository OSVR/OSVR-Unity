using UnityEditor;
using System.Collections;

public class OSVRUnityBuild {

	static void build() {
		string[] assets = {
			"Assets/OSVRUnity",
			"Assets/Plugins"
		};
		AssetDatabase.ExportPackage(assets,
		                            "OSVR-Unity.unitypackage",
		                            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
		AssetDatabase.ExportPackage("Assets/_scenes/minigame.unity",
		                            "OSVR-Unity-sample.unitypackage",
		                            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
	}
}
