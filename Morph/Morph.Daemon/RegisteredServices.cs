using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using Morph.Base;

namespace Morph.Daemon
{
  public abstract class RegisteredRunning
  {
    public RegisteredRunning(RegisteredService RegisteredService)
    {
      this.RegisteredService = RegisteredService;
    }

    public RegisteredService RegisteredService;

    public bool AccessLocal = true;
    public bool AccessRemote = false;

    public abstract void HandleMessage(LinkMessage Message);
  }

  public class RegisteredStartup
  {
    public RegisteredStartup(string FileName, string Parameters, int Timeout)
    {
      this.FileName = FileName;
      this.Parameters = Parameters;
      this.Timeout = new TimeSpan(0, 0, 0, Timeout);
    }

    public string FileName;
    public string Parameters;
    public TimeSpan Timeout;

    #region Internal

    internal ManualResetEvent _StartupGate = new ManualResetEvent(false);

    internal void Run()
    {
      //  Copied from http://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
      //Create process
      Process pProcess = new Process();
      //path and file name of command to run
      pProcess.StartInfo.FileName = FileName;
      //parameters to pass to program
      pProcess.StartInfo.Arguments = Parameters;
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
      //  Wait for start up to complete
      _StartupGate.WaitOne(Timeout);
    }

    #endregion
  }

  public class RegisteredService : IDisposable
  {
    internal RegisteredService(string ServiceName)
    {
      _Name = ServiceName;
    }

    private string _Name;
    public string Name
    {
      get { return _Name; }
    }

    public bool IsRunning
    {
      get { return _Running != null; }
    }

    private RegisteredRunning _Running = null;
    public RegisteredRunning Running
    {
      get
      {
        RegisteredStartup Starter = null;
        //  Either return the service handler, or...
        lock (this)
          if (_Running == null)
            Starter = _Startup;
          else
            return _Running;
        //  ...try to start the service
        if (Starter == null)
          throw new EMorphDaemon("Service " + Name + " is not available.");
        Starter.Run();
        lock (this)
          if (_Running == null)
            throw new EMorphDaemon("Service " + Name + " failed to start.");
          else
            return _Running;
      }

      set
      {
        lock (this)
        {
          //  Prepare for change
          if (value == null)
          { //  - Need to tidy up "dying" service
            if ((_Running != null) && (_Running is IDisposable))
              ((IDisposable)_Running).Dispose();
          }
          else
          { //  - Not allowed to replace an existing service
            if (_Running != null)
              throw new EMorphDaemon("Service " + Name + " is already running.");
          }
          //  Callback: Notify listeners that the service is no longer running
          if (_Running != null)
            ServicesImpl._ServiceCallbacks.DoCallbackRemoved(Name);
          //  Apply the change
          _Running = value;
          if (_Running == null)
          {
            //  Will need to wait for startup to complete
            if (_Startup != null)
              _Startup._StartupGate.Reset();
            //  Tidy up self
            if (_Startup == null)
              RegisteredServices.ReleaseByName(Name);
          }
          else
          {
            //  Release any threads that are waiting for startup
            if (_Startup != null)
              _Startup._StartupGate.Set();
            //  Callback: Notify listeners about the newly running service
            ServicesImpl._ServiceCallbacks.DoCallbackAdded(Name);
          }
        }
      }
    }

    internal RegisteredStartup _Startup = null;
    public RegisteredStartup Startup
    {
      get { return _Startup; }
      set
      {
        lock (this)
        {
          RegistryKey MorphStartups = RegisteredServices.MorphStartupsKey();
          //  Save
          MorphStartups.DeleteValue(_Name, false);
          //  Assign new startup
          _Startup = value;
          if (_Startup != null)
          {
            //  Save
            RegistryKey StartupKey = MorphStartups.CreateSubKey(_Name);
            StartupKey.SetValue("Filename", value.FileName, RegistryValueKind.String);
            StartupKey.SetValue("Parameters", value.Parameters, RegistryValueKind.String);
            StartupKey.SetValue("Timeout", (int)value.Timeout.TotalSeconds, RegistryValueKind.DWord);
            //  Correct the start gate
            if (_Running == null)
              _Startup._StartupGate.Reset();  //  Will need to wait for startup to complete
            else
              _Startup._StartupGate.Set();  //  Release any threads that are waiting for startup
          }
        }
      }
    }

    #region IDisposable

    public void Dispose()
    {
      Running = null;
    }

    #endregion
  }

  static public class RegisteredServices
  {
    #region Internal

    static Hashtable Services = new Hashtable();
    static Hashtable Connections = new Hashtable();

    static RegisteredServices()
    {
      LoadStartups();
    }

    #endregion

    static public RegisteredService FindByName(string ServiceName)
    {
      lock (Services)
        return (RegisteredService)Services[ServiceName];
    }

    static public RegisteredService ObtainByName(string ServiceName)
    {
      lock (Services)
      {
        RegisteredService Service = (RegisteredService)Services[ServiceName];
        if (Service == null)
        {
          Service = new RegisteredService(ServiceName);
          Services.Add(ServiceName, Service);
        }
        return Service;
      }
    }

    static public void ReleaseByName(string ServiceName)
    {
      lock (Services)
        Services.Remove(ServiceName);
    }

    static public RegisteredService[] ListAll()
    {
      DictionaryEntry[] entries;
      lock (Services)
      {
        entries = new DictionaryEntry[Services.Count];
        Services.CopyTo(entries, 0);
      }
      RegisteredService[] result = new RegisteredService[entries.Length];
      for (int i = 0; i < entries.Length; i++)
        result[i] = (RegisteredService)entries[i].Value;
      return result;
    }

    #region Persistance

    static internal RegistryKey MorphStartupsKey()
    {
      RegistryKey Key = Registry.CurrentUser;
      Key = Key.CreateSubKey("Software");
      Key = Key.CreateSubKey("Morph");
      Key = Key.CreateSubKey("Startups");
      return Key;
    }

    static public void LoadStartups()
    {
      RegistryKey Key = MorphStartupsKey();
      foreach (string serviceName in Key.GetSubKeyNames())
      {
        //  Load
        RegistryKey StartupKey = Key.OpenSubKey(serviceName);
        string Filename = (string)StartupKey.GetValue("Filename");
        string Parameters = (string)StartupKey.GetValue("Parameters");
        Int32 Timeout = (Int32)StartupKey.GetValue("Timeout");
        //  Apply
        ObtainByName(serviceName)._Startup = new RegisteredStartup(Filename, Parameters, Timeout);
      }
    }

    #endregion
  }
}