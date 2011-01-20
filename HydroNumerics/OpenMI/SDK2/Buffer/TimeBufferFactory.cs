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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Buffer
{
  public class TimeBufferFactory : IAdaptedOutputFactory
  {
    private readonly string id;

    public TimeBufferFactory(string id)
    {
      this.id = id;
    }

    #region Implementation of IDescribable

    public string Caption { get; set; }

    public string Description { get; set; }

    #endregion

    #region Implementation of IIdentifiable

    public string Id
    {
      get
      {
        return id;
      }
    }

    #endregion

    #region Implementation of IAdaptedOutputFactory

    public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput parentItem, IBaseInput target)
    {
      // The time methods in this factory only works on an ITimeSpaceOutput
      ITimeSpaceOutput tsoutput = parentItem as ITimeSpaceOutput;
      if (tsoutput == null)
        return (new IIdentifiable[0]);
      return new IIdentifiable[] { new TimeInterpolator(tsoutput), new TimeExtrapolator(tsoutput) };
    }

    public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputIdentifier, IBaseOutput adaptee, IBaseInput target)
    {
      IBaseAdaptedOutput adaptor = adaptedOutputIdentifier as IBaseAdaptedOutput;
      if (adaptor == null)
      {
        throw new ArgumentException("Unknown adaptedOutput type - it does not originate from this factory");
      }
      // Connect the adaptor and the adaptee
      if (!adaptee.AdaptedOutputs.Contains(adaptor))
      {
        adaptee.AddAdaptedOutput(adaptor);
      }
      return adaptor;
    }

    #endregion
  }
}
