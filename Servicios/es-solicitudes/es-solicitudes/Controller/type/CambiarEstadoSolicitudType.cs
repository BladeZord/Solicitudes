using System.ComponentModel.DataAnnotations;

namespace es_solicitudes.Controller.type
{
    public class CambiarEstadoSolicitudType
    {
        [Required]
        public int SolicitudId { get; set; }

        [Required]
        public int NuevoEstadoId { get; set; }

        [Required]
        public int UsuarioAccionId { get; set; }
    }
} 