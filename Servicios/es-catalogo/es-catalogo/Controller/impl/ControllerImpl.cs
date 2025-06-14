using es_catalogo.Constans;
using es_catalogo.Controller.contract;
using es_catalogo.Controller.type;
using es_catalogo.Services.contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;

namespace es_catalogo.Controller.impl
{
    /// <summary>
    /// Controlador para la gestión de Catálogos
    /// </summary>
    [Route(ApiConstants.Routes.BasePath)]
    [Tags(ApiConstants.Routes.ControllerName)]
    [ApiController]
    [Authorize(Roles = "ANALISTA")]
    public class ControllerImpl : ControllerBase, IController
    {
        private readonly IService _service;
        private readonly ILogger<ControllerImpl> _logger;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="service">Servicio para la lógica de negocio</param>
        /// <param name="logger">Servicio de logging</param>
        public ControllerImpl(IService service, ILogger<ControllerImpl> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="EntityType">Entidad con los datos actualizados.</param>
        /// <returns>Resultado de la operación con la entidad actualizada.</returns>
        /// <response code="200">Catálogo actualizado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Catálogo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut]
        [ProducesResponseType(typeof(CatalogoType), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Actualizar(CatalogoType EntityType)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["catalogoId"] = EntityType.Id
            });

            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "Actualizar",
                    EntityType.Id
                );

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ApiConstants.ErrorMessages.InvalidInput, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var result = await _service.Actualizar(EntityType);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "Actualizar",
                    EntityType.Id,
                    "Success"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "Actualizar",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="EntityType">Entidad con los datos a guardar.</param>
        /// <returns>Resultado de la operación con la entidad guardada.</returns>
        /// <response code="201">Catálogo guardado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CatalogoType), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Guardar(CatalogoType EntityType)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["catalogoId"] = EntityType.Id
            });

            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "Guardar",
                    EntityType.Id
                );

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ApiConstants.ErrorMessages.InvalidInput, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var result = await _service.Guardar(EntityType);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "Guardar",
                    result.Id,
                    "Success"
                );

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "Guardar",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Elimina un registro de catálogo.
        /// </summary>
        /// <param name="Id">Identificador del registro a eliminar.</param>
        /// <returns>Mensaje con el resultado de la eliminación.</returns>
        /// <response code="200">Catálogo eliminado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Catálogo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{Id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Eliminar(int Id)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["catalogoId"] = Id
            });

            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "Eliminar",
                    Id
                );

                var result = await _service.Eliminar(Id);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "Eliminar",
                    Id,
                    "Success"
                );

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "Eliminar",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Obtiene el listado completo de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        /// <response code="200">Listado de catálogos obtenido exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CatalogoType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerListado()
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerListado"
                );

                var result = await _service.ObtenerTodos();

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerListado",
                    result.Count
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerListado",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los registros de catálogos por el tipo.
        /// </summary>
        /// <param name="Tipo">Tipo de parametros.</param>
        /// <returns>Lista de catálogos.</returns>
        [HttpGet("tipo/")]
        [ProducesResponseType(typeof(List<CatalogoType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerPorTipo([FromQuery] string? Tipo)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerPorTipo"
                );

                var result = await _service.ObtenerPorTipo(Tipo);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerPorTipo",
                    result.Count
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerPorTipo",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="Id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        /// <response code="200">Catálogo encontrado exitosamente</response>
        /// <response code="404">Catálogo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(CatalogoType), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerPorId(int Id)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerPorId",
                    Id
                );

                var result = await _service.ObtenerPorId(Id);

                if (result == null)
                {
                    _logger.LogInformation(
                        ApiConstants.LogMessages.OperationEnd,
                        "ObtenerPorId",
                        Id,
                        "NotFound"
                    );
                    return NotFound(new { Success = false, Message = ApiConstants.ErrorMessages.NotFound });
                }

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerPorId",
                    Id,
                    "Success"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerPorId",
                    ex.Message
                );
                throw;
            }
        }
    }
}
