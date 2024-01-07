using System;
using System.Collections.Generic;
using System.Windows.Forms;
#if LOG_MESSAGES
using Bat.Library.Logging;
using Morph;
#endif

namespace MorphDemoBookingClient
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
#if LOG_MESSAGES
      LinkTypes.AppName = "MorphDemoBookingClient";
      Log.Default = new Log("C:\\Temp\\Morph.log");
      Log.Default.Add("Starting");
#endif
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new BookingClientForm());
    }
  }
}