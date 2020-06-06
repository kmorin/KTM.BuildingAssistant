namespace KTM.BuildingAssistant.Common
{
  public interface IApiConnector
  {
    bool IsServiceAvailable();
    object Get(string resource);
    object Post(string resource, object body);
  }
}
