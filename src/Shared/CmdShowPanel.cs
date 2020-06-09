using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Settings = KTM.BuildingAssistant.Revit.Properties.Settings;

namespace KTM.BuildingAssistant.Revit
{
  [Transaction(TransactionMode.Manual)]
  public class CmdShowPanel : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
      
      DockablePaneId dockablePaneId = GetDockablePaneId();
      DockablePane pane = commandData.Application.GetDockablePane(dockablePaneId);

      if (pane.IsShown())
        pane.Hide();
      else
        pane.Show();

      return Result.Succeeded;

    }

    private DockablePaneId GetDockablePaneId() {
      Guid dockableGuid = new Guid(Settings.Default.DockableGuid);
      var dockPaneId = new DockablePaneId(dockableGuid);
      return dockPaneId;
    }
  }
}