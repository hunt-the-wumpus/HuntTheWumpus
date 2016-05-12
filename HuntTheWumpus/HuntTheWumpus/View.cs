using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace HuntTheWumpus
{
    public enum RegionCave
    {
        Up,
        UpLeft,
        DownLeft,
        Down,
        DownRight,
        UpRight,
        BuyArrow,
        BuyHint,
        UpConsole,
        DownConsole,
        Empty
    }
    public enum RegionPickCave
    {
        Cave0,
        Cave1,
        Cave2,
        Cave3,
        Cave4,
        Play,
        Empty
    }
    public enum RegionMenu
    {
        NewGame,
        Continue,
        ScoreList,
        Exit,
        Empty
    }

    public class View
    {
        public System.Drawing.Graphics Graphics { get; private set; }
        private System.Drawing.Bitmap Bitmap;

		public bool IsAnimated { get; set; }
        public bool IsBaner { get; set; }
		public bool IsBatAnimated { get; private set; }
        private Stopwatch BanerTimer;

        private Image MainMenuImage;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Form1 MainForm { get; private set; }

        const int CountImageRoom = 4;
        private CompressionImage[] CaveRoom;
        private CompressionImage PitRoom;
        private int[] TypeImageRoom;
        private CompressionImage DarkRoom;
        private CompressionImage[] room = new CompressionImage[6];
        private CompressionImage Bat;
        private CompressionImage BackGround;
        private List<float> StownPosX = new List<float>();
        private List<float> StownPosY = new List<float>();
        private List<float> ScaleRoomX = new List<float>();
        private List<float> ScaleRoomY = new List<float>();

        private List<string> ConsoleList;
        private int IndexConsole = 0;

        private float Progress = 0.0f;
        private List<bool>[] isActiveLast;
        private List<Danger> DangerListLast;
        private Danger dangerLast;
        private int CurrentRoomLast;
        private int numberstone = 0;
        private int length = 0;
        private int deltaY = 0;
		private int normallength;

        public void InitEvent(KeyEventHandler KeyDown, MouseEventHandler MouseDown, MouseEventHandler MouseUp, MouseEventHandler MouseMove)
        {
            MainForm.InitEvent(KeyDown, MouseDown, MouseUp, MouseMove);
        }

        public View(int width, int height)
        {
            Bitmap = new System.Drawing.Bitmap(width, height);
            Graphics = System.Drawing.Graphics.FromImage(Bitmap);
            ConsoleList = new List<string>();
            BanerTimer = new Stopwatch();
            Width = width;
            Height = height;
			length = Height * 8 / 12;
			normallength = length;
            #region setted images
            room[0] = new CompressionImage("data/Cave/TryTop1.png", length / 3, length / 3);
            room[1] = new CompressionImage("data/Cave/TryUpperLeft1.png", length / 3, length / 3);
            room[2] = new CompressionImage("data/Cave/TryBottomLeft1.png", length / 3, length / 3);
            room[3] = new CompressionImage("data/Cave/TryTop1.png", length / 3, length / 3);
            room[4] = new CompressionImage("data/Cave/TryUpperLeft1.png", length / 3, length / 3);
            room[5] = new CompressionImage("data/Cave/TryBottomLeft1.png", length / 3, length / 3);

            for (int i = 0; i < 6; ++i)
            {
                room[i].ScreenWidth = width;
                room[i].ScreenHeight = height;
            }
            CaveRoom = new CompressionImage[CountImageRoom];
            for (int i = 0; i < CountImageRoom; ++i)
            {
                CaveRoom[i] = new CompressionImage("data/Cave/ColorRoom" + (i + 1) + ".png", length, length);
                CaveRoom[i].ScreenWidth = width;
                CaveRoom[i].ScreenHeight = height;
            }
            PitRoom = new CompressionImage("data/Cave/PitRoom.png", length, length);
            PitRoom.ScreenHeight = height;
            PitRoom.ScreenWidth = width;
            TypeImageRoom = new int[30];
            UpdateImage();
            DarkRoom = new CompressionImage("data/Cave/DarkRoom.png", length, length);
            DarkRoom.ScreenWidth = width;
            DarkRoom.ScreenHeight = height;

            Bat = new CompressionImage("data/Cave/Bat.png", length, length);
            BackGround = new CompressionImage("data/Cave/background.png", width, 120);
            #endregion
            #region setted constants
            StownPosX.Add(1.0f / 3.0f - 1.0f / 70.0f); // 0 item
            StownPosY.Add(-1.0f / 6.0f);
            StownPosX.Add(0);           // 1 item
            StownPosY.Add(1.0f / 9.0f);
            StownPosX.Add(0);           // 2 item
            StownPosY.Add(1.0f / 9.0f + 1.0f / 2.0f);
            StownPosX.Add(StownPosX[0]);    // 3 item
            StownPosY.Add(StownPosY[0] + 1);
            StownPosX.Add(StownPosX[1] + 2.0f / 3.0f);    // 4 item
            StownPosY.Add(StownPosY[1] + 1.0f / 2.0f);
            StownPosX.Add(StownPosX[2] + 2.0f / 3.0f);    // 5 item
            StownPosY.Add(StownPosY[2] - 1.0f / 2.0f);
            // for animation
            ScaleRoomX.Add(0);          // 0 item
            ScaleRoomY.Add(-1);
            ScaleRoomX.Add(-0.67f);     // 1 item
            ScaleRoomY.Add(-0.5f);
            ScaleRoomX.Add(-0.67f);     // 2 item
            ScaleRoomY.Add(0.5f);
            ScaleRoomX.Add(0);          // 3 item
            ScaleRoomY.Add(1);
            ScaleRoomX.Add(0.67f);      // 4 item
            ScaleRoomY.Add(0.5f);
            ScaleRoomX.Add(0.67f);      // 5 item
            ScaleRoomY.Add(-0.5f);
            #endregion
            MainMenuImage = Image.FromFile(@".\data\Sprites\Menu.png");
            MainForm = new Form1(Drawing, width, height);
            MainForm.Show();
            deltaY = height / 36 + 30;
        }

        public void UpdateImage()
        {
            for (int i = 0; i < 30; ++i)
                TypeImageRoom[i] = Utily.Next() % CountImageRoom;
        }

        public void DrawText(string str, int x, int y, int size_font)
        {
            Font fn = new Font("Arial", size_font);
            Graphics.DrawString(str, fn, Brushes.Black, x, y);
        }

        public void DrawText(string str, int x, int y, int size_font, string typefont)
        {
            Font fn = new Font(typefont, size_font);
            Graphics.DrawString(str, fn, Brushes.Black, x, y);
        }

        public void DrawText(string str, int x, int y, int size_font, string typefont, Color cl)
        {
            Font fn = new Font(typefont, size_font);
            Graphics.DrawString(str, fn, new SolidBrush(cl), x, y);
        }

        public void DrawTextMid(string str, int x, int y, int size_font, string typefont, Color cl)
        {
            Font fn = new Font(typefont, size_font);
            Graphics.DrawString(str, fn, new SolidBrush(cl), x - Graphics.MeasureString(str, fn).Width / 2, y);
        }

        public void Drawing(System.Object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bitmap, 0, 0);
        }

        public void Clear()
        {
            Graphics.Clear(System.Drawing.Color.White);
        }

        public void Clear(System.Drawing.Color cl)
        {
            Graphics.Clear(cl);
        }

        public bool Created()
        {
            return MainForm.Created;
        }

        public void DrawMainMenu()
        {
            Graphics.DrawImage(MainMenuImage, 0, 0, Width, Height);
        }

        public RegionMenu GetRegionMainMenu(int x, int y)
        {
            if (x < 287 || x > 745)
                return RegionMenu.Empty;
            if (y >= 182 && y <= 256)
                return RegionMenu.NewGame;
            if (y >= 290 && y <= 365)
                return RegionMenu.Continue;
            if (y >= 394 && y <= 459)
                return RegionMenu.ScoreList;
            if (y >= 486 && y <= 549)
                return RegionMenu.Exit;
            return RegionMenu.Empty;
        }

        public void DrawRoom(int x, int y, Danger danger, int number, List<int>[] graph, List<bool>[] Active, bool StartRoom)
        {
			if (IsBatAnimated && !IsAnimated) {
				if (StartRoom && danger != Danger.Pit)
					Graphics.DrawImage(CaveRoom[TypeImageRoom[number]].CompressedImage, x, y, length, length);
				else if (StartRoom)
					Graphics.DrawImage(PitRoom.CompressedImage, x, y, length, length);
				else
					Graphics.DrawImage(DarkRoom.CompressedImage, x, y, length, length);
			} else {
				//Graphics.DrawImage(img, new Rectangle(x, y, length, length));
				if (StartRoom && danger != Danger.Pit)
					CaveRoom[TypeImageRoom[number]].Draw(Graphics, x, y);
				else if (StartRoom)
					PitRoom.Draw(Graphics, x, y);
				else
					DarkRoom.Draw(Graphics, x, y);
			}
            /*if (danger == Danger.Bat && DrawDanger) {
				Bat.Draw(Graphics, x, y);
			}*/
            for (int i = 0; i < 6; i++)
            {
                if (StartRoom && Active[number][i])
                {
					if (IsBatAnimated && !IsAnimated) {
						Graphics.DrawImage(room[i].CompressedImage, x + (int)(length * StownPosX[i]), y + (int)(length * StownPosY[i]), length / 3, length / 3);
					} else {
						room[i].Draw(Graphics, x + (int)(length * StownPosX[i]), y + (int)(length * StownPosY[i]));
					}
                }
            }
        }

        public void DrawAllFriends(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int basex, int basey, bool activated = true)
        {
            int loclen = length - 1;
            DrawRoom(basex, basey - loclen, DangerList[0], graph[CurrentRoom][0], graph, isActive, false);
            DrawRoom(basex, basey + loclen, DangerList[3], graph[CurrentRoom][3], graph, isActive, false);
            DrawRoom(basex + loclen * 2 / 3, basey - loclen / 2, DangerList[5], graph[CurrentRoom][5], graph, isActive, false);
            DrawRoom(basex - loclen * 2 / 3, basey - loclen / 2, DangerList[1], graph[CurrentRoom][1], graph, isActive, false);
            DrawRoom(basex + loclen * 2 / 3, basey + loclen / 2, DangerList[4], graph[CurrentRoom][4], graph, isActive, false);
            DrawRoom(basex - loclen * 2 / 3, basey + loclen / 2, DangerList[2], graph[CurrentRoom][2], graph, isActive, false);
            DrawRoom(basex, basey, danger, CurrentRoom, graph, isActive, activated);
        }

		private Stopwatch moveTimerAnimation;
		private Stopwatch batTimerAnimation;

        public void DrawCave(List<int>[] graph, List<bool>[] isActive, List<Danger> DangerList, Danger danger, int CurrentRoom, int Coins, int Arrows)
        {
			//Clear(Color.White);
			//Clear();
			if (!IsAnimated) {
				DrawAllFriends(graph, isActive, DangerList, danger, CurrentRoom, Width / 2 - length / 2, (Height - length) / 2 - deltaY);
				//Graphics.DrawImage(bar, new Rectangle(0, Height - 60, Width, 30));
				DrawInterface(Coins, Arrows, CurrentRoom);
				//Graphics.DrawEllipse(Pens.Red, Width / 2 - length / 2 + length / 3 - 10, Height / 12 - deltaY - 10, 20, 20);
				//Graphics.DrawEllipse(Pens.Blue, Width / 2 - length / 2 - 10, Height / 12 + length / 2 - deltaY - 10, 20, 20);
				isActiveLast = isActive;
				DangerListLast = DangerList;
				dangerLast = danger;
				CurrentRoomLast = CurrentRoom;
			}
			if (IsAnimated) {
				long Milliseconds = moveTimerAnimation.ElapsedMilliseconds;
				Progress = Milliseconds / 2500.0f;
				int TargetCenterX = Width / 2 - length / 2;
				int TargetCenterY = (Height - length) / 2 - deltaY;
				DrawAllFriends(graph, null, DangerListLast, Danger.Empty, 0, TargetCenterX - (int)(length * ScaleRoomX[numberstone] * Progress) + (int)(ScaleRoomX[numberstone] * length), TargetCenterY - (int)(length * ScaleRoomY[numberstone] * Progress) + (int)(ScaleRoomY[numberstone] * length), false);
				DrawAllFriends(graph, isActiveLast, DangerListLast, dangerLast, CurrentRoomLast, TargetCenterX - (int)(length * ScaleRoomX[numberstone] * Progress), TargetCenterY - (int)(length * ScaleRoomY[numberstone] * Progress));
				DrawInterface(Coins, Arrows, CurrentRoom);
				if (Progress >= 1.0f) {
					Progress = 0.0f;
					moveTimerAnimation.Stop();
					IsAnimated = false;
				}
			}
			if (IsBatAnimated && !IsAnimated) {
				if (batTimerAnimation == null) {
					batTimerAnimation = new Stopwatch();
					batTimerAnimation.Start();
				} else {
					long Milliseconds = batTimerAnimation.ElapsedMilliseconds;
					float progress = Milliseconds / 2500.0f;
					length = (int)(normallength * (1.0f - progress));
					if (progress >= 1.0f) {
						IsBatAnimated = false;
						batTimerAnimation = null;
						length = normallength;
					}
				}
			}
		}

		public void StartMoveAnimation(int direction) {
			Progress = 0.0f;
			numberstone = direction;
			IsAnimated = true;
			moveTimerAnimation = new Stopwatch();
			moveTimerAnimation.Start();
		}

		public void StartBatAnimation() {
			IsBatAnimated = true;
		}

        public void DrawInterface(int coins, int arrows, int room)
        {
            int yup = Height - 120;
            BackGround.Draw(Graphics, 0, Height - 120);
            //Graphics.FillRectangle(Brushes.Gray, 0, yup, Width, 120);
            DrawText("Arrows " + arrows, 10, yup + 10, 20, "Arial", Color.White);
            DrawText("Coins " + coins, 10, yup + 40, 20, "Arial", Color.White);
            DrawText("Room " + (room + 1), 10, yup + 70, 20, "Arial", Color.White);
            DrawText("Buy Arrows", 170, yup + 20, 22, "Arial", Color.FromArgb(0, 255, 0));
            DrawText("Buy Hint", 170, yup + 70, 20, "Arial", Color.FromArgb(0, 255, 0));
            for (int i = IndexConsole; i > IndexConsole - 5 && i >= 0; --i)
                DrawText(ConsoleList[i], 730, yup + 10 + (IndexConsole - i) * 18, 15, "Consolas", Color.White);
            if (IsBaner && BanerTimer.ElapsedMilliseconds > 2000)
            {
                IsBaner = false;
                BanerTimer.Reset();
            }
            if (IsBaner)
            {
                DrawTextMid(ConsoleList[ConsoleList.Count - 1], Width / 2, 100, 30, "Batang", Color.FromArgb(0, 255, 0));
            }
        }

        private bool isLeftUpper(int x1, int y1, int x2, int y2, int ix, int iy)
        {
            int proectionX = x2;
            int proectionY = iy;
            int BigSizeX = x1 - x2;
            int BigSizeY = y2 - y1;
            int SmallSizeY = y2 - iy;
            float k = (float)(SmallSizeY) / BigSizeY;
            float HipotinuzeX = proectionX + (int)(BigSizeX * k);
            return (HipotinuzeX - ix > 0 && iy > y1 && iy < y2);
        }

        private bool isRightUpper(int x1, int y1, int x2, int y2, int ix, int iy)
        {
            int proectionX = x2;
            int proectionY = iy;
            int BigSizeX = x2 - x1;
            int BigSizeY = y2 - y1;
            int SmallSizeY = y2 - iy;
            float k = (float)(SmallSizeY) / BigSizeY;
            float HipotinuzeX = x2 - (int)(BigSizeX * k);
            return (ix - HipotinuzeX > 0 && iy > y1 && iy < y2);
        }

        private bool isLeftDown(int x1, int y1, int x2, int y2, int ix, int iy)
        {
            int proectionX = x1;
            int proectionY = iy;
            int BigSizeX = x2 - x1;
            int BigSizeY = y2 - y1;
            int SmallSizeY = iy - y1;
            float k = (float)(SmallSizeY) / BigSizeY;
            float HipotinuzeX = proectionX + (int)(BigSizeX * k);
            return (HipotinuzeX - ix > 0 && iy > y1 && iy < y2);
        }

        private bool isRightDown(int x1, int y1, int x2, int y2, int ix, int iy)
        {
            int proectionX = x1;
            int proectionY = iy;
            int BigSizeX = x1 - x2;
            int BigSizeY = y2 - y1;
            int SmallSizeY = iy - y1;
            float k = (float)(SmallSizeY) / BigSizeY;
            float HipotinuzeX = proectionX - (int)(BigSizeX * k);
            return (ix - HipotinuzeX > 0 && iy > y1 && iy < y2);
        }

        private bool isUnder(int x1, int x2, int y12, int x, int y)
        {
            return (y < y12 && x1 < x && x2 > x);
        }

        private bool isDown(int x1, int x2, int y12, int x, int y)
        {
            return (y > y12 && x1 < x && x2 > x);
        }

        public RegionCave GetRegionCave(int x, int y)
        {
            RegionCave result = RegionCave.Empty;
            int yup = Height - 120;
            if (isLeftUpper(Width / 2 - length / 2 + length / 3, (Height - length) / 2 - deltaY, Width / 2 - length / 2, (Height - length) / 2 + length / 2 - deltaY, x, y))
            {
                return RegionCave.UpLeft;
            }
            if (isRightUpper(Width / 2 + length / 2 - length / 3, (Height - length) / 2 - deltaY, Width / 2 + length / 2, (Height - length) / 2 + length / 2 - deltaY, x, y))
            {
                return RegionCave.UpRight;
            }
            if (isLeftDown(Width / 2 - length / 2, (Height - length) / 2 + length / 2 - deltaY, Width / 2 - length / 2 + length / 3, (Height - length) / 2 + length - deltaY, x, y))
            {
                return RegionCave.DownLeft;
            }
            if (isRightDown(Width / 2 + length / 2, (Height - length) / 2 + length / 2 - deltaY, Width / 2 + length / 2 - length / 3, (Height - length) / 2 + length - deltaY, x, y))
            {
                return RegionCave.DownRight;
            }
            if (isUnder(Width / 2 - length / 2 + length / 3, Width / 2 + length / 2 - length / 3, (Height - length) / 2 - deltaY, x, y))
            {
                return RegionCave.Up;
            }
            if (isDown(Width / 2 - length / 2 + length / 3, Width / 2 + length / 2 - length / 3, (Height - length) / 2 + length - deltaY, x, y))
            {
                return RegionCave.Down;
            }


            if (x >= 168 && x <= 326 && y - yup >= 16 && y - yup <= 56)
            {
                return RegionCave.BuyArrow;
            }
            if (x >= 168 && x <= 326 && y - yup >= 71 && y - yup <= 104)
            {
                return RegionCave.BuyHint;
            }
            if (x >= 692 && x <= 718 && y >= yup + 26 && y <= yup + 45)
            {
                return RegionCave.UpConsole;
            }
            if (x >= 692 && x <= 718 && y >= yup + 65 && y <= yup + 82)
            {
                return RegionCave.DownConsole;
            }
            return result;
        }

        public void ClearConsole()
        {
            ConsoleList = new List<string>();
            AddComand("Left mouse bottom for#moving", false);
            AddComand("Right mouse bottom for#shot arrow", false);
            IndexConsole = ConsoleList.Count - 1;
        }

        public void AddComand(string s, bool b)
        {
            if (IndexConsole == ConsoleList.Count - 1)
                ++IndexConsole;
            string[] strs = s.Split('#');
            for (int i = strs.Length - 1; i >= 0; i--)
                ConsoleList.Add(strs[i]);
            if (b)
            {
                BanerTimer.Restart();
                IsBaner = true;
            }
        }

        public void ChangeIndex(int up)
        {
            if (ConsoleList.Count <= 5)
                return;
            if (up > 0)
                IndexConsole = Math.Min(IndexConsole + up, ConsoleList.Count - 1);
            else
                IndexConsole = Math.Max(IndexConsole + up, 4);
        }

        public void DrawPickCave(List<int>[] graph, List<bool>[] isActive, int num, string seed)
        {
            Clear(Color.DarkOrange);
            int size = 55;
            int d = 5;
            int hght = (int)(size * 1.732);
            for (int i = 0; i <= 6; ++i)
                for (int j = 0; j <= 5; ++j)
                {
                    int lx = i * size * 3 / 2 + 100 + i * d, ly = j * hght + (i % 2) * (hght + d) / 2 + 50 + j * d;
                    if (i < 6 && j < 5)
                    {
                        Point[] pn = new Point[6];
                        pn[0] = new Point(lx + size / 2, ly);
                        pn[1] = new Point(lx, ly + hght / 2);
                        pn[2] = new Point(lx + size / 2, ly + hght);
                        pn[3] = new Point(lx + size * 3 / 2, ly + hght);
                        pn[4] = new Point(lx + size * 2, ly + hght / 2);
                        pn[5] = new Point(lx + size * 3 / 2, ly);
                        Graphics.FillPolygon(Brushes.BlueViolet, pn);
                    }
                    Pen pen = new Pen(Color.Brown, 9);
                    for (int k = 0; k < 3; ++k)
                    {
                        int v = (i % 6) + (j % 5) * 6;
                        if (isActive[v][k])
                        {
                            if (k == 0 && i < 6)
                                Graphics.DrawLine(pen, lx + size, ly + hght / 6, lx + size, ly - hght / 6 - d);
                            if (k == 1 && ((i < 6 && j < 5) || (i == 6 && j > 0) || (j == 5 && i > 1 && i % 2 == 0)))
                                Graphics.DrawLine(pen, lx + 25, ly + 29, lx - 1, ly + 13);
                            if (k == 2 &&  j < 5)
                                Graphics.DrawLine(pen, lx + 25, ly + hght - 29, lx - 1, ly + hght - 13);
                            
                        }
                    }
                }
            for (int i = 0; i < 5; ++i)
            {
                if (i > 0)
                    Graphics.DrawLine(new Pen(Color.Black), i * Width / 5, Height - 120, i * Width / 5, Height);
                if (i != num)
                    Graphics.DrawLine(new Pen(Color.Black), i * Width / 5, Height - 120, (i + 1) * Width / 5, Height - 120);
				else {
					Graphics.FillRectangle(Brushes.Blue, i * Width / 5, Height - 120, Width / 5, 120);
				}
                DrawText((i + 1).ToString(), i * Width / 5 + 70, Height - 100, 40);
            }
            DrawText("Enter your seed:", 660, 100, 30);
            Graphics.FillRectangle(Brushes.White, 660, 145, 360, 50);
            DrawText(seed, 658, 150, 30);
			Graphics.FillRectangle(Brushes.Blue, 840, 540, 200, 90);
            DrawText("PLAY!", 850, 550, 40);
        }

        public RegionPickCave GetRegionPickCave(int x, int y)
        {
            if (y > Height - 120)
                return (RegionPickCave)(x * 5 / Width);
            if (y > 550 && x > 850)
                return RegionPickCave.Play;
            return RegionPickCave.Empty;
        }

		public void StopAnimation() {
			IsBaner = false;
			BanerTimer.Reset();
		}

        public bool IsEndAnimation()
        {
            return !IsAnimated;
        }
    }
}
//kekes