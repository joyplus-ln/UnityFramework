namespace FrameWorkCore.Core
{
    public class NumberFactory
    {
        /// <summary>
        /// 返回一个浮点型的分制数量，四舍五入，不丢失
        /// </summary>
        /// <param name="number"></param>
        public int GetCentCout(float number)
        {
            float num = number * 100;
            float last = num % 1;
            int cover = 0;
            if (last>0.5f)
            {
                cover += 1;
            }

            return (int)num + cover;
        }
    }
}