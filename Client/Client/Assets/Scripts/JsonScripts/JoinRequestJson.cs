using System;

[Serializable]
public class JoinRequestJson
{
    public string lobby;
    public string userName;
    public string password;
    public bool isArtificialIntelligence;
    public string[] mods;

}
