package de.team05.server.control;

import com.google.gson.JsonSyntaxException;
import de.team05.server.Config;
import de.team05.server.Strings;
import de.team05.server.json.*;
import de.team05.server.model.Client;
import de.team05.server.model.Game;
import de.team05.server.model.MatchConfig;
import org.pmw.tinylog.Logger;

/**
 * The game controller handles all incoming and outgoing messages for one game.
 * <p>
 * Incoming messages are forwared to either a PreGameMessageHandler, InGameMessageHandler, ReplayMessageHandler or DebugMessageHandler.
 * The game controller als checks if the payload that was sent by a client is valid according to the current state of the client and discards invalid messages.
 * Outgoing messages are either sent to the corresponding WebSocket connection of one client or to all clients in the game.
 */
public class GameController {

    /**
     * The game that this game controller handles
     */
    private final Game game;

    /**
     * The PreGameMessageHandler to which teamFormations and teamConfig messages are forwarded
     */
    private final PreGameMessageHandler preGameMessageHandler;

    /**
     * The InGameMessageHandler to which deltaRequest, pauseRequest and continueRequest messages are forwarded
     */
    private final InGameMessageHandler inGameMessageHandler;

    /**
     * The ReplayMessageHandler to which getReplay messages are forwarded
     */
    private final ReplayMessageHandler replayMessageHandler;

    /**
     * The DebugMessageHandler to which sendDebug messages are forwarded
     */
    private final DebugMessageHandler debugMessageHandler;

    /**
     * Creates a new game controller using the specified game name and matchconfig
     *
     * @param name        Name for the game that this game controller handles
     * @param matchConfig matchconfig for the game that this game controller handles
     */
    public GameController(String name, MatchConfig matchConfig) {
        this.game = new Game(name, matchConfig);
        preGameMessageHandler = new PreGameMessageHandler(game);
        inGameMessageHandler = new InGameMessageHandler(game);
        replayMessageHandler = new ReplayMessageHandler(game);
        debugMessageHandler = new DebugMessageHandler(game);
    }

    /**
     * Adds a spectator to the game and sends a login greeting and join response
     *
     * @param client spectator to be added to the game
     */
    public void addSpectator(Client client) {
        client.state = Client.IN_LOBBY;
        game.spectators.add(client);

        //send joinResponse
        JoinResponseJson joinResponse = new JoinResponseJson(Strings.JOIN_RESPONSE_MESSAGE);
        ProtocolUtility.sendPayload(client, joinResponse);

        //Send loginGreeting
        LoginGreetingJson loginGreeting = new LoginGreetingJson();
        loginGreeting.userName = client.name;
        ProtocolUtility.broadcastPayload(game, loginGreeting);

        Logger.debug("Added spectator " + client.name + " to game controller " + this.getName());

        if (game.matchStarted) {
            sendReconnect(client);
        }
    }

    /**
     * Constructs and sends a reconnect json to a given client.
     *
     * @param client Client to send the reconnect to
     */
    private void sendReconnect(Client client) {
        ReconnectJson reconnect = new ReconnectJson(
                game.matchStartJsonContainer,
                game.lastSnapshotContainer,
                game.lastNextContainer
        );
        Logger.debug("Sending reconnect to client " + client.name + ": " + reconnect);
        ProtocolUtility.sendPayload(client, reconnect);
    }

