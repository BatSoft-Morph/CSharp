using Morph.Endpoint;
using Morph.Params;
using MorphDemoBooking;
using MorphDemoBookingClientAndroid;
using System.Threading;

namespace MorphDemoBookingClient
{
  public class BookingRegistrationProxy : IBookingRegistration
  {
    public BookingRegistrationProxy(ServletProxy servletProxy)
    {
      _servletProxy = servletProxy;
    }

    private readonly ServletProxy _servletProxy;

    #region BookingRegistration Members

    public IBookingDiplomatServer Register(string clientName, IBookingDiplomatClient client)
    {
      return (IBookingDiplomatServer)_servletProxy.CallMethod("Register", new object[] { clientName, client });
    }

    #endregion
  }

  public class BookingDiplomatServerProxy : IBookingDiplomatServer
  {
    public BookingDiplomatServerProxy(ServletProxy servletProxy)
    {
      _servletProxy = servletProxy;
    }

    private readonly ServletProxy _servletProxy;
    public ServletProxy ServletProxy
    { get { return _servletProxy; } }

    #region BookingDiplomatServer Members

    public string Book(string objectName)
    {
      return (string)_servletProxy.CallMethod("Book", new object[] { objectName });
    }

    public string Unbook(string objectName)
    {
      return (string)_servletProxy.CallMethod("Unbook", new object[] { objectName });
    }

    public string OwnerOf(string objectName)
    {
      return (string)_servletProxy.CallMethod("OwnerOf", new object[] { objectName });
    }

    public string[] GetQueue(string objectName)
    {
      return (string[])_servletProxy.CallMethod("GetQueue", new object[] { objectName });
    }

    public void Nudge(string objectName)
    {
      _servletProxy.SendMethod("Nudge", new object[] { objectName });
    }

    #endregion
  }

  public class BookingDiplomatClientImpl : MorphReference, IBookingDiplomatClient
  {
    public BookingDiplomatClientImpl(MorphApartment apartment, ActivityDemoBooking form)
      : base(BookingInterface.DiplomatClientTypeName)
    {
      _form = form;
      MorphApartment = apartment;
    }

    private readonly ActivityDemoBooking _form;

    #region BookingDiplomatClient Members

    public void NewOwner(string objectName, string clientName)
    {
      _form.NewOwner(objectName, clientName);
    }

    public void NudgedBy(string clientName)
    {
      _form.NudgedBy(clientName);
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

      public bool DecodeReference(ServletProxy value, out object reference)
      {
        if (BookingInterface.DiplomatServerTypeName.Equals(value.TypeName))
        {
          reference = new BookingDiplomatServerProxy(value);
          return true;
        }
        else
        {
          reference = null;
          return false;
        }
      }

      #endregion
    }
  }
}