using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using GameNest_Backend.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UsersController> _logger;
    private readonly ICommentsService _commentsService;
    public CommentsController(

        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ICommentsService commentsService,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _commentsService = commentsService;
        _logger = logger;
    }
    // GET: api/Comments
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        try
        {
            var comments = _commentsService.GetAllComments();
            var commentDtos = new List<CommentResponseDTO>();

            foreach (var comment in comments)
            {
                var user = await _userManager.FindByIdAsync(comment.UsuarioId);

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

    // GET: api/Comments/{PostId}
    [Authorize(Policy = "AllUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostComments(int id)
    {
        try
        {
            var comments = _commentsService.GetPostComments(id);
            var commentDtos = new List<CommentResponseDTO>();

            foreach (var comment in comments)
            {
                var user = await _userManager.FindByIdAsync(comment.UsuarioId);

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

    // POST: api/Comments
    [Authorize(Policy = "AllUsers")]
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentCreateDTO dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario.");

            var comment = new Comment
            {
                UsuarioId = userId,
                Contenido = dto.Contenido,
                PublicacionId = dto.PublicacionId,
                FechaComentario = DateTime.UtcNow
            };

            var result = await _commentsService.CreateComment(comment);

            if (!result.Success)
                return BadRequest(result.Message);


            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando comentario.");
            return StatusCode(500, "Error interno");
        }
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


            var comment = await _commentsService.GetComment(id);

            if (comment == null)
                return NotFound("Comentario no encontrado.");

            if (userId != comment.UsuarioId)
            {
                return Unauthorized("No se puede modificar un comentario de otro usuario.");
            }

            var result = await _commentsService.UpdateComment(dto, comment);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error editando comentario {id}");
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

            var comment = await _commentsService.GetComment(id);

            if (comment == null) return BadRequest("Este comentario no existe.");

            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = comment.UsuarioId == userId;

            if (!isOwner && !isAdmin)
                return Unauthorized("No se puede eliminar un comentario de otro usuario.");

            var result = await _commentsService.DeleteComment(id);

            if (!result.Success) return StatusCode(500, result.Message);

            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error eliminando comentario {id}");
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

