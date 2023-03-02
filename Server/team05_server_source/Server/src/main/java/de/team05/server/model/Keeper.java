package de.team05.server.model;

/**
 * Represents a keeper in Quidditch.
 */
public class Keeper extends Player {

    /**
     * Creates a Keeper using the given String id and String for a specific broom
     *
     * @param id corresponding to the activeEntityId in the standard document. Either leftKeeper or rightKeeper.
     * @param broom corresponding to the five different broom names in the specifications
     */
    public Keeper(String id, String broom) {
        super(id, broom);
    }
}
