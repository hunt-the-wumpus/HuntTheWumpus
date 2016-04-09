using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace HuntTheWumpus {
	class Scores {
		// Player score
		public int Score { get; private set; }

		private int CanvasWidth;
		private int CanvasHeight;

		// Queue at draw achievements
		private List<string> Queue = new List<string>();
		private List<string> WasAhievements = new List<string>();

		private Image activeImage = null;
		private Image BackGround = null;

		private const double BeginDrawingPosition = -100;
		private const double EndDrawingPosition = 0;
		private const int TimerShowStarting = 3000;

		private string MessageAchievement = "СУДАРЬ ВЫ УПОРОЛИСЬ!!!";

		private Stopwatch Event_timer = new Stopwatch();

		private double AchievementDrawingPosition = 0;
		private double SpeedChangeDrawingPosition = 40;
		private int TimerShowAchievement = 3000;
		private int NowChange = 1;

		public Scores(int Width, int Height) {
			CanvasWidth = Width;
			CanvasHeight = Height;
			BackGround = Image.FromFile("data/Achievements/BackGround.png");
			Queue.Add("Sunduk.png/ДВЕНАДЦАТЬ ЧЕЛОВЕК#НА СУНДУК МЕРТВЕЦА");
			Queue.Add("Bottle.png/ЙО-ХО-ХО!#И БУТЫЛКА РОМА");
			Queue.Add("Demon.png/ПЕЙ И ДЬЯВОЛ#ДОВЕДЕТ#ТЕБЯ ДО КОНЦА");
			Queue.Add("Bottle.png/ЙО-ХО-ХО!#И БУТЫЛКА РОМА");
			Event_timer.Start();
		}

		/// Adding score
		public void AddScores(int add) {
			Score += add;
		}

		public void getAchievement(List<string> achievements) {
			for (int i = 0; i < achievements.Count; ++i) {
				Queue.Add(achievements[i]);
				WasAhievements.Add(achievements[i]);
			}
		}

		public void TickTime() {
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
			g.DrawString("Score: " + Score.ToString(), f, brush, 0, 20);
			int width = CanvasWidth / 5; //6
			int height = CanvasHeight / 8; //10
			if (activeImage != null) {
				g.DrawImage(BackGround, new Rectangle(CanvasWidth - width, (int)AchievementDrawingPosition, width, height));
				g.DrawImage(activeImage, new Rectangle(CanvasWidth - width + 5, (int)AchievementDrawingPosition + height / 4, height / 2, height / 2));
				g.DrawString("New achievement", new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), CanvasWidth - width / 3 * 2, (float)AchievementDrawingPosition + height / 6);
				string[] strings = MessageAchievement.Split('#');
				for (int i = 0; i < strings.Length; ++i) {
					g.DrawString(strings[i], new Font("Colibri", 8), new SolidBrush(Color.LimeGreen), CanvasWidth - width / 3 * 2, (float)AchievementDrawingPosition + height / 3 + i * 12);
				}
			}
		}
	}
}
