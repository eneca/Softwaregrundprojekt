using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;



public class Visualize : MonoBehaviour
{
    public Tilemap playerlayer;
    public Tile tile;

    public TeamConfigJson tcleft;
    public TeamConfigJson tcright;

    public bool side;

    public int BALL_Z = -5;
    public int PLAYER_Z = -4;
    public int POO_Z = -3;

    private string colorleft;
    public Tile tileLeftKeeper;
    public Tile tileLeftBeater1;
    private Tile tileLeftBeater2;
    public Tile tileLeftChaser1;
    private Tile tileLeftChaser2;
    private Tile tileLeftChaser3;
    public Tile tileLeftSeeker;

    private string colorright;
    public Tile tileRightKeeper;
    public Tile tileRightBeater1;
    private Tile tileRightBeater2;
    public Tile tileRightChaser1;
    private Tile tileRightChaser2;
    private Tile tileRightChaser3;
    public Tile tileRightSeeker;

    //TODO private
    public Tile tileBludger1;
    public Tile tileBludger2;
    public Tile tileQuaffel;
    public Tile tileSnitch;
    public Tile tileWombatPoo;
    public Tile tileBlue;

    public GameObject _Menu;
    public GameObject _SetPhase;
    public GameObject _GameLoop;
    public string[] colorpalette = new string[] { "blau", "gelb", "gruen", "rot" };

    //Visualize move player
    public void VisualizeMovePlayer(Vector3Int playerPos, Vector3Int toPos)
    {
        playerPos.z = PLAYER_Z;
        TileBase pos_Tile = playerlayer.GetTile(playerPos);
        playerlayer.SetTile(playerPos, null); // Remove tile at playerPos

        toPos.z = PLAYER_Z;
        playerlayer.SetTile(toPos, tile);
    }

    //Visualize move ball
    public void VisualizeMoveBall(Vector3Int playerPos, Vector3Int toPos)
    {
        playerPos.z = BALL_Z;
        TileBase pos_Tile = playerlayer.GetTile(playerPos);
        playerlayer.SetTile(playerPos, null); // Remove tile at playerPos

        toPos.z = BALL_Z;
        playerlayer.SetTile(toPos, tile);
    }

    //To indicate whos turn it is
    public void NextBlue(Vector3Int next)
    {
        next = new Vector3Int(next.x, next.y, -3);
        tile = tileBlue;
        playerlayer.SetTile(next, tile);
    }


    #region VisualizePlayer

    #region TeamConfig

    //TeamConfig and other stuff are given sprites (or tiles)
    public void DissolveTeamConfig(TeamConfigJson l, TeamConfigJson r, bool b)
    {
        tileBludger1 = Resources.Load<Tile>("Baelle/klatscher_isometrik");
        tileBludger2 = Resources.Load<Tile>("Baelle/klatscher_isometrik");
        tileQuaffel = Resources.Load<Tile>("Baelle/quaffel_isometrik");
        tileSnitch = Resources.Load<Tile>("Baelle/schnatz");
        tileWombatPoo = Resources.Load<Tile>("Baelle/wombat_poo");
        tileBlue = Resources.Load<Tile>("Baelle/blueUnderground");

        side = b;
        tcleft = l;
        tcright = r;

        ressolveColor(tcleft.colors.primary, tcleft.colors.secondary, tcright.colors.primary, tcright.colors.secondary);//Findout colors of teams

        string sex = tcleft.players.keeper.sex;
        tileLeftKeeper = Resources.Load<Tile>(colorleft + "/" + sex + "/keeper_" + sex + "_h");

        sex = tcleft.players.beater1.sex;
        tileLeftBeater1 = Resources.Load<Tile>(colorleft + "/" + sex + "/beater_" + sex + "_h");
        sex = tcleft.players.beater2.sex;
        tileLeftBeater2 = Resources.Load<Tile>(colorleft + "/" + sex + "/beater_" + sex + "_h");

        sex = tcleft.players.chaser1.sex;
        tileLeftChaser1 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");
        sex = tcleft.players.chaser2.sex;
        tileLeftChaser2 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");
        sex = tcleft.players.chaser3.sex;
        tileLeftChaser3 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");

        sex = tcleft.players.seeker.sex;
        tileLeftSeeker = Resources.Load<Tile>(colorleft + "/" + sex + "/seeker_" + sex + "_h");


        sex = tcright.players.keeper.sex;
        tileRightKeeper = Resources.Load<Tile>(colorright + "/" + sex + "/keeper_" + sex + "_v");

        sex = tcleft.players.beater1.sex;
        tileRightBeater1 = Resources.Load<Tile>(colorright + "/" + sex + "/beater_" + sex + "_v");
        sex = tcleft.players.beater2.sex;
        tileRightBeater2 = Resources.Load<Tile>(colorright + "/" + sex + "/beater_" + sex + "_v");

        sex = tcleft.players.chaser1.sex;
        tileRightChaser1 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");
        sex = tcleft.players.chaser2.sex;
        tileRightChaser2 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");
        sex = tcleft.players.chaser3.sex;
        tileRightChaser3 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");

        sex = tcleft.players.seeker.sex;
        tileRightSeeker = Resources.Load<Tile>(colorright + "/" + sex + "/seeker_" + sex + "_v");

        _SetPhase.GetComponent<SetPhase>().InitClass();
    }

