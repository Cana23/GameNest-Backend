using System;
using GameNest_Backend.Service.IServices;
using Microsoft.AspNetCore.Mvc;
using GameNest_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace GameNest_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint para obtener todos los logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogs()
        {
            var logs = await _context.Logs.ToListAsync();
            return Ok(logs);
        }
    }
}
