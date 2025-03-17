using GameNest_Backend.DTOs;
using GameNest_Backend.Models;

namespace GameNest_Backend.Service.Services
{
    public interface ICommentsService
    {
        public List<Comment> GetPostComments(int id);
        public Task<Comment> GetComment(int id);
        public List<Comment> GetAllComments();
        public Task<ResponseHelper> CreateComment(Comment comment);
        public Task<ResponseHelper> UpdateComment(CommentUpdateDTO commentUpdate, Comment comment);
        public Task<ResponseHelper> DeleteComment(int id);
    }
}
