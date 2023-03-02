package de.team05.server.json;

public class LogJson {
    public String timestamp;
    public String payloadType;
    public LogPayloadJson payload;

    public LogJson(String timestamp, String payloadType, LogPayloadJson payload) {
        this.timestamp = timestamp;
        this.payloadType = payloadType;
        this.payload = payload;
    }

    @Override
    public String toString() {
        return "LogJson{" +
                "timestamp='" + timestamp + '\'' +
                ", payloadType='" + payloadType + '\'' +
                ", payload=" + payload +
                '}';
    }
}
