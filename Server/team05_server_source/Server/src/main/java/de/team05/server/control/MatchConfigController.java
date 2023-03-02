package de.team05.server.control;

import com.google.gson.JsonSyntaxException;
import de.team05.server.Config;
import de.team05.server.json.ContainerJson;
import de.team05.server.json.MatchConfigJson;
import de.team05.server.model.MatchConfig;
import org.pmw.tinylog.Logger;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

/**
 * The MatchConfigController loads a MatchConfig from a file and creates the MatchConfig object.
 */
public class MatchConfigController {
    private MatchConfigJson matchConfigJson;
    public boolean validated;
    private MatchConfig matchConfig;

    /**
     * Constructs a MatchConfigController by loading the MatchConfig from the given filepath
     *
     * @param filename filepath of MatchConfigFile
     */
    public MatchConfigController(String filename) {

        try {
            String matchConfigString = loadMatchConfig(filename);
            ContainerJson container = JsonUtility.getContainer(matchConfigString);
            matchConfigJson = JsonUtility.getPayload(container, MatchConfigJson.class);
            validated = isMatchConfigValid();
            if (validated) {
                matchConfig = new MatchConfig(matchConfigJson);
            }
            Logger.info("MatchConfig was loaded.");
            Logger.trace("MatchConfig: " + matchConfig);

        } catch (IOException e) {
            matchConfigJson = null;
            validated = false;
            Logger.error("MatchConfig loading from File failed critically.");
            Logger.error(e.toString());
        } catch (JsonSyntaxException e) {
            matchConfigJson = null;
            validated = false;
            Logger.error("MatchConfig loading from File failed. ");
            Logger.error(e.toString());
        }
    }

    private String loadMatchConfig(String filename) throws IOException {
        String line;
        StringBuilder file = new StringBuilder();
        FileReader fileReader = new FileReader(filename);
        BufferedReader bufferedReader = new BufferedReader(fileReader);

        while ((line = bufferedReader.readLine()) != null) {
            file.append(line);
        }
        return file.toString();
    }

    /**
     * validates MatchConfig Timeouts and calls validateProbabilities to check if values are within appropriate limitations
     *
     * @return false if validation failed, true if succeeded
     */
    private boolean isMatchConfigValid() {
        if (matchConfigJson == null) {
            return false;
        }

        if (matchConfigJson.maxRounds == null || matchConfigJson.maxRounds > Config.MAX_ROUNDS_MAX_LIMIT || matchConfigJson.maxRounds < Config.MAX_ROUNDS_MIN_LIMIT) {
            return false;
        }


        if (matchConfigJson.timings.teamFormationTimeout == null) {
            return false;
        }
        if (matchConfigJson.timings.playerTurnTimeout == null || matchConfigJson.timings.playerTurnTimeout > Config.MAXIMUM_TIMEOUT_LIMIT || matchConfigJson.timings.playerTurnTimeout < 1) {
            return false;
        }
        if (matchConfigJson.timings.fanTurnTimeout == null || matchConfigJson.timings.fanTurnTimeout > Config.MAXIMUM_TIMEOUT_LIMIT || matchConfigJson.timings.fanTurnTimeout < 1) {
            return false;
        }
        if (matchConfigJson.timings.minBallPhaseAnimationDuration == null) {
            return false;
        }
        if (matchConfigJson.timings.minFanPhaseAnimationDuration == null) {
            return false;
        }
        if (matchConfigJson.timings.minPlayerPhaseAnimationDuration == null) {
            return false;
        }

        return areProbabilitiesValid();
    }

