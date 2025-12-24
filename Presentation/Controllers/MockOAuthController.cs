using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("oauth")]
    public class MockOAuthController : ControllerBase
    {
        // This is a local mock token endpoint used for testing the OAuth flow.
        // It accepts form-data with grant_type=client_credentials and client_id/client_secret
        [HttpPost("token")]
        public IActionResult Token([FromForm] string grant_type, [FromForm] string client_id, [FromForm] string client_secret)
        {
            if (grant_type != "client_credentials")
                return BadRequest(new { error = "unsupported_grant_type" });

            // Very simple credential check — these values will be documented in README
            if (client_id != "terpel_test_client" || client_secret != "s3cr3t_Terpel!2025")
            {
                return Unauthorized(new { error = "invalid_client" });
            }

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var obj = new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = 3600
            };
            return Ok(obj);
        }
    }
}
