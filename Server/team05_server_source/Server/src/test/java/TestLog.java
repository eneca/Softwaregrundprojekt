import org.pmw.tinylog.Logger;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class TestLog {
    public final List<Action> actions;
    public final HashMap<String, MockupClient> mockupClientsByName;
    public final List<String> expectedClientNames;
    int curr;

    private TestLog() {
        actions = new ArrayList<>();
        mockupClientsByName = new HashMap<>();
        expectedClientNames = new ArrayList<>();
        curr = 0;
    }

    public void restart(){
        curr = 0;
    }

    public Action getNextAction(){
        if(curr < actions.size()) {
            return actions.get(curr++);
        }else{
            return null;
        }
    }

    public static TestLog fromLogFile(String logName){
        TestLog testLog = new TestLog();

        String path = "src/test/json/logs/" + logName + ".testlog";
        try {
            FileReader fileReader = new FileReader(path);

            String line;
            int currLine = 0;
            boolean expected = false;
            BufferedReader bufferedReader = new BufferedReader(fileReader);
            while ((line = bufferedReader.readLine()) != null) {
                currLine ++;

                //ignore empty lines
                if(line.equals("")){
                    continue;
                }

                //cut comments
                line = line.split("//")[0];

                //get the parts of the command
                String[] parts = line.split(" ");

                if(!expected) {
                    //log
                    Action action = new Action();

                    if (parts.length < 1) {
                        throw new IllegalArgumentException("Invalid testlog: missing command in line " + currLine);
                    }

                    switch (parts[0]) {
                        case "connect":
                            //connect command: "connect CLIENTNAME"
                            if (parts.length != 2) {
                                throw new IllegalArgumentException("Invalid testlog: missing argument in line " + currLine);
                            }
                            action.command = "connect";
                            action.client = new MockupClient(parts[1]);
                            testLog.mockupClientsByName.put(parts[1], action.client);
                            break;
                        case "json":
                            //json command: "json CLIENT_NAME JSON_NAME ISVALID NUM"
                            if (parts.length != 5) {
                                throw new IllegalArgumentException("Invalid testlog: missing argument in line " + currLine);
                            }
                            action.command = "json";

                            action.client = testLog.mockupClientsByName.get(parts[1]);

                            action.json = parts[2];

                            action.valid = parts[3].equals("valid");

                            if (parts[4].equals("x")) {
                                action.num = 1;
                                action.allNums = true;
                            } else {
                                action.num = Integer.parseInt(parts[4]);
                                action.allNums = false;
                            }
                            break;
                        case "wait":
                            //wait commant: "wait MILLIS"
                            if (parts.length != 2) {
                                throw new IllegalArgumentException("Invalid testlog: missing argument in line " + currLine);
                            }
                            action.command = "wait";
                            action.millis = Long.parseLong(parts[1]);
                            break;
                        case "EXPECTED":
                            //end of log, start of expected
                            expected = true;
                            continue;

                    }
                    testLog.actions.add(action);
                }else{
                    //expected
                    MockupClient c = testLog.mockupClientsByName.get(parts[0]);
                    c.expectedClose = parts[1].equals("closed");
                    for(int i = 2; i < parts.length; i ++){
                        c.expectedJsons.add(parts[i]);
                    }
                    testLog.expectedClientNames.add(parts[0]);
                }
            }
            bufferedReader.close();

        } catch (IOException e){
            Logger.debug("Loading log failed");
            return null;
        }
        return testLog;
    }


    public static class Action {
        public String command;
        public MockupClient client;
        public String json;
        public boolean valid;
        public int num;
        public long millis;
        public boolean allNums = false;

        public Action(){
        }

        @Override
        public String toString() {
            return "Action{" +
                    "command='" + command + '\'' +
                    ", client=" + client +
                    ", json='" + json + '\'' +
                    ", valid=" + valid +
                    '}';
        }
    }

    public static class MockupClient{
        public final String name;
        public MockupWebsocket mockupWebsocket;
        public final List<String> expectedJsons;
        public boolean expectedClose = false;

        public MockupClient(String name) {
            this.name = name;
            expectedJsons = new ArrayList<>();
        }

        public boolean hasWebsocket(){
            return mockupWebsocket != null;
        }
    }
}
