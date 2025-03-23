using System;
using System.Collections.Generic;

namespace GameNest_Backend.Models
{
    public class Publication
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublicationDate { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } // Nueva propiedad

        public User User { get; set; }

        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}