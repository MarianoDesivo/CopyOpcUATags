using Hylasoft.Opc.Ua;
using System;
using eFALCOM.Logger;
using System.Configuration;
using Opc.Ua;
using System.Threading.Tasks;

namespace CopyTags.HOpcClient
{
    public class HOpcHandler
    {
        private readonly ILogger _logger = new LoggerFactory(LogEngines.SmartInspect).GetLogger("UnifiedAutomatioHandler");
        private string origenUrl;
        private string destinoUrl;
        private UaClient clientOrigen;
        private UaClient clientDestino;

        public void CopyTagsAsync()
        {
            var tagsOrigen = ConfigurationManager.AppSettings.Get("TagsOrigen").Split(',');
            var tagsDestino = ConfigurationManager.AppSettings.Get("TagsDestino").Split(',');

            for(int i = 0;i < tagsOrigen.Length;i++)
            {
                try
                {
                    //leer tag origen                    
                    if (clientOrigen.Status != StatusCodes.Good)
                    {
                        _logger.LogMessage("Reconectando Cliente UA a " + origenUrl);
                        clientOrigen.ReConnect();
                    }

                    var res = clientOrigen.Read<object>(tagsOrigen[i]);

                    var origenValue = res.Value;
                    if (origenValue != null)
                    {
                        if (clientDestino.Status != StatusCodes.Good)
                        {
                            _logger.LogMessage("Reconectando Cliente UA a " + origenUrl);
                            clientDestino.ReConnect();
                        }
                        _logger.LogDebug("Leido: " + origenValue.ToString());

                        //escribirlo en Destino
                        _logger.LogDebug("escribiendo");
                        clientOrigen.Write<object>(tagsDestino[i], origenValue);
                        _logger.LogDebug($"escrito {origenValue} en {tagsDestino[i]}");
                    }
                    else _logger.LogDebug("Hubo un error leyendo " + tagsOrigen[i]);
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            }
        }
        public void DisconnectClients()
        {
            clientOrigen.Dispose();
            clientOrigen = null;
            clientDestino.Dispose();
            clientDestino = null;
        }

        public void InitOpcConnections()
        {
            var options = new UaClientOptions
            {
                #region OPTIONS

                ApplicationName = "h-opc-client",
                AutoAcceptUntrustedCertificates = true,
                ConfigSectionName = "h-opc-client",
                DefaultMonitorInterval = 100,
                SessionName = "h-opc-client",
                //SessionTimeout = (uint)row.OpcClientKeepAliveTimeout,
                UseMessageSecurity = false,
                SubscriptionLifetimeCount = 0u,
                SubscriptionKeepAliveCount = 0u,
                MaxSubscriptionCount = 100,
                MaxMessageQueueSize = 10,
                MaxNotificationQueueSize = 100,
                MaxPublishRequestCount = 20
                #endregion
            };

            origenUrl = ConfigurationManager.AppSettings.Get("OrigenOpcServerUrl");
            clientOrigen = new UaClient(new Uri(origenUrl), options);
            destinoUrl = ConfigurationManager.AppSettings.Get("DestinoOpcServerUrl");
            clientDestino = new UaClient(new Uri(destinoUrl), options);

            try
            {
                clientOrigen.Connect();
                _logger.LogDebug("Cliente Origen OPC conectado a " + origenUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError("No se pudo conectar a " + origenUrl);
                _logger.LogException(ex);
                throw new Exception("Deteniendo servicio");
            }
            try
            {
                clientDestino.Connect();
                _logger.LogDebug("Cliente Destino OPC conectado a " + destinoUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError("No se pudo conectar a " + destinoUrl);
                _logger.LogException(ex);
                throw new Exception("Deteniendo servicio");
            }
        }
    }
}