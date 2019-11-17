using System;
using System.Text;

namespace Framework.Reflection.SQLite3Helper
{
    public class SQLite3Prefs
    {
        private static SQLite3Operate sqlite3Operate;
        private static readonly string tableName = "SQLite3PrefsTable";
        private static readonly string sql = "CREATE TABLE " + tableName + "(singletonKey TEXT PRIMARY KEY, Value TEXT)";

        static SQLite3Prefs()
        {
            InitDatabase();
        }

        private static void InitDatabase()
        {
            sqlite3Operate = SQLite3Factory.OpenToWrite(MD5Utility.GetBytesMD5(Encoding.UTF8.GetBytes("SQLite3Prefs.db")));

            if (!sqlite3Operate.TableExists(tableName)) sqlite3Operate.Exec(sql);
        }

        public static void SetInt(string InKey, int InValue)
        {
            SetValue(InKey, InValue);
        }

        public static int GetInt(string InKey, int InDefaultValue = 0)
        {
            object value = GetValue(InKey);
            return null == value ? InDefaultValue : Convert.ToInt32(value);
        }

        public static void SetFloat(string InKey, float InValue)
        {
            SetValue(InKey, InValue);
        }

        public static float GetFloat(string InKey, float InDefaultValue = 0)
        {
            object value = GetValue(InKey);
            return null == value ? InDefaultValue : Convert.ToSingle(value);
        }

        public static void SetString(string InKey, string InValue)
        {
            SetValue(InKey, InValue);
        }

        public static string GetString(string InKey, string InDefaultValue = "")
        {
            object value = GetValue(InKey);
            return null == value ? InDefaultValue : value.ToString();
        }

        public static void SetObject(string InKey, object InObj)
        {
            //Type type = InObj.GetType();
            //FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public);
            //fieldInfos[0].getv
        }

        public static bool HasKey(string InKey)
        {
            return sqlite3Operate.ValueExists(tableName, "singletonKey", InKey);
        }

        public static void DeleteKey(string InKey)
        {
            if (sqlite3Operate.ValueExists(tableName, "singletonKey", InKey))
                sqlite3Operate.Delete(tableName, "singletonKey", InKey);
        }

        public static void DeleteAll()
        {
            if (null == sqlite3Operate) InitDatabase();
            sqlite3Operate.DeleteAll(tableName);
            sqlite3Operate.CloseDb();
            sqlite3Operate = null;
        }

        public static void ClearAll()
        {
            sqlite3Operate.DeleteAll(tableName);
        }

        private static void SetValue(string InKey, object InValue)
        {
            if (sqlite3Operate.ValueExists(tableName, "singletonKey", InKey))
                sqlite3Operate.Update(tableName, "Value", InValue, "singletonKey", InKey);
            else sqlite3Operate.Insert(tableName, InKey, InValue);
        }

        private static object GetValue(string InKey)
        {
            return sqlite3Operate.ValueExists(tableName, "singletonKey", InKey)
                ? sqlite3Operate.SelectSingleObject("Value", tableName, "singletonKey", InKey)
                : null;
        }
    }
}
