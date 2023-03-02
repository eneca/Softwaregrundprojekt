using System;

[Serializable]
public class DeltaRequestJson
{
    public string deltaType;
    public bool success;
    public int xPosOld;
    public int yPosOld;
    public int xPosNew;
    public int yPosNew;
    public string activeEntity;
    public string passiveEntity;
    public string phase;
    public int leftPoints;
    public int rightPoints;
    public int round;
    public string banReason;
}
