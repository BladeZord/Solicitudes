using System.Text.RegularExpressions;

namespace es_usuario.utils
{
    public class SanitizarInput
    {
        /// <summary>
        /// Sanitiza el correo electrónico del usuario
        /// </summary>
        /// <param name="correo">Correo electrónico a sanitizar</param>
        /// <returns>Correo electrónico sanitizado o null si no es válido</returns>
        public static string? SanitizarCorreo(string? correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return null;

            // Eliminar espacios en blanco al inicio y final
            correo = correo.Trim();

            // Validar formato de correo electrónico
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(correo, pattern))
                return null;

            // Convertir a minúsculas para consistencia
            return correo.ToLower();
        }

        /// <summary>
        /// Sanitiza la contraseña del usuario
        /// </summary>
        /// <param name="contrasenia">Contraseña a sanitizar</param>
        /// <returns>Contraseña sanitizada o null si no cumple con los requisitos mínimos</returns>
        public static string? SanitizarContrasenia(string? contrasenia)
        {
            if (string.IsNullOrWhiteSpace(contrasenia))
                return null;

            // Eliminar espacios en blanco al inicio y final
            contrasenia = contrasenia.Trim();

            // Validar longitud mínima
            if (contrasenia.Length < 4)
                return null;

            // Validar caracteres permitidos (letras, números y caracteres especiales básicos)
            string pattern = @"^[a-zA-Z0-9@#$%^&+=!]*$";
            if (!Regex.IsMatch(contrasenia, pattern))
                return null;

            return contrasenia;
        }

        /// <summary>
        /// Sanitiza el nombre del usuario
        /// </summary>
        /// <param name="nombre">Nombre a sanitizar</param>
        /// <returns>Nombre sanitizado o null si no es válido</returns>
        public static string? SanitizarNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null;

            // Eliminar espacios en blanco al inicio y final
            nombre = nombre.Trim();

            // Validar longitud mínima
            if (nombre.Length < 2)
                return null;

            // Validar que solo contenga letras, números y espacios
            string pattern = @"^[a-zA-Z0-9\s]*$";
            if (!Regex.IsMatch(nombre, pattern))
                return null;

            return nombre;
        }

        /// <summary>
        /// Sanitiza los datos de autenticación
        /// </summary>
        /// <param name="correo">Correo electrónico</param>
        /// <param name="contrasenia">Contraseña</param>
        /// <returns>Tupla con los valores sanitizados (correo, contraseña) o (null, null) si alguno no es válido</returns>
        public static (string? correo, string? contrasenia) SanitizarCredenciales(string? correo, string? contrasenia)
        {
            var correoSanitizado = SanitizarCorreo(correo);
            var contraseniaSanitizada = SanitizarContrasenia(contrasenia);

            if (correoSanitizado == null || contraseniaSanitizada == null)
                return (null, null);

            return (correoSanitizado, contraseniaSanitizada);
        }
    }
}
