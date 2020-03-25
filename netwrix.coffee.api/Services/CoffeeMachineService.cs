using Microsoft.Extensions.Logging;
using netwrix.coffee.api.Models;
using netwrix.coffee.shared.Requests.Coffee;
using netwrix.coffee.shared.Responses;
using netwrix.coffee.shared.Responses.Coffee;
using netwrix.coffee.shared.Types;
using System.Linq;
using System.Threading.Tasks;

namespace netwrix.coffee.api.Services
{
    public interface ICoffeeMachineService
    {
        /// <summary>
        /// Determines whether the coffee machine is currently alerting or not.
        /// </summary>
        bool IsCurrentlyAlerting();

        /// <summary>
        /// Gets the coffee machine status as a form of a request.
        /// </summary>
        /// <returns></returns>
        CoffeeMachineStatusResponse GetStatus();

        /// <summary>
        /// Turns the on coffee machine safe, ensuring that its in the correct state.
        /// </summary>
        /// <remarks>If the machine is not in a good state, the action will not take place.</remarks>
        Task<BaseResponse> TurnOnSafeAsync();

        /// <summary>
        /// Turns the off coffee machine safe, ensuring that its in the correct state.
        /// </summary>
        /// <remarks>If the machine is not in a good state, the action will not take place.</remarks>
        Task<BaseResponse> TurnOffSafeAsync();

        /// <summary>
        /// Makes the coffee safe by ensuring the correct state and context to start making coffee.
        /// </summary>
        /// <remarks>If the machine is not in a good state, the action will not take place.</remarks>
        BaseResponse MakeCoffeeSafe(MakeCoffeeRequest request);

        /// <summary>
        /// Descales the coffe machine safe by ensuring the correct state and context to start making coffee.
        /// </summary>
        /// <remarks>If the machine is not in a good state, the action will not take place.</remarks>
        BaseResponse DescaleCoffeeMachineSafe();
    }

    public class CoffeeMachineService : ICoffeeMachineService
    {
        /// <summary>
        /// The database context used to record usages.
        /// </summary>
        private readonly DatabaseContext _context;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ICoffeeMachineService> _logger;

        /// <summary>
        /// The coffee machine
        /// </summary>
        private readonly ICoffeeMachine _coffeeMachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="context">The context.</param>
        /// <param name="coffeeMachine">The coffee machine.</param>
        public CoffeeMachineService(ILogger<ICoffeeMachineService> logger, DatabaseContext context, ICoffeeMachine coffeeMachine)
        {
            this._logger = logger;
            this._context = context;
            this._coffeeMachine = coffeeMachine;
        }

        /// <summary>
        /// Determines whether the coffee machine is currently alerting or not.
        /// </summary>
        /// <inheritdoc/>
        public bool IsCurrentlyAlerting()
        {
            return new State[]
            {
                this._coffeeMachine.WaterLevelState,
                this._coffeeMachine.BeanFeedState,
                this._coffeeMachine.WasteCoffeeState,
                this._coffeeMachine.WaterTrayState,
                this._coffeeMachine.DescaleState
            }.Any(e => e == State.Alert);
        }

        /// <summary>
        /// Turns the off safety asynchronous.
        /// </summary>
        /// <inheritdoc/>
        public async Task<BaseResponse> TurnOffSafeAsync()
        {
            // if we are already off, you should not be able to attempt to turn it off again.
            if (!this._coffeeMachine.IsOn) return new CoffeeMachineOfflineErrorResponse("turn off");

            CoffeeMachineStatusResponse statusBeforeTurnOff = this.GetStatus();
            await this._coffeeMachine.TurnOffAsync().ConfigureAwait(false);

            return statusBeforeTurnOff;
        }

        /// <summary>
        /// Turns the on coffee machine safety, ensuring that its in the correct state.
        /// </summary>
        /// <inheritdoc/>
        public async Task<BaseResponse> TurnOnSafeAsync()
        {
            if (this._coffeeMachine.IsOn) return new CoffeeMachineOnlineErrorResponse("turn on");
            await this._coffeeMachine.TurnOnAsync().ConfigureAwait(false);

            return this.GetStatus();
        }

        /// <summary>
        /// Gets the coffee machine status as a form of a request.
        /// </summary>
        /// <inheritdoc/>
        public CoffeeMachineStatusResponse GetStatus()
        {
            return new CoffeeMachineStatusResponse(this._coffeeMachine);
        }

        /// <summary>
        /// Makes the coffee safe by ensuring the correct state and context to start making coffee.
        /// </summary>
        /// <inheritdoc/>
        public BaseResponse MakeCoffeeSafe(MakeCoffeeRequest makeCoffeeRequest)
        {
            // if we are off or already running, stop the execution
            if (!this._coffeeMachine.IsOn) return new CoffeeMachineOfflineErrorResponse("make coffee");
            if (this._coffeeMachine.IsMakingCoffee) return new CoffeeMachineMakingCoffeeErrorResponse();

            // if we are alerting, you cannot start making coffee, only turn off or descale.
            if (this.IsCurrentlyAlerting()) return new CoffeeMachineAlertingErrorResponse("make coffee");

            var coffeeOptions = new CoffeeCreationOptions
            {
                AddMilk = makeCoffeeRequest.AddMilk,
                NumEspressoShots = makeCoffeeRequest.NumberOfEspressoShots
            };

            Task.Run(async () => await this._coffeeMachine.MakeCoffeeAsync(coffeeOptions).ConfigureAwait(false));
            return new MakeCoffeeResponse(11);
        }

        /// <summary>
        /// Descales the coffe machine safe by ensuring the correct state and context to start making coffee.
        /// </summary>
        /// <inheritdoc/>
        public BaseResponse DescaleCoffeeMachineSafe()
        {
            // if we are off or already running, stop the execution
            if (!this._coffeeMachine.IsOn) return new CoffeeMachineOfflineErrorResponse("make coffee");
            if (this._coffeeMachine.IsMakingCoffee) return new CoffeeMachineMakingCoffeeErrorResponse();

            if (this._coffeeMachine.IsDescaling || this._coffeeMachine.DescaleState == State.Okay)
                return new CoffeeMachineDescalingErrorResponse("descaling");

            Task.Run(async () => await this._coffeeMachine.DescaleAsync().ConfigureAwait(false));
            return new DescalingCoffeeMachineResponse(31);
        }
    }
}
