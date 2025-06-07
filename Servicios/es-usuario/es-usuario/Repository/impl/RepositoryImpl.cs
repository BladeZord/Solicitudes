using Dapper;
using es_usuario.Constans;
using es_usuario.Controller.type;
using es_usuario.Repository.contract;
using es_usuario.utils;
using System.Data.SqlClient;
using System.Text;

namespace es_usuario.Repository.impl
{
    /// <summary>
    /// Implementación del repositorio para acceso a datos de catálogos
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
        /// Obtiene un catálogo por id desde una conexión abierta.
        /// </summary>
        /// <param name="id">Id del catálogo.</param>
        /// <param name="connection">Conexión existente.</param>
        /// <returns>Objeto de catálogo.</returns>
        private async Task<UsuarioType> ObtenerPorIdDesdeConexion(int id, SqlConnection connection)
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
                                   u.Id,
                                   u.Nombre,
                                   u.Correo,
                                   u.Rol_Id,
                                   c.Descripcion AS Rol_Descripcion
                             FROM Usuarios u
                             INNER JOIN catalogos c ON c.id = u.Rol_Id 
                             WHERE c.Tipo = 'TIPO_PERSONA' AND u.Id = @Id";

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);

                return await connection.QueryFirstOrDefaultAsync<UsuarioType>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
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
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                             SELECT 
                                   u.Id,
                                   u.Nombre,
                                   u.Correo,
                                   u.Rol_Id,
                                   c.Descripcion AS Rol_Descripcion
                             FROM Usuarios u
                             INNER JOIN catalogos c ON c.id = u.Rol_Id WHERE c.Tipo = 'TIPO_PERSONA'
                             ORDER BY u.Nombre";

                var result = (await connection.QueryAsync<UsuarioType>(sql)).ToList();

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
        /// Obtiene un catálogo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del catálogo a consultar.</param>
        /// <returns>Catálogo encontrado.</returns>
        public async Task<UsuarioType> ObtenerPorId(int id)
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
        /// Guarda un nuevo registro de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos a guardar.</param>
        /// <returns>Entidad guardada con su identificador asignado.</returns>
        public async Task<UsuarioType> Guardar(UsuarioType entityType)
        {
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, nameof(Guardar));
            const string operation = nameof(Guardar);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["nombre"] = entityType.Nombre,
                ["correo"] = entityType.Correo,
                ["rol_id"] = entityType.Rol_Id
            });

            try
            {
                using var connection = await _dbConexion.ObtenerConexion();
                using var transaction = connection.BeginTransaction();

                const string insertQuery = @"
                    INSERT INTO Usuarios 
                    (
                        Nombre,
                        Correo,
                        Rol_Id, 
                        Contrasenia
                    )
                    VALUES 
                    (
                        @Nombre,
                        @Correo,
                        @Rol_Id,
                        @Contrasenia
                    );
                    SELECT SCOPE_IDENTITY();";

                int id = await connection.ExecuteScalarAsync<int>(insertQuery, entityType, transaction);
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
        /// Actualiza un registro existente de catálogo.
        /// </summary>
        /// <param name="entityType">Entidad con los datos actualizados.</param>
        /// <returns>Entidad actualizada.</returns>
        public async Task<UsuarioType> Actualizar(UsuarioType entityType)
        {
            const string operation = nameof(Actualizar);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["nombre"] = entityType.Nombre,
                ["correo"] = entityType.Correo,
                ["rol_id"] = entityType.Rol_Id
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);
            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                    UPDATE Usuarios
                    SET Nombre = @Nombre,
                        Correo = @Correo,
                        Contrasenia = @Contrasenia,
                        Rol_Id = @Rol_Id
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, entityType);
                return await ObtenerPorIdDesdeConexion(entityType.Id, connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.UpdateError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Elimina un registro de catálogo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <returns>True si el registro fue eliminado exitosamente, false en caso contrario.</returns>
        public async Task<bool> Eliminar(int id)
        {
            const string operation = nameof(Eliminar);
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["id"] = id });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"DELETE FROM Usuarios WHERE Id = @Id";
                var rows = await connection.ExecuteAsync(sql, new { Id = id });

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
