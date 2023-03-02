package de.team05.server.control;

import de.team05.server.model.Client;
import org.java_websocket.WebSocket;
import org.pmw.tinylog.Logger;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

/**
 * The client manager is a Singleton class which stores information about all clients that are currently on the server.
 * <p>
 * Stores all the connected clients, that can be accessed using their WebSocket connection and all the disconnected clients, that can be accessed using their user name.
 * Clients should be removed when they are not needed anymore and marked disconnected if they are still in a lobby/game but the connection was closed for some reason.
 */
public class ClientManager {

    /**
     * All connected clients in a hashmap where they can be accessed using their WebSocket
     * <p>
     * This is useful to determine the corresponding client for an incoming message via a specific WebSocket connection.
     */
    private final HashMap<WebSocket, Client> clientsByWebsocket;

    /**
     * A list of all clients currently on the server (connected and disconnected)
     */
    private final List<Client> allClients;

    /**
     * The singleton instance of the client manager
     */
    private static final ClientManager INSTANCE = new ClientManager();

    /**
     * Creates the singleton instance of the client manager
     */
    private ClientManager() {
        clientsByWebsocket = new HashMap<>();
        allClients = new ArrayList<>();
    }

    /**
     * Returns the singleton instance of the client manager.
     *
     * @return The singleton instance of the client manager
     */
    public static ClientManager getInstance() {
        return INSTANCE;
    }

    /**
     * Adds a client to the client manager
     *
     * @param client Client to be added
     */
    public void addClient(Client client) {
        this.clientsByWebsocket.put(client.webSocket, client);
        allClients.add(client);
    }

    /**
     * Returns the connected client with the corresponding WebSocket connection
     *
     * @param webSocket WebSocket connection of the connected client
     * @return The connected client with the corresponding WebSocket connection
     */
    public Client getClient(WebSocket webSocket) {
        return clientsByWebsocket.get(webSocket);
    }

    /**
     * Returns the client with the specified user name
     *
     * @param name Username of the client
     * @return The client with the specified user name or null if there is no client with that name
     */
    public Client getClient(String name) {
        for(Client c : allClients){
            if(c.name.equals(name)){
                return c;
            }
        }
        return null;
    }

    /**
     * Removes a client from the client manager and closes the websocket connection
     *
     * @param client Client to be removed
     */
    public void removeClient(Client client) {
        clientsByWebsocket.remove(client.webSocket);
        allClients.remove(client);
        client.webSocket.close();
        Logger.debug("Client " + client.toString() + " removed from the server");
    }

    /**
     * Returns all clients currently on the server (connected and disconnected)
     *
     * @return All clients currently on the server (connected and disconnected)
     */
    public List<Client> getAllClients() {
        return allClients;
    }

    public void clear(){
        clientsByWebsocket.clear();
        allClients.clear();
    }

    /**
     * Returns client manager information as a string
     *
     * @return string of full client manager information
     */
    @Override
    public String toString() {
        return "ClientManager{\n" +
                ",\n\tallClients=" + allClients +
                "\n}";
    }
}
