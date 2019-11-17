using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    public enum WrapMode
    {
        Clamp,
        Loop,
        PingPong
    }


    [System.Flags]
    public enum ArrivedAction
    {
        KeepPlaying = 0,
        StopOnForwardArrived = 1,
        StopOnBackArrived = 2,
        AlwaysStopOnArrived = 3
    }


    public enum PlayDirection
    {
        Forward,
        Back
    }


    /// <summary>
    /// 插值动画
    /// </summary>
    [AddComponentMenu("Unity Extensions/Tween")]
    [ExecuteInEditMode]
    public partial class Tween : ConfigurableUpdateComponent
    {
        const float _minDuration = 0.0001f;


        [SerializeField, Min(_minDuration)]
        float _duration = 1f;

        public TimeMode timeMode = TimeMode.Unscaled;
        public WrapMode wrapMode = WrapMode.Clamp;
        public ArrivedAction arrivedAction = ArrivedAction.AlwaysStopOnArrived;

        [SerializeField] UnityEvent _onForwardArrived;
        [SerializeField] UnityEvent _onBackArrived;

        [SerializeField] List<TweenAnimation> _animations;


        float _normalizedTime = 0f;


        [NonSerialized]
        public PlayDirection direction;


        public float duration
        {
            get { return _duration; }
            set { _duration = value > _minDuration ? value : _minDuration; }
        }


        public event UnityAction onForwardArrived
        {
            add
            {
                if (_onForwardArrived == null) _onForwardArrived = new UnityEvent();
                _onForwardArrived.AddListener(value);
            }
            remove { _onForwardArrived?.RemoveListener(value); }
        }


        public event UnityAction onBackArrived
        {
            add
            {
                if (_onBackArrived == null) _onBackArrived = new UnityEvent();
                _onBackArrived.AddListener(value);
            }
            remove { _onBackArrived?.RemoveListener(value); }
        }


        public float normalizedTime
        {
            get { return _normalizedTime; }
            set
            {
                _normalizedTime = Mathf.Clamp01(value);
                if (_animations != null)
                {
                    for (int i = 0; i < _animations.Count; i++)
                    {
                        var item = _animations[i];
                        if (item)
                        {
                            if (item.enabled) item.OnUpdate(_normalizedTime);
                        }
                        else _animations.RemoveAt(i--);
                    }
                }
            }
        }


        /// <summary>
        /// 颠倒播放方向
        /// </summary>
        public void ReverseDirection()
        {
            direction = direction == PlayDirection.Forward ? PlayDirection.Back : PlayDirection.Forward;
        }


        /// <summary>
        /// 设置正向播放（Unity 事件序列化辅助）
        /// </summary>
        public void SetDirectionForward()
        {
            direction = PlayDirection.Forward;
        }


        /// <summary>
        /// 设置反向播放（Unity 事件序列化辅助）
        /// </summary>
        public void SetDirectionBack()
        {
            direction = PlayDirection.Back;
        }


        public void ForwardEnable()
        {
            enabled = true;
            direction = PlayDirection.Forward;
        }


        public void BackEnable()
        {
            enabled = true;
            direction = PlayDirection.Back;
        }


        public void Sample(float normalizedTime)
        {
            float t = _normalizedTime;
            this.normalizedTime = normalizedTime;
            _normalizedTime = t;
        }


        public T AddAnimation<T>() where T : TweenAnimation
        {
            var anim = gameObject.AddComponent<T>();
            AddAnimationInternal(anim);
            return anim;
        }


        public TweenAnimation AddAnimation(Type type)
        {
            var anim = gameObject.AddComponent(type) as TweenAnimation;
            AddAnimationInternal(anim);
            return anim;
        }


        void AddAnimationInternal(TweenAnimation anim)
        {
            anim.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            if (_animations == null) _animations = new List<TweenAnimation>(4);
            _animations.Add(anim);
        }


        public bool RemoveAnimation<T>(T animation) where T : TweenAnimation
        {
            if (_animations.Remove(animation))
            {
                if (Application.isPlaying) Destroy(animation);
                else DestroyImmediate(animation);
                return true;
            }
            return false;
        }


        protected override void OnUpdate()
        {
#if UNITY_EDITOR
            if (_dragging) return;
#endif

            float deltaTime =
#if UNITY_EDITOR
            !Application.isPlaying ? UnityExtensions.Editor.EditorApplicationKit.deltaTime :
#endif
            (timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);

            while (enabled && deltaTime > Mathf.Epsilon)
            {
                if (direction == PlayDirection.Forward)
                {
                    if (wrapMode == WrapMode.Clamp && _normalizedTime == 1f)
                    {
                        normalizedTime = 1f;
                        return;
                    }

                    float time = _normalizedTime * _duration + deltaTime;

                    if (time < _duration)
                    {
                        normalizedTime = time / _duration;
                        return;
                    }

                    deltaTime = time - _duration;
                    normalizedTime = 1f;

                    if (wrapMode == WrapMode.PingPong) direction = PlayDirection.Back;
                    else if (wrapMode == WrapMode.Loop) _normalizedTime = 0f;

                    if ((arrivedAction & ArrivedAction.StopOnForwardArrived) == ArrivedAction.StopOnForwardArrived)
                        enabled = false;

                    _onForwardArrived?.Invoke();
                }
                else
                {
                    if (wrapMode == WrapMode.Clamp && _normalizedTime == 0f)
                    {
                        normalizedTime = 0f;
                        return;
                    }

                    float time = _normalizedTime * _duration - deltaTime;

                    if (time > 0f)
                    {
                        normalizedTime = time / _duration;
                        return;
                    }

                    deltaTime = -time;
                    normalizedTime = 0f;

                    if (wrapMode == WrapMode.PingPong) direction = PlayDirection.Forward;
                    else if (wrapMode == WrapMode.Loop) _normalizedTime = 1f;

                    if ((arrivedAction & ArrivedAction.StopOnBackArrived) == ArrivedAction.StopOnBackArrived)
                        enabled = false;

                    _onBackArrived?.Invoke();
                }
            }
        }


        void OnDestroy()
        {
            if (_animations != null)
            {
                foreach (var anim in _animations)
                {
                    if (anim)
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            // 为避免奇怪的报错，这里就不直接删除了，而是交给编辑器 update 检查吧 :-/
                            //DestroyImmediate(anim);
                            _objectList.Add(gameObject);
                        }
                        else
#endif
                            Destroy(anim);
                    }
                }
            }
        }

    } // class Tween

} // UnityExtensions
