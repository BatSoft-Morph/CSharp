using Morph.Base;
using Morph.Core;

namespace Morph.Endpoint
{
  public class LinkService : Link
  {
    protected internal LinkService(string ServiceName)
      : base(LinkTypeID.Service)
    {
      _ServiceName = ServiceName;
    }

    private string _ServiceName;
    public string ServiceName
    {
      get { return _ServiceName; }
    }

    #region Link implementation

    public override int Size()
    {
      return 5 + MorphWriter.SizeOfString(_ServiceName);
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, false, false, false);
      Writer.WriteString(_ServiceName);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return (obj is LinkService) && (((LinkService)obj)._ServiceName.ToLower().Equals(_ServiceName.ToLower()));
    }

    public override int GetHashCode()
    {
      return _ServiceName.GetHashCode();
    }

    public override string ToString()
    {
      return "{Service Name=" + ServiceName + '}';
    }
  }

  public class LinkApartment : Link
  {
    public LinkApartment(int ApartmentID)
      : base(LinkTypeID.Service)
    {
      _ApartmentID = ApartmentID;
    }

    public LinkApartment(MorphApartment Apartment)
      : this(Apartment.ID)
    {
    }

    private int _ApartmentID = MorphApartment.DefaultID;
    public int ApartmentID
    {
      get { return _ApartmentID; }
    }

    #region Link implementation

    public override int Size()
    {
      return 5;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, true, false, false);
      Writer.WriteInt32(ApartmentID);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return (obj is LinkApartment) && (((LinkApartment)obj)._ApartmentID == _ApartmentID);
    }

    public override int GetHashCode()
    {
      return _ApartmentID;
    }

    public override string ToString()
    {
      return "{Apartment ID=" + ApartmentID.ToString() + '}';
    }
  }

  public class LinkApartmentProxy : Link
  {
    public LinkApartmentProxy(int ApartmentProxyID)
      : base(LinkTypeID.Service)
    {
      _ApartmentProxyID = ApartmentProxyID;
    }

    private int _ApartmentProxyID;
    public int ApartmentProxyID
    {
      get { return _ApartmentProxyID; }
    }

    #region Link implementation

    public override int Size()
    {
      return 5;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, true, true, false);
      Writer.WriteInt32(_ApartmentProxyID);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return (obj is LinkApartmentProxy) && (((LinkApartmentProxy)obj)._ApartmentProxyID == _ApartmentProxyID);
    }

    public override int GetHashCode()
    {
      return _ApartmentProxyID;
    }

    public override string ToString()
    {
      return "{ApartmentProxy ID=" + ApartmentProxyID.ToString() + '}';
    }
  }

  public class LinkTypeService : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Service; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool IsApartment, IsProxy, z;
      Reader.ReadLinkByte(out IsApartment, out IsProxy, out z);
      if (IsApartment)
        if (IsProxy)
          return new LinkApartmentProxy(Reader.ReadInt32());
        else
          return new LinkApartment(Reader.ReadInt32());
      else
        return new LinkService(Reader.ReadString());
    }

    protected virtual void ActionLinkService(LinkMessage Message, LinkService LinkService)
    {
      //  Obtain an apartment (create new or get shared)
      MorphApartment apartment = MorphServices.Obtain(LinkService.ServiceName).ApartmentFactory.ObtainDefault();
      //  Replace the service link with an apartment link
      Message.PathTo.Pop();
      Message.PathTo.Push(new LinkApartment(apartment));
      //  Update the path to the client side
      if (apartment is MorphApartmentSession)
        ((MorphApartmentSession)apartment).Path = Message.PathFrom;
      //  Action the new link
      LinkTypes.ActionCurrentLink(Message);
    }

    protected virtual void ActionLinkApartment(LinkMessage Message, LinkApartment LinkApartment)
    {
      //  Find the apartment
      MorphApartment Apartment = MorphApartmentFactory.Obtain(LinkApartment.ApartmentID);
      //  If it's a session apartment, then update the path
      if ((Apartment is MorphApartmentSession) && Message.HasPathFrom)
      {
        Message.PathFrom.PeekAll();
        ((MorphApartmentSession)Apartment).Path = Message.PathFrom.Clone();
      }
      //  Move along
      Message.Context = Apartment;
      Message.NextLinkAction();
    }

    protected virtual void ActionLinkApartmentProxy(LinkMessage Message, LinkApartmentProxy LinkApartmentProxy)
    {
      //  Find the apartment proxy
      MorphApartmentProxy ApartmentProxy = MorphApartmentProxy.Find(LinkApartmentProxy.ApartmentProxyID);
      if (ApartmentProxy == null)
        throw new EMorph("Apartment proxy not found");
      //  Move along
      Message.Context = ApartmentProxy;
      Message.NextLinkAction();
    }

    public virtual void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      if (CurrentLink is LinkService) ActionLinkService(Message, (LinkService)CurrentLink);
      if (CurrentLink is LinkApartment) ActionLinkApartment(Message, (LinkApartment)CurrentLink);
      if (CurrentLink is LinkApartmentProxy) ActionLinkApartmentProxy(Message, (LinkApartmentProxy)CurrentLink);
    }
  }
}