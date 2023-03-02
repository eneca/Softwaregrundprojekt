package de.team05.server.control;

import de.team05.server.Config;
import de.team05.server.Strings;
import de.team05.server.json.*;
import de.team05.server.model.*;
import org.pmw.tinylog.Logger;

import java.util.Arrays;
import java.util.Timer;
import java.util.TimerTask;

/**
 * The PreGameMessageHandler handles incoming teamConfig and teamFormation messages and sends the
 * according responses.
 */
public class PreGameMessageHandler {
    /**
     * The game that this PreGameMessageHandler handles
     */
    private final Game game;

    /**
     * The matchStart that is being sent to the clients after both teamConfigs are received
     */
    private MatchStartJson matchStartJson;

    /**
     * The error message that is sent when a client sends an invalid message
     */
    private String errorMessage;

    /**
     * The Timer used to check if the clients did not send a teamFormation in time
     */
    private final Timer timer;

    /**
     * The TimerTask which gets scheduled for when the clients did not send a teamFormation in time
     */
    public final TimerTask timerTask;

    /**
     * Constructs a PreGameMessageHandler handling the given game
     *
     * @param game The game to handle
     */
    public PreGameMessageHandler(Game game) {
        this.game = game;
        this.timer = new Timer("teamFormationTimer");
        timerTask = new TimerTask() {
            @Override
            public void run() {
                timedOut();
            }
        };
    }

    /**
     * This method gets called when 1 or 2 Clients did not send a teamFormation in time.
     * A matchFinish gets sent
     */
    private void timedOut() {
        if (game.getLeftClient().state != Client.FORMATION_RECEIVED && game.getRightClient().state != Client.FORMATION_RECEIVED) {
            //no client sent team formation
            Logger.warn("No client sent a team formation in time.");
            ProtocolUtility.kickClient(game.getRightClient(), "No team formation received in time");
            ProtocolUtility.kickClient(game.getLeftClient(), "No team formation received in time");
        }

        if (game.getLeftClient().state != Client.FORMATION_RECEIVED) {
            Logger.warn("Team formation for client " + game.getLeftClient().name + " not received" +
                    " in time. Other client wins game.");
            MatchFinishJson matchFinishJson = new MatchFinishJson(
                    game.roundCounter,
                    game.getLeftClient().team.points,
                    game.getRightClient().team.points,
                    game.getRightClient().name,
                    Strings.VIOLATION_OF_PROTOCOL
            );

            ProtocolUtility.broadcastPayload(game, matchFinishJson);
        }

        if (game.getRightClient().state != Client.FORMATION_RECEIVED) {
            Logger.warn("Team formation for client " + game.getRightClient().name + " not received" +
                    " in time. Other client wins game.");
            MatchFinishJson matchFinishJson = new MatchFinishJson(
                    game.roundCounter,
                    game.getLeftClient().team.points,
                    game.getRightClient().team.points,
                    game.getLeftClient().name,
                    Strings.VIOLATION_OF_PROTOCOL
            );

            ProtocolUtility.broadcastPayload(game, matchFinishJson);
        }

    }

