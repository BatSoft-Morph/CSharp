using System.Collections.Generic;
using Morph.Endpoint;

namespace Morph.Daemon
{
  public class ServiceCallback
  {
    internal ServiceCallback(ServletProxy ServletProxy)
    {
      _ServletProxy = ServletProxy;
    }

    private ServletProxy _ServletProxy;

    public void added(string serviceName)
    {
      _ServletProxy.SendMethod("added", new object[] { serviceName });
    }

    public void removed(string serviceName)
    {
      _ServletProxy.SendMethod("removed", new object[] { serviceName });
    }
  }

  public class ServiceCallbacks
  {
    private List<ServiceCallback> _Callbacks = new List<ServiceCallback>();

    public void DoCallbackAdded(string serviceName)
    {
      lock (_Callbacks)
        for (int i = _Callbacks.Count - 1; i >= 0; i--)
          try
          {
            _Callbacks[i].added(serviceName);
          }
          catch
          {
            _Callbacks.RemoveAt(i);
          }
    }

    public void DoCallbackRemoved(string serviceName)
    {
      lock (_Callbacks)
        for (int i = _Callbacks.Count - 1; i >= 0; i--)
          try
          {
            _Callbacks[i].removed(serviceName);
          }
          catch
          {
            _Callbacks.RemoveAt(i);
          }
    }

    public void Listen(ServiceCallback Callback)
    {
      lock (_Callbacks)
        _Callbacks.Add(Callback);
    }

    public void Removed(ServiceCallback Callback)
    {
      lock (_Callbacks)
        _Callbacks.Remove(Callback);
    }
  }
}