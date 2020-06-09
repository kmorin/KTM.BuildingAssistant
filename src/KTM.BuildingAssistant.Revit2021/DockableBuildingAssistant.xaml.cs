using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Revit.Properties;

namespace KTM.BuildingAssistant.Revit
{
  /// <summary>
  /// Interaction logic for DockableBuildingAssistant.xaml
  /// </summary>
  public partial class DockableBuildingAssistant : Page, IDockablePaneProvider
  {
    public DockableBuildingAssistant() {
      InitializeComponent();
    }

    public void SetupDockablePane(DockablePaneProviderData data) {
      data.FrameworkElement = this;
      var dockPaneProviderData = new DockablePaneProviderData();
      data.InitialState = new DockablePaneState();
      data.InitialState.DockPosition = DockPosition.Right;
      _ = GetDockablePaneId();
    }

    private DockablePaneId GetDockablePaneId() {
      Guid dockGuid = new Guid(Settings.Default.DockableGuid);
      return new DockablePaneId(dockGuid);
    }
  }
}
