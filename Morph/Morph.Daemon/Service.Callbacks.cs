using System.Collections.Generic;
using Morph.Endpoint;

namespace Morph.Daemon
{
  public class ServiceCallback
  {
    internal ServiceCallback(ServletProxy servletProxy)
    {
      _servletProxy = servletProxy;
    }

    private readonly ServletProxy _servletProxy;

    public void Added(string serviceName)
    {
      _servletProxy.SendMethod("Added", new object[] { serviceName });
    }

    public void Removed(string serviceName)
    {
      _servletProxy.SendMethod("Removed", new object[] { serviceName });
    }
  }

  public class ServiceCallbacks
  {
    private readonly List<ServiceCallback> _callbacks = new List<ServiceCallback>();

    public void DoCallbackAdded(string serviceName)
    {
      lock (_callbacks)
        for (int i = _callbacks.Count - 1; i >= 0; i--)
          try
          {
            _callbacks[i].Added(serviceName);
          }
          catch
          {
            _callbacks.RemoveAt(i);
          }
    }

    public void DoCallbackRemoved(string serviceName)
    {
      lock (_callbacks)
        for (int i = _callbacks.Count - 1; i >= 0; i--)
          try
          {
            _callbacks[i].Removed(serviceName);
          }
          catch
          {
            _callbacks.RemoveAt(i);
          }
    }

    public void Listen(ServiceCallback Callback)
    {
      lock (_callbacks)
        _callbacks.Add(Callback);
    }

    public void Removed(ServiceCallback Callback)
    {
      lock (_callbacks)
        _callbacks.Remove(Callback);
    }
  }
}