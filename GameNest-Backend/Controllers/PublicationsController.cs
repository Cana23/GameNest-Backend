
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
    public class PublicationsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PublicationsController> _logger;

        public PublicationsController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            ILogger<PublicationsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // POST: api/publications
        [HttpPost]
        [Authorize(Policy = "AllUsers")] 
        public async Task<IActionResult> CreatePublication([FromBody] PublicationDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null || !Guid.TryParse(userId, out Guid userGuid))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound("Usuario no existe");

                var publication = new Publication
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    ImageUrl = dto.ImageUrl,
                    PublicationDate = DateTime.UtcNow,
                    UserId = userGuid // Asignar el UserId del token
                };

                _context.Publications.Add(publication);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPublication), new { id = publication.Id }, publication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando publicación");
                return StatusCode(500, "Error interno");
            }
        }

        // GET: api/publications/{id}
        [Authorize(Policy = "AllUsers")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublication(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication == null) return NotFound();

            return Ok(publication);
        }
    }
}