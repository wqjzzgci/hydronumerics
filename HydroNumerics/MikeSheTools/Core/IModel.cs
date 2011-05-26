using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace HydroNumerics.MikeSheTools.Core
{
  [ServiceContract]
  public interface IModel
  {
    [OperationContract]
    void Load(string SheFileName);
    System.Collections.Generic.List<MikeSheWell> ObservationWells { get; }
    [DataMember]
    System.Collections.Generic.List<CalibrationParameter> Parameters { get; }
    Results Results { get; }
    [OperationContract]
    void Run();
  }
}
