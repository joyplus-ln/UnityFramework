using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Mono.Data.Sqlite;

public class DbRowsReader
{
    private readonly List<List<object>> data = new List<List<object>>();
    private int currentRow = -1;
    public int FieldCount { get; private set; }
    public int VisibleFieldCount { get; private set; }
    public int RowCount { get { return data.Count; } }
    public bool HasRows { get { return RowCount > 0; } }

    public void Init(SqliteDataReader sqliteDataReader)
    {
        FieldCount = sqliteDataReader.FieldCount;
        VisibleFieldCount = sqliteDataReader.VisibleFieldCount;
        while (sqliteDataReader.Read())
        {
            var buffer = new object[sqliteDataReader.FieldCount];
            sqliteDataReader.GetValues(buffer);
            data.Add(new List<object>(buffer));
        }
        ResetReader();
    }

    public bool Read()
    {
        if (currentRow + 1 >= RowCount)
            return false;
        ++currentRow;
        return true;
    }

    public System.DateTime GetDateTime(int index)
    {
        return (System.DateTime)data[currentRow][index];
    }

    public char GetChar(int index)
    {
        return (char)data[currentRow][index];
    }

    public string GetString(int index)
    {
        return (string)data[currentRow][index];
    }

    public bool GetBoolean(int index)
    {
        return (bool)data[currentRow][index];
    }

    public short GetInt16(int index)
    {
        return (short)((long)data[currentRow][index]);
    }

    public int GetInt32(int index)
    {
        return (int)((long)data[currentRow][index]);
    }

    public long GetInt64(int index)
    {
        return (long)data[currentRow][index];
    }

    public decimal GetDecimal(int index)
    {
        return (decimal)((double)data[currentRow][index]);
    }

    public float GetFloat(int index)
    {
        return (float)((double)data[currentRow][index]);
    }

    public double GetDouble(int index)
    {
        return (double)data[currentRow][index];
    }

    public void ResetReader()
    {
        currentRow = -1;
    }
}

public partial class SQLiteGameService
{
    public string dbPath = "./tbRpgDb.sqlite3";

    private SqliteConnection connection;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            if (dbPath.StartsWith("./"))
                dbPath = dbPath.Substring(1);
            if (!dbPath.StartsWith("/"))
                dbPath = "/" + dbPath;
            dbPath = Application.persistentDataPath + dbPath;
        }

        if (!File.Exists(dbPath))
            SqliteConnection.CreateFile(dbPath);

        // open connection
        connection = new SqliteConnection("URI=file:" + dbPath);

        ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS player (
            id TEXT NOT NULL PRIMARY KEY,
            profileName TEXT NOT NULL,
            loginToken TEXT NOT NULL,
            exp INTEGER NOT NULL,
            selectedFormation TEXT NOT NULL)");

       
    }

    public void ExecuteNonQuery(string sql, params SqliteParameter[] args)
    {
        connection.Open();
        using (var cmd = new SqliteCommand(sql, connection))
        {
            foreach (var arg in args)
            {
                cmd.Parameters.Add(arg);
            }
            cmd.ExecuteNonQuery();
        }
        connection.Close();
    }

    public object ExecuteScalar(string sql, params SqliteParameter[] args)
    {
        object result;
        connection.Open();
        using (var cmd = new SqliteCommand(sql, connection))
        {
            foreach (var arg in args)
            {
                cmd.Parameters.Add(arg);
            }
            result = cmd.ExecuteScalar();
        }
        connection.Close();
        return result;
    }

    public DbRowsReader ExecuteReader(string sql, params SqliteParameter[] args)
    {
        DbRowsReader result = new DbRowsReader();
        connection.Open();
        using (var cmd = new SqliteCommand(sql, connection))
        {
            foreach (var arg in args)
            {
                cmd.Parameters.Add(arg);
            }
            result.Init(cmd.ExecuteReader());
        }
        connection.Close();
        return result;
    }
/// <summary>
/// 使用的例子
/// </summary>
/// <param name="playerId"></param>
/// <param name="loginToken"></param>
/// <param name="onFinish"></param>
    protected void DoGetAuthList(string playerId, string loginToken, UnityAction<List<PlayerAuth>> onFinish)
    {
        var player = ExecuteScalar(@"SELECT COUNT(*) FROM player WHERE id=@playerId AND loginToken=@loginToken",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@loginToken", loginToken));
        if (player == null || (long) player <= 0)
        {
            Debug.LogError("失败逻辑");
        }
        else
        {
            var reader = ExecuteReader(@"SELECT * FROM playerAuth WHERE playerId=@playerId", new SqliteParameter("@playerId", playerId));
            var list = new List<PlayerAuth>();
            while (reader.Read())
            {
                var entry = new PlayerAuth();
                entry.Id = reader.GetString(0);
                entry.PlayerId = reader.GetString(1);
                entry.Type = reader.GetString(2);
                entry.Username = reader.GetString(3);
                entry.Password = reader.GetString(4);
                list.Add(entry);
            }
            PlayerAuth.list = list;
        }
        onFinish(PlayerAuth.list);
    }
}
public class GameServiceErrorCode
{
    private const string ERROR_PREFIX = "ERROR_";
    public const string UNKNOW = ERROR_PREFIX + "UNKNOW";
    public const string EMPTY_USERNMAE_OR_PASSWORD = ERROR_PREFIX + "EMPTY_USERNMAE_OR_PASSWORD";
    public const string EXISTED_USERNAME = ERROR_PREFIX + "EXISTED_USERNAME";
    public const string EMPTY_PROFILE_NAME = ERROR_PREFIX + "EMPTY_PROFILE_NAME";
    public const string EXISTED_PROFILE_NAME = ERROR_PREFIX + "EXISTED_PROFILE_NAME";
    public const string INVALID_USERNMAE_OR_PASSWORD = ERROR_PREFIX + "INVALID_USERNMAE_OR_PASSWORD";
    public const string INVALID_LOGIN_TOKEN = ERROR_PREFIX + "INVALID_LOGIN_TOKEN";
    public const string INVALID_PLAYER_DATA = ERROR_PREFIX + "INVALID_PLAYER_DATA";
    public const string INVALID_PLAYER_ITEM_DATA = ERROR_PREFIX + "INVALID_PLAYER_ITEM_DATA";
    public const string INVALID_ITEM_DATA = ERROR_PREFIX + "INVALID_ITEM_DATA";
    public const string INVALID_FORMATION_DATA = ERROR_PREFIX + "INVALID_FORMATION_DATA";
    public const string INVALID_STAGE_DATA = ERROR_PREFIX + "INVALID_STAGE_DATA";
    public const string INVALID_LOOT_BOX_DATA = ERROR_PREFIX + "INVALID_LOOT_BOX_DATA";
    public const string INVALID_EQUIP_POSITION = ERROR_PREFIX + "INVALID_EQUIP_POSITION";
    public const string INVALID_BATTLE_SESSION = ERROR_PREFIX + "INVALID_BATTLE_SESSION";
    public const string NOT_ENOUGH_SOFT_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_SOFT_CURRENCY";
    public const string NOT_ENOUGH_HARD_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_HARD_CURRENCY";
    public const string NOT_ENOUGH_STAGE_STAMINA = ERROR_PREFIX + "NOT_ENOUGH_STAGE_STAMINA";
    public const string NOT_ENOUGH_ITEMS = ERROR_PREFIX + "NOT_ENOUGH_ITEMS";
    public const string CANNOT_EVOLVE = ERROR_PREFIX + "CANNOT_EVOLVE";
}