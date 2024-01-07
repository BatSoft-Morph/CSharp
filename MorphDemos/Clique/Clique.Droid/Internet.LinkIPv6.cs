using System.IO;
using System.Net;
using Morph.Core;

namespace Morph.Internet
{
  public class LinkInternetIPv6 : LinkInternet
  {
    public LinkInternetIPv6(IPEndPoint EndPoint)
      : base(EndPoint)
    {
    }

    static public LinkInternetIPv6 ReadNew(MorphReader Reader, bool HasURI, bool HasPort)
    {
      //  Read host
      string URI;
      if (HasURI)
        //  String
        URI = Reader.ReadString();
      else
      { //  Binary
        //  Unfortunately one can't create an instance of IPAddress using short[8],
        //  so we create a string that IPAddress is able to parse.
        MorphWriter stream = new MorphWriter(new MemoryStream());
        int i = 0;
        do
        {
          short value = (short)Reader.ReadInt16();
          stream.WriteInt16(value);
          if (i == 8)
            break;
          stream.WriteString(":");
        } while (true);
        URI = stream.ToString();
      }
      //  Parse the address
      IPAddress address;
      try
      {
        address = IPAddress.Parse(URI);
      }
      catch
      {
        throw new EMorph("Invalid IPv6 Address");
      }
      //  Read port
      int port = LinkInternet.MorphPort;
      if (HasPort)
        port = (short)Reader.ReadInt16();
      //  Done
      return new LinkInternetIPv6(new IPEndPoint(address, port));
    }

    #region Link members

    public override int Size()
    {
      int size = 1;
      //  Host
      size += 4 + MorphWriter.SizeOfString(EndPoint.Address.ToString());
      //  Port
      if (EndPoint.Port != LinkInternet.MorphPort)
        size += 2;
      return size;
    }

    public override void Write(MorphWriter Writer)
    {
      bool IsIPv6 = true;
      bool IsString = true;
      bool HasPort = EndPoint.Port != LinkInternet.MorphPort;
      //  Link byte
      Writer.WriteLinkByte(LinkTypeID, IsIPv6, IsString, HasPort);
      //  Host
      Writer.WriteString(EndPoint.Address.ToString());
      // Port
      if (HasPort)
        Writer.WriteInt16(EndPoint.Port);
    }

    #endregion
  }
}