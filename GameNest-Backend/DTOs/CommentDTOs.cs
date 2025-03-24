using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.DTOs
{
    public class CommentCreateDTO
    {
        [Required]
        public int PublicacionId { get; set; } // No necesitas UsuarioId aquí

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Contenido { get; set; }
    }

    public class CommentUpdateDTO
    {
        [StringLength(100, MinimumLength = 1)]
        public string Contenido { get; set; } // Se puede quitar 'required' si no es obligatorio
    }

    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaComentario { get; set; }
    }
}
