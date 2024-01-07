using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Internet
{
  public class LinkMessageFromIP : LinkMessage
  {
    public LinkMessageFromIP(LinkStack PathTo, LinkStack PathFrom, bool IsForceful, Connection Connection)
      : base(PathTo, PathFrom, IsForceful)
    {
      _Connection = Connection;
    }

    private Connection _Connection;
    public Connection Connection
    {
      get { return _Connection; }
    }
  }

  internal class DataInHandler
  {
    public DataInHandler(Connection Connection)
    {
      _Connection = Connection;
      _Stream = new MorphStream();
      _Reader = new MorphReaderSizeless(_Stream);
    }

    //  Incoming data
    private Connection _Connection;

    private MorphStream _Stream;
    private MorphReader _Reader;

    //  Message information

    private enum Stage { Clear, LinkByte, Message, Closed };

    private Stage _Stage = Stage.Clear;
    private LinkTypeID _LinkTypeID;
    private bool _HasCallNumber, _IsForceful, _HasPathFrom;
    private int _CallNumber, _PathToLength, _PathFromLength;

    private void ClearToLinkByte()
    {
      if ((_Stage == Stage.Clear) && (_Stream.Remaining > 0))
      {
        //  Examine in the link byte
        _LinkTypeID = _Reader.ReadLinkByte(out _HasCallNumber, out _IsForceful, out _HasPathFrom);
        //  Next stage
        _Stage = Stage.LinkByte;
      }
    }

    private void LinkTypeToMessage()
    {
      if (_Stage == Stage.LinkByte)
        try
        {
          //  If received a LinkEnd message to the socket, then end the socket
          if (_LinkTypeID == LinkTypeID.End)
          {
            //  Next stage
            _Stage = Stage.Closed;
            //  Close the connection
            _Connection.Close();
            return;
          }
          //  If receiving a LinkMessage, then see if there's enough data for reading it in
          if (_LinkTypeID == LinkTypeID.Message)
          {
            if (_Stream.Remaining >= 4 + (_HasCallNumber ? 4 : 0) + (_HasPathFrom ? 4 : 0))
            {
              //  - CallNumber
              _CallNumber = _HasCallNumber ? _Reader.ReadInt32() : 0;
              //  - PathTo size
              _PathToLength = _Reader.ReadInt32();
              //  - PathFrom size
              _PathFromLength = _HasPathFrom ? _Reader.ReadInt32() : 0;
              //  Next stage
              _Stage = Stage.Message;
            }
          }
          else
            //  No other link types are accepted outside here
            throw new EMorph("Unexpected link type");
        }
        catch
        {
          _Connection.Close();
          throw;
        }
    }

    private void MessageToClear()
    {
      if ((_Stage == Stage.Message) && (_PathToLength + _PathFromLength <= _Stream.Remaining))
      {
        //  Read PathTo 
        LinkStack PathTo = new LinkStack(_Reader.SubReader(_PathToLength));
        //  Read PathFrom
        LinkStack PathFrom = null;
        if (_HasPathFrom)
          if (_PathFromLength == 0)
            PathFrom = new LinkStack();
          else
            PathFrom = new LinkStack(_Reader.SubReader(_PathFromLength));
        //  Create message
        LinkMessage Message = new LinkMessageFromIP(PathTo, PathFrom, _IsForceful, _Connection);
        Message.Source = _Connection;
        if (_HasCallNumber) Message.CallNumber = _CallNumber;
        Message.IsForceful = _IsForceful;
        //  Apply NAT workaround for IPv4
        if ((_Connection.Socket.AddressFamily == AddressFamily.InterNetwork) && (PathFrom != null))
          Message.PathFrom.Push(new LinkInternetIPv4((IPEndPoint)_Connection.Socket.RemoteEndPoint));
        //  Add the (now completely received) message to the action queue
        ActionHandler.Add(Message);
        //  Next stage
        _Stage = Stage.Clear;
      }
    }

    public void AddData(byte[] data, int offset, int count)
    {
      if ((data == null) || (count == 0))
        return;
      //  Write data to stream
      _Stream.Write(data, offset, count);
      //  Read messages from stream
      do
      {
        ClearToLinkByte();
        LinkTypeToMessage();
        MessageToClear();
      } while ((_Stage == Stage.Clear) && (_Stream.Remaining > 0));
    }
  }

  public class Connection : RegisterItemName
  {
    internal Connection(Socket Socket)
    {
      _Socket = Socket;
      _Name = _Socket.RemoteEndPoint.ToString();
      _DataIn = new DataInHandler(this);
      SendMorphValidation();
      Connections.Add(this);
      StartReceivingData();
    }

    #region Connection validation

    //  "Morph"#0 + Major version:1 + Minor version:1 
    private static byte[] MorphValidation = new byte[] { 0x4D, 0x6F, 0x72, 0x70, 0x68, 0x00, 0x01, 0x01 };

    private ManualResetEvent _MorphValidationSent = new ManualResetEvent(false);

    private void SendMorphValidation()
    {
      _Socket.Send(MorphValidation);
      _MorphValidationSent.Set();
    }

    private void TestMorphValidation()
    {
      byte[] Buffer = new byte[8];
      if (Buffer.Length != _Socket.Receive(Buffer))
        throw new EMorph("Remote connection appears to not be a Morph connection.");
      for (int i = 5; i >= 0; i--)
        if (Buffer[i] != MorphValidation[i])
          throw new EMorph("Remote connection appears to not be a Morph connection.");
      if (Buffer[6] != MorphValidation[6])
        throw new EMorph("Incompatible Major versions of Morph.");
      if (Buffer[7] != MorphValidation[7])
        throw new EMorph("Incompatible Minor versions of Morph.");
    }

    #endregion

    private Socket _Socket;
    public Socket Socket
    {
      get { return _Socket; }
    }

    private DataInHandler _DataIn;
    private const int BufferSize = 2048;

    public EndPoint LocalEndPoint
    {
      get { return _Socket.LocalEndPoint; }
    }

    public EndPoint RemoteEndPoint
    {
      get { return _Socket.RemoteEndPoint; }
    }

    private LinkInternet _Link = null;
    public LinkInternet Link
    {
      get
      {
        if (_Link == null)
          _Link = LinkInternet.New(RemoteEndPoint);
        return _Link;
      }
    }

    public void Write(LinkMessage Message)
    {
      //  Don't write until Morph validation information has been sent
      _MorphValidationSent.WaitOne();
      //  Get the data to send.
      //  Alot of optimisation could be done here.
      //  This ought to use BeginSend(), but that'll have to wait for now.
      //  Also, this is where one would batch messages
      //
      //  Create a stream
      MemoryStream Stream = new MemoryStream();
      //  Write the Message to the stream
      Message.Write(new MorphWriter(Stream));
      //  Start from the beginning
      long TotalCount = Stream.Length;
      Stream.Position = 0;
      //  Write the stream to the socket
      byte[] Buffer = new byte[BufferSize];
      lock (_Socket)
        if (_Socket.Connected)
          while (TotalCount > 0)
          {
            int count = (int)(TotalCount < BufferSize ? TotalCount : BufferSize);
            count = Stream.Read(Buffer, 0, count);
            _Socket.Send(Buffer, 0, count, SocketFlags.Partial);
            TotalCount -= count;
          }
    }

    #region Receiving

    private byte[] _IncomingData = new byte[BufferSize];

    internal void StartReceivingData()
    {
      new Thread(new ThreadStart(ReceivingData)).Start();
    }

    private void ReceivingData()
    {
      Thread.CurrentThread.Name = "Connection";
      try
      {
        //  Validate the connection
        TestMorphValidation();
        //  Read incoming data
        int NoDataIteration = 0;
        while (_Socket.Connected)
        {
          int count = _Socket.Receive(_IncomingData, 0, _IncomingData.Length, SocketFlags.Partial);
          //  Workaround for _Socket.Receive() continuously returning 0 instead of waiting for data.
          //  Seems to happen when connection has been abandoned by remote host.
          if (count > 0)
            NoDataIteration = 0;
          else if (NoDataIteration++ > 5)
            Close();
          //  Add the received data to the message stream
          _DataIn.AddData(_IncomingData, 0, count);
        }
      }
      catch (Exception x)
      {
        //  Whatever the problem, make sure the thread is closed
        try
        {
          Close();
        }
        catch (Exception)
        {
        }
        MorphErrors.NotifyAbout(this, x);
      }
    }

    #endregion

    public void Close()
    {
      //  Prevent other threads from writing to the socket
      Connections.Remove(this);
      lock (_Socket)
      {
        //  Notify attached objects to tidy themselves up
        if (OnClose != null)
          OnClose(this, new EventArgs());
        //  Try to tell other end that the socket is closing
        if (_Socket.Connected)
        {
          try
          {
            _Socket.Send(new byte[] { 0 }); //  Sending LinkEnd
          }
          catch
          {
          }
          //  Now close the socket
          _Socket.Close();
        }
      }
    }

    #region RegisterItemName Members

    private string _Name;
    public string Name
    {
      get { return _Name; }
    }

    #endregion

    public event EventHandler OnClose;
  }

  static public class Connections
  {
    #region internal

    static private RegisterItems<Connection> Conns = new RegisterItems<Connection>();
    static private Hashtable LocalEndPoints = new Hashtable();

    private class LocalEndPointCounter
    {
      public int Count = 1;
    }

    static private IPAddress[] CurrentLocalAddresses()
    {
      return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
    }

    static private Socket NewSocket(IPEndPoint RemoteEndPoint)
    {
      ProtocolType Protocol;
      if (RemoteEndPoint.AddressFamily == AddressFamily.InterNetwork)
        Protocol = ProtocolType.IP;
      else if (RemoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
        Protocol = ProtocolType.IPv6;
      else
        throw new Exception("Unsupported protocol");
      //  Create socket connection
      Socket socket = new Socket(RemoteEndPoint.AddressFamily, SocketType.Stream, Protocol);
      //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
      //socket.Bind(new IPEndPoint(IPAddress.Any, 0));
      socket.Connect(RemoteEndPoint);
      //  Create connection
      return socket;
    }

    static internal void Add(Connection Connection)
    {
      lock (Conns)
      {
        Conns.Add(Connection);
        //  If this is a server process, then multiple sockets may use the same local endpoint,
        //  so keep a reference count.
        lock (LocalEndPoints)
        {
          LocalEndPointCounter counter = (LocalEndPointCounter)LocalEndPoints[Connection.LocalEndPoint.ToString()];
          if (counter == null)
            LocalEndPoints.Add(Connection.LocalEndPoint.ToString(), new LocalEndPointCounter());
          else
            counter.Count++;
        }
      }
    }

    static internal void Remove(Connection Connection)
    {
      lock (Conns)
      {
        Conns.Remove(Connection);
        lock (LocalEndPoints)
        {
          LocalEndPointCounter counter = (LocalEndPointCounter)LocalEndPoints[Connection.LocalEndPoint.ToString()];
          if (counter != null)  //  Should not be possible to be null, but just just in case
          { //  Decrement the refence count
            counter.Count--;
            //  If there are no more, then remove the counter
            if (counter.Count == 0)
              LocalEndPoints.Remove(Connection.LocalEndPoint.ToString());
          }
        }
      }
    }

    #endregion

    static public Connection Add(Socket Socket)
    {
      if (Socket == null)
        throw new EMorphUsage("Cannot connect with a null connection");
      //  Create (and register) the new connection
      return new Connection(Socket);
    }

    static public Connection Add(IPEndPoint RemoteEndPoint)
    {
      if (RemoteEndPoint == null)
        throw new EMorphUsage("Cannot connect to a null end point");
      //  Create (and register) the new connection
      return Add(NewSocket(RemoteEndPoint));
    }

    static public Connection Obtain(IPEndPoint RemoteEndPoint)
    {
      Connection Connection = Find(RemoteEndPoint);
      if (Connection == null)
        return new Connection(NewSocket(RemoteEndPoint));
      return Connection;
    }

    static public Connection Find(IPEndPoint RemoteEndPoint)
    {
      lock (Conns)
        return Conns.Find(RemoteEndPoint.ToString());
    }

    static public void CloseAll()
    {
      List<Connection> AllConns;
      lock (Conns)
        AllConns = Conns.List();
      foreach (Connection Conn in AllConns)
        Conn.Close();
    }

    static public bool IsEndPointOnThisDevice(IPEndPoint EndPoint)
    {
      IPAddress[] LocalAddresses = CurrentLocalAddresses();
      foreach (IPAddress LocalAddress in LocalAddresses)
        if (EndPoint.Address.Equals(LocalAddress))
          return true;
      return IPAddress.Loopback.Equals(EndPoint.Address);
    }

    static public bool IsEndPointOnThisProcess(IPEndPoint EndPoint)
    {
      lock (LocalEndPoints)
        return LocalEndPoints[EndPoint.ToString()] != null;
    }
  }
}