using System;
using System.Collections.Generic;
using System.Threading;

namespace Bat.Library.Threading
{
  public class Monitor
  {
    #region Enter/Exit

    public static bool TryEnter(Object obj)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      return ObjectItem.Enter(null, 0) != null;
    }

    public static void Enter(Object obj)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      ObjectItem.Enter(ObjectItem.FindMonitorThread(), Timeout.Infinite);
    }

    public static void Exit(Object obj)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      ObjectItem.Exit();
      MayDropMonitorObject(ObjectItem);
    }

    #endregion

    #region Wait

    static public bool Wait(Object obj)
    {
      return Wait(obj, Timeout.Infinite);
    }

    static public bool Wait(Object obj, int millisecondsTimeout)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      MonitorThread ThreadItem = ObjectItem.FindMonitorThread();
      ObjectItem.ValidateIsOwner(ThreadItem);
      //  Queue the thread
      ObjectItem.WaitingPush(ThreadItem);
      //  Release the object (temporarily)
      ObjectItem.Unlock(ThreadItem);
      //  Sleep for a while, or until woken
      bool TimedOut = ThreadItem.Sleep(millisecondsTimeout);
      //  Reclaim obj
      ObjectItem.Lock(ThreadItem, Timeout.Infinite);
      return TimedOut;
    }

    static public bool Wait(Object obj, TimeSpan timeout)
    {
      return Wait(obj, (int)timeout.TotalMilliseconds);
    }

    #endregion

    #region Pulse

    static public void Pulse(Object obj)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      MonitorThread ThreadItem = ObjectItem.FindMonitorThread();
      ObjectItem.ValidateIsOwner(ThreadItem);
      //  Pop the first thread off the waiting queue
      MonitorThread ThreadNext = ObjectItem.WaitingPop();
      if (ThreadNext != null)
        ThreadNext.Wake();
    }

    static public void PulseAll(Object obj)
    {
      MonitorObject ObjectItem = ObtainMonitorObject(obj);
      MonitorThread ThreadItem = ObjectItem.FindMonitorThread();
      ObjectItem.ValidateIsOwner(ThreadItem);
      //  Pulse all currently waiting threads
      lock (ObjectItem._Waiting)
        while (ObjectItem._Waiting.Count > 0)
          ObjectItem.WaitingPop().Wake();
    }

    #endregion

    #region Internal objects

    static private List<MonitorObject> _Objects = new List<MonitorObject>();

    static private MonitorObject ObtainMonitorObject(Object obj)
    {
      if (obj == null)
        throw new ArgumentNullException();
      lock (_Objects)
      {
        for (int i = _Objects.Count - 1; i >= 0; i--)
          if (_Objects[i].Obj == obj)
            return _Objects[i];
        MonitorObject Queue = new MonitorObject(obj);
        _Objects.Add(Queue);
        return Queue;
      }
    }

    static private void MayDropMonitorObject(MonitorObject ObjectItem)
    {
      lock (ObjectItem._Threads)
        if (ObjectItem._Threads.Count == 0)
          lock (_Objects)
            _Objects.Remove(ObjectItem);
    }

    private class MonitorObject
    {
      internal MonitorObject(object Obj)
      {
        this.Obj = Obj;
      }

      internal object Obj;
      internal MonitorThread Current = null;

      internal AutoResetEvent GateEntry = new AutoResetEvent(true);

      internal void ValidateIsOwner(MonitorThread ThreadItem)
      {
        if ((ThreadItem == null) || (Current != ThreadItem))
          throw new SynchronizationLockException();
      }

      #region MonitorThread maintenance

      internal List<MonitorThread> _Threads = new List<MonitorThread>();

      internal MonitorThread FindMonitorThread()
      {
        Thread ThisThread = Thread.CurrentThread;
        lock (_Threads)
        {
          foreach (MonitorThread Item in _Threads)
            if (Item._Thread == ThisThread)
              return Item;
          return null;
        }
      }

      internal MonitorThread NewMonitorThread()
      {
        MonitorThread NewItem = new MonitorThread(this);
        _Threads.Add(NewItem);
        return NewItem;
      }

      internal void DropMonitorThread(MonitorThread MonThread)
      {
        lock (_Threads)
          _Threads.Remove(MonThread);
      }

      #endregion

      #region Lock/Unlock

      internal MonitorThread Enter(MonitorThread ThreadItem, int Timeout)
      {
        ThreadItem = Lock(ThreadItem, Timeout);
        //  If we successfully locked, then we're a step deeper into ownership
        if (ThreadItem != null)
          ThreadItem._Depth++;
        return ThreadItem;
      }

      internal MonitorThread Lock(MonitorThread ThreadItem, int Timeout)
      {
        //  If this thread already owns the object, then done
        if ((ThreadItem != null) && (ThreadItem == Current))
          return ThreadItem;
        //  Wait for turn to lock object (if Timeout > 0)
        if (GateEntry.WaitOne(Timeout, false))
          try
          {
            //  Wait for Obj to be unlocked and make this 
            //  Monitor class compatible with System.Threading.Monitor
            System.Threading.Monitor.Enter(Obj);
            //  Ensure we have a ThreadItem
            if (ThreadItem == null)
              ThreadItem = NewMonitorThread();
            //  Claim ownership
            Current = ThreadItem;
            return ThreadItem;
          }
          finally
          {
            GateEntry.Set();
          }
        //  Failed to get the lock
        return null;
      }

      internal void Unlock(MonitorThread ThreadItem)
      {
        ValidateIsOwner(ThreadItem);
        //  Release Obj
        Current = null;
        System.Threading.Monitor.Exit(Obj);
      }

      public void Exit()
      {
        MonitorThread ThreadItem = FindMonitorThread();
        ValidateIsOwner(ThreadItem);
        ThreadItem._Depth--;
        if (ThreadItem._Depth == 0)
        {
          Unlock(ThreadItem);
          DropMonitorThread(ThreadItem);
        }
      }

      #endregion

      #region Waiting threads

      internal List<MonitorThread> _Waiting = new List<MonitorThread>();

      internal void WaitingPush(MonitorThread Item)
      {
        lock (_Waiting)
          _Waiting.Add(Item);
      }

      internal MonitorThread WaitingPop()
      {
        lock (_Waiting)
        {
          if (_Waiting.Count == 0)
            return null;
          MonitorThread Item = _Waiting[0];
          _Waiting.RemoveAt(0);
          return Item;
        }
      }

      internal void WaitingRemove(MonitorThread Item)
      {
        lock (_Waiting)
          _Waiting.Remove(Item);
      }

      #endregion
    }

    private class MonitorThread
    {
      public MonitorThread(MonitorObject Owner)
      {
        _Owner = Owner;
        _Thread = Thread.CurrentThread;
      }

      private MonitorObject _Owner;
      internal Thread _Thread;
      internal int _Depth = 0;

      #region Sleeping

      internal ManualResetEvent AlarmClock = null;

      internal bool Sleep(int millisecondsTimeout)
      {
        AlarmClock = new ManualResetEvent(false);
        bool Result = AlarmClock.WaitOne(millisecondsTimeout, false);
        AlarmClock = null;
        return Result;
      }

      internal void Wake()
      {
        ManualResetEvent Alarm = AlarmClock;
        if (Alarm != null)
          Alarm.Set();
      }

      #endregion
    }

    #endregion
  }
}