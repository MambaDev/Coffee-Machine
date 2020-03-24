using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netwrix.coffee.api.Responses.Coffee;
using netwrix.coffee.api.Types;
using System.Collections.Generic;

namespace netwrix.coffee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CoffeeController> _logger;

        /// <summary>
        /// The coffee machine
        /// </summary>
        private readonly ICoffeeMachine _coffeeMachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CoffeeController(ILogger<CoffeeController> logger, ICoffeeMachine coffeeMachine)
        {
            this._logger = logger;
            this._coffeeMachine = coffeeMachine;
        }

        [HttpGet("status")]
        public IActionResult Get()
        {
            var response = new CoffeeMachineStatusResponse(this._coffeeMachine);
            return this.StatusCode(response.Status, response);
        }
    }
}
