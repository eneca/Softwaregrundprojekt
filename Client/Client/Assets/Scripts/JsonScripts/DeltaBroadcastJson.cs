using System;

[Serializable]
public class DeltaBroadcastJson
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

    //public DeltaBroadcastJson(string deltaType, bool success, int xPosOld, int yPosOld, int xPosNew, int yPosNew, string activeEntity, string passiveEntity)
    //{
    //    this.deltaType = deltaType;
    //    this.success = success;
    //    this.xPosOld = xPosOld;
    //    this.yPosOld = yPosOld;
    //    this.xPosNew = xPosNew;
    //    this.yPosNew = yPosNew;
    //    this.activeEntity = activeEntity;
    //    this.passiveEntity = passiveEntity;
    //}

  
    //public override string ToString()
    //{
    //    return "DeltaBroadcastJson{" +
    //            "deltaType='" + deltaType + '\'' +
    //            ", success=" + success +
    //            ", xPosOld=" + xPosOld +
    //            ", yPosOld=" + yPosOld +
    //            ", xPosNew=" + xPosNew +
    //            ", yPosNew=" + yPosNew +
    //            ", activeEntity='" + activeEntity + '\'' +
    //            ", passiveEntity='" + passiveEntity + '\'' +
    //            ", phase='" + phase + '\'' +
    //            ", leftPoints=" + leftPoints +
    //            ", rightPoints=" + rightPoints +
    //            ", round=" + round +
    //            ", banReason='" + banReason + '\'' +
    //            '}';
    //}

}
