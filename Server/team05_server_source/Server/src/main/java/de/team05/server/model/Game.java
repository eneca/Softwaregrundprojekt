package de.team05.server.model;

import de.team05.server.Config;
import de.team05.server.Strings;
import de.team05.server.json.ContainerJson;
import de.team05.server.json.LogJson;
import de.team05.server.json.SnapshotJson;
import de.team05.server.json.WombatCubesJson;
import org.apache.commons.lang3.ArrayUtils;
import org.pmw.tinylog.Logger;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;

/**
 * Represents a match of Quidditch.
 */
public class Game {

    public static final int BALL_PHASE = 1;
    public static final int PLAYER_PHASE = 2;
    public static final int FAN_PHASE = 3;

    public static final int NO_OVERTIME = 0; //not in overtime yet
    public static final int OVERTIME_PHASE_1 = 1; //first 3 rounds of overtime (100 % catch chance, no snitchsnatch)
    public static final int OVERTIME_PHASE_2 = 2; //after 3 rounds of overtime (snatch moves towards middle square)
    public static final int OVERTIME_PHASE_3 = 3; //first 3 rounds after snitch reached middle square
    public static final int OVERTIME_PHASE_4 = 50; //snitch was 3 rounds on middle square (moves to next seeker instantly)

    public ArrayList<Square> wombatPoos;
    public ArrayList<Player> actionPlayers;

    /**
     * Name of the lobby this game is referring to.
     */
    public final String name;

    /**
     * Counter to keep track of the rounds.
     */
    public int roundCounter;

    /**
     * Used to keep track of the different phases. The phases can be 1 = ballPhase, 2 = playerPhase and 3 = fanPhase.
     */
    public int state;

    /**
     * Used to keep track if this game is paused.
     */
    public boolean paused;

    /**
     * Used to keep track of the different overtime phases.
     */
    public int overtime;

    /**
     * Keeps track of the starting round for the last overtime phase, were the snitch flies straight to a seeker.
     */
    public int overtimePhaseFourStart;

    /**
     * The matchStart that was sent wrapped in a container
     */
    public ContainerJson matchStartJsonContainer;

    /**
     * List that is used to send out player removeBan next broadcasts after a goal happened in a round.
     */
    private LinkedList<Player> unbanList;

    /**
     * The left client of this game.
     */
    private Client leftClient;

    /**
     * The right client of this game.
     */
    private Client rightClient;

    /**
     * List that is used to keep track of all spectators for this game.
     */
    public final List<Client> spectators;

    /**
     * Hashmap to easily retrieve a Ball object of this game.
     */
    private final HashMap<String, Ball> balls;

    /**
     * Used to differentiate betweeen inGame, preGame and afterGame state.
     */
    public boolean inGame;

    /**
     * The matchConfig for this game.
     */
    public final MatchConfig matchConfig;

    /**
     * Used to keep track if a goal happened this round. Makes determining unbans for a round easy.
     */
    private boolean goalThisRound;

    /**
     * Used to keep track of the activeEntity that should have the next move/action.
     */
    public Entity activeEntity;

    /**
     * Used to keep track of the next action/move that should happen. Can either be move, action, fan or removaBan.
     */
    public String activeTurnType; //move, action, fan, removeBan

    /**
     * List that keeps track of the turn order of all players and fans that can still act in this round.
     */
    private LinkedList<Entity> turnList;

    /**
     * Used to figure out who won if both teams are disqualified at the end of a round, have the same amount of points
     * and no team caught the snitch.
     */
    private Player lastBanned;

    /**
     * List of logs used for the replay. Contains every deltaBroadcast that was sent and one
     * matchFinish.
     */
    public final List<LogJson> replay;

    /**
     * The first snapshot that was sent.
     */
    public SnapshotJson firstSnapshot;

    /**
     * The timestamp of the first snapshot.
     */
    public String initialTimestamp;

    /**
     * The status of the game regarding the matchStart. True if the matchStart was sent, false if
     * not.
     */
    public boolean matchStarted;

    /**
     * The last snapshot that was sent wrapped in it's container.
     */
    public ContainerJson lastSnapshotContainer;

    /**
     * The last next that was sent wrapped in it's container or null if there was a snapshot
     * after the last next.
     */
    public ContainerJson lastNextContainer;

    /**
     * Constructs a Game object using the given lobby name and matchConfig. Creates the Balls.
     *
     * @param name        Name of the lobby / name for this game
     * @param matchConfig MatchConfig to be used for this game
     */
    public Game(String name, MatchConfig matchConfig) {
        this.name = name;
        this.roundCounter = 0;
        this.state = BALL_PHASE;
        this.matchConfig = matchConfig;
        balls = new HashMap<>();

        balls.put(Strings.QUAFFLE, new Ball(Ball.QUAFFLE, Strings.QUAFFLE));
        balls.put(Strings.BLUDGER1, new Ball(Ball.BLUDGER, Strings.BLUDGER1));
        balls.put(Strings.BLUDGER2, new Ball(Ball.BLUDGER, Strings.BLUDGER2));
        balls.put(Strings.SNITCH, new Ball(Ball.SNITCH, Strings.SNITCH));
        inGame = false;
        matchStarted = false;
        overtime = NO_OVERTIME;
        overtimePhaseFourStart = -1;
        spectators = new ArrayList<>();
        turnList = new LinkedList<>();
        unbanList = new LinkedList<>();
        replay = new ArrayList<>();
        wombatPoos = new ArrayList<>();
        actionPlayers= new ArrayList<>();
    }


