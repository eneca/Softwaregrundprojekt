using Newtonsoft.Json;
using System;


[Serializable]
public class PartieConfig
{
    [JsonProperty("maxRounds")]
    public int MaxRounds { get; set; }
    [JsonProperty("timings")]
    public Timeouts Timeouts { get; set; }
    [JsonProperty("propabilities")]
    public Propabilities Propabilities { get; set; }
    [JsonProperty("foulDetection")]
    public FoulDetection FoulDetection { get; set; }
    [JsonProperty("fanFoulDetection")]
    public FanFoulDetection FanFoulDetection { get; set; }

    public PartieConfig()
    {
        Timeouts = new Timeouts();
        Propabilities = new Propabilities();
        FoulDetection = new FoulDetection();
        FanFoulDetection = new FanFoulDetection();
    }
}