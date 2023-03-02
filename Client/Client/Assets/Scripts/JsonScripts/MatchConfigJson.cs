using System;

[Serializable]
public class MatchConfigJson
{
    public int maxRounds;
    public TimeoutsJson timings;
    public ProbabilitiesJson probabilities;

    [Serializable]
    public class TimeoutsJson
    {
        public int teamFormationTimeout;
        public int playerTurnTimeout;
        public int fanTurnTimeout;
        public int minPlayerPhaseAnimationDuration;
        public int minFanPhaseAnimationDuration;
        public int minBallPhaseAnimationDuration;
    }

    [Serializable]
    public class ProbabilitiesJson
    {
        public float throwSuccess;
        public float knockOut;
        public float foolAway;
        public float catchSnitch;
        public float catchQuaffle;
        public float wrestQuaffle;
        public ExtraMoveJson extraMove;
        public FoulDetectionJson foulDetection;
        public FanFoulDetectionJson fanFoulDetection;
    }

    [Serializable]
    public class ExtraMoveJson
    {
        public float tinderblast;
        public float cleansweep11;
        public float comet260;
        public float nimbus2001;
        public float firebolt;
    }

    [Serializable]
    public class FoulDetectionJson
    {
        public float flacking;
        public float haversacking;
        public float stooging;
        public float blatching;
        public float snitchnip;
    }

    [Serializable]
    public class FanFoulDetectionJson
    {
        public float elfTeleportation;
        public float goblinShock;
        public float trollRoar;
        public float snitchSnatch;
    }
}
