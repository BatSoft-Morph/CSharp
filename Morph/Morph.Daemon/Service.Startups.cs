using System.Collections.Generic;
using System.Net;
using Morph.Base;
using Morph.Internet;
using Morph.Params;

namespace Morph.Daemon
{
  /*
  public class RegisteredStartup : AwareObject<string>
  {
    internal RegisteredStartup(StartupImpl Owner, string ServiceName, string FileName, string Parameters, TimeSpan Timeout)
      : base(Owner, ServiceName)
    {
      _ServiceName = ServiceName;
      _FileName = FileName;
      _Parameters = Parameters;
      _Timeout = Timeout;
    }

    private string _ServiceName;
    public string ServiceName
    {
      get { return _ServiceName; }
    }

    private string _FileName;
    public string FileName
    {
      get { return _FileName; }
    }

    private string _Parameters;
    public string Parameters
    {
      get { return _Parameters; }
    }

    private TimeSpan _Timeout;
    public TimeSpan Timeout
    {
      get { return _Timeout; }
    }

    #region Internal

    private bool _IsRunning = false;
    private List<EventWaitHandle> _Waits = new List<EventWaitHandle>();
    private void ApplicationStart()
    { //  Copied from http://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
      //Create process
      Process pProcess = new Process();
      //path and file name of command to run
      pProcess.StartInfo.FileName = _FileName;
      //parameters to pass to program
      pProcess.StartInfo.Arguments = _Parameters;
      //pProcess.StartInfo.UseShellExecute = true;
      //Set output of program to be written to process output stream
      //pProcess.StartInfo.RedirectStandardOutput = false;
      //Optional
      //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
      //Start the process
      pProcess.Start();
      //Get program output
      //string strOutput = pProcess.StandardOutput.ReadToEnd();
      //Wait for process to finish
      //pProcess.WaitForExit();
    }

    internal void ApplicationStarted(RegisteredService MorphService)
    {
      lock (_Waits)
        for (int i = 0; i < _Waits.Count; i++)
          _Waits[i].Set();
    }

    internal void ApplicationStopped()
    {
      _IsRunning = false;
    }

    #endregion

    //  This method has its design flaws, but it is thread safe enough for non-critical use.
    public RegisteredService Startup()
    {
      bool IsStartupThread = false;
      RegisteredService MorphService = null;
      EventWaitHandle Wait = new AutoResetEvent(false);
      lock (_Waits)
        _Waits.Add(Wait);
      try
      {
        //  Determine if this thread should start the service
        lock (this)
        {
          IsStartupThread = !_IsRunning;
          _IsRunning = true;
        }
        try
        {
          //  Start up
          if (IsStartupThread)
            ApplicationStart();
          //  Wait for it
          if (Wait.WaitOne(Timeout))
            //  MorphService started
            MorphService = ServicesImpl.Find(ServiceName);
          else
            //  Waited for too long
            MorphService = null;
        } //  Tidy up
        finally
        {
          if (IsStartupThread)
            lock (this)
              _IsRunning = MorphService != null;
        }
      }
      finally
      {
        lock (_Waits)
          _Waits.Remove(Wait);
      }
      //  Final result
      if (MorphService == null)
        throw new EMorphDaemon("Failed to obtain service \"" + ServiceName + '\"');
      return MorphService;
    }
  }
  */
  public class StartupImpl : IMorphParameters
  {
    #region Internal

    static internal ServiceCallbacks _ServiceCallbacks = new ServiceCallbacks();

    private void VerifyAccess(LinkMessage Message)
    {
      if (Message is LinkMessageFromIP)
      {
        LinkMessageFromIP MessageIP = (LinkMessageFromIP)Message;
        if (!Connections.IsEndPointOnThisDevice((IPEndPoint)MessageIP.Connection.RemoteEndPoint))
          throw new EMorphDaemon("Unauthorised access");
      }
    }

    #endregion

    public void refresh(LinkMessage Message)
    {
      RegisteredServices.LoadStartups();
    }

    public void add(LinkMessage Message, string serviceName, string fileName, string parameters, int timeout)
    {
      VerifyAccess(Message);
      //  Add startup
      RegisteredServices.ObtainByName(serviceName).Startup = new RegisteredStartup(fileName, parameters, timeout);
      //  Fire event
      StartupImpl._ServiceCallbacks.DoCallbackAdded(serviceName);
    }

    public void remove(LinkMessage Message, string serviceName)
    {
      VerifyAccess(Message);
      //  Remove startup
      RegisteredServices.ObtainByName(serviceName).Startup = null;
      //  Fire event
      StartupImpl._ServiceCallbacks.DoCallbackRemoved(serviceName);
    }

    public DaemonStartup[] listServices(LinkMessage Message)
    {
      //  List services
      RegisteredService[] AllServices = RegisteredServices.ListAll();
      //  Filter in running services
      List<DaemonStartup> result = new List<DaemonStartup>();
      for (int i = 0; i < AllServices.Length; i++)
      {
        RegisteredService Service = AllServices[i];
        lock (Service)
          if (Service.Startup != null)
          {
            DaemonStartup RunningService = new DaemonStartup();
            RunningService.serviceName = Service.Name;
            RunningService.fileName = Service.Startup.FileName;
            RunningService.timeout = (int)Service.Startup.Timeout.TotalSeconds;
            result.Add(RunningService);
          }
      }
      return result.ToArray();
    }

    public void listen(LinkMessage Message, ServiceCallback callback)
    {
      _ServiceCallbacks.Listen(callback);
    }

    public void unlisten(LinkMessage Message, ServiceCallback callback)
    {
      _ServiceCallbacks.Removed(callback);
    }
  }

  public struct DaemonStartup
  {
    public string serviceName;
    public string fileName;
    public int timeout;
  }
}