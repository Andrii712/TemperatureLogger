using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TemperatureLogger
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        readonly ServiceInstaller serviceInstaller;
        readonly ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "TemperatureLogger";
            serviceInstaller.DisplayName = "Temperature Logger";

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
