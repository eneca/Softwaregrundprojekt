using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Tests
    {
        public bool ValidatePartieConfig(int maxRounds, int playerTurnTimeout,int fanTurnTimeout, int playerPhaseTime, int fanPhaseTime, int ballPhaseTime,
            float goal, float throwSuccess, float knockOut, float foolAway, float catchSnitch, float catchQuaffle, float wrestQuaffle,
            float thinderblast, float cleansweep11, float comet260, float nimbus2001, float firebolt,
            float flacking, float haversacking, float stooging, float blatching, float snitchnip,
            float elfTeleportation, float goblinShock, float trollRoar, float snitchSnatch
            )
        {
            bool returnValue = true;

            if (maxRounds <= 0) returnValue = false;
            if (playerTurnTimeout <= 0) returnValue = false;
            if (fanTurnTimeout <= 0) returnValue = false;
            if (playerPhaseTime <= 0) returnValue = false;
            if (fanPhaseTime <= 0) returnValue = false;
            if (ballPhaseTime <= 0) returnValue = false;

            if (goal <= 0 || goal > 1) returnValue = false;
            if (throwSuccess <= 0 || throwSuccess > 1) returnValue = false;
            if (knockOut <= 0 || knockOut > 1) returnValue = false;
            if (foolAway <= 0 || foolAway > 1) returnValue = false;
            if (catchSnitch <= 0 || catchSnitch > 1) returnValue = false;
            if (catchQuaffle <= 0 || catchQuaffle > 1) returnValue = false;
            if (wrestQuaffle <= 0 || wrestQuaffle > 1) returnValue = false;

            if (thinderblast <= 0 || thinderblast > 1) returnValue = false;
            if (cleansweep11 <= 0 ||cleansweep11 > 1) returnValue = false;
            if (comet260 <= 0 || comet260 > 1) returnValue = false;
            if (nimbus2001 <= 0 || nimbus2001 > 1) returnValue = false;
            if (firebolt <= 0 || firebolt > 1) returnValue = false;

            if (flacking <= 0 || flacking > 1) returnValue = false;
            if (haversacking <= 0 || haversacking > 1) returnValue = false;
            if (stooging <= 0 || stooging > 1) returnValue = false;
            if (blatching <= 0 || blatching > 1) returnValue = false;
            if (snitchnip <= 0 || snitchnip > 1) returnValue = false;

            if (elfTeleportation <= 0 || elfTeleportation > 1) returnValue = false;
            if (goblinShock <= 0 || goblinShock > 1) returnValue = false;
            if (trollRoar <= 0 || trollRoar > 1) returnValue = false;
            if (snitchSnatch <= 0 || snitchSnatch > 1) returnValue = false;

            return returnValue;
        }

        public bool ValidateConfig(string textBoxTeamname, string textBoxMotto, bool colorPicked, int elves, int trolls, int goblins, int nifflers, bool img, string[] sexes,string[] names, string[]broomsSelected)
        {
            bool valid = true;
            List<string> problems = new List<string>();

            if (textBoxTeamname == "" || textBoxTeamname == "Insert Team-Name" || textBoxTeamname.Length < 4)
            {
                problems.Add("Missing Team-Name");
                valid = false;
            }
            if (textBoxMotto == "" || textBoxMotto == "Insert Motto" || textBoxMotto.Length < 4)
            {
                problems.Add("Missing Motto");
                valid = false;
            }
            if (!colorPicked)
            {
                problems.Add("Missing Color");
                valid = false;
            }
            int fans = elves + trolls + goblins + nifflers;
            if (fans != 7)
            {
                problems.Add("Wrong number of Fans");
                valid = false;
            }
            if (!img)
            {
                problems.Add("Missing Image");
                valid = false;
            }
            int gender = 0;
            bool[] brooms = new bool[5];
            for (int x = 0; x < 5; x++)
            {
                if (sexes[x] == "f")
                {
                    gender--;
                }
                else if (sexes[x] == "m")
                {
                    gender++;
                }
                else
                {
                    int c = x + 1;
                    problems.Add("Missing Gender at Player: " + c);
                    valid = false;
                }
                if (names[x] == "" || names[x] == "Insert Name" || names[x].Length < 3)
                {
                    int c = x + 1;
                    problems.Add("Wrong or None Name at Player: " + c);
                    valid = false;
                }

                switch (broomsSelected[x])
                {
                    case "Zunderfauch":
                        brooms[0] = true;
                        break;
                    case "Sauberwisch 11":
                        brooms[1] = true;
                        break;
                    case "Komet 2-60":
                        brooms[2] = true;
                        break;
                    case "Nimbus 2001":
                        brooms[3] = true;
                        break;
                    case "Feuerblitz":
                        brooms[4] = true;
                        break;
                    case "":
                        int c = x + 1;
                        problems.Add("Missing Broom at Player: " + c);
                        valid = false;
                        break;
                }
            }
            if (!(brooms[0] && brooms[1] && brooms[2] && brooms[3] && brooms[4]))
            {
                problems.Add("You need each broom at least one time");
                valid = false;
            }

            if (gender < -1)
            {
                problems.Add("Too many Females");
                valid = false;
            }
            if (gender > 1)
            {
                problems.Add("Too many Males");
                valid = false;
            }

            string message = "Your Configuration wasn't valid:\n";

            if (!valid)
            {
                foreach (string s in problems)
                {
                    message += "     " + s + "\n";
                }
            }
            return valid;
        }

    }
}
