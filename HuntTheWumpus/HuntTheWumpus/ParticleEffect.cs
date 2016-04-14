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
		Maximization,
		ManyMaximizations,
		None
	}

	class ParticleEffect {

		public Image particle { get; set; }
		public int LongTime { get; set; }
		public int TimeNewEffect { get; set; }
		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }
		public int EndMaximization { get; set; }
		public bool RandomPositions { get; set; }
		public int RandomX { get; set; }
		public int RandomY { get; set; }
		public bool Different { get; set; }
		public int frequency { get; set; }
		public bool OptimizeFPS { get; set; }
		public TypeAnimation effect { get; set; }

		private CompressionImage img;
		public List<Image> cadr { get; set; }
		private Stopwatch timer;
		public double StartPositionX { get; set; }
		public double StartPositionY { get; set; }
		public double EndPositionX { get; set; }
		public double EndPositionY { get; set; }

		private List<double> Sizes;
		private double Size = 1.0f;
		private double NowPositionX;
		private double NowPositionY;
		private double MoveSpeedX;
		private double MoveSpeedY;
		private List<double> ImagePositionsX;
		private List<double> ImagePositionsY;
		private int TimeNext = 0;
		private Random range = new Random();

		public ParticleEffect() {
			LongTime = 1000;
			effect = TypeAnimation.None;
			cadr = new List<Image>();
			ImagePositionsX = new List<double>();
			ImagePositionsY = new List<double>();
			Sizes = new List<double>();
			timer = new Stopwatch();
		}

		public void Start() {
			timer.Start();
			if (effect == TypeAnimation.Move || effect == TypeAnimation.QueueMove) {
				img = new CompressionImage(particle, ImageWidth, ImageHeight);
				NowPositionX = StartPositionX;
				NowPositionY = StartPositionY;
				MoveSpeedX = (EndPositionX - StartPositionX) * 1000 / LongTime;
				MoveSpeedY = (EndPositionY - StartPositionY) * 1000 / LongTime;
			}
			if (effect == TypeAnimation.Maximization || effect == TypeAnimation.ManyMaximizations) {
				NowPositionX = StartPositionX;
				NowPositionY = StartPositionY;
				MoveSpeedX = EndMaximization * 1000 / LongTime;
			}
		}

		public void Draw(Graphics g) {
			if (effect == TypeAnimation.Move) {
				img.Draw(g, (int)NowPositionX, (int)NowPositionY);
				//g.DrawImage(particle, new Rectangle((int)NowPositionX, (int)NowPositionY, ImageWidth, ImageHeight));
			}
			if (effect == TypeAnimation.QueueMove) {
				bool drawed = true;
				for (int i = ImagePositionsX.Count - 1; i >= 0; --i) {
					if (drawed && OptimizeFPS && i < ImagePositionsX.Count - 1 && Math.Abs(ImagePositionsX[i] - ImagePositionsX[i + 1]) < ImageWidth / 5 && Math.Abs(ImagePositionsY[i] - ImagePositionsY[i + 1]) < ImageHeight / 5) {
						drawed = false;
						continue;
					} else {
						drawed = true;
					}
					img.Draw(g, (int)ImagePositionsX[i], (int)ImagePositionsY[i]);
					//g.DrawImage(particle, new Rectangle((int)ImagePositionsX[i], (int)ImagePositionsY[i], ImageWidth, ImageHeight));
				}
			}
			if (effect == TypeAnimation.Maximization) {
				g.DrawImage(particle, new Rectangle((int)NowPositionX - (int)Size / 2, (int)NowPositionY - (int)Size / 2, (int)Size, (int)Size));
			}
			if (effect == TypeAnimation.ManyMaximizations) {
				for (int i = 0; i < Sizes.Count; ++i) {
					g.DrawImage(particle, new Rectangle((int)(ImagePositionsX[i] - Sizes[i] / 2), (int)(ImagePositionsY[i] - Sizes[i] / 2), (int)Sizes[i], (int)Sizes[i]));
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
					if (!RandomPositions) {
						NowPositionX = StartPositionX;
						NowPositionY = StartPositionY;
					} else {
						NowPositionX = range.Next((int)StartPositionX, (int)StartPositionX + RandomX);
						NowPositionY = range.Next((int)StartPositionY, (int)StartPositionY + RandomY);
					}
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
					if (!RandomPositions) {
						ImagePositionsX.Add(StartPositionX);
						ImagePositionsY.Add(StartPositionY);
					} else {
						ImagePositionsX.Add(range.Next((int)StartPositionX, (int)StartPositionX + RandomX));
						ImagePositionsY.Add(range.Next((int)StartPositionY, (int)StartPositionY + RandomY));
					}
				}
				if (maxX > EndPositionX || maxY > EndPositionY) {
					ImagePositionsX.Remove(maxX);
					ImagePositionsY.Remove(maxY);
				}
			}
			if (effect == TypeAnimation.Maximization) {
				Size += MoveSpeedX * Milliseconds / LongTime;
				if (Size > EndMaximization) {
					Size = 1.0f;
					if (RandomPositions) {
						NowPositionX = range.Next((int)StartPositionX, (int)StartPositionX + RandomX);
						NowPositionY = range.Next((int)StartPositionY, (int)StartPositionY + RandomY);
					}
				}
			}
			if (effect == TypeAnimation.ManyMaximizations) {
				TimeNext += (int)(Milliseconds);
				double Maximal = 0;
				double posX = -1;
				double posY = -1;
				for (int i = 0; i < Sizes.Count; ++i) {
					Sizes[i] += MoveSpeedX * Milliseconds / LongTime;
					if (Maximal < Sizes[i]) {
						Maximal = Sizes[i];
						posX = ImagePositionsX[i];
						posY = ImagePositionsY[i];
					}
				}
				if (Maximal > EndMaximization) {
					Sizes.Remove(Maximal);
					ImagePositionsX.Remove(posX);
					ImagePositionsY.Remove(posY);
				}
				if (TimeNext > TimeNewEffect && !Different) {
					if (!RandomPositions) {
						ImagePositionsX.Add(StartPositionX);
						ImagePositionsY.Add(StartPositionY);
						Sizes.Add(1);
					} else {
						ImagePositionsX.Add(range.Next((int)StartPositionX, (int)StartPositionX + RandomX));
						ImagePositionsY.Add(range.Next((int)StartPositionX, (int)StartPositionY + RandomY));
						Sizes.Add(1);
					}
				}
				if (Different && range.Next(0, frequency) == 0) {
					if (!RandomPositions) {
						ImagePositionsX.Add(StartPositionX);
						ImagePositionsY.Add(StartPositionY);
						Sizes.Add(1);
					} else {
						ImagePositionsX.Add(range.Next((int)StartPositionX, (int)StartPositionX + RandomX));
						ImagePositionsY.Add(range.Next((int)StartPositionX, (int)StartPositionY + RandomY));
						Sizes.Add(1);
					}
				}
			}
			timer.Restart();
		}
	}
}
