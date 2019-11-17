using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace ResourceCheckerPlus
{
    public class MeshChecker : ObjectChecker
    {
        public class SubMeshData
        {
            public string name;
            public string format;
            public int vertexCount;
            public int tranCount;
            public Object meshObject;
        }

        public class MeshDetail : ObjectDetail
        {
            public MeshDetail(Object obj, MeshChecker checker) : base(obj, checker)
            {

            }

            public override void InitDetailCheckObject(Object obj)
            {
                MeshChecker checker = currentChecker as MeshChecker;
                Mesh mesh = obj as Mesh;
                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                //Mesh的object直接指向FBX根物体
                checkObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                checkMap[checker.previewItem] = AssetPreview.GetMiniThumbnail(checkObject);
                string readable = buildInType;
                string compression = buildInType;
                string tangent = buildInType;
                string optimize = buildInType;
                string normal = buildInType;
                string blendshape = buildInType;
                string animation = buildInType;
                string genCollider = buildInType;
                string keepQuads = buildInType;
                string swapUVs = buildInType;
                string generateLightMapUVs = buildInType;
                float scale = 1.0f;
                if (importer != null)
                {
                    readable = importer.isReadable.ToString();
                    optimize = importer.optimizeMesh.ToString();
                    blendshape = importer.importBlendShapes.ToString();
                    animation = importer.animationType.ToString();
                    normal = importer.importNormals.ToString();
                    tangent = importer.importTangents.ToString();
                    compression = importer.meshCompression.ToString();
                    genCollider = importer.addCollider.ToString();
                    swapUVs = importer.swapUVChannels.ToString();
                    generateLightMapUVs = importer.generateSecondaryUV.ToString();
                    scale = importer.globalScale;
                }
                checkMap.Add(checker.meshSubMeshCount, 0);
                checkMap.Add(checker.meshVertexCount, 0);
                checkMap.Add(checker.meshTrangleCount, 0);
                if (mesh == null && checker.isReloadCheckItem)
                {
                    List<string> oriSubMeshList = subMeshList.Select(x => x.name).ToList();
                    subMeshList.Clear();
                    foreach (var v in EditorUtility.CollectDependencies(new Object[] { obj }))
                    {
                        if (v is Mesh)
                        {
                            mesh = v as Mesh;
                            if (oriSubMeshList.Contains(mesh.name))
                            {
                                AddSubMesh(mesh, checker);
                            }
                        }
                    }
                }
                checkMap.Add(checker.meshFormat, GetMeshFormat(mesh));
                checkMap.Add(checker.meshReadable, readable);
                checkMap.Add(checker.meshImortBlendShaps, blendshape);
                checkMap.Add(checker.meshGenCollider, genCollider);
                checkMap.Add(checker.meshSwapUVs, swapUVs);
                checkMap.Add(checker.meshGenLightMapUVs, generateLightMapUVs);
                checkMap.Add(checker.meshKeepQuads, keepQuads);
                checkMap.Add(checker.meshOptimized, optimize);
                checkMap.Add(checker.meshAnimSetting, animation);
                checkMap.Add(checker.meshCompression, compression);
                checkMap.Add(checker.meshTanSetting, tangent);
                checkMap.Add(checker.meshNormalSetting, normal);
                checkMap.Add(checker.meshScaleFactor, scale);
            }

            public List<SubMeshData> subMeshList = new List<SubMeshData>(1);
            public bool showSubMesh = false;

            public void AddSubMesh(Mesh subMesh, MeshChecker checker)
            {
                foreach (var v in subMeshList)
                {
                    if (v.meshObject == subMesh)
                        return;
                }
                SubMeshData data = new SubMeshData();
                data.meshObject = subMesh;
                data.format = GetMeshFormat(subMesh);
                data.name = subMesh.name;
                data.vertexCount = subMesh.vertexCount;
                data.tranCount = subMesh.triangles.Length / 3;
                subMeshList.Add(data);
                //重算总体顶点以及面数
                checkMap[checker.meshSubMeshCount] = subMeshList.Count;
                checkMap[checker.meshVertexCount] = subMeshList.Sum(x => x.vertexCount);
                checkMap[checker.meshTrangleCount] = subMeshList.Sum(x => x.tranCount);
            }

            #region 辅助函数
            private string GetMeshFormat(Mesh mesh)
            {
                string haveNormal = mesh.normals.Length > 0 ? "Normal" : "";
                string haveUV = mesh.uv.Length > 0 ? "UV" : "";
                string haveTan = mesh.tangents.Length > 0 ? "Tan" : "";
                string haveColor = mesh.colors.Length > 0 ? "Color" : "";
                string haveUV2 = mesh.uv2.Length > 0 ? "UV2" : "";
                string haveUV3 = mesh.uv3.Length > 0 ? "UV3" : "";
                string haveUV4 = mesh.uv4.Length > 0 ? "UV4" : "";
                string haveColor32 = mesh.colors32.Length > 0 ? "Color32" : "";

                string format = haveNormal + haveTan + haveColor + haveUV + haveUV2 + haveUV3 + haveUV4 + haveColor32;
                return format;
            }
            #endregion
        }

        CheckItem meshSubMeshCount;
        CheckItem meshVertexCount;
        CheckItem meshTrangleCount;
        CheckItem meshFormat;
        CheckItem meshReadable;
        CheckItem meshOptimized;
        CheckItem meshGenCollider;
        CheckItem meshKeepQuads;
        CheckItem meshSwapUVs;
        CheckItem meshGenLightMapUVs;
        CheckItem meshNormalSetting;
        CheckItem meshTanSetting;
        CheckItem meshCompression;
        CheckItem meshAnimSetting;
        CheckItem meshScaleFactor;
        CheckItem meshImortBlendShaps;

        public bool checkMeshFilter = true;
        public bool checkSkinnedMeshRenderer = true;
        public bool checkMeshCollider = true;
        public bool checkParticleMesh = true;

        private GUIContent checkMeshFilterContent = new GUIContent("MeshFilter", "检查MeshFliter上引用的Mesh");
        private GUIContent checkSkinnedMeshRendererContent = new GUIContent("SkinnedMeshRenderer", "检查SkinnedMeshRenderer上引用的Mesh");
        private GUIContent checkMeshColliderContent = new GUIContent("MeshCollider", "检查MeshCollider上引用的Mesh");
        private GUIContent checkParticleSystemContent = new GUIContent("ParticleSystem", "检查Particle上引用的Mesh");

        public override void InitCheckItem()
        {
            checkerName = "Model";
            checkerFilter = "t:Model";
            enableReloadCheckItem = true;
            meshSubMeshCount = new CheckItem(this, "子网格数", 80, CheckType.Int, OnButtonSubMeshCountClick);
            meshVertexCount = new CheckItem(this, "顶点数", 80, CheckType.Int);
            meshTrangleCount = new CheckItem(this, "面数", 80, CheckType.Int);
            meshFormat = new CheckItem(this, "格式", 200);
            meshReadable = new CheckItem(this, "Readable");
            meshOptimized = new CheckItem(this, "Optimize");
            meshNormalSetting = new CheckItem(this, "Normals", 80);
            meshTanSetting = new CheckItem(this, "Tangents", 200);
            meshCompression = new CheckItem(this, "Compression");
            meshScaleFactor = new CheckItem(this, "ScaleFactor", 80, CheckType.Float);
            meshImortBlendShaps = new CheckItem(this, "BlendShape");
            meshGenCollider = new CheckItem(this, "GenCollider");
            meshKeepQuads = new CheckItem(this, "KeepQuads");
            meshSwapUVs = new CheckItem(this, "SwapUVs");
            meshGenLightMapUVs = new CheckItem(this, "GenLightMapUV", 120);
            meshAnimSetting = new CheckItem(this, "Anim");
        }

        public override void AddObjectDetail(Object obj, Object refObj, Object detailRefObj)
        {
            Mesh mesh = obj as Mesh;
            if (mesh == null)
                return;
            MeshDetail detail = null;
            foreach (var checker in CheckList)
            {
                if (checker.assetPath == AssetDatabase.GetAssetPath(obj))
                    detail = checker as MeshDetail;
            }
            if (detail == null)
            {
                detail = new MeshDetail(obj, this);
            }
            detail.AddSubMesh(mesh, this);
            detail.AddObjectReference(refObj, detailRefObj);
        }

        public override void AddObjectDetailRef(GameObject rootObj)
        {
            if (checkMeshFilter)
                AddMeshInternal<MeshFilter>(rootObj);
            if (checkSkinnedMeshRenderer)
                AddMeshInternal<SkinnedMeshRenderer>(rootObj);
            if (checkMeshCollider)
                AddMeshInternal<MeshCollider>(rootObj);
            if (checkParticleMesh)
                AddParticleSystemMeshInternal(rootObj);
        }

        private void AddMeshInternal<T>(GameObject rootObj) where T : Component
        {
            Component[] coms = rootObj.GetComponentsInChildren<T>(true);
            if (coms == null || coms.Length == 0)
                return;
            PropertyInfo info = coms[0].GetType().GetProperty("sharedMesh");
            foreach(var v in coms)
            {
                Mesh mesh = info.GetValue(v, null) as Mesh;
                AddObjectDetailByCheckModule(mesh, rootObj, v.gameObject);
            }
        }

        private void AddParticleSystemMeshInternal(GameObject rootObj)
        {
            //Mesh形状发射
            ParticleSystem[] psComs = rootObj.GetComponentsInChildren<ParticleSystem>(true);
            if (psComs == null || psComs.Length == 0)
                return;//木有Ps组件Renderer貌似也不用判断了
            foreach(var ps in psComs)
            {
                Mesh mesh = ps.shape.mesh;
                AddObjectDetailByCheckModule(mesh, rootObj, ps.gameObject);
            }
            //发射的Mesh
            ParticleSystemRenderer[] coms = rootObj.GetComponentsInChildren<ParticleSystemRenderer>(true);
            if (coms == null || coms.Length == 0)
                return;
            foreach(var psRenderer in coms)
            {
#if UNITY_5_5_OR_NEWER
                Mesh[] meshArray = new Mesh[4];
                int count = psRenderer.GetMeshes(meshArray);
                for(int i = 0; i < count; i++)
                {
                    AddObjectDetailByCheckModule(meshArray[i], rootObj, psRenderer.gameObject);
                }
#else
                //5.3木有GetMesh方法...反射貌似也取不到，so...只取一个Mesh吧，一般Particle也只发射一个
                AddObjectDetailByCheckModule(psRenderer.mesh, rootObj, psRenderer.gameObject);
#endif
            }
        }

        private void AddObjectDetailByCheckModule(Object checkObj, Object rootObj, Object refObj)
        {
            if (checkModule is SceneResCheckModule)
                AddObjectDetail(checkObj, refObj, null);
            else if (checkModule is ReferenceResCheckModule)
                AddObjectDetail(checkObj, rootObj, refObj);
        }

        public override void CheckDetailSummary()
        {
            int totalVertices = 0;
            int totalTrangles = 0;
            int totalCount = 0;
            foreach (var detail in FilterList)
            {
                totalCount += detail.foundInReference.Count;
                totalVertices += (int)detail.checkMap[meshVertexCount] * detail.foundInReference.Count;
                totalTrangles += (int)detail.checkMap[meshTrangleCount] * detail.foundInReference.Count;
            }
            checkSummary = FilterList.Count + " meshes(kind) - " + totalCount + " meshes(ref) - " + totalVertices + " verts - " + totalTrangles + " trangles";
        }

        public override void ShowChildDetail(ObjectDetail detail)
        {
            MeshDetail md = detail as MeshDetail;
            if (md.showSubMesh)
            {
                foreach(var child in md.subMeshList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(170);
                    if (GUILayout.Button(child.name, GUILayout.Width(245)))
                    {
                        SelectObject(child.meshObject);
                    }
                    string vertexCount = "" + child.vertexCount;
                    GUILayout.Label(vertexCount, GUILayout.Width(80));
                    string tranCount = "" + child.tranCount;
                    GUILayout.Label(tranCount, GUILayout.Width(80));
                    GUILayout.Label(child.format, GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                }
            }
        }

        public override void BatchSetResConfig()
        {
            BatchMeshSettingEditor.Init(GetBatchOptionList());
        }

        private void OnButtonSubMeshCountClick(ObjectDetail detail)
        {
            MeshDetail md = detail as MeshDetail;
            md.showSubMesh = !md.showSubMesh;
        }

        //直接查找资源采用查Model然后找Model依赖的方式检查目录下的Mesh
        //用AssetDatabase.FindAssets("t:Mesh"， path)方式查找时，如果一个FBX下挂有n个Mesh，
        //会返回n个相同的guid指向同一个子mesh！！！目测是bug....
        public override void DirectResCheck(Object[] selection)
        {
            List<Object> objects = GetAllDirectCheckObjectFromInput(selection, "t:Model");
            if (objects != null && objects.Count > 0)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    Object o = objects[i];
                    EditorUtility.DisplayProgressBar("正在检查" + checkerName + "类型资源", "已完成：" + i + "/" + objects.Count, (float)i / objects.Count);
                    AddMeshDetailFromFBX(o);
                }
                EditorUtility.ClearProgressBar();
            }
        }

        public void AddMeshDetailFromFBX(Object fbx)
        {
            Object[] dependency = EditorUtility.CollectDependencies(new Object[] { fbx });
            foreach (var dep in dependency)
            {
                if (dep is Mesh)
                {
                    AddObjectDetail(dep, null, null);
                }
            }
        }

        public override void ShowOptionButton()
        {
            base.ShowOptionButton();
            if (ShowCustomOpeion())
            {
                checkMeshFilter = GUILayout.Toggle(checkMeshFilter, checkMeshFilterContent);
                checkSkinnedMeshRenderer = GUILayout.Toggle(checkSkinnedMeshRenderer, checkSkinnedMeshRendererContent);
                checkMeshCollider = GUILayout.Toggle(checkMeshCollider, checkMeshColliderContent);
                checkParticleMesh = GUILayout.Toggle(checkParticleMesh, checkParticleSystemContent);
            }
        }

        private bool ShowCustomOpeion()
        {
            if (checkModule is SceneResCheckModule)
            {
                return !(checkModule as SceneResCheckModule).completeRefCheck;
            }
            else if (checkModule is ReferenceResCheckModule)
            {
                return (checkModule as ReferenceResCheckModule).checkPrefabDetailRef;
            }
            else
            {
                return false;
            }
        }
    }
}