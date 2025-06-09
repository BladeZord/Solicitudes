using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace es_usuario.Controller.type
{
    /// <summary>
    /// Representa los datos de autenticación de un usuario.
    /// </summary>
    public class AuthType
    {
        /// <summary>
        /// Correo del usuario.
        /// </summary>
        [Required]
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        public string Contrasenia { get; set; } = string.Empty;
    }

    /// <summary>
    /// Representa la respuesta de autenticación.
    /// </summary>
    public class AuthResponseType
    {
        /// <summary>
        /// Token JWT generado.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Correo del usuario.
        /// </summary>
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Lista de roles del usuario.
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
} 