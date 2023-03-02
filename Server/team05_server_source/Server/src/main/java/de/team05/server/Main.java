package de.team05.server;

import de.team05.server.control.MatchConfigController;
import de.team05.server.control.Server;
import de.team05.server.model.MatchConfig;
import org.apache.commons.cli.*;
import org.pmw.tinylog.Configurator;
import org.pmw.tinylog.Level;
import org.pmw.tinylog.Logger;
import org.pmw.tinylog.writers.ConsoleWriter;
import org.pmw.tinylog.writers.FileWriter;

import java.net.*;

/**
 * Main is the main class used to run the program. It contains the main method, parses command line arguments, sets
 * up the logging and starts the
 * server. It also starts a command line controller which handles command entered while the server is running.
 */
public class Main {

    private static int verbosity = Config.DEFAULT_VERBOSITY;
    private static int port = Config.DEFAULT_PORT;
    private static String matchConfigPath = Config.DEFAULT_MATCHCONFIG;
    private static String logFilePath = null;

    /**
     * The main method of the program.
     *
     * @param args command line arguments
     */
    public static void main(String[] args) {

        parseArguments(args);

        //start logging
        setupLogger(verbosity, logFilePath);
        Logger.info("This is the Fantastic Feasts Server [VERSION " + Config.VERSION + "]");
        Logger.info("Created by Team05");
        Logger.info("Logging level set to " + Logger.getLevel());

        //load match config
        MatchConfigController matchConfigController = new MatchConfigController(matchConfigPath);
        MatchConfig matchConfig = matchConfigController.getMatchConfig();

        //start the server
        Server server = new Server(new InetSocketAddress(getServerAddress(), port), matchConfig);
        server.setConnectionLostTimeout(0);
        server.start();

        //start a command line controller in a different thread
        CommandlineController commandlineController = new CommandlineController(server);
        Thread commandLineControllerThread = new Thread(commandlineController);
        commandLineControllerThread.start();
    }

    /**
     * Parses the command line options and sets matchconfig path, port and verbosity accordingly
     *
     * @param args string array of command line arguments
     */
    private static void parseArguments(String[] args) {

        //create the options
        Options options = new Options();

        Option helpOption = new Option("h", "help", false, "Print this message");

        Option verbosityOption = new Option("v", "verbosity", true, "Set the verbosity of the program on a scale from" +
                " 0 to " +
                "5, where 0 is no output and 5 is full output");
        verbosityOption.setArgName("0-5");

        Option portOption = new Option("p", "port", true, "Set the port on which the server should be started " +
                "(default: " +
                Config.DEFAULT_PORT + ")");
        portOption.setArgName("port");
        portOption.setType(Integer.class);

        Option matchconfigOption = new Option("m", "match", true, "Set the path for the matchconfig file (default: " +
                "matchconfig.json)");
        matchconfigOption.setArgName("path");

        Option logOption = new Option("l", "log", true, "Set a file path where a full verbosity log (verbosity 5) is " +
                "written to.");
        logOption.setArgName("path");

        options.addOption(helpOption);
        options.addOption(verbosityOption);
        options.addOption(portOption);
        options.addOption(matchconfigOption);
        options.addOption(logOption);

        //parse the arguments
        CommandLineParser parser = new DefaultParser();
        CommandLine cmd = null;
        try {
            cmd = parser.parse(options, args);
        } catch (ParseException e) {
            System.out.println("Something went wrong parsing the command line arguments.");
            System.exit(0);
        }

        //check the help argument
        if (cmd.hasOption("h")) {
            HelpFormatter formatter = new HelpFormatter();
            formatter.printHelp("server", options);
            System.exit(0);
        } else {

            //check the matchconfig argument
            if (cmd.hasOption("m")) {
                matchConfigPath = cmd.getOptionValue("m");
            }

            //check the port argument
            if (cmd.hasOption("p")) {
                try {
                    port = Integer.parseInt(cmd.getOptionValue("p"));
                } catch (NumberFormatException e) {
                    System.out.println("Invalid port. Must be an integer from 0 to 65535");
                    System.exit(0);
                }
                if (port > 65535 || port < 0) {
                    System.out.println("Invalid port. Must be an integer from 0 to 65535");
                    System.exit(0);
                }
            }

            //check the verbosity argument
            if (cmd.hasOption("v")) {
                try {
                    verbosity = Integer.parseInt(cmd.getOptionValue("v"));
                } catch (NumberFormatException e) {
                    System.out.println("Invalid verbosity. Must be an integer from 0 to 5");
                    System.exit(0);
                }
            }

            //check the log argument
            if(cmd.hasOption("l")){
                logFilePath = cmd.getOptionValue("l");
            }
        }
    }

    /**
     * Determines this devices local IP address.
     *
     * @return String of the IP address of this device
     */
    private static String getServerAddress() {
        String ip = null;
        try (final DatagramSocket socket = new DatagramSocket()) {
            socket.connect(InetAddress.getByName("8.8.8.8"), 10002);
            ip = socket.getLocalAddress().getHostAddress();
        } catch (SocketException | UnknownHostException e) {
            e.printStackTrace();
        }
        return ip;
    }

    /**
     * Sets up the logger configuration used for all global logging
     *
     * @param verbosity Verbosity of the program:
     *                  0 = Level.OFF
     *                  1 = Level.ERROR
     *                  2 = Level.WARNING
     *                  3 = Level.INFO
     *                  4 = Level.DEBUG
     *                  5 = Level.TRACE
     * @param logFile path of the logfile or null if the logger should not write to a logfile
     */
    private static void setupLogger(int verbosity, String logFile) {
        Level[] levels = Level.values();

        Configurator configurator = Configurator.defaultConfig()
                .writer(new ConsoleWriter(), levels[5 - verbosity])
                .formatPattern("{level}: {message} ({class}.{method})");
        if (logFile != null) {
            configurator.addWriter(new FileWriter(logFile), Level.TRACE);
        }

        configurator.activate();

        Logger.info("Logging to file: " + logFile);
    }

    /**
     * Sets up the logger configuration used for all global logging, without writing to a log file.
     *
     * @param verbosity Verbosity of the program:
     *                  0 = Level.OFF
     *                  1 = Level.ERROR
     *                  2 = Level.WARNING
     *                  3 = Level.INFO
     *                  4 = Level.DEBUG
     *                  5 = Level.TRACE
     */

    public static void setupLogger(int verbosity) {
        setupLogger(verbosity, null);
    }
}
