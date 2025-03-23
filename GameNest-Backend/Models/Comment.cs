using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }

        // Clave foránea para Publication (int)
        public int PublicacionId { get; set; }

        // Clave foránea para User (Guid)
        [ForeignKey("Usuario")]
        public Guid UsuarioId { get; set; } 

        public string Contenido { get; set; }
        public DateTime FechaComentario { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Propiedades de navegación
        public virtual User Usuario { get; set; }
        public virtual Publication Publicacion { get; set; } 
    }
}