package de.team05.server.control;

import de.team05.server.json.GlobalDebugJson;
import de.team05.server.json.PrivateDebugJson;
import de.team05.server.model.Client;
import org.pmw.tinylog.Logger;

import java.util.List;

/**
 * The DebugController handles creating and sending of debug messages.
 */
class DebugController {

    /**
     * This method sends a debug message to all clients
     * @param message message which shall be sent as debug
     */
    public static void sendGlobalDebug(String message) {
        List<Client> allClients = ClientManager.getInstance().getAllClients();

        for (Client c : allClients) {
            GlobalDebugJson globalDebug = new GlobalDebugJson();
            globalDebug.information = message;
            ProtocolUtility.sendPayload(c, globalDebug);
        }
        Logger.debug("Sent global debug: " + message);
    }

    /**
     * This method sends a debug message to a specific client
     *
     * @param client  the client which shall receive the debug message
     * @param message message which shall be sent as debug
     */
    public static void sendPrivateDebug(Client client, String message) {
        PrivateDebugJson privateDebug = new PrivateDebugJson();
        privateDebug.information = message;
        ProtocolUtility.sendPayload(client, privateDebug);
        Logger.debug("Sent private debug to client " + client.toString() + ": " + message);
    }
}
