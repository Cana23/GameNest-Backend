using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GameNest_Backend.Models;

namespace GameNest_Backend.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            var request = context.Request;
            var response = context.Response;

            // Leer el cuerpo de la solicitud
            string requestBody = await ReadRequestBody(request);

            // Filtrar datos sensibles en el request
            requestBody = SanitizeRequestData(request.Path, requestBody);

            // Guardar la respuesta en un MemoryStream para poder leerla
            var originalBodyStream = response.Body;
            using var responseBody = new MemoryStream();
            response.Body = responseBody;

            await _next(context); // Pasar al siguiente middleware

            // Leer la respuesta
            string responseBodyContent = await ReadResponseBody(response);

            // Filtrar datos sensibles en la respuesta
            responseBodyContent = SanitizeResponseData(request.Path, responseBodyContent);

            // Guardar en BD
            var log = new LogEntry
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                RequestData = requestBody,
                ResponseData = responseBodyContent,
                StatusCode = response.StatusCode,
                Headers = SanitizeHeaders(request.Headers)
            };

            dbContext.Logs.Add(log);
            await dbContext.SaveChangesAsync();

            // Restaurar el cuerpo de la respuesta original
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            if (request.Body == null || !request.Body.CanRead)
                return string.Empty;

            request.EnableBuffering(); // Permite leer el body sin perderlo
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private string SanitizeRequestData(string path, string requestData)
        {
            if (path.Contains("/api/Auth/register") || path.Contains("/api/Auth/login"))
            {
                return "[SENSITIVE DATA HIDDEN]";
            }
            return requestData;
        }

        private string SanitizeResponseData(string path, string responseData)
        {
            if (path.Contains("/api/Auth/register") || path.Contains("/api/Auth/login"))
            {
                return "[SENSITIVE DATA HIDDEN]";
            }
            return responseData;
        }

        private string SanitizeHeaders(IHeaderDictionary headers)
        {
            var filteredHeaders = headers
                .Where(h => h.Key != "Authorization" && h.Key != "Cookie") // Ocultar datos sensibles
                .ToDictionary(h => h.Key, h => h.Value.ToString().Length > 500 ? "[TRUNCATED]" : h.Value.ToString()); // Truncar si es muy largo

            return JsonSerializer.Serialize(filteredHeaders, new JsonSerializerOptions { WriteIndented = false });
        }

    }
}
