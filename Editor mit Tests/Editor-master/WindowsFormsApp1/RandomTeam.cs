using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /*Klasse zum generieren eines zufälligen Teams*/
    class RandomTeam
    {
        private List<string> FemaleNames;
        private List<string> MaleNames;
        private List<string> Teamname;
        private List<string> Motto;
        private List<string> Brooms;
        private List<Image> Logos;
        private Color[] TrikotColor;

        /*Konstruktor mit festen Namen*/
        public RandomTeam()
        {
            FemaleNames = new List<string> {"Emily","Madison","Emma","Hannah","Olivia","Abigail","Isabella","Ashley",
                "Samantha","Elizabeth","Alexis","Sarah","Alyssa","Grace","Sophia","Taylor","Brianna","Lauren","Ava",
                "Kayla","Jessica","Natalie","Chloe","Anna","Victoria","Hailey","Mia","Sydney","Jasmine","Morgan",
                "Julia","Destiny","Rachel","Megan","Kaitlyn","Katherine","Jennifer","Savannah","Ella","Alexandra",
                "Haley","Allison","Maria","Nicole","Mackenzie","Brooke","Makayla","Kaylee","Lily","Stephanie","Andrea",
                "Faith","Amanda","Katelyn","Madeline","Gabrielle","Zoe","Trinity","Alexa","Mary","Jenna","Lillian",
                "Paige","Kylie","Gabriella","Rebecca","Jordan","Sara","Addison","Michelle","Riley","Vanessa","Angelina",
                "Leah","Caroline","Sofia","Audrey","Maya","Avery","Evelyn","Autumn","Amber","Ariana","Jocelyn","Claire",
                "Jada","Danielle","Bailey","Isabel","Arianna","Sierra","Mariah","Aaliyah","Melanie","Erin","Nevaeh",
                "Brooklyn","Marissa","Jacqueline"};

            MaleNames = new List<string> {"Jacob","Michael","Joshua","Matthew","Christopher","Andrew","Daniel","Ethan",
                "Joseph","William","Anthony","Nicholas","David","Alexander","Ryan","Tyler","James","John","Jonathan",
                "Brandon","Christian","Dylan","Zachary","Noah","Samuel","Benjamin","Nathan","Logan","Justin","Jose",
                "Gabriel","Austin","Kevin","Caleb","Robert","Elijah","Thomas","Jordan","Cameron","Hunter","Jack","Angel",
                "Isaiah","Jackson","Evan","Luke","Jason","Isaac","Mason","Aaron","Connor","Gavin","Kyle","Jayden","Aidan",
                "Juan","Luis","Charles","Aiden","Adam","Brian","Eric","Lucas","Sean","Nathaniel","Alex","Adrian","Carlos",
                "Bryan","Ian","Jesus","Owen","Julian","Cole","Andon","Diego","Steven","Chase","Timothy","Jeremiah",
                "Sebastian","Xavier","Devin","Cody","Seth","Hayden","Blake","Richard","Carter","Wyatt","Dominic",
                "Antonio","Jaden","Miguel","Brayden","Patrick","Alejandro","Carson","Jesse","Tristan"};


            Teamname = new List<string> { "Die Vollpfosten", "Demolition Dudes", "Angry Nerds", "The Big Beer Theory"};

            Motto = new List<string> {"Ich denke niemals an die Zukunft, sie kommt früh genug",
                "Du kannst den Wind nicht ändern, aber Du kannst die Segel anders setzen",
                "Wenn Deine Träume Dir keine Angst machen, sind sie nicht groß genug",
                "Wege entstehen dadurch, dass wir sie gehen",
                "Die Zukunft hängt von dem ab, was Du heute tust",
                "Das Leben beginnt am Ende Deiner Komfortzone",
                "Der leichte Weg ist auch der richtige Weg",
                "Mut steht am Anfang des Handelns, Glück am Ende",
                "Einfachheit ist die höchste Stufe der Vollendung",
                "Gibt es etwa eine bessere Motivation als den Erfolg?",
                "Besessenheit ist der Motor – Verbissenheit ist die Bremse"};

            Brooms = new List<string> { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };

            Logos = new List<Image> { global::WindowsFormsApp1.Properties.Resources.Logo1,
                                        global::WindowsFormsApp1.Properties.Resources.Logo2,
                                        global::WindowsFormsApp1.Properties.Resources.Logo3}


;           TrikotColor = new Color[2];
        }

        /*erstellen einer zufälligen aber validen Team-config*/
        public OutputData RandomTeamComposition()
        {
            Random random = new Random();
            TrikotColor[0] = Color.FromArgb(255, random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
            TrikotColor[1] = Color.FromArgb(255, 255 - TrikotColor[0].R, 255 - TrikotColor[0].G, 255 - TrikotColor[0].B);
            Colors colors = new Colors(TrikotColor[0],TrikotColor[1]);
            Image image = Logos[random.Next(0,Logos.Count)];
            int[] fans = new int[] { 1, 1, 1, 1 };

            for (int x=0;x<3;x++)
            {
                int ran = random.Next(0, 4);
                fans[ran] = fans[ran] + 1;
            }
            Fans fan = new Fans(fans[0], fans[1], fans[2], fans[3]);
            string teamname = Teamname[random.Next(0,Teamname.Count)];
            string motto = Motto[random.Next(0, Motto.Count)];

            string[] name = new string[7];
            string[] broom = new string[7];
            string[] sex = new string[7];

            int offset = random.Next(0,10);
            int genderValid=0;
            for (int x = 0; x < 7; x++)
            {
                int gender = random.Next(0, 2);
                if (genderValid > 1)
                {
                    gender = 1;
                }
                if (genderValid < 1)
                {
                    gender = 0;
                }
                if (gender == 0)
                {
                    genderValid++;
                    name[x] = MaleNames[random.Next(0, MaleNames.Count)];
                    broom[x] = Brooms[(x+offset) % 5];
                    sex[x] = "m";
                }else{
                    genderValid--;
                    name[x] = FemaleNames[random.Next(0, FemaleNames.Count)];
                    broom[x] = Brooms[(x+offset) % 5];
                    sex[x] = "f";
                }
            }

            Players pl = new Players(new Data(name, broom, sex));


            Bitmap im = new Bitmap(image, 256, 256);
            string base64;
            using (MemoryStream ms = new MemoryStream())
            {
                im.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                base64 = Convert.ToBase64String(imageBytes);
                ms.Close();
            }

            return new OutputData(teamname, motto, colors, base64, fan, pl);
        }

    }
}
