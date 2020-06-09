using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Common;
using KTM.BuildingAssistant.Revit.Properties;

namespace KTM.BuildingAssistant.Revit
{
  public class AppMain : IExternalApplication
  {
    private UIControlledApplication _uiControlledApp;
    private RibbonPanel _panel;
    private readonly string _tabName = "Building Assistant";
    private readonly string _panelName = "BA";
    private DockableBuildingAssistant _dockableWindow;

    public Result OnShutdown(UIControlledApplication application) {
      //TODO: clear session and dump all files if needed.
      return Result.Succeeded;
    }

    public Result OnStartup(UIControlledApplication application) {
      _uiControlledApp = application;

      if (Bootstrap()) {
        return Result.Succeeded;
      }
      else {
        return Result.Failed;
      }
    }

    private bool Bootstrap() {
      try {
        //get current lang
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(CultureInfo.InstalledUICulture.Name);

        //init session
        InitSession();

        //make ribbon
        BuildRibbon();

        //make buttons
        List<PushButtonData> buttons = BuildButtons();

        AddButtonsToRibbon(buttons);

        //init dockable pane
        RegisterDockablePane();
      }
      catch (Exception ex) {
        Logger.Log(nameof(Bootstrap), ex);
        return false;
      }
      return true;
    }
    private void InitSession() {
      //TODO: session manager
    }
    /// <summary>
    /// Creates the Ribbon Tab and Panel.
    /// </summary>
    private void BuildRibbon() {
      try {
        _uiControlledApp.CreateRibbonTab(_tabName);
      }
      catch {
        //do nothing, bad api design.
      }

      _panel = _uiControlledApp.CreateRibbonPanel(_tabName, _panelName);
    }

    /// <summary>
    /// Create the buttons to append to the ribbon panel
    /// </summary>
    private List<PushButtonData> BuildButtons() {
      string dllPath = Assembly.GetExecutingAssembly().Location;
      string m_namespace = typeof(AppMain).Namespace;

      var listOfButtons = new List<PushButtonData> {
        //ShowPanelButton
        CreatePushButtonData(
          dllPath: dllPath,
          className: nameof(CmdShowPanel),
          buttonText: "BuildingAssistant",
          image16: "save_16.png",
          image32: "save_32.png",
          toolTip: "tooltip",
          nameSpace: m_namespace
          )
      };

      return listOfButtons;
    }

    /// <summary>
    /// Adds all pushbuttons to ribbon
    /// </summary>
    private void AddButtonsToRibbon(List<PushButtonData> buttons) {
      foreach (PushButtonData pbData in buttons) {
        _panel.AddItem(pbData);
      }
    }

    /// <summary>
    /// Create a PushButtonData object to add to a ribbon panel.
    /// </summary>
    /// <param name="dllPath"></param>
    /// <param name="className"></param>
    /// <param name="buttonText"></param>
    /// <param name="image16"></param>
    /// <param name="image32"></param>
    /// <param name="toolTip"></param>
    /// <param name="nameSpace"></param>
    /// <param name="availabilityClassName"></param>
    /// <returns></returns>
    private PushButtonData CreatePushButtonData(string dllPath,
      string className,
      string buttonText,
      string image16,
      string image32,
      string toolTip,
      string nameSpace,
      string availabilityClassName = "") {

      if (!File.Exists(dllPath)) {
        throw new FileNotFoundException();
      }

      var pbd = new PushButtonData(
        name: className,
        text: buttonText,
        assemblyName: dllPath,
        className: $"{nameSpace}.{className}") {
        Image = LoadPngImgSource(image16),
        LargeImage = LoadPngImgSource(image32),
        ToolTip = toolTip
      };

      if (!string.IsNullOrWhiteSpace(availabilityClassName)) {
        pbd.AvailabilityClassName = $"{nameSpace}.{availabilityClassName}";
      }

      return pbd;
    }

    /// <summary>
    /// Load the PNG image from manifest resouce to a usable ImageSource.
    /// NOTE: Must be EmbeddedResource
    /// NOTE: images for Revit must be 96dpi.
    /// </summary>
    /// <param name="imageName"></param>
    /// <returns></returns>
    private System.Windows.Media.ImageSource LoadPngImgSource(string imageName) {
      string ns = typeof(AppMain).Namespace;
      string fullImageName = $"{ns}.img.{imageName}";

      var assembly = Assembly.GetExecutingAssembly();
      Stream icon = assembly.GetManifestResourceStream(fullImageName);

      if (icon == null) {
        throw new FileNotFoundException();
      }

      var decoder = new PngBitmapDecoder(bitmapStream: icon,
        createOptions: BitmapCreateOptions.PreservePixelFormat,
        cacheOption: BitmapCacheOption.Default);

      return decoder.Frames[0];

    }

    private void RegisterDockablePane() {
      var dockableGuid = new Guid(Settings.Default.DockableGuid);
      var dockablePaneId = new DockablePaneId(dockableGuid);

      _uiControlledApp.RegisterDockablePane(dockablePaneId, "Building Assistant", GetDockablePane());
    }

    private IDockablePaneProvider GetDockablePane() {
      if (_dockableWindow == null) {
        _dockableWindow = new DockableBuildingAssistant();
      }
      return _dockableWindow;
    }
  }
}