    /**
     * Initializes the balls to their corresponding Squares.
     * <p>
     * Quaffle: Middle Square
     * Bludgers: Middle Square
     * Snitch: null Square
     */
    private void initBalls() {
        getBall(Strings.QUAFFLE).setPos(Config.MIDDLE_SQUARE);
        getBall(Strings.BLUDGER1).setPos(Config.MIDDLE_SQUARE);
        getBall(Strings.BLUDGER2).setPos(Config.MIDDLE_SQUARE);
        getBall(Strings.SNITCH).setPos(new Square(null, null));
    }

    /**
     * Polls the turnList to retrieve the next player or fan that should act.
     *
     * @return The next player or fan that should act.
     */
    public Entity getNextEntityAtTurn() {
        return turnList.pollFirst();
    }

    /**
     * Sets the turnList to the given list.
     *
     * @param turnList to set the turnList.
     */
    public void setTurnList(List<Entity> turnList) {
        this.turnList = new LinkedList<>(turnList);
    }

    /**
     * Sets the unbanList to the given list.
     *
     * @param unbanList to set the unbanList.
     */
    public void setUnbanList(List<Player> unbanList) {
        this.unbanList = new LinkedList<>(unbanList);
    }

    /**
     * Polls the unbanList to retrieve the next player that gets unbanned.
     *
     * @return The next player that gets unbanned.
     */
    public Player getNextUnban() {
        return unbanList.pollFirst();
    }

    /**
     * Gets the corresponding ball for the given ballType.
     *
     * @param ballType String defining the ballType.
     * @return The corresponding ball for the given ballType.
     */
    public Ball getBall(String ballType){
        return balls.get(ballType);
    }

    /**
     * Helper function that retrieves a player from a given String that corresponds to the standard document's
     * activeEntityId.
     *
     * @param playerType Corresponding to the activeEntityId from the standard document.
     * @return The corresponding player from this game.
     */
    public Player getPlayer(String playerType) {
        Player player = rightClient.team.getPlayer(playerType);
        if (player == null) {
            player = leftClient.team.getPlayer(playerType);
        }
        return player;
    }

    /**
     * Gets all Ball objects off this game.
     *
     * @return All Ball objects off this game as Ball array.
     */
    public Ball[] getAllBalls(){
        Ball[] allBalls = new Ball[4];
        allBalls[0] = balls.get(Strings.QUAFFLE);
        allBalls[1] = balls.get(Strings.BLUDGER1);
        allBalls[2] = balls.get(Strings.BLUDGER2);
        allBalls[3] = balls.get(Strings.SNITCH);
        return allBalls;
    }

    /**
     * Gets all Player objects from this game's leftClient and rightClient together in one array.
     *
     * @return A Player array with all players off this game.
     */
    public Player[] getAllPlayers(){
        return ArrayUtils.addAll(this.leftClient.team.getPlayers(), this.rightClient.team.getPlayers());
    }

    /**
     * Gets all entities (Player objects and Fan objects) off this game.
     *
     * @return A Entity array with all of this game's entities.
     */
    public Entity[] getAllMovableEntities(){
        Entity[] allEntities = new Entity[18];
        for (int i = 0; i < this.getAllPlayers().length; i++) {
            allEntities[i] = this.getAllPlayers()[i];
        }
        for (int i = 0; i < this.getAllBalls().length; i++) {
            allEntities[i+14] = this.getAllBalls()[i];
        }
        return allEntities;
    }

    /**
     * This is a utility function for snapshot creating. Retrieves all spectator clients by username;
     *
     * @return A String array off this game's spectators.
     */
    public String[] getSpectatorsByUsername(){
        String[] usernames = new String[spectators.size()];
        for (int i = 0; i < spectators.size(); i++) {
            usernames[i] = spectators.get(i).name;
        }

        return usernames;
    }

    /**
     * Retrieves this game's phase as String. Used for creating snapshots.
     *
     * @return This game's phase as String.
     */
    public String getPhaseAsString() {
        switch (this.state){
            case 1:
                return Strings.BALL_PHASE;
            case 2:
                return Strings.PLAYER_PHASE;
            case 3:
                return Strings.FAN_PHASE;
            default:
                Logger.warn("this should not happen");
                return "this should never be sent";
        }
    }

    /**
     * Check if both clients are not null.
     *
     * @return True if both clients are not null.
     */
    public boolean bothPlayersConnected() {
        return leftClient != null && rightClient != null;
    }

