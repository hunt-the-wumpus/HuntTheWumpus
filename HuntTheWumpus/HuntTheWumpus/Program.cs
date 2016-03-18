using System;
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
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
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
    }
}
