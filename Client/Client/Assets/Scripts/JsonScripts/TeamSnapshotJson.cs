using System;

[Serializable]
public class TeamSnapshotJson
{
    public int points;
    public FanSnapshotJson[] fans;
    public PlayersSnapshotJson players;
}