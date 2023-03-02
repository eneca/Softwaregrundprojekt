using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class ExtraMove
    {
        [JsonProperty("thinderblast")]
        public float thinderblast;
        [JsonProperty("cleansweep11")]
        public float cleansweep11;
        [JsonProperty("comet260")]
        public float comet260;
        [JsonProperty("nimbus2001")]
        public float nimbus2001;
        [JsonProperty("firebolt")]
        public float firebolt;
    }
}
