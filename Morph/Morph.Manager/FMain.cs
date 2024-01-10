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
        MorphManager.Services.Listen(new DaemonEvent(this, new DelegateVoid(PopulateServices)));
        MorphManager.Startups.Listen(new DaemonEvent(this, new DelegateVoid(PopulateStartups)));
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
      string serviceName = GetSelectedServiceName(listStartups);
      DaemonStartup[] startups = MorphManager.Startups.ListServices();
      listStartups.Items.Clear();
      if (startups != null)
        for (int i = 0; i < startups.Length; i++)
        {
          DaemonStartup Startup = startups[i];
          ListViewItem item = listStartups.Items.Add(Startup.serviceName);
          item.SubItems.Add(Startup.timeout.ToString());
          item.SubItems.Add(Startup.fileName);
        }
      SetSelectedServiceName(listStartups, serviceName);
    }

    private void butAddStartup_Click(object sender, EventArgs e)
    {
      FStartup startupDialog = new FStartup(true);
      if (DialogResult.OK == startupDialog.ShowDialog(this))
        try
        {
          MorphManager.Startups.Add(startupDialog.ServiceName, startupDialog.FileName, startupDialog.Parameters, startupDialog.Timeout);
        }
        catch (Exception x)
        {
          ShowException(x);
        }
    }

    private void butEditStartup_Click(object sender, EventArgs e)
    {
      ListViewItem item = listStartups.FocusedItem;
      if (item == null)
        return;
      FStartup startupDialog = new FStartup(false);
      startupDialog.ServiceName = item.SubItems[0].Text;
      startupDialog.Timeout = int.Parse(item.SubItems[1].Text);
      startupDialog.FileName = item.SubItems[2].Text;
      if (DialogResult.OK == startupDialog.ShowDialog(this))
        try
        {
          MorphManager.Startups.Remove(startupDialog.ServiceName);
          MorphManager.Startups.Add(startupDialog.ServiceName, startupDialog.FileName, startupDialog.Parameters, startupDialog.Timeout);
        }
        catch (Exception x)
        {
          ShowException(x);
        }
    }

    private void butRemStartup_Click(object sender, EventArgs e)
    {
      string serviceName = listStartups.FocusedItem.Text;
      if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to remove automotic startup of service \"" + serviceName + "\"?", "Removing startup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
        MorphManager.Startups.Remove(serviceName);
    }

    private void listStartups_SelectedIndexChanged(object sender, EventArgs e)
    {
      bool isSelected = listStartups.FocusedItem != null;
      butEditStartup.Enabled = isSelected;
      butRemStartup.Enabled = isSelected;
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
      string serviceName = GetSelectedServiceName(listServices);
      DaemonService[] services = MorphManager.Services.ListServices();
      listServices.Items.Clear();
      if (services != null)
        for (int i = 0; i < services.Length; i++)
        {
          ListViewItem item = listServices.Items.Add(services[i].serviceName);
          item.SubItems.Add(services[i].accessLocal ? "Yes" : "No");
          item.SubItems.Add(services[i].accessRemote ? "Yes" : "No");
        }
      SetSelectedServiceName(listServices, serviceName);
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
      MorphManager.Shutdown();
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
      MorphApartment = _MorphApartment;
    }

    static DaemonEvent()
    {
      _MorphApartment = new MorphApartmentShared(DaemonClient.InstanceFactory);
    }

    private readonly FMain _owner;
    private readonly DelegateVoid _method;
    private static readonly MorphApartment _MorphApartment;

    public override void Added(string serviceName)
    {
      _owner.Invoke(_method);
    }

    public override void Removed(string serviceName)
    {
      _owner.Invoke(_method);
    }
  }
}