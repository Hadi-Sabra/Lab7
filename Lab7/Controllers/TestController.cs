using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // Public endpoint - no authentication required
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new
            {
                Message = "✅ This is a public endpoint. No authentication required!"
            });
        }

        // Protected endpoint - requires authentication
        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok(new
            {
                Message = "🔒 This is a protected endpoint. You are authenticated!",
                User = User.Identity?.Name
            });
        }

        // Protected endpoint with role-based access
        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok(new
            {
                Message = "👑 Welcome Admin! You have access to the admin endpoint."
            });
        }
    }
}