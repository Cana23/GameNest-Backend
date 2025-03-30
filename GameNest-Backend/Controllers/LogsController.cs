using GameNest_Backend.Service.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GameNest_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var logs = await _logService.GetLogsAsync();
                return Ok(logs);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al leer los logs: {ex.Message}");
            }
        }
    }
}
