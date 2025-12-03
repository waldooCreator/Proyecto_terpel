using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/terpel/callback")]
    public class CallbackController : ControllerBase
    {
        private readonly ILogger<CallbackController> _logger;

        public CallbackController(ILogger<CallbackController> logger)
        {
            _logger = logger;
        }

        [HttpPost("success")]
        public IActionResult Success([FromBody] object payload)
        {
            _logger.LogInformation("Callback success received: {Payload}", payload);
            return Ok();
        }

        [HttpPost("error")]
        public IActionResult Error([FromBody] object payload)
        {
            _logger.LogWarning("Callback error received: {Payload}", payload);
            return Ok();
        }
    }
}
