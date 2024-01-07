#region Description
/* Essentially, an apartment represents a logical memory space on a device.
 * Servlets represent objects within that memory space.
 * 
 * For any remote procedure call, an apartment represents the called endpoint.
 * So, a method call will be sent from an apartment proxy to an apartment, and
 * the method reply will be sent from the apartment to the apartment proxy.
 */
#endregion

#region Client side usage
#endregion

using System;
using System.Collections.Generic;
using Morph.Base;
using Morph.Core;
using Morph.Lib;
using Morph.Params;
using Morph.Sequencing;

namespace Morph.Endpoint
{
  public abstract class MorphApartment : RegisterItemID, IDisposable, IActionLinkSequence, IActionLast
  {
    protected MorphApartment(MorphApartmentFactory Owner, InstanceFactories InstanceFactories, object DefaultObject)
    {
      if (InstanceFactories == null)
        throw new EMorphUsage("Apartment must have InstanceFactories object");
      _InstanceFactories = InstanceFactories;
      _Owner = Owner;
      _ID = IDFactory.Generate();
      _Servlets = new Servlets(this, DefaultObject);
      MorphApartmentFactory.RegisterApartment(this);
    }

    #region IDisposable Members

    public virtual void Dispose()
    {
      try
      {
        IDFactory.Release(ID);
      }
      finally
      {
        MorphApartmentFactory.UnregisterApartment(this);
      }
    }

    #endregion

    #region RegisterItemID Members

    private int _ID;
    public int ID
    {
      get { return _ID; }
    }

    #endregion

    public const int DefaultID = 0;

    static public IIDFactory IDFactory = new IDSeed(DefaultID + 1);

    private MorphApartmentFactory _Owner;
    public MorphApartmentFactory Owner
    {
      get { return _Owner; }
    }

    public virtual void ResetTimeout()
    {
      //  May be used by subclass
    }

    public virtual Servlet DefaultServlet
    {
      get { return Servlets.Find(Servlet.DefaultID); }
    }

    private Servlets _Servlets;
    public Servlets Servlets
    {
      get { return _Servlets; }
    }

    private InstanceFactories _InstanceFactories;
    public InstanceFactories InstanceFactories
    {
      get { return _InstanceFactories; }
    }

    #region IActionLinkSequence

    public virtual void ActionLinkSequence(LinkMessage Message, LinkSequence LinkSequence)
    {
      //  Shared apartments don't use sequences
      throw new EMorph("Unexpected link type");
    }

    #endregion

    #region IActionLast

    public void ActionLast(LinkMessage Message)
    {
      Message.CreateReply().NextLinkAction();
    }

    #endregion
  }

  public interface DefaultServletObjectFactory
  {
    object ObtainServlet();
  }

  public abstract class MorphApartmentFactory
  {
    protected MorphApartmentFactory(InstanceFactories InstanceFactories)
    {
      _InstanceFactories = InstanceFactories;
    }

    internal MorphService _Service;

    private InstanceFactories _InstanceFactories;
    public InstanceFactories InstanceFactories
    {
      get { return _InstanceFactories; }
    }

    static private RegisterItems<MorphApartment> All = new RegisterItems<MorphApartment>();

    static internal void RegisterApartment(MorphApartment Apartment)
    {
      lock (All)
        MorphApartmentFactory.All.Add(Apartment);
    }

    static internal void UnregisterApartment(MorphApartment Apartment)
    {
      lock (All)
        MorphApartmentFactory.All.Remove(Apartment);
    }

    static public MorphApartment Find(int ApartmentID)
    {
      lock (All)
        return All.Find(ApartmentID);
    }

    static public MorphApartment Obtain(int ApartmentID)
    {
      if (ApartmentID == MorphApartment.DefaultID)
        throw new EMorph("Obtain a default apartment by specifing a service.");
      //  Lookup existing apartment
      MorphApartment apartment = Find(ApartmentID);
      if (apartment == null)
        throw new EMorph("Apartment not found");
      //  Keep session apartments alive
      apartment.ResetTimeout();
      return apartment;
    }

    public abstract MorphApartment ObtainDefault();

    protected internal virtual void ShutDown()
    {
      lock (All)
      {
        List<MorphApartment> AllApartments = All.List();
        foreach (MorphApartment apartment in AllApartments)
          apartment.Dispose();
      }
    }
  }
}