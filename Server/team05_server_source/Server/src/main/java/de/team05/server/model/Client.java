package de.team05.server.model;

import org.java_websocket.WebSocket;

/**
 * Represents a client in a lobby. Is created when a new connection reaches the server.
 */
public class Client {

    public static final int IN_LOBBY = 2; //in a lobby as a spectator
    public static final int IN_LOBBY_AS_PLAYER = 3; //in a lobby as a playing client
    public static final int SPECTATING = 4; //in game as a spectator (all following states are
    // for playing clients only!)
    public static final int WAITING_FOR_FORMATION = 5; //game started, but no team formation was
    // sent yet
    public static final int FORMATION_RECEIVED = 6; //game started and a team formation was sent
    public static final int IN_GAME = 128; //client is active and in game
    public static final int GAME_OVER = 69; //the game is over

    /**
     * Boolean to check whether a client is an ai.
     */
    public boolean isAi;

    /**
     * Boolean to check whether a client is a spectator.
     */
    public boolean isSpectator;

    /**
     * Boolean to check whether a client is connected to the server.
     */
    public boolean connected;

    /**
     * The team off this client.
     */
    public Team team;

    /**
     * The username that was sent when this client connected to the server.
     */
    public String name;

    /**
     * The password to identify the client with its username.
     */
    public String password;

    /**
     * This client's websocket.
     */
    public WebSocket webSocket;

    /**
     * This clients state. Can be one of the following:
     * <br>{@link Client#IN_LOBBY}
     * <br>{@link Client#IN_LOBBY_AS_PLAYER}
     * <br>{@link Client#SPECTATING}
     * <br>{@link Client#WAITING_FOR_FORMATION}
     * <br>{@link Client#FORMATION_RECEIVED}
     * <br>{@link Client#IN_GAME}
     * <br>{@link Client#GAME_OVER}
     */
    public int state;

    /**
     * Boolean to check if this client should get kicked at the end of a round. (After protocol violation)
     */
    public boolean kicked;

    /**
     * Boolean to check whether a client is disqualified (3 or more banned players).
     */
    public boolean disqualified;

    /**
     * Creates a new Client using a WebSocket but no other additional information.
     * <p>
     * Sets the client state to PRE_GAME and connected to true. Does not create a team and does
     * not set any other attributes.
     *
     * @param webSocket WebSocket through which the client sends and receives messages
     */
    public Client(WebSocket webSocket) {
        this.webSocket = webSocket;
        this.state = IN_LOBBY;
        this.connected = true;
    }

    @Override
    public boolean equals(Object client) {
        //client is either the same object or the name and password are equal
        return client instanceof Client && (client == this ||
                (this.name.equals(((Client) client).name) &&
                        this.password.equals(((Client) client).password)));
    }

    @Override
    public String toString() {
        return "Client{" +
                "\nisAi=" + isAi +
                ",\nisSpectator=" + isSpectator +
                ",\nconnected=" + connected +
                ", \nteam=" + team +
                ", \nname='" + name + '\'' +
                ", \npassword='" + password + '\'' +
                ", \nwebSocket=" + webSocket +
                ", \nstate=" + state +
                ", \nkicked=" + kicked +
                ", \ndisqualified=" + disqualified +
                '}';
    }
}