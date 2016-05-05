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

	public class Scores {
		// Player score
		public int Score { get; private set; }
		public bool Final { get; set; }

		private int CanvasWidth;
		private int CanvasHeight;

		// Queue at draw achievements
		private List<string> Queue = new List<string>();
		private List<string> WasAchievements = new List<string>();

		private Image activeImage = null;
		private Image BackGround = null;
		private Image FinalPicture = null;

		private const double BeginDrawingPosition = -250;
		private const double EndDrawingPosition = 0;
		private const int TimerShowStarting = 3000;

		private string MessageAchievement = "";

		private Stopwatch Event_timer = new Stopwatch();

		private double AchievementDrawingPosition = 0;
		private double SpeedChangeDrawingPosition = 45;
		private int TimerShowAchievement = 3000;
		private int NowChange = 1;
		private VKApi vk;
		private FaceBookApi face;
		private bool Winner = false;

		// This properties for draw button "Share in VK"
		private int ShareVKbuttonX;
		private int ShareVKbuttonY;
		private int ShareVKbuttonSize = 30;
		private CompressionImage Vk;

		private int HintX = -140;
		private int HintY = -140;
		private string HintMessage = "";

		public Scores(int Width, int Height) {
			CanvasWidth = Width;
			CanvasHeight = Height;
			ShareVKbuttonX = 50;
			ShareVKbuttonY = Height - ShareVKbuttonSize - 40;
			Vk = new CompressionImage("data/ShareVK.png", ShareVKbuttonSize, ShareVKbuttonSize);
			BackGround = Image.FromFile("data/Achievements/BackGround.png");
			List<string> bb = new List<string>();
			Event_timer.Start();
		}

		/// Adding score
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
			if (Final && vk != null) {
				vk.Access_authorize();
				return;
			}
			if (Final && face != null) {
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

		public void DrawScores(Graphics g) {
			Font f = new Font("Arial", 15);
			Brush brush = new SolidBrush(Color.Yellow);
			//g.DrawString("Score: " + Score.ToString(), f, brush, 0, 20);
			int width = CanvasWidth / 5; //6
			int height = width / 3; //10
			if (activeImage != null) {
				g.DrawImage(BackGround, new Rectangle(CanvasWidth - width, (int)AchievementDrawingPosition + height, width, height * 3 / 2));
				g.DrawImage(activeImage, new Rectangle(CanvasWidth - width + 5, (int)AchievementDrawingPosition, width, height));
				g.DrawString("New achievement", new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), CanvasWidth - width / 3 * 2, (float)AchievementDrawingPosition + height * 7 / 6);
				string[] strings = MessageAchievement.Split('#');
				for (int i = 0; i < strings.Length; ++i) {
					g.DrawString(strings[i], new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), CanvasWidth - width + 5, (float)AchievementDrawingPosition + height * 7 / 6 + (i + 1) * 12);
				}
            }
		}

		public void SetFinalState(bool isWinner) {
			Winner = isWinner;
			Final = true;
			if (isWinner) {
				FinalPicture = Image.FromFile("data/Final.png");
				AddScores(500);
			} else {
				FinalPicture = Image.FromFile("data/Defeat.png");
			}
		}

		private void DrawFinalForShare() {
			Bitmap b = new Bitmap(CanvasWidth,CanvasHeight);
			Graphics g = Graphics.FromImage(b);
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
			b.Save("data/Share.jpg");
		}

		public void DrawFinal(Graphics g) {
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
			g.DrawImage(BackGround, new Rectangle(HintX, HintY, CanvasWidth / 5, CanvasWidth / 15));
			string[] strings = HintMessage.Split('#');
			for (int i = 0; i < strings.Length; ++i) {
				g.DrawString(strings[i], new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), HintX + 15, HintY + 5 + i * 12);
			}
			Vk.Draw(g, ShareVKbuttonX, ShareVKbuttonY);
		}

		bool Clicked(int buttonX, int buttonY, int buttonSize, int x, int y) {
			return (buttonX < x && buttonX + buttonSize > x && buttonY < y && buttonY + buttonSize > y);
		}

		public void MouseUp(MouseEventArgs e) {
			if (Final && e.Button == MouseButtons.Left && Clicked(ShareVKbuttonX, ShareVKbuttonY, ShareVKbuttonSize, e.X, e.Y)) {
				DrawFinalForShare();
				face = new FaceBookApi();
				face.OauthAuthorize();
			}
		}

		public void MouseMove(MouseEventArgs e) {
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

	}
}
