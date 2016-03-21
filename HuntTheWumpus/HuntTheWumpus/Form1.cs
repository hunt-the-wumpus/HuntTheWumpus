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
        public Form1(System.Windows.Forms.PaintEventHandler DrawForm)
        {
            InitializeComponent();
            pictureBox1.Paint += DrawForm;
            pictureBox1.Width = this.ClientSize.Width;
            pictureBox1.Height = this.ClientSize.Height;
            //this.KeyDown += ??.KeyDown; KeyDown(object sender, KeyEventArgs e)
            //this.pictureBox1.MouseDown += ??.MouseDown; MouseDown(object sender, MouseEventArgs e) 
            //this.pictureBox1.MouseUp += ??.MouseUp; MouseUp(object sender, MouseEventArgs e)
            //this.pictureBox1.MouseMove += ??.MouseMove; MouseMove(object sender, MouseEventArgs e)
            //this.FormBorderStyle = FormBorderStyle.None;//If we want form without border
        }
        
        public void Loop()
        {
            long time = 0;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            while (true)
            {
                time += timer.ElapsedMilliseconds;
                timer.Restart();
                if (time >= 1000 / 25)
                {
                    time -= 1000 / 25;
                    DrawAll();
                }
            }
        }

        public void DrawAll()//принудительная перерисовка
        { 
           pictureBox1.Refresh(); 
        }
    }
}
