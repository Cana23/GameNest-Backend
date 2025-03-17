using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GameNest_Backend.DTOs

{
    public class FollowerDTO
    {
        public string NombreUsuario { get; set; }
        public string UsuarioSeguidoId { get; set; }
    }

}
