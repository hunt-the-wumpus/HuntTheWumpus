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
        public System.Drawing.Graphics g { get; set; }
        public Form1 MainForm { get; private set; }
        private System.Drawing.Bitmap bmp;

        public View(int width, int height, KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            bmp = new System.Drawing.Bitmap(width, height);
            g = System.Drawing.Graphics.FromImage(bmp);
            MainForm = new Form1(Drawing, KeyDown, MouseDown, MouseUp, MouseMove, width, height);
            MainForm.Show();
        }

        public void DrawText(string str, int x, int y, int size_font)
        {
            var fn = new System.Drawing.Font("Arial", size_font);
            g.DrawString(str, fn, System.Drawing.Brushes.Black, x, y);
        }

        public void Drawing(System.Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        public void Clear()
        {
            g.Clear(System.Drawing.Color.White);
        }

        public void Clear(System.Drawing.Color cl)
        {
            g.Clear(cl);
        }

        public bool Created()
        {
            return MainForm.Created;
        }
    }
}
