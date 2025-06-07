using es_solicitudes.Controller.type;

namespace es_solicitudes.Repository.contract
{
    /// <summary>
    /// Interfaz para operaciones CRUD sobre la entidad SolicitudType.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Obtiene todas las solicitudes.
        /// </summary>
        /// <returns>Lista de solicitudes.</returns>
        Task<List<SolicitudType>> ObtenerTodos();

        /// <summary>
        /// Obtiene una solicitud específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a consultar.</param>
        /// <returns>Solicitud encontrada.</returns>
        Task<SolicitudType> ObtenerPorId(int id);

        /// <summary>
        /// Guarda una nueva solicitud.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        Task<SolicitudType> Guardar(SolicitudType entityType);

        /// <summary>
        /// Actualiza una solicitud existente.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        Task<SolicitudType> Actualizar(SolicitudType entityType);

        /// <summary>
        /// Elimina una solicitud cambiando su estado.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a eliminar.</param>
        /// <returns>True si la solicitud fue eliminada exitosamente, false en caso contrario.</returns>
        Task<bool> Eliminar(int id);
    }
} 