using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventMonitoring.Data;

namespace TemperatureLogger
{
    public partial class Service1 : ServiceBase
    {
        MonitorChange monitor;

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            monitor = new MonitorChange();
            monitor.StartMonitorTableChange();
        }

        protected override void OnStop()
        {
            monitor.StopMonitorTableChange();
        }
    }
}
