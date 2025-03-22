using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        try
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                _logger.LogInformation($"Usuario {model.UserName} registrado exitosamente");
                // Generar token JWT después del registro
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                return Ok(new { message = "Registro exitoso", token });
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        _logger.LogInformation($"Intento de login con email: {loginDto.Email}");

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogWarning($"Usuario con email {loginDto.Email} no encontrado.");
            return Unauthorized();
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
        {
            _logger.LogWarning($"Contraseña incorrecta para el usuario con email {loginDto.Email}.");
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        _logger.LogInformation($"Login exitoso para el usuario con email {loginDto.Email}.");

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            // Añadir los roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

