using System;
using UnityEngine;

public class CheckScript
{
    /// <summary>
    /// Created by Denny Rapp
    /// This class will check if something is valid or not
    /// and can convert certain things like the map coordinates
    /// </summary>
    private Vector2[] goalPos = new Vector2[6];
    public CheckScript()
    {
        goalPos[0] = new Vector2(2, -4);
        goalPos[1] = new Vector2(2, -6);
        goalPos[2] = new Vector2(2, -8);
        goalPos[3] = new Vector2(14, -4);
        goalPos[4] = new Vector2(14, -6);
        goalPos[5] = new Vector2(14, -8);
    }

    public float ConvertToX(Vector3 v)
    {
        return v.x + 8;
    }
    public float ConvertToY(Vector3 v)
    {
        return v.y - 5;
    }
    public Vector2 ConvertToVector2(Vector3 v)
    {
        v.x = v.x + 8;
        v.y = v.y - 5;
        Vector2 v2 = v;
        return v2;
    }
    public bool CanHitGoal(Vector3Int playerPos, Vector3Int goalPos)
    {
        return Math.Abs(playerPos.y - goalPos.y) <= Math.Abs(playerPos.x - goalPos.x);
    }
    public bool PosIsGoal(Vector3Int hit)
    {
        Vector2 vec;
        vec.x = hit.x;
        vec.y = hit.y;
        for (int i = 0; i < 3; i++)
        {
            if (goalPos[i] == vec || goalPos[5 - i] == vec)
            {
                return true;
            }
        }
        return false;
    }

    public int HuntersInDefenseZoneLeft(Player chaser1, Player chaser2, Player chaser3)
    {
        int ret = 0;
        //alles links von x = 2 y =1 y = 11
        //oder x = 3  && zwischen y = 2 y = 10
        //oder x = 4 && y= 4 y = 8

        if (chaser1.pos.x <= 2 || (chaser1.pos.x <= 3 && chaser1.pos.y >= 2 && chaser1.pos.y <= 10) || (chaser1.pos.x <= 4 && chaser1.pos.y >= 4 && chaser1.pos.y <= 8))
        {
            ret++;
        }
        if (chaser2.pos.x <= 2 || (chaser2.pos.x <= 3 && chaser2.pos.y >= 2 && chaser2.pos.y <= 10) || (chaser2.pos.x <= 4 && chaser2.pos.y >= 4 && chaser2.pos.y <= 8))
        {
            ret++;
        }
        if (chaser3.pos.x <= 2 || (chaser3.pos.x <= 3 && chaser3.pos.y >= 2 && chaser3.pos.y <= 10) || (chaser3.pos.x <= 4 && chaser3.pos.y >= 4 && chaser3.pos.y <= 8))
        {
            ret++;
        }
        return ret;
    }
    public int HuntersInDefenseZoneRight(Player chaser1, Player chaser2, Player chaser3)
    {
        int ret = 0;
        if (chaser1.pos.x >= 14 || (chaser1.pos.x >= 13 && chaser1.pos.y >= 2 && chaser1.pos.y <= 10) || (chaser1.pos.x >= 12 && chaser1.pos.y >= 4 && chaser1.pos.y <= 8))
        {
            ret++;
        }
        if (chaser2.pos.x >= 14 || (chaser2.pos.x >= 13 && chaser2.pos.y >= 2 && chaser2.pos.y <= 10) || (chaser2.pos.x >= 12 && chaser2.pos.y >= 4 && chaser2.pos.y <= 8))
        {
            ret++;
        }
        if (chaser3.pos.x >= 14 || (chaser3.pos.x >= 13 && chaser3.pos.y >= 2 && chaser3.pos.y <= 10) || (chaser3.pos.x >= 12 && chaser3.pos.y >= 4 && chaser3.pos.y <= 8))
        {
            ret++;
        }
        return ret;
    }

    public bool SeekerCheckLeft(Vector3Int gridPos)
    {
        if (gridPos.x <= 2 || (gridPos.x <= 3 && gridPos.y >= 2 && gridPos.y <= 10) || (gridPos.x <= 4 && gridPos.y >= 4 && gridPos.y <= 8))
        {
            return false;
        }
        return true;
    }
    public bool SeekerCheckRight(Vector3Int gridPos)
    {
        if (gridPos.x >= 14 || (gridPos.x >= 13 && gridPos.y >= 2 && gridPos.y <= 10) || (gridPos.x >= 12 && gridPos.y >= 4 && gridPos.y <= 8))
        {
            return false;
        }
        return true;
    }
}
