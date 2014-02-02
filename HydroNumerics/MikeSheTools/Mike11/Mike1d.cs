using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Mike1D.Generic;
using DHI.Mike1D.CrossSectionModule;


namespace HydroNumerics.MikeSheTools.Mike11
{
  public class Mike1d
  {

    public void Open()
    {
      Connection c = Connection.Create(@"..\..\..\testdata\mike11\novomr6.xns11");

      CrossSectionDataFactory cd = new CrossSectionDataFactory();

      var xsecs = cd.Open(@"..\..\..\testdata\mike11\novomr6.xns11", null);
      

      CrossSectionFactory cf = new CrossSectionFactory();
      var v= new CrossSectionPointList();
      v.Add(new CrossSectionPoint(0,5));
      v.Add(new CrossSectionPoint(1,2));
      v.Add(new CrossSectionPoint(2,2));
      v.Add(new CrossSectionPoint(3,5));

      cf.BuildOpen("tempo");
      cf.SetRawPoints(v);
      
      var cs = cf.GetCrossSection();

      CrossSectionData d = new CrossSectionData();
      d.Add(cs);
      d.Connection = Connection.Create(@"c:\temp\new.xns11");
      


      
     
      CrossSectionFactory cdd = new CrossSectionFactory();
     
     
    }

  }
}
