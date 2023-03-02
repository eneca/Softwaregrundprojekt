using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
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
        this.enabled = false;
    }
    // Update is called once per frame
    //checks for click on the map
    void Update()
    {
        if (gLoop.GetComponent<GameLoop>().userTurn && gLoop.GetComponent<GameLoop>().turnType.Equals("move") && Input.GetMouseButtonDown(0))
        {
            if (gLoop.GetComponent<GameLoop>().InMoveRange(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition)) || gLoop.GetComponent<GameLoop>().InThrowRange(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition)))
            {
                UpdateInfos();
                lastPosClicked = gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition);
                gLoop.GetComponent<GameLoop>().lastPosClicked = lastPosClicked;
                WhatHasBeenClickedMove(gLoop.GetComponent<GameLoop>().ConvertToTileMap(Input.mousePosition));
            }
        }
    }
    //checks what has been clicked
    void WhatHasBeenClickedMove(Vector3Int gridPosV3)
    {
        if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3) || gLoop.GetComponent<GameLoop>().InThrowRange(gridPosV3))
        {
            bool clickedOwnPlayer = false;
            foreach (Player p in allPlayersOwnTeam)
            {
                if (p.pos == gridPosV3)
                {
                    clickedOwnPlayer = true;
                }
            }
            if (CheckPoo(gridPosV3))
            {
                clickedOwnPlayer = true;
            }
            if (selectedPlayer.hasBall)
            {
                if (gLoop.GetComponent<GameLoop>().InThrowRange(gridPosV3))
                {
                    SelectedThrow(gridPosV3);
                }
                else if (gLoop.GetComponent<GameLoop>().InMoveRange(gridPosV3))
                {
                    if (!clickedOwnPlayer)
                    {
                        //menuInstance.GetComponent<Menu>().MoveDecision(true, gridPosV3);
                        menuInstance.GetComponent<Menu>().SetActiveMoveDecision(true);
                    }
                }
            }
            else
            {
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
                    else
                    {
                        MoveToPosition(gridPosV3);
                    }
                }
            }
        }
    }
    //gets all infos
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
    //selected throw with button
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
        menuInstance.GetComponent<Menu>().AddLog("Bludger beating");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("bludgerBeating", pos.x, pos.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()), b);
    }
    public void WrestQuaffle()
    {
        menuInstance.GetComponent<Menu>().AddLog("Wrest quaffle");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("wrestQuaffle", gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
    }
    //Move selected palyer to pos
    public void MoveToPosition(Vector3Int pos)
    {
        Vector3Int pp = new Vector3Int(pos.x, pos.y, 0);
        selectedPlayer.pos = pp;
        gLoop.GetComponent<GameLoop>().UpdateInfos(allPlayersOwnTeam, allPlayersOpposingTeam ,allballs);
        gLoop.GetComponent<GameLoop>().DeleteMarks();
        if (selectedPlayer.hasBall) { gLoop.GetComponent<GameLoop>().DisplayThrowRange(pp); }
        gLoop.GetComponent<GameLoop>().DisplayMoveRange(pp);
        menuInstance.GetComponent<Menu>().AddLog("Move Entity");
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", pos.x, pos.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
    }
    #endregion Deltarequests
    //prepares delta for throw if button was clicked
    public void ButtonSelectedThrow()
    {
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
                //menuInstance.GetComponent<Menu>().MoveDecision(false, new Vector3Int(0,0,0));
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
    //Sends move delta if button was selected
    public void ButtonMoveToPos()
    {
        if (currentlyEnabled)
        {
            Vector3Int pp = new Vector3Int(lastPosClicked.x, lastPosClicked.y, 0);
            selectedPlayer.pos = pp;
            gLoop.GetComponent<GameLoop>().UpdateInfos(allPlayersOwnTeam, allPlayersOpposingTeam, allballs);
            gLoop.GetComponent<GameLoop>().DeleteMarks();
            if (selectedPlayer.hasBall) { gLoop.GetComponent<GameLoop>().DisplayThrowRange(pp); }
            gLoop.GetComponent<GameLoop>().DisplayMoveRange(pp);
            menuInstance.GetComponent<Menu>().AddLog("Move Entity");
            gLoop.GetComponent<GameLoop>().UpdateInfos(allPlayersOwnTeam, allPlayersOpposingTeam, allballs);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("move", lastPosClicked.x, lastPosClicked.y, gLoop.GetComponent<GameLoop>().ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
            //menuInstance.GetComponent<Menu>().MoveDecision(false, new Vector3Int(0, 0, 0));
            menuInstance.GetComponent<Menu>().SetActiveMoveDecision(false);
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
    //check for poo
    bool CheckPoo(Vector3Int gridPosV3)
    {
        foreach (Vector2Int vec in gLoop.GetComponent<GameLoop>().wombatPoo)
        {
            if (gridPosV3.x == vec.x && gridPosV3.y == vec.y)
            {
                return true;
            }
        }
        return false;
    }
}