    //not really good...
    private void ressolveColor(string primeleft, string secleft, string primeright, string secright)
    {
        if (primeleft == primeright)
        {
            colorleft = "blau";
            colorright = "rot";
        }
        else
        {
            colorleft = "gruen";
            colorright = "rot";
        }
        //System.Random random = new System.Random();
        //int v = random.Next(0,4);
        //colorleft=colorpalette[v];
        //int i = random.Next(0, 4);
        //while (i == v) { i = random.Next(0, 4); }
        //colorright = colorpalette[i];
    }

    #endregion TeamConfig


    #region deltaBroadcast

    //Visaulize deltaBroadcast
    public void VisualizeBroadcast(DeltaBroadcastJson dbj)
    {
        _Menu.GetComponent<Menu>().ChangeScore(dbj.leftPoints, dbj.rightPoints);

        int x = dbj.xPosOld;
        int y = dbj.yPosOld;

        TileBase ball_Tile = playerlayer.GetTile(new Vector3Int(x, y, BALL_Z));
        TileBase player_Tile = playerlayer.GetTile(new Vector3Int(x, y, PLAYER_Z));

        playerlayer.SetTile(new Vector3Int(x, y, BALL_Z), null);
        playerlayer.SetTile(new Vector3Int(x, y, PLAYER_Z), null);

        x = dbj.xPosNew;
        y = dbj.xPosNew;

        playerlayer.SetTile(new Vector3Int(x, y, BALL_Z), ball_Tile);
        playerlayer.SetTile(new Vector3Int(x, y, PLAYER_Z), player_Tile);

    }

    #endregion deltaBroadcast


    #region Snapshot

    //This methode loads a snapshot
    public void VisualizeSnapshot(SnapshotJson s)
    {
        int r = s.round;
        string p = s.phase;
        bool ban;
        bool mov;
        bool ko;
        bool hb;
        DeltaBroadcastJson dbj = s.lastDeltaBroadcast;
        playerlayer.ClearAllTiles();

        #region balls

        int x = s.balls.bludger1.xPos;
        int y = s.balls.bludger1.yPos;
        Vector3Int v3 = new Vector3Int(x, -y, BALL_Z); //Debug.LogWarning("Bludger1: "+v3);
        tile = tileBludger1;
        playerlayer.SetTile(v3, tile);
        _GameLoop.GetComponent<GameLoop>().SetBallPos("bludger1", x, -y);

        x = s.balls.bludger2.xPos;
        y = s.balls.bludger2.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z); //Debug.LogWarning("Bludger2: " + v3);
        tile = tileBludger2;
        playerlayer.SetTile(v3, tile);
        _GameLoop.GetComponent<GameLoop>().SetBallPos("bludger2", x, -y);

