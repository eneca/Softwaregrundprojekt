import de.team05.server.Main;
import de.team05.server.control.MatchConfigController;
import de.team05.server.control.PreGameMessageHandler;
import de.team05.server.json.TeamConfigJson;
import de.team05.server.json.TeamFormationJson;
import de.team05.server.model.Client;
import de.team05.server.model.Game;
import de.team05.server.model.MatchConfig;
import org.java_websocket.WebSocket;
import org.java_websocket.WebSocketImpl;
import org.java_websocket.WebSocketListener;
import org.java_websocket.drafts.Draft;
import org.java_websocket.drafts.Draft_6455;
import org.java_websocket.framing.Framedata;
import org.java_websocket.handshake.ClientHandshake;
import org.java_websocket.handshake.Handshakedata;
import org.java_websocket.handshake.ServerHandshake;
import org.java_websocket.handshake.ServerHandshakeBuilder;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import static org.junit.jupiter.api.Assertions.*;

class PreGameMessageHandlerTest {

    private boolean valid;

    @BeforeEach
    void init(){
        Main.setupLogger(3);
    }

    @Test
    void testOnTeamConfigMessageReceived() throws ClassNotFoundException {
        Client client = createClient("teamConfigTest");

        MatchConfigController matchConfigController = new MatchConfigController("matchconfig.json");
        MatchConfig matchConfig  = matchConfigController.getMatchConfig();

        for (int code = 1; code <= 10; code++){
            PreGameMessageHandler pgm = new PreGameMessageHandler(new Game("test", matchConfig));
            valid = false;
            assertEquals(-1, pgm.onTeamConfigMessageReceived(client,
                    (TeamConfigJson) loadJsonFile(valid, code, false, false)),
                    "Invalid teamConfig was falsely accepted." + "Fehlercode (valid/code):" + valid + "/" + code);
        }

        for (int code = 1; code <= 5; code++){
            PreGameMessageHandler pgm = new PreGameMessageHandler(new Game("test", matchConfig));
            valid = true;
            assertEquals(1, pgm.onTeamConfigMessageReceived(client,
                    (TeamConfigJson) loadJsonFile(valid, code, false, false)),
                    "Valid teamConfig was falsely rejected." + "Fehlercode (valid/code):" + valid + "/" + code);
        }

        PreGameMessageHandler pgm = new PreGameMessageHandler(new Game("test", matchConfig));
        assertEquals(1, pgm.onTeamConfigMessageReceived(client,
                (TeamConfigJson) loadJsonFile(valid, 1, false, false)),
                "Valid teamConfig was falsely rejected." + "Fehlercode (valid/code):" + valid + "/" + 1);
        assertEquals(2, pgm.onTeamConfigMessageReceived(client,
                (TeamConfigJson) loadJsonFile(valid, 2, false, false)),
                "Valid teamConfig was falsely rejected or not both clients could be added to game."
                        + "Fehlercode (valid/code):" + valid + "/" + 2);

    }


    @Test
    void testOnTeamFormationMessageReceived() throws ClassNotFoundException {
        for (int i = 1; i <= 4; i++) {
            MatchConfigController matchConfigController = new MatchConfigController("matchconfig.json");
            MatchConfig matchConfig  = matchConfigController.getMatchConfig();
            PreGameMessageHandler preGameMessageHandler = new PreGameMessageHandler(new Game("test", matchConfig));
            Client client1 = createClient("teamFormationTest1.1."+i);
            Client client2 = createClient("teamFormationTest1.2."+i);

            preGameMessageHandler.onTeamConfigMessageReceived(client1,
                    (TeamConfigJson) loadJsonFile(true, 1, false, false));
            preGameMessageHandler.onTeamConfigMessageReceived(client2,
                    (TeamConfigJson) loadJsonFile(true, 2, false, false));

            assertEquals(-1,preGameMessageHandler.onTeamFormationMessageReceived(client1,
                    (TeamFormationJson) loadJsonFile(false, i, true, true)));
            assertEquals(-1,preGameMessageHandler.onTeamFormationMessageReceived(client2,
                    (TeamFormationJson) loadJsonFile(false, i, true, false)));
        }

        for (int i = 1; i <= 3; i++) {
            MatchConfigController matchConfigController = new MatchConfigController("matchconfig.json");
            MatchConfig matchConfig  = matchConfigController.getMatchConfig();
            PreGameMessageHandler preGameMessageHandler = new PreGameMessageHandler(new Game("test", matchConfig));
            valid = true;
            Client client1 = createClient("teamFormationTest2.1."+i);
            Client client2 = createClient("teamFormationTest2.2."+i);
            preGameMessageHandler.onTeamConfigMessageReceived(client1,
                    (TeamConfigJson) loadJsonFile(valid, 1, false, false));
            preGameMessageHandler.onTeamConfigMessageReceived(client2,
                    (TeamConfigJson) loadJsonFile(valid, 2, false, false));
            assertEquals(1,preGameMessageHandler.onTeamFormationMessageReceived(client1,
                    (TeamFormationJson) loadJsonFile(valid, i, true, true)),
                    "Fehlercode (valid/code):" + valid + "/" + i + " Client 1");
            assertEquals(2,preGameMessageHandler.onTeamFormationMessageReceived(client2,
                    (TeamFormationJson) loadJsonFile(valid, i, true, false)),
                    "Fehlercode (valid/code):" + valid + "/" + i + " Client 2");

        }

    }

    private Object loadJsonFile(boolean valid, int code, boolean teamFormation, boolean leftSide) throws
            ClassNotFoundException {
        return JsonTestLoader.loadJsonObject(valid, (teamFormation?"teamFormation" + (leftSide?"_left":"_right"):"teamConfig"), code );
    }

    Client createClient(String name){
        WebSocketImpl webSocket = new WebSocketImpl(new WebSocketListener() {
            @Override
            public ServerHandshakeBuilder onWebsocketHandshakeReceivedAsServer(WebSocket conn, Draft draft, ClientHandshake request) {
                return null;
            }

            @Override
            public void onWebsocketHandshakeReceivedAsClient(WebSocket conn, ClientHandshake request, ServerHandshake response) {

            }

            @Override
            public void onWebsocketHandshakeSentAsClient(WebSocket conn, ClientHandshake request) {

            }

            @Override
            public void onWebsocketMessage(WebSocket conn, String message){
            }

            @Override
            public void onWebsocketMessage(WebSocket conn, ByteBuffer blob) {

            }

            @Override
            public void onWebsocketOpen(WebSocket conn, Handshakedata d) {

            }

            @Override
            public void onWebsocketClose(WebSocket ws, int code, String reason, boolean remote) {

            }

            @Override
            public void onWebsocketClosing(WebSocket ws, int code, String reason, boolean remote) {

            }

            @Override
            public void onWebsocketCloseInitiated(WebSocket ws, int code, String reason) {

            }

            @Override
            public void onWebsocketError(WebSocket conn, Exception ex) {

            }

            @Override
            public void onWebsocketPing(WebSocket conn, Framedata f) {

            }

            @Override
            public void onWebsocketPong(WebSocket conn, Framedata f) {

            }

            @Override
            public void onWriteDemand(WebSocket conn) {

            }

            @Override
            public InetSocketAddress getLocalSocketAddress(WebSocket conn) {
                return null;
            }

            @Override
            public InetSocketAddress getRemoteSocketAddress(WebSocket conn) {
                return null;
            }
        }, new Draft_6455());

        Client client = new Client(webSocket);
        client.name = name;
        client.password = "pw";
        return  client;
    }

}