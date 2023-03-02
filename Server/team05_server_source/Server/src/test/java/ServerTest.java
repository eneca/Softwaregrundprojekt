import de.team05.server.control.Server;
import org.junit.jupiter.api.Test;

import java.net.InetSocketAddress;

import static org.junit.jupiter.api.Assertions.assertNotNull;

class ServerTest {

    @Test
    void testConstructor() {
        Server server = new Server(new InetSocketAddress("localhost", 8080), null);
        assertNotNull(server);
    }
}
