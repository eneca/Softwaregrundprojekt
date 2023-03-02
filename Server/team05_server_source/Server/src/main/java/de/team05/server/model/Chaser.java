package de.team05.server.model;

/**
 * Represents a chaser in Quidditch.
 */
public class Chaser extends Player {

    /**
     * Creates a Chaser using the given String id and String for a specific broom
     *
     * @param id corresponding to the activeEntityId in the standard document. Either leftChaser[1-3] or rightBeater[1-3].
     * @param broom corresponding to the five different broom names in the specifications
     */
    public Chaser(String id, String broom) {
        super(id, broom);
    }
}
