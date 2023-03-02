package de.team05.server.model;

/**
 * Represents beaters in Quidditch.
 */
public class Beater extends Player {

    /**
     * Creates a Beater using the given String id and String for a specific broom
     *
     * @param id corresponding to the activeEntityId in the standard document. Either leftBeater or rightBeater
     * @param broom corresponding to the five different broom names in the specifications
     */
    public Beater(String id, String broom) {
        super(id, broom);
    }
}
