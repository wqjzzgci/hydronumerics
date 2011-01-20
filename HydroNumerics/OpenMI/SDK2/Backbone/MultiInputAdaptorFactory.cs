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
using HydroNumerics.OpenMI.Sdk.Backbone.Generic;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;
using HydroNumerics.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// Common interface for adaptors that the <see cref="MultiInputAdaptor"/> can utilize
  /// </summary>
  /// <remarks>
  /// this interface is required in order to use the "optimized" version of the
  /// element mapping (reusing of target elementset).
  /// </remarks>
  public interface ITimeSpaceOutputAdder : ITimeSpaceOutput
  {
    ///<summary>
    /// Version of <see cref="ITimeSpaceOutput.GetValues"/> where the result instead
    /// is added to the <paramref name="targetSet"/>, instead of returning a new 
    /// <see cref="ITimeSpaceValueSet"/>.
    /// <para>
    /// It is assumed the the <paramref name="targetSet"/> has the correct size, matching
    /// the <paramref name="querySpecifier"/>
    /// </para>
    /// </summary>
    void GetValues(ITimeSpaceValueSet<double> targetSet, IBaseExchangeItem querySpecifier);
  }

  /// <summary>
  /// A factory for creating adapted outputs that connects to the same input.
  /// <para>
  /// It also includes all the spatial mappings of the <see cref="SpatialAdaptedOutputFactory"/>
  /// </para>
  /// </summary>
  public class MultiInputAdaptorFactory : IAdaptedOutputFactory
  {
    private readonly IBaseLinkableComponent _component;

    /// <summary>
    /// List of already created <see cref="MultiInputAdaptor"/>s and which <see cref="IBaseInput"/> it adapts.
    /// </summary>
    private readonly Dictionary<IBaseInput, MultiInputAdaptor> _existingMultiInputAdaptors = new Dictionary<IBaseInput, MultiInputAdaptor>();

    /// <summary>
    /// The <see cref="SpatialAdaptedOutputFactory"/> used for creating element mapping adaptors.
    /// </summary>
    private readonly SpatialAdaptedOutputFactory _spatialAdaptorFactory = new SpatialAdaptedOutputFactory(string.Empty);

    /// <summary>
    /// Id of the identity adaptor, i.e. the adaptor that does nothing to data.
    /// </summary>
    private static readonly Identifier _identityAdaptor =
        new Identifier("IdentityAdaptedOutput")
            {
              Caption = "Identity adapted output",
              Description = "An adapted output that can be used when the source and the target element set exactly matches"
            };

    public MultiInputAdaptorFactory(IBaseLinkableComponent component)
    {
      _component = component;
      Id = "MultiInputAdaptorFactory";
      Caption = "MultiInputAdaptorFactory";
      Description = "A factory for creating adapted outputs, when connecting more than one output to a single input";
    }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string Id { get; private set; }

    /// <summary>
    /// List of already created <see cref="MultiInputAdaptor"/>s and which <see cref="IBaseInput"/> it adapts.
    /// </summary>
    public Dictionary<IBaseInput, MultiInputAdaptor> ExistingMultiInputAdaptors
    {
      get { return _existingMultiInputAdaptors; }
    }

    public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
    {
      if (target.Component != _component)
        return (new IIdentifiable[0]);

      List<IIdentifiable> res = new List<IIdentifiable>();

      // TODO: Implement some criteria here
      //if (adaptee and target spatialDefintion equals)
      //{

      res.Add(_identityAdaptor);
      //}
      //else
      //{
      res.AddRange(_spatialAdaptorFactory.GetAvailableAdaptedOutputIds(adaptee, target));
      //}


      return (res.ToArray());
    }

    public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
    {
      // TODO: Some usefull criteria here
      if (true)
      {
        // If a MultiInputAdaptor already exists for the target, get that one, otherwise create a new.
        MultiInputAdaptor multiInputAdaptor;
        if (!ExistingMultiInputAdaptors.TryGetValue(target, out multiInputAdaptor))
        {
          multiInputAdaptor = new MultiInputAdaptor("SomeId")
                                  {
                                    SpatialDefinition = ((ITimeSpaceInput)target).SpatialDefinition
                                  };
          multiInputAdaptor.AddConsumer(target);
          ExistingMultiInputAdaptors.Add(target, multiInputAdaptor);
        }

        if (adaptedOutputId == _identityAdaptor)
        {
          // Identity adaptor
          IBaseAdaptedOutput adaptedOutput = multiInputAdaptor.CreateChildAdaptor(adaptee);
          return (adaptedOutput);
        }
        else
        {
          // ElementMapping adaptor
          IBaseAdaptedOutput adaptedOutput = _spatialAdaptorFactory.CreateAdaptedOutput(adaptedOutputId, adaptee, target);
          if (adaptedOutput != null)
          {
            multiInputAdaptor.Adaptees.Add((ITimeSpaceOutputAdder) adaptedOutput);
            return (adaptedOutput);
          }
        }
      }

      return (null);

    }
  }


  /// <summary>
  /// A MultiInputAdaptor can connect more than one output to a single input.
  /// <para>
  /// For each output, use <see cref="CreateChildAdaptor"/> to create a <see cref="ChildIdentityAdaptor"/>,
  /// which is added as an adapted-output of the output.
  /// </para>
  /// <para>
  /// Instead of using the <see cref="CreateChildAdaptor"/>, any <see cref="ITimeSpaceOutputAdder"/> can
  /// be used. It must be added to the <see cref="Adaptees"/> list manually (the <see cref="CreateChildAdaptor"/>
  /// automatically adds one there).
  /// </para>
  /// </summary>
  /// <remarks>
  /// Since this adapted output adapts more than one output, properties as <see cref="Component"/>,
  /// <see cref="Values"/> etc. are not well defined, and dummy values are returned.
  /// </remarks>
  public class MultiInputAdaptor : ITimeSpaceAdaptedOutput
  {
    // TODO (JG): Decide: Leave values, timeset and elementset at null, or some default value?

    private readonly string _id;

    public MultiInputAdaptor(string id)
    {
      _id = id;
    }

    public IBaseAdaptedOutput CreateChildAdaptor(IBaseOutput adaptee)
    {
      var adaptedOutput = new ChildIdentityAdaptor(this, (ITimeSpaceOutput)adaptee);
      Adaptees.Add(adaptedOutput);
      return (adaptedOutput);
    }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string Id { get { return _id; } }

    public IValueDefinition ValueDefinition
    {
      // TODO (JG): Assume they all are the same, and use the first child?
      get;
      set;
    }

    public IBaseLinkableComponent Component
    {
      // TODO (JG): More than one component, no simple answer 
      get { return (null); }
    }

    public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

    private List<ITimeSpaceInput> _consumers;

    public virtual IList<IBaseInput> Consumers
    {
      get
      {
        if (_consumers == null)
        {
          _consumers = new List<ITimeSpaceInput>();
        }
        return new ListWrapper<ITimeSpaceInput, IBaseInput>(_consumers.AsReadOnly());
      }
    }

    public virtual void AddConsumer(IBaseInput consumer)
    {
      ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
      if (timeSpaceConsumer == null)
        throw new ArgumentException("Must be a ITimeSpaceInput - may need to add adaptor");

      // Create list of consumers
      if (_consumers == null)
      {
        _consumers = new List<ITimeSpaceInput>();
      }

      // consumer should not be already added
      if (_consumers.Contains(timeSpaceConsumer))
      {
        throw new Exception("consumer \"" + consumer.Caption +
            "\" has already been added to \"" + Caption);
      }

      // Check if this consumer can be added
      if (!ExchangeItemHelper.ConsumerCanBeAdded(_consumers, timeSpaceConsumer))
      {
        throw new Exception("consumer \"" + consumer.Caption +
            "\" can not be added to \"" + Caption +
            "\", because it is incompatible with existing consumers");
      }

      _consumers.Add(timeSpaceConsumer);
      if (consumer.Provider != this)
      {
        consumer.Provider = this;
      }
    }

    public virtual void RemoveConsumer(IBaseInput consumer)
    {
      ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
      if (timeSpaceConsumer == null || _consumers == null || !_consumers.Contains(timeSpaceConsumer))
      {
        throw new Exception("consumer \"" + consumer.Caption +
            "\" can not be removed from \"" + Caption +
            "\", because it was not added");
      }
      _consumers.Remove(timeSpaceConsumer);
      consumer.Provider = null;
    }

    readonly List<IBaseAdaptedOutput> _adaptedOutputs = new List<IBaseAdaptedOutput>();

    public IList<IBaseAdaptedOutput> AdaptedOutputs
    {
      get { return (_adaptedOutputs); }
    }

    public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      throw new NotImplementedException();
    }

    public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      throw new NotImplementedException();
    }

    public ITimeSpaceValueSet Values
    {
      // TODO (JG): decide what to return here.
      get { return new TimeSpaceValueSet<double>(); }
    }

    public ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("Must be an ITimeSpaceExchangeItem, add an adaptor", "querySpecifier");

      if (_children.Count == 0)
        return (null); // TODO: Or throw an exception?
      
      TimeSpaceValueSet<double> resultSet = ElementMapper.CreateResultValueSet(timeSpaceQuery.TimeSet.Times.Count, this.ElementSet().ElementCount);

      // Get values from all adaptees/children, and add them together
      for (int i = 0; i < _children.Count; i++)
      {
        _children[i].GetValues(resultSet, querySpecifier);
      }
      return (resultSet);
    }

    IBaseValueSet IBaseOutput.Values
    {
      get { return Values; }
    }

    IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
    {
      return GetValues(querySpecifier);
    }

    public IList<IArgument> Arguments
    {
      get { return new List<IArgument>(); }
    }

    public void Initialize()
    {
    }

    public IBaseOutput Adaptee
    {
      // TODO: Nothing meaninfull to return, in case of more than one adaptee...
      get { return (null); }
    }

    private readonly List<ITimeSpaceOutputAdder> _children = new List<ITimeSpaceOutputAdder>();

    public IList<ITimeSpaceOutputAdder> Adaptees
    {
      get { return (_children); }
    }


    public void Refresh()
    {
      /// Should probably not do anything. At least have to make sure
      /// that the Refresh call does not trigger an update of all other components
      /// (which in their turn will invoke refresh again)

      foreach (IBaseAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }

    public ITimeSet TimeSet
    {
      get;
      set;
    }

    public ISpatialDefinition SpatialDefinition
    {
      get;
      set;
    }

    #region Helper class

    class ChildIdentityAdaptor : ITimeSpaceAdaptedOutput, ITimeSpaceOutputAdder
    {
      private MultiInputAdaptor _parent;
      private ITimeSpaceOutput _adaptee;

      public ChildIdentityAdaptor(MultiInputAdaptor parent, ITimeSpaceOutput adaptee)
      {
        _parent = parent;
        _adaptee = adaptee;
      }

      public string Caption
      {
        get { return (_adaptee.Caption); }
        set { throw new NotSupportedException(); }
      }

      public string Description
      {
        get { return (_adaptee.Description); }
        set { throw new NotSupportedException(); }
      }

      public string Id
      {
        get { return (_adaptee.Id + "-ChildAdaptor"); }
      }

      public IValueDefinition ValueDefinition
      {
        get { return (_adaptee.ValueDefinition); }
      }

      public IBaseLinkableComponent Component
      {
        get { return (_adaptee.Component); ; }
      }

      public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

      public IList<IBaseInput> Consumers
      {
        get { return new List<IBaseInput>(); }
      }

      public void AddConsumer(IBaseInput consumer)
      {
        throw new NotSupportedException();
      }

      public void RemoveConsumer(IBaseInput consumer)
      {
        throw new NotSupportedException();
      }

      public IList<IBaseAdaptedOutput> AdaptedOutputs
      {
        get { return (new List<IBaseAdaptedOutput>()); }
      }

      public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
      {
        throw new NotSupportedException();
      }

      public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
      {
        throw new NotSupportedException();
      }

      public ITimeSpaceValueSet Values
      {
        get { return _adaptee.Values; }
      }

      public ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
      {
        return (_adaptee.GetValues(querySpecifier));
      }


      public void GetValues(ITimeSpaceValueSet<double> targetSet, IBaseExchangeItem querySpecifier)
      {
        // Copy values from the adaptee to the targetSet
        ITimeSpaceValueSet sourceValues = _adaptee.GetValues(querySpecifier);

        for (int i = 0; i < targetSet.TimesCount(); i++)
        {
          double[] sourceTimeValues = sourceValues.GetElementValuesForTime<double>(i);
          for (int j = 0; j < targetSet.ElementCount(); j++)
          {
            targetSet.Values2D[i][j] += sourceTimeValues[j];
          }
        }
      }

      IBaseValueSet IBaseOutput.Values
      {
        get { return Values; }
      }

      IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
      {
        return GetValues(querySpecifier);
      }

      public IList<IArgument> Arguments
      {
        get { return new List<IArgument>(); }
      }

      public void Initialize()
      {
      }

      public IBaseOutput Adaptee
      {
        get { return (_adaptee); }
      }

      public void Refresh()
      {
        _parent.Refresh();
      }

      public ITimeSet TimeSet
      {
        get { return (_adaptee.TimeSet); }
      }

      public ISpatialDefinition SpatialDefinition
      {
        get { return (_adaptee.SpatialDefinition); }
      }
    }
    #endregion

  }

}