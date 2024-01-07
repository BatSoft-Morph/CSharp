using System;
using Morph.Base;
using Morph.Internet;

namespace Morph.Daemon
{
  public class RegisteredRunningInternet : RegisteredRunning, IDisposable
  {
    public RegisteredRunningInternet(RegisteredService RegisteredService, Connection Connection)
      : base(RegisteredService)
    {
      _Connection = Connection;
      _Connection.OnClose += new EventHandler(ConnectionClose);
    }

    private void ConnectionClose(object sender, EventArgs e)
    {
      lock (RegisteredService)
        if (RegisteredService.IsRunning)
          if (RegisteredService.Running == this)
            RegisteredService.Running = null;
      Dispose();
    }

    #region IDisposable

    public void Dispose()
    {
      lock (this)
        _Connection.OnClose -= new EventHandler(ConnectionClose);
    }

    #endregion

    private Connection _Connection;
    public Connection Connection
    {
      get { return _Connection; }
    }

    public override void HandleMessage(LinkMessage Message)
    {
      _Connection.Write(Message);
    }
  }
}