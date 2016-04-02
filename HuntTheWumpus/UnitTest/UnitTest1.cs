using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void MiniGameTest()
        {
            var minigame = new HuntTheWumpus.MiniGame(1024, 768);
            minigame.InitializeMiniGame(1);
            Assert.AreEqual(minigame.Is_playing, true);
            minigame.InitializeMiniGame(2);
            Assert.AreEqual(minigame.Is_playing, true);
            minigame.InitializeMiniGame(3);
            Assert.AreEqual(minigame.Is_playing, true);
        }
    }
}