    /**
     * Is called when server receives TeamConfig, checks the deserialized object and updates the
     * model client
     *
     * @param client         client which has sent the TeamConfig
     * @param teamConfigJson TeamConfig which was deserialized
     * @return -1 if validation failed, 1 if first client was validated, 2 if both clients are
     * validated
     */
    public int onTeamConfigMessageReceived(Client client, TeamConfigJson teamConfigJson) {
        if (!isTeamConfigValid(teamConfigJson)) {
            ProtocolUtility.kickClient(client, errorMessage);
            return Config.VALIDATION_FAILED;
        } else {
            boolean left;
            left = game.getLeftClient() == null;

            client.team = new Team(left, teamConfigJson);
            client.state = Client.IN_LOBBY_AS_PLAYER;
            Logger.debug("TeamConfig validation successful for client " + client.name);

            if (!game.hasLeftClient()) {
                game.setLeftClient(client);

                matchStartJson = new MatchStartJson();
                matchStartJson.leftTeamConfigJson = teamConfigJson;
                matchStartJson.leftTeamUserName = client.name;
                matchStartJson.matchConfig = game.matchConfig.matchConfigJson;

                Logger.debug("Client " + client.name + " is now left Client of game " + game.name + ".");

                return Config.VALIDATION_FIRST_SUCCESS;
            } else {
                game.setRightClient(client);

                Logger.debug("Client " + client.name + " is now right Client of game " + game.name + ".");

                game.getLeftClient().state = Client.WAITING_FOR_FORMATION;
                game.getRightClient().state = Client.WAITING_FOR_FORMATION;

                matchStartJson.rightTeamConfigJson = teamConfigJson;
                matchStartJson.rightTeamUserName = client.name;
                matchStartJson.matchConfig =game.matchConfig.matchConfigJson;

                timer.schedule(timerTask, game.matchConfig.timings.get(Strings.TEAM_FORMATION_TIMEOUT));
                game.matchStarted = true;
                game.matchStartJsonContainer = ProtocolUtility.broadcastPayload(game, matchStartJson);

                Logger.info("onTeamConfigMessageReceived finished successful.");

                return Config.VALIDATION_BOTH_SUCCESS;
            }
        }
    }

    /**
     * Is called when server receives TeamFormation, calls methods to check the deserialized object and updates
     * the model client
     *
     * @param client            client which has sent the TeamFormation
     * @param teamFormationJson TeamFormation which was deserialized
     * @return -1 if validation failed, 1 if first client was validated, 2 if both clients are
     * validated
     */
    public int onTeamFormationMessageReceived(Client client, TeamFormationJson teamFormationJson) {
        boolean leftSide = false;
        if (game.getLeftClient() == client) {
            leftSide = true;
        }
        if (!isPlayerPositionOnPitch(teamFormationJson.players.seeker) || !isPlayerPositionOnPitch(teamFormationJson.players.keeper) || !isPlayerPositionOnPitch(teamFormationJson.players.chaser1) || !isPlayerPositionOnPitch(teamFormationJson.players.chaser2) || !isPlayerPositionOnPitch(teamFormationJson.players.chaser3) || !isPlayerPositionOnPitch(teamFormationJson.players.beater1) || !isPlayerPositionOnPitch(teamFormationJson.players.beater2)) {
            ProtocolUtility.kickClient(client, "PlayerPositionOutOfPitch");
            ProtocolUtility.kickClient(client, errorMessage);
            return Config.VALIDATION_FAILED;
        }
        if (!isPlayerStartPosValid(teamFormationJson.players.keeper, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.seeker, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.chaser1, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.chaser2, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.chaser3, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.beater1, leftSide) || !isPlayerStartPosValid(teamFormationJson.players.beater2, leftSide)) {
            ProtocolUtility.kickClient(client, "AnyPlayerNotAtValidStartingPosition");
            return Config.VALIDATION_FAILED;
        }

        client.state = Client.FORMATION_RECEIVED;
        if (game.getLeftClient().state == game.getRightClient().state) {
            setPositions(client.team.players, teamFormationJson.players);
            Logger.info("onTeamFormationMessageReceived finished successful for both clients.");
            timerTask.cancel();
            return Config.VALIDATION_BOTH_SUCCESS;
        } else {
            setPositions(client.team.players, teamFormationJson.players);
            Logger.info("onTeamFormationMessageReceived finished successful for first client.");
            return Config.VALIDATION_FIRST_SUCCESS;
        }
    }

