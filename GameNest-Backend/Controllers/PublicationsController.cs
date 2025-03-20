using GameNest_Backend.Data;
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
        [Authorize(Policy = "AllUsers")]
        [HttpPost]
        public async Task<IActionResult> CreatePublication([FromBody] PublicationCreateDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var publication = new Publication
                {
                    UserId = userId,
                    Title = dto.Title,
                    Content = dto.Content,
                    ImageUrl = dto.ImageUrl,
                    PublicationDate = DateTime.UtcNow
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