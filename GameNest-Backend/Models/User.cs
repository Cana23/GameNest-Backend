namespace GameNest_Backend.Models;
using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
{
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
