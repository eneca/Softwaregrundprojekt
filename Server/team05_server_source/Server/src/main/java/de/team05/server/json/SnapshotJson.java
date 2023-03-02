package de.team05.server.json;

public class SnapshotJson {

    public DeltaBroadcastJson lastDeltaBroadcast;
    public String phase;
    public String[] spectatorUserName;
    public Integer round;
    public TeamSnapshotJson leftTeam;
    public TeamSnapshotJson rightTeam;
    public BallsJson balls;
    public WombatCubesJson[] wombatCubes;

}
