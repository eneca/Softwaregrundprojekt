using System;

[Serializable]
public class TeamConfigJson
{

    public string name;
    public string motto;
    public ColorJson colors;
    public string image;
    public FanJson fans;
    public PlayersTeamConfigJson players;

    [Serializable]
    public class PlayersTeamConfigJson
    {
        public PlayerTeamConfigJson seeker;
        public PlayerTeamConfigJson keeper;
        public PlayerTeamConfigJson chaser1;
        public PlayerTeamConfigJson chaser2;
        public PlayerTeamConfigJson chaser3;
        public PlayerTeamConfigJson beater1;
        public PlayerTeamConfigJson beater2;

        [Serializable]
        public class PlayerTeamConfigJson
        {

            public string name;
            public string broom;
            public string sex;
        }
    }
}


