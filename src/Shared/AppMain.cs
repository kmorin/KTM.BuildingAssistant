using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Autodesk.Revit.UI;
using KTM.BuildingAssistant.Common;

namespace KTM.BuildingAssistant.Revit
{
  public class AppMain : IExternalApplication
  {
    private UIControlledApplication _uiControlledApp;
    private readonly string _tabName = "Building Assistant";
    private readonly string _panelName = "BA";

    public Result OnShutdown(UIControlledApplication application) {
      //TODO: clear session and dump all files if needed.
      return Result.Succeeded;
    }

    public Result OnStartup(UIControlledApplication application) {
      _uiControlledApp = application;

      Bootstrap();

      return Result.Succeeded;
    }

    private void Bootstrap() {
      try {
        //get current lang
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(CultureInfo.InstalledUICulture.Name);

        //init session
        InitSession();

        //make ribbon
        BuildRibbon();

        //make and add buttons
      }
      catch (Exception ex) {
        Logger.Log(nameof(Bootstrap), ex);
      }
    }
    private void InitSession() {
      //TODO: session manager
    }
    private void BuildRibbon() {
      try {
        _uiControlledApp.CreateRibbonTab(_tabName);
      }
      catch {
        //do nothing, bad api design.
      }

      RibbonPanel panel = _uiControlledApp.CreateRibbonPanel(_panelName);

      string dllPath = Assembly.GetExecutingAssembly().Location;
      string m_namespace = typeof(AppMain).Namespace;

      PushButtonData pbData = CreatePushButtonData(
        dllPath: dllPath,
        className: nameof(CmdShowPanel),
        buttonText: "BuildingAssistant",
        image16: "img16.png",
        image32: "img32.png",
        toolTip: "tooltip",
        commandAvailability: $"cmdAvail");
    }

    private PushButtonData CreatePushButtonData(string dllPath,
      string className,
      string buttonText,
      string image16,
      string image32,
      string toolTip,
      string commandAvailability = "") {

      throw new NotImplementedException();
    }
  }
}
