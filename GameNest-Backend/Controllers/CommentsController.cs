using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using GameNest_Backend.Services;


namespace GameNest_Backend.Controllers
{
    namespace GameNest_Backend.Controllers
    {
        [Authorize]
        [ApiController]
        [Route("api/[controller]")]
        public class CommentsController : ControllerBase
        {
            private readonly UserManager<User> _userManager;
            private readonly ILogger<UsersController> _logger;
            private readonly ICommentsService _commentService;
            public CommentsController(

                UserManager<User> userManager,
                ICommentsService commentService,
                ILogger<UsersController> logger)
            {
                _userManager = userManager;
                _commentService = commentService;
                _logger = logger;
            }

            [Authorize(Policy = "AdminOnly")]
            [HttpGet]
            public async Task<IActionResult> GetAllComments()
            {
                try
                {
                    var comments = _commentService.GetAllComments();
                    var commentDtos = new List<CommentResponseDTO>();

                    foreach (var comment in comments)
                    {
                        var user = await _userManager.FindByIdAsync(comment.UsuarioId.ToString());

                        if (user == null)
                        {
                            continue;
                        }

                        commentDtos.Add(MapToDto(comment, user.UserName));
                    }

                    return Ok(commentDtos);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error obteniendo comentarios.");
                    return StatusCode(500, "Error interno");
                }
            }
            // POST: api/comments
            [HttpPost]
            public async Task<IActionResult> CreateComment([FromBody] CommentCreateDTO dto)
            {
                try
                {
                    // Obtener el UserId del Claim
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null) return Unauthorized();

                    var comment = await _commentService.CreateCommentAsync(dto, Guid.Parse(userId));

                    // Retorna la respuesta de creación
                    return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno");
                }
            }

            // GET: api/comments/{id}
            [HttpGet("{id}")]
            public async Task<IActionResult> GetComment(int id)
            {
                var comment = await _commentService.GetComment(id);
                if (comment == null) return NotFound();

                return Ok(comment);
            }

            // PUT: api/Comments/{CommentDTO}
            [Authorize(Policy = "AllUsers")]
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateDTO dto)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized("No se pudo obtener el ID del usuario.");


                    var comment = await _commentService.GetComment(id);

                    if (comment == null)
                        return NotFound("Comentario no encontrado.");

                    if (Guid.Parse(userId) != comment.UsuarioId)
                    {
                        return Unauthorized("No se puede modificar un comentario de otro usuario.");
                    }

                    var result = await _commentService.UpdateComment(dto, comment);

                    if (!result.Success)
                        return BadRequest(result.Message);

                    return Ok(result.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno");
                }
            }

            // DELETE: api/Comments/{CommentId}    
            [Authorize(Policy = "AllUsers")]
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteComment(int id)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized("No se pudo obtener el ID del usuario.");

                    var comment = await _commentService.GetComment(id);

                    if (comment == null) return BadRequest("Este comentario no existe.");

                    bool isAdmin = User.IsInRole("Admin");
                    bool isOwner = comment.UsuarioId == Guid.Parse(userId);

                    if (!isOwner && !isAdmin)
                        return Unauthorized("No se puede eliminar un comentario de otro usuario.");

                    var result = await _commentService.DeleteComment(id);

                    if (!result.Success) return StatusCode(500, result.Message);

                    return Ok(result.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno");
                }
            }

            private CommentResponseDTO MapToDto(Comment comment, string NombreUsuario)
            {
                return new CommentResponseDTO
                {
                    Id = comment.Id,
                    NombreUsuario = NombreUsuario,
                    Contenido = comment.Contenido,
                    FechaComentario = comment.FechaComentario,
                };
            }
        }
    }
}