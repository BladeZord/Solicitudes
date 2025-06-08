using System.ComponentModel.DataAnnotations;

namespace es_usuario.Controller.type
{
    public class AuthType
    {
        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasenia { get; set; } = string.Empty;
    }

    public class AuthResponseType
    {
        public string Token { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;  
    }
} 