
using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using GameNest_Backend.Controllers;

namespace GameNest_Backend.Service.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;
        public CommentsService(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Comment> GetPostComments(int id)
        {
            List<Comment> comments = new();

            try
            {
                comments = _context.Comments.Where(c => c.PublicacionId == id && c.IsDeleted == false).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los comentarios del post {id}.");
            }

            return comments;
        }
        public async Task<Comment> GetComment(int id)
        {
            Comment comment = new();

            try
            {
                comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false) ?? throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener los comentarios del post {id}.");
            }

            return comment;
        }

        public List<Comment> GetAllComments()
        {
            List<Comment> comments = new();
            try
            {
                comments = _context.Comments.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al obtener todos los comentarios.");
            }

            return comments;
        }

        public async Task<ResponseHelper> CreateComment(Comment comment)
        {
            ResponseHelper response = new();

            try
            {
                _context.Comments.Add(comment);
                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Comentario creado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al crear el comentario {comment}.");
                response.Message = "Ocurrió un error al crear el comentario. Inténtelo más tarde.";
            }

            return response;
        }

        public async Task<ResponseHelper> UpdateComment(CommentUpdateDTO commentUpdate, Comment comment)
        {
            ResponseHelper response = new();

            try
            {
                comment.Contenido = commentUpdate.Contenido;
                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Comentario editado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al editar el comentario {comment}.");
                response.Message = "Ocurrió un error al editar el comentario. Inténtelo más tarde.";
            }
            return response;
        }

        public async Task<ResponseHelper> DeleteComment(int id)
        {
            ResponseHelper response = new();

            try
            {
                var comment = _context.Comments.FirstOrDefault(c => c.Id == id && c.IsDeleted == false);

                if (comment == null)
                {
                    response.Message = "Comentario no encontrado o borrado.";
                    return response;
                }

                comment.IsDeleted = true;

                response.Success = await _context.SaveChangesAsync() > 0;
                response.Message = "Comentario borrado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error al eliminar el comentario {id}.");
                response.Message = "Ocurrió un error al eliminar el comentario. Inténtelo más tarde.";
            }
            return response;
        }
    }
}
