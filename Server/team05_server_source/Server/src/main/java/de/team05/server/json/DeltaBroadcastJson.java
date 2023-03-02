package de.team05.server.json;

public class DeltaBroadcastJson extends LogPayloadJson {

    public String deltaType;
    public Boolean success;
    public Integer xPosOld;
    public Integer yPosOld;
    public Integer xPosNew;
    public Integer yPosNew;
    public String activeEntity;
    public String passiveEntity;
    public String phase;
    public Integer leftPoints;
    public Integer rightPoints;
    public Integer round;
    public String banReason;

    public DeltaBroadcastJson(String deltaType, Boolean success, Integer xPosOld, Integer yPosOld, Integer xPosNew, Integer yPosNew, String activeEntity, String passiveEntity) {
        this.deltaType = deltaType;
        this.success = success;
        this.xPosOld = xPosOld;
        this.yPosOld = yPosOld;
        this.xPosNew = xPosNew;
        this.yPosNew = yPosNew;
        this.activeEntity = activeEntity;
        this.passiveEntity = passiveEntity;
    }

    @Override
    public String toString() {
        return "DeltaBroadcastJson{" +
                "deltaType='" + deltaType + '\'' +
                ", success=" + success +
                ", xPosOld=" + xPosOld +
                ", yPosOld=" + yPosOld +
                ", xPosNew=" + xPosNew +
                ", yPosNew=" + yPosNew +
                ", activeEntity='" + activeEntity + '\'' +
                ", passiveEntity='" + passiveEntity + '\'' +
                ", phase='" + phase + '\'' +
                ", leftPoints=" + leftPoints +
                ", rightPoints=" + rightPoints +
                ", round=" + round +
                ", banReason='" + banReason + '\'' +
                '}';
    }
}
