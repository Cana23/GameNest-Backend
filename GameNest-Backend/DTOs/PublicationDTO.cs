using System.ComponentModel.DataAnnotations;

public class PublicationDTO
{

    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
    public string Title { get; set; }


    [Required(ErrorMessage = "El contenido es obligatorio.")]
    [MaxLength(5000, ErrorMessage = "El contenido no puede tener más de 5000 caracteres.")]
    public string Content { get; set; }

    public string? ImageUrl { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(ImageUrl))
        {
            var isValidUrl = Uri.TryCreate(ImageUrl, UriKind.Absolute, out var UriResult)
                             && (UriResult.Scheme == Uri.UriSchemeHttp || UriResult.Scheme == Uri.UriSchemeHttps);
            if (!isValidUrl)
            {
                yield return new ValidationResult("La URL de la imagen no es válida.", new[] { nameof(ImageUrl) });
            }
        }
    }
}
