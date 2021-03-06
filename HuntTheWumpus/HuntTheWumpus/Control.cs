﻿using System;
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

        private ControlState state = ControlState.MainMenu;
        private ControlState OldState = ControlState.MainMenu;

        private int Width, Height;

        private bool MiniGameEnd = true;
        private MiniGame minigame;
        private bool CheckDanger;
        private StoryMG StoryMiniGame;
        private bool UseMiniGame;

        private Scores score;

        private List<string> HintMessage;

        private int[] ArroDiff = { 1, 1, 2 };
        private int[] HintDiff = { 2, 2, 3 };
        private int[] PitDiff = { 1, 2, 3 };
        private int[] WumpusDiff = { 2, 3, 4 };

        private int numPickCave = 0;
        private int numDiff = 0;
        private int Difficulty = 0;
        private Map[] MapForPiсk;
        private Map map;
        private int[] seeds;
        private string seed = "";

        private bool IsWin;
        public Player player;

        public View view;

        public Control(int width, int height)
        {
            view = new View(width, height);
            Width = width;
            Height = height;
            view.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
            view.ClearConsole();
            MapForPiсk = new Map[5];
            seeds = new int[5];
            HintMessage = new List<string>();
            HintMessage.Add(Messages.WumpusHint);
            HintMessage.Add(Messages.PitHint);
            HintMessage.Add(Messages.BatHint);
            HintMessage.Add(Messages.BadLuckHint);
        }

        public void UpDate(long time)
        {
            if (state == ControlState.Cave)
            {
				List<string> listachiv = new List<string>();
				map.GetAchievement(listachiv);
				if (minigame != null) {
					minigame.GetAchievement(listachiv);
				}
				player.GetAchievement(listachiv);
				score.getAchievement(listachiv);
				view.DrawCave(map.graph, map.isActive, map.GetDangerList(), map.danger, map.Room);
                if (!MiniGameEnd)
                {
                    minigame.TickTime();
                    minigame.DrawMiniGame(view.Graphics);
                    if (!minigame.Is_playing)
                    {
                        if (!minigame.Is_Winner && StoryMiniGame != StoryMG.BuyArrow && StoryMiniGame != StoryMG.BuyHint)
                        {
                            IsWin = false;
                            EndGame();
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyHint)
                        {
                            int rnd = Utily.Next() % HintMessage.Count;
                            Hint NowHint = (Hint)rnd;
                            int HintData = 0;
                            if (NowHint == Hint.Bat)
                                HintData = map.GetBat() + 1;
                            if (NowHint == Hint.Pit)
                                HintData = map.GetPit() + 1;
                            if (NowHint == Hint.Wumpus)
                                HintData = map.Wumpus + 1;
                            if (NowHint != Hint.NoLuck)
                                view.AddComand(HintMessage[(int)NowHint] + HintData, true);
                            else
                                view.AddComand(HintMessage[(int)NowHint], true);
                        }
                        if (minigame.Is_Winner && StoryMiniGame == StoryMG.BuyArrow)
                        {
                            player.GiveArrows();
                            view.AddComand(Messages.GiveArrowsString, true, false);
                        }
                        if (minigame.Is_Winner && StoryMG.Wumpus == StoryMiniGame)
                        {
                            map.WumpusGoAway();
                            view.AddComand(Messages.WumpusRunString, true);
                        }
                        if (minigame.Is_Winner)
                        {
                            score.AddScores(50);
                        }
                        MiniGameEnd = true;
                    }
                } else 
					view.DrawInterface(player.Coins, player.Arrow, map.Room, player.CanBuyArrow(), player.CanBuyHint());
				if (!CheckDanger && !view.MinimizeBat && !view.IsAnimated)
                {
                    CheckDanger = true;
                    if (map.danger == Danger.Pit)
                    {
                        minigame = new MiniGame(Width, Height);
                        StoryMiniGame = StoryMG.Pit;
						minigame.HintText = Messages.PitHintText;
                        minigame.InitializeMiniGame(PitDiff[Difficulty]);
                        UseMiniGame = true;
                        MiniGameEnd = false;
                    }
                    if (map.danger == Danger.Bat)
                    {
                        view.AddComand(Messages.MetBatString, true);
                        map.Respaw();
                    }
                    if (map.danger == Danger.Wumpus)
                    {
                        minigame = new MiniGame(Width, Height);
                        StoryMiniGame = StoryMG.Wumpus;
						minigame.HintText = Messages.WumpusHintText;
                        MiniGameEnd = false;
                        UseMiniGame = true;
                        minigame.InitializeMiniGame(WumpusDiff[Difficulty]);
                    }
                    Danger dangerabout = map.GetDangerAbout();
                    if (dangerabout == Danger.Bat)
                        view.AddComand(String.Format(Messages.BatWarning, map.Room + 1), true);
                    if (dangerabout == Danger.Pit)
                        view.AddComand(String.Format(Messages.PitWarning, map.Room + 1), true);
                    if (dangerabout == Danger.Wumpus)
                    {
                        string strout = String.Format(Messages.WumpusWarning, map.Room + 1);
                        if (!player.IsShotArrow)
                            strout += Messages.HintRightClick;
                        view.AddComand(strout, true);
                    }
                }
                if (map.IsWin && view.IsEndAnimation())
                {
                    IsWin = true;
                    EndGame();
                }
                else if (player.Arrow == 0 && view.IsEndAnimation())
                {
                    IsWin = false;
                    EndGame();
                }
            }

            if (state == ControlState.MainMenu)
            {
                view.DrawMainMenu();
            }

            if (state == ControlState.PickCave)
            {
                view.DrawPickCave(MapForPiсk[numPickCave].graph, MapForPiсk[numPickCave].isActive, numPickCave, numDiff, seed);
            }
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, "Arial", Color.White);
			if (score != null) {
				score.TickTime();
				score.Draw(view.Graphics);
			}
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
            {
                seeds[i] = Utily.Next();
                Utily.ChangeSeed(seeds[i]);
                MapForPiсk[i] = new Map();
            }
            numPickCave = 0;
            numDiff = 1;
            MiniGameEnd = true;
            minigame = new MiniGame(Width, Height);
            player = new Player();
            score = new Scores(Width, Height);
            score.active = ScoreState.Achievements;
            CheckDanger = false;
            IsWin = false;
            StoryMiniGame = StoryMG.Empty;
            view.UpdateImage();
            view.ClearConsole();
            seed = "";
            state = ControlState.PickCave;
            UseMiniGame = false;
        }

        private void EndGame()
        {
            state = ControlState.LastWindow;
            if (!UseMiniGame)
            {
                List<string> ls = new List<string>();
                ls.Add(Messages.DodgerAchiv);
                score.getAchievement(ls);
            }
            score.SetFinalState(IsWin);
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
				if (state == ControlState.PickCave)
                {
                    state = ControlState.MainMenu;
                    OldState = ControlState.PickCave;
                }
            }
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                if (state == ControlState.PickCave && seed.Length < 16)
                {
                    seed += (e.KeyCode - Keys.D0);
                }
            }
            if (e.KeyCode == Keys.Back)
            {
                if (state == ControlState.PickCave)
                {
                    if (seed.Length > 0)
                        seed = seed.Remove(seed.Length - 1);
                }
                score.KeyDown("del");
            }
            if (e.KeyCode == Keys.Enter)
            {
                score.KeyDown("enter");
                if (state == ControlState.PickCave)
                {
                    StartAfterPick();
                }
            }
            if (score != null)
            {
                score.KeyDown((new KeysConverter()).ConvertToString(e.KeyCode));
            }
        }

        public void StartAfterPick()
        {
            Difficulty = numDiff;
            long nowseed = seeds[numPickCave];
            if (seed != "")
                nowseed = long.Parse(seed);
            Utily.ChangeSeed(nowseed);
            map = new Map();
            state = ControlState.Cave;
            view.ClearConsole();
            view.AddComand(String.Format(Messages.MapSeedString, nowseed), false);
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Down(e);
            }
            if (state == ControlState.Cave && MiniGameEnd && view.IsEndAnimation())
            {
                view.StopAnimation();
                RegionCave rg = view.GetRegionCave(e.X, e.Y);
                if (rg >= 0 && (int)rg < 6 && map.isActive[map.Room][(int)rg])
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        int WumpusRun = Utily.Next() % 8;
                        if (WumpusRun < Difficulty)
                            map.WumpusGoAway(1);
                        int add = map.Move((int)rg);
                        player.AddCoins(add);
                        score.AddScores(5 * add);
						view.StartAddCoinAnimation(add);
                        if (map.danger == Danger.Bat)
                            view.StartBatAnimation();
                        view.StartMoveAnimation((int)rg);
                        CheckDanger = false;
                    }
                    else
                    {
                        player.PushArrow();
                        map.PushArrow((int)rg);
                        view.StartArrowAnimation((int)rg);
                        List<string> achiv = new List<string>();
                        player.GetAchievement(achiv);
                        score.getAchievement(achiv);
                    }
                }
                if (rg == RegionCave.BuyArrow)
                {
                    if (player.CanBuyArrow())
                    {
                        StoryMiniGame = StoryMG.BuyArrow;
                        MiniGameEnd = false;
                        minigame = new MiniGame(Width, Height);
						minigame.HintText = Messages.ArrowHintText;
                        minigame.InitializeMiniGame(ArroDiff[Difficulty]);
                        UseMiniGame = true;
                        player.BuyArrows();
                    }
                    else
                        view.AddComand(string.Format(Messages.NotEnoughCoins, player.NeedForBuyArrows()), true, false);
                }
                if (rg == RegionCave.BuyHint)
                {
                    if (player.CanBuyHint())
                    {
                        StoryMiniGame = StoryMG.BuyHint;
                        MiniGameEnd = false;
                        minigame = new MiniGame(Width, Height);
						minigame.HintText = Messages.HintHintText;
                        minigame.InitializeMiniGame(HintDiff[Difficulty]);
                        UseMiniGame = true;
                        player.BuyHint();
                    }
                    else
                        view.AddComand(string.Format(Messages.NotEnoughCoins, player.NeedForBuyHint()), true, false);
                }
                if (rg == RegionCave.UpConsole)
                    view.ChangeIndex(1);
                if (rg == RegionCave.DownConsole)
                    view.ChangeIndex(-1);
            }
            if (state == ControlState.MainMenu)
            {
                RegionMenu rg = view.GetRegionMainMenu(e.X, e.Y);
                if (rg == RegionMenu.NewGame)
                {
                    NewGame();
                }
                if (rg == RegionMenu.Continue)
                {
                    ContinueMenu();
                }
                if (rg == RegionMenu.ScoreList)
                {
					score = new Scores(Width, Height);
					score.LoadLeaders(false);
                    state = ControlState.ScoreList;
                }
                if (rg == RegionMenu.Exit)
                {
                    Application.Exit();
                }
            }
            if (state == ControlState.PickCave)
            {
                RegionPickCave rg = view.GetRegionPickCave(e.X, e.Y);
                int Intrg = (int)rg;
                if (Intrg < 5)
                    numPickCave = Intrg;
                if (Intrg >= (int)RegionPickCave.Diff1 && Intrg <= (int)RegionPickCave.Diff3)
                    numDiff = Intrg - (int)RegionPickCave.Diff1;
                if (rg == RegionPickCave.Play)
                {
                    StartAfterPick();
                }
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Up(e);
            }
            if (state == ControlState.LastWindow && score != null)
                score.MouseUp(e);
			if (e.Button == MouseButtons.Left && score != null && (state == ControlState.LastWindow || state == ControlState.ScoreList)) {
				ScoreRegion rg = score.GetRegion(e.X, e.Y);
				if (rg == ScoreRegion.Back) {
					score = null;
					player = null;
					map = null;
					minigame = null;
					state = ControlState.MainMenu;
				}
			}
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (state == ControlState.Cave && !MiniGameEnd)
            {
                minigame.Move(e);
            }
            if (state == ControlState.LastWindow || state == ControlState.ScoreList)
                score.MouseMove(e);
        }
    }
}

