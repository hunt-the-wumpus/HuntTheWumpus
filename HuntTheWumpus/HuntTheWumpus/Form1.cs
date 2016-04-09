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
        public Form1(PaintEventHandler DrawForm, int width, int height)
        {
            InitializeComponent();
            pictureBox1.Paint += DrawForm;
            this.ClientSize = new Size(width, height);
            pictureBox1.Width = width;
            pictureBox1.Height = height;
        }

        public void InitEvent(KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            this.pictureBox1.MouseDown += MouseDown;
            this.pictureBox1.MouseMove += MouseMove;
            this.pictureBox1.MouseUp += MouseUp;
        }

        public void DrawAll()//принудительная перерисовка
        { 
           pictureBox1.Refresh(); 
        }
    }
}
