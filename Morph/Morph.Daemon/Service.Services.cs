using System.Collections.Generic;
using Morph.Base;
using Morph.Internet;
using Morph.Params;

namespace Morph.Daemon
{
  public class ServicesImpl : IMorphParameters
  {
    static internal ServiceCallbacks _ServiceCallbacks = new ServiceCallbacks();

    public void start(LinkMessage Message, string serviceName, bool accessLocal, bool accessRemote)
    {
      //  Register the service with the daemon
      RegisteredService Service = RegisteredServices.ObtainByName(serviceName);
      lock (Service)
        if (Message is LinkMessageFromIP)
        {
          Service.Running = new RegisteredRunningInternet(Service, ((LinkMessageFromIP)Message).Connection);
          Service.Running.AccessLocal = accessLocal;
          Service.Running.AccessRemote = accessRemote;
        }
        else
          throw new EMorphDaemon(GetType().Name + ".start(): Unhandled message type \"" + Message.GetType().Name + "\".");
    }

    public void stop(LinkMessage Message, string serviceName)
    {
      //  Find the service
      RegisteredService Service = RegisteredServices.FindByName(serviceName);
      if (Service == null)
        return; //  Nothing to find
      //  Stop service from IP
      if (Message is LinkMessageFromIP)
        lock (Service)
        {
          Connection connection = ((LinkMessageFromIP)Message).Connection;
          if (!Service.IsRunning)
            if (!(Service.Running is RegisteredRunningInternet) || (((RegisteredRunningInternet)Service.Running).Connection != connection))
              throw new EMorphDaemon("Caller cannot stop a service " + serviceName + " which it does not own.");
          Service.Running = null;
          return;
        }
      //  This should not happen with a full implementation
      throw new EMorphDaemon(MorphDaemonService.ServiceName_Services + ".stop(): \"" + serviceName + "\"");
    }

    public DaemonService[] listServices(LinkMessage Message)
    {
      //  List services
      RegisteredService[] AllServices = RegisteredServices.ListAll();
      //  Filter in running services
      List<DaemonService> result = new List<DaemonService>();
      for (int i = 0; i < AllServices.Length; i++)
      {
        RegisteredService Service = AllServices[i];
        lock (Service)
          if (Service.IsRunning)
          {
            DaemonService RunningService = new DaemonService();
            RunningService.serviceName = Service.Name;
            RunningService.accessLocal = Service.Running.AccessLocal;
            RunningService.accessRemote = Service.Running.AccessRemote;
            result.Add(RunningService);
          }
      }
      return result.ToArray();
    }

    public void listen(LinkMessage Message, ServiceCallback callback)
    {
      _ServiceCallbacks.Listen(callback);
    }

    public void unlisten(LinkMessage Message, ServiceCallback callback)
    {
      _ServiceCallbacks.Removed(callback);
    }
  }

  public struct DaemonService
  {
    public string serviceName;
    public bool accessLocal;
    public bool accessRemote;
  }
}