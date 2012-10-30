using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMI.Standard;
using DHI.OpenMI.Backbone;
using DHI.OpenMI.Spatial;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.OpenMI.MikeShe
{
  public class MikeSheLinkableComponent2:ILinkableComponent
  {

    private DHI.OpenMI.MikeShe.MSheWMLinkableComponent MsheOrg = new DHI.OpenMI.MikeShe.MSheWMLinkableComponent();

    private List<IOutputExchangeItem> OutputExchangeItems = new List<IOutputExchangeItem>();
    private Dictionary<string, ILink> AlteredLinks = new Dictionary<string, ILink>();
    Dictionary<string, ElementMapper> mappers = new Dictionary<string, ElementMapper>();

    private List<int> InnerIdeces = new List<int>();


     public void Initialize(IArgument[] properties)
    {
      MsheOrg.Initialize(properties);

      string mshefilename = System.IO.Path.Combine(properties.First(a => a.Key == "SetupPath").Value, properties.First(a => a.Key == "SetupFileName").Value);

      Model mshe = new Model(mshefilename);

      List<double> InnerXValues= new List<double>();
      List<double> InnerYValues = new List<double>();

      int k=0;
        for(int j=0;j<mshe.GridInfo.NumberOfColumns;j++)
          for (int i = 0; i < mshe.GridInfo.NumberOfRows; i++)
          {
          if (mshe.GridInfo.ModelDomainAndGrid.Data[i, j] == 1)
          {
            InnerXValues.Add(mshe.GridInfo.GetXCenter(j));
            InnerYValues.Add(mshe.GridInfo.GetYCenter(i));
            InnerIdeces.Add(k);
            k++;
          }
          else if (mshe.GridInfo.ModelDomainAndGrid.Data[i, j] == 2)
            k++;
        }


      DHI.OpenMI.MikeShe.BaseGrid NewBaseGrid = new DHI.OpenMI.MikeShe.BaseGrid(InnerXValues.Count, InnerXValues.ToArray(), InnerYValues.ToArray(), mshe.GridInfo.GridSize);

      mshe.Dispose();
      for (int i = 0; i < MsheOrg.OutputExchangeItemCount; i++)
      {
        var org = MsheOrg.GetOutputExchangeItem(i);

        if (org.ElementSet.ID == "BaseGrid")
        {
          OutputExchangeItem onew = new OutputExchangeItem();
          onew.Quantity = org.Quantity;
          onew.ElementSet = NewBaseGrid;
          for (int j = 0; j < org.DataOperationCount; j++)
          {
            onew.AddDataOperation(org.GetDataOperation(j));
          }
          OutputExchangeItems.Add(onew);
        }
        else
        {
          OutputExchangeItems.Add(org);
        }
      }
    }

    public IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
    {
      return OutputExchangeItems[outputExchangeItemIndex];
    }

    public int OutputExchangeItemCount
    {
      get { return OutputExchangeItems.Count; }
    }

    public IValueSet GetValues(ITime time, string linkID)
    {

      var values = MsheOrg.GetValues(time, linkID);
      var link = AlteredLinks[linkID];

      if (link.SourceElementSet.ID == "BaseGrid")
      {
        ScalarSet scinner = new ScalarSet(values as IScalarSet);

        List<double> InnerValues = new List<double>();

        foreach (int i in InnerIdeces)
          InnerValues.Add(((ScalarSet)values).data[i]);

        scinner.data = InnerValues.ToArray();

        if (mappers.ContainsKey(linkID))
        {
          return mappers[linkID].MapValues(scinner);
        }
        return scinner;
      }
      return values;
    }




    #region ILinkableComponent Members

    public void AddLink(ILink link)
    {
      DHI.OpenMI.Backbone.Link locallink = new Link();
      AlteredLinks.Add(link.ID, locallink);

      //Mikeshe is the source. Create new local dataoperations.
      if (link.SourceComponent == this)
      {
        locallink.SourceComponent = MsheOrg;
        for (int i = 0; i < link.DataOperationsCount; i++)
        {
          ElementMapper em = new ElementMapper();
          string desc="";
          for (int j=0;j<link.GetDataOperation(i).ArgumentCount;j++)
            if (link.GetDataOperation(i).GetArgument(j).Key=="Description")
              desc = link.GetDataOperation(i).GetArgument(j).Value;

          em.Initialise(desc, link.SourceElementSet, link.TargetElementSet);
          mappers.Add(link.ID, em);
        }
      }
      else
        locallink.SourceComponent = link.SourceComponent;

      //Mike she is the target. Copy the dataoperations
      if (link.TargetComponent == this)
      {
        locallink.TargetComponent = MsheOrg;
        for (int i = 0; i < link.DataOperationsCount; i++)
          locallink.AddDataOperation(link.GetDataOperation(i));
      }
      else
        locallink.TargetComponent = link.TargetComponent;

      locallink.ID = link.ID;
      locallink.SourceElementSet = link.SourceElementSet;
      locallink.SourceQuantity = link.SourceQuantity;

      locallink.TargetElementSet = link.TargetElementSet;
      locallink.TargetQuantity = link.TargetQuantity;



     

      MsheOrg.AddLink(locallink);

    }

    public string ComponentDescription
    {
      get { return MsheOrg.ComponentDescription; }
    }

    public string ComponentID
    {
      get { return MsheOrg.ComponentID; }
    }

    public void Dispose()
    {
      MsheOrg.Dispose();
    }

    public ITimeStamp EarliestInputTime
    {
      get { return MsheOrg.EarliestInputTime; }
    }

    public void Finish()
    {
      MsheOrg.Finish();
    }

    public IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
    {
      return MsheOrg.GetInputExchangeItem(inputExchangeItemIndex);
    }




    public int InputExchangeItemCount
    {
      get { return MsheOrg.InputExchangeItemCount; }
    }

    public string ModelDescription
    {
      get { return MsheOrg.ModelDescription; }
    }

    public string ModelID
    {
      get { return MsheOrg.ModelID; }
    }


    public void Prepare()
    {
      MsheOrg.Prepare();
    }

    public void RemoveLink(string linkID)
    {
      MsheOrg.RemoveLink(linkID);
    }

    public ITimeSpan TimeHorizon
    {
      get { return MsheOrg.TimeHorizon; }
    }

    public string Validate()
    {
      return MsheOrg.Validate();
    }

    #endregion

    #region IPublisher Members

    public EventType GetPublishedEventType(int providedEventTypeIndex)
    {
      return MsheOrg.GetPublishedEventType(providedEventTypeIndex);
    }

    public int GetPublishedEventTypeCount()
    {
      return MsheOrg.GetPublishedEventTypeCount();
    }

    public void SendEvent(IEvent Event)
    {
      MsheOrg.SendEvent(Event);
    }

    public void Subscribe(IListener listener, EventType eventType)
    {
      MsheOrg.Subscribe(listener, eventType);
    }

    public void UnSubscribe(IListener listener, EventType eventType)
    {
      MsheOrg.UnSubscribe(listener, eventType);
    }

    #endregion
  }
}
