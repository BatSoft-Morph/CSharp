using System;
using Morph.Daemon.Client;
using Morph.Endpoint;
using Morph.Params;
using Morph.Sequencing;
using MorphDemoBooking;

namespace MorphDemoBookingServer
{
  public class BookingRegistrationApartmentFactory : MorphApartmentFactorySession
  {
    public BookingRegistrationApartmentFactory(DefaultServletObjectFactory DefaultServletObject, InstanceFactories InstanceFactories, TimeSpan Timeout, SequenceLevel Level)
      : base(DefaultServletObject, InstanceFactories, Timeout, Level)
    {
    }

    protected override MorphApartmentSession CreateApartment(object DefaultObject, SequenceLevel Level)
    {
      return new BookingRegistrationSession(this, DefaultObject, Level);
    }
  }

  public class BookingRegistrationSession : MorphApartmentSession
  {
    public BookingRegistrationSession(MorphApartmentFactory Owner, object DefaultObject, SequenceLevel Level)
      : base(Owner, DefaultObject, Level)
    {
      Count++;
    }

    public override void Dispose()
    {
      base.Dispose();
      //  If there are no more booking sessions, then shut down
      if ((--Count) == 0)
      {
        //  Both of these work when the application is visible.  (ie. manually started)
        //  But how to kill this application when it is not visible?  (ie. started by the Morph.Daemon)
        MorphManager.shutdown();
        //BookingServerForm.Instance.Invoke(new VoidDelegate(BookingServerForm.Instance.Close));
        //BookingServerForm.Instance.Invoke(new VoidDelegate(Application.Exit));
      }
    }

    static private int Count = 0;

    private delegate void VoidDelegate();
  }

  public class BookingRegistrationFactory : DefaultServletObjectFactory
  {
    #region DefaultServletObjectFactory Members

    public object ObtainServlet()
    {
      return new BookingRegistrationImpl();
    }

    #endregion
  }

  public class BookingInstanceFactories : InstanceFactories
  {
    public BookingInstanceFactories()
      : base()
    {
      Add(new BookingDiplomatClientFactory());
    }

    private class BookingDiplomatClientFactory : IReferenceDecoder
    {
      #region IReferenceFactory Members

      public bool DecodeReference(ServletProxy Value, out object Reference)
      {
        if (BookingInterface.DiplomatClientTypeName.Equals(Value.TypeName))
          Reference = new BookingDiplomatClientProxy(Value);
        else
          Reference = null;
        return Reference != null;
      }

      #endregion
    }
  }
}