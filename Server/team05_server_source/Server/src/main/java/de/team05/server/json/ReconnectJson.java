package de.team05.server.json;

public class ReconnectJson {
    ContainerJson matchStart;
    ContainerJson snapshot;
    ContainerJson next;

    public ReconnectJson(ContainerJson matchStart, ContainerJson snapshot, ContainerJson next) {
        this.matchStart = matchStart;
        this.snapshot = snapshot;
        this.next = next;
    }

    @Override
    public String toString() {
        return "ReconnectJson{" +
                "matchStart=" + matchStart +
                ", snapshot=" + snapshot +
                ", next=" + next +
                '}';
    }
}
