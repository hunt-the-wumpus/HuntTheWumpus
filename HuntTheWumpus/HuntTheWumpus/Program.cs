﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuntTheWumpus
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;//double.Parse(str) use format a.b
            var MainForm = new Form1();
            MainForm.Show();
            MainForm.DrawAll();
            var ticktim = new System.Diagnostics.Stopwatch();
            ticktim.Start();
            while (MainForm.Created)
            {
                MainForm.DrawAll();
                Application.DoEvents();
                //Apdate(ticktim.ElapsedMilliseconds);
                ticktim.Restart();
            }
        }

        public static void GetScreen(System.Drawing.Graphics g)
        {
            g.Clear(System.Drawing.Color.Green);
            var fn = new System.Drawing.Font("Arial", 20);
            g.DrawString("Hello, world!", fn, System.Drawing.Brushes.Red, 20, 30);
        }
    }
}
