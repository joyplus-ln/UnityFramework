using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class BatchTextureSettingEditor : CheckerPluginEditor
    {
        public class TextureSettingConfig
        {
#if UNITY_5_5_OR_NEWER
            public TextureImporterType TextureType = TextureImporterType.Default;
            public TextureImporterAlphaSource alphasource = TextureImporterAlphaSource.None;
            public TextureImporterCompression mCompression = TextureImporterCompression.Compressed;
            public bool sRGB = true;
#else
            public TextureImporterType TextureType = TextureImporterType.Image;
            public bool alphaFromGrayScale = false;
            public bool alphaIsTransparency = false;
#endif
            public bool ReadAndWrite = false;
            public bool Mipmaps = false;
            public bool BorderMipMaps = false;
            public bool FadeoutMipMaps = false;
            public FilterMode mFilterMode = FilterMode.Bilinear;
            public TextureWrapMode mTextureWrapMode = TextureWrapMode.Clamp;
            public TextureImporterMipFilter mTextureImporterMipFilter = TextureImporterMipFilter.BoxFilter;
            public int AnisoLevel = 1;
            //public TextureImporterFormat ImporterFormat = TextureImporterFormat.AutomaticCompressed;
            public int MaxSizeInt = 1024;
        }

        TextureSettingConfig textureconfig = new TextureSettingConfig();
        public bool bTextureType = false;
#if UNITY_5_5_OR_NEWER
        public bool bAlphaSource = false;
        public bool bCompression = false;
        public bool bsRGB = false;
#else
        public bool bAlphaFromGray = false;
        public bool bAlphaIsTransparency = false;
#endif
        public bool bReadAndWrite = false;
        public bool bMipmaps = false;
        public bool bBorderMipMaps = false;
        public bool bFadeoutMipMaps = false;
        public bool bFilterMode = false;
        public bool bTextureWrapMode = false;
        public bool bTextureImporterMipFilter = false;
        public bool bAnisoLevel = false;
        public bool bMaxSizeInt = false;

        private int[] IntArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        private string[] MaxSizeString = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };
        private int MaxSizeIndex = 5;

        public static void Init(List<Object> objects)
        {
            GetWindow<BatchTextureSettingEditor>();
            objectList = objects;
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            textureconfig.TextureType = (TextureImporterType)EditorGUILayout.EnumPopup("TextureType", textureconfig.TextureType);
            bTextureType = GUILayout.Toggle(bTextureType, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.mFilterMode = (FilterMode)EditorGUILayout.EnumPopup("FilterMode", textureconfig.mFilterMode);
            bFilterMode = GUILayout.Toggle(bFilterMode, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.mTextureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("WrapMode", textureconfig.mTextureWrapMode);
            bTextureWrapMode = GUILayout.Toggle(bTextureWrapMode, "");
            GUILayout.EndHorizontal();
#if UNITY_5_5_OR_NEWER
            GUILayout.BeginHorizontal();
            textureconfig.mCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", textureconfig.mCompression);
            bCompression = GUILayout.Toggle(bCompression, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.alphasource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup("AlphaSource", textureconfig.alphasource);
            bAlphaSource = GUILayout.Toggle(bAlphaSource, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.sRGB = GUILayout.Toggle(textureconfig.sRGB, "sRGB");
            bsRGB = GUILayout.Toggle(bsRGB, "");
            GUILayout.EndHorizontal();
#else
            GUILayout.BeginHorizontal();
            textureconfig.alphaFromGrayScale = GUILayout.Toggle(textureconfig.alphaFromGrayScale, "Alpha From GrayScale");
            bAlphaFromGray = GUILayout.Toggle(bAlphaFromGray, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.alphaIsTransparency = GUILayout.Toggle(textureconfig.alphaIsTransparency, "AlphaIsTransparency");
            bAlphaIsTransparency = GUILayout.Toggle(bAlphaIsTransparency, "");
            GUILayout.EndHorizontal();
#endif

            GUILayout.BeginHorizontal();
            textureconfig.ReadAndWrite = GUILayout.Toggle(textureconfig.ReadAndWrite, "Read/Write Enable");
            bReadAndWrite = GUILayout.Toggle(bReadAndWrite, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            textureconfig.Mipmaps = GUILayout.Toggle(textureconfig.Mipmaps, "GenMipMap");
            bMipmaps = GUILayout.Toggle(bMipmaps, "");
            GUILayout.EndHorizontal();

            if (textureconfig.Mipmaps)
            {
                GUILayout.BeginHorizontal();
                textureconfig.BorderMipMaps = GUILayout.Toggle(textureconfig.BorderMipMaps, "Border Mip Maps");
                bBorderMipMaps = GUILayout.Toggle(bBorderMipMaps, "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                textureconfig.mTextureImporterMipFilter = (TextureImporterMipFilter)EditorGUILayout.EnumPopup("Mip Map Filtering", textureconfig.mTextureImporterMipFilter);
                bTextureImporterMipFilter = GUILayout.Toggle(bTextureImporterMipFilter, "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                textureconfig.FadeoutMipMaps = GUILayout.Toggle(textureconfig.FadeoutMipMaps, "In Linear Space");
                bFadeoutMipMaps = GUILayout.Toggle(bFadeoutMipMaps, "");
                GUILayout.EndHorizontal();

            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Aniso Level ");
            textureconfig.AnisoLevel = EditorGUILayout.IntSlider(textureconfig.AnisoLevel, 0, 9);
            bAnisoLevel = GUILayout.Toggle(bAnisoLevel, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            MaxSizeIndex = EditorGUILayout.IntPopup("Max Size ", MaxSizeIndex, MaxSizeString, IntArray);
            int.TryParse(MaxSizeString[MaxSizeIndex], out textureconfig.MaxSizeInt);
            Debug.Log(textureconfig.MaxSizeInt);
            bMaxSizeInt = GUILayout.Toggle(bMaxSizeInt, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Texture Batch"))
            {
                SetTextureBatch();
            }
            GUILayout.EndHorizontal();
            ShowList();
        }

        private void SetTextureBatch()
        {
            if (objectList == null || objectList.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "当前无选中内容", "OK");
                return;
            }
            foreach (Object t in objectList)
            {
                Texture2D tex = t as Texture2D;
                if (tex == null)
                    continue;
                SetTextrueConfig(tex);
            }
        }

        private void SetTextrueConfig(Texture texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter texImporter = TextureImporter.GetAtPath(path) as TextureImporter;
            if (texImporter == null)
                return;

            if (bTextureType)
                texImporter.textureType = textureconfig.TextureType;
            if (bReadAndWrite)
                texImporter.isReadable = textureconfig.ReadAndWrite;
            if (bMipmaps)
                texImporter.mipmapEnabled = textureconfig.Mipmaps;
#if UNITY_5_5_OR_NEWER
            if (bAlphaSource)
                texImporter.alphaSource = textureconfig.alphasource;
            if (bsRGB)
                texImporter.sRGBTexture = textureconfig.sRGB;
            if (bCompression)
                texImporter.textureCompression = textureconfig.mCompression;
#else
            if (bAlphaIsTransparency)
                texImporter.alphaIsTransparency = textureconfig.alphaIsTransparency;
            if (bAlphaFromGray)
                texImporter.grayscaleToAlpha = textureconfig.alphaFromGrayScale;
#endif
            if (bBorderMipMaps)
                texImporter.borderMipmap = textureconfig.BorderMipMaps;
            if (bFadeoutMipMaps)
                texImporter.fadeout = textureconfig.FadeoutMipMaps;
            if (bFilterMode)
                texImporter.filterMode = textureconfig.mFilterMode;
            if (bTextureWrapMode)
                texImporter.wrapMode = textureconfig.mTextureWrapMode;
            if (bTextureImporterMipFilter)
                texImporter.mipmapFilter = textureconfig.mTextureImporterMipFilter;
            if (bAnisoLevel)
                texImporter.anisoLevel = textureconfig.AnisoLevel;
            if (bMaxSizeInt)
                texImporter.maxTextureSize = textureconfig.MaxSizeInt;

            texImporter.SaveAndReimport();
        }
    }
}