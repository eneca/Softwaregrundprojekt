package de.team05.server;

import de.team05.server.control.Server;
import org.pmw.tinylog.Logger;

import java.util.Scanner;

/**
 * The CommandLineController handles commands that are typed in System.in while the Server is
 * running. The controller runs in a different Thread.
 */
class CommandlineController implements Runnable {

    /**
     * Server that runs for this CommandLineController
     */
    private final Server server;

    /**
     * Boolean to state if the CommandLineController is stopped. True if it is stopped, false if
     * it is running.
     */
    private boolean stop = false;

    /**
     * Constructs a CommandLineController for the given Server
     *
     * @param server Server for this CommandLineController
     */
    public CommandlineController(Server server) {
        this.server = server;
    }

    /**
     * Run method for this Runnable. Waits for commands and calls the corresponding methods.
     */
    @Override
    public void run() {
        Scanner scanner = new Scanner(System.in);
        while (!stop) {
            String cmd = scanner.next();
            switch (cmd) {
                case "info":
                    commandInfo();
                    break;
                case "exit":
                case "quit":
                case "q":
                    commandExit();
                    break;
                case "help":
                case "?":
                    commandHelp();
                    break;
                default:
                    commandNotFound();
                    commandHelp();
            }
        }
    }

    /**
     * Prints help
     */
    private void commandHelp() {
        System.out.println("-- Help --");
        System.out.println("info: print complete info about the current state of the server");
        System.out.println("exit: Stop the server and exit the program");
        System.out.println("help: Get this help message");

    }

    /**
     * Called when a command is not found. Prints "Command not found"
     */
    private void commandNotFound() {
        System.out.println("Command not found!");
    }

    /**
     * Stops the Server
     */
    private void commandExit() {
        Logger.info("Stopping server ...");
        try {
            server.stop(1000);
            this.stop = true;
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        System.exit(0);
    }

    /**
     * Prints full info for the server
     */
    private void commandInfo() {
        System.out.println(Strings.ANSI_YELLOW + "INFO START #####################################" + Strings
                .ANSI_RESET);
        System.out.println(server);
        System.out.println(Strings.ANSI_YELLOW + "INFO END   #####################################" + Strings
                .ANSI_RESET);
    }

}
