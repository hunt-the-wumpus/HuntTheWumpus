using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace HuntTheWumpus {
	class MiniGame {
		// This is TRUE, if player winner at last game, else it's FALSE
		public bool Is_Winner = true;
		// This is TRUE, if now game initialized and playing, else it's FALSE
		public bool Is_playing = false;

		// This integers for set height and width canvas
		// and correct drawing figure
		private int CanvasHeight;
		private int CanvasWidth;

		// Coordinats of circles points
		private List<int> CircleCoordinateX = new List<int>();
		private List<int> CircleCoordinateY = new List<int>();

		// Coordinats of mouse cursors positions,
		// when user draw a figure
		private List<int> MousePositionsX = new List<int>();
		private List<int> MousePositionsY = new List<int>();

		// Radius of point
		private const int radius = 10;

		private const int MaxPointsFromOneFigure = 750;
		private const float Speed_Change_Scale_Distance = 1.0f;

		// Scale for transform figure position to screen position 
		private float Scale_Distance = 20.0f;

		/// Class constructor, say me window's width and height
		public MiniGame (int DrawingWidth, int DrawingHeight) {
			CanvasWidth = DrawingWidth / 2;
			CanvasHeight = DrawingHeight / 2;
		}

		/// Read and apply info about figure
		private void Set_Figure(string filename) {
			//* Clear memory about last drawing figure... *//
			CircleCoordinateX.Clear();
			CircleCoordinateY.Clear();
			MousePositionsX.Clear();
			MousePositionsY.Clear();			
			//* ...and read info about new figure */
			StreamReader file = new StreamReader(@"" + filename);
			string line = "";
			while ((line = file.ReadLine()) != null) {
				string[] points = line.Split(' ').ToArray();
				int x = Convert.ToInt32(points[0]);
				CircleCoordinateX.Add(x);
				CircleCoordinateY.Add(int.Parse(points[1]));
			}
			Scale_Distance = 20.0f;
		}

		/// Call this method for create new game
		public void InitializeMiniGame(int difficult) {
			//* Read info about game level and set random figure *//
			Is_playing = true;
			StreamReader file = new StreamReader(@"Difficulties" + difficult.ToString() + ".txt");
			List<string> files_Difficulties = new List<string>();
			string line = "";
			while ((line = file.ReadLine()) != null) {
				files_Difficulties.Add(line);
			}
			file.Close();
			Random range = new Random();
			Set_Figure(files_Difficulties[range.Next(0, files_Difficulties.Count)]);
		}

		/// Call this method, then game playing
		public void DrawMiniGame(Graphics g) {
			//* If game not initialized, then we not drawing *//
			if (!Is_playing) {
				return;
			}
			//* Clear window and drawing the points *//
			g.Clear(Color.FromArgb(80, 0, 0, 0));
			for (int i = 0; i < CircleCoordinateX.Count; ++i) {
				g.DrawEllipse(Pens.White, new Rectangle(
					CircleCoordinateX[i] * (int)Scale_Distance + CanvasWidth,
					CircleCoordinateY[i] * (int)Scale_Distance + CanvasHeight, 
					2 * radius, 2 * radius));
			}
		}

		public void TickTime(long Milliseconds) {
			Scale_Distance += Speed_Change_Scale_Distance * Milliseconds / 1000;
		}

		public void MouseEvents() {

		}
	}
}
