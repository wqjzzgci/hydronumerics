using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace HydroNumerics.Tough2.ViewModel
{

  public delegate void NewTimeStepHandler(object sender, TimeStepInfo TimeStep);

  public class OutputFileParser
  {
    BlockPointer _status = BlockPointer.Prelude;
    public List<TimeStepInfo> TimeSteps = new List<TimeStepInfo>();
    public List<TimesOutput> Vectors {get;private set;}

    public event NewTimeStepHandler NewTimeStep;

    protected virtual void OnNewTimeStep(TimeStepInfo TimeStep)
    {
      NewTimeStep(this, TimeStep);
    }

    private Model _model;

    public OutputFileParser(Model M)
    {
      _model = M;
      Vectors = new List<TimesOutput>();
    }


    public void ReadOutputFile(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        while (!sr.EndOfStream)
          ReadOutputLine(sr.ReadLine());
      }
    }

    public void Clear()
    {
      this.TimeSteps.Clear();
      this.Vectors.Clear();
      foreach (var el in _model.Elements)
        el.PrintData.Clear();
    }

    int _outputLines;
    TimesOutput CurrentResults;
    public void ReadOutputLine(string Line)
    {
      switch (_status)
      {
        case BlockPointer.Prelude:
          if (Line.StartsWith(" ********** VOLUME- AND MASS-BALANCES"))
            _status = BlockPointer.MassBalance;
          break;
        case BlockPointer.MassBalance:
          _status = BlockPointer.Iterating;
          break;
        case BlockPointer.Iterating:

          if (Line.Length > 25 && Line.Substring(17, 4).Equals("ST ="))
          {
            TimeStepInfo tsi = new TimeStepInfo();
            tsi.TimeStep =TimeSpan.FromSeconds(double.Parse(Line.Substring(40, 12)));
            tsi.TotalTime = TimeSpan.FromSeconds(Double.Parse(Line.Substring(22, 12)));
            tsi.TimeStepNumber = TimeSteps.Count()+1;
            tsi.MaxResidualElement = Line.Substring(1, 5);
            tsi.NumberOfIterations = int.Parse(Line.Substring(13, 2));
            TimeSteps.Add(tsi);


            if (NewTimeStep!=null)
              NewTimeStep(this, tsi);
          }
          //Switch to output reading
          else if (Line.StartsWith("          OUTPUT DATA AFTER"))
          {
            _status = BlockPointer.FirstMatrixOutput;
            _outputLines = 0;
          }
          else if (Line.StartsWith(" ELEM.  INDEX"))
          {
            _status = BlockPointer.SecondMatrixOutput;
            CurrentResults.Pointer = BlockPointer.SecondMatrixOutput;
            CurrentResults.ReadLine(Line);
          }
          else if (Line.StartsWith(" ********** VOLUME- AND MASS-BALANCES"))
            _status = BlockPointer.MassBalance;
          break;
        case BlockPointer.FirstMatrixOutput:
          {
            if (Line.Trim() != "") //Skip empty lines
            {
              if (_outputLines == 2)
              {
                CurrentResults = new TimesOutput(TimeSpan.FromSeconds(double.Parse(Line.Substring(1, 12))), _model.Elements);
                CurrentResults.Pointer = BlockPointer.FirstMatrixOutput;
                Vectors.Add(CurrentResults);
              }
              if (_outputLines > 6)
              {
                if (!CurrentResults.ReadLine(Line))
                  _status = BlockPointer.Iterating;
              }
              _outputLines++;
            }
          }
          break;
        case BlockPointer.SecondMatrixOutput:
          {
            if (!CurrentResults.ReadLine(Line))
              _status = BlockPointer.Iterating;
            break;

          }
        default:
          break;
      }
    }
  }
}
