using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScript : MonoBehaviour
{
    public Tilemap tmap;
    // Start is called before the first frame update
    void Start()
    {

    }
    /*
    void OnClickAction(Vector2 mousePos)
    {
        print("ON CLICK: mouse pos: " + mousePos);
    }
    */
    // Update is called once per frame
    void Update()
    {
        //x=-48 y = -6.3
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int pp;
            Vector3 feck = Input.mousePosition;
            feck.x = feck.x + 48f;
            feck.y = feck.y + 6.3f;
            pp = Vector3Int.FloorToInt(feck);
            print("click mich haerter" + pp);
            print("was zum fick" + feck);

            tmap.SetTileFlags(pp, TileFlags.None);
            tmap.SetColor(pp, Color.red);
        }
    }
}
