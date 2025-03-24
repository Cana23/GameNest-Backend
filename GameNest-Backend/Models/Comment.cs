using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int PublicacionId { get; set; }
        public virtual Publication Publicacion { get; set; }

        public Guid UsuarioId { get; set; }
        public virtual User Usuario { get; set; }

        public string Contenido { get; set; }
        public DateTime FechaComentario { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
