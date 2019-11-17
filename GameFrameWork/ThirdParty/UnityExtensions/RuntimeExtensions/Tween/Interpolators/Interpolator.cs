using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 插值器
    /// </summary>
    [Serializable]
    public partial struct Interpolator
    {
        /// <summary>
        /// 插值器类型
        /// </summary>
        public enum Type
        {
            Linear = 0,
            Accelerate,
            Decelerate,
            AccelerateDecelerate,
            Anticipate,
            Overshoot,
            AnticipateOvershoot,
            Bounce,
            Parabolic,
            Sine
        }


        public Type type;
        [Range(0, 1)]
        public float strength;


        internal static readonly Func<float, float, float>[] _interpolators =
        {
            (t, s) => t,
            Accelerate,
            Decelerate,
            AccelerateDecelerate,
            Anticipate,
            Overshoot,
            AnticipateOvershoot,
            Bounce,
            (t, s) => Parabolic(t),
            (t, s) => Sine(t)
        };


        /// <summary>
        /// 计算插值
        /// </summary>
        /// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
        /// <returns> 插值结果 </returns>
        public float this[float t]
        {
            get { return _interpolators[(int)type](t, strength); }
        }


        public Interpolator(Type type, float strength = 0.5f)
        {
            this.type = type;
            this.strength = strength;
        }

    } // struct Interpolator

} // namespace UnityExtensions