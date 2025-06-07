using log4net.Config;
using log4net;
using System.Reflection;
using log4net.Repository;

namespace es_usuario.utils
{
    public class LogUtil
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(LogUtil));

        public LogUtil()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            ILoggerRepository logRepository = LogManager.GetRepository(assembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public void LogInfo(string NombreServicio, string Mensaje, string Metodo)
        {
            LogMensaje("INFO", NombreServicio, Mensaje, Metodo);
        }

        public void LogWarning(string NombreServicio, string Mensaje, string Metodo)
        {
            LogMensaje("WARN", NombreServicio, Mensaje, Metodo);
        }

        public void LogError(string NombreServicio, string Mensaje, Exception? ex = null, string Metodo = "")
        {
            string LogMensaje = $"ERROR - Servicio: {NombreServicio} - Operacion:  {Mensaje} - Metodo: {Metodo}";
            if (ex != null)
            {
                _log.Error($"{LogMensaje} - {ex}", ex);
            }
            else
            {
                _log.Error(LogMensaje);
            }
        }

        private void LogMensaje(string NombreServicio, string TipoLog, string Mensaje, string Metodo)
        {
            string logMensaje = $"Servicio: {NombreServicio} Operacion: {Mensaje} - Metodo: {Metodo}";
            switch (TipoLog.ToUpper())
            {
                case "INFO":
                    _log.Info(logMensaje);
                    break;
                case "WARN":
                    _log.Warn(logMensaje);
                    break;
            }
        }
    }
}
