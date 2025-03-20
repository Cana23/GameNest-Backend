namespace GameNest_Backend.Models
{
    public class Publication
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
    }
}