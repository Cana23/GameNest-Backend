using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameNest_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Ahora ClaimTypes.NameIdentifier contiene el GUID (vía el claim "sub")
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("ID de usuario no encontrado en el token");
                    return BadRequest("ID de usuario no encontrado");
                }

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogError($"Formato de GUID inválido: {userIdClaim}");
                    return BadRequest("Formato de ID inválido");
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el perfil");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/users/publications
        [HttpGet("publications")]
        public async Task<IActionResult> GetUserPublications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var userGuid = Guid.Parse(userId); // Convertir a Guid

                var publications = await _context.Publications
                    .Where(p => p.UserId == userGuid)
                    .ToListAsync();

                return Ok(publications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo las publicaciones del usuario");
                return StatusCode(500, "Error interno");
            }
        }

        // POST: api/users/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var userGuid = Guid.Parse(userId); // Convertir a Guid

                var user = await _userManager.FindByIdAsync(userGuid.ToString());
                if (user == null) return NotFound();

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cambiando la contraseña");
                return StatusCode(500, "Error interno");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO model)
        {
            try
            {
                var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return Ok(new { message = "Usuario creado exitosamente" });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando el usuario");
                return StatusCode(500, "Error interno");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();

                var userProfile = new UserProfileDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FechaCreacion = user.FechaCreacion
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el usuario");
                return StatusCode(500, "Error interno");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();

                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el usuario");
                return StatusCode(500, "Error interno");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando el usuario");
                return StatusCode(500, "Error interno");
            }
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .Select(user => new UserListDTO
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FechaCreacion = user.FechaCreacion
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo la lista de usuarios");
                return StatusCode(500, "Error interno");
            }
        }

        // GET: api/users/role/user
        [HttpGet("role/user")]
        public async Task<IActionResult> GetUsersWithUserRole()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("User");

                var userDtos = users.Select(user => new UserListDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FechaCreacion = user.FechaCreacion
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo los usuarios con rol 'User'");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/users/role/admin
        [HttpGet("role/admin")]
        public async Task<IActionResult> GetUsersWithAdminRole()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("Admin");

                var userDtos = users.Select(user => new UserListDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FechaCreacion = user.FechaCreacion
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo los usuarios con rol 'Admin'");
                return StatusCode(500, "Error interno del servidor");
            }
        }


    }
}