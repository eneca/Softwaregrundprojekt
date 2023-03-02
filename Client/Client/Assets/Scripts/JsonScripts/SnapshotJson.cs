using System;

[Serializable]
public class SnapshotJson
{
    public DeltaBroadcastJson lastDeltaBroadcast;
    public string phase;
    public string[] spectatorUserName;
    public int round;
    public TeamSnapshotJson leftTeam;
    public TeamSnapshotJson rightTeam;
    public BallsJson balls;
    public WombatCubesJson[] wombatCubes;
}

