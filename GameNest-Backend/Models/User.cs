using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GameNest_Backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<Publication> Publications { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }

        public virtual ICollection<Follower> Siguiendo { get; set; } = new List<Follower>();

        public virtual ICollection<Follower> Seguidores { get; set; } = new List<Follower>();
    }
}