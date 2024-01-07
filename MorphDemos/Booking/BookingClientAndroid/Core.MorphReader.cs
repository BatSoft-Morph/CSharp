using System;
using System.Text;

namespace Morph.Core
{
  public abstract class MorphReader
  {
    public abstract MorphReaderSized SubReader(int Count);

    static protected Encoding ASCII = new ASCIIEncoding();
    static protected Encoding Unicode = new UnicodeEncoding();

    private int ReadByteCount(byte ByteCountSize)
    {
      switch (ByteCountSize)
      {
        case 0: return ReadInt8();
        case 1: return ReadInt16();
        case 2: return ReadInt32();
        case 3:
          { //  This implementation cannot handle byte lengths greater than int.MaxValue because of limitations in the Stream class.
            long Count = ReadInt64();
            if (Count >= int.MaxValue)
              throw new EMorphImplementation();
            return (int)Count;
          }
        default: throw new EMorphImplementation();
      }
    }

    #region Link byte

    private const byte BitX = 0x10;
    private const byte BitY = 0x20;
    private const byte BitZ = 0x40;
    private const byte BitMSB = 0x80;

    static public LinkTypeID DecodeLinkByte(byte LinkByte, out bool x, out bool y, out bool z)
    {
      x = (LinkByte & BitX) != 0;
      y = (LinkByte & BitY) != 0;
      z = (LinkByte & BitZ) != 0;
      return (LinkTypeID)(LinkByte & 0x0F);
    }

    private LinkTypeID DecodeLinkByte(byte LinkByte, out bool x, out bool y, out bool z, out bool MSB)
    {
      x = (LinkByte & BitX) != 0;
      y = (LinkByte & BitY) != 0;
      z = (LinkByte & BitZ) != 0;
      MSB = (LinkByte & BitMSB) != 0;
      return (LinkTypeID)(LinkByte & 0x0F);
    }

    public LinkTypeID PeekLinkByte(out bool x, out bool y, out bool z)
    {
      return DecodeLinkByte(PeekInt8(), out x, out y, out z, out _MSB);
    }

    public LinkTypeID ReadLinkByte(out bool x, out bool y, out bool z)
    {
      return DecodeLinkByte(ReadInt8(), out x, out y, out z, out _MSB);
    }

    #endregion

    protected bool _MSB;
    public bool MSB
    {
      get { return _MSB; }
    }

    public abstract bool CanRead
    { get; }

    public abstract long Remaining
    { get; }

    public abstract byte PeekInt8();

    public abstract byte ReadInt8();

    public virtual Int16 ReadInt16()
    {
      byte b0 = ReadInt8();
      byte b1 = ReadInt8();
      if (_MSB)
        return (Int16)((b0 << 8) | b1);
      else
        return (Int16)((b1 << 8) | b0);
    }

