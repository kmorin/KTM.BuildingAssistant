using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Common;

namespace KTM.BuildingAssistant.Revit.Events
{
  public class EvtCollectInstances : IExternalEventHandler
  {
    public void Execute(UIApplication app) {
      //get open doc
      Document doc = GetOpenDocument(app);

      //cache all instances
      CollectFamilyInstances(doc);
    }
    public string GetName() {
      return nameof(EvtCollectInstances);
    }

    private void CollectFamilyInstances(Document doc) {
      FilteredElementCollector fec = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance));

      //cache items
      foreach(Element e in fec) {
        GlobalCollections.InstanceHashSet.Add(e.Id.IntegerValue);
      }
    }

    private Document GetOpenDocument(UIApplication app) {
      return app.ActiveUIDocument.Document;
    }
  }
}