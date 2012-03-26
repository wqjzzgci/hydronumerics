using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;

using System.Text;

using HydroNumerics.JupiterTools;

namespace HydroNumerics.JupiterTools.Web
{
  public class Class1
  {


    public void DownloadUpdates()
    {
      Select.SelectInterfaceClient sel = new Select.SelectInterfaceClient();

      JupiterClassesDataContext jc = new JupiterClassesDataContext("Server=tcp:te5cczmjwk.database.windows.net;Database=Sjaelland;User ID=JacobGudbjerg;Password=Stinj99!;Trusted_Connection=False;Encrypt=True");

      string MaxDate = "2012-01-18 00:00:00";

      List<int> regionnumbers = new List<int>();
      regionnumbers.Add(1084);
      regionnumbers.Add(1085);

      foreach (var rgn in regionnumbers)
      {
        string sql = "select w.* from DRWPlant d, WRRCatchment w where d.REGION=" + rgn + " and w.plantid=d.PLANTID and w.UPDATEDATE >to_date('" + MaxDate + "', 'yyyy-mm-dd hh24:mi:ss')";
        Linq2Sql.WRRCATCHMENT wr = new Linq2Sql.WRRCATCHMENT();

        var pks = GetPrimaryKeys<WRRCATCHMENT>().Single();
        var res = sel.select(sql);
        int indexofid = res.resultset.columnNames.ToList().IndexOf(pks.Name);

        foreach (var r in res.resultset.records)
        {
          var vr =  jc.WRRCATCHMENTs.Single(v=> v.PLANTCATCHMENTID == int.Parse(r.data[indexofid].ToString()));
          UpdateObject(vr, r, res.resultset.columnNames);
        }


        sql = "select * from DRWPlant d where d.REGION=" + rgn + " and d.UPDATEDATE >to_date('" + MaxDate + "', 'yyyy-mm-dd hh24:mi:ss')";
        Linq2Sql.DRWPLANT dp = new Linq2Sql.DRWPLANT();

        var pksP = GetPrimaryKeys<DRWPLANT>().Single();
        var res2 = sel.select(sql);
        int indexofidP = res2.resultset.columnNames.ToList().IndexOf(pksP.Name);

        foreach (var r in res2.resultset.records)
        {
          var vr = jc.DRWPLANTs.Single(v => v.PLANTID == int.Parse(r.data[indexofid].ToString()));
          UpdateObject(vr, r, res2.resultset.columnNames);
        }

        jc.SubmitChanges();
      }
    }


    



    public void Download()
    {
      Select.SelectInterfaceClient sel = new Select.SelectInterfaceClient();

      JupiterClassesDataContext jc = new JupiterClassesDataContext("Server=tcp:te5cczmjwk.database.windows.net;Database=Sjaelland;User ID=JacobGudbjerg;Password=Stinj99!;Trusted_Connection=False;Encrypt=True");
      Stopwatch sw = new Stopwatch();
      sw.Start();

      var MaxDate = (from d in jc.WRRCATCHMENTs select d.INSERTDATE).Max().Value.ToString("yyyy-MM-dd HH:mm:ss");

      var MaxDatePlant = (from d in jc.DRWPLANTs select d.INSERTDATE).Max().Value.ToString("yyyy-MM-dd HH:mm:ss");


      MaxDate = "2012-01-18 00:00:00";
      MaxDatePlant = "2012-01-18 00:00:00";


      List<int> regionnumbers = new List<int>();
      regionnumbers.Add(1084);
      regionnumbers.Add(1085);

      var pks = GetPrimaryKeys<WRRCATCHMENT>().Single();
      var pksP = GetPrimaryKeys<DRWPLANT>().Single();



      foreach (var rgn in regionnumbers)
      {
        string sql = "select w.* from DRWPlant d, WRRCatchment w where d.REGION=" + rgn + " and w.plantid=d.PLANTID and w.INSERTDATE >to_date('" + MaxDate + "', 'yyyy-mm-dd hh24:mi:ss')";

        var res = sel.select(sql);
        int indexofid = res.resultset.columnNames.ToList().IndexOf(pks.Name);

          foreach (var r in res.resultset.records)
          {
            var vr = jc.WRRCATCHMENTs.FirstOrDefault(v => v.PLANTCATCHMENTID == int.Parse(r.data[indexofid].ToString()));

            if (vr == null)
            {
              var v = new Linq2Sql.WRRCATCHMENT();
              UpdateObject(v, r, res.resultset.columnNames);
              jc.WRRCATCHMENTs.InsertOnSubmit(v);
            }
          }
        sql = "select * from DRWPlant d where d.REGION=" + rgn + " and d.INSERTDATE >to_date('" + MaxDatePlant + "', 'yyyy-mm-dd hh24:mi:ss')";

        var res2 = sel.select(sql);
        int indexofidP = res2.resultset.columnNames.ToList().IndexOf(pksP.Name);


        foreach (var r in res2.resultset.records)
        {
          var vr = jc.DRWPLANTs.FirstOrDefault(v => v.PLANTID == int.Parse(r.data[indexofid].ToString()));
          if (vr == null)
          {
            var v = new Linq2Sql.DRWPLANT();

            UpdateObject(v, r, res2.resultset.columnNames);
            jc.DRWPLANTs.InsertOnSubmit(v);
          }
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


    public static List<PropertyInfo> GetPrimaryKeys<T>()
    {
      PropertyInfo[] infos = typeof(T).GetProperties();
      List<PropertyInfo> PrimaryKeys = new List<PropertyInfo>();

      foreach (PropertyInfo info in infos)
      {
        var column = info.GetCustomAttributes(false)
         .Where(x => x.GetType() == typeof(ColumnAttribute))
         .Single(x => ((ColumnAttribute)x).IsPrimaryKey && ((ColumnAttribute)x).DbType.Contains("NOT NULL"));
        if (column != null)
        {
          PrimaryKeys.Add(info);
          break;
        }
      }
      return PrimaryKeys;
    }
  }
}
