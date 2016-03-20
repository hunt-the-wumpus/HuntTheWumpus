using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace HuntTheWumpus {
	class MiniGame {
		/// This is TRUE, if player winner at last game, else it's FALSE
		public bool Is_Winner = true;
		/// This is TRUE, if now game initialized and playing, else it's FALSE
		public bool Is_playing = false;

		private List<int> CircleCoordinateX;
		private List<int> CircleCoordinateY;
		private const int radius = 10;
		private int Scale_Distance = 20;

		public MiniGame () {
			
		}

		/// Call this method for create new game
		public void InitializeMiniGame(int difficult) {
			CircleCoordinateX.Clear();
			CircleCoordinateY.Clear();
			Is_playing = true;
			StreamReader file = new StreamReader(@"Difficulties" + difficult.ToString() + ".txt");
			List<List<string>> files_Difficulties = new List<List<string>>();
			string line = "";
			int now_index = -1;
			while ((line = file.ReadLine()) != null) {
				if (line == "new_difficult") {
					files_Difficulties.Add(new List<string>());
					now_index++;
				} else {
					files_Difficulties[now_index].Add(line);
				}
			}
			file.Close();
			Random range = new Random();
			int number_file = range.Next(0, files_Difficulties[difficult - 1].Count - 1);
			file = new StreamReader(@"" + files_Difficulties[difficult - 1][number_file]);
			while ((line = file.ReadLine()) != null) {

			}
		}

		/// Call this method, then game playing
		public void DrawMiniGame(Graphics g) {
			if (!Is_playing) {
				return;
			}
			g.Clear(Color.FromArgb(80, 0, 0, 0));
			g.DrawEllipse(Pens.White, new Rectangle(50, 50, 2 * radius, 2 * radius));
		}

		public void MouseEvents() {

		}
	}
}
