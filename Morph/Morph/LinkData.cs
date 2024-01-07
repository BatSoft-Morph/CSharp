using Morph.Lib;

namespace Morph
{
  public class LinkData : Link
  {
    public LinkData(byte[] Data, ValueTypeStringData ValueTypeStringData, bool MSB)
      : base(LinkTypeID.Data)
    {
      _Data = Data;
      _MSB = MSB;
      _ValueTypeStringData = ValueTypeStringData;
    }

    public LinkData(MorphWriter Writer)
      : base(LinkTypeID.Data)
    {
      _Data = Writer.ToArray();
      _MSB = Writer.MSB;
      //  Values that describe this implementation
      _ValueTypeStringData.ValueTypeStringLength = 0;
      _ValueTypeStringData.ValueTypeIsUnicode = false;
    }

    private byte[] _Data;
    public byte[] Data
    {
      get { return _Data; }
    }

    private ValueTypeStringData _ValueTypeStringData;
    public ValueTypeStringData ValueTypeStringData
    {
      get { return _ValueTypeStringData; }
    }

    private bool _MSB;
    public bool MSB
    {
      get { return _MSB; }
    }

    public MorphReader Reader
    {
      get { return new MorphReaderSized(_Data, _MSB); }
    }

    #region Link

    public override int Size()
    {
      return 5 + _Data.Length;
    }

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, _ValueTypeStringData.ValueTypeStringLengthX, _ValueTypeStringData.ValueTypeStringLengthY, _ValueTypeStringData.ValueTypeIsUnicode);
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
      return "{Data Length=" + Data.Length.ToString() + '}';
    }
  }

  public interface IActionLinkData
  {
    void ActionLinkData(LinkMessage Message, LinkData Data);
  }

  public struct ValueTypeStringData
  {
    public byte ValueTypeStringLength
    {
      get { return (byte)((ValueTypeStringLengthX ? 0x01 : 0x00) | (ValueTypeStringLengthY ? 0x02 : 0x00)); }
      set
      {
        ValueTypeStringLengthX = (ValueTypeStringLength & 0x01) != 0;
        ValueTypeStringLengthY = (ValueTypeStringLength & 0x02) != 0;
      }
    }
    public bool ValueTypeStringLengthX;
    public bool ValueTypeStringLengthY;

    public bool ValueTypeIsUnicode;

    static public ValueTypeStringData Default
    {
      get
      {
        ValueTypeStringData Result;
        Result.ValueTypeStringLengthX = false;
        Result.ValueTypeStringLengthY = false;
        Result.ValueTypeIsUnicode = false;
        return Result;
      }
    }
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
      ValueTypeStringData ValueTypeStringData;
      Reader.ReadLinkByte(out ValueTypeStringData.ValueTypeStringLengthX, out ValueTypeStringData.ValueTypeStringLengthY, out ValueTypeStringData.ValueTypeIsUnicode);
      int Size = Reader.ReadInt32();
      return new LinkData(Reader.ReadBytes(Size), ValueTypeStringData, Reader.MSB);
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