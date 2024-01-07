using Morph.Lib;

namespace Morph
{
  public abstract class Link
  {
    protected Link(LinkTypeID LinkTypeID)
    {
      _LinkTypeID = LinkTypeID;
    }

    private LinkTypeID _LinkTypeID;
    public LinkTypeID LinkTypeID
    { get { return _LinkTypeID; } }

    public abstract int Size();
    public abstract void Write(MorphWriter Writer);
  }

  public interface IActionLink
  {
    void ActionLink(LinkMessage Message, Link CurrentLink);
  }

  public interface IActionLast
  {
    void ActionLast(LinkMessage Message);
  }
}