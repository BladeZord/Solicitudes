namespace es_solicitudes.Controller.type
{
    /// <summary>
    /// Representa el historial de auditoría de una solicitud.
    /// Coincide exactamente con el resultado devuelto por el procedimiento almacenado sp_ConsultarHistorialAuditoria.
    /// </summary>
    public class HistorialAuditoriaType
    {
        /// <summary>
        /// Identificador único del registro de auditoría.
        /// </summary>
        public int LogId { get; set; }

        /// <summary>
        /// Fecha y hora en que se registró el cambio.
        /// </summary>
        public DateTime Fecha_registro { get; set; }

        /// <summary>
        /// Acción realizada (ejemplo: 'Creación', 'Modificación', 'Eliminación').
        /// </summary>
        public string Accion { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del estado anterior de la solicitud.
        /// </summary>
        public string EstadoAnterior { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del estado actual de la solicitud.
        /// </summary>
        public string EstadoActual { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del usuario que realizó la acción.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nombre completo del usuario que realizó la acción.
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la solicitud asociada al registro.
        /// </summary>
        public int SolicitudId { get; set; }

        /// <summary>
        /// Código único de la solicitud.
        /// </summary>
        public string CodigoSolicitud { get; set; } = string.Empty;

        /// <summary>
        /// Monto solicitado.
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Plazo en meses solicitado.
        /// </summary>
        public int PlazoMeses { get; set; }

        /// <summary>
        /// Fecha y hora de creación de la solicitud.
        /// </summary>
        public DateTime FechaSolicitud { get; set; }
    }

}
