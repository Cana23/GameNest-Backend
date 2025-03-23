using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameNest_Backend.Controllers;

namespace GameNest_Backend.Service.Services
{
    public class FollowersService : IFollowersService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public FollowersService(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int GetFollowerCount(Guid userId)
        {
            int FollowerCount = 0;

            try
            {
                FollowerCount = _context.Followers.Where(c => c.UsuarioSeguidoId == userId && c.IsDeleted == false).Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los seguidores del usuario {userId}.");
            }

            return FollowerCount;
        }

        public List<Follower> GetFollowers(Guid userId)
        {
            List<Follower> followers = new();

            try
            {
                followers = _context.Followers.Where(c => c.UsuarioSeguidoId == userId && c.IsDeleted == false).ToList() ?? throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los seguidores del usuario {userId}.");
            }

            return followers;
        }

        public async Task<ResponseHelper> Follow(Follower follower)
        {
            ResponseHelper response = new();

            try
            {
                var alreadyFollowed = _context.Followers.Any(c => c.UsuarioSeguidoId == follower.UsuarioSeguidoId && c.UsuarioSeguidorId == follower.UsuarioSeguidorId && c.IsDeleted == false);
                var userExists = _context.Users.FirstOrDefault(u => u.Id == follower.UsuarioSeguidoId);

                if (userExists == null)
                {
                    response.Success = false;
                    response.Message = "Usuario inexistente.";
                    return response;
                }

                if (alreadyFollowed)
                {
                    response.Success = true;
                    response.Message = "Ya se ha seguido a este usuario.";
                    return response;
                }

                _context.Followers.Add(follower);
                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Siguiendo al usuario.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al seguir al usuario {follower.UsuarioSeguidoId}.");
                response.Message = "Ocurrió un error al seguir al usuario. Inténtelo más tarde.";
            }

            return response;
        }

        public async Task<ResponseHelper> UnFollow(Guid followerId, Guid followId)
        {
            ResponseHelper response = new();

            try
            {
                var follow = _context.Followers.FirstOrDefault(f => f.UsuarioSeguidorId == followerId && f.UsuarioSeguidoId == followId && f.IsDeleted == false) ?? throw new Exception();
                follow.IsDeleted = true;

                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Se ha dejado de seguir correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al dejar de seguir al usuario {followerId}.");
                response.Message = "Ocurrió un error al dejar de seguir al usuario. Inténtelo más tarde.";
            }
            return response;
        }
    }
}