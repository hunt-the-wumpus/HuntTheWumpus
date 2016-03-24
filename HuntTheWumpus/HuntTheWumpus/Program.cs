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
		[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;//double.Parse(str) use format a.b
            Control control = new Control(1024, 768);
            long TimeDrawing = 0;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            
            while (control.view.Created())
            {
                Application.DoEvents();
                control.UpDate();

                TimeDrawing += timer.ElapsedMilliseconds;
                timer.Restart();
                if (TimeDrawing >= 1000 / 60)
                {
                    TimeDrawing -= 1000 / 60;
                    control.view.MainForm.DrawAll();
                }
            }
        }
    }
}
