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
      var par = args.FirstOrDefault(a => a.StartsWith("-"));


      string FileName="";
      if (args.Count(a => !a.StartsWith("-")) == 0)
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
        FileName = args.First(a => !a.StartsWith("-"));

      if (!string.IsNullOrEmpty(FileName))
      {
        if (par != null)
        {
          ExtraPrinter ep = new ExtraPrinter();

          ep.FromConfigFile(FileName);
        
        }
        else
        {
          MainModel m = new MainModel();
          m.ReadConfiguration(FileName);
          m.Initialize();
          m.Run();
          m.Print();
          m.DebugPrint();
        }
      }
    }
  }
}
