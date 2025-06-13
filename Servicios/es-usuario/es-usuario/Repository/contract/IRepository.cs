using es_usuario.Controller.type;

namespace es_usuario.Repository.contract
{
    /// <summary>
    /// Interfaz para operaciones CRUD sobre la entidad CatalogoType.
    /// </summary>
    public interface IRepository
    {
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
        /// <returns>True si el registro fue eliminado exitosamente, false en caso contrario.</returns>
        Task<bool> Eliminar(int id);


        /// <summary>
        /// Compara si el usuario existe.
        /// </summary>
        /// <param name="AuthParam">Parametros de consulta.</param>
        /// <returns>Entidad guardada con su usuario y contrañas.</returns>
        Task<UsuarioType> ConsultarPorUsuarioYContrasenia(AuthType AuthParam);

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nuevaContrasenia">Nueva contraseña del usuario.</param>
        /// <returns>True si la contraseña fue actualizada exitosamente, false en caso contrario.</returns>
        Task<bool> ActualizarContrasenia(int id, string nuevaContrasenia);

        // Nuevos métodos para Usuario_Roles
        Task<List<UsuarioRolType>> ObtenerRolesPorUsuario(int usuarioId);
        Task<bool> AsignarRolAUsuario(UsuarioRolType usuarioRol);
        Task<bool> QuitarRolDeUsuario(UsuarioRolType usuarioRol);
    }
} 