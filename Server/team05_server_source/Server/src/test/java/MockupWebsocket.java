import org.java_websocket.WebSocket;
import org.java_websocket.drafts.Draft;
import org.java_websocket.enums.Opcode;
import org.java_websocket.enums.ReadyState;
import org.java_websocket.framing.Framedata;

import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.util.Collection;
import java.util.LinkedList;

public class MockupWebsocket implements WebSocket {
    private final LinkedList<String> lastMessages;
    boolean closed = false;

    public MockupWebsocket() {
        lastMessages = new LinkedList<>();
    }

    public String getMessage() {
        return lastMessages.pollFirst();
    }

    public boolean hasMessage() {
        return !lastMessages.isEmpty();
    }


    public LinkedList<String> getMessages() {
        return lastMessages;
    }

    @Override
    public void close(int code, String message) {
        closed = true;
    }

    @Override
    public void close(int code) {
        closed = true;
    }

    @Override
    public void close() {
        closed = true;
    }

    @Override
    public void closeConnection(int code, String message) {
        closed = true;
    }

    @Override
    public void send(String text) {
        lastMessages.add(text);
    }

    @Override
    public void send(ByteBuffer bytes) {

    }

    @Override
    public void send(byte[] bytes) {
        lastMessages.add(new String(bytes));
    }

    @Override
    public void sendFrame(Framedata framedata) {

    }

    @Override
    public void sendFrame(Collection<Framedata> frames) {

    }

    @Override
    public void sendPing() {

    }

    @Override
    public void sendFragmentedFrame(Opcode op, ByteBuffer buffer, boolean fin) {

    }

    @Override
    public boolean hasBufferedData() {
        return false;
    }

    @Override
    public InetSocketAddress getRemoteSocketAddress() {
        return null;
    }

    @Override
    public InetSocketAddress getLocalSocketAddress() {
        return null;
    }

    @Override
    public boolean isOpen() {
        return false;
    }

    @Override
    public boolean isClosing() {
        return false;
    }

    @Override
    public boolean isFlushAndClose() {
        return false;
    }

    @Override
    public boolean isClosed() {
        return false;
    }

    @Override
    public Draft getDraft() {
        return null;
    }

    @Override
    public ReadyState getReadyState() {
        return null;
    }

    @Override
    public String getResourceDescriptor() {
        return null;
    }

    @Override
    public <T> void setAttachment(T attachment) {

    }

    @Override
    public <T> T getAttachment() {
        return null;
    }

}
