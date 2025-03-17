using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Follower
    {
        public int Id { get; set; }

        [ForeignKey("UsuarioSeguidorId")]
        public string UsuarioSeguidorId { get; set; }

        [ForeignKey("UsuarioSeguidoId")]
        public string UsuarioSeguidoId { get; set; }

        public DateTime FechaSeguimiento { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public virtual User UsuarioSeguidor { get; set; }
        public virtual User UsuarioSeguido { get; set; }
    }
}

