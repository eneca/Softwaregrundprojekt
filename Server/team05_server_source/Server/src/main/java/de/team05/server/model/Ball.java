package de.team05.server.model;

/**
 * Represents the balls in Quidditch. Different balls are not several subclasses. Balls are differentiated by type.
 */
public class Ball extends Entity {

    /**
     * Used to define Ball as a snitch.
     */
    public static final int SNITCH = 1;

    /**
     * Used to define Ball as a quaffle.
     */
    public static final int QUAFFLE = 2;

    /**
     * Used to define Ball as a bludger.
     */
    public static final int BLUDGER = 3;

    /**
     * Used to define Ball object type.
     */
    public final int type;

    /**
     * Creates a ball using the given type and String id.
     *
     * @param type of the ball. Either snitch, quaffle or bludger
     * @param id of the ball. Either snitch, quaffle, bludger1 or bludger2
     */
    public Ball(int type, String id) {
        this.type = type;
        this.id = id;
    }
}
