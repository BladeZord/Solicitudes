using Dapper;
using es_solicitudes.Constans;
using es_solicitudes.Controller.type;
using es_solicitudes.Repository.contract;
using es_solicitudes.utils;
using System.Data.SqlClient;
using System.Text;

namespace es_solicitudes.Repository.impl
{
    /// <summary>
    /// Implementación del repositorio para acceso a datos de solicitudes
    /// </summary>
    public class RepositoryImpl : IRepository
    {
        private readonly DbConnectionManager _dbConexion;
        private readonly ILogger<RepositoryImpl> _logger;

        /// <summary>
        /// Constructor del repositorio
        /// </summary>
        /// <param name="dbConexion">Gestor de conexiones a la base de datos</param>
        /// <param name="logger">Servicio de logging</param>
        public RepositoryImpl(DbConnectionManager dbConexion, ILogger<RepositoryImpl> logger)
        {
            _dbConexion = dbConexion;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una solicitud por id desde una conexión abierta.
        /// </summary>
        /// <param name="id">Id de la solicitud.</param>
        /// <param name="connection">Conexión existente.</param>
        /// <returns>Objeto de solicitud.</returns>
        private async Task<SolicitudType> ObtenerPorIdDesdeConexion(int id, SqlConnection connection)
        {
            const string operation = nameof(ObtenerPorIdDesdeConexion);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = id,
                ["conexion"] = connection
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);
            try
            {
                var sql = @"
                    SELECT 
                        s.Id,
                        ISNULL(s.Codigo, '') AS Codigo,
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        ISNULL(c.Descripcion, '') AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        ISNULL(u.Nombre + ' ' + u.Apellidos, '') AS Nombre_Usuario
                    FROM Solicitudes s
                    INNER JOIN Catalogos c ON c.Id = s.Estado_Id AND c.Tipo = 'ESTADO_SOLICITUD'
                    INNER JOIN Usuarios u ON u.Id = s.Usuario_Id
                    WHERE s.Id = @Id";

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);

                return await connection.QueryFirstOrDefaultAsync<SolicitudType>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las solicitudes.
        /// </summary>
        /// <returns>Lista de solicitudes.</returns>
        public async Task<List<SolicitudType>> ObtenerTodos()
        {
            const string operation = nameof(ObtenerTodos);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                    SELECT 
                        s.Id,
                        ISNULL(s.Codigo, '') AS Codigo,
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        ISNULL(c.Descripcion, '') AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        ISNULL(u.Nombre + ' ' + u.Apellidos, '') AS Nombre_Usuario
                    FROM Solicitudes s
                    INNER JOIN Catalogos c ON c.Id = s.Estado_Id AND c.Tipo = 'ESTADO_SOLICITUD'
                    INNER JOIN Usuarios u ON u.Id = s.Usuario_Id
                    ORDER BY s.Fecha_registro DESC";

                var result = (await connection.QueryAsync<SolicitudType>(sql)).ToList();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
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
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["id"] = id });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();
                var result = await ObtenerPorIdDesdeConexion(id, connection);
                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Guarda una nueva solicitud.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        public async Task<SolicitudType> Guardar(SolicitudType entityType)
        {
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, nameof(Guardar));
            const string operation = nameof(Guardar);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["monto"] = entityType.Monto,
                ["plazo_meses"] = entityType.PlazoMeses,
                ["ingresos_mensual"] = entityType.IngresosMensual,
                ["antiguedad_laboral"] = entityType.AntiguedadLaboral,
                ["usuario_id"] = entityType.Usuario_Id
            });

            try
            {
                using var connection = await _dbConexion.ObtenerConexion();
                using var transaction = connection.BeginTransaction();

                var parameters = new
                {
                    entityType.Monto,
                    entityType.PlazoMeses,
                    entityType.IngresosMensual,
                    entityType.AntiguedadLaboral,
                    entityType.Usuario_Id
                };

                var id = await connection.ExecuteScalarAsync<int>("sp_CrearSolicitud", parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return await ObtenerPorIdDesdeConexion(id, connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
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
                ["id"] = entityType.Id,
                ["monto"] = entityType.Monto,
                ["plazo_meses"] = entityType.PlazoMeses,
                ["ingresos_mensual"] = entityType.IngresosMensual,
                ["antiguedad_laboral"] = entityType.AntiguedadLaboral
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);
            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var parameters = new
                {
                    SolicitudId = entityType.Id,
                    entityType.Monto,
                    entityType.PlazoMeses,
                    entityType.IngresosMensual,
                    entityType.AntiguedadLaboral
                };

                await connection.ExecuteAsync("sp_ActualizarSolicitud", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return await ObtenerPorIdDesdeConexion(entityType.Id, connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.UpdateError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Elimina una solicitud cambiando su estado.
        /// </summary>
        /// <param name="id">Identificador de la solicitud a eliminar.</param>
        /// <returns>True si la solicitud fue eliminada exitosamente, false en caso contrario.</returns>
        public async Task<bool> Eliminar(int id)
        {
            const string operation = nameof(Eliminar);
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["id"] = id });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var parameters = new
                {
                    SolicitudId = id,
                    EstadoEliminadoId = 3 // 3 = Estado Eliminado
                };

                var rows = await connection.ExecuteAsync("sp_EliminarSolicitud", parameters, commandType: System.Data.CommandType.StoredProcedure);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Cambia el estado de una solicitud y registra la acción en el log de auditoría.
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a modificar.</param>
        /// <param name="nuevoEstadoId">ID del nuevo estado.</param>
        /// <param name="usuarioAccionId">ID del usuario que realiza la acción.</param>
        /// <returns>True si el cambio fue exitoso, false en caso contrario.</returns>
        public async Task<bool> CambiarEstado(int solicitudId, int nuevoEstadoId, int usuarioAccionId)
        {
            const string operation = nameof(CambiarEstado);
            using var scope = _logger.BeginScope(new Dictionary<string, object> 
            { 
                ["solicitudId"] = solicitudId,
                ["nuevoEstadoId"] = nuevoEstadoId,
                ["usuarioAccionId"] = usuarioAccionId
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var parameters = new
                {
                    SolicitudId = solicitudId,
                    NuevoEstadoId = nuevoEstadoId,
                    UsuarioAccionId = usuarioAccionId
                };

                var rows = await connection.ExecuteAsync("sp_CambiarEstadoSolicitudYAuditar", parameters, commandType: System.Data.CommandType.StoredProcedure);

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
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
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                    SELECT 
                        s.Id,
                        ISNULL(s.Codigo, '') AS Codigo,
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        ISNULL(c.Descripcion, '') AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        ISNULL(u.Nombre + ' ' + u.Apellidos, '') AS Nombre_Usuario
                    FROM Solicitudes s
                    INNER JOIN Catalogos c ON c.Id = s.Estado_Id AND c.Tipo = 'ESTADO_SOLICITUD'
                    INNER JOIN Usuarios u ON u.Id = s.Usuario_Id
                    WHERE s.Usuario_Id = @UsuarioId
                    ORDER BY s.Fecha_registro DESC";

                var result = (await connection.QueryAsync<SolicitudType>(sql, new { UsuarioId = usuarioId })).ToList();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
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
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                    SELECT 
                        s.Id,
                        ISNULL(s.Codigo, '') AS Codigo,
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        ISNULL(c.Descripcion, '') AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        ISNULL(u.Nombre + ' ' + u.Apellidos, '') AS Nombre_Usuario
                    FROM Solicitudes s
                    INNER JOIN Catalogos c ON c.Id = s.Estado_Id AND c.Tipo = 'ESTADO_SOLICITUD'
                    INNER JOIN Usuarios u ON u.Id = s.Usuario_Id
                    WHERE s.Estado_Id = @EstadoId
                    ORDER BY s.Fecha_registro DESC";

                var result = (await connection.QueryAsync<SolicitudType>(sql, new { EstadoId = estadoId })).ToList();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
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
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = new StringBuilder(@"
                    SELECT 
                        s.Id,
                        ISNULL(s.Codigo, '') AS Codigo,
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        ISNULL(c.Descripcion, '') AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        ISNULL(u.Nombre + ' ' + u.Apellidos, '') AS Nombre_Usuario
                    FROM Solicitudes s
                    INNER JOIN Catalogos c ON c.Id = s.Estado_Id AND c.Tipo = 'ESTADO_SOLICITUD'
                    INNER JOIN Usuarios u ON u.Id = s.Usuario_Id
                    WHERE 1=1");

                var parameters = new DynamicParameters();

                if (filtros.UsuarioId.HasValue && filtros.UsuarioId.Value != 0)
                {
                    sql.Append(" AND s.Usuario_Id = @UsuarioId");
                    parameters.Add("UsuarioId", filtros.UsuarioId.Value);
                }

                if (filtros.EstadoId.HasValue && filtros.EstadoId.Value != 0)
                {
                    sql.Append(" AND s.Estado_Id = @EstadoId");
                    parameters.Add("EstadoId", filtros.EstadoId.Value);
                }


                if (filtros.FechaInicio.HasValue)
                {
                    sql.Append(" AND s.Fecha_registro >= @FechaInicio");
                    parameters.Add("FechaInicio", filtros.FechaInicio.Value);
                }

                if (filtros.FechaFin.HasValue)
                {
                    sql.Append(" AND s.Fecha_registro <= @FechaFin");
                    parameters.Add("FechaFin", filtros.FechaFin.Value);
                }

                sql.Append(" ORDER BY s.Fecha_registro DESC");

                var result = (await connection.QueryAsync<SolicitudType>(sql.ToString(), parameters)).ToList();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
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
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var parameters = new
                {
                    SolicitudId = filtros.SolicitudId,
                    UsuarioId = filtros.UsuarioId,
                    EstadoAnteriorId = filtros.EstadoAnteriorId,
                    EstadoActualId = filtros.EstadoActualId,
                    FechaInicio = filtros.FechaInicio,
                    FechaFin = filtros.FechaFin
                };

                var result = (await connection.QueryAsync<HistorialAuditoriaType>("sp_ConsultarHistorialAuditoria", parameters, commandType: System.Data.CommandType.StoredProcedure)).ToList();

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }
    }
}
