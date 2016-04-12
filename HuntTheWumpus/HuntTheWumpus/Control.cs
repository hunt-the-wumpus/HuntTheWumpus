using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    public class Control
    {
        enum ControlState
        {
            MainMenu,
            Cave,
            LastWindow,
            PickCave
        }

        enum StoryMG
        {
            Empty,
            Bat,
            Pit,
            Wumpus,
            BuyArrow,
            BuyHint
        }

        enum Hint
        {
            Wumpus,
            Pit,
            Bat,
            NoLuck,
            Empty
        }

        private ControlState state = ControlState.MainMenu;
        private ControlState OldState = ControlState.MainMenu;

        private int Width, Height;

        private bool MiniGameEnd = false;
        private MiniGame minigame;
        private bool CheckDanger;
        private StoryMG StoryMiniGame;

		//private Cave cave;
        private Scores score;

        private List<string> HintMessage;
        private Hint NowHint;
        private int HintData;

        private Map[] MapForPcik;
        private Map map;

        private Random random = new Random();

        private bool IsWin;

        public View view;

        public Control(int width, int height)
        {
            view = new View(width, height);
            view.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
            MapForPcik = new Map[5];
            Width = width;
            Height = height;
            HintMessage = new List<string>();
            HintMessage.Add("Wumpus in ");
            HintMessage.Add("Pit in ");
            HintMessage.Add("Bat in ");
            HintMessage.Add("You have bad luck...");
            NowHint = (Hint)HintMessage.Count;
        }

        public void UpDate(long time)
        {
        }

        void ContinueMenu()
        {
            state = OldState;
            if (state == ControlState.Cave && minigame.Is_playing)
                minigame.Pause(false);
        }

        void NewGame()
        {
            for (int i = 0; i < 5; ++i)
                MapForPcik[i] = new Map();
            MiniGameEnd = true;
            minigame = new MiniGame(Width, Height);
            score = new Scores(Width, Height);
            CheckDanger = true;
            IsWin = false;
            StoryMiniGame = StoryMG.Empty;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            NowHint = Hint.Empty;
            if (e.KeyCode == Keys.Escape)
            {
                if (state == ControlState.MainMenu)
                    ContinueMenu();
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
			NowHint = Hint.Empty;
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Down(e);
            }
            if (state == ControlState.Cave && MiniGameEnd)
            {
                int rg = 0;//view.GetRegionCave(e.X, e.Y);
                if (rg >= 0 && rg < 6)//мы ходим
                {
                    map.Move(rg);
                    CheckDanger = false;
                }
                if (rg == 6)//стрела
                {
                    StoryMiniGame = StoryMG.BuyArrow;
                    MiniGameEnd = false;
                    minigame.InitializeMiniGame(2);
                }
                if (rg == 7)//hint
                {
                    StoryMiniGame = StoryMG.BuyHint;
                    MiniGameEnd = false;
                    minigame.InitializeMiniGame(2);
                }
            }
            if (state == ControlState.MainMenu)
            {
				int rg = 0;// = view.GetRegionMainMenu(e.X, e.Y);
                if (rg == 1)//Новая игра
                {
                    state = ControlState.PickCave;
					NewGame();
					//player = new player
				}
                if (rg == 2)//Продолжить
                {
                    ContinueMenu();
                }
                if (rg == 3)//Выход
                {
                    Application.Exit();
                }
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

