using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /*Klasse zum serializen (richtig formatiert)*/
    [Serializable]
    class OutputData
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("motto")]
        public string Motto;
        [JsonProperty("colors")]
        public Colors color;
        [JsonProperty("image")]
        public string Image;
        [JsonProperty("fans")]
        public Fans Fans;
        [JsonProperty("players")]
        public Players Players;

        public OutputData(string name, string motto, Colors color, string image, Fans fans, Players players) {
            this.Name = name;
            this.Motto = motto;
            this.color = color;
            this.Image = image;
            this.Fans = fans;
            this.Players = players;
        }

    }
}
