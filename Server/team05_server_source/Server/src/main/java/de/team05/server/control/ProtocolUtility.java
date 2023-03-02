package de.team05.server.control;

import com.google.gson.Gson;
import de.team05.server.json.ContainerJson;
import de.team05.server.model.Client;
import de.team05.server.model.Game;
import org.java_websocket.exceptions.WebsocketNotConnectedException;
import org.pmw.tinylog.Logger;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;

/**
 * Static class for sending messages using the FantasticFeasts protocol
 */
class ProtocolUtility {

    /**
     * Sends an object to a client using WebSocket and JSON.
     * <p>
     * Puts the given object in a container object adding a timestamp and the payload type
     * The object must be a valid json object (classname ends with Json)
     * This does NOT check for any other violations of the Fantastic Feasts protocol in the
     * provided object.
     *
     * @param client Client to send the object to
     * @param o      Object to be converted to JSON and send to the client (valid json object,
     *               classname ends with Json)
     * @return The Container that was sent
     */
    public static ContainerJson sendPayload(Client client, Object o) {
        //create container and add timestamp, payloadType and payload
        ContainerJson container = new ContainerJson();
        container.timestamp = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyy-MM-dd " +
                "HH:mm:ss.SSS"));
        container.payloadType = JsonUtility.getPayloadType(o);
        container.payload = new Gson().toJsonTree(o).getAsJsonObject();

        String toSend = new Gson().toJson(container);
        Logger.trace("Sending message to client " + client.name + ": " + toSend);

        try {
            client.webSocket.send(toSend);
        } catch (WebsocketNotConnectedException e) {
            //the message can not be sent
            Logger.warn("Cannot send message to client " + client.name);
        }
        return container;
    }

    /**
     * Sends an object to a list of clients using WebSocket and JSON.
     * <p>
     * Puts the given object in a container object adding a timestamp and the payload type
     * The object must be a valid json object (classname ends with Json)
     * This does NOT check for any other violations of the Fantastic Feasts protocol in the
     * provided object.
     *
     * @param clients List of clients to send the object to
     * @param o       Object to be converted to JSON and send to the clients (valid json object,
     *                classname ends with Json)
     * @return The Container that was sent
     */
    public static ContainerJson sendPayload(List<Client> clients, Object o) {
        ContainerJson container = null;
        for (Client c : clients) {
            container = sendPayload(c, o);
        }
        return container;
    }

    /**
     * Sends an object to all clients in the game (including spectators) using WebSocket and JSON.
     * <p>
     * Puts the given object in a container object adding a timestamp and the payload type
     * The object must be a valid json object (classname ends with Json)
     * This does NOT check for any other violations of the Fantastic Feasts protocol in the
     * provided object.
     *
     * @param game The game to which the Object is broadcasted
     * @param o Object to be converted to JSON and broadcasted to all clients
     * @return The Container that was sent
     */
    public static ContainerJson broadcastPayload(Game game, Object o) {
        ContainerJson container = null;
        if (game.getLeftClient() != null) {
            container = sendPayload(game.getLeftClient(), o);
        }
        if (game.getRightClient() != null) {
            container = sendPayload(game.getRightClient(), o);
        }
        for (Client c : game.spectators) {
            container = sendPayload(c, o);
        }
        return container;
    }

    /**
     * Removes a client from the server by sending a debug message and removing the client from
     * the client manager
     *
     * @param client Client to be kicked
     * @param info   Further information that is send to the client
     */
    public static void kickClient(Client client, String info) {
        DebugController.sendPrivateDebug(client, "You have been kicked from the server: " + info);
        ClientManager.getInstance().removeClient(client);
        client.kicked = true;
        Logger.warn("Client " + client.name + " has been kicked from the server: " + info);
    }
}
