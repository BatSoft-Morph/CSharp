using System;
using System.Collections;
using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Sequencing
{
  public class SequenceSender : IDisposable
  {
    internal SequenceSender(int senderID, bool isLossless)
    {
      this.SenderID = senderID;
      this.IsLossless = isLossless;
      lock (SequenceSenders.s_all)
        SequenceSenders.s_all.Add(senderID, this);
    }

    #region IDisposable Members

    public void Dispose()
    {
      lock (SequenceSenders.s_all)
        SequenceSenders.s_all.Remove(SenderID);
    }

    #endregion

    public int SequenceID { get; set; } = 0;

    public int SenderID { get; private set; }

    private int _index = 0;
    private Hashtable _notAcked = null;

    public bool IsLossless
    {
      get => _notAcked != null;
      set
      {
        if (IsLossless != value)
          if (value)
            _notAcked = new Hashtable();
          else
            throw new EMorphUsage("Losslessness cannot be turned off.  Instead, replace current sequence with a lossy sequence.");
      }
    }

    public bool IsStopped
    {
      get => SenderID == 0;
    }

    public void AddNextLink(bool isLast, LinkMessage message)
    {
      lock (this)
      {
        //  Already stopped
        if (SenderID == 0)
          throw new EMorphUsage("Cannot use a sequence that has been stopped.");
        //  Next
        if (SequenceID != 0)
        {
          ++_index;
          if (_notAcked != null)
            lock (_notAcked)
              _notAcked[_index] = message;
          message.PathTo.Push(new LinkSequenceIndexSend(SequenceID, _index, isLast));
        }
        //  Last, but never even started
        else if (isLast)
        {
          Expire();
          return;
        }
        //  Start
        if ((SequenceID == 0) || (_index == 1))
          message.PathTo.Push(new LinkSequenceStartSend(SequenceID, SenderID, IsLossless));
      }
    }

    public void Expire()
    {
      lock (this)
      {
        SenderID = 0;
        if ((_notAcked == null) || (_notAcked.Count == 0))
          Dispose();
      }
    }

    public void Halt()
    {
      lock (this)
      {
        SenderID = 0;
        Dispose();
      }
    }

    private void TryEnd()
    {
      lock (this)
      {
        if ((_notAcked != null) && (_notAcked.Count == 0))
          Dispose();
      }
    }

    internal void Ack(int index)
    {
      if (_notAcked != null)
        lock (_notAcked)
          _notAcked.Remove(index);
    }

    internal void Resend(int index)
    {
      if (_notAcked != null)
      {
        LinkMessage Message;
        lock (_notAcked)
          Message = (LinkMessage)_notAcked[index];
        Message.NextLinkAction();
      }
    }
  }

  public static class SequenceSenders
  {
    static internal Hashtable s_all = new Hashtable();
    private static readonly IDSeed s_senderIDSeed = new IDSeed();

    static internal SequenceSender Find(int senderID)
    {
      lock (s_all)
        return (SequenceSender)s_all[senderID];
    }

    static internal SequenceSender New(LinkStack path, bool isLossless)
    {
      lock (s_all)
      {
        int SenderID;
        do
        {
          SenderID = s_senderIDSeed.Generate();
        } while (s_all.Contains(SenderID));
        return new SequenceSender(SenderID, isLossless);
      }
    }
  }
}