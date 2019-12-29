/***
 * ArrayHelper.cs
 * 
 * @author administrator 
 */
namespace GameEngine
{
    /// <summary>
    /// 数组辅助类
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// 合并数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T[] Merge<T>(this T[] first, T[] second)
        {
            T[] result = new T[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }

        /// <summary>
        /// 数组转换至指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T[] To<T>(this UnityEngine.Object[] data) where T : UnityEngine.Object
        {
            int len = data.Length;
            T[] returnList = new T[len];
            for (int i = 0; i < len; i++) {
                returnList[i] = data[i] as T;
            }
            return returnList;
        }

        /// <summary>
        /// 数组转换至指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T[] To<T>(this System.Object[] data)
        {
            int len = data.Length;
            T[] returnList = new T[len];
            for (int i = 0; i < len; i++) {
                returnList[i] = (T)data[i];
            }
            return returnList;
        }
    }
}
