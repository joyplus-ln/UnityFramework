using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks; 
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using System.Xml;
using com.forads.sdk.android;
using com.forads.sdk.ios;
using UnityEditor.Build.Reporting;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class FOREditor : Editor,IPreprocessBuildWithReport
{

#if UNITY_IOS
    
#endif
#if UNITY_ANDROID
    private string androidPluginsPath;
    private string FORADSManifestPath;
    private string appManifestPath;
    private string configPathAndroid;
#endif
#if UNITY_IOS
    private static string jsonOutPathIos;
    private static string jsonOutPathIos2;
    private static string skinOutPathIos;
    private static string FtAdsPlatformPathIos;
    private static string xcodeFtAdsPlatformPathIos;
    private static string platformlibsPathIos2;
    private static string configPathIos;
    private static string configPathIos2;
    private static string skinConfigPathIos;

#endif

    private void OnEnable()
    {
    #if UNITY_ANDROID
       androidPluginsPath = Path.Combine(Application.dataPath, "Plugins/Android");
       FORADSManifestPath = Path.Combine(Application.dataPath, "FORADS/Base/Android/FORADSAndroidManifest.xml");
       appManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
       configPathAndroid = Path.Combine(Application.dataPath, "Plugins/Android/FORAndroidConfig.json");
    #endif
    #if UNITY_IOS
       jsonOutPathIos = "/Libraries/FORADS/Base/Ios/FORIosConfig.json";
       jsonOutPathIos2 = "/Libraries/FORADS/Base/Ios/forads_ios.json";
       skinOutPathIos = "/Libraries/FORADS/Base/Ios/fat_skin.bundle";
       FtAdsPlatformPathIos = "Assets/FORADS/Base/Ios/FtAdsPlatform.framework";
       xcodeFtAdsPlatformPathIos = "Libraries/FORADS/Base/Ios/FtAdsPlatform.framework";
       platformlibsPathIos2 = "FORADS/Platformlibs/Ios";
       configPathIos = Path.Combine(Application.dataPath, "Plugins/IOS/FORIosConfig.json");
       configPathIos2 = Path.Combine(Application.dataPath, "Plugins/IOS/forads_ios.json");
       skinConfigPathIos = Path.Combine(Application.dataPath, "FORADS/Platformlibs/Ios/Arlington/fat_skin.bundle");
    
    #endif
    }

    public int callbackOrder
    {
        get { return 0; }
    }


    [PostProcessBuild]
    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_ANDROID
        RunPostProcessTasksAndroid();
#endif
        
#if UNITY_IOS
        updateXcode(report);
#endif
    }
    
