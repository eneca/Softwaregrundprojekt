using System;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Basically same as ClientServer.cs, but without connection to GameLoop
/// </summary>
public class ClientSpectator : MonoBehaviour
{
    /*Suppose to be input from gui*/
    public Text ServerIpAddress;
    public Text ServerPort;
    public Text username;
    public Text userlobby;
    public Text userpassword;

    private string url;
    private static ClientWebSocket client = null;
    private bool showDisc = true;

    public GameObject visual;
    public GameObject menu;


    #region initial connection

    public void StartConnectingSpectator()
    {
        ConnectToS();

        //Debug.Log(ServerIpAddress.text);
        //Debug.Log(ServerPort.text);
        //Debug.Log(username.text);
        //Debug.Log(userlobby.text);
        //Debug.Log(userpassword.text);
    }

    /*Build connection to Server with first message*/
    private async void ConnectToS()
    {

        client = new ClientWebSocket();

        Debug.Log("Versuch zu Verbinden");

        if (!IsValidUrl())
        {
            Debug.Log("Kein gültiger Port/Adresse ");
        }

        try
        {
            await client.ConnectAsync(new Uri(url), CancellationToken.None); //Build connection
            if (client.State == WebSocketState.Open)
            {

                ArraySegment<byte> bts = new ArraySegment<byte>(StringToByteArray(CreateContainer("joinRequest", CreateJoinRequest()))); //Creates container with needed data

                await client.SendAsync(bts, WebSocketMessageType.Text, true, CancellationToken.None); //Sending message in plain text to Server

                RecieveMessage();
            }
        }

        catch (Exception e)
        {
            Debug.Log(e);
            menu.GetComponent<Menu>().ShowDisconnectError();
            menu.GetComponent<Menu>().StopLoading();
        }
    }

    /*"Checks" if url is at least not empty*/
    private bool IsValidUrl()
    {
        if (ServerPort.text != "" && ServerIpAddress.text != "")
        {
            url = "ws://" + ServerIpAddress.text + ":" + ServerPort.text; //string
            return true;
        }
        return false;


    }

    /*Creates Json for the JoinRequest*/
    private JoinRequestJson CreateJoinRequest()
    {
        JoinRequestJson jjr = new JoinRequestJson();

        jjr.lobby = this.userlobby.text;
        jjr.password = this.userpassword.text;
        jjr.userName = this.username.text;
        jjr.isArtificialIntelligence = false;
        jjr.mods = new string[0];

        string output = JsonConvert.SerializeObject(jjr); //Convert to json

        return jjr;
    }

    #endregion initial connection


    #region send methode

    /*Methode for sending messages to the server*/
    /*Message has to go through methode CreateContainer first for the server to accept the messages format*/
    public async void SendToS(string s)
    {
        ArraySegment<byte> bts = new ArraySegment<byte>(StringToByteArray(s));

        await client.SendAsync(bts, WebSocketMessageType.Text, true, CancellationToken.None); //Sending message in plain text to Server
    }

    /*Creates a json container with input parameters payloadtype and json*/
    public string CreateContainer(string payloadT, object dataJson)
    {
        string s = JsonConvert.SerializeObject(
         new
         {
             timestamp = GetTimeStamp(DateTime.Now),
             payloadType = payloadT,
             payload = dataJson
         });

        return s;
    }

