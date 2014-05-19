using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Linq;
using NPOI;
using NPOI.HSSF.UserModel;

namespace HydroNumerics.Nitrate.Model
{
  public static class Extensions
  {

    public static void ToExcelTemplate(this DataTable data, string TemplateFilename, string OutputFolder)
    {
      Dictionary<int, List<DataRow>> tempdata = new Dictionary<int, List<DataRow>>();
      Dictionary<int, List<DataRow>> datawithnitrate = new Dictionary<int, List<DataRow>>();

      Directory.CreateDirectory(OutputFolder);

      for (int i = 0; i < data.Rows.Count; i++)
      {
        List<DataRow> currentdata;
        int ID15 = (int)data.Rows[i][0];
        if (!tempdata.TryGetValue(ID15, out currentdata))
        {
          currentdata = new List<DataRow>();
          tempdata.Add(ID15, currentdata);
        }
        currentdata.Add(data.Rows[i]);
        if (!data.Rows[i].IsNull("ObservedNitrate") & !data.Rows[i].IsNull("GroundWater"))
          if ((double)data.Rows[i]["GroundWater"] > 1e-5 & (double)data.Rows[i]["ObservedNitrate"] > 1e-5)
            if (!datawithnitrate.ContainsKey(ID15))
              datawithnitrate.Add(ID15, currentdata);
      }


      using (FileStream fs = new FileStream(TemplateFilename, FileMode.Open, FileAccess.Read))
      {
        HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true); // Getting the worksheet by its name... 
        var sheet = templateWorkbook.GetSheet("Data");
        foreach (var v in datawithnitrate)
        {
          for (int i = 0; i < v.Value.Count;i++ )
          {
            // Getting the row... 0 is the first row. 
            var dataRow = sheet.GetRow(i+1);
            dataRow.GetCell(0, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(v.Key);
            dataRow.GetCell(1, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue((DateTime)v.Value[i][1]);
            for (int j = 2; j < v.Value[i].Table.Columns.Count; j++)
              if (!v.Value[i].IsNull(j) & v.Value[i].Table.Columns[j].DataType== typeof(double))
                dataRow.GetCell(j, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue((double)v.Value[i][j]);
              else
                dataRow.GetCell(j, NPOI.SS.UserModel.MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(0);
          }
          // Forcing formula recalculation... 
          for (int i = 0; i < templateWorkbook.NumberOfSheets - 1; i++)
            templateWorkbook.GetSheetAt(i).ForceFormulaRecalculation = true;
          MemoryStream ms = new MemoryStream();
          // Writing the workbook content to the FileStream... 
          templateWorkbook.Write(ms);
          // Sending the server processed data back to the user computer...
          File.WriteAllBytes(Path.Combine(OutputFolder, v.Key + ".xls"), ms.GetBuffer());
        }
      }
    }

    public static SortedList<int, Time2.FixedTimeStepSeries> ExtractTimeSeries(this DataTable data, string parameterName)
    {
      SortedList<int, Time2.FixedTimeStepSeries> ToReturn = new SortedList<int, Time2.FixedTimeStepSeries>();
      for (int i = 0; i < data.Rows.Count; i++)
      {
        Time2.FixedTimeStepSeries par;
        int id15 = (int)data.Rows[i][0];

        if (!ToReturn.TryGetValue(id15, out par))
        {
          par = new Time2.FixedTimeStepSeries();
          par.TimeStepSize = Time2.TimeStepUnit.Month;
          par.StartTime = (DateTime)data.Rows[0][1];
          ToReturn.Add(id15, par);
        }
        if (data.Rows[i].IsNull(parameterName))
          par.Add(par.DeleteValue);
        else
          par.Add((double)data.Rows[i][parameterName]);
      }
      return ToReturn;
    }

    /// <summary>
    /// Writes a .csv-file with data for a particular ID15 catchment
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ID15"></param>
    /// <param name="filename"></param>
    public static void ToCSV(this DataTable data, string parametername, string filename)
    {

      if (!data.Columns.Contains(parametername))
        return;

      List<DateTime> dates = new List<DateTime>();

      int firstid = (int)data.Rows[0][0];
      //Resort the data
      SortedList<int, List<double>> pivot = new SortedList<int, List<double>>();
      for (int i = 0; i < data.Rows.Count; i++)
      {
        List<double> par;
        int id15 = (int)data.Rows[i][0];
        if (id15 == firstid)
          dates.Add((DateTime)data.Rows[i][1]);

        if (!pivot.TryGetValue(id15, out par))
        {
          par = new List<double>();
          pivot.Add(id15, par);
        }
        if (data.Rows[i].IsNull(parametername))
          par.Add(0);
        else
          par.Add((double)data.Rows[i][parametername]);
      }

      //Now write
      using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default))
      {
        //Headline
        sw.Write("ID15");
        foreach (var date in dates)
          sw.Write("," + date.ToString());
        sw.Write("\n");

        //Actual data
        foreach (int key in pivot.Keys)
        {
          sw.Write(key);
          for (int i = 0; i < pivot.Values.First().Count; i++)
          {
            sw.Write("," + pivot[key][i]);
          }
          sw.Write("\n");
        }
      }
    }


    /// <summary>
    /// Writes a .csv-file with data for a particular ID15 catchment
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ID15"></param>
    /// <param name="filename"></param>
    public static void ToCSV(this DataTable data, int ID15, string filename)
    {
      DataTable dt = data.Copy();

      for(int i =dt.Rows.Count-1;i>=0;i--)
        if((int)dt.Rows[i][0]!=ID15)
          dt.Rows.RemoveAt(i);

      dt.ToCSV(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)+ ID15+".csv"));
    }



