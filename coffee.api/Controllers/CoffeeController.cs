using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using coffee.api.Services;
using coffee.shared.Models;
using coffee.shared.Requests.Coffee;
using coffee.shared.Responses;
using coffee.shared.Responses.Coffee;
using System.Threading.Tasks;

namespace coffee.api.Controllers
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
        /// The database context
        /// </summary>
        private readonly IAuditService _auditService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CoffeeController(ILogger<CoffeeController> logger, IAuditService auditService, ICoffeeMachineService coffeeMachineService)
        {
            this._logger = logger;
            this._coffeeMachineService = coffeeMachineService;
            this._auditService = auditService;
        }

        /// <summary>
        /// Gets the coffee machine status, which includes all sub components in the device.
        /// </summary>
        /// <response code="200">returns the given coffee machines state and component states.</response>
        [HttpGet("status")]
        public async Task<IActionResult> GetCoffeeMachineStatusAsync()
        {
            this._logger.LogInformation("Gathering the current coffee machines status for requesting user.");
            CoffeeMachineStatusResponse response = this._coffeeMachineService.GetStatus();

            await this._auditService.AddAuditEntryForRequest(this.Request, response,
                AuditActionType.GetMachineState).ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Turns the off coffee machine asynchronous.
        /// </summary>
        /// <response code="200">returns the given coffee machines state before it was turned off</response>
        /// <response code="409">the machine was already turned on while attempting to turn it on.</response>
        [HttpDelete("status/online")]
        public async Task<IActionResult> TurnOffCoffeeMachineAsync()
        {
            this._logger.LogInformation("Attempting to turn off the machine safely.");

            BaseResponse response = await this._coffeeMachineService
                    .TurnOffSafeAsync().ConfigureAwait(false);

            await this._auditService.AddAuditEntryForRequest(this.Request, response,
                AuditActionType.TurnOffMachine).ConfigureAwait(false);

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

            await this._auditService.AddAuditEntryForRequest(this.Request, response,
                AuditActionType.TurnOnMachine).ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Start the process of making the coffee for the user.
        /// </summary>
        /// <response code="201">Returns a simple okay when the making coffee has started.</response>
        /// <response code="409">The machine was already started making coffee.</response>
        /// <response code="409">The machine is off and cannot currently start making coffee or already making coffee.</response>
        /// <response code="409">The machine is in alert state and cannot perform any action.</response>
        [HttpPost("make")]
        public async Task<ActionResult> StartMakingCoffeeAsync([FromBody] MakeCoffeeRequest request)
        {
            this._logger.LogInformation("Attempting to start making coffee with the machine.");
            BaseResponse response = this._coffeeMachineService.MakeCoffeeSafe(request);

            await this._auditService.AddAuditEntryForRequest(this.Request, response,
                AuditActionType.MakeCoffee).ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }

        /// <summary>
        /// Start the process of descaling coffee for the user.
        /// </summary>
        /// <response code="200">Returns a simple okay when the scaling has started.</response>
        /// <response code="409">The machine was already started descaling or not ready for descaling.</response>
        /// <response code="409">The machine is off and cannot start descaling.</response>
        [HttpPost("descale")]
        public async Task<ActionResult> StartDescalingCoffeeMachineAsync()
        {
            this._logger.LogInformation("Attempting to start making coffee with the machine.");
            BaseResponse response = this._coffeeMachineService.DescaleCoffeeMachineSafe();

            await this._auditService.AddAuditEntryForRequest(this.Request, response,
                AuditActionType.DescaleMachine).ConfigureAwait(false);

            return this.StatusCode(response.Status, response);
        }
    }
}
