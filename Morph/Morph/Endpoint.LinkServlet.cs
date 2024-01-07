using Morph.Base;
using Morph.Core;

namespace Morph.Endpoint
{
  public class LinkServlet : Link
  {
    internal LinkServlet(int ServletID)
      : base(LinkTypeID.Servlet)
    {
      _ServetID = ServletID;
    }

    private int _ServetID;
    public int ServletID
    {
      get { return _ServetID; }
    }

    #region Link implementation

    public override int Size()
    {
      return 5;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, false, false, false);
      Writer.WriteInt32(ServletID);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return (obj is LinkServlet) && (((LinkServlet)obj).ServletID == _ServetID);
    }

    public override int GetHashCode()
    {
      return _ServetID;
    }

    public override string ToString()
    {
      return "{Servlet ID=" + ServletID.ToString() + '}';
    }
  }

  public class LinkTypeServlet : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Servlet; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      Reader.ReadInt8();
      return new LinkServlet(Reader.ReadInt32());
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      //  Find the apartment
      if (!Message.ContextIs(typeof(MorphApartment)))
        throw new EMorph("Link type not supported by context");
      MorphApartment Apartment = (MorphApartment)Message.Context;
      //  Find the servlet
      Servlet Servlet = Apartment.Servlets.Find(((LinkServlet)CurrentLink).ServletID);
      if (Servlet == null)
        throw new EMorph("Servlet not found");
      //  Move along
      Message.Context = Servlet;
      Message.NextLinkAction();
    }
  }
}