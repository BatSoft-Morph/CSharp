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

    public abstract void Action(LinkMessage Message);
  }

  public interface IActionLinkSequence
  {
    void ActionLinkSequence(LinkMessage Message, LinkSequence LinkSequence);
  }

  public class LinkTypeSequence : ILinkTypeReader, ILinkTypeAction
  {
    public LinkTypeID ID
    {
      get { return LinkTypeID.Sequence; }
    }

    public Link ReadLink(MorphReader Reader)
    {
      bool IsStart, ToSender, z;
      Reader.ReadLinkByte(out IsStart, out ToSender, out z);
      int Value1 = Reader.ReadInt32();
      int Value2 = Reader.ReadInt32();
      if (IsStart)
        if (ToSender)
          return new LinkSequenceStartReply(Value1, Value2, z);
        else
          return new LinkSequenceStartSend(Value1, Value2, z);
      else  //  IsIndex
        if (ToSender)
          return new LinkSequenceIndexReply(Value1, Value2, z);
        else
          return new LinkSequenceIndexSend(Value1, Value2, z);
    }

    public void ActionLink(LinkMessage Message, Link CurrentLink)
    {
      if (!Message.ContextIs(typeof(IActionLinkSequence)))
        throw new EMorph("Unexpected link type");
      ((IActionLinkSequence)Message.Context).ActionLinkSequence(Message, (LinkSequence)CurrentLink);
    }
  }
}