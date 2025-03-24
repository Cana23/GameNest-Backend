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
            private readonly ICommentsService _commentService;

            public CommentsController(ICommentsService commentService)
            {
                _commentService = commentService;
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
                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null) return NotFound();

                return Ok(comment);
            }
        }
    }
}