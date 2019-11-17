using System;
using System.IO;

namespace UnityExtensions
{
    /// <summary>
    /// 存档基类。不必直接继承此类，继承 BinarySave 和 TextSave 以简化编程。
    /// 关于此设计的解释：
    ///     有人将存档数据打包为一个对象，然后对此对象执行读写操作。这其实不是一种好的
    ///     设计。回想一下，为什么会有存档？因为硬件系统不够完美，无法做到永不断电地运
    ///     行，为了用户在下次打开软件时可以接着之前的状态运行下去，我们需要把一些重要
    ///     的数据通过外部存储设备保存下来。也就是说，存档是一种恢复软件状态的工具。如
    ///     果没有存档，我们会高兴地按照软件工程的方式去设计代码结构、组织数据，而不是
    ///     将一堆完全无关的数据捆绑到一起。所以，在这个存档系统中，存档是一种虚拟概念，
    ///     它不显式地包含任何数据（当然也不阻止你这么做），而是定义一对保存、加载数据
    ///     的过程，你可以自由决定将任何模块的任何数据添加到这个保存、加载过程中。
    /// </summary>
    public abstract class Save
    {
        internal Exception ToBytes(out byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(256))
                {
                    Write(stream);
                    data = stream.ToArray();
                    return null;
                }
            }
            catch (Exception e)
            {
                data = null;
                return e;
            }
        }


        internal Exception FromBytes(ref byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(data))
                {
                    Read(stream);
                    return null;
                }
            }
            catch (Exception e)
            {
                Reset();
                return e;
            }
        }


        /// <summary>
        /// 重置数据. 当读取失败时会自动执行
        /// </summary>
        public abstract void Reset();


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(Stream stream);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(Stream stream);

    } // class Save


    /// <summary>
    /// 二进制存档
    /// 继承此类并实现 Reset, Read 和 Write
    /// </summary>
    public abstract class BinarySave : Save
    {
        protected sealed override void Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                Read(reader);
            }
        }


        protected sealed override void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                Write(writer);
            }
        }


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(BinaryReader reader);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(BinaryWriter writer);

    } // class BinarySave


    /// <summary>
    /// 文本存档
    /// 继承此类并实现 Reset, Read 和 Write
    /// </summary>
    public abstract class TextSave : Save
    {
        protected sealed override void Read(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                Read(reader);
            }
        }


        protected sealed override void Write(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                Write(writer);
            }
        }


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(StreamReader reader);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(StreamWriter writer);

    } // class TextSave

} // namespace UnityExtensions