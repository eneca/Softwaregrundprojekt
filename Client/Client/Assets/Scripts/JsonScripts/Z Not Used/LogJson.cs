using System;

/*ZU machen*/

[Serializable]
public class LogJson
{
    public string timestamp;
    public string payloadType;
    public LogPayloadJson payload;

    //public LogJson(string timestamp, string payloadType, LogPayloadJson payload)
    //{
    //    this.timestamp = timestamp;
    //    this.payloadType = payloadType;
    //    this.payload = payload;
    //}

    //public override string ToString()
    //{
    //    return "LogJson{" +
    //            "timestamp='" + timestamp + '\'' +
    //            ", payloadType='" + payloadType + '\'' +
    //            ", payload=" + payload +
    //            '}';
    //}
}
