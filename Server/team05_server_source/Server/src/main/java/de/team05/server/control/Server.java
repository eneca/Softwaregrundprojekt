package de.team05.server.control;

import de.team05.server.Strings;
import de.team05.server.json.ContainerJson;
import de.team05.server.json.JoinRequestJson;
import de.team05.server.model.Client;
import de.team05.server.model.MatchConfig;
import org.java_websocket.WebSocket;
import org.java_websocket.handshake.ClientHandshake;
import org.java_websocket.server.WebSocketServer;
import org.pmw.tinylog.Logger;

import java.io.IOException;
import java.net.InetSocketAddress;

/**
 * The Server class handles the Websocket connection to the clients.
 * It receives messages from a given port using Websocket and passes these messages to game controllers. New clients are added to the client manager.
 */
public class Server extends WebSocketServer {
    /**
     * The instance of the client manager.
     */
    private ClientManager clientManager;

    /**
     * The instance of the game controller manager.
     */
    private GameControllerManager gameControllerManager;

    /**
     * The matchConfig of the Server
     */
    private final MatchConfig matchConfig;

    /**
     * Constructs a server object using the given address
     *
     * @param address The address (and port) for the server
     * @param matchConfig The matchconfig to be used for the server
     */
    public Server(InetSocketAddress address, MatchConfig matchConfig) {
        super(address);
        this.matchConfig = matchConfig;
    }

    /**
     * Constructs a server object
     *
     * @param matchConfig The matchconfig to be used for the server
     */

    public Server(MatchConfig matchConfig){
        super();
        this.matchConfig = matchConfig;
    }

    /**
     * Passes the String by calling onIncomingMessage on the corresponding game controller.
     *
     * @param client  Client who sent the message
     * @param message Message that was sent
     */
    private void passMessageToGameController(Client client, String message) {
        gameControllerManager.getGameController(client).onIncomingMessage(client, message);
    }

    /**
     * Handles the first message sent by a Client
     * <p>
     * If the client was disconnected, the WebSocket connection is updated and the client is
     * reconnected. If the client is a new client, a new client is created
     *
     * @param webSocket WebSocket connection through which the message was received
     * @param message   First message that was received
     */
    private void handleFirstMessage(WebSocket webSocket, String message) {
        Logger.debug("Handling first message ...");

        ContainerJson container;
        try {
            container = JsonUtility.getContainer(message);
        } catch (Exception e) {
            Logger.warn("Message was invalid JSON! Closing connection.");
            webSocket.close();
            return;
        }

        if (container.payloadType.equals(Strings.JOIN_REQUEST)) {
            JoinRequestJson joinRequest = JsonUtility.getPayload(container, JoinRequestJson.class);
            // check for mods
            if (joinRequest.mods.length > 0) {
                Logger.warn("Client has unsupported mods. Closing connection.");
                webSocket.close();
                return;
            }

            //check if the lobby name is valid
            if (!isValidLobby(joinRequest.lobby)) {
                Logger.warn("Client sent invalid lobby name: \"" + joinRequest.lobby + "\". Closing connection.");
                webSocket.close();
                return;
            }

            //check if the username is valid
            if (!isValidUsername(joinRequest.userName)) {
                Logger.warn("Client sent invalid username: \"" + joinRequest.userName + "\". Closing connection.");
                webSocket.close();
                return;
            }

            //check if the username is/was connected to the server
            Client client = clientManager.getClient(joinRequest.userName);

            if (client == null) {
                //There is/was no client with this username
                createClient(webSocket, joinRequest);
            } else {
                if (client.connected) {
                    //There is a connected client with the same username
                    if (client.password.equals(joinRequest.password)) {
                        //the password matches
                        //close the old connection and update the client information
                        client.webSocket.close();
                        client.webSocket = webSocket;
                        Logger.debug("Client information was updated for client " + client.name);
                    } else {
                        //the password does not match, close the connection
                        Logger.debug("Client sent an invalid password for a connected client! Closing connection.");
                        webSocket.close();
                    }
                } else {
                    //There is a disconnected client with the same username
                    if (client.password.equals(joinRequest.password)) {
                        //the password matches, update the client information
                        client.connected = true;
                        client.webSocket = webSocket;
                        Logger.debug("Client reconnected: " + client.toString());
                    } else {
                        //the password does not match, close the connection
                        Logger.debug("Client sent an invalid password for a disconnected client! Closing connection.");
                        webSocket.close();
                    }
                }
            }
        } else {
            //The message was not a join request, so it is ignored
            Logger.debug("Client sent an invalid message that was not a join request. Ignoring message.");
        }
    }

    /**
     * Checks if a username contains 3-20 alpha-numerical chars ([a-ZA-Z0-9])
     *
     * @param username username to be tested
     * @return true if the username is valid, false if not
     */
    private boolean isValidUsername(String username) {
        return username.matches("[a-zA-Z0-9]{3,20}");
    }

