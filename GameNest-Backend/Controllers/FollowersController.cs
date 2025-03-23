using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using GameNest_Backend.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameNest_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FollowersController : ControllerBase
    {
        private readonly IFollowersService _followersService;
        private readonly ILogger<FollowersController> _logger;

        public FollowersController(IFollowersService followersService, ILogger<FollowersController> logger)
        {
            _followersService = followersService;
            _logger = logger;
        }

        // GET: api/followers/count
        [HttpGet("count")]
        public IActionResult GetFollowerCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var followerCount = _followersService.GetFollowerCount(Guid.Parse(userId));
                return Ok(followerCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el conteo de seguidores.");
                return StatusCode(500, "Error interno");
            }
        }

        // GET: api/followers
        [HttpGet]
        public IActionResult GetFollowers()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var followers = _followersService.GetFollowers(Guid.Parse(userId));
                return Ok(followers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo los seguidores.");
                return StatusCode(500, "Error interno");
            }
        }

        // POST: api/followers
        [HttpPost]
        public async Task<IActionResult> Follow([FromBody] Follower follower)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                follower.UsuarioSeguidorId = Guid.Parse(userId);
                var response = await _followersService.Follow(follower);

                if (!response.Success) return BadRequest(response.Message);

                return Ok(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error siguiendo al usuario.");
                return StatusCode(500, "Error interno");
            }
        }

        // DELETE: api/followers/{followId}
        [HttpDelete("{followId}")]
        public async Task<IActionResult> UnFollow(Guid followId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var response = await _followersService.UnFollow(Guid.Parse(userId), followId);

                if (!response.Success) return BadRequest(response.Message);

                return Ok(response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dejando de seguir al usuario.");
                return StatusCode(500, "Error interno");
            }
        }
    }
}