using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAuth
{
    public static List<PlayerAuth> list = new List<PlayerAuth>();
    public static readonly Dictionary<string, PlayerAuth> DataMap = new Dictionary<string, PlayerAuth>();
    public string Id { get { return GetId(PlayerId, Type); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string type;
    public string Type { get { return type; } set { type = value; } }
    public string username;
    public string Username { get { return username; } set { username = value; } }
    public string password;
    public string Password { get { return password; } set { password = value; } }
    



    public static string GetId(string playerId, string type)
    {
        return playerId + "_" + type;
    }

}
