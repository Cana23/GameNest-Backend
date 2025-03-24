using System;
using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.DTOs
{
    public class FollowerCreateDTO
    {
        [Required]
        public Guid FollowerId { get; set; }  // ID del usuario que sigue

        [Required]
        public Guid FolloweeId { get; set; }  // ID del usuario al que se sigue
    }

    public class FollowerResponseDTO
    {
        public Guid FollowerId { get; set; }
        public string FollowerUsername { get; set; }
        public DateTime FollowedAt { get; set; }
    }

    public class UserSearchDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}