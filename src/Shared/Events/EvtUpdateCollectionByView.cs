using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Common;

namespace KTM.BuildingAssistant.Revit.Events
{
  public class EvtUpdateCollectionByView : IExternalEventHandler
  {
    private View _activeView;

    public EvtUpdateCollectionByView(View activeView) {
      _activeView = activeView;
    }
    public void Execute(UIApplication app) {
      ElementId viewId = _activeView.Id;
      if (GlobalCollections.FamilyInstancesCachedByView.ContainsKey(viewId.IntegerValue)) {
        //set collection to view

      }
      else {
        //create and store filtered elem collection in memo cache
      }
    }

    public string GetName() {
      return nameof(EvtUpdateCollectionByView);
    }
  }
}