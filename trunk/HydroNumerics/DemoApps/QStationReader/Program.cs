using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using HydroNumerics.MikeSheTools.DFS;


namespace QStationReader
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      string TextFileName = "";
      string dfs0FileName = "";
      if (args.Length == 0)
      {
        OpenFileDialog OFD = new OpenFileDialog();
        OFD.Title = "Select a text file with discharge data";
        if (DialogResult.OK == OFD.ShowDialog())
          TextFileName = OFD.FileName;
        else
          return;

        SaveFileDialog SFD = new SaveFileDialog();
        SFD.Title = "Select a .dfs0 file or give a new name";
        SFD.Filter = "Known file types (*.dfs0)|*.dfs0";
        SFD.OverwritePrompt = false;
        if (DialogResult.OK == SFD.ShowDialog())
          dfs0FileName = SFD.FileName;
        else
          return;
      }
      else
      {
        TextFileName = args[0];
        dfs0FileName = args[1];
      }

      if (args.Length > 2 || !File.Exists(TextFileName))
      {
        if (DialogResult.Cancel ==
            MessageBox.Show("This program needs two file names as input. If the file names contain spaces the filename should be embraced by \"\". \n Are these file names correct:? \n" + TextFileName + "\n" + dfs0FileName, "Two many arguments!", MessageBoxButtons.OKCancel))
          return;
      }


      List<QStation> _stations = new List<QStation>();
      //Loop to read the Q-stations.
      using (StreamReader SR = new StreamReader(TextFileName, Encoding.Default))
      {
        string line;
        while (!SR.EndOfStream)
        {
          line = SR.ReadLine();
          if (line.Equals("*"))
          {
            QStation qs = new QStation();
            qs.ReadEntryFromText(SR);
            _stations.Add(qs);
          }
        }
      }

      DFS0 _data;
      //Append to existing file
      if (File.Exists(dfs0FileName))
      {
        _data = new DFS0(dfs0FileName);
      }
      else
      {
        //Create new .dfs0-file and list of q-stations
        using (StreamWriter SW = new StreamWriter(Path.Combine(Path.GetDirectoryName(dfs0FileName), "DetailedTimeSeriesImport.txt"), false, Encoding.Default))
        {
          int k = 1;
          _data = new DFS0(dfs0FileName, _stations.Count);

          for (int i = 0; i < _stations.Count; i++)
          {
            //Build the TSITEMS

            _data.Items[i].ValueType = DHI.Generic.MikeZero.DFS.DataValueType.Instantaneous;
            _data.Items[i].EumItem = DHI.Generic.MikeZero.eumItem.eumIDischarge;

            //Provide an ITEM name following the convention by Anker
            if (_stations[i].DmuMaalerNr != "")
              _data.Items[i].Name = _stations[i].DmuMaalerNr;
            else
              _data.Items[i].Name = _stations[i].DmuStationsNr.ToString();

            SW.WriteLine(_data.Items[i].Name + "\t" + _stations[i].UTMX + "\t" + _stations[i].UTMY + "\t" + k);
            k++;
          }
        }
      }

      DateTime LastTimeStep;
      if (_data.TimeSteps.Count==0) //We have a new file
        LastTimeStep = DateTime.MinValue;
      else      // 12 hours have been added in dfs0!
        LastTimeStep = _data.TimeSteps.Last().Subtract(new TimeSpan(12, 0, 0));
      int TSCount = _data.NumberOfTimeSteps;

      DateTime CurrentLastTimeStep = LastTimeStep;


      //Loop the stations from the text-file
      foreach (var qs in _stations.Where(var=>var.Discharge.Items.Count>0))
      {
        qs.Discharge.Sort();
        //See if the station has newer data
        if (qs.Discharge.EndTime > LastTimeStep)
        {
          Item I = _data.Items.FirstOrDefault(var => var.Name == qs.DmuMaalerNr);
          if (I == null)
            I = _data.Items.FirstOrDefault(var => var.Name == qs.DmuStationsNr.ToString());
          if (I == null)
            Console.WriteLine("DMU MÅLER Nr: " + qs.DmuMaalerNr + " eller DMU sted nr: " + qs.DmuStationsNr + " blev ikke fundet i dfs0.filen");
          else
          {
            foreach (var TSE in qs.Discharge.ItemsInPeriod(LastTimeStep, qs.Discharge.EndTime))
            {
              _data.SetData(TSE.Time.AddHours(12), I.ItemNumber, TSE.Value);
            }
          }
        }
      }
      _data.Dispose();
    }
  }
}
