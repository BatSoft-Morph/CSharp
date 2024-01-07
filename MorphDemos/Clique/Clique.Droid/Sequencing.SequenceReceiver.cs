using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Morph.Base;
using Morph.Core;
using Morph.Lib;

namespace Morph.Sequencing
{
  public class SequenceReceiver : IDisposable
  {
    internal SequenceReceiver(int SequenceID, bool IsLossless)
    {
      _SequenceID = SequenceID;
      this.IsLossless = IsLossless;
      lock (SequenceReceivers.All)
        SequenceReceivers.All.Add(SequenceID, this);
      Start();
    }

    #region IDisposable Members

    public void Dispose()
    {
      lock (SequenceReceivers.All)
        SequenceReceivers.All.Remove(_SequenceID);
    }

    #endregion

    private int _SequenceID;
    public int SequenceID
    {
      get { return _SequenceID; }
    }

    private int _SenderID = 0;
    public int SenderID
    {
      get { return _SenderID; }
      set { _SenderID = value; }
    }

    private ISequenceImplementation _Impl = null;
    public bool IsLossless
    {
      get { return _Impl is ImplLossless; }
      set
      {
        if (_Impl != null)
          if (IsLossless == value)
            return;
          else if (!value)
            throw new EMorphUsage("Sequence cannot change from lossless to lossy.");
        lock (_Queue)
          if (_CurrentIndex == 0)
            if (value)
              _Impl = new ImplLossless(this);
            else
              _Impl = new ImplLossy(this);
          else
            throw new EMorphUsage("Sequence has already started.");
      }
    }

    private LinkStack _PathToProxy;
    public LinkStack PathToProxy
    {
      get { return _PathToProxy; }
      set { _PathToProxy = value; }
    }

    private TimeSpan _Timeout = SequenceReceivers.DefaultTimeout;
    public TimeSpan Timeout
    {
      get { return _Timeout; }
      set
      {
        if (value != null)
          _Timeout = value;
      }
    }

    internal void Index(int Index, LinkMessage Message)
    {
      _Impl.Add(Index, Message);
    }

    internal void Stop(int Index)
    {
      throw new System.Exception("The method or operation is not implemented.");
    }

    private void SendReply(LinkSequence LinkReply)
    {
      try
      {
        LinkMessage Reply = new LinkMessage(_PathToProxy.Clone(), null, true);
        Reply.PathTo.Append(LinkReply);
        Reply.NextLinkAction();
      }
      catch (Exception x)
      {
        MorphErrors.NotifyAbout(x);
      }
    }

    #region Thread control

    private bool _IsStopped = false;
    private bool _IsStoppedForce;
    private int _CurrentIndex = 0;
    private AutoResetEvent _Gate = new AutoResetEvent(false);
    private Hashtable _Queue = new Hashtable();

    private void Start()
    {
      new Thread(new ThreadStart(ExecutionMethod)).Start();
    }

    public void Stop(bool Force)
    {
      _IsStoppedForce = Force;
      _IsStopped = true;
      lock (_Queue)
        _Gate.Set();
    }

    protected void ExecutionMethod()
    {
      Thread.CurrentThread.Name = "Sequence";
      while (_Impl.ExecutionIteration()) ;
      //  Send termination message
      //  SendReply(new LinkSequence(_CurrentIndex));
    }

    private interface ISequenceImplementation
    {
      bool ExecutionIteration();
      void Add(int NewIndex, LinkMessage Message);
    }

    #region Lossy

    private class ImplLossy : ISequenceImplementation
    {
      internal ImplLossy(SequenceReceiver Owner)
      {
        _Owner = Owner;
      }

      private SequenceReceiver _Owner;

      public bool ExecutionIteration()
      {
        //  Might be time to stop
        if (_Owner._IsStopped)
          lock (_Owner._Queue)
            if (_Owner._IsStoppedForce || (_Owner._Queue.Count == 0))
              return false;
        //  Get next item
        object item = null;
        lock (_Owner._Queue)
          if (_Owner._Queue.Count > 0)
          {
            while (item == null)
              item = _Owner._Queue[++_Owner._CurrentIndex];
            _Owner._Queue.Remove(_Owner._CurrentIndex);
          }
        //  Handle empty queue
        if (item == null)
          _Owner._Gate.WaitOne();
        //  Handle message
        else
          try
          {
            ((LinkMessage)item).NextLinkAction();
          }
          catch (Exception x)
          {
            MorphErrors.NotifyAbout(x);
          }
        return true;
      }

      public void Add(int NewIndex, LinkMessage Message)
      {
        lock (_Owner._Queue)
        {
          if (_Owner._CurrentIndex <= NewIndex)
            _Owner._Queue[NewIndex] = Message;
          //  Make sure the execution thread is awake
          _Owner._Gate.Set();
        }
      }
    }

