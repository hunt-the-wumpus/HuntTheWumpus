﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HuntTheWumpus {
	public class CompressionImage {

		public Image CompressedImage { get; private set; }
		public int ScreenWidth { get; set; }
		public int ScreenHeight { get; set; }

		public CompressionImage(string file, int width, int height) {
			Image img = Image.FromFile(file);
			CompressedImage = ScaleImage(img, width, height);
			ScreenWidth = 100000;
			ScreenHeight = 100000;
		}

        public CompressionImage(string file, int width, int height, int ScreenWid, int ScreenHei)
        {
            Image img = Image.FromFile(file);
            CompressedImage = ScaleImage(img, width, height);
            ScreenWidth = ScreenWid;
            ScreenHeight = ScreenHei;
        }

		public CompressionImage(Image img, int width, int height) {
			CompressedImage = ScaleImage(img, width, height);
			ScreenWidth = 100000;
			ScreenHeight = 100000;
		}

		public void Draw(Graphics g, int x, int y) {
			if (x > ScreenWidth || y > ScreenHeight) {
				return;
			}
			if (x + CompressedImage.Width < 0 || y + CompressedImage.Height < 0) {
				return;
			}
            g.DrawImage(CompressedImage, Math.Max(0, x), Math.Max(0, y), new Rectangle(Math.Max(0, -x), Math.Max(0, -y), 
                Math.Min(CompressedImage.Width, ScreenWidth - x) - Math.Max(0, -x), 
                Math.Min(CompressedImage.Height, ScreenHeight - y) - Math.Max(0, -y)), GraphicsUnit.Pixel);
        }

		private Image ScaleImage(Image source, int width, int height) {
			Image dest = new Bitmap(width, height);
			using (Graphics gr = Graphics.FromImage(dest)) {
				gr.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0, 0)), 0, 0, width, height);  // Очищаем экран
				gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				gr.DrawImage(source, 0, 0, width, height);
				return dest;
			}
		}
	}
}
