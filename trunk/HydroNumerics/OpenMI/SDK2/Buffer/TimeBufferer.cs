#region Copyright
/*
* Copyright (c) HydroInform ApS & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the HydroInform ApS & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "HydroInform ApS & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "HydroInform ApS & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Buffer
{
  public abstract class TimeBufferer : AbstractAdaptedOutput
  {
    protected SmartBuffer _buffer;
    //private TimeSet _timeBufferTimeSet;

    protected TimeBufferer(string id)
      : base(id)
    {
      CreateBufferAndTimeSet();
    }

    protected TimeBufferer(string id, ITimeSpaceOutput decoratedOutput)
      : base(id, decoratedOutput)
    {
      CreateBufferAndTimeSet();
    }

    public override ITimeSet TimeSet
    {
      get { return _buffer.TimeSet; }
      set { throw new NotSupportedException("Setting Timeset not allowed"); }
    }


    public override ISpatialDefinition SpatialDefinition
    {
      get
      {
        if (_adaptee != null)
          return _adaptee.SpatialDefinition;
        return (base.SpatialDefinition);
      }
    }

    public TimeSet TTimeSet
    {
      get { return _buffer.TimeSet; }
    }

    public override ITimeSpaceValueSet Values
    {
      get { return _buffer.ValueSet; }
      set { throw new NotSupportedException("Setting Values not allowed"); }
    }

    public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      if (timeSpaceQuery.TimeSet == null || timeSpaceQuery.TimeSet.Times == null ||
          timeSpaceQuery.TimeSet.Times.Count == 0)
      {
        throw new Exception("Invalid query specifier \"" + timeSpaceQuery.Id +
                            "\" for in GetValues() call to time decorater " + Id);
      }

      // Determinee query time and currently available time
      double queryTimeAsMJD =
          timeSpaceQuery.TimeSet.Times[timeSpaceQuery.TimeSet.Times.Count - 1].StampAsModifiedJulianDay +
          timeSpaceQuery.TimeSet.Times[timeSpaceQuery.TimeSet.Times.Count - 1].DurationInDays;

      double availableTimeAsMJD = Double.NegativeInfinity;
      IList<ITime> currentTimes = TimeSet.Times;
      if (currentTimes.Count > 0)
      {
        availableTimeAsMJD =
            currentTimes[currentTimes.Count - 1].StampAsModifiedJulianDay +
            currentTimes[currentTimes.Count - 1].DurationInDays;
      }

      // Check if we need to update
      if (availableTimeAsMJD < queryTimeAsMJD)
      {
        // TODO (JGr): output item should not do the actual update of the component?
        if (Adaptee == null)
        {
          throw new Exception("Can not update, no parent output defined, calling GetValues() of time bufferer " + Id);
        }
        if (_adaptee.TimeSet == null || _adaptee.TimeSet.Times == null)
        {
          throw new Exception("Invalid time specifier in decorated output item \"" + Adaptee.Id +
                              "\" for in GetValues() call to time decorater " + Id);
        }

        // Update as far as needed.
        IBaseLinkableComponent linkableComponent = Adaptee.Component;
        while ((linkableComponent.Status == LinkableComponentStatus.Valid ||
                linkableComponent.Status == LinkableComponentStatus.Updated) &&
               availableTimeAsMJD < queryTimeAsMJD)
        {
          linkableComponent.Update();
          // Determine newly available time
          IList<ITime> parentTimes = _adaptee.TimeSet.Times;
          availableTimeAsMJD =
              parentTimes[parentTimes.Count - 1].StampAsModifiedJulianDay +
              parentTimes[parentTimes.Count - 1].DurationInDays;
        }
      }

      // Return the values for the required time(s)
      IList<IList<double>> resultValues = new List<IList<double>>();
      if (timeSpaceQuery.TimeSet != null && timeSpaceQuery.TimeSet.Times != null)
      {
        for (int t = 0; t < timeSpaceQuery.TimeSet.Times.Count; t++)
        {
          resultValues.Add(new List<double>());
          ITime queryTime = timeSpaceQuery.TimeSet.Times[t];
          List<double> valuesForTimeStep = _buffer.GetValues(queryTime);
          foreach (double d in valuesForTimeStep)
          {
            resultValues[t].Add(d);
          }
        }
      }
      return new TimeSpaceValueSet<double>(resultValues);
    }

    public override void Refresh()
    {
      if (Adaptee.Component.Status != LinkableComponentStatus.Validating &&
          Adaptee.Component.Status != LinkableComponentStatus.Updating)
      {
        throw new Exception(
            "Update function can only be called from component when it is validating or updating");
      }
      AddNewValuesToBuffer();

      // Update dependent adaptedOutput
      foreach (ITimeSpaceAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }

    private void CreateBufferAndTimeSet()
    {
      _buffer = new SmartBuffer();
      _timeSet = null;
      _values = null;
      //_timeBufferTimeSet = new TimeSet();
      UpdateTimeHorizonFromDecoratedOutputItem();
    }

    private void UpdateTimeHorizonFromDecoratedOutputItem()
    {
      if (Adaptee != null)
      {
        if (_adaptee.TimeSet == null)
        {
          // The decorated item has no time set, should not occur
          throw new Exception("Parent output \"" + Adaptee.Id + "\" has no time set");
        }
        // Use the time horizon of the decorated item
        ITime decoratedTimeHorizon = _adaptee.TimeSet.TimeHorizon;
        if (decoratedTimeHorizon != null)
        {
          _buffer.TimeSet.TimeHorizon =
              new Time(decoratedTimeHorizon.StampAsModifiedJulianDay,
                       decoratedTimeHorizon.DurationInDays);
        }
      }
      else
      {
        _buffer.TimeSet.TimeHorizon = null;
      }
    }

    private void AddNewValuesToBuffer()
    {
      ITimeSpaceValueSet decoratedOutputItemValues = (ITimeSpaceValueSet)Adaptee.Values;

      if (decoratedOutputItemValues == null)
      {
        throw new Exception("AdaptedOutput \"" + Id +
                            "\" did not receive values from Decorated OutputItem \"" + Adaptee.Id +
                            "\"");
      }

      for (int t = 0; t < _adaptee.TimeSet.Times.Count; t++)
      {
        ITime time = _adaptee.TimeSet.Times[t];
        IList elementSetValues = decoratedOutputItemValues.GetElementValuesForTime(t);
        _buffer.SetOrAddValues(time, elementSetValues);
      }
    }
  }
}