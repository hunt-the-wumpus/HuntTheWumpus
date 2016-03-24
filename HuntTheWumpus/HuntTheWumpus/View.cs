using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    class View
    {
        public System.Drawing.Graphics Grapchis { get; private set; }
        private System.Drawing.Bitmap Bitmap;
        public Form1 MainForm { get; private set; }
        
        public View(int width, int height, KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            Bitmap = new System.Drawing.Bitmap(width, height);
            Grapchis = System.Drawing.Graphics.FromImage(Bitmap);
            Grapchis.Clear(System.Drawing.Color.Blue);
            MainForm = new Form1(Drawing, KeyDown, MouseDown, MouseUp, MouseMove, width, height);
            MainForm.Show();
        }
        
        public void DrawText(string str, int x, int y, int size_font)
        {
            var fn = new System.Drawing.Font("Arial", size_font);
            Grapchis.DrawString(str, fn, System.Drawing.Brushes.Black, x, y);
        }

        public void Drawing(System.Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bitmap, 0, 0);
        }

        public void Clear()
        {
            Grapchis.Clear(System.Drawing.Color.White);
        }

        public void Clear(System.Drawing.Color cl)
        {
            Grapchis.Clear(cl);
        }

        public bool Created()
        {
            return MainForm.Created;
        }
    }
}
