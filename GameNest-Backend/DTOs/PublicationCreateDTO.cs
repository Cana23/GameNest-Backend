using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.DTOs
{
    public class PublicationCreateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
    }
}