using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Players
{
    public Player seeker { get; set; }
    public Player keeper { get; set; }
    public Player chaser1 { get; set; }
    public Player chaser2 { get; set; }
    public Player chaser3 { get; set; }
    public Player beater1 { get; set; }
    public Player beater2 { get; set; }


    public Players(Player n1, Player n2, Player n3, Player n4, Player n5, Player n6, Player n7)
    {
        seeker = n1;
        keeper = n2;
        chaser1 = n3;
        chaser2 = n4;
        chaser3 = n5;
        beater1 = n6;
        beater2 = n7;

    }



}
