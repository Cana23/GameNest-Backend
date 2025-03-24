using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Like
    {
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public Guid UsuarioId { get; set; } 


        [ForeignKey("Publicacion")]
        public int PublicacionId { get; set; }

        public DateTime FechaLike { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public virtual User Usuario { get; set; }
        public virtual Publication Publicacion { get; set; }
    }
}