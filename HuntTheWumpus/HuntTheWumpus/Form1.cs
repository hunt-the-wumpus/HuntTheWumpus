using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += DrawForm;
            //this.KeyDown += ??.KeyDown; KeyDown(object sender, KeyEventArgs e)
            //this.pictureBox1.MouseDown += ??.MouseDown; MouseDown(object sender, MouseEventArgs e) 
            //this.pictureBox1.MouseUp += ??.MouseUp; MouseUp(object sender, MouseEventArgs e)
            //this.pictureBox1.MouseMove += ??.MouseMove; MouseMove(object sender, MouseEventArgs e)
            this.FormBorderStyle = FormBorderStyle.None;//If we want form without border

        }

        public void DrawForm(System.Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var bmp = new System.Drawing.Bitmap(this.Width, this.Height);
            var g = System.Drawing.Graphics.FromImage(bmp);
            //??.GetScreen(g);
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        public void DrawAll()//принудительная перерисовка
        { 
           pictureBox1.Refresh(); 
        }
    }
}
