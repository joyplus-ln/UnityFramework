using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenUtils
{
    [MenuItem ("FastFramework/Tools/ScreenShot")]  
    public static void ScreenShot ()
    {
        ScreenCapture.CaptureScreenshot( Application.streamingAssetsPath + "/"  + Time.time+ "_"+ Screen.width + "_" + Screen.height +".png", 0);  
    }
}
