using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    [TweenAnimation("Rendering/Material Property", "Material Property")]
    public class TweenMaterialProperty : TweenAnimation
    {
        public enum Type
        {
            Color = 0,
            Vector = 1,
            Float = 2,
            Range = 3
        }


        public Renderer targetRenderer;

        [SerializeField]
        int _materialMask = ~0;

        [SerializeField]
        string _propertyName;

        [SerializeField]
        Type _propertyType;


        [SerializeField]
        Vector4 _from;

        [SerializeField]
        Vector4 _to;


        int _propertyID = -1;


        public string propertyName
        {
            get { return _propertyName; }
        }


        public Type propertyType
        {
            get { return _propertyType; }
        }


        public int propertyID
        {
            get
            {
                if (_propertyID == -1 && !string.IsNullOrEmpty(_propertyName))
                {
                    _propertyID = Shader.PropertyToID(_propertyName);
                }
                return _propertyID;
            }
        }


        public bool allMaterialSelected
        {
            get { return _materialMask == ~0; }
        }


        public bool noneMaterialSelected
        {
            get { return _materialMask == 0; }
        }


        public void SelectAllMaterials()
        {
            _materialMask = ~0;
        }


        public void DeselectAllMaterials()
        {
            _materialMask = 0;
        }


        public void SetProperty(string name, Type type)
        {
            _propertyName = name;
            _propertyType = type;
            _propertyID = -1;
        }


        public bool IsMaterialSelected(int materialIndex)
        {
            return _materialMask.GetBit(materialIndex);
        }


        public void SetMaterialSelected(int materialIndex, bool selected)
        {
            _materialMask.SetBit(materialIndex, selected);
        }


        protected override void OnInterpolate(float factor)
        {
            if (propertyID != -1 && targetRenderer)
            {
                var value = Vector4.LerpUnclamped(_from, _to, factor);

                using (var properties = GeneralKit.materialPropertyBlockPool.GetTemp())
                {
                    if (allMaterialSelected)
                    {
                        targetRenderer.GetPropertyBlock(properties.item);
                        SetPropertyBlockValue();
                        targetRenderer.SetPropertyBlock(properties.item);
                    }
                    else
                    {
                        int materialCount = GetMaterials();

                        for (int i = 0; i < materialCount; i++)
                        {
                            if (IsMaterialSelected(i))
                            {
                                targetRenderer.GetPropertyBlock(properties.item, i);
                                SetPropertyBlockValue();
                                targetRenderer.SetPropertyBlock(properties.item, i);
                            }
                        }
                    }

                    void SetPropertyBlockValue()
                    {
                        switch (_propertyType)
                        {
                            case Type.Color:
                            case Type.Vector:
                                properties.item.SetVector(propertyID, value);
                                return;

                            case Type.Float:
                            case Type.Range:
                                properties.item.SetFloat(propertyID, value.x);
                                return;
                        }
                    }
                }
            }
        }


        static List<Material> _materials = new List<Material>(8);

        int GetMaterials()
        {
            _materials.Clear();
            if (targetRenderer) targetRenderer.GetSharedMaterials(_materials);
            return Mathf.Min(_materials.Count, 32);
        }


#if UNITY_EDITOR

        Renderer _originalTarget;
        List<MaterialPropertyBlock> _tempBlocks;


        public override void Record()
        {
            _originalTarget = targetRenderer;
            if (targetRenderer)
            {
                if (_tempBlocks == null) _tempBlocks = new List<MaterialPropertyBlock>();

                int materialCount = GetMaterials();
                for (int i = 0; i < materialCount; i++)
                {
                    _tempBlocks.Add(GeneralKit.materialPropertyBlockPool.Spawn());
                    targetRenderer.GetPropertyBlock(_tempBlocks[i], i);
                }

                _tempBlocks.Add(GeneralKit.materialPropertyBlockPool.Spawn());
                targetRenderer.GetPropertyBlock(_tempBlocks[_tempBlocks.Count - 1]);
            }
        }


        public override void Restore()
        {
            if (_originalTarget)
            {
                var t = targetRenderer;
                targetRenderer = _originalTarget;

                targetRenderer.SetPropertyBlock(_tempBlocks[_tempBlocks.Count - 1].isEmpty ? null : _tempBlocks[_tempBlocks.Count - 1]);

                int materialCount = GetMaterials();
                int count = Mathf.Min(_tempBlocks.Count - 1, materialCount);
                for (int i = 0; i < count; i++)
                {
                    targetRenderer.SetPropertyBlock(_tempBlocks[i].isEmpty ? null : _tempBlocks[i], i);
                }

                targetRenderer = t;

                foreach (var item in _tempBlocks)
                {
                    GeneralKit.materialPropertyBlockPool.Despawn(item);
                }
            }
            _tempBlocks.Clear();
        }


        public override void Reset()
        {
            base.Reset();
            targetRenderer = GetComponent<Renderer>();
            _materialMask = ~0;
            _propertyName = null;
            _propertyType = Type.Color;
            _propertyID = -1;
            _from = Color.white;
            _to = Color.white;
        }


        void OnValidate()
        {
            _propertyID = -1;
        }


        [CustomEditor(typeof(TweenMaterialProperty))]
        new class Editor : Editor<TweenMaterialProperty>
        {
            struct Property
            {
                public string name;
                public ShaderUtil.ShaderPropertyType type;
            }

            SerializedProperty _fromProp;
            SerializedProperty _fromXProp;
            SerializedProperty _fromYProp;
            SerializedProperty _fromZProp;
            SerializedProperty _fromWProp;

            SerializedProperty _toProp;
            SerializedProperty _toXProp;
            SerializedProperty _toYProp;
            SerializedProperty _toZProp;
            SerializedProperty _toWProp;


            static StringBuilder _builder = new StringBuilder();


            SerializedProperty _targetRendererProp;


            void DrawMaterialMask(int materialCount)
            {
                int count = 0;

                if (target.allMaterialSelected) _builder.Append("All (Apply to Renderer)");
                else if (target.noneMaterialSelected) _builder.Append("None");
                else
                {
                    for (int i = 0; i < materialCount; i++)
                    {
                        if (target.IsMaterialSelected(i))
                        {
                            count++;

                            if (count > 1)
                            {
                                _builder.Append(", ");
                                if (count == 4)
                                {
                                    _builder.Append("...");
                                    break;
                                }
                            }

                            _builder.Append(i);
                            _builder.Append(": ");
                            _builder.Append(_materials[i] ? _materials[i].name : "(None)");
                        }
                    }
                }

                var rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, EditorGUIKit.TempContent("Materials"));

                if (GUI.Button(rect, _builder.ToString(), EditorStyles.layerMaskField))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("All (Apply to Renderer)"), target.allMaterialSelected, () =>
                    {
                        Undo.RecordObject(target, "Select Material");
                        target.SelectAllMaterials();
                    });

                    menu.AddItem(new GUIContent("None"), target.noneMaterialSelected, () =>
                    {
                        Undo.RecordObject(target, "Select Material");
                        target.DeselectAllMaterials();
                    });

                    if (materialCount > 0) menu.AddSeparator(string.Empty);

                    for (int i = 0; i < materialCount; i++)
                    {
                        int index = i;

                        menu.AddItem(new GUIContent(index + ": " + (_materials[index] ? _materials[index].name : "(None)")),
                            target.IsMaterialSelected(index),
                            () =>
                            {
                                Undo.RecordObject(target, "Select Material");
                                target.SetMaterialSelected(index, !target.IsMaterialSelected(index));
                            });
                    }

                    menu.DropDown(rect);
                }

                _builder.Clear();
            }


            void DrawProperty(int materialCount)
            {
                var rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, EditorGUIKit.TempContent("Property"));

                if (!string.IsNullOrEmpty(target.propertyName))
                {
                    _builder.Append(target.propertyName);
                    _builder.Append(" (");
                    _builder.Append(target._propertyType);
                    _builder.Append(')');
                }

                if (GUI.Button(rect, _builder.ToString(), EditorStyles.layerMaskField))
                {
                    var properties = new HashSet<Property>();
                    var menu = new GenericMenu();

                    for (int i = 0; i < materialCount; i++)
                    {
                        if (target.IsMaterialSelected(i) && _materials[i] && _materials[i].shader)
                        {
                            var shader = _materials[i].shader;
                            int count = ShaderUtil.GetPropertyCount(shader);

                            for (int idx = 0; idx < count; idx++)
                            {
                                if (!ShaderUtil.IsShaderPropertyHidden(shader, idx))
                                {
                                    var prop = new Property
                                    {
                                        name = ShaderUtil.GetPropertyName(shader, idx),
                                        type = ShaderUtil.GetPropertyType(shader, idx)
                                    };

                                    if (properties.Contains(prop)) continue;
                                    properties.Add(prop);

                                    string description = ShaderUtil.GetPropertyDescription(shader, idx);

                                    if (prop.type == ShaderUtil.ShaderPropertyType.TexEnv)
                                    {
                                        prop.name += "_ST";
                                        prop.type = ShaderUtil.ShaderPropertyType.Vector;
                                        description += " Scale and Offest";
                                    }

                                    _builder.Clear();
                                    _builder.Append(prop.name);
                                    _builder.Append(" (\"");
                                    _builder.Append(description);
                                    _builder.Append("\", ");
                                    _builder.Append(prop.type);
                                    _builder.Append(')');

                                    menu.AddItem(new GUIContent(_builder.ToString()),
                                        target._propertyName == prop.name && target._propertyType == (Type)(int)prop.type,
                                        () =>
                                        {
                                            Undo.RecordObject(target, "Select Property");
                                            Type oldType = target.propertyType;
                                            target.SetProperty(prop.name, (Type)(int)prop.type);

                                            if (oldType != target.propertyType)
                                            {
                                                if (target.propertyType == Type.Color)
                                                    target._from = target._to = Color.white;

                                                if (target.propertyType == Type.Float || target.propertyType == Type.Range)
                                                    target._from.x = target._to.x = 1f;

                                                if (target.propertyType == Type.Vector)
                                                {
                                                    if (prop.name.EndsWith("_ST"))
                                                        target._from = target._to = new Vector4(1, 1, 0, 0);
                                                    else
                                                        target._from = target._to = new Vector4(1, 1, 1, 1);
                                                }
                                            }
                                        });

                                    _builder.Clear();
                                }
                            }
                        }
                    }

                    if (properties.Count == 0) menu.AddItem(new GUIContent("(No Valid Property)"), false, () => { });

                    menu.DropDown(rect);
                }

                _builder.Clear();
            }


            protected override void OnEnable()
            {
                base.OnEnable();

                _targetRendererProp = serializedObject.FindProperty("targetRenderer");

                _fromProp = serializedObject.FindProperty("_from");
                _fromXProp = _fromProp.FindPropertyRelative("x");
                _fromYProp = _fromProp.FindPropertyRelative("y");
                _fromZProp = _fromProp.FindPropertyRelative("z");
                _fromWProp = _fromProp.FindPropertyRelative("w");

                _toProp = serializedObject.FindProperty("_to");
                _toXProp = _toProp.FindPropertyRelative("x");
                _toYProp = _toProp.FindPropertyRelative("y");
                _toZProp = _toProp.FindPropertyRelative("z");
                _toWProp = _toProp.FindPropertyRelative("w");
            }


            protected override void InitOptionsMenu(GenericMenu menu, Tween tween)
            {
                base.InitOptionsMenu(menu, tween);

                menu.AddSeparator(string.Empty);

                menu.AddItem(new GUIContent("Swap From and To"), false, () =>
                {
                    Undo.RecordObject(target, "Swap From and To");
                    GeneralKit.Swap(ref target._from, ref target._to);
                });
            }


            protected override void OnPropertiesGUI(Tween tween)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_targetRendererProp);

                int materialCount = target.GetMaterials();
                DrawMaterialMask(materialCount);
                DrawProperty(materialCount);

                EditorGUILayout.Space();

                switch (target.propertyType)
                {
                    case Type.Float:
                    case Type.Range:
                        FromToFieldLayout("Value", _fromXProp, _toXProp);
                        break;

                    case Type.Vector:
                        FromToFieldLayout("X", _fromXProp, _toXProp);
                        FromToFieldLayout("Y", _fromYProp, _toYProp);
                        FromToFieldLayout("Z", _fromZProp, _toZProp);
                        FromToFieldLayout("W", _fromWProp, _toWProp);
                        break;

                    case Type.Color:
                        var rect = EditorGUILayout.GetControlRect();
                        float labelWidth = EditorGUIUtility.labelWidth;

                        var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth) / 2 - 2, rect.height);
                        var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
                        rect.width = labelWidth - 8;

                        EditorGUI.LabelField(rect, "Color");

                        using (new LabelWidthScope(14))
                        {
                            _fromProp.vector4Value = EditorGUI.ColorField(fromRect, EditorGUIKit.TempContent("F"), _fromProp.vector4Value, false, true, true);
                            _toProp.vector4Value = EditorGUI.ColorField(toRect, EditorGUIKit.TempContent("T"), _toProp.vector4Value, false, true, true);
                        }
                        break;
                }
            }
        }

#endif

    } // class TweenMaterialProperty

} // namespace UnityExtensions