using System.ComponentModel.DataAnnotations;

public class PublicationDTO
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string ImageUrl { get; set; }
}