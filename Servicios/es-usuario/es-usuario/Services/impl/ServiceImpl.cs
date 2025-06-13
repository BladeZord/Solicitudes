using es_usuario.Constans;
using es_usuario.Controller.type;
using es_usuario.exception;
using es_usuario.Repository.contract;
using es_usuario.Services.contract;
using es_usuario.utils;

namespace es_usuario.Services.impl
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
        /// Consulta un usuario por su correo y contraseña
        /// </summary>
        /// <param name="authType">Datos de autenticación del usuario</param>
        /// <returns>Usuario encontrado o null si no existe</returns>
        public async Task<UsuarioType> ConsultarPorUsuarioYContrasenia(AuthType authType)
        {
            const string operation = nameof(ConsultarPorUsuarioYContrasenia);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["correo"] = authType.Correo
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

                // Sanitizar las credenciales
                var (correoSanitizado, contraseniaSanitizada) = SanitizarInput.SanitizarCredenciales(authType.Correo, authType.Contrasenia);
                
                if (correoSanitizado == null || contraseniaSanitizada == null)
                {
                    throw new ServiceException("Credenciales inválidas o mal formateadas");
                }

                var usuario = await _repository.ConsultarPorUsuarioYContrasenia(new AuthType 
                { 
                    Correo = correoSanitizado, 
                    Contrasenia = contraseniaSanitizada 
                });

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);

                return usuario;
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los registros de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        public async Task<List<UsuarioType>> ObtenerTodos()
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
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        public async Task<UsuarioType> ObtenerPorId(int id)
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
        public async Task<UsuarioType> Guardar(UsuarioType entityType)
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
                throw new ServiceException($"Error al guardar el usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        public async Task<UsuarioType> Actualizar(UsuarioType entityType)
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
                throw new ServiceException($"Error al actualizar el usuario: {ex.Message}");
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

        // Nuevos métodos para Usuario_Roles
        public async Task<List<UsuarioRolType>> ObtenerRolesPorUsuario(int usuarioId)
        {
            const string operation = nameof(ObtenerRolesPorUsuario);

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { usuarioId });

                var resultado = await _repository.ObtenerRolesPorUsuario(usuarioId);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { usuarioId, Count = resultado.Count });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al obtener roles del usuario: {ex.Message}");
            }
        }

        public async Task<bool> AsignarRolAUsuario(UsuarioRolType usuarioRol)
        {
            const string operation = nameof(AsignarRolAUsuario);

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id });

                var resultado = await _repository.AsignarRolAUsuario(usuarioRol);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id, Status = resultado });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al asignar rol al usuario: {ex.Message}");
            }
        }

        public async Task<bool> QuitarRolDeUsuario(UsuarioRolType usuarioRol)
        {
            const string operation = nameof(QuitarRolDeUsuario);

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id });

                var resultado = await _repository.QuitarRolDeUsuario(usuarioRol);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id, Status = resultado });

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al quitar rol del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nuevaContrasenia">Nueva contraseña del usuario.</param>
        /// <returns>Mensaje con el resultado de la operación.</returns>
        public async Task<string> ActualizarContrasenia(int id, string nuevaContrasenia)
        {
            const string operation = nameof(ActualizarContrasenia);

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = id
            });

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation, new { id });

                // Sanitizar la nueva contraseña
                var contraseniaSanitizada = SanitizarInput.SanitizarContrasenia(nuevaContrasenia);
                
                if (contraseniaSanitizada == null)
                {
                    throw new ServiceException("La nueva contraseña no cumple con los requisitos mínimos de seguridad");
                }

                var resultado = await _repository.ActualizarContrasenia(id, contraseniaSanitizada);

                if (!resultado)
                {
                    throw new ServiceException("No se pudo actualizar la contraseña. El usuario no fue encontrado.");
                }

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation, new { id, Status = "Success" });

                return "Contraseña actualizada exitosamente";
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw new ServiceException($"Error al actualizar la contraseña: {ex.Message}");
            }
        }
    }
}