    /**
     * Gets the client from the given entity.
     *
     * @param entity The entity that is part of the returned Client
     * @return The client of the given entity.
     */
    public Client getClientFromEntity(Entity entity) {
        for (Player p : leftClient.team.players) {
            if (p == entity) {
                return leftClient;
            }
        }
        for (Fan f : leftClient.team.fans) {
            if (f == entity) {
                return leftClient;
            }
        }

        for (Player p : rightClient.team.players) {
            if (p == entity) {
                return rightClient;
            }
        }
        for (Fan f : rightClient.team.fans) {
            if (f == entity) {
                return rightClient;
            }
        }

        return null;
    }

    @Override
    public String toString() {
        return Strings.ANSI_PURPLE + "Game{\n" +
                "name='" + name + '\'' +
                ",\nroundCounter=" + roundCounter +
                ",\nstate=" + state +
                Strings.ANSI_PURPLE +
                ",\nleftClient=" + leftClient +
                Strings.ANSI_PURPLE +
                ",\nrightClient=" + rightClient +
                Strings.ANSI_PURPLE +
                ",\nspectators=" + spectators +
                Strings.ANSI_PURPLE +
                ",\nballs=" + balls +
                ",\ninGame=" + inGame +
                ",\nmatchConfig=" + matchConfig +
                "\n}" + Strings.ANSI_RESET;
    }

    /**
     * Checks whether this game's leftClient has been set or not.
     *
     * @return True if the leftClient off this game is not null. False otherwise.
     */
    public boolean hasLeftClient() {
        return leftClient != null;
    }

    /**
     * Checks whether this game's rightClient has been set or not.
     *
     * @return True if the rightClient off this game is not null. False otherwise.
     */
    public boolean hasRightClient() {
        return rightClient != null;
    }

    /**
     * Getter for the leftClient off this game.
     *
     * @return the leftClient.
     */
    public Client getLeftClient() {
        return leftClient;
    }

    /**
     * Getter for the rightClient off this game.
     *
     * @return the rightClient.
     */
    public Client getRightClient() {
        return rightClient;
    }

    /**
     * Setter for the leftClient off this game.
     *
     * @param client that the leftClient off this game is set to.
     */
    public void setLeftClient(Client client) {
        this.leftClient = client;
        this.spectators.remove(client);
        client.isSpectator = false;
    }

    /**
     * Setter for the rightClient off this game.
     *
     * @param client that the rightClient off this game is set to.
     */
    public void setRightClient(Client client) {
        this.rightClient = client;
        this.spectators.remove(client);
        client.isSpectator = false;
    }

    /**
     * Set goalThisRound to true. Useful for figuring out unbans.
     */
    public void goalThisRound() {
        this.goalThisRound = true;
    }

    /**
     * Updates the roundCounter and resets the goalThisRound attribute. Also resets the hadTurn
     * variable for every player and fan in the game.
     */
    public void newRound() {
        this.goalThisRound = false;
        roundCounter++;

        for (Player p : getAllPlayers()) {
            p.hadTurn = false;
            p.hadFirstMove = false;
            p.hadSecondMove = false;
        }

        for (Fan f : getAllFans()) {
            f.hadTurn = false;
        }
    }

    private Fan[] getAllFans() {
        return ArrayUtils.addAll(this.leftClient.team.getFans(), this.rightClient.team.getFans());
    }


    /**
     * Checks if a goal has occured this round.
     *
     * @return true if a goal has occured this round. False otherwise.
     */
    public boolean hasGoalOccuredThisRound(){
        return this.goalThisRound;
    }

    /**
     * Bans a player. Also checks if a client now has 3 banned players. If that is the case the client becomes
     * disqualified.
     *
     * @param player Player that is getting banned.
     */
    public void ban(Player player) {
        Client client = getClientFromEntity(player);
        player.ban();

        if (!client.disqualified) {
            lastBanned = player;
        }

        if (client.team.amountOfBannedPlayers() == 4) {
            client.disqualified = true;
        }
    }

    /**
     * Retrieves the last banned player.
     *
     * @return the last banned player.
     */
    public Player getLastBannedPlayer() {
        return lastBanned;
    }

    //Get all poos
    public Square[] getAllPoos(){
        return wombatPoos.toArray(new Square[wombatPoos.size()]);
    }

    public WombatCubesJson[] PoosToJson()
    { ArrayList<WombatCubesJson> wc= new ArrayList<WombatCubesJson>();

        for (Square s: wombatPoos) {
            
            WombatCubesJson wombatCubesJson= new WombatCubesJson();
            wombatCubesJson.xPos=s.x;
            wombatCubesJson.yPos=s.y;
            wc.add(wombatCubesJson);
        }
        return wc.toArray(new WombatCubesJson[wc.size()]);
    }
    public void removeFromUnban(Entity player){
        unbanList.remove(player);
    }

    /**
     * Starts the game by initializing the balls and setting the client states
     */
    public void start() {
        initBalls();

        getLeftClient().state = Client.IN_GAME;
        getRightClient().state = Client.IN_GAME;
        inGame = true;

        for (Client c : spectators) {
            c.state = Client.SPECTATING;
        }
    }

}


