#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor
{
    public class EditorAsset : ScriptableAssetSingleton<EditorAsset>
    {
        [Space]
        public Texture2D play;
        public Texture2D rightArrow;
        public Texture2D leftArrow;

        [Space]
        public Texture2D moveToolPan;
        public Texture2D moveTool3D;
        public Texture2D RotateTool;
        public Texture2D addNodeForward;
        public Texture2D addNodeBack;
        public Texture2D removeNode;


        [MenuItem("Assets/Create/Unity Extensions/Editor or Test/Editor Asset")]
        static void CreateAsset()
        {
            CreateOrSelectAsset();
        }

    } // EditorAsset

} // UnityExtensions.Editor

#endif