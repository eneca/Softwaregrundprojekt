package de.team05.server.model;

import de.team05.server.Strings;
import de.team05.server.json.MatchConfigJson;

import java.util.HashMap;

/**
 * A MatchConfig contains different settings for a Game. There are several values for timeouts,
 * timings and probabilities
 */
public class MatchConfig {

    /**
     * Defines the round when overlength starts
     */
    public final int maxRounds;

    /**
     * Different timings and timeouts
     */
    public final HashMap<String, Integer> timings = new HashMap<>();

    /**
     * Different probabilites
     */
    public final HashMap<String, Float> probabilities = new HashMap<>();

    public MatchConfigJson matchConfigJson;

    /**
     * Constructs a MatchConfig using the given MatchConfigJson
     *
     * @param matchConfigJson MatchConfigJson from which to construct the MatchConfig
     */
    public MatchConfig(MatchConfigJson matchConfigJson) {
        this.matchConfigJson = matchConfigJson;
        maxRounds = matchConfigJson.maxRounds;

        timings.put(Strings.TEAM_FORMATION_TIMEOUT, matchConfigJson.timings.teamFormationTimeout);
        timings.put(Strings.PLAYER_TURN_TIMEOUT, matchConfigJson.timings.playerTurnTimeout);
        timings.put(Strings.FAN_TURN_TIMEOUT, matchConfigJson.timings.fanTurnTimeout);
        timings.put(Strings.MIN_PLAYER_PHASE_ANIMATION_DURATION,
                matchConfigJson.timings.minPlayerPhaseAnimationDuration);
        timings.put(Strings.MIN_FAN_PHASE_ANIMATION_DURATION,
                matchConfigJson.timings.minFanPhaseAnimationDuration);
        timings.put(Strings.MIN_BALL_PHASE_ANIMATION_DURATION,
                matchConfigJson.timings.minBallPhaseAnimationDuration);

        probabilities.put(Strings.THROW_SUCCESS, matchConfigJson.probabilities.throwSuccess);
        probabilities.put(Strings.KNOCK_OUT, matchConfigJson.probabilities.knockOut);
        probabilities.put(Strings.FOOL_AWAY, matchConfigJson.probabilities.foolAway);
        probabilities.put(Strings.CATCH_SNITCH, matchConfigJson.probabilities.catchSnitch);
        probabilities.put(Strings.CATCH_QUAFFLE, matchConfigJson.probabilities.catchQuaffle);
        probabilities.put(Strings.WREST_QUAFFLE, matchConfigJson.probabilities.wrestQuaffle);
        probabilities.put(Strings.brooms[0], matchConfigJson.probabilities.extraMove.tinderblast);
        probabilities.put(Strings.brooms[1], matchConfigJson.probabilities.extraMove.cleansweep11);
        probabilities.put(Strings.brooms[2], matchConfigJson.probabilities.extraMove.comet260);
        probabilities.put(Strings.brooms[3], matchConfigJson.probabilities.extraMove.nimbus2001);
        probabilities.put(Strings.brooms[4], matchConfigJson.probabilities.extraMove.firebolt);
        probabilities.put(Strings.FLACKING, matchConfigJson.probabilities.foulDetection.flacking);
        probabilities.put(Strings.HAVERSACKING, matchConfigJson.probabilities.foulDetection.haversacking);
        probabilities.put(Strings.STOOGING, matchConfigJson.probabilities.foulDetection.stooging);
        probabilities.put(Strings.BLATCHING, matchConfigJson.probabilities.foulDetection.blatching);
        probabilities.put(Strings.SNITCHNIP, matchConfigJson.probabilities.foulDetection.snitchnip);
        probabilities.put(Strings.ELF_TELEPORTATION,
                matchConfigJson.probabilities.fanFoulDetection.elfTeleportation);
        probabilities.put(Strings.GOBLIN_SHOCK, matchConfigJson.probabilities.fanFoulDetection.goblinShock);
        probabilities.put(Strings.TROLL_ROAR,
                matchConfigJson.probabilities.fanFoulDetection.trollRoar);
        probabilities.put(Strings.SNITCH_SNATCH, matchConfigJson.probabilities.fanFoulDetection.snitchSnatch);
        probabilities.put(Strings.WOMBAT_POO,matchConfigJson.probabilities.fanFoulDetection.wombatPoo);
    }

    /**
     * Returns MatchConfig information as a string
     *
     * @return string of full MatchConfig information
     */
    @Override
    public String toString() {
        return "MatchConfig{" +
                "maxRounds=" + maxRounds +
                ", timings=" + timings +
                ", probabilities=" + probabilities +
                '}';
    }
}
