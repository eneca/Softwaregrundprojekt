package de.team05.server.control;

import de.team05.server.model.Game;

/**
 * The DebugMessageHandler reacts to incoming debug messages. It is not currently in use.
 */
class DebugMessageHandler {
    private final Game game;

    /**
     * Constructs a DebugMessageHandler using the given Game
     *
     * @param game Game that the DebugMessageHandler uses
     */
    public DebugMessageHandler(Game game) {
        this.game = game;
    }
    //TODO: feeling unnecessary, might delete later ;)
}
