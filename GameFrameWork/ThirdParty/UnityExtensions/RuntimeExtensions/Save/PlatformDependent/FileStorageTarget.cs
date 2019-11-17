using System.IO;

namespace UnityExtensions
{
    /// <summary>
    /// 文件储存目标
    /// </summary>
    public class FileStorageTarget : IStorageTarget
    {
        string _fullDir;
        string _fullPath;


        /// <summary>
        /// 创建储存目标
        /// </summary>
        /// <param name="fileName"> 文件名，可使用 "/" 添加父级目录；文件名相对于 Application.persistentDataPath 目录 </param>
        public FileStorageTarget(string fileName)
        {
            _fullPath = string.Format("{0}/{1}", UnityEngine.Application.persistentDataPath, fileName);

            int firstSlash = _fullPath.IndexOf('/');
            int lastSlash = _fullPath.LastIndexOf('/');
            if (firstSlash != lastSlash) _fullDir = _fullPath.Substring(0, lastSlash);
        }


        void IStorageTarget.Write(byte[] data)
        {
            if (_fullDir != null) Directory.CreateDirectory(_fullDir);
            using (var stream = new FileStream(_fullPath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(data, 0, data.Length);
            }
        }


        byte[] IStorageTarget.Read()
        {
            using (var stream = new FileStream(_fullPath, FileMode.Open, FileAccess.Read))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }


        void IStorageTarget.Delete()
        {
            File.Delete(_fullPath);
        }

    } // class FileStorageTarget

} // namespace UnityExtensions