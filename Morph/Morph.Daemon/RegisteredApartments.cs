using System;
using System.Collections;
using Morph.Base;
using Morph.Endpoint;
using Morph.Internet;

namespace Morph.Daemon
{
  public abstract class RegisteredApartment : IDisposable
  {
    public RegisteredApartment(RegisteredApartments Owner, int ID)
    {
      _Owner = Owner;
      _ID = ID;
      _Owner.Register(this);
    }

    #region IDisposable

    public virtual void Dispose()
    {
      _Owner.Unregister(_ID);
    }

    #endregion

    protected RegisteredApartments _Owner;

    private int _ID;
    public int ID
    { get { return _ID; } }

    public abstract bool CanUnregister
    { get; }

    public abstract void HandleMessage(LinkMessage Message);
  }

  public class RegisteredApartmentDaemon : RegisteredApartment
  {
    public RegisteredApartmentDaemon(RegisteredApartments Owner, int ID)
      : base(Owner, ID)
    { }

    public override bool CanUnregister
    {
      get { return false; }
    }

    private LinkTypeService _LinkTypeService = new LinkTypeService();

    public override void HandleMessage(LinkMessage Message)
    {
      _LinkTypeService.ActionLink(Message, Message.Current);
    }
  }

  public class RegisteredApartmentInternet : RegisteredApartment
  {
    public RegisteredApartmentInternet(RegisteredApartments Owner, int ID, Connection Connection)
      : base(Owner, ID)
    {
      lock (this)
      {
        _Connection = Connection;
        _Connection.OnClose += ConnectionClose;
      }
    }

    public override void Dispose()
    {
      lock (this)
        if (_Connection != null)
        {
          _Connection.OnClose -= ConnectionClose;
          _Connection = null;
        }
      base.Dispose();
    }

    private Connection _Connection;

    private void ConnectionClose(object sender, EventArgs e)
    {
      Dispose();
    }

    public override bool CanUnregister
    {
      get { return true; }
    }

    public override void HandleMessage(LinkMessage Message)
    {
      _Connection.Write(Message);
    }
  }

  public class RegisteredApartments
  {
    private Hashtable _Apartments = new Hashtable();

    public void Register(RegisteredApartment Apartment)
    {
      lock (_Apartments)
        if (_Apartments[Apartment.ID] == null)
          _Apartments.Add(Apartment.ID, Apartment);
        else
          throw new EMorphDaemon("Duplicate apartment ID");
    }

    public void Unregister(int ApartmentID)
    {
      lock (_Apartments)
      {
        RegisteredApartment Apartment = (RegisteredApartment)_Apartments[ApartmentID];
        if (Apartment != null)
          if (Apartment.CanUnregister)
            _Apartments.Remove(ApartmentID);
          else
            throw new EMorphDaemon("Cannot deregister this apartment");
      }
    }

    public RegisteredApartment Find(int ApartmentID)
    {
      lock (_Apartments)
      {
        RegisteredApartment apartment = (RegisteredApartment)_Apartments[ApartmentID];
        if (apartment == null)
          throw new EMorphDaemon("Apartment ID not found: " + ApartmentID.ToString());
        return apartment;
      }
    }

    static public RegisteredApartments Apartments = new RegisteredApartments();
    static public RegisteredApartments ApartmentProxies = new RegisteredApartments();
  }
}