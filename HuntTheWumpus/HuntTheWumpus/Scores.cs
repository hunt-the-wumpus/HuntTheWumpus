using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace HuntTheWumpus {

	public enum ScoreState {
		Null,
		Achievements,
		Final,
		CheckingName,
		Leaders
	}
	public enum ScoreRegion {
		Null,
		Continue,
		Back
	}

	public class Button {
		public int x { get; set; }
		public int y { get; set; }
		public int width { get; private set; }
		public int height { get; private set; }
		public CompressionImage img;

		public Button(Image create, int w, int h) {
			width = w;
			height = h;
			img = new CompressionImage(create, width, height);
		}

		public Button(string s, int w, int h) {
			width = w;
			height = h;
			img = new CompressionImage(s, width, height);
		}

		public void Draw(Graphics g) {
			img.Draw(g, x, y);
		}

		public bool Clicked(int mousex, int mousey) {
			return (mousex > x) && (mousex < x + width) && (mousey > y) && (mousey < y + height);
		}
	}

	public class Scores {
		#region Player score
		public int Score { get; private set; }
		public ScoreState active { get; set; }
		#endregion

		#region Canvas properties
		private int CanvasWidth;
		private int CanvasHeight;
		#endregion

		#region Queue at draw achievements
		private List<string> Queue = new List<string>();
		private List<string> WasAchievements = new List<string>();
		#endregion

		#region Main images properties
		private Image activeImage = null;
		private Image BackGround = null;
		private Image FinalPicture = null;
		private CompressionImage ListBackground;
		#endregion

		#region Timer show achievements
		private const double BeginDrawingPosition = -250;
		private const double EndDrawingPosition = 0;
		private const int TimerShowStarting = 3000;
		private double AchievementDrawingPosition = 0;
		private double SpeedChangeDrawingPosition = 90;
		private int TimerShowAchievement = 3000;
		private int NowChange = 1;
		#endregion

		#region Integration
		private VKApi vk;
		private FaceBookApi face;
		#endregion

		#region Buttons
		private CompressionImage ShareForm;
		private Button ShareVK;
		private Button ShareFaceBook;
		private Button Continue;
		private Button Back;
		#endregion

		#region Hint messages
		private int HintX = -140;
		private int HintY = -140;
		private string HintMessage = "";
		#endregion

		#region Checking name properties
		private string name = "";
		private int editX = 0;
		private int editY = 0;
		private int editWidth = 490;
		private int editHeight = 50;
		#endregion

		#region Leaders
		private List<string> Players;
		private const int ScaleY = 50;
		private const string LeadersFile = "data/Leaders.dat";
		private const int FieldSize = 30;
		private int ActiveLeader = -1;
		private int ShowNow = -1;
		#endregion

		private string MessageAchievement = "";
		private bool Winner = false;
		private Stopwatch Event_timer = new Stopwatch();

		public Scores(int Width, int Height) {
			CanvasWidth = Width;
			CanvasHeight = Height;
			#region SetShareForm
			ShareForm = new CompressionImage("data/ShareButton.png", 125, 70);
			#endregion
			#region SetVKshareButton
			ShareVK = new Button("data/ShareVK.png", 30, 30);
			ShareVK.x = 50;
			ShareVK.y = Height - 30 - 40;
			#endregion
			#region SetFACEBOOKsharebutton
			ShareFaceBook = new Button("data/ShareFACEBOOK.png", 30, 30);
			ShareFaceBook.x = 50 + 60 + 3;
			ShareFaceBook.y = Height - 30 - 40;
			#endregion
			#region SetContinueButton
			Continue = new Button("data/continue.png", 150, 30);
			Continue.x = 50 + 30 + 3 + 30 + 3 + 50;
			Continue.y = Height - 30 - 40;
			#endregion
			#region SetBackButton
			Back = new Button("data/back.png", 150, 30);
			Back.x = (Width - 150) / 2;
			Back.y = Height - 30 - 50;
			#endregion
			#region SetEdit
			editX = (Width - editWidth) / 2;
			editY = (Height - editHeight) / 2;
			#endregion
			BackGround = Image.FromFile("data/Achievements/BackGround.png");
			ListBackground = new CompressionImage("data/Sprites/ListBackGround.png", Width, Height);
			active = ScoreState.Null;
			Event_timer.Start();
		}

		public void AddScores(int add) {
			Score += add;
		}

		public void getAchievement(List<string> achievements) {
			for (int i = 0; i < achievements.Count; ++i) {
				if (WasAchievements.IndexOf(achievements[i]) == -1) {
					Queue.Add(achievements[i]);
					AddScores(100);
					WasAchievements.Add(achievements[i]);
				}
			}
		}

		public void TickTime() {
			if (active == ScoreState.Final && vk != null) {
				vk.Access_authorize();
				return;
			}
			if (active == ScoreState.Final && face != null) {
				face.Access_code();
				return;
			}
			long Milliseconds = Event_timer.ElapsedMilliseconds;
			if (Queue.Count > 0 && activeImage == null) {
				activeImage = Image.FromFile("data/Achievements/" + Queue[0].Split('/')[0]);
				MessageAchievement = Queue[0].Split('/')[1];
				AchievementDrawingPosition = BeginDrawingPosition;
				TimerShowAchievement = TimerShowStarting;
			}
			if (TimerShowAchievement <= 0) {
				NowChange = -1;
			} else {
				NowChange = 1;
			}
			if (activeImage != null && EndDrawingPosition >= AchievementDrawingPosition && (TimerShowAchievement <= 0 || TimerShowAchievement == TimerShowStarting)) {
				AchievementDrawingPosition += NowChange * SpeedChangeDrawingPosition * Milliseconds / 1000.0f;
				AchievementDrawingPosition = Math.Min(AchievementDrawingPosition, EndDrawingPosition);
			}
			if (AchievementDrawingPosition == EndDrawingPosition && activeImage != null) {
				AchievementDrawingPosition = EndDrawingPosition;
				TimerShowAchievement -= (int)Milliseconds;
			}
			if (AchievementDrawingPosition < BeginDrawingPosition) {
				Queue.Remove(Queue[0]);
				activeImage = null;
				AchievementDrawingPosition = BeginDrawingPosition;
			}
			Event_timer.Restart();
		}

		public void Draw(Graphics g) {
			if (active == ScoreState.Achievements) {
				DrawScores(g);
			}
			if (active == ScoreState.Final) {
				DrawFinal(g);
			}
			if (active == ScoreState.CheckingName) {
				DrawCheckingName(g);
			}
			if (active == ScoreState.Leaders) {
				DrawLeaders(g);
			}
			if (active == ScoreState.Null) {
				g.Clear(Color.White);
				g.DrawString("Ps! before call, check active != null", new Font("Colibri", 20), Brushes.Black, 0, 0);
			}
		}

		private void DrawScores(Graphics g) {
			Font StandardFont = new Font("Colibri", 8);
			Brush brush = new SolidBrush(Color.Yellow);
			int width = CanvasWidth / 5;
			int height = width / 3;
			if (activeImage != null) {
				g.DrawImage(BackGround, new Rectangle(CanvasWidth - width, (int)AchievementDrawingPosition + height, width, height * 3 / 2));
				g.DrawImage(activeImage, new Rectangle(CanvasWidth - width + 5, (int)AchievementDrawingPosition, width, height));
				g.DrawString("New achievement!", StandardFont, new SolidBrush(Color.LimeGreen), CanvasWidth - width / 3 * 2, (float)AchievementDrawingPosition + height * 7 / 6);
				string[] strings = MessageAchievement.Split('#');
				for (int i = 0; i < strings.Length; ++i) {
					g.DrawString(strings[i], StandardFont, new SolidBrush(Color.LimeGreen), CanvasWidth - width + 5, (float)AchievementDrawingPosition + height * 7 / 6 + (i + 1) * 12);
				}
            }
		}

		private void DrawFinal(Graphics g) {
			DrawFinalMain(g);
			g.DrawImage(BackGround, new Rectangle(HintX, HintY, CanvasWidth / 5, CanvasWidth / 15));
			string[] strings = HintMessage.Split('#');
			for (int i = 0; i < strings.Length; ++i) {
				g.DrawString(strings[i], new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), HintX + 15, HintY + 5 + i * 12);
			}
			ShareForm.Draw(g, (ShareVK.x + ShareFaceBook.x - 95) / 2, (ShareVK.y - 30));
			ShareVK.Draw(g);
			ShareFaceBook.Draw(g);
			Continue.Draw(g);
		}

		private void DrawCheckingName(Graphics g) {
			ListBackground.Draw(g, 0, 0);
			Brush standardField = new SolidBrush(Color.Cyan);
			Brush standardText = new SolidBrush(Color.DarkBlue);
			Font fieldText = new Font("Arial", editHeight * 2 / 3);
			g.DrawString("Enter your name", fieldText, Brushes.White, 50, 50);
			g.FillRectangle(standardField, new Rectangle(editX, editY, editWidth, editHeight));
			g.DrawString(name, fieldText, standardText, editX + 3, editY + 1);
		}

		private void DrawLeaders(Graphics g) {
			ListBackground.Draw(g, 0, 0);
			g.DrawString("Leaders", new Font("Arial", 30), Brushes.White, 30, 30);
			Font StandardTextFont = new Font("Arial", 20);
			Brush StandardFieldBrush = new SolidBrush(Color.Cyan);
			Brush StandardTextBrush = new SolidBrush(Color.Black);
			int lastscores = -1;
			int point = -1;
			Brush[] best = new SolidBrush[3];
			best[2] = new SolidBrush(Color.FromArgb(255, 205, 127, 50));
			best[1] = new SolidBrush(Color.Silver);
			best[0] = new SolidBrush(Color.Gold);
			for (int i = 0; i < Players.Count; ++i) {
				string[] splitted = Players[i].Split(' ');
				string playername = splitted[2];
				string isvictory = splitted[1];
				if (isvictory == "True") {
					isvictory = "Victory";
				} else {
					isvictory = "Defeated";
				}
				string scores = splitted[0];
				int CountAchievements = splitted[3].Split('/').Length - 1;
				string achievements = CountAchievements.ToString() + " achievements";
				if (CountAchievements > 0) {
					achievements += "\t\t[Show]";
				}
				Brush FieldBrush = Brushes.White;
				if (i == ShowNow) {
					FieldBrush = Brushes.Green;
				}
				if (!(point < 3 && lastscores == Convert.ToInt32(scores))) {
					point++;
				}
				if (point > 2 && i != ShowNow) {
					FieldBrush = StandardFieldBrush;
				} else if (point <= 2) {
					FieldBrush = best[point];
				}
				g.FillRectangle(FieldBrush, new Rectangle(50, 50 + i * FieldSize + i * 2 + ScaleY, CanvasWidth - 100, FieldSize));
				g.DrawString(playername, StandardTextFont, StandardTextBrush, 50 + 3, 50 + i * FieldSize + i * 2 + 1 + ScaleY);
				g.DrawString(scores, StandardTextFont, StandardTextBrush, 50 + 3 + 200, 50 + i * FieldSize + i * 2 + 1 + ScaleY);
				g.DrawString(isvictory, StandardTextFont, StandardTextBrush, 50 + 3 + 200 + 75, 50 + i * FieldSize + i * 2 + 1 + ScaleY);
				g.DrawString(achievements, StandardTextFont, StandardTextBrush, 50 + 3 + 200 + 75 + 200, 50 + i * FieldSize + i * 2 + 1 + ScaleY);
				lastscores = Convert.ToInt32(scores);
			}
			if (ActiveLeader != -1) {
				Brush ShowAchBrush = new SolidBrush(Color.Gray);
				int width = 140;
				int height = 46;
				string[] ach = Players[ActiveLeader].Split(' ')[3].Split('/');
				int hw = (ach.Length - 1) % 3;
				if (ach.Length > 2) {
					hw = 3;
				}
				int HintWidth = hw * width + 10;
				int HintHeight = ((ach.Length + 1) / 3) * height;
				g.FillRectangle(ShowAchBrush, new Rectangle(CanvasWidth - HintWidth - 75, 50 + ActiveLeader * FieldSize + 2 * ActiveLeader + ScaleY, HintWidth, HintHeight));
				for (int i = 0; i < ach.Length; ++i) {
					if (ach[i] == "") {
						break;
					}
					Image img = Image.FromFile("data/Achievements/" + ach[i]);
					int activestring = i / 3;
					int number = i % 3;
					g.DrawImage(img, new Rectangle(CanvasWidth - HintWidth - 75 + 2 + number * width, 50 + ScaleY + ActiveLeader * FieldSize + 2 * ActiveLeader + 1 + height * activestring, width, height));
				}
			}
			Back.Draw(g);
		}

		public void SetFinalState(bool isWinner) {
			Winner = isWinner;
			if (isWinner) {
				FinalPicture = Image.FromFile("data/Final.png");
				AddScores(500);
			} else {
				FinalPicture = Image.FromFile("data/Defeat.png");
			}
			active = ScoreState.Final;
		}
		
		private void DrawFinalMain(Graphics g) {
			g.DrawImage(FinalPicture, 0, 0);
			g.DrawString("Your scores " + Score.ToString(), new Font("Arial", 20),  new SolidBrush(Color.Cyan), 75, 200);
			int activestring = 0;
			int DrawAchivements = 0;
			for (int i = 0; i < WasAchievements.Count; ++i) {
				if (DrawAchivements >= 3) {
					++activestring;
					DrawAchivements = 0;
				}
				Image img = Image.FromFile("data/Achievements/" + WasAchievements[i].Split('/')[0]);
				g.DrawImage(img, 80 + DrawAchivements * 140, 430 + activestring * 46, 140, 46);
				++DrawAchivements;
			}
		}

        private void ShareCreate() {
            Bitmap b = new Bitmap(CanvasWidth, CanvasHeight);
            Graphics g = Graphics.FromImage(b);
            DrawFinalMain(g);
            b.Save("data/Share.jpg");
        }

        public bool Clicked(int buttonX, int buttonY, int buttonSize, int x, int y) {
			return (buttonX < x && buttonX + buttonSize > x && buttonY < y && buttonY + buttonSize > y);
		}

		public void MouseUp(MouseEventArgs e) {
			if (active == ScoreState.Final && e.Button == MouseButtons.Left && Continue.Clicked(e.X, e.Y)) {
				active = ScoreState.CheckingName;
				name = "";
			}
			if (active == ScoreState.Final && e.Button == MouseButtons.Left && ShareVK.Clicked(e.X, e.Y)) {
				face = null;
				vk = null;
				ShareCreate();
				vk = new VKApi();
				vk.OauthAuthorize();
			}
			if (active == ScoreState.Final && e.Button == MouseButtons.Left && ShareFaceBook.Clicked(e.X, e.Y)) {
				vk = null;
				face = null;
				ShareCreate();
				face = new FaceBookApi();
				face.OauthAuthorize();
			}
		}

		public void MouseMove(MouseEventArgs e) {
			if (active == ScoreState.Final) {
				int activestring = 0;
				int DrawAchivements = 0;
				HintX = -140;
				HintY = -140;
				for (int i = 0; i < WasAchievements.Count; ++i) {
					if (DrawAchivements >= 3) {
						++activestring;
						DrawAchivements = 0;
					}
					int achX = 80 + DrawAchivements * 140;
					int achY = 430 + activestring * 46;
					if (e.X > achX && e.Y > achY && e.X < achX + 140 && e.Y < achY + 46) {
						HintX = e.X;
						HintY = e.Y;
						HintMessage = WasAchievements[i].Split('/')[1];
						break;
					}
					++DrawAchivements;
				}
			}
			if (active == ScoreState.Leaders) {
				int activestring = -1;
				for (int i = 0; i < Players.Count; ++i) {
					if (e.X > CanvasWidth - 400 && e.X < CanvasWidth - 50 && e.Y > 50 + ScaleY + i * FieldSize + i * 2 && e.Y < 50 + ScaleY + (i + 1) * FieldSize + i * 2) {
						activestring = i;
					}
				}
				ActiveLeader = activestring;				
			}
		}

		public ScoreRegion GetRegion(int x, int y) {
			if (Continue.Clicked(x, y) && active == ScoreState.Final) {
				return ScoreRegion.Continue;
			}
			if (active == ScoreState.Leaders && Back.Clicked(x, y)) {
				return ScoreRegion.Back;
			}
			return ScoreRegion.Null;
		}

		public void KeyDown(string c) {
			if (active == ScoreState.CheckingName) {
				if (name.Length < 15 && c.Length == 1) {
					name += c;
				}
				if (c == "del" && name.Length > 0) {
					name = name.Remove(name.Length - 1);
				}
				if (c == "enter" && name.Length > 0) {
					SaveLeader();
					LoadLeaders();
				}
			}
		}

		public int Comparer(string a, string b) {
			int valuea = Convert.ToInt32(a.Split(' ')[0]);
			int valueb = Convert.ToInt32(b.Split(' ')[0]);
			if (valuea == valueb) {
				string wa = a.Split(' ')[1];
				string wb = b.Split(' ')[1];
				return a.CompareTo(b);
			}
			if (valuea > valueb) {
				return 1;
			} else {
				return -1;
			}
		}

		private string ArrayToString(List<string> array, char Cut = '~', int AddedPart = 0) {
			string result = "";
			for (int i = 0; i < array.Count; ++i) {
				if (Cut != '~') {
					result += array[i].Split(Cut)[AddedPart] + "/";
				} else {
					result += array[i] + "/";
				}
			}
			return result;
		}

		public void LoadLeaders(bool FromLast = true) {
			Players = new List<string>();
			StreamReader file = new StreamReader(@"" + LeadersFile);
			string line = "";
			while ((line = file.ReadLine()) != null) {
				Players.Add(line);
			}
			Players.Remove("");
			Players.Sort(Comparer);
			Players.Reverse();
			for (int i = 0; i < Players.Count && FromLast; i++) {
				string nowPlayer = Score.ToString() + " " + Winner.ToString() + " " + name + " " + ArrayToString(WasAchievements, '/', 0);
				if (nowPlayer == Players[i]) {
					ShowNow = i;
					break;
				}
			}
			active = ScoreState.Leaders;
		}

		private void SaveLeader() {
			StreamWriter file = new StreamWriter(@"" + LeadersFile, true);
			file.WriteLine(Score.ToString() + " " + Winner.ToString() + " " + name + " " + ArrayToString(WasAchievements, '/'));
			file.Close();
		}
	}
}
