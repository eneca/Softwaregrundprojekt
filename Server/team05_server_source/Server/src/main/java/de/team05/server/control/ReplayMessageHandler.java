package de.team05.server.control;

import de.team05.server.Strings;
import de.team05.server.json.LogJson;
import de.team05.server.json.MatchConfigJson;
import de.team05.server.json.ReplayJson;
import de.team05.server.model.Client;
import de.team05.server.model.Game;
import de.team05.server.model.MatchConfig;

import java.util.List;

/**
 * The ReplayMessageHandler responds to incoming getReplay messages by sending a replay
 */
class ReplayMessageHandler {
    private final Game game;

    /**
     * Constructs a ReplayMessageHandler for the given game
     *
     * @param game Game that this ReplayMessageHandler handles
     */
    public ReplayMessageHandler(Game game) {
        this.game = game;
    }

    /**
     * Gets called when a getReplay message is received.
     * Sends a replay to the client that requested it
     *
     * @param client Client that sent the getReplay message
     */
    public void onGetReplayReceived(Client client) {
        ReplayJson replayJson = new ReplayJson(game.name, game.initialTimestamp, getMatchConfig(game),
                game.getLeftClient().team.teamConfig, game.getRightClient().team.teamConfig,
                game.getLeftClient().team.teamConfig.name, game.getRightClient().team.teamConfig.name,
                getNames(game.spectators), game.firstSnapshot, getLog(game.replay));
        ProtocolUtility.sendPayload(client, replayJson);
    }

    /**
     * Creates a MatchConfigJson for the given game
     *
     * @param game Game for whicht a MatchConfigJson is created
     * @return The MatchConfigJson that is created
     */
    private MatchConfigJson getMatchConfig(Game game) {
        MatchConfig matchConfig = game.matchConfig;
        MatchConfigJson matchConfigJson = new MatchConfigJson();
        matchConfigJson.maxRounds = matchConfig.maxRounds;
        matchConfigJson.timings.teamFormationTimeout = matchConfig.timings.get(Strings.TEAM_FORMATION_TIMEOUT);
        matchConfigJson.timings.playerTurnTimeout = matchConfig.timings.get(Strings.PLAYER_TURN_TIMEOUT);
        matchConfigJson.timings.fanTurnTimeout = matchConfig.timings.get(Strings.FAN_TURN_TIMEOUT);
        matchConfigJson.timings.minPlayerPhaseAnimationDuration = matchConfig.timings.get(Strings.MIN_PLAYER_PHASE_ANIMATION_DURATION);
        matchConfigJson.timings.minFanPhaseAnimationDuration = matchConfig.timings.get(Strings.MIN_FAN_PHASE_ANIMATION_DURATION);
        matchConfigJson.timings.minBallPhaseAnimationDuration = matchConfig.timings.get(Strings.MIN_BALL_PHASE_ANIMATION_DURATION);
        matchConfigJson.probabilities.throwSuccess = matchConfig.probabilities.get(Strings.THROW_SUCCESS);
        matchConfigJson.probabilities.knockOut = matchConfig.probabilities.get(Strings.KNOCK_OUT);
        matchConfigJson.probabilities.foolAway = matchConfig.probabilities.get(Strings.FOOL_AWAY);
        matchConfigJson.probabilities.catchSnitch = matchConfig.probabilities.get(Strings.CATCH_SNITCH);
        matchConfigJson.probabilities.catchQuaffle = matchConfig.probabilities.get(Strings.CATCH_QUAFFLE);
        matchConfigJson.probabilities.wrestQuaffle = matchConfig.probabilities.get(Strings.WREST_QUAFFLE);
        matchConfigJson.probabilities.extraMove.tinderblast = matchConfig.probabilities.get(Strings.EXTRA_MOVE_TINDERBLAST);
        matchConfigJson.probabilities.extraMove.cleansweep11 = matchConfig.probabilities.get(Strings.EXTRA_MOVE_CLEANSWEEP11);
        matchConfigJson.probabilities.extraMove.comet260 = matchConfig.probabilities.get(Strings.EXTRA_MOVE_COMET260);
        matchConfigJson.probabilities.extraMove.nimbus2001 = matchConfig.probabilities.get(Strings.EXTRA_MOVE_NIMBUS2001);
        matchConfigJson.probabilities.extraMove.firebolt = matchConfig.probabilities.get(Strings.EXTRA_MOVE_FIREBOLT);
        matchConfigJson.probabilities.foulDetection.flacking = matchConfig.probabilities.get(Strings.FLACKING);
        matchConfigJson.probabilities.foulDetection.haversacking = matchConfig.probabilities.get(Strings.HAVERSACKING);
        matchConfigJson.probabilities.foulDetection.stooging = matchConfig.probabilities.get(Strings.STOOGING);
        matchConfigJson.probabilities.foulDetection.blatching = matchConfig.probabilities.get(Strings.BLATCHING);
        matchConfigJson.probabilities.foulDetection.snitchnip = matchConfig.probabilities.get(Strings.SNITCHNIP);
        matchConfigJson.probabilities.fanFoulDetection.elfTeleportation = matchConfig.probabilities.get(Strings.ELF_TELEPORTATION);
        matchConfigJson.probabilities.fanFoulDetection.goblinShock = matchConfig.probabilities.get(Strings.GOBLIN_SHOCK);
        matchConfigJson.probabilities.fanFoulDetection.trollRoar = matchConfig.probabilities.get(Strings.TROLL_ROAR);
        matchConfigJson.probabilities.fanFoulDetection.snitchSnatch = matchConfig.probabilities.get(Strings.SNITCH_SNATCH);

        return matchConfigJson;
    }

    /**
     * Gets the names for a list of clients
     *
     * @param spectators List of clients from which to get the names
     * @return A String array of all the names of the given clients
     */
    private String[] getNames(List<Client> spectators) {
        String[] spectatornames = new String[spectators.size() - 1];
        for (int i = 0; i < spectatornames.length; i++) {
            spectatornames[i] = spectators.get(i + 1).name;
        }
        return spectatornames;
    }

    /**
     * Converts the given replay to an array of LogJsons
     *
     * @param replay The replay to be converted
     * @return The Array of LogJsons
     */
    private LogJson[] getLog(List<LogJson> replay) {
        LogJson[] logJsons = new LogJson[replay.size()];
        return replay.toArray(logJsons);
    }
}
