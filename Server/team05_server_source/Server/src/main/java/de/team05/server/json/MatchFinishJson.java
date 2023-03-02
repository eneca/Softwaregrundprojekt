package de.team05.server.json;

public class MatchFinishJson extends LogPayloadJson {
    Integer endRound;
    Integer leftPoints;
    Integer rightPoints;
    String winnerUserName;
    String victoryReason;

    public MatchFinishJson(Integer endRound, Integer leftPoints, Integer rightPoints, String winnerUserName, String victoryReason) {
        this.endRound = endRound;
        this.leftPoints = leftPoints;
        this.rightPoints = rightPoints;
        this.winnerUserName = winnerUserName;
        this.victoryReason = victoryReason;
    }
}
