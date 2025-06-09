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
        /// Apellidos del usuario.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Apellidos { get; set; } = string.Empty;

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
        /// Domicilio del usuario.
        /// </summary>
        [StringLength(500)]
        public string? Domicilio { get; set; }

        /// <summary>
        /// Teléfono del usuario.
        /// </summary>
        [StringLength(20)]
        public string? Telefono { get; set; }

        /// <summary>
        /// Lista de roles del usuario.
        /// </summary>
        [NotMapped]
        public List<RolType> Roles { get; set; } = new List<RolType>();
    }

    /// <summary>
    /// Representa un rol de usuario.
    /// </summary>
    public class RolType
    {
        /// <summary>
        /// Identificador del rol.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descripción del rol.
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Representa la relación entre usuario y rol.
    /// </summary>
    public class UsuarioRolType
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int Usuario_Id { get; set; }

        /// <summary>
        /// Identificador del rol.
        /// </summary>
        public int Rol_Id { get; set; }
    }
} 