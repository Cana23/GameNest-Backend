using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PublicacionId { get; set; }
        [ForeignKey("UsuarioId")]
        public string UsuarioId { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaComentario { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public virtual User Usuario { get; set; }
    }
}
