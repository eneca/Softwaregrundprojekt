package de.team05.server.model;

import de.team05.server.Strings;
import de.team05.server.json.TeamConfigJson;

import java.util.Arrays;
import java.util.HashMap;

/**
 * Represents a team in a Quidditch game.
 */
public class Team {

    /**
     * Boolean to check whether this team is the left team in a game. True -&gt; team is the left
     * team. False -&gt; right team.
     */
    private final boolean left;

    /**
     * The teamConfig Json that was used to create this team.
     */
    public final TeamConfigJson teamConfig;

    /**
     * An array of this team's players.
     */
    public final Player[] players;

    /**
     * Hashmap of this team's players. Key is the String id corresponding to the activeEntityId in the specifications.
     */
    private final HashMap<String, Player> playersByEntityId;

    /**
     * An array of this team's fans.
     */
    public final Fan[] fans;

    /**
     * This team's current points in the match.
     */
    public Integer points;

    /**
     * Boolean to check whether this team caught the snitch. Used for figuring out the victory reason of the match.
     */
    public boolean caughtSnitch;

    /**
     * Updates the team using the given team config.
     *
     * @param left Boolean defining if the Team is on the left or on the right side of the pitch.
     *            True if the team is on the left side, false if it is on the right.
     * @param teamConfigJson The team config to be used for the team
     */
    public Team(boolean left, TeamConfigJson teamConfigJson) {
        players = new Player[7];
        fans = new Fan[7];
        playersByEntityId = new HashMap<>();

        this.teamConfig = teamConfigJson;
        this.points = 0;
        this.left = left;


        String prefix;
        if (left) {
            prefix = "left";
        } else {
            prefix = "right";
        }

        for (int i = 0; i < teamConfigJson.fans.goblins; i++) {
            Fan goblin = new Fan(Strings.GOBLIN, prefix + Strings.GOBLIN);
            fans[i] = goblin;
        }
        for (int i = 0; i < teamConfigJson.fans.trolls; i++) {
            Fan troll = new Fan(Strings.TROLL, prefix + Strings.TROLL);
            fans[i + teamConfigJson.fans.goblins] = troll;
        }
        for (int i = 0; i < teamConfigJson.fans.elves; i++) {
            Fan elf = new Fan(Strings.ELF, prefix + Strings.ELF);
            fans[i + teamConfigJson.fans.goblins + teamConfigJson.fans.trolls] = elf;
        }
        for (int i = 0; i < teamConfigJson.fans.wombats; i++) {
            Fan wombat = new Fan(Strings.WOMBAT, prefix + Strings.WOMBAT);
            fans[i + teamConfigJson.fans.elves + teamConfigJson.fans.trolls+ teamConfigJson.fans.goblins] = wombat;
        }
        for (int i = 0; i < teamConfigJson.fans.nifflers; i++) {
            Fan niffler = new Fan(Strings.NIFFLER, prefix + Strings.NIFFLER);
            fans[i+teamConfigJson.fans.goblins + teamConfigJson.fans.trolls + teamConfigJson.fans.elves + teamConfigJson.fans.wombats] = niffler;
        }

        Seeker seeker = new Seeker(prefix + Strings.SEEKER, teamConfigJson.players.seeker.broom);
        Keeper keeper = new Keeper(prefix + Strings.KEEPER, teamConfigJson.players.keeper.broom);
        Chaser chaser1 = new Chaser(prefix + Strings.CHASER1, teamConfigJson.players.chaser1.broom);
        Chaser chaser2 = new Chaser(prefix + Strings.CHASER2, teamConfigJson.players.chaser2.broom);
        Chaser chaser3 = new Chaser(prefix + Strings.CHASER3, teamConfigJson.players.chaser3.broom);
        Beater beater1 = new Beater(prefix + Strings.BEATER1, teamConfigJson.players.beater1.broom);
        Beater beater2 = new Beater(prefix + Strings.BEATER2, teamConfigJson.players.beater2.broom);

        players[0] = seeker;
        players[1] = keeper;
        players[2] = chaser1;
        players[3] = chaser2;
        players[4] = chaser3;
        players[5] = beater1;
        players[6] = beater2;

        playersByEntityId.put(seeker.id, seeker);
        playersByEntityId.put(keeper.id, keeper);
        playersByEntityId.put(chaser1.id, chaser1);
        playersByEntityId.put(chaser2.id, chaser2);
        playersByEntityId.put(chaser3.id, chaser3);
        playersByEntityId.put(beater1.id, beater1);
        playersByEntityId.put(beater2.id, beater2);
    }

    /**
     * Returns the corresponding Player of this team with the given playerId, or null if the
     * player is not in the team.
     *
     * @param playerId PlayerId of the Player
     * @return The Player in this team with the corresponding playerId, or null if there is none
     */
    public Player getPlayer(String playerId){
        return playersByEntityId.get(playerId);
    }

    /**
     * Gets this teams players array.
     *
     * @return The players array.
     */
    public Player[] getPlayers() {
        return players;
    }

    /**
     * Gets this teams fans array.
     *
     * @return The fans array.
     */
    public Fan[] getFans() {
        return fans;
    }

    /**
     * Gets the amount of this team's banned players.
     *
     * @return The amount of this team's banned players.
     */
    public int amountOfBannedPlayers() {
        int counter = 0;
        for (Player p : players) {
            if (p.isBanned()) {
                counter++;
            }
        }
        return counter;
    }

    @Override
    public String toString() {
        return "Team{" +
                "\nleft=" + left +
                ",\nteamConfig='" + teamConfig + '\'' +
                ", \nplayers=" + Arrays.toString(players) +
                ", \nfans=" + Arrays.toString(fans) +
                ", \npoints=" + points +
                ", \ncaughtSnitch=" + caughtSnitch +
                '}';
    }
}
