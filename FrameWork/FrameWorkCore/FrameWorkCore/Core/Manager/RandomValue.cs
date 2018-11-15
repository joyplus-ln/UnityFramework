using System;

namespace FrameWorkCore.Core
{
    public class RandomValue
    {
        private int min = 0;
        private int max = 10;
        private Random _random;

        public void SetRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public RandomValue()
        {
            _random = new Random();
        }

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <returns></returns>
        public int Nest()
        {
            return _random.Next(min, max);
        }
    }
}