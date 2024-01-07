using System.IO;
using System.Text;

namespace Morph.Lib
{
  public class MorphWriter
  {
    public MorphWriter(MemoryStream Stream)
    {
      _Stream = Stream;
    }

    static private Encoding ASCII = new ASCIIEncoding();
    static private Encoding Unicode = new UnicodeEncoding();

    private MemoryStream _Stream;
    private const int BufferSize = 256;

    public Stream Stream
    {
      get { return _Stream; }
    }

    static public int SizeOfString(string Chars)
    {
      return Unicode.GetByteCount(Chars);
    }

    #region Link byte

    private const byte Zero = 0x00;
    private const byte BitX = 0x10;
    private const byte BitY = 0x20;
    private const byte BitZ = 0x40;
    private const byte BitMSB = 0x80;

    static public byte EncodeLinkByte(LinkTypeID LinkTypeID, bool x, bool y, bool z)
    {
      return (byte)(BitMSB | (z ? BitZ : Zero) | (y ? BitY : Zero) | (x ? BitX : Zero) | (byte)LinkTypeID);
    }

    public void WriteLinkByte(LinkTypeID LinkTypeID, bool x, bool y, bool z)
    {
      WriteInt8(BitMSB | (z ? BitZ : Zero) | (y ? BitY : Zero) | (x ? BitX : Zero) | (byte)LinkTypeID);
    }

    #endregion

    public bool MSB
    {
      get { return true; }
    }

    public void WriteInt8(int Value)
    {
      _Stream.WriteByte((byte)Value);
    }

    public void WriteInt16(int Value)
    {
      _Stream.WriteByte((byte)(Value >> 8));
      _Stream.WriteByte((byte)(Value));
    }

    public void WriteInt32(int Value)
    {
      _Stream.WriteByte((byte)(Value >> 24));
      _Stream.WriteByte((byte)(Value >> 16));
      _Stream.WriteByte((byte)(Value >> 8));
      _Stream.WriteByte((byte)(Value));
    }

    public void WriteInt64(long Value)
    {
      _Stream.WriteByte((byte)(Value >> 56));
      _Stream.WriteByte((byte)(Value >> 48));
      _Stream.WriteByte((byte)(Value >> 40));
      _Stream.WriteByte((byte)(Value >> 32));
      _Stream.WriteByte((byte)(Value >> 24));
      _Stream.WriteByte((byte)(Value >> 16));
      _Stream.WriteByte((byte)(Value >> 8));
      _Stream.WriteByte((byte)(Value));
    }

    public void WriteString(string Value)
    {
      byte[] buffer = Unicode.GetBytes(Value);
      WriteInt32(buffer.Length);
      _Stream.Write(buffer, 0, buffer.Length);
    }

    public void WriteChars(string Chars, bool AsUnicode)
    {
      byte[] buffer;
      if (AsUnicode)
        buffer = Unicode.GetBytes(Chars);
      else
        buffer = ASCII.GetBytes(Chars);
      _Stream.Write(buffer, 0, buffer.Length);
    }

    public void WriteBytes(byte[] Buffer)
    {
      _Stream.Write(Buffer, 0, Buffer.Length);
    }

    public void WriteStream(MorphReader Reader)
    {
      while (Reader.Remaining > 0)
        WriteStream(Reader, Reader.Remaining < int.MaxValue ? (int)Reader.Remaining : int.MaxValue);
    }

    public void WriteStream(MorphReader Reader, int Count)
    {
      if (Count == 0)
        return;
      byte[] Buffer = new byte[Count < BufferSize ? Count : BufferSize];
      int Length;
      do
      {
        Length = Reader.ReadBytes(Buffer);
        _Stream.Write(Buffer, 0, Length);
        Count -= Length;
      }
      while ((Count > 0) && (Length > 0));
      if (Count > 0)
        throw new EMorph("EOS");
    }

    public byte[] ToArray()
    {
      return _Stream.ToArray();
    }
  }
}