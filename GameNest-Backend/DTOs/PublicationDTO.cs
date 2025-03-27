using System.ComponentModel.DataAnnotations;

public class PublicationDTO
{

    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
    public string Title { get; set; }


    [Required(ErrorMessage = "El contenido es obligatorio.")]
    [MaxLength(5000, ErrorMessage = "El contenido no puede tener más de 5000 caracteres.")]
    public string Content { get; set; }


    [Url(ErrorMessage = "La URL de la imagen no es válida.")]
    public string? ImageUrl { get; set; }
}
