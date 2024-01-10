using System;
using Morph.Base;
using Morph.Internet;

namespace Morph.Daemon
{
  public class RegisteredRunningInternet : RegisteredRunning, IDisposable
  {
    public RegisteredRunningInternet(RegisteredService registeredService, Connection connection)
      : base(registeredService)
    {
      _connection = connection;
      _connection.OnClose += new EventHandler(ConnectionClose);
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
        _connection.OnClose -= new EventHandler(ConnectionClose);
    }

    #endregion

    private readonly Connection _connection;
    public Connection Connection
    {
      get => _connection;
    }

    public override void HandleMessage(LinkMessage message)
    {
      _connection.Write(message);
    }
  }
}