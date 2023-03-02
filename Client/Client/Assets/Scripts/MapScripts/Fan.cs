using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public GameObject seSc;
    public GameObject menuInstance;
    public GameObject gLoop;
    public GameObject sound;

    private Player[] allPlayersOwnTeam;
    private Player[] allPlayersOpposingTeam;
    private Balls[] allballs;
    private CheckScript cs = new CheckScript();

    private bool fanButtonCLicked = false;
    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    //checks for click
    void Update()
    {
        if (gLoop.GetComponent<GameLoop>().userTurn && gLoop.GetComponent<GameLoop>().turnType.Equals("fan") && Input.GetMouseButtonDown(0))
        {
            WhatHasBeenClickedFan(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition));
        }
    }

    void WhatHasBeenClickedFan(Vector3Int gridPosV3)
    {
        if (fanButtonCLicked)
        {
            switch (gLoop.GetComponent<GameLoop>().selectedFan.ToString())
            {
                case "rightGoblin":
                case "leftGoblin":
                    GoblinAction(gridPosV3);
                    break;
                case "leftWombat":
                case "rightWombat":
                    WombatAction(gridPosV3);
                    break;
                case "leftElf":
                case "rightElf":
                    ElveAction(gridPosV3);
                    break;
                default:
                    break;
            }
        }
    }
    //updates info
    void UpdateInfos()
    {
        allPlayersOwnTeam = gLoop.GetComponent<GameLoop>().allPlayersOwnTeam;
        allPlayersOpposingTeam = gLoop.GetComponent<GameLoop>().allPlayersOpposingTeam;
        allballs = gLoop.GetComponent<GameLoop>().allballs;
    }
    #region FanButtons
    public void TrollButton()
    {
        if (gLoop.GetComponent<GameLoop>().turnType.Equals("fan"))
        {
            UpdateInfos();
            sound.GetComponent<SoundEffects>().PlaySound(4);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("trollRoar");
            menuInstance.GetComponent<Menu>().DeactivateAllFans();
            gLoop.GetComponent<GameLoop>().TurnFalse();
        }
    }
    public void WombatButton()
    {
        if (gLoop.GetComponent<GameLoop>().turnType.Equals("fan"))
        {
            UpdateInfos();
            fanButtonCLicked = true;
        }
    }

    void WombatAction(Vector3Int gridPosV3)
    {
        bool notValid = false;
        for (int i = 0; i <= 6; i++)
        {
            if (allPlayersOwnTeam[i].pos == gridPosV3 || allPlayersOpposingTeam[i].pos == gridPosV3)
            {
                notValid = true;
            }
        }
        foreach (Balls i in allballs)
        {
            if (i.pos == gridPosV3)
            {
                notValid = true;
                break;
            }
        }
        foreach (Vector2Int vec in gLoop.GetComponent<GameLoop>().wombatPoo)
        {
            if (gridPosV3.x == vec.x && gridPosV3.y == vec.y)
            {
                notValid = true;
                break;
            }
        }
        if (!notValid && !cs.PosIsGoal(gridPosV3))
        {
            sound.GetComponent<SoundEffects>().PlaySound(2);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("wombatPoo", gridPosV3.x, gridPosV3.y);
            menuInstance.GetComponent<Menu>().DeactivateAllFans();
            gLoop.GetComponent<GameLoop>().TurnFalse();
            fanButtonCLicked = false;
        }
        else
        {
            menuInstance.GetComponent<Menu>().Animation("not a valid wombat poo pos");
        }
    }

    public void NifflerButton()
    {
        if (gLoop.GetComponent<GameLoop>().turnType.Equals("fan"))
        {
            UpdateInfos();
            sound.GetComponent<SoundEffects>().PlaySound(6);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("snitchSnatch");
            menuInstance.GetComponent<Menu>().DeactivateAllFans();
            gLoop.GetComponent<GameLoop>().TurnFalse();
        }
    }

    public void ElveButton()
    {
        if (gLoop.GetComponent<GameLoop>().turnType.Equals("fan"))
        {
            UpdateInfos();
            fanButtonCLicked = true;
        }
    }

    void ElveAction(Vector3Int vec)
    {
        for (int i = 0; i <= 6; i++)
        {
            if (allPlayersOpposingTeam[i].pos == vec)
            {
                sound.GetComponent<SoundEffects>().PlaySound(3);
                seSc.GetComponent<SerializeScript>().SendDeltaRequest("elfTeleportation", gLoop.GetComponent<GameLoop>().ConvertPlayerRoleOpposingTeam(allPlayersOpposingTeam[i].playerRole.ToString()));
                menuInstance.GetComponent<Menu>().DeactivateAllFans();
                gLoop.GetComponent<GameLoop>().TurnFalse();
                fanButtonCLicked = false;
                break;
            }
            else if (allPlayersOwnTeam[i].pos == vec)
            {
                sound.GetComponent<SoundEffects>().PlaySound(3);
                seSc.GetComponent<SerializeScript>().SendDeltaRequest("elfTeleportation", gLoop.GetComponent<GameLoop>().ConvertPlayerRole(allPlayersOwnTeam[i].playerRole.ToString()));
                menuInstance.GetComponent<Menu>().DeactivateAllFans();
                gLoop.GetComponent<GameLoop>().TurnFalse();
                fanButtonCLicked = false;
                break;
            }
        }
    }

    public void GoblinButton()
    {
        if (gLoop.GetComponent<GameLoop>().turnType.Equals("fan"))
        {
            UpdateInfos();
            fanButtonCLicked = true;
        }
    }

    void GoblinAction(Vector3Int gridPos)
    {
        for (int i = 0; i <= 6; i++)
        {
            if (allPlayersOpposingTeam[i].pos == gridPos)
            {
                sound.GetComponent<SoundEffects>().PlaySound(5);
                gLoop.GetComponent<GameLoop>().TurnFalse();
                seSc.GetComponent<SerializeScript>().SendDeltaRequest("goblinShock", gLoop.GetComponent<GameLoop>().ConvertPlayerRoleOpposingTeam(allPlayersOpposingTeam[i].playerRole.ToString()));
                menuInstance.GetComponent<Menu>().DeactivateAllFans();
                fanButtonCLicked = false;
                break;
            }
        }
    }
    #endregion FanButtons
    //sets status
    public void ScriptStatus(bool active)
    {
        this.enabled = active;
    }
}
