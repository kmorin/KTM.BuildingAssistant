using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace KTM.BuildingAssistant.Common
{
  public class Logger
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

    public static void Log(string message, Exception ex = null) {
      BasicConfigurator.Configure();
      
      if (ex == null) {
        log.Info(message);
      }
      else {
        log.Error($"{message} -- {ex}");
      }
    }
  }
}
