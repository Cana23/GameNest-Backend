using System;

namespace GameNest_Backend.Models
{
    public class RevokedToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
    }
}