using System.IO;
using System.Collections.Generic;
using System;

namespace UnityExtensions
{
    public interface IBinarySavable
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
        void Reset();
    }


    public interface ITextSavable
    {
        IList<string> keys { get; }
        void Read(string key, string value);
        void Write(string key, StreamWriter writer);
        void Reset(string key);
    }


    /// <summary>
    /// 代表一个二进制存档字段，用于简化存档实现
    /// </summary>
    public class BinarySavableField : IBinarySavable
    {
        Action<BinaryReader> _read;
        Action<BinaryWriter> _write;
        Action _reset;

        public BinarySavableField(Action<BinaryReader> read, Action<BinaryWriter> write, Action reset)
        {
            _read = read;
            _write = write;
            _reset = reset;
        }

        void IBinarySavable.Read(BinaryReader reader)
        {
            _read(reader);
        }

        void IBinarySavable.Write(BinaryWriter writer)
        {
            _write(writer);
        }

        void IBinarySavable.Reset()
        {
            _reset();
        }
    }


    /// <summary>
    /// 代表一个文本存档字段，用于简化存档实现
    /// </summary>
    public class TextSavableField : ITextSavable
    {
        string _key;
        Action<string> _read;
        Action<StreamWriter> _write;
        Action _reset;

        static string[] _sharedKeyArray = new string[1];

        public TextSavableField(string key, Action<string> read, Action<StreamWriter> write, Action reset)
        {
            _key = key;
            _read = read;
            _write = write;
            _reset = reset;
        }

        IList<string> ITextSavable.keys
        {
            get
            {
                _sharedKeyArray[0] = _key;
                return _sharedKeyArray;
            }
        }

        void ITextSavable.Read(string key, string value)
        {
            _read(value);
        }

        void ITextSavable.Write(string key, StreamWriter writer)
        {
            _write(writer);
        }

        void ITextSavable.Reset(string key)
        {
            _reset();
        }
    }


    public sealed class BinarySavableCollection : BinarySave
    {
        IList<IBinarySavable> _savables;


        public BinarySavableCollection(IList<IBinarySavable> savables)
        {
            _savables = savables;
        }


        protected sealed override void Read(BinaryReader reader)
        {
            foreach (var s in _savables)
            {
                s.Read(reader);
            }
        }


        protected sealed override void Write(BinaryWriter writer)
        {
            foreach (var s in _savables)
            {
                s.Write(writer);
            }
        }


        public sealed override void Reset()
        {
            foreach (var s in _savables)
            {
                s.Reset();
            }
        }
    }


    public sealed class TextSavableCollection : TextSave
    {
        Dictionary<string, ITextSavable> _savables;


        public TextSavableCollection(IList<ITextSavable> savables)
        {
            _savables = new Dictionary<string, ITextSavable>(savables.Count * 2);

            foreach (var s in savables)
            {
                foreach (var k in s.keys)
                {
                    _savables.Add(k, s);
                }
            }
        }


        protected sealed override void Read(StreamReader reader)
        {
            var texts = reader.ReadToEnd().Split(new string[] { ": ", ":", "\n", "\r\n" }, StringSplitOptions.None);

            var items = new Dictionary<string, string>(texts.Length / 2);

            for (int i = 1; i < texts.Length; i+=2)
            {
                var key = texts[i - 1].Trim();
                var value = texts[i].Trim();
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    items[key] = value;
                }
            }

            foreach (var s in _savables)
            {
                if (items.TryGetValue(s.Key, out string value))
                {
                    s.Value.Read(s.Key, value);
                }
                else
                {
                    s.Value.Reset(s.Key);
                }
            }
        }


        protected sealed override void Write(StreamWriter writer)
        {
            foreach (var s in _savables)
            {
                writer.Write(s.Key);
                writer.Write(": ");
                s.Value.Write(s.Key, writer);
                writer.WriteLine();
            }
        }


        public sealed override void Reset()
        {
            foreach (var s in _savables)
            {
                s.Value.Reset(s.Key);
            }
        }
    }

} // namespace UnityExtensions