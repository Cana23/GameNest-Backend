using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // GET: api/users
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserResponseDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(MapToDto(user, roles));
            }

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo usuarios");
            return StatusCode(500, "Error interno");
        }
    }

    // GET: api/users/{id}
    [Authorize(Policy = "AllUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(MapToDto(user, roles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error obteniendo usuario {id}");
            return StatusCode(500, "Error interno");
        }
    }

    // POST: api/users
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
        try
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FechaCreacion = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Asignar roles
            if (dto.Roles != null)
            {
                foreach (var role in dto.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando usuario");
            return StatusCode(500, "Error interno");
        }
    }

    // PUT: api/users/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDTO dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Actualizar propiedades
            user.UserName = dto.UserName ?? user.UserName;
            user.Email = dto.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Actualizar roles
            if (dto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                foreach (var role in dto.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            return Ok(MapToDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando usuario {id}");
            return StatusCode(500, "Error interno");
        }
    }

    // DELETE: api/users/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded
                ? NoContent()
                : BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error eliminando usuario {id}");
            return StatusCode(500, "Error interno");
        }
    }

    private UserResponseDTO MapToDto(User user, IList<string> roles = null)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FechaCreacion = user.FechaCreacion,
            Roles = roles?.ToList() ?? new List<string>()
        };
    }
}
