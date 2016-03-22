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
        private int state = 1;
        //0 - menu
        //1 - game in cave

        private bool MiniGameEnd = true;
        int MiniGameOK = 0, MiniGameFail = 0;

        public Model model;
        public View view;
        public Control(int width, int height)
        {
            model = new Model(width, height);
            view = new View(width, height, KeyDown, MouseDown, MouseUp, MouseMove);
        }

        public void UpDate()
        {
            var gen = new System.Random();
            view.Clear();
            if (state == 1 && !MiniGameEnd)
            {
                model.minigame.DrawMiniGame(view.g);
                if (!model.minigame.Is_playing)
                {
                    if (model.minigame.Is_Winner)
                        ++MiniGameOK;
                    else
                        ++MiniGameFail;
                    if (MiniGameOK == 2 || MiniGameFail == 2)
                    {
                        MiniGameEnd = true;
                        MiniGameFail = MiniGameOK = 0;
                    }
                    else
                    {
                        model.minigame.InitializeMiniGame(2);
                    }
                }
            }
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
