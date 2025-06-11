using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace es_solicitudes.Controller.type
{
    /// <summary>
    /// Representa una solicitud en el sistema.
    /// </summary>
    public class SolicitudType
    {
        /// <summary>
        /// Identificador único de la solicitud.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código único de la solicitud generado por el sistema.
        /// </summary>
        public string? Codigo { get; set; }

        /// <summary>
        /// Monto de la solicitud.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Monto { get; set; }

        /// <summary>
        /// Plazo en meses de la solicitud.
        /// </summary>
        [Required]
        public int PlazoMeses { get; set; }

        /// <summary>
        /// Ingresos mensuales del solicitante.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal IngresosMensual { get; set; }

        /// <summary>
        /// Antigüedad laboral en meses.
        /// </summary>
        [Required]
        public int AntiguedadLaboral { get; set; }

        /// <summary>
        /// Identificador del estado de la solicitud.
        /// </summary>
        [Required]
        public int Estado_Id { get; set; }

        /// <summary>
        /// Descripción del estado (no mapeado a la base de datos).
        /// </summary>
        [NotMapped]
        public string? Estado_Descripcion { get; set; }

        /// <summary>
        /// Fecha de registro de la solicitud.
        /// </summary>
        [Required]
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó la solicitud.
        /// </summary>
        [Required]
        public int Usuario_Id { get; set; }

        /// <summary>
        /// Nombre del usuario (no mapeado a la base de datos).
        /// </summary>
        [NotMapped]
        public string? Nombre_Usuario { get; set; }
    }
} 