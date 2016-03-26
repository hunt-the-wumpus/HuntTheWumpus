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

		public Scores(int Width, int Height) {
			CanvasWidth = Width / 2;
			CanvasHeight = Height / 2;
		}

		/// Adding score
		public void AddScores(int add) {
			Score += add;
		}

		public void getAchievement(List<string> achievements) {
			for (int i = 0; i < achievements.Count; ++i) {
				Queue.Add(achievements[i]);
			}
		}

		public void DrawScores(Graphics g) {
			Font f = new Font("Arial", 15);
			Brush brush = new SolidBrush(Color.Yellow);
			g.DrawString("Score: " + Score.ToString(), f, brush, 0, 0);
			if (Queue.Count > 0) {
				//Image img;
				//img.
			}
		}
	}
}

