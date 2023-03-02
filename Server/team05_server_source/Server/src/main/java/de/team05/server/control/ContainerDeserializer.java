package de.team05.server.control;

import com.google.gson.*;
import de.team05.server.json.ContainerJson;

import java.lang.reflect.Type;

/**
 * The ContainerDeserializer is a JsonSerializer that is used to parse containers and put them in
 * the ContainerJson class.
 */
class ContainerDeserializer implements JsonDeserializer {


    /**
     * Deserializes a given JsonoElement and returns an object of a given type
     *
     * @param jsonElement                element which will be deserialized to a container
     * @param type                       no function, exists due to implementation of interface
     * @param jsonDeserializationContext no function, exists due to implementation of interface
     * @return containerJson file
     * @throws JsonParseException is thrown if JSON-file contains no valid container
     */
    @Override
    public Object deserialize(JsonElement jsonElement, Type type,
                              JsonDeserializationContext jsonDeserializationContext) throws JsonParseException {
        ContainerJson container = new ContainerJson();
        try {
            container.payloadType = jsonElement.getAsJsonObject().get("payloadType").getAsString();
            container.timestamp = jsonElement.getAsJsonObject().get("timestamp").getAsString();
            container.payload = jsonElement.getAsJsonObject().getAsJsonObject("payload");
        }catch(NullPointerException e){
            throw new JsonSyntaxException("Malformed container.");
        }
        return container;
    }
}
