package de.team05.server.json;

public class NextJson {
    public String turn;
    public String type;
    public Integer timeout;

    public NextJson(String turn, String type, Integer timeout) {
        this.turn = turn;
        this.type = type;
        this.timeout = timeout;
    }

    //needed for GSON
    public NextJson() {
    }
}