    /*Methode to get the exact time in the needed format*/
    private string GetTimeStamp(DateTime time)
    {
        return time.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    /*Transformes string to an array of bytes*/
    private byte[] StringToByteArray(string str)
    {
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        return enc.GetBytes(str);
    }

    #endregion send methode

    #region recieve

    /*Receive messages from server*/
    private async void RecieveMessage()
    {
        try
        {
            while (client.State == WebSocketState.Open || client.State == WebSocketState.Connecting)
            {
                byte[] rcvBytes = new byte[1024];
                ArraySegment<byte> rcvBuffer = new ArraySegment<byte>(rcvBytes);
                WebSocketReceiveResult rcvResult = null;

                var allBytes = new List<byte>();
                do
                {
                    rcvResult = await client.ReceiveAsync(rcvBuffer, CancellationToken.None);
                    for (int i = 0; i < rcvResult.Count; i++)
                    {
                        allBytes.Add(rcvBuffer.Array[i]);
                    }
                }
                while (!rcvResult.EndOfMessage);

                string message = Encoding.UTF8.GetString(allBytes.ToArray(), 0, allBytes.Count);
                DecryptMessage(message);

            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            if (showDisc)
            {
                menu.GetComponent<Menu>().ShowDisconnectError();
            }
        }

    }

    /*Finds out what the payloadType is*/
    //unicast= Server to one client
    //broadcast= Server to all clients
    private void DecryptMessage(string rcvmsg)
    {
        var json = JsonConvert.DeserializeObject<ContainerJson>(rcvmsg);

        int i;
        string s;
        string s2;

        Debug.Log(json.payloadType + " " + json.payload);
        switch (json.payloadType)
        {
            case "joinResponse": //unicsat

                JoinResponseJson jr = JsonConvert.DeserializeObject<JoinResponseJson>(json.payload.ToString());
                s = jr.message;

                break;

            case "loginGreeting": //broadcast

                LoginGreetingJson lg = JsonConvert.DeserializeObject<LoginGreetingJson>(json.payload.ToString());
                s = lg.userName;

                //Debug.Log(s);
                break;

            case "globalDebug": //broadcast

                GlobalDebugJson gd = JsonConvert.DeserializeObject<GlobalDebugJson>(json.payload.ToString());
                s = gd.information;

                //Debug.Log(s);
                break;

            case "privateDebug": //unicast

                PrivateDebugJson pd = JsonConvert.DeserializeObject<PrivateDebugJson>(json.payload.ToString());
                s = pd.information;

                //Debug.Log(s);
                break;

            case "matchStart": //broadcast

                DissolveMatchstart(json.payload.ToString());

                break;

            case "snapshot": //broadcast

                SnapshotJson sj = JsonConvert.DeserializeObject<SnapshotJson>(json.payload.ToString());

                visual.GetComponent<Visualize>().VisualizeSnapshotSpectator(sj);

                menu.GetComponent<Menu>().StopLoading();

                break;

            case "pauseResponse": //broadcast

                PauseResponseJson prj = JsonConvert.DeserializeObject<PauseResponseJson>(json.payload.ToString());
                s = prj.message;
                s2 = prj.userName;
                bool b = prj.pause;
                visual.GetComponent<Timer>().pause = b;

                break;

            case "next": //broadcast

                DissolveNext(json.payload.ToString());

                break;

            case "matchFinish": //broadcast

                MatchFinishJson mfj = JsonConvert.DeserializeObject<MatchFinishJson>(json.payload.ToString());

                s = mfj.winnerUserName;
                s2 = mfj.victoryReason;
                i = mfj.endRound;
                int lp = mfj.leftPoints;
                int rp = mfj.rightPoints;

                visual.GetComponent<Timer>().enabled = false;
                menu.GetComponent<Menu>().WinOrDefeat(lp, rp, s2, s);
                showDisc = false;
                Disconnect();
                break;

            case "replay": //broadcast

                ReplayJson rj = JsonConvert.DeserializeObject<ReplayJson>(json.payload.ToString());

                s = rj.lobby;
                s2 = rj.startTimestamp;
                MatchConfigJson mcj = rj.matchConfig;
                TeamConfigJson tcll = rj.leftTeamConfig;
                TeamConfigJson tcrr = rj.rightTeamConfig;
                string ltu = rj.leftTeamUserName;
                string rtu = rj.rightTeamUserName;
                string[] sunrj = rj.spectatorUserName;
                SnapshotJson fss = rj.firstSnapshot;
                LogJson[] l = rj.log;

                break;

            case "reconnect": //unicast

                ReconnectJson rc = JsonConvert.DeserializeObject<ReconnectJson>(json.payload.ToString());
                ContainerJson msrc = rc.matchStart;
                ContainerJson sjrc = rc.snapshot;
                ContainerJson nrc = rc.next;

                DissolveMatchstart(rc.matchStart.payload.ToString());

                SnapshotJson rsj = JsonConvert.DeserializeObject<SnapshotJson>(rc.snapshot.payload.ToString());

                visual.GetComponent<Visualize>().VisualizeSnapshotSpectator(rsj);

                DissolveNext(rc.next.payload.ToString());

                break;

            case "privateError":

                Debug.Log(json.payload);

                break;

            case "privateWarning":

                Debug.Log(json.payload);

                break;

            default:

                break;
        }
    }

    //Decrypt MatchStart message
    private void DissolveMatchstart(string matchString)
    {
        MatchStartJson ms = JsonConvert.DeserializeObject<MatchStartJson>(matchString);

        String s = ms.leftTeamUserName;
        String s2 = ms.rightTeamUserName;
        TeamConfigJson tcl = ms.leftTeamConfigJson; //TeamConfig data of left team
        TeamConfigJson tcr = ms.rightTeamConfigJson; // TeamConfig data of right team

        visual.GetComponent<Visualize>().DissolveTeamConfigSpectator(tcl, tcr);
        visual.GetComponent<Timer>().GUITimer(ms.matchConfigJson.timings.teamFormationTimeout);
        visual.GetComponent<Timer>().EnableScript();
        menu.GetComponent<Menu>().StopLoading();//Stops displaying loadingscreen
    }

    //Decrypt Next message
    private void DissolveNext(string next)
    {
        NextJson nj = JsonConvert.DeserializeObject<NextJson>(next);
       visual.GetComponent<Timer>().GUITimer(nj.timeout);
    }


    #endregion recieve


    //Rebuilds connection to server
    public async void ReconnectToServer()
    {
        try
        {
            await client.ConnectAsync(new Uri(url), CancellationToken.None);
            SendToS(CreateContainer("joinRequest", CreateJoinRequest()));
            RecieveMessage();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    //Disconnects from server
    public void Disconnect()
    {
        client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
    }

}
