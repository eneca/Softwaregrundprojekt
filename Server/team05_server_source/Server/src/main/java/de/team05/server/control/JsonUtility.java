package de.team05.server.control;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import de.team05.server.json.ContainerJson;
import org.pmw.tinylog.Logger;

/**
 * This class handles the extracting of sent JSON-files and the parsing to the respective class.
 */
public class JsonUtility {

    /**
     * The Gson instance that is used for the parsing
     */
    private static final Gson gson = new GsonBuilder()
            .registerTypeAdapter(ContainerJson.class, new ContainerDeserializer())
            .create();


    /**
     * This method parses an incoming JSON-String to an Container object
     *
     * @param message incoming SON-String
     *
     * @return Container-type object
     */
    public static ContainerJson getContainer(String message) {
        return gson.fromJson(message, ContainerJson.class);
    }

    /**
     * This method extracts the payload of the given container
     *
     * @param container JSON container which wraps the type and payload of a message
     *
     * @param classOfT blueprint in which data of payload will be parsed
     *
     * @param <T> Type of the Class we want to parse the payload to
     *
     * @return the class, which has now the data from the container
     */
    public static <T> T getPayload(ContainerJson container, Class<T> classOfT) {
        return gson.fromJson(container.payload, classOfT);
    }

    /**
     * This method converts a JSON-String to the given class.
     *
     * @param s JSON-String that shall be converted
     *
     * @param classOfT class in which the String will be converted
     *
     * @param <T> Type of the Class we want to parse the payload to
     *
     * @return the class, which has now the data from the JSON-String
     */
    public static <T> T getPayload(String s, Class<T> classOfT) {
        return gson.fromJson(s, classOfT);
    }

    /**
     * This method converts the payload-type into the name of an in server.json existing class
     *
     * @param payloadType extracted type from container
     *
     * @return Class with converted name
     *
     * @throws ClassNotFoundException for the unlikely case a not existing payload type is sent
     */
    public static Class getClassFromPayloadType(String payloadType) throws ClassNotFoundException {

        //set the first letter to uppercase
        char[] c = payloadType.toCharArray();
        c[0] = Character.toUpperCase(c[0]);
        String modifiedPayloadType = new String(c);

        //add the "Json" part of the classname
        modifiedPayloadType += "Json";

        return Class.forName("de.team05.server.json." + modifiedPayloadType);
            /*
        } catch (ClassNotFoundException e) {
            Logger.error("Something went wrong trying to get the Json class for the " +
                    "payload type "
                    + payloadType + ". Modified payload type: " + modifiedPayloadType);
        }
        */
        //return null;
    }

    /**
     * This method converts the name of the given object into a payload-type String
     *
     * @param o object which shall be converted
     *
     * @return Name of the object without "Json" at the end and first letter to lowercase
     */
    public static String getPayloadType(Object o) {
        String className = o.getClass().getSimpleName();

        //set the first letter to lowercase
        char[] c = className.toCharArray();
        c[0] = Character.toLowerCase(c[0]);
        className = new String(c);

        //cut the "Json" part of the classname
        if (className.length() >= 4) {
            className = className.substring(0, className.length() - 4);
        }

        Logger.trace("Got payloadType for " + o.getClass().getCanonicalName() + ": " + className);
        return className;
    }

}
