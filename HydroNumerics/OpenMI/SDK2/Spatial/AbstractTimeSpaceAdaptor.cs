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
using System.Collections.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// An abstract base class for an <see cref="ITimeSpaceAdaptedOutput"/>.
  /// <para>
  /// By default all methods are implemented as virtual methods, calling
  /// the same method on the <see cref="Adaptee"/>. An actual adaptor
  /// must override the required methods.
  /// </para>
  /// </summary>
  public abstract class AbstractTimeSpaceAdaptor : ITimeSpaceAdaptedOutput
  {
    protected readonly ITimeSpaceOutput _adaptee;
    private readonly string _id;

    protected AbstractTimeSpaceAdaptor(ITimeSpaceOutput adaptee, string id)
    {
      _adaptee = adaptee;
      _id = id;
      AdaptedOutputs = new List<IBaseAdaptedOutput>();
      Consumers = new List<IBaseInput>();
    }

    public virtual string Caption
    {
      get { return _adaptee.Caption; }
      set { _adaptee.Caption = value; }
    }

    public virtual string Description
    {
      get { return _adaptee.Description; }
      set { _adaptee.Description = value; }
    }

    public string Id
    {
      get { return _id; }
    }

    public virtual IValueDefinition ValueDefinition
    {
      get { return _adaptee.ValueDefinition; }
    }

    public virtual IBaseLinkableComponent Component
    {
      get { return _adaptee.Component; }
    }

    public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

    protected void OnItemChanged(string message)
    {
      if (ItemChanged != null)
        ItemChanged.Invoke(this, new ExchangeItemChangeEventArgs(this, message));
    }

    public virtual IList<IBaseInput> Consumers
    {
      get; private set;
    }

    public virtual void AddConsumer(IBaseInput consumer)
    {
      Consumers.Add(consumer);
    }

    public virtual void RemoveConsumer(IBaseInput consumer)
    {
      Consumers.Remove(consumer);
    }

    public IList<IBaseAdaptedOutput> AdaptedOutputs
    {
      get;
      private set;
    }

    public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      AdaptedOutputs.Add(adaptedOutput);
    }

    public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      AdaptedOutputs.Remove(adaptedOutput);
    }

    public virtual ITimeSpaceValueSet Values
    {
      get { return _adaptee.Values; }
    }

    public virtual ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      return _adaptee.GetValues(querySpecifier);
    }

    IBaseValueSet IBaseOutput.Values
    {
      get { return Values; }
    }

    IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
    {
      return GetValues(querySpecifier);
    }

      public void Initialize()
      {
          throw new NotImplementedException();
      }

      public virtual IList<IArgument> Arguments
    {
      get { return (new List<IArgument>()); }
    }

    public IBaseOutput Adaptee
    {
      get { return _adaptee; }
    }

    public virtual void Refresh()
    {
    }

    public virtual ITimeSet TimeSet
    {
      get { return _adaptee.TimeSet; }
    }

    public virtual ISpatialDefinition SpatialDefinition
    {
      get { return _adaptee.SpatialDefinition; }
    }
  }
}