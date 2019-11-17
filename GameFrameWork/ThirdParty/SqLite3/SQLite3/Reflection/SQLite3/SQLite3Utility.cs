using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Reflection.SQLite3Helper
{
    public class SQLite3Utility
    {
        public static string ConvertSQLite3ConstraintToStr(SQLite3Constraint InConstraint)
        {
            string result = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) != 0)
                result += " PrimaryKey ";
            if ((InConstraint & SQLite3Constraint.Unique) != 0)
                result += " Unique ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) != 0)
                result += " AutoIncrement ";
            if ((InConstraint & SQLite3Constraint.NotNull) != 0)
                result += " NotNull ";

            return result == string.Empty ? string.Empty : result.Remove(result.Length - 1, 1);
        }

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
}