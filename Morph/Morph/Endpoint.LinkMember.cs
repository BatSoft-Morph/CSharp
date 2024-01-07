using System;
using System.Collections.Generic;
using System.Reflection;
using Morph.Base;
using Morph.Core;

namespace Morph.Endpoint
{
  public abstract class LinkMember : Link, IActionLinkData, IActionLast
  {
    public LinkMember()
      : base(LinkTypeID.Member)
    {
    }

    public abstract string Name
    {
      get;
    }

    private LinkStack DevicePathOf(LinkStack Path)
    {
      if (Path == null)
        return null;
      List<Link> Links = Path.ToLinks();
      for (int i = Links.Count - 1; i >= 0; i--)
      {
        Link Link = Links[i];
        if ((Link is LinkApartment) ||
            (Link is LinkApartmentProxy) ||
            (Link is LinkService) ||
            (Link is LinkServlet) ||
            (Link is LinkMember) ||
            (Link is LinkData))
          Links.RemoveAt(i);
      }
      return new LinkStack(Links);
    }

    private LinkStack EndpointPathOf(LinkStack Path)
    {
      if (Path == null)
        return null;
      List<Link> Links = Path.ToLinks();
      for (int i = Links.Count - 1; i >= 0; i--)
      {
        Link Link = Links[i];
        if ((Link is LinkServlet) ||
            (Link is LinkMember) ||
            (Link is LinkData))
          Links.RemoveAt(i);
      }
      return new LinkStack(Links);
    }

    protected internal Servlet _Servlet;

    protected internal abstract LinkData Invoke(LinkMessage Message, LinkStack SenderDevicePath, LinkData DataIn);

    #region IActionLinkData

    public void ActionLinkData(LinkMessage Message, LinkData DataIn)
    {
      //  Obtain apartment
      MorphApartment Apartment = _Servlet.Apartment;
      try
      {
        //  Get a device path
        LinkStack PathToSender = null;
        if (Apartment is MorphApartmentSession)
          PathToSender = ((MorphApartmentSession)Apartment).Path;
        else if (Message.HasPathFrom)
          PathToSender = Message.PathFrom;
        //  Invoke the method
        LinkData DataOut = Invoke(Message, DevicePathOf(PathToSender), DataIn);
        //  Send a reply
        SendReply(Message, Apartment, DataOut, null);
      }
      catch (EMorph x)
      {
        SendReply(Message, Apartment, null, x);
      }
      catch (TargetInvocationException x)
      {
        Exception y = x.InnerException;
        SendReply(Message, Apartment, new LinkData(y), y);
      }
      catch (Exception x)
      {
        SendReply(Message, Apartment, new LinkData(x), x);
      }
    }

    #endregion

    #region IActionLast

    public void ActionLast(LinkMessage Message)
    {
      ActionLinkData(Message, null);
    }

    #endregion

    private void SendReply(LinkMessage Message, MorphApartment Apartment, Link Payload, Exception Error)
    {
      //  Identify cases when we don't reply
      if (!Message.HasCallNumber)
        return; //  CallNumber is required on the calling end to match call and reply
      if (!Message.HasPathFrom && !(Apartment is MorphApartmentSession))
        return; //  Wouldn't know where to reply to        
      //  In this implementation, we only bother replying with a from path if the call had a from path.
      LinkStack PathFrom = null;
      if (Message.HasPathFrom)
        PathFrom = new LinkStack();
      //  Build a destination (return) path
      LinkStack PathTo = null;
      if (Apartment is MorphApartmentSession)
        PathTo = ((MorphApartmentSession)Apartment).GenerateReturnPath();
      else if (Message.HasPathFrom)
        PathTo = EndpointPathOf(Message.PathFrom);
      PathTo.Append(Payload);
      //  Build reply message
      LinkMessage ReplyMessage = new LinkMessage(PathTo, PathFrom, Message.IsForceful);
      if (Message.HasCallNumber)
        ReplyMessage.CallNumber = Message.CallNumber;
      //  Send the reply
      ReplyMessage.NextLinkAction();
    }

    public override bool Equals(object obj)
    {
      return (obj is LinkMember) && (((LinkMember)obj).Name.ToLower().Equals(Name.ToLower()));
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }

  public class LinkTypeMember : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Member; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool IsProperty, IsSet, HasIndex;
      Reader.ReadLinkByte(out IsProperty, out IsSet, out HasIndex);
      string Name = Reader.ReadString();
      if (IsProperty)
        return new LinkProperty(Name, IsSet, HasIndex);
      else
        return new LinkMethod(Name);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      //  Obtain servlet
      Servlet Servlet;
      if (Message.ContextIs(typeof(Servlet)))
        Servlet = (Servlet)Message.Context;
      else if (Message.ContextIs(typeof(MorphApartment)))
        Servlet = ((MorphApartment)Message.Context).DefaultServlet;
      else
        throw new EMorph("Link type not supported by context");
      //  Hold on to the servlet
      ((LinkMember)CurrentLink)._Servlet = Servlet;
      //  Move along
      Message.Context = CurrentLink;
      Message.NextLinkAction();
    }
  }
}