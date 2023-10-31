using CopyTags.HOpcClient;
using eFALCOM.Logger;
using System;
using System.Configuration;
using System.Timers;

namespace CopyTags.HostService
{
    public class CopyTagsServiceClass
    {
        private readonly HOpcHandler hOpcHandler = new HOpcHandler();
        private readonly ILogger _logger = new LoggerFactory(LogEngines.SmartInspect).GetLogger("CopyTagsService");
        private Timer timer = null;

        public void Start()
        {
            _logger.LogMessage("Iniciando CopyTagsService");
            hOpcHandler.InitOpcConnections();

            timer = new Timer();
            timer.Elapsed += TimerEventHandler;
            timer.AutoReset = true;
            timer.Interval = double.Parse(ConfigurationManager.AppSettings.Get("ReadWriteInterval"));
            timer.Start();

            //hOpcHandler.CopyTagsAsync();
        }

        public void Stop()
        {
            _logger.LogMessage("CopyTagsService detenido");

            timer.Elapsed -= TimerEventHandler;
            timer.Stop();
            timer.Dispose();
            timer = null;

            hOpcHandler.DisconnectClients();
        }

        private void TimerEventHandler(object sender, ElapsedEventArgs e)
        {
            _logger.LogDebug("Copiando tags...");

            try
            {
                hOpcHandler.CopyTagsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }
}

