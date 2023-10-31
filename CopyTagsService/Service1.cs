using CopyTags.HostService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CopyTagsService
{
    partial class Service1 : ServiceBase
    {
        CopyTagsServiceClass service = new CopyTagsServiceClass();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            service.Start();
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            service.Stop();
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
