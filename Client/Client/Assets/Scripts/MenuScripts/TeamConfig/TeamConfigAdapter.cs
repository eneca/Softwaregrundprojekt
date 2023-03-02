using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TeamConfigAdapter
{
    public string name { get; set; }
    public string motto { get; set; }
    public Colors colors { get; set; }
    public string image { get; set; }
    public Fans fans { get; set; }
    public Players players { get; set; }


    public Player[] GetPlayerArray()
    {
        Player[] result = new Player[7];
        result[0] = players.seeker;
        result[1] = players.keeper;
        result[2] = players.chaser1;
        result[3] = players.chaser2;
        result[4] = players.chaser3;
        result[5] = players.beater1;
        result[6] = players.beater2;
        return result;
    }

}
