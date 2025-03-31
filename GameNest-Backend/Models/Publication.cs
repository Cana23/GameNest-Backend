using System;
using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.Models
{
    public class Publication
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.UtcNow;
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public bool IsDeleted { get; set; } = false;
        public bool IsHidden { get; set; } = false;

        public DateTime? LastEditedDate { get; set; }

    }
}
