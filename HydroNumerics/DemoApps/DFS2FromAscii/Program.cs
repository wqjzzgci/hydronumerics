using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; //Due to OpenFileDialog

using HydroNumerics.MikeSheTools.DFS; //Due to dfs-files
using HydroNumerics.Geometry.Shapes; //Due to AsciiReader

namespace DFS2FromAscii
{
  class Program
  {
    /// <summary>
    /// A short program made for Xin He, to read a lot of ascii files with radar data of precipitation and put them into dfs2-files.
    /// FileNames must have this format: YYYY.MM.DD_name001.asc for instance: 26.01.21_radarData001.asc. First part is date and the
    /// last three digits is the file. 
    /// </summary>
    /// <param name="args"></param>
    [STAThread]//Due to OpenFileDialog
    static void Main(string[] args)
    {
      //Creates an open FileDialog
      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Filter = "Known file types (*.asc)|*.asc"; //Only open .asc-files
      ofd.Multiselect = false;

      //Now show the dialog and continue if the user presses ok
      if (ofd.ShowDialog() == DialogResult.OK)
      {
        //Prepare a dictiondary to hold the DFS2-Files
        Dictionary<int, DFS2> _files = new Dictionary<int, DFS2>();

        //Get the directory of the chosen file
        string dir = Path.GetDirectoryName(ofd.FileName);
        
        //Get all the file names in the directory sorted alphabetically
        var AllAscFiles = Directory.GetFiles(dir,"*"+ Path.GetExtension(ofd.FileName), SearchOption.TopDirectoryOnly).OrderBy(var => var.ToString());

        //Loop all the files
        foreach (var file in AllAscFiles)
        {
          //Create an asciireader
          AsciiReader asc = new AsciiReader(file);
          DFS2 dfs;

          //Get the filename
          string fileName =Path.GetFileNameWithoutExtension(file);
          
          //The filekey is the last three digits in the filename
          int FileKey = int.Parse(fileName.Substring(fileName.Length-3,3));

          //Find the file in the dictionnary of files
          if (!_files.TryGetValue(FileKey, out dfs))
          {
            //The file was not there
            //Create a new DFS2
            dfs = new DFS2( Path.Combine(Path.GetDirectoryName(file), fileName.Substring(11, fileName.Length - 11) + ".dfs2"), 1);
            //Set grid and geo info
            dfs.NumberOfColumns = asc.NumberOfColumns;
            dfs.NumberOfRows = asc.NumberOfRows;
            dfs.XOrigin = asc.XOrigin;
            dfs.YOrigin = asc.YOrigin;
            dfs.GridSize = asc.GridSize;
            //set time info
            dfs.TimeOfFirstTimestep = DateTime.Parse(fileName.Substring(0, 10));
            dfs.TimeStep = TimeSpan.FromDays(1);
            //Set item info
            dfs.FirstItem.EumItem = DHI.Generic.MikeZero.eumItem.eumIPrecipitationRate;
            dfs.FirstItem.Name = "Radar precipitation";
            dfs.FirstItem.EumUnit = DHI.Generic.MikeZero.eumUnit.eumUmillimeterPerDay; 
            
            //Add to dictionary
            _files.Add(FileKey, dfs);
          }
          //Set the data of the next timestep
          dfs.SetData(dfs.NumberOfTimeSteps, 1,asc.Data);
        }
        //All the files have been read
        //Dispose the dfs-files
        foreach (var dfs in _files.Values)
          dfs.Dispose();
      }
    }
  }
}
