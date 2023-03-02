package de.team05.server.control;

import de.team05.server.Config;
import de.team05.server.Strings;
import de.team05.server.json.*;
import de.team05.server.model.*;
import org.pmw.tinylog.Logger;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.*;

import static java.lang.StrictMath.abs;

/**
 * The IngameMessageHandler receives DeltaRequests, PauseRequests and ContinueRequests and
 * handles them appropriately by sending messages.
 *
 * DeltaRequests are validated and the corresponding DeltaBroadcasts/Next messages are sent
 * Pause-/ContinueRequests are validated and the corresponding responses are sent.
 */
class InGameMessageHandler {

    /**
     * the Game that this inGameMessageHandler is handling.
     */
    private final Game game;

    /**
     * The time in millis when the last snapshot was sent.
     */
    private long lastSnapshotMillis;

    /**
     * The Timer that is responsible for handling the timings.
     */
    private final Timer timer;

    /**
     * The TimerTask that is called when the timer runs out.
     */
    public TimerTask timerTask;

    /**
     * Creates a new InGameMessageHandler using the given Game.
     *
     * @param game Game to be handled
     */
    public InGameMessageHandler(Game game) {
        this.game = game;
        timer = new Timer("timer");

        //empty timerTask so we can cancel it later
        timerTask = new TimerTask() {
            @Override
            public void run() {
                //empty
            }
        };
    }

    /**
     * Executes appropriate actions for incoming deltaRequests. Checks if the messages are valid,
     * are sent by valid clients and sends corresponding snapshots (including deltaResponses),
     * next messages and matchStart messages.
     *
     * @param client           Client that sent the deltaRequest
     * @param deltaRequestJson The DeltaRequest that was sent
     */
    public void onDeltaRequestMessageReceived(Client client, DeltaRequestJson deltaRequestJson) {

        Logger.debug("Received deltaRequest of type " + deltaRequestJson.deltaType);

        if (client != game.getClientFromEntity(game.activeEntity)) {
            //oh boy. The wrong client sent a message
            Logger.warn("Wrong client " + client.name + " + sent message");
            return;
        }

        if (game.paused) {
            //game is paused -> discard
            Logger.warn(client.name + " sent a delta request even though the game is paused.");
            return;
        }

        boolean deltaIsMove = false;
        boolean validMove = false;

        switch (deltaRequestJson.deltaType) {
            case Strings.BLUDGER_BEATING:
                if (isBludgerBeatingValid(client, deltaRequestJson)) {
                    bludgerBeating(client, deltaRequestJson);
                    validMove = true;
                }
                break;
            case Strings.QUAFFLE_THROW:
                if (isQuaffleThrowValid(client, deltaRequestJson)) {
                    quaffleThrow(deltaRequestJson);
                    validMove = true;
                }
                break;
            case Strings.SNITCH_SNATCH:
                if (isSnitchSnatchValid()) {
                    snitchSnatch();
                    validMove = true;
                }
                break;
            case Strings.TROLL_ROAR:
                if (isTrollRoarValid()) {
                    trollRoar();
                    validMove = true;
                }
                break;
            case Strings.ELF_TELEPORTATION:
                if (isElfTeleportationValid(deltaRequestJson)) {
                    elfTeleportation(deltaRequestJson);
                    validMove = true;
                }
                break;
            case Strings.GOBLIN_SHOCK:
                if (isGoblinShockValid(client, deltaRequestJson)) {
                    goblinShock(deltaRequestJson);
                    validMove = true;
                }
                break;//
            case Strings.WOMBAT_POO:
                if(isWombatPooValid(deltaRequestJson)){
                    wombatPoo(deltaRequestJson);
                    validMove=true;
                }
                break; //3333
            case Strings.MOVE:
                if (isMoveValid(deltaRequestJson)) {
                    move(client, deltaRequestJson);
                    validMove = true;
                    deltaIsMove = true;
                }
                break;
            case Strings.WREST_QUAFFLE:
                if (isWrestQuaffleValid(deltaRequestJson)) {
                    wrestQuaffle(deltaRequestJson);
                    validMove = true;
                }
                break;
            case Strings.SKIP:
                if (isSkipValid(deltaRequestJson)) {
                    skip(client);
                    validMove = true;
                }
                break;
            case Strings.UNBAN:
                if (isUnbanValid(client, deltaRequestJson)) {
                    unban(deltaRequestJson);
                    validMove = true;
                }
                break;
            default:
                Logger.warn("DeltaRequest from client " + client.name + " was not found.");
                //ProtocolUtility.kickClient(client, "DeltaRequest was not found.");
        }

        if (!deltaIsMove) {
            moveQuaffleFromGoalPost();
        }

        if (validMove && !deltaRequestJson.deltaType.equals(Strings.UNBAN)) {
            //do all the stuff you need to do after each deltaRequest (calculation, next, ...)
            //Except for unbans
            calculateAndSendNext();
        } else if (!validMove) {
            //kick client
            //ProtocolUtility.kickClient(client, "Invalid delta!");
            Logger.warn("Client " + client.name + " send invalid delta");
        }

        Logger.debug("WAITING FOR NEW DELTA\n\n");
    }

    //validates wombat poo
    private boolean isWombatPooValid(DeltaRequestJson deltaRequestJson){
    Square poocube=new Square(deltaRequestJson.xPosNew,deltaRequestJson.yPosNew);
        if(!isSquareOnPitch(poocube)){
            Logger.warn("Is not a valid position for "+Strings.WOMBAT_POO);
            return false;
        }
        if(isGoalPostSquare(poocube)){
            Logger.warn("Is not a valid position for "+Strings.WOMBAT_POO);
            return false;
        }
        if(!isSquareFree(poocube)){
            Logger.warn("Is not a valid position for "+Strings.WOMBAT_POO);
            return false;
        }

        return true;
    }

    private void wombatPoo(DeltaRequestJson deltaRequestJson){
        Square poocube=new Square(deltaRequestJson.xPosNew,deltaRequestJson.yPosNew);
        Fan wombat = (Fan) game.activeEntity;
        game.wombatPoos.add(poocube);
        sendWombatPooBroadcast(wombat, poocube);

        if (isProbabilitySuccessful(Strings.WOMBAT_POO)) {
            //foul detected
            wombat.ban();

            sendBanBroadcast(wombat, Strings.BAN_WOMBAT_POO);
        }
    }
    /**
     * Finishes the game by updating the Game and Clients. Then a matchFinish message is sent to
     * the Clients
     *
     * @param winningClient     The Client that won the game
     * @param victoryReasonType The type of the victory. Can be one of the following:
     *                          disqualification
     *                          bothDisqualificationMostPoints
     *                          bothDisqualificationPointsEqualSnitchCatch
     *                          bothDisqualificationPointsEqualLastDisqualification
     *                          mostPoints
     *                          pointsEqualSnitchCatch
     *                          violationOfProtocol
     */
    private void matchFinish(Client winningClient, String victoryReasonType) {
        game.inGame = false;
        game.getLeftClient().state = Client.GAME_OVER;
        game.getLeftClient().state = Client.GAME_OVER;

        sendMatchFinishBroadcast(winningClient, victoryReasonType);
    }

    /**
     * Finishes the game by determining the Client that won and the reason for the victory.
     */
    private void matchFinish() {
        Client rightClient = game.getRightClient();
        Client leftClient = game.getLeftClient();

        if (rightClient.disqualified && !leftClient.disqualified) {
            //right client is disqualified
            matchFinish(leftClient, Strings.DISQUALIFICATION);

        } else if (!rightClient.disqualified && leftClient.disqualified) {
            //left client is disqualified
            matchFinish(rightClient, Strings.DISQUALIFICATION);

        } else if (rightClient.disqualified && leftClient.disqualified) {
            //both clients disqualified

            if (leftClient.team.points > rightClient.team.points) {
                //left client has more points and both dq
                matchFinish(leftClient, Strings.BOTH_DISQUALIFICATION_MOST_POINTS);

            } else if (leftClient.team.points < rightClient.team.points) {
                //right client has more points and both dq
                matchFinish(rightClient, Strings.BOTH_DISQUALIFICATION_MOST_POINTS);

            } else {
                //both clients have the same score and both dq

                if (leftClient.team.caughtSnitch) {
                    //left clients team caught the snitch and both dq and same score
                    matchFinish(leftClient, Strings.BOTH_DISQUALIFICATION_POINTS_EQUAL_SNITCH_CATCH);

                } else if (rightClient.team.caughtSnitch) {
                    //right clients team caught the snitch and both dq and same score
                    matchFinish(rightClient, Strings.BOTH_DISQUALIFICATION_POINTS_EQUAL_SNITCH_CATCH);

                } else {
                    //no one caught the snitch and both dq and same score

                    Client lastBannedClient = game.getClientFromEntity(game.getLastBannedPlayer());

                    matchFinish(lastBannedClient, Strings.BOTH_DISQUALIFICATION_POINTS_EQUAL_LAST_DISQUALIFICATION);
                }
            }
        } else {
            //no client disqualified

            if (leftClient.team.points > rightClient.team.points) {
                //left client has more points
                matchFinish(leftClient, Strings.MOST_POINTS);

            } else if (leftClient.team.points < rightClient.team.points) {
                //right client has more points
                matchFinish(rightClient, Strings.MOST_POINTS);

            } else {
                //both clients have the same score

                if (leftClient.team.caughtSnitch) {
                    //left clients team caught the snitch
                    matchFinish(leftClient, Strings.POINTS_EQUAL_SNITCH_CATCH);

                } else if (rightClient.team.caughtSnitch) {
                    //right clients team caught the snitch
                    matchFinish(rightClient, Strings.POINTS_EQUAL_SNITCH_CATCH);

                } else {
                    //THIS SHOULD NOT HAPPEN
                    Logger.warn("This should not happen");
                }

            }
        }
    }

    /**
     * Sends a matchFinish message to all Clients in the Game. Also adds the broadcast to the
     * replay.
     *
     * @param winningClient    The Client that won the match
     * @param vicoryReasonType The reason for the victory. Can be one of the following:
     *                         disqualification
     *                         bothDisqualificationMostPoints
     *                         bothDisqualificationPointsEqualSnitchCatch
     *                         bothDisqualificationPointsEqualLastDisqualification
     *                         mostPoints
     *                         pointsEqualSnitchCatch
     *                         violationOfProtocol
     */
    private void sendMatchFinishBroadcast(Client winningClient, String vicoryReasonType) {
        MatchFinishJson matchFinishJson = new MatchFinishJson(
                game.roundCounter,
                game.getLeftClient().team.points,
                game.getRightClient().team.points,
                winningClient.name,
                vicoryReasonType
        );

        ProtocolUtility.broadcastPayload(game, matchFinishJson);
        saveToReplay(matchFinishJson);
    }

    /**
     * Moves the quaffle from the goal post to either the defending Keeper if he is in his keeper
     * zone or to the middle square.
     */
    private void moveQuaffleFromGoalPost() {
        Logger.trace("Moving quaffle from goal post (if needed)");
        Ball quaffle = game.getBall(Strings.QUAFFLE);

        Player keeper;
        if (isRightGoalPostSquare(quaffle.getPos())) {
            //get the keeper
            keeper = game.getPlayer(Strings.RIGHT_KEEPER);

        } else if (isLeftGoalPostSquare(quaffle.getPos())) {
            keeper = game.getPlayer(Strings.LEFT_KEEPER);
        } else {
            //not on goal post
            return;
        }

        if (isKeeperInHisKeeperZone(keeper)) {
            Square oldPos = quaffle.getPos();
            quaffle.setPos(keeper);
            sendMoveBroadcast(quaffle, oldPos);
            checkGoalThrow(oldPos);
        } else {
            Square oldPos = quaffle.getPos();
            quaffle.setPos(Config.MIDDLE_SQUARE);
            sendMoveBroadcast(quaffle, oldPos);
            checkGoalThrow(oldPos);
        }
    }

    /**
     * Checks if the quaffle moved on a goal post square on a valid vector and sends according
     * messages. If the goal throw was valid and there is no defending player on the goal post
     * square, a goal is counted and the right messages are broadcasted.
     *
     * @param oldPos The old position of the quaffle
     */
    private void checkGoalThrow(Square oldPos) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        //is the new square a goal post square?
        if (!isGoalPostSquare(quaffle.getPos())) {
            return;
        }

        //the new square is a goal post square

        //is there a player on the goal post square?
        Player onNewSquare = getPlayer(quaffle.getPos());

