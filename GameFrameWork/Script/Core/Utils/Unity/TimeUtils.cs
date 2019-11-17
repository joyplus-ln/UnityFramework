using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtils
{
    /// <summary>
    /// 时间戳转datetime
    /// </summary>
    /// <param name="unixtime"></param>
    /// <returns></returns>
    public static DateTime UnixToDateTime(string unixtime)
    {
        long time = 0;
        bool okOrNot = long.TryParse(unixtime, out time);
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        if (!okOrNot)
        {
            TimeSpan distanceTimeSpan = DateTime.Now - startTime;
            long.TryParse(Math.Ceiling(distanceTimeSpan.TotalSeconds) + "", out time);
        }
        DateTime dt = startTime.AddSeconds(time);
        return dt;

    }
}
