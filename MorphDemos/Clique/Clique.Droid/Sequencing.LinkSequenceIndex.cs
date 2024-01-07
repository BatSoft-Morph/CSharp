using Morph.Base;
using Morph.Core;

namespace Morph.Sequencing
{
  public class LinkSequenceIndexSend : LinkSequence
  {
    public LinkSequenceIndexSend(int SequenceID, int Index, bool IsLast)
      : base()
    {
      _SequenceID = SequenceID;
      _Index = Index;
      _IsLast = IsLast;
    }

    private int _SequenceID;
    public int SequenceID
    {
      get { return _SequenceID; }
    }

    private int _Index;
    public int Index
    {
      get { return _Index; }
    }

    private bool _IsLast;
    public bool IsLast
    {
      get { return _IsLast; }
    }

    public override object FindLinkObject()
    {
      return SequenceReceivers.Find(_SequenceID);
    }

    #region Link

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, false, false, _IsLast);
      Writer.WriteInt32(_SequenceID);
      Writer.WriteInt32(_Index);
    }

    public override void Action(LinkMessage Message)
    {
      if (Message.PathTo.Peek() == this)
        Message.PathTo.Pop();
      SequenceReceiver Sequence = SequenceReceivers.Find(_SequenceID);
      if (Sequence == null)
        throw new EMorph("SequenceID " + _SequenceID.ToString() + " not found.");
      if (_IsLast)
        Sequence.Stop(_Index);
      else
        Sequence.Index(_Index, Message);
    }

    #endregion

    public override string ToString()
    {
      string str = "{Sequence";
      str += " SequenceID=" + _SequenceID.ToString();
      str += " Index=" + _Index.ToString();
      if (_IsLast)
        str += " IsLast";
      return str + '}';
    }
  }

  public class LinkSequenceIndexReply : LinkSequence
  {
    public LinkSequenceIndexReply(int SenderID, int Index, bool Resend)
      : base()
    {
      _SenderID = SenderID;
      _Index = Index;
      _Resend = Resend;
    }

    private int _SenderID;
    public int SenderID
    {
      get { return _SenderID; }
    }

    private int _Index;
    public int Index
    {
      get { return _Index; }
    }

    private bool _Resend;
    public bool Resend
    {
      get { return _Resend; }
    }

    public override object FindLinkObject()
    {
      return SequenceSenders.Find(_SenderID);
    }

    #region Link

    public override void Write(MorphWriter Writer)
    {
      Writer.WriteLinkByte(LinkTypeID, false, true, _Resend);
      Writer.WriteInt32(_SenderID);
      Writer.WriteInt32(_Index);
    }

    public override void Action(LinkMessage Message)
    {
      SequenceSender Sender = SequenceSenders.Find(_SenderID);
      if (Sender == null)
        throw new EMorph("Sequence SenderID " + _SenderID.ToString() + " not found.");
      if (_Resend)
        Sender.Resend(_Index);
      else
        Sender.Ack(_Index);
      Message.NextLinkAction();
    }

    #endregion

    public override string ToString()
    {
      string str = "{Sequence";
      str += " SenderID=" + _SenderID.ToString();
      str += " Index=" + _Index.ToString();
      if (_Resend)
        str += " Resend";
      else
        str += " Ack";
      return str + '}';
    }
  }
}