using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using HuntTheWumpus;
using Xunit;

namespace xUnit.Tests
{
    public class Tests
    {
        [Fact]
        public void MiniGameTest()
        {
            var minigame = new HuntTheWumpus.MiniGame(1024, 768);
            minigame.InitializeMiniGame(1);
            Assert.True(minigame.Is_playing);
            minigame.InitializeMiniGame(2);
            Assert.True(minigame.Is_playing);
            minigame.InitializeMiniGame(3);
            Assert.True(minigame.Is_playing);
        }

        [Fact]
        public void ControlTest()
        {
            var control = new HuntTheWumpus.Control(1024, 768);
            control.UpDate(0);
            control.UpDate(100000000);
            control.UpDate(100);
        }

        [Fact]
        public void DrawTest()
        {
            var view = new HuntTheWumpus.View(1024, 768);
            view.Clear();
            view.Clear(System.Drawing.Color.Aqua);
            view.Created();
            view.Graphics.FillRectangle(Brushes.Aqua, 0, 0, 10, 10);
            view.DrawMainMenu();
            var map = new HuntTheWumpus.Map();
            view.DrawPickCave(map.graph, map.isActive, 0);
            view.MainForm.DrawAll();
        }

        [Fact]
        public void GoodMap()
        {
            var map = new HuntTheWumpus.Map();
            bool[] b = new bool[30];
            b[0] = true;
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
                Assert.True(cnt <= 3, "Degree > 3");
            }
            Assert.True(step == 30, "Graph isn't connected");
        }

        [Fact]
        public void MapTest()
        {
            var map = new HuntTheWumpus.Map();
            int mem = map.Wumpus;
            map.WumpusGoAway();
            Assert.False(mem == map.Wumpus, "Wumpus come back");
            for (int i = 0; i < 6; ++i)
                map.PushArrow(i);
            for (int i = 0; i < 6; ++i)
                map.Move(i);
            map.Respaw();
            for (int i = 0; i < 10; ++i)
            {
                var bat = map.GetBat();
                var pit = map.GetPit();
                Assert.True(bat != pit, "Bat in pit");
            }
        }

        [Fact]
        public void PlayerTest()
        {
            var player = new HuntTheWumpus.Player();
            int mem = player.Coins;
            player.AddCoins(100);
            player.AddCoins(0);
            Assert.True(mem + 100 == player.Coins, "AddCoins isn't correct");
            mem += 100;
            player.BuyArrow();
            Assert.True(mem > player.Coins, "BuyArrow isn't correct");
            mem = player.Coins;
            player.BuyHint();
            Assert.True(mem > player.Coins, "BuyHint isn't correct");
            int ch = 0;
            while (player.CanBuyArrow() && ch < 1000)
            {
                player.BuyArrow();
                ++ch;
            }
            Assert.True(ch < 1000, "CanBuyArrow isn't correct");
            player.AddCoins(100);
            ch = 0;
            while (player.CanBuyHint() && ch < 1000)
            {
                player.BuyHint();
                ++ch;
            }
            Assert.True(ch < 1000, "CanBuyHint isn't correct");
        }

        [Fact]
        public void AchivTest()
        {
            var view = new HuntTheWumpus.View(1024, 768);
            var score = new HuntTheWumpus.Scores(1024, 768);
            score.DrawScores(view.Graphics);
            var minigame = new HuntTheWumpus.MiniGame(1024, 768);
            List<string> ls = new List<string>();
            ls.Add("step.png/STEP#IDONT know#opisanie##podlyanka#");
            minigame.InitializeMiniGame(2);
            minigame.GetAchievement(ls);
            score.getAchievement(ls);
            score.DrawScores(view.Graphics);
        }

        [Fact]
        public void TestNavigator()
        {
            /*var view = new View(1024, 768);
            int midx = 1024 * 2 / 3;
            int midy = 768 - 60;
            var rg = view.GetRegionCave(midx + 10, midy + 5);
            Assert.True(rg == RegionCave.DownRight, "Bad region 4");
            rg = view.GetRegionCave(midx, midy + 5);
            Assert.True(rg == RegionCave.Down, "Bad region 3");
            rg = view.GetRegionCave(midx - 10, midy + 5);
            Assert.True(rg == RegionCave.DownLeft, "Bad region 2");
            rg = view.GetRegionCave(midx - 10, midy - 5);
            Assert.True(rg == RegionCave.UpLeft, "Bad region 1");
            rg = view.GetRegionCave(midx, midy - 5);
            Assert.True(rg == RegionCave.Up, "Bad region 0");
            rg = view.GetRegionCave(midx + 10, midy - 5);
            Assert.True(rg == RegionCave.UpRight, "Bad region 5");
            */
        }

        [Fact]
        public void TestUtily()
        {
            int a = 2, b = 3;
            Utily.Swap<int>(ref a, ref b);
            Assert.True(a == 3 && b == 2, "Swap isn't correct");
            a = Utily.Next();
            b = Utily.Next();
            int c = Utily.Next();
            Assert.False(a == b && a == c, "It's realy strange Ranodm");
        }
    }
}
