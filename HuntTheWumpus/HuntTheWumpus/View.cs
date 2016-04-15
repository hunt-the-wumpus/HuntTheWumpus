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
        UpDown,
        Down,
        DownRight,
        UpRight,
        BuyArrow,
        BuyHint,
        ChangeMode,
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
                if (!Active[number][i]) {
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
        public void DrawCave(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int Coins, int Arrows, ModeUserControl NowMode)
        {
            //Clear(Color.White);
            //Clear();
            int length = Height * 10 / 12;
            DrawAllFriends(graph, isActive, DangerList, danger, CurrentRoom, Width / 2 - length / 2, Height / 12);
            //Graphics.DrawImage(bar, new Rectangle(0, Height - 60, Width, 30));
            DrawInterface(Coins, Arrows, CurrentRoom, NowMode);
        }
        public void DrawInterface(int coins, int arrows, int room, ModeUserControl nowmode)
        {
            int yup = Height - 120;
            Graphics.FillRectangle(Brushes.Gray, 0, yup, Width, 150);
            DrawText("Arrows " + arrows, 20, yup + 10, 20);
            DrawText("Coins " + coins, 20, yup + 60, 20);
            DrawText("Buy Hint", 820, yup + 10, 20);
            DrawText("Buy Arrows", 820, yup + 60, 20);
            if (nowmode == ModeUserControl.Push)
                DrawText("Push", 530, yup + 50, 25);
            else
                DrawText("Move", 530, yup + 50, 25);
            DrawText(room + "", 20, 20, 20);
            for (int i = ConsoleList.Count - 1; i > ConsoleList.Count - 3 && i >= 0; --i)
                DrawText((i + 1) + ": " + ConsoleList[i], 170, yup + 10 + (ConsoleList.Count - i - 1) * 50, 20);
            int midx = Width * 2 / 3;
            int midy = Height - 60;
            int size = 60;
            int hght = (int)(size * 1.732);//sqrt(3)*hgth
            Pen pen = new Pen(Color.Black);
            Graphics.DrawLine(pen, midx - size / 2, midy - hght / 2, midx + size / 2, midy - hght / 2);
            Graphics.DrawLine(pen, midx + size / 2, midy - hght / 2, midx + size, midy);
            Graphics.DrawLine(pen, midx + size, midy, midx + size / 2, midy + hght / 2);
            Graphics.DrawLine(pen, midx + size / 2, midy + hght / 2, midx - size / 2, midy + hght / 2);
            Graphics.DrawLine(pen, midx - size / 2, midy + hght / 2, midx - size, midy);
            Graphics.DrawLine(pen, midx - size, midy, midx - size / 2, midy - hght / 2);
            Graphics.DrawLine(pen, midx - size / 2, midy - hght / 2, midx + size / 2, midy + hght / 2);
            Graphics.DrawLine(pen, midx + size / 2, midy - hght / 2, midx - size / 2, midy + hght / 2);
            Graphics.DrawLine(pen, midx - size, midy, midx + size, midy);
            Graphics.FillEllipse(Brushes.Red, midx - size - 40, yup + 5, 40, 40);
        }
        public RegionCave GetRegionCave(int x, int y)
        {
            int yup = Height - 120;
            if (y < yup)
                return RegionCave.Empty;
            if (x > 820 && y - yup < 60)
                return RegionCave.BuyHint;
            if (x > 820 && y - yup > 60)
                return RegionCave.BuyArrow;
            int midx = Width * 2 / 3;
            int size = 60;
            int midy = Height - 60;
            if (x >= midx - size && x <= midx + size)
            {
                int angle = (int)Math.Floor(Math.Atan2(y - midy, x - midx) / Math.PI * 3);
                return (RegionCave)((angle + 5) % 6);
            }
            if (Math.Pow(x - midx + size + 20, 2) + Math.Pow(y - yup - 25, 2) <= 400)
                return RegionCave.ChangeMode;
            return RegionCave.Empty;
        }
        public void ClearConsole()
        {
            ConsoleList = new List<string>();
        }
        public void AddComand(string s)
        {
            ConsoleList.Add(s);
        }
    }
}
