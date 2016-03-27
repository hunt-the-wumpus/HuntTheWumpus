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
            MainMenu,
            Cave,
            LastWindow
        }

        private ControlState state = ControlState.Cave;
        private ControlState OldState = ControlState.MainMenu;

        private bool MiniGameEnd = false;
        private MiniGame minigame;
        private bool CheckDanger = true;
        private int StoryMiniGame;

        private Scores score;

        private IMap map;

        private bool IsWin;

        public View view;

        public Control(int width, int height)
        {
            view = new View(width, height, KeyDown, MouseDown, MouseUp, MouseMove);
            minigame = new MiniGame(width, height);
            minigame.InitializeMiniGame(3);
            score = new Scores(width, height);
        }

        public void UpDate()
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                view.Clear();//view.DrawCave()
                minigame.DrawMiniGame(view.Graphics);
                minigame.TickTime();
                if (!minigame.Is_playing)
                {
                    if (!minigame.Is_Winner && (StoryMiniGame != 1 || StoryMiniGame != 3))//не покупка 
                    {
                        IsWin = false;
                        state = ControlState.LastWindow;
                    }
                    if (!minigame.Is_Winner && StoryMiniGame == 4)
                    {
                        //hint
                    }
                    if (!minigame.Is_Winner && StoryMiniGame == 5)
                    {
                        //стрелы
                    }
                    MiniGameEnd = true;
                }
                return;
            }
            if (state == ControlState.MainMenu)
            {
                //view.DrawMainMenu();
                return;
            }
            if (state == ControlState.Cave && MiniGameEnd)
            {
                //view.DrawCave(тут надо что-то дать)
                if (!CheckDanger)
                {
                    CheckDanger = true;
                    if (map.danger == 1)//яма
                    {
                        StoryMiniGame = map.danger;
                        minigame.InitializeMiniGame(2);
                        MiniGameEnd = false;
                    }
                    if (map.danger == 2)//мыши
                    {
                        map.Respaw();
                    }
                    if (map.danger  == 3)//вампус
                    {
                        StoryMiniGame = map.danger;
                        MiniGameEnd = false;
                        minigame.InitializeMiniGame(3);
                    }
                }
                if (map.IsWin)
                {
                    state = ControlState.LastWindow;
                    IsWin = true;
                }
                return;
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (state == ControlState.MainMenu)
                {
                    state = OldState;
                    if (state == ControlState.Cave && minigame.Is_playing)
                        minigame.Pause(false);
                }
                else
                {
                    if (state == ControlState.Cave)
                    {
                        OldState = ControlState.Cave;
                        state = ControlState.MainMenu;
                        if (minigame.Is_playing)
                            minigame.Pause(true);
                        return;
                    }
                    if (state == ControlState.LastWindow)
                        OldState = state = ControlState.MainMenu;
                }
            }
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Down(e);
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Up(e);
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Move(e);
            }
        }
    }
}

