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
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Buffer
{
  /// <summary>
  /// The SmartBuffer class provides bufferig functionality that will store values needed for a
  /// particular output item in memory and functionality that will interpolate, extrapolate and aggregate 
  /// values from these values.
  /// </summary>
  /// <remarks>
  /// The content of the SmartBuffer is lists of corresponding times and ValueSets,
  /// where times can be TimeStamps or TimeSpans and the ValueSets are double[].
  /// 
  /// It works on a <see cref="ValueSetArray{T}"/> of type double.
  ///
  /// SmartBuffer objects may not contain mixtures of TimeSpans and TimeStamps.
  /// The number of Times (TimeSpans or TimeStamps) must equal the number of ValueSets 
  /// ( double[]s) in the SmartBuffer.
  /// </remarks>
  [Serializable]
  public class SmartBuffer
  {
    //readonly List<ITime> _times = new List<ITime>();
    readonly TimeSet _times = new TimeSet();
    //readonly List<double[]> values = new List<double[]>();
    readonly ValueSetArray<double> _values = new ValueSetArray<double>();
    double _relaxationFactor;  //Used for the extrapolation algorithm see also RelaxationFactor property
    bool _doExtendedDataVerification;
    private bool _doExtrapolateAtEnd = true;

    /// <summary>
    /// Create an empty smart-buffer.
    /// </summary>
    public SmartBuffer()
    {
      Create();
    }

    /// <summary>
    /// Create a new SmartBuffer with values and times copied from another SmartBuffer
    /// </summary>
    /// <param name="smartBuffer">The SmartBuffer to copy</param>
    public SmartBuffer(SmartBuffer smartBuffer)
    {
      Create();

      if (smartBuffer.TimesCount > 0)
      {
        for (int i = 0; i < smartBuffer.TimesCount; i++)
        {
          AddValues(new Time(smartBuffer.GetTimeAt(i)), GetCopy(smartBuffer.GetValuesAt(i)));
        }
      }
    }

    public TimeSet TimeSet { get { return (_times); } }
    public ValueSetArray<double> ValueSet { get { return (_values); } }

    private static double[] GetCopy(ICollection array)
    {
      double[] copyArray = new double[array.Count];
      array.CopyTo(copyArray, 0);
      return copyArray;
    }

    private void Create()
    {
      _doExtendedDataVerification = true;
      _relaxationFactor = 1.0;
    }

    public bool DoExtrapolateAtEnd
    {
      get { return _doExtrapolateAtEnd; }
      set { _doExtrapolateAtEnd = value; }
    }

    /// <summary>
    ///	Add corresponding values for time and values to the SmartBuffer.
    /// </summary>
    /// <param name="time"> Description of the time parameter</param>
    /// <param name="valueSet">Description of the values parameter</param>
    /// <remarks>
    /// The AddValues method will internally make a copy of the added times and values. The reason for
    /// doing this is that the times and values arguments are references, and the correspondign values 
    /// could be changed by the owner of the classes
    /// </remarks>
    public void AddValues(ITime time, double[] valueSet)
    {
      //            Console.WriteLine(((time.DurationInDays > 0) ? "Span" : "Stamp") + " ToBuffer: " + time + ": " + valueSet[0]);

      _times.Add(new Time(time)); // save a copy of time

      double[] x = GetCopy(valueSet);
      _values.Values2DArray.Add(x);

      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
    }

    public void AddValues(ITime time, IList values)
    {
      if (values == null || values.Count == 0)
        return;
      if (!(values[0] is double))
      {
        throw new Exception("Buffer only handles doubles");
      }

      _times.Add(new Time(time)); // save a copy of time

      double[] x = GetCopy(values);
      _values.Values2DArray.Add(x);

      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
    }

    /// <summary>
    /// RelaxationFactor. The relaxation factor must be in the interval [0; 1]. The relaxation
    /// parameter is used when doing extrapolation. A value of 1 results in nearest extrapolation
    /// whereas a value 0 results in linear extrapolation.
    /// </summary>
    public double RelaxationFactor
    {
      get
      {
        return _relaxationFactor;
      }
      set
      {
        _relaxationFactor = value;
        if (_relaxationFactor < 0 || _relaxationFactor > 1)
        {
          throw new Exception("ReleaxationFactor is out of range");
        }
      }
    }

    /// <summary>
    /// Returns the timeStep´th ITime.
    /// </summary>
    /// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th ITime</returns>
    public ITime GetTimeAt(int timeStep)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
      return _times[timeStep];
    }

    //===============================================================================================
    // GetValuesAt(int timeStep) : double[]
    //===============================================================================================
    /// <summary>
    /// Returns the timeStep´th double[]
    /// </summary>
    /// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th double[]</returns>
    public double[] GetValuesAt(int timeStep)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
      return _values[timeStep];
    }

    /// <summary>
    /// Returns the ValueSet that corresponds to requestTime. The ValueSet may be found by 
    /// interpolation, extrapolation and/or aggregation.
    /// </summary>
    /// <param name="requestedTime">time for which the value is requested</param>
    /// <returns>valueSet that corresponds to requestTime</returns>
    public List<double> GetValues(ITime requestedTime)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }

      if (!_doExtrapolateAtEnd)
      {
        if (requestedTime.StampAsModifiedJulianDay + requestedTime.DurationInDays >
            _times[_times.Count - 1].StampAsModifiedJulianDay + _times[_times.Count - 1].DurationInDays)
        {
          throw new Exception("Extrapolation after last time step not allowed for this buffer");
        }
      }

      List<double> returnValues = new List<double>(); ;
      if (_values.Values2DArray.Count != 0)
      {
        if (_times[0].DurationInDays > 0 && requestedTime.DurationInDays > 0)
        {
          returnValues = MapFromTimeSpansToTimeSpan(requestedTime);
        }
        else if (_times[0].DurationInDays > 0 && requestedTime.DurationInDays == 0)
        {
          returnValues = MapFromTimeSpansToTimeStamp(requestedTime);
        }
        else if (_times[0].DurationInDays == 0 && requestedTime.DurationInDays > 0)
        {
          returnValues = MapFromTimeStampsToTimeSpan(requestedTime);
        }
        else // time stamps
        {
          returnValues = MapFromTimeStampsToTimeStamp(requestedTime);
        }
      }
      //            Console.WriteLine(((requestedTime.DurationInDays > 0) ? "Span" : "Stamp") + " from " +
      //                ((times[0].DurationInDays > 0) ? "Span" : "Stamp") + " in Buffer: " + requestedTime + ": "
      //                + returnValueSet[0]);
      return returnValues;
    }

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeSpans.
    /// </summary>
    /// <param name="requestedTime">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private List<double> MapFromTimeSpansToTimeSpan(ITime requestedTime)
    {
      try
      {
        int m = _values[0].Length;
        double[][] xr = new double[m][];                                                    // Values to return
        double trb = requestedTime.StampAsModifiedJulianDay;                                // Begin time in requester time interval
        double tre = requestedTime.StampAsModifiedJulianDay + requestedTime.DurationInDays; // End time in requester time interval

        const int nk = 1;

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        for (int i = 0; i < m; i++)
        {
          for (int k = 0; k < nk; k++)
          {
            xr[i][k] = 0;
          }
        }

        for (int n = 0; n < _times.Count; n++)
        {
          double tbbn = _times[n].StampAsModifiedJulianDay;
          double tben = _times[n].StampAsModifiedJulianDay + _times[n].DurationInDays;

          //---------------------------------------------------------------------------
          //    B:           <-------------------------->
          //    R:        <------------------------------------->
          // --------------------------------------------------------------------------
          if (trb <= tbbn && tre >= tben) //Buffered TimeSpan fully included in requested TimeSpan
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                xr[i][k - 1] += sbin * (tben - tbbn) / (tre - trb);
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:        t1|-----------------------|t2
          //           Requested Interval:          rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (tbbn <= trb && tre <= tben) //cover all
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                xr[i][k - 1] += BufferHelper.GetVal(_values[n], i, k);
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:       t1|-----------------|t2
          //           Requested Interval:                 rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (tbbn < trb && trb < tben && tre > tben)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                xr[i][k - 1] += sbin * (tben - trb) / (tben - tbbn);

                if (n == _times.Count - 1) // extrapolation needed
                {
                  xr[i][k - 1] += sbin * (tre - tben) / (tben - tbbn);
                }
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:             t1|-----------------|t2
          //           Requested Interval:      rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (trb < tbbn && tre > tbbn && tre < tben)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                xr[i][k - 1] += sbin * (tre - tbbn) / (tben - tbbn);

                if (n == 0) // extrapolation needed
                {
                  xr[i][k - 1] += sbin * (tbbn - trb) / (tben - tbbn);
                }
              }
            }
          }
        }

        if (_relaxationFactor != 1 && (_times.Count >= 2))
        {
          //--------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //        |----------------|                  R
          //---------------------------------------------------------------------------
          double tbb0 = _times[0].StampAsModifiedJulianDay;
          double tbe0 = _times[0].StampAsModifiedJulianDay + _times[0].DurationInDays;
          //double tbb1 = ((ITimeSpan) times[1]).Start.StampAsModifiedJulianDay;
          double tbe1 = _times[1].StampAsModifiedJulianDay + _times[1].DurationInDays;

          if (trb < tbb0 && tre > tbb0)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i, k);
                double sbi1 = BufferHelper.GetVal(_values[1], i, k);
                xr[i][k - 1] += ((tbb0 - trb) / (tre - trb)) * (sbi0 - (1 - _relaxationFactor) * ((tbb0 - trb) * (sbi1 - sbi0) / (tbe1 - tbe0)));
              }
            }
          }

          //-------------------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //                                    |----------------|                  R
          //-------------------------------------------------------------------------------------

          double tbeN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay + _times[_times.Count - 1].DurationInDays;
          double tbbN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay;

          if (tre > tbeN_1 && trb < tbeN_1)
          {
            //double tbeN_2 = ((ITimeSpan) times[times.Count-2]).End.StampAsModifiedJulianDay;
            double tbbN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay;

            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);
                double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);
                xr[i][k - 1] += ((tre - tbeN_1) / (tre - trb)) * (sbiN_1 + (1 - _relaxationFactor) * ((tre - tbbN_1) * (sbiN_1 - sbiN_2) / (tbeN_1 - tbbN_2)));
              }
            }
          }
          //-------------------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //                                              |----------------|   R
          //-------------------------------------------------------------------------------------

          if (trb >= tbeN_1)
          {
            double tbeN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay + _times[_times.Count - 2].DurationInDays;
            //double tbbN_1 = ((ITimeSpan) times[times.Count-1]).Start.StampAsModifiedJulianDay;

            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);
                double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);
                xr[i][k - 1] = sbiN_1 + (1 - _relaxationFactor) * ((sbiN_1 - sbiN_2) / (tbeN_1 - tbbN_2)) * (trb + tre - tbeN_1 - tbeN_2);
              }
            }
          }

          //-------------------------------------------------------------------------------------
          //                           |--------|---------|--------| B
          //        |----------------|   R
          //-------------------------------------------------------------------------------------

          if (tre <= tbb0)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i, k);
                double sbi1 = BufferHelper.GetVal(_values[1], i, k);
                xr[i][k - 1] = sbi0 - (1 - _relaxationFactor) * ((sbi1 - sbi0) / (tbe1 - tbb0)) * (tbe0 + tbb0 - tre - trb);
              }
            }
          }
        }

        //-------------------------------------------------------------------------------------
        List<double> xx = new List<double>(m);

        for (int i = 0; i < m; i++)
        {
          xx.Add(xr[i][0]);
        }

        return xx;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeSpan Failed", e);
      }
    }

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeStamps.
    /// </summary>
    /// <param name="requestedTime">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private List<double> MapFromTimeStampsToTimeSpan(ITime requestedTime)
    {
      try
      {
        int m = _values[0].Length;
        //int        N  = times.Count;								   	      // Number of time steps in buffer
        double[][] xr = new double[m][];                                      // Values to return
        double trb = requestedTime.StampAsModifiedJulianDay;   // Begin time in requester time interval
        double tre = requestedTime.StampAsModifiedJulianDay + requestedTime.DurationInDays;    // End time in requester time interval

        const int nk = 1;

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        for (int i = 0; i < m; i++)
        {
          for (int k = 0; k < nk; k++)
          {
            xr[i][k] = 0;
          }
        }


        for (int n = 0; n < _times.Count - 1; n++)
        {
          double tbn = _times[n].StampAsModifiedJulianDay;
          double tbnp1 = _times[n + 1].StampAsModifiedJulianDay;


          //---------------------------------------------------------------------------
          //    B:           <-------------------------->
          //    R:        <------------------------------------->
          // --------------------------------------------------------------------------
          if (trb <= tbn && tre >= tbnp1)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                double sbinp1 = BufferHelper.GetVal(_values[n + 1], i, k);
                xr[i][k - 1] += 0.5 * (sbin + sbinp1) * (tbnp1 - tbn) / (tre - trb);
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:        t1|-----------------------|t2
          //           Requested Interval:          rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (tbn <= trb && tre <= tbnp1) //cover all
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                double sbinp1 = BufferHelper.GetVal(_values[n + 1], i, k);
                xr[i][k - 1] += sbin + ((sbinp1 - sbin) / (tbnp1 - tbn)) * ((tre + trb) / 2 - tbn);
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:       t1|-----------------|t2
          //           Requested Interval:                 rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (tbn < trb && trb < tbnp1 && tre > tbnp1)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                double sbinp1 = BufferHelper.GetVal(_values[n + 1], i, k);
                xr[i][k - 1] += (sbinp1 - (sbinp1 - sbin) / (tbnp1 - tbn) * ((tbnp1 - trb) / 2)) * (tbnp1 - trb) / (tre - trb);
              }
            }
          }

          //---------------------------------------------------------------------------
          //           Times[i] Interval:             t1|-----------------|t2
          //           Requested Interval:      rt1|--------------|rt2
          // --------------------------------------------------------------------------
          else if (trb < tbn && tre > tbn && tre < tbnp1)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
              {
                double sbin = BufferHelper.GetVal(_values[n], i, k);
                double sbinp1 = BufferHelper.GetVal(_values[n + 1], i, k);
                xr[i][k - 1] += (sbin + (sbinp1 - sbin) / (tbnp1 - tbn) * ((tre - tbn) / 2)) * (tre - tbn) / (tre - trb);
              }
            }
          }
        }


        // In case of only one value in the buffer, regardless of its position relative to R
        //--------------------------------------------------------------------------
        //    |     or     |     or   |               B
        //        |----------------|                  R
        //---------------------------------------------------------------------------
        if (_times.Count == 1)
        {
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++)
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i, k);
              xr[i][k - 1] = sbi0;
            }
          }
        }
        else
        {
          //--------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //        |----------------|                  R
          //---------------------------------------------------------------------------
          double tb0 = _times[0].StampAsModifiedJulianDay;
          //double tb1   = ((ITimeStamp) times[0]).ModifiedJulianDay;
          double tb1 = _times[1].StampAsModifiedJulianDay; // line above was corrected to this Gregersen Sep 15 2004
          double tbN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay;
          double tbN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay;

          if (trb < tb0 && tre > tb0)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i, k);
                double sbi1 = BufferHelper.GetVal(_values[1], i, k);
                xr[i][k - 1] += ((tb0 - trb) / (tre - trb)) * (sbi0 - (1 - _relaxationFactor) * 0.5 * ((tb0 - trb) * (sbi1 - sbi0) / (tb1 - tb0)));
              }
            }
          }
          //-------------------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //                                    |----------------|                  R
          //-------------------------------------------------------------------------------------
          if (tre > tbN_1 && trb < tbN_1)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);
                double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);
                xr[i][k - 1] += ((tre - tbN_1) / (tre - trb)) * (sbiN_1 + (1 - _relaxationFactor) * 0.5 * ((tre - tbN_1) * (sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)));
              }
            }
          }
          //-------------------------------------------------------------------------------------
          //              |--------|---------|--------| B
          //                                              |----------------|   R
          //-------------------------------------------------------------------------------------
          if (trb >= tbN_1)
          {

            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);
                double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);

                xr[i][k - 1] = sbiN_1 + (1 - _relaxationFactor) * ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (0.5 * (trb + tre) - tbN_1);
              }
            }
          }
          //-------------------------------------------------------------------------------------
          //                           |--------|---------|--------| B
          //        |----------------|   R
          //-------------------------------------------------------------------------------------
          if (tre <= tb0)
          {
            for (int k = 1; k <= nk; k++)
            {
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i, k);
                double sbi1 = BufferHelper.GetVal(_values[1], i, k);
                xr[i][k - 1] = sbi0 - (1 - _relaxationFactor) * ((sbi1 - sbi0) / (tb1 - tb0)) * (tb0 - 0.5 * (trb + tre));
              }
            }
          }
        }
        //-------------------------------------------------------------------------------------
        List<double> xx = new List<double>(m);

        for (int i = 0; i < m; i++)
        {
          xx.Add(xr[i][0]);
        }

        return xx;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeSpan Failed", e);
      }
    }


    /// <summary>
    /// A ValueSet corresponding to a TimeStamp is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeStamps.
    /// </summary>
    /// <param name="requestedTimeStamp">TimeStamp for which the values are requested</param>
    /// <returns>ValueSet that corresponds to the requested time stamp</returns>
    private List<double> MapFromTimeStampsToTimeStamp(ITime requestedTimeStamp)
    {
      try
      {
        int m = (_values[0]).Length;
        double[][] xr = new double[m][];                             // Values to return
        double tr = requestedTimeStamp.StampAsModifiedJulianDay;		     // Requested TimeStamp

        const int nk = 1;

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        if (_times.Count == 1)
        {
          //---------------------------------------------------------------------------
          //    Buffered TimesStamps: |          >tb0<  
          //    Requested TimeStamp:  |    >tr<
          // or Requested TimeStamp:  |          >tr<
          // or Requested TimeStamp:  |                >tr<
          //                           -----------------------------------------> t
          // --------------------------------------------------------------------------
          if (tr > (_times[0].StampAsModifiedJulianDay + Time.EpsilonForTimeCompare) && !_doExtrapolateAtEnd)
          {
            throw new Exception("Extrapolation not allowed");
          }
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              xr[i][k - 1] = BufferHelper.GetVal(_values[0], i, k);
            }
          }
        }
        else if (tr <= _times[0].StampAsModifiedJulianDay)
        {
          //---------------------------------------------------------------------------
          //  Buffered TimesStamps: |          >tb0<   >tb1<   >tb2<  >tbN<
          //  Requested TimeStamp:  |    >tr<
          //                         -----------------------------------------> t
          // --------------------------------------------------------------------------
          double tb0 = _times[0].StampAsModifiedJulianDay;
          double tb1 = _times[1].StampAsModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i, k);
              double sbi1 = BufferHelper.GetVal(_values[1], i, k);
              xr[i][k - 1] = ((sbi0 - sbi1) / (tb0 - tb1)) * (tr - tb0) * (1 - _relaxationFactor) + sbi0;
            }
          }
        }
        else if (tr > _times[_times.Count - 1].StampAsModifiedJulianDay)
        {
          //---------------------------------------------------------------------------
          //  Buffered TimesStamps: |    >tb0<   >tb1<   >tb2<  >tbN_2<  >tbN_1<
          //  Requested TimeStamp:  |                                             >tr<
          //                         ---------------------------------------------------> t
          // --------------------------------------------------------------------------
          double tbN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay;
          double tbN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [N-1]
            {
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);

              xr[i][k - 1] = ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (tr - tbN_1) * (1 - _relaxationFactor) + sbiN_1;
            }
          }
        }
        else
        {
          //---------------------------------------------------------------------------
          //  Availeble TimesStamps: |    >tb0<   >tb1<  >tbna<       >tnb<   >tbN_1<  >tbN_2<
          //  Requested TimeStamp:   |                          >tr<
          //                         -------------------------------------------------> t
          // --------------------------------------------------------------------------
          for (int n = _times.Count - 2; n >= 0; n--)
          {
            double tbn1 = _times[n].StampAsModifiedJulianDay;
            double tbn2 = _times[n + 1].StampAsModifiedJulianDay;

            if (tbn1 <= tr && tr <= tbn2)
            {
              double tbn2Fraction = (tr - tbn1) / (tbn2 - tbn1);
              for (int k = 1; k <= nk; k++)
              {
                for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [n]
                {
                  double[] valueSet_n = _values[n];
                  double sbin1 = BufferHelper.GetVal(valueSet_n, i, k);

                  double[] valueSet_nPlus1 = _values[n + 1];
                  double sbin2 = BufferHelper.GetVal(valueSet_nPlus1, i, k);

                  xr[i][k - 1] = tbn2Fraction * (sbin2 - sbin1) + sbin1;
                }
              }
              break;
            }
          }
        }
        //----------------------------------------------------------------------------------------------
        List<double> xx = new List<double>(m);

        for (int i = 0; i < m; i++)
        {
          xx.Add(xr[i][0]);
        }

        return xx;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeStamp Failed", e);
      }
    }

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeSpans.
    /// </summary>
    /// <param name="requestedTimeStamp">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private List<double> MapFromTimeSpansToTimeStamp(ITime requestedTimeStamp)
    {
      try
      {
        int m = (_values[0]).Length;
        double[][] xr = new double[m][];                             // Values to return
        double tr = requestedTimeStamp.StampAsModifiedJulianDay; 	     // Requested TimeStamp

        const int nk = 1;

        for (int i = 0; i < m; i++)
        {
          xr[i] = new double[nk];
        }

        if (_times.Count == 1)
        {
          //---------------------------------------------------------------------------
          //    Buffered TimesStamps: |         >tbb0<  
          //    Requested TimeStamp:  |    >tr<
          // or Requested TimeStamp:  |          >tr<
          // or Requested TimeStamp:  |                >tr<
          //                           -----------------------------------------> t
          // --------------------------------------------------------------------------
          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              xr[i][k - 1] = BufferHelper.GetVal(_values[0], i, k);
            }
          }
        }
        //---------------------------------------------------------------------------
        //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
        //  Requested TimeStamp:  |    >tr<
        //                         -----------------------------------------> t
        // --------------------------------------------------------------------------
        else if (tr <= _times[0].StampAsModifiedJulianDay)
        {
          double tbb0 = _times[0].StampAsModifiedJulianDay;
          double tbb1 = _times[1].StampAsModifiedJulianDay;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i, k);
              double sbi1 = BufferHelper.GetVal(_values[1], i, k);
              xr[i][k - 1] = ((sbi0 - sbi1) / (tbb0 - tbb1)) * (tr - tbb0) * (1 - _relaxationFactor) + sbi0;
            }
          }
        }

        //---------------------------------------------------------------------------
        //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
        //  Requested TimeStamp:  |                                             >tr<
        //                         ---------------------------------------------------> t
        // --------------------------------------------------------------------------
        else if (tr >= _times[_times.Count - 1].StampAsModifiedJulianDay + _times[_times.Count - 1].DurationInDays)
        {
          double tbeN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay + _times[_times.Count - 2].DurationInDays;
          double tbeN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay + _times[_times.Count - 1].DurationInDays;

          for (int k = 1; k <= nk; k++)
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [N-1]
            {
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i, k);
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i, k);

              xr[i][k - 1] = ((sbiN_1 - sbiN_2) / (tbeN_1 - tbeN_2)) * (tr - tbeN_1) * (1 - _relaxationFactor) + sbiN_1;
            }
          }
        }

                //---------------------------------------------------------------------------
        //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
        //  Requested TimeStamp:   |                          >tr<
        //                         -------------------------------------------------> t
        // --------------------------------------------------------------------------
        else
        {
          for (int n = _times.Count - 1; n >= 0; n--)
          {
            double tbbn = _times[n].StampAsModifiedJulianDay;
            double tben = _times[n].StampAsModifiedJulianDay + _times[n].DurationInDays;

            if (tbbn <= tr && tr < tben)
            {
              for (int k = 1; k <= nk; k++)
              {
                for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [n]
                {
                  xr[i][k - 1] = BufferHelper.GetVal(_values[n], i, k);
                }
              }
              break;
            }
          }
        }

        //----------------------------------------------------------------------------------------------

        List<double> xx = new List<double>(m);

        for (int i = 0; i < m; i++)
        {
          xx.Add(xr[i][0]);
        }

        return xx;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeStamp Failed", e);
      }
    }

    /// <summary>
    /// Number of time streps in the buffer.
    /// </summary>
    public int TimesCount
    {
      get
      {
        return _times.Count;
      }
    }

    /// <summary>
    /// Read only property for the number of values in each of the valuesets contained in the buffer.
    /// </summary>
    public int ValuesCount
    {
      get
      {
        return (_values[0]).Length;
      }
    }

    /// <summary>
    /// Checks weather the contents of the buffer is valid.
    /// </summary>
    public void CheckBuffer()
    {
      if (_times.Count != _values.Values2DArray.Count)
      {
        throw new Exception("Different numbers of values and times in buffer");
      }

      if (_times.Count == 0)
      {
        throw new Exception("Buffer is empty");
      }

      foreach (ITime t in _times)
      {
        if (t.DurationInDays < 0)
        {
          throw new NotSupportedException("Duration is less than zero");
        }
      }

      for (int i = 1; i < _times.Count; i++)
      {
        if (_times[i].StampAsModifiedJulianDay <= _times[i - 1].StampAsModifiedJulianDay)
          throw new Exception("TimeStamps are not increasing in buffer");
      }
    }

    /// <summary>
    /// Read/Write property flag that indicates wheather or not to perform extended data
    /// checking.
    /// </summary>
    public bool DoExtendedDataVerification
    {
      get
      {
        return _doExtendedDataVerification;
      }
      set
      {
        _doExtendedDataVerification = value;
      }
    }


    /// <summary>
    /// Clear all times and values in the buffer at or later than the specified time
    /// If the specified time is type ITimeSpan the Start time is used.
    /// </summary>
    /// <param name="time"></param>
    public void ClearAfter(ITime time)
    {
      for (int i = 0; i < _times.Count; i++)
      {
        if (time.StampAsModifiedJulianDay <= _times[i].StampAsModifiedJulianDay)
        {
          // clear after current time
          int numberOfValuesToRemove = _times.Count - i;
          _times.RemoveRange(i, numberOfValuesToRemove);
          _values.Values2DArray.RemoveRange(i, numberOfValuesToRemove);
          return;
        }
      }
    }

    /// <summary>
    /// Clear all records in the buffer assocaited to time that is earlier that the
    /// time specified in the argument list. However, one record associated to time 
    /// before the time in the argument list is left in the buffer.
    /// The criteria when comparing TimeSpans is that they may not overlap in order
    /// to be regarded as before each other.
    /// (see also HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(ITime ta, ITime tb)
    /// </summary>
    /// <param name="time">time before which the records are removed</param>
    public void ClearBefore(ITime time)
    {
      for (int i = _times.Count - 1; i >= 0; i--)
      {
        if (BufferHelper.IsBefore(_times[i], time))
        {
          // clear before current time
          _times.RemoveRange(0, i);
          _values.Values2DArray.RemoveRange(0, i);
          return;
        }
      }
    }

    public IList<IList<double>> GetAllValues()
    {
      IList<IList<double>> valuesList = new List<IList<double>>();
      int nTimes = _times.Count;
      for (int t = 0; t < nTimes; t++)
      {
        valuesList[t] = new List<double>();
        double[] timeStepValues = _values[t];
        for (int j = 0; j < timeStepValues.Length; j++)
        {
          valuesList[t].Add(timeStepValues[j]);
        }
      }
      return valuesList;
    }

    public void SetOrAddValues(ITime time, IList values)
    {
      int index = _times.Times.IndexOf(time);
      if (index < 0)
      {
        AddValues(time, values);
      }
      else
      {
        double[] copy = GetCopy(values);
        this._values[index] = copy;
      }
    }
  }
}
