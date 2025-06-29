﻿using es_solicitudes.Constans;
using es_solicitudes.Controller.type;
using es_solicitudes.exception;
using es_solicitudes.Repository.contract;
using es_solicitudes.Services.contract;

namespace es_solicitudes.Services.impl
{
    /// <summary>
    /// Implementación del servicio para la gestión de solicitudes
    /// </summary>
    public class ServiceImpl : IService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ServiceImpl> _logger;

        /// <summary>
        /// Constructor del servicio
        /// </summary>
        /// <param name="repository">Repositorio para acceso a datos</param>
        /// <param name="logger">Servicio de logging</param>
        public ServiceImpl(IRepository repository, ILogger<ServiceImpl> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las solicitudes.
        /// </summary>
        /// <returns>Lista de solicitudes.</returns>
        public async Task<List<SolicitudType>> ObtenerTodos()
        {
            const string operation = nameof(ObtenerTodos);

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerTodos();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene una solicitud específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a consultar.</param>
        /// <returns>Solicitud encontrada.</returns>
        public async Task<SolicitudType> ObtenerPorId(int id)
        {
            const string operation = nameof(ObtenerPorId);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = id
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { id });

                var resultado = await _repository.ObtenerPorId(id);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { id, Status = "Success" });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Guarda una nueva solicitud.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        public async Task<SolicitudType> Guardar(SolicitudType entityType)
        {
            const string operation = nameof(Guardar);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = entityType.Id
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { entityType.Id });

                var resultado = await _repository.Guardar(entityType);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { resultado.Id, Status = "Success" });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al guardar la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una solicitud existente.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        public async Task<SolicitudType> Actualizar(SolicitudType entityType)
        {
            const string operation = nameof(Actualizar);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = entityType.Id
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { entityType.Id });

                var resultado = await _repository.Actualizar(entityType);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { entityType.Id, Status = "Success" });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al actualizar la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una solicitud cambiando su estado.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a eliminar.</param>
        /// <returns>Retorna mensaje acorde al resultado de la operación.</returns>
        public async Task<string> Eliminar(int id)
        {
            const string operation = nameof(Eliminar);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = id
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { id });

                var resultado = await _repository.Eliminar(id);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { id, Status = "Success" });

                return resultado ? "Solicitud eliminada exitosamente" : "No se pudo eliminar la solicitud";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al eliminar la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Cambia el estado de una solicitud y registra la acción en el log de auditoría.
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a modificar.</param>
        /// <param name="nuevoEstadoId">ID del nuevo estado.</param>
        /// <param name="usuarioAccionId">ID del usuario que realiza la acción.</param>
        /// <returns>Mensaje con el resultado de la operación.</returns>
        public async Task<string> CambiarEstado(int solicitudId, int nuevoEstadoId, int usuarioAccionId)
        {
            const string operation = nameof(CambiarEstado);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["solicitudId"] = solicitudId,
                ["nuevoEstadoId"] = nuevoEstadoId,
                ["usuarioAccionId"] = usuarioAccionId
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { solicitudId, nuevoEstadoId, usuarioAccionId });

                var resultado = await _repository.CambiarEstado(solicitudId, nuevoEstadoId, usuarioAccionId);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { solicitudId, Status = "Success" });

                return resultado ? "Estado de la solicitud actualizado exitosamente" : "No se pudo actualizar el estado de la solicitud";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al cambiar el estado de la solicitud: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las solicitudes por ID de usuario.
        /// </summary>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <returns>Lista de solicitudes del usuario.</returns>
        public async Task<List<SolicitudType>> ObtenerPorUsuarioId(int usuarioId)
        {
            const string operation = nameof(ObtenerPorUsuarioId);
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["usuarioId"] = usuarioId });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerPorUsuarioId(usuarioId);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener las solicitudes del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las solicitudes por ID de estado.
        /// </summary>
        /// <param name="estadoId">ID del estado.</param>
        /// <returns>Lista de solicitudes con el estado especificado.</returns>
        public async Task<List<SolicitudType>> ObtenerPorEstadoId(int estadoId)
        {
            const string operation = nameof(ObtenerPorEstadoId);
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["estadoId"] = estadoId });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerPorEstadoId(estadoId);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener las solicitudes por estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las solicitudes por filtros dinámicos.
        /// </summary>
        /// <param name="filtros">Filtros de búsqueda.</param>
        /// <returns>Lista de solicitudes que cumplen con los filtros.</returns>
        public async Task<List<SolicitudType>> ObtenerPorFiltros(FiltrosSolicitudType filtros)
        {
            const string operation = nameof(ObtenerPorFiltros);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["usuarioId"] = filtros.UsuarioId,
                ["estadoId"] = filtros.EstadoId,
                ["fechaInicio"] = filtros.FechaInicio,
                ["fechaFin"] = filtros.FechaFin
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerPorFiltros(filtros);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener las solicitudes por filtros: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el historial de auditoría por filtros.
        /// </summary>
        /// <param name="filtros">Filtros de búsqueda.</param>
        /// <returns>Lista de registros de auditoría que cumplen con los filtros.</returns>
        public async Task<List<HistorialAuditoriaType>> ObtenerHistorialAuditoria(FiltrosHistorialAuditoriaType filtros)
        {
            const string operation = nameof(ObtenerHistorialAuditoria);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["solicitudId"] = filtros.SolicitudId,
                ["usuarioId"] = filtros.UsuarioId,
                ["estadoAnteriorId"] = filtros.EstadoAnteriorId,
                ["estadoActualId"] = filtros.EstadoActualId,
                ["fechaInicio"] = filtros.FechaInicio,
                ["fechaFin"] = filtros.FechaFin
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerHistorialAuditoria(filtros);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener el historial de auditoría: {ex.Message}");
            }
        }
    }
}
