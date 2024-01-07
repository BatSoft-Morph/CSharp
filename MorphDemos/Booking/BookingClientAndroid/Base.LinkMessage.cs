using System;
using Morph.Core;

namespace Morph.Base
{
  public class LinkMessage : Link
  {
    public LinkMessage(LinkStack PathTo, LinkStack PathFrom, bool IsForceful)
      : base(LinkTypeID.Message)
    {
      _PathTo = PathTo;
      _PathFrom = PathFrom;
      _IsForceful = IsForceful;
    }

    internal protected LinkMessage(MorphReader Reader)
      : base(LinkTypeID.Message)
    {
      //  Read link byte
      bool HasPathFrom;
      Reader.ReadLinkByte(out _HasCallNumber, out _IsForceful, out HasPathFrom);
      //  Read link values
      //  - CallNumber
      if (_HasCallNumber)
        _CallNumber = Reader.ReadInt32();
      //  - PathTo size
      int PathToSize = Reader.ReadInt32();
      //  - PathFrom size
      int PathFromSize = 0;
      if (HasPathFrom)
        PathFromSize = Reader.ReadInt32();
      //  Paths
      _PathTo = new LinkStack(Reader.SubReader(PathToSize));
      if (HasPathFrom)
        _PathFrom = new LinkStack(Reader.SubReader(PathFromSize));
    }

    private bool _HasCallNumber = false;
    public bool HasCallNumber
    {
      get { return _HasCallNumber; }
    }

    private int _CallNumber;
    public int CallNumber
    {
      get
      {
        if (!_HasCallNumber)
          throw new EMorphImplementation();
        return _CallNumber;
      }
      set
      {
        _CallNumber = value;
        _HasCallNumber = true;
      }
    }

    private bool _IsForceful;
    public bool IsForceful
    {
      get { return _IsForceful; }
      set { _IsForceful = value; }
    }

    private LinkStack _PathTo = null;
    public LinkStack PathTo
    {
      get { return _PathTo; }
    }

    public bool HasPathFrom
    {
      get { return _PathFrom != null; }
    }

    private LinkStack _PathFrom = null;
    public LinkStack PathFrom
    {
      get { return _PathFrom; }
    }

    public object Source = null;  //  The connection that the message comes from

    public object Context = null;

    public bool ContextIs(Type ContextType)
    {
      return (Context != null) && ContextType.IsInstanceOfType(Context);
    }

    public Link Current
    {
      get
      {
        if (_PathTo == null)
          return null;
        else
          return _PathTo.Peek();
      }
    }

    public bool CurrentIs(LinkTypeID LinkTypeID)
    {
      Link CurrentLink = Current;
      return (CurrentLink != null) && (CurrentLink.LinkTypeID == LinkTypeID);
    }

    public void NextLink()
    {
      if (_PathFrom != null)
        _PathFrom.Push(_PathTo.Pop());
      else
        _PathTo.Pop();
    }

    public void NextLinkAction()
    {
      NextLink();
      LinkTypes.ActionCurrentLink(this);
    }

    public void Action()
    {
      LinkTypes.ActionCurrentLink(this);
    }

    #region Link

    public override int Size()
    {
      long TotalSize = 1;
      //  Call number
      if (HasCallNumber)
        TotalSize += 4;
      //  Path To size
      TotalSize += 4 + PathTo.ByteSize;
      //  Path From size
      if (HasPathFrom)
        TotalSize += 4 + PathFrom.ByteSize;
      //  Analyze limits
      if (TotalSize > int.MaxValue)
        throw new EMorph("Message is too large");
      return (int)TotalSize;
    }

    public override void Write(MorphWriter Writer)
    {
      lock (Writer)
      {
        //  Message link byte
        Writer.WriteLinkByte(LinkTypeID, _HasCallNumber, _IsForceful, HasPathFrom);
        //  Call number
        if (_HasCallNumber)
          Writer.WriteInt32(CallNumber);
        //  Path to size
        Writer.WriteInt32(PathTo.ByteSize);
        //  Path from size
        if (HasPathFrom)
          Writer.WriteInt32(PathFrom.ByteSize);
        //  Path to
        PathTo.Write(Writer);
        //  Path from
        if (HasPathFrom)
          PathFrom.Write(Writer);
      }
    }

    #endregion

    public LinkMessage Clone()
    {
      LinkMessage clone = new LinkMessage(PathTo, PathFrom, _IsForceful);
      clone._HasCallNumber = this._HasCallNumber;
      clone._CallNumber = this._CallNumber;
      if (this._PathTo != null)
        clone._PathTo = this._PathTo.Clone();
      if (this._PathFrom != null)
        clone._PathFrom = this._PathFrom.Clone();
      return clone;
    }

    public LinkMessage CreateReply()
    {
      if (!HasPathFrom)
        return null;
      LinkMessage Reply = new LinkMessage(PathFrom, HasPathFrom ? new LinkStack() : null, _IsForceful);
      if (HasCallNumber)
        Reply.CallNumber = CallNumber;
      Reply.IsForceful = IsForceful;
      return Reply;
    }

    public LinkMessage CreateReply(Exception x)
    {
      if (!HasPathFrom)
        return null;
      LinkMessage Reply = new LinkMessage(PathFrom, null, _IsForceful);
      if (HasCallNumber)
        Reply.CallNumber = CallNumber;
      Reply.IsForceful = IsForceful;
      Reply.PathTo.Append(new LinkData(x));
      return Reply;
    }

    public LinkMessage CreateReply(Link Payload)
    {
      LinkMessage Reply = CreateReply();
      if (Reply == null)
        return null;
      Reply.PathTo.Append(Payload);
      return Reply;
    }

    public LinkMessage CreateReply(LinkStack Payload)
    {
      LinkMessage Reply = CreateReply();
      if (Reply == null)
        return null;
      Reply.PathTo.Append(Payload);
      return Reply;
    }

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
      string str = "{Message";
      if (HasCallNumber)
        str += " CallNumber=" + CallNumber.ToString();
      if (_IsForceful)
        str += " IsForceful";
      str += " To=" + PathTo.ToString();
      if (HasPathFrom)
        str += " From=" + PathFrom.ToString();
      return str + '}';
    }
  }

  public class LinkTypeMessage : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Message; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      return new LinkMessage(Reader);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      throw new NotImplementedException();
    }
  }
}