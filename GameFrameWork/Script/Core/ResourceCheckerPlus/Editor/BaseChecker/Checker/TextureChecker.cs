using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ResourceCheckerPlus
{
    public class TextureChecker : ObjectChecker
    {
        public class TextureDetail : ObjectDetail
        {
            public TextureDetail(Object obj, TextureChecker checker) : base(obj, checker)
            {
                
            }

            public override void InitDetailCheckObject(Object obj)
            {
                TextureChecker checker = currentChecker as TextureChecker;
                Texture tex = obj as Texture;
                string format = "Special";
                if (tex is Texture2D)
                {
                    Texture2D tex2D = tex as Texture2D;
                    format = tex2D.format + "\n" + tex2D.width + " x " + tex2D.height + " " + tex2D.mipmapCount + "mip ";
                }
                else if (tex is Cubemap)
                {
                    Cubemap texCube = tex as Cubemap;
                    format = texCube.format + "\n" + tex.width + " x " + tex.height + " x6 ";
                }

                TextureImporter texImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
                string mip = buildInType;
                string readable = buildInType;
                string type = buildInType;
                string npotScale = buildInType;
                int anisoLevel = 1;
                int texOriSize = 0;

                string androidOverride = buildInType;
                int androidMaxSize = 0;
                string androidFormat = buildInType;
                string androidCompressQuality = buildInType;

                string iosOverride = buildInType;
                int iosMaxSize = 0;
                string iosFormat = buildInType;
                string iosCompressQuality = buildInType;

#if UNITY_5_5_OR_NEWER
                string alpha = buildInType;
                string compression = buildInType;
#else
                string alphaFromGray = buildInType;
                string alphaIsTran = buildInType;
                string sourceAlpha = buildInType;
#endif
                if (texImporter)
                {
                    mip = texImporter.mipmapEnabled.ToString();
                    readable = texImporter.isReadable.ToString();
                    type = texImporter.textureType.ToString();
                    npotScale = texImporter.npotScale.ToString();
                    anisoLevel = texImporter.anisoLevel;
                    texOriSize = GetOriTextureSize(texImporter);
#if UNITY_5_5_OR_NEWER
                    TextureImporterPlatformSettings androidsettings = texImporter.GetPlatformTextureSettings(platformAndroid);
                    androidOverride = androidsettings.overridden.ToString();
                    androidMaxSize = androidsettings.maxTextureSize;
                    androidFormat = androidsettings.format.ToString();
                    androidCompressQuality = GetCompressionQuality(androidsettings.compressionQuality);

                    TextureImporterPlatformSettings iossettings = texImporter.GetPlatformTextureSettings(platformIOS);
                    iosOverride = iossettings.overridden.ToString();
                    iosMaxSize = iossettings.maxTextureSize;
                    iosFormat = iossettings.format.ToString();
                    iosCompressQuality = GetCompressionQuality(iossettings.compressionQuality);

                    alpha = texImporter.alphaSource.ToString();
                    compression = texImporter.textureCompression.ToString();
#else
                    TextureImporterFormat androidImportFormat;
                    int androidImportCompressionQa;
                    androidOverride = texImporter.GetPlatformTextureSettings(platformAndroid, out androidMaxSize, out androidImportFormat, out androidImportCompressionQa).ToString();
                    androidFormat = androidImportFormat.ToString();
                    androidCompressQuality = GetCompressionQuality(androidImportCompressionQa);

                    TextureImporterFormat iosImportFormat;
                    int iosImportCompressionQa;
                    iosOverride = texImporter.GetPlatformTextureSettings(platformIOS, out iosMaxSize, out iosImportFormat, out iosImportCompressionQa).ToString();
                    iosFormat = iosImportFormat.ToString();
                    iosCompressQuality = GetCompressionQuality(iosImportCompressionQa);

                    alphaFromGray = texImporter.grayscaleToAlpha.ToString();
                    alphaIsTran = texImporter.alphaIsTransparency.ToString();
                    //5.5之前可以用
                    sourceAlpha = texImporter.DoesSourceTextureHaveAlpha().ToString();
#endif
                }
                int memSize = CalculateTextureSizeBytes(tex) / 1024;
                int size = Mathf.Max(tex.width, tex.height);
                bool isSquare = tex.width == tex.height;
                bool isPoworOfTwo = TextureIsPowerOfTwo(tex);
                string postfix = ResourceCheckerHelper.GetAssetPostfix(assetPath);
                checkMap.Add(checker.texFormat, format);
                checkMap.Add(checker.texMipmap, mip);
                checkMap.Add(checker.texReadable, readable);
                checkMap.Add(checker.texType, type);
                checkMap.Add(checker.texMemSize, memSize);
                checkMap.Add(checker.texSize, size);
                checkMap.Add(checker.texPostfix, postfix);
                checkMap.Add(checker.texAnisoLevel, anisoLevel);
                checkMap.Add(checker.texIsSquareMap, isSquare.ToString());
                checkMap.Add(checker.texNonPowerOfTwo, isPoworOfTwo.ToString());
                checkMap.Add(checker.texNpotScale, npotScale);
                checkMap.Add(checker.texWrapMode, tex.wrapMode.ToString());
                checkMap.Add(checker.texFilterMode, tex.filterMode.ToString());
                checkMap.Add(checker.texOriSize, texOriSize);

                checkMap.Add(checker.texAndroidOverride, androidOverride);
                checkMap.Add(checker.texAndroidMaxSize, androidMaxSize);
                checkMap.Add(checker.texAndroidFormat, androidFormat);
                checkMap.Add(checker.texAndroidCompressQuality, androidCompressQuality);

                checkMap.Add(checker.texIOSOverride, iosOverride);
                checkMap.Add(checker.texIOSMaxSize, iosMaxSize);
                checkMap.Add(checker.texIOSFormat, iosFormat);
                checkMap.Add(checker.texIOSCompressQuality, iosCompressQuality);
#if UNITY_5_5_OR_NEWER
                checkMap.Add(checker.texAlpha, alpha);
                checkMap.Add(checker.texCompression, compression);
#else
                checkMap.Add(checker.texAlphaFromGray, alphaFromGray);
                checkMap.Add(checker.texAlphaIsTransparent, alphaIsTran);
                checkMap.Add(checker.texSourceAlpha, sourceAlpha);
#endif
            }

            #region 辅助函数
            int CalculateTextureSizeBytes(Texture tTexture)
            {
                if (tTexture == null)
                    return 0;
                int tWidth = tTexture.width;
                int tHeight = tTexture.height;
                if (tTexture is Texture2D)
                {
                    Texture2D tTex2D = tTexture as Texture2D;
                    int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
                    int mipMapCount = tTex2D.mipmapCount;
                    int mipLevel = 1;
                    int tSize = 0;
                    while (mipLevel <= mipMapCount)
                    {
                        tSize += tWidth * tHeight * bitsPerPixel / 8;
                        tWidth = tWidth / 2;
                        tHeight = tHeight / 2;
                        mipLevel++;
                    }
                    return tSize;
                }

                if (tTexture is Cubemap)
                {
                    Cubemap tCubemap = tTexture as Cubemap;
                    int bitsPerPixel = GetBitsPerPixel(tCubemap.format);
                    return tWidth * tHeight * 6 * bitsPerPixel / 8;
                }
                return 0;
            }

            int GetBitsPerPixel(TextureFormat format)
            {
                switch (format)
                {
                    case TextureFormat.Alpha8: //	 Alpha-only texture format.
                        return 8;
                    case TextureFormat.ARGB4444: //	 A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                        return 16;
                    case TextureFormat.RGBA4444: //	 A 16 bits/pixel texture format.
                        return 16;
                    case TextureFormat.RGB24:   // A color texture format.
                        return 24;
                    case TextureFormat.RGBA32:  //Color with an alpha channel texture format.
                        return 32;
                    case TextureFormat.ARGB32:  //Color with an alpha channel texture format.
                        return 32;
                    case TextureFormat.RGB565:  //	 A 16 bit color texture format.
                        return 16;
                    case TextureFormat.DXT1:    // Compressed color texture format.
                        return 4;
                    case TextureFormat.DXT5:    // Compressed color with alpha channel texture format.
                        return 8;
                    /*
                    case TextureFormat.WiiI4:	// Wii texture format.
                    case TextureFormat.WiiI8:	// Wii texture format. Intensity 8 bit.
                    case TextureFormat.WiiIA4:	// Wii texture format. Intensity + Alpha 8 bit (4 + 4).
                    case TextureFormat.WiiIA8:	// Wii texture format. Intensity + Alpha 16 bit (8 + 8).
                    case TextureFormat.WiiRGB565:	// Wii texture format. RGB 16 bit (565).
                    case TextureFormat.WiiRGB5A3:	// Wii texture format. RGBA 16 bit (4443).
                    case TextureFormat.WiiRGBA8:	// Wii texture format. RGBA 32 bit (8888).
                    case TextureFormat.WiiCMPR:	//	 Compressed Wii texture format. 4 bits/texel, ~RGB8A1 (Outline alpha is not currently supported).
                        return 0;  //Not supported yet
                    */
                    case TextureFormat.PVRTC_RGB2://	 PowerVR (iOS) 2 bits/pixel compressed color texture format.
                        return 2;
                    case TextureFormat.PVRTC_RGBA2://	 PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
                        return 2;
                    case TextureFormat.PVRTC_RGB4://	 PowerVR (iOS) 4 bits/pixel compressed color texture format.
                        return 4;
                    case TextureFormat.PVRTC_RGBA4://	 PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
                        return 4;
                    case TextureFormat.ETC_RGB4://	 ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                        return 4;
                  
                    case TextureFormat.ETC2_RGBA8://	 ATC (ATITC) 8 bits/pixel compressed RGB texture format.
                        return 8;
                    case TextureFormat.BGRA32://	 Format returned by iPhone camera
                        return 32;
                
                    case TextureFormat.ETC2_RGBA1:
                        return 4;
                        //case TextureFormat.ATF_RGB_DXT1://	 Flash-specific RGB DXT1 compressed color texture format.
                        //case TextureFormat.ATF_RGBA_JPG://	 Flash-specific RGBA JPG-compressed color texture format.
                        //case TextureFormat.ATF_RGB_JPG://	 Flash-specific RGB JPG-compressed color texture format.
                        //    return 0; //Not supported yet
                }
                return 0;
            }

            public bool TextureIsPowerOfTwo(Texture tex)
            {
                if (tex == null)
                    return true;
                if (isPowerOfTwo(tex.width) && isPowerOfTwo(tex.height))
                    return true;
                return false;
            }

            private bool isPowerOfTwo(int num)
            {
                return ((num & (num - 1)) == 0);
            }

            private string GetCompressionQuality(int quality)
            {
                if (quality == 50)
                    return "Normal";
                return quality == 0 ? "Fast" : "Best";
            }

            private int GetOriTextureSize(TextureImporter importer)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(importer, args);
                return Mathf.Max((int)args[0], (int)args[1]);
            }

            #endregion
        }

        CheckItem texFormat;
        CheckItem texMipmap;
        CheckItem texReadable;
        CheckItem texType;
        CheckItem texSize;
        CheckItem texMemSize;
        CheckItem texPostfix;
        CheckItem texNonPowerOfTwo;
        CheckItem texNpotScale;
        CheckItem texIsSquareMap;
        CheckItem texFilterMode;
        CheckItem texWrapMode;
        CheckItem texAnisoLevel;
        CheckItem texOriSize;

        CheckItem texAndroidOverride;
        CheckItem texAndroidMaxSize;
        CheckItem texAndroidFormat;
        CheckItem texAndroidCompressQuality;

        CheckItem texIOSOverride;
        CheckItem texIOSMaxSize;
        CheckItem texIOSFormat;
        CheckItem texIOSCompressQuality;
#if UNITY_5_5_OR_NEWER
        CheckItem texAlpha;
        CheckItem texCompression;
#else
        CheckItem texAlphaFromGray;
        CheckItem texAlphaIsTransparent;
         //这个属性，在5.5之后的版本之后不管有没有都是true....暂时只在之前的版本上检查吧
        CheckItem texSourceAlpha;
#endif

        public bool showAndroidPlatformSettings = true;
        public bool showIOSPlatformSetting = true;
        private GUIContent showAndroidSettingContent = new GUIContent("Android", "显示Android平台资源格式设置");
        private GUIContent showIOSSettingContent = new GUIContent("IOS", "显示IOS平台资源格式设置");

        public override void InitCheckItem()
        {
            checkerName = "Texture";
            checkerFilter = "t:Texture";
            enableReloadCheckItem = true;
            texFormat = new CheckItem(this, "格式", 150);
            texMipmap = new CheckItem(this, "Mip");
            texReadable = new CheckItem(this, "Readable");
            texType = new CheckItem(this, "类型", 100);
            texSize = new CheckItem(this, "贴图大小", 100, CheckType.Int, null);
            texMemSize = new CheckItem(this, "内存占用", 100, CheckType.FormatSize, null);
            texPostfix = new CheckItem(this, "后缀名");
#if UNITY_5_5_OR_NEWER
            texAlpha = new CheckItem(this, "Alpha", 100);
            texCompression = new CheckItem(this, "纹理压缩", 100);
#else
            texAlphaFromGray = new CheckItem(this, "AlphaFromGray");
            texAlphaIsTransparent = new CheckItem(this, "AlphaIsTran");
            texSourceAlpha = new CheckItem(this, "原始图片Alpha", 100);
#endif
            texIsSquareMap = new CheckItem(this, "正方形贴图", 100);
            texNonPowerOfTwo = new CheckItem(this, "2次幂贴图", 100);
            texNpotScale = new CheckItem(this, "NonPower Of 2", 100);
            texWrapMode = new CheckItem(this, "WrapMode");
            texFilterMode = new CheckItem(this, "FilterMode");
            texAnisoLevel = new CheckItem(this, "AnisoLevel", 100, CheckType.Int);
            texOriSize = new CheckItem(this, "源图大小", 80, CheckType.Int);
            texAndroidOverride = new CheckItem(this, "安卓开启", 120);
            texAndroidMaxSize = new CheckItem(this, "安卓MaxSize", 120, CheckType.Int);
            texAndroidFormat = new CheckItem(this, "安卓Format", 120);
            texAndroidCompressQuality = new CheckItem(this, "安卓压缩质量", 150);
            texIOSOverride = new CheckItem(this, "IOS开启", 120);
            texIOSMaxSize = new CheckItem(this, "IOSMaxSize", 120, CheckType.Int);
            texIOSFormat = new CheckItem(this, "IOSFormat", 120);
            texIOSCompressQuality = new CheckItem(this, "IOS压缩质量", 150);

            ShowIOS(showIOSPlatformSetting);
            ShowAndroid(showAndroidPlatformSettings);
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            if (obj is Texture)
            {
                ObjectDetail detail = null;
                foreach (var v in CheckList)
                {
                    if (v.checkObject == obj)
                        detail = v;
                }
                if (detail == null)
                {
                    detail = new TextureDetail(obj, this);
                }
                detail.AddObjectReference(refObj, detailRefObj);
            }
        }

        public override void CheckDetailSummary()
        {
            int totalTextureSummary = 0;
            foreach (var texDetail in FilterList)
            {
                totalTextureSummary += (int)texDetail.checkMap[texMemSize];
            }
            checkSummary = FilterList.Count + " " + checkerName + " - " + FormatSizeString(totalTextureSummary);
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    Object[] obj = EditorUtility.CollectDependencies(new Object[] { mat });
                    foreach (var o in obj)
                    {
                        if (checkModule is SceneResCheckModule)
                            AddObjectDetail(o, r.gameObject, null);
                        else if (checkModule is ReferenceResCheckModule)
                            AddObjectDetail(o, rootObj, r.gameObject);
                    }
                }
            }
        }

        public override void BatchSetResConfig()
        {
            BatchTextureSettingEditor.Init(GetBatchOptionList());
        }

        public override void ShowOptionButton()
        {
            base.ShowOptionButton();
            EditorGUI.BeginChangeCheck();
            showAndroidPlatformSettings = GUILayout.Toggle(showAndroidPlatformSettings, showAndroidSettingContent, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                ShowAndroid(showAndroidPlatformSettings);
            }
            EditorGUI.BeginChangeCheck();
            showIOSPlatformSetting = GUILayout.Toggle(showIOSPlatformSetting, showIOSSettingContent, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                ShowIOS(showIOSPlatformSetting);
            }
        }

        private void ShowAndroid(bool show)
        {
            texAndroidOverride.show = show;
            texAndroidMaxSize.show = show;
            texAndroidFormat.show = show;
            texAndroidCompressQuality.show = show;
        }

        private void ShowIOS(bool show)
        {
            texIOSOverride.show = show;
            texIOSMaxSize.show = show;
            texIOSFormat.show = show;
            texIOSCompressQuality.show = show;
        }
    }
}
