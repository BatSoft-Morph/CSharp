using System;
using System.ServiceProcess;
using Bat.Library.Logging;

namespace Morph.Daemon
{
  class Program
  {
    static void Main(string[] args)
    {
      Log.Default.Types.Add(new LogTypeException());
      Log.Default.Add("");
      Log.Default.Add("Starting instance: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
      try
      {
        ServiceBase.Run(new MorphDaemonService());
      }
      finally
      {
        Log.Default.Add("Stopping instance: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
      }
    }
  }
}