    /**
     * Checks if a lobby name contains 3-20 alpha-numerical chars ([a-ZA-Z0-9])
     *
     * @param lobby lobby name to be tested
     * @return true if the lobby name is valid, false if not
     */
    private boolean isValidLobby(String lobby) {
        return lobby.matches("[a-zA-Z0-9]{3,40}");
    }

    /**
     * Creates a client using the information in the join request.
     * <p>
     * Also puts the client in the right game (either an existing one or a new game)
     *
     * @param webSocket   WebSocket connection of the new client
     * @param joinRequest Join request that was sent by the client
     */
    private void createClient(WebSocket webSocket, JoinRequestJson joinRequest) {
        Client client = new Client(webSocket);

        //set client info
        client.name = joinRequest.userName;
        client.password = joinRequest.password;
        client.isAi = joinRequest.isArtificialIntelligence;
        client.isSpectator = true;
        client.state = Client.IN_LOBBY;

        clientManager.addClient(client);

        Logger.debug("Created client " + client.name);

        GameController gameController = gameControllerManager.getGameController(joinRequest.lobby);

        if (gameController != null) {
            //The game controller already exists
            gameControllerManager.addToGameController(client, gameController);
        } else {
            //create a new game controller
            createNewGameController(joinRequest.lobby, client);
        }
    }

    /**
     * Creates a new game controller and adds the first client
     *
     * @param name        Name of the game
     * @param firstClient First client to be added to the game
     */
    private void createNewGameController(String name, Client firstClient) {
        GameController gameController = new GameController(name, matchConfig);
        gameControllerManager.addGameController(gameController);
        gameControllerManager.addToGameController(firstClient, gameController);
        Logger.debug("Created new game controller " + gameController.getName() + " for client " + firstClient.name);
    }

    /**
     * onOpen is called after the Websocket handshake of a new client.
     * <p>
     * Adds the client to the client manager.
     * This method will be called by org.java_websocket
     *
     * @param webSocket       The Websocket instance of the client
     * @param clientHandshake The Handshake of the Websocket instance (not needed)
     */
    @Override
    public void onOpen(WebSocket webSocket, ClientHandshake clientHandshake) {
        Logger.debug("New connection from " + webSocket.getRemoteSocketAddress());
    }

    /**
     * onClose is called after the Websocket connection of a client has been closed.
     * <p>
     * Removes the client if the game is over or the client is not in a game yet.
     * Tells the client manager that the client is disconnected if it can not be removed.
     * This method will be called by org.java_websocket
     *
     * @param webSocket The Websocket instance of the client
     * @param code      (not needed)
     * @param reason    (not needed)
     * @param remote    (not needed)
     */
    @Override
    public void onClose(WebSocket webSocket, int code, String reason, boolean remote) {
        Client client = clientManager.getClient(webSocket);

        if (client == null) {
            //don't need to worry about the connection if there was no client associated with it
            return;
        }

        Logger.warn("Connection closed for client " + client.name + ". Reason: " + reason + ". Code: " + code);

        client.connected = false;
        Logger.info("Marked client " + client.name + " as disconnected.");
    }

    /**
     * onMessage is called after a Weboscket message is received from a client.
     * <p>
     * Passes the message to the corresponding game controller or calls handleFirstMessage if the
     * client is not in a game yet.
     * This method will be called by org.java_websocket
     *
     * @param webSocket The Websocket of the client who sent the message
     * @param message   The message that was sent via the Websocket
     */
    @Override
    public void onMessage(WebSocket webSocket, String message) {

        Client sendingClient = clientManager.getClient(webSocket);

        Logger.trace("Received Message from client " + (sendingClient==null?"null":sendingClient.name) + ": " + message);

        if (sendingClient == null) {
            //Client is not in a game/lobby yet. This must be the first message
            handleFirstMessage(webSocket, message);
        } else {
            //Client is already in a game/lobby
            passMessageToGameController(sendingClient, message);
        }
    }

    /**
     * onError is called after an error occurs.
     * <p>
     * This method will be called by org.java_websocket
     *
     * @param webSocket The Websocket where the error occurred
     * @param e         The exception causing the error
     */
    @Override
    public void onError(WebSocket webSocket, Exception e) {
        e.printStackTrace();
        Logger.error("Error in the server:\n" + e.getStackTrace());
        System.exit(1);
    }

    /**
     * onStart is called when the server is started.
     * <p>
     * This method will be called by org.java_websocket
     */
    @Override
    public void onStart() {
        clientManager = ClientManager.getInstance();
        gameControllerManager = GameControllerManager.getInstance();
        Logger.info("Server started on address " + this.getAddress());
    }

    @Override
    public void stop() throws IOException, InterruptedException {
        super.stop();

    }

    /**
     * Returns server information as a string
     *
     * @return string of full server information
     */
    @Override
    public String toString() {
        return "Server{\n" +
                "\tclientManager=" + clientManager +
                ",\n\tgameControllerManager=" + gameControllerManager +
                ",\n\tmatchConfig=" + matchConfig +
                "\n}";
    }
}
