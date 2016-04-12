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
            ScoreList,
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

        private ControlState state = ControlState.Cave;
        private ControlState OldState = ControlState.MainMenu;

        private int Width, Height;

        private bool MiniGameEnd = true;
        private MiniGame minigame;
        private bool CheckDanger;
        private StoryMG StoryMiniGame;

		//private Cave cave;
        private Scores score;

        private List<string> HintMessage;
        private Hint NowHint;
        private int HintData;

        private Map[] MapForPiсk;
        private Map map;

        private Random random = new Random();

        private bool IsWin;

        public View view;

        public Control(int width, int height)
        {
            view = new View(width, height);
            view.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
            MapForPiсk = new Map[5];
            score = new Scores(width, height);
            map = new Map();
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
            if (state == ControlState.Cave)
            {
                view.DrawCave(map.graph, map.isActive, map.GetDangerList(), map.danger, map.Room);
                if (NowHint != Hint.Empty)
                {
                    if (NowHint != Hint.NoLuck)
                        view.DrawHint(HintMessage[(int)NowHint] + HintData);
                    else
                        view.DrawHint(HintMessage[(int)NowHint]);
                }
                if (!MiniGameEnd)
                {
                    minigame.DrawMiniGame(view.Graphics);
                    minigame.TickTime();
                    if (!minigame.Is_playing)
                    {
                        if (!minigame.Is_Winner && StoryMiniGame != StoryMG.BuyArrow && StoryMiniGame != StoryMG.BuyHint)//не покупка 
                        {
                            IsWin = false;
                            state = ControlState.LastWindow;
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyHint)
                        {
                            int rnd = random.Next() % HintMessage.Count;
                            NowHint = (Hint)rnd;
                            if (NowHint == Hint.Bat)
                                HintData = map.GetBat();
                            if (NowHint == Hint.Pit)
                                HintData = map.GetPit();
                            if (NowHint == Hint.Wumpus)
                                HintData = map.Wumpus;
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyArrow)
                        {
                            //player.BuyArrow();
                        }
                        if (minigame.Is_Winner && StoryMG.Wumpus == StoryMiniGame)
                        {
                            map.WumpusGoAway();
                        }
                        MiniGameEnd = true;
                    }
                }
                if (!CheckDanger)
                {
                    CheckDanger = true;
                    if (map.danger == Danger.Pit)
                    {
                        StoryMiniGame = StoryMG.Pit;
                        minigame.InitializeMiniGame(2);
                        MiniGameEnd = false;
                    }
                    if (map.danger == Danger.Bat)
                    {
                        map.Respaw();
                    }
                    if (map.danger == Danger.Wumpus)
                    {
                        StoryMiniGame = StoryMG.Wumpus;
                        MiniGameEnd = false;
                        minigame.InitializeMiniGame(3);
                    }
                }
                if (map.IsWin)
                {
                    state = ControlState.LastWindow;
                    IsWin = true;
                }
            }

            if (state == ControlState.MainMenu)
            {
                view.DrawMainMenu();
            }

            if (state == ControlState.LastWindow)
            {
                //score.DrawFinal();
            }

            if (state == ControlState.ScoreList)
            {
                //score.DrawScoreList();
            }

            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10);
            //score.DrawScores(view.Graphics);
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
                MapForPiсk[i] = new Map();
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
                else if (state == ControlState.Cave)
                {
                    OldState = ControlState.Cave;
                    state = ControlState.MainMenu;
                    if (minigame.Is_playing)
                        minigame.Pause(true);
                }
                else if (state == ControlState.LastWindow)
                    state = ControlState.ScoreList;
                else if (state == ControlState.ScoreList)
                    OldState = state = ControlState.MainMenu;
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
                int rg = view.GetRegionCave(e.X, e.Y);
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
                int rg = view.GetRegionMainMenu(e.X, e.Y);
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

