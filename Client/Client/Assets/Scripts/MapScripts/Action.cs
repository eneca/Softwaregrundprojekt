using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    /// <summary>
    /// Documentation same as Move.cs
    /// </summary>
    public GameObject gLoop;
    public GameObject menuInstance;
    public GameObject seSc;

    private Player[] allPlayersOwnTeam;
    private Player[] allPlayersOpposingTeam;
    private Balls[] allballs;
    private Player selectedPlayer;
    private Vector3Int lastPosClicked;
    private bool currentlyEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        //gLoop.GetComponent<GameLoop>().UpdateInfos(allPlayersOwnTeam,allPlayersOpposingTeam,allballs);
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gLoop.GetComponent<GameLoop>().userTurn && gLoop.GetComponent<GameLoop>().turnType.Equals("action") && Input.GetMouseButtonDown(0))
        {
            if (gLoop.GetComponent<GameLoop>().InMoveRange(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition)) || gLoop.GetComponent<GameLoop>().InThrowRange(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition)))
            {
                UpdateInfos();
                lastPosClicked = gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition);
                gLoop.GetComponent<GameLoop>().lastPosClicked = lastPosClicked;
                WhatHasBeenClickedAction(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition));
            }
        }
    }
    
    void WhatHasBeenClickedAction(Vector3Int gridPosV3)
    {
        if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3) || gLoop.GetComponent<GameLoop>().InThrowRange(gridPosV3))
        {
            if (selectedPlayer.hasBall)
            {
                if (gLoop.GetComponent<GameLoop>().InThrowRange(gridPosV3))
                {
                    SelectedThrow(gridPosV3);
                }
                else if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3))
                {
                    //TODO Decision if Foul or Throw
                    //menuInstance.GetComponent<Menu>().MoveDecision(true, gridPosV3);
                    menuInstance.GetComponent<Menu>().SetActiveMoveDecision(true);
                }
            }
            else
            {
                //TODO Check Foul
                bool clickedOwnPlayer = false;
                foreach (Player p in allPlayersOwnTeam)
                {
                    if (p.pos == gridPosV3)
                    {
                        clickedOwnPlayer = true;
                        break;
                    }
                }
                if (!clickedOwnPlayer)
                {
                    bool clickedEnemy = false;
                    foreach (Player p in allPlayersOpposingTeam)
                    {
                        if (p.pos == gridPosV3)
                        {
                            clickedEnemy = true;
                            break;
                        }
                    }
                    if (clickedEnemy)
                    {
                        if ((selectedPlayer.playerRole == Role.Beater1 || selectedPlayer.playerRole == Role.Beater2 || selectedPlayer.playerRole == Role.Seeker) && gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3))
                        {
                            seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", lastPosClicked.x, lastPosClicked.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
                        }
                        else
                        {
                            bool balled = false;
                            foreach (Balls b in allballs)
                            {
                                if (b.pos == gridPosV3 && b.type == Ball.quaffle)
                                {
                                    if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3))
                                    {
                                        //menuInstance.GetComponent<Menu>().MoveDecision(true, gridPosV3);
                                        menuInstance.GetComponent<Menu>().SetActiveMoveDecision(true);
                                        balled = true;
                                        break;
                                    }
                                }
                                if (b.pos == gridPosV3)
                                {
                                    break;
                                }
                            }
                            if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3) && !balled)
                            {
                                seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", lastPosClicked.x, lastPosClicked.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
                            }
                        }
                    }
                }
            }
        }
    }

    void UpdateInfos()
    {
        allPlayersOwnTeam = gLoop.GetComponent<GameLoop>().allPlayersOwnTeam;
        allPlayersOpposingTeam = gLoop.GetComponent<GameLoop>().allPlayersOpposingTeam;
        allballs = gLoop.GetComponent<GameLoop>().allballs;
        selectedPlayer = gLoop.GetComponent<GameLoop>().selectedPlayer;
        if (selectedPlayer != null)
        {
            gLoop.GetComponent<GameLoop>().DeleteMarks();
            if (selectedPlayer.hasBall) { gLoop.GetComponent<GameLoop>().DisplayThrowRange(selectedPlayer.pos); }
            gLoop.GetComponent<GameLoop>().DisplayMoveRange(selectedPlayer.pos);
        }
    }

    public void SelectedThrow(Vector3Int gridPosV3)
    {
        string thrownball = "";
        switch (selectedPlayer.playerRole)
        {
            case Role.Keeper:
                ThrowQuaffle(gridPosV3);
                break;
            case Role.Chaser1:
            case Role.Chaser2:
            case Role.Chaser3:
                ThrowQuaffle(gridPosV3);
                break;
            case Role.Beater1:
            case Role.Beater2:
                foreach (Balls b in allballs)
                {
                    if (b.pos == selectedPlayer.pos)
                    {
                        thrownball = b.type.ToString();
                    }
                }
                ThrowBludger(gridPosV3, thrownball);
                break;
            default:
                break;
        }
    }
    #region DeltaRequests
    public void ThrowQuaffle(Vector3Int pos)
    {
        menuInstance.GetComponent<Menu>().AddLog("Throw quaffle");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("quaffleThrow", pos.x, pos.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
    }
    public void ThrowQuaffle()
    {
        menuInstance.GetComponent<Menu>().AddLog("Throw quaffle");
        allballs[0].pos = lastPosClicked;
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("quaffleThrow", lastPosClicked.x, lastPosClicked.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
        gLoop.GetComponent<GameLoop>().TurnFalse();
    }
    public void ThrowBludger(Vector3Int pos, string b)
    {
        menuInstance.GetComponent<Menu>().AddLog("BludgerBeating");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("bludgerBeating", pos.x, pos.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()), b);
    }
    public void WrestQuaffle()
    {
        menuInstance.GetComponent<Menu>().AddLog("WrestQuaffle");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("wrestQuaffle", gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
    }
    //Move selected palyer to pos
    public void MoveToPosition(Vector3Int pos)
    {
        Vector3Int pp = new Vector3Int(pos.x, pos.y, 0);
        selectedPlayer.pos = pp;
        gLoop.GetComponent<GameLoop>().DeleteMarks();
        gLoop.GetComponent<GameLoop>().DisplayThrowRange(pp);
        gLoop.GetComponent<GameLoop>().DisplayMoveRange(pp);
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", pos.x, pos.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
        menuInstance.GetComponent<Menu>().AddLog("Move Entity");
    }
    #endregion Deltarequests

    public void ButtonSelectedThrow()
    {
        UpdateInfos();
        if (currentlyEnabled)
        {
            if (selectedPlayer.hasBall)
            {
                Vector3Int gridPosV3 = lastPosClicked;
                string thrownball = "";
                switch (selectedPlayer.playerRole)
                {
                    case Role.Keeper:
                        ThrowQuaffle(gridPosV3);
                        break;
                    case Role.Chaser1:
                    case Role.Chaser2:
                    case Role.Chaser3:
                        ThrowQuaffle(gridPosV3);
                        break;
                    case Role.Beater1:
                    case Role.Beater2:
                        foreach (Balls b in allballs)
                        {
                            if (b.pos == selectedPlayer.pos)
                            {
                                thrownball = b.type.ToString();
                            }
                        }
                        ThrowBludger(gridPosV3, thrownball);
                        break;
                    default:
                        break;
                }
                //menuInstance.GetComponent<Menu>().MoveDecision(false, new Vector3Int(0, 0, 0));
                menuInstance.GetComponent<Menu>().SetActiveMoveDecision(false);
            }
            else
            {
                WrestQuaffle();
                //menuInstance.GetComponent<Menu>().MoveDecision(false, new Vector3Int(0, 0, 0));
                menuInstance.GetComponent<Menu>().SetActiveMoveDecision(false);
            }
        }
    }
    public void ButtonMoveToPos()
    {
        if (currentlyEnabled)
        {
            gLoop.GetComponent<GameLoop>().UpdateInfos(allPlayersOwnTeam, allPlayersOpposingTeam, allballs);
            //menuInstance.GetComponent<Menu>().MoveDecision(false, new Vector3Int(0, 0, 0));
            menuInstance.GetComponent<Menu>().SetActiveMoveDecision(false);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", lastPosClicked.x, lastPosClicked.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
            menuInstance.GetComponent<Menu>().AddLog("Move Entity");
        }
    }
    public void ScriptStatus(bool active)
    {
        if (active)
        {
            UpdateInfos();
        }
        currentlyEnabled = active;
        this.enabled = active;
    }
}
