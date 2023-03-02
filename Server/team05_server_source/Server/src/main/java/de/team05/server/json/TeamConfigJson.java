package de.team05.server.json;

public class TeamConfigJson {

    public String name;
    public String motto;
    public ColorJson colors;
    public String image;
    public FanJson fans;
    public PlayersTeamConfigJson players;

    public class PlayersTeamConfigJson {
        public PlayerTeamConfigJson seeker;
        public PlayerTeamConfigJson keeper;
        public PlayerTeamConfigJson chaser1;
        public PlayerTeamConfigJson chaser2;
        public PlayerTeamConfigJson chaser3;
        public PlayerTeamConfigJson beater1;
        public PlayerTeamConfigJson beater2;

        public class PlayerTeamConfigJson {

            public String name;
            public String broom;
            public String sex;
        }
    }
}


