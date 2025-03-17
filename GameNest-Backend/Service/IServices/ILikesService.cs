using GameNest_Backend.DTOs;
using GameNest_Backend.Models;

namespace GameNest_Backend.Service.Services
{
    public interface ILikesService
    {
        public int GetPostLikes(int id);
        public Task<Like> GetLike(int id);
        public Task<ResponseHelper> AddLike(Like like);
        public Task<ResponseHelper> RemoveLike(int id, Like like);
    }
}
