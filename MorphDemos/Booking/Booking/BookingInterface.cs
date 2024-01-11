namespace MorphDemoBooking
{
  static public class BookingInterface
  {
    //  Service constants
    public const string ServiceName = "Morph.Demo.Booking";

    //  Type names
    public const string RegistrationTypeName = "BookingRegistration";
    public const string DiplomatServerTypeName = "BookingDiplomatServer";
    public const string DiplomatClientTypeName = "BookingDiplomatClient";
  }

  public interface IBookingRegistration
  {
    IBookingDiplomatServer Register(string ClientName, IBookingDiplomatClient client);
  }

  public interface IBookingDiplomatServer
  {
    string Book(string objectName);
    string Unbook(string objectName);
    string OwnerOf(string objectName);
    string[] GetQueue(string objectName);
    void Nudge(string objectName);
  }

  public interface IBookingDiplomatClient
  {
    void NewOwner(string objectName, string clientName);
    void NudgedBy(string clientName);
  }
}