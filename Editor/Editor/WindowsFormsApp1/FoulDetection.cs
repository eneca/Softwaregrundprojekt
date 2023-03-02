using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class FoulDetection
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
}
