using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netwrix.coffee.api.Services;
using netwrix.coffee.shared.Responses;
using netwrix.coffee.shared.Responses.Coffee;
using System.Threading.Tasks;

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
        private readonly ICoffeeMachineService _coffeeMachineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CoffeeController(ILogger<CoffeeController> logger, ICoffeeMachineService coffeeMachineService)
        {
            this._logger = logger;
            this._coffeeMachineService = coffeeMachineService;
        }

        /// <summary>
        /// Gets the coffee machine status, which includes all sub components in the device.
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult GetCoffeeMachineStatus()
        {
            this._logger.LogInformation("Gathering the current coffee machines status for requesting user.");
            CoffeeMachineStatusResponse response = this._coffeeMachineService.GetStatus();

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Turns the off coffee machine asynchronous.
        /// </summary>
        /// <response code="200">returns the given coffee machines state before it was turned off</response>
        /// <response code="400">the machine was already turned on while attempting to turn it on.</response>
        [HttpPost("status/offline")]
        public async Task<IActionResult> TurnOffCoffeeMachineAsync()
        {
            this._logger.LogInformation("Attempting to turn off the machine safely.");

            BaseResponse response = await this._coffeeMachineService
                    .TurnOffSafeAsync().ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Turns the on the coffee machine asynchronous.
        /// </summary>
        /// <response code="200">returns the given coffee machines state after turning on</response>
        /// <response code="400">the machine was already turned on while attempting to turn it on.</response>
        [HttpPost("status/online")]
        public async Task<IActionResult> TurnOnCoffeeMachineAsync()
        {
            this._logger.LogInformation("Attempting to turn on the machine safely.");

            BaseResponse response = await this._coffeeMachineService
                .TurnOnSafeAsync().ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }
    }
}
