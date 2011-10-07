using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;


namespace HydroNumerics.MikeSheTools.LayerStatistics
{
  /// <summary>
  /// This class reads the input file and writes the output files for layer statistics. It uses a HeadObservation object
  /// and reads into the Wells and writes from the Working list.
  /// </summary>
  public class InputOutput
  {

    private string _baseOutPutFileName;
    private int _numberOfLayers;

    public InputOutput(int NumberOfLayers)
    {
      _numberOfLayers = NumberOfLayers;
    }

    public string BaseOutPutFileName
    {
      get { return _baseOutPutFileName; }
      set { _baseOutPutFileName = value; }
    }

    /// <summary>
    /// Reads in head observations from txt file with this format.
    /// "WellID X Y Z Head  Date  Layer". Separated with tabs. Layer is optional
    /// </summary>
    /// <param name="LSFileName"></param>
    public Dictionary<string, MikeSheWell> ReadFromLSText(string LSFileName)
    {
      Dictionary<string, MikeSheWell> Wells = new Dictionary<string, MikeSheWell>();
      //Sets the output file name for subsequent writing
      string path = Path.GetDirectoryName(LSFileName);
      string FileName = Path.GetFileNameWithoutExtension(LSFileName);
      _baseOutPutFileName = Path.Combine(path, FileName);

      //Now read the input
      using (StreamReader SR = new StreamReader(LSFileName))
      {
        //Reads the HeadLine
        string line = SR.ReadLine();
        string[] s;
        MikeSheWell OW;

        while ((line = SR.ReadLine()) != null)
        {
          s = line.Split('\t');

          //Check that s has correct lengt and does not consist of empty entries
          if (s.Length > 5 & s.Aggregate<string>((a,b)=>a+b)!="")
          {
            try
            {
              LsIntake I = null;
              //If the well has not already been read in create a new one
              if (!Wells.TryGetValue(s[0], out OW))
              {
                OW = new MikeSheWell(s[0]);
                I = new LsIntake(OW, 1);
                OW.AddIntake(I);
                Wells.Add(OW.ID, OW);
                OW.X = double.Parse(s[1]);
                OW.Y = double.Parse(s[2]);

                //Layer is provided directly. Calculate Z
                if (s.Length >= 7 && s[6] != "")
                {
                  OW.Layer = _numberOfLayers - int.Parse(s[6]);
                }
                //Use the Z-coordinate
                else
                {
                  OW.Depth = double.Parse(s[3]);
                  OW.Layer = -3;
                }
              }

              if (I == null)
                I = OW.Intakes.First() as LsIntake;

              //Now add the observation
              I.Observations.Add(new Observation(DateTime.Parse(s[5]), double.Parse(s[4]),OW));
            }
            catch (FormatException e)
            {
              MessageBox.Show("Error reading this line:\n\n" + line +"\n\nFrom file: "+ LSFileName + "\n\nLine skipped!", "Format error!");
            }
          }
        }
      } //End of streamreader
      return Wells;
    }

    /// <summary>
    /// Skriver 3 filer med beregnede værdier for hvert lag
    /// </summary>
    /// <param name="ME"></param>
    /// <param name="RMSE"></param>
    /// <param name="ObsUsed"></param>
    /// <param name="ObsTotal"></param>
    public void WriteLayers(double[] ME, double[] RMSE, int[] ObsUsed, int[] ObsTotal)
    {
      //Writes a file with ME
      using (StreamWriter sw = new StreamWriter(_baseOutPutFileName + "_ME.txt"))
      {
        //Write backwards because of MSHE Layering
        for (int i = ME.Length -1 ; i >= 0;i--) 
        {
          sw.WriteLine(ME[i].ToString());
        }
      }

      //Writes a file with RMSE
      using (StreamWriter sw = new StreamWriter(_baseOutPutFileName + "_RMSE.txt"))
      {
        //Write backwards because of MSHE Layering
        for (int i = RMSE.Length - 1; i >= 0; i--)
        {
          sw.WriteLine(RMSE[i].ToString());
        }
      }

      //Writes a file with a summary for each layer
      using (StreamWriter sw = new StreamWriter(_baseOutPutFileName + "_layers.txt"))
      {
        //Writes the headline
        sw.WriteLine("Layer\tRMSE\tME\t#obs used\tobs total");
        //Write backwards because of MSHE Layering
        for (int i = ME.Length - 1; i >= 0; i--)
        {
          StringBuilder str = new StringBuilder();
          str.Append((ME.Length - i) + "\t"); //MSHE -layering
          str.Append(RMSE[i] + "\t");
          str.Append(ME[i] + "\t");
          str.Append(ObsUsed[i] + "\t");
          str.Append(ObsTotal[i] + "\t");
          sw.WriteLine(str.ToString());
        }
      }


    }


  }
}
