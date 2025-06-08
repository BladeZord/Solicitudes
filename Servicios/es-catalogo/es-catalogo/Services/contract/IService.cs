using es_catalogo.Controller.type;

namespace es_catalogo.Services.contract
{
    public interface IService
    {
        /// <summary>
        /// Obtiene todos los registros de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        Task<List<CatalogoType>> ObtenerTodos();

        /// <summary>
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        Task<CatalogoType> ObtenerPorId(int id);

        /// <summary>
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        Task<CatalogoType> Guardar(CatalogoType entityType);

        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        Task<CatalogoType> Actualizar(CatalogoType entityType);

        /// <summary>
        /// Elimina un registro de catálogo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <returns>Retorna mensaje acorde al resultado de la operación.</returns>
        Task<string> Eliminar(int id);

        /// <summary>
        /// Obtiene todos los registros de catálogos por el tipo.
        /// </summary>
        /// <param name="Tipo">Tipo de parametros.</param>

        /// <returns>Lista de catálogos.</returns>
        Task<List<CatalogoType>> ObtenerPorTipo(string Tipo);
    }
}
