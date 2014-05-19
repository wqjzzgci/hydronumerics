using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Data;

using HydroNumerics.Core;
using HydroNumerics.Geometry.Shapes;


namespace HydroNumerics.Nitrate.Model
{
  public class StreamSink:BaseModel, ISink
  {

    private SortedList<string, StreamClassification> StreamClasses = new SortedList<string, StreamClassification>();

    private Dictionary<int, Tuple<double, double>> ReductionFactors = new Dictionary<int, Tuple<double, double>>();

    private DataTable Data;


    public StreamSink()
    {

      Data = new DataTable();
      Data.Columns.Add("ID15", typeof(int));
      Data.Columns.Add("Lenght1", typeof(double));
      Data.Columns.Add("Lenght2", typeof(double));
      Data.Columns.Add("Lenght3", typeof(double));
      Data.Columns.Add("Red_win_1", typeof(double));
      Data.Columns.Add("Red_win_2", typeof(double));
      Data.Columns.Add("Red_win_3", typeof(double));
      Data.Columns.Add("Red_win_total", typeof(double));
      Data.Columns.Add("Red_sum_1", typeof(double));
      Data.Columns.Add("Red_sum_2", typeof(double));
      Data.Columns.Add("Red_sum_3", typeof(double));
      Data.Columns.Add("Red_sum_total", typeof(double));

      Data.PrimaryKey = new DataColumn[]{ Data.Columns[0]};

      //Default streams for testing
      //StreamClasses.Add(1.23, new StreamClassification() { 
      //  Name = "Stream 0 - 2.5m", 
      //  Width = 1.23,
      //  StreamDepthWinter = 0.21, 
      //  StreamDepthSummer = 0.17, 
      //  StreamVelocityWinter = 0.22,
      //  StreamVelocitySummer = 0.18
      //});

      //StreamClasses.Add(5.15, new StreamClassification()
      //{
      //  Name = "Stream 2.5 - 12m",
      //  Width = 5.15,
      //  StreamDepthWinter = 0.54,
      //  StreamDepthSummer = 0.44,
      //  StreamVelocityWinter = 0.37,
      //  StreamVelocitySummer = 0.30
      //});

      //StreamClasses.Add(16.61, new StreamClassification()
      //{
      //  Name = "Stream 12+ m",
      //  Width = 16.61,
      //  StreamDepthWinter = 1.2,
      //  StreamDepthSummer = 1.1,
      //  StreamVelocityWinter = 0.48,
      //  StreamVelocitySummer = 0.35
      //});
    }



    public void LoadDBFFile(string FileName, string IDColumn, string NameColumn, string LengthColumn, string LRColumn)
    {
      NewMessage("Reading: " + FileName);

      using (DBFReader dbr = new DBFReader(FileName))
      {
        while (!dbr.EndOfData)
        {
          var data = dbr.ReadNext();
          int catchid = int.Parse(data[IDColumn].ToString());
          string type = data[NameColumn].ToString();
          double length = double.Parse(data[LengthColumn].ToString());
          double lengthReductionFactor = double.Parse(data[LRColumn].ToString());

          StreamClassification streamclass;
          if (StreamClasses.TryGetValue(type, out streamclass))
          {

            Tuple<double, double> previous;
            if (!ReductionFactors.TryGetValue(catchid, out previous))
            {
              previous = new Tuple<double, double>(0, 0);
              ReductionFactors.Add(catchid, previous);
            }

            DataRow datarow = Data.Rows.Find(new object[] { catchid });
            if (datarow == null)
            {
              datarow = Data.NewRow();
              datarow[0] = catchid;
              Data.Rows.Add(datarow);
            }


            double summer = 0;
            double winter = 0;

            if (length > 0)
            {
              summer = 0.01 * MultiplicationFactor * Math.Pow(streamclass.StreamDepthSummer / (length * lengthReductionFactor / (streamclass.StreamVelocitySummer * 365.0 * 86400.0)), Exponent);
              winter = 0.01 * MultiplicationFactor * Math.Pow(streamclass.StreamDepthWinter / (length * lengthReductionFactor / (streamclass.StreamVelocityWinter * 365.0 * 86400.0)), Exponent);
            }

            int i = StreamClasses.IndexOfKey(streamclass.StreamType);

            datarow[i + 1] = length;
            datarow[i + 4] = winter;
            datarow[i + 8] = summer;
            var newfact = new Tuple<double, double>(1.0 - (1.0 - previous.Item1) * (1 - summer), 1.0 - (1.0 - previous.Item2) * (1 - winter));
            datarow[7] = newfact.Item2;
            datarow[11] = newfact.Item1;
            ReductionFactors[catchid] = newfact;
          }
        }
      }
    }


    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {

        MultiplicationFactor = Configuration.SafeParseDouble("MultiplicationFactor") ?? _MultiplicationFactor;
        Exponent = Configuration.SafeParseDouble( "Exponent") ?? _Exponent;
        ReachLengthReductionFactor = Configuration.SafeParseDouble("ReachLenghtReductionFactor") ?? _ReachLengthReductionFactor;
        FirstSummerMonth = Configuration.SafeParseInt("FirstSummerMonth") ?? _FirstSummerMonth;
        LastSummerMonth = Configuration.SafeParseInt("LastSummerMonth") ?? _LastSummerMonth;

        foreach (var v in Configuration.Element("StreamClasses").Elements("StreamClass"))
        {
          StreamClassification sc = new StreamClassification();
          sc.StreamType = v.SafeParseString("Name");
          sc.StreamDepthSummer = v.SafeParseDouble("DepthSummer") ?? 0;
          sc.StreamDepthWinter = v.SafeParseDouble("DepthWinter") ?? 0;
          sc.StreamVelocitySummer = v.SafeParseDouble("VelocitySummer") ?? 0;
          sc.StreamVelocityWinter = v.SafeParseDouble( "VelocityWinter") ?? 0;
          StreamClasses.Add(sc.StreamType, sc);
        }
        foreach (var v in Configuration.Element("DBFFiles").Elements("DBFFile"))
        {
          var dbf = new SafeFile();
          dbf.FileName = v.SafeParseString("FileName");
          dbf.ColumnNames.Add(v.SafeParseString("IDColumn"));
          dbf.ColumnNames.Add(v.SafeParseString("NameColumn"));
          dbf.ColumnNames.Add(v.SafeParseString("LengthColumn"));
          dbf.ColumnNames.Add(v.SafeParseString("LRColumn"));
          this.DBFFiles.Add(dbf);
        }
      }

    }

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      foreach (var c in Catchments)
        ReductionFactors.Add(c.ID, new Tuple<double, double>(0, 0));

