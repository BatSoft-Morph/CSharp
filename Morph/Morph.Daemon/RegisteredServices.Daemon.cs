using Morph.Base;
using Morph.Endpoint;

namespace Morph.Daemon
{
  public class LinkTypeServiceDaemon : LinkTypeService
  {
    protected override void ActionLinkService(LinkMessage Message, LinkService LinkService)
    {
      RegisteredService Service = RegisteredServices.FindByName(LinkService.ServiceName);
      if (Service == null)
        throw new EMorphDaemon("Service not registered: \"" + LinkService.ServiceName + "\"");
      Service.Running.HandleMessage(Message);
    }

    protected override void ActionLinkApartment(LinkMessage Message, LinkApartment LinkApartment)
    {
      RegisteredApartments.Apartments.Find(LinkApartment.ApartmentID).HandleMessage(Message);
    }

    protected override void ActionLinkApartmentProxy(LinkMessage Message, LinkApartmentProxy LinkApartmentProxy)
    {
      RegisteredApartments.ApartmentProxies.Find(LinkApartmentProxy.ApartmentProxyID).HandleMessage(Message);
    }
  }

  public class RegisteredRunningDaemon : RegisteredRunning
  {
    public RegisteredRunningDaemon(RegisteredService RegisteredService)
      : base(RegisteredService)
    { }

    static private LinkTypeService LinkType = new LinkTypeService();

    public override void HandleMessage(LinkMessage Message)
    {
      LinkType.ActionLink(Message, Message.Current);
    }
  }
}