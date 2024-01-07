using System.Configuration.Install;
using System.ServiceProcess;

namespace Bat.Library.Service
{
  /* Instructions:
   * 1.  Declare a subclass that implements the Initialise() method.
   * 2.  Preceed your sublass with:
   *       [RunInstallerAttribute(true)]
   * 3.  Using section:
   *       using Bat.Library.Service;
   *       using System.ComponentModel;
   *       using System.ServiceProcess;
   */
  public abstract class WindowsServiceInstaller : Installer
  {
    private ServiceInstaller _ServiceInstaller;
    private ServiceProcessInstaller _ServiceProcessInstaller;

    public WindowsServiceInstaller()
    {
      ServiceAccount Account;
      ServiceStartMode StartMode;
      string ServiceName, DisplayName, Description;
      Initialise(out Account, out StartMode, out ServiceName, out DisplayName, out Description);

      // Instantiate installers for process and services.
      _ServiceInstaller = new ServiceInstaller();
      _ServiceProcessInstaller = new ServiceProcessInstaller();

      // The services run under the system account.
      _ServiceProcessInstaller.Account = Account;

      // The services are started upon startup.
      _ServiceInstaller.StartType = StartMode;

      // ServiceName must equal those on ServiceBase derived classes.            
      _ServiceInstaller.ServiceName = ServiceName;
      _ServiceInstaller.DisplayName = DisplayName;
      _ServiceInstaller.Description = Description;

      // Add installers to collection. Order is not important.
      Installers.Add(_ServiceInstaller);
      Installers.Add(_ServiceProcessInstaller);
    }

    protected abstract void Initialise(out ServiceAccount Account, out ServiceStartMode StartMode, out string ServiceName, out string DisplayName, out string Description);
  }
}