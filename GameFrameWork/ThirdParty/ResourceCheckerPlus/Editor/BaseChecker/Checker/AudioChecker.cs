using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class AudioChecker : ObjectChecker
    {
        public class AudioDetail : ObjectDetail
        {
            public AudioDetail(Object obj, AudioChecker checker) : base(obj, checker)
            {
                
            }

            public override void InitDetailCheckObject(Object obj)
            {
                AudioClip clip = obj as AudioClip;
                AudioChecker checker = currentChecker as AudioChecker;
                string compression = buildInType;
                int quality = 0;
                string sampleRateSetting = buildInType;
                int overrideSampleRate = 0;

                string androidOverride = buildInType;
                string androidLoadType = buildInType;
                string androidCompression = buildInType;
                int androidQuality = 0;
                string androidSampleRateSetting = buildInType;
                int androidSampleRate = 0;

                string iosOverride = buildInType;
                string iosLoadType = buildInType;
                string iosCompression = buildInType;
                int iosQuality = 0;
                string iosSampleRateSetting = buildInType;
                int iosSampleRate = 0;

                AudioImporter importer = AudioImporter.GetAtPath(assetPath) as AudioImporter;
                if (importer != null)
                {
                    compression = importer.defaultSampleSettings.compressionFormat.ToString();
                    quality = Mathf.Clamp((int)(importer.defaultSampleSettings.quality * 100), 1, 100);
                    sampleRateSetting = importer.defaultSampleSettings.sampleRateSetting.ToString();
                    overrideSampleRate = (int)importer.defaultSampleSettings.sampleRateOverride;

                    AudioImporterSampleSettings androidSettings = importer.GetOverrideSampleSettings(platformAndroid);
                    androidOverride = importer.ContainsSampleSettingsOverride(platformAndroid).ToString();
                    androidLoadType = androidSettings.loadType.ToString();
                    androidCompression = androidSettings.compressionFormat.ToString();
                    androidQuality = Mathf.Clamp((int)(androidSettings.quality * 100), 1, 100);
                    androidSampleRateSetting = androidSettings.sampleRateSetting.ToString();
                    androidSampleRate = (int)androidSettings.sampleRateOverride;

                    AudioImporterSampleSettings iosSettings = importer.GetOverrideSampleSettings(platformIOS);
                    iosOverride = importer.ContainsSampleSettingsOverride(platformIOS).ToString();
                    iosLoadType = iosSettings.loadType.ToString();
                    iosCompression = iosSettings.compressionFormat.ToString();
                    iosQuality = Mathf.Clamp((int)(iosSettings.quality * 100), 1, 100);
                    iosSampleRateSetting = iosSettings.sampleRateSetting.ToString();
                    iosSampleRate = (int)iosSettings.sampleRateOverride;

                }
                checkMap.Add(checker.audioLength, clip.length);
                checkMap.Add(checker.audioType, clip.loadType.ToString());
                checkMap.Add(checker.audioChannel, clip.channels);
                checkMap.Add(checker.audioCompression, compression);
                checkMap.Add(checker.audioQuality, quality);
                checkMap.Add(checker.audioSampleRateSetting, sampleRateSetting);
                checkMap.Add(checker.audioSampleRate, overrideSampleRate);
                checkMap.Add(checker.audioPostfix, ResourceCheckerHelper.GetAssetPostfix(assetPath));

                checkMap.Add(checker.audioAndroidOverride, androidOverride);
                checkMap.Add(checker.audioAndroidLoadType, androidLoadType);
                checkMap.Add(checker.audioAndroidCompressionFormat, androidCompression);
                checkMap.Add(checker.audioAndroidQuality, androidQuality);
                checkMap.Add(checker.audioAndroidSampleRateSetting, androidSampleRateSetting);
                checkMap.Add(checker.audioAndroidSampleRate, androidSampleRate);

                checkMap.Add(checker.audioIOSOverride, iosOverride);
                checkMap.Add(checker.audioIOSLoadType, iosLoadType);
                checkMap.Add(checker.audioIOSCompressionFormat, iosCompression);
                checkMap.Add(checker.audioIOSQuality, iosQuality);
                checkMap.Add(checker.audioIOSSampleRateSetting, iosSampleRateSetting);
                checkMap.Add(checker.audioIOSSampleRate, iosSampleRate);
            }
        }

        CheckItem audioLength;
        CheckItem audioType;
        CheckItem audioChannel;
        CheckItem audioCompression;
        CheckItem audioQuality;
        CheckItem audioSampleRate;
        CheckItem audioSampleRateSetting;
        CheckItem audioPostfix;

        CheckItem audioAndroidOverride;
        CheckItem audioAndroidLoadType;
        CheckItem audioAndroidCompressionFormat;
        CheckItem audioAndroidQuality;
        CheckItem audioAndroidSampleRateSetting;
        CheckItem audioAndroidSampleRate;

        CheckItem audioIOSOverride;
        CheckItem audioIOSLoadType;
        CheckItem audioIOSCompressionFormat;
        CheckItem audioIOSQuality;
        CheckItem audioIOSSampleRateSetting;
        CheckItem audioIOSSampleRate;

        public bool showAndroidPlatformSettings = true;
        public bool showIOSPlatformSetting = true;
        private GUIContent showAndroidSettingContent = new GUIContent("Android", "显示Android平台资源格式设置");
        private GUIContent showIOSSettingContent = new GUIContent("IOS", "显示IOS平台资源格式设置");

        public override void InitCheckItem()
        {
            checkerName = "AudioClip";
            checkerFilter = "t:AudioClip";
            enableReloadCheckItem = true;
            audioLength = new CheckItem(this, "长度", 80, CheckType.Float);
            audioChannel = new CheckItem(this, "声道", 80, CheckType.Int);
            audioType = new CheckItem(this, "加载类型", 130, CheckType.String);
            audioCompression = new CheckItem(this, "压缩", 100);
            audioQuality = new CheckItem(this, "质量", 80, CheckType.Int);
            audioSampleRateSetting = new CheckItem(this, "采样率设置", 100);
            audioSampleRate = new CheckItem(this, "自定义采样率", 100, CheckType.Int);
            audioPostfix = new CheckItem(this, "后缀");

            audioAndroidOverride = new CheckItem(this, "安卓开启");
            audioAndroidLoadType = new CheckItem(this, "安卓加载类型", 130);
            audioAndroidCompressionFormat = new CheckItem(this, "安卓压缩", 100);
            audioAndroidQuality = new CheckItem(this, "安卓质量", 80, CheckType.Int);
            audioAndroidSampleRateSetting = new CheckItem(this, "安卓采样率设置", 130);
            audioAndroidSampleRate = new CheckItem(this, "安卓自定义采样率", 130, CheckType.Int);

            audioIOSOverride = new CheckItem(this, "IOS开启");
            audioIOSLoadType = new CheckItem(this, "IOS加载类型", 130);
            audioIOSCompressionFormat = new CheckItem(this, "IOS压缩", 100);
            audioIOSQuality = new CheckItem(this, "IOS质量", 80, CheckType.Int);
            audioIOSSampleRateSetting = new CheckItem(this, "IOS采样率设置", 130);
            audioIOSSampleRate = new CheckItem(this, "IOS自定义采样率", 130, CheckType.Int);

            ShowIOS(showIOSPlatformSetting);
            ShowAndroid(showAndroidPlatformSettings);
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            AudioClip clip = obj as AudioClip;
            if (clip == null)
                return;
            ObjectDetail detail= null;
            //先查缓存
            foreach (var checker in CheckList)
            {
                if (checker.checkObject == obj)
                   detail = checker;
            }
            if (detail == null)
            {
               detail= new AudioDetail(obj, this);
            }
            detail.AddObjectReference(refObj, detailRefObj);
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            AudioSource[] audios = rootObj.GetComponentsInChildren<AudioSource>(true);
            foreach (var audio in audios)
            {
                if (checkModule is SceneResCheckModule)
                    AddObjectDetail(audio.clip, audio.gameObject, null);
                else if (checkModule is ReferenceResCheckModule)
                    AddObjectDetail(audio.clip, rootObj, audio.gameObject);
            }
        }

        public override void BatchSetResConfig()
        {
            BatchAudioClipSettingEditor.Init(GetBatchOptionList());
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
            audioAndroidOverride.show = show;
            audioAndroidLoadType.show = show;
            audioAndroidCompressionFormat.show = show;
            audioAndroidQuality.show = show;
            audioAndroidSampleRateSetting.show = show;
            audioAndroidSampleRate.show = show;
        }

        private void ShowIOS(bool show)
        {
            audioIOSOverride.show = show;
            audioIOSLoadType.show = show;
            audioIOSCompressionFormat.show = show;
            audioIOSQuality.show = show;
            audioIOSSampleRateSetting.show = show;
            audioIOSSampleRate.show = show;
        }
    }
}