        if (onNewSquare != null) {
            //there is a player on the goal post square

            //is the player a defending player?
            Client clientOfPlayer = game.getClientFromEntity(onNewSquare);

            if ((clientOfPlayer == game.getRightClient() && isRightGoalPostSquare(quaffle.getPos())) ||
                    clientOfPlayer == game.getLeftClient() && isLeftGoalPostSquare(quaffle.getPos())) {
                //there is a defending player on the goal post square
                //-> the quaffle is blocked (flacking) and gets moved

                //move quaffle to adjacent square
                Square secondOldPos = quaffle.getPos();

                quaffle.setPos(getRandomFreeAdjacentSquare(quaffle.getPos()));

                sendMoveBroadcast(quaffle, secondOldPos);

                //this is flacking
                if (isProbabilitySuccessful(Strings.FLACKING)) {
                    //foul was detected
                    game.ban(onNewSquare);

                    sendBanBroadcast(onNewSquare, Strings.BAN_FLACKING);

                    if(game.getLeftClient().disqualified||game.getRightClient().disqualified){matchFinish();}
                }

                //we did every action we need to do. Done.
                return;
            }
            //the player is not a defending player
            //-> this is just handled like a normal goal throw

        }

        //there is either a player on the goal post square that is not defending or the goal post
        // square does not have a player on it.

        if (isValidGoalThrow(oldPos, quaffle.getPos())) {
            //it is a valid goal throw.
            //-> goal

            //increase the points of the corresponding team
            if (isLeftGoalPostSquare(quaffle.getPos())) {
                game.getLeftClient().team.points += Config.GOAL_POINTS;
            } else {
                game.getRightClient().team.points += Config.GOAL_POINTS;
            }

            game.goalThisRound();
            sendGoalPointsChangeBroadcast();
        }

