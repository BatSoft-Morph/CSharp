using Morph.Base;
using Morph.Core;

namespace Morph.Sequencing
{
    public class LinkSequenceIndexSend : LinkSequence
    {
        public LinkSequenceIndexSend(int sequenceID, int index, bool isLast)
          : base()
        {
            _sequenceID = sequenceID;
            _index = index;
            _isLast = isLast;
        }

        private readonly int _sequenceID;
        public int SequenceID
        {
            get => _sequenceID;
        }

        private readonly int _index;
        public int Index
        {
            get => _index;
        }

        private readonly bool _isLast;
        public bool IsLast
        {
            get => _isLast;
        }

        public override object FindLinkObject()
        {
            return SequenceReceivers.Find(_sequenceID);
        }

        #region Link

        public override void Write(MorphWriter writer)
        {
            writer.WriteLinkByte(LinkTypeID, false, false, _isLast);
            writer.WriteInt32(_sequenceID);
            writer.WriteInt32(_index);
        }

        public override void Action(LinkMessage message)
        {
            if (message.PathTo.Peek() == this)
                message.PathTo.Pop();
            SequenceReceiver Sequence = SequenceReceivers.Find(_sequenceID);
            if (Sequence == null)
                throw new EMorph("SequenceID " + _sequenceID.ToString() + " not found.");
            if (_isLast)
                Sequence.Stop(_index);
            else
                Sequence.Index(_index, message);
        }

        #endregion

        public override string ToString()
        {
            string str = "{Sequence";
            str += " SequenceID=" + _sequenceID.ToString();
            str += " Index=" + _index.ToString();
            if (_isLast)
                str += " IsLast";
            return str + '}';
        }
    }

    public class LinkSequenceIndexReply : LinkSequence
    {
        public LinkSequenceIndexReply(int senderID, int index, bool resend)
          : base()
        {
            _senderID = senderID;
            _index = index;
            _resend = resend;
        }

        private readonly int _senderID;
        public int SenderID
        {
            get => _senderID;
        }

        private readonly int _index;
        public int Index
        {
            get => _index;
        }

        private readonly bool _resend;
        public bool Resend
        {
            get => _resend;
        }

        public override object FindLinkObject()
        {
            return SequenceSenders.Find(_senderID);
        }

        #region Link

        public override void Write(MorphWriter writer)
        {
            writer.WriteLinkByte(LinkTypeID, false, true, _resend);
            writer.WriteInt32(_senderID);
            writer.WriteInt32(_index);
        }

        public override void Action(LinkMessage message)
        {
            SequenceSender Sender = SequenceSenders.Find(_senderID);
            if (Sender == null)
                throw new EMorph("Sequence SenderID " + _senderID.ToString() + " not found.");
            if (_resend)
                Sender.Resend(_index);
            else
                Sender.Ack(_index);
            message.NextLinkAction();
        }

        #endregion

        public override string ToString()
        {
            string str = "{Sequence";
            str += " SenderID=" + _senderID.ToString();
            str += " Index=" + _index.ToString();
            if (_resend)
                str += " Resend";
            else
                str += " Ack";
            return str + '}';
        }
    }
}