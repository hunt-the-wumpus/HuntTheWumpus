using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace HuntTheWumpus
{
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

		private Image img = Image.FromFile("data/Cave/TestRoom2.png");
		private Image room;

		public void InitEvent(KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            MainForm.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
        }

        public View(int width, int height)
        {
            Bitmap = new System.Drawing.Bitmap(width, height);
            Graphics = System.Drawing.Graphics.FromImage(Bitmap);
            Width = width;
            Height = height;
			room = Image.FromFile("data/Cave/TestRoom.png");
			MainMenuImage = Image.FromFile(@".\data\Sprites\MainMenuBackground.png");
            MainForm = new Form1(Drawing, width, height);
            MainForm.Show();
        }
        
        public void DrawText(string str, int x, int y, int size_font)
        {
            var fn = new System.Drawing.Font("Arial", size_font);
            Graphics.DrawString(str, fn, System.Drawing.Brushes.Black, x, y);
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

		public void DrawRoom(int x, int y, Danger danger, int length) {
			Graphics.DrawImage(img, new Rectangle(x, y, length, length));
		}

		public void DrawAllFriends(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int basex, int basey) {
			int length = Height * 10 / 12;
			DrawRoom(basex, basey, danger, length);
			DrawRoom(basex - length, basey, DangerList[2], length);
			DrawRoom(basex - length / 2, basey - length * 2 / 3, DangerList[0], length);
			DrawRoom(basex + length / 2, basey - length * 2 / 3, DangerList[1], length);
			DrawRoom(basex + length, basey, DangerList[3], length);
			DrawRoom(basex - length / 2, basey + length * 2 / 3, DangerList[4], length);
			DrawRoom(basex + length / 2, basey + length * 2 / 3, DangerList[5], length);
		}

        public void DrawCave(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom)
        {
			//Clear(Color.Aqua);
			int length = Height * 10 / 12;
			DrawAllFriends(graph, isActive, DangerList, danger, CurrentRoom, Width / 2 - length / 2, Height / 12);
        }

        public void DrawHint(string s)
        {
            //
        }

        public int GetRegionCave(int x, int y)
        {
            return -1;//тут бы тоже enum
        }
    }
}
