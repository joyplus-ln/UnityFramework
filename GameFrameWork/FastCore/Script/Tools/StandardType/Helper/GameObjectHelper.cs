using UnityEngine;

/***
 * GameObjectHelper.cs
 * 
 * @author administrator
 */
namespace GameEngine
{
    public static class GameObjectHelper
    {
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetMissingScript<T>(this GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null) {
                t = go.AddComponent<T>();
            }
            return t;
        }

        /// <summary>
        /// 设置GameObject的Layer
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layerMask"></param>
        public static void SetLayer(this GameObject go, int layerMask)
        {
            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
            int count = transforms.Length;
            for(int i = 0; i < count; ++i) {
                transforms[i].gameObject.layer = layerMask;
            }
        }

        /// <summary>
        /// 设置GameObject的Child对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="childGo"></param>
        public static void SetChild(this GameObject go, GameObject childGo, bool isResetTransform = true)
        {
            if(childGo == null) {
                return;
            }
            Transform childTr = childGo.transform;
            childTr.parent = go.transform;
            if (isResetTransform) {
                childTr.localPosition = Vector3.zero;
                childTr.localRotation = Quaternion.identity;
                childTr.localScale = Vector3.one;
            }
        }
    }
}