      foreach (var f in DBFFiles)
        LoadDBFFile(f.FileName, f.ColumnNames[0], f.ColumnNames[1], f.ColumnNames[2], f.ColumnNames[3]);

      NewMessage("Initialized");
    }


    public double GetReduction(Catchment c, double CurrentInflowRate, DateTime CurrentTime)
    {
      double red = 0;
      CurrentInflowRate /= DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0;
      if (CurrentTime.Month >= FirstSummerMonth & CurrentTime.Month <= LastSummerMonth)
        red= ReductionFactors[c.ID].Item1 * CurrentInflowRate; //Summer
      else
        red= ReductionFactors[c.ID].Item2 * CurrentInflowRate; //Winter

      return red *MultiplicationPar + AdditionPar;
    }

    public override void  DebugPrint(string Directory, Dictionary<int,Catchment> Catchments)
    {
      using (ShapeWriter sw = new ShapeWriter(System.IO.Path.Combine(Directory, Name + "_debug")))
      {
        for (int i = 0; i < Data.Rows.Count;i++ )
        {
          if (Catchments.ContainsKey((int)Data.Rows[i][0]))
          {
            Geometry.GeoRefData gd = new Geometry.GeoRefData() { Geometry = Catchments[(int)Data.Rows[i][0]].Geometry, Data = Data.Rows[i] };
            sw.Write(gd);
          }
        }
      }
    }



    #region Properties

    private List<SafeFile> _DBFFiles = new List<SafeFile>();
    public List<SafeFile> DBFFiles
    {
      get { return _DBFFiles; }
      set
      {
        if (_DBFFiles != value)
        {
          _DBFFiles = value;
          NotifyPropertyChanged("DBFFiles");
        }
      }
    }
    

    private int _FirstSummerMonth =4;
    public int FirstSummerMonth
    {
      get { return _FirstSummerMonth; }
      set
      {
        if (_FirstSummerMonth != value)
        {
          _FirstSummerMonth = value;
          NotifyPropertyChanged("FirstSummerMonth");
        }
      }
    }

    private int _LastSummerMonth =10;
    public int LastSummerMonth
    {
      get { return _LastSummerMonth; }
      set
      {
        if (_LastSummerMonth != value)
        {
          _LastSummerMonth = value;
          NotifyPropertyChanged("LastSummerMonth");
        }
      }
    }

    private double _MultiplicationFactor = 74.61;
    public double MultiplicationFactor
    {
      get { return _MultiplicationFactor; }
      set
      {
        if (_MultiplicationFactor != value)
        {
          _MultiplicationFactor = value;
          NotifyPropertyChanged("MultiplicationFactor");
        }
      }
    }
    

    private double _Exponent = -0.344;
    public double Exponent
    {
      get { return _Exponent; }
      set
      {
        if (_Exponent != value)
        {
          _Exponent = value;
          NotifyPropertyChanged("Exponent");
        }
      }
    }

    private double _ReachLengthReductionFactor = 0.25;
    public double ReachLengthReductionFactor
    {
      get { return _ReachLengthReductionFactor; }
      set
      {
        if (_ReachLengthReductionFactor != value)
        {
          _ReachLengthReductionFactor = value;
          NotifyPropertyChanged("ReachLengthReductionFactor");
        }
      }
    }
    
    
    

    #endregion

  }
}