        x = s.balls.quaffle.xPos;
        y = s.balls.quaffle.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z); //Debug.LogWarning("Quaffel: " + v3);
        tile = tileQuaffel;
        playerlayer.SetTile(v3, tile);
        _GameLoop.GetComponent<GameLoop>().SetBallPos("quaffle", x, -y);

        x = s.balls.snitch.xPos;
        y = s.balls.snitch.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z); //Debug.LogWarning("Snitch: " + v3);
        tile = tileSnitch;
        playerlayer.SetTile(v3, tile);
        _GameLoop.GetComponent<GameLoop>().SetBallPos("snitch", x, -y);

        #endregion balls

        #region left side

        int scoreLeft = s.leftTeam.points;

        x = s.leftTeam.players.keeper.xPos;
        y = s.leftTeam.players.keeper.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftKeeper;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.keeper.banned;
        mov = s.leftTeam.players.keeper.turnUsed;
        ko = s.leftTeam.players.keeper.knockout;
        hb = s.leftTeam.players.keeper.holdsQuaffle;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Keeper", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Keeper", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);
        _GameLoop.GetComponent<GameLoop>().SetSide(side);

        x = s.leftTeam.players.beater1.xPos;
        y = s.leftTeam.players.beater1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftBeater1;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.beater1.banned;
        mov = s.leftTeam.players.beater1.turnUsed;
        ko = s.leftTeam.players.beater1.knockout;
        hb = s.leftTeam.players.beater1.holdsBludger;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Beater1", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Beater1", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.leftTeam.players.beater2.xPos;
        y = s.leftTeam.players.beater2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftBeater2;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.beater2.banned;
        mov = s.leftTeam.players.beater2.turnUsed;
        ko = s.leftTeam.players.beater2.knockout;
        hb = s.leftTeam.players.beater2.holdsBludger;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Beater2", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Beater2", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser1.xPos;
        y = s.leftTeam.players.chaser1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser1;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.chaser1.banned;
        mov = s.leftTeam.players.chaser1.turnUsed;
        ko = s.leftTeam.players.chaser1.knockout;
        hb = s.leftTeam.players.chaser1.holdsQuaffle;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser1", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser1", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser2.xPos;
        y = s.leftTeam.players.chaser2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser2;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.chaser2.banned;
        mov = s.leftTeam.players.chaser2.turnUsed;
        ko = s.leftTeam.players.chaser2.knockout;
        hb = s.leftTeam.players.chaser2.holdsQuaffle;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser2", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser2", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser3.xPos;
        y = s.leftTeam.players.chaser3.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser3;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.chaser3.banned;
        mov = s.leftTeam.players.chaser3.turnUsed;
        ko = s.leftTeam.players.chaser3.knockout;
        hb = s.leftTeam.players.chaser3.holdsQuaffle;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser3", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser3", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.leftTeam.players.seeker.xPos;
        y = s.leftTeam.players.seeker.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftSeeker;
        playerlayer.SetTile(v3, tile);
        ban = s.leftTeam.players.seeker.banned;
        mov = s.leftTeam.players.seeker.turnUsed;
        ko = s.leftTeam.players.seeker.knockout;
        if (!side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Seeker", x, -y, ban, mov, ko, false);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Seeker", x, -y, ban, mov, ko, false);
        }
        KnockedOut(ko, v3);

        #endregion left side

        #region right side

        int scoreRight = s.rightTeam.points;

        x = s.rightTeam.players.keeper.xPos;
        y = s.rightTeam.players.keeper.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightKeeper;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.keeper.banned;
        mov = s.rightTeam.players.keeper.turnUsed;
        ko = s.rightTeam.players.keeper.knockout;
        hb = s.rightTeam.players.keeper.holdsQuaffle;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Keeper", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Keeper", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.beater1.xPos;
        y = s.rightTeam.players.beater1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightBeater1;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.beater1.banned;
        mov = s.rightTeam.players.beater1.turnUsed;
        ko = s.rightTeam.players.beater1.knockout;
        hb = s.rightTeam.players.beater1.holdsBludger;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Beater1", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Beater1", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.beater2.xPos;
        y = s.rightTeam.players.beater2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightBeater2;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.beater2.banned;
        mov = s.rightTeam.players.beater2.turnUsed;
        ko = s.rightTeam.players.beater2.knockout;
        hb = s.rightTeam.players.beater2.holdsBludger;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Beater2", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Beater2", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser1.xPos;
        y = s.rightTeam.players.chaser1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser1;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.chaser1.banned;
        mov = s.rightTeam.players.chaser1.turnUsed;
        ko = s.rightTeam.players.chaser1.knockout;
        hb = s.rightTeam.players.chaser1.holdsQuaffle;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser1", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser1", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser2.xPos;
        y = s.rightTeam.players.chaser2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser2;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.chaser2.banned;
        mov = s.rightTeam.players.chaser2.turnUsed;
        ko = s.rightTeam.players.chaser2.knockout;
        hb = s.rightTeam.players.chaser2.holdsQuaffle;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser2", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser2", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser3.xPos;
        y = s.rightTeam.players.chaser3.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser3;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.chaser3.banned;
        mov = s.rightTeam.players.chaser3.turnUsed;
        ko = s.rightTeam.players.chaser3.knockout;
        hb = s.rightTeam.players.chaser3.holdsQuaffle;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Chaser3", x, -y, ban, mov, ko, hb);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Chaser3", x, -y, ban, mov, ko, hb);
        }
        KnockedOut(ko, v3);

        x = s.rightTeam.players.seeker.xPos;
        y = s.rightTeam.players.seeker.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightSeeker;
        playerlayer.SetTile(v3, tile);
        ban = s.rightTeam.players.seeker.banned;
        mov = s.rightTeam.players.seeker.turnUsed;
        ko = s.rightTeam.players.seeker.knockout;
        if (side)
        {
            _GameLoop.GetComponent<GameLoop>().SetOpponentPlayers("Seeker", x, -y, ban, mov, ko, false);
        }
        else
        {
            _GameLoop.GetComponent<GameLoop>().SetOwnPlayers("Seeker", x, -y, ban, mov, ko, false);
        }
        KnockedOut(ko, v3);

        #endregion right side

        tile = tileWombatPoo;
        _GameLoop.GetComponent<GameLoop>().DeleteAllWombatPoo();
        foreach (WombatCubesJson wc in s.wombatCubes)
        {
            v3 = new Vector3Int(wc.xPos, -wc.yPos, POO_Z);
            _GameLoop.GetComponent<GameLoop>().SetWombatPoo(wc.xPos, -wc.yPos);
            playerlayer.SetTile(v3, tile);
        }

        _Menu.GetComponent<Menu>().ChangeScore(scoreLeft, scoreRight);//Change the score

        v3 = new Vector3Int(0, 0, PLAYER_Z);
        playerlayer.SetTile(v3, null);
        v3 = new Vector3Int(0, 0, BALL_Z);
        playerlayer.SetTile(v3, null);

        //update minimap
        _Menu.GetComponent<Menu>().UpdateBallsMinimap(s.balls);
        _Menu.GetComponent<Menu>().UpdatePlayerMinimap(s.leftTeam.players, s.rightTeam.players);

    }

    #endregion Snapshot
    #endregion VisualizePlayer


    private void KnockedOut(bool b, Vector3Int v)
    {
        if (b)
        {
            playerlayer.SetTileFlags(v, TileFlags.None);
            playerlayer.SetColor(v, Color.grey);
        }
        else
        {
            playerlayer.SetTileFlags(v, TileFlags.None);
            playerlayer.SetColor(v, Color.white);
        }
    }

    #region VisualizeSpectator

    #region TeamConfigSpectator

    //TeamConfig and other stuff are given sprites (or tiles)
    public void DissolveTeamConfigSpectator(TeamConfigJson l, TeamConfigJson r)
    {
        tileBludger1 = Resources.Load<Tile>("Baelle/klatscher_isometrik");
        tileBludger2 = Resources.Load<Tile>("Baelle/klatscher_isometrik");
        tileQuaffel = Resources.Load<Tile>("Baelle/quaffel_isometrik");
        tileSnitch = Resources.Load<Tile>("Baelle/schnatz");
        tileWombatPoo = Resources.Load<Tile>("Baelle/wombat_poo");
        tileBlue = Resources.Load<Tile>("Baelle/blueUnderground");

        tcleft = l;
        tcright = r;

        ressolveColor(tcleft.colors.primary, tcleft.colors.secondary, tcright.colors.primary, tcright.colors.secondary);//Findout colors of teams

        string sex = tcleft.players.keeper.sex;
        tileLeftKeeper = Resources.Load<Tile>(colorleft + "/" + sex + "/keeper_" + sex + "_h");
        sex = tcleft.players.beater1.sex;
        tileLeftBeater1 = Resources.Load<Tile>(colorleft + "/" + sex + "/beater_" + sex + "_h");
        sex = tcleft.players.beater2.sex;
        tileLeftBeater2 = Resources.Load<Tile>(colorleft + "/" + sex + "/beater_" + sex + "_h");
        sex = tcleft.players.chaser1.sex;
        tileLeftChaser1 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");
        sex = tcleft.players.chaser2.sex;
        tileLeftChaser2 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");
        sex = tcleft.players.chaser3.sex;
        tileLeftChaser3 = Resources.Load<Tile>(colorleft + "/" + sex + "/chaser_" + sex + "_h");
        sex = tcleft.players.seeker.sex;
        tileLeftSeeker = Resources.Load<Tile>(colorleft + "/" + sex + "/seeker_" + sex + "_h");


        sex = tcright.players.keeper.sex;
        tileRightKeeper = Resources.Load<Tile>(colorright + "/" + sex + "/keeper_" + sex + "_v");
        sex = tcleft.players.beater1.sex;
        tileRightBeater1 = Resources.Load<Tile>(colorright + "/" + sex + "/beater_" + sex + "_v");
        sex = tcleft.players.beater2.sex;
        tileRightBeater2 = Resources.Load<Tile>(colorright + "/" + sex + "/beater_" + sex + "_v");
        sex = tcleft.players.chaser1.sex;
        tileRightChaser1 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");
        sex = tcleft.players.chaser2.sex;
        tileRightChaser2 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");
        sex = tcleft.players.chaser3.sex;
        tileRightChaser3 = Resources.Load<Tile>(colorright + "/" + sex + "/chaser_" + sex + "_v");
        sex = tcleft.players.seeker.sex;
        tileRightSeeker = Resources.Load<Tile>(colorright + "/" + sex + "/seeker_" + sex + "_v");
    }


    #endregion TeamConfigSpectator

    #region SnapshotSpectator

    //This methode loads a snapshot
    public void VisualizeSnapshotSpectator(SnapshotJson s)
    {
        int r = s.round;
        string p = s.phase;

        DeltaBroadcastJson dbj = s.lastDeltaBroadcast;

        playerlayer.ClearAllTiles();


        #region balls

        int x = s.balls.bludger1.xPos;
        int y = s.balls.bludger1.yPos;
        Vector3Int v3 = new Vector3Int(x, -y, BALL_Z);//Debug.LogWarning("Bludger1: " + v3);
        tile = tileBludger1;
        playerlayer.SetTile(v3, tile);

        x = s.balls.bludger2.xPos;
        y = s.balls.bludger2.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z);//Debug.LogWarning("Bludger2: " + v3);
        tile = tileBludger2;
        playerlayer.SetTile(v3, tile);

        x = s.balls.quaffle.xPos;
        y = s.balls.quaffle.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z);//Debug.LogWarning("Quaffel: " + v3);
        tile = tileQuaffel;
        playerlayer.SetTile(v3, tile);

        x = s.balls.snitch.xPos;
        y = s.balls.snitch.yPos;
        v3 = new Vector3Int(x, -y, BALL_Z);//Debug.LogWarning("Snitch: " + v3);
        tile = tileSnitch;
        playerlayer.SetTile(v3, tile);

        #endregion balls

        #region left side

        int scoreLeft = s.leftTeam.points;

        x = s.leftTeam.players.keeper.xPos;
        y = s.leftTeam.players.keeper.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftKeeper;
        playerlayer.SetTile(v3, tile);
        bool ko = s.leftTeam.players.keeper.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.beater1.xPos;
        y = s.leftTeam.players.beater1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftBeater1;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.beater1.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.beater2.xPos;
        y = s.leftTeam.players.beater2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftBeater2;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.beater2.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser1.xPos;
        y = s.leftTeam.players.chaser1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser1;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.chaser1.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser2.xPos;
        y = s.leftTeam.players.chaser2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser2;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.chaser2.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.chaser3.xPos;
        y = s.leftTeam.players.chaser3.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftChaser3;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.chaser3.knockout;
        KnockedOut(ko, v3);

        x = s.leftTeam.players.seeker.xPos;
        y = s.leftTeam.players.seeker.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileLeftSeeker;
        playerlayer.SetTile(v3, tile);
        ko = s.leftTeam.players.seeker.knockout;
        KnockedOut(ko, v3);

        #endregion left side

        #region right side

        int scoreRight = s.rightTeam.points;

        x = s.rightTeam.players.keeper.xPos;
        y = s.rightTeam.players.keeper.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightKeeper;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.keeper.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.beater1.xPos;
        y = s.rightTeam.players.beater1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightBeater1;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.beater1.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.beater2.xPos;
        y = s.rightTeam.players.beater2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightBeater2;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.beater2.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser1.xPos;
        y = s.rightTeam.players.chaser1.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser1;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.chaser1.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser2.xPos;
        y = s.rightTeam.players.chaser2.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser2;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.chaser2.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.chaser3.xPos;
        y = s.rightTeam.players.chaser3.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightChaser3;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.chaser3.knockout;
        KnockedOut(ko, v3);

        x = s.rightTeam.players.seeker.xPos;
        y = s.rightTeam.players.seeker.yPos;
        v3 = new Vector3Int(x, -y, PLAYER_Z);
        tile = tileRightSeeker;
        playerlayer.SetTile(v3, tile);
        ko = s.rightTeam.players.seeker.knockout;
        KnockedOut(ko, v3);
        #endregion right side

        tile = tileWombatPoo;
        foreach (WombatCubesJson wc in s.wombatCubes)
        {
            v3 = new Vector3Int(wc.xPos, -wc.yPos, POO_Z);
            playerlayer.SetTile(v3, tile);
        }

        _Menu.GetComponent<Menu>().ChangeScore(scoreLeft, scoreRight);//Change the score

        v3 = new Vector3Int(0, 0, PLAYER_Z);
        playerlayer.SetTile(v3, null);
        v3 = new Vector3Int(0, 0, BALL_Z);
        playerlayer.SetTile(v3, null);

        //update minimap
        _Menu.GetComponent<Menu>().UpdateBallsMinimap(s.balls);
        _Menu.GetComponent<Menu>().UpdatePlayerMinimap(s.leftTeam.players, s.rightTeam.players);


    }

    #endregion SnapshotSpectator

    #endregion VisualizeSpectator

}
