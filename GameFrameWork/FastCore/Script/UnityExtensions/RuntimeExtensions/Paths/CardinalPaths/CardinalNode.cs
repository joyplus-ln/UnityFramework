using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// Cardinal 路径节点
    /// </summary>
    [Serializable]
    public class CardinalNode : Path.Node
    {
        public Vector3 position;
        public float tension = 0.5f;

    } // class CardinalNode

} // namespace UnityExtensions