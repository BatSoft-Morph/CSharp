using System.Net;
using Morph.Core;

namespace Morph.Internet
{
  public class LinkInternetIPv4 : LinkInternet
  {
    public LinkInternetIPv4(IPEndPoint endPoint)
      : base(endPoint)
    {
    }

    static public LinkInternetIPv4 ReadNew(MorphReader reader, bool hasURI, bool hasPort)
    {
      //  Read host
      IPAddress address;
      if (hasURI)
        try
        { //  String
          address = IPAddress.Parse(reader.ReadString());
        }
        catch
        {
          throw new EMorph("Invalid IPv4 Address");
        }
      else
      { //  Binary
        byte[] host = new byte[4];
        host[0] = (byte)reader.ReadInt8();
        host[1] = (byte)reader.ReadInt8();
        host[2] = (byte)reader.ReadInt8();
        host[3] = (byte)reader.ReadInt8();
        address = new IPAddress(host);
      }
      //  Read port
      int port = LinkInternet.MorphPort;
      if (hasPort)
        port = reader.ReadInt16()&0x0000FFFF;
      //  Done
      return new LinkInternetIPv4(new IPEndPoint(address, port));
    }

    #region Link members

    public override int Size()
    {
      bool hasURI = false;// Host == null;  //  Is there ever a need for more than byte[4]?
      bool hasPort = EndPoint.Port != LinkInternet.MorphPort;
      int size = 1;
      if (hasURI)
        size += 4 + (EndPoint.Address.ToString().Length * 2);
      else
        size += 4;
      if (hasPort)
        size += 2;
      return size;
    }

    public override void Write(MorphWriter writer)
    {
      bool isIPv6 = false;
      bool isString = false;// Host == null;  //  Is there ever a need for more than byte[4]?
      bool hasPort = EndPoint.Port != LinkInternet.MorphPort;
      //  Link byte
      writer.WriteLinkByte(LinkTypeID, isIPv6, isString, hasPort);
      //  Host
      if (isString)
        writer.WriteString(EndPoint.Address.ToString());
      else
      {
        byte[] host = EndPoint.Address.GetAddressBytes();
        writer.WriteInt8(host[0]);
        writer.WriteInt8(host[1]);
        writer.WriteInt8(host[2]);
        writer.WriteInt8(host[3]);
      }
      // Port
      if (hasPort)
        writer.WriteInt16(EndPoint.Port);
    }

    #endregion
  }
}