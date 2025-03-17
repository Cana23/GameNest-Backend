using GameNest_Backend.Data;
using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace GameNest_Backend.Service.Services
{
    public class LikesService : ILikesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;
        public LikesService(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int GetPostLikes(int id)
        {
            int LikeCount = 0;

            try
            {
                LikeCount = _context.Likes.Where(c => c.PublicacionId == id && c.IsDeleted == false).Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los likes del post {id}.");
            }

            return LikeCount;
        }
        public async Task<Like> GetLike(int id)
        {
            Like like = new();

            try
            {
                like = await _context.Likes.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false) ?? throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los likes del post {id}.");
            }

            return like;
        }

        public async Task<ResponseHelper> AddLike(Like like)
        {
            ResponseHelper response = new();

            try
            {
                var alreadyLiked = _context.Likes.Where(c => c.UsuarioId == like.UsuarioId && c.PublicacionId == like.PublicacionId && c.IsDeleted == false).Any();

                if (alreadyLiked)
                {
                    response.Success = true;
                    response.Message = "Ya se ha dado like a esta publicación.";
                    return response;
                }

                _context.Likes.Add(like);
                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Like agregado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al agregar like {like}.");
                response.Message = "Ocurrió un error al darle like. Inténtelo más tarde.";
            }

            return response;
        }


        public async Task<ResponseHelper> RemoveLike(int id, Like like)
        {
            ResponseHelper response = new();

            try
            {
                like.IsDeleted = true;

                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Like borrado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al borrar el like {id}.");
                response.Message = "Ocurrió un error al borrar el like. Inténtelo más tarde.";
            }
            return response;
        }
    }
}
