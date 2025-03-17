using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Like
    {
        public int Id { get; set; }

        [ForeignKey("UsuarioId")]
        public string UsuarioId { get; set; }
        //[ForeignKey("PublicacionId")]
        public int PublicacionId { get; set; }
        public DateTime FechaLike { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public virtual User Usuario { get; set; }
        //public virtual User Publicacion { get; set; }
    }
}

