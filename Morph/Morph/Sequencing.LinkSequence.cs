using Morph.Base;
using Morph.Core;

namespace Morph.Sequencing
{
  public abstract class LinkSequence : Link
  {
    protected LinkSequence()
      : base(LinkTypeID.Sequence)
    {
    }

    public abstract object FindLinkObject();

    #region Link

    public override int Size()
    {
      return 9;
    }

    #endregion

    public abstract void Action(LinkMessage message);
  }

  public interface IActionLinkSequence
  {
    void ActionLinkSequence(LinkMessage message, LinkSequence linkSequence);
  }

  public class LinkTypeSequence : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get => LinkTypeID.Sequence;
    }

    public Link ReadLink(MorphReader reader)
    {
      bool isStart, toSender, z;
      reader.ReadLinkByte(out isStart, out toSender, out z);
      int value1 = reader.ReadInt32();
      int value2 = reader.ReadInt32();
      if (isStart)
        if (toSender)
          return new LinkSequenceStartReply(value1, value2, z);
        else
          return new LinkSequenceStartSend(value1, value2, z);
      else  //  IsIndex
        if (toSender)
          return new LinkSequenceIndexReply(value1, value2, z);
        else
          return new LinkSequenceIndexSend(value1, value2, z);
    }

    public void ActionLink(LinkMessage message, Link currentLink)
    {
      if (!message.ContextIs(typeof(IActionLinkSequence)))
        throw new EMorph("Unexpected link type");
      ((IActionLinkSequence)message.Context).ActionLinkSequence(message, (LinkSequence)currentLink);
    }
  }
}