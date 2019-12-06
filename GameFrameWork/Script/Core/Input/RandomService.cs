using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class RandomService
{
    static RandomHandel s_onRandomCreat;

    static bool s_isFixedRandom = false;
    static List<int> s_randomList = new List<int>();

    public static RandomHandel OnRandomCreat
    {
        get { return RandomService.s_onRandomCreat; }
        set { RandomService.s_onRandomCreat = value; }
    }

    public static void SetRandomList(List<int> list)
    {
        s_isFixedRandom = true;
        s_randomList = list;
    }

    public static int GetRandomListCount()
    {
        return s_randomList.Count;
    }

    public static int GetRand(int min, int max)
    {
        return Range(min, max);
    }

    public static int GetRandReal(int min, int max)
    {
        return Range(min, max + 1);
    }

    public static int Range(int min, int max)
    {
        if (!s_isFixedRandom)
        {
            int random = UnityEngine.Random.Range(min, max);

            DispatchRandom(random);
            return random;
        }
        else
        {
            return GetFixedRandom();
        }
    }

    public static float Range01()
    {
        int random = 0;

        if (s_isFixedRandom)
        {
            random = GetFixedRandom();
        }
        else
        {
            random = UnityEngine.Random.Range(0, 10001);
            DispatchRandom(random);
        }

        float result = ((float)random) / 10000;

        return result;
    }

    static void DispatchRandom(int random)
    {
        if (s_onRandomCreat != null)
        {
            s_onRandomCreat(random);
        }
    }

    static int GetFixedRandom()
    {
        if (s_randomList != null && s_randomList.Count > 0)
        {
            int random = s_randomList[0];
            s_randomList.RemoveAt(0);
            return random;
        }
        else
        {
            throw new Exception("RandomService Exception no RandomList!");
        }
    }

    public class FixRandom
    {
        public int m_RandomSeed = 0;

        int m_randomA = 9301;
        int m_randomB = 49297;
        int m_randomC = 233280;

        public FixRandom(int seed)
        {
            SetFixRandomSeed(seed);
        }

        public void SetFixRandomSeed(int seed)
        {
            m_RandomSeed = seed;
        }

        public void SetFixRandomParm(int a, int b, int c)
        {
            m_randomA = a;
            m_randomB = b;
            m_randomC = c;
        }

        public int GetFixRandom()
        {
            m_RandomSeed = Math.Abs((m_RandomSeed * m_randomA + m_randomB) % m_randomC);

            return m_RandomSeed;
        }

        public int Range(int min, int max)
        {
            if (max <= min)
                return min;
            int random = GetFixRandom();
            int range = max - min;
            return (random % range) + min;
        }


    }
    
    /// <summary>
    /// 上下随机偏移一个数值
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static int RangeRegionI(int seed,int region = 1)
    {
        return UnityEngine.Random.Range(seed - region, seed + region + 1);
    }

    /// <summary>
    /// 上下偏移百分多少
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    public static int RangeRegionF(int seed,float region = 0.05f)
    {
        int regions = (int)(seed * region);
        return UnityEngine.Random.Range(seed - regions, seed + regions + 1);
    }
    
    /// <summary>
    /// 生成不重复的随机数，调用次数有限   超出以后有报错提醒
    /// </summary>
    public class SingleRandom
    {
        private List<int> randomList = new List<int>();
        public SingleRandom(int min,int max)
        {
            for (int i = min; i < max; i++)
            {
                randomList.Add(i);
            }
            
        }

        public int Next()
        {
            if (randomList.Count > 0)
            {
                int randomIndex = Random.Range(0, randomList.Count);
                int randomNumber = randomList[randomIndex];
                randomList.RemoveAt(randomIndex);
                return randomNumber;
            }
            Debug.LogException(new Exception("Random out of Index"));
            return 0;
        }
        
    }
}

public delegate void RandomHandel(int random);
