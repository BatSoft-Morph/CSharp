using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Morph.Lib;

namespace Morph.Internet
{
  public class Listener
  {
    internal Listener(IPEndPoint EndPoint)
    {
      _EndPoint = EndPoint;
    }

    private IPEndPoint _EndPoint;
    public IPEndPoint EndPoint
    {
      get { return _EndPoint; }
    }

    private Socket _Listener;

    private Socket CreateSocket(IPEndPoint EndPoint)
    {
      Socket socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
      socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
      socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
      socket.Blocking = true;
      return socket;
    }

    private void AsynchListen()
    {
      Thread.CurrentThread.Name = "Listener";
      try
      {
        try
        {
          _Listener.Bind(_EndPoint);
          _Listener.Listen(5);
          while (_IsStarted)
            Connections.Add(_Listener.Accept());
        }
        finally
        {
          lock (this)
            _IsActive = false;
        }
      }
      catch (Exception x)
      {
        MorphErrors.NotifyAbout(this, x);
      }
    }

    private bool _IsStarted = false;
    private bool _IsActive = false;
    public bool IsActive
    {
      get { return _IsActive; }
    }

    public void Start()
    {
      lock (this)
        if (!_IsActive)
        {
          //  Create the listener
          _Listener = CreateSocket(_EndPoint);
          //  Start the thread
          try
          {
            _IsStarted = true;
            (new Thread(new ThreadStart(AsynchListen))).Start();
            _IsActive = true;
          }
          //  Just in case
          catch
          {
            _IsStarted = false;
            _Listener.Close();
            _Listener = null;
            throw;
          }
        }
        else if (!_IsStarted)
          throw new EMorphUsage("Cannot start while still busy stopping");
    }

    public void Stop()
    {
      lock (this)
        if (_IsActive)
        {
          //  Tell the thread to stop
          _IsStarted = false;
          //  Close the listener
          _Listener.Close();
          _Listener = null;
        }
    }
  }

  public class Listeners
  {
    internal Listeners(List<Listener> Items)
    {
      _Items = Items;
    }

    private List<Listener> _Items;

    public int Count
    {
      get { return _Items.Count; }
    }

    public Listener this[int index]
    {
      get { return _Items[index]; }
    }

    public Listener[] ToArray()
    {
      return _Items.ToArray();
    }

    public void StartAll()
    {
      foreach (Listener Listener in _Items)
        Listener.Start();
    }

    public void StopAll()
    {
      foreach (Listener Listener in _Items)
        Listener.Stop();
    }
  }

  public class ListenerManager
  {
    static private List<Listener> All = new List<Listener>();

    static private bool IsLocal(IPAddress Address)
    {
      IPAddress[] LocalAddresses = GetAllLocalAddresses();
      foreach (IPAddress LocalAddress in LocalAddresses)
        if (Address.Equals(LocalAddress))
          return true;
      return IPAddress.IsLoopback(Address);
    }

    static public IPAddress[] GetAllLocalAddresses()
    {
      return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
    }

    static public Listener Obtain(IPAddress Address, int Port)
    {
      return Obtain(new IPEndPoint(Address, Port));
    }

    static public Listener Obtain(IPEndPoint EndPoint)
    {
      if (!IsLocal(EndPoint.Address))
        throw new EMorphUsage("Not a local IP address");
      //  See if it already exists
      Listener PortListener = Find(EndPoint);
      if (PortListener == null)
      { //  Create and register the listener
        PortListener = new Listener(EndPoint);
        All.Add(PortListener);
      }
      return PortListener;
    }

    static public Listeners Obtain(int Port)
    {
      List<Listener> Items = new List<Listener>();
      //  Add all network addresses to result
      IPAddress[] Addresses = GetAllLocalAddresses();
      for (int i = 0; i < Addresses.Length; i++)
        Items.Add(Obtain(new IPEndPoint(Addresses[i], Port)));
      //  Ensure loopback is included
      bool AddLoopback = true;
      for (int i = 0; i < Items.Count; i++)
        if (Items[i].EndPoint.Address.Equals(IPAddress.Loopback))
        {
          AddLoopback = false;
          break;
        }
      if (AddLoopback)
        Items.Add(Obtain(new IPEndPoint(IPAddress.Loopback, Port)));
      //  Return a list of listeners
      return new Listeners(Items);
    }

    static public Listener Find(IPEndPoint EndPoint)
    {
      foreach (Listener Listener in All)
        if (Listener.EndPoint.Equals(EndPoint))
          return Listener;
      return null;
    }

    static public Listeners Find(IPAddress Address)
    {
      List<Listener> Items = new List<Listener>();
      foreach (Listener Listener in All)
        if (Listener.EndPoint.Address.Equals(Address))
          Items.Add(Listener);
      return new Listeners(Items);
    }

    static public Listeners Find(int Port)
    {
      List<Listener> Items = new List<Listener>();
      //  Add all network addresses to result
      foreach (Listener Listener in All)
        if (Listener.EndPoint.Port == Port)
          Items.Add(Listener);
      //  Return a list of listeners
      return new Listeners(Items);
    }

    static public Listeners FindAll()
    {
      return new Listeners(All);
    }

    static public void StartAll()
    {
      foreach (Listener Listener in All)
        Listener.Start();
    }

    static public void StopAll()
    {
      foreach (Listener Listener in All)
        Listener.Stop();
    }

    static public void RemoveAllInactive()
    {
      lock (All)
        for (int i = All.Count - 1; 0 <= i; i--)
        {
          Listener Listener = All[i];
          if (!Listener.IsActive)
            All.Remove(Listener);
        }
    }
  }
}