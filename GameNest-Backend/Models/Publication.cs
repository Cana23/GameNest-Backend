using GameNest_Backend.Models;

public class Publication
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PublicationDate { get; set; }

    // Relación con User
    public Guid UserId { get; set; }
    public User User { get; set; }

    // Colección de Likes
    public ICollection<Like> Likes { get; set; } = new List<Like>(); // <-- Nueva propiedad

    public ICollection<Comment> Comments { get; set; } = new List<Comment>(); // <-- Nueva propiedad
}