using UnityEngine;

namespace UnityExtensions
{
    public abstract class TweenFromTo<T> : TweenAnimation where T : struct
    {
        public T from;
        public T to;


        /// <summary>
        /// 当前状态
        /// </summary>
        public abstract T current
        {
            get;
            set;
        }


#if UNITY_EDITOR

        T _temp;


        public override void Record()
        {
            _temp = current;
        }


        public override void Restore()
        {
            current = _temp;
        }


        public override void Reset()
        {
            base.Reset();
            from = default(T);
            to = default(T);
        }


        protected new abstract class Editor<U> : TweenAnimation.Editor<U> where U : TweenFromTo<T>
        {
            protected UnityEditor.SerializedProperty _fromProp;
            protected UnityEditor.SerializedProperty _toProp;


            protected override void OnEnable()
            {
                base.OnEnable();
                _fromProp = serializedObject.FindProperty("from");
                _toProp = serializedObject.FindProperty("to");
            }


            protected override void InitOptionsMenu(UnityEditor.GenericMenu menu, Tween tween)
            {
                base.InitOptionsMenu(menu, tween);

                menu.AddSeparator(string.Empty);

                menu.AddItem(new GUIContent("Swap From and To"), false, () =>
                {
                    UnityEditor.Undo.RecordObject(target, "Swap From and To");
                    GeneralKit.Swap(ref target.from, ref target.to);
                });

                menu.AddItem(new GUIContent("Set From to Current"), false, () => 
                {
                    UnityEditor.Undo.RecordObject(target, "Set From to Current");
                    target.from = target.current;
                });

                menu.AddItem(new GUIContent("Set To to Current"), false, () =>
                {
                    UnityEditor.Undo.RecordObject(target, "Set To to Current");
                    target.to = target.current;
                });

                menu.AddSeparator(string.Empty);

                menu.AddItem(new GUIContent("Set Current to From"), false, () =>
                {
                    target.OnInterpolate(0);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
                });

                menu.AddItem(new GUIContent("Set Current to To"), false, () =>
                {
                    target.OnInterpolate(1);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
                });
            }
        }

#endif

    } // class TweenLeftRight<T>

} // namespace UnityExtensions