        //it is not a valid goal throw. Do nothing
    }

    /**
     * executes appropriate action for incoming paused request
     *
     * @param client           that requested the paused
     * @param pauseRequestJson the request in json format
     */
    public void onPauseRequestMessageReceived(Client client, PauseRequestJson pauseRequestJson) {
        Logger.info("Pause message received from " + client.name + " with message " + pauseRequestJson.message);

        if (!client.isSpectator) {
            //client is not a spectator
            if (!game.paused) {
                game.paused = true;
                timerTask.cancel();
                sendPauseResponse(pauseRequestJson.message,client);
            } else {
                Logger.warn(client.name + " sent a pause request even though the game is paused already");
            }
        } else {
            //just ignore
            Logger.warn("client " + client.name + " is not a spectator but sent a pause request");
        }
    }

    /**
     * executes appropriate action for incoming continue request
     *
     * @param client              that requested the continuation of the game
     * @param continueRequestJson the request in json format
     */
    public void onContinueRequestMessageReceived(Client client, ContinueRequestJson continueRequestJson) {
        Logger.info("Continue message received from " + client.name + " with message " + continueRequestJson.message);
        if (!client.isSpectator) {
            //client is not a spectator
            if (game.paused) {
                game.paused = false;

                startTimer(game.matchConfig.timings.get(Strings.PLAYER_TURN_TIMEOUT) + Config.TIMEOUT_BUFFER);
                sendPauseResponse(continueRequestJson.message,client);
            } else {
                Logger.warn(client.name + " sent a continue request even though the game is not paused");
            }
        } else {
            //just ignore
            Logger.warn("client " + client.name + " is not a spectator but sent a continue request");
        }
    }

    /**
     * Is called when the game starts. Places the balls on the pitch and starts the first round.
     */
    public void onGameStart() {

        game.start();

        //start new round
        newRound();
    }


    /*
    isValid methods and action methods (e.g.: isUnbanValid() & unban())
    ####################################
     */

    /**
     * Checks if a deltaRequest of type bludgerBeating is valid
     *
     * @param client           Client that sent the deltaRequest
     * @param deltaRequestJson DeltaRequest that was sent by the client
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isBludgerBeatingValid(Client client, DeltaRequestJson deltaRequestJson) {
        Square squareToMoveBludgerTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Player beatingPlayer = game.getPlayer(deltaRequestJson.activeEntity);
        Ball bludger = game.getBall(deltaRequestJson.passiveEntity);

        if (squareToMoveBludgerTo.hasNullPos()) {
            Logger.warn(client.name + " tried to move bludger to a square with a null value");
            return false;
        }

        //activeEntity is activeEntity?
        if (!deltaRequestJson.activeEntity.equals(game.activeEntity.id)) {
            Logger.warn("tried to do an action on a player that was not active");
            Logger.trace("active entity: " + game.activeEntity);
            return false;
        }

        //beater is supposed to do an action?
        if (!game.activeTurnType.equals(Strings.MOVE)) {//?
            Logger.warn("sent wrong delta for active turn type");
            return false;
        }

        //new position more than 3 distance away from old position?
        if (calculateDistance(bludger.getPos(), squareToMoveBludgerTo) > 3) {
            Logger.warn("tried to beat bludger farther than 3 distance");
            return false;
        }

        //new position not on field?
        if (!isSquareOnPitch(squareToMoveBludgerTo)) {
            Logger.warn("tried to hit bludger out of pitch");
            return false;
        }

        //all overpassed fields are empty?
        for (Square s : getOverpassedSquares(beatingPlayer.getPos(), bludger.getPos())) {
            if (!isSquareFree(s)) {
                Logger.warn("tried to beat a bludger over an occupied square: " + s);
                return false;
            }
        }

        //lookin good
        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type bludgerBeating.
     *
     * @param client           Client that sent the deltaRequest
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     */
    private void bludgerBeating(Client client, DeltaRequestJson deltaRequestJson) {
        Square squareToMoveBludgerTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Player beatingPlayer = game.getPlayer(deltaRequestJson.activeEntity);
        Ball bludger = game.getBall(deltaRequestJson.passiveEntity);

        bludger.setPos(squareToMoveBludgerTo);

        Player playerOnDestination = this.getPlayer(squareToMoveBludgerTo);


        if (playerOnDestination == null || playerOnDestination instanceof Beater) {

            //no player (except for beaters) on new square of bludger
            sendBludgerBeatingBroadcast(beatingPlayer, bludger);
        } else {
            //chaser, keeper, seeker on new square of bludger
            boolean knockout = isProbabilitySuccessful(Strings.KNOCK_OUT);

            if (knockout) {
                //player gets knocked out and ball looks for a random free square on entire field and moves there
                playerOnDestination.knockedOut = true;
                sendBludgerBeatingBroadcast(beatingPlayer, bludger);

                //calculate new bludger position
                bludger.setPos(getRandomFreeSquareOnPitch());
                sendBludgerKnockoutBroadcast(bludger, playerOnDestination, true);

                //we need to recalculate the turnList starting with the other client.
                createTurnList(getOtherClient(client));

            } else {
                sendBludgerBeatingBroadcast(beatingPlayer, bludger);
                sendBludgerKnockoutBroadcast(bludger, playerOnDestination, false);
            }
        }
    }

    /**
     * Checks if a deltaRequest of type quaffleThrow is valid
     *
     * @param client           Client that sent the deltaRequest
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isQuaffleThrowValid(Client client, DeltaRequestJson deltaRequestJson) {
        Square throwTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        if (throwTo.hasNullPos()) {
            Logger.warn(client.name + " tried to throw a quaffle with a null value");
            return false;
        }

        //activeEntity is activeEntity?
        if (!deltaRequestJson.activeEntity.equals(game.activeEntity.id)) {
            Logger.warn("tried to do an action on a player that was not active");
            Logger.trace("active entity: " + game.activeEntity);
            return false;
        }
        //thrower is supposed to do an action?
        if (!game.activeTurnType.equals(Strings.MOVE)) {
            Logger.warn("sent wrong delta for active turn type");
            return false;
        }

        //check if new square is still on pitch
        if (!isSquareOnPitch(throwTo)) {
            Logger.warn("tried to throw the quaffle out of the pitch");
            return false;
        }

        //lookin good
        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type quaffleThrow.
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     */
    private void quaffleThrow(DeltaRequestJson deltaRequestJson) {
        Square throwTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        Player thrower = game.getPlayer(deltaRequestJson.activeEntity);

        //check if the quaffle gets intercepted by another player
        Player interceptor = isQuaffleIntercepted(quaffle.getPos(), throwTo);

        if (interceptor != null) {
            //quaffle was intercepted
            quaffle.setPos(interceptor);
            sendQuaffleThrowBroadcast(thrower, interceptor, false);
        } else {
            //quaffle was not intercepted

            if (Math.random() < Math.pow(game.matchConfig.probabilities.get(Strings.THROW_SUCCESS), (calculateDistance(throwTo, quaffle.getPos())))) {
                //throw successful
                quaffle.setPos(throwTo);
                sendQuaffleThrowBroadcast(thrower, null, true);

            } else {
                //throw was not successful

                int n = (int) Math.ceil(calculateDistance(throwTo, quaffle.getPos()) / 7.0f);
                Square newToSquare = getRandomFreeSquareAround(throwTo, n);

                quaffle.setPos(newToSquare);
                sendQuaffleThrowBroadcast(thrower, null, false);
            }

        }
        checkGoalThrow(thrower.getPos());
    }

    /**
     * Checks if a deltaRequest of type snitchSnatch is valid
     *
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isSnitchSnatchValid() {
        if (!(game.activeEntity instanceof Fan)) {
            Logger.warn(" tried to snitch a snatch with a not fan");
            return false;
        } else {
            Fan activeFan = (Fan) game.activeEntity;
            if (!activeFan.type.equals(Strings.NIFFLER)) {
                Logger.warn("tried to snitch a snatch with a fan that is not a niffler");
                return false;
            }
        }
        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type snitchSnatch.
     */
    private void snitchSnatch() {
        Fan niffler = (Fan) game.activeEntity;
        Ball snitch = game.getBall(Strings.SNITCH);

        if (game.overtime == Game.NO_OVERTIME && !snitch.getPos().hasNullPos()) {
            Square moveTo = getRandomFreeAdjacentSquare(snitch.getPos());
            Square oldPos = snitch.getPos();

            snitch.setPos(moveTo);

            sendSnitchSnatchBroadcast(niffler, oldPos);
        }

        if (isProbabilitySuccessful(Strings.SNITCH_SNATCH)) {
            //foul detected
            niffler.ban();

            sendBanBroadcast(niffler, Strings.BAN_SNITCH_SNATCH);
        }
    }

    /**
     * Checks if a deltaRequest of type trollRoar is valid
     *
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isTrollRoarValid() {
        if (!(game.activeEntity instanceof Fan)) {
            Logger.warn("tried to roar with an entity that is not a fan");
            return false;
        } else {
            Fan activeFan = (Fan) game.activeEntity;
            if (!activeFan.type.equals(Strings.TROLL)) {
                Logger.warn("tried to roar with a fan that is not a troll");
                return false;
            }
        }
        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type trollRoar.
     */
    private void trollRoar() {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        Fan troll = (Fan) game.activeEntity;
        Square oldPos = quaffle.getPos();
        Player playerOnQuaffle = getPlayer(quaffle.getPos());

        if (playerOnQuaffle != null) {
            //no player holds quaffle

            quaffle.setPos(getRandomFreeAdjacentSquare(quaffle.getPos()));
        }

        sendTrollRoarBroadcast(troll, oldPos, playerOnQuaffle);

        if (isProbabilitySuccessful(Strings.TROLL_ROAR)) {
            //foul detected
            troll.ban();

            sendBanBroadcast(troll, Strings.BAN_TROLL_ROAR);
        }
    }

    /**
     * Checks if a deltaRequest of type elfTeleportation is valid
     *
     * @param deltaRequestJson DeltaRequest that was sent by the client
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isElfTeleportationValid(DeltaRequestJson deltaRequestJson) {
        Player teleportedPlayer = game.getPlayer(deltaRequestJson.passiveEntity);

        if (!(game.activeEntity instanceof Fan)) {
            Logger.warn("tried to teleport with an entity that is not a fan");
            return false;
        } else {
            Fan activeFan = (Fan) game.activeEntity;
            if (!activeFan.type.equals(Strings.ELF)) {
                Logger.warn("tried to teleport with a fan that is not an elf");
                return false;
            }
        }

        if (teleportedPlayer == null) {
            Logger.warn("tried to teleport null");
            return false;
        }

        if (teleportedPlayer.isBanned()) {
            Logger.warn("tried to teleport a banned player");
            return false;
        }

        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type elfTeleportation
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     */
    private void elfTeleportation(DeltaRequestJson deltaRequestJson) {
        Player teleportedPlayer = game.getPlayer(deltaRequestJson.passiveEntity);
        Fan elf = (Fan) game.activeEntity;
        Square oldPos = teleportedPlayer.getPos();

        teleportedPlayer.setPos(getRandomFreeSquareOnPitch());

        Square pos= teleportedPlayer.getPos();
        for (Square s: game.getAllPoos()) {
            if(s.equals(pos)){
                sendRemovePooBroadcast(teleportedPlayer,pos);
                game.wombatPoos.remove(s);
            }
        }


        sendElfTeleportationBroadcast(elf, teleportedPlayer, oldPos);

        //Ball quaffle = game.getBall(Strings.QUAFFLE);
        //quaffle.setPos(teleportedPlayer.getPos());
        //sendMoveBroadcast(quaffle, oldPos);

        checkGoalThrow(oldPos);

        if (isProbabilitySuccessful(Strings.ELF_TELEPORTATION)) {
            //foul detected
            elf.ban();

            sendBanBroadcast(elf, Strings.BAN_ELF_TELEPORTATION);
        }
    }

    /**
     * Checks if a deltaRequest of type goblinShock is valid
     *
     * @param client           Client that sent the deltaRequest
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isGoblinShockValid(Client client, DeltaRequestJson deltaRequestJson) {
        Player shockedPlayer = game.getPlayer(deltaRequestJson.passiveEntity);

        if (!(game.activeEntity instanceof Fan)) {
            Logger.warn("active entity is not fan");
            return false;
        } else {
            Fan activeFan = (Fan) game.activeEntity;
            if (!activeFan.type.equals(Strings.GOBLIN)) {
                Logger.warn("active entity is not goblin");
                return false;
            }
        }

        if (shockedPlayer == null) {
            Logger.warn("there is no shocked player");
            return false;
        }

        if (shockedPlayer.isBanned()) {
            Logger.warn("the shocked player is banned");
            return false;
        }

        if (game.getClientFromEntity(shockedPlayer) == client) {
            Logger.warn("the shocked player is from the same team");
            return false;
        }

        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type goblinShock
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     */
    private void goblinShock(DeltaRequestJson deltaRequestJson) {
        Player shockedPlayer = game.getPlayer(deltaRequestJson.passiveEntity);
        Fan goblin = (Fan) game.activeEntity;
        Ball quaffle = getQuaffle(shockedPlayer.getPos());

        if (quaffle != null) {
            //the shocked player holds the quaffle

            quaffle.setPos(getRandomFreeAdjacentSquare(quaffle.getPos()));

            sendFoolAwayBroadcast(shockedPlayer);
            checkGoalThrow(shockedPlayer.getPos());
        }

        Square oldPos = shockedPlayer.getPos();
        shockedPlayer.setPos(getRandomFreeAdjacentSquare(shockedPlayer.getPos()));
        sendGoblinShockBroadcast(goblin, shockedPlayer, oldPos);

        if (isProbabilitySuccessful(Strings.GOBLIN_SHOCK)) {
            //foul detected
            goblin.ban();

            sendBanBroadcast(goblin, Strings.BAN_GOBLIN_SHOCK);
        }
    }

    /**
     * Checks if a deltaRequest of type move is valid
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     * @return true if the deltaRequest is valid, false if it isn't
     */
    private boolean isMoveValid(DeltaRequestJson deltaRequestJson) {
        Logger.trace("Checking if move is valid.");
        Player player = game.getPlayer(deltaRequestJson.activeEntity);
        Square moveTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);

        if (moveTo.hasNullPos()) {
            Logger.warn("tried to move somewhere with a null value");
            return false;
        }

        if (player == null) {
            Logger.warn("tried to move with a null player");
            return false;
        }

        if (!(player == game.activeEntity)) {
            Logger.warn("tried to move with a player is not active entity");
            return false;
        }

        if (!(game.activeTurnType.equals(Strings.MOVE))) {
            Logger.warn("wrong delta for active turn type");
            return false;
        }

        if (!isSquareOnPitch(moveTo)) {
            Logger.warn("tried to move off pitch");
            return false;
        }

        if (calculateDistance(player.getPos(), moveTo) > 1) {
            Logger.warn("tried to move more than 1 distance");
            return false;
        }
        if(wombatPooedField(moveTo)){
            Logger.warn("tried to move on wombatPoo!");
            return false;
        }

        Logger.trace("move is valid.");
        return true;
    }

    /**
     * Handles the responses to a deltaRequest of type move.
     *
     * @param client           Client that sent the deltaRequest.
     * @param deltaRequestJson DeltaRequest that was sent by the Client.
     */
    private void move(Client client, DeltaRequestJson deltaRequestJson) {
        Logger.trace("Starting move.");

        boolean playerHasQuaffle = false;
        String banReason = null;
        Player player = game.getPlayer(deltaRequestJson.activeEntity);
        Square moveTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Player playerOnMoveTo = getPlayer(moveTo);


        //move the player
        Square oldPos = player.getPos();
        player.setPos(moveTo);

        sendMoveBroadcast(player, oldPos);

        //foul blatching if player moves on the same square as someone else. chaser/keeper looses quaffle
        if (playerOnMoveTo != null && player.id != playerOnMoveTo.id) {
            //found another player, where the moving player is moving

            Ball quaffleOnMoveTo = getQuaffle(moveTo);
            if (quaffleOnMoveTo != null) {
                //fool away quaffle

                quaffleOnMoveTo.setPos(getRandomFreeAdjacentSquare(quaffleOnMoveTo.getPos()));

                sendFoolAwayBroadcast(playerOnMoveTo);
                checkGoalThrow(playerOnMoveTo.getPos());
            }

            //player gets blatched
            Square oldPosPlayerOnMoveTo = playerOnMoveTo.getPos();
            playerOnMoveTo.setPos(getRandomFreeAdjacentSquare(playerOnMoveTo.getPos()));

            sendMoveBroadcast(playerOnMoveTo, oldPosPlayerOnMoveTo);


            if (isProbabilitySuccessful(Strings.BLATCHING)) {
                banReason = Strings.BAN_BLATCHING;
            }

        }


        //move the quaffle (if needed)
        Ball quaffle = getQuaffle(oldPos);
        if (isPlayerChaserOrKeeper(player) && quaffle != null) {
            playerHasQuaffle = true;
            quaffle.setPos(moveTo);

            sendMoveBroadcast(quaffle, oldPos);
        }

        //flacking if defending player moves on a goalpostsquare
        if (client == game.getLeftClient()) {
            if (isLeftGoalPostSquare(moveTo)) {
                //left player moved on left goal post square
                if (banReason == null) {
                    if (isProbabilitySuccessful(Strings.FLACKING)) {
                        banReason = Strings.BAN_FLACKING;
                    }
                }
            }
        } else if (client == game.getRightClient()) {
            if (isRightGoalPostSquare(moveTo)) {
                //right player moved on right goal post square
                if (banReason == null) {
                    if (isProbabilitySuccessful(Strings.FLACKING)) {
                        banReason = Strings.BAN_FLACKING;
                    }
                }
            }
        }

        //check for snitch catch
        Ball snitch = getSnitch(moveTo);
        if (snitch != null) {
            //there is a snitch on the square

            if (!(player instanceof Seeker)) {
                //player is not a seeker -> snitchnip
                if (banReason == null) {
                    if (isProbabilitySuccessful(Strings.BAN_SNITCHNIP)) {
                        banReason = Strings.BAN_SNITCHNIP;
                    }
                }
            } else {
                //player is a seeker
                boolean successfullSnitchCatch = isProbabilitySuccessful(Strings.CATCH_SNITCH);

                if (!(game.overtime == Game.NO_OVERTIME)) {
                    successfullSnitchCatch = true;
                }

                game.getClientFromEntity(player).team.points += Config.SNITCH_POINTS;
                sendSnitchCatchBroadcast(player, successfullSnitchCatch);

                //the game is immediately over
                client.team.caughtSnitch = true;
                matchFinish();
            }
        }


        //check for haversacking (chaser moves on a goal post with a quaffle)
        if (player instanceof Chaser && isGoalPostSquare(moveTo) && playerHasQuaffle) {
            //haversacking

            //the player scores
            if (isLeftGoalPostSquare(moveTo)) {
                game.getLeftClient().team.points += Config.GOAL_POINTS;
            } else {
                game.getRightClient().team.points += Config.GOAL_POINTS;
            }

            game.goalThisRound();
            sendGoalPointsChangeBroadcast();

            if (banReason == null) {
                if (isProbabilitySuccessful(Strings.BAN_HAVERSACKING)) {
                    banReason = Strings.BAN_HAVERSACKING;
                }
            }
        }


        //check for stooging (more than one chaser in the enemy keeper zone)
        if (player instanceof Chaser) {
            if (client == game.getRightClient()) {
                if (isPlayerInLeftKeeperZone(player)) {
                    //the player moved in the enemy keeper zone
                    if (countEnemyChasersInLeftKeeperZone() > 1) {
                        //STOOGING!
                        if (banReason == null) {
                            if (isProbabilitySuccessful(Strings.BAN_STOOGING)) {
                                banReason = Strings.BAN_STOOGING;
                            }
                        }
                    }
                }
            } else {
                if (isPlayerInRightKeeperZone(player)) {
                    //the player moved in the enemy keeper zone
                    if (countEnemyChasersInRightKeeperZone() > 1) {
                        //STOOGING!
                        if (banReason == null) {
                            if (isProbabilitySuccessful(Strings.BAN_STOOGING)) {
                                banReason = Strings.BAN_STOOGING;
                            }
                        }
                    }
                }
            }
        }

        //ban the player if needed
        if (banReason != null) {
            game.ban(player);
            sendBanBroadcast(player, banReason);
            if(game.getLeftClient().disqualified||game.getRightClient().disqualified){matchFinish();}
        }

        Logger.trace("Player move finished. Player moved to: " + player.getPos());
    }

    /**
     * Counts the amount of enemy chasers (chasers of the right team) in the left keeper zone.
     *
     * @return The amount of enemy chasers in the left keeper zone.
     */
    private int countEnemyChasersInLeftKeeperZone() {
        int counter = 0;

        for (Player p : game.getRightClient().team.players) {
            if(p.getPos().x!=null&&p.getPos().y!=null){
                if (p instanceof Chaser && isPlayerInLeftKeeperZone(p)) {
                    counter++;
                }
            }
            else{continue;}
        }
        return counter;
    }

    /**
     * Counts the amount of enemy chasers (chasers of the left team) in the right keeper zone.
     *
     * @return The amount of enemy chasers in the right keeper zone.
     */
    private int countEnemyChasersInRightKeeperZone() {
        int counter = 0;
        for (Player p : game.getLeftClient().team.players) {
            if(p.getPos().x!=null&&p.getPos().y!=null){
                if (p instanceof Chaser && isPlayerInRightKeeperZone(p)) {
                    counter++;
                }
            }
            else{
            continue;
            }
        }
        return counter;
    }

    /**
     * Checks if a deltaRequest of type wrestQuaffle is valid.
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client
     * @return True if the deltaRequest is valid. False otherwise.
     */
    private boolean isWrestQuaffleValid(DeltaRequestJson deltaRequestJson) {
        Player wrester = game.getPlayer(deltaRequestJson.activeEntity);

        if (wrester == null) {
            Logger.warn("tried to wrest with a null player");
            return false;
        }

        if (wrester != game.activeEntity) {
            Logger.warn("tried to wrest with a not activeEntity");
            return false;
        }

        if (!(game.activeTurnType.equals(Strings.MOVE))) {
            Logger.warn("wrong delta for active turn type");
            return false;
        }

        if (!(wrester instanceof Chaser)) {
            Logger.warn("tried to wrest with a player that is not a chaser");
            return false;
        }

        if (!canChaserWrestQuaffle(wrester)) {
            Logger.warn("tried to wrest with a chaser that cannot wrest at the current time");
            return false;
        }

        return true;
    }


    /**
     * Handles the responses to a deltaRequest of type wrestQuaffle.
     *
     * @param deltaRequestJson DeltaRequest that was sent by the Client.
     */
    private void wrestQuaffle(DeltaRequestJson deltaRequestJson) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        Player wrester = game.getPlayer(deltaRequestJson.activeEntity);
        Player wrested = getPlayer(quaffle.getPos());

        boolean successful = isProbabilitySuccessful(Strings.WREST_QUAFFLE);

        if (successful) {
            quaffle.setPos(wrester.getPos());
        }

        if (wrested == null) {
            Logger.warn("This should not happen. Wrester is null on pos: " + quaffle.getPos());
        }

        sendWrestQuaffleBroadcast(wrester, wrested, successful);
    }

    /**
     * Checks if a skip request is valid.
     *
     * @param deltaRequestJson to check for validity
     * @return True if the request is valid. False otherwise.
     */
    private boolean isSkipValid(DeltaRequestJson deltaRequestJson) {

        if (!game.activeEntity.id.equals(deltaRequestJson.activeEntity)) {
            Logger.warn("tried to skip with an entity that is not the activeEntity");
            return false;
        }
        return true;
    }

    /**
     * Figures out what to do after a skip request.
     *
     * @param client that send the skip request.
     */
    private void skip(Client client) {
        sendSkipBroadcast(game.activeEntity);

        if(game.activeTurnType!=null){
        //if we skip the move, we want to skip the second move too, so we pretend we did the first move before
        if (game.activeTurnType.equals(Strings.MOVE)) {
            ((Player) game.activeEntity).hadFirstMove = true;
        } else if (game.activeTurnType.equals(Strings.REMOVE_BAN)) {
            //we are skipping an unban

            Player player = (Player) game.activeEntity;

            //place the player on a random free square on the corresponding half of the pitch
            if (client == game.getRightClient()) {
                player.unban(getRandomFreeSquareInRightHalf());
                game.removeFromUnban(player);
            } else {
                player.unban(getRandomFreeSquareInLeftHalf());
                game.removeFromUnban(player);
            }

            sendUnbanBroadcast(player);
        }
        }
    }

    /**
     * Gets a random free square in the right half of the pitch.
     *
     * @return a random free square in the right half of the pitch.
     */
    private Square getRandomFreeSquareInRightHalf() {
        while (true) {
            int x = (int) (Math.random() * 8) + 9;
            int y = (int) (Math.random() * 13);
            Square nextSquare = new Square(x, y);
            if (isSquareOnPitch(nextSquare) && isSquareFree(nextSquare)) {
                return nextSquare;
            }
        }
    }

    /**
     * Gets a random free square in the left half of the pitch.
     *
     * @return a random free square in the left half of the pitch.
     */
    private Square getRandomFreeSquareInLeftHalf() {
        while (true) {
            int x = (int) (Math.random() * 8);
            int y = (int) (Math.random() * 13);
            Square nextSquare = new Square(x, y);
            if (isSquareOnPitch(nextSquare) && isSquareFree(nextSquare)) {
                return nextSquare;
            }
        }
    }

    /**
     * Checks if incoming unban request is valid.
     *
     * @param client           that requested the unbane
     * @param deltaRequestJson the request in json format
     * @return True if the unban is valid. False otherwise.
     */
    private boolean isUnbanValid(Client client, DeltaRequestJson deltaRequestJson) {
        Square moveTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Player player = game.getPlayer(deltaRequestJson.activeEntity);

        if (moveTo.hasNullPos()) {
            Logger.warn("sent null pos.");
            return false;
        }

        if (player == null) {
            Logger.warn("sent invalid player.");
            return false;
        }

        if (player != game.activeEntity) {
            Logger.warn("sent wrong active entity.");
            return false;
        }

        if (!(game.activeTurnType.equals(Strings.REMOVE_BAN))) {
            Logger.warn("sent wrong delta for active turn type");
            return false;
        }

        //position is not on pitch
        if (!isSquareOnPitch(moveTo)) {
            Logger.warn("sent new player position that is not on the pitch.");
            return false;
        }

        //new position is not free
        if (!isSquareFree(moveTo)) {
            Logger.warn("sent square that was not free");
            return false;
        }

        //square is on correct pitch side
        if (client == game.getRightClient()) {
            //client is right client
            //not on right side
            return moveTo.x > 8;
        } else {
            //client is left client
            //not on left side
            return moveTo.x < 8;
        }

    }

    /**
     * Sends an unban broadcast and updates the unban list.
     *
     * @param deltaRequestJson to which this method is reacting
     */
    private void unban(DeltaRequestJson deltaRequestJson) {
        Square moveTo = new Square(deltaRequestJson.xPosNew, deltaRequestJson.yPosNew);
        Player player = game.getPlayer(deltaRequestJson.activeEntity);

        player.unban(moveTo);

        sendUnbanBroadcast(player);

        updateUnbanList();
    }

    /**
     * Updates the unban list. Starts a new round if the unban list is empty.
     */
    private void updateUnbanList() {
        game.activeEntity = game.getNextUnban();

        if (game.activeEntity == null) {
            //no more unbans available

            //start new round
            newRound();
        } else {
            //send next
            game.activeTurnType = Strings.REMOVE_BAN;
            game.removeFromUnban(game.activeEntity);
            game.activeEntity.hadTurn=true;
            sendNextBroadcast(game.activeEntity, Strings.REMOVE_BAN, game.matchConfig.timings.get(Strings.PLAYER_TURN_TIMEOUT));
        }
    }

    /*
    Sender methods
    ####################################
     */

    /**
     * creates and sends a next broadcast with the given information
     *
     * @param entity The Entity that is the next at turn
     * @param type    either move or action or fan or removaBan
     * @param timeout until the client has to respond with an appropriate request
     */
    private void sendNextBroadcast(Entity entity, String type, Integer timeout) {
        NextJson nextJson = new NextJson();
        nextJson.turn = entity.id;
        nextJson.type = type;
        nextJson.timeout = timeout;
        Logger.debug("sending to clients: nextBroadcast with entity: " + entity + " type: " + type +
                " timeout: " + timeout);
        game.lastNextContainer = ProtocolUtility.broadcastPayload(game, nextJson);

        timerTask.cancel();
        startTimer(timeout + Config.TIMEOUT_BUFFER);
    }

    /**
     * Creates and sends a bludger beating broadcast with the given parameters.
     *
     * @param beater  that beat
     * @param bludger that got beaten
     */
    private void sendBludgerBeatingBroadcast(Player beater, Ball bludger) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.BLUDGER_BEATING,
                null,
                beater.getPos().x,
                beater.getPos().y,
                bludger.getPos().x,
                bludger.getPos().y,
                beater.id,
                bludger.id

        );
        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a quaffle throw broadcast with the given parameters.
     *
     * @param thrower that threw the quaffle
     * @param catcher that might have caught the quaffle
     * @param success if the quaffle didnt get caught and landed on the correct square
     */
    private void sendQuaffleThrowBroadcast(Player thrower, Player catcher, Boolean success) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        String passiveEntity;
        if (catcher == null) {
            passiveEntity = null;
        } else {
            passiveEntity = catcher.id;
        }

        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.QUAFFLE_THROW,
                success,
                thrower.getPos().x,
                thrower.getPos().y,
                quaffle.getPos().x,
                quaffle.getPos().y,
                thrower.id,
                passiveEntity
        );
        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a snitch snatch broadcast with the given parameters.
     *
     * @param niffler that snitch snatched
     * @param oldPos  of the snitch before
     */
    private void sendSnitchSnatchBroadcast(Fan niffler, Square oldPos) {
        Ball snitch = game.getBall(Strings.SNITCH);
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.SNITCH_SNATCH,
                null,
                oldPos.x,
                oldPos.y,
                snitch.getPos().x,
                snitch.getPos().y,
                niffler.id,
                snitch.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a troll roar broadcast with the given parameters.
     *
     * @param troll         that roared
     * @param oldPos        of the quaffle if it got fooled away
     * @param quaffleHolder player that held the quaffle
     */
    private void sendTrollRoarBroadcast(Fan troll, Square oldPos, Player quaffleHolder) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        String passiveEntity;
        if (quaffleHolder == null) {
            passiveEntity = null;
        } else {
            passiveEntity = quaffleHolder.id;
        }
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.TROLL_ROAR,
                null,
                oldPos.x,
                oldPos.y,
                quaffle.getPos().x,
                quaffle.getPos().y,
                troll.id,
                passiveEntity
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends an elf teleportation broadcast with the given parameters.
     *
     * @param elf        that teleportet someone
     * @param teleported the player that got teleported
     * @param oldPos     of the teleported player
     */
    private void sendElfTeleportationBroadcast(Fan elf, Player teleported, Square oldPos) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.ELF_TELEPORTATION,
                null,
                oldPos.x,
                oldPos.y,
                teleported.getPos().x,
                teleported.getPos().y,
                elf.id,
                teleported.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a goblin shock broadcast with the given parameters.
     *
     * @param goblin  that shocked someone
     * @param shocked the shocked player
     * @param oldPos  of the player before he got shocked
     */
    private void sendGoblinShockBroadcast(Fan goblin, Player shocked, Square oldPos) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.GOBLIN_SHOCK,
                null,
                oldPos.x,
                oldPos.y,
                shocked.getPos().x,
                shocked.getPos().y,
                goblin.id,
                shocked.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a move broadcast with the given parameters.
     *
     * @param entity that got moved
     * @param oldPos of the entity before it moved
     */
    private void sendMoveBroadcast(Entity entity, Square oldPos) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.MOVE,
                null,
                oldPos.x,
                oldPos.y,
                entity.getPos().x,
                entity.getPos().y,
                entity.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a wrest quaffle broadcast with the given parameters.
     *
     * @param wrester the player that wrested the quaffle away
     * @param wrested the player that got the quaffle wrested away from him
     * @param success True if the action was successful. False otherwise.
     */
    private void sendWrestQuaffleBroadcast(Player wrester, Player wrested, Boolean success) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.WREST_QUAFFLE,
                success,
                wrested.getPos().x,
                wrested.getPos().y,
                quaffle.getPos().x,
                quaffle.getPos().y,
                wrester.id,
                wrested.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a skip broadcast with the given parameter.
     *
     * @param skipped Entity that skipped its move.
     */
    private void sendSkipBroadcast(Entity skipped) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.SKIP,
                null,
                null,
                null,
                null,
                null,
                skipped.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends an unban broadcast with the given parameter.
     *
     * @param player that got unbanned.
     */
    private void sendUnbanBroadcast(Player player) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.UNBAN,
                null,
                null,
                null,
                player.getPos().x,
                player.getPos().y,
                player.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a snitch catch broadcast with the given parameters.
     *
     * @param seeker  that might have caught the snitch
     * @param success True if the seeker caught the snitch. False otherwise.
     */
    private void sendSnitchCatchBroadcast(Player seeker, boolean success) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.SNITCH_CATCH,
                success,
                null,
                null,
                null,
                null,
                seeker.id,
                null
        );
        deltaBroadcastJson.leftPoints = game.getLeftClient().team.points;
        deltaBroadcastJson.rightPoints = game.getRightClient().team.points;

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a ban broadcast with the given parameters.
     *
     * @param entity    that got banned. (player or fan)
     * @param banReason String with the ban reason
     */
    private void sendBanBroadcast(Entity entity, String banReason) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.BAN,
                null,
                null,
                null,
                null,
                null,
                null,
                entity.id
        );
        deltaBroadcastJson.leftPoints = game.getLeftClient().team.points;
        deltaBroadcastJson.rightPoints = game.getRightClient().team.points;
        deltaBroadcastJson.banReason = banReason;

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a bludger knockout broadcast with the given parameters.
     *
     * @param bludger    that got moved
     * @param knockedOut player that might have gotten knocked out
     * @param success    True if the player got knocked out. False otherwise.
     */
    private void sendBludgerKnockoutBroadcast(Ball bludger, Player knockedOut, Boolean success) {
        Integer newPosX = null;
        Integer newPosY = null;
        if (success) {
            newPosX = bludger.getPos().x;
            newPosY = bludger.getPos().y;
        }
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.BLUDGER_KNOCKOUT,
                success,
                knockedOut.getPos().x,
                knockedOut.getPos().y,
                newPosX,
                newPosY,
                bludger.id,
                knockedOut.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a fool away broadcast with the given parameter.
     *
     * @param loser Player that lost the quaffle.
     */
    private void sendFoolAwayBroadcast(Player loser) {
        Ball quaffle = game.getBall(Strings.QUAFFLE);
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.FOOL_AWAY,
                null,
                loser.getPos().x,
                loser.getPos().y,
                quaffle.getPos().x,
                quaffle.getPos().y,
                quaffle.id,
                loser.id
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a phase change broadcast with the given parameter.
     *
     * @param phaseType current phase type of the match as String
     */
    private void sendPhaseChangeBroadcast(String phaseType) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.PHASE_CHANGE,
                null,
                null,
                null,
                null,
                null,
                null,
                null
        );
        deltaBroadcastJson.phase = phaseType;

        if(phaseType.equals(Strings.FAN_PHASE))
            sendRemovePooBroadcast();


        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a goal points changed broadcast.
     */
    private void sendGoalPointsChangeBroadcast() {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.GOAL_POINTS_CHANGE,
                null,
                null,
                null,
                null,
                null,
                null,
                null
        );
        deltaBroadcastJson.leftPoints = game.getLeftClient().team.points;
        deltaBroadcastJson.rightPoints = game.getRightClient().team.points;

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a round changed broadcast.
     */
    private void sendRoundChangeBroadcast() {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.ROUND_CHANGE,
                null,
                null,
                null,
                null,
                null,
                null,
                null
        );
        deltaBroadcastJson.round = game.roundCounter;

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /**
     * Creates and sends a turn used broadcast with a given entity.
     *
     * @param entity that used its turn.
     */
    private void sendTurnUsedBroadcast(Entity entity) {
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.TURN_USED,
                null,
                null,
                null,
                null,
                null,
                entity.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    //wombatPoo broadcast
    private void sendWombatPooBroadcast(Fan wombat, Square s){
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.WOMBAT_POO,
                null,
                null,
                null,
                s.x,
                s.y,
                wombat.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);

    }

    private void sendRemovePooBroadcast(){
        game.wombatPoos.clear();
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.REMOVE_POO,
                null,
                null,
                null,
                null,
                null,
                null,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    private void sendRemovePooBroadcast(Player teleportedPlayer,Square oldPos){
        game.wombatPoos.clear();
        DeltaBroadcastJson deltaBroadcastJson = new DeltaBroadcastJson(
                Strings.REMOVE_POO,
                null,
                oldPos.x,
                oldPos.y,
                null,
                null,
                teleportedPlayer.id,
                null
        );

        Logger.debug("sending to clients: " + deltaBroadcastJson);
        //broadcast the full snapshot
        broadcastDeltaSnapshot(deltaBroadcastJson);
    }

    /*
    General helper methods
    ####################################
     */

    /**
     * Gets called by the timer after a certain amount of time. This should only happen when the client fails to send a message in time.
     */
    private void timerIsOver() {
        Logger.warn("Timer is over. Skipping entity: " + game.activeEntity);
        skip(game.getClientFromEntity(game.activeEntity));
        calculateAndSendNext();
    }

    /**
     * Starts a timer that calls timerIsOver after millis milliseconds.
     *
     * @param millis Amount of milliseconds to wait.
     */
    private void startTimer(long millis) {
        timerTask = new TimerTask() {
            @Override
            public void run() {
                timerIsOver();
            }
        };

        timer.schedule(timerTask, millis);
    }

    /**
     * Gets a random free square around a given square with the radius n.
     *
     * @param square Of which we want a free square within the radius n.
     * @param n      Radius around the given square.
     * @return A square with the above listed properties.
     */
    private Square getRandomFreeSquareAround(Square square, int n) {
        List<Square> squares = new ArrayList<>();
        for (int x = square.x - n; x < square.x + n; x++) {
            for (int y = square.y - n; y < square.y + n; y++) {
                Square curr = new Square(x, y);
                if (isSquareFree(curr) && isSquareOnPitch(curr)) {
                    squares.add(curr);
                }
            }
        }

        if (squares.isEmpty()) {
            //tunnel if necessary
            return getRandomFreeAdjacentSquare(square);
        } else {
            return getRandomSquare(squares);
        }
    }

    /**
     * Gets a free random square that is somewhere on the pitch.
     *
     * @return Free square somewhere on the pitch.
     */
    private Square getRandomFreeSquareOnPitch() {
        while (true) {
            int x = (int) (Math.random() * 17);
            int y = (int) (Math.random() * 13);
            Square nextSquare = new Square(x, y);
            if (isSquareOnPitch(nextSquare) && altIsSquareFree(nextSquare)) {
                return nextSquare;
            }
        }
    }

    /**
     * Figures the next player or fan out that should act.
     */
    private void calculateAndSendNext() {
        Logger.trace("Starting calculateAndSendNext");

        if(game.activeTurnType != null) {
            if (game.activeTurnType.equals(Strings.ACTION) || game.activeTurnType.equals(Strings.FAN)) {
                game.activeEntity.hadTurn = true;
                sendTurnUsedBroadcast(game.activeEntity);
            } else if (game.activeTurnType.equals(Strings.MOVE)&& !(game.activeEntity instanceof Fan)) {
                Player player = (Player) game.activeEntity;
                if (player.hadFirstMove) {
                    player.hadSecondMove = true;
                } else {
                    player.hadFirstMove = true;
                }
            }
        }

        //get the next possible turn type for the entity (or null if there is none))
        String nextTurnType = getNextTurnType(game.activeEntity);

        if (nextTurnType == null) {
            //the active entities turn is over

            //get the next entity
            game.activeEntity = game.getNextEntityAtTurn();

            if (game.activeEntity == null) {
                //no more entities

                if (game.state == Game.PLAYER_PHASE) {
                    //we were in the player phase
                    //there are no fans

                    game.state = Game.FAN_PHASE;
                    sendPhaseChangeBroadcast(Strings.FAN_PHASE);
                    game.actionPlayers.clear();
                }

                //fan phase is over

                //create the unban list and update it
                game.setUnbanList(createUnbanList());
                updateUnbanList();

            } else {
                //there are still entities left

                if (game.state == Game.PLAYER_PHASE && game.activeEntity instanceof Fan) {
                    //phase change

                    game.state = Game.FAN_PHASE;
                    sendPhaseChangeBroadcast(Strings.FAN_PHASE);
                    game.actionPlayers.clear();
                }

                nextTurnType = getNextTurnType(game.activeEntity);
                game.activeTurnType = nextTurnType;
                sendNextBroadcast(game.activeEntity, nextTurnType, getCurrentPhaseTimeout());
            }
        } else {
            //the active entity still has a turn to do (should be a player)
            sendNextBroadcast(game.activeEntity, nextTurnType, getCurrentPhaseTimeout());
        }


        Logger.trace("Set new activeTurnType: " + game.activeTurnType);
    }

    /**
     * Creates the unbanlist.
     *
     * @return A player list that is used to get the next player that will get unbanned.
     */
    private List<Player> createUnbanList() {
        List<Player> unbanList = new LinkedList<>();
        if (game.hasGoalOccuredThisRound()) {
            List<Player> unbanListRight = new LinkedList<>();
            List<Player> unbanListLeft = new LinkedList<>();

            for (Player p : game.getRightClient().team.getPlayers()) {
                if (p.isBanned()) {
                    unbanListRight.add(p);
                }
            }

            for (Player p : game.getLeftClient().team.getPlayers()) {
                if (p.isBanned()) {
                    unbanListLeft.add(p);
                }
            }

            boolean leftTeamUnbanFirst = Math.random() < 0.5;

            if (leftTeamUnbanFirst) {
                unbanList = getAlternating(unbanListLeft, unbanListRight);
            } else {
                unbanList = getAlternating(unbanListRight, unbanListLeft);
            }

        }
        return unbanList;
    }

    /**
     * Get the next available turn type for the given entity. Returns null if there is no turn available for the entity or if no entity was given
     * <p>
     * A turn type can be:
     * move (for player moves)
     * action (for player actions)
     * fan (for fan actions)
     * removeBan (for unbanning a player) (not used here)
     *
     * @param activeEntity The entity for which the turn type should be determined
     * @return The next available turn type for the entity or null if there is none
     */
    private String getNextTurnType(Entity activeEntity) {
        if (activeEntity == null) {
            return null;
        }
        if (activeEntity.hadTurn) {
            //entity had a turn, can only do one turn.
            return null;
        } else {
            if (activeEntity instanceof Fan) {
                // it's a bird, it's a plane, NO. It's a fan!!
                return Strings.FAN;
            } else {
                //it's a player
                Player activePlayer = (Player) activeEntity;

                if(activePlayer.knockedOut){
                    activePlayer.hadTurn = true;
                    activePlayer.knockedOut = false;
                    return null;
                    }
                else if (activePlayer.hadSecondMove) {
                    //the player can't move anymore

                    if (canPlayerDoAction(activePlayer)&&playerDidAction(activePlayer)) {
                        //the player can do an action

                        return Strings.ACTION;
                    } else {
                        //the player can't do an action

                        return null;
                    }
                } else {
                    //the player has moved once or not at all

                    if (activePlayer.hadFirstMove) {
                        //the player has moved once

                        if (canPlayerMoveAgain(activePlayer)) {
                            //the player gets a second move

                            return Strings.MOVE;
                        } else {
                            //the player does not get a second move
                            // -> try again with no moves left
                            activePlayer.hadSecondMove = true;
                            return getNextTurnType(activeEntity);
                        }
                    } else {
                        //the player has not moved yet

                        return Strings.MOVE;
                    }
                }
            }
        }
    }

    /**
     * Determines if a given player is able to do any action.
     * <p>
     * Possible actions:
     * bludgerBeating (if the player is a beater and is on the same square as a bludger)
     * quaffleThrow (if the player is a keeper/chaser and is on the same square as the quaffle)
     * wrestQuaffle (if the player is a chaser and there is a chaser (with a quaffle) on an adjacent square or a keeper (with a quaffle) outside his keeper zone
     *
     * @param player Player that is checked
     * @return true if the player can do any action, false if not
     */
    private boolean canPlayerDoAction(Player player) {
        if (player instanceof Beater) { //bludgerBeating
            if (getBludger(player.getPos()) != null) {
                return true;
            }
        } else if (isPlayerChaserOrKeeper(player)) { //quaffleThrow
            if (getQuaffle(player.getPos()) != null) {
                return true;
            }
        }

        if (player instanceof Chaser) { //wrestQuaffle
            return canChaserWrestQuaffle(player);
        }
        return false;
    }
    private boolean playerDidAction(Player player){
       for(Player p: game.actionPlayers){
           if(player==p){
               return false;
           }
       }
       game.actionPlayers.add(player);

        return true;
    }

    /**
     * Checks if a given player, which is always a chaser, can wrest the quaffle.
     *
     * @param chaser That might be able to wrest the quaffle.
     * @return True if the given chaser can wrest the quaffle. False otherwise.
     */
    private boolean canChaserWrestQuaffle(Player chaser) {

        for (Square s : getAllAdjacentSquaresOnPitch(chaser.getPos())) {

            Player playerOnSquare = getPlayer(s);
            if (getQuaffle(s) != null && playerOnSquare != null && game.getClientFromEntity(playerOnSquare) != game.getClientFromEntity(chaser)) {
                //there is an enemy player (with a quaffle) on an adjacent square

                if (playerOnSquare instanceof Keeper) {
                    if (game.getClientFromEntity(playerOnSquare) == game.getRightClient()) {
                        if (!(isPlayerInRightKeeperZone(playerOnSquare))) {
                            return true;
                        }
                    } else {
                        if (!(isPlayerInLeftKeeperZone(playerOnSquare))) {
                            return true;
                        }
                    }
                } else if (playerOnSquare instanceof Chaser) {
                    return true;
                }
            }
        }
        return false;
    }

    /**
     * Gets the duration for the game's current phase, which the server has to for between sending snapshots.
     *
     * @return The duration in milliseconds.
     */
    private Integer getCurrentPhaseTimeout() {
        if (game.state == Game.PLAYER_PHASE) {
            return game.matchConfig.timings.get(Strings.FAN_TURN_TIMEOUT);
        } else if (game.state == Game.FAN_PHASE) {
            return game.matchConfig.timings.get(Strings.PLAYER_TURN_TIMEOUT);
        } else {
            Logger.error("This should not happen. game.state: " + game.state);
            return null;
        }
    }

    /**
     * Gets the other client in perspective of the given client.
     *
     * @param client Of which we want the other client.
     * @return The other client in perspective from the given client.
     */
    private Client getOtherClient(Client client) {
        if (game.getRightClient() == client) {
            return game.getLeftClient();
        } else {
            return game.getRightClient();
        }
    }

    /**
     * Checks if a given player, which is always a keeper, is in his keeper zone.
     *
     * @param keeper Keeper that needs to be checked.
     * @return True if the keeper is in his keeper zone. False otherwise.
     */
    private boolean isKeeperInHisKeeperZone(Player keeper) {
        if (keeper.id.equals(Strings.LEFT_KEEPER)) {
            return (isPlayerInLeftKeeperZone(keeper));
        } else if (keeper.id.equals(Strings.RIGHT_KEEPER)) {
            return isPlayerInLeftKeeperZone(keeper);
        }
        Logger.warn("Should not reach this " + keeper);
        return false;
    }

    /**
     * Checks if the given player is in the left keeper zone.
     *
     * @param player Player that is to be checked.
     * @return True if the given player is in the left keeper zone. False otherwise.
     */
    private boolean isPlayerInLeftKeeperZone(Player player) {
        if(player.getPos().x==null||player.getPos().y==null){
            return false;
        }
        return (player.getPos().x < 3 || (player.getPos().x < 4 && (player.getPos().y > 1 && player.getPos().y < 11)) ||
                (player.getPos().x < 5 && (player.getPos().y > 3 && player.getPos().y < 9)));
    }

    /**
     * Checks if the given player is in the right keeper zone.
     *
     * @param player Player that is to be checked.
     * @return True if the given player is in the right keeper zone. False otherwise.
     */
    private boolean isPlayerInRightKeeperZone(Player player) {
        if(player.getPos().x==null||player.getPos().y==null){
            return false;
        }
        return (player.getPos().x > 13 || (player.getPos().x > 12 && (player.getPos().y > 1 && player.getPos().y < 11)) ||
                (player.getPos().x > 11 && (player.getPos().y > 3 && player.getPos().y < 9)));
    }

    private Player isQuaffleIntercepted(Square from, Square to) {
        List<Square> overpassedSquares = getOverpassedSquares(from, to);

        overpassedSquares.sort((square, other) -> {
            int squareDistance = abs(from.x - square.x) + abs(from.y - square.y);
            int otherDistance = abs(from.x - other.x) + abs(from.y - other.y);
            return squareDistance - otherDistance;
        });

        System.out.println(overpassedSquares);
        return null;
    }


    /**
     * Gets a list of all overpassed Squares. Needed for quaffle throwing and bludger beating.
     *
     * @param from starting Square
     * @param to   ending Square
     * @return A list of all overpassed squares.
     */
    private List<Square> getOverpassedSquares(Square from, Square to) {
        List<Square> ret = new ArrayList<>();

        if (from.x.equals(to.x)) {
            for (int i = Math.min(from.y, to.y); i <= Math.max(from.y, to.y); i++) {
                Square current = new Square(from.x, i);
                if (!isSquareOnPitch(current) || current.equals(from)) {
                    continue;
                }
                ret.add(current);
            }
            return ret;
        }
        if (from.y.equals(to.y)) {
            for (int i = Math.min(from.x, to.x); i <= Math.max(from.x, to.x); i++) {
                Square current = new Square(i, from.y);
                if (!isSquareOnPitch(current) || current.equals(from)) {
                    continue;
                }
                ret.add(current);
            }
            return ret;
        }

        double angleTo = abs((double) (to.y - from.y) / (to.x - from.x));
        double angleFrom = abs((double) (from.y - to.y) / (from.x - to.x));

        for (int x = Math.min(from.x, to.x); x <= Math.max(from.x, to.x); x++) {
            for (int y = Math.min(from.y, to.y); y <= Math.max(from.y, to.y); y++) {
                Square current = new Square(x, y);
                if (!isSquareOnPitch(current)) {
                    continue;
                }

                if (current.equals(from) || current.equals(new Square(from.x, to.y)) || current.equals(new Square(to.x, from.y))) {
                    continue;
                }

                if (x == from.x || y == from.y) {
                    double diffX = abs(x - to.x);
                    double diffY = abs(y - to.y);
                    double corner1Angle = (diffY - 0.5) / (diffX + 0.5);
                    double corner2Angle = (diffY + 0.5) / (diffX - 0.5);
                    if ((corner1Angle > angleTo && corner2Angle < angleTo) || (corner1Angle < angleTo && corner2Angle > angleTo)) {
                        ret.add(current);
                    }
                } else {
                    double diffX = abs(x - from.x);
                    double diffY = abs(y - from.y);
                    double corner1Angle = (diffY - 0.5) / (diffX + 0.5);
                    double corner2Angle = (diffY + 0.5) / (diffX - 0.5);
                    if ((corner1Angle > angleFrom && corner2Angle < angleFrom) || (corner1Angle < angleFrom && corner2Angle > angleFrom)) {
                        ret.add(current);
                    }
                }
            }
        }
        return ret;
    }

    /**
     * Checks whether a goal throw is valid. Needs the starting and ending location as squares.
     *
     * @param from starting Square
     * @param to   ending Square
     * @return True if a valid goal throw happened. False otherwise.
     */
    private boolean isValidGoalThrow(Square from, Square to) {
        if (!isGoalPostSquare(to)) {
            return false;
        }

        return abs(from.y - to.y) <= abs(from.x - to.x);
    }

    /**
     * Checks whether a given square is a left goal post.
     *
     * @param square That is to be checked.
     * @return True if the given square is a left goal post. False otherwise.
     */
    private boolean isLeftGoalPostSquare(Square square) {
        return square.equals(Config.LEFT_GOALPOST_1) || square.equals(Config.LEFT_GOALPOST_2) || square.equals(Config.LEFT_GOALPOST_3);
    }

    /**
     * Checks whether a given square is a right goal post.
     *
     * @param square That is to be checked.
     * @return True if the given square is a right goal post. False otherwise.
     */
    private boolean isRightGoalPostSquare(Square square) {
        return square.equals(Config.RIGHT_GOALPOST_1) || square.equals(Config.RIGHT_GOALPOST_2) || square.equals(Config.RIGHT_GOALPOST_3);
    }

    /**
     * Checks whether a given square is a goal post.
     *
     * @param square That is to be checked.
     * @return True if the given square is a goal post. False otherwise.
     */
    private boolean isGoalPostSquare(Square square) {
        return isLeftGoalPostSquare(square) || isRightGoalPostSquare(square);
    }

    /**
     * Checks if a given position is on the pitch.
     *
     * @param s Square that is getting checked.
     * @return True if the given position is on the pitch.
     */
    private boolean isSquareOnPitch(Square s) {
        if (s.x < 0 || s.x > 16 || s.y < 0 || s.y > 12) {
            return false;
        }
        return ((s.x != 0 && s.x != 16) || (s.y <= 8 && s.y > 3)) && ((s.x != 1 && s.x != 15) || (s.y <= 10 && s.y > 1)) && ((s.x != 2 && s.x != 14) || (s.y != 12 && s.y != 0));
    }

    /**
     * Starts a new round by starting the ballphase, executing it and starting the player phase. It figures out the first player.
     */
    private void newRound() {
        //increase round counter
        game.newRound();

        sendRoundChangeBroadcast();

        //broadcast ball phase
        game.state = Game.BALL_PHASE;
        sendPhaseChangeBroadcast(Strings.BALL_PHASE);

        //check if snitch needs to appear
        if (game.roundCounter == Config.SNITCH_START_ROUND) {
            //snitch needs to spawn

            Ball snitch = game.getBall(Strings.SNITCH);
            snitch.setPos(getSnitchSpawnPos());

            sendMoveBroadcast(snitch, snitch.getPos()); //new Square(null, null));//TODO ist das richtig?
        }

        //overtime
        if (game.roundCounter == game.matchConfig.maxRounds) {
            //start overtime phase 1
            game.overtime = Game.OVERTIME_PHASE_1;
        } else if (game.roundCounter == game.matchConfig.maxRounds + 3) {
            //start overtime phase 2, 3 rounds after phase 1
            game.overtime = Game.OVERTIME_PHASE_2;
        } else if (game.roundCounter == game.overtimePhaseFourStart) {
            //start overtime phase 4
            game.overtime = Game.OVERTIME_PHASE_4;
        }

        //do ball phase
        doBallPhase();

        //broadcast player phase
        game.state = Game.PLAYER_PHASE;
        sendPhaseChangeBroadcast(Strings.PLAYER_PHASE);

        //create turnList
        game.setTurnList(createTurnList(getRandomClient()));

        //broadcast
        game.activeEntity = null;
        game.activeTurnType = null;
        calculateAndSendNext();
    }


    /**
     * Creates the turnList giving the clients alternating turns.
     *
     * @param startClient That is the first client in the turnList.
     * @return A entity list that is used to get the next entity that should act in a round.
     */
    private List<Entity> createTurnList(Client startClient) {
        //starting client is starting client for player phase

        Client otherClient = getOtherClient(startClient);
        List<Entity> turnList;

        //get all viable players from each team
        //viable: !hadTurn and !knockedOut and !banned

        List<Entity> viablePlayersStart = getAllViablePlayers(startClient.team);
        Collections.shuffle(viablePlayersStart);

        List<Entity> viablePlayersOther = getAllViablePlayers(otherClient.team);
        Collections.shuffle(viablePlayersOther);

        //fill the list with alternating fans from both teams
        turnList = getAlternating(viablePlayersStart, viablePlayersOther);


        List<Entity> viableFansStart = getAllViableFans(startClient.team);
        Collections.shuffle(viableFansStart);

        List<Entity> viableFansOther = getAllViableFans(otherClient.team);
        Collections.shuffle(viableFansOther);

        //add to the list: alternating fans from both teams
        turnList.addAll(getAlternating(viableFansStart, viableFansOther));

        //care for all the knocked out players
        for (Player p : game.getAllPlayers()) {

            //if the player is knocked out, but did not have a turn yet, then this is the turn he
            // skips -> hadTurn = true; knockedOut = false;
            if (!p.hadTurn && p.knockedOut) {
                p.hadTurn = true;
                p.knockedOut = false;
            }
        }

        return turnList;
    }

    /**
     * Gets a list of all available players from a given team. Available in this case means the player is not banned, not knocked out and has not had its turn this round yet.
     *
     * @param team For which all available players are to be returned.
     * @return A list of all available players. Can be an empty list.
     */
    private List<Entity> getAllViablePlayers(Team team) {
        List<Entity> viablePlayers = new ArrayList<>();
        for (Player p : team.getPlayers()) {
            if (!p.knockedOut && !p.isBanned() && !p.hadTurn) {
                viablePlayers.add(p);
            }
        }
        return viablePlayers;
    }

    /**
     * Gets a list of all available fans from a given team. Available in this case means the fan is not banned and has not had its turn this round yet.
     *
     * @param team For which all available fans are to be returned.
     * @return A list of all available fans. Can be an empty list.
     */
    private List<Entity> getAllViableFans(Team team) {
        List<Entity> viableFans = new ArrayList<>();
        for (Fan f : team.getFans()) {
            if (!f.isBanned() && !f.hadTurn) {
                viableFans.add(f);
            }
        }
        return viableFans;
    }

    /**
     * Gets a list with objects of the two given lists in alternating order.
     *
     * @param firstList  First list.
     * @param secondList Second list.
     * @param <T>        Type of the given and return lists
     * @return A list with the same type as both given lists and alternating order (firstListObject, secondListObject, firstListobject...).
     */
    private <T> List<T> getAlternating(List<T> firstList, List<T> secondList) {
        List<T> returnList = new ArrayList<>();

        while (!firstList.isEmpty() && !secondList.isEmpty()) {
            returnList.add(firstList.remove(0));
            returnList.add(secondList.remove(0));
        }
        while (!firstList.isEmpty()) {
            returnList.add(firstList.remove(0));
        }
        while (!secondList.isEmpty()) {
            returnList.add(secondList.remove(0));
        }

        return returnList;
    }

    /**
     * Executes the entire ballPhase. This includes all different snitch movements according to the overtime state of the game as well as both bludgers movement.
     */
    private void doBallPhase() {
        Logger.trace("Starting doBallPhase");
        //snitch
        Ball snitch = game.getBall(Strings.SNITCH);

        Square snitchMoveSquare = snitch.getPos();

        if (game.overtime == Game.OVERTIME_PHASE_2) {
            //overtime phase 2
            //snitch moves to the middle square

            //get the adjacent square with the least distance to the middle square
            int minDistanceToMiddleSquare = Integer.MAX_VALUE;
            Square bestSquare = null;
            for (Square s : getAllAdjacentSquaresOnPitch(snitch.getPos())) {
                int distance = calculateDistance(s, Config.MIDDLE_SQUARE);
                if (distance < minDistanceToMiddleSquare) {
                    minDistanceToMiddleSquare = distance;
                    bestSquare = s;
                }
            }

            if (bestSquare == null) {
                //THIS SHOULD NOT HAPPEN
                Logger.error("This should not happen. Snitch: " + snitch);
                bestSquare = getRandomFreeAdjacentSquare(snitch.getPos());
            }

            snitchMoveSquare = bestSquare;
            if (snitchMoveSquare.equals(Config.MIDDLE_SQUARE)) {
                //we reached the middle square
                game.overtime = Game.OVERTIME_PHASE_3;
                game.overtimePhaseFourStart = game.roundCounter + 3;

            }

        } else if (game.roundCounter == Game.OVERTIME_PHASE_4) {
            //we are now in phase 4
            //-> jump to the nearest seeker

            Player nearestSeeker = getNearestSeeker(getSnitchSpawnPos());

            snitchMoveSquare = nearestSeeker.getPos();
            game.getClientFromEntity(nearestSeeker).team.points += Config.SNITCH_POINTS;

            matchFinish();

        } else if (!snitch.getPos().hasNullPos() && !(game.overtime == Game.OVERTIME_PHASE_3)) {
            //no overtime, but the snitch spawned already (and we are not in phase 3)
            //-> regular movement

            Player nearestSeeker = getNearestSeeker(snitch.getPos());


            List<Square> viableSquares = new ArrayList<>();
            int previousDistanceToSeeker = calculateDistance(nearestSeeker.getPos(), snitch.getPos());
            for (Square s : getAllAdjacentSquaresOnPitch(snitch.getPos())) {
                if (isSquareFree(s) && calculateDistance(s, nearestSeeker.getPos()) > previousDistanceToSeeker) {
                    //s is free and is further away
                    viableSquares.add(s);
                }
            }

            if (viableSquares.isEmpty()) {
                //no viable squares
                //get random free adjacent square (tunneling etc.)
                snitchMoveSquare = getRandomFreeAdjacentSquare(snitch.getPos());
            } else {
                snitchMoveSquare = getRandomSquare(viableSquares);
            }
        }

        if (!snitch.getPos().hasNullPos()) {
            //snitch is spawned -> needs to move

            Square oldPos = snitch.getPos();
            snitch.setPos(snitchMoveSquare);
            sendMoveBroadcast(snitch, oldPos);
        }

        //check win condition
        if (game.overtime == Game.OVERTIME_PHASE_4) {
            matchFinish();
        }


        //bludger

        Ball bludger1 = game.getBall(Strings.BLUDGER1);
        Ball bludger2 = game.getBall(Strings.BLUDGER2);

        boolean bludger1First = Math.random() < 0.5;
        if (bludger1First) {
            moveBludger(bludger1);
            moveBludger(bludger2);
        } else {
            moveBludger(bludger2);
            moveBludger(bludger1);
        }


    }

    /**
     * Calculates the next position to which a given bludger should move to and sends broadcasts accordingly.
     *
     * @param bludger For which the move gets calculated.
     */
    private void moveBludger(Ball bludger) {
        Player nextPlayer = getNearestPlayerNotBeater(bludger.getPos());

        Square moveTo = bludger.getPos();
        Square oldPos = bludger.getPos();

        List<Square> viableSquares = new ArrayList<>();
        int previousDistanceToNextPlayer = calculateDistance(nextPlayer.getPos(), bludger.getPos());
        for (Square s : getAllAdjacentSquaresOnPitch(bludger.getPos())) {
            if (calculateDistance(s, nextPlayer.getPos()) < previousDistanceToNextPlayer||isSquareFreeForBludger(s)) {
                //s is closer
                viableSquares.add(s);
            }
        }

        if (viableSquares.isEmpty()) {
            //the nextPlayer should be on the same square as the bludger

            //don't move, don't knock out
        } else {
            //we've got viable squares
            moveTo = getRandomSquare(viableSquares);

        }

        bludger.setPos(moveTo);
        sendMoveBroadcast(bludger, oldPos);

        if (oldPos.equals(moveTo)) {
            //we moved -> knock out?

            Player playerOnMoveTo = getPlayer(moveTo);
            if (!(playerOnMoveTo == null || playerOnMoveTo instanceof Beater || playerOnMoveTo.knockedOut)) {
                boolean knockOut = isProbabilitySuccessful(Strings.KNOCK_OUT);
                if (knockOut) {
                    playerOnMoveTo.knockedOut = true;
                }

                bludger.setPos(getRandomFreeSquareOnPitch());
                sendBludgerKnockoutBroadcast(bludger, playerOnMoveTo, knockOut);
            }
        }
    }

    /**
     * Gets a random player that is not a beater and nearest to a given square.
     *
     * @param pos Square from which the nearest player gets calculated.
     * @return A random nearest player to the given square that is not a beater. Null if it cannot find one, which should theoretically never happen in a valid game.
     */
    private Player getNearestPlayerNotBeater(Square pos) {
        List<Player> viablePlayers = new ArrayList<>();

        int minDistance = Integer.MAX_VALUE;
        for (Player p : game.getAllPlayers()) {
            if(p.getPos().x!=null &&p.getPos().y!=null){
                if (p instanceof Beater) {
                    continue;
                }

                int distance = calculateDistance(pos, p.getPos());
                if (distance < minDistance) {
                    viablePlayers.clear();
                    viablePlayers.add(p);
                    minDistance = distance;
                } else if (distance == minDistance) {
                    viablePlayers.add(p);
                }
            }
            else{
                continue;
            }
        }

        if (viablePlayers.isEmpty()) {
            Logger.error("This should not happen. pos: " + pos);
        }

        return getRandomPlayer(viablePlayers);
    }

    /**
     * Gets a random player from a given list of players or null if the list is empty.
     *
     * @param players List of players that this method returns a random player from.
     * @return A random player from the given player or null if the list is empty.
     */
    private Player getRandomPlayer(List<Player> players) {
        if (players.isEmpty()) {
            return null;
        } else {
            return players.get((int) (Math.random() * players.size()));
        }
    }

    /**
     * Gets the nearest seeker to a given square.
     *
     * @param pos Square to be checked for nearest seeker.
     * @return The player which is the nearest seeker.
     */
    private Player getNearestSeeker(Square pos) {
        Player leftSeeker = game.getPlayer(Strings.LEFT_SEEKER);
        Player rightSeeker = game.getPlayer(Strings.RIGHT_SEEKER);

        if (calculateDistance(pos, leftSeeker.getPos()) < calculateDistance(pos, rightSeeker.getPos())) {
            return leftSeeker;
        } else if (calculateDistance(pos, leftSeeker.getPos()) > calculateDistance(pos, rightSeeker.getPos())) {
            return rightSeeker;
        } else {
            boolean left = Math.random() < 0.5;
            if (left) {
                return leftSeeker;
            } else {
                return rightSeeker;
            }
        }
    }

    /**
     * Gets a square that has the same distance to both team's seekers or very similar if same distance is not possible.
     * Used to get the snitch spawn position.
     *
     * @return The square with the properties as described above.
     */
    private Square getSnitchSpawnPos() {
        Player leftSeeker = game.getPlayer(Strings.LEFT_SEEKER);
        Player rightSeeker = game.getPlayer(Strings.RIGHT_SEEKER);

        List<Square> allSquares = getAllFreeSquaresOnPitch();

        List<Square> viableSquares = new ArrayList<>();

        for (int i = 0; i < 16; i++) {

            //check for each square on the pitch
            for (Square s : allSquares) {
                if(leftSeeker.getPos().x!=null&&leftSeeker.getPos().y!=null&&rightSeeker.getPos().x!=null&&rightSeeker.getPos().y!=null){
                    //is the distance to each seeker smaller or equal to i (for i = 0: distances are the same, for i = 1: distances are max 1 apart, ...)
                    if (abs(calculateDistance(s, leftSeeker.getPos()) - calculateDistance(s, rightSeeker.getPos())) <= i) {
                        viableSquares.add(s);
                    }
                }
            }
            if (!viableSquares.isEmpty()) {
                //there is at least one viable square in the list now
                break;
            }else{
               viableSquares.add(getRandomFreeSquareOnPitch());
            }
        }

        return getRandomSquare(viableSquares);
    }

    /**
     * Gets a list of all free squares on the pitch.
     *
     * @return A list of all free squares that are still on the pitch.
     */
    private List<Square> getAllFreeSquaresOnPitch() {
        List<Square> squares = new ArrayList<>();
        for (int x = 0; x <= 16; x++) {
            for (int y = 0; y <= 12; y++) {
                Square curr = new Square(x, y);
                if (isSquareFree(curr)) {
                    squares.add(curr);
                }
            }
        }
        return squares;
    }

    /**
     * Gets a random playing client.
     *
     * @return A random playing client.
     */
    private Client getRandomClient() {
        int rand = (int) (Math.random() * 2);
        if (rand == 1) {
            return game.getRightClient();
        } else {
            return game.getLeftClient();
        }
    }

    /**
     * Checks if the given player is a keeper or chaser. Useful for quaffle interactions.
     *
     * @param player That needs to be checked.
     * @return True if the given player is a chaser or keeper. False otherwise.
     */
    private boolean isPlayerChaserOrKeeper(Player player) {
        return player instanceof Chaser || player instanceof Keeper;
    }

    /**
     * Calculates the distance between two given squares.
     *
     * @param from Value of old square.
     * @param to   value of new square.
     * @return The distance between the two squares as int.
     */
    private int calculateDistance(Square from, Square to) {

        return Math.max(Math.abs(from.x - to.x), Math.abs(from.y - to.y));
    }

    /**
     * Gets a random free square adjacent to the given square. Tunnels recursively if needed.
     *
     * @param square Of which to get a random free adjacent square.
     * @return A random free adjacent square (with recursive tunneling if needed).
     */
    private Square getRandomFreeAdjacentSquare(Square square) {
        List<Square> freeSquares = new ArrayList<>();

        List<Square> adjacentSquares = getAllAdjacentSquaresOnPitch(square);

        //get all free adjacent squares
        for (Square s : adjacentSquares) {
            if (isSquareFree(s)) {
                freeSquares.add(s);
            }
        }

        if (freeSquares.isEmpty()) {
            // no free adjacent squares -> tunneling
            // choose a random (not free) adjacent square and call recursively
            return getRandomFreeAdjacentSquare(getRandomSquare(adjacentSquares));
        } else {
            //there is at least one free adjacent square
            return getRandomSquare(freeSquares);
        }
    }

    /**
     * Gets a random square from a list of given squares.
     *
     * @param squares List of squares of which a random square is to be determined.
     * @return A random square of the given list.
     */
    private Square getRandomSquare(List<Square> squares) {
        return squares.get((int) (Math.random() * squares.size()));
    }

    /**
     * Gets all adjacentSquares that are still valid squares on the pitch.
     *
     * @param square To get adjacent squares from.
     * @return A list of all valid adjacent squares.
     */
    private List<Square> getAllAdjacentSquaresOnPitch(Square square) {
        List<Square> ret = new ArrayList<>();

        if(square.x!= null && square.y!=null){
        for (int x = square.x - 1; x <= square.x + 1; x++) {
            for (int y = square.y - 1; y <= square.y + 1; y++) {
                Square curr = new Square(x, y);
                if (isSquareOnPitch(curr) && !square.equals(curr)) {
                    ret.add(curr);
                }
            }
        }
        return ret;
        }
        ret.add(new Square(1,1));
        return ret;
    }

    /**
     * Checks a given probability against a random number between 0 and 1.
     *
     * @param probabilityString The String id of the probability
     * @return True if the event that the probability describes is to occur. False otherwise.
     */
    private boolean isProbabilitySuccessful(String probabilityString) {
        float probability = game.matchConfig.probabilities.get(probabilityString);
        return Math.random() <= probability;
    }

    /**
     * Calculates if a given player can move again this turn.
     *
     * @param player That might move again this turn.
     * @return True if the given player can move again. False otherwise.
     */
    private boolean canPlayerMoveAgain(Player player) {
        return Math.random() <= game.matchConfig.probabilities.get(player.broom);
    }

    /**
     * Checks if a given square is free of balls and players.
     *
     * @param square That is to be checked.
     * @return True if the square is free of any ball or player.
     */
    private boolean isSquareFree(Square square) {
        for (int i = 0; i < game.getAllMovableEntities().length; i++) {
            if (game.getAllMovableEntities()[i].getPos().equals(square)) {
                return false;
            }
        }
        //checks poo positions
        for (Square s: game.getAllPoos()) {
            if(s.equals(square))
                return false;
        }
        return true;
    }

    private boolean altIsSquareFree(Square square) {
        for (int i = 0; i < game.getAllMovableEntities().length; i++) {
            if (game.getAllMovableEntities()[i].getPos().equals(square)) {
                return false;
            }
        }
        //checks poo positions
        return true;
    }
    //So bludgers aren't on same square
    private boolean isSquareFreeForBludger(Square square) {
        for (int i = 0; i < game.getAllBalls().length; i++) {
            if (game.getAllBalls()[i].getPos().equals(square)) {
                return false;
            }
        }
        //checks poo positions
        for (Square s: game.getAllPoos()) {
            if(s.equals(square))
                return false;
        }
        return true;
    }
    private boolean wombatPooedField(Square square){
        for (Square s: game.getAllPoos()) {
            if(s.equals(square))
                return true;
        }
        return false;
    }




    /**
     * Get the player on the given square if there is one.
     *
     * @param square Square to be checked for a player.
     * @return The player having the same position as the given square or null if there is none.
     */
    private Player getPlayer(Square square) {
        for (int i = 0; i < game.getAllPlayers().length; i++) {
            if (game.getAllPlayers()[i].getPos().equals(square)) {
                return game.getAllPlayers()[i];
            }
        }
        return null;
    }

    /**
     * Get the snitch on the given square if it is there.
     *
     * @param square Square to be checked for the snitch.
     * @return The snitch if it is on the given square or null if it is not there.
     */
    private Ball getSnitch(Square square) {
        for (int i = 0; i < game.getAllBalls().length; i++) {
            if (game.getAllBalls()[i].getPos().equals(square)) {
                if (game.getAllBalls()[i].type == Ball.SNITCH) {
                    return game.getAllBalls()[i];
                }
            }
        }
        return null;
    }

    /**
     * Get the quaffle on the given square if it is there.
     *
     * @param square Square to be checked for the quaffle.
     * @return The quaffle if it is on the given square or null if it is not there.
     */
    private Ball getQuaffle(Square square) {
        for (int i = 0; i < game.getAllBalls().length; i++) {
            if (game.getAllBalls()[i].getPos().equals(square)) {
                if (game.getAllBalls()[i].type == Ball.QUAFFLE) {
                    return game.getAllBalls()[i];
                }
            }
        }
        return null;
    }

    /**
     * Get a bludger on the given square if there is one.
     *
     * @param square Square to be checked for bludger.
     * @return A bludger if it is on the given square or null if it is not there. If both bludgers are on that square
     * the method returns the first bludger from this game's balls array.
     */
    private Ball getBludger(Square square) {
        for (int i = 0; i < game.getAllBalls().length; i++) {
            if (game.getAllBalls()[i].getPos().equals(square)) {
                if (game.getAllBalls()[i].type == Ball.BLUDGER) {
                    return game.getAllBalls()[i];
                }
            }
        }
        return null;
    }


    //############################# SENDER HELPER METHODS

    /**
     * Creates a snapshot from a given DeltaBroadcastJson.
     *
     * @param deltaBroadcastJson That is used in the snapshot.
     * @return A snapshot ready to be sent.
     */
    private SnapshotJson createSnapshotFromDeltaBroadcast(DeltaBroadcastJson deltaBroadcastJson) {
        //we can only send a new snapshot after a certain time has passed since the last one

        long currentTimeMillis = System.currentTimeMillis();

        while (currentTimeMillis - lastSnapshotMillis < getCurrentPhaseAnimationDuration()) {
            //wait until we can create the next snapshot
            try {
                Thread.sleep(10);
                currentTimeMillis = System.currentTimeMillis();
            } catch (InterruptedException e) {
                Logger.warn("Thread was interrupted while sleeping. Why tho?");
            }
        }

        lastSnapshotMillis = currentTimeMillis;

        TeamSnapshotJson leftTeam = createTeamSnapshotJson(game.getLeftClient().team);
        TeamSnapshotJson rightTeam = createTeamSnapshotJson(game.getRightClient().team);

        SnapshotJson snapshotJson = new SnapshotJson();
        snapshotJson.lastDeltaBroadcast = deltaBroadcastJson;
        snapshotJson.balls = createBallsJson();
        snapshotJson.leftTeam = leftTeam;
        snapshotJson.rightTeam = rightTeam;
        snapshotJson.round = game.roundCounter;
        snapshotJson.spectatorUserName = game.getSpectatorsByUsername();
        snapshotJson.phase = game.getPhaseAsString();
        snapshotJson.wombatCubes= game.PoosToJson();

        return snapshotJson;
    }

    /**
     * Gets the duration for animations of the game's current phase, which the server has to for between sending snapshots.
     *
     * @return The duration in milliseconds.
     */
    private int getCurrentPhaseAnimationDuration() {
        if (game.state == Game.BALL_PHASE) {
            return game.matchConfig.timings.get(Strings.MIN_BALL_PHASE_ANIMATION_DURATION);
        } else if (game.state == Game.PLAYER_PHASE) {
            return game.matchConfig.timings.get(Strings.MIN_PLAYER_PHASE_ANIMATION_DURATION);
        } else if (game.state == Game.FAN_PHASE) {
            return game.matchConfig.timings.get(Strings.MIN_FAN_PHASE_ANIMATION_DURATION);
        } else {
            //this should not happen
            Logger.warn("This should not happen, state: " + game.state);
            return -1;
        }
    }

    /**
     * Creates a ballsJson with the information of all balls of this game.
     *
     * @return A ballsJson of all balls of this game.
     */
    private BallsJson createBallsJson() {
        BallsJson ballsJson = new BallsJson();

        BallJson snitch = createBallJsonByStringId(Strings.SNITCH);
        BallJson quaffle = createBallJsonByStringId(Strings.QUAFFLE);
        BallJson bludger1 = createBallJsonByStringId(Strings.BLUDGER1);
        BallJson bludger2 = createBallJsonByStringId(Strings.BLUDGER2);
        ballsJson.snitch = snitch;
        ballsJson.quaffle = quaffle;
        ballsJson.bludger1 = bludger1;
        ballsJson.bludger2 = bludger2;

        return ballsJson;
    }

    /**
     * Creates ballJson for a given ball String and fills the ballJson.
     *
     * @param ballType For which a ballJson is created. Either snitch, quaffle, bludger1 or bludger2
     * @return A valid ballJson for the given String ballType.
     */
    private BallJson createBallJsonByStringId(String ballType) {
        BallJson ballJson = new BallJson();
        ballJson.xPos = game.getBall(ballType).getPos().x;
        ballJson.yPos = game.getBall(ballType).getPos().y;
        return ballJson;
    }

    /**
     * Creates a teamSnapshotJson for a given team.
     *
     * @param team To create a snapshotjson for.
     * @return A TeamSnapshotJson for the given team.
     */
    private TeamSnapshotJson createTeamSnapshotJson(Team team) {
        TeamSnapshotJson teamSnapshotJson = new TeamSnapshotJson();
        teamSnapshotJson.points = team.points;

        FanSnapshotJson[] fans = new FanSnapshotJson[team.fans.length];
        for (int i = 0; i < team.fans.length; i++) {
            FanSnapshotJson fanSnapshotJson = new FanSnapshotJson();
            fanSnapshotJson.fanType = team.fans[i].type;
            fanSnapshotJson.banned = team.fans[i].isBanned();
            fanSnapshotJson.turnUsed = team.fans[i].hadTurn;
            fans[i] = fanSnapshotJson;
        }

        teamSnapshotJson.fans = fans;

        SeekerSnapshotJson seekerSeekerSnapshotJson = createSeekerSnapshotJson(team.players[0]);
        KeeperChaserSnapshotJson keeperSeekerSnapshotJson = createKeeperChaserSnapshotJson(team.players[1]);
        KeeperChaserSnapshotJson chaser1SeekerSnapshotJson = createKeeperChaserSnapshotJson(team.players[2]);
        KeeperChaserSnapshotJson chaser2SeekerSnapshotJson = createKeeperChaserSnapshotJson(team.players[3]);
        KeeperChaserSnapshotJson chaser3SeekerSnapshotJson = createKeeperChaserSnapshotJson(team.players[4]);
        BeaterSnapshotJson beater1SeekerSnapshotJson = createBeaterSnapshotJson(team.players[5]);
        BeaterSnapshotJson beater2SeekerSnapshotJson = createBeaterSnapshotJson(team.players[6]);

        PlayersSnapshotJson playersSnapshotJson = new PlayersSnapshotJson();
        playersSnapshotJson.seeker = seekerSeekerSnapshotJson;
        playersSnapshotJson.keeper = keeperSeekerSnapshotJson;
        playersSnapshotJson.chaser1 = chaser1SeekerSnapshotJson;
        playersSnapshotJson.chaser2 = chaser2SeekerSnapshotJson;
        playersSnapshotJson.chaser3 = chaser3SeekerSnapshotJson;
        playersSnapshotJson.beater1 = beater1SeekerSnapshotJson;
        playersSnapshotJson.beater2 = beater2SeekerSnapshotJson;
        teamSnapshotJson.players = playersSnapshotJson;

        return teamSnapshotJson;
    }

    /**
     * Helper method to create a seekerSnapshotJson easily.
     *
     * @param player For which this seekerSnapshotJson is created.
     * @return The correct seekerSnapshotJson for the given player.
     */
    private SeekerSnapshotJson createSeekerSnapshotJson(Player player) {
        SeekerSnapshotJson seekerSnapshotJson = new SeekerSnapshotJson();
        seekerSnapshotJson.banned = player.isBanned();
        seekerSnapshotJson.knockout = player.knockedOut;
        seekerSnapshotJson.turnUsed = player.hadTurn;
        seekerSnapshotJson.xPos = player.getPos().x;
        seekerSnapshotJson.yPos = player.getPos().y;

        return seekerSnapshotJson;
    }

    /**
     * Helper method to create a keeperChaserSnapshotJson easily.
     *
     * @param player For which this keeperChaserSnapshotJson is created.
     * @return The correct keeperChaserSnapshotJson for the given player.
     */
    private KeeperChaserSnapshotJson createKeeperChaserSnapshotJson(Player player) {
        KeeperChaserSnapshotJson keeperChaserSnapshotJson = new KeeperChaserSnapshotJson();
        keeperChaserSnapshotJson.banned = player.isBanned();
        keeperChaserSnapshotJson.knockout = player.knockedOut;
        keeperChaserSnapshotJson.turnUsed = player.hadTurn;
        keeperChaserSnapshotJson.xPos = player.getPos().x;
        keeperChaserSnapshotJson.yPos = player.getPos().y;
        keeperChaserSnapshotJson.holdsQuaffle = (getQuaffle(player.getPos()) != null);

        return keeperChaserSnapshotJson;
    }

    /**
     * Helper method to create a beaterSnapshotJson easily.
     *
     * @param player For which this beaterSnapshotJson is created.
     * @return The correct beaterSnapshotJson for the given player.
     */
    private BeaterSnapshotJson createBeaterSnapshotJson(Player player) {
        BeaterSnapshotJson beaterSnapshotJson = new BeaterSnapshotJson();
        beaterSnapshotJson.banned = player.isBanned();
        beaterSnapshotJson.knockout = player.knockedOut;
        beaterSnapshotJson.turnUsed = player.hadTurn;
        beaterSnapshotJson.xPos = player.getPos().x;
        beaterSnapshotJson.yPos = player.getPos().y;
        beaterSnapshotJson.holdsBludger = getBludger(player.getPos()) != null;


        return beaterSnapshotJson;
    }

    /**
     * Puts the given deltaBroadcast in a snapshot and sends it to all Clients in the Game. Also
     * adds the deltaBroadcast to the replay
     *
     * @param deltaBroadcastJson DeltaBroadcast to be broadcasted
     */
    private void broadcastDeltaSnapshot(DeltaBroadcastJson deltaBroadcastJson) {

        SnapshotJson snapshotJson = createSnapshotFromDeltaBroadcast(deltaBroadcastJson);

        ContainerJson container = ProtocolUtility.broadcastPayload(game, snapshotJson);

        if (game.firstSnapshot == null) {
            game.firstSnapshot = snapshotJson;
            game.initialTimestamp = container.timestamp;
        } else {
            saveToReplay(deltaBroadcastJson);
        }
        game.lastSnapshotContainer = container;
        game.lastNextContainer = null;
    }

    private void sendPauseResponse(String message,Client client){

        PauseResponseJson pauseResponseJson =new PauseResponseJson();

        pauseResponseJson.message=message;
        pauseResponseJson.userName= client.name;
        pauseResponseJson.pause=game.paused;

        ContainerJson container = ProtocolUtility.broadcastPayload(game, pauseResponseJson);
    }

    /**
     * Converts the given deltaBroadcast to a logJson and adds it to the replay
     *
     * @param deltaBroadcastJson deltaBroadcast to be added to the replay
     */
    private void saveToReplay(DeltaBroadcastJson deltaBroadcastJson) {
        LogJson logJson = new LogJson(
                LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyy-MM-dd " +
                        "HH:mm:ss.SSS")),
                Strings.DELTA_BROADCAST,
                deltaBroadcastJson
        );

        game.replay.add(logJson);
        Logger.trace("Saved to replay: " + logJson);
    }

    /**
     * Converts the given matchFinish to a logJson and adds it to the replay
     *
     * @param matchFinishJson matchFinish to be added to the replay
     */
    private void saveToReplay(MatchFinishJson matchFinishJson) {
        LogJson logJson = new LogJson(
                LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyy-MM-dd " +
                        "HH:mm:ss.SSS")),
                Strings.MATCH_FINISH,
                matchFinishJson
        );

        game.replay.add(logJson);
        Logger.trace("Saved to replay: " + logJson);
    }
}