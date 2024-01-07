/**
 * An apartment proxy is the client side of a connection.
 * 
 * Use the Via... class methods to create a new apartment proxy, thereby establishing a connection to a service.
 * 
 * Parameters:
 * - Address:  This is the internet addess where the service is hosted.  This can be a domain name or an IPv4 or IPv6 internet address.
 * - ServiceName:  The name of the service that this apartment proxy aims to represent.
 * - Timeout:  How long to wait for replies to sent messages.  Timed outs raise exceptions.
 * - InstanceFactories:  A factory that converts parameters into values.  Must not be null.
 */

using System;
using System.Collections.Generic;
using System.Net;
using Morph.Base;
using Morph.Core;
using Morph.Internet;
using Morph.Lib;
using Morph.Params;
using Morph.Sequencing;

namespace Morph.Endpoint
{
  public class MorphApartmentProxy : RegisterItemID, IDisposable, IActionLinkData, IActionLast
  {
    #region Internal

    internal MorphApartmentProxy(Device Device, string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories)
    {
      //  Servlet proxies
      _ServletProxies = new ServletProxies(this);
      _DefaultServlet = _ServletProxies.Obtain(Servlet.DefaultID, null);
      //  Timeout
      _Timeout = Timeout;
      //  Path
      _ApartmentProxyLink = new LinkApartmentProxy(_ApartmentProxyID);
      _Device = Device;
      ApartmentLink = null;
      //  Set MorphService link
      _ServiceName = ServiceName;
      //  Registration
      lock (All)
        All.Add(this);
      //  Instance _actories
      _InstanceFactories = InstanceFactories;
    }

    internal MorphApartmentProxy(Device Device, int ApartmentID, TimeSpan Timeout, InstanceFactories InstanceFactories)
    {
      //  Servlet proxies
      _ServletProxies = new ServletProxies(this);
      _DefaultServlet = _ServletProxies.Obtain(Servlet.DefaultID, null);
      //  Timeout
      _Timeout = Timeout;
      //  Path
      _ApartmentProxyLink = new LinkApartmentProxy(_ApartmentProxyID);
      _Device = Device;
      ApartmentLink = new LinkApartment(ApartmentID);
      //  Set MorphService link
      _ServiceName = null;
      //  Registration
      lock (All)
        All.Add(this);
      //  Instance _actories
      _InstanceFactories = InstanceFactories;
    }

    static private bool ReadByteFromAddress(StringParser Parser, out byte Byte)
    {
      Byte = 0;
      string Str = Parser.ReadDigits();
      if ((Str == null) || (Str.Length > 3))
        return false;
      uint Num = uint.Parse(Str);
      if (256 <= Num)
        return false;
      Byte = (byte)Num;
      return true;
    }

    static private IPAddress ResolveIPv4(string String)
    {
      StringParser Parser = new StringParser(String);
      byte[] Address = new byte[4];
      if (ReadByteFromAddress(Parser, out Address[0]))
        if (Parser.ReadChar('.'))
          if (ReadByteFromAddress(Parser, out Address[1]))
            if (Parser.ReadChar('.'))
              if (ReadByteFromAddress(Parser, out Address[2]))
                if (Parser.ReadChar('.'))
                  if (ReadByteFromAddress(Parser, out Address[3]))
                    if (Parser.IsEnded())
                      return new IPAddress(Address);
      return null;
    }

    static private IPAddress Resolve(string String)
    {
      IPAddress Address = ResolveIPv4(String);
      if (Address != null)
        return Address;
      return Dns.GetHostEntry(String).AddressList[0];
    }

    private void EstablishConnection()
    {
      object[] OutParams;
      Call(new LinkMessage(new LinkStack(), new LinkStack(), true), out OutParams);
    }

    private void EndConnection()
    {
      LinkMessage Message = new LinkMessage(new LinkStack(), new LinkStack(), false);
      Message.PathTo.Push(LinkTypeEnd.End);
      Send(Message);
    }

    internal void Send(LinkMessage Message)
    {
      //  MorphService/MorphApartment
      PushToPathTo(Message);
      //  Send the message off. (No need to action the MorphApartment Proxy link.)
      Message.NextLinkAction();
    }

