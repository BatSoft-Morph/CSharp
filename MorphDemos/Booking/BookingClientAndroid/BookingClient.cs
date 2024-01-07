using Morph.Endpoint;
using Morph.Params;
using MorphDemoBooking;
using MorphDemoBookingClientAndroid;
using System.Threading;

namespace MorphDemoBookingClient
{
  public class BookingRegistrationProxy : BookingRegistration
  {
    public BookingRegistrationProxy(ServletProxy ServletProxy)
    {
      _ServletProxy = ServletProxy;
    }

    private ServletProxy _ServletProxy;

    #region BookingRegistration Members

    public BookingDiplomatServer register(string ClientName, BookingDiplomatClient client)
    {
      return (BookingDiplomatServer)_ServletProxy.CallMethod("register", new object[] { ClientName, client });
    }

    #endregion
  }

  public class BookingDiplomatServerProxy : BookingDiplomatServer
  {
    public BookingDiplomatServerProxy(ServletProxy ServletProxy)
    {
      _ServletProxy = ServletProxy;
    }

    private ServletProxy _ServletProxy;
    public ServletProxy ServletProxy
    { get { return _ServletProxy; } }

    #region BookingDiplomatServer Members

    public string book(string objectName)
    {
      return (string)_ServletProxy.CallMethod("book", new object[] { objectName });
    }

    public string unbook(string objectName)
    {
      return (string)_ServletProxy.CallMethod("unbook", new object[] { objectName });
    }

    public string ownerOf(string objectName)
    {
      return (string)_ServletProxy.CallMethod("ownerOf", new object[] { objectName });
    }

    public string[] getQueue(string objectName)
    {
      return (string[])_ServletProxy.CallMethod("getQueue", new object[] { objectName });
    }

    public void nudge(string objectName)
    {
      _ServletProxy.SendMethod("nudge", new object[] { objectName });
    }

    #endregion
  }

  public class BookingDiplomatClientImpl : MorphReference, BookingDiplomatClient
  {
    public BookingDiplomatClientImpl(MorphApartment apartment, ActivityDemoBooking form)
      : base(BookingInterface.DiplomatClientTypeName)
    {
      _Form = form;
      MorphApartment = apartment;
    }

    ActivityDemoBooking _Form;

    #region BookingDiplomatClient Members

    public void newOwner(string objectName, string clientName)
    {
      _Form.NewOwner(objectName, clientName);
    }

    public void nudgedBy(string clientName)
    {
      _Form.NudgedBy(clientName);
    }

    #endregion
  }

  public class BookingFactory : InstanceFactories
  {
    public BookingFactory()
      : base()
    {
      Add(new BookingDiplomatServerFactory());
    }

    private class BookingDiplomatServerFactory : IReferenceDecoder
    {
      #region IReferenceFactory Members

      public bool DecodeReference(ServletProxy Value, out object Reference)
      {
        if (BookingInterface.DiplomatServerTypeName.Equals(Value.TypeName))
        {
          Reference = new BookingDiplomatServerProxy(Value);
          return true;
        }
        else
        {
          Reference = null;
          return false;
        }
      }

      #endregion
    }
  }
}