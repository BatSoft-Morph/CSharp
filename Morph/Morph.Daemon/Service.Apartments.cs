using Morph.Base;
using Morph.Internet;
using Morph.Lib;
using Morph.Params;

namespace Morph.Daemon
{
  public class ApartmentObjects : MorphReference, IMorphParameters
  {
    internal ApartmentObjects(string TypeName, IIDFactory IDFactory, RegisteredApartments Registered)
      : base(TypeName)
    {
      _IDFactory = IDFactory;
      _Registered = Registered;
    }

    private IIDFactory _IDFactory;
    private RegisteredApartments _Registered;

    public int obtain(LinkMessage Message)
    {
      int ID = _IDFactory.Generate();
      if (Message is LinkMessageFromIP)
        new RegisteredApartmentInternet(_Registered, ID, ((LinkMessageFromIP)Message).Connection);
      else
        throw new EMorphDaemon(GetType().Name + ".obtain(): Unhandled message type \"" + Message.GetType().Name + "\".");
      return ID;
    }

    public void release(LinkMessage Message, int id)
    {
      if (Message is LinkMessageFromIP)
      {
        Connection connection = ((LinkMessageFromIP)Message).Connection;
        _Registered.Unregister(id);
      }
      else
        throw new EMorphDaemon(GetType().Name + ".release(): Unhandled message type \"" + Message.GetType().Name + "\".");
    }
  }
}