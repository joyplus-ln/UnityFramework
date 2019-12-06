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
		
	}
}