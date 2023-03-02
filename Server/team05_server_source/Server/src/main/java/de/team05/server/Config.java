package de.team05.server;

import de.team05.server.model.Square;

/**
 * Config contains different configuration values for the Server.
 */
public class Config {
    public static final String VERSION = "1.0";

    //Ruben
    public static final int TEAM_NAME_UPPER_CHAR_LIMIT = 40;
    public static final int TEAM_NAME_LOWER_CHAR_LIMIT = 3;
    public static final int VALIDATION_FAILED = -1;
    public static final int VALIDATION_FIRST_SUCCESS = 1;
    public static final int VALIDATION_BOTH_SUCCESS = 2;


    //Vulpix
    //Torringfelder 2,4 2,6 2,8 14,4 14,6, 14,8

    public static final Square LEFT_GOALPOST_1 = new Square(2, 4);
    public static final Square LEFT_GOALPOST_2 = new Square(2, 6);
    public static final Square LEFT_GOALPOST_3 = new Square(2, 8);
    public static final Square RIGHT_GOALPOST_1 = new Square(14, 4);
    public static final Square RIGHT_GOALPOST_2 = new Square(14, 6);
    public static final Square RIGHT_GOALPOST_3 = new Square(14, 8);

    public static final Square MIDDLE_SQUARE = new Square(8, 6);

    //Soeren
    public static final int MAX_ROUNDS_MAX_LIMIT = 100;
    public static final int MAX_ROUNDS_MIN_LIMIT = 13;
    public static final int MAXIMUM_TIMEOUT_LIMIT = Integer.MAX_VALUE;
    public static final int DEFAULT_PORT = 18080;//18080
    public static final int DEFAULT_VERBOSITY = 3;
    public static final String DEFAULT_MATCHCONFIG = "matchconfig.json";
    public static final int SNITCH_POINTS = 30;
    public static final int GOAL_POINTS = 10;
    public static final int SNITCH_START_ROUND = 13;
    public static final int TIMEOUT_BUFFER = 500;
}
