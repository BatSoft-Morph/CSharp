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
      MonitorObject objectItem = ObtainMonitorObject(obj);
      return objectItem.Enter(null, 0) != null;
    }

    public static void Enter(Object obj)
    {
      MonitorObject objectItem = ObtainMonitorObject(obj);
      objectItem.Enter(objectItem.FindMonitorThread(), Timeout.Infinite);
    }

    public static void Exit(Object obj)
    {
      MonitorObject objectItem = ObtainMonitorObject(obj);
      objectItem.Exit();
      MayDropMonitorObject(objectItem);
    }

    #endregion

    #region Wait

    static public bool Wait(Object obj)
    {
      return Wait(obj, Timeout.Infinite);
    }

    static public bool Wait(Object obj, int millisecondsTimeout)
    {
      MonitorObject objectItem = ObtainMonitorObject(obj);
      MonitorThread threadItem = objectItem.FindMonitorThread();
      objectItem.ValidateIsOwner(threadItem);
      //  Queue the thread
      objectItem.WaitingPush(threadItem);
      //  Release the object (temporarily)
      objectItem.Unlock(threadItem);
      //  Sleep for a while, or until woken
      bool timedOut = threadItem.Sleep(millisecondsTimeout);
      //  Reclaim obj
      objectItem.Lock(threadItem, Timeout.Infinite);
      return timedOut;
    }

    static public bool Wait(Object obj, TimeSpan timeout)
    {
      return Wait(obj, (int)timeout.TotalMilliseconds);
    }

    #endregion

    #region Pulse

    static public void Pulse(Object obj)
    {
      MonitorObject objectItem = ObtainMonitorObject(obj);
      MonitorThread threadItem = objectItem.FindMonitorThread();
      objectItem.ValidateIsOwner(threadItem);
      //  Pop the first thread off the waiting queue
      MonitorThread threadNext = objectItem.WaitingPop();
      threadNext?.Wake();
    }

    static public void PulseAll(Object obj)
    {
      MonitorObject objectItem = ObtainMonitorObject(obj);
      MonitorThread threadItem = objectItem.FindMonitorThread();
      objectItem.ValidateIsOwner(threadItem);
      //  Pulse all currently waiting threads
      lock (objectItem._waiting)
        while (objectItem._waiting.Count > 0)
          objectItem.WaitingPop().Wake();
    }

    #endregion

    #region Internal objects

    private static readonly List<MonitorObject> s_objects = new List<MonitorObject>();

    static private MonitorObject ObtainMonitorObject(Object obj)
    {
      if (obj == null)
        throw new ArgumentNullException();
      lock (s_objects)
      {
        for (int i = s_objects.Count - 1; i >= 0; i--)
          if (s_objects[i]._obj == obj)
            return s_objects[i];
        MonitorObject queue = new MonitorObject(obj);
        s_objects.Add(queue);
        return queue;
      }
    }

    static private void MayDropMonitorObject(MonitorObject objectItem)
    {
      lock (objectItem._threads)
        if (objectItem._threads.Count == 0)
          lock (s_objects)
            s_objects.Remove(objectItem);
    }

    private class MonitorObject
    {
      internal MonitorObject(object Obj)
      {
        _obj = Obj;
      }

      internal object _obj;
      internal MonitorThread _current = null;

      internal AutoResetEvent _gateEntry = new AutoResetEvent(true);

      internal void ValidateIsOwner(MonitorThread threadItem)
      {
        if ((threadItem == null) || (_current != threadItem))
          throw new SynchronizationLockException();
      }

      #region MonitorThread maintenance

      internal List<MonitorThread> _threads = new List<MonitorThread>();

      internal MonitorThread FindMonitorThread()
      {
        Thread thisThread = Thread.CurrentThread;
        lock (_threads)
        {
          foreach (MonitorThread item in _threads)
            if (item._thread == thisThread)
              return item;
          return null;
        }
      }

      internal MonitorThread NewMonitorThread()
      {
        MonitorThread newItem = new MonitorThread(this);
        _threads.Add(newItem);
        return newItem;
      }

      internal void DropMonitorThread(MonitorThread monitorThread)
      {
        lock (_threads)
          _threads.Remove(monitorThread);
      }

      #endregion

      #region Lock/Unlock

      internal MonitorThread Enter(MonitorThread threadItem, int timeout)
      {
        threadItem = Lock(threadItem, timeout);
        //  If we successfully locked, then we're a step deeper into ownership
        if (threadItem != null)
          threadItem._depth++;
        return threadItem;
      }

      internal MonitorThread Lock(MonitorThread threadItem, int timeout)
      {
        //  If this thread already owns the object, then done
        if ((threadItem != null) && (threadItem == _current))
          return threadItem;
        //  Wait for turn to lock object (if Timeout > 0)
        if (_gateEntry.WaitOne(timeout, false))
          try
          {
            //  Wait for Obj to be unlocked and make this 
            //  Monitor class compatible with System.Threading.Monitor
            System.Threading.Monitor.Enter(_obj);
            //  Ensure we have a ThreadItem
            if (threadItem == null)
              threadItem = NewMonitorThread();
            //  Claim ownership
            _current = threadItem;
            return threadItem;
          }
          finally
          {
            _gateEntry.Set();
          }
        //  Failed to get the lock
        return null;
      }

      internal void Unlock(MonitorThread threadItem)
      {
        ValidateIsOwner(threadItem);
        //  Release Obj
        _current = null;
        System.Threading.Monitor.Exit(_obj);
      }

      public void Exit()
      {
        MonitorThread threadItem = FindMonitorThread();
        ValidateIsOwner(threadItem);
        threadItem._depth--;
        if (threadItem._depth == 0)
        {
          Unlock(threadItem);
          DropMonitorThread(threadItem);
        }
      }

      #endregion

      #region Waiting threads

      internal List<MonitorThread> _waiting = new List<MonitorThread>();

      internal void WaitingPush(MonitorThread Item)
      {
        lock (_waiting)
          _waiting.Add(Item);
      }

      internal MonitorThread WaitingPop()
      {
        lock (_waiting)
        {
          if (_waiting.Count == 0)
            return null;
          MonitorThread item = _waiting[0];
          _waiting.RemoveAt(0);
          return item;
        }
      }

      internal void WaitingRemove(MonitorThread item)
      {
        lock (_waiting)
          _waiting.Remove(item);
      }

      #endregion
    }

    private class MonitorThread
    {
      public MonitorThread(MonitorObject owner)
      {
        _owner = owner;
        _thread = Thread.CurrentThread;
      }

      private readonly MonitorObject _owner;
      internal Thread _thread;
      internal int _depth = 0;

      #region Sleeping

      internal ManualResetEvent _alarmClock = null;

      internal bool Sleep(int millisecondsTimeout)
      {
        _alarmClock = new ManualResetEvent(false);
        bool result = _alarmClock.WaitOne(millisecondsTimeout, false);
        _alarmClock = null;
        return result;
      }

      internal void Wake()
      {
        _alarmClock?.Set();
      }

      #endregion
    }

    #endregion
  }
}