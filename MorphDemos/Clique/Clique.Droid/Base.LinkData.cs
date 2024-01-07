using System;
using Morph.Core;
using Morph.Params;

namespace Morph.Base
{
  public class LinkData : Link
  {
    public LinkData(byte[] Data, bool MSB, bool IsException, int ErrorCode)
      : base(LinkTypeID.Data)
    {
      _Data = Data;
      _MSB = MSB;
      _IsException = IsException;
      _ErrorCode = ErrorCode;
    }

    public LinkData(MorphWriter Writer)
      : base(LinkTypeID.Data)
    {
      _Data = Writer.ToArray();
      _MSB = Writer.MSB;
      _IsException = false;
    }

    public LinkData(Exception x)
      : base(LinkTypeID.Data)
    {
      MorphWriter Writer = Parameters.Encode(null, x, ExceptionInstanceFactory);
      _Data = Writer.ToArray();
      _MSB = Writer.MSB;
      _IsException = true;
      if (x is EMorph)
        _ErrorCode = ((EMorph)x).ErrorCode;
      else
        _ErrorCode = 0;
    }

    static private InstanceFactories ExceptionInstanceFactory = new InstanceFactories();

    private byte[] _Data;
    public byte[] Data
    {
      get { return _Data; }
    }

    private bool _MSB;
    public bool MSB
    {
      get { return _MSB; }
    }

    private bool _IsException;
    public bool IsException
    {
      get { return _IsException; }
    }

    private int _ErrorCode;
    public int ErrorCode
    {
      get { return _ErrorCode; }
    }

    public MorphReader Reader
    {
      get { return new MorphReaderSized(_Data, _MSB); }
    }

    #region Link

    public override int Size()
    {
      return 5 + (_IsException ? 4 : 0) + _Data.Length;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, _IsException, false, false);
      if (_IsException)
        Writer.WriteInt32(_ErrorCode);
      Writer.WriteInt32(_Data.Length);
      Writer.WriteBytes(_Data);
    }

    #endregion

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return _Data.GetHashCode();
    }

    public override string ToString()
    {
      string str = "{Data";
      if (IsException)
      {
        str += " Error";
        if (ErrorCode != 0)
          str += '=' + ErrorCode.ToString();
      }
      return str + " Length=" + Data.Length.ToString() + '}';
    }
  }

  public interface IActionLinkData
  {
    void ActionLinkData(LinkMessage Message, LinkData Data);
  }

  public class LinkTypeData : ILinkTypeReader, ILinkTypeAction
  {
    #region LinkType Members

    public LinkTypeID ID
    {
      get { return LinkTypeID.Data; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool IsException, y, z;
      Reader.ReadLinkByte(out IsException, out y, out z);
      int ErrorCode = 0;
      if (IsException)
        ErrorCode = Reader.ReadInt32();
      int Size = Reader.ReadInt32();
      byte[] Data = Reader.ReadBytes(Size);
      return new LinkData(Data, Reader.MSB, IsException, ErrorCode);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      if (!Message.ContextIs(typeof(IActionLinkData)))
        throw new EMorph("Link type not supported by context");
      ((IActionLinkData)Message.Context).ActionLinkData(Message, (LinkData)CurrentLink);
    }

    #endregion
  }
}