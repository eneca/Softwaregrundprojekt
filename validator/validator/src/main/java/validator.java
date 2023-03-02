import com.google.gson.Gson;
import data.Teamconfig;

import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Paths;


//USAGE: java -jar validator.jar [json-path]

public class validator {

    public static void main(String[] args) {

        Gson gson = new Gson();
        try {//try falls die Datei die Falsche ist
            Teamconfig teamconfig = gson.fromJson(readFile(args[0], Charset.defaultCharset()), Teamconfig.class);
            //nimmt datei aus der Bash und erzeugt ein Teamconfig-Objekt mit den Attributen der JSON
            String sFinalOutputMessage = teamconfig.teamconfigChecker(); //der finale Output (mit diagnostik) wird von der Methode erzeugt
            if (sFinalOutputMessage.length() == 0) {
                System.out.println("Your Teamconfiguration is valid!"); //wenn der Diagnostikstring leer ist, gibt es keine Probleme, d.h. teamconfig is valid
            } else {
                System.out.println("Your Teamconfiguration is invalid! Reasons:" + "\n" + sFinalOutputMessage); //Diagnostik nicht leer, es existieren fehler, die werden mit ausgegeben
            }
        } catch (IOException e) {
            System.out.println("The specified file is not existing or in the wrong format!"); //Diagnostik für die IOException
        }


    }

    static String readFile(String path, Charset encoding)//Datei einlesen
            throws IOException {
        byte[] encoded = Files.readAllBytes(Paths.get(path));
        return new String(encoded, encoding);
    }

}
