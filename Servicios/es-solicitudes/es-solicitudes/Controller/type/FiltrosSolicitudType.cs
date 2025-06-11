namespace es_solicitudes.Controller.type
{
    /// <summary>
    /// Representa los filtros para la búsqueda de solicitudes.
    /// </summary>
    public class FiltrosSolicitudType
    {
        /// <summary>
        /// ID del usuario para filtrar.
        /// </summary>
        public int? UsuarioId { get; set; }

        /// <summary>
        /// ID del estado para filtrar.
        /// </summary>
        public int? EstadoId { get; set; }

        /// <summary>
        /// Fecha de inicio del rango para filtrar.
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin del rango para filtrar.
        /// </summary>
        public DateTime? FechaFin { get; set; }
    }
} 