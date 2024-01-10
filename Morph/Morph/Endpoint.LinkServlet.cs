using Morph.Base;
using Morph.Core;

namespace Morph.Endpoint
{
  public class LinkServlet : Link
  {
    internal LinkServlet(int servletID)
      : base(LinkTypeID.Servlet)
    {
      _servetID = servletID;
    }

    private readonly int _servetID;
    public int ServletID
    {
      get => _servetID;
    }

    #region Link implementation

    public override int Size()
    {
      return 5;
    }

    public override void Write(MorphWriter writer)
    {
      writer.WriteLinkByte(LinkTypeID, false, false, false);
      writer.WriteInt32(ServletID);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return (obj is LinkServlet linkServlet) && (linkServlet.ServletID == _servetID);
    }

    public override int GetHashCode()
    {
      return _servetID;
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
      get => LinkTypeID.Servlet;
    }

    public Link ReadLink(MorphReader reader)
    {
      reader.ReadInt8();
      return new LinkServlet(reader.ReadInt32());
    }

    public void ActionLink(LinkMessage message, Link currentLink)
    {
      //  Find the apartment
      if (!message.ContextIs(typeof(MorphApartment)))
        throw new EMorph("Link type not supported by context");
      MorphApartment apartment = (MorphApartment)message.Context;
      //  Find the servlet
      Servlet servlet = apartment.Servlets.Find(((LinkServlet)currentLink).ServletID);
      if (servlet == null)
        throw new EMorph("Servlet not found");
      //  Move along
      message.Context = servlet;
      message.NextLinkAction();
    }
  }
}