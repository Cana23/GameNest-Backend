using GameNest_Backend.DTOs;
using GameNest_Backend.Models;
using GameNest_Backend.Service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameNest_Backend.Services
{
    public class CommentService : ICommentsService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crear un comentario
        public async Task<Comment> CreateCommentAsync(CommentCreateDTO dto, Guid userId)
        {
            var comment = new Comment
            {
                PublicacionId = dto.PublicacionId,
                UsuarioId = userId,
                Contenido = dto.Contenido,
                FechaComentario = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        // Obtener un comentario por ID
        public async Task<Comment> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Publicacion)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            return comment ?? new Comment();
        }

        // Obtener comentarios por publicación
        public async Task<IEnumerable<CommentResponseDTO>> GetCommentsForPublicationAsync(int publicacionId)
        {
            var comments = await _context.Comments
                .Where(c => c.PublicacionId == publicacionId)
                .Include(c => c.Usuario)
                .Select(c => new CommentResponseDTO
                {
                    Id = c.Id,
                    NombreUsuario = c.Usuario.UserName,
                    Contenido = c.Contenido,
                    FechaComentario = c.FechaComentario
                })
                .ToListAsync();

            return comments;
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
                response.Message = "Ocurrió un error al eliminar el comentario. Inténtelo más tarde.";
            }
            return response;
        }
    }
}
