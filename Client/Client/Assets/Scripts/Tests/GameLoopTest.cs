using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameLoopTest
    {
        #region CanHitGoal
        [Test]
        public void CheckScript_CanHitGoal_Success_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            //example 1
            //Player pos x = 4 y= 2 
            //Goal Pos x = 2 y = 4
            Assert.IsTrue(cs.CanHitGoal(new Vector3Int(4,2,0), new Vector3Int(2,4,0)));
        }
        [Test]
        public void CheckScript_CanHitGoal_Success_2()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            //example 1
            //Player pos x =4 y = 5
            //Goal Pos x = 2 y = 4
            Assert.IsTrue(cs.CanHitGoal(new Vector3Int(4, 5, 0), new Vector3Int(2, 4, 0)));
        }
        [Test]
        public void CheckScript_CanHitGoal_Fail_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            //example 1
            //Player pos x =4 y = 5
            //Goal Pos x = 2 y = 8
            Assert.IsFalse(cs.CanHitGoal(new Vector3Int(4, 11, 0), new Vector3Int(2, 8, 0)));
        }
        #endregion CanHitGoal
        #region PosIsGoal
        [Test]
        ///*
        ///Goal Positions Are
        ///Goal left 1 x = 2 y = 4
        ///Goal left 2 x = 2 y = 6
        ///Goal left 3 x = 2 y = 8
        ///Goal right 1 x = 14 y = 4
        ///Goal right 2 x = 14 y = 6
        ///Goal right 3 x = 14 y = 8
        public void CheckScript_PosIsGoal_Success_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(2, -4, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Success_2()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(2, -6, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Success_3()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(2, -8, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Success_4()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(14, -4, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Success_5()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(14, -6, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Success_6()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsTrue(cs.PosIsGoal(new Vector3Int(14, -8, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Fail_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsFalse(cs.PosIsGoal(new Vector3Int(10, -8, 0)));
        }
        [Test]
        public void CheckScript_PosIsGoal_Fail_2()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.IsFalse(cs.PosIsGoal(new Vector3Int(14, -1, 0)));
        }
        #endregion PosIsGoal
        #region HuntersInDefenseZone
        [Test]
        public void CheckScript_HuntersInDefenseZoneLeft_Success_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)2, (int)cs.HuntersInDefenseZoneLeft(new Player("", "", "", new Vector3Int(0, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(0, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneLeft_Success_2()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)1, (int)cs.HuntersInDefenseZoneLeft(new Player("", "", "", new Vector3Int(0, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneLeft_Success_3()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)3, (int)cs.HuntersInDefenseZoneLeft(new Player("", "", "", new Vector3Int(0, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(0, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(0, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneLeft_Success_4()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)0, (int)cs.HuntersInDefenseZoneLeft(new Player("", "", "", new Vector3Int(6, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(7, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneRight_Success_1()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)2, (int)cs.HuntersInDefenseZoneRight(new Player("", "", "", new Vector3Int(14, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(15, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneRight_Success_2()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)1, (int)cs.HuntersInDefenseZoneRight(new Player("", "", "", new Vector3Int(15, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(6, 6, 0), false, Role.Chaser1, false, false)));
        }
        [Test]
        public void CheckScript_HuntersInDefenseZoneRight_Success_3()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)3, (int)cs.HuntersInDefenseZoneRight(new Player("", "", "", new Vector3Int(14, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(15, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(14, 6, 0), false, Role.Chaser1, false, false)));
        }
        public void CheckScript_HuntersInDefenseZoneRight_Success_4()
        {
            //Example taken from specifications
            CheckScript cs = new CheckScript();
            Assert.AreEqual((int)0, (int)cs.HuntersInDefenseZoneRight(new Player("", "", "", new Vector3Int(0, 4, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(0, 5, 0), false, Role.Chaser1, false, false), new Player("", "", "", new Vector3Int(0, 6, 0), false, Role.Chaser1, false, false)));
        }
        #endregion HuntersInDefenseZone
        #region ResolveNext
        [Test]
        public void ResolveNext_Left_1()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Seeker:move", tm.ResolveNext("move","leftSeeker",true,true));
        }
        [Test]
        public void ResolveNext_Left_2()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Keeper:move", tm.ResolveNext("move", "leftKeeper", true, true));
        }
        [Test]
        public void ResolveNext_Left_3()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser1:move", tm.ResolveNext("move", "leftChaser1", true, true));
        }
        [Test]
        public void ResolveNext_Left_4()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser2:action", tm.ResolveNext("action", "leftChaser2", true, true));
        }
        [Test]
        public void ResolveNext_Left_5()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser3:move", tm.ResolveNext("move", "leftChaser3", true, true));
        }
        [Test]
        public void ResolveNext_Left_6()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Beater1:move", tm.ResolveNext("move", "leftBeater1", true, true));
        }
        [Test]
        public void ResolveNext_Left_7()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Beater2:move", tm.ResolveNext("move", "leftBeater2", true, true));
        }
        [Test]
        public void ResolveNext_Left_8()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("leftNiffler:fan", tm.ResolveNext("fan", "leftNiffler", true, true));
        }
        [Test]
        public void ResolveNext_Left_9()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("leftWombat:fan", tm.ResolveNext("fan", "leftWombat", true, true));
        }
        [Test]
        public void ResolveNext_Left_10()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("leftGoblin:fan", tm.ResolveNext("fan", "leftGoblin", true, true));
        }
        [Test]
        public void ResolveNext_Left_11()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("leftElf:fan", tm.ResolveNext("fan", "leftElf", true, true));
        }
        [Test]
        public void ResolveNext_Left_12()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("leftTroll:fan", tm.ResolveNext("fan", "leftTroll", true, true));
        }
        [Test]
        public void ResolveNext_Right_1()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Seeker:move", tm.ResolveNext("move", "rightSeeker", false, true));
        }
        [Test]
        public void ResolveNext_Right_2()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Keeper:move", tm.ResolveNext("move", "rightKeeper", false, true));
        }
        [Test]
        public void ResolveNext_Right_3()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser1:move", tm.ResolveNext("move", "rightChaser1", false, true));
        }
        [Test]
        public void ResolveNext_Right_4()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser2:action", tm.ResolveNext("action", "rightChaser2", false, true));
        }
        [Test]
        public void ResolveNext_Right_5()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Chaser3:move", tm.ResolveNext("move", "rightChaser3", false, true));
        }
        [Test]
        public void ResolveNext_Right_6()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Beater1:move", tm.ResolveNext("move", "rightBeater1", false, true));
        }
        [Test]
        public void ResolveNext_Right_7()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("Beater2:move", tm.ResolveNext("move", "rightBeater2", false, true));
        }
        [Test]
        public void ResolveNext_Right_8()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("rightNiffler:fan", tm.ResolveNext("fan", "rightNiffler", false, true));
        }
        [Test]
        public void ResolveNext_Right_9()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("rightWombat:fan", tm.ResolveNext("fan", "rightWombat", false, true));
        }
        [Test]
        public void ResolveNext_Right_10()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("rightGoblin:fan", tm.ResolveNext("fan", "rightGoblin", false, true));
        }
        [Test]
        public void ResolveNext_Right_11()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("rightElf:fan", tm.ResolveNext("fan", "rightElf", false, true));
        }
        [Test]
        public void ResolveNext_Right_12()
        {
            TestMono tm = new TestMono();
            StringAssert.AreEqualIgnoringCase("rightTroll:fan", tm.ResolveNext("fan", "rightTroll", false, true));
        }
        #endregion ResolveNext
        #region CheckCircle
        [Test]
        public void TestCircle_Success_1()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(7, -5, 0)));
        }
        [Test]
        public void TestCircle_Success_2()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(7, -6, 0)));
        }
        [Test]
        public void TestCircle_Success_3()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(7, -7, 0)));
        }
        [Test]
        public void TestCircle_Success_4()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(8, -5, 0)));
        }
        [Test]
        public void TestCircle_Success_5()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(8, -6, 0)));
        }
        [Test]
        public void TestCircle_Success_6()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(8, -7, 0)));
        }
        [Test]
        public void TestCircle_Success_7()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(9, -5, 0)));
        }
        [Test]
        public void TestCircle_Success_8()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(9, -6, 0)));
        }
        [Test]
        public void TestCircle_Success_9()
        {
            TestMono tm = new TestMono();
            Assert.IsTrue(tm.CheckCircle(new Vector3Int(9, -7, 0)));
        }
        [Test]
        public void TestCircle_Failure_1()
        {
            TestMono tm = new TestMono();
            Assert.IsFalse(tm.CheckCircle(new Vector3Int(2, -7, 0)));
        }
        public void TestCircle_Failure_2()
        {
            TestMono tm = new TestMono();
            Assert.IsFalse(tm.CheckCircle(new Vector3Int(7, -11, 0)));
        }
        public void TestCircle_Failure_3()
        {
            TestMono tm = new TestMono();
            Assert.IsFalse(tm.CheckCircle(new Vector3Int(5, -4, 0)));
        }
        #endregion CheckCircle
    }
}
