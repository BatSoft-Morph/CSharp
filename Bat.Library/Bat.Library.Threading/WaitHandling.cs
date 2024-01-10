using System;
using System.Threading;

namespace Bat.Library.Threading
{
  public class WaitHandling
  {
    //
    // Summary:
    //     Waits for any of the elements in the specified array to receive a signal.
    //
    // Parameters:
    //   waitHandles:
    //     A WaitHandle array containing the objects for which the current instance
    //     will wait.
    //
    // Returns:
    //     The array index of the object that satisfied the wait.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     The waitHandles parameter is null.-or-One or more of the objects in the waitHandles
    //     array is null.
    //
    //   System.NotSupportedException:
    //     The number of objects in waitHandles is greater than the system permits.
    //
    //   System.ApplicationException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 1.0 or 1.1.
    //
    //   System.Threading.AbandonedMutexException:
    //     The wait completed because a thread exited without releasing a mutex. This
    //     exception is not thrown on Windows 98 or Windows Millennium Edition.
    //
    //   System.ArgumentException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 2.0.
    //
    //   System.InvalidOperationException:
    //     The waitHandles array contains a transparent proxy for a System.Threading.WaitHandle
    //     in another application domain.
    public static int WaitAny(WaitHandle[] waitHandles)
    {
      return WaitAny(waitHandles, Timeout.Infinite, true);
    }

    //
    // Summary:
    //     Waits for any of the elements in the specified array to receive a signal,
    //     using a 32-bit signed integer to measure the time interval, and specifying
    //     whether to exit the synchronization domain before the wait.
    //
    // Parameters:
    //   waitHandles:
    //     A WaitHandle array containing the objects for which the current instance
    //     will wait.
    //
    //   millisecondsTimeout:
    //     The number of milliseconds to wait, or System.Threading.Timeout.Infinite
    //     (-1) to wait indefinitely.
    //
    //   exitContext:
    //     true to exit the synchronization domain for the context before the wait (if
    //     in a synchronized context), and reacquire it afterward; otherwise, false.
    //
    // Returns:
    //     The array index of the object that satisfied the wait, or System.Threading.WaitHandle.WaitTimeout
    //     if no object satisfied the wait and a time interval equivalent to millisecondsTimeout
    //     has passed.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     The waitHandles parameter is null.-or-One or more of the objects in the waitHandles
    //     array is null.
    //
    //   System.NotSupportedException:
    //     The number of objects in waitHandles is greater than the system permits.
    //
    //   System.ApplicationException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 1.0 or 1.1.
    //
    //   System.ArgumentOutOfRangeException:
    //     millisecondsTimeout is a negative number other than -1, which represents
    //     an infinite time-out.
    //
    //   System.Threading.AbandonedMutexException:
    //     The wait completed because a thread exited without releasing a mutex. This
    //     exception is not thrown on Windows 98 or Windows Millennium Edition.
    //
    //   System.ArgumentException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 2.0.
    //
    //   System.InvalidOperationException:
    //     The waitHandles array contains a transparent proxy for a System.Threading.WaitHandle
    //     in another application domain.
    public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
    {
      if (waitHandles == null)
        throw new ArgumentNullException();
      for (int i = waitHandles.Length - 1; i >= 0; i--)
        if (waitHandles[i] == null)
          throw new ArgumentNullException();
      if (waitHandles.Length == 0)
        throw new ArgumentException();
      if (waitHandles.Length > WaitTimeout)
        throw new NotSupportedException();
      return (new Waiters(waitHandles)).WaitForFirst(millisecondsTimeout, exitContext);
    }

    //
    // Summary:
    //     Waits for any of the elements in the specified array to receive a signal,
    //     using a System.TimeSpan to measure the time interval and specifying whether
    //     to exit the synchronization domain before the wait.
    //
    // Parameters:
    //   waitHandles:
    //     A WaitHandle array containing the objects for which the current instance
    //     will wait.
    //
    //   timeout:
    //     A System.TimeSpan that represents the number of milliseconds to wait, or
    //     a System.TimeSpan that represents -1 milliseconds to wait indefinitely.
    //
    //   exitContext:
    //     true to exit the synchronization domain for the context before the wait (if
    //     in a synchronized context), and reacquire it afterward; otherwise, false.
    //
    // Returns:
    //     The array index of the object that satisfied the wait, or System.Threading.WaitHandle.WaitTimeout
    //     if no object satisfied the wait and a time interval equivalent to timeout
    //     has passed.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     The waitHandles parameter is null.-or-One or more of the objects in the waitHandles
    //     array is null.
    //
    //   System.NotSupportedException:
    //     The number of objects in waitHandles is greater than the system permits.
    //
    //   System.ApplicationException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 1.0 or 1.1.
    //
    //   System.ArgumentOutOfRangeException:
    //     timeout is a negative number other than -1 milliseconds, which represents
    //     an infinite time-out. -or-timeout is greater than System.Int32.MaxValue.
    //
    //   System.Threading.AbandonedMutexException:
    //     The wait completed because a thread exited without releasing a mutex. This
    //     exception is not thrown on Windows 98 or Windows Millennium Edition.
    //
    //   System.ArgumentException:
    //     waitHandles is an array with no elements, and the .NET Framework version
    //     is 2.0.
    //
    //   System.InvalidOperationException:
    //     The waitHandles array contains a transparent proxy for a System.Threading.WaitHandle
    //     in another application domain.
    public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
    {
      return WaitAny(waitHandles, (int)timeout.TotalMilliseconds, exitContext);
    }

    const int WaitTimeout = 258;

    #region Internal objects

    private class Waiters
    {
      internal Waiters(WaitHandle[] handles)
      {
        _handles = handles;
        _waiters = new Waiter[handles.Length];
        _gate = new ManualResetEvent(false);
      }

      private readonly WaitHandle[] _handles;
      private Waiter[] _waiters;
      private readonly ManualResetEvent _gate;
      private int _index = WaitTimeout;

      public int WaitForFirst(int Timeout, bool exitContext)
      {
        int count = _handles.Length;
        //  Spawn waiting threads
        for (int i = 0; i < count; i++)
          _waiters[i] = new Waiter(this, i);
        //  Wait for one to signal this thread
        _gate.WaitOne(Timeout, exitContext);
        //  Tidy up
        for (int i = 0; i < count; i++)
          _waiters[i].Abort();
        _waiters = null;
        //  Return the lowest index
        return _index;
      }

      internal void SetIndex(int i)
      {
        lock (this)
        {
          if (_index > i)
            _index = i;
        }
      }

      private class Waiter
      {
        internal Waiter(Waiters group, int index)
        {
          _group = group;
          _index = index;
          _done = false;
          _thread = new Thread(new ThreadStart(ThreadedWait));
        }

        private readonly Waiters _group;
        private readonly int _index;
        private readonly Thread _thread;
        private bool _done;

        internal void ThreadedWait()
        {
          //  Wait for the WaitHandle allocatated to this thread
          _group._handles[_index].WaitOne();
          //  No need to spend resources aborting this thread
          _done = true;
          //  Volunteer this index as the result
          _group.SetIndex(_index);
          //  Let the parent thread go
          _group._gate.Set();
        }

        internal void Abort()
        {
          if (!_done) // Avoid many Abort() calls.  If some slip through, then tough.
            _thread.Abort();
        }
      }
    }

    #endregion
  }

#if Smartphone || PocketPC
    public class SynchronizationLockException : Exception
    {
    }
#endif
}