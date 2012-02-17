using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using System.Text;

using HydroNumerics.JupiterTools;

namespace HydroNumerics.JupiterTools.Web
{
  public class Class1
  {


    public void Download(DateTime UpdateDate)
    {
      Select.SelectInterfaceClient sel = new Select.SelectInterfaceClient();

      JupiterClassesDataContext jc = new JupiterClassesDataContext("Server=tcp:te5cczmjwk.database.windows.net;Database=Sjaelland;User ID=JacobGudbjerg;Password=Stinj99!;Trusted_Connection=False;Encrypt=True");
      Stopwatch sw = new Stopwatch();
      sw.Start();

      var MaxDate = (from d in jc.WRRCATCHMENTs select d.INSERTDATE).Max().Value.ToString("yyyy-MM-dd hh:mm:ss");

      List<int> regionnumbers = new List<int>();
      regionnumbers.Add(1084);
      regionnumbers.Add(1085);

      foreach (var rgn in regionnumbers)
      {
        string sql = "select w.* from DRWPlant d, WRRCatchment w where d.REGION=" + rgn + " and w.plantid=d.PLANTID and w.INSERTDATE >to_date('" + MaxDate + "', 'yyyy-mm-dd hh24:mi:ss')";

        var res = sel.select(sql);
        sw.Stop();

        var t = sw.Elapsed;

        foreach (var r in res.resultset.records)
        {
          var v = new Linq2Sql.WRRCATCHMENT();

          UpdateObject(v, r, res.resultset.columnNames);
          jc.WRRCATCHMENTs.InsertOnSubmit(v);
        }

        jc.SubmitChanges();
      }
    }


    public void UpdateObject(object o, Select.DataRecord data, string[] ColumnNames)
    {

      for (int i = 0; i < ColumnNames.Count(); i++)
      {
        PropertyInfo propInfo = o.GetType().GetProperty(ColumnNames[i]);

        object value=null;

        if (data.data[i] != null)
        {
          if (propInfo.PropertyType.Equals(typeof(double)) | propInfo.PropertyType.Equals(typeof(double?)))
            value = double.Parse(data.data[i].ToString());
          else if (propInfo.PropertyType.Equals(typeof(int)) | propInfo.PropertyType.Equals(typeof(int?)))
            value = int.Parse(data.data[i].ToString());
          else if (propInfo.PropertyType.Equals(typeof(string)))
            value = data.data[i].ToString();
          else if (propInfo.PropertyType.Equals(typeof(float)) | propInfo.PropertyType.Equals(typeof(float?)))
            value = float.Parse(data.data[i].ToString());
          else if (propInfo.PropertyType.Equals(typeof(DateTime)) | propInfo.PropertyType.Equals(typeof(DateTime?)))
            value = DateTime.Parse(data.data[i].ToString());
          else
            throw new Exception("Unmapped type");

          propInfo.SetValue(o, value, null);
        }
      }

    }

  }
}