    /**
     * Checks if a given teamConfig is valid
     *
     * @param teamConfigJson TeamConfig, which shall be validated
     * @return true if teamConfig is valid, false otherwise
     */
    private boolean isTeamConfigValid(TeamConfigJson teamConfigJson) {
        return isNameValid(teamConfigJson) && isMottoValid(teamConfigJson) && areColorsValid(teamConfigJson) &&
                isImageValid(teamConfigJson) && areFansValid(teamConfigJson) &&
                arePlayersValid(teamConfigJson) && isEveryBroomUsed(teamConfigJson) && isGenderGapExisting(teamConfigJson);
    }

    /**
     * Checks if the name of a given team is valid
     *
     * @param teamConfigJson TeamConfig that contains the name of the team
     * @return true if every player is valid, false otherwise
     */
    private boolean isNameValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.name == null || teamConfigJson.name.isEmpty()) {
            errorMessage = "EmptyTeamName";
            return false;
        } else if (teamConfigJson.name.length() > Config.TEAM_NAME_UPPER_CHAR_LIMIT || teamConfigJson.name.length() <= Config.TEAM_NAME_LOWER_CHAR_LIMIT) {
            errorMessage = "TeamNameOutOfBounds";
            return false;
        } else if (!teamConfigJson.name.matches("[a-zA-Z0-9 ]*")) {
            errorMessage = "InvalidLetterInTeamName";
            return false;
        }
        return true;
    }

    /**
     * Checks if motto is valid
     *
     * @param teamConfigJson contains motto
     * @return true if motto is valid, false otherwise
     */
    private boolean isMottoValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.motto == null || teamConfigJson.motto.isEmpty()) {
            errorMessage = "EmptyTeamMotto";
            return false;
        } else if (teamConfigJson.name.length() > Config.TEAM_NAME_UPPER_CHAR_LIMIT || teamConfigJson.name.length() <= Config.TEAM_NAME_LOWER_CHAR_LIMIT) {
            errorMessage = "TeamMottoOutOfBounds";
            return false;
        } else if (!teamConfigJson.name.matches("[a-zA-Z0-9 .:,;?!'-]*")) {
            errorMessage = "InvalidLetterInTeamMotto";
            return false;
        }
        return true;
    }


    /**
     * Checks if colors are valid
     *
     * @param teamConfigJson contains colors
     * @return true if both colors are valid, false otherwise
     */
    private boolean areColorsValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.colors == null || teamConfigJson.colors.primary == null || teamConfigJson.colors.secondary == null || teamConfigJson.colors.primary.isEmpty() || teamConfigJson.colors.secondary.isEmpty()) {
            errorMessage = "InvalidTeamColors";
            return false;
        } else if (teamConfigJson.colors.primary.equals(teamConfigJson.colors.secondary)) {
            errorMessage = "TeamColorsAreTheSame";
            return false;
        } else {
            try {
                Long.parseLong(teamConfigJson.colors.primary, 16);
                Long.parseLong(teamConfigJson.colors.secondary, 16);
            } catch (NumberFormatException nfe) {
                errorMessage = "InvalidTeamColorFormat";
                return false;
            }
        }
        return true;
    }

    /**
     * Checks if Image is valid
     *
     * @param teamConfigJson contains image
     * @return true if image is valid, false otherwise
     */
    private boolean isImageValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.image == null || teamConfigJson.image.isEmpty()) {
            errorMessage = "InvalidTeamImage";
            return false;
        }
        return true;
    }

    /**
     * Checks if (7) fans exist and have correct distribution
     *
     * @param teamConfigJson contains fans
     * @return true if fans are valid, false otherwise
     */
    private boolean areFansValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.fans == null) {
            errorMessage = "FansNotFound";
            return false;
        } else if (teamConfigJson.fans.goblins == 0 || teamConfigJson.fans.trolls == 0 || teamConfigJson.fans.elves == 0 || teamConfigJson.fans.nifflers == 0|| teamConfigJson.fans.wombats== 0) {
            errorMessage = "InvalidFanDistribution";
            //System.out.println(teamConfigJson.fans.goblins+" "+teamConfigJson.fans.trolls+" "+teamConfigJson.fans.elves+" "+teamConfigJson.fans.nifflers+" "+teamConfigJson.fans.wombats);
            return false;
        } else if (teamConfigJson.fans.goblins + teamConfigJson.fans.trolls + teamConfigJson.fans.elves + teamConfigJson.fans.nifflers + teamConfigJson.fans.wombats != 7) {
            errorMessage = "InvalidFanNumber";
            return false;
        }
        return true;
    }

    /**
     * Checks if every player exists and has a name, broom and sex.
     *
     * @param teamConfigJson contains playersByName
     * @return true if every player is valid, false otherwise
     */
    private boolean arePlayersValid(TeamConfigJson teamConfigJson) {
        if (teamConfigJson.players == null) {
            errorMessage = "PlayersNotFound";
            return false;
        } else if (teamConfigJson.players.seeker == null || teamConfigJson.players.keeper == null || teamConfigJson.players.chaser1 == null || teamConfigJson.players.chaser2 == null || teamConfigJson.players.chaser3 == null || teamConfigJson.players.beater1 == null || teamConfigJson.players.beater2 == null) {
            errorMessage = "AnyPlayerNotFound";
            return false;
        } else if (teamConfigJson.players.seeker.name == null || teamConfigJson.players.seeker.broom == null || teamConfigJson.players.seeker.sex == null ||
                teamConfigJson.players.seeker.name.isEmpty() || teamConfigJson.players.seeker.broom.isEmpty() || teamConfigJson.players.seeker.sex.isEmpty() ||
                teamConfigJson.players.keeper.name == null || teamConfigJson.players.keeper.broom == null || teamConfigJson.players.keeper.sex == null ||
                teamConfigJson.players.keeper.name.isEmpty() || teamConfigJson.players.keeper.broom.isEmpty() || teamConfigJson.players.keeper.sex.isEmpty() ||
                teamConfigJson.players.chaser1.name == null || teamConfigJson.players.chaser1.broom == null || teamConfigJson.players.chaser1.sex == null ||
                teamConfigJson.players.chaser1.name.isEmpty() || teamConfigJson.players.chaser1.broom.isEmpty() || teamConfigJson.players.chaser1.sex.isEmpty() ||
                teamConfigJson.players.chaser2.name == null || teamConfigJson.players.chaser2.broom == null || teamConfigJson.players.chaser2.sex == null ||
                teamConfigJson.players.chaser2.name.isEmpty() || teamConfigJson.players.chaser2.broom.isEmpty() || teamConfigJson.players.chaser2.sex.isEmpty() ||
                teamConfigJson.players.chaser3.name == null || teamConfigJson.players.chaser3.broom == null || teamConfigJson.players.chaser3.sex == null ||
                teamConfigJson.players.chaser3.name.isEmpty() || teamConfigJson.players.chaser3.broom.isEmpty() || teamConfigJson.players.chaser3.sex.isEmpty() ||
                teamConfigJson.players.beater1.name == null || teamConfigJson.players.beater1.broom == null || teamConfigJson.players.beater1.sex == null ||
                teamConfigJson.players.beater1.name.isEmpty() || teamConfigJson.players.beater1.broom.isEmpty() || teamConfigJson.players.beater1.sex.isEmpty() ||
                teamConfigJson.players.beater2.name == null || teamConfigJson.players.beater2.broom == null || teamConfigJson.players.beater2.sex == null ||
                teamConfigJson.players.beater2.name.isEmpty() || teamConfigJson.players.beater2.broom.isEmpty() || teamConfigJson.players.beater2.sex.isEmpty()) {
            errorMessage = "InvalidPlayer";
            return false;
        }
        return true;
    }

    /**
     * Checks if every broom is used and if the brooms are valid.
     *
     * @param teamConfigJson contains brooms of all playersByName
     * @return true if every broom is used at least once, false otherwise
     */
    private boolean isEveryBroomUsed(TeamConfigJson teamConfigJson) {
        int[] brooms = new int[Strings.brooms.length];

        try {
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.seeker.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.keeper.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.chaser1.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.chaser2.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.chaser3.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.beater1.broom)]++;
            brooms[Arrays.asList(Strings.brooms).indexOf(teamConfigJson.players.beater2.broom)]++;
        } catch (ArrayIndexOutOfBoundsException e) {
            errorMessage = "BroomDoesNotExist";
            return false;
        }

        for (int i : brooms) {
            if (i == 0) {
                errorMessage = "BroomWasNotUsed";
                return false;
            }
        }

        return true;
    }

    /**
     * Checks if sexes exist and are equally distributed.
     *
     * @param teamConfigJson contains sexes of all playersByName
     * @return true if genders are equally distributed, false otherwise
     */
    private boolean isGenderGapExisting(TeamConfigJson teamConfigJson) {
        int[] sexes = new int[Strings.sex.length];

        try {
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.seeker.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.keeper.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.chaser1.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.chaser2.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.chaser3.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.beater1.sex)]++;
            sexes[Arrays.asList(Strings.sex).indexOf(teamConfigJson.players.beater2.sex)]++;
        } catch (ArrayIndexOutOfBoundsException e) {
            errorMessage = "SexDoesNotExist";
            return false;
        }
        if (sexes[0] != 3 && sexes[1] != 3) {
            errorMessage = "GenderGapExists";
            return false;
        }
        return true;
    }

    /**
     * Checks if player is positioned on the pitch
     *
     * @param player player that will be checked
     * @return true if player position is valid, false otherwise
     */
    private boolean isPlayerPositionOnPitch(PlayerPosJson player) {
        if (player.xPos < 0 || player.xPos > 16 || player.yPos < 0 || player.yPos > 12) {
            return false;
        }
        return ((player.xPos != 0 && player.xPos != 16) || (player.yPos <= 8 && player.yPos > 3)) && ((player.xPos != 1 && player.xPos != 15) || (player.yPos <= 10 && player.yPos > 1)) && ((player.xPos != 2 && player.xPos != 14) || (player.yPos != 12 && player.yPos != 0));
    }


    /**
     * Checks if player is inside of his zone.
     *
     * @param player   the player that shall be checked
     * @param leftSide true if player belongs to left team, false otherwise
     * @return true if player is in his zone, false otherwise
     */
    private boolean isPlayerStartPosValid(PlayerPosJson player, boolean leftSide) {
        if (leftSide) {
            if (player.xPos >= 8)
                return false;
        } else {
            if (player.xPos <= 8)
                return false;
        }
        return (player.xPos != 7 && player.xPos != 9 || 4 >= player.yPos || player.yPos >= 8);
    }

    /**
     * Store player positions from teamFormation in respective model class
     *
     * @param players        array consisting of all players of respective team
     * @param playersPosJson positions from teamFormation
     */
    private void setPositions(Player[] players, PlayersPosJson playersPosJson) {
        players[0].setPos(new Square(playersPosJson.seeker.xPos, playersPosJson.seeker.yPos));
        players[1].setPos(new Square(playersPosJson.keeper.xPos, playersPosJson.keeper.yPos));
        players[2].setPos(new Square(playersPosJson.chaser1.xPos, playersPosJson.chaser1.yPos));
        players[3].setPos(new Square(playersPosJson.chaser2.xPos, playersPosJson.chaser2.yPos));
        players[4].setPos(new Square(playersPosJson.chaser3.xPos, playersPosJson.chaser3.yPos));
        players[5].setPos(new Square(playersPosJson.beater1.xPos, playersPosJson.beater1.yPos));
        players[6].setPos(new Square(playersPosJson.beater2.xPos, playersPosJson.beater2.yPos));
    }
}
