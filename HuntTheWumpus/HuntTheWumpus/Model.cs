using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    class Model
    {
        public IMiniGame minigame { get; set; }
        public Model(int width, int height)
        {
            //minigame = new IMiniGame(width, height);
        }
    }
    interface IMiniGame
    {
        void Down(MouseEventArgs e);
        void Up(MouseEventArgs e);
        void Move(MouseEventArgs e);
        void DrawMiniGame(System.Drawing.Graphics g);
        void InitializeMiniGame(int difficult);
        bool Is_Winner { get; }
 		bool Is_playing { get; }
    }
}
