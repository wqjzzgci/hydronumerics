using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HydroNumerics.Time.Tools.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //Testing TimeseriesGrid
            //Application.Run(new TimestampGridTestForm());

            //testing TimespanSeriesGrid
            Application.Run(new TimespanSeriesGridFrom());
        }
    }
}
