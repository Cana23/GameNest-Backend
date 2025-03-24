using System;

namespace GameNest_Backend.DTOs
{
    public class UserListDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}