    internal object Call(LinkMessage Message, out object[] OutParams)
    {
      //  Call number
      int CallNumber = CallNumberSeed.Generate();
      Message.CallNumber = CallNumber;
      //  Sequence
      if (_SequenceSender != null)
        _SequenceSender.AddNextLink(false, Message);
      //  MorphService/MorphApartment path
      PushToPathTo(Message);
      //  Prepare to wait for the reply
      //  If the server end of this call is in this process, then this thread will end up replying to itself.
      //  Calling Prepare() here allows the server end to unblock the wait before Wait() is called.
      Waits.Prepare(CallNumber);
      try
      {
        //  Send the message off. (No need to action the MorphApartment Proxy link.)
        Message.NextLinkAction();
        Message = null;
        //  Wait for a response
        if (Waits.Wait(CallNumber, Timeout))
          //  Received a reply
          return Replies.GetReply(CallNumber, out OutParams);
        else
          //  No reply
          throw new EMorph("Timeout");
      }
      catch (Exception x)
      { //  Tidy up in case of error
        Replies.Remove(CallNumber);
        Waits.End(CallNumber);
        throw x;
      }
    }

    #endregion

    #region Constructors

    static public MorphApartmentProxy ViaLocal(string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories)
    {
      return ViaEndPoint(ServiceName, Timeout, InstanceFactories, new IPEndPoint(IPAddress.Loopback, LinkInternet.MorphPort));
    }

    static public MorphApartmentProxy ViaString(string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories, string Address)
    {
      return ViaAddress(ServiceName, Timeout, InstanceFactories, Resolve(Address));
    }

    static public MorphApartmentProxy ViaString(string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories, string Address, int Port)
    {
      return ViaEndPoint(ServiceName, Timeout, InstanceFactories, new IPEndPoint(Resolve(Address), Port));
    }

    static public MorphApartmentProxy ViaAddress(string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories, IPAddress Address)
    {
      return ViaEndPoint(ServiceName, Timeout, InstanceFactories, new IPEndPoint(Address, LinkInternet.MorphPort));
    }

    static public MorphApartmentProxy ViaEndPoint(string ServiceName, TimeSpan Timeout, InstanceFactories InstanceFactories, IPEndPoint EndPoint)
    {
      //  Add Internet link
      LinkStack DevicePath = new LinkStack();
      if (EndPoint != null)
        DevicePath.Push(LinkInternet.New(EndPoint));
      //  Obtain the device
      Device Device = Devices.Obtain(DevicePath);
      //  Create MorphApartment Proxy
      MorphApartmentProxy NewProxy = new MorphApartmentProxy(Device, ServiceName, Timeout, InstanceFactories);
      //  Establish connection
      NewProxy.EstablishConnection();
      //  Return the proxy object (removing redundancy)
      MorphApartmentProxy OldProxy = Device.Find(NewProxy.ApartmentID);
      if (OldProxy != NewProxy)
        NewProxy.Dispose();
      return OldProxy;
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      lock (All)
        All.Remove(this);
      if ((_Device != null) && (ApartmentID != 0))
        lock (_Device._ApartmentProxiesByApartmentID)
          if (_Device._ApartmentProxiesByApartmentID[ApartmentID] == this)
            _Device._ApartmentProxiesByApartmentID.Remove(ApartmentID);
      if (_SequenceSender != null)
      {
        _SequenceSender.Halt();
        _SequenceSender = null;
      }
      EndConnection();
    }

    #endregion

    #region Register

    ~MorphApartmentProxy()
    {
      Dispose();
    }

    #region RegisterItemID Members

    internal int ApartmentID
    {
      get
      {
        if (ApartmentLink == null)
          return 0;
        return ApartmentLink.ApartmentID;
      }
    }

    private int _ApartmentProxyID = IDFactory.Generate();
    public int ID
    {
      get { return _ApartmentProxyID; }
    }

    static public IIDFactory IDFactory = new IDSeed();

    #endregion

    static private RegisterItems<MorphApartmentProxy> All = new RegisterItems<MorphApartmentProxy>();

    static public MorphApartmentProxy Find(int ID)
    {
      lock (All)
        return All.Find(ID);
    }

    #endregion

    #region ServletProxies

    private ServletProxies _ServletProxies;
    internal ServletProxies ServletProxies
    {
      get { return _ServletProxies; }
    }

    public ServletProxy FindServlet(int ServletID)
    {
      return _ServletProxies.Find(ServletID);
    }

    private ServletProxy _DefaultServlet;
    public ServletProxy DefaultServlet
    {
      get { return _DefaultServlet; }
    }

    #endregion

    #region Timeout

    private TimeSpan _Timeout;
    public TimeSpan Timeout
    {
      get { return _Timeout; }
    }

    #endregion

    #region Path

    private LinkApartmentProxy _ApartmentProxyLink;

    private Device _Device;
    public Device Device
    {
      get { return _Device; }
    }

    private string _ServiceName;

