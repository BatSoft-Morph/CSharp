using System;

namespace Morph.Daemon.Client
{
  public class MorphManagerStartups : DaemonClient
  {
    public MorphManagerStartups(TimeSpan DefaultTimeout)
      : base("Morph.Startup", DefaultTimeout)
    {
    }

    public void refresh()
    {
      ServletProxy.SendMethod("refresh", null);
    }

    /**
     * Timeout in in seconds
     */
    public void add(string serviceName, string fileName, string parameters, int timeout)
    {
      ServletProxy.CallMethod("add", new object[] { serviceName, fileName, parameters, timeout });
    }

    public void remove(string serviceName)
    {
      ServletProxy.CallMethod("remove", new object[] { serviceName });
    }

    public DaemonStartup[] listServices()
    {
      return (DaemonStartup[])ServletProxy.CallMethod("listServices", null);
    }

    public void listen(DaemonServiceCallback callback)
    {
      ServletProxy.CallMethod("listen", new object[] { callback });
    }

    public void unlisten(DaemonServiceCallback callback)
    {
      ServletProxy.CallMethod("unlisten", new object[] { callback });
    }
  }

  public struct DaemonStartup
  {
    public string serviceName;
    public string fileName;
    public int timeout;
  }
}