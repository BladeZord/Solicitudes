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
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        c.Descripcion AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        u.Nombre AS Nombre_Usuario
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
                        s.Monto,
                        s.Plazo_meses AS PlazoMeses,
                        s.Ingresos_mensual AS IngresosMensual,
                        s.Antiguedad_laboral AS AntiguedadLaboral,
                        s.Estado_Id,
                        c.Descripcion AS Estado_Descripcion,
                        s.Fecha_registro AS FechaRegistro,
                        s.Usuario_Id,
                        u.Nombre AS Nombre_Usuario
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
                ["estado_id"] = entityType.Estado_Id,
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
                    entityType.Estado_Id,
                    entityType.Usuario_Id
                };

                await connection.ExecuteAsync("sp_CrearSolicitud", parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();

                // Obtener el ID de la solicitud creada
                var sql = "SELECT TOP 1 Id FROM Solicitudes WHERE Usuario_Id = @Usuario_Id ORDER BY Fecha_registro DESC";
                var id = await connection.ExecuteScalarAsync<int>(sql, new { entityType.Usuario_Id });

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
    }
}
