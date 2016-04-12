using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace HuntTheWumpus {
	enum TypeAnimation {
		Move,
		QueueMove,
		Refresh,
		None
	}

	class ParticleEffect {

		public Image particle { get; set; }
		public int LongTime { get; set; }
		public int TimeNewEffect { get; set; }
		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }
		public TypeAnimation effect { get; set; }

		public List<Image> cadr { get; set; }
		private Stopwatch timer;
		public double StartPositionX { get; set; }
		public double StartPositionY { get; set; }
		public double EndPositionX { get; set; }
		public double EndPositionY { get; set; }

		private double NowPositionX;
		private double NowPositionY;
		private double MoveSpeedX;
		private double MoveSpeedY;
		private List<double> ImagePositionsX;
		private List<double> ImagePositionsY;
		private int TimeNext = 0;

		public ParticleEffect() {
			LongTime = 1000;
			effect = TypeAnimation.None;
			cadr = new List<Image>();
			ImagePositionsX = new List<double>();
			ImagePositionsY = new List<double>();
			timer = new Stopwatch();
		}

		public void Start() {
			timer.Start();
			if (effect == TypeAnimation.Move || effect == TypeAnimation.QueueMove) {
				NowPositionX = StartPositionX;
				NowPositionY = StartPositionY;
				MoveSpeedX = (EndPositionX - StartPositionX) * 1000 / LongTime;
				MoveSpeedY = (EndPositionY - StartPositionY) * 1000 / LongTime;
			}
		}

		public void Draw(Graphics g) {
			if (effect == TypeAnimation.Move) {
				g.DrawImage(particle, new Rectangle((int)NowPositionX, (int)NowPositionY, ImageWidth, ImageHeight));
			}
			if (effect == TypeAnimation.QueueMove) {
				for (int i = 0; i < ImagePositionsX.Count; ++i) {
					g.DrawImage(particle, new Rectangle((int)ImagePositionsX[i], (int)ImagePositionsY[i], ImageWidth, ImageHeight));
				}
			}
		}

		public void TickTime() {
			long Milliseconds = timer.ElapsedMilliseconds;
			timer.Stop();
			if (effect == TypeAnimation.Move) {
				NowPositionX += MoveSpeedX * Milliseconds / LongTime;
				NowPositionY += MoveSpeedY * Milliseconds / LongTime;
				if (NowPositionX > EndPositionX || NowPositionY > EndPositionY) {
					NowPositionX = StartPositionX;
					NowPositionY = StartPositionY;
				}
			}
			if (effect == TypeAnimation.QueueMove) {
				TimeNext += (int)Milliseconds;
				double maxX = 0;
				double maxY = 0;
				for (int i = 0; i < ImagePositionsX.Count; ++i) {
					ImagePositionsX[i] += MoveSpeedX * Milliseconds / LongTime;
					ImagePositionsY[i] += MoveSpeedY * Milliseconds / LongTime;
					maxX = Math.Max(maxX, ImagePositionsX[i]);
					maxY = Math.Max(maxY, ImagePositionsY[i]);
				}
				if (TimeNewEffect < TimeNext) {
					TimeNext = 0;
					ImagePositionsX.Add(StartPositionX);
					ImagePositionsY.Add(StartPositionY);
				}
				if (maxX > EndPositionX || maxY > EndPositionY) {
					ImagePositionsX.Remove(maxX);
					ImagePositionsY.Remove(maxY);
				}
			}
			timer.Restart();
		}
	}
}
