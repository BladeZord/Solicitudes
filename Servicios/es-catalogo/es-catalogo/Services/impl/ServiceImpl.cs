using es_catalogo.Constans;
using es_catalogo.Controller.type;
using es_catalogo.exception;
using es_catalogo.Repository.contract;
using es_catalogo.Services.contract;

namespace es_catalogo.Services.impl
{
    /// <summary>
    /// Implementación del servicio para la gestión de catálogos
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
        /// Obtiene todos los registros de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        public async Task<List<CatalogoType>> ObtenerTodos()
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
        /// Obtiene todos los registros de catálogos por el tipo.
        /// </summary>
        /// <param name="Tipo">Tipo de parametros.</param>
        /// <returns>Lista de catálogos.</returns>
        public async Task<List<CatalogoType>> ObtenerPorTipo(string? Tipo)
        {
            const string operation = nameof(ObtenerPorTipo);

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                var resultado = await _repository.ObtenerPorTipo(Tipo);

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
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        public async Task<CatalogoType> ObtenerPorId(int id)
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
                throw new ServiceException($"Error al obtener el catálogo: {ex.Message}");
            }
        }

        /// <summary>
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        public async Task<CatalogoType> Guardar(CatalogoType entityType)
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
                throw new ServiceException($"Error al guardar el catálogo: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        public async Task<CatalogoType> Actualizar(CatalogoType entityType)
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
                throw new ServiceException($"Error al actualizar el catálogo: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un registro de catálogo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
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
                if (!resultado)
                    throw new ServiceException("No se pudo eliminar el catálogo");

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { id, Status = "Success" });

                return "Catálogo eliminado exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al eliminar el catálogo: {ex.Message}");
            }
        }
    }
}