    /**
     * Handles incoming messages by checking if they are valid for the client to send and passing
     * the payload to the corresponding message handler
     * <p>
     * Uses the client states to check if the message is valid according to the Fantastic Feasts
     * protocol.
     *
     * @param client  Client that sent the message
     * @param message Message that was sent by the client
     */
    public void onIncomingMessage(Client client, String message) {
        ContainerJson container;

        //parse JSON
        try {
            container = JsonUtility.getContainer(message);
        } catch (JsonSyntaxException e) {
            Logger.warn("Invalid JSON. Kicking client");
            Logger.trace(e.toString());
            ProtocolUtility.kickClient(client, "Invalid JSON! For more information look at the " +
                    "server trace log");
            return;
        }
        Logger.debug("Payload type of received message: " + container.payloadType);

        //Check if the payload type is valid for the client to send and call the corresponding
        // methods in the message handler classes
        switch (container.payloadType) {
            case Strings.TEAM_CONFIG:
                if (isTeamConfigValidPayloadType(client)) {
                    TeamConfigJson teamConfigJson = JsonUtility.getPayload(container,
                            TeamConfigJson.class);
                    preGameMessageHandler.onTeamConfigMessageReceived(client, teamConfigJson);
                }
                break;
            case Strings.TEAM_FORMATION:
                if (isTeamFormationValidPayloadType(client)) {
                    TeamFormationJson teamFormationJson = JsonUtility.getPayload(container,
                            TeamFormationJson.class);
                    int res = preGameMessageHandler.onTeamFormationMessageReceived(client, teamFormationJson);
                    if (res == Config.VALIDATION_BOTH_SUCCESS) {
                        inGameMessageHandler.onGameStart();
                    }
                }
                break;
            case Strings.PAUSE_REQUEST:
                if (isPauseRequestValidPayloadType(client)) {
                    PauseRequestJson pauseRequestJson = JsonUtility.getPayload(container,
                            PauseRequestJson.class);
                    inGameMessageHandler.onPauseRequestMessageReceived(client, pauseRequestJson);
                }
                break;
            case Strings.CONTINUE_REQUEST:
                if (isContinueRequestValidPayloadType()) {
                    ContinueRequestJson continueRequestJson = JsonUtility.getPayload(container,
                            ContinueRequestJson.class);
                    inGameMessageHandler.onContinueRequestMessageReceived(client,
                            continueRequestJson);
                }
                break;
            case Strings.DELTA_REQUEST:
                if (isDeltaRequestValidPayloadType(client)) {
                    DeltaRequestJson deltaRequestJson = JsonUtility.getPayload(container,
                            DeltaRequestJson.class);
                    inGameMessageHandler.onDeltaRequestMessageReceived(client, deltaRequestJson);
                }
                break;
            case Strings.GET_REPLAY:
                if (isGetReplayValidPayloadType(client)) {
                    replayMessageHandler.onGetReplayReceived(client);
                }
                break;
            default:
                //invalid json
                Logger.warn("Client sent an invalid Json. Payload was NOT: teamConfig, " +
                        "teamFormation, pauseRequest, continueRequest, deltaRequest, getReplay");
        }
    }

    /**
     * Checks if the getReplay payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @param client Client that sent the getReplay message
     * @return true if the getReplay payload is a valid message for the current client state, false if not
     */
    private boolean isGetReplayValidPayloadType(Client client) {
        //client has to be in a game or game over
        return client.state != Client.IN_LOBBY &&
                client.state != Client.IN_LOBBY_AS_PLAYER &&
                client.state != Client.WAITING_FOR_FORMATION &&
                client.state != Client.FORMATION_RECEIVED;
    }

    /**
     * Checks if the deltaRequest payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @param client Client that sent the deltaRequest message
     * @return true if the deltaRequest payload is a valid message for the current client state, false if not
     */
    private boolean isDeltaRequestValidPayloadType(Client client) {
        //client has to be a playing client in a game (player phase or fan phase)
        return client.state == Client.IN_GAME;
    }

    /**
     * Checks if the continueRequest payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @return true if the continueRequest payload is a valid message for the current client state, false if not
     */
    private boolean isContinueRequestValidPayloadType() {
        //client has to be a playing client in a game and the game has to be paused
        return game.paused;
    }

    /**
     * Checks if the pauseRequest payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @param client Client that sent the pauseRequest message
     * @return true if the pauseRequest payload is a valid message for the current client state, false if not
     */
    private boolean isPauseRequestValidPayloadType(Client client) {
        //client has to be a playing client in a game and the game can't be paused
        return client.state == Client.IN_GAME;
    }

    /**
     * Checks if the teamFormation payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @param client Client that sent the teamFormation message
     * @return true if the teamFormation payload is a valid message for the current client state, false if not
     */
    private boolean isTeamFormationValidPayloadType(Client client) {
        //client has to be in the waiting for team formation state
        return client.state == Client.WAITING_FOR_FORMATION;
    }

    /**
     * Checks if the teamConfig payload is a valid message sent by a client using its current state. Does NOT check if the payload is valid, only if it is a valid message at the current time.
     *
     * @param client Client that sent the teamConfig message
     * @return true if the teamConfig payload is a valid message for the current client state, false if not
     */
    private boolean isTeamConfigValidPayloadType(Client client) {
        //client has to be in lobby and there has to be a free spot as a playing client
        return client.state == Client.IN_LOBBY && !game.bothPlayersConnected();
    }

    public String getName() {
        return game.name;
    }

    /**
     * Returns game controller information as a string
     *
     * @return string of full game controller information
     */
    @Override
    public String toString() {
        return "GameController{\n" +
                "game=" + game +
                ",\npreGameMessageHandler=" + preGameMessageHandler +
                ",\ninGameMessageHandler=" + inGameMessageHandler +
                ",\nreplayMessageHandler=" + replayMessageHandler +
                ",\ndebugMessageHandler=" + debugMessageHandler +
                "\n}";
    }
}
