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
        public async Task<CommentResponseDTO> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Publicacion)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return null;

            return new CommentResponseDTO
            {
                Id = comment.Id,
                NombreUsuario = comment.Usuario.UserName,
                Contenido = comment.Contenido,
                FechaComentario = comment.FechaComentario
            };
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
    }
}
