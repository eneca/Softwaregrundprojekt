using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /*Klasse zum speichern der Daten eines Spielers*/
    [Serializable]
    class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("broom")]
        public string Broom { get; set; }
        [JsonProperty("sex")]
        public string Sex { get; set; }

        public Player(string name, string broom, string sex) {
            this.Name = name;
            this.Broom = broom;
            this.Sex = sex;
        }

    }
}
