using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 可以播放字符出现效果的 UI 文本组件
    /// 暂不支持 rich text
    /// </summary>
    [AddComponentMenu("Unity Extensions/UI/Playable Text")]
    public class PlayableText : Text
    {
        [Header("Playing")]
        public TimeMode playTimeMode = TimeMode.Unscaled;
        public float playSpeed = 40;


        int _visibleCharacterCount = -1;
        double _playingCount = -1;


        /// <summary>
        /// 可见字符数变化时触发，参数代表是否播放完成
        /// </summary>
        public event System.Action<bool> onVisibleCharacterCountChanged;


        /// <summary>
        /// -1 means all characters
        /// </summary>
        public int visibleCharacterCount
        {
            get => _visibleCharacterCount;
            set
            {
                value = Mathf.Clamp(value, -1, m_Text.Length);

                if (value != _visibleCharacterCount)
                {
                    if (_playingCount >= 0) _playingCount += value - (int)_playingCount;
                    _visibleCharacterCount = value;

                    SetVerticesDirty();
                }
            }
        }


        public override string text
        {
            get => m_Text;
            set
            {
                base.text = value;
                visibleCharacterCount = _visibleCharacterCount;
            }
        }


        public bool playing
        {
            get => _playingCount >= 0;
            set
            {
                if (value != playing)
                {
                    if (value)
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            Debug.LogError("Can't play in editor.");
                            return;
                        }
#endif
                        _playingCount = _visibleCharacterCount < 0 ? m_Text.Length : _visibleCharacterCount;
                    }
                    else _playingCount = -1;
                }
            }
        }


        [ContextMenu("Restart Playing")]
        public void RestartPlaying()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.LogError("Can't play in editor.");
                return;
            }
#endif
            visibleCharacterCount = 0;
            playing = true;
        }


        void LateUpdate()
        {
            if (_playingCount < 0) return;

            float delta = playSpeed * (playTimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);

            if (delta != 0f)
            {
                int lastCount = (int)_playingCount;

                _playingCount += delta;
                if (_playingCount < 0) _playingCount = 0;

                int count = (int)_playingCount;

                if (count != lastCount)
                {
                    visibleCharacterCount = count;

                    bool finished = count >= m_Text.Length;
                    if (finished) _playingCount = -1;
                    onVisibleCharacterCountChanged?.Invoke(finished);
                }
            }
        }


        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count - 4;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            // ----------------------------------------------------------------------------------------
            // 相对原 OnPopulateMesh 唯一的不同：
            if (_visibleCharacterCount >= 0) vertCount -= (m_Text.Length - _visibleCharacterCount) * 4;
            // ----------------------------------------------------------------------------------------

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
        }


#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();
            supportRichText = false;
        }


        protected override void OnValidate()
        {
            base.OnValidate();
            supportRichText = false;
            visibleCharacterCount = _visibleCharacterCount;
        }


        // UI 编辑器内容无法访问 :/
        //[CustomEditor(typeof(PlayableText), true)]
        //[CanEditMultipleObjects]
        //public class PlayableTextEditor : UnityEditor.UI.TextEditor
        //{
        //}

#endif
    }
}