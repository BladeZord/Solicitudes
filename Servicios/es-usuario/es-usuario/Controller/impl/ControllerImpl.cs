using es_usuario.Constans;
using es_usuario.Controller.contract;
using es_usuario.Controller.type;
using es_usuario.Services.contract;
using es_usuario.exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace es_usuario.Controller.impl
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
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="service">Servicio para la lógica de negocio</param>
        /// <param name="logger">Servicio de logging</param>
        /// <param name="configuration">Servicio de configuración</param>
        public ControllerImpl(
            IService service, 
            ILogger<ControllerImpl> logger,
            IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica un usuario y genera un token JWT
        /// </summary>
        /// <param name="authType">Datos de autenticación del usuario</param>
        /// <returns>Token JWT y datos del usuario</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseType), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseType>> Login([FromBody] AuthType authType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = "Datos de entrada inválidos", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var usuario = await _service.ConsultarPorUsuarioYContrasenia(authType);

                if (usuario == null)
                {
                    return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
                }

                var token = GenerarToken(usuario);

                return Ok(new AuthResponseType
                {
                    Id = usuario.Id,
                    Token = token,
                    Correo = usuario.Correo,
                    Nombre = usuario.Nombre,
                    Roles = usuario.Roles
                });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación en la autenticación: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno en la autenticación: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        private string GenerarToken(UsuarioType usuario)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key) || key.Length < 32)
            {
                throw new InvalidOperationException("La clave JWT debe tener al menos 32 caracteres");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo)
            };

            // Agregar cada rol como un claim separado
            foreach (var rol in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(UsuarioType), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Actualizar(UsuarioType EntityType)
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al actualizar: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
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
        [ProducesResponseType(typeof(UsuarioType), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> Guardar(UsuarioType EntityType)
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al guardar: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
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
        [Authorize]
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al eliminar: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al eliminar: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el listado completo de catálogos.
        /// </summary>
        /// <returns>Lista de catálogos.</returns>
        /// <response code="200">Listado de catálogos obtenido exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioType>), StatusCodes.Status200OK)]
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al obtener listado: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al obtener listado: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
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
        [Authorize]
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(UsuarioType), StatusCodes.Status200OK)]
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
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al obtener por ID: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al obtener por ID: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los roles de un usuario específico.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <returns>Lista de roles del usuario.</returns>
        [Authorize]
        [HttpGet("roles/{usuarioId}")]
        [ProducesResponseType(typeof(List<UsuarioRolType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ObtenerRolesPorUsuario(int usuarioId)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ObtenerRolesPorUsuario",
                    usuarioId
                );

                var result = await _service.ObtenerRolesPorUsuario(usuarioId);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ObtenerRolesPorUsuario",
                    usuarioId,
                    "Success"
                );

                return Ok(result);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al obtener roles: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al obtener roles: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Asigna un rol a un usuario.
        /// </summary>
        /// <param name="usuarioRol">Relación usuario-rol a crear.</param>
        /// <returns>Resultado de la operación.</returns>
        [Authorize]
        [HttpPost("roles")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> AsignarRolAUsuario([FromBody] UsuarioRolType usuarioRol)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "AsignarRolAUsuario",
                    new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id }
                );

                var result = await _service.AsignarRolAUsuario(usuarioRol);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "AsignarRolAUsuario",
                    new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id },
                    "Success"
                );

                return Ok(new { Success = result });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.OperationError, "AsignarRolAUsuario", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al asignar rol: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Quita un rol de un usuario.
        /// </summary>
        /// <param name="usuarioRol">Relación usuario-rol a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
        [Authorize]
        [HttpDelete("roles")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> QuitarRolDeUsuario([FromBody] UsuarioRolType usuarioRol)
        {
            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "QuitarRolDeUsuario",
                    new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id }
                );

                var result = await _service.QuitarRolDeUsuario(usuarioRol);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "QuitarRolDeUsuario",
                    new { usuarioRol.Usuario_Id, usuarioRol.Rol_Id },
                    "Success"
                );

                return Ok(new { Success = result });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al quitar rol: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al quitar rol: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="actualizarContrasenia">Datos para actualizar la contraseña.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Contraseña actualizada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [Authorize]
        [HttpPut("contrasenia")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Produces(MimeType.JSON)]
        public async Task<ActionResult<object>> ActualizarContrasenia([FromBody] ActualizarContraseniaType actualizarContrasenia)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["usuarioId"] = actualizarContrasenia.Id
            });

            try
            {
                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationStart,
                    "ActualizarContrasenia",
                    actualizarContrasenia.Id
                );

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ApiConstants.ErrorMessages.InvalidInput, Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var result = await _service.ActualizarContrasenia(actualizarContrasenia.Id, actualizarContrasenia.NuevaContrasenia);

                _logger.LogInformation(
                    ApiConstants.LogMessages.OperationEnd,
                    "ActualizarContrasenia",
                    actualizarContrasenia.Id,
                    "Success"
                );

                return Ok(new { Success = true, Message = result });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error de validación al actualizar contraseña: {Message}", ex.Message);
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar contraseña: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = "Error interno del servidor" });
            }
        }
    }
}