    #endregion

    #region Lossless

    private class ImplLossless : ISequenceImplementation
    {
      internal ImplLossless(SequenceReceiver Owner)
      {
        _Owner = Owner;
      }

      private SequenceReceiver _Owner;
      private int _FurthestIndex = 1;

      public bool ExecutionIteration()
      {
        //  Might be time to stop
        if (_Owner._IsStopped)
          lock (_Owner._Queue)
            if (_Owner._IsStoppedForce || (_Owner._Queue.Count == 0))
              return false;
        //  Get next item
        object item = null;
        lock (_Owner._Queue)
          if (_Owner._Queue.Count > 0)
            item = _Owner._Queue[_Owner._CurrentIndex];
        //  Handle empty queue
        if (item == null)
        {
          _Owner._Gate.WaitOne();
          return true;
        }
        //  Handle message
        if (item is LinkMessage)
        {
          try
          {
            _Owner._Queue.Remove(_Owner._CurrentIndex);
            _Owner._CurrentIndex++;
            ((LinkMessage)item).NextLinkAction();
          }
          catch (Exception x)
          {
            MorphErrors.NotifyAbout(x);
          }
          return true;
        }
        //  Handle timeout
        if (item is DateTime)
        {
          //  Send Resend requests for those messages that have timed out
          List<int> Resends = new List<int>();
          DateTime EarliestDue = DateTime.MaxValue;
          lock (_Owner._Queue)
            for (int i = _Owner._CurrentIndex; i < _FurthestIndex; i++)
            {
              item = _Owner._Queue[i];
              if (!(item is DateTime))
                break;
              DateTime DueAt = (DateTime)item;
              //  If it was due before now then...
              if (DueAt.CompareTo(DateTime.Now) < 0)
              {
                //  ...update the due time
                DueAt = DueAt.Add(_Owner._Timeout);
                _Owner._Queue[i] = DueAt;
                //  ...make a note to send Resend request
                Resends.Add(i);
              }
              if (DueAt.CompareTo(EarliestDue) < 0)
                EarliestDue = DueAt;
            }
          //  Send Resend requests (outside the lock)
          for (int i = 0; i < Resends.Count; i++)
            _Owner.SendReply(new LinkSequenceIndexReply(_Owner._SenderID, Resends[i], true));
          //  Wait for the earliest timeout to expire
          //  Note: if EarliestDue = MaxValue, then _Gate will also be set, yielding no wait.
          int WaitTime = EarliestDue.Subtract(DateTime.Now).Milliseconds;
          if (WaitTime > 0)
            _Owner._Gate.WaitOne(WaitTime, false);
        }
        return true;
      }

      public void Add(int NewIndex, LinkMessage Message)
      {
        if (_Owner._CurrentIndex == 0)
          _Owner._CurrentIndex = 1;
        lock (_Owner._Queue)
          //  May be a resend for a message that's already been digested
          if (_Owner._CurrentIndex <= NewIndex)
          {
            //  Add the message into the queue. (Replacing an existing message is fine.)
            _Owner._Queue[NewIndex] = Message;
            //  Update the furthest index
            if (_FurthestIndex < NewIndex)
            {
              _FurthestIndex++;
              //  Fill in any Lossy with "expected by" times
              if (_FurthestIndex < NewIndex)
              {
                DateTime ExpectedBy = DateTime.Now.Add(_Owner.Timeout);
                do
                {
                  _Owner._Queue[_FurthestIndex] = ExpectedBy;
                  _FurthestIndex++;
                } while (_FurthestIndex < NewIndex);
              }
            }
            //  Make sure the execution thread is awake
            _Owner._Gate.Set();
          }
        //  Send Ack
        _Owner.SendReply(new LinkSequenceIndexReply(_Owner._SenderID, NewIndex, false));
      }
    }

    #endregion

    #endregion

    public Link StartLink()
    {
      lock (_Queue)
        if (_CurrentIndex == 0)
          return new LinkSequenceStartReply(_SequenceID, _SenderID, IsLossless);
        else
          return null;
    }
  }

  public static class SequenceReceivers
  {
    static internal Hashtable All = new Hashtable();
    static private IDSeed _SequenceIDSeed = new IDSeed();

    static internal SequenceReceiver Find(int SequenceID)
    {
      lock (All)
        return (SequenceReceiver)All[SequenceID];
    }

    static public SequenceReceiver New(bool IsLossless)
    {
      lock (All)
      {
        int SequenceID;
        do
        {
          SequenceID = _SequenceIDSeed.Generate();
        } while (All.Contains(SequenceID));
        return new SequenceReceiver(SequenceID, IsLossless);
      }
    }

    static public TimeSpan DefaultTimeout = new TimeSpan(0, 0, 5);
  }
}