#if UNITY_ANDROID
    private  void RunPostProcessTasksAndroid()
    {
        bool isFORADSManifestUsed = false;
      

        // Check if user has already created AndroidManifest.xml file in its location.
        // If not, use already predefined FORADSAndroidManifest.xml as default one.
        if (!File.Exists(appManifestPath))
        {
            if (!Directory.Exists(androidPluginsPath))
            {
                Directory.CreateDirectory(androidPluginsPath);
            }

            File.Copy(FORADSManifestPath, appManifestPath);

            UnityEngine.Debug.Log("FORADS: User defined AndroidManifest.xml file not found in Plugins/Android folder.");
            UnityEngine.Debug.Log("FORADS: Creating default app's AndroidManifest.xml from FORADSAndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FORADS: User defined AndroidManifest.xml file located in Plugins/Android folder.");
        }

        // If FORADS manifest is used, we have already set up everything in it so that 
        // our native Android SDK can be used properly.
        if (!isFORADSManifestUsed)
        {
            // However, if you already had your own AndroidManifest.xml, we'll now run
            // some checks on it and tweak it a bit if needed to add some stuff which
            // our native Android SDK needs so that it can run properly.

            // Let's open the app's AndroidManifest.xml file.
            XmlDocument manifestFile = new XmlDocument();
            manifestFile.Load(appManifestPath);
            XmlElement manifestRoot = manifestFile.DocumentElement;
            XmlNode applicationNode = null;

            // Let's find the application node.
            foreach (XmlNode node in manifestRoot.ChildNodes)
            {
                if (node.Name == "application")
                {
                    applicationNode = node;
                    break;
                }
            }
            // If there's no applicatio node, something is really wrong with your AndroidManifest.xml.
            if (applicationNode == null)
            {
                UnityEngine.Debug.LogError("FORADS: Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
                return;
            }

            // Add needed permissions if they are missing.
            AddPermissions(manifestFile);

            // Add intent filter to main activity if it is missing.
            AddMetaData(applicationNode,manifestFile);

            AddhardwareAccelerated(applicationNode);

            resetMainActivity(applicationNode);
            // Save the changes.
            manifestFile.Save(appManifestPath);

            // Clean the manifest file.
            CleanManifestFile(appManifestPath);

            UnityEngine.Debug.Log("FORADS: App's AndroidManifest.xml file check and potential modification completed.");
            UnityEngine.Debug.Log("FORADS: Please check if any error message was displayed during this process "
                + "and make sure to fix all issues in order to properly use the FORADS SDK in your app.");
        }
    }
    private static bool removeMeta(XmlNode applicationNode, String name)
    {
        if("admob_applicationid" == name)
        {
            name = "com.google.android.gms.ads.APPLICATION_ID";
        }else if("applovin_key" == name)
        {
            name = "applovin.sdk.key";
        }
        // Check if permissions are already there.
        for (int i = applicationNode.ChildNodes.Count - 1; i >= 0; i--)
        {
            XmlNode node = applicationNode.ChildNodes[i];
            if (node.Name == "meta-data")
            {
                for (int a = node.Attributes.Count - 1; a >= 0; a--)
                {
                    XmlAttribute attribute = node.Attributes[a];
                    if (attribute.Value.Contains(name))
                    {
                        applicationNode.RemoveChild(node);
                        continue;
                    }
                }
              
            }

        }
        return false;
    }
    private static bool resetMainActivity(XmlNode applicationNode)
    {
        // Check if permissions are already there.
        for (int i = applicationNode.ChildNodes.Count - 1; i >= 0; i--)
        {
            XmlNode node = applicationNode.ChildNodes[i];
            if (node.Name == "activity")
            {
                for (int a = node.Attributes.Count - 1; a >= 0; a--)
                {
                    XmlAttribute attribute = node.Attributes[a];
                    if (attribute.Value.Contains("com.unity3d.player.UnityPlayerActivity"))
                    {
                        attribute.Value = "com.forads.www.UnityActivity.ForUnityActivity";
                        return true;
                    }
                    if (attribute.Value.Contains("com.forads.www.UnityActivity.ForUnityActivity"))
                    {
                        return true;
                    }
                }
            }
        }
        EditorUtility.DisplayDialog("FORADS", "检测到游戏没有使用unity默认的MainActivity：UnityPlayerActivity\n如果游戏使用自己实现的MainAcitivity，请确保你的MainActivity继承自FORADS SDK的ForUnityActivity.否则将会影响Banner广告类型的展示.", "OK");
        return false;
    }
    private static bool AddhardwareAccelerated(XmlNode applicationNode)
    {
        String name = "android__hardwareAccelerated";
        // Check if permissions are already there.
        for (int i = applicationNode.Attributes.Count - 1; i >= 0; i--)
        {
            XmlAttribute attribute = applicationNode.Attributes[i];
            if (attribute.Name.Contains("hardwareAccelerated"))
            {
                applicationNode.Attributes.RemoveNamedItem(attribute.Name);
            }
        }
       applicationNode.Attributes.Append(CreateAttribute(applicationNode,name,"true"));
        return false;
    }
    static private XmlAttribute CreateAttribute(XmlNode node, string attributeName, string value)
    {
        try
        {
            XmlDocument doc = node.OwnerDocument;
            XmlAttribute attr = null;
            attr = doc.CreateAttribute(attributeName);
            attr.Value = value;
            node.Attributes.SetNamedItem(attr);
            return attr;
        }
        catch (Exception err)
        {
            string desc = err.Message;
            return null;
        }
    }
    private static void AddPermissions(XmlDocument manifest)
    {
        // The FORADS SDK needs two permissions to be added to you app's manifest file:
        // <uses-permission android:name="android.permission.INTERNET" />
        // <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
        // <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

        UnityEngine.Debug.Log("FORADS: Checking if all permissions needed for the FORADS SDK are present in the app's AndroidManifest.xml file.");

        bool hasInternetPermission = false;
        bool hasAccessWifiStatePermission = false;
        bool hasAccessNetworkStatePermission = false;

        XmlElement manifestRoot = manifest.DocumentElement;

        // Check if permissions are already there.
        foreach (XmlNode node in manifestRoot.ChildNodes)
        {
            if (node.Name == "uses-permission")
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Value.Contains("android.permission.INTERNET"))
                    {
                        hasInternetPermission = true;
                    }
                    else if (attribute.Value.Contains("android.permission.ACCESS_WIFI_STATE"))
                    {
                        hasAccessWifiStatePermission = true;
                    }
                    else if (attribute.Value.Contains("android.permission.ACCESS_NETWORK_STATE"))
                    {
                        hasAccessNetworkStatePermission = true;
                    }
                  
                }
            }
        }

        // If android.permission.INTERNET permission is missing, add it.
        if (!hasInternetPermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.INTERNET");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FORADS: android.permission.INTERNET permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FORADS: Your app's AndroidManifest.xml file already contains android.permission.INTERNET permission.");
        }

        // If android.permission.ACCESS_WIFI_STATE permission is missing, add it.
        if (!hasAccessWifiStatePermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.ACCESS_WIFI_STATE");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FORADS: android.permission.ACCESS_WIFI_STATE permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FORADS: Your app's AndroidManifest.xml file already contains android.permission.ACCESS_WIFI_STATE permission.");
        }

        // If android.permission.ACCESS_NETWORK_STATE permission is missing, add it.
        if (!hasAccessNetworkStatePermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.ACCESS_NETWORK_STATE");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FORADS: android.permission.ACCESS_NETWORK_STATE permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FORADS: Your app's AndroidManifest.xml file already contains android.permission.ACCESS_NETWORK_STATE permission.");
        }

    }

    private  void AddMetaData(XmlNode applicationNode,XmlDocument manifest)
    {


        // Okay, there's an application node in the AndroidManifest.xml file.
        // Let's now check if user has already defined a meta-data which is com.google.android.gms.ads.APPLICATION_ID.
        // If that is already defined, don't force the FORADS  meta-data to the manifest file.

        
            //读取json文件
            UnityEngine.Debug.Log("FORADS: Android SDK json path ： " + configPathAndroid);

            string jsonString = File.ReadAllText(configPathAndroid);
            AndroidConfigBean platFormBean = JsonUtility.FromJson<AndroidConfigBean>(jsonString);
            if (platFormBean == null)
            {
                Debug.Log("FORADS: 严重错误： 解析配置文件失败");
            }

            //添加Admob的配置文件
            if (platFormBean != null)
            {
                Type t = platFormBean.GetType();
                PropertyInfo[] PropertyList = t.GetProperties();
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name.ToLower();
                    object value = item.GetValue(platFormBean, null);
                    removeMeta(applicationNode, name);
                    XmlElement element = manifest.CreateElement("meta-data");
                    element.SetAttribute("android__name", name);
                    element.SetAttribute("android__value", value+"");
                    applicationNode.AppendChild(element);
                    UnityEngine.Debug.Log("FORADS: meta-data: name="+ name);
                    UnityEngine.Debug.Log("FORADS: meta-data:value=" + value);
               
                }
            }
    }


    private static void CleanManifestFile(String manifestPath)
    {
        // Due to XML writing issue with XmlElement methods which are unable
        // to write "android:[param]" string, we have wrote "android__[param]" string instead.
        // Now make the replacement: "android:[param]" -> "android__[param]"

        TextReader manifestReader = new StreamReader(manifestPath);
        string manifestContent = manifestReader.ReadToEnd();
        manifestReader.Close();

        Regex regex = new Regex("android__");
        manifestContent = regex.Replace(manifestContent, "android:");

        Regex regex1 = new Regex("admob_applicationid");
        manifestContent = regex1.Replace(manifestContent, "com.google.android.gms.ads.APPLICATION_ID");

        Regex regex2 = new Regex("applovin_key");
        manifestContent = regex2.Replace(manifestContent, "applovin.sdk.key");

        TextWriter manifestWriter = new StreamWriter(manifestPath);
        manifestWriter.Write(manifestContent);
        manifestWriter.Close();
    }
