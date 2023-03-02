using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Player
{
    public string name { get; set; }
    public string broom { get; set; }
    public string sex { get; set; }
    public Vector3Int pos { get; set; }
    public bool hasBall { get; set; }
    public Role playerRole { get; set; }
    public bool banned { get; set; }
    public bool knockedOut { get; set; }
    public bool hasMoved { get; set; }

    public Player(string n, string b, string s, Vector3Int v, bool hB, Role pr, bool ban, bool ko)
    {
        name = n;
        broom = b;
        sex = s;
        pos = v;
        hasBall = hB;
        playerRole = pr;
        banned = ban;
        knockedOut = ko;
        hasMoved = false;
        banned = false;
    }

    public Player()
    {
        banned = false;
        knockedOut = false;
        hasBall = false;
        hasMoved = false;
    }

    [JsonConstructor]
    public Player(string n, string b, string s)
    {
        name = n;
        broom = b;
        sex = s;
        banned = false;
        knockedOut = false;
        hasBall = false;
        hasMoved = false;
    }

}