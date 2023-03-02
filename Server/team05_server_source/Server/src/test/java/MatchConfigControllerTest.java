import de.team05.server.control.MatchConfigController;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class MatchConfigControllerTest {

    @Test
    void getValidMatchConfig() {

        MatchConfigController m1 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig1.json");
        assertFalse(m1.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m2 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig2.json");
        assertFalse(m2.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m3 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig3.json");
        assertFalse(m3.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m4 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig4.json");
        assertFalse(m4.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m5 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig5.json");
        assertFalse(m5.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m6 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig6.json");
        assertFalse(m6.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m7 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig7.json");
        assertFalse(m7.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m8 = new MatchConfigController("src/test/json/matchConfig/invalid_matchConfig8.json");
        assertFalse(m8.validated, "FAIL: False MatchConfig was successful validated.");

        MatchConfigController m9 = new MatchConfigController("src/test/json/matchConfig/valid_matchConfig1.json");
        assertTrue(m9.validated, "FAIL: Correct MatchConfig was not successful validated.");

        MatchConfigController m10 = new MatchConfigController("src/test/json/matchConfig/valid_matchConfig2.json");
        assertTrue(m10.validated, "FAIL: Correct MatchConfig was not successful validated.");

        MatchConfigController m11 = new MatchConfigController("src/test/json/matchConfig/valid_matchConfig3.json");
        assertTrue(m11.validated, "Correct MatchConfig was successful validated.");

        MatchConfigController m12 = new MatchConfigController("src/test/json/matchConfig/valid_matchConfig4.json");
        assertTrue(m12.validated, "FAIL: Correct MatchConfig was not successful validated.");

    }
}