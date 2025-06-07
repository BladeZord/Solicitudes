using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace es_usuario.Controller.type
{
    /// <summary>
    /// Representa un usuario en el sistema.
    /// </summary>
    public class UsuarioType
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Correo del usuario.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Contrasenia { get; set; } = string.Empty;

        /// <summary>
        /// Codigo del rol que cumple en el catalogo.
        /// </summary>
        public int? Rol_Id { get; set; } = 0;

        /// <summary>
        /// Descripcion del rol.
        /// </summary>
        [NotMapped]
        public string? Rol_Descripcion { get; set; } = string.Empty;
    }
} 