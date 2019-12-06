using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceCheckerPlus
{
    [CreateAssetMenu(fileName ="CheckModuleConfig", menuName = "ResourceCheckerPlus/CheckModuleConfig", order = 999)]
    public class CheckModuleConfig : ScriptableObject
    {
        public CheckerCfg[] checkerCfgs;
        public string CheckModuleTitleName;
        public string CheckModuleDescription;
        public string CheckModuleClassName;
        public int checkModuleOrder;
        public string[] checkRecord;

        public CheckModuleConfig()
        {
            checkModuleOrder = 1;
            CheckModuleTitleName = "自定义";
            CheckModuleDescription = "自定义的检查方式";
            CheckModuleClassName = "CustomCheckModule";
            checkerCfgs = new CheckerCfg[]
            {
                new CheckerCfg("TextureChecker", true),
                new CheckerCfg("MeshChecker", true),
                new CheckerCfg("MaterialChecker", true),
                new CheckerCfg("ComponentChecker", true),
                new CheckerCfg("ShaderChecker"),
                new CheckerCfg("AudioChecker"),
                new CheckerCfg("ParticleChecker"),
                new CheckerCfg("AnimClipChecker"),
                new CheckerCfg("GameObjectChecker"),
                new CheckerCfg("PrefabChecker"),
                new CheckerCfg("RefObjectChecker"),
                new CheckerCfg("SceneChecker"),
                new CheckerCfg("NGUIChecker")
            };
        }
    }
}

