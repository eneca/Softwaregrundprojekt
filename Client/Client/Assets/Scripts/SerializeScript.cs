using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class SerializeScript : MonoBehaviour
{
    public GameObject _ClientServer;

    //Gets Debug from server
    public void SendSendDebug(string s)
    {
        string i = JsonConvert.SerializeObject(   //Weils kein JsonObject gibt
         new
         {
             information = s
         });
        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("sendDebug", i));
    }

    //Gets Replay from server
    public void getReplay()
    {
        string i = JsonConvert.SerializeObject(   //Weils kein JsonObject gibt
         new
         {
         });
        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("getReplay", i));

    }

    //Sends teamConfig to server
    public void SendTeamConfig(TeamConfigJson tcj) //TeamConfig from Editor
    {
        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("teamConfig", tcj));
    }


    #region teamFormation

    //Sends teamFormation to server
    //input = PlayersPos
    public void SendTeamFormation(PlayersPosJson playersPos)
    {
        TeamFormationJson tfj = new TeamFormationJson();
        tfj.players = playersPos;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("teamFormation", tfj));
    }


    //seeker, keeper, chaser1, chaser2, chaser3, beater1, beater2
    //input = PlayerPos
    public PlayersPosJson CreatePlayersPos(PlayerPosJson p1, PlayerPosJson p2, PlayerPosJson p3, PlayerPosJson p4, PlayerPosJson p5, PlayerPosJson p6, PlayerPosJson p7)
    {
        PlayersPosJson p = new PlayersPosJson();
        p.seeker = p1;
        p.keeper = p2;
        p.chaser1 = p3;
        p.chaser2 = p4;
        p.chaser3 = p5;
        p.beater1 = p6;
        p.beater2 = p7;

        return p;
    }

    public PlayerPosJson CreatePlayerPos(int x, int y)
    {
        PlayerPosJson p = new PlayerPosJson();
        p.xPos = x;
        p.yPos = -y;

        return p;
    }
    #endregion teamFormation


    #region pause

    //Sends a pause request to the server
    public void SendPauseRequest(string s)
    {
        PauseRequestJson pr = new PauseRequestJson();
        pr.message = s;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("pauseRequest", pr));
    }

    //Sends a continue request to the server
    public void SendContinueRequestJson(string s)
    {
        ContinueRequestJson cr = new ContinueRequestJson();
        cr.message = s;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("continueRequest", cr));

    }
    #endregion pause


    #region deltaRequest

    //bludgerBeating
    public void SendDeltaRequest(string dtyp, int xposnew, int yposnew, string actentity, string pasentity)
    {
        DeltaRequestJson drj = new DeltaRequestJson();
        drj.deltaType = dtyp;
        drj.xPosNew = xposnew;
        drj.yPosNew = -yposnew;
        drj.activeEntity = actentity;
        drj.passiveEntity = pasentity;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
    }

    //quaffleThrow, move, unban
    public void SendDeltaRequest(string dtyp, int xposnew, int yposnew, string actentity)
    {
        DeltaRequestJson drj = new DeltaRequestJson();
        drj.deltaType = dtyp;
        drj.xPosNew = xposnew;
        drj.yPosNew = -yposnew;
        drj.activeEntity = actentity;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
    }

    //wombatPoo
    public void SendDeltaRequest(string dtyp, int xposnew, int yposnew)
    {
        DeltaRequestJson drj = new DeltaRequestJson();
        drj.deltaType = dtyp;
        drj.xPosNew = xposnew;
        drj.yPosNew = -yposnew;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
    }

    //elfTeleportation, goblinShock 
    //wrestQuaffle, skip
    public void SendDeltaRequest(string dtyp, string entity)
    {
        DeltaRequestJson drj = new DeltaRequestJson();

        if (dtyp == "wrestQuaffle" || dtyp == "skip")
        {
            drj.deltaType = dtyp;
            drj.activeEntity = entity;

            _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
        }
        else
        {
            drj.deltaType = dtyp;
            drj.passiveEntity = entity;

            _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
        }
    }

    //snitchSnatch, trollRoar
    public void SendDeltaRequest(string dtyp)
    {
        DeltaRequestJson drj = new DeltaRequestJson();
        drj.deltaType = dtyp;

        _ClientServer.GetComponent<ClientServer>().SendToS(_ClientServer.GetComponent<ClientServer>().CreateContainer("deltaRequest", drj));
    }
    #endregion deltaRequest
}
