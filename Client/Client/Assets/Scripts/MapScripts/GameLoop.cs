using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    /// <summary>
    /// Created by Denny Rapp
    /// This will be the main game loop
    /// it will serve as a game controler
    /// </summary>
    #region variables
    //Public variables_____________________________
    public bool userTurn = false;
    public Player[] allPlayersOwnTeam;
    public Player[] allPlayersOpposingTeam;
    public Balls[] allballs;
    public Grid tileGrid;
    public Tilemap tmap;
    public GameObject menuInstance;
    public GameObject seSc;
    public GameObject visual;
    public GameObject moveObject;
    public GameObject fanObject;
    public GameObject actionObject;
    public GameObject sadPhase;
    public GameObject sound;
    public bool pause;
    public bool leftSide;
    public bool[] banMessageDisplayedOwnTeam;
    public bool[] banMessageDisplayedOpposingTeam;
    public bool[] knockoutMessageDisplayedOwnTeam;
    public bool[] knockutMessageDisplayedOpposingTeam;
    public Text TextRoundTimer;

    //---------------------------------------------
    //Private variables____________________________
    public Player selectedPlayer = null;
    public string selectedFan = "";
    private CheckScript cs = new CheckScript();
    private int rangeThrow = 3;
    private Vector3Int lastPos;
    public Vector3Int lastPosClicked;
    private bool playersSet = false;
    private float timeout { get; set; }
    public string turnType { get; set; }

    public Vector2Int [] wombatPoo;
    //---------------------------------------------
    #endregion variables


    /// <summary>
    /// void Start()
    /// Start is called before the first frame update
    /// it will initilize all variables
    /// </summary>
    void Start()
    {
        allPlayersOwnTeam = new Player[7];
        allPlayersOpposingTeam = new Player[7];
        allballs = new Balls[4];
        //Sets the Role of the Players 
        SetPlayerRole();
        SetOpponentPlayerRole();
        //the game is not paused when started
        pause = false;

        allballs[0] = new Balls(Ball.quaffle);
        allballs[1] = new Balls(Ball.bludger1);
        allballs[2] = new Balls(Ball.bludger2);
        allballs[3] = new Balls(Ball.snitch);

        wombatPoo = new Vector2Int[10];
        wombatPoo[0] = new Vector2Int(0,0);
        wombatPoo[1] = new Vector2Int(0, 0);
        wombatPoo[2] = new Vector2Int(0, 0);
        wombatPoo[3] = new Vector2Int(0, 0);
        wombatPoo[4] = new Vector2Int(0, 0);
        wombatPoo[5] = new Vector2Int(0, 0);
        wombatPoo[6] = new Vector2Int(0, 0);
        wombatPoo[7] = new Vector2Int(0, 0);
        wombatPoo[9] = new Vector2Int(0, 0);

        banMessageDisplayedOwnTeam = new bool [7];
        banMessageDisplayedOpposingTeam = new bool[7];
        knockoutMessageDisplayedOwnTeam = new bool[7];
        knockutMessageDisplayedOpposingTeam = new bool[7];

}

    /// <summary>
    /// void Update()
    /// Update is called once per frame
    /// it is responsible for the timer
    /// and plays a timeout animation
    /// </summary>    
    void Update()
    {
        if (userTurn && !pause && playersSet)
        {
            timeout = timeout - Time.deltaTime;
            if (timeout <= 0)
            {
                menuInstance.GetComponent<Menu>().Animation("Timeout");
                userTurn = false;
            }
            TextRoundTimer.text = "" + (int)timeout;
        }
    }
    //---------------------------------------------
    #region DisplayRange
    //DisplayMoveRange methods______________________
    //This will display the move range of the selected
    //player
    #region moveRange
    /// <summary>
    /// void DisplayMoveRange(Vector3Int gridPos)
    /// This method clears the previous painted tiles
    /// and paints the tiles around the player in magenta
    /// </summary>
    /// <param name="gridPos">position on tilemap</param>
    public void DisplayMoveRange(Vector3Int gridPos)
    {
        RemovePaint(lastPos);
        Paint(gridPos);
        lastPos = gridPos;
    }
    /// <summary>
    /// void Help(Vector3Int gridPos)
    /// This method sets the TileFlag on the gridpos tile
    /// to None and sets the color to magenta but only if the
    /// position is a tile position
    /// </summary>
    /// <param name="gridPos">position on tilemap</param>
    void Help(Vector3Int gridPos)
    {
        if (tmap.HasTile(gridPos))
        {
            tmap.SetTileFlags(gridPos, TileFlags.None);
            tmap.SetColor(gridPos, Color.magenta);
        }
    }
    /// <summary>
    /// void Help2(Vector3Int gridPos)
    /// This method sets the TileFlag on the gridpos tile
    /// to None and sets the color to white and therefore removes
    /// any other color but only if the
    /// position is a tile position
    /// </summary>
    /// <param name="gridPos">position on tilemap</param>
    void Help2(Vector3Int gridPos)
    {
        if (tmap.HasTile(gridPos))
        {
            tmap.SetTileFlags(gridPos, TileFlags.None);
            tmap.SetColor(gridPos, Color.white);
        }
    }
    /// <summary>
    /// void Paint(Vector3Int gridPos)
    /// This method will paint all fields
    /// arround the player magenta
    /// </summary>
    /// <param name="gridPos">player position on tilemap</param>
    void Paint(Vector3Int gridPos)
    {
        Vector3Int tempPos = gridPos;
        for (int i = 0; i< 3;i++)
        {
            tempPos.Set(gridPos.x-1,gridPos.y -1 + i,gridPos.z);
            Help(tempPos);
            tempPos.Set(gridPos.x, gridPos.y - 1 + i, gridPos.z);
            Help(tempPos);
            tempPos.Set(gridPos.x+1, gridPos.y - 1 + i, gridPos.z);
            Help(tempPos);
        }

        /*
        for (int i = 0; i <= range; i++)
        {
            Vector3Int tempPos = gridPos;
            tempPos.Set(gridPos.x + i, gridPos.y, gridPos.z);
            Help(tempPos);
            tempPos.Set(gridPos.x - i, gridPos.y, gridPos.z);
            Help(tempPos);
            for (int j = 0; j <= range - i; j++)
            {
                tempPos.Set(gridPos.x + i, gridPos.y + j, gridPos.z);
                Help(tempPos);
                tempPos.Set(gridPos.x + i, gridPos.y - j, gridPos.z);
                Help(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y + j, gridPos.z);
                Help(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y - j, gridPos.z);
                Help(tempPos);
            }
        }
        */
    }
    /// <summary>
    /// void RemovePaint(Vector3Int gridPos)
    /// This method will remove the paint 
    /// on all fields
    /// arround the player
    /// </summary>
    /// <param name="gridPos"></param>
    void RemovePaint(Vector3Int gridPos)
    {
        Vector3Int tempPos = gridPos;
        for (int i = 0; i < 3; i++)
        {
            tempPos.Set(gridPos.x - 1, gridPos.y - 1 + i, gridPos.z);
            Help2(tempPos);
            tempPos.Set(gridPos.x, gridPos.y - 1 + i, gridPos.z);
            Help2(tempPos);
            tempPos.Set(gridPos.x + 1, gridPos.y - 1 + i, gridPos.z);
            Help2(tempPos);
        }
        /*
        for (int i = 0; i <= range; i++)
        {
            Vector3Int tempPos = gridPos;
            tempPos.Set(gridPos.x + i, gridPos.y, gridPos.z);
            Help2(tempPos);
            tempPos.Set(gridPos.x - i, gridPos.y, gridPos.z);
            Help2(tempPos);
            for (int j = 0; j <= range - i; j++)
            {
                tempPos.Set(gridPos.x + i, gridPos.y + j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x + i, gridPos.y - j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y + j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y - j, gridPos.z);
                Help2(tempPos);
            }
        }
        */
    }
    #endregion moveRange
    //-----------------------------------------------------------------

    //DisplayMoveRange methods______________________
    //This will display the move range of the selected
    //player
    #region throwRange

    public void DisplayThrowRange(Vector3Int gridPos)
    {
        RemovePaintThrow(lastPos);
        PaintThrow(gridPos);
        lastPos = gridPos;
    }

    void HelpThrow(Vector3Int gridPos)
    {
        if (tmap.HasTile(gridPos))
        {
            tmap.SetTileFlags(gridPos, TileFlags.None);
            tmap.SetColor(gridPos, Color.red);

        }
    }

    void PaintThrow(Vector3Int gridPos)
    {
        for (int i = 0; i <= rangeThrow; i++)
        {
            Vector3Int tempPos = gridPos;
            tempPos.Set(gridPos.x + i, gridPos.y, gridPos.z);
            HelpThrow(tempPos);
            tempPos.Set(gridPos.x - i, gridPos.y, gridPos.z);
            HelpThrow(tempPos);
            for (int j = 0; j <= rangeThrow - i; j++)
            {
                tempPos.Set(gridPos.x + i, gridPos.y + j, gridPos.z);
                HelpThrow(tempPos);
                tempPos.Set(gridPos.x + i, gridPos.y - j, gridPos.z);
                HelpThrow(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y + j, gridPos.z);
                HelpThrow(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y - j, gridPos.z);
                HelpThrow(tempPos);
            }
        }
    }

    public void RemovePaintThrow(Vector3Int gridPos)
    {
        for (int i = 0; i <= rangeThrow; i++)
        {
            Vector3Int tempPos = gridPos;
            tempPos.Set(gridPos.x + i, gridPos.y, gridPos.z);
            Help2(tempPos);
            tempPos.Set(gridPos.x - i, gridPos.y, gridPos.z);
            Help2(tempPos);
            for (int j = 0; j <= rangeThrow - i; j++)
            {
                tempPos.Set(gridPos.x + i, gridPos.y + j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x + i, gridPos.y - j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y + j, gridPos.z);
                Help2(tempPos);
                tempPos.Set(gridPos.x - i, gridPos.y - j, gridPos.z);
                Help2(tempPos);
            }
        }
    }
    #endregion throwRange
    //-----------------------------------------------------------------
    #endregion DisplayRange
    #region DisplaySetArea
    /// <summary>
    /// void DisplaySadPhaseArea(bool left)
    /// this method will paint the playing area
    /// light gray
    /// </summary>
    /// <param name="left">bool if the player is on the left side</param>
    public void DisplaySadPhaseArea(bool left)
    {
        if (left)
        {
            for (int i = 0; i <= 12; i++)
            {
                HelpMarker(new Vector3Int(6, -i, 0));
                HelpMarker(new Vector3Int(5, -i, 0));
            }
            for (int i = 0; i<4;i++)
            {
                HelpMarker(new Vector3Int(4, -(12-i), 0));
                HelpMarker(new Vector3Int(4, -(0+i), 0));
            }
            for (int i = 0; i <5; i++)
            {
                HelpMarker(new Vector3Int(7, -(12 - i), 0));
                HelpMarker(new Vector3Int(7, -(0 + i), 0));
            }
            for (int i = 0; i < 2; i++)
            {
                HelpMarker(new Vector3Int(3, -(12-i), 0));
                HelpMarker(new Vector3Int(3, -(0+i), 0));
            }
        }
        else
        {
            for (int i = 0; i <= 12; i++)
            {
                HelpMarker(new Vector3Int(10, -i, 0));
                HelpMarker(new Vector3Int(11, -i, 0));
            }
            for (int i = 0; i < 5; i++)
            {
                HelpMarker(new Vector3Int(9, -(12 - i), 0));
                HelpMarker(new Vector3Int(9, -(0 + i), 0));
            }
            for (int i = 0; i < 4; i++)
            {
                HelpMarker(new Vector3Int(12, -(12 - i), 0));
                HelpMarker(new Vector3Int(12, -(0 + i), 0));
            }
            for (int i = 0; i < 2; i++)
            {
                HelpMarker(new Vector3Int(13, -(12 - i), 0));
                HelpMarker(new Vector3Int(13, -(0 + i), 0));
            }
        }
    }
    /// <summary>
    /// void HelpMarker(Vector3Int gridPos)
    /// This method will paint the given tile
    /// gray if its on the tilemap
    /// </summary>
    /// <param name="gridPos">position on the tilemap</param>
    void HelpMarker(Vector3Int gridPos)
    {
        if (tmap.HasTile(gridPos))
        {
            tmap.SetTileFlags(gridPos, TileFlags.None);
            tmap.SetColor(gridPos, Color.gray);

        }
    }
    /// <summary>
    /// void DeleteMarks()
    /// This method will clear the whole 
    /// pitch from all marks
    /// </summary>
    public void DeleteMarks()
    {
        for (int i = 0; i <= 16; i++)
        {
            for (int j = 0; j <= 12; j++)
            {
                Help2(new Vector3Int(i, -j, 0));
            }
        }
    }
    #endregion DisplaySetArea
    #region setRole
    /// <summary>
    /// void SetPlayerRole()
    /// this method sets all player roles
    /// of the own team
    /// </summary>
    void SetPlayerRole()
    {
        for (int x = 0; x < 7; x++)
        {
            allPlayersOwnTeam[x] = new Player();
        }
        allPlayersOwnTeam[0].playerRole = Role.Seeker;
        allPlayersOwnTeam[1].playerRole = Role.Keeper;
        allPlayersOwnTeam[2].playerRole = Role.Chaser1;
        allPlayersOwnTeam[3].playerRole = Role.Chaser2;
        allPlayersOwnTeam[4].playerRole = Role.Chaser3;
        allPlayersOwnTeam[5].playerRole = Role.Beater1;
        allPlayersOwnTeam[6].playerRole = Role.Beater2;
    }
    /// <summary>
    /// void SetPlayerRole()
    /// this method sets all player roles
    /// of the opposing team
    /// </summary>
    void SetOpponentPlayerRole()
    {
        for (int x = 0; x < 7; x++)
        {
            allPlayersOpposingTeam[x] = new Player();
        }
        allPlayersOpposingTeam[0].playerRole = Role.Seeker;
        allPlayersOpposingTeam[1].playerRole = Role.Keeper;
        allPlayersOpposingTeam[2].playerRole = Role.Chaser1;
        allPlayersOpposingTeam[3].playerRole = Role.Chaser2;
        allPlayersOpposingTeam[4].playerRole = Role.Chaser3;
        allPlayersOpposingTeam[5].playerRole = Role.Beater1;
        allPlayersOpposingTeam[6].playerRole = Role.Beater2;
    }
    #endregion setRole
    #region inRange
    /// <summary>
    /// bool InRange(Vector3Int gridPos)
    /// Checks if something is in range of the
    /// selected player
    /// </summary>
    /// <param name="gridPos">tile position</param>
    /// <returns>true if its in range and false if not</returns>
    public bool InRange(Vector3Int gridPos)
    {
        return tmap.GetColor(gridPos) == Color.magenta || tmap.GetColor(gridPos) == Color.red;
        //In case the code above doesnt work
        /*
        for (int i = 0; i <= range; i++)
        {
            Vector3Int tempPos = lastPos;
            tempPos.Set(lastPos.x + i, lastPos.y, lastPos.z);
            if(tempPos == gridPos) { return true; }
            tempPos.Set(lastPos.x - i, lastPos.y, lastPos.z);
            if (tempPos == gridPos) { return true; }
            for (int j = 0; j <= range - i; j++)
            {
                tempPos.Set(lastPos.x + i, lastPos.y + j, lastPos.z);
                if (tempPos == gridPos) { return true; }
                tempPos.Set(lastPos.x + i, lastPos.y - j, lastPos.z);
                if (tempPos == gridPos) { return true; }
                tempPos.Set(lastPos.x - i, lastPos.y + j, lastPos.z);
                if (tempPos == gridPos) { return true; }
                tempPos.Set(lastPos.x - i, lastPos.y - j, lastPos.z);
                if (tempPos == gridPos) { return true; }
            }
        }
        return false;
        */
    }
    /// <summary>
    /// bool InMoveRange(Vector3Int gridPos)
    /// this method checks if the selected tile is
    /// in move range
    /// </summary>
    /// <param name="gridPos">tile position</param>
    /// <returns>true if in move range else false</returns>
    public bool InMoveRange(Vector3Int gridPos)
    {
        return tmap.GetColor(gridPos) == Color.magenta;
    }
    /// <summary>
    /// bool InThrowRange(Vector3Int gridPos)
    /// this method will check if the tile
    /// is in move range
    /// </summary>
    /// <param name="gridPos">tilemap position</param>
    /// <returns>true if in throw range else false</returns>
    public bool InThrowRange(Vector3Int gridPos)
    {
        return tmap.GetColor(gridPos) == Color.red;
    }
    #endregion inRange
    #region buttons
    /// <summary>
    /// void PausButtonActive()
    /// this will send a pause request to the server
    /// </summary>
    public void PausButtonActive()
    {
        seSc.GetComponent<SerializeScript>().SendPauseRequest("Pause ist fuer alle da");
    }
    /// <summary>
    /// PauseButtonNotActive()
    /// this will send a reqeust to stop the
    /// </summary>
    public void PauseButtonNotActive()
    {
        seSc.GetComponent<SerializeScript>().SendContinueRequestJson("Pause ist vorbei");
    }
    /// <summary>
    /// void Skip()
    /// sends the right skip request to the server
    /// by evaluating the active entity and turn tpye
    /// </summary>
    public void Skip()
    {
        RemovePaintThrow(selectedPlayer.pos);
        if ((turnType.Equals("move") || turnType.Equals("action")) && userTurn)
        {
            menuInstance.GetComponent<Menu>().AddLog("Tryed skip: " +selectedPlayer.playerRole.ToString());
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("skip", ConvertPlayerRole(selectedPlayer.playerRole.ToString()));
            userTurn = false;
            return;
        }
        else if (turnType.Equals("fan") && userTurn)//fan = fanphase
        {
            menuInstance.GetComponent<Menu>().AddLog("Tryed skip: " +selectedFan);
            seSc.GetComponent<SerializeScript>().SendDeltaRequest("skip", selectedFan);
            menuInstance.GetComponent<Menu>().DeactivateAllFans();
            userTurn = false;
            return;
        }
    }
    /// <summary>
    /// void Pause(bool pp)
    /// sets if its a pause or not
    /// </summary>
    /// <param name="pp">what the paus status will bee</param>
    public void Pause(bool pp)
    {
        pause = pp;
    }
    #endregion buttons
    //----------------------------------------
    /// <summary>
    /// void SendTeamFormationToServer()
    /// this will send the teamformation to the server
    /// </summary>
    public void SendTeamFormationToServer()
    {
        seSc.GetComponent<SerializeScript>().SendTeamFormation(
            seSc.GetComponent<SerializeScript>().CreatePlayersPos(
                seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[0].pos.x, allPlayersOwnTeam[0].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[1].pos.x, allPlayersOwnTeam[1].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[2].pos.x, allPlayersOwnTeam[2].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[3].pos.x, allPlayersOwnTeam[3].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[4].pos.x, allPlayersOwnTeam[4].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[5].pos.x, allPlayersOwnTeam[5].pos.y)
                , seSc.GetComponent<SerializeScript>().CreatePlayerPos(allPlayersOwnTeam[6].pos.x, allPlayersOwnTeam[6].pos.y)));
    }
    #region SetSelectedPlayer
    /// <summary>
    /// void SetSelectedPlayer(string r)
    /// sets the selected player
    /// </summary>
    /// <param name="r">string of the player role</param>
    public void SetSelectedPlayer(string r)
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.playerRole.ToString().Equals(r))
            {
                selectedPlayer = p;
                visual.GetComponent<Visualize>().NextBlue(p.pos);
            }
        }
        ResolveTurnType();
    }
    /// <summary>
    /// void SetSelectedPlayer(string r, bool turnUsed)
    /// sets the selected player
    /// </summary>
    /// <param name="r">player role as string</param>
    /// <param name="turnUsed">if the turn is already used</param>
    public void SetSelectedPlayer(string r, bool turnUsed)
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.playerRole.ToString().Equals(r))
            {
                selectedPlayer = p;
                selectedPlayer.hasMoved = turnUsed;
            }
        }
        ResolveTurnType();
    }
    /// <summary>
    /// ResolveTurnType()
    /// displays the right things for
    /// the corresponding turntype
    /// </summary>
    void ResolveTurnType()
    {
        DeleteMarks();
        switch (turnType)
        {
            case "move":
                if (selectedPlayer.hasBall) { DisplayThrowRange(selectedPlayer.pos); }
                DisplayMoveRange(selectedPlayer.pos);
                break;
            case "action":
                if (selectedPlayer.hasBall) { DisplayThrowRange(selectedPlayer.pos); }
                else { DisplayMoveRange(selectedPlayer.pos); }
                break;
            case "fan":
                RemovePaintThrow(selectedPlayer.pos);
                break;
            case "unban":
                RemovePaintThrow(selectedPlayer.pos);
                break;
            default:
                break;
        }
    }
    #endregion SetSelectedPlayer
    #region SetInfoFromSnapShot
    /// <summary>
    /// void SetOpponentPlayers(string role, int x, int y, bool b, bool m,bool ko, bool hb)
    /// sets all information for a player of the
    /// oppsing team
    /// </summary>
    /// <param name="role">role</param>
    /// <param name="x">x pos</param>
    /// <param name="y">y pos</param>
    /// <param name="b">banned</param>
    /// <param name="m">has moved</param>
    /// <param name="ko">knocked out</param>
    /// <param name="hb">has ball</param>
    public void SetOpponentPlayers(string role, int x, int y, bool b, bool m,bool ko, bool hb)
    {
        foreach (Player p in allPlayersOpposingTeam)
        {
            if (p.playerRole.ToString().Equals(role))
            {
                Vector3Int vec = new Vector3Int(x, y, 0);
                p.pos = vec;
                p.hasMoved = m;
                p.banned = b;
                p.knockedOut = ko;
                p.hasBall = hb;
                if (p.banned)
                {
                    BanMessageOpposingTeam(p.playerRole);
                    menuInstance.GetComponent<Menu>().AddLog(role + " banned");
                }
                else
                {
                    BanMessagedResetOpposing(p.playerRole);
                }
                if (p.knockedOut)
                {
                    KnockoutMessageOpposingTeam(p.playerRole);
                    menuInstance.GetComponent<Menu>().AddLog(role + " knocked out");
                }
                else
                {
                    KnockoutMessagedResetOpposing(p.playerRole);
                }
            }
        }
    }
    /// <summary>
    /// void SetOwnPlayers(string role, int x, int y, bool b, bool m,bool ko, bool hb)
    /// sets all information for a player of the
    /// own team
    /// </summary>
    /// <param name="role">role</param>
    /// <param name="x">x pos</param>
    /// <param name="y">y pos</param>
    /// <param name="b">banned</param>
    /// <param name="m">has moved</param>
    /// <param name="ko">knocked out</param>
    /// <param name="hb">has ball</param>
    public void SetOwnPlayers(string role, int x, int y, bool b, bool m, bool ko, bool hb)
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.playerRole.ToString().Equals(role))
            {
                Vector3Int vec = new Vector3Int(x, y, 0);
                p.pos = vec;
                p.hasMoved = m;
                p.banned = b;
                p.knockedOut = ko;
                p.hasBall = hb;
                if (p.banned)
                {
                    menuInstance.GetComponent<Menu>().AddLog(role + " banned");
                    BanMessageOwnTeam(p.playerRole);
                }
                else
                {
                    BanMessagedResetOwn(p.playerRole);
                }
                if (p.knockedOut)
                {
                    KnockoutMessageOwnTeam(p.playerRole);
                    menuInstance.GetComponent<Menu>().AddLog(role + " knocked out");
                }
                else
                {
                    KnockoutMessagedResetOwn(p.playerRole);
                }
            }
        }
    }
    #endregion SetInfoFromSnapShot
    /// <summary>
    /// void ResolveNext(int i, string s, string s2)
    /// resolves the next message of the server and starts the 
    /// update methods that are required for the
    /// turntype. This method also sets
    /// the neccessary information
    /// like selected player and writes messages
    /// into the log
    /// </summary>
    /// <param name="i">timeout</param>
    /// <param name="s">turntype</param>
    /// <param name="s2">active entity</param>
    public void ResolveNext(int i, string s, string s2)
    {
        /*
         Next:
         • move
         • action
         • fan
         • removeBan
        */
        menuInstance.GetComponent<Menu>().AddLog("Next TT" + s + "Entity " + s2);
        timeout = (float)(i/1000);
        turnType = s;
        DeleteMarks();
        userTurn = false;
        menuInstance.GetComponent<Menu>().SetActiveMoveDecision(false);
        menuInstance.GetComponent<Menu>().DeactivateAllFans();
        fanObject.GetComponent<Fan>().ScriptStatus(false);
        moveObject.GetComponent<Move>().ScriptStatus(false);
        actionObject.GetComponent<Action>().ScriptStatus(false);
        if (leftSide)
        {
            switch (s2)
            {
                case "leftKeeper":
                    TurnTrue();
                    SetSelectedPlayer("Keeper");
                    break;
                case "leftChaser1":
                    TurnTrue();
                    SetSelectedPlayer("Chaser1");
                    break;
                case "leftChaser2":
                    TurnTrue();
                    SetSelectedPlayer("Chaser2");
                    break;
                case "leftChaser3":
                    TurnTrue();
                    SetSelectedPlayer("Chaser3");
                    break;
                case "leftBeater1":
                    TurnTrue();
                    SetSelectedPlayer("Beater1");
                    break;
                case "leftBeater2":
                    TurnTrue();
                    SetSelectedPlayer("Beater2");
                    break;
                case "leftSeeker":
                    TurnTrue();
                    SetSelectedPlayer("Seeker");
                    break;
                case "leftNiffler":
                case "leftGoblin":
                case "leftWombat":
                case "leftElf":
                case "leftTroll":
                    TurnTrue();
                    selectedFan = s2;
                    menuInstance.GetComponent<Menu>().SetActiveFan(s2);
                    if (selectedPlayer != null)
                    {
                        RemovePaintThrow(selectedPlayer.pos);
                    }
                    break;
                default:
                    userTurn = false;
                    if (selectedPlayer != null)
                    {
                        RemovePaintThrow(selectedPlayer.pos);
                    }
                    break;
            }
        }
        else
        {
            switch (s2)
            {
                case "rightSeeker":
                    TurnTrue();
                    SetSelectedPlayer("Seeker");
                    break;
                case "rightKeeper":
                    TurnTrue();
                    SetSelectedPlayer("Keeper");
                    break;
                case "rightChaser2":
                    TurnTrue();
                    SetSelectedPlayer("Chaser2");
                    break;
                case "rightChaser3":
                    TurnTrue();
                    SetSelectedPlayer("Chaser3");
                    break;
                case "rightChaser1":
                    TurnTrue();
                    SetSelectedPlayer("Chaser1");
                    break;
                case "rightBeater2":
                    TurnTrue();
                    SetSelectedPlayer("Beater2");
                    break;
                case "rightBeater1":
                    TurnTrue();
                    SetSelectedPlayer("Beater1");
                    break;
                case "rightNiffler":
                case "rightGoblin":
                case "rightWombat":
                case "rightElf":
                case "rightTroll":
                    TurnTrue();
                    selectedFan = s2;
                    menuInstance.GetComponent<Menu>().SetActiveFan(s2);
                    if (selectedPlayer != null)
                    {
                        RemovePaintThrow(selectedPlayer.pos);
                    }
                    break;
                default:
                    userTurn = false;
                    if (selectedPlayer != null)
                    {
                        RemovePaintThrow(selectedPlayer.pos);
                    }
                    break;
            }
        }
        if (userTurn)
        {
            switch (turnType)
            {
                case "move":
                    fanObject.GetComponent<Fan>().ScriptStatus(false);
                    moveObject.GetComponent<Move>().ScriptStatus(true);
                    actionObject.GetComponent<Action>().ScriptStatus(false);
                    menuInstance.GetComponent<Menu>().TurnOffPlacing();
                    if (selectedPlayer.banned)
                    {
                        Skip();
                    }
                    break;
                case "fan":
                    fanObject.GetComponent<Fan>().ScriptStatus(true);
                    moveObject.GetComponent<Move>().ScriptStatus(false);
                    actionObject.GetComponent<Action>().ScriptStatus(false);
                    menuInstance.GetComponent<Menu>().TurnOffPlacing();
                    if (selectedPlayer.banned)
                    {
                        Skip();
                    }
                    break;
                case "action":
                    fanObject.GetComponent<Fan>().ScriptStatus(false);
                    moveObject.GetComponent<Move>().ScriptStatus(false);
                    actionObject.GetComponent<Action>().ScriptStatus(true);
                    menuInstance.GetComponent<Menu>().TurnOffPlacing();
                    if (selectedPlayer.banned)
                    {
                        Skip();
                    }
                    break;
                case "removeBan":
                    fanObject.GetComponent<Fan>().ScriptStatus(false);
                    moveObject.GetComponent<Move>().ScriptStatus(false);
                    actionObject.GetComponent<Action>().ScriptStatus(false);
                    sadPhase.GetComponent<SetPhase>().StartUnban(allPlayersOwnTeam, selectedPlayer.playerRole.ToString());
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// void UnbanDelta(Vector3Int pos, string b)
    /// sends the unban delta to the server
    /// </summary>
    /// <param name="pos">position on tilemap</param>
    /// <param name="b">player role</param>
    public void UnbanDelta(Vector3Int pos, string b)
    {
        seSc.GetComponent<SerializeScript>().SendDeltaRequest("unban", pos.x, pos.y,ConvertPlayerRole(b));
        menuInstance.GetComponent<Menu>().TurnOffPlacing();
    }
    #region OwnHelp
    /// <summary>
    /// void TurnTrue()
    /// sets the turn variable true
    /// </summary>
    void TurnTrue()
    {
        userTurn = true;
    }
    /// <summary>
    /// void TurnFalse()
    /// sets the turn variable false
    /// </summary>
    public void TurnFalse()
    {
        userTurn = false;
    }
    /// <summary>
    /// string ConvertPlayerRole(string s)
    /// converts the role for the own side
    /// </summary>
    /// <param name="s">role</param>
    /// <returns></returns>
    public string ConvertPlayerRole(string s)
    {
        if (leftSide)
        {
            return "left" + s;
        }
        else
        {
            return "right" + s;
        }
    }
    /// <summary>
    /// string ConvertPlayerRoleOpposingTeam(string s)
    /// converts the role for the opposing side
    /// </summary>
    /// <param name="s">role</param>
    /// <returns></returns>
    public string ConvertPlayerRoleOpposingTeam(string s)
    {
        if (!leftSide)
        {
            return "left" + s;
        }
        else
        {
            return "right" + s;
        }
    }
    /// <summary>
    /// void SetSide(bool lSide)
    /// sets the side variable
    /// </summary>
    /// <param name="lSide">is the player on the left side</param>
    public void SetSide(bool lSide)
    {
        leftSide = lSide;
    }
    /// <summary>
    /// SetPlayersOwnTeam(Player[] p)
    /// overwrites all players of the own team
    /// </summary>
    /// <param name="p">player array with 7 players</param>
    public void SetPlayersOwnTeam(Player[] p)
    {
        allPlayersOwnTeam = p;
        playersSet = true;
    }
    /// <summary>
    /// SetPlayersOpposingTeam(Player[] p)
    /// overwrites all players of the opposing team
    /// </summary>
    /// <param name="p">player array with 7 players</param>
    public void SetPlayersOpposingTeam(Player[] p)
    {
        allPlayersOpposingTeam = p;
    }
    /// <summary>
    /// void SetPlayerBall(string b)
    /// sets if a player has a ball
    /// </summary>
    /// <param name="b">has ball</param>
    public void SetPlayerBall(string b)
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.playerRole.ToString().Equals(b))
            {
                selectedPlayer.hasBall = true;
            }
        }
    }
    /// <summary>
    /// void RemovePlayerBall(string b)
    /// removes the ball from a palyer
    /// </summary>
    /// <param name="b">has ball</param>
    public void RemovePlayerBall(string b)
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.playerRole.ToString().Equals(b))
            {
                selectedPlayer.hasBall = false;
            }
        }
    }
    /// <summary>
    /// void SetBallPos(string ballString, int x, int y)
    /// overwrites ball pos
    /// </summary>
    /// <param name="ballString">what ball</param>
    /// <param name="x">x pos</param>
    /// <param name="y">y pos</param>
    public void SetBallPos(string ballString, int x, int y)
    {
        for (int i = 0; i<4; i++)
        {
            if (allballs[i].type.ToString().Equals(ballString))
            {
                allballs[i].pos = new Vector3Int(x,y,0);
            }
        }
    }
    /// <summary>
    /// void UpdateBalls(int qx, int qy, int bx, int by, int b2x, int b2y, int sx, int sy)
    /// sets all ball positions
    /// </summary>
    /// <param name="qx">quaffle x</param>
    /// <param name="qy">quaffle y</param>
    /// <param name="bx">blutcher1 x</param>
    /// <param name="by">blutcher1 y</param>
    /// <param name="b2x">blutcher2 x</param>
    /// <param name="b2y">blutcher2 y</param>
    /// <param name="sx">snicht x</param>
    /// <param name="sy">snicth y</param>
    public void UpdateBalls(int qx, int qy, int bx, int by, int b2x, int b2y, int sx, int sy)
    {
        int z = allballs[0].pos.z;
        allballs[0].pos = new Vector3Int(qx, qy, z);
        allballs[1].pos = new Vector3Int(bx, by, z);
        allballs[2].pos = new Vector3Int(b2x, b2y, z);
        allballs[3].pos = new Vector3Int(sx, sy, z);
    }
    //Checks if a ball is on the position
    bool CheckIfBallIsOnPos(Vector3Int gridPos)
    {
        for (int i = 0; i < 4; i++)
        {
            if (allballs[i].pos == gridPos)
            {
                return true;
            }
        }
        return false;
    }
    //Checks what ball is on the position
    Ball CheckWhatBallIsOnPos(Vector3Int gridPos)
    {
        for (int i = 0; i < 4; i++)
        {
            if (allballs[i].pos == gridPos)
            {
                return allballs[i].type;
            }
        }
        return Ball.quaffle;
    }
    //sets wombat poo on pos
    public void SetWombatPoo(int x, int y)
    {
        for (int i = 0; i< wombatPoo.Length;i++)
        {
            if (wombatPoo[i].x == 0 && wombatPoo[i].y == 0)
            {
                wombatPoo[i] = new Vector2Int(x,y);
                break;
            }
        }

    }
    //removes all wombat poo
    public void DeleteAllWombatPoo()
    {
        for (int i = 0; i < wombatPoo.Length; i++)
        {
            wombatPoo[i] = new Vector2Int(0,0);
        }
    }
    #endregion OwnHelp
    #region HelpLogic
    public Vector3Int ConvertToTileMap(Vector3 vec)
    {
        Vector3Int gridPosV3 = tmap.WorldToCell(Camera.main.ScreenToWorldPoint(vec));
        gridPosV3.Set(gridPosV3.x, gridPosV3.y, 0);
        return gridPosV3;
    }
    public void UpdateInfos(Player[]p,Player[]p2,Balls[]b)
    {
        allPlayersOwnTeam = p;
        allPlayersOpposingTeam = p2;
        allballs = b;
    }
    #endregion HelpLogic
    //plays a small animation if a player gets banned or ko
    #region SpecialAnimations
    void BanMessageOwnTeam(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                if (!banMessageDisplayedOwnTeam[i])
                {
                    sound.GetComponent<SoundEffects>().PlaySound(1);
                    menuInstance.GetComponent<Menu>().Animation(ConvertPlayerRole(r.ToString())+"banned");
                    banMessageDisplayedOwnTeam[i] = true;
                }
            }
        }
    }
    void BanMessagedResetOwn(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                banMessageDisplayedOwnTeam[i] = false;
            }
        }
    }
    void BanMessageOpposingTeam(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                if (!banMessageDisplayedOpposingTeam[i])
                {
                    menuInstance.GetComponent<Menu>().Animation(ConvertPlayerRoleOpposingTeam(r.ToString()) + "banned");
                    sound.GetComponent<SoundEffects>().PlaySound(1);
                    banMessageDisplayedOpposingTeam[i] = true;
                }
            }
        }
    }
    void BanMessagedResetOpposing(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                banMessageDisplayedOpposingTeam[i] = false;
            }
        }
    }
    //----------------------------------------------------
    void KnockoutMessageOwnTeam(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                if (!knockoutMessageDisplayedOwnTeam[i])
                {
                    sound.GetComponent<SoundEffects>().PlaySound(5);
                    menuInstance.GetComponent<Menu>().Animation(ConvertPlayerRole(r.ToString()) + "banned");
                    knockoutMessageDisplayedOwnTeam[i] = true;
                }
            }
        }
    }
    void KnockoutMessagedResetOwn(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                knockoutMessageDisplayedOwnTeam[i] = false;
            }
        }
    }
    void KnockoutMessageOpposingTeam(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                if (!knockutMessageDisplayedOpposingTeam[i])
                {
                    menuInstance.GetComponent<Menu>().Animation(ConvertPlayerRoleOpposingTeam(r.ToString()) + "banned");
                    sound.GetComponent<SoundEffects>().PlaySound(5);
                    knockutMessageDisplayedOpposingTeam[i] = true;
                }
            }
        }
    }
    void KnockoutMessagedResetOpposing(Role r)
    {
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].playerRole == r)
            {
                knockutMessageDisplayedOpposingTeam[i] = false;
            }
        }
    }
    #endregion SpecialAnimations
}