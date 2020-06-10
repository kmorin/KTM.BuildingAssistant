using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KTM.BuildingAssistant.Revit.Events
{
  public class EvtUpdateCollectionInstances : IExternalEventHandler
  {
    private Dictionary<int, List<ElementId>> _changedElements;

    public EvtUpdateCollectionInstances(Dictionary<int,List<ElementId>> hashMap) {
      _changedElements = hashMap;
    }
    public void Execute(UIApplication app) {
      //update the view's family instances cache

    }

    public string GetName() {
      return nameof(EvtUpdateCollectionInstances);
    }
  }
}