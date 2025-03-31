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
                .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && !p.IsHidden);

            if (publication == null) return NotFound();

            var commentsDTO = publication.Comments.Where(c => c.IsDeleted == false).Select(comment => new CommentResponseDTO
            {
                Id = comment.Id,
                NombreUsuario = comment.Usuario.UserName,
                Contenido = comment.Contenido,
                FechaComentario = comment.FechaComentario
            }).ToList();

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
                TotalComments = publication.Comments.Count,
                Comments = commentsDTO,
                LastEditedDate = publication.LastEditedDate
            };

            return Ok(publicationDTO);
        }

        // GET: api/publications
        [Authorize(Policy = "AllUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAllPublications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var publications = await _context.Publications
                    .Include(p => p.Likes)
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.Usuario)
                    .Where(p => !p.IsDeleted && !p.IsHidden)
                    .ToListAsync();

                var publicationsDTO = publications.Select(publication => new
                {
                    publication.Id,
                    publication.Title,
                    publication.Content,
                    publication.ImageUrl,
                    publication.PublicationDate,
                    publication.UserId,
                    publication.UserName,
                    TotalLikes = publication.Likes.Count,
                    TotalComments = publication.Comments.Count,
                    Comments = publication.Comments.Where(c => c.IsDeleted == false).Select(comment => new CommentResponseDTO
                    {
                        Id = comment.Id,
                        NombreUsuario = comment.Usuario.UserName,
                        Contenido = comment.Contenido,
                        FechaComentario = comment.FechaComentario
                    }).ToList(),
                    hasLiked = publication.Likes.Where(l => l.UsuarioId == Guid.Parse(userId) && l.IsDeleted == false).Any(),
                    LastEditedDate = publication.LastEditedDate // Mostrar la fecha de última edición
                }).ToList();

                return Ok(publicationsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las publicaciones");
                return StatusCode(500, "Error interno");
            }
        }

        // DELETE: api/publications/{id}
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            try
            {
                var publication = await _context.Publications.FindAsync(id);
                if (publication == null) return NotFound();

                publication.IsDeleted = true;
                await _context.SaveChangesAsync();

                return Ok("Publicación eliminada lógicamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando publicación");
                return StatusCode(500, "Error interno");
            }
        }

        // PATCH: api/publications/hide/{id}
        [Authorize(Policy = "AllUsers")]
        [HttpPatch("hide/{id}")]
        public async Task<IActionResult> HidePublication(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var publication = await _context.Publications.FindAsync(id);
                if (publication == null) return NotFound();

                if (publication.UserId != Guid.Parse(userId)) return Unauthorized("No puedes ocultar esta publicación.");

                publication.IsHidden = true;
                await _context.SaveChangesAsync();

                return Ok("Publicación oculta.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocultando publicación");
                return StatusCode(500, "Error interno");
            }
        }

        // PATCH: api/publications/unhide/{id}
        [Authorize(Policy = "AllUsers")]
        [HttpPatch("unhide/{id}")]
        public async Task<IActionResult> UnhidePublication(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var publication = await _context.Publications.FindAsync(id);
                if (publication == null) return NotFound();

                if (publication.UserId != Guid.Parse(userId)) return Unauthorized("No puedes desocultar esta publicación.");

                publication.IsHidden = false;
                await _context.SaveChangesAsync();

                return Ok("Publicación desocultada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error desocultando publicación");
                return StatusCode(500, "Error interno");
            }
        }

        // PUT: api/publications/{id}
        [Authorize(Policy = "AllUsers")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublication(int id, [FromBody] PublicationDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var publication = await _context.Publications.FindAsync(id);
                if (publication == null) return NotFound();

                if (publication.UserId != Guid.Parse(userId)) return Unauthorized("No puedes editar esta publicación.");

                publication.Title = dto.Title;
                publication.Content = dto.Content;
                publication.ImageUrl = dto.ImageUrl;
                publication.LastEditedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok("Publicación actualizada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando publicación");
                return StatusCode(500, "Error interno");
            }
        }
    }
}