#endif

#if UNITY_IOS
    private static void updateXcode(BuildReport report)
    {
       
        UnityEngine.Debug.Log("FORADS: IOS SDK json path ： " + configPathIos);

        //1.添加库、修改配置
        addLibAndUpdate(FrameWorkConst.BuildOutPutPath, configPathIos);

        //2、修改Info.plist文件
        updatePlist(FrameWorkConst.BuildOutPutPath, configPathIos);
    }

    private static void addLibAndUpdate(string path, string jsonPath)
    {
        //这里是固定的
        string projPath = PBXProject.GetPBXProjectPath(path);
        PBXProject proj = new PBXProject();
//        proj.ReadFromString(File.ReadAllText(projPath));
        proj.ReadFromFile(projPath);
        string target = proj.TargetGuidByName("Unity-iPhone");

        //todo:这种写死目录的方法不是很灵活,万一sdk文件目录有变动还会出问题,临时这样弄,在想办法
        //检测有没有copy json文件的目录,没有就创建
        string copyJsonFileToDirectory = path + "/Libraries/FORADS/Base/Ios";
        if (!Directory.Exists(copyJsonFileToDirectory))
        {
            Directory.CreateDirectory(copyJsonFileToDirectory);
            Debug.Log("FORADS:创建[" + copyJsonFileToDirectory + "]目录成功");
        }

        //获取打包打包成xcode的路径
        string xcodejsonOutPathIos = path + jsonOutPathIos;
        UnityEngine.Debug.Log("FORADS: IOS SDK json copy to path ： " + xcodejsonOutPathIos);

        //把json文件拷贝到xcode工程里
        File.Copy(jsonPath, xcodejsonOutPathIos, true);
        Debug.Log("FORADS: 配置文件复制成功 " + xcodejsonOutPathIos);

        //把文件copy到xcode引用中
        string fileGuid = proj.AddFile(xcodejsonOutPathIos, jsonOutPathIos, PBXSourceTree.Source);
        proj.AddFileToBuild(target, fileGuid);



        //forads_ios
        //获取打包打包成xcode的路径
        string xcodejsonOutPathIos2 = path + jsonOutPathIos2;
        UnityEngine.Debug.Log("FORADS: IOS SDK 广告配置 json copy to path ： " + xcodejsonOutPathIos2);


        //把json文件拷贝到xcode工程里
        File.Copy(configPathIos2, xcodejsonOutPathIos2, true);
        Debug.Log("FORADS: 配置文件复制成功 " + xcodejsonOutPathIos2);
        
        //把文件copy到xcode引用中
        string fileGuid3 = proj.AddFile(xcodejsonOutPathIos2, jsonOutPathIos2, PBXSourceTree.Source);
        proj.AddFileToBuild(target, fileGuid3);
        
//skin.bundle 文件暂时不copy到xcode工程里
//        string xcodeBundleOutPathIos2 = path + skinOutPathIos;
//        //把skin.bundle文件拷贝到xcode工程里 
//        CopyAndReplaceDirectory(skinConfigPathIos, xcodeBundleOutPathIos2);
//        Debug.Log("FORADS: skin.bunlde配置文件复制成功 " + xcodeBundleOutPathIos2);
//        //把文件copy到xcode引用中
//        string fileGuid2 = proj.AddFile(xcodeBundleOutPathIos2, skinOutPathIos, PBXSourceTree.Source);
//        proj.AddFileToBuild(target, fileGuid2);

        // 添加ios自己的框架 
//        CopyAndReplaceDirectory(FtAdsPlatformPathIos, Path.Combine(path, xcodeFtAdsPlatformPathIos));
//
//        proj.AddFileToBuild(target, proj.AddFile(xcodeFtAdsPlatformPathIos, xcodeFtAdsPlatformPathIos, PBXSourceTree.Source));
//        UnityEngine.Debug.Log("FORADS: FAT_ads.framework added successfully.");


        //1、修改设置
        //添加系统库 
        proj.AddFrameworkToProject(target, "libc++.tbd", true);
        Debug.Log("FORADS: 添加libc++.tbd 成功");
        proj.AddFrameworkToProject(target, "CoreMotion.framework", true);
        Debug.Log("FORADS: 添加CoreMotion.framework 成功");
        proj.AddFrameworkToProject(target, "libxml2.tbd", true);
        Debug.Log("FORADS: 添加libxml2.tbd 成功");
        proj.AddFrameworkToProject(target, "CoreMedia.framework", true);
        Debug.Log("FORADS: 添加CoreMedia.framework 成功");
        proj.AddFrameworkToProject(target, "StoreKit.framework", true);
        Debug.Log("FORADS: 添加StoreKit.framework 成功");
        proj.AddFrameworkToProject(target, "AVFoundation.framework", true);
        Debug.Log("FORADS: 添加AVFoundation.framework 成功");
        proj.AddFrameworkToProject(target, "AdSupport.framework", true);
        Debug.Log("FORADS: 添加AdSupport.framework 成功");
        proj.AddFrameworkToProject(target, "iAd.framework", true);
        Debug.Log("FORADS: 添加iAd.framework 成功");
        proj.AddFrameworkToProject(target, "CoreTelephony.framework", true);
        Debug.Log("FORADS: 添加CoreTelephony.framework 成功");
     
        proj.AddFrameworkToProject(target, "AudioToolbox.framework", true);
        Debug.Log("FORADS: 添加AudioToolbox.framework 成功");
        proj.AddFrameworkToProject(target, "CFNetwork.framework", true);
        Debug.Log("FORADS: 添加CFNetwork.framework 成功");
        proj.AddFrameworkToProject(target, "CoreGraphics.framework", true);
        Debug.Log("FORADS: 添加CoreGraphics.framework 成功");
        proj.AddFrameworkToProject(target, "libz.tbd", true);
        Debug.Log("FORADS: 添加libz.tbd 成功");
        proj.AddFrameworkToProject(target, "MediaPlayer.framework", true);
        Debug.Log("FORADS: 添加MediaPlayer.framework 成功");
        proj.AddFrameworkToProject(target, "MediaPlayer.framework", true);
        Debug.Log("FORADS: 添加MediaPlayer.framework 成功");
        proj.AddFrameworkToProject(target, "QuartzCore.framework", true);
        Debug.Log("FORADS: 添加QuartzCore.framework 成功");
        proj.AddFrameworkToProject(target, "SystemConfiguration.framework", true);
        Debug.Log("FORADS: 添加SystemConfiguration.framework 成功");
        proj.AddFrameworkToProject(target, "UIKit.framework", true);
        Debug.Log("FORADS: 添加UIKit.framework 成功");
        proj.AddFrameworkToProject(target, "WebKit.framework", true);
        Debug.Log("FORADS: 添加WebKit.framework 成功");
        proj.AddFrameworkToProject(target, "Foundation.framework", true);
        Debug.Log("FORADS: 添加Foundation.framework 成功");
        proj.AddFrameworkToProject(target, "CFNetwork.framework", true);
        Debug.Log("FORADS: 添加CFNetwork.framework 成功");
        proj.AddFrameworkToProject(target, "JavaScriptCore.framework", true);
        Debug.Log("FORADS: 添加JavaScriptCore.framework 成功");
        proj.AddFrameworkToProject(target, "libsqlite3.tbd", true);
        Debug.Log("FORADS: 添加libsqlite3.tbd 成功");
        proj.AddFrameworkToProject(target, "Security.framework", true);
        Debug.Log("FORADS: 添加Security.framework 成功");
        proj.AddFrameworkToProject(target, "CoreText.framework", true);
        Debug.Log("FORADS: 添加CoreText.framework 成功");
        proj.AddFrameworkToProject(target, "libz.1.2.5.tbd", true);
        Debug.Log("FORADS: 添加libz.1.2.5.tbd 成功");
        proj.AddFrameworkToProject(target, "MessageUI.framework", true);
        Debug.Log("FORADS: MessageUI.framework 成功");
        proj.AddFrameworkToProject(target, "MobileCoreServices.framework", true);
        Debug.Log("FORADS: MobileCoreServices.framework 成功");
        proj.AddFrameworkToProject(target, "Social.framework", true);
        Debug.Log("FORADS: Social.framework 成功");
        proj.AddFrameworkToProject(target, "WatchConnectivity.framework", true);
        Debug.Log("FORADS: WatchConnectivity.framework 成功"); 
        proj.AddFrameworkToProject(target, "CoreFoundation.framework", true);
        Debug.Log("FORADS: CoreFoundation.framework 成功"); 
        proj.AddFrameworkToProject(target, "SafariServices.framework", true);
        Debug.Log("FORADS: SafariServices.framework 成功"); 
        proj.AddFrameworkToProject(target, "libz.tbd", true);
        Debug.Log("FORADS: libz.tbd 成功"); 
        proj.AddFrameworkToProject(target, "VideoToolbox.framework", true);
        Debug.Log("FORADS: VideoToolbox.framework 成功"); 

        //修改属性
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        Debug.Log("FORADS: BitCode 设置为 NO");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
        Debug.Log("FORADS: other linker flags 添加 -ObjC");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-fobjc-arc");
        Debug.Log("FORADS: other linker flags 添加 -fobjc-arc");

        proj.SetBuildProperty(target,"USYM_UPLOAD_AUTH_TOKEN","FakeToken");
        //保存
//        File.WriteAllText(projPath, proj.WriteToString());
        proj.WriteToFile(projPath);  
    }

    //修改Info.plist文件
    private static void updatePlist(string path,string jsonPath)
    {
        //读取json文件
        string jsonString = File.ReadAllText(jsonPath);
        PlatFormBean platFormBean = JsonUtility.FromJson<PlatFormBean>(jsonString);
        if (platFormBean == null)
        {
            Debug.Log("FORADS: 严重错误： 解析配置文件失败");
        }
        
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
         
        
        //添加Admob的配置文件 
        if (platFormBean != null && platFormBean.adMob != null && platFormBean.adMob.Enable != null && platFormBean.adMob.Enable.Equals("True") && platFormBean.adMob.GADApplicationIdentifier != null)
        {
            Type type = platFormBean.adMob.GetType();
            FieldInfo[] fieldInfos = type.GetFields();
            Debug.Log("FORADS: fieldInfos 长度：" + fieldInfos.Length);
            foreach (var f in fieldInfos)
            {
                //字段名称
                string fieldName = f.Name;
                if (fieldName == null || fieldName.Equals("Enable"))
                {
                    continue;
                }

                //字段类型
                string fieldType = f.FieldType.ToString();
                //字段的值
                string fieldValue = f.GetValue(platFormBean.adMob).ToString();
                plist.root.SetString(fieldName, fieldValue);
                Debug.Log("FORADS: 添加 " + fieldName);
            }
        }
        else
        {
            Debug.Log("FORADS: Admob 配置未添加2 ");
        }

        if (platFormBean != null && platFormBean.appLovin != null && platFormBean.appLovin.Enable != null && platFormBean.appLovin.Enable.Equals("True") && platFormBean.appLovin.AppLovinSdkKey != null)
        {
            Type type = platFormBean.appLovin.GetType();
            FieldInfo[] fieldInfos = type.GetFields();
            Debug.Log("FORADS: fieldInfos 长度：" + fieldInfos.Length);
            foreach (var f in fieldInfos)
            {
                //字段名称
                string fieldName = f.Name;
                if (fieldName == null || fieldName.Equals("Enable"))
                {
                    continue;
                }

                //字段类型
                string fieldType = f.FieldType.ToString();
                //字段的值
                string fieldValue = f.GetValue(platFormBean.appLovin).ToString();
                plist.root.SetString(fieldName, fieldValue);
                Debug.Log("FORADS: 添加 " + fieldName);
            }
        }
        else
        {
            Debug.Log("FORADS: appLovin 配置未添加2 ");
        }
        //添加AdColony需要的配置
        if (platFormBean != null && platFormBean.AdColony != null && platFormBean.AdColony.Enable != null && platFormBean.AdColony.Enable.Equals("True") && platFormBean.AdColony.appID != null)
        {
            PlistElementDict dict = plist.root.AsDict();
            PlistElementArray array = dict.CreateArray("LSApplicationQueriesSchemes");
            array.AddString("fb");
            array.AddString("instagram");
            array.AddString("tumblr");
            array.AddString("twitter");

            if (platFormBean.AdColony.NSPhotoLibraryUsageDescription != null)
            {
                plist.root.SetString("NSPhotoLibraryUsageDescription", platFormBean.AdColony.NSPhotoLibraryUsageDescription);
            }
            if (platFormBean.AdColony.NSCameraUsageDescription != null)
            {
                plist.root.SetString("NSCameraUsageDescription", platFormBean.AdColony.NSCameraUsageDescription);
            }
            if (platFormBean.AdColony.NSMotionUsageDescription != null)
            {
                plist.root.SetString("NSMotionUsageDescription", platFormBean.AdColony.NSMotionUsageDescription);
            }
            if (platFormBean.AdColony.NSPhotoLibraryAddUsageDescription != null)
            {
                plist.root.SetString("NSPhotoLibraryAddUsageDescription", platFormBean.AdColony.NSPhotoLibraryAddUsageDescription);
            }
           
        }
        else
        {
            Debug.Log("FORADS: AdColony 配置未添加2 ");
        }

//        File.WriteAllText(plistPath, plist.WriteToString());
        plist.WriteToFile(plistPath); 
    }
#endif
    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
            Directory.Delete(dstPath);
        if (File.Exists(dstPath))
            File.Delete(dstPath);

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
            File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
    /**
        public void OnPreprocessBuild(BuildReport report)
        {
     #if UNITY_ANDROID

                RunPostProcessTasksAndroid();
#endif
        }
        **/
}