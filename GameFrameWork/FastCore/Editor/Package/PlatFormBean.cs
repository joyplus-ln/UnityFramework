using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.forads.sdk.ios
{
    [System.Serializable]
    public class AdMob
    {
        public string GADApplicationIdentifier;
        public string Enable;
    }

    [System.Serializable]
    public class Unity
    {
        public string gameID;
        public string Enable;
    }

    [System.Serializable]
    public class IronSource
    {
        public string APPKEY;
        public string Enable;
    }

    [System.Serializable]
    public class Vungle
    {
        public string appID;
        public string Enable;
    }

    [System.Serializable]
    public class AppLovin
    {
        public string AppLovinSdkKey;
        public string Enable;
    }
    [System.Serializable]
    public class AdColony
    {
        public string appID;
        public string NSPhotoLibraryUsageDescription;
        public string NSCameraUsageDescription;
        public string NSMotionUsageDescription;
        public string NSPhotoLibraryAddUsageDescription;
        public string Enable;
    }

    [System.Serializable]
    public class PlatFormBean
    {

        public AdMob adMob;
        public Unity unity;
        public IronSource ironSource;
        public Vungle vungle;
        public AppLovin appLovin;
        public AdColony AdColony;
    }
}