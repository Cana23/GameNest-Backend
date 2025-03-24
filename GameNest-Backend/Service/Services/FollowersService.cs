using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FollowersService : IFollowersService
{
    private readonly ApplicationDbContext _context;

    public FollowersService(ApplicationDbContext context)
    {
        _context = context;
    }

    public int GetFollowerCount(Guid userId)
    {
        return _context.Followers.Count(f => f.UsuarioSeguidoId == userId && !f.IsDeleted);
    }

    // Obtener los seguidores de un usuario
    public List<FollowerResponseDTO> GetFollowers(Guid userId)
    {
        return _context.Followers
            .Where(f => f.UsuarioSeguidoId == userId && !f.IsDeleted)
            .Select(f => new FollowerResponseDTO
            {
                FollowerId = f.UsuarioSeguidorId,
                FollowerUsername = f.UsuarioSeguidor.UserName, // Asegúrate de que UserName esté disponible
                FollowedAt = f.FechaSeguimiento
            }).ToList();
    }

    public async Task<ResponseHelper> Follow(FollowerCreateDTO dto)
    {
        var response = new ResponseHelper();

        // Verificar si ya está siguiendo
        var existingFollow = await _context.Followers
            .FirstOrDefaultAsync(f => f.UsuarioSeguidorId == dto.FollowerId && f.UsuarioSeguidoId == dto.FolloweeId && !f.IsDeleted);

        if (existingFollow != null)
        {
            response.Success = false;
            response.Message = "Ya sigues a este usuario.";
            return response;
        }

        var follow = new Follower
        {
            UsuarioSeguidorId = dto.FollowerId,
            UsuarioSeguidoId = dto.FolloweeId,
            FechaSeguimiento = DateTime.UtcNow
        };

        _context.Followers.Add(follow);
        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Seguiste exitosamente.";
        return response;
    }

    // Dejar de seguir a un usuario
    public async Task<ResponseHelper> UnFollow(Guid followerId, Guid followeeId)
    {
        var response = new ResponseHelper();

        var follow = await _context.Followers
            .FirstOrDefaultAsync(f => f.UsuarioSeguidorId == followerId && f.UsuarioSeguidoId == followeeId && !f.IsDeleted);

        if (follow == null)
        {
            response.Success = false;
            response.Message = "No estás siguiendo a este usuario.";
            return response;
        }

        // Marcar como eliminado en lugar de eliminar físicamente
        follow.IsDeleted = true;
        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Dejaste de seguir a este usuario.";
        return response;
    }

    // Buscar usuarios
    public List<UserSearchDTO> SearchUsers(string query)
    {
        return _context.Users
            .Where(u => u.UserName.Contains(query)) // Filtra por nombre de usuario
            .Select(u => new UserSearchDTO
            {
                UserId = u.Id,
                UserName = u.UserName
            }).ToList();
    }
}