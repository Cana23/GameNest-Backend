using GameNest_Backend.Service.IServices;

namespace GameNest_Backend.Service.Services
{
    public class LogService : ILogService
    {
        private readonly IConfiguration _configuration;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string[]> GetLogsAsync()
        {
            var logBasePath = "C:\\Users\\ethan\\Documents\\GameNest\\logs\\";

            var today = DateTime.Now.ToString("yyyyMMdd");
            var logFileName = $"logs{today}.txt";
            var logPath = Path.Combine(logBasePath, logFileName);

            if (!File.Exists(logPath))
                throw new FileNotFoundException("Archivo de logs no encontrado.");

            var lines = await File.ReadAllLinesAsync(logPath);
            return lines;
        }
    }
}
