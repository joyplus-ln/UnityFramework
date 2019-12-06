using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace FastBundle.Editor
{
    public static class FastBundleMenuItem
	{
		[MenuItem ("FastFramework/Bundle/Copy Asset Path")]
		static void CopyAssetPath ()
		{
			if (EditorApplication.isCompiling) {
				return;
			}
			string path = AssetDatabase.GetAssetPath (Selection.activeInstanceID);   
			GUIUtility.systemCopyBuffer = path;
			Debug.Log (string.Format ("systemCopyBuffer: {0}", path));
		}  

		
		[MenuItem ("FastFramework/Bundle/Build Manifest")]  
		public static void BuildAssetManifestJson ()
		{  
			if (EditorApplication.isCompiling) {
				return;
			}     
			List<AssetBundleBuild> builds = BuildRule.GetBuilds (ConstPath.assetsManifesttxt);
			BuildScript.BuildManifestJson (ConstPath.assetsManifesttxt, builds);
		}  
		
		[MenuItem ("FastFramework/Bundle/ClearAllBundles")]  
		public static void ClearAllBundles ()
		{  
			if (EditorApplication.isCompiling) {
				return;
			}     
			FileTool.DeleteDirectory(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/')) + "/AssetBundles");
			FileTool.DeleteDirectory(Application.streamingAssetsPath + "/AssetBundles");
			AssetDatabase.Refresh();
		}  

		[MenuItem ("FastFramework/Bundle/Build AssetBundles")]  
		public static void BuildAssetBundles ()
		{  
			if (EditorApplication.isCompiling) {
				return;
			}       
			List<AssetBundleBuild> builds = BuildRule.GetBuilds (ConstPath.assetsManifesttxt);
            BuildScript.BuildManifestJson (ConstPath.assetsManifesttxt, builds);
			BuildScript.BuildAssetBundles (builds);
		}  

		[MenuItem ("FastFramework/Bundle/Copy AssetBundles to StreamingAssets")]  
		public static void CopyAssetBundlesToStreamingAssets ()
		{  
			if (EditorApplication.isCompiling) {
				return;
			}        
			BuildScript.CopyAssetBundlesTo (Path.Combine (Application.streamingAssetsPath, AssetBundleUtility.AssetBundlesOutputPath));

            AssetDatabase.Refresh();
		}  

		[MenuItem ("FastFramework/Bundle/AddressablesConfig")]  
		public static void AddressablesFileConfig()
		{  
			AddressablesBundleBuildScript.CreatConfig();
		}
		
		[MenuItem ("FastFramework/Bundle/Addressables")]  
		public static void AddressablesFileSelect()
		{  
			AddressablesBundleBuildScript.AddFileToAddressables();
		}
		
		[MenuItem ("FastFramework/Bundle/AddressablesEditor")]  
		public static void Editor()
		{  
			AddressablesBundleBuildScript.AddFileToAddressablesInDevelop();
		}

	}
}