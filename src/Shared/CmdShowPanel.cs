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
        HidePane(pane);
      else
        ShowPane(pane);

      return Result.Succeeded;

    }

    private void HidePane(DockablePane pane) {
      pane.Hide();
    }
    private void ShowPane(DockablePane pane) {
      pane.Show();
    }

    private DockablePaneId GetDockablePaneId() {
      Guid dockableGuid = new Guid(Settings.Default.DockableGuid);
      var dockPaneId = new DockablePaneId(dockableGuid);
      return dockPaneId;
    }
  }
}