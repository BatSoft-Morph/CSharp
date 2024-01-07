using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Bat.Library.Service
{
  /* Instructions:
   * 1.  Sublass WindowsService and set the required parameters in the constructor.
   * 2.  Your service is controlled by the protected Do...() methods.
   * 3.  In the Program.Main() method, add the line:
   *       ServiceBase.Run(new MyService());
   * 4.  Also, create an installer by subclassing WindowsServiceInstaller.
   */
  public class WindowsService : ServiceBase
  {
    /*  Ensure the constructor sets the following properties: 
     *    ServiceName
     *    CanHandlePowerEvent
     *    CanHandleSessionChangeEvent
     *    CanPauseAndContinue
     *    CanShutdown
     *    CanStop
     *    AutoLog
     *    IsLogging
     */
    public WindowsService()
      : base()
    {
    }

    #region Internal

    protected override void OnStart(string[] args)
    {
      try
      {
        DoStart(args);
      }
      catch (Exception x)
      {
        WriteToLog(x.Message);
        throw;
      }
    }

    protected override void OnPause()
    {
      try
      {
        DoPause();
      }
      catch (Exception x)
      {
        WriteToLog(x.Message);
      }
    }

    protected override void OnContinue()
    {
      try
      {
        DoContinue();
      }
      catch (Exception x)
      {
        WriteToLog(x.Message);
      }
    }

    protected override void OnStop()
    {
      try
      {
        this.ExitCode = DoStop();
      }
      catch (Exception x)
      {
        this.ExitCode = -1;
        WriteToLog(x.Message);
      }
    }

    protected override void OnShutdown()
    {
      OnStop();
    }

    #endregion
  
    #region Logging

    private bool _IsLogging = true;
    protected bool IsLogging
    {
      get { return _IsLogging; }
      set { _IsLogging = value; }
    }

    protected void WriteToLog(string Message)
    {
      if (IsLogging)
        EventLog.WriteEntry(GetType().Name, Message);
    }

    #endregion

    protected virtual void DoStart(string[] args)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    protected virtual void DoPause()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    protected virtual void DoContinue()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    /* Return 0 when successful.
     */
    protected virtual int DoStop()
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}