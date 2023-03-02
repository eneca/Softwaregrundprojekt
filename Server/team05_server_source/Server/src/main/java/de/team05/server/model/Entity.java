package de.team05.server.model;

/**
 * Represents an entity in Quidditch. Either a ball, player or a fan.
 */
public class Entity {

    /**
     * The square that this entity is positioned on. Always null for fans.
     */
    private Square pos;

    /**
     * Boolean to check whether an entity already had a turn this round.
     */
    public boolean hadTurn;

    /**
     * The String id off this entity. According to the activeEntity in the specifications.
     */
    public String id;

    /**
     * Set the position of this entity to the given square.
     *
     * @param pos To set this entity to.
     */
    public void setPos(Square pos) {
        this.pos = new Square(pos.x, pos.y);
    }

    /**
     * Utility method to set the position of this entity to the position of the given entity.
     *
     * @param entity Off which this entity gets its position set to.
     */
    public void setPos(Entity entity) {
        this.pos.x = entity.pos.x;
        this.pos.y = entity.pos.y;
    }

    /**
     * Get the position of this entity.
     *
     * @return this entity's position.
     */
    public Square getPos() {
        return this.pos;
    }
}
