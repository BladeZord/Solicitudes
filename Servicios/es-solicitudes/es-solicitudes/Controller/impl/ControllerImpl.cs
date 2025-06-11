using es_solicitudes.Constans;
using es_solicitudes.Controller.contract;
using es_solicitudes.Controller.type;
using es_solicitudes.Services.contract;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System;

namespace es_solicitudes.Controller.impl
{
    /// <summary>
    /// Controlador para la gestión de Catálogos
    /// </summary>
    [Route(ApiConstants.Routes.BasePath)]
    [Tags(ApiConstants.Routes.ControllerName)]
    [ApiController]
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
        [ProducesResponseType(typeof(SolicitudType), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Actualizar(SolicitudType EntityType)
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
        [ProducesResponseType(typeof(SolicitudType), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Guardar(SolicitudType EntityType)
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
        /// Cambia el estado de una solicitud y registra la acción en el log de auditoría.
        /// </summary>
        /// <param name="request">Datos para el cambio de estado.</param>
        /// <returns>Mensaje con el resultado de la operación.</returns>
        /// <response code="200">Estado actualizado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Solicitud no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("cambiar-estado")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> CambiarEstado(CambiarEstadoSolicitudType request)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["solicitudId"] = request.SolicitudId,
                ["nuevoEstadoId"] = request.NuevoEstadoId,
                ["usuarioAccionId"] = request.UsuarioAccionId
            });

            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "CambiarEstado",
                    request.SolicitudId
                );

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ApiConstants.ErrorMessages.InvalidInput, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var result = await _service.CambiarEstado(request.SolicitudId, request.NuevoEstadoId, request.UsuarioAccionId);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "CambiarEstado",
                    request.SolicitudId,
                    "Success"
                );

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "CambiarEstado",
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
        [ProducesResponseType(typeof(List<SolicitudType>), StatusCodes.Status200OK)]
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
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="Id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        /// <response code="200">Catálogo encontrado exitosamente</response>
        /// <response code="404">Catálogo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(SolicitudType), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Obtiene las solicitudes por ID de usuario.
        /// </summary>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <returns>Lista de solicitudes del usuario.</returns>
        /// <response code="200">Lista de solicitudes obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(List<SolicitudType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerPorUsuarioId(int usuarioId)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerPorUsuarioId",
                    usuarioId
                );

                var result = await _service.ObtenerPorUsuarioId(usuarioId);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerPorUsuarioId",
                    usuarioId,
                    "Success"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerPorUsuarioId",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Obtiene las solicitudes por ID de estado.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        /// <returns>Lista de solicitudes con el estado especificado.</returns>
        /// <response code="200">Lista de solicitudes obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("estado/{estadoId}")]
        [ProducesResponseType(typeof(List<SolicitudType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerPorEstadoId(int estadoId)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerPorEstadoId",
                    estadoId
                );

                var result = await _service.ObtenerPorEstadoId(estadoId);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerPorEstadoId",
                    estadoId,
                    "Success"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerPorEstadoId",
                    ex.Message
                );
                throw;
            }
        }

        /// <summary>
        /// Obtiene las solicitudes por filtros dinámicos.
        /// </summary>
        /// <param name="filtros">Filtros de búsqueda.</param>
        /// <returns>Lista de solicitudes que cumplen con los filtros.</returns>
        /// <response code="200">Lista de solicitudes obtenida exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("filtros")]
        [ProducesResponseType(typeof(List<SolicitudType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerPorFiltros(FiltrosSolicitudType filtros)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerPorFiltros",
                    filtros
                );

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ApiConstants.ErrorMessages.InvalidInput, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var result = await _service.ObtenerPorFiltros(filtros);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerPorFiltros",
                    result.Count,
                    "Success"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ApiConstants.LogMessages.OperationError,
                    "ObtenerPorFiltros",
                    ex.Message
                );
                throw;
            }
        }
    }
}