    /// <summary>
    /// Writes the entire data table to a csv-file
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filename"></param>
    public static void ToCSV(this DataTable data, string filename)
    {
      using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default))
      {
        for (int i = 0; i < data.Columns.Count; i++)
        {
          if (i == 0)
            sw.Write(data.Columns[i].ColumnName);
          else
            sw.Write("," + data.Columns[i].ColumnName);
        }
        sw.Write("\n");

        foreach (DataRow dr in data.Rows)
        {
          for (int i = 0; i < data.Columns.Count; i++)
          {
            if (i == 0)
              sw.Write(dr[i].ToString());
            else
              sw.Write("," + dr[i].ToString());
          }
          sw.Write("\n");
        }
      }
    }

    public static void FromCSV(this DataTable data, string filename, string DateFormat)
    {
      using (StreamReader sr = new StreamReader(filename, Encoding.Default))
      {
        var headline = sr.ReadLine().Split(',');
        data.Columns.Add(headline[0], typeof(int));
        data.Columns.Add(headline[1], typeof(DateTime));

        foreach (var col in headline.Skip(2))
          data.Columns.Add(col, typeof(double));

        while (!sr.EndOfStream)
        {
          var rowdata = sr.ReadLine().Split(',');
          var newrow = data.NewRow();
          newrow[0] = int.Parse(rowdata[0]);
          if (string.IsNullOrEmpty(DateFormat))
            newrow[1] = DateTime.Parse (rowdata[1]);
          else
            newrow[1]= DateTime.ParseExact(rowdata[1],DateFormat, null);

          for (int i = 2; i < rowdata.Count(); i++)
          {
            if (!string.IsNullOrEmpty(rowdata[i]))
            {
              double val =double.Parse(rowdata[i]);
              if (val == 0)
                newrow[i] = 0;
              else if (Math.Abs(val) > 1.0e-34)
                newrow[i] = val;
            }
          }
          data.Rows.Add(newrow);
        }
      }
      data.PrimaryKey = new DataColumn[] { data.Columns[0], data.Columns[1] };

    }

    public static bool? SafeParseBool(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return bool.Parse(attrib.Value);
    }

    public static string SafeParseString(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return attrib.Value;
    }

    public static double? SafeParseDouble(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return double.Parse(attrib.Value);
    }

    public static int? SafeParseInt(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return int.Parse(attrib.Value);
    }



  }
}
