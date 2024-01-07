using Morph.Base;
using Morph.Core;

namespace Morph.Sequencing
{
  public abstract class LinkSequenceStart : LinkSequence
  {
    public LinkSequenceStart(int SequenceID, int SenderID, bool IsLossless)
      : base()
    {
      _SequenceID = SequenceID;
      _SenderID = SenderID;
      _IsLossless = IsLossless;
    }

    protected int _SequenceID;
    public int SequenceID
    {
      get { return _SequenceID; }
    }

    protected int _SenderID;
    public int SenderID
    {
      get { return _SenderID; }
    }

    protected bool _IsLossless;
    public bool IsLossless
    {
      get { return _IsLossless; }
    }

    protected abstract bool ToSender
    {
      get;
    }

    #region Link

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, true, ToSender, _IsLossless);
      Writer.WriteInt32(_SequenceID);
      Writer.WriteInt32(_SenderID);
    }

    #endregion

    public override string ToString()
    {
      string str = "{Sequence";
      str += " SequenceID=" + _SequenceID.ToString();
      str += " SenderID=" + _SenderID.ToString();
      if (_IsLossless)
        str += " Lossless";
      else
        str += " Lossy";
      return str + '}';
    }
  }

  public class LinkSequenceStartSend : LinkSequenceStart
  {
    public LinkSequenceStartSend(int SequenceID, int SenderID, bool IsLossless)
      : base(SequenceID, SenderID, IsLossless)
    {
    }

    protected override bool ToSender
    {
      get { return false; }
    }

    public override object FindLinkObject()
    {
      return SequenceReceivers.Find(_SequenceID);
    }

    public override void Action(LinkMessage Message)
    {
      if (_SenderID == 0)
        throw new EMorph("At sequence, SenderID cannot be 0.");
      SequenceReceiver Sequence;
      if (_SequenceID == 0)
        //  Start new sequence
        Sequence = SequenceReceivers.New(_IsLossless);
      else
      { //  Find existing sequence
        Sequence = SequenceReceivers.Find(_SequenceID);
        if (Sequence == null)
          throw new EMorph("SequenceID " + _SequenceID.ToString() + " does not exist.");
        Sequence.IsLossless = _IsLossless || Sequence.IsLossless;
        if (Message.PathFrom != null)
          Sequence.PathToProxy = Message.PathFrom.Clone();
      }
      //  Set the sender's ID
      Sequence.SenderID = _SenderID;
      //  Done
      Message.NextLinkAction();
    }
  }

  public class LinkSequenceStartReply : LinkSequenceStart
  {
    public LinkSequenceStartReply(int SequenceID, int SenderID, bool IsLossless)
      : base(SequenceID, SenderID, IsLossless)
    {
    }

    protected override bool ToSender
    {
      get { return true; }
    }

    public override object FindLinkObject()
    {
      return SequenceSenders.Find(_SenderID);
    }

    public override void Action(LinkMessage Message)
    {
      if (Message.PathTo.Peek() == this)
        Message.PathTo.Pop();
      if (_SequenceID == 0)
        throw new EMorph("At sequence sender, SequenceID cannot be 0.");
      SequenceSender Sequence;
      if (_SenderID == 0)
      { //  Start new sequence
        Sequence = SequenceSenders.New(Message.PathFrom, _IsLossless);
        _SenderID = Sequence.SenderID;
      }
      else
      { //  Find existing sequence
        Sequence = SequenceSenders.Find(_SenderID);
        if (Sequence == null)
          throw new EMorph("Sequence SenderID " + _SenderID.ToString() + " does not exist.");
        Sequence.IsLossless = _IsLossless || Sequence.IsLossless;
      }
      //  Set the sender's ID
      Sequence.SequenceID = _SequenceID;
    }
  }
}