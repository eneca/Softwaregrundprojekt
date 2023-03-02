using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class SetPhase : MonoBehaviour
{

    public Tilemap tilemap;
    public Tilemap setzen;

    private Tile testtile;

    private string colorleft;
    private Tile tileKeeper;
    private Tile tileBeater;
    private Tile tileChaser;
    private Tile tileSeeker;
    private Tile[] tileArr;
    public bool half;
    private CheckScript cs = new CheckScript();

    public GameObject _visualize;
    public GameObject _menu;
    public GameObject _gLoop;
    public GameObject _spielfeld;

    public Player[] allPlayersOwnTeam;
    private int index;

    private Vector2 oldPos2;
    private Vector2 newPos2 = new Vector2(-1.8f, 0);
    private Vector3 oldScale3 = new Vector3(0.13f, 0.13f, 1);
    private Vector3 newScale3 = new Vector3(0.1f, 0.1f, 1);


    private bool init = false;
    public bool isUnban = false;
    private string unbanPlayer;

    // Start is called before the first frame update
    public void InitClass()
    {
        oldPos2 = _spielfeld.transform.localPosition;
        _spielfeld.transform.localPosition = newPos2;

        _spielfeld.transform.localScale = newScale3;

        tileArr = new Tile[7];
        allPlayersOwnTeam = new Player[7];

        for (int i = 0; i <= 6; i++)
        {
            allPlayersOwnTeam[i] = new Player();
        }
        half = _visualize.GetComponent<Visualize>().side;

        if (half)
        {
            _menu.GetComponent<Menu>().Animation("You're left");
        }
        else
        {
            _menu.GetComponent<Menu>().Animation("You're right");
        }

        if (half)
        {
            tileKeeper = _visualize.GetComponent<Visualize>().tileLeftKeeper;
            tileBeater = _visualize.GetComponent<Visualize>().tileLeftBeater1;
            tileChaser = _visualize.GetComponent<Visualize>().tileLeftChaser1;
            tileSeeker = _visualize.GetComponent<Visualize>().tileLeftSeeker;
        }
        else
        {
            tileKeeper = _visualize.GetComponent<Visualize>().tileRightKeeper;
            tileBeater = _visualize.GetComponent<Visualize>().tileRightBeater1;
            tileChaser = _visualize.GetComponent<Visualize>().tileRightChaser1;
            tileSeeker = _visualize.GetComponent<Visualize>().tileRightSeeker;
        }

        try
        {
            allPlayersOwnTeam[0].playerRole = Role.Seeker;
            allPlayersOwnTeam[0].banned = true;
            allPlayersOwnTeam[1].playerRole = Role.Keeper;
            allPlayersOwnTeam[1].banned = true;
            allPlayersOwnTeam[2].playerRole = Role.Chaser1;
            allPlayersOwnTeam[2].banned = true;
            allPlayersOwnTeam[3].playerRole = Role.Chaser2;
            allPlayersOwnTeam[3].banned = true;
            allPlayersOwnTeam[4].playerRole = Role.Chaser3;
            allPlayersOwnTeam[4].banned = true;
            allPlayersOwnTeam[5].playerRole = Role.Beater1;
            allPlayersOwnTeam[5].banned = true;
            allPlayersOwnTeam[6].playerRole = Role.Beater2;
            allPlayersOwnTeam[6].banned = true;
        }
        catch (Exception e) { Debug.Log(e); }


        tileArr[0] = tileSeeker;
        tileArr[1] = tileKeeper;
        tileArr[2] = tileChaser;
        tileArr[3] = tileChaser;
        tileArr[4] = tileChaser;
        tileArr[5] = tileBeater;
        tileArr[6] = tileBeater;

        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (init)
        {
            _gLoop.GetComponent<GameLoop>().DisplaySadPhaseArea(half);
            if (isUnban)
            {
                for (int i = 0; i<allPlayersOwnTeam.Length;i++)
                {
                    if (allPlayersOwnTeam[i].playerRole.ToString().Equals(unbanPlayer))
                    {
                        index = i;
                    }
                }
            }
            else
            {
                index = _menu.GetComponent<Menu>().lastSet;
            }
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = tilemap.WorldToCell(mousePos);
            //allPlayersOwnTeam[index].playerRole.ToString();

            if (half)
            {
                if (tilemap.HasTile(gridPos))// makes sure you're on the tilemap
                {
                    Vector3Int g = new Vector3Int(gridPos.x, gridPos.y, -5);

                    if (Input.GetMouseButtonDown(0) && gridPos.x < 8 && !cs.PosIsGoal(gridPos) && !setzen.HasTile(g) && allPlayersOwnTeam[index].banned&& !CheckCircle(gridPos))// checks if mouse is clicked
                    {
                        if (isUnban && allPlayersOwnTeam[index].playerRole.ToString().Equals(unbanPlayer))
                        {
                            testtile = tileArr[index];
                            setzen.SetTile(new Vector3Int(gridPos.x, gridPos.y, -5), testtile);
                            allPlayersOwnTeam[index].pos = gridPos;
                            allPlayersOwnTeam[index].banned = false;
                            _menu.GetComponent<Menu>().ChangeViewState(index, false);
                            _gLoop.GetComponent<GameLoop>().UnbanDelta(gridPos, allPlayersOwnTeam[index].playerRole.ToString());
                            Terminate();
                        }
                        else if (!isUnban)
                        {
                            testtile = tileArr[index];
                            setzen.SetTile(new Vector3Int(gridPos.x, gridPos.y, -5), testtile);
                            allPlayersOwnTeam[index].pos = gridPos;
                            allPlayersOwnTeam[index].banned = false;
                            _menu.GetComponent<Menu>().ChangeViewState(index, false);
                        }
                    }
                    tilemap.SetTileFlags(gridPos, TileFlags.None);
                    tilemap.SetColor(gridPos, Color.magenta);
                }
            }
            else
            {
                if (tilemap.HasTile(gridPos))// makes sure you're in the tilemap
                {
                    Vector3Int g = new Vector3Int(gridPos.x, gridPos.y, -5);

                    if (Input.GetMouseButtonDown(0) && gridPos.x > 8 && !cs.PosIsGoal(gridPos) && !setzen.HasTile(g) && allPlayersOwnTeam[index].banned && !CheckCircle(gridPos))// checks if mouse is clicked
                    {
                        if (isUnban && allPlayersOwnTeam[index].playerRole.ToString().Equals(unbanPlayer))
                        {
                            testtile = tileArr[index];
                            setzen.SetTile(new Vector3Int(gridPos.x, gridPos.y, -5), testtile);
                            allPlayersOwnTeam[index].pos = gridPos;
                            allPlayersOwnTeam[index].banned = false;
                            _menu.GetComponent<Menu>().ChangeViewState(index, false);
                            _gLoop.GetComponent<GameLoop>().UnbanDelta(gridPos, allPlayersOwnTeam[index].playerRole.ToString());
                            Terminate();
                        }
                        else if (!isUnban)
                        {
                            testtile = tileArr[index];
                            setzen.SetTile(new Vector3Int(gridPos.x, gridPos.y, -5), testtile);
                            allPlayersOwnTeam[index].pos = gridPos;
                            allPlayersOwnTeam[index].banned = false;
                            _menu.GetComponent<Menu>().ChangeViewState(index, false);
                        }
                    }
                    tilemap.SetTileFlags(gridPos, TileFlags.None);
                    tilemap.SetColor(gridPos, Color.magenta);
                }
            }
            
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    if (gridPos.x == j && gridPos.y == -i) { }
                    else
                    {
                        Vector3Int v3 = new Vector3Int(j, -i, -4);
                        if (!isUnban)
                        {
                            setzen.SetTile(v3, null);
                        }
                        
                        v3 = new Vector3Int(j, -i, gridPos.z);
                        if (tilemap.GetColor(v3) == Color.magenta)
                        {
                            tilemap.SetColor(v3, Color.white);
                        }
                    }
                }
            }
        }
    }
    private bool CheckCircle(Vector3Int pos)
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

    public void Terminate()
    {
        if (isUnban)
        {
            _gLoop.GetComponent<GameLoop>().SetPlayersOwnTeam(allPlayersOwnTeam);
            this.enabled = false;
            //init = false;
        }
        else
        {
            if (AllSet())
            {
                _gLoop.GetComponent<GameLoop>().SetPlayersOwnTeam(allPlayersOwnTeam);
                _gLoop.GetComponent<GameLoop>().SendTeamFormationToServer();
                _spielfeld.transform.localPosition = oldPos2;
                _spielfeld.transform.localScale = oldScale3;
                this.enabled = false;
                //init = false;
            }
        }
        isUnban = false; ;
    }

    public bool AllSet()
    {
        foreach (Player p in allPlayersOwnTeam)
        {
            if (p.banned)
            {
                return false;
            }
        }
        return true;
    }
    public void StartScript()
    {
        this.enabled = true;
        setzen.ClearAllTiles();
    }

    public void StartUnban(Player[] pla, string bannedGuy)
    {
        isUnban = true;
        allPlayersOwnTeam = pla;
        unbanPlayer = bannedGuy;
        for (int i = 0; i < allPlayersOwnTeam.Length; i++)
        {
            if (allPlayersOwnTeam[i].banned && bannedGuy.Equals(allPlayersOwnTeam[i].playerRole.ToString()))
            {
                _menu.GetComponent<Menu>().ChangeViewState(i, true);
            }
            else
            {
                _menu.GetComponent<Menu>().ChangeViewState(i, false);
            }
        }
        _menu.GetComponent<Menu>().UnbanPlacing();
        this.enabled = true;
    }
    public void StandardFormation()
    {
        for (int i = 0; i <= 6; i++)
        {
            if (half)
            {
                setzen.SetTile(new Vector3Int(5, -(i + 1), -5), tileArr[i]);
                allPlayersOwnTeam[i].pos = new Vector3Int(5, -(i + 1), 0);
                allPlayersOwnTeam[i].banned = false;
            }
            else
            {
                setzen.SetTile(new Vector3Int(10, -(i + 1), -5), tileArr[i]);
                allPlayersOwnTeam[i].pos = new Vector3Int(10, -(i + 1), 0);
                allPlayersOwnTeam[i].banned = false;
            }
        }
        Terminate();
    }
}
