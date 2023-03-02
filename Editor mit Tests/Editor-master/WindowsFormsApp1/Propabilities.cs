using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class Propabilities
    {
        [JsonProperty("goal")]
        public float Goal;
        [JsonProperty("throwSuccess")]
        public float ThrowSuccess;
        [JsonProperty("knockOut")]
        public float KnockOut;
        [JsonProperty("foolAway")]
        public float FoolAway;
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
}
