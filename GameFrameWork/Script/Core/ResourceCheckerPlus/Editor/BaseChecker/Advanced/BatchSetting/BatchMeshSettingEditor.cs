using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceCheckerPlus
{
    public class BatchMeshSettingEditor : CheckerPluginEditor
    {
        public static void Init(List<Object> objects)
        {
            GetWindow(typeof(BatchMeshSettingEditor));
            objectList = objects;
        }

        public class MeshSettingConfig
        {
            public bool readWriteEnable = false;
            public bool optimizeMesh = true;
            public ModelImporterMeshCompression compression = ModelImporterMeshCompression.Off;
            public ModelImporterTangents tangent = ModelImporterTangents.None;
        }

        public MeshSettingConfig meshConfig = new MeshSettingConfig();
        public bool bReadWriteEnable = false;
        public bool bOptimizeMesh = false;
        public bool bCompression = false;
        public bool bTangent = false;

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            meshConfig.readWriteEnable = GUILayout.Toggle(meshConfig.readWriteEnable, "Read/Write Enable");
            bReadWriteEnable = GUILayout.Toggle(bReadWriteEnable, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            meshConfig.optimizeMesh = GUILayout.Toggle(meshConfig.optimizeMesh, "Optimize Mesh");
            bOptimizeMesh = GUILayout.Toggle(bOptimizeMesh, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            meshConfig.compression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Compression", meshConfig.compression);
            bCompression = GUILayout.Toggle(bCompression, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            meshConfig.tangent = (ModelImporterTangents)EditorGUILayout.EnumPopup("Tangent", meshConfig.tangent);
            bTangent = GUILayout.Toggle(bTangent, "");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("SetMesh ConfigBatch"))
            {
                SetMeshConfigBatch();
            }

            ShowList();
        }

        void SetMeshConfig(Mesh mesh)
        {
            string path = AssetDatabase.GetAssetPath(mesh);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
                return;
            if (bReadWriteEnable)
                importer.isReadable = meshConfig.readWriteEnable;
            if (bOptimizeMesh)
                importer.optimizeMesh = meshConfig.optimizeMesh;
            if (bCompression)
                importer.meshCompression = meshConfig.compression;
            if (bTangent)
                importer.importTangents = meshConfig.tangent;
            importer.SaveAndReimport();
        }

        void SetMeshConfigBatch()
        {
            if (objectList == null || objectList.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "当前无选中内容", "OK");
                return;
            }

            foreach (var t in objectList)
            {
                Mesh mesh = t as Mesh;
                if (mesh == null)
                    continue;
                SetMeshConfig(mesh);
            }
        }
    }
}