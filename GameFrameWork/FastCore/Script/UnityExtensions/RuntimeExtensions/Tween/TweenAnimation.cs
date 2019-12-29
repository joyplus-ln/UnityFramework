using System;
using UnityEngine;

namespace UnityExtensions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TweenAnimationAttribute : Attribute
    {
        public readonly string menu;
        public readonly string name;

        public TweenAnimationAttribute(string menu, string name)
        {
            this.menu = menu;
            this.name = name;
        }
    }


    public abstract partial class TweenAnimation : ScriptableComponent
    {
        [SerializeField]
        float _minNormalizedTime = 0f;

        [SerializeField]
        float _maxNormalizedTime = 1f;

        [SerializeField]
        CustomizableInterpolator _interpolator;


        public float minNormalizedTime
        {
            get { return _minNormalizedTime; }
            set
            {
                _minNormalizedTime = Mathf.Clamp01(value);
                _maxNormalizedTime = Mathf.Clamp(_maxNormalizedTime, _minNormalizedTime, 1f);
            }
        }


        public float maxNormalizedTime
        {
            get { return _maxNormalizedTime; }
            set
            {
                _maxNormalizedTime = Mathf.Clamp01(value);
                _minNormalizedTime = Mathf.Clamp(_minNormalizedTime, 0f, _maxNormalizedTime);
            }
        }


        public void OnUpdate(float normalizedTime)
        {
            if (normalizedTime <= _minNormalizedTime) normalizedTime = 0f;
            else if (normalizedTime >= _maxNormalizedTime) normalizedTime = 1f;
            else normalizedTime = (normalizedTime - _minNormalizedTime) / (_maxNormalizedTime - _minNormalizedTime);

            OnInterpolate(_interpolator[normalizedTime]);
        }


        protected abstract void OnInterpolate(float factor);

    } // class TweenAnimation

} // UnityExtensions
