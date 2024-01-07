using System;
using System.Collections.Generic;
using System.Threading;

namespace Morph.Lib
{
  public interface Action
  {
    void Execute();
  }

  public class ThreadedActionQueue
  {
    public ThreadedActionQueue()
    {
    }

    public ThreadedActionQueue(int ThreadCount)
    {
      SetThreadCount(ThreadCount);
    }

    #region Threading

    //  Used for managing the number of threads
    private int _ThreadCount = 0;
    private int _ThreadCountTarget = 0;
    private object _ThreadCountSemaphore = new Object();

    //  This gate lets the threads rest when there's no work to do
    private AutoResetEvent _GateRest = new AutoResetEvent(true);

    //  This gate may be useful when tidying up
    private ManualResetEvent _GateNoThreads = new ManualResetEvent(true);

    private void CreateThread()
    {
      _GateNoThreads.Reset();
      (new Thread(new ThreadStart(ThreadCode))).Start();
      _ThreadCount++;
    }

    private void ThreadCode()
    {
      Thread.CurrentThread.Name = "ThreadedActionQueue";
      while (true)
      { //  Wait
        _GateRest.WaitOne();
        //  Is this thread superfluous?
        if (_ThreadCountTarget < _ThreadCount)
          lock (_ThreadCountSemaphore)
            if (_ThreadCountTarget < _ThreadCount)
            {
              _ThreadCount--;
              //  Release the next thread 
              _GateRest.Set();
              //  Might be idle now
              if (_ThreadCount == 0)
                _GateNoThreads.Set();
              return;
            }
        //  Get the next action
        Action Action = null;
        lock (_Actions)
          if (_Actions.Count > 0)
            Action = _Actions.Dequeue();
        //  If the queue is empty, then make the threads wait
        if (Action != null)
        {
          _GateRest.Set();
          //  Run the Action
          try
          {
            Action.Execute();
          }
          catch (Exception x)
          {
            HandleException(x);
          }
        }
      }
    }

    public void SetThreadCount(int ThreadCount)
    {
      if (ThreadCount < 0)
        throw new Exception("ThreadCount cannot be less than 0");
      //  Set the target
      _ThreadCountTarget = ThreadCount;
      //  Ensure at least one thread exists, to create other threads.
      lock (_ThreadCountSemaphore)
        while (_ThreadCount < _ThreadCountTarget)
          CreateThread();
      //  Release a thread
      _GateRest.Set();
    }

    public void WaitUntilNoThreads()
    {
      WaitUntilNoThreads(Timeout.Infinite);
    }

    public bool WaitUntilNoThreads(int TimeoutMilliseconds)
    {
      SetThreadCount(0);
      return _GateNoThreads.WaitOne(TimeoutMilliseconds, false);
    }

    #endregion

    #region Exception handling

    public event ExceptionEventHandler Error;

    private void HandleException(Exception x)
    {
      try
      {
        if (Error != null)
          Error(this, new ExceptionArgs(x));
      }
      catch
      {
        //  Ignore exceptions thrown from an exception handler
      }
    }

    #endregion

    #region Action queue

    private Queue<Action> _Actions = new Queue<Action>();

    public void Push(Action Action)
    {
      lock (_Actions)
        _Actions.Enqueue(Action);
      _GateRest.Set();
    }

    //  Used for getting a rough estimate when examining efficiency
    public int Count
    {
      get
      {
        lock (_Actions)
          return _Actions.Count;
      }
    }

    #endregion
  }
}