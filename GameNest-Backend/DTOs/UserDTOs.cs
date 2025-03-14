using System.ComponentModel.DataAnnotations;
namespace GameNest_Backend.DTOs

{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserUpdateDTO
    {
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }

    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

}
