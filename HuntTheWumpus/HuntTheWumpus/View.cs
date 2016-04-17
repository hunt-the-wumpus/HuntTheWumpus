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
        private List<float> StownPosX = new List<float>();
        private List<float> StownPosY = new List<float>();

        private List<string> ConsoleList;
        private int IndexConsole = 0;

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
            for (int i = 0; i < 6; ++i) {
                room[i] = new CompressionImage("data/Cave/" + i.ToString() + ".png", height * 10 / 12 / 3, height * 10 / 12 / 2);
                room[i].ScreenWidth = width;
                room[i].ScreenHeight = height;
            }
            cave_room = new CompressionImage("data/Cave/TestRoom2.png", height * 10 / 12, height * 10 / 12);
            cave_room.ScreenWidth = width;
            cave_room.ScreenHeight = height;
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

        public void DrawRoom(int x, int y, Danger danger, int length, int number, List<int>[] graph, List<bool>[] Active) {
            //Graphics.DrawImage(img, new Rectangle(x, y, length, length));
            cave_room.Draw(Graphics, x, y);
            for (int i = 0; i < 6; i++) {
                if (Active[number][i]) {
                    //Graphics.DrawImage(room[i], new Rectangle(x, y, length, length));
                    room[i].Draw(Graphics, x + (int)(length * StownPosX[i]), y + (int)(length * StownPosY[i]));
                }
            }
            /*if (danger == Danger.Empty) {
				Graphics.DrawRectangle(Pens.Blue, new Rectangle(x + length / 2 - 100, y + length / 2 - 100, 200, 200));
			}*/
        }
        public void DrawAllFriends(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int basex, int basey) {
            int length = Height * 10 / 12;
            DrawRoom(basex, basey, danger, length, CurrentRoom, graph, isActive);
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
            DrawAllFriends(graph, isActive, DangerList, danger, CurrentRoom, Width / 2 - length / 2, Height / 12);
            //Graphics.DrawImage(bar, new Rectangle(0, Height - 60, Width, 30));
            DrawInterface(Coins, Arrows, CurrentRoom);
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
        public RegionCave GetRegionCave(int x, int y)
        {
            int yup = Height - 120;
            if (y < yup)
                return RegionCave.Empty;
            if (x > 170 && x < 340 && y - yup > 60)
                return RegionCave.BuyHint;
            if (x > 170 && x < 340 && y - yup < 60)
                return RegionCave.BuyArrow;
            if (x >= 705 && x < 730 && y > yup + 37 && y < yup + 55)
                return RegionCave.UpConsole;
            if (x >= 705 && x < 730 && y > yup + 55 && y < yup + 85)
                return RegionCave.DownConsole;
            return RegionCave.Empty;
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
    }
}
