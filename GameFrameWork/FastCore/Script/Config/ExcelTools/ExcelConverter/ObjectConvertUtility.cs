using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConvertUtility
{
    public static int[] ConvertIntArraryValue(object value)
    {
        string[] strs = value.ToString().Split('|');
        List<int> lists = new List<int>();
        for (int i = 0; i < strs.Length; i++)
        {
            int temp = 0;
            if (int.TryParse(strs[i], out temp))
            {
                lists.Add(temp);
            }
        }

        return lists.ToArray();
    }

    public static string[] ConvertStringArraryValue(object value)
    {
        string[] strs = value.ToString().Split('|');
        List<string> lists = new List<string>();
        for (int i = 0; i < strs.Length; i++)
        {
            if (!string.IsNullOrEmpty(strs[i]))
            {
                lists.Add(strs[i]);
            }
        }

        return lists.ToArray();
    }
}
