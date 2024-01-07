using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Morph.Base;
using Morph.Core;
using Morph.Endpoint;
using Morph.Internet;

namespace TestStream
{
  [TestClass]
  public class UnitTest1
  {
    static UnitTest1()
    {
      //  Register link types
      LinkTypes.Register(new LinkTypeEnd());
      LinkTypes.Register(new LinkTypeMessage());
      LinkTypes.Register(new LinkTypeData());
      LinkTypes.Register(new LinkTypeInternet());
      LinkTypes.Register(new LinkTypeService());
      LinkTypes.Register(new LinkTypeServlet());
      LinkTypes.Register(new LinkTypeMember());
    }

    public byte[] MessageToBytes(LinkMessage Message)
    {
      MemoryStream Stream = new MemoryStream();
      MorphWriter Writer = new MorphWriter(Stream);
      Message.Write(Writer);
      return Writer.ToArray();
    }

    public void CompareBytes(byte[] Test, byte[] Facit)
    {
      if (Facit.Length != Test.Length)
        throw new Exception();
      for (int i = 0; i < Facit.Length; i++)
        if (Facit[i] != Test[i])
          throw new Exception();
    }

    public void TestMessage(LinkMessage Message, byte[] Facit)
    {
      CompareBytes(MessageToBytes(Message), Facit);
    }

    [TestMethod]
    public void TestMessageReply()
    {
      //  Path From
      LinkStack PathFrom = new LinkStack();
      PathFrom.Push(new LinkApartment(0x10203040));
      //  Path To
      LinkStack PathTo = new LinkStack();
      PathTo.Push(new LinkApartmentProxy(0x1A2A3A4A));
      //  Message
      LinkMessage Message = new LinkMessage(PathFrom, PathTo, true);
      Message.CallNumber = 1;
      //  Test
      TestMessage(Message, new byte[] { 
        0xF8, //  {Message
        0x00, 0x00, 0x00, 0x01, //  CallNumber=1
        0x00, 0x00, 0x00, 0x05, //  ToPathSize=5
        0x00, 0x00, 0x00, 0x05, //  FromPathSize=5
        0x92, //  {MorphApartment
        0x10, 0x20, 0x30, 0x40, //  ApartmentID=0x10203040
        0xB2, //  {MorphApartmentProxy
        0x1A, 0x2A, 0x3A, 0x4A  //  ApartmentProxyID=0x1A2A3A4A
      });
      //  Test reply
      Message.NextLink();
      LinkMessage Reply = Message.CreateReply();
      Reply.NextLink();
      TestMessage(Reply, new byte[] { 
        0xF8, //  {Message
        0x00, 0x00, 0x00, 0x01, //  CallNumber=1
        0x00, 0x00, 0x00, 0x05, //  ToPathSize=5
        0x00, 0x00, 0x00, 0x05, //  FromPathSize=5
        0xB2, //  {MorphApartmentProxy
        0x1A, 0x2A, 0x3A, 0x4A, //  ApartmentProxyID=0x1A2A3A4A
        0x92, //  {MorphApartment
        0x10, 0x20, 0x30, 0x40  //  ApartmentID=0x10203040
      });
    }

    [TestMethod]
    public void TestMessageStream()
    {
      //  Path From
      LinkStack PathFrom = new LinkStack();
      PathFrom.Push(new LinkApartment(0x10203040));
      //  Path To
      LinkStack PathTo = new LinkStack();
      PathTo.Push(new LinkApartmentProxy(0x1A2A3A4A));
      //  Message
      LinkMessage Message = new LinkMessage(PathFrom, PathTo, true);
      Message.CallNumber = 1;
      //  Write
      byte[] Bytes = MessageToBytes(Message);
      //  Read
      MorphReader Reader = new MorphReaderSized(Bytes);
      Message = (LinkMessage)LinkTypes.ReadLink(Reader);
      //  Test
      TestMessage(Message, new byte[] { 
        0xF8, //  {Message
        0x00, 0x00, 0x00, 0x01, //  CallNumber=1
        0x00, 0x00, 0x00, 0x05, //  ToPathSize=5
        0x00, 0x00, 0x00, 0x05, //  FromPathSize=5
        0x92, //  {MorphApartment
        0x10, 0x20, 0x30, 0x40, //  ApartmentID=0x10203040
        0xB2, //  {MorphApartmentProxy
        0x1A, 0x2A, 0x3A, 0x4A  //  ApartmentProxyID=0x1A2A3A4A
      });
    }
  }
}