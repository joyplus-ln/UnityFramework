using System;

namespace UnityExtensions
{
    [Serializable]
    public struct bool2
    {
        public bool x;
        public bool y;

        public bool anyTrue { get { return x || y; } }
        public bool allTrue { get { return x && y; } }
    }


    [Serializable]
    public struct bool3
    {
        public bool x;
        public bool y;
        public bool z;

        public bool anyTrue { get { return x || y || z; } }
        public bool allTrue { get { return x && y && z; } }
    }

    [Serializable]
    public struct bool4
    {
        public bool x;
        public bool y;
        public bool z;
        public bool w;

        public bool anyTrue { get { return x || y || z || w; } }
        public bool allTrue { get { return x && y && z && w; } }
    }

} // namespace UnityExtensions