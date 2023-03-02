package de.team05.server.model;

/**
 * Represents a seeker in Quidditch.
 */
public class Seeker extends Player {

    /**
     * Creates a Seeker using the given String id and String for a specific broom
     *
     * @param id corresponding to the activeEntityId in the standard document. Either leftSeeker or rightSeeker
     * @param broom corresponding to the five different broom names in the specifications
     */
    public Seeker(String id, String broom) {
        super(id, broom);
    }
}