    private LinkApartment _ApartmentLink = null;
    internal LinkApartment ApartmentLink
    {
      get { return _ApartmentLink; }
      set
      {
        if (ApartmentID == (value == null ? 0 : value.ApartmentID))
          return;
        if ((_Device != null) && (_ApartmentLink != null))
          lock (_Device._ApartmentProxiesByApartmentID)
            _Device._ApartmentProxiesByApartmentID.Remove(ApartmentID);
        _ApartmentLink = value;
        if ((_Device != null) && (_ApartmentLink != null))
          lock (_Device._ApartmentProxiesByApartmentID)
            if (_Device._ApartmentProxiesByApartmentID[ApartmentID] == null)
              _Device._ApartmentProxiesByApartmentID.Add(ApartmentID, this);
      }
    }

    public bool RequiresFromPath
    {
      get { return true; }
    }

    public LinkStack Path
    {
      get
      {
        LinkStack FullPath = new LinkStack();
        if (ApartmentLink != null)
          FullPath.Push(ApartmentLink);
        else if (_ServiceName != null)
          FullPath.Push(new LinkService(_ServiceName));
        FullPath.Push(_Device.Path);
        FullPath.Push(_ApartmentProxyLink);
        return FullPath;
      }
    }

    internal void SetPath(LinkStack Path)
    {
      if (Path == null)
        throw new EMorphUsage("A proper path to the apartment is expected");
      List<Link> Links = Path.ToLinks();
      //  Find this apartment proxy
      Link Link = null;
      if (Links.Count > 0)
      {
        Link = Links[Links.Count - 1];
        Links.RemoveAt(Links.Count - 1);
      }
      if ((Link == null) || !(Link is LinkApartmentProxy) || (((LinkApartmentProxy)Link).ApartmentProxyID != _ApartmentProxyID))
        throw new EMorphImplementation();
      //  Find the apartment link
      for (int i = Links.Count - 1; i >= 0; i--)
      {
        Link = Links[0];
        Links.RemoveAt(0);
        if (Link is LinkApartment)
        {
          ApartmentLink = (LinkApartment)Link;
          _Device._Path = new LinkStack(Links);
          return;
        }
      }
      throw new EMorph("Path did not include apartment link");
    }

    public void PushToPathTo(LinkMessage Message)
    {
      if (ApartmentLink != null)
        Message.PathTo.Push(ApartmentLink);
      else if (_ServiceName != null)
        Message.PathTo.Push(new LinkService(_ServiceName));
      Message.PathTo.Push(_Device.Path);
      Message.PathTo.Push(_ApartmentProxyLink);
    }

    #endregion

    #region Reply handling

    private IDSeed CallNumberSeed = new IDSeed();

    private Replies Replies = new Replies();

    private NumberedWaits Waits = new NumberedWaits();

    private void HandleReplyBegin(LinkMessage Message)
    {
      /*  Sequence
      if (NextLink is LinkSequence)
      {
        Message.ActionNext();
        MorphApartmentProxy._SequenceSender = (SequenceSender)((LinkSequence)NextLink).FindLinkObject();
      }
       */
      //  Find call number to match response with waiting thread
      int CallNumber = Message.CallNumber;
      //  Tell the calling thread to hold on, to prevent a timeout while we're assigning the return data.
      //  If Hold() fails, then it's because no thread is waiting for this reply.
      if (!Waits.Hold(CallNumber))
        throw new EMorph("No one waiting for reply");
      //  Update the proxy's path to the apartment
      if (Message.HasPathFrom)
        SetPath(Message.PathFrom);
    }

    private void HandleReplyEnd(int CallNumber)
    {
      Waits.End(CallNumber);
    }

    #endregion

    #region Instance factories

    private InstanceFactories _InstanceFactories;
    public InstanceFactories InstanceFactories
    {
      get { return _InstanceFactories; }
      set { _InstanceFactories = value; }
    }

    #endregion

    #region Sequencing

    internal SequenceSender _SequenceSender = null;
    public SequenceLevel SequenceLevel
    {
      get
      {
        if (_SequenceSender == null)
          return SequenceLevel.None;
        if (_SequenceSender.IsLossless)
          return SequenceLevel.Lossless;
        else
          return SequenceLevel.Lossy;
      }
    }

    #endregion

    #region IActionLinkData

    public void ActionLinkData(LinkMessage Message, LinkData Data)
    {
      int CallNumber = Message.CallNumber;
      try
      {
        HandleReplyBegin(Message);
        Replies.AssignReply(CallNumber, InstanceFactories, Device, Message.PathFrom, Data);
      }
      finally
      {
        HandleReplyEnd(CallNumber);
      }
    }

    #endregion

    #region IActionLast

    public void ActionLast(LinkMessage Message)
    {
      int CallNumber = Message.CallNumber;
      try
      {
        HandleReplyBegin(Message);
        Replies.AssignReply(CallNumber, InstanceFactories, Device, null, null);
      }
      finally
      {
        HandleReplyEnd(CallNumber);
      }
    }

    #endregion
  }
}