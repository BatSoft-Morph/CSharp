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
        private readonly ServiceInstaller _serviceInstaller;
        private readonly ServiceProcessInstaller _serviceProcessInstaller;

        public WindowsServiceInstaller()
        {
            Initialise(out ServiceAccount account, out ServiceStartMode startMode, out string serviceName, out string displayName, out string description);

            // Instantiate installers for process and services.
            _serviceInstaller = new ServiceInstaller();
            _serviceProcessInstaller = new ServiceProcessInstaller();

            // The services run under the system account.
            _serviceProcessInstaller.Account = account;

            // The services are started upon startup.
            _serviceInstaller.StartType = startMode;

            // ServiceName must equal those on ServiceBase derived classes.            
            _serviceInstaller.ServiceName = serviceName;
            _serviceInstaller.DisplayName = displayName;
            _serviceInstaller.Description = description;

            // Add installers to collection. Order is not important.
            Installers.Add(_serviceInstaller);
            Installers.Add(_serviceProcessInstaller);
        }

        protected abstract void Initialise(out ServiceAccount account, out ServiceStartMode startMode, out string serviceName, out string displayName, out string description);
    }
}