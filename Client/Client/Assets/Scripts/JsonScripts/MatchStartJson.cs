using System;

[Serializable]
public class MatchStartJson
{
    public MatchConfigJson matchConfigJson;
    public TeamConfigJson leftTeamConfigJson;
    public TeamConfigJson rightTeamConfigJson;
    public string leftTeamUserName;
    public string rightTeamUserName;
}
