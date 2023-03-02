using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class FanFoulDetection
    {
        [JsonProperty("elfTeleportation")]
        public float ElfTeleportation { get; set; }
        [JsonProperty("goblinShock")]
        public float GoblinShock { get; set; }
        [JsonProperty("trollRoar")]
        public float TrollRoar { get; set; }
        [JsonProperty("snitchSnatch")]
        public float SnitchSnatch { get; set; }
    }
}
