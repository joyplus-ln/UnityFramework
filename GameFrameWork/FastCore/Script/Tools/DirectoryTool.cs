using System.IO;

public class DirectoryTool
{
    /// <summary>
    /// 如果不存在就创建一个文件夹
    /// </summary>
    public static void CreatIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
