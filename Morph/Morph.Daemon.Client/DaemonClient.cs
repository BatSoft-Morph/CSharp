using System;
using System.Net;
using Morph.Endpoint;
using Morph.Internet;
using Morph.Params;

namespace Morph.Daemon.Client
{
    public class DaemonClient
    {
        protected DaemonClient(string serviceName, TimeSpan defaultTimeout)
        {
            IPEndPoint daemonEndPoint = new IPEndPoint(IPAddress.Loopback, LinkInternet.MorphPort);
            _servletProxy = MorphApartmentProxy.ViaEndPoint(serviceName, defaultTimeout, InstanceFactory, daemonEndPoint).DefaultServlet;
        }

        #region Instance factory

        static private InstanceFactories s_instanceFactory = new DaemonInstanceFactory();
        public static InstanceFactories InstanceFactory
        {
            get => s_instanceFactory;
            set => s_instanceFactory = value;
        }

        private class DaemonInstanceFactory : InstanceFactories
        {
            public DaemonInstanceFactory()
              : base()
            {
                //  Struct factory
                InstanceFactoryStruct structFactory = new InstanceFactoryStruct();
                structFactory.AddStructType(typeof(DaemonService));
                structFactory.AddStructType(typeof(DaemonStartup));
                Add(structFactory);
                //  Array factory
                InstanceFactoryArray arrayFactory = new InstanceFactoryArray();
                arrayFactory.AddArrayElemType(typeof(DaemonService));
                arrayFactory.AddArrayElemType(typeof(DaemonStartup));
                Add(arrayFactory);
            }
        }

        #endregion

        private readonly ServletProxy _servletProxy;
        protected ServletProxy ServletProxy
        {
            get => _servletProxy;
        }
    }
}