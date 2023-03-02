using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono
{
    public TestMono()
    {

    }
    public string ResolveNext(string turnType, string s2, bool leftSide, bool userTurn)
    {
        /*
         Next:
         • move
         • action
         • fan
         • removeBan
        */
        string ret = "";
        if (leftSide)
        {
            switch (s2)
            {
                case "leftKeeper":
                    ret = "Keeper";
                    break;
                case "leftChaser1":
                    ret="Chaser1";
                    break;
                case "leftChaser2":
                    ret="Chaser2";
                    break;
                case "leftChaser3":
                    ret="Chaser3";
                    break;
                case "leftBeater1":
                    ret = "Beater1";
                    break;
                case "leftBeater2":
                    ret = "Beater2";
                    break;
                case "leftSeeker":
                    ret = "Seeker";
                    break;
                case "leftNiffler":
                case "leftGoblin":
                case "leftWombat":
                case "leftElf":
                case "leftTroll":
                    ret = s2;
                    break;
                default:
                    ret = "";
                    break;
            }
        }
        else
        {
            switch (s2)
            {
                case "rightSeeker":
                    ret ="Seeker";
                    break;
                case "rightKeeper":
                    ret = "Keeper";
                    break;
                case "rightChaser2":
                    ret = "Chaser2";
                    break;
                case "rightChaser3":
                    ret = "Chaser3";
                    break;
                case "rightChaser1":
                    ret = "Chaser1";
                    break;
                case "rightBeater2":
                    ret = "Beater2";
                    break;
                case "rightBeater1":
                    ret = "Beater1";
                    break;
                case "rightNiffler":
                case "rightGoblin":
                case "rightWombat":
                case "rightElf":
                case "rightTroll":
                    ret = s2;
                    break;
                default:
                    ret = "";
                    break;
            }
        }
        if (userTurn)
        {
            switch (turnType)
            {
                case "move":
                    ret = ret + ":move";
                    break;
                case "fan":
                    ret = ret + ":fan";
                    break;
                case "action":
                    ret = ret + ":action";
                    break;
                case "removeBan":
                    ret = ret + ":removeBan";
                    break;
                default:
                    break;
            }
        }
        return ret;
    }

    public bool CheckCircle(Vector3Int pos)
    {

        if (pos == new Vector3Int(7, -5, 0))
            return true;
        if (pos == new Vector3Int(7, -6, 0))
            return true;
        if (pos == new Vector3Int(7, -7, 0))
            return true;

        if (pos == new Vector3Int(8, -5, 0))
            return true;
        if (pos == new Vector3Int(8, -6, 0))
            return true;
        if (pos == new Vector3Int(8, -7, 0))
            return true;

        if (pos == new Vector3Int(9, -5, 0))
            return true;
        if (pos == new Vector3Int(9, -6, 0))
            return true;
        if (pos == new Vector3Int(9, -7, 0))
            return true;

        return false;
    }
}
