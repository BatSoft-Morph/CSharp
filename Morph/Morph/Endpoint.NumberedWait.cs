/**
 * A thread might want to wait until another thread completes some action.
 * The waiting thread would then call Wait(ID), 
 * and the other thread then calls End(ID) to release the waiting thread from the wait.
 * 
 * The waiting thread may prepare a wait by calling Prepare(ID).  
 * If the waiting thread does not call Wait(ID), then it *must* call Unprepare(ID).
 * 
 * The control thread can control Hold(ID) to stall the waiting thread for as long as necessary.
 * If the control thread calls Hold(ID), then it *must* call End(ID).
 */

using System;
using System.Threading;
using Morph.Lib;

namespace Morph.Endpoint
{
  public class NumberedWaits// : IDisposable
  {
    /*
    ~NumberedWaits()
    {
      Dispose();
    }

    #region IDisposable Members

    public void Dispose()
    {
      foreach (NumberedWait Wait in _Waits)
        End(Wait.ID);
    }

    #endregion
    */

    #region Internals

    private class NumberedWait : RegisterItemID
    {
      internal NumberedWait(int ID)
      {
        _ID = ID;
      }

      #region RegisterItemID Members

      private int _ID;
      public int ID
      {
        get { return _ID; }
      }

      #endregion

      internal AutoResetEvent _Gate = new AutoResetEvent(false);
      internal bool _Hold = false;
      internal bool _Held = false;
    }

    private RegisterItems<NumberedWait> _Waits = new RegisterItems<NumberedWait>();

    private bool Find(int ID, out NumberedWait Wait)
    {
      lock (_Waits)
        Wait = _Waits.Find(ID);
      return Wait != null;
    }

    private NumberedWait Obtain(int ID)
    {
      NumberedWait Wait;
      if (!Find(ID, out Wait))
      { //  Doesn't already exist, so create and register a new wait
        Wait = new NumberedWait(ID);
        lock (_Waits)
          _Waits.Add(Wait);
      }
      return Wait;
    }

    #endregion

    #region For waiting thread

    public void Prepare(int ID)
    {
      Obtain(ID);
    }

    public void Unprepare(int ID)
    {
      lock (_Waits)
        _Waits.Remove(ID);
    }

    public bool Wait(int ID, TimeSpan Timeout)
    {
      //  Obtain the wait
      NumberedWait Wait = Obtain(ID);
      try
      {
        //  Wait
        bool Result = Wait._Gate.WaitOne(Timeout, false);
        //  Might have been requested to hold
        lock (Wait)
          if (Wait._Hold)
            Wait._Gate.WaitOne();
        //  Done
        return Result || Wait._Held;
      }
      finally
      {
        //  Done, so deregister the wait
        Unprepare(ID);
      }
    }

    #endregion

    #region For control thread

    public bool Hold(int ID)
    {
      NumberedWait Wait;
      bool IsFound = Find(ID, out Wait);
      if (IsFound)
        lock (Wait)
        {
          Wait._Hold = true;
          Wait._Held = true;
        }
      return IsFound;
    }

    public void End(int ID)
    {
      NumberedWait Wait;
      if (Find(ID, out Wait))
      {
        Wait._Hold = false; //  Don't hold
        Wait._Gate.Set();   //  Release it
      }
    }

    #endregion
  }
}