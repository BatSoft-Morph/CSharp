using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Clique.Interface;
using Morph.Base;
using Morph.Core;
using Morph.Endpoint;
using Morph.Internet;

namespace Clique.Droid
{
  [Activity(Label = "Clique.Droid", MainLauncher = true, Icon = "@drawable/icon")]
  public class CliqueActivity : Activity
  {
    private MorphService _Service;

    private EditText edIP;
    private Button butConnect;
    private EditText edText;
    private ListView lstFriends;

    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);
      //  Register link types        
      LinkTypes.Register(new LinkTypeEnd());
      LinkTypes.Register(new LinkTypeMessage());
      LinkTypes.Register(new LinkTypeData());
      LinkTypes.Register(new LinkTypeInternet());
      LinkTypes.Register(new LinkTypeService());
      LinkTypes.Register(new LinkTypeServlet());
      LinkTypes.Register(new LinkTypeMember());
      ActionHandler.SetThreadCount(2);

      //  Create the Morph.Demo.Clique service
      //  - Create default object (ie. Connector)
      CliqueConnectorImpl Connector = new CliqueConnectorDroid(this);
      //  - Create the apartment factory
      MorphApartmentFactory apartmentFactory = new MorphApartmentFactoryShared(Connector, CliqueInterface.Factories);
      //  - Create a diplomat for this device
      CliqueDiplomatImpl Diplomat = new CliqueDiplomatDroid(this);
      Diplomat.MorphApartment = Connector.MorphApartment;
      CliqueObjects.Initialise(Diplomat);

      //  Make the apartment factory visible under the service name "Morph.Demo.Clique" to make it active
      _Service = MorphServices.Register(CliqueInterface.ServiceName, apartmentFactory);

      //  Setup the UI
      SetContentView(Resource.Layout.Main);
      edIP = FindViewById<EditText>(Resource.Id.edIP);
      butConnect = FindViewById<Button>(Resource.Id.butConnect);
      edText = FindViewById<EditText>(Resource.Id.edText);
      lstFriends = FindViewById<ListView>(Resource.Id.lstFriends);
      butConnect.Click += butConnectClick;
      edText.KeyPress += edTextPress;
    }

    protected override void OnDestroy()
    {
      //  Say bye to all friends before...
      CliqueObjects.Finalise();
      //  ...shutting down Morph communication and stopping service Morph.Demo.Clique
      ActionHandler.SetThreadCount(0);
      _Service.Deregister();
      base.OnDestroy();
    }

    private void butConnectClick(object sender, EventArgs e)
    {
      try
      {
        //  Create a representation of remote service
        MorphApartmentProxy ApartmentProxy = MorphApartmentProxy.ViaString(CliqueInterface.ServiceName, new TimeSpan(0, 20, 10), CliqueInterface.Factories, edIP.Text);
        //  Create a representation of remote services default object (ie. the CliqueConnector)
        CliqueConnector RemoteConnector = new CliqueConnectorProxy(ApartmentProxy.DefaultServlet);
        //  Exchange clique diplomats
        CliqueDiplomat friend = RemoteConnector.hello(CliqueObjects.MyDiplomat);
        CliqueObjects.AddFriend(friend);
      }
      catch (Exception x)
      {
        Show(x);
      }
    }

    private void edTextPress(object sender, View.KeyEventArgs e)
    {
      CliqueObjects.ChangeText(edText.Text);
    }

    private void Show(Exception x)
    {
      Toast.MakeText(this, x.Message, ToastLength.Long);
    }

    public void ShowFriends()
    {
      CliqueDiplomat[] friends = CliqueObjects.Friends.ToArray();
      //  Get texts for each friend
      List<string> friendTexts = new List<string>();
      for (int i = 0; i < friends.Length; i++)
        friendTexts.Add(friends[i].text);
      //  Display the texts
      lstFriends.Adapter = new ArrayAdapter<string>(this, Resource.Layout.Friend, friendTexts.ToArray());
    }
  }
}