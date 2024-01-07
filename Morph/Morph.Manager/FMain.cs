using System;
using System.Windows.Forms;
using Morph.Daemon.Client;
using Morph.Endpoint;

namespace Morph.Manager
{
  public partial class FMain : Form
  {
    public FMain()
    {
      InitializeComponent();
      try
      {
        MorphManager.Services.listen(new DaemonEvent(this, new DelegateVoid(PopulateServices)));
        MorphManager.Startups.listen(new DaemonEvent(this, new DelegateVoid(PopulateStartups)));
      }
      catch (Exception x)
      {
        ShowException(x);
      }
    }

    private void FMain_Shown(object sender, EventArgs e)
    {
      PopulateServices();
      PopulateStartups();
    }

    private string GetSelectedServiceName(ListView list)
    {
      ListViewItem item = list.FocusedItem;
      if (item == null)
        return null;
      else
        return item.Text;
    }

    private void SetSelectedServiceName(ListView list, string serviceName)
    {
      if (serviceName == null)
        list.FocusedItem = null;
      else
        list.FocusedItem = list.FindItemWithText(serviceName);
    }

    private void ShowException(Exception x)
    {
      if (x is EMorphInvocation)
        MessageBox.Show(x.Message, ((EMorphInvocation)x).ClassName);
      else if (x.InnerException == null)
        MessageBox.Show(x.Message, x.GetType().Name);
      else
        MessageBox.Show(x.Message + '\n' + x.InnerException.Message, x.GetType().Name);
    }

    #region Startups

    public void PopulateStartups()
    {
      //  Remember selected service
      string ServiceName = GetSelectedServiceName(listStartups);
      DaemonStartup[] startups = MorphManager.Startups.listServices();
      listStartups.Items.Clear();
      if (startups != null)
        for (int i = 0; i < startups.Length; i++)
        {
          DaemonStartup Startup = startups[i];
          ListViewItem item = listStartups.Items.Add(Startup.serviceName);
          item.SubItems.Add(Startup.timeout.ToString());
          item.SubItems.Add(Startup.fileName);
        }
      SetSelectedServiceName(listStartups, ServiceName);
    }

    private void butAddStartup_Click(object sender, EventArgs e)
    {
      FStartup StartupDialog = new FStartup(true);
      if (DialogResult.OK == StartupDialog.ShowDialog(this))
        try
        {
          MorphManager.Startups.add(StartupDialog.ServiceName, StartupDialog.FileName, StartupDialog.Parameters, StartupDialog.Timeout);
        }
        catch (Exception x)
        {
          ShowException(x);
        }
    }

    private void butEditStartup_Click(object sender, EventArgs e)
    {
      ListViewItem Item = listStartups.FocusedItem;
      if (Item == null)
        return;
      FStartup StartupDialog = new FStartup(false);
      StartupDialog.ServiceName = Item.SubItems[0].Text;
      StartupDialog.Timeout = int.Parse(Item.SubItems[1].Text);
      StartupDialog.FileName = Item.SubItems[2].Text;
      if (DialogResult.OK == StartupDialog.ShowDialog(this))
        try
        {
          MorphManager.Startups.remove(StartupDialog.ServiceName);
          MorphManager.Startups.add(StartupDialog.ServiceName, StartupDialog.FileName, StartupDialog.Parameters, StartupDialog.Timeout);
        }
        catch (Exception x)
        {
          ShowException(x);
        }
    }

    private void butRemStartup_Click(object sender, EventArgs e)
    {
      string ServiceName = listStartups.FocusedItem.Text;
      if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to remove automotic startup of service \"" + ServiceName + "\"?", "Removing startup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
        MorphManager.Startups.remove(ServiceName);
    }

    private void listStartups_SelectedIndexChanged(object sender, EventArgs e)
    {
      bool IsSelected = listStartups.FocusedItem != null;
      butEditStartup.Enabled = IsSelected;
      butRemStartup.Enabled = IsSelected;
    }

    private void listStartups_DoubleClick(object sender, EventArgs e)
    {
      butEditStartup_Click(sender, e);
    }

    #endregion

    #region MorphServices

    public void PopulateServices()
    {
      //  Remember selected service
      string ServiceName = GetSelectedServiceName(listServices);
      DaemonService[] services = MorphManager.Services.listServices();
      listServices.Items.Clear();
      if (services != null)
        for (int i = 0; i < services.Length; i++)
        {
          ListViewItem item = listServices.Items.Add(services[i].serviceName);
          item.SubItems.Add(services[i].accessLocal ? "Yes" : "No");
          item.SubItems.Add(services[i].accessRemote ? "Yes" : "No");
        }
      SetSelectedServiceName(listServices, ServiceName);
    }

    #endregion

    private void butRefresh_Click(object sender, EventArgs e)
    {
      try
      {
        if (tabControl1.SelectedTab == tabStartups)
          PopulateStartups();
        else
          PopulateServices();
      }
      catch (Exception x)
      {
        ShowException(x);
      }
    }

    private void butClose_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void FMain_FormClosed(object sender, FormClosedEventArgs e)
    {
      MorphManager.shutdown();
    }
  }

  public delegate void DelegateVoid();

  public class DaemonEvent : DaemonServiceCallback
  {
    public DaemonEvent(FMain Owner, DelegateVoid method)
      : base()
    {
      _owner = Owner;
      _method = method;
      this.MorphApartment = _MorphApartment;
    }

    static DaemonEvent()
    {
      _MorphApartment = new MorphApartmentShared(DaemonClient.InstanceFactory);
    }

    private FMain _owner;
    private DelegateVoid _method;
    static private MorphApartment _MorphApartment;

    public override void added(string serviceName)
    {
      _owner.Invoke(_method);
    }

    public override void removed(string serviceName)
    {
      _owner.Invoke(_method);
    }
  }
}