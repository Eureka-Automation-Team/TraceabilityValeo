using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Trace.OpcHandlerMachine06
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool instanceCountOne = false;
            using (Mutex mtex = new Mutex(true, "Station 5 UPPER", out instanceCountOne))
            {
                if (instanceCountOne)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MonitoringForm());
                }
                else
                {
                    MessageBox.Show("Application Station 5 upper is already running.");
                }
            }
        }
    }
}
