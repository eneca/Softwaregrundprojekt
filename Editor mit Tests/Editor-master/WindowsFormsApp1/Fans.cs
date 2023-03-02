using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /*Klasse zum speichern der Fans*/
    [Serializable]
    class Fans
    {
        public int goblins { get; set; }
        public int trolls { get; set; }
        public int elfs { get; set; }
        public int nifflers { get; set; }

        public Fans(int a,int b, int c, int d)
        {
            goblins = a;
            trolls = b;
            elfs = c;
            nifflers= d;
        }

    }
}
