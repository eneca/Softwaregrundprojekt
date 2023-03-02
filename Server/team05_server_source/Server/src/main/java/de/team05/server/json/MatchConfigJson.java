package de.team05.server.json;

public class MatchConfigJson {

    public Integer maxRounds;
    public TimeoutsJson timings;
    public ProbabilitiesJson probabilities;

    public class TimeoutsJson {
        public Integer teamFormationTimeout;
        public Integer playerTurnTimeout;
        public Integer fanTurnTimeout;
        public Integer minPlayerPhaseAnimationDuration;
        public Integer minFanPhaseAnimationDuration;
        public Integer minBallPhaseAnimationDuration;
    }

    public class ProbabilitiesJson {
        public Float throwSuccess;
        public Float knockOut;
        public Float foolAway;
        public Float catchSnitch;
        public Float catchQuaffle;
        public Float wrestQuaffle;
        public ExtraMoveJson extraMove;
        public FoulDetectionJson foulDetection;
        public FanFoulDetectionJson fanFoulDetection;
    }

    public class ExtraMoveJson {
        public Float tinderblast;
        public Float cleansweep11;
        public Float comet260;
        public Float nimbus2001;
        public Float firebolt;
    }

    public class FoulDetectionJson {
        public Float flacking;
        public Float haversacking;
        public Float stooging;
        public Float blatching;
        public Float snitchnip;
    }

    public class FanFoulDetectionJson {

        public Float elfTeleportation;
        public Float goblinShock;
        public Float trollRoar;
        public Float snitchSnatch;
        public Float wombatPoo;
    }
}
