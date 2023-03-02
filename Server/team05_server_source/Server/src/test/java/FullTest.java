import de.team05.server.Main;
import de.team05.server.control.*;
import de.team05.server.json.ContainerJson;
import de.team05.server.model.MatchConfig;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.pmw.tinylog.Logger;

import java.io.IOException;
import java.net.InetSocketAddress;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

//Test the whole program
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
class FullTest {

    private final String serverAddress = "localhost";
    private final int serverPort = 8080;

    private Server server;
    private MatchConfig matchConfig;
    private int count = 1;

    @BeforeAll
    void loadMatchConfig() {
        Main.setupLogger(3);

        MatchConfigController matchConfigController = new MatchConfigController("matchconfig.json");
        this.matchConfig = matchConfigController.getMatchConfig();
    }


    void init(){
        Logger.info("\n\nStarting new full server test [number " + count + "]");
        count ++;

        //start server
        ClientManager.getInstance().clear();
        GameControllerManager.getInstance().clear();
        server = new Server(new InetSocketAddress(serverAddress, serverPort), matchConfig);
        server.onStart();
    }

    @Test
    void testFull() throws InterruptedException {
        TestLog testLog = TestLog.fromLogFile("full");
        if(testLog == null){
            return;
        }

        testTestLog(testLog);
    }

    @Test
    void testJoinRequest() throws InterruptedException {
        TestLog testLog = TestLog.fromLogFile("join_request");
        if (testLog == null) {
            return;
        }

        testTestLog(testLog);
    }

    @Disabled
    @Test
    void testManyClients() throws InterruptedException {
        TestLog testLog = TestLog.fromLogFile("many_clients");
        if(testLog == null){
            return;
        }

        testTestLog(testLog);
    }

    private void testTestLog(TestLog testLog) throws InterruptedException {


        //is there a num of a json name that has been changed in the course of the test log
        boolean numChanged;

        TestLog.Action a;
        do {
            init();
            testLog.restart();

            numChanged = false;

            //go through the whole test log
            while ((a = testLog.getNextAction()) != null) {
                Logger.trace("Current action: " + a);

                ///check each command
                switch (a.command) {
                    case "connect":
                        connectClient(a.client);
                        break;
                    case "json":
                        sendJson(a.client, a.json, a.valid, a.num);

                        if(a.allNums && !numChanged){
                            //x instead of num. No num has been changed yet
                            if(a.num < JsonTestLoader.amountOfJsons(a.valid, a.json)){
                                //we can increase the num (num has been changed)
                                a.num ++;
                                numChanged = true;
                            }else{
                                //we can reset the num (next num should be changed)
                                a.num = 1;
                            }

                        }
                        break;
                    case "wait":
                        Thread.sleep(a.millis);
                        break;
                }
            }

            checkExpected(testLog);

        }while(numChanged);
    }

    private void checkExpected(TestLog testLog) {
        for(String s : testLog.expectedClientNames){
            Logger.trace("Testing client " + s);
            TestLog.MockupClient mockupClient = testLog.mockupClientsByName.get(s);
            Logger.trace(mockupClient.mockupWebsocket.getMessages());
            for(String expected : mockupClient.expectedJsons){
                String actual = mockupClient.mockupWebsocket.getMessage();
                ContainerJson container = JsonUtility.getContainer(actual);
                assertNotNull(container, "Container was null");
                assertEquals( expected, container.payloadType, "Wrong payload type. Expected: " + expected + ", actual: " + container.payloadType);
            }
            assertEquals(false, mockupClient.mockupWebsocket.hasMessage(), "Too many messages were sent to " + s + ": " + mockupClient.mockupWebsocket.getMessage());
            assertEquals(mockupClient.expectedClose, mockupClient.mockupWebsocket.closed, "MockupWebsocket closed state was not as expected");
        }
    }

    private void sendJson(TestLog.MockupClient client, String jsonName, boolean valid, int num) {
        String json = JsonTestLoader.loadJson(valid, jsonName, num);

        jsonName = jsonName.replace("_left", "");
        jsonName = jsonName.replace("_right", "");

        String container = "{\n" +
                "  \"timestamp\":\"2019-04-25 16:56:20.459\",\n" +
                "  \"payloadType\":\"" + jsonName + "\",\n" +
                "  \"payload\":";
        container += json + "\n}";
        server.onMessage(client.mockupWebsocket, container);
    }

    private void connectClient(TestLog.MockupClient client) {
        client.mockupWebsocket = new MockupWebsocket();
        server.onOpen(client.mockupWebsocket, null);
    }
}
