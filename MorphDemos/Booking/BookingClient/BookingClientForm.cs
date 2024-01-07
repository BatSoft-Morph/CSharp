using System;
using System.Windows.Forms;
using Morph.Daemon.Client;
using Morph.Endpoint;
using Morph.Params;
using MorphDemoBooking;

namespace MorphDemoBookingClient
{
  public partial class BookingClientForm : Form
  {
    public BookingClientForm()
    {
      InitializeComponent();
      try
      {
        MorphManager.startup(5);
        MorphManager.ReplyTimeout = new TimeSpan(0, 20, 0);
        MorphApartment apartment = new MorphApartmentShared(new InstanceFactories());
        _BookingClient = new BookingDiplomatClientImpl(apartment, this);
      }
      catch
      {
        MessageBox.Show("Morph.Daemon is not accessible.  Ensure that it is running.");
        Close();
      }
    }

    private void ShowException(string Message, Exception x)
    {
      if (Message != null)
        Message = Message + "\u000D\u000A" + x.Message + "\u000D\u000A" + x.StackTrace;
      else
        Message = x.Message + "\u000D\u000A" + x.StackTrace;
      MessageBox.Show(Message, x.GetType().Name);
    }

    private BookingDiplomatServer _BookingServer = null;
    private BookingDiplomatClient _BookingClient = null;

    private void butClose_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void BookingClientForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (butRelease.Enabled)
        butRelease_Click(sender, e);
      if (_BookingServer != null)
        ((BookingDiplomatServerProxy)_BookingServer).ServletProxy.ApartmentProxy.Dispose();
      MorphManager.shutdown();
    }

    private void textClientName_TextChanged(object sender, EventArgs e)
    {
      butRequest.Enabled = (textClientName.Text.Length > 0) && (textObjectName.Text.Length > 0);
    }

    private void textObjectName_TextChanged(object sender, EventArgs e)
    {
      butRequest.Enabled = (textClientName.Text.Length > 0) && (textObjectName.Text.Length > 0);
    }

    private void butRequest_Click(object sender, EventArgs e)
    {
      try
      {
        if (_BookingServer == null)
          try
          {
            MorphApartmentProxy ServerSide = MorphApartmentProxy.ViaLocal(BookingInterface.ServiceName, new TimeSpan(0, 30, 10), new BookingFactory());
            BookingRegistration Registration = new BookingRegistrationProxy(ServerSide.DefaultServlet);
            _BookingServer = Registration.register(textClientName.Text, _BookingClient);
          }
          catch (Exception x)
          {
            ShowException("Ensure that the Booking Server is running:", x);
            return;
          }
        textOwner.Text = _BookingServer.book(textObjectName.Text);
        textClientName.Enabled = false;
        textObjectName.Enabled = false;
        butRequest.Enabled = false;
        butRelease.Enabled = true;
        butNudge.Enabled = true;
      }
      catch (Exception x)
      {
        ShowException(null, x);
      }
    }

    private void butRelease_Click(object sender, EventArgs e)
    {
      try
      {
        textOwner.Text = _BookingServer.unbook(textObjectName.Text);
        textClientName.Enabled = true;
        textObjectName.Enabled = true;
        butRequest.Enabled = true;
        butRelease.Enabled = false;
        butNudge.Enabled = false;
      }
      catch (Exception x)
      {
        ShowException(null, x);
      }
    }

    private void butNudge_Click(object sender, EventArgs e)
    {
      try
      {
        _BookingServer.nudge(textObjectName.Text);
      }
      catch (Exception x)
      {
        ShowException(null, x);
      }
    }

    #region BookingDiplomatClient implementation

    public void newOwner(string objectName, string clientName)
    {
      textOwner.Text = clientName;
    }

    public void nudgedBy(string clientName)
    {
      MessageBox.Show(this, clientName + " wants the object", "Nudging " + textClientName.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
  }
}