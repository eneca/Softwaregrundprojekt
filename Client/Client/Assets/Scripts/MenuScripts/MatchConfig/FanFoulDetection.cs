using Newtonsoft.Json;
using System;


[Serializable]
public class FanFoulDetection
{
    [JsonProperty("elfTeleportation")]
    public float ElfTeleportation { get; set; }
    [JsonProperty("goblinShock")]
    public float GoblinShock { get; set; }
    [JsonProperty("trollRoar")]
    public float TrollRoar { get; set; }
    [JsonProperty("snitchSnatch")]
    public float SnitchSnatch { get; set; }
    [JsonProperty("wombatPoo")]
    public float WombatPoo { get; set; }
}