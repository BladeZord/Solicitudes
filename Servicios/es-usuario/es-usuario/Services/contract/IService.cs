using es_usuario.Controller.type;

namespace es_usuario.Services.contract
{
    public interface IService
    {
        /// <summary>
        /// Consulta un usuario por su correo y contraseña
        /// </summary>
        /// <param name="authType">Datos de autenticación del usuario</param>
        /// <returns>Usuario encontrado o null si no existe</returns>
        Task<UsuarioType> ConsultarPorUsuarioYContrasenia(AuthType authType);

        /// <summary>
        /// Obtiene todos los registros de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        Task<List<UsuarioType>> ObtenerTodos();

        /// <summary>
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        Task<UsuarioType> ObtenerPorId(int id);

        /// <summary>
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        Task<UsuarioType> Guardar(UsuarioType entityType);

        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        Task<UsuarioType> Actualizar(UsuarioType entityType);

        /// <summary>
        /// Elimina un registro de catálogo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <returns>Retorna mensaje acorde al resultado de la operación.</returns>
        Task<string> Eliminar(int id);
    }
}
