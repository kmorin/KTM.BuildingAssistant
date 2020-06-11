using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Common;

namespace KTM.BuildingAssistant.Revit.Events
{
  public class EvtUpdateCollectionInstances : IExternalEventHandler
  {
    private Dictionary<int, List<ElementId>> _changedElements;

    public EvtUpdateCollectionInstances(Dictionary<int,List<ElementId>> hashMap) {
      _changedElements = hashMap;
    }
    public void Execute(UIApplication app) {
      //update the family instances cache
      List<ElementId> addedElementIds = _changedElements[1];
      List<ElementId> deletedElementIds = _changedElements[-1];
      if(addedElementIds.Count>0) {
        AddElementIdsToCollection(addedElementIds);
      }
      if (deletedElementIds.Count > 0) {
        RemoveElementIdsFromCollection(deletedElementIds);
      }
    }

    private void RemoveElementIdsFromCollection(List<ElementId> deletedElementIds) {
      foreach(ElementId eid in deletedElementIds) {
        GlobalCollections.InstanceHashSet.Remove(eid.IntegerValue);
      }
    }

    private void AddElementIdsToCollection(List<ElementId> addedElementIds) {
      foreach(ElementId eid in addedElementIds) {
        GlobalCollections.InstanceHashSet.Add(eid.IntegerValue);
      }
    }

    public string GetName() {
      return nameof(EvtUpdateCollectionInstances);
    }
  }
}