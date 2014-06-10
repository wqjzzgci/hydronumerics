using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; //Due to OpenFileDialog

namespace HydroNumerics.Nitrate.Model
{
  class Program
  {
    [STAThread]//Due to OpenFileDialog
    static void Main(string[] args)
    {
      string FileName="";
      if (args.Count() == 0)
      {
        //Creates an open FileDialog
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Known file types (*.xml)|*.xml"; //Only open .xml-files
        ofd.Multiselect = false;

        //Now show the dialog and continue if the user presses ok
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          FileName = ofd.FileName;
        }
      }
      else
        FileName = args[0].ToString();

      if (!string.IsNullOrEmpty(FileName))
      {
        MainViewModel m = new MainViewModel();
        m.ReadConfiguration(FileName);
        m.Initialize();
        m.Run();
        m.Print();
        m.DebugPrint();
      }
    }
  }
}
