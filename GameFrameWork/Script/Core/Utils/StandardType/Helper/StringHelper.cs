using System;
/***
 * StringHelper.cs
 * 
 * @author administrator
 */
namespace GameEngine
{
    public static class StringHelper
    {
        /// <summary>
        /// 标准化路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Standard(this string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>字符串转Byte</summary>
        /// <param name="str">值</param>
        /// <returns></returns>
        public static byte[] ToByte(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        /// <summary>Byte转字符串</summary>
        /// <param name="bytes">值</param>
        /// <returns></returns>
        public static string String(this byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) {
                return string.Empty;
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static T[] SplitString<T>(this string str, char segment = '|') where T : struct
        {
            if (string.IsNullOrEmpty(str)) {
                return null;
            }

            string[] strArr = str.Split(segment);
            int count = strArr.Length;
            T[] tArray = new T[count];
            for (int i = 0; i < count; i++) {
                tArray[i] = (T)Convert.ChangeType(strArr[i], typeof(T));
            }

            return tArray;
        }

        /// <summary>
        /// 分割字符串为特定类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitString(this string str, char separator = '|')
        {
            if (string.IsNullOrEmpty(str)) {
                return null;
            }

            string[] array = str.Split(separator);
            for (int i = 0, len = array.Length; i < len; ++i) {
                array[i] = array[i].Trim();
            }

            return array;
        }
    }
}
