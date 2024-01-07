using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MorphDemoBooking;
using MorphDemoBookingClient;
using Morph.Endpoint;
using System.Net;
using Morph.Core;
using Morph.Base;
using Morph.Internet;

namespace MorphDemoBookingClientAndroid
{
  [Activity(Label = "MorphDemoBookingClientAndroid", MainLauncher = true, Icon = "@drawable/icon")]
  public class ActivityDemoBooking : Activity
  {
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      //  Setup UI
      SetContentView(Resource.Layout.Main);
      //  - Controls
      edIP = FindViewById<EditText>(Resource.Id.edIP);
      edClientName = FindViewById<EditText>(Resource.Id.edMyName);
      edObjectName = FindViewById<EditText>(Resource.Id.edObjectName);
      edCurrentOwner = FindViewById<EditText>(Resource.Id.edCurrentOwner);
      butRequest = FindViewById<Button>(Resource.Id.butRequest);
      butRelease = FindViewById<Button>(Resource.Id.butRelease);
      butNudge = FindViewById<Button>(Resource.Id.butNudge);
      butExit = FindViewById<Button>(Resource.Id.butExit);
      //  Control states
      //edClientName.setIsInEditMode = true;
      //edObjectName.IsInEditMode = true;
      //edCurrentOwner.IsInEditMode = false;
      //  - Events
      butRequest.Click += RequestClick;
      butRelease.Click += ReleaseClick;
      butNudge.Click += NudgeClick;
      butExit.Click += ExitClick;

      //  Register link types        
      LinkTypes.Register(new LinkTypeEnd());
      LinkTypes.Register(new LinkTypeMessage());
      LinkTypes.Register(new LinkTypeData());
      LinkTypes.Register(new LinkTypeInternet());
      LinkTypes.Register(new LinkTypeService());
      LinkTypes.Register(new LinkTypeServlet());
      LinkTypes.Register(new LinkTypeMember());
      //  Setup Morph interface
      ActionHandler.SetThreadCount(2);
      _BookingFactory = new BookingFactory();
      MorphApartment apartment = new MorphApartmentShared(_BookingFactory);
      _LocalDiplomat = new BookingDiplomatClientImpl(apartment, this);
      _ServerDiplomat = null;
    }

    protected override void OnDestroy()
    {
      if (_ServerDiplomat != null)
      {
        //  Be polite and unbook
        _ServerDiplomat.unbook(edObjectName.Text);
        //  Be polite and tell the server to release resources that have been allocated for this client
        ((BookingDiplomatServerProxy)_ServerDiplomat).ServletProxy.ApartmentProxy.Dispose();
      }
      ActionHandler.SetThreadCount(0);
      Connections.CloseAll();
      base.OnDestroy();
    }

    #region UI

    private EditText edIP;
    private EditText edClientName;
    private EditText edObjectName;
    private EditText edCurrentOwner;
    private Button butRequest;
    private Button butRelease;
    private Button butNudge;
    private Button butExit;

    private void ShowMessage(string Message)
    {
      Toast.MakeText(this, Message, ToastLength.Short);
    }

    private void ShowError(Exception x)
    {
      ShowMessage(x.GetType().Name + ": " + x.Message);
    }

    private void RequestClick(object sender, EventArgs e)
    {
      IPAddress address;
      if (_ServerDiplomat != null)
        ShowMessage("Already waiting for object " + edObjectName.Text + ".");
      else if (!IPAddress.TryParse(edIP.Text, out address))
        ShowMessage("Not a valid IP address:/n" + edIP.Text);
      else
        try
        {
          MorphApartmentProxy apartmentProxy = MorphApartmentProxy.ViaAddress("Morph.Demo.Booking", new TimeSpan(0, 10, 10), _BookingFactory, address);
          BookingRegistration _Registration = new BookingRegistrationProxy(apartmentProxy.DefaultServlet);
          _ServerDiplomat = _Registration.register(edClientName.Text, _LocalDiplomat);
          edCurrentOwner.Text = _ServerDiplomat.book(edObjectName.Text);
          //edClientName.setIsInEditMode = false;
          //edObjectName.IsInEditMode = false;
        }
        catch (Exception x)
        {
          ShowError(x);
        }
    }

    private void ReleaseClick(object sender, EventArgs e)
    {
      if (_ServerDiplomat == null)
        ShowMessage("Already waiting for object " + edObjectName.Text + ".");
      else
        try
        {
          edCurrentOwner.Text = _ServerDiplomat.unbook(edObjectName.Text);
          _ServerDiplomat = null;
        }
        catch (Exception x)
        {
          ShowError(x);
        }
    }

    private void NudgeClick(object sender, EventArgs e)
    {
      try
      {
        _ServerDiplomat.nudge(edObjectName.Text);
      }
      catch (Exception x)
      {
        ShowError(x);
      }
    }

    private void ExitClick(object sender, EventArgs e)
    {
      Finish();
    }

    #endregion

    #region Morph interface

    private BookingFactory _BookingFactory;
    private BookingDiplomatClient _LocalDiplomat;
    private BookingDiplomatServer _ServerDiplomat;

    public void NewOwner(string objectName, string clientName)
    {
      edObjectName.Text = objectName;
      edClientName.Text = clientName;
      edObjectName.PostInvalidate();
      edObjectName.PostInvalidate();
    }

    public void NudgedBy(string clientName)
    {
      Toast.MakeText(this, "Client " + clientName + " wants the object.", ToastLength.Short);
    }

    #endregion
  }
}