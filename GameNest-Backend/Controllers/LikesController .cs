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
public class LikesController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<LikesController> _logger;
    private readonly ILikesService _likesService;

    public LikesController(
        UserManager<User> userManager,
        ILikesService likesService,
        ILogger<LikesController> logger)
    {
        _userManager = userManager;
        _likesService = likesService;
        _logger = logger;
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("{id}")]
    public IActionResult GetPostLikes(int id)
    {
        try
        {
            var likes = _likesService.GetPostLikes(id);
            return Ok(likes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo likes.");
            return StatusCode(500, "Error interno");
        }
    }

    [Authorize(Policy = "AllUsers")]
    [HttpPost("{id}")]
    public async Task<IActionResult> AddLike(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario.");

            var like = new Like
            {
                UsuarioId = Guid.Parse(userId),
                PublicacionId = id,
            };

            var result = await _likesService.AddLike(like);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error agregando like.");
            return StatusCode(500, "Error interno");
        }
    }

    [Authorize(Policy = "AllUsers")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveLike(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario.");

            var like = await _likesService.GetLike(id);

            if (like == null) return BadRequest("Esta publicación no existe.");

            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = like.UsuarioId == Guid.Parse(userId);

            if (!isOwner && !isAdmin)
                return Unauthorized("No se puede eliminar un like de otro usuario.");

            var result = await _likesService.RemoveLike(id, like);

            if (!result.Success) return StatusCode(500, result.Message);

            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error eliminando like {id}");
            return StatusCode(500, "Error interno");
        }
    }
}