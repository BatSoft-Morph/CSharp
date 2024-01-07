using System.Net;
using Morph.Core;

namespace Morph.Internet
{
  public class LinkInternetIPv4 : LinkInternet
  {
    public LinkInternetIPv4(IPEndPoint EndPoint)
      : base(EndPoint)
    {
    }

    static public LinkInternetIPv4 ReadNew(MorphReader Reader, bool HasURI, bool HasPort)
    {
      //  Read host
      IPAddress address;
      if (HasURI)
        try
        { //  String
          address = IPAddress.Parse(Reader.ReadString());
        }
        catch
        {
          throw new EMorph("Invalid IPv4 Address");
        }
      else
      { //  Binary
        byte[] host = new byte[4];
        host[0] = (byte)Reader.ReadInt8();
        host[1] = (byte)Reader.ReadInt8();
        host[2] = (byte)Reader.ReadInt8();
        host[3] = (byte)Reader.ReadInt8();
        address = new IPAddress(host);
      }
      //  Read port
      int port = LinkInternet.MorphPort;
      if (HasPort)
        port = Reader.ReadInt16()&0x0000FFFF;
      //  Done
      return new LinkInternetIPv4(new IPEndPoint(address, port));
    }

    #region Link members

    public override int Size()
    {
      bool HasURI = false;// Host == null;  //  Is there ever a need for more than byte[4]?
      bool HasPort = EndPoint.Port != LinkInternet.MorphPort;
      int size = 1;
      if (HasURI)
        size += 4 + (EndPoint.Address.ToString().Length * 2);
      else
        size += 4;
      if (HasPort)
        size += 2;
      return size;
    }

    public override void Write(MorphWriter Writer)
    {
      bool IsIPv6 = false;
      bool IsString = false;// Host == null;  //  Is there ever a need for more than byte[4]?
      bool HasPort = EndPoint.Port != LinkInternet.MorphPort;
      //  Link byte
      Writer.WriteLinkByte(LinkTypeID, IsIPv6, IsString, HasPort);
      //  Host
      if (IsString)
        Writer.WriteString(EndPoint.Address.ToString());
      else
      {
        byte[] host = EndPoint.Address.GetAddressBytes();
        Writer.WriteInt8(host[0]);
        Writer.WriteInt8(host[1]);
        Writer.WriteInt8(host[2]);
        Writer.WriteInt8(host[3]);
      }
      // Port
      if (HasPort)
        Writer.WriteInt16(EndPoint.Port);
    }

    #endregion
  }
}