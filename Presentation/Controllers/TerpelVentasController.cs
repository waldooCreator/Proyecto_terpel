using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/terpel/ventas")]
    public class TerpelVentasController : ControllerBase
    {
        private readonly IProcessingService _processingService;
        private readonly ILogger<TerpelVentasController> _logger;

        public TerpelVentasController(IProcessingService processingService, ILogger<TerpelVentasController> logger)
        {
            _processingService = processingService;
            _logger = logger;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> ProcessSync([FromBody] ProcessingRequestDto request)
        {
            _logger.LogInformation("/api/terpel/ventas/sync called");
            var result = await _processingService.ProcessSyncAsync(request);
            return Ok(result);
        }

        [HttpPost("async")]
        public async Task<IActionResult> ProcessAsync([FromBody] ProcessingRequestDto request)
        {
            _logger.LogInformation("/api/terpel/ventas/async called");
            var txId = await _processingService.ProcessAsync(request);
            return Accepted(new { idTransaccion = txId });
        }
    }
}
