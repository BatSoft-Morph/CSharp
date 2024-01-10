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

    Link ReadLink(MorphReader reader);
  }

  public interface ILinkTypeAction
  {
    LinkTypeID ID
    { get; }

    void ActionLink(LinkMessage message, Link currentLink);
  }

  public static class LinkTypes
  {
    private static readonly ILinkTypeReader[] _linkTypeReaders = new ILinkTypeReader[16];
    static public ILinkTypeReader ReaderByLinkTypeID(LinkTypeID linkTypeID)
    {
      return _linkTypeReaders[(byte)linkTypeID];
    }

    private static readonly ILinkTypeAction[] _linkTypeActions = new ILinkTypeAction[16];
    static public ILinkTypeAction ActionByLinkTypeID(LinkTypeID LinkTypeID)
    {
      return _linkTypeActions[(int)LinkTypeID];
    }

    #region Registration

    static public void Register(object linkType)
    {
      if (linkType is ILinkTypeReader linkTypeReader) RegisterReader(linkTypeReader);
      if (linkType is ILinkTypeAction linkTypeWriter) RegisterAction(linkTypeWriter);
    }

    static public void RegisterReader(ILinkTypeReader linkTypeReader)
    {
      if (linkTypeReader == null) throw new EMorphUsage("Reader cannot be null");
      byte linkTypeID = (byte)linkTypeReader.ID;
      if (_linkTypeReaders[linkTypeID] != null)
        throw new EMorph("A link reader for " + linkTypeID + " is already registered");
      _linkTypeReaders[linkTypeID] = linkTypeReader;
    }

    static public void RegisterAction(ILinkTypeAction linkTypeAction)
    {
      if (linkTypeAction == null) throw new EMorphUsage("Action cannot be null");
      byte linkTypeID = (byte)linkTypeAction.ID;
      if (_linkTypeActions[linkTypeID] != null)
        throw new EMorph("A link action for " + linkTypeID + " is already registered");
      _linkTypeActions[linkTypeID] = linkTypeAction;
    }

    #endregion

    static public Link ReadLink(MorphReader reader)
    {
      if (!reader.CanRead)
        return null;
      ILinkTypeReader linkTypeReader = _linkTypeReaders[reader.PeekInt8() & 0xF];
      if (linkTypeReader == null)
        throw new EMorph("Link type not available");
      return linkTypeReader.ReadLink(reader);
    }

    static public void ActionCurrentLink(LinkMessage message)
    {
      Link currentLink = message.Current;
      //  Handle abrupt end of message
      if (currentLink == null)
      {
        if (message.ContextIs(typeof(IActionLast)))
          ((IActionLast)message.Context).ActionLast(message);
        return;
      }
      //  Find the appropriate link type
      ILinkTypeAction linkType = ActionByLinkTypeID(currentLink.LinkTypeID);
      if (linkType == null)
        throw new EMorph("Unsupported link type");
      //  Action the link
      try
      {
        linkType.ActionLink(message, currentLink);
      }
      catch (EMorph x)
      {
        //  If there's a return path, then return an error message
        if (message.HasPathFrom)
          message.CreateReply(x).Action();
      }
    }
  }
}