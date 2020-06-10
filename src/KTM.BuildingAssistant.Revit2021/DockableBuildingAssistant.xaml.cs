using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Revit.Properties;

namespace KTM.BuildingAssistant.Revit
{
  /// <summary>
  /// Interaction logic for DockableBuildingAssistant.xaml
  /// </summary>
  public partial class DockableBuildingAssistant : Page, IDockablePaneProvider
  {
    public ControlledApplication App { get; set; }
    private Document _doc;
    public DockableBuildingAssistant() {
      InitializeComponent();
      InitializeDoc();
      RefreshViewContent();
    }

    private void RefreshViewContent() {
      if (_doc == null)
        return;
    }

    private void InitializeDoc() {
      //TODO: Set current open doc
    }

    public void SetupDockablePane(DockablePaneProviderData data) {
      data.FrameworkElement = this;
      var dockPaneProviderData = new DockablePaneProviderData();
      data.InitialState = new DockablePaneState();
      data.InitialState.DockPosition = DockPosition.Right;
      _ = GetDockablePaneId();
    }

    private DockablePaneId GetDockablePaneId() {
      Guid dockGuid = new Guid(Properties.Settings.Default.DockableGuid);
      return new DockablePaneId(dockGuid);
    }
  }
}
