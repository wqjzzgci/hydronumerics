using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HydroNumerics.Time.TimeSeriesEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          if (args.Length!=0)
          
          {
            //MessageBox.Show(args[0]);
            Application.Run(new TimeSeriesEditor(args[0]));
          }
          else
            Application.Run(new TimeSeriesEditor());
        }
    }
}