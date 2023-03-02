using System;

[Serializable]
public class ReplayJson
{
    public string lobby;
    public string startTimestamp;
    public MatchConfigJson matchConfig;
    public TeamConfigJson leftTeamConfig;
    public TeamConfigJson rightTeamConfig;
    public string leftTeamUserName;
    public string rightTeamUserName;
    public string[] spectatorUserName;
    public SnapshotJson firstSnapshot;
    public LogJson[] log;

    //public ReplayJson(string lobby, string startTimestamp, MatchConfigJson matchConfig, TeamConfigJson leftTeamConfig,
    //                  TeamConfigJson rightTeamConfig, string leftTeamUserName, string rightTeamUserName,
    //                  string[] spectatorUserName, SnapshotJson firstSnapshot, LogJson[] log)
    //{
    //    this.lobby = lobby;
    //    this.startTimestamp = startTimestamp;
    //    this.matchConfig = matchConfig;
    //    this.leftTeamConfig = leftTeamConfig;
    //    this.rightTeamConfig = rightTeamConfig;
    //    this.leftTeamUserName = leftTeamUserName;
    //    this.rightTeamUserName = rightTeamUserName;
    //    this.spectatorUserName = spectatorUserName;
    //    this.firstSnapshot = firstSnapshot;
    //    this.log = log;
    //}
}
