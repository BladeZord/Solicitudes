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

        /// <summary>
        /// Cambia el estado de una solicitud y registra la acción en el log de auditoría.
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a modificar.</param>
        /// <param name="nuevoEstadoId">ID del nuevo estado.</param>
        /// <param name="usuarioAccionId">ID del usuario que realiza la acción.</param>
        /// <returns>True si el cambio fue exitoso, false en caso contrario.</returns>
        Task<bool> CambiarEstado(int solicitudId, int nuevoEstadoId, int usuarioAccionId);

        /// <summary>
        /// Obtiene solicitudes por ID de usuario.
        /// </summary>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <returns>Lista de solicitudes.</returns>
        Task<List<SolicitudType>> ObtenerPorUsuarioId(int usuarioId);

        /// <summary>
        /// Obtiene solicitudes por ID de estado.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        /// <returns>Lista de solicitudes.</returns>
        Task<List<SolicitudType>> ObtenerPorEstadoId(int estadoId);

        /// <summary>
        /// Obtiene solicitudes por filtros.
        /// </summary>
        /// <param name="filtros">Filtros para la búsqueda.</param>
        /// <returns>Lista de solicitudes.</returns>
        Task<List<SolicitudType>> ObtenerPorFiltros(FiltrosSolicitudType filtros);
    }
} 