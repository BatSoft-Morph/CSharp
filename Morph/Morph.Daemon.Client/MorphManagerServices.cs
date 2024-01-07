using System;
using Morph.Endpoint;
using Morph.Params;
using Morph.Sequencing;

namespace Morph.Daemon.Client
{
  public class MorphManagerServices : DaemonClient
  {
    #region Internal

    internal MorphManagerServices(TimeSpan DefaultTimeout)
      : base("Morph.Services", DefaultTimeout)
    {
    }

    private MorphService RegisterLocalService(string serviceName, bool accessLocal, bool accessRemote, MorphApartmentFactory apartmentFactory)
    {
      MorphService Service = MorphServices.Register(serviceName, apartmentFactory);
      //  Try to register the service with the Morph Daemon
      try
      { //  Tell the Morph daemon to redirect service requests to here
        ServletProxy.CallMethod("start", new object[] { serviceName, accessLocal, accessRemote });
        //  Done
        return Service;
      }
      catch
      { //  If that fails, then tidy up
        MorphServices.Deregister(serviceName);
        throw;
      }
    }

    #endregion

    public MorphService startServiceShared(string serviceName, bool accessLocal, bool accessRemote, object defaultObject, InstanceFactories instanceFactories)
    {
      MorphApartmentFactory apartmentFactory = new MorphApartmentFactoryShared(defaultObject, instanceFactories);
      return RegisterLocalService(serviceName, accessLocal, accessRemote, apartmentFactory);
    }

    public MorphService startServiceSessioned(string serviceName, bool accessLocal, bool accessRemote, DefaultServletObjectFactory defaultObjectFactory, InstanceFactories instanceFactories, TimeSpan timeout, SequenceLevel sequenceLevel)
    {
      MorphApartmentFactory apartmentFactory = new MorphApartmentFactorySession(defaultObjectFactory, instanceFactories, timeout, sequenceLevel);
      return RegisterLocalService(serviceName, accessLocal, accessRemote, apartmentFactory);
    }

    public MorphService startServiceSessioned(string serviceName, bool accessLocal, bool accessRemote, MorphApartmentFactorySession apartmentFactory)
    {
      return RegisterLocalService(serviceName, accessLocal, accessRemote, apartmentFactory);
    }

    public void stopService(string serviceName)
    {
      try
      {
        ServletProxy.CallMethod("stop", new object[] { serviceName });
      }
      finally
      {
        MorphServices.Deregister(serviceName);
      }
    }

    public void stopService(MorphService service)
    {
      try
      {
        ServletProxy.CallMethod("stop", new object[] { service.Name });
      }
      finally
      {
        service.Deregister();
      }
    }

    public DaemonService[] listServices()
    {
      return (DaemonService[])ServletProxy.CallMethod("listServices", null);
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

  public struct DaemonService
  {
    public string serviceName;
    public bool accessLocal;
    public bool accessRemote;
  }
}