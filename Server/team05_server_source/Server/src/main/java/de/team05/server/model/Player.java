package de.team05.server.model;

/**
 * Represents a player in Quidditch.
 */
public class Player extends Entity{

    /**
     * String with the corresponding broom name from the specifications.
     */
    public final String broom;

    /**
     * Used to check whether a player already moved once this turn.
     */
    public boolean hadFirstMove;

    /**
     * Used to check whether a player is banned.
     */
    private boolean banned;

    /**
     * Used to check whether a player is knocked out.
     */
    public boolean knockedOut;

    /**
     * Used to check whether a player moved twice in a round and therefore cannot move anymore.
     */
    public boolean hadSecondMove;

    /**
     * Creates a player with the given String id and String for the broom. Sets all booleans to false.
     *
     * @param id this players String id corresponding to the activeEntityId s in the specifications
     * @param broom this players broom as a String corresponding to the five different brooms in the specifications
     */
    Player(String id, String broom) {
        super();
        this.id = id;
        this.broom = broom;
        this.hadTurn = false;
        this.banned = false;
        this.knockedOut = false;
        this.hadSecondMove = false;
        this.hadFirstMove = false;
    }

    /**
     * Used to set this players ban boolean to true and null his position (as he is not on the pitch anymore)
     */
    public void ban(){
        banned = true;
        setPos(new Square(null, null));
    }

    /**
     * Unbans this player and sets his position to newPos.
     *
     * @param newPos New position of the player after the unban.
     */
    public void unban(Square newPos) {
        banned = false;
        setPos(newPos);
    }

    /**
     * Retrieves this player's ban status.
     *
     * @return true if the player is banned. False otherwise.
     */
    public boolean isBanned(){
        return banned;
    }

    @Override
    public String toString() {
        return "Player{" +
                "broom='" + broom + '\'' +
                ", hadFirstMove=" + hadFirstMove +
                ", banned=" + banned +
                ", knockedOut=" + knockedOut +
                ", hadSecondMove=" + hadSecondMove +
                ", hadTurn=" + hadTurn +
                ", id='" + id + '\'' +
                '}';
    }
}
