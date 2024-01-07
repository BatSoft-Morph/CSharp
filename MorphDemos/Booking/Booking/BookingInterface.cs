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

  public interface BookingRegistration
  {
    BookingDiplomatServer register(string ClientName, BookingDiplomatClient client);
  }

  public interface BookingDiplomatServer
  {
    string book(string objectName);
    string unbook(string objectName);
    string ownerOf(string objectName);
    string[] getQueue(string objectName);
    void nudge(string objectName);
  }

  public interface BookingDiplomatClient
  {
    void newOwner(string objectName, string clientName);
    void nudgedBy(string clientName);
  }
}