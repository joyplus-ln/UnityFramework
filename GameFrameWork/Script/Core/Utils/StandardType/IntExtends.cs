using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntExtends
{

    /// <summary>
    /// int 倍率
    /// </summary>
    /// <param name="number"></param>
    /// <param name="muti"></param>
    /// <returns></returns>
    public static int MultiInt(int number,float muti)
    {
        return (int) (number * muti);
    }

    /// <summary>
    /// 百分比 倍率
    /// </summary>
    /// <returns></returns>
    public static int MultiHInt(int number,int muti)
    {
        return (int) (number * muti * 0.01f);
    }
}