    public virtual Int32 ReadInt32()
    {
      byte b0 = ReadInt8();
      byte b1 = ReadInt8();
      byte b2 = ReadInt8();
      byte b3 = ReadInt8();
      if (_MSB)
        return (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
      else
        return (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
    }

    public virtual Int64 ReadInt64()
    {
      byte b0 = ReadInt8();
      byte b1 = ReadInt8();
      byte b2 = ReadInt8();
      byte b3 = ReadInt8();
      byte b4 = ReadInt8();
      byte b5 = ReadInt8();
      byte b6 = ReadInt8();
      byte b7 = ReadInt8();
      if (_MSB)
        return (b0 << 56) | (b1 << 48) | (b2 << 40) | (b3 << 32) | (b4 << 24) | (b5 << 16) | (b6 << 8) | b7;
      else
        return (b7 << 56) | (b6 << 48) | (b5 << 40) | (b4 << 32) | (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
    }

    public string ReadString()
    {
      return ReadChars(ReadInt32(), true);
    }

    //  ByteCountSize limited to 0..3
    public string ReadString(byte ByteCountSize, bool AsUnicode)
    {
      return ReadChars(ReadByteCount(ByteCountSize), true);
    }

    public string ReadIdentifier()
    {
      return ReadChars(ReadInt16(), true);
    }

    public virtual string ReadChars(int ByteCount, bool AsUnicode)
    {
      byte[] bytes = ReadBytes(ByteCount);
      if (AsUnicode)
        return Unicode.GetString(bytes);
      else
        return ASCII.GetString(bytes);
    }

    public abstract byte[] ReadBytes(int Count);

    public abstract int ReadBytes(byte[] Buffer);
  }

  public class MorphReaderSizeless : MorphReader
  {
    public MorphReaderSizeless(MorphStream Stream)
    {
      _Stream = Stream;
    }

    public override MorphReaderSized SubReader(int Count)
    {
      return new MorphReaderSized(_Stream.Read(Count), _MSB);
    }

    private MorphStream _Stream;

    public override bool CanRead
    {
      get { return _Stream.Remaining > 0; }
    }

    public override long Remaining
    {
      get { return (long)_Stream.Remaining; }
    }

    public override byte PeekInt8()
    {
      return _Stream.Peek();
    }

    public override byte ReadInt8()
    {
      return (byte)_Stream.ReadByte();
    }

    public override byte[] ReadBytes(int Count)
    {
      if (_Stream.Remaining < Count)
        throw new EMorph("EOS");
      byte[] bytes = new byte[Count];
      if (_Stream.Read(bytes, 0, Count) < Count)
        throw new EMorphImplementation();
      return bytes;
    }

    public override int ReadBytes(byte[] Buffer)
    {
      return _Stream.Read(Buffer, 0, Buffer.Length);
    }
  }

  public class MorphReaderSized : MorphReader
  {
    public MorphReaderSized(byte[] Bytes)
      : this(Bytes, 0, Bytes.Length)
    {
    }

    internal MorphReaderSized(byte[] Bytes, bool MSB)
      : this(Bytes, 0, Bytes.Length)
    {
      _MSB = MSB;
    }

    private MorphReaderSized(byte[] Bytes, int Pos, int Count)
    {
      _Bytes = Bytes;
      _Pos = Pos;
      _End = Pos + Count;
      if (_Bytes.Length < _End)
        throw new EMorphUsage("");
    }

    public override MorphReaderSized SubReader(int Count)
    {
      try
      {
        return new MorphReaderSized(_Bytes, _Pos, Count);
      }
      finally
      {
        _Pos += Count;
      }
    }

    private byte[] _Bytes;
    private int _Pos;
    private int _End;

    public override bool CanRead
    {
      get { return _Pos < _End; }
    }

    public override long Remaining
    {
      get { return _End - _Pos; }
    }

    private void ValidateRead(int Count)
    {
      if (_Pos + Count > _End)
        throw new EMorph("EOS");
    }

    public override byte PeekInt8()
    {
      if (_Pos >= _End)
        throw new EMorph("EOS");
      return _Bytes[_Pos];
    }

    public override byte ReadInt8()
    {
      if (_Pos >= _End)
        throw new EMorph("EOS");
      return _Bytes[_Pos++];
    }

    public override Int16 ReadInt16()
    {
      ValidateRead(2);
      byte b0 = _Bytes[_Pos++];
      byte b1 = _Bytes[_Pos++];
      if (_MSB)
        return (Int16)((b0 << 8) | b1);
      else
        return (Int16)((b1 << 8) | b0);
    }

    public override Int32 ReadInt32()
    {
      ValidateRead(4);
      byte b0 = _Bytes[_Pos++];
      byte b1 = _Bytes[_Pos++];
      byte b2 = _Bytes[_Pos++];
      byte b3 = _Bytes[_Pos++];
      if (_MSB)
        return (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
      else
        return (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
    }

    public override Int64 ReadInt64()
    {
      ValidateRead(8);
      byte b0 = _Bytes[_Pos++];
      byte b1 = _Bytes[_Pos++];
      byte b2 = _Bytes[_Pos++];
      byte b3 = _Bytes[_Pos++];
      byte b4 = _Bytes[_Pos++];
      byte b5 = _Bytes[_Pos++];
      byte b6 = _Bytes[_Pos++];
      byte b7 = _Bytes[_Pos++];
      if (_MSB)
        return (b0 << 56) | (b1 << 48) | (b2 << 40) | (b3 << 32) | (b4 << 24) | (b5 << 16) | (b6 << 8) | b7;
      else
        return (b7 << 56) | (b6 << 48) | (b5 << 40) | (b4 << 32) | (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
    }

    public override byte[] ReadBytes(int Count)
    {
      ValidateRead(Count);
      byte[] bytes = new byte[Count];
      Array.Copy(_Bytes, _Pos, bytes, 0, Count);
      _Pos += Count;
      return bytes;
    }

    public override int ReadBytes(byte[] Buffer)
    {
      int Count = (int)Remaining < Buffer.Length ? (int)Remaining : Buffer.Length;
      ValidateRead(Count);
      Array.Copy(_Bytes, _Pos, Buffer, 0, Count);
      _Pos += Count;
      return Count;
    }

    public override string ReadChars(int ByteCount, bool AsUnicode)
    {
      ValidateRead(ByteCount);
      return base.ReadChars(ByteCount, AsUnicode);
    }
  }
}