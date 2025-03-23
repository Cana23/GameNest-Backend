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
        public async Task<IActionResult> CreatePublication([FromBody] PublicationDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return Unauthorized();

                var publication = new Publication
                {
                    UserId = Guid.Parse(userId),
                    UserName = user.UserName,
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
            var publication = await _context.Publications
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publication == null) return NotFound();

            var publicationDTO = new
            {
                publication.Id,
                publication.Title,
                publication.Content,
                publication.ImageUrl,
                publication.PublicationDate,
                publication.UserId,
                publication.UserName,
                TotalLikes = publication.Likes.Count,
                TotalComments = publication.Comments.Count
            };

            return Ok(publicationDTO);
        }
    }
}