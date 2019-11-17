using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class BatchAudioClipSettingEditor : CheckerPluginEditor
    {
        public enum ResourceTarget
        {
            Default,
            Android,
            IOS,
        }

        public class AudioClipConfig
        {
            public bool forceToMono = true; //强制单声道
            public AudioImporterSampleSettings defaultAudioSettings;
            public bool loadInBackground = false;
        }

        public ResourceTarget target = ResourceTarget.Default;
        public bool bForceToMono = false;
        public bool bLoadInBackground = false;
        public bool bAudioCompressionFormat = false;
        public bool bAudioLoadType = false;
        public bool bQuality = false;
        public bool bSampleRateOverride = false;
        public bool bAudioSampleRateSetting = false;
        public AudioClipConfig audioConfig = new AudioClipConfig();

        private int quality = 1;

        public static void Init(List<Object> objects)
        {
            GetWindow(typeof(BatchAudioClipSettingEditor));
            objectList = objects;
        }

        public void SetAudioClipBatch()
        {
            if (objectList == null || objectList.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "当前无选中内容", "OK");
                return;
            }
            foreach (var t in objectList)
            {
                AudioClip clip = t as AudioClip;
                if (clip == null)
                    continue;
                SetAudioClipConfig(clip);
            }
        }

        public void SetAudioClipConfig(AudioClip clip)
        {
            string path = AssetDatabase.GetAssetPath(clip);
            AudioImporter audioImporter = AudioImporter.GetAtPath(path) as AudioImporter;
            if (audioImporter == null)
                return;
            AudioImporterSampleSettings settings = audioImporter.defaultSampleSettings;
            switch (target)
            {
                case ResourceTarget.Default:
                    settings = audioImporter.defaultSampleSettings;
                    break;
                case ResourceTarget.Android:
                    settings = audioImporter.GetOverrideSampleSettings("Android");
                    break;
                case ResourceTarget.IOS:
                    settings = audioImporter.GetOverrideSampleSettings("IOS");
                    break;
            }

            if (bForceToMono)
                audioImporter.forceToMono = audioConfig.forceToMono;
            if (bLoadInBackground)
                audioImporter.loadInBackground = audioConfig.loadInBackground;
            if (bAudioCompressionFormat)
                settings.compressionFormat = audioConfig.defaultAudioSettings.compressionFormat;
            if (bAudioLoadType)
                settings.loadType = audioConfig.defaultAudioSettings.loadType;
            if (bQuality)
                settings.quality = audioConfig.defaultAudioSettings.quality;
            if (bAudioSampleRateSetting)
                settings.sampleRateSetting = audioConfig.defaultAudioSettings.sampleRateSetting;
            switch (target)
            {
                case ResourceTarget.Default:
                    audioImporter.defaultSampleSettings = settings;
                    break;
                case ResourceTarget.Android:
                    audioImporter.SetOverrideSampleSettings("Android", settings);
                    break;
                case ResourceTarget.IOS:
                    audioImporter.SetOverrideSampleSettings("IOS", settings);
                    break;
            }
            audioImporter.SaveAndReimport();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            audioConfig.forceToMono = GUILayout.Toggle(audioConfig.forceToMono, "Force To Mono");
            bForceToMono = GUILayout.Toggle(bForceToMono, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            audioConfig.loadInBackground = GUILayout.Toggle(audioConfig.loadInBackground, "Load In Background");
            bLoadInBackground = GUILayout.Toggle(bLoadInBackground, "");
            GUILayout.EndHorizontal();

            target = (ResourceTarget)EditorGUILayout.EnumPopup("Target Platform", target);

            GUILayout.BeginHorizontal();
            audioConfig.defaultAudioSettings.compressionFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Compression Format", audioConfig.defaultAudioSettings.compressionFormat);
            bAudioCompressionFormat = GUILayout.Toggle(bAudioCompressionFormat, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Quality:");
            quality = EditorGUILayout.IntSlider(quality, 1, 100);
            audioConfig.defaultAudioSettings.quality = quality / 100.0f;
            bQuality = GUILayout.Toggle(bQuality, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            audioConfig.defaultAudioSettings.loadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("LoadType", audioConfig.defaultAudioSettings.loadType);
            bAudioLoadType = GUILayout.Toggle(bAudioLoadType, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            audioConfig.defaultAudioSettings.sampleRateSetting = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Sample Rate Setting", audioConfig.defaultAudioSettings.sampleRateSetting);
            bAudioSampleRateSetting = GUILayout.Toggle(bAudioSampleRateSetting, "");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("SetAudioBatch"))
            {
                SetAudioClipBatch();
            }
            ShowList();
        }
    }
}
