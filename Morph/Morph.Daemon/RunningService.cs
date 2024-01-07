using System.Collections;
using Morph.Internet;
using Morph.Endpoint;

namespace Morph.Daemon
{
  public interface RunningService
  {
    string ServiceName
    { get; }

    bool AccessLocal
    { get; }

    bool AccessRemote
    { get; }
  }

  public class LocalService : RunningService
  {
    public LocalService(Service Service, bool AccessLocal, bool AccessRemote)
    {
      _Service = Service;
      _AccessLocal = AccessLocal;
      _AccessRemote = AccessRemote;
    }

    private Service _Service;
    public Service Service
    {
      get { return _Service; }
    }

    #region RunningService

    public string ServiceName
    { get { return _Service.Name; } }

    private bool _AccessLocal;
    public bool AccessLocal
    { get { return _AccessLocal; } }

    private bool _AccessRemote;
    public bool AccessRemote
    { get { return _AccessRemote; } }

    #endregion
  }

  public class ConnectedService : RunningService
  {
    public ConnectedService(Connection Connection, string ServiceName, bool AccessLocal, bool AccessRemote)
    {
      _Connection = Connection;
      _ServiceName = ServiceName;
      _AccessLocal = AccessLocal;
      _AccessRemote = AccessRemote;
    }

    private Connection _Connection;
    public Connection Connection
    { get { return _Connection; } }

    #region RunningService

    private string _ServiceName;
    public string ServiceName
    { get { return _ServiceName; } }

    private bool _AccessLocal;
    public bool AccessLocal
    { get { return _AccessLocal; } }

    private bool _AccessRemote;
    public bool AccessRemote
    { get { return _AccessRemote; } }

    #endregion
  }

  static public class RunningServices
  {
    static private Hashtable _Services = new Hashtable();

    static public void Register(RunningService Service)
    {
      lock (_Services)
      {
        object ServiceObj = _Services[Service.ServiceName];
        if (ServiceObj != null)
          throw new EMorphDaemon("Service \"" + Service.ServiceName + "\" is already registered.");
        _Services[Service.ServiceName] = Service;
      }
    }

    static public void Deregister(string ServiceName)
    {
      lock (_Services)
      {
        object ServiceObj = _Services[ServiceName];
        if (ServiceObj != null)
        {
          RunningService Service = (RunningService)ServiceObj;
          //  Todo: Verify that sender has the right to remove the service
          _Services.Remove(ServiceName);
        }
      }
    }

    static public RunningService Find(string ServiceName)
    {
      return (RunningService)_Services[ServiceName];
    }

    static public RunningService[] List()
    {
      RunningService[] Services;
      lock (_Services)
      {
        Services = new RunningService[_Services.Count];
        _Services.CopyTo(Services, 0);
      }
      return Services;
    }
  }
}