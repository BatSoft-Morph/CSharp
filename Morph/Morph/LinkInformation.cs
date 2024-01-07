using Morph.Lib;

namespace Morph
{
  public class LinkInformation : Link
  {
    internal LinkInformation()
      : base(LinkTypeID.Information)
    {
    }

    #region Link

    public override int Size()
    {
      throw new System.Exception("The method or operation is not implemented.");
    }

    public override void Write(MorphWriter Writer)
    {
      throw new System.Exception("The method or operation is not implemented.");
    }

    #endregion

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return "{Info}";
    }
  }

  public class LinkTypeReaderInformation : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Information; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      throw new System.Exception("The method or operation is not implemented.");
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      throw new System.Exception("The method or operation is not implemented.");
    }
  }
}