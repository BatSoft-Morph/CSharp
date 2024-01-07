using System.Collections;
using Morph.Endpoint;
using Morph.Params;
using MorphDemoBooking;

namespace MorphDemoBookingServer
{
  /* Multiple clients can claim to have the same name.  This class ensures
   * that each client is distinguished by its apartment ID instead of its name.
   */
  public class Registration
  {
    #region Static

    static Registration()
    {
      ObjectInstance.OnClientIDChanged += OnClientIDChanged;
    }

    static private Hashtable AllRegistrations = new Hashtable();

    static public Registration FindBy(string ClientID)
    {
      if (ClientID == null)
        return null;
      else
        lock (AllRegistrations)
          return ((Registration)AllRegistrations[ClientID]);
    }

    static public string ClientID_To_ClientName(string ClientID)
    {
      if (ClientID == null)
        return null;
      Registration reg = FindBy(ClientID);
      if (reg != null)
        return reg._ClientName;
      else
        return null;
    }

    #endregion

    public Registration(string ClientName, BookingDiplomatClientProxy ClientProxy, MorphApartment Apartment)
    {
      //  Client identification
      _ClientName = ClientName;
      _ClientID = Apartment.ID.ToString();
      //  Keep track of remote diplomat
      _ClientProxy = ClientProxy;
      //  Keep track of local diplomat
      _ServerImpl = new BookingDiplomatServerImpl();
      _ServerImpl._Registration = this;
      //  Register the Server servlet with this session's apartment
      _ServerImpl.MorphApartment = Apartment;
      //  Register the Registration
      lock (AllRegistrations)
        AllRegistrations[_ClientID] = this;
    }

    public void Unregister()
    {
      //  Let other clients have access to objects
      lock (AllRegistrations)
        ObjectInstances.ReleaseAll(_ClientID);
      //  Unregister the Registration
      AllRegistrations[_ClientID] = null;
      _ServerImpl.MorphApartment.Dispose();
    }

    public string _ClientName;
    public string _ClientID;
    public BookingDiplomatClientProxy _ClientProxy;
    public BookingDiplomatServerImpl _ServerImpl;

    // This event method notifies all interested clients when object ownership has changed
    static void OnClientIDChanged(object Sender, ClientIDArgs args)
    {
      ObjectInstance obj = args.ObjectInstance;
      //  Get new client name
      string NewClientName = Registration.ClientID_To_ClientName(args.NewClientID);
      //  List all clients who are waiting for this object
      string[] ClientIDs = ObjectInstances.ListClientIDs(obj.ObjectName);
      //  Tell each client that is interested in this object that the owner has changed
      for (int i = 0; i < ClientIDs.Length; i++)
      {
        Registration WaitingClient = FindBy(ClientIDs[i]);
        try
        { //  Tell the waiting client about the change of owner
          WaitingClient._ClientProxy.newOwner(obj.ObjectName, NewClientName);
        }
        catch
        { //  Zero tolerance.  If there's a problem, then unregister the client
          WaitingClient.Unregister();
        }
      }
    }
  }

  public class BookingRegistrationImpl : MorphReference, BookingRegistration
  {
    public BookingRegistrationImpl()
      : base(BookingInterface.RegistrationTypeName)
    {
    }

    #region BookingRegistration Members

    public BookingDiplomatServer register(string ClientName, BookingDiplomatClient client)
    {
      BookingDiplomatClientProxy ClientProxy = (BookingDiplomatClientProxy)client;
      Registration registration = new Registration(ClientName, ClientProxy, this.MorphApartment);
      return registration._ServerImpl;
    }

    #endregion
  }

  public class BookingDiplomatServerImpl : MorphReference, BookingDiplomatServer
  {
    public BookingDiplomatServerImpl()
      : base(BookingInterface.DiplomatServerTypeName)
    {
    }

    public Registration _Registration;

    #region BookingDiplomatServer Members

    public string book(string objectName)
    {
      string NewOwnersClientID = ObjectInstances.Obtain(objectName, _Registration._ClientID);
      return Registration.ClientID_To_ClientName(NewOwnersClientID);
    }

    public string unbook(string objectName)
    {
      string NewOwnersClientID = ObjectInstances.Release(objectName, _Registration._ClientID);
      return Registration.ClientID_To_ClientName(NewOwnersClientID);
    }

    public string ownerOf(string objectName)
    {
      string OwnersClientID = ObjectInstances.CurrentOwnerOf(objectName);
      return Registration.ClientID_To_ClientName(OwnersClientID);
    }

    public string[] getQueue(string objectName)
    {
      string[] ClientIDs = ObjectInstances.ListClientIDs(objectName);
      string[] ClientNames = new string[ClientIDs.Length];
      for (int i = 0; i < ClientIDs.Length; i++)
        ClientNames[i] = Registration.ClientID_To_ClientName(ClientIDs[i]);
      return ClientNames;
    }

    public void nudge(string objectName)
    {
      string OwnerID = ObjectInstances.CurrentOwnerOf(objectName);
      if (OwnerID != null)
      {
        Registration CurrentOwner = Registration.FindBy(OwnerID);
        try
        { //  Nudge the current owner of the object, telling them who the nudge is from
          CurrentOwner._ClientProxy.nudgedBy(_Registration._ClientName);
        }
        catch
        { //  Zero tolerance.  If there's a problem, then unregister the client
          CurrentOwner.Unregister();
        }
      }
    }

    #endregion
  }

  public class BookingDiplomatClientProxy : BookingDiplomatClient
  {
    public BookingDiplomatClientProxy(ServletProxy ServletProxy)
    {
      _ServletProxy = ServletProxy;
    }

    private ServletProxy _ServletProxy;

    #region BookingDiplomatClient Members

    public void newOwner(string objectName, string clientName)
    {
      _ServletProxy.SendMethod("newOwner", new object[] { objectName, clientName });
    }

    public void nudgedBy(string clientName)
    {
      _ServletProxy.CallMethod("nudgedBy", new object[] { clientName });
    }

    #endregion
  }
}