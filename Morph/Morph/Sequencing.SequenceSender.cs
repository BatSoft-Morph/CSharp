using System;
using System.Collections;
using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Sequencing
{
  public class SequenceSender : IDisposable
  {
    internal SequenceSender(int SenderID, bool IsLossless)
    {
      _SenderID = SenderID;
      this.IsLossless = IsLossless;
      lock (SequenceSenders.All)
        SequenceSenders.All.Add(SenderID, this);
    }

    #region IDisposable Members

    public void Dispose()
    {
      lock (SequenceSenders.All)
        SequenceSenders.All.Remove(_SenderID);
    }

    #endregion

    private int _SequenceID = 0;
    public int SequenceID
    {
      get { return _SequenceID; }
      set { _SequenceID = value; }
    }

    private int _SenderID;
    public int SenderID
    {
      get { return _SenderID; }
    }

    private int _Index = 0;
    private Hashtable _NotAcked = null;

    public bool IsLossless
    {
      get { return _NotAcked != null; }
      set
      {
        if (IsLossless != value)
          if (value)
            _NotAcked = new Hashtable();
          else
            throw new EMorphUsage("Losslessness cannot be turned off.  Instead, replace current sequence with a lossy sequence.");
      }
    }

    public bool IsStopped
    {
      get { return _SenderID == 0; }
    }

    public void AddNextLink(bool IsLast, LinkMessage Message)
    {
      lock (this)
      {
        //  Already stopped
        if (_SenderID == 0)
          throw new EMorphUsage("Cannot use a sequence that has been stopped.");
        //  Next
        if (_SequenceID != 0)
        {
          ++_Index;
          if (_NotAcked != null)
            lock (_NotAcked)
              _NotAcked[_Index] = Message;
          Message.PathTo.Push(new LinkSequenceIndexSend(_SequenceID, _Index, IsLast));
        }
        //  Last, but never even started
        else if (IsLast)
        {
          Expire();
          return;
        }
        //  Start
        if ((_SequenceID == 0) || (_Index == 1))
          Message.PathTo.Push(new LinkSequenceStartSend(_SequenceID, _SenderID, IsLossless));
      }
    }

    public void Expire()
    {
      lock (this)
      {
        _SenderID = 0;
        if ((_NotAcked == null) || (_NotAcked.Count == 0))
          Dispose();
      }
    }

    public void Halt()
    {
      lock (this)
      {
        _SenderID = 0;
        Dispose();
      }
    }

    private void TryEnd()
    {
      lock (this)
      {
        if ((_NotAcked != null) && (_NotAcked.Count == 0))
          Dispose();
      }
    }

    internal void Ack(int Index)
    {
      if (_NotAcked != null)
        lock (_NotAcked)
          _NotAcked.Remove(Index);
    }

    internal void Resend(int Index)
    {
      if (_NotAcked != null)
      {
        LinkMessage Message;
        lock (_NotAcked)
          Message = (LinkMessage)_NotAcked[Index];
        Message.NextLinkAction();
      }
    }
  }

  public static class SequenceSenders
  {
    static internal Hashtable All = new Hashtable();
    static private IDSeed _SenderIDSeed = new IDSeed();

    static internal SequenceSender Find(int SenderID)
    {
      lock (All)
        return (SequenceSender)All[SenderID];
    }

    static internal SequenceSender New(LinkStack Path, bool IsLossless)
    {
      lock (All)
      {
        int SenderID;
        do
        {
          SenderID = _SenderIDSeed.Generate();
        } while (All.Contains(SenderID));
        return new SequenceSender(SenderID, IsLossless);
      }
    }
  }
}