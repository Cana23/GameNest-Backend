using System;
using System.ComponentModel.DataAnnotations;

namespace GameNest_Backend.Models
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Method { get; set; } = string.Empty;  // GET, POST, etc.

        [Required]
        public string Path { get; set; } = string.Empty;    // URL de la petición

        public string? QueryString { get; set; }           // Parámetros de consulta

        public string? RequestData { get; set; }           // Cuerpo de la petición

        public string? ResponseData { get; set; }          // Respuesta del servidor

        public int StatusCode { get; set; }                // Código de respuesta HTTP

        public string? Headers { get; set; }               // Headers de la petición

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // Fecha del log
    }
}
