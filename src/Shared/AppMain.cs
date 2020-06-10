using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using KTM.BuildingAssistant.Common;
using KTM.BuildingAssistant.Revit.Events;
using Settings = KTM.BuildingAssistant.Revit.Properties.Settings;

namespace KTM.BuildingAssistant.Revit
{
  public class AppMain : IExternalApplication
  {
    public bool IsPaneVisible => GetIsPaneVisible();

    private UIControlledApplication _uiControlledApp;
    private RibbonPanel _panel;
    private readonly string _tabName = "Building Assistant";
    private readonly string _panelName = "BA";
    private DockableBuildingAssistant _dockableWindow;
    private ExternalEvent _collectInstancesEvent;

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

        //external event handler
        _collectInstancesEvent = ExternalEvent.Create(new EvtCollectInstances());

        //doc changed event handler
        _uiControlledApp.ControlledApplication.DocumentOpened += DocOpenedEventHandler;
        _uiControlledApp.ControlledApplication.DocumentChanged += DocChangedEventHandler;
        _uiControlledApp.ViewActivated += ViewActivatedEventHandler;
      }
      catch (Exception ex) {
        Logger.Log(nameof(Bootstrap), ex);
        return false;
      }
      return true;
    }

    private void ViewActivatedEventHandler(object sender, ViewActivatedEventArgs e) {
        var viewEvent = ExternalEvent.Create(new EvtUpdateCollectionByView(e.CurrentActiveView));
        viewEvent.Raise();
    }

    private void DocChangedEventHandler(object sender, DocumentChangedEventArgs e) {
      
      //build data of added/deleted elements to update cache.
      var dataTable = new Dictionary<int, List<ElementId>> {
        {-1,e.GetDeletedElementIds().ToList() },
        {1,e.GetAddedElementIds().ToList() }
      };
      
      var updatedEvent = ExternalEvent.Create(new EvtUpdateCollectionInstances(dataTable));

      //update cache per view
      updatedEvent.Raise();
    }

    private void DocOpenedEventHandler(object sender, DocumentOpenedEventArgs e) {
      //on doc opned, trigger external event.
      _collectInstancesEvent.Raise();
    }

    private void InitSession() {
      //TODO: session manager

      //TODO: setup session cache for each document in temp

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
      DockablePaneId dockablePaneId = GetDockablePaneId();

      _uiControlledApp.RegisterDockablePane(dockablePaneId, "Building Assistant", GetDockablePane());
    }

    private static DockablePaneId GetDockablePaneId() {
      Guid dockableGuid = new Guid(Settings.Default.DockableGuid);
      return new DockablePaneId(dockableGuid);
    }

    private IDockablePaneProvider GetDockablePane() {
      if (_dockableWindow == null) {
        _dockableWindow = new DockableBuildingAssistant();
      }
      return _dockableWindow;
    }

    private bool GetIsPaneVisible() {
      return _dockableWindow != null && _dockableWindow.IsVisible;
    }
  }
}