    /**
     * checks if probability values are within appropriate limitations
     *
     * @return false if validation failed, true if succeeded
     */
    private boolean areProbabilitiesValid() {
        if (matchConfigJson.probabilities.throwSuccess == null || matchConfigJson.probabilities.throwSuccess > 1 || matchConfigJson.probabilities.throwSuccess < 0 ||
                matchConfigJson.probabilities.knockOut == null || matchConfigJson.probabilities.knockOut > 1 || matchConfigJson.probabilities.knockOut < 0 ||
                matchConfigJson.probabilities.foolAway == null || matchConfigJson.probabilities.foolAway > 1 || matchConfigJson.probabilities.foolAway < 0 ||
                matchConfigJson.probabilities.catchSnitch == null || matchConfigJson.probabilities.catchSnitch > 1 || matchConfigJson.probabilities.catchSnitch < 0 ||
                matchConfigJson.probabilities.catchQuaffle == null || matchConfigJson.probabilities.catchQuaffle > 1 || matchConfigJson.probabilities.catchQuaffle < 0 ||
                matchConfigJson.probabilities.wrestQuaffle == null || matchConfigJson.probabilities.wrestQuaffle > 1 || matchConfigJson.probabilities.wrestQuaffle < 0) {
            return false;
        } else if (matchConfigJson.probabilities.extraMove.tinderblast == null || matchConfigJson.probabilities.extraMove.tinderblast > 1 || matchConfigJson.probabilities.extraMove.tinderblast < 0 ||
                matchConfigJson.probabilities.extraMove.cleansweep11 == null || matchConfigJson.probabilities.extraMove.cleansweep11 > 1 || matchConfigJson.probabilities.extraMove.cleansweep11 < 0 ||
                matchConfigJson.probabilities.extraMove.comet260 == null || matchConfigJson.probabilities.extraMove.comet260 > 1 || matchConfigJson.probabilities.extraMove.comet260 < 0 ||
                matchConfigJson.probabilities.extraMove.nimbus2001 == null || matchConfigJson.probabilities.extraMove.nimbus2001 > 1 || matchConfigJson.probabilities.extraMove.nimbus2001 < 0 ||
                matchConfigJson.probabilities.extraMove.firebolt == null || matchConfigJson.probabilities.extraMove.firebolt > 1 || matchConfigJson.probabilities.extraMove.firebolt < 0) {
            return false;
        } else
            return matchConfigJson.probabilities.foulDetection.flacking != null && matchConfigJson.probabilities
                    .foulDetection.flacking <= 1 && matchConfigJson.probabilities.foulDetection.flacking >= 0 &&
                    matchConfigJson.probabilities.foulDetection.haversacking != null && matchConfigJson.probabilities
                    .foulDetection.haversacking <= 1 && matchConfigJson.probabilities.foulDetection.haversacking >= 0 &&
                    matchConfigJson.probabilities.foulDetection.stooging != null && matchConfigJson.probabilities
                    .foulDetection.stooging <= 1 && matchConfigJson.probabilities.foulDetection.stooging >= 0 &&
                    matchConfigJson.probabilities.foulDetection.blatching != null && matchConfigJson.probabilities
                    .foulDetection.blatching <= 1 && matchConfigJson.probabilities.foulDetection.blatching >= 0 &&
                    matchConfigJson.probabilities.foulDetection.snitchnip != null && matchConfigJson.probabilities
                    .foulDetection.snitchnip <= 1 && matchConfigJson.probabilities.foulDetection.snitchnip >= 0 &&
                    matchConfigJson.probabilities.fanFoulDetection.elfTeleportation != null && matchConfigJson
                    .probabilities.fanFoulDetection.elfTeleportation <= 1 && matchConfigJson.probabilities
                    .fanFoulDetection.elfTeleportation >= 0 &&
                    matchConfigJson.probabilities.fanFoulDetection.goblinShock != null && matchConfigJson
                    .probabilities.fanFoulDetection.goblinShock <= 1 && matchConfigJson.probabilities
                    .fanFoulDetection.goblinShock >= 0 &&
                    matchConfigJson.probabilities.fanFoulDetection.trollRoar != null && matchConfigJson.probabilities
                    .fanFoulDetection.trollRoar <= 1 && matchConfigJson.probabilities.fanFoulDetection.trollRoar >= 0 &&
                    matchConfigJson.probabilities.fanFoulDetection.snitchSnatch != null && matchConfigJson
                    .probabilities.fanFoulDetection.snitchSnatch <= 1 && matchConfigJson.probabilities
                    .fanFoulDetection.snitchSnatch >= 0;
    }

    /**
     * Getter for MatchConfig
     *
     * @return returns MatchConfig, calling method has to check if Config is valid
     */
    public MatchConfig getMatchConfig() {
        return matchConfig;
    }
}