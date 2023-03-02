using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Colors
{
    public string primary { get; set; }
    public string secondary { get; set; }

    public Colors(string p, string s)
    {
        primary = p;
        secondary = s;
    }
}
