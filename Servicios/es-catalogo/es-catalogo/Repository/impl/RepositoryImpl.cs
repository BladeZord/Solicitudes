using Dapper;
using es_catalogo.Constans;
using es_catalogo.Controller.type;
using es_catalogo.Repository.contract;
using es_catalogo.utils;
using System.Data.SqlClient;

namespace es_catalogo.Repository.impl
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
        private async Task<CatalogoType> ObtenerPorIdDesdeConexion(int id, SqlConnection connection)
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
                        Id,
                        Codigo,
                        Descripcion,
                        ISNULL(Padre_Id, 0) AS Padre_Id,
                        ISNULL(Tipo, '') AS Tipo
                    FROM Catalogos
                    WHERE Id = @Id";

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);

                return await connection.QueryFirstOrDefaultAsync<CatalogoType>(sql, new { Id = id });
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
        public async Task<List<CatalogoType>> ObtenerTodos()
        {
            const string operation = nameof(ObtenerTodos);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                const string sql = @"
                                    SELECT 
                                        Id,
                                        Codigo,
                                        Descripcion,
                                        ISNULL(Padre_Id, 0) AS Padre_Id,
                                        ISNULL(Tipo, '') AS Tipo
                                    FROM Catalogos
                                    ORDER BY Codigo";


                var result = (await connection.QueryAsync<CatalogoType>(sql)).ToList();

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
        /// Obtiene todos los registros de catálogos por el tipo.
        /// </summary>
        /// <param name="Tipo">Tipo de parametros.</param>

        /// <returns>Lista de catálogos.</returns>
        public async Task<List<CatalogoType>> ObtenerPorTipo(string? Tipo)
        {
            const string operation = nameof(ObtenerPorTipo);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                string sql;
                var parameters = new DynamicParameters();

                var connection = await _dbConexion.ObtenerConexion();

                if (string.IsNullOrWhiteSpace(Tipo))
                {
                    // Si Tipo es NULL, vacío o espacios → traer registros *sin tipo*
                    sql = @"
                            SELECT 
                                Id,
                                Codigo,
                                Descripcion,
                                ISNULL(Padre_Id, 0) AS Padre_Id,
                                ISNULL(Tipo, '') AS Tipo
                            FROM Catalogos
                            WHERE (Tipo IS NULL OR Tipo = '') AND (Padre_Id = 0 OR Padre_Id IS NULL)
                            ORDER BY Codigo;";
                }
                else
                {
                    // Si Tipo tiene valor → traer los registros con ese Tipo
                    sql = @"
                            SELECT 
                                Id,
                                Codigo,
                                Descripcion,
                                ISNULL(Padre_Id, 0) AS Padre_Id,
                                ISNULL(Tipo, '') AS Tipo
                            FROM Catalogos
                            WHERE Tipo = @Tipo
                            ORDER BY Codigo;";

                    parameters.Add("Tipo", Tipo);
                }

                var result = (await connection.QueryAsync<CatalogoType>(sql, parameters)).ToList();

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
        public async Task<CatalogoType> ObtenerPorId(int id)
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
        public async Task<CatalogoType> Guardar(CatalogoType entityType)
        {
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, nameof(Guardar));
            const string operation = nameof(Guardar);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["codigo"] = entityType.Codigo,
                ["descripcion"] = entityType.Descripcion,
                ["padreId"] = entityType.Padre_Id
            });

            try
            {
                using var connection = await _dbConexion.ObtenerConexion();
                using var transaction = connection.BeginTransaction();

                const string insertQuery = @"
                    INSERT INTO Catalogos 
                    (
                        Codigo,
                        Descripcion,
                        Padre_Id,
                        Tipo
                    )
                    VALUES 
                    (
                        @Codigo,
                        @Descripcion,
                        @Padre_Id,
                        @Tipo
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
        public async Task<CatalogoType> Actualizar(CatalogoType entityType)
        {
            const string operation = nameof(Actualizar);
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["id"] = entityType.Id,
                ["codigo"] = entityType.Codigo,
                ["descripcion"] = entityType.Descripcion,
                ["padreId"] = entityType.Padre_Id
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);
            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                    UPDATE Catalogos
                    SET Codigo = @Codigo,
                        Descripcion = @Descripcion,
                        Padre_Id = @Padre_Id,
                        Tipo = @Tipo
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

                var sql = @"DELETE FROM Catalogos WHERE Id = @Id";
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
