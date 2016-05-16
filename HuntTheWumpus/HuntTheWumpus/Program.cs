using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace HuntTheWumpus
{

	class Program
    {
        /// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		public const int FPS = 60;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;//double.Parse(str) use format a.b
            Control control = new Control(1024, 745);
            long TimeDrawing = 1;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            
            while (control.view.Created())
            {
                timer.Start();
                Application.DoEvents();
                control.view.MainForm.DrawAll();
                control.UpDate(TimeDrawing);
                TimeDrawing = timer.ElapsedMilliseconds;
                timer.Reset();
                if (TimeDrawing < 1000 / FPS)
                {
                    Thread.Sleep((int)(1000 / FPS - TimeDrawing));
                }
            }
        }
    }
}
