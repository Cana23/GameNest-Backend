using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameNest_Backend.Models
{
    public class Follower
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UsuarioSeguidor")]
        public Guid UsuarioSeguidorId { get; set; }

        [ForeignKey("UsuarioSeguido")]
        public Guid UsuarioSeguidoId { get; set; }
        public DateTime FechaSeguimiento { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public virtual User UsuarioSeguidor { get; set; }
        public virtual User UsuarioSeguido { get; set; }
    }
}
