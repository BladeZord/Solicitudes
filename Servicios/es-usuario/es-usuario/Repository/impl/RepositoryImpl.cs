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
                        u.Apellidos,
                        u.Correo,
                        u.Domicilio,
                        u.Telefono,
                        r.Id as Rol_Id,
                        r.codigo as Rol_Descripcion
                    FROM Usuarios u
                    LEFT JOIN Usuario_Roles ur ON ur.Usuario_Id = u.Id
                    LEFT JOIN Catalogos r ON r.Id = ur.Rol_Id
                    WHERE u.Id = @Id";

                var usuarioDict = new Dictionary<int, UsuarioType>();
                
                await connection.QueryAsync<UsuarioType, RolType, UsuarioType>(
                    sql,
                    (usuario, rol) =>
                    {
                        if (!usuarioDict.TryGetValue(usuario.Id, out var usuarioEntry))
                        {
                            usuarioEntry = usuario;
                            usuarioEntry.Roles = new List<string>();
                            usuarioDict.Add(usuario.Id, usuarioEntry);
                        }
                        if (rol != null)
                        {
                            usuarioEntry.Roles.Add(rol.Rol_Descripcion);
                        }
                        return usuarioEntry;
                    },
                    new { Id = id },
                    splitOn: "Rol_Id"
                );

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return usuarioDict.Values.FirstOrDefault();
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
                        u.Apellidos,
                        u.Correo,
                        u.Domicilio,
                        u.Telefono,
                        r.Id as Rol_Id,
                        r.codigo as Rol_Descripcion
                    FROM Usuarios u
                    LEFT JOIN Usuario_Roles ur ON ur.Usuario_Id = u.Id
                    LEFT JOIN Catalogos r ON r.Id = ur.Rol_Id
                    ORDER BY u.Nombre";

                var usuarioDict = new Dictionary<int, UsuarioType>();
                
                await connection.QueryAsync<UsuarioType, RolType, UsuarioType>(
                    sql,
                    (usuario, rol) =>
                    {
                        if (!usuarioDict.TryGetValue(usuario.Id, out var usuarioEntry))
                        {
                            usuarioEntry = usuario;
                            usuarioEntry.Roles = new List<string>();
                            usuarioDict.Add(usuario.Id, usuarioEntry);
                        }
                        if (rol != null)
                        {
                            usuarioEntry.Roles.Add(rol.Rol_Descripcion);
                        }
                        return usuarioEntry;
                    },
                    splitOn: "Rol_Id"
                );

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return usuarioDict.Values.ToList();
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
                ["correo"] = entityType.Correo
            });

            try
            {
                using var connection = await _dbConexion.ObtenerConexion();
                using var transaction = connection.BeginTransaction();

                const string insertQuery = @"
                    INSERT INTO Usuarios 
                    (
                        Nombre,
                        Apellidos,
                        Correo,
                        Contrasenia,
                        Domicilio,
                        Telefono
                    )
                    VALUES 
                    (
                        @Nombre,
                        @Apellidos,
                        @Correo,
                        @Contrasenia,
                        @Domicilio,
                        @Telefono
                    );
                    SELECT SCOPE_IDENTITY();";

                int id = await connection.ExecuteScalarAsync<int>(insertQuery, entityType, transaction);

                // Buscar el catálogo con código "SOLICITANTE" y asignar el rol por defecto
                const string buscarSolicitanteQuery = @"
                    SELECT Id FROM Catalogos WHERE Codigo = 'SOLICITANTE'";
                
                var rolSolicitanteId = await connection.ExecuteScalarAsync<int?>(buscarSolicitanteQuery, null, transaction);
                
                if (rolSolicitanteId.HasValue)
                {
                    const string insertRolQuery = @"
                        INSERT INTO Usuario_Roles (Usuario_Id, Rol_Id)
                        VALUES (@Usuario_Id, @Rol_Id)";
                    
                    await connection.ExecuteAsync(insertRolQuery, 
                        new { Usuario_Id = id, Rol_Id = rolSolicitanteId.Value }, 
                        transaction);
                }

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
                ["correo"] = entityType.Correo
            });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);
            try
            {
                var connection = await _dbConexion.ObtenerConexion();
                using var transaction = connection.BeginTransaction();

                var sql = @"
                    UPDATE Usuarios
                    SET Nombre = @Nombre,
                        Apellidos = @Apellidos,
                        Correo = @Correo,
                        Contrasenia = @Contrasenia,
                        Domicilio = @Domicilio,
                        Telefono = @Telefono
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, entityType, transaction);
                transaction.Commit();
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

        /// <summary>
        /// Compara si el usuario existe.
        /// </summary>
        /// <param name="AuthParam">Parametros de consulta.</param>
        /// <returns>Entidad guardada con su usuario y contrañas.</returns>
        public async Task<UsuarioType> ConsultarPorUsuarioYContrasenia(AuthType AuthParam)
        {
            const string operation = nameof(ConsultarPorUsuarioYContrasenia);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"
                            SELECT 
                                u.Id,
                                u.Nombre,
                                u.Apellidos,
                                u.Correo,
                                u.Contrasenia,
                                u.Domicilio,
                                u.Telefono,
                                r.Id as Rol_Id,
                                r.codigo as Rol_Descripcion
                            FROM Usuarios u
                            LEFT JOIN Usuario_Roles ur ON ur.Usuario_Id = u.Id
                            LEFT JOIN Catalogos r ON r.Id = ur.Rol_Id AND r.tipo = 'TIPO_PERSONA'
                            WHERE u.Correo = @Correo AND u.Contrasenia = @Contrasenia";


                var usuarioDict = new Dictionary<int, UsuarioType>();
                
                await connection.QueryAsync<UsuarioType, RolType, UsuarioType>(
                    sql,
                    (usuario, rol) =>
                    {
                        if (!usuarioDict.TryGetValue(usuario.Id, out var usuarioEntry))
                        {
                            usuarioEntry = usuario;
                            usuarioEntry.Nombre = $"{usuario.Nombre} {usuario.Apellidos}".Trim();
                            usuarioEntry.Roles = new List<string>();
                            usuarioDict[usuario.Id] = usuarioEntry;

                        }
                        if (rol != null)
                        {
                            usuarioEntry.Roles.Add(rol.Rol_Descripcion);
                        }
                        return usuarioEntry;
                    },
                    new { Correo = AuthParam.Correo, Contrasenia = AuthParam.Contrasenia },
                    splitOn: "Rol_Id"
                );

                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return usuarioDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        // Nuevos métodos para Usuario_Roles
        public async Task<List<UsuarioRolType>> ObtenerRolesPorUsuario(int usuarioId)
        {
            const string operation = nameof(ObtenerRolesPorUsuario);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();
                var sql = "SELECT Usuario_Id, Rol_Id FROM Usuario_Roles WHERE Usuario_Id = @UsuarioId";
                var result = (await connection.QueryAsync<UsuarioRolType>(sql, new { UsuarioId = usuarioId })).ToList();
                
                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        public async Task<bool> AsignarRolAUsuario(UsuarioRolType usuarioRol)
        {
            const string operation = nameof(AsignarRolAUsuario);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();
                var sql = @"
                    INSERT INTO Usuario_Roles (Usuario_Id, Rol_Id)
                    VALUES (@Usuario_Id, @Rol_Id)";
                
                var result = await connection.ExecuteAsync(sql, usuarioRol);
                
                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        public async Task<bool> QuitarRolDeUsuario(UsuarioRolType usuarioRol)
        {
            const string operation = nameof(QuitarRolDeUsuario);
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();
                var sql = @"
                    DELETE FROM Usuario_Roles 
                    WHERE Usuario_Id = @Usuario_Id AND Rol_Id = @Rol_Id";
                
                var result = await connection.ExecuteAsync(sql, usuarioRol);
                
                _logger.LogInformation(ApiConstants.LogMessages.OperationEnd, operation);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ApiConstants.LogMessages.OperationError, operation, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nuevaContrasenia">Nueva contraseña del usuario.</param>
        /// <returns>True si la contraseña fue actualizada exitosamente, false en caso contrario.</returns>
        public async Task<bool> ActualizarContrasenia(int id, string nuevaContrasenia)
        {
            const string operation = nameof(ActualizarContrasenia);
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["id"] = id });
            _logger.LogInformation(ApiConstants.LogMessages.OperationStart, operation);

            try
            {
                var connection = await _dbConexion.ObtenerConexion();

                var sql = @"UPDATE Usuarios SET Contrasenia = @Contrasenia WHERE Id = @Id";
                var rows = await connection.ExecuteAsync(sql, new { Id = id, Contrasenia = nuevaContrasenia });

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
