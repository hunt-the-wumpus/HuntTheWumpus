using System;
using System.Linq;
using System.Collections.Generic;
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
            view.MainForm.DrawAll();
        }

        [TestMethod]
        public void GoodMap()
        {
            var map = new HuntTheWumpus.Map();
            bool[] b = new bool[30];
            b[0] = true;
            bool fl = true;
            Queue<int> q = new Queue<int>();
            q.Enqueue(0);
            int step = 0;
            while (q.Count > 0)
            {
                int top = q.Dequeue();
                int cnt = 0;
                ++step;
                for (int i = 0; i < map.graph[i].Count; ++i)
                    if (map.isActive[top][i])
                    {
                        ++cnt;
                        if (!b[map.graph[top][i]])
                        {
                            b[map.graph[top][i]] = true;
                            q.Enqueue(map.graph[top][i]);
                        }
                    }
                Assert.IsTrue(cnt <= 3, "Degree > 3");
            }
            Assert.IsTrue(step == 30, "Graph isn't connected");
        }
    }
}
