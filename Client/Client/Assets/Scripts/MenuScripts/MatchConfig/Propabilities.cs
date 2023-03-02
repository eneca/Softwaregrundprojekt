using Newtonsoft.Json;
using System;


[Serializable]
public class Propabilities
{
    [JsonProperty("goal")]
    public float Goal;
    [JsonProperty("throwSuccess")]
    public float ThrowSuccess;
    [JsonProperty("knockOut")]
    public float KnockOut;
    [JsonProperty("catchSnitch")]
    public float CatchSnitch;
    [JsonProperty("catchQuaffle")]
    public float CatchQuaffle;
    [JsonProperty("wrestQuaffle")]
    public float WrestQuaffle;
    [JsonProperty("extraMove")]
    public ExtraMove ExtraMove { get; set; }

    public Propabilities()
    {
        ExtraMove = new ExtraMove();
    }
}

