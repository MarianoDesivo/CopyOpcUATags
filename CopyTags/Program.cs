using CopyTags.HostService;
using eFALCOM.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyTags
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger _logger = new LoggerFactory(LogEngines.SmartInspect).GetLogger("CopyTagsService");

            CopyTags.HostService.CopyTagsServiceClass service = new HostService.CopyTagsServiceClass();

            service.Start();
            Console.Read();
        }
    }
}
