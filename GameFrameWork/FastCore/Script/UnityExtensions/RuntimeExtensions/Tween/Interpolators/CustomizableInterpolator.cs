using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 包含 AnimationCurve 的插值器
    /// </summary>
    [Serializable]
    public struct CustomizableInterpolator
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
            Sine,

            CustomCurve = -1
        }


        public Type type;
        [Range(0, 1)]
        public float strength;
        public AnimationCurve customCurve;


        /// <summary>
        /// 计算插值
        /// </summary>
        /// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
        /// <returns> 插值结果 </returns>
        public float this[float t]
        {
            get { return type == Type.CustomCurve ? customCurve.Evaluate(t) : Interpolator._interpolators[(int)type](t, strength); }
        }


        public CustomizableInterpolator(Type type, float strength = 0.5f, AnimationCurve customCurve = null)
        {
            this.type = type;
            this.strength = strength;
            this.customCurve = customCurve;
        }

    } // struct CustomizableInterpolator

} // namespace UnityExtensions