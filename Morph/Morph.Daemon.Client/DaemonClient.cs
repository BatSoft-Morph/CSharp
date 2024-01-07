using System;
using System.Net;
using Morph.Endpoint;
using Morph.Internet;
using Morph.Params;

namespace Morph.Daemon.Client
{
  public class DaemonClient
  {
    protected DaemonClient(string ServiceName, TimeSpan DefaultTimeout)
    {
      IPEndPoint DaemonEndPoint = new IPEndPoint(IPAddress.Loopback, LinkInternet.MorphPort);
      _ServletProxy = MorphApartmentProxy.ViaEndPoint(ServiceName, DefaultTimeout, InstanceFactory, DaemonEndPoint).DefaultServlet;
    }

    #region Instance factory

    static private InstanceFactories _InstanceFactory = new DaemonInstanceFactory();
    public static InstanceFactories InstanceFactory
    {
      get { return _InstanceFactory; }
      set { _InstanceFactory = value; }
    }

    private class DaemonInstanceFactory : InstanceFactories
    {
      public DaemonInstanceFactory()
        : base()
      {
        //  Struct factory
        InstanceFactoryStruct StructFactory = new InstanceFactoryStruct();
        StructFactory.AddStructType(typeof(DaemonService));
        StructFactory.AddStructType(typeof(DaemonStartup));
        Add(StructFactory);
        //  Array factory
        InstanceFactoryArray ArrayFactory = new InstanceFactoryArray();
        ArrayFactory.AddArrayElemType(typeof(DaemonService));
        ArrayFactory.AddArrayElemType(typeof(DaemonStartup));
        Add(ArrayFactory);
      }
    }

    #endregion

    private ServletProxy _ServletProxy;
    protected ServletProxy ServletProxy
    {
      get { return _ServletProxy; }
    }
  }
}