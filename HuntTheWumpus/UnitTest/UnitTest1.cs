using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

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

        [TestMethod]
        public void ControlTest()
        {
            var control = new HuntTheWumpus.Control(1024, 768);
            control.UpDate(0);
            control.UpDate(1000000000);
            control.UpDate(100);
        }

        [TestMethod]
        public void DrawTest()
        {
            var view = new HuntTheWumpus.View(1024, 768);
            view.Clear();
            view.Clear(System.Drawing.Color.Aqua);
            view.Created();
            view.Graphics.FillRectangle(Brushes.Aqua, 0, 0, 10, 10);
            view.DrawMainMenu();
        }
    }
}
