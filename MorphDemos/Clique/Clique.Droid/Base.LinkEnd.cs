using System;
using Morph.Core;

namespace Morph.Base
{
  public class LinkEnd : Link
  {
    internal LinkEnd()
      : base(LinkTypeID.End)
    {
    }

    #region Link

    public override int Size()
    {
      return 1;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteInt8((byte)LinkTypeID.End);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return obj is LinkEnd;
    }

    public override int GetHashCode()
    {
      return 0;
    }

    public override string ToString()
    {
      return "{End}";
    }
  }

  public class LinkTypeEnd : ILinkTypeReader, ILinkTypeAction
  {
    static private LinkEnd _End = new LinkEnd();
    static public LinkEnd End
    {
      get { return _End; }
    }

    public LinkTypeID ID
    {
      get { return LinkTypeID.End; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      Reader.ReadInt8();
      return _End;
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      if (Message.ContextIs(typeof(IDisposable)))
        ((IDisposable)Message.Context).Dispose();
    }
  }
}