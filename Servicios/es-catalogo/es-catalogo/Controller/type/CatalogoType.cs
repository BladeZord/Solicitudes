using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace es_catalogo.Controller.type
{
    /// <summary>
    /// Representa un catálogo en el sistema.
    /// </summary>
    public class CatalogoType
    {
        /// <summary>
        /// Identificador único del catálogo.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código único del catálogo.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del catálogo.
        /// </summary>
        [Required]
        [StringLength(60)]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del catálogo padre (si existe).
        /// </summary>
        public int? Padre_Id { get; set; }

        /// <summary>
        /// Tipo del catálogo padre (si existe).
        /// </summary>
        public string? Tipo { get; set; }
    }
} 