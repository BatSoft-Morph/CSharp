using System;

namespace Morph.Daemon.Client
{
    public class MorphManagerStartups : DaemonClient
    {
        public MorphManagerStartups(TimeSpan defaultTimeout)
          : base("Morph.Startup", defaultTimeout)
        {
        }

        public void Refresh()
        {
            ServletProxy.SendMethod("Refresh", null);
        }

        /**
         * Timeout in in seconds
         */
        public void Add(string serviceName, string fileName, string parameters, int timeout)
        {
            ServletProxy.CallMethod("Add", new object[] { serviceName, fileName, parameters, timeout });
        }

        public void Remove(string serviceName)
        {
            ServletProxy.CallMethod("Remove", new object[] { serviceName });
        }

        public DaemonStartup[] ListServices()
        {
            return (DaemonStartup[])ServletProxy.CallMethod("ListServices", null);
        }

        public void Listen(DaemonServiceCallback callback)
        {
            ServletProxy.CallMethod("Listen", new object[] { callback });
        }

        public void Unlisten(DaemonServiceCallback callback)
        {
            ServletProxy.CallMethod("Unlisten", new object[] { callback });
        }
    }

    public struct DaemonStartup
    {
        public string serviceName;
        public string fileName;
        public int timeout;
    }
}