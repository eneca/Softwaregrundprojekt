using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class Timeouts
    {
        [JsonProperty("playerTurnTimeout")]
        public int PlayerTurnTimeout { get; set; }
        [JsonProperty("fanTurnTimeout")]
        public int FanTurnTimeout { get; set; }
        [JsonProperty("playerPhaseTime")]
        public int PlayerPhaseTime { get; set; }
        [JsonProperty("fanPhaseTime")]
        public int FanPhaseTime { get; set; }
        [JsonProperty("ballPhaseTime")]
        public int BallPhaseTime { get; set; }
    }
}
