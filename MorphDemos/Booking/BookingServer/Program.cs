using System;
using Morph.Daemon.Client;
using Morph.Sequencing;
using MorphDemoBooking;
#if LOG_MESSAGES
using Bat.Library.Logging;
using Morph;
#endif

namespace MorphDemoBookingServer
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
      LinkTypes.AppName = "MorphDemoBookingServer";
      Log.Default = new Log("C:\\Temp\\Morph.log");
      Log.Default.Add("Starting");
#endif
      MorphManager.startup(5);
      MorphManager.ReplyTimeout = new TimeSpan(0, 20, 0);
      MorphManager.Services.startServiceSessioned(
        BookingInterface.ServiceName,
        true, true,
        new BookingRegistrationApartmentFactory(new BookingRegistrationFactory(), new BookingInstanceFactories(), new TimeSpan(2, 0, 0), SequenceLevel.None)
        );
      /*
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new BookingServerForm());
       */
    }
  }
}