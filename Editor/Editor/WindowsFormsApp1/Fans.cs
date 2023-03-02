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
        public int elves { get; set; }
        public int nifflers { get; set; }
        public int wombats { get; set; }

        public Fans(int a,int b, int c, int d, int e)
        {
            goblins = a;
            trolls = b;
            elves = c;
            nifflers = d;
            wombats = e;
        }

    }
}
