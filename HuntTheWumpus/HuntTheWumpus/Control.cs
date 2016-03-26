using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    class Control
    {
        enum ControlState
        {
            Menu, 
            Cave
        }

        private ControlState state = ControlState.Cave;
        
        private bool MiniGameEnd = false;

        public Model model;
        public View view;
        public Control(int width, int height)
        {
            model = new Model(width, height);
            view = new View(width, height, KeyDown, MouseDown, MouseUp, MouseMove);
        }

        public void UpDate()
        {
			state = 0;
			//model.minigame.TickTime();
			//model.minigame.DrawMiniGame(view.Grapchis);
			model.scores.DrawScores(view.Grapchis);
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (state == 0 && !MiniGameEnd)
            {
                model.minigame.Down(e);
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (state == 0 && !MiniGameEnd)
            {
                model.minigame.Up(e);
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (state == 0 && !MiniGameEnd)
            {
                model.minigame.Move(e);
            }
        }
    }
}
