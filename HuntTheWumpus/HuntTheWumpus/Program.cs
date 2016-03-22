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
            Control control = new Control(640, 480);
            Thread t = new Thread(new ThreadStart(control.view.MainForm.Loop));
            t.Start();
            while (control.view.Created())
            {
                Application.DoEvents();
                control.UpDate();
            }
            t.Abort();
        }
    }
}
