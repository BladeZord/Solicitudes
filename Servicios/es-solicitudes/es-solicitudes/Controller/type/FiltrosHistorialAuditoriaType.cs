namespace es_solicitudes.Controller.type
{
    /// <summary>
    /// Representa los filtros para la consulta del historial de auditoría.
    /// </summary>
    public class FiltrosHistorialAuditoriaType
    {
        /// <summary>
        /// ID de la solicitud a filtrar. Si es 0, no se aplica el filtro.
        /// </summary>
        public int SolicitudId { get; set; }

        /// <summary>
        /// ID del usuario a filtrar. Si es 0, no se aplica el filtro.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// ID del estado anterior a filtrar. Si es 0, no se aplica el filtro.
        /// </summary>
        public int EstadoAnteriorId { get; set; }

        /// <summary>
        /// ID del estado actual a filtrar. Si es 0, no se aplica el filtro.
        /// </summary>
        public int EstadoActualId { get; set; }

        /// <summary>
        /// Fecha de inicio para filtrar. Si es null, no se aplica el filtro.
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin para filtrar. Si es null, no se aplica el filtro.
        /// </summary>
        public DateTime? FechaFin { get; set; }
    }
} 