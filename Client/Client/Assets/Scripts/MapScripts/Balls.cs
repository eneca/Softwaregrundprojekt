using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Balls
{
    public Vector3Int pos { get; set; }
    public Ball type { get; set; }
    public bool pickedUp { get; set; }





    public Balls()
    {
        pickedUp = false;
    }

    public Balls(Ball b)
    {
        pickedUp = false;
        type = b;
    }
}
