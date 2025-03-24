using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameNest_Backend.Services
{
    public interface ICommentsService
    {
        Task<Comment> CreateCommentAsync(CommentCreateDTO dto, Guid userId);
        Task<CommentResponseDTO> GetCommentByIdAsync(int id);
        Task<IEnumerable<CommentResponseDTO>> GetCommentsForPublicationAsync(int publicacionId);
    }
}
