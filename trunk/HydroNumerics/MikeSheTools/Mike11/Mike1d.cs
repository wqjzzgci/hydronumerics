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
      Connection c = Connection.Create(@"K:\silkeborg\mike11\moto50m_M11.xns11");

      CrossSectionDataFactory cd = new CrossSectionDataFactory();
      var f = CrossSectionDataFactory.KnownTypes;

      CrossSectionFactory cf = new CrossSectionFactory();
     

      var xsecs = cd.Read(c, null);
      CrossSectionData d = new CrossSectionData();
      CrossSectionFactory cdd = new CrossSectionFactory();
     
     
    }

  }
}
