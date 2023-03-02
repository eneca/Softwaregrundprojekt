import com.google.gson.Gson;
import de.team05.server.control.JsonUtility;
import org.pmw.tinylog.Logger;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

class JsonTestLoader {

    public static final boolean VALID = true;
    public static final boolean INVALID = false;

    public static Object loadJsonObject(boolean valid, String jsonName, int num) throws ClassNotFoundException {
        String path = "src/test/json/" + jsonName + "/" + (valid?"valid":"invalid") + "_" + jsonName + num + ".json";
        Logger.debug("Loading file " + path);
        try {
            FileReader fileReader = new FileReader(path);

            //read the whole file
            String json = "";
            String line;
            BufferedReader bufferedReader = new BufferedReader(fileReader);
            while ((line = bufferedReader.readLine()) != null) {
                json += line;
            }
            bufferedReader.close();
            Logger.debug("Loaded successfully");

            //parse the Json
            jsonName = jsonName.replace("_left", "");
            jsonName = jsonName.replace("_right", "");

            Gson gson = new Gson();
            return gson.fromJson(json, JsonUtility.getClassFromPayloadType(jsonName));
        } catch (IOException e){
            Logger.debug("Loading failed");
            return null;
        }
    }

    public static String loadJson(boolean valid, String jsonName, int num) {
        String path = "src/test/json/" + jsonName + "/" + (valid?"valid":"invalid") + "_" + jsonName + num + ".json";
        Logger.debug("Loading file " + path);
        try {
            FileReader fileReader = new FileReader(path);

            //read the whole file
            String json = "";
            String line;
            BufferedReader bufferedReader = new BufferedReader(fileReader);
            while ((line = bufferedReader.readLine()) != null) {
                json += line + "\n";
            }
            bufferedReader.close();
            Logger.debug("Loaded successfully");

            return json;
        } catch (IOException e){
            Logger.debug("Loading failed");
            return null;
        }
    }

    public static int amountOfJsons(boolean valid, String jsonName){
        for(int i = 1;;i++) {
            String path = "src/test/json/" + jsonName + "/" + (valid ? "valid" : "invalid") + "_" + jsonName + i + ".json";
            if(Files.notExists(Path.of(path))){
                return i-1;
            }
        }

    }
}
