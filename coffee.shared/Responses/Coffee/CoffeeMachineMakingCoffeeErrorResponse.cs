using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class CoffeeMachineMakingCoffeeErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineMakingCoffeeErrorResponse"/> class.
        /// </summary>
        public CoffeeMachineMakingCoffeeErrorResponse() : base((int)HttpStatusCode.Conflict)
        {
            this.Message = $"The coffee machine is already making coffee, please wait.";
        }
    }
}
