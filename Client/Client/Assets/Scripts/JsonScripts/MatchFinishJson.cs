using System;

[Serializable]
public class MatchFinishJson
{
    public int endRound;
    public int leftPoints;
    public int rightPoints;
    public string winnerUserName;
    public string victoryReason;

    //public MatchFinishJson(int endRound, int leftPoints, int rightPoints, string winnerUserName, string victoryReason)
    //{
    //    this.endRound = endRound;
    //    this.leftPoints = leftPoints;
    //    this.rightPoints = rightPoints;
    //    this.winnerUserName = winnerUserName;
    //    this.victoryReason = victoryReason;
    //}
}