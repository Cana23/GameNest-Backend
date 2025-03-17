using GameNest_Backend.DTOs;
using GameNest_Backend.Migrations;
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
public class FollowersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;
    private readonly IFollowersService _followersService;
    public FollowersController(

        UserManager<User> userManager,
        IFollowersService followersService,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _followersService = followersService;
        _logger = logger;
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("/api/Followers/{id}")]
    public async Task<IActionResult> GetAllFollowers(string id)
    {
        try
        {
            var followers = _followersService.GetFollowers(id);
            var followerDto = new List<FollowerDTO>();

            foreach (var follower in followers)
            {
                var user = await _userManager.FindByIdAsync(follower.UsuarioSeguidorId);

                if (user == null)
                {
                    continue;
                }

                followerDto.Add(MapToDto(follower, user.UserName));
            }

            return Ok(followerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo seguidores.");
            return StatusCode(500, "Error interno");
        }
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("/api/FollowerCount/{id}")]
    public IActionResult GetFollowerCount(string id)
    {
        try
        {
            var followerCount = _followersService.GetFollowerCount(id);

            return Ok(followerCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo cantidad de seguidores.");
            return StatusCode(500, "Error interno");
        }
    }

    [Authorize(Policy = "AllUsers")]
    [HttpPost("/api/Follow/{id}")]
    public async Task<IActionResult> Follow(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario.");

            var follower = new Follower
            {
                UsuarioSeguidoId = id,
                UsuarioSeguidorId = userId
            };

            var result = await _followersService.Follow(follower);

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

    [Authorize(Policy = "AllUsers")]
    [HttpDelete("/api/UnFollow/{id}")]
    public async Task<IActionResult> UnFollow(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario.");

            var result = await _followersService.UnFollow(userId, id);

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


    private FollowerDTO MapToDto(Follower follower, string NombreUsuario)
    {
        return new FollowerDTO
        {
            NombreUsuario = NombreUsuario,
            UsuarioSeguidoId = follower.UsuarioSeguidoId
        };
    }
}

