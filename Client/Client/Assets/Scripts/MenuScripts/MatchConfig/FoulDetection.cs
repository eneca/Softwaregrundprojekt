using Newtonsoft.Json;
using System;


[Serializable]
public class FoulDetection
{
    [JsonProperty("flacking")]
    public float Flacking { get; set; }
    [JsonProperty("haversacking")]
    public float Haversacking { get; set; }
    [JsonProperty("stooging")]
    public float Stooging { get; set; }
    [JsonProperty("blatching")]
    public float Blatching { get; set; }
    [JsonProperty("snitchnip")]
    public float Snitchnip { get; set; }
}

