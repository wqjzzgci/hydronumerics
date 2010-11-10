using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;

namespace MultiplyDfs0AndAscii
{
  /// <summary>
  /// A small console application that takes an text file and dfs0-file as input and multiplies the factors in text file
  /// to all time steps of the specified items. 
  /// Format of text-file:
  /// 
  /// ItemName  ItemNumber  Factor
  /// 1245  1 2.3
  /// 
  /// Columns should be separated by tabs.
  /// The program first tries to find the Item by name. If that does not succeed it uses the ItemNumber.
  /// 
  /// The program references HydroNumerics.MikeSheTools.Dfs and requires a MikeZero installation
  /// </summary>
  class Program
  {
    static void Main(string[] args)
    {
      //Extract the filenames from the arguments
      string dfs0FileName = args.First(var => Path.GetExtension(var).ToLower() == ".dfs0");
      string AsciiFileName = args.First(var => Path.GetExtension(var).ToLower() != ".dfs0");

      //Create a DFS0-object
      using (DFS0 _dfs0 = new DFS0(dfs0FileName)) //"Using" will make sure the object is correctly disposed 
      {
        //Creates a streamreader object
        using (StreamReader sr = new StreamReader(AsciiFileName))
        {
          //Not used but need to advance to next line
          string headline = sr.ReadLine();

          //Loop until end of stream
          while (!sr.EndOfStream)
          {
            //Reads next line
            string line = sr.ReadLine();

            //Splits on tabs into an array of strings
            string[] lineArray = line.Split('\t');

            //Gets the ItemName from the first column
            string ItemName =lineArray[0];

            //Gets the Item number as the second column
            int ItemNumber = int.Parse(lineArray[1]);

            //Gets the Multiplication factor as the third column
            double MultiplyFactor = double.Parse(lineArray[2]);

            //Try to find the item based on the item name
            Item I = _dfs0.Items.FirstOrDefault(var => var.Name == ItemName);

            //I will be zero if it was not found
            if (I != null)
              ItemNumber = I.ItemNumber; //Now sets the item number to the correct number

            //Loop all the timesteps in the dfs0
            for (int i = 0; i < _dfs0.NumberOfTimeSteps; i++)
            {
              //Get the original value
              double val = _dfs0.GetData(i, ItemNumber);
              //Calculate new value
              double newval = val * MultiplyFactor;
              //Set the new value
              _dfs0.SetData(i, ItemNumber, newval);
            }
          }
        }
      }
    }
  }
}
