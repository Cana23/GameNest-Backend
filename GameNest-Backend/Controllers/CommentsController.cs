using GameNest_Backend.Controllers;
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
                // Reemplazar la línea que causa el error
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
                var user = await _userManager.FindByIdAsync(comment.UsuarioId.ToString()); // Convertir a string

                if (user == null) continue;

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
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Convertir userId (string) a Guid
            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Formato de ID de usuario inválido");

            var comment = new Comment
            {
                UsuarioId = userGuid, // Usar Guid
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
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Formato de ID de usuario inválido");

            var comment = await _commentsService.GetComment(id);
            if (comment == null) return NotFound();

            if (userGuid != comment.UsuarioId) 
                return Unauthorized("No puedes editar este comentario");

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
            if (string.IsNullOrEmpty(userId)) return Unauthorized();


            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Formato de ID de usuario inválido");

            var comment = await _commentsService.GetComment(id);
            if (comment == null) return NotFound();

            bool isOwner = comment.UsuarioId == userGuid; 
            bool isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isAdmin)
                return Unauthorized("No puedes eliminar este comentario");

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

