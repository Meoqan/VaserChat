using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using VaserChatServerModule;

namespace VaserChatService
{
    public partial class VaserChatService : ServiceBase
    {
        public VaserChatService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Server.Start();
        }

        protected override void OnStop()
        {
            Server.Stop();
        }
    }
}
