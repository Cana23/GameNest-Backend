using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}