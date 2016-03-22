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

		// If now analytics method is running it's TRUE
		// for locking mouse events
		private bool Is_Analytics = false;

		// While this bigger than zero, game playing
		private int PlayerPoints = 500;

		List<string> files_Difficulties = new List<string>();

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
			files_Difficulties.Clear();
			Is_playing = true;
			StreamReader file = new StreamReader(@"Difficulties" + difficult.ToString() + ".txt");
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
			for (int i = 0; i < CircleCoordinateX.Count && !Is_Analytics; ++i) {
				g.DrawEllipse(Pens.White, new Rectangle(
					(int)(CircleCoordinateX[i] * Scale_Distance) + CanvasWidth,
					(int)(CircleCoordinateY[i] * Scale_Distance) + CanvasHeight, 
					2 * radius, 2 * radius));
			}
		}

		public void TickTime(long Milliseconds) {
			if (Is_Analytics || !Is_playing) {
				return;
			}
			Scale_Distance += Speed_Change_Scale_Distance * Milliseconds / 1000;
		}

		private void Analytics() {
			int points = MaxPointsFromOneFigure;
			List<int> EndCoordinatsX = new List<int>();
			List<int> EndCoordinatsY = new List<int>();
			/* Set now coordinats, because tickTime can change coordinats */
			for (int i = 0; i < CircleCoordinateX.Count; ++i) {
				EndCoordinatsX.Add((int)(CircleCoordinateX[i] * Scale_Distance) + CanvasWidth);
				EndCoordinatsY.Add((int)(CircleCoordinateY[i] * Scale_Distance) + CanvasHeight);
			}
			/* Get how good drawing the figure */
			for (int i = 0; i < EndCoordinatsX.Count; ++i) {
				double minimal_distance = 1000000.0f;
				for (int j = 0; j < MousePositionsX.Count; ++j) {
					double now_distance = Math.Sqrt(Math.Pow(EndCoordinatsX[i] - MousePositionsX[j], 2) + Math.Pow(EndCoordinatsY[i] - MousePositionsY[j], 2));
					minimal_distance = Math.Min(now_distance, minimal_distance);
				}
				if (minimal_distance > radius) {
					points -= MaxPointsFromOneFigure / EndCoordinatsX.Count;
				} else {
					points -= (int)(MaxPointsFromOneFigure / EndCoordinatsX.Count * (float)radius - minimal_distance);
				}
			}
			/* "Added" points 
				if points bigger than zero
				create new figure
			*/
			PlayerPoints -= points;
			if (PlayerPoints <= 0) {
				Is_playing = false;
				Is_Winner = true;
			} else {
				Random range = new Random();
				Set_Figure(files_Difficulties[range.Next(0, files_Difficulties.Count)]);
			}
			Is_Analytics = false;
		}

		public void MouseEvents(/* Here must be parameters MouseEvents or ... */) {
			// I can't realize this, while Engine can't working with mouse
			if (Is_Analytics) {
				return;
			}
			/*if (Is_playing && /* mouse_down /) {
				// On Moving mouse
				MousePositionsX.Add(/* Here mouseX position/);
				MousePositionsY.Add(/* Here mouseY position/);
			}
			if (Is_playing && /* mouseup /) {
				Is_Analytics = true;
				Analytics();
			}*/
		}
	}
}
