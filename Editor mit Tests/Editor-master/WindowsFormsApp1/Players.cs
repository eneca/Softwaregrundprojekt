using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    
    /*Speichern aller Spieler*/

    [Serializable]
    class Players
    {
        
        [JsonProperty("seeker")]
        public Player Seeker;
        [JsonProperty("keeper")]
        public Player Keeper;
        [JsonProperty("chaser1")]
        public Player Chaser1;
        [JsonProperty("chaser2")]
        public Player Chaser2;
        [JsonProperty("chaser3")]
        public Player Chaser3;
        [JsonProperty("beater1")]
        public Player Beater1;
        [JsonProperty("beater2")]
        public Player Beater2;

        [JsonConstructor]
        public Players() { }

        public Players(Data data) {
            Seeker = new Player(data.name[0], data.broom[0], data.sex[0]);
            Keeper = new Player(data.name[1], data.broom[1], data.sex[1]);
            Chaser1 = new Player(data.name[2], data.broom[2], data.sex[2]);
            Chaser2 = new Player(data.name[3], data.broom[3], data.sex[3]);
            Chaser3 = new Player(data.name[4], data.broom[4], data.sex[4]);
            Beater1 = new Player(data.name[5], data.broom[5], data.sex[5]);
            Beater2 = new Player(data.name[6], data.broom[6], data.sex[6]);
        }

        public Data getData()
        {
            Data data = new Data();

            data.name[0] = Seeker.Name;
            data.broom[0] = Seeker.Broom;
            data.sex[0] = Seeker.Sex;
            data.name[1] = Keeper.Name;
            data.broom[1] = Keeper.Broom;
            data.sex[1] = Keeper.Sex;
            data.name[2] = Chaser1.Name;
            data.broom[2] = Chaser1.Broom;
            data.sex[2] = Chaser1.Sex;
            data.name[3] = Chaser2.Name;
            data.broom[3] = Chaser2.Broom;
            data.sex[3] = Chaser2.Sex;
            data.name[4] = Chaser3.Name;
            data.broom[4] = Chaser3.Broom;
            data.sex[4] = Chaser3.Sex;
            data.name[5] = Beater1.Name;
            data.broom[5] = Beater1.Broom;
            data.sex[5] = Beater1.Sex;
            data.name[6] = Beater2.Name;
            data.broom[6] = Beater2.Broom;
            data.sex[6] = Beater2.Sex;

            return data;
        }
    }
}
