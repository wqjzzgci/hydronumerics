using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;
using DHI.Generic.MikeZero;

namespace Txt2dfs0
{
  class Program
  {
    static void Main(string[] args)
    {
      //Gets the text file name from the first argument
      string TxtFileName = args[0];
      //Gets the dfs0 file name from the second argument
      string dfs0FileName = args[1];

      //Creates a new dfs0-file with one item
      DFS0 dfsfile = new DFS0(dfs0FileName, 1);

      //Sets the eumitem and unit.
      dfsfile.Items[0].EumItem = eumItem.eumIConcentration;
      dfsfile.Items[0].EumUnit = eumUnit.eumUmilliGramPerL;
      dfsfile.Items[0].Name = "Concentration";
      dfsfile.Items[0].ValueType = DHI.Generic.MikeZero.DFS.DataValueType.Instantaneous; 

      //Opens the text file
      using (StreamReader sr = new StreamReader(TxtFileName))
      {
        //Count the timesteps. From zero.
        int TimeStepCounter = 0;

        //Loop until at the end of file
        while (!sr.EndOfStream)
        {
          //Read the line
          string line = sr.ReadLine();
          //Split on ";"
          string[] splitLine = line.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          //Convert first text to date
          DateTime date = DateTime.Parse(splitLine[0]);
          //Convert second text to number
          double value = double.Parse(splitLine[1]);

          //Now set time of time step
          dfsfile.SetTime(TimeStepCounter, date);
          //Now set value
          dfsfile.SetData(TimeStepCounter, 1, value);

          //Increment time step counter
          TimeStepCounter++;
        }
      }
      dfsfile.Dispose();
    }
  }
}
