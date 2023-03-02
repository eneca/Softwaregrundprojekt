package de.team05.server.control;

import de.team05.server.model.Client;

import java.util.HashMap;

/**
 * The GameControllerManager is a singleton class which stores information about all
 * GameControllers on the Server.
 * <p>
 * Stores all GameControllers and can retrieve them by their name or one of their Clients.
 */
public class GameControllerManager {
    private static final GameControllerManager INSTANCE = new GameControllerManager();

    private final HashMap<String, GameController> gameControllersByName;
    private final HashMap<Client, GameController> gameControllersByClient;

    /**
     * Constructs a GameControllerManager. This method is private because this class is used as a
     * singleton.
     */
    private GameControllerManager() {
        gameControllersByName = new HashMap<>();
        gameControllersByClient = new HashMap<>();
    }

    /**
     * Adds a GameController to the GameControllerManager
     *
     * @param gameController gameController, which will be added
     */
    public void addGameController(GameController gameController) {
        gameControllersByName.put(gameController.getName(), gameController);
    }

    /**
     * Adds client as spectator to the specified gameController
     *
     * @param client         client, that will be added
     * @param gameController specified gameController
     */
    public void addToGameController(Client client, GameController gameController) {
        gameControllersByClient.put(client, gameController);
        gameController.addSpectator(client);
    }

    /**
     * Empties the GameControllerManager
     */
    public void clear(){
        gameControllersByClient.clear();
        gameControllersByName.clear();
    }

    /**
     * Gets the corresponding GameController to the given Client.
     *
     * @param client Client that is part of the returned GameController
     * @return The GameController that contains the given Client
     */
    public GameController getGameController(Client client) {
        return gameControllersByClient.get(client);
    }

    /**
     * Gets the GameController with the given lobby name
     *
     * @param name The name of the lobby
     * @return The GameController with the given lobby name
     */
    public GameController getGameController(String name) {
        return gameControllersByName.get(name);
    }

    /**
     * Gets the Singleton instance of the GameControllerManager
     *
     * @return The Singleton instance of the GameControllerManager
     */
    public static GameControllerManager getInstance() {
        return INSTANCE;
    }

    /**
     * Returns GameControllerManager information as a string
     *
     * @return string of full GameControllerManager information
     */
    @Override
    public String toString() {
        return "GameControllerManager{\n" +
                "\tgameControllersByName=" + gameControllersByName +
                "\n}";
    }
}
