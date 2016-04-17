using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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
        
        private Scores score;

        private List<string> HintMessage;
       
        private Map[] MapForPiсk;
        private Map map;

        private Random random = new Random();

        private bool IsWin;
        public Player player;
        
        public View view;

        public Control(int width, int height)
        {
			view = new View(width, height);
            view.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
            view.ClearConsole();
            MapForPiсk = new Map[5];
            score = new Scores(width, height);
            minigame = new MiniGame(width, height);
            map = new Map();
            player = new Player();
            Width = width;
            Height = height;
            HintMessage = new List<string>();
            HintMessage.Add("Wumpus in ");
            HintMessage.Add("Pit in ");
            HintMessage.Add("Bat in ");
            HintMessage.Add("You have bad luck...");
        }

        public void UpDate(long time)
        {
            if (state == ControlState.Cave)
            {
				view.Clear();
				view.DrawCave(map.graph, map.isActive, map.GetDangerList(), map.danger, map.Room, player.Coins, player.Arrow);
				if (!MiniGameEnd)
                {
                    minigame.DrawMiniGame(view.Graphics);
                    minigame.TickTime();
                    if (!minigame.Is_playing)
                    {
                        List<string> listachiv = new List<string>();
                        minigame.GetAchievement(listachiv);
                        score.getAchievement(listachiv);
                        if (!minigame.Is_Winner && StoryMiniGame != StoryMG.BuyArrow && StoryMiniGame != StoryMG.BuyHint)//не покупка 
                        {
                            IsWin = false;
                            state = ControlState.LastWindow;
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyHint)
                        {
                            player.BuyHint();
                            int rnd = random.Next() % HintMessage.Count;
                            Hint NowHint = (Hint)rnd;
                            int HintData = 0;
                            if (NowHint == Hint.Bat)
                                HintData = map.GetBat();
                            if (NowHint == Hint.Pit)
                                HintData = map.GetPit();
                            if (NowHint == Hint.Wumpus)
                                HintData = map.Wumpus;
                            if (NowHint != Hint.NoLuck)
                                view.AddComand(HintMessage[(int)NowHint] + HintData);
                            else
                                view.AddComand(HintMessage[(int)NowHint]);
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyArrow)
                        {
                            player.BuyArrow();
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
                        minigame = new MiniGame(Width, Height);
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
                        minigame = new MiniGame(Width, Height);
                        StoryMiniGame = StoryMG.Wumpus;
                        MiniGameEnd = false;
                        minigame.InitializeMiniGame(3);
                    }
                    Danger dangerabout = map.GetDangerAbout();
                    if (dangerabout == Danger.Bat)
                        view.AddComand("Bats Nearby");
                    if (dangerabout == Danger.Pit)
                        view.AddComand("I feel a draft");
                    if (dangerabout == Danger.Wumpus)
                        view.AddComand("I smell a Wumpus!");
                }
                if (map.IsWin)
                {
                    state = ControlState.LastWindow;
                    IsWin = true;
                }
                score.DrawScores(view.Graphics);
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
            score.DrawScores(view.Graphics);
            score.TickTime();
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
			if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Down(e);
            }
            if (state == ControlState.Cave && MiniGameEnd)
            {
                RegionCave rg = view.GetRegionCave(e.X, e.Y);
                if ((int)rg >= 0 && (int)rg < 6 && map.isActive[map.Room][(int)rg])
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        map.Move((int)rg);
                        CheckDanger = false;
                    }
                    else
                    {
                        player.PushArrow();
                        map.PushArrow((int)rg);
                        if (map.IsWin)
                        {
                            IsWin = true;
                            state = ControlState.LastWindow;
                        }
                        else if (player.Arrow == 0)
                        {
                            IsWin = false;
                            state = ControlState.LastWindow;
                        }
                    }
                }
                if (rg == RegionCave.BuyArrow && player.CanBuyArrow())
                {
                    StoryMiniGame = StoryMG.BuyArrow;
                    MiniGameEnd = false;
                    minigame = new MiniGame(Width, Height);
                    minigame.InitializeMiniGame(2);
                }
                if (rg == RegionCave.BuyHint && player.CanBuyHint())
                {
                    StoryMiniGame = StoryMG.BuyHint;
                    MiniGameEnd = false;
                    minigame = new MiniGame(Width, Height);
                    minigame.InitializeMiniGame(2);
                }
                if (rg == RegionCave.UpConsole)
                    view.ChangeIndex(1);
                if (rg == RegionCave.DownConsole)
                    view.ChangeIndex(-1);
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

