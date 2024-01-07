using System.Net;
using Morph.Endpoint;
using Morph.Internet;

namespace Morph.Daemon
{
  public class LinkTypeServiceDaemon : LinkTypeService
  {
    protected override void ActionLinkService(LinkMessage Message, LinkService LinkService)
    {
      RegisteredService service = ServicesImpl.Find(LinkService.ServiceName);
      //  If the service was not found then...
      if (service == null)
      { //  ...and if it can be started up then
        RegisteredStartup startup = StartupImpl.Find(LinkService.ServiceName);
        if (startup != null)
        { //  ...start the service.
          service = startup.Startup();
        }
      }
      //  If still not found, then it's either a Morph daemon service or an error, so handle normally.
      if (service == null)
        base.ActionLinkService(Message, LinkService);
      else
      { //  The service was found.
        //  Validate access.
        IPEndPoint RemoteEndPoint = (IPEndPoint)((Connection)Message.Source).RemoteEndPoint;
        bool IsLocal = Connections.IsEndPointOnThisDevice(RemoteEndPoint);
        if ((IsLocal && !service.AccessLocal) || (!IsLocal && !service.AccessRemote))
          throw new EMorph("Access denied");
        //  Send it down the socket that holds the implementation.
        Message.PathTo.Push(LinkInternet.New(service.Socket.RemoteEndPoint)); //XX
        Message.Action();
      }
    }

    protected override void ActionLinkApartment(LinkMessage Message, LinkApartment LinkApartment)
    {
      ApartmentObject apartment = ApartmentsImpl.Find(LinkApartment.ApartmentID);
      if (apartment == null)
        base.ActionLinkApartment(Message, LinkApartment);
      else
      {
        Message.PathTo.Push(LinkInternet.New(apartment.Socket.RemoteEndPoint));
        Message.NextLinkAction();
      }
    }

    protected override void ActionLinkApartmentProxy(LinkMessage Message, LinkApartmentProxy LinkApartmentProxy)
    {
      ApartmentObject apartment = ApartmentProxiesImpl.Find(LinkApartmentProxy.ApartmentProxyID);
      if (apartment == null)
        base.ActionLinkApartmentProxy(Message, LinkApartmentProxy);
      else
      {
        Message.PathTo.Push(LinkInternet.New(apartment.Socket.RemoteEndPoint));
        Message.NextLinkAction();
      }
    }
  }
}