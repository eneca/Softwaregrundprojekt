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
        [JsonProperty("teamFormationTimeout")]
        public int TeamFormationTimeout { get; set; }
        [JsonProperty("playerTurnTimeout")]
        public int PlayerTurnTimeout { get; set; }
        [JsonProperty("fanTurnTimeout")]
        public int FanTurnTimeout { get; set; }
        [JsonProperty("unbanTurnTimeout")]
        public int UnbanTurnTimeout { get; set; }
        [JsonProperty("minPlayerPhaseAnimationDuration")]
        public int MinPlayerPhaseAnimationDuration { get; set; }
        [JsonProperty("minFanPhaseAnimationDuration")]
        public int MinFanPhaseAnimationDuration { get; set; }
        [JsonProperty("minBallPhaseAnimationDuration")]
        public int MinBallPhaseAnimationDuration { get; set; }
        [JsonProperty("minUnbanPhaseAnimationDuration")]
        public int MinUnbanPhaseAnimationDuration { get; set; }
    }
}
