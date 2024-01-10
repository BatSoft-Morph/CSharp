using Morph.Base;
using Morph.Endpoint;

namespace Morph.Daemon
{
  public class LinkTypeServiceDaemon : LinkTypeService
  {
    protected override void ActionLinkService(LinkMessage message, LinkService linkService)
    {
      RegisteredService service = RegisteredServices.FindByName(linkService.ServiceName);
      if (service == null)
        throw new EMorphDaemon("Service not registered: \"" + linkService.ServiceName + "\"");
      service.Running.HandleMessage(message);
    }

    protected override void ActionLinkApartment(LinkMessage message, LinkApartment linkApartment)
    {
      RegisteredApartments.Apartments.Find(linkApartment.ApartmentID).HandleMessage(message);
    }

    protected override void ActionLinkApartmentProxy(LinkMessage message, LinkApartmentProxy linkApartmentProxy)
    {
      RegisteredApartments.ApartmentProxies.Find(linkApartmentProxy.ApartmentProxyID).HandleMessage(message);
    }
  }

  public class RegisteredRunningDaemon : RegisteredRunning
  {
    public RegisteredRunningDaemon(RegisteredService RegisteredService)
      : base(RegisteredService)
    { }

    private static readonly LinkTypeService s_linkType = new LinkTypeService();

    public override void HandleMessage(LinkMessage Message)
    {
      s_linkType.ActionLink(Message, Message.Current);
    }
  }
}