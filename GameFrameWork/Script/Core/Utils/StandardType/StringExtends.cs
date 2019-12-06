using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class StringExtends
{
    /// <summary>
    /// 分割特定开始字符串和结束字符串包裹的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startSign"></param>
    /// <param name="endSign"></param>
    /// <returns></returns>
    public static string[] SplitExtend(this string value,string startSign,string endSign)
    {
        List<string> results = new List<string>();

        string content = value;
        int startIndex = content.IndexOf(startSign);
        while (startIndex != -1)
        {
            int tempInt = startIndex + startSign.Length;

            int endIndex = content.IndexOf(endSign, tempInt);

            if (endIndex == -1)
                break;
            else
            {
                results.Add(content.Substring(tempInt, endIndex - tempInt));
                content = content.Remove(0, endIndex + endSign.Length);
                startIndex = content.IndexOf(startSign); 
            }
        }

        return results.ToArray();
    }

    /// <summary>
    /// 1,2;2,3;
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> ToStr_IntDictionary(this string value)
    {
        string[] dicts = value.Split(';');
        Dictionary<string,int> dict = new Dictionary<string, int>();
        for (int i = 0; i < dicts.Length; i++)
        {
            if (!string.IsNullOrEmpty(dicts[i]))
            {
                string[] dictChilds = dicts[i].Split('|');
                dict.Add(dictChilds[0],int.Parse(dictChilds[1]));
            }
        }
        return dict;
    }
    
    /// <summary>
    /// 1,2;2,3;
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ToStr_StrDictionary(this string value)
    {
        string[] dicts = value.Split(';');
        Dictionary<string,string> dict = new Dictionary<string, string>();
        for (int i = 0; i < dicts.Length; i++)
        {
            if (!string.IsNullOrEmpty(dicts[i]))
            {
                string[] dictChilds = dicts[i].Split('|');
                dict.Add(dictChilds[0],dictChilds[1]);
            }
        }
        return dict;
    }
}

