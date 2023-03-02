package de.team05.server.json;

public class ReplayJson {

    public String lobby;
    public String startTimestamp;
    public MatchConfigJson matchConfig;
    public TeamConfigJson leftTeamConfig;
    public TeamConfigJson rightTeamConfig;
    public String leftTeamUserName;
    public String rightTeamUserName;
    public String[] spectatorUserName;
    public SnapshotJson firstSnapshot;
    public LogJson[] log;

    public ReplayJson(String lobby, String startTimestamp, MatchConfigJson matchConfig, TeamConfigJson leftTeamConfig,
                      TeamConfigJson rightTeamConfig, String leftTeamUserName, String rightTeamUserName,
                      String[] spectatorUserName, SnapshotJson firstSnapshot, LogJson[] log) {
        this.lobby = lobby;
        this.startTimestamp = startTimestamp;
        this.matchConfig = matchConfig;
        this.leftTeamConfig = leftTeamConfig;
        this.rightTeamConfig = rightTeamConfig;
        this.leftTeamUserName = leftTeamUserName;
        this.rightTeamUserName = rightTeamUserName;
        this.spectatorUserName = spectatorUserName;
        this.firstSnapshot = firstSnapshot;
        this.log = log;
    }

}
