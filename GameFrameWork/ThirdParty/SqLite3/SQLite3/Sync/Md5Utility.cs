using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class MD5Utility
{
    private static MD5CryptoServiceProvider md5;
    public static string GetStringMD5(string InString)
    {
        return GetBytesMD5(Encoding.UTF8.GetBytes(InString));
    }

    public static string GetFileMD5(string InFilePath)
    {
        if (!string.IsNullOrEmpty(InFilePath) && File.Exists(InFilePath))
        {
            return GetBytesMD5(File.ReadAllBytes(InFilePath));
        }

        return string.Empty;
    }

    public static string GetBytesMD5(byte[] InBytes)
    {
        if (null == md5) md5 = new MD5CryptoServiceProvider();

        byte[] hashBytes = md5.ComputeHash(InBytes);
        int hashBytesLength = hashBytes.Length;
        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < hashBytesLength; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }

        return sb.ToString();
    }
}