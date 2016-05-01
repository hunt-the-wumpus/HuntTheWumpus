using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace HuntTheWumpus
{
    public enum RegionCave
    {
        Up,
        UpLeft,
        DownLeft,
        Down,
        DownRight,
        UpRight,
        BuyArrow,
        BuyHint,
        UpConsole,
        DownConsole,
        Empty
    }
    public enum RegionPickCave
    {
        Cave0,
        Cave1,
        Cave2,
        Cave3,
        Cave4,
        Play,
        Empty
    }

    public class View
    {
        public System.Drawing.Graphics Graphics { get; private set; }
        private System.Drawing.Bitmap Bitmap;
        public bool IsAnimated { get; set; }
        public int AnimatedProgress { get; set; }

        private Image MainMenuImage;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Form1 MainForm { get; private set; }

        private CompressionImage cave_room;
        private Image bar = Image.FromFile("data/Sprites/InfoBar.png");
        private CompressionImage[] room = new CompressionImage[6];
		private CompressionImage Bat;
        private List<float> StownPosX = new List<float>();
        private List<float> StownPosY = new List<float>();

        private List<string> ConsoleList;
        private int IndexConsole = 0;

		private int deltaY = 0;

        public void InitEvent(KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            MainForm.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
        }

        public View(int width, int height)
        {
            Bitmap = new System.Drawing.Bitmap(width, height);
            Graphics = System.Drawing.Graphics.FromImage(Bitmap);
            ConsoleList = new List<string>();
            Width = width;
            Height = height;
            #region setted images
            for (int i = 0; i < 6; ++i)
            {
                room[i] = new CompressionImage("data/Cave/" + i.ToString() + ".png", height * 10 / 12 / 3, height * 10 / 12 / 2);
                room[i].ScreenWidth = width;
                room[i].ScreenHeight = height;
            }
            cave_room = new CompressionImage("data/Cave/TestRoom2.png", height * 10 / 12, height * 10 / 12);
            cave_room.ScreenWidth = width;
            cave_room.ScreenHeight = height;
			Bat = new CompressionImage("data/Cave/Bat.png", height / 12 * 10, height / 12 * 10);
			#endregion
			#region setted constants
			StownPosX.Add(1.0f / 3.0f); // 0 item
            StownPosY.Add(0);
            StownPosX.Add(0);           // 1 item
            StownPosY.Add(0);
            StownPosX.Add(0);           // 2 item
            StownPosY.Add(0.5f);
            StownPosX.Add(1 / 3.0f);    // 3 item
            StownPosY.Add(0.5f);
            StownPosX.Add(2 / 3.0f);    // 4 item
            StownPosY.Add(0.5f);
            StownPosX.Add(2 / 3.0f);    // 5 item
            StownPosY.Add(0);
            #endregion
            MainMenuImage = Image.FromFile(@".\data\Sprites\MainMenuBackground.png");
            MainForm = new Form1(Drawing, width, height);
            MainForm.Show();
			deltaY = height / 36;
        }

        public void DrawText(string str, int x, int y, int size_font)
        {
            Font fn = new Font("Arial", size_font);
            Graphics.DrawString(str, fn, Brushes.Black, x, y);
        }

        public void DrawText(string str, int x, int y, int size_font, string typefont)
        {
            Font fn = new Font(typefont, size_font);
            Graphics.DrawString(str, fn, Brushes.Black, x, y);
        }

        public void Drawing(System.Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bitmap, 0, 0);
        }

        public void Clear()
        {
            Graphics.Clear(System.Drawing.Color.White);
        }

        public void Clear(System.Drawing.Color cl)
        {
            Graphics.Clear(cl);
        }

        public bool Created()
        {
            return MainForm.Created;
        }

        public void DrawMainMenu()
        {
            Graphics.DrawImage(MainMenuImage, 0, 0, Width, Height);
        }

		public int GetRegionMainMenu(int x, int y)
        {
            return -1;//тут бы enum
        }

        public void DrawRoom(int x, int y, Danger danger, int length, int number, List<int>[] graph, List<bool>[] Active, bool DrawDanger = false)
        {
			//Graphics.DrawImage(img, new Rectangle(x, y, length, length));
			//cave_room.Draw(Graphics, x, y);
			if (danger == Danger.Bat && DrawDanger) {
				Bat.Draw(Graphics, x, y);
			}
			for (int i = 0; i < 6; i++)
            {
                if (!Active[number][i])
                {
                    //Graphics.DrawImage(room[i], new Rectangle(x, y, length, length));
                    room[i].Draw(Graphics, x + (int)(length * StownPosX[i]), y + (int)(length * StownPosY[i]));
                }
            }
        }

        public void DrawAllFriends(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int basex, int basey)
        {
            int length = Height * 10 / 12;
            DrawRoom(basex, basey, danger, length, CurrentRoom, graph, isActive, true);
            DrawRoom(basex, basey - length, DangerList[0], length, graph[CurrentRoom][0], graph, isActive);
            DrawRoom(basex, basey + length, DangerList[3], length, graph[CurrentRoom][3], graph, isActive);
            DrawRoom(basex + length * 2 / 3, basey - length / 2, DangerList[5], length, graph[CurrentRoom][5], graph, isActive);
            DrawRoom(basex - length * 2 / 3, basey - length / 2, DangerList[1], length, graph[CurrentRoom][1], graph, isActive);
            DrawRoom(basex + length * 2 / 3, basey + length / 2, DangerList[4], length, graph[CurrentRoom][4], graph, isActive);
            DrawRoom(basex - length * 2 / 3, basey + length / 2, DangerList[2], length, graph[CurrentRoom][2], graph, isActive);
        }

        public void DrawCave(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int Coins, int Arrows)
        {
			//Clear(Color.White);
			//Clear();
            int length = Height * 10 / 12;
            DrawAllFriends(graph, isActive, DangerList, danger, CurrentRoom, Width / 2 - length / 2, Height / 12 - deltaY);
			//Graphics.DrawImage(bar, new Rectangle(0, Height - 60, Width, 30));
            DrawInterface(Coins, Arrows, CurrentRoom);
			Graphics.DrawEllipse(Pens.Red, Width / 2 - length / 2 + length / 3 - 10, Height / 12 - deltaY - 10, 20, 20);
			Graphics.DrawEllipse(Pens.Blue, Width / 2 - length / 2 - 10, Height / 12 + length / 2 - deltaY - 10, 20, 20);
		}

        public void DrawInterface(int coins, int arrows, int room)
        {
            int yup = Height - 120;
            Graphics.FillRectangle(Brushes.Gray, 0, yup, Width, 120);
            Point[] pn = new Point[3];
            Graphics.DrawLine(Pens.Black, 150, yup, 150, Height);
            DrawText("Arrows " + arrows, 20, yup + 10, 20);
            DrawText("Coins " + coins, 20, yup + 60, 20);
            DrawText("Buy Arrows", 170, yup + 10, 20);
            DrawText("Buy Hint", 170, yup + 60, 20);
            for (int i = IndexConsole; i > IndexConsole - 5 && i >= 0; --i)
                DrawText(ConsoleList[i], 730, yup + 10 + (IndexConsole - i) * 18, 15, "Consolas");
            DrawText("^", 704, yup + 37, 23);
            DrawText("v", 705, yup + 52, 20);
        }

		private bool isLeftUpper(int x1, int y1, int x2, int y2, int ix, int iy) {
			int proectionX = x2;
			int proectionY = iy;
			int BigSizeX = x1 - x2;
			int BigSizeY = y2 - y1;
			int SmallSizeY = y2 - iy;
			float k = (float)(SmallSizeY) / BigSizeY;
			float HipotinuzeX = proectionX + (int)(BigSizeX * k);
			return (HipotinuzeX - ix > 0 && iy > y1 && iy < y2);
		}

		private bool isRightUpper(int x1, int y1, int x2, int y2, int ix, int iy) {
			int proectionX = x2;
			int proectionY = iy;
			int BigSizeX = x2 - x1;
			int BigSizeY = y2 - y1;
			int SmallSizeY = y2 - iy;
			float k = (float)(SmallSizeY) / BigSizeY;
			float HipotinuzeX = x2 - (int)(BigSizeX * k);
			return (ix - HipotinuzeX > 0 && iy > y1 && iy < y2);
		}

		private bool isLeftDown(int x1, int y1, int x2, int y2, int ix, int iy) {
			int proectionX = x1;
			int proectionY = iy;
			int BigSizeX = x2 - x1;
			int BigSizeY = y2 - y1;
			int SmallSizeY = iy - y1;
			float k = (float)(SmallSizeY) / BigSizeY;
			float HipotinuzeX = proectionX + (int)(BigSizeX * k);
			return (HipotinuzeX - ix > 0 && iy > y1 && iy < y2);
		}

		private bool isRightDown(int x1, int y1, int x2, int y2, int ix, int iy) {
			int proectionX = x1;
			int proectionY = iy;
			int BigSizeX = x1 - x2;
			int BigSizeY = y2 - y1;
			int SmallSizeY = iy - y1;
			float k = (float)(SmallSizeY) / BigSizeY;
			float HipotinuzeX = proectionX - (int)(BigSizeX * k);
			return (ix  - HipotinuzeX > 0 && iy > y1 && iy < y2);
		}

		private bool isUnder(int x1, int x2, int y12, int x, int y) {
			return (y < y12 && x1 < x && x2 > x);
		}

		private bool isDown(int x1, int x2, int y12, int x, int y) {
			return (y > y12 && x1 < x && x2 > x);
		}

		public RegionCave GetRegionCave(int x, int y) {
			RegionCave result = RegionCave.Empty;
			int yup = Height - 120;
			int length = Height / 12 * 10;
			if (isLeftUpper(Width / 2 - length / 2 + length / 3, Height / 12 - deltaY, Width / 2 - length / 2, Height / 12 + length / 2 - deltaY, x, y)) {
				return RegionCave.UpLeft;
			}
			if (isRightUpper(Width / 2 + length / 2 - length / 3, Height / 12 - deltaY, Width / 2 + length / 2, Height / 12 + length / 2 - deltaY, x, y)) {
				return RegionCave.UpRight;
			}
			if (isLeftDown(Width / 2 - length / 2, Height / 12 + length / 2 - deltaY, Width / 2 - length / 2 + length / 3, Height / 12 + length - deltaY, x, y)) {
				return RegionCave.DownLeft;
			}
			if (isRightDown(Width / 2 + length / 2, Height / 12 + length / 2 - deltaY, Width / 2 + length / 2 - length / 3, Height / 12 + length - deltaY, x, y)) {
				return RegionCave.DownRight;
			}
			if (isUnder(Width / 2 - length / 2 + length / 3, Width / 2 + length / 2 - length / 3, Height / 12 - deltaY, x, y)) {
				return RegionCave.Up;
			}
			if (isDown(Width / 2 - length / 2 + length / 3, Width / 2 + length / 2 - length / 3, Height / 12 + length - deltaY, x, y)) {
				return RegionCave.Down;
			}
			if (x > 170 && x < 340 && y - yup > 60) {
				return RegionCave.BuyHint;
			}
			if (x > 170 && x < 340 && y - yup < 60) {
				return RegionCave.BuyArrow;
			}
			if (x >= 705 && x < 730 && y > yup + 37 && y < yup + 55) {
				return RegionCave.UpConsole;
			}
			if (x >= 705 && x < 730 && y > yup + 55 && y < yup + 85) {
				return RegionCave.DownConsole;
			}
            return result;
        }

        public void ClearConsole()
        {
            ConsoleList = new List<string>();
            AddComand("Left mouse bottom for#moving");
            AddComand("Right mouse bottom for#shot arrow");
            IndexConsole = ConsoleList.Count - 1;
        }

		public void AddComand(string s)
        {
            if (IndexConsole == ConsoleList.Count - 1)
                ++IndexConsole;
            string[] strs = s.Split('#');
            for (int i = strs.Length - 1; i >= 0; i--)
                ConsoleList.Add(strs[i]);
        }

		public void ChangeIndex(int up)
        {
            if (ConsoleList.Count <= 5)
                return;
            if (up > 0)
                IndexConsole = Math.Min(IndexConsole + up, ConsoleList.Count - 1);
            else
                IndexConsole = Math.Max(IndexConsole + up, 4);
        }

		public void DrawPickCave(List<int>[] graph, List<bool>[] isActive, int num)
        {
            Clear(Color.LightGreen);
            int size = 55;
            int d = 5;
            int hght = (int)(size * 1.732);
            for (int i = 0; i < 6; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    int lx = i * size * 3 / 2 + 100 + i * d, ly = j * hght + (i % 2) * hght / 2 + 50 + j * d;
                    Point[] pn = new Point[6];
                    pn[0] = new Point(lx + size / 2, ly);
                    pn[1] = new Point(lx, ly + hght / 2);
                    pn[2] = new Point(lx + size / 2, ly + hght);
                    pn[3] = new Point(lx + size * 3 / 2, ly + hght);
                    pn[4] = new Point(lx + size * 2, ly + hght / 2);
                    pn[5] = new Point(lx + size * 3 / 2, ly);
                    Graphics.FillPolygon(Brushes.BlueViolet, pn);
                    Pen pen = new Pen(Color.Brown, 9);
                    for (int k = 0; k < 3; ++k)
                    {
                        int v = i + j * 6;
                        if (isActive[v][k])
                        {
                            if (k == 0)
                                Graphics.DrawLine(pen, lx + size, ly + hght / 3, lx + size, ly - hght / 3 - d);
                            if (k == 1)
                                Graphics.DrawLine(pen, lx + size * 2 / 3, ly + hght / 3, lx - size / 3, ly);
                            if (k == 2)
                                Graphics.DrawLine(pen, lx + size * 2 / 3, ly + hght * 2 / 3, lx - size / 3, ly + hght);
                        }
                    }
                }
            for (int i = 0; i < 5; ++i)
            {
                if (i > 0)
                    Graphics.DrawLine(new Pen(Color.Black), i * Width / 5, Height - 120, i * Width / 5, Height);
                DrawText((i + 1).ToString(), i * Width / 5 + 60, Height - 100, 40);
            }
            DrawText("GO!!", 850, 550, 40);
        }

		public RegionPickCave GetRegionPickCave(int x, int y)
        {
            if (y > Height - 120)
                return (RegionPickCave)(x * 5 / Width);
            if (y > 550 && x > 850)
                return RegionPickCave.Play;
            return RegionPickCave.Empty;
        }
    }
}
