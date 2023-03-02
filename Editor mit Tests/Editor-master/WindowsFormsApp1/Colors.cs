using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class Colors
    {
        [JsonProperty("primary")]
        public string Primary { get; set; }
        [JsonProperty("secondary")]
        public string Secondary { get; set; }

        [JsonConstructor]
        public Colors(string primary, string secondary)
        {
            this.Primary = primary;
            this.Secondary = secondary;
        }

        /*Komstruktor*/
        public Colors(Color c1, Color c2)
        {
            Primary = c1.ToArgb().ToString("X").Substring(2, 6);
            Secondary = c2.ToArgb().ToString("X").Substring(2, 6);
        }
        /*Getter für erste Farbe*/
        public Color GetPrimaryColor()
        {
            Primary = "#" + Primary;
            return ColorTranslator.FromHtml(Primary);
        }
        /*Getter für zweite Farbe*/
        public Color GetSecondaryColor()
        {
            Secondary = "#" + Secondary;
            return ColorTranslator.FromHtml(Secondary);
        }

    }
}
