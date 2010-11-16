using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HydroNumerics.HydroNet.Core
{

  [DataContract (IsReference=true)]
  public class StagnantExchangeBoundary:AbstractBoundary,ISink,ISource
  {
    #region ISource Members

    [DataMember]
    private double _exchangeRate;

    public StagnantExchangeBoundary(double ExchangeRate)
    {
      _exchangeRate = ExchangeRate;
    }

    private DateTime _currentTime;

    private void MoveInTime(DateTime Start, TimeSpan TimeStep)
    {
      var c = ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon);
      double d = _waterPacket.GetConcentration(c);
      _waterPacket.SetConcentration(c, d + 2.500 * TimeStep.TotalDays);
    }
    

    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      MoveInTime(Start, TimeStep);
      return WaterSample.Substract(_exchangeRate * TimeStep.TotalSeconds);     
    }

    private WaterPacket _waterPacket;

    [DataMember]
    public IWaterPacket WaterSample
    {
      get
      {
        return _waterPacket;
      }
      set
      {
        _waterPacket = (WaterPacket)value;
      }
    }

    #endregion

    #region IBoundary Members


    public void Initialize()
    {
    }

    public DateTime EndTime
    {
      get { return DateTime.MaxValue;  }
    }

    public DateTime StartTime
    {
      get { return DateTime.MinValue; }
    }

    #endregion

    #region ISink Members

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      return _exchangeRate * TimeStep.TotalSeconds;
    }

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      WaterSample.Add(Water);
      Output.Log(WaterSample, Start, Start.Add(TimeStep));
    }

    #endregion
  }
}
