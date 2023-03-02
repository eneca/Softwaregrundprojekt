using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [TestFixture]
    class NUnit3
    {
        Random random = new Random();
        Tests t = new Tests();
        //_____________________PartieConfig
        [TestCase]
        public void PartieConfigSuccess() {
            float d = (float)random.NextDouble();
            if (d == 0.0F) {
                d += 0.1F;
            }
            Assert.IsTrue(t.ValidatePartieConfig(random.Next(100), random.Next(100), random.Next(100), random.Next(100), random.Next(100), random.Next(100),d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d, d));
        }
        [TestCase]
        public void PartieConfigFailure()
        {
            Assert.IsFalse(t.ValidatePartieConfig(random.Next(100), random.Next(100), random.Next(100), random.Next(100), random.Next(100), random.Next(100), 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 1.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F, 0.5F));
        }
        //_____________________ValidateConfig
        [TestCase]
        public void ValidateConfigSuccess() {
            String[] sex= new string[5] {"f","m","f","m","f"};
            String[] name= new string[5] {"test", "test33", "test3", "test22", "test2" };
            String[] broom= new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsTrue(t.ValidateConfig("textBoxTeamName","TextBoxMotto",true,2,2,2,1,true,sex,name,broom));
        }
        [TestCase]
        public void ValidateConfigFailureSex() {
            String[] sex = new string[5] { "m", "m", "m", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "motten", true, 2, 2, 2, 1, true, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureBroom()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "motten", true, 2, 2, 2, 1, true, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureName()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "", "", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "motten", true, 2, 2, 2, 1, true, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureImage()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "motten", false, 2, 2, 2, 1, true, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureColor()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "motten", true, 2, 2, 2, 1, false, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureTeamname()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("", "motten", true, 2, 2, 2, 1, false, sex, name, broom));
        }
        [TestCase]
        public void ValidateConfigFailureTeamMotto()
        {
            String[] sex = new string[5] { "f", "m", "f", "m", "f" };
            String[] name = new string[5] { "test", "test33", "test3", "test22", "test2" };
            String[] broom = new string[5] { "Zunderfauch", "Sauberwisch 11", "Komet 2-60", "Nimbus 2001", "Feuerblitz" };
            Assert.IsFalse(t.ValidateConfig("PPsse", "", true, 2, 2, 2, 1, false, sex, name, broom));
        }
    }
}
