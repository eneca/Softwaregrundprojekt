using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /*Klasse zum speichern der Player*/
    class Data
    {
        public string[] name;
        public string[] broom;
        public string[] sex;

        public Data() {
            name = new string[7];
            broom = new string[7];
            sex = new string[7];

            for (int x=0;x<7;x++) {
                name[x]= "";
                broom[x] = "";
                sex[x] = "";
            }
        }
        public Data(string[] n, string[] b, string[] s)
        {
            name = n;
            broom = b;
            sex = s;
        }
    }
}
