using System.Net;
using System.Net.Sockets;
using Morph.Base;
using Morph.Core;

namespace Morph.Internet
{
  public abstract class LinkInternet : Link
  {
    protected LinkInternet(IPEndPoint EndPoint)
      : base(LinkTypeID.Internet)
    {
      _EndPoint = EndPoint;
    }

    public const int MorphPort = 0x3000;

    private IPEndPoint _EndPoint;
    public IPEndPoint EndPoint
    {
      get { return _EndPoint; }
    }

    static public LinkInternet New(EndPoint EndPoint)
    {
      if (!(EndPoint is IPEndPoint))
        throw new EMorphImplementation();
      if (EndPoint.AddressFamily == AddressFamily.InterNetworkV6)
        return new LinkInternetIPv6((IPEndPoint)EndPoint);
      else
        return new LinkInternetIPv4((IPEndPoint)EndPoint);
    }

    public override bool Equals(object obj)
    {
      return (obj is LinkInternet) && (((LinkInternet)obj).EndPoint.Equals(_EndPoint));
    }

    public override int GetHashCode()
    {
      return _EndPoint.GetHashCode();
    }

    public override string ToString()
    {
      return "{Internet EndPoint=" + EndPoint.ToString() + '}';
    }
  }

  public class LinkTypeInternet : ILinkTypeReader, ILinkTypeAction
  {
    static public void Register()
    {
      LinkTypes.Register(new LinkTypeInternet());
    }

    public LinkTypeID ID
    {
      get { return LinkTypeID.Internet; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool IsIPv6, IsString, HasPort;
      Reader.ReadLinkByte(out IsIPv6, out IsString, out HasPort);
      if (IsIPv6)
        return LinkInternetIPv6.ReadNew(Reader, IsString, HasPort);
      else
        return LinkInternetIPv4.ReadNew(Reader, IsString, HasPort);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      LinkInternet Link = (LinkInternet)CurrentLink;
      //  If this link represents here, then move on to the next link
      if (Connections.IsEndPointOnThisProcess(Link.EndPoint))
      {
        Message.Context = Link.EndPoint;
        Message.NextLinkAction();
        return;
      }
      //  Obtain a connection to the device that link refers to.
      Connection Connection;
      if (Message.IsForceful)
        Connection = Connections.Obtain(Link.EndPoint);
      else
        Connection = Connections.Find(Link.EndPoint);
      //  If not forceful and connection not found, then stop the message here
      if (Connection == null)
        return;
      //  Remove this address (this is done only in IPv4 due to NAT)
      if (Link is LinkInternetIPv4)
        Message.PathTo.Pop();
      else
        //  Ensure we have a link representing this device in Message.PathFrom
        if (Message.HasPathFrom)
        {
          bool AddLocalLink = false;
          Link LastLink = Message.PathFrom.Peek();
          if (LastLink is LinkInternet)
            AddLocalLink = !Connections.IsEndPointOnThisProcess(((LinkInternet)LastLink).EndPoint);
          else
            AddLocalLink = true;
          if (AddLocalLink)
            Message.PathFrom.Push(LinkInternet.New(Connection.LocalEndPoint));
        }
      //  Send the message on
      Connection.Write(Message);
    }
  }
}