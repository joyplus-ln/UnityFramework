using Framework.Reflection.SQLite3Helper;
using NPOI.SS.UserModel;

namespace Framework.Editor.SQLite3Creator
{
    public enum SQLite3ValueType
    {
        INTEGER,
        REAL,
        TEXT,
       // BLOB
    }

    public struct TableData
    {
        public bool IsEnable;
        public string TableName;
        public string[] ColumnName;
        public SQLite3ValueType[] SQLite3Types;
        public SQLite3Constraint[] SQLite3Constraints;
        public string[] CSharpTypes;
        public string[] ColumnDescribes;
        public bool[] IsColumnEnables;
        public bool IsNeedCreateScript;
        public bool IsNeedCreatedb;

        public ICell[][] ExcelContents;
    }
    
    public class SQLite3EditorConfig
    {
        public const int NAME_ROW_INDEX_I = 0;
        public const int TYPE_ROW_INDEX_I = 1;
        public const int DESCRIBE_ROW_INDEX_I = 2;

        public readonly string EXCEL_CONFIG_DIRECTORY_S = UnityEngine.Application.dataPath +  "/ConfigTable";
        public readonly string SQLITE3_SCRIPT_SAVE_DIRECTORY_S = UnityEngine.Application.dataPath + "/Scripts/Data/Config/";
        public readonly string SQLITE3_DATABASE_SAVE_DIRECTORY_S = UnityEngine.Application.streamingAssetsPath + "/";
    }
}
