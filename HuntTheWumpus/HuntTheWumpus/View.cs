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

        private Image MainMenuImage;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Form1 MainForm { get; private set; }

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

        public void DrawCave(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom)
        {
            Clear(Color.Aqua);
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
