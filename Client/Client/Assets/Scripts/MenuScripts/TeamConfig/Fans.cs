using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Fans
{
    public int goblins { get; set; }
    public int trolls { get; set; }
    public int elfs { get; set; }
    public int niffler { get; set; }
    public int wombats { get; set; }

    public Fans(int g,int t, int e, int n, int w)
    {
        goblins = g;
        trolls = t;
        elfs = e;
        niffler = n;
        wombats = w;
    }
}
