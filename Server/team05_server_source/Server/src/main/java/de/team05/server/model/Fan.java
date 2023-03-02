package de.team05.server.model;

/**
 * Represents a fan in Quidditch.
 */
public class Fan extends Entity{

    /**
     * The type of this fan. Can be troll, elf, goblin or niffler.
     */
    public final String type;

    /**
     * Boolean to check whether this entity is banned.
     */
    private boolean banned;

    /**
     * Creates a fan with the given String type and String id. Sets all boolean fields to false.
     *
     * @param type The type of this fan. Can be troll, elf, goblin or niffler.
     * @param id The String id of this fan. Can be leftNiffler, rightNiffler, leftTroll, rightTroll, leftElf, rightElf,
     *           leftGoblin or rightGoblin.
     */
    public Fan(String type, String id) {
        this.type = type;
        this.hadTurn = false;
        this.banned = false;
        this.id = id;
    }

    /**
     * Ban this fan.
     */
    public void ban() {
        banned = true;
    }

    /**
     * Check if this fan is banned.
     *
     * @return True if this fan is banned. False otherwise.
     */
    public boolean isBanned() {
        return banned;
    }

    @Override
    public String toString() {
        return "Fan{" +
                "type='" + type + '\'' +
                ", banned=" + banned +
                ", hadTurn=" + hadTurn +
                ", id='" + id + '\'' +
                '}';
    }
}
