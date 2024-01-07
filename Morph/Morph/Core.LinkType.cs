using Morph.Base;

namespace Morph.Core
{
  public enum LinkTypeID
  {
    End = 0x0,
    Message = 0x8,
    Data = 0x4,
    Information = 0xC,
    Service = 0x2,
    Servlet = 0xA,
    Member = 0x6,
    _E = 0xE,
    Process = 0x1,
    Internet = 0x9,
    _5 = 0x5,
    _D = 0xD,
    Sequence = 0x3,
    Encoding = 0xB,
    Stream = 0x7,
    _F = 0xF
  };

  public interface ILinkTypeReader
  {
    LinkTypeID ID
    { get; }

    Link ReadLink(MorphReader Reader);
  }

  public interface ILinkTypeAction
  {
    LinkTypeID ID
    { get; }

    void ActionLink(LinkMessage Message, Link CurrentLink);
  }

  public static class LinkTypes
  {
    static private ILinkTypeReader[] _LinkTypeReaders = new ILinkTypeReader[16];
    static public ILinkTypeReader ReaderByLinkTypeID(LinkTypeID LinkTypeID)
    {
      return _LinkTypeReaders[(byte)LinkTypeID];
    }

    static private ILinkTypeAction[] _LinkTypeActions = new ILinkTypeAction[16];
    static public ILinkTypeAction ActionByLinkTypeID(LinkTypeID LinkTypeID)
    {
      return _LinkTypeActions[(int)LinkTypeID];
    }

    #region Registration

    static public void Register(object LinkType)
    {
      if (LinkType is ILinkTypeReader) RegisterReader((ILinkTypeReader)LinkType);
      if (LinkType is ILinkTypeAction) RegisterAction((ILinkTypeAction)LinkType);
    }

    static public void RegisterReader(ILinkTypeReader LinkTypeReader)
    {
      if (LinkTypeReader == null) throw new EMorphUsage("Reader cannot be null");
      byte LinkTypeID = (byte)LinkTypeReader.ID;
      if (_LinkTypeReaders[LinkTypeID] != null)
        throw new EMorph("A link reader for " + LinkTypeID + " is already registered");
      _LinkTypeReaders[LinkTypeID] = LinkTypeReader;
    }

    static public void RegisterAction(ILinkTypeAction LinkTypeAction)
    {
      if (LinkTypeAction == null) throw new EMorphUsage("Action cannot be null");
      byte LinkTypeID = (byte)LinkTypeAction.ID;
      if (_LinkTypeActions[LinkTypeID] != null)
        throw new EMorph("A link action for " + LinkTypeID + " is already registered");
      _LinkTypeActions[LinkTypeID] = LinkTypeAction;
    }

    #endregion

    static public Link ReadLink(MorphReader Reader)
    {
      if (!Reader.CanRead)
        return null;
      ILinkTypeReader LinkTypeReader = _LinkTypeReaders[Reader.PeekInt8() & 0xF];
      if (LinkTypeReader == null)
        throw new EMorph("Link type not available");
      return LinkTypeReader.ReadLink(Reader);
    }

    static public void ActionCurrentLink(LinkMessage Message)
    {
      Link CurrentLink = Message.Current;
      //  Handle abrupt end of message
      if (CurrentLink == null)
      {
        if (Message.ContextIs(typeof(IActionLast)))
          ((IActionLast)Message.Context).ActionLast(Message);
        return;
      }
      //  Find the appropriate link type
      ILinkTypeAction LinkType = ActionByLinkTypeID(CurrentLink.LinkTypeID);
      if (LinkType == null)
        throw new EMorph("Unsupported link type");
      //  Action the link
      try
      {
        LinkType.ActionLink(Message, CurrentLink);
      }
      catch (EMorph x)
      {
        //  If there's a return path, then return an error message
        if (Message.HasPathFrom)
          Message.CreateReply(x).Action();
      }
    }
  }
}