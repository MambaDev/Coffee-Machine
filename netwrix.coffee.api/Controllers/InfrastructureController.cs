using Microsoft.AspNetCore.Mvc;
using netwrix.coffee.api.Responses.Infrastructure;

namespace netwrix.coffee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfrastructureController : ControllerBase
    {
        // GET: api/Infrastructure
        [HttpGet]
        public IActionResult Health()
        {
            var response = new HealthyResponse(true);
            return this.StatusCode(response.Status, response);
        }
    }
}
