using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netwrix.coffee.api.Services;
using netwrix.coffee.shared.Requests.Coffee;
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
        /// <response code="200">returns the given coffee machines state and component states.</response>
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
        /// <response code="409">the machine was already turned on while attempting to turn it on.</response>
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
        /// <response code="409">the machine was already turned on while attempting to turn it on.</response>
        [HttpPost("status/online")]
        public async Task<IActionResult> TurnOnCoffeeMachineAsync()
        {
            this._logger.LogInformation("Attempting to turn on the machine safely.");

            BaseResponse response = await this._coffeeMachineService
                .TurnOnSafeAsync().ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Start the process of making the coffee for the user.
        /// </summary>
        /// <response code="201">Returns a simple okay when the making coffee has started.</response>
        /// <response code="409">The machine was already started making coffee.</response>
        /// <response code="409">The machine is off and cannot currently start making coffee.</response>
        /// <response code="409">The machine is in alert state and cannot perform any action.</response>
        /// <response code="409">The machine is already making coffee and thus cannot make two at the same time.</response>
        [HttpPost("make")]
        public IActionResult MakeCoffeeRequest([FromBody] MakeCoffeeRequest request)
        {
            this._logger.LogInformation("Attempting to start making coffee with the machine.");

            BaseResponse response = this._coffeeMachineService.MakeCoffeeSafe(request);
            return this.StatusCode(response.Status, response);
        }
    }
}
