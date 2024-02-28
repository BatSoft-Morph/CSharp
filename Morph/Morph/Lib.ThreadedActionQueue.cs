using System;
using System.Collections.Generic;
using System.Threading;

namespace Morph.Lib
{
    public interface IAction
    {
        void Execute();
    }

    public class ThreadedActionQueue
    {
        public ThreadedActionQueue()
        {
        }

        public ThreadedActionQueue(int threadCount)
        {
            SetThreadCount(threadCount);
        }

        #region Threading

        //  Used for managing the number of threads
        private int _threadCount = 0;
        private int _threadCountTarget = 0;
        private object _threadCountSemaphore = new Object();

        //  This gate lets the threads rest when there's no work to do
        private readonly AutoResetEvent _gateRest = new AutoResetEvent(true);

        //  This gate may be useful when tidying up
        private readonly ManualResetEvent _gateNoThreads = new ManualResetEvent(true);

        private void CreateThread()
        {
            _gateNoThreads.Reset();
            (new Thread(new ThreadStart(ThreadCode))).Start();
            _threadCount++;
        }

        private void ThreadCode()
        {
            Thread.CurrentThread.Name = "ThreadedActionQueue";
            while (true)
            { //  Wait
                _gateRest.WaitOne();
                //  Is this thread superfluous?
                if (_threadCountTarget < _threadCount)
                    lock (_threadCountSemaphore)
                        if (_threadCountTarget < _threadCount)
                        {
                            _threadCount--;
                            //  Release the next thread 
                            _gateRest.Set();
                            //  Might be idle now
                            if (_threadCount == 0)
                                _gateNoThreads.Set();
                            return;
                        }
                //  Get the next action
                IAction action = null;
                lock (_actions)
                    if (_actions.Count > 0)
                        action = _actions.Dequeue();
                //  If the queue is empty, then make the threads wait
                if (action != null)
                {
                    _gateRest.Set();
                    //  Run the Action
                    try
                    {
                        action.Execute();
                    }
                    catch (Exception x)
                    {
                        HandleException(x);
                    }
                }
            }
        }

        public void SetThreadCount(int threadCount)
        {
            if (threadCount < 0)
                throw new Exception("ThreadCount cannot be less than 0");
            //  Set the target
            _threadCountTarget = threadCount;
            //  Ensure at least one thread exists, to create other threads.
            lock (_threadCountSemaphore)
                while (_threadCount < _threadCountTarget)
                    CreateThread();
            //  Release a thread
            _gateRest.Set();
        }

        public void WaitUntilNoThreads()
        {
            WaitUntilNoThreads(Timeout.Infinite);
        }

        public bool WaitUntilNoThreads(int timeoutMilliseconds)
        {
            SetThreadCount(0);
            return _gateNoThreads.WaitOne(timeoutMilliseconds, false);
        }

        #endregion

        #region Exception handling

        public event ExceptionEventHandler Error;

        private void HandleException(Exception x)
        {
            try
            {
                Error?.Invoke(this, new ExceptionArgs(x));
            }
            catch
            {
                //  Ignore exceptions thrown from an exception handler
            }
        }

        #endregion

        #region Action queue

        private readonly Queue<IAction> _actions = new Queue<IAction>();

        public void Push(IAction Action)
        {
            lock (_actions)
                _actions.Enqueue(Action);
            _gateRest.Set();
        }

        //  Used for getting a rough estimate when examining efficiency
        public int Count
        {
            get
            {
                lock (_actions)
                    return _actions.Count;
            }
        }

        #endregion
    }
}