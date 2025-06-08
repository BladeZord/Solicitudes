using es_catalogo.Controller.type;
using Microsoft.AspNetCore.Mvc;

namespace es_catalogo.Controller.contract
{
    /// <summary>
    /// Define las operaciones disponibles para el controlador de catálogos.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="EntityType">Entidad con los datos actualizados.</param>
        /// <returns>Resultado de la operación con la entidad actualizada.</returns>
        Task<ActionResult<object>> Actualizar(CatalogoType EntityType);

        /// <summary>
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="EntityType">Entidad con los datos a guardar.</param>
        /// <returns>Resultado de la operación con la entidad guardada.</returns>
        Task<ActionResult<object>> Guardar(CatalogoType EntityType);

        /// <summary>
        /// Elimina un registro de catálogo.
        /// </summary>
        /// <param name="Id">Identificador del registro a eliminar.</param>
        /// <returns>Mensaje con el resultado de la eliminación.</returns>
        Task<ActionResult<object>> Eliminar(int Id);

        /// <summary>
        /// Obtiene el listado completo de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        Task<ActionResult<object>> ObtenerListado();

        /// <summary>
        /// Obtiene todos los registros de catálogos por el tipo.
        /// </summary>
        /// <param name="Tipo">Tipo de parametros.</param>
        /// <returns>Lista de catálogos.</returns>
        Task<ActionResult<object>> ObtenerPorTipo(string Tipo);

        /// <summary>
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="Id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        Task<ActionResult<object>> ObtenerPorId(int Id);
    }
}
