using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class OffsetConverter : TimeSeriesConverter
  {

    public OffsetConverter()
    {
      TypeOfConverter = ConverterTypes.Offset;
    }



    protected override void Initialize()
    {
      ConvertFunction = new Func<double, double>(x => x + Par1 ?? 0);
      ConvertBackFunction = new Func<double, double>(x => x - Par1 ?? 0);
    }



  
  